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

		#region StringConstants for jqGrid
		
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

		#endregion



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
				new JProperty( GridViewName, _View.ViewName ),
				new JProperty( GridViewWidth, Width ),
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
            var ColumnNames = new JProperty( GridColumnNames, ColumnArray );
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
                                new JProperty( GridName, "nodename" ),
                                new JProperty( "index", "nodename" )
                                ) );
            
            //we'll want CswNbtNodeKey for add/edit/delete
            ColumnArray.AddFirst( new JObject(
                                new JProperty( GridName, "cswnbtnodekey" ),
                                new JProperty( "index", "cswnbtnodekey" )
                                ) );
            
            //better to use int for jqGrid key
			ColumnArray.AddFirst( new JObject(
								new JProperty( GridName, "nodeid" ),
								new JProperty( "index", "nodeid" ),
								new JProperty( "key", "true" )
								) );
            
			JProperty ColumnDefinition = new JProperty( GridColumnDefinition, ColumnArray );

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
			IEnumerable<XElement> NodesInGrid = ( from Element in RawXml.DescendantNodes().OfType<XElement>()
												  where Element.Name == ( "NbtNode" ) && //only concerned with "NbtNode" elements
														Element.Attribute( "nodeid" ).Value != "0" && //has a valid nodeid
														Element.Elements( "NbtNodeProp" ).Count() > 0 //has at least one property
												  select Element );
			return NodesInGrid;
			//XDocument GridXDoc = null;
			//if( null != RawXml )
			//{
			//    GridXDoc = new XDocument(
			//        new XDeclaration( "1.0", "utf-8", "yes" ),
			//        new XComment( "Grid XML" ),
			//        new XElement( GridRows,
			//                      new XElement( GridPage ),
			//                      new XElement( GridTotal ),
			//                      new XElement( GridRecords, NodesInGrid.Count() ),
			//                      from c in NodesInGrid
			//                          //RawXml.DescendantNodes().OfType<XElement>()
			//                      //where c.Name == ( GridNbtNode ) && c.Elements("NbtNode").Count() == 0 && c.Attribute( GridNodeId ).Value != "0"
			//                      select new XElement( GridRow,
			//                                           new XAttribute( GridId, c.Attribute( GridNodeId ).Value ),
			//                                           from x in c.Elements()
			//                                           where x.Name == ( GridNbtNodeProp )
			//                                           select new XElement( GridCell,
			//                                                                new XText( new XCData( x.Attribute( GridGestalt ).Value ) ),
			//                                                                new XAttribute( GridPropName, x.Attribute( GridName ).Value ),
			//                                                                new XAttribute( GridFieldType, x.Attribute( GridFieldType ).Value ),
			//                                                                new XAttribute( GridNodeTypePropId, x.Attribute( GridNodeTypePropId ).Value )
			//                                            )
			//                        )
			//            )
			//        );
			//}
			//return GridXDoc;
		} // getGridXElements()

		/// <summary>
		/// Transforms the Tree XML into a JProperty
		/// </summary>
		private static JProperty _getGridRowsJson( IEnumerable<XElement> NodesInGrid )
		{
			JProperty GridJObj = null;

			GridJObj = new JProperty("grid",
							new JObject(
								new JProperty( GridTotal, "1" ),
								new JProperty( GridPage, "1" ),
								new JProperty( GridRecords, NodesInGrid.Elements().Count() ),
								new JProperty( GridRows,
												new JArray(
												from Element in NodesInGrid
												select new JObject(
													new JProperty( GridId, Element.Attribute( GridNodeId ).Value ),
													new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam(Element.Attribute( "key" ).Value) ),
                                                    new JProperty( "nodename", Element.Attribute( "nodename" ).Value ),
                                                    from DirtyElement in Element.Elements()
													where DirtyElement.Name == ( GridNbtNodeProp )
													select _massageGridCell( DirtyElement ) 
													)
												)
											)
										)
								);
			
			return GridJObj;
		} // getGridRowsJson()

		private static JProperty _massageGridCell(XElement DirtyElement)
		{
			string CleanPropName = DirtyElement.Attribute( GridName ).Value.ToLower().Replace( " ", "_" );
			string CleanValue = string.Empty;
			string DirtyValue = DirtyElement.Attribute( GridGestalt ).Value;
			string PropFieldTypeString = DirtyElement.Attribute( GridFieldType ).Value;
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

		/// <summary>
		/// For the transformation of XElement Attribute types into valid JProperty and JObject types
		/// </summary>
		private class JqGridViewProperty
		{
			private readonly CswNbtViewProperty NbtViewProperty;
			private readonly CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtMetaDataFieldType.NbtFieldType.Unknown;
			private readonly CswNbtMetaDataNodeTypeProp NbtMetaDataNodeTypeProp;
			private readonly CswNbtMetaDataObjectClassProp NbtMetaDataObjectClassProp;
			private readonly bool _DoCssOverride = false; 
			public enum JqFieldType
			{
				date,
				integer,
				number,
				currency,
				text,
				function,
				email,
				link,
				showlink,
				checkbox,
				select,
				actions
			} ;

			public JqGridViewProperty( CswNbtViewProperty ViewProperty, bool DoCssOverride = false)
			{
				NbtViewProperty = ViewProperty;
				FieldType = NbtViewProperty.FieldType.FieldType;
				NbtMetaDataNodeTypeProp = ViewProperty.NodeTypeProp;
				NbtMetaDataObjectClassProp = ViewProperty.ObjectClassProp;
				_DoCssOverride = DoCssOverride;
			} //ctor

			/// <summary>
			/// Transforms an Nbt View Property into well-form JSON for consumption by a jqGrid
			/// </summary>
			public static JObject getJqGridAttributesForViewProp( CswNbtViewProperty ViewProperty )
			{
				var JqGridViewProp = new JqGridViewProperty( ViewProperty );
				var ReturnObj = new JObject();
				const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
				Type JType = JqGridViewProp.GetType();

				//foreach public/instanced property in the JqGridViewProperty class of type JProperty which is not null, 
				//add the property value to the return JObject
				foreach( JProperty ThisPropAttribute in ( from PropertyInfo in JType.GetProperties( Flags )
														  where PropertyInfo.PropertyType == typeof (JProperty)
														  select (JProperty) PropertyInfo.GetValue( JqGridViewProp, null ) )
															.Where( ThisPropAttribute => null != ThisPropAttribute ) )
				{
					if( null != ReturnObj.Property( ThisPropAttribute.Name ) )
					{
						throw new CswDniException( "Error attempting to add duplicate property to collection", "Property: " + ThisPropAttribute + " already exists in the JObject: " + ReturnObj.ToString() );
					}
					ReturnObj.Add( ThisPropAttribute );
				}
				
				return ReturnObj;
			}

			#region jqGrid Attributes
			
			/// <summary>
			/// Defines the alignment of the cell in the Body layer, not in header cell. Possible values: left, center, right.
			/// </summary>
			public JProperty Align
			{
				get
				{
					JProperty ReturnProp = null;
					if( _DoCssOverride )
					{
						ReturnProp = new JProperty( "align", "left" );
					}
					return ReturnProp;
				}
			}
			
			/// <summary>
			/// This option allows adding classes to the column. 
			/// If more than one class will be used a space should be set. 
			/// By example classes:'class1 class2' will set a class1 and class2 to every cell on that column. 
			/// In the grid css there is a predefined class ui-ellipsis which allow to attach ellipsis to a particular row.
			/// </summary>
			public JProperty Classes
			{
				get
				{
					JProperty ReturnProp = null;
					if( _DoCssOverride )
					{
						ReturnProp = new JProperty( "classes", string.Empty );
					}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Governs format of sorttype:
			///		date (when datetype is set to local) and editrules {date:true} fields. 
			/// Determines the expected date format for that column. Uses a PHP-like date formatting. 
			/// Currently ”/”, ”-”, and ”.” are supported as date separators. 
			/// Valid formats are: 
			///		y,Y,yyyy for four digits year 
			///		YY, yy for two digits year 
			///		m,mm for months 
			///		d,dd for days. 
			/// </summary>
			public JProperty DateFmt
			{
				get
				{
					JProperty ReturnProp = null;
					if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Date )
					{
						ReturnProp = new JProperty( "datefmt", "mm/dd/yyyy" );
					}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines if the field is editable. 
			/// This option is used in cell, inline and form modules.
			/// See http://www.trirand.com/jqgridwiki/doku.php?id=wiki:common_rules#editable
			/// </summary>
			public JProperty Editable
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no plans for inline grid editing yet
					return ReturnProp;
				}
			}
			/// <summary>
			/// Array of allowed options (attributes) for edittype option editing
			/// </summary>
			public JProperty EditOptions
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no plans for inline grid editing yet
					return ReturnProp;
				}
			}

			/// <summary>
			/// sets additional rules for the editable field editing
			/// </summary>
			public JProperty EditRules
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no plans for inline grid editing yet
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines the edit type for inline and form editing Possible values: text, textarea, select, checkbox, password, button, image and file.
			/// </summary>
			public JProperty EditType
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no plans for inline grid editing yet
					return ReturnProp;
				}
			}
			
			/// <summary>
			/// If set to asc or desc, the column will be sorted in that direction on first sort.Subsequent sorts of the column will toggle as usual
			/// </summary>
			public JProperty FirstSortOrder
			{
				get
				{
					JProperty ReturnProp = null;
					if( NbtViewProperty.SortBy )
					{
						switch( NbtViewProperty.SortMethod )
						{
							case CswNbtViewProperty.PropertySortMethod.Ascending:
								ReturnProp = new JProperty("firstsortorder","asc");
								break;
							case CswNbtViewProperty.PropertySortMethod.Descending:
								ReturnProp = new JProperty("firstsortorder","desc");
								break;
							default:
								ReturnProp = new JProperty("firstsortorder","asc");
								break;

						}
					}
					return ReturnProp;
				}
			}

			/// <summary>
			/// If set to true this option does not allow recalculation of the width of the column if shrinkToFit option is set to true. 
			/// Also the width does not change if a setGridWidth method is used to change the grid width.
			/// </summary>
			public JProperty Fixed
			{
				get
				{
					JProperty ReturnProp = null;
					if( _DoCssOverride )
					{
						ReturnProp = new JProperty( "fixed", false );
					}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines various options for form editing. 
			/// </summary>
			public JProperty FormOptions
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no plans for inline grid editing yet
					return ReturnProp;
				}
			}

			/// <summary>
			/// The predefined types (string) or custom function name that controls the format of this field.
			/// Additional configuration can be applied by supplying additional formatoptions array.
			/// See http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
			/// </summary>
			public JProperty Formatter
			{
				get
				{
					JProperty ReturnProp = null;
					switch ( FieldType )
					{
						case CswNbtMetaDataFieldType.NbtFieldType.Date:
							ReturnProp = new JProperty( "formatter", JqFieldType.date.ToString() );
							break;
						case CswNbtMetaDataFieldType.NbtFieldType.Time:
							ReturnProp = new JProperty( "formatter", JqFieldType.date.ToString());
							break;
						case CswNbtMetaDataFieldType.NbtFieldType.Number:
							ReturnProp = new JProperty( "formatter", JqFieldType.number.ToString());
							break;
						case CswNbtMetaDataFieldType.NbtFieldType.Link:
							ReturnProp = new JProperty( "formatter", JqFieldType.link.ToString());
							break;
						//case CswNbtMetaDataFieldType.NbtFieldType.Logical:
						//    ReturnVal = "checkbox";
						//    break;
						default:
							ReturnProp = new JProperty( "formatter", JqFieldType.text.ToString());
							break; 

					}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Format options can be defined for particular columns, overwriting the defaults from the language file.
			/// </summary>
			public JProperty FormatOptions
			{
				get
				{
					JProperty ReturnProp = null;
					//if() --no requirements for customizing cell rendering yet
					return ReturnProp;
				}
			}

			/// <summary>
			/// If set to true this column will not appear in the modal dialog where users can choose which columns to show or hide.
			/// </summary>
			public JProperty HidedLg
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if( _DoCssOverride )
					//{
					//    ReturnProp = new JProperty( "hidelg", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Specifies whether the property is hidden by default
			/// </summary>
			public JProperty Hidden
			{
				get
				{
					JProperty ReturnProp = null;
					// Only the nodeid PK column will be hidden at the moment
					//if( _DoCssOverride )
					//{
					//    ReturnProp = new JProperty( "hidden", string.Empty );
					//}
					return ReturnProp;
				}
			}

			private string _ColumnName
			{
				get
				{
					string ColumnName = NbtViewProperty.NodeTypeProp.PropName.ToLower().Replace(" ","_");
					if( ColumnName == "subgrid" || ColumnName == "cb" || ColumnName == "rn" )
					{
						ColumnName += "_col";
					}
					return ColumnName;
				}
			}

			/// <summary>
			/// Set the index name when sorting. Passed as sidx parameter.
			/// </summary>
			public JProperty Index
			{
				get
				{
					JProperty ReturnProp = new JProperty( "index", _ColumnName );
					return ReturnProp;
				}
			}

			/// <summary>
			/// When colNames array is empty, defines the heading for this column. 
			/// If both the colNames array and this setting are empty, the heading for this column comes from the name property.
			/// </summary>
			public JProperty Label
			{
				get
				{
					JProperty ReturnProp = new JProperty( "label", NbtViewProperty.NodeTypeProp.PropName );
					return ReturnProp;
				}
			}

			/// <summary>
			/// Set the unique name in the grid for the column. 
			/// This property is required. As well as other words used as property/event names, 
			/// the reserved words (which cannot be used for names) include subgrid, cb and rn.
			/// </summary>
			public JProperty Name
			{
				get
				{
					JProperty ReturnProp = new JProperty( "name", _ColumnName );
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines if the column can be resized. 
			/// Default = true.
			/// </summary>
			public JProperty Resizable
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if( _DoCssOverride )
					//{
					//    ReturnProp = new JProperty( "resizable", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// When used in search modules, disables or enables searching on that column.
			/// </summary>
			public JProperty Search
			{
				get
				{
					JProperty ReturnProp = new JProperty( "search", true );
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines the search options used searching Search Configuration. 
			/// See http://www.trirand.com/jqgridwiki/doku.php?id=wiki:search_config
			/// </summary>
			public JProperty SearchOptions
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if( )
					//{
					//    ReturnProp = new JProperty( "searchoptions", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Defines if this can be sorted.
			/// For now, always true.
			/// </summary>
			public JProperty Sortable
			{
				get
				{
					JProperty ReturnProp = new JProperty( "sortable", true );
					return ReturnProp;
				}
			}

			/// <summary>
			/// Used when datatype is local. Defines the type of the column for appropriate sorting.Possible values: 
			/// int/integer - for sorting integer 
			/// float/number/currency - for sorting decimal numbers 
			/// date - for sorting date 
			/// text - for text sorting 
			/// function - defines a custom function for sorting. To this function we pass the value to be sorted and it should return a value too. 
			/// </summary>
			public JProperty SortType
			{
				get
				{
					JProperty ReturnProp = null;

						switch( FieldType )
						{
							case CswNbtMetaDataFieldType.NbtFieldType.Date:
								ReturnProp = new JProperty("sorttype", JqFieldType.date.ToString() );
								break;
							case CswNbtMetaDataFieldType.NbtFieldType.Time:
								ReturnProp = new JProperty("sorttype", JqFieldType.date.ToString() );
								break;
							case CswNbtMetaDataFieldType.NbtFieldType.Number:
								ReturnProp = new JProperty("sorttype", JqFieldType.number.ToString() );
								break;
							default:
								ReturnProp = new JProperty("sorttype", JqFieldType.text.ToString() );
								break;
						}
					
					return ReturnProp;
				}
			}	
				

			/// <summary>
			/// Determines the type of the element when searching.
			/// </summary>
			public JProperty SType
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if(  )
					//{
					//    ReturnProp = new JProperty( "stype", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Valid only in Custom Searching and edittype : 'select' and describes the url from where we can get already-constructed select element.
			/// </summary>
			public JProperty SUrl
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if(  )
					//{
					//    ReturnProp = new JProperty( "surl", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// If this option is false the title is not displayed in that column when we hover a cell with the mouse.
			/// Default value == true.
			/// </summary>
			public JProperty Title
			{
				get
				{
					JProperty ReturnProp = null;
					// No need to implement this yet.
					//if(  )
					//{
					//    ReturnProp = new JProperty( "title", string.Empty );
					//}
					return ReturnProp;
				}
			}

			/// <summary>
			/// Set the initial width of the column, in pixels. This value currently can not be set as percentage.
			/// Default value is 150.
			/// </summary>
			public JProperty Width
			{
				get
				{
					JProperty ReturnProp = null;
					if( 0 < NbtViewProperty.Width )
					{
						string RetWidth = NbtViewProperty.Width.ToString();
						ReturnProp = new JProperty( "width", RetWidth );
					}
					return ReturnProp;
				}
			}
			#endregion
		} // private class JqGridViewProperty


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
