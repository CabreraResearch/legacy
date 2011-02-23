using System;
using System.Collections.Generic;
using ChemSW.Core;
using System.Web.UI.WebControls;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using FarPoint.Web.Spread;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceGrid
	{
		private CswNbtResources _CswNbtResources;
		private CswNbtView _View;
		
		/// <summary>
		/// rows
		/// </summary>
		private const string GridRows = "rows";
		/// <summary>
		/// page
		/// </summary>
		private const string GridPage = "page";
		/// <summary>
		/// total
		/// </summary>
		private const string GridTotal = "total";
		/// <summary>
		/// records
		/// </summary>
		private const string GridRecords = "records";
		/// <summary>
		/// NbtNode
		/// </summary>
		private const string GridNbtNode = "NbtNode";
		/// <summary>
		/// nodeid
		/// </summary>
		private const string GridNodeId = "nodeid";
		/// <summary>
		/// row
		/// </summary>
		private const string GridRow = "row";
		/// <summary>
		/// NbtNodeProp
		/// </summary>
		private const string GridNbtNodeProp = "NbtNodeProp";
		/// <summary>
		/// id
		/// </summary>
		private const string GridId = "nodeid";
		/// <summary>
		/// cell
		/// </summary>
		private const string GridCell = "cell";
		/// <summary>
		/// name
		/// </summary>
		private const string GridName = "name";
		/// <summary>
		/// gestalt
		/// </summary>
		private const string GridGestalt = "gestalt";
		/// <summary>
		/// fieldtype
		/// </summary>
		private const string GridFieldType = "fieldtype";
		/// <summary>
		/// propname
		/// </summary>
		private const string GridPropName = "propname";
		/// <summary>
		/// nodetypepropid
		/// </summary>
		private const string GridNodeTypePropId = "nodetypepropid";
		/// <summary>
		/// viewname
		/// </summary>
		private const string GridViewName = "viewname";
		/// <summary>
		/// viewwidth
		/// </summary>
		private const string GridViewWidth = "viewwidth";
		/// <summary>
		/// columnnames
		/// </summary>
		private const string GridColumnNames = "columnnames";
		/// <summary>
		/// columndefinition
		/// </summary>
		private const string GridColumnDefinition = "columndefinition";

		public enum GridReturnType
		{
			Xml,
			Json
		};

		public CswNbtWebServiceGrid( CswNbtResources CswNbtResources, CswNbtView View )
		{
			_CswNbtResources = CswNbtResources;
			_View = View;
		} //ctor

		public String getGrid( GridReturnType GridType )
		{
			String GridString = string.Empty;
			switch( GridType)
			{
				case GridReturnType.Xml:
					XDocument GridXDoc = getGridXDoc();
					if( null != GridXDoc )
					{
						GridString = GridXDoc.ToString();
					}
					break;
				case GridReturnType.Json:
					JObject GridJson = getGridOuterJson();
					if( null != GridJson )
					{
						GridString = GridJson.ToString();
					}
					else
					{
						GridString = getDebugGridJson().ToString(); // for debug only
					}
					break;
			}
			return GridString;
		} // getGrid()

		private JObject getDebugGridJson()
		{
			String JsonString = @"{""viewname"": ""Debug View""
								,""viewwnodeidth"": ""150""
								,""columnnames"": [""nodeid"",""Equipment"",""Assembly""]
								,""columndefinition"": [{""name"": ""nodeid"", ""index"": ""nodeid"", ""key"":""true"", ""sortable"":""true"", ""sorttype"":""int""}
													  ,{""name"": ""Equipment"", ""index"": ""Equipment"", ""sortable"":""true"", ""search"":""true""}
												      ,{""name"": ""Assembly"", ""index"": ""Assembly"", ""sortable"":""true"", ""search"":""true""}]
								,""grid"": {""total"": ""1""
										   ,""page"": ""1""
										   ,""records"": ""2""
										   ,""rows"": [{""nodeid"":""0"", ""Equipment"":""big box"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""1"", ""Equipment"":""small box 1"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""2"", ""Equipment"":""small box 2"", ""Assembly"":""ancient collection of boxes""}
													  ,{""nodeid"":""3"", ""Equipment"":""small box 3"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""4"", ""Equipment"":""small box 4"", ""Assembly"":""dazzling collection of boxes""}
													  ,{""nodeid"":""5"", ""Equipment"":""small box 5"", ""Assembly"":""old collection of boxes""}
													  ,{""nodeid"":""6"", ""Equipment"":""small box 6"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""7"", ""Equipment"":""small box 7"", ""Assembly"":""dazzling collection of boxes""}
													  ,{""nodeid"":""8"", ""Equipment"":""small box 8"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""9"", ""Equipment"":""small box 9"", ""Assembly"":""old collection of boxes""}
													  ,{""nodeid"":""10"", ""Equipment"":""small box 10"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""11"", ""Equipment"":""small box 11"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""12"", ""Equipment"":""small box 12"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""13"", ""Equipment"":""small box 13"", ""Assembly"":""big collection of boxes""}
													  ,{""nodeid"":""14"", ""Equipment"":""small box 14"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""15"", ""Equipment"":""small box 15"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""16"", ""Equipment"":""small box 16"", ""Assembly"":""medium collection of boxes""}
													  ,{""nodeid"":""17"", ""Equipment"":""small box 17"", ""Assembly"":""collection of boxes""}
													  ,{""nodeid"":""18"", ""Equipment"":""small box 18"", ""Assembly"":""dazzling collection of boxes""}
													  ,{""nodeid"":""19"", ""Equipment"":""small box 19"", ""Assembly"":""dazzling collection of boxes""}
													  ,{""nodeid"":""20"", ""Equipment"":""small box 20"", ""Assembly"":""dazzling collection of boxes""}
													  ,{""nodeid"":""21"", ""Equipment"":""small box 21"", ""Assembly"":""new collection of boxes""}
													  ,{""nodeid"":""22"", ""Equipment"":""small box 22"", ""Assembly"":""new collection of boxes""}
													  ]
											}
								}";
			JObject DebugGrid = JObject.Parse( JsonString );
			return DebugGrid;
		}

		/// <summary>
		/// Returns a JSON Object of Column Names, Definition and Rows representing a jqGrid-consumable JSON object
		/// </summary>
		private JObject getGridOuterJson()
		{
			JObject GridShellJObj = null;
			XDocument GridXDoc = getGridXDoc();

			if( null != GridXDoc && GridXDoc.Nodes().Count() > 0 )
			{
				IEnumerable<XElement> ColumnCollection = GridXDoc.Elements( GridRows ).Elements( GridRow ).First().Elements( GridCell );

				JProperty GridRowsJObj = getGridRowsJson();
				JProperty GridColumnNames = getGridColumnNamesJson( ColumnCollection );
				JProperty GrodColumnDef = getGridColumnDefinitionJson( ColumnCollection );
				string Width = Unit.Parse( ( CswConvert.ToInt32( _View.Width*7 ) ).ToString() + "px" ).ToString();

				GridShellJObj = new JObject(
					new JProperty( GridViewName, _View.ViewName ),
					new JProperty( GridViewWidth, Width ),
					GridColumnNames,
					GrodColumnDef,
					GridRowsJObj
					);
			} // if( null != GridXDoc && GridXDoc.Nodes().Count() > 0 )

			return GridShellJObj;
		} // getGridOuterJson()

		/// <summary>
		/// Generates a JSON property of an array of friendly Column Names
		/// </summary>
		private JProperty getGridColumnNamesJson(IEnumerable<XElement> ColumnCollection)
		{
			JArray ColumnArray = new JArray(
				               			from Column in ColumnCollection
				               			where Column.Attributes( GridPropName ).Count() == 1 
				               			select new JValue( Column.Attribute( GridPropName ).Value )
				               			);
			ColumnArray.AddFirst( new JValue( "nodeid" ) );

			JProperty ColumnNames = new JProperty( GridColumnNames, ColumnArray );
			return ColumnNames;
		} // getGridColumnNamesJson()

		/// <summary>
		/// Generates a JSON property with the definitional data for a jqGrid Column Array
		/// </summary>
		private JProperty getGridColumnDefinitionJson( IEnumerable<XElement> ColumnCollection )
		{
			
			JArray ColumnArray = new JArray( 
									from Column in ColumnCollection
				               		where Column.Attributes( GridPropName ).Count() == 1 && Column.Attributes( GridNodeTypePropId ).Count() == 1
				               		select new JObject( 
										new JProperty( GridName, Column.Attribute( GridPropName ).Value ),
										new JProperty( "index", Column.Attribute( GridPropName ).Value.ToLower().Replace( " ", "_" ) ),
										new JProperty( "sortable", "true"),
										new JProperty( "search", "true" ),
										new JProperty( "resizable", "true" ),
										new JProperty( "fieldtype", Column.Attribute( GridFieldType ).Value )
				               			) 
									);

			ColumnArray.AddFirst( new JObject(
								new JProperty( GridName, "nodeid" ),
			                   	new JProperty( "index", "nodeid" ),
								new JProperty( "key", "true" )
			                   	) );

			//var ColFieldType = CswNbtMetaDataFieldType.NbtFieldType.Unknown;
			//foreach( int NodeTypePropId in ColumnCollection.Select( XElement => CswConvert.ToInt32( XElement.Attribute( GridNodeTypePropId ).Value ) ) )
			//{
			      //fetch fieldtype and inject fieldtype specific logic into jqGrid Column Definition array
				  //dates, bools, etc
			      // set 'sorttype' to make columns sortable
			//}

			JProperty ColumnDefinition = new JProperty( GridColumnDefinition, ColumnArray );

			return ColumnDefinition;
		} // getGridColumnDefinitionJson()

		/// <summary>
		/// Returns an XElement of the View's Tree
		/// </summary>
		private XElement getGridTree()
		{
			XElement RawXml = null;
			ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, true, true, false, false );
			Int32 NodeCount = Tree.getChildNodeCount();
			if( NodeCount > 0 )
			{
				RawXml = XElement.Parse( Tree.getRawTreeXml() );
			}
			return RawXml;
		} // getGridColumnsJson()

		/// <summary>
		/// Transforms the Tree XML into an XDocument
		/// </summary>
		private XDocument getGridXDoc()
		{
			var RawXML = getGridTree();
			XDocument GridXDoc = null;
			if( null != RawXML )
			{
				GridXDoc = new XDocument(
					new XDeclaration( "1.0", "utf-8", "yes" ),
					new XComment( "Grid XML" ),
					new XElement( GridRows,
					              new XElement( GridPage ),
					              new XElement( GridTotal ),
					              new XElement( GridRecords ),
					              from c in RawXML.DescendantNodes().OfType<XElement>()
					              where c.Name == ( GridNbtNode ) && c.Attribute( GridNodeId ).Value != "0"
					              select new XElement( GridRow,
					                                   new XAttribute( GridId, c.Attribute( GridNodeId ).Value ),
					                                   from x in c.Elements()
					                                   where x.Name == ( GridNbtNodeProp )
					                                   select new XElement( GridCell,
					                                                        new XText( new XCData( x.Attribute( GridGestalt ).Value ) ),
					                                                        new XAttribute( GridPropName, x.Attribute( GridName ).Value ),
					                                                        new XAttribute( GridFieldType, x.Attribute( GridFieldType ).Value ),
					                                                        new XAttribute( GridNodeTypePropId, x.Attribute( GridNodeTypePropId ).Value )
					                                   	)
					              	)
						)
					);
			}
			return GridXDoc;
		} // getGridXDoc()

		/// <summary>
		/// Transforms the Tree XML into a JProperty
		/// </summary>
		private JProperty getGridRowsJson()
		{
			var RawXML = getGridTree();
			JProperty GridJObj = null;
			if( null != RawXML )
			{
				GridJObj = new JProperty("grid",
								new JObject(
									new JProperty( GridTotal, "1" ),
									new JProperty( GridPage, "1" ),
									new JProperty( GridRecords, "1" ),
									new JProperty( GridRows,
												   new JArray(
					               					from c in RawXML.DescendantNodes().OfType<XElement>()
					               					where c.Name == ( GridNbtNode ) && c.Attribute( GridNodeId ).Value != "0"
					               					select new JObject(
					               						new JProperty( GridId, c.Attribute( GridNodeId ).Value ),
					               						from x in c.Elements()
					               						where x.Name == ( GridNbtNodeProp )
					               						select new JProperty( x.Attribute( GridName ).Value, x.Attribute( GridGestalt ).Value )
					               						)
					               					)
												)
											)
										
									);

			}
			return GridJObj;
		} // getGridRowsJson()


	} // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
