using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
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
        private Collection<CswViewBuilderProp> _PropsInGrid = null;
        public enum GridReturnType
        {
            Xml,
            Json
        };

        public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View, CswNbtNodeKey ParentNodeKey = null )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;

            if( _View.ViewMode != NbtViewRenderingMode.Grid )
            {
                throw new CswDniException( ErrorType.Error, "Cannot create a grid using a view type of " + _View.ViewMode, "Cannot create a grid view if the view is not a grid." );
            }

            _ParentNodeKey = ParentNodeKey;

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

                if( Relationship.SecondType == RelatedIdType.ObjectClassId &&
                    Relationship.SecondId != Int32.MinValue )
                {
                    CswNbtMetaDataObjectClass SecondOc = _CswNbtResources.MetaData.getObjectClass( Relationship.SecondId );
                    foreach( CswNbtMetaDataNodeType NT in SecondOc.getNodeTypes() )
                    {
                        FirstLevelNodeTypes.Add( NT );
                    }
                }
                else if( Relationship.SecondType == RelatedIdType.NodeTypeId &&
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
            _PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, _PropsInGrid );
        } //ctor

        public JObject runGrid( bool IncludeInQuickLaunch )
        {
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

            JArray GridOrderedColumnDisplayNames = _makeHiddenColumnNames();
            _AddIconColumnName( ref GridOrderedColumnDisplayNames );
            _CswGridData.getGridColumnNamesJson( GridOrderedColumnDisplayNames, _PropsInGrid );

            JArray GridColumnDefinitions = _CswGridData.getGridColumnDefinitionJson( _PropsInGrid );
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

            RetObj["jqGridOpt"] = _CswGridData.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, null );

            return RetObj;
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of all Grid Rows
        /// </summary>
        public JObject getAllGridRows( bool IsReport )
        {
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Int32 StartingNode = 0;
            Int32 EndingNode = Tree.getChildNodeCount();
            if( _View.Visibility == NbtViewVisibility.Property )
            {
                Tree.goToNthChild( 0 );
                EndingNode = Tree.getChildNodeCount();
            }
            return _getGridRows( Tree, 1, _CswGridData.PageSize, StartingNode, EndingNode, IsReport );
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of Grid Rows for a specific page
        /// </summary>
        public JObject getGridRowsByPage( Int32 PageNumber, Int32 PageSize, bool IsReport )
        {
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
            Int32 StartingNode = PageSize * PageNumber;
            Int32 EndingNode = PageSize * ( PageNumber + 1 );
            return _getGridRows( Tree, PageNumber, PageSize, StartingNode, EndingNode, IsReport );
        } // getGridOuterJson()

        private JObject _getGridRows( ICswNbtTree Tree, Int32 PageNumber, Int32 PageSize, Int32 StartingNode, Int32 EndingNode, bool IsReport )
        {
            JObject RetObj = new JObject();
            JArray GridRows = new JArray();

            Int32 NodeCount = Tree.getChildNodeCount();
            if( NodeCount > 0 )
            {
                for( Int32 C = StartingNode; ( C < EndingNode || IsReport ) && C < NodeCount; C += 1 )
                {
                    Tree.goToNthChild( C );

                    GridRows.Add( _getGridRow( Tree, _PropsInGrid ) );

                    Tree.goToParentNode();
                }
            }

            Int32 PageCount;
            if( IsReport )
            {
                PageCount = 1;
            }
            else
            {
                PageCount = ( ( NodeCount + PageSize - 1 ) / PageSize );
            }
            RetObj["total"] = PageCount;
            RetObj["page"] = PageNumber + 1;
            RetObj["records"] = NodeCount;
            RetObj["rows"] = GridRows;
            return RetObj;
        }

        /// <summary>
        /// Adds required columns for edit/add/delete functions
        /// </summary>
        private JArray _makeHiddenColumnNames()
        {
            JArray Ret = new JArray();
            Ret.Add( "jqgridid" ); //better to use int for jqGrid key
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
                                new JProperty( "name", "jqgridid" ),
                                new JProperty( "index", "jqgridid" ),
                                new JProperty( "key", true ),
                                new JProperty( "hidden", true )
                                ) );

        } // _AddHiddenColumnDefiniton()

        private void _AddIconColumnDefinition( ref JArray ColumnDefArray )
        {
            ColumnDefArray.AddFirst( new JObject(
                                new JProperty( "name", "Icon" ),
                                new JProperty( "index", "Icon" ),
                                new JProperty( "formatter", "image" ),
                                new JProperty( CswGridData.JqGridJsonOptions.width.ToString(), "40" )
                                ) );
        }
        private void _AddIconColumnName( ref JArray ColumnNameArray )
        {
            ColumnNameArray.Add( "Icon" );
        }

        private JObject _getGridRow( ICswNbtTree Tree, Collection<CswViewBuilderProp> PropsInGrid )
        {
            JObject ThisNodeObj = new JObject();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            string ThisNodeIcon = ThisNodeType.IconFileName;
            string ThisNodeKeyString = ThisNodeKey.ToString();
            string ThisNodeId = ThisNodeKey.NodeId.PrimaryKey.ToString();
            bool ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();

            ThisNodeObj["jqgridid"] = ThisNodeId;
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
            ThisNodeObj["Icon"] = Icon;

            foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
            {
                _addSafeCellContent( _CswNbtResources, Prop, ThisNodeObj, PropsInGrid );
            }

            return ThisNodeObj;

        } // _treeNodeJObject()


        /// <summary>
        /// Translates property value into human readable text.
        /// Currently only handles Logical fieldtype.
        /// </summary>
        private static void _addSafeCellContent( CswNbtResources CswNbtResources, JObject DirtyElement, JObject ParentObj, Collection<CswViewBuilderProp> PropsInGrid )
        {
            if( null != DirtyElement )
            {
                string CleanPropName = DirtyElement["propname"].ToString().Trim().ToLower().Replace( " ", "_" );
                string DirtyValue = DirtyElement["gestalt"].ToString();
                string PropFieldTypeString = DirtyElement["fieldtype"].ToString();
                string PropId = DirtyElement["nodetypepropid"].ToString();
                CswNbtMetaDataNodeTypeProp Prop = CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );

                var PropFieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropFieldTypeString );
                string CleanValue;
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
    } // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
