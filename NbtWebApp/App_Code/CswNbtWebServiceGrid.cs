using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceGrid
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtView _View;
        private CswNbtNodeKey _ParentNodeKey;
        private CswGridData _CswGridData;
        private bool _CanEdit = true;
        private bool _CanDelete = true;
        private wsTreeOfView _WsTreeOfView;
        private readonly string _IdPrefix;

        public enum GridReturnType
        {
            Xml,
            Json
        };

        public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View, CswNbtNodeKey ParentNodeKey = null, string IdPrefix = "grid_" )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;

            if( _View.ViewMode != NbtViewRenderingMode.Grid )
            {
                throw new CswDniException( ErrorType.Error, "Cannot create a grid using a view type of " + _View.ViewMode, "Cannot create a grid view if the view is not a grid." );
            }

            _ParentNodeKey = ParentNodeKey;
            _IdPrefix = IdPrefix;
            _WsTreeOfView = new wsTreeOfView( _CswNbtResources, _View, _IdPrefix );

            Collection<CswNbtViewRelationship> FirstLevelRelationships = new Collection<CswNbtViewRelationship>();
            if( null != _ParentNodeKey && _View.Visibility == NbtViewVisibility.Property )
            {
                foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships.SelectMany( NodeRelationship => NodeRelationship.ChildRelationships ) )
                {
                    FirstLevelRelationships.Add( Relationship );
                }
            }
            else
            {
                FirstLevelRelationships = _View.Root.ChildRelationships;
            }
            // Case 21778
            // Maybe do this in Permit someday; however, the meaning of Edit and Delete is very specific in this context:
            // only evaluating visibility of the option to edit or delete root nodetypes of a view
            foreach( CswNbtViewRelationship Relationship in FirstLevelRelationships )
            {
                Collection<CswNbtMetaDataNodeType> FirstLevelNodeTypes = new Collection<CswNbtMetaDataNodeType>();

                if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId &&
                    Relationship.SecondId != Int32.MinValue )
                {
                    CswNbtMetaDataObjectClass SecondOc = _CswNbtResources.MetaData.getObjectClass( Relationship.SecondId );
                    foreach( CswNbtMetaDataNodeType NT in SecondOc.NodeTypes )
                    {
                        FirstLevelNodeTypes.Add( NT );
                    }
                }
                else if( Relationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId &&
                         Relationship.SecondId != Int32.MinValue )
                {
                    FirstLevelNodeTypes.Add( _CswNbtResources.MetaData.getNodeType( Relationship.SecondId ) );
                }

                foreach( CswNbtMetaDataNodeType NodeType in FirstLevelNodeTypes )
                {
                    _CanEdit = ( _CanEdit &&
                                 _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeType ) );
                    _CanDelete = ( _CanDelete &&
                                   _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, NodeType ) );
                    //exit if we already know both are false
                    if( !_CanEdit && !_CanDelete ) break;
                }

            }

            _CswGridData = new CswGridData( _CswNbtResources );

        } //ctor

        public JObject runGrid( bool IncludeInQuickLaunch )
        {
            _WsTreeOfView.deleteTreeFromCache();

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );

            Tree.goToRoot();

            _WsTreeOfView.saveTreeToCache( Tree );
            _View.SaveToCache( IncludeInQuickLaunch );

            return _getGridOuterJson();
        } // getGrid()

        private void _getGridProperties( Collection<CswNbtViewRelationship> ChildRelationships, Collection<CswViewBuilderProp> Ret )
        {
            CswCommaDelimitedString ColumnNames = new CswCommaDelimitedString();
            Collection<CswNbtViewProperty> PropsAtThisLevel = new Collection<CswNbtViewProperty>();
            Collection<CswNbtViewRelationship> NextChildRelationships = new Collection<CswNbtViewRelationship>();

            //Iterate all Relationships at this level first. This ensures our properties are properly collected.
            foreach( CswNbtViewRelationship Relationship in ChildRelationships )
            {
                foreach( CswNbtViewProperty Property in Relationship.Properties )
                {
                    PropsAtThisLevel.Add( Property );
                }
                //This will make recursion smoother: we're always iterating the collection of relationships at the same distance from root.
                foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                {
                    NextChildRelationships.Add( ChildRelationship );
                }
            }
            //Now iterate props
            foreach( CswNbtViewProperty Prop in PropsAtThisLevel )
            {
                CswViewBuilderProp VbProp = new CswViewBuilderProp( Prop );
                string PropName = Prop.Name.Trim().Replace( " ", "_" ).ToLower();
                if( false == ColumnNames.Contains( PropName ) )
                {
                    ColumnNames.Add( PropName );
                    Ret.Add( VbProp );
                }
                else
                {
                    //The Tree XML won't give us anything to map a "duplicate" prop/column back to the column definition. Leave our own breadcrumbs.
                    foreach( CswViewBuilderProp RetProp in Ret )
                    {
                        if( RetProp.PropNameUnique == PropName && false == RetProp.AssociatedPropIds.Contains( VbProp.MetaDataPropId.ToString() ) )
                        {
                            RetProp.AssociatedPropIds.Add( VbProp.MetaDataPropId.ToString() );
                        }
                    }
                }
            }

            //Now recurse, damn you.
            if( NextChildRelationships.Count > 0 )
            {
                _getGridProperties( NextChildRelationships, Ret );
            }
        }

        /// <summary>
        /// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        /// </summary>
        private JObject _getGridOuterJson()
        {
            JObject RetObj = new JObject();
            RetObj["nodetypeid"] = _View.ViewMetaDataTypeId;

            Collection<CswViewBuilderProp> PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, PropsInGrid );

            JArray GridRows = new JArray();

            JArray GridOrderedColumnDisplayNames = _makeHiddenColumnNames();
            _AddIconColumnName( ref GridOrderedColumnDisplayNames );
            _CswGridData.getGridColumnNamesJson( GridOrderedColumnDisplayNames, PropsInGrid );

            JArray GridColumnDefinitions = _CswGridData.getGridColumnDefinitionJson( PropsInGrid );
            _AddIconColumnDefinition( ref GridColumnDefinitions );
            _AddHiddenColumnDefiniton( GridColumnDefinitions );

            _CswGridData.GridWidth = ( _View.Width * 7 );
            if( _View.Visibility != NbtViewVisibility.Property )
            {
                _CswGridData.GridTitle = _View.ViewName;
            }
            _CswGridData.CanEdit = _CanEdit;
            _CswGridData.CanDelete = _CanDelete;

            // Sort
            CswNbtViewProperty SortProp = _View.getSortProperty();
            if( SortProp != null )
            {
                _CswGridData.GridSortName = SortProp.NodeTypeProp.PropName.ToUpperInvariant().Replace( " ", "_" );
            }
            else
            {
                _CswGridData.GridSortName = "nodename";
            }

            RetObj["jqGridOpt"] = _CswGridData.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, GridRows );

            return RetObj;
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        /// </summary>
        public JObject getGridPage( Int32 PageNumber, Int32 PageSize )
        {
            JObject RetObj = new JObject();

            string MoreNodeKey = String.Empty;

            Collection<CswViewBuilderProp> PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, PropsInGrid );

            JArray GridRows = _getGridTree( PageNumber, PropsInGrid );

            //RetObj["moreNodeKey"] = wsTools.ToSafeJavaScriptParam( MoreNodeKey );
            Int32 PageCount = ( ( GridRows.Count + _CswGridData.PageSize - 1 ) / _CswGridData.PageSize );
            RetObj["total"] = PageCount.ToString();
            RetObj["page"] = PageNumber + 1;
            RetObj["records"] = GridRows.Count.ToString();
            RetObj["rows"] = GridRows;
            return RetObj;
        } // getGridOuterJson()

        /// <summary>
        /// Adds required columns for edit/add/delete functions
        /// </summary>
        private JArray _makeHiddenColumnNames()
        {
            JArray Ret = new JArray();
            Ret.Add( "nodepk" );
            Ret.Add( "nodeid" ); //better to use int for jqGrid key
            Ret.Add( "cswnbtnodekey" ); //we'll want CswNbtNodeKey for add/edit/delete
            Ret.Add( "nodename" );
            return Ret;
        } // _makeHiddenColumnNames()

        /// <summary>
        /// Generates a JSON property with the definitional data for a jqGrid Column Array
        /// </summary>
        private void _AddHiddenColumnDefiniton( JArray ColumnDefArray )
        {
            //we'll want NodeName for edit/delete
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "nodename" ),
                                new JProperty( "index", "nodename" ),
                                new JProperty( "hidden", true )
                                ) );

            //we'll want CswNbtNodeKey for add/edit/delete
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "cswnbtnodekey" ),
                                new JProperty( "index", "cswnbtnodekey" ),
                                new JProperty( "hidden", true )
                                ) );

            //better to use int for jqGrid key
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "nodeid" ),
                                new JProperty( "index", "nodeid" ),
                                new JProperty( "key", true ),
                                new JProperty( "hidden", true )
                                ) );

            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "nodepk" ),
                                new JProperty( "index", "nodepk" ),
                                new JProperty( "hidden", true )
                                ) );

        } // _AddHiddenColumnDefiniton()

        private void _AddIconColumnDefinition( ref JArray ColumnDefArray )
        {
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "icon" ),
                                new JProperty( "index", "icon" ),
                                new JProperty( "formatter", "image" ),
                                new JProperty( CswGridData.JqGridJsonOptions.width.ToString(), "30" )
                                ) );
        }
        private void _AddIconColumnName( ref JArray ColumnNameArray )
        {
            ColumnNameArray.Add( "icon" );
        }

        /// <summary>
        /// Returns an XElement of the View's Tree
        /// </summary>
        private JArray _getGridTree( Int32 PageNo, Collection<CswViewBuilderProp> PropsInGrid )
        {
            JArray RetArray = new JArray();


            ICswNbtTree Tree = _WsTreeOfView.getTreeFromCache();

            Int32 PageSize = _CswGridData.PageSize;
            Int32 NodeCount = Tree.getChildNodeCount();
            if( NodeCount > 0 )
            {
                Tree.goToRoot();
                for( Int32 C = PageSize * PageNo; C < PageSize * ( PageNo + 1 ) && C < NodeCount; C += 1 )
                {
                    Tree.goToNthChild( C );

                    RetArray.Add( _getGridRow( Tree, PropsInGrid ) );

                    Tree.goToParentNode();
                }
            }

            return RetArray;
        } // _getGridTree()

        private JObject _getGridRow( ICswNbtTree Tree, Collection<CswViewBuilderProp> PropsInGrid )
        {
            JObject ThisNodeObj = new JObject();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeIcon = default( string );
            string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
            string ThisNodeId = default( string );
            string ThisNodePk = default( string );

            //string ThisNodeRel = default(string);
            bool ThisNodeLocked = false;
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            switch( ThisNodeKey.NodeSpecies )
            {
                case NodeSpecies.More:
                    ThisNodePk = ThisNodeKey.NodeId.PrimaryKey.ToString();
                    ThisNodeId = _IdPrefix + ThisNodeKey.NodeId.ToString();
                    ThisNodeName = NodeSpecies.More.ToString() + "...";
                    ThisNodeIcon = "triangle_blueS.gif";
                    //ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                    break;
                case NodeSpecies.Plain:
                    ThisNodePk = ThisNodeKey.NodeId.PrimaryKey.ToString();
                    ThisNodeId = _IdPrefix + ThisNodeKey.NodeId.ToString();
                    ThisNodeName = Tree.getNodeNameForCurrentPosition();
                    ThisNodeIcon = ThisNodeType.IconFileName;
                    //ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                    ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();
                    break;
                //case NodeSpecies.Group:
                //    ThisNodeRel = "group";
                //    break;
            }

            ThisNodeObj["nodeid"] = ThisNodeId;
            ThisNodeObj["nodepk"] = ThisNodePk;
            ThisNodeObj["cswnbtnodekey"] = ThisNodeKeyString;
            ThisNodeObj["nodename"] = ThisNodeName;
            string Icon = "<img src=\'";
            if( ThisNodeLocked )
            {
                Icon += "Images/quota/lock.gif\' title=\'Quota exceeded";
            }
            else
            {
                Icon += "Images/icons/" + ThisNodeIcon;
            }
            Icon += "\'/>";
            ThisNodeObj["icon"] = Icon;

            foreach( XElement Prop in Tree.getChildNodePropsOfNode() )
            {
                _addSafeCellContent( _CswNbtResources, Prop, ThisNodeObj, PropsInGrid );
            }

            return ThisNodeObj;

        } // _treeNodeJObject()


        /// <summary>
        /// Translates property value into human readable text.
        /// Currently only handles Logical fieldtype.
        /// </summary>
        private static void _addSafeCellContent( CswNbtResources CswNbtResources, XElement DirtyElement, JObject ParentObj, Collection<CswViewBuilderProp> PropsInGrid )
        {
            if( null != DirtyElement )
            {
                string CleanPropName = DirtyElement.Attribute( "name" ).Value.Trim().ToLower().Replace( " ", "_" );
                string CleanValue;
                string DirtyValue = DirtyElement.Attribute( "gestalt" ).Value;
                string PropFieldTypeString = DirtyElement.Attribute( "fieldtype" ).Value;
                string PropId = DirtyElement.Attribute( "nodetypepropid" ).Value;
                CswNbtMetaDataNodeTypeProp Prop = CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );

                var PropFieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropFieldTypeString );
                switch( PropFieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        CleanValue = CswConvert.ToDisplayString( CswConvert.ToTristate( DirtyValue ) );
                        break;
                    default:
                        CleanValue = DirtyValue;
                        break;
                }
                foreach( CswViewBuilderProp VbProp in PropsInGrid )
                {
                    if( Prop != null && VbProp.PropNameUnique == CleanPropName && VbProp.AssociatedPropIds.Contains( Prop.FirstPropVersionId ) )
                    {
                        CleanPropName += "_" + VbProp.MetaDataPropId;
                    }
                }

                ParentObj[CleanPropName] = CleanValue;
            }
        }

        #region Archived Valid Grid Json

        //        private static JObject getDebugGridJson()
        //        {
        //            String JsonString = @"{""viewname"": ""Debug View""
        //								,""viewwnodeidth"": ""150""
        //								,""columnnames"": [""nodeid"",""Equipment"",""Assembly""]
        //								,""columndefinition"": [{""name"": ""nodeid"", ""index"": ""nodeid"", ""key"":""true"", ""sortable"":""true"", ""sorttype"":""int""}
        //													  ,{""name"": ""Equipment"", ""index"": ""Equipment"", ""sortable"":""true"", ""search"":""true""}
        //												      ,{""name"": ""Assembly"", ""index"": ""Assembly"", ""sortable"":""true"", ""search"":""true""}]
        //								,""grid"": {""total"": ""1""
        //										   ,""page"": ""1""
        //										   ,""records"": ""2""
        //										   ,""rows"": [{""nodeid"":""0"", ""Equipment"":""big box"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""1"", ""Equipment"":""small box 1"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""2"", ""Equipment"":""small box 2"", ""Assembly"":""ancient collection of boxes""}
        //													  ,{""nodeid"":""3"", ""Equipment"":""small box 3"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""4"", ""Equipment"":""small box 4"", ""Assembly"":""dazzling collection of boxes""}
        //													  ,{""nodeid"":""5"", ""Equipment"":""small box 5"", ""Assembly"":""old collection of boxes""}
        //													  ,{""nodeid"":""6"", ""Equipment"":""small box 6"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""7"", ""Equipment"":""small box 7"", ""Assembly"":""dazzling collection of boxes""}
        //													  ,{""nodeid"":""8"", ""Equipment"":""small box 8"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""9"", ""Equipment"":""small box 9"", ""Assembly"":""old collection of boxes""}
        //													  ,{""nodeid"":""10"", ""Equipment"":""small box 10"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""11"", ""Equipment"":""small box 11"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""12"", ""Equipment"":""small box 12"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""13"", ""Equipment"":""small box 13"", ""Assembly"":""big collection of boxes""}
        //													  ,{""nodeid"":""14"", ""Equipment"":""small box 14"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""15"", ""Equipment"":""small box 15"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""16"", ""Equipment"":""small box 16"", ""Assembly"":""medium collection of boxes""}
        //													  ,{""nodeid"":""17"", ""Equipment"":""small box 17"", ""Assembly"":""collection of boxes""}
        //													  ,{""nodeid"":""18"", ""Equipment"":""small box 18"", ""Assembly"":""dazzling collection of boxes""}
        //													  ,{""nodeid"":""19"", ""Equipment"":""small box 19"", ""Assembly"":""dazzling collection of boxes""}
        //													  ,{""nodeid"":""20"", ""Equipment"":""small box 20"", ""Assembly"":""dazzling collection of boxes""}
        //													  ,{""nodeid"":""21"", ""Equipment"":""small box 21"", ""Assembly"":""new collection of boxes""}
        //													  ,{""nodeid"":""22"", ""Equipment"":""small box 22"", ""Assembly"":""new collection of boxes""}
        //													  ]
        //											}
        //								}";
        //            JObject DebugGrid = JObject.Parse( JsonString );
        //            return DebugGrid;
        //        }
        #endregion Archived Valid Grid Json
    } // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
