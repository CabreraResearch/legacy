using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json;
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

		public JObject getGrid(bool ShowEmpty)
		{
            return _getGridOuterJson( ShowEmpty );
		} // getGrid()

		/// <summary>
		/// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
		/// </summary>
        private JObject _getGridOuterJson( bool ShowEmpty )
		{
			JObject GridShellJObj = null;
			IEnumerable<XElement> GridNodes = _getGridXElements();
            IEnumerable<CswNbtViewProperty> ColumnCollection = _View.getOrderedViewProps( true );

			JArray GridRows = new JArray();
		    var HasResults = ( !ShowEmpty && null != GridNodes && GridNodes.Count() > 0 );
			if( HasResults )
			{
                GridRows = _CswGridData.getGridRowsJSON( GridNodes ); //_getGridRowsJson( GridNodes );
			}

            JArray GridOrderedColumnDisplayNames = _CswGridData.getGridColumnNamesJson( ColumnCollection );   //_getGridColumnNamesJson( ColumnCollection );
		    _AddHiddenColumnNames( ref GridOrderedColumnDisplayNames );

            JArray GridColumnDefinitions = _CswGridData.getGridColumnDefinitionJson( ColumnCollection );
            _AddHiddenColumnDefiniton( ref GridColumnDefinitions );
            
			_CswGridData.GridWidth = ( _View.Width*7 ) ;
            if( _View.Visibility != NbtViewVisibility.Property )
            {
                _CswGridData.GridTitle = _View.ViewName;
            }
		    _CswGridData.CanEdit = _CanEdit;
		    _CswGridData.CanDelete = _CanDelete;

		    _CswGridData.GridSortName = "nodeid";

		    JObject JqGridOpt = _CswGridData.makeJqGridJSON( GridOrderedColumnDisplayNames, GridColumnDefinitions, GridRows );
            
			GridShellJObj = new JObject(
				new JProperty( "nodetypeid", _View.ViewMetaDataTypeId ),
				new JProperty( "jqGridOpt", JqGridOpt)
				);

			return GridShellJObj;
		} // getGridOuterJson()

		/// <summary>
		/// Adds required columns for edit/add/delete functions
		/// </summary>
        private void _AddHiddenColumnNames( ref JArray ColumnNameArray )
		{
            ColumnNameArray.AddFirst( new JValue( "nodename" ) ); //better to use int for jqGrid key
            ColumnNameArray.AddFirst( new JValue( "cswnbtnodekey" ) ); //we'll want CswNbtNodeKey for add/edit/delete
            ColumnNameArray.AddFirst( new JValue( "nodeid" ) ); //better to use int for jqGrid key

		} // _AddHiddenColumnNames()

		/// <summary>
		/// Generates a JSON property with the definitional data for a jqGrid Column Array
		/// </summary>
        private void _AddHiddenColumnDefiniton( ref JArray ColumnDefArray )
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

		} // _AddHiddenColumnDefiniton()

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
                Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, ref _ParentNodeKey, null, Int32.MinValue, true, false, null, false );
			}
			else
			{
                Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, true, false, false );    
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
