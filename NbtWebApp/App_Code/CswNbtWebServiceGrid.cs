using System;
using System.Collections.Generic;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceGrid
	{
		private readonly CswNbtResources _CswNbtResources;
		private readonly CswNbtView _View;
		private CswNbtNodeKey _ParentNodeKey;

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
		} //ctor

		public JObject getGrid()
		{
			return _getGridOuterJson(); 
		} // getGrid()

		/// <summary>
		/// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
		/// </summary>
		private JObject _getGridOuterJson()
		{
			JObject GridShellJObj = null;
			IEnumerable<XElement> GridNodes = _getGridXElements();
			IEnumerable<CswNbtViewProperty> ColumnCollection = _View.getOrderedViewProps();
			
			JProperty GridRows = null;
			if(GridNodes.Count() > 0 )
			{
				GridRows = _getGridRowsJson( GridNodes );
			}
			else
			{
				GridRows = new JProperty("grid");
			}

			JProperty GridOrderedColumnDisplayNames = _getGridColumnNamesJson( ColumnCollection );
			JProperty GridColumnDefinitions = _getGridColumnDefinitionJson( ColumnCollection );
			string Width =  ( _View.Width*7 ).ToString() ;

			GridShellJObj = new JObject(
				new JProperty( "viewname", _View.ViewName ),
				new JProperty( "viewwidth", Width ),
                new JProperty( "nodetypeid", _View.ViewNodeTypeId),
				GridOrderedColumnDisplayNames,
				GridColumnDefinitions,
				GridRows
				);

			return GridShellJObj;
		} // getGridOuterJson()

		/// <summary>
		/// Generates a JSON property of an array of friendly Column Names
		/// </summary>
		private static JProperty _getGridColumnNamesJson(IEnumerable<CswNbtViewProperty> PropCollection)
		{
			JArray ColumnArray = new JArray(
										from ViewProp in  PropCollection
										//where !string.IsNullOrEmpty(ViewProp.Name)  
										select new JValue( ViewProp.NodeTypeProp.PropName )
										);

            ColumnArray.AddFirst( new JValue( "nodename" ) ); //better to use int for jqGrid key
			ColumnArray.AddFirst( new JValue( "cswnbtnodekey" ) ); //we'll want CswNbtNodeKey for add/edit/delete
            ColumnArray.AddFirst( new JValue( "nodeid" ) ); //better to use int for jqGrid key
            var ColumnNames = new JProperty( "columnnames", ColumnArray );
			return ColumnNames;

		} // getGridColumnNamesJson()

		/// <summary>
		/// Generates a JSON property with the definitional data for a jqGrid Column Array
		/// </summary>
		private static JProperty _getGridColumnDefinitionJson( IEnumerable<CswNbtViewProperty> PropCollection )
		{

			JArray ColumnArray = new JArray(
										from JqGridProp in PropCollection 
										where JqGridProp != null
										select JqGridViewProperty.getJqGridAttributesForViewProp(JqGridProp)
										);

            //we'll want NodeName for edit/delete
            ColumnArray.AddFirst( new JObject(
                                new JProperty( "name", "nodename" ),
                                new JProperty( "index", "nodename" )
                                ) );
            
            //we'll want CswNbtNodeKey for add/edit/delete
            ColumnArray.AddFirst( new JObject(
                                new JProperty( "name", "cswnbtnodekey" ),
                                new JProperty( "index", "cswnbtnodekey" )
                                ) );
            
            //better to use int for jqGrid key
			ColumnArray.AddFirst( new JObject(
								new JProperty( "name", "nodeid" ),
								new JProperty( "index", "nodeid" ),
								new JProperty( "key", "true" )
								) );
            
			JProperty ColumnDefinition = new JProperty( "columndefinition", ColumnArray );

			return ColumnDefinition;
		} // getGridColumnDefinitionJson()

		/// <summary>
		/// Returns an XElement of the View's Tree
		/// </summary>
		private XElement _getGridTree()
		{
			XElement RawXml;

			if( _ParentNodeKey != null && _View.Root.ChildRelationships.Count > 0 )
			{
				// This is a Grid Property
				( _View.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( _ParentNodeKey.NodeId );
			}
			
			ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, true, false, false );
			Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, ref _ParentNodeKey, null, Int32.MinValue, true, false, null, false );
			Int32 NodeCount = Tree.getChildNodeCount();
			if( NodeCount > 0 )
			{
				RawXml = XElement.Parse( Tree.getRawTreeXml() );
			}
			else
			{
			    RawXml = new XElement( "root",
			                           new XElement( "item",
			                                new XAttribute( "id", "-1" ),
			                                new XAttribute( "rel", "root" ),
			                                new XElement( "content",
			                                    new XElement( "name", _View.ViewName ) ),
			                                new XElement( "item",
			                                    new XAttribute( "id", "-1" ),
			                                    new XAttribute( "rel", "child" ),
			                                    new XElement( "content",
			                                        new XElement( "name", "No Results" ) ) ) ) );
			}
		

			return RawXml;
		} // getGridColumnsJson()

		/// <summary>
		/// Transforms the Tree XML into an XDocument
		/// </summary>
		private IEnumerable<XElement> _getGridXElements()
		{
			var RawXml = _getGridTree();
            // case 21463: this collection should represent the XElements of distinct rows
			IEnumerable<XElement> NodesInGrid = ( from Element in RawXml.Elements("NbtNode").Elements("NbtNode") //root == <NbtTree />, 
                                                                                                                 //first child <NbtNode /> == View, 
                                                                                                                 //second child <NbtNode /> is first CswNbtNode
			                                      where Element.Attribute( "nodeid" ).Value != "0" && //has a valid nodeid
					                                    Element.DescendantNodesAndSelf().OfType<XElement>().Elements( "NbtNodeProp" ).Count() > 0 //has at least one property
			                                      select Element );
			return NodesInGrid;
		} // getGridXElements()



		/// <summary>
		/// Transforms the Tree XML into a JProperty
		/// </summary>
		private static JProperty _getGridRowsJson( IEnumerable<XElement> NodesInGrid )
		{
			JProperty GridJObj = null;

			GridJObj = new JProperty("grid",
							new JObject(
								new JProperty( "total", "1" ),
								new JProperty( "page", "1" ),
								new JProperty( "records", NodesInGrid.Elements().Count() ),
								new JProperty( "rows",
												new JArray(
												from Element in NodesInGrid
												select new JObject(
													new JProperty( "nodeid", Element.Attribute( "nodeid" ).Value ),
													new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam(Element.Attribute( "key" ).Value) ),
                                                    new JProperty( "nodename", Element.Attribute( "nodename" ).Value ),
                                                    from DirtyElement in Element.Elements()
													where DirtyElement.Name == ( "NbtNodeProp" )
													select _massageGridCell( DirtyElement ) 
													)
												)
											)
										)
								);
			
			return GridJObj;
		} // get"rows"Json()

		private static JProperty _massageGridCell(XElement DirtyElement)
		{
			string CleanPropName = DirtyElement.Attribute( "name" ).Value.ToLower().Replace( " ", "_" );
			string CleanValue = string.Empty;
			string DirtyValue = DirtyElement.Attribute( "gestalt" ).Value;
			string PropFieldTypeString = DirtyElement.Attribute( "fieldtype" ).Value;
			var PropFieldType = CswNbtMetaDataFieldType.getFieldTypeFromString( PropFieldTypeString );
			switch (PropFieldType)
			{
				case CswNbtMetaDataFieldType.NbtFieldType.Logical:
					CleanValue = CswConvert.ToDisplayString( CswConvert.ToTristate( DirtyValue ) );
					break;
				default:
					CleanValue = DirtyValue;
					break;
			}
			JProperty CleanProp = new JProperty( CleanPropName, CleanValue );
			return CleanProp;

		}

        #region Archive
        //public string getGrid( GridReturnType GridType )
        //{
        //    string GridString = string.Empty;
        //    switch( GridType)
        //    {
        //        case GridReturnType.Xml:
        //            XDocument GridXDoc = getGridXElements();
        //            if( null != GridXDoc )
        //            {
        //                GridString = GridXDoc.ToString();
        //            }
        //            break;
        //        case GridReturnType.Json:
        //            JObject GridJson = getGridOuterJson();
        //            if( null != GridJson )
        //            {
        //                GridString = GridJson.ToString();
        //            }
        //            //else
        //            //{
        //            //    GridString = getDebugGridJson().ToString(); // for debug only
        //            //}
        //            break;
        //    }
        //    return GridString;
        //} // getGrid()

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
        #endregion

	} // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
