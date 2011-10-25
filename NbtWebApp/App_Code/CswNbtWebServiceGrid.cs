﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
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

        public enum GridReturnType
        {
            Xml,
            Json
        };

        public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View, CswNbtNodeKey ParentNodeKey )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;
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

        public JObject getGrid( bool ShowEmpty = false, bool ForReporting = false )
        {
            return _getGridOuterJson( ShowEmpty, ForReporting );
        } // getGrid()

        private void _getGridProperties( Collection<CswNbtViewRelationship> ChildRelationships, ref Collection<CswViewBuilderProp> Ret )
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
                _getGridProperties( NextChildRelationships, ref Ret );
            }
        }

        /// <summary>
        /// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        /// </summary>
        private JObject _getGridOuterJson( bool ShowEmpty = false, bool ForReporting = false )
        {
            JObject RetObj = new JObject();
            RetObj["nodetypeid"] = _View.ViewMetaDataTypeId;


            //IEnumerable<CswNbtViewProperty> ColumnCollection = _View.getOrderedViewProps( false );

            Collection<CswViewBuilderProp> PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, ref PropsInGrid );

            JArray GridRows = null;
            if( ForReporting )
            {
                GridRows = new JArray();
                IEnumerable<XElement> GridNodes = _getGridXElements();
                var HasResults = ( false == ShowEmpty && null != GridNodes && GridNodes.Count() > 0 );
                if( HasResults )
                {
                    GridRows = _CswGridData.getGridRowsJSON( GridNodes, PropsInGrid ); //_getGridRowsJson( GridNodes );
                }
            }
            JArray GridOrderedColumnDisplayNames = _makeHiddenColumnNames();
			_AddIconColumnName( ref GridOrderedColumnDisplayNames );
            _CswGridData.getGridColumnNamesJson( GridOrderedColumnDisplayNames, PropsInGrid );   //_getGridColumnNamesJson( ColumnCollection );
            //_makeHiddenColumnNames( ref GridOrderedColumnDisplayNames );

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

            _CswGridData.GridSortName = "nodeid";

            RetObj["jqGridOpt"] = _CswGridData.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, GridRows );

            return RetObj;
        } // getGridOuterJson()

        /// <summary>
        /// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
        /// </summary>
        public JObject getGridRows( bool ShowEmpty )
        {
            JObject RetObj = new JObject();

            IEnumerable<XElement> GridNodes = _getGridXElements();

            Collection<CswViewBuilderProp> PropsInGrid = new Collection<CswViewBuilderProp>();
            _getGridProperties( _View.Root.ChildRelationships, ref PropsInGrid );

            JArray GridRows = new JArray();
            var HasResults = ( false == ShowEmpty && null != GridNodes && GridNodes.Count() > 0 );
            if( HasResults )
            {
                GridRows = _CswGridData.getGridRowsJSON( GridNodes, PropsInGrid ); //_getGridRowsJson( GridNodes );
            }


            RetObj["total"] = "1";
            RetObj["page"] = "1";
            RetObj["records"] = GridNodes.Count().ToString();
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
        private XElement _getGridTree()
        {
            XElement RawXml = null;
            ICswNbtTree Tree;
            if( _ParentNodeKey != null && _View.Visibility == NbtViewVisibility.Property ) // This is a Grid Property
            {
                ( _View.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Clear(); // case 21676. Clear() to avoid cache persistence.
                ( _View.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( _ParentNodeKey.NodeId );
                Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, ref _ParentNodeKey, null, 50, true, false, null, false );
            }
            else
            {
                Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, true, false, false, 50 );
            }
            Int32 NodeCount = Tree.getChildNodeCount();
            if( NodeCount > 0 )
            {
                RawXml = XElement.Parse( Tree.getRawTreeXml() );
            }
            //else jqGrid effectively handles 'else' with emptyrecords property

            return RawXml;
        } // getGridColumnsJson()

        /// <summary>
        /// Transforms the Tree XML into an XDocument
        /// </summary>
        private IEnumerable<XElement> _getGridXElements()
        {
            var RawXml = _getGridTree();
            IEnumerable<XElement> NodesInGrid = null;
            // case 21535: tree is not null
            if( null != RawXml )
            {
                // case 21463: this collection should represent the XElements of distinct rows
                // root == <NbtTree />, 
                // first child <NbtNode /> == View, 
                // second child <NbtNode /> is first CswNbtNode
                IEnumerable<XElement> GridRows = RawXml.Elements( "NbtNode" ).Elements( "NbtNode" );
                //case 21627
                if( _View.Visibility == NbtViewVisibility.Property )
                {
                    //Grid Properties have an additional level of depth
                    GridRows = GridRows.Elements( "NbtNode" );
                }

                NodesInGrid = ( from Element in GridRows
                                where Element.Attribute( "nodeid" ).Value != "0" && //has a valid nodeid
                                      Element.DescendantNodesAndSelf().OfType<XElement>().Elements( "NbtNodeProp" ).Count() > 0 //has at least one property
                                select Element );
            }
            return NodesInGrid;
        } // getGridXElements()

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
