using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswGridData
    {
        private CswNbtResources _CswNbtResources;
        public CswGridData( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
        }

        public string PkColumn = string.Empty;
        public bool HidePkColumn = true;
        public Int32 PageSize;
        public bool GridAutoEncode = true;
        public Int32 GridHeight = 300;
        //public string GridRowList = "[10,25,50]";
        public string GridSortName = string.Empty;
        public string GridTitle = string.Empty;
        public Int32 GridWidth = Int32.MinValue;
        public bool CanEdit = false;
        public bool CanDelete = false;

        private Int32 _ColumnsWidth = 0;

        public enum JqGridJsonOptions
        {
            Unknown,
            /// <summary>
            /// Autodetect jqGrid fieldtypes and encode accordingly
            /// </summary>
            autoencode,
            /// <summary>
            /// Automatically adjust grid width to fit contents
            /// </summary>
            autowidth,
            /// <summary>
            /// Display name of the grid
            /// </summary>
            caption,
            /// <summary>
            /// Complex Array of column names and definitional data determined in part by JqFieldType
            /// </summary>
            colModel,
            /// <summary>
            /// Simple Array of friendly column names
            /// </summary>
            colNames,
            /// <summary>
            /// Either a URL or a JSON object containing column
            /// </summary>
            data,
            /// <summary>
            /// Data type for grid, usually == local
            /// </summary>
            datatype,
            /// <summary>
            /// If (viewrecords), defines the text to display if record count == 0
            /// </summary>
            emptyrecords,
            /// <summary>
            /// Height of grid
            /// </summary>
            height,
            /// <summary>
            /// An array of numbers used to indicate how many rows to display in the grid
            /// </summary>
            rowList,
            /// <summary>
            /// Row Number
            /// </summary>
            rowNum,
            /// <summary>
            /// Column name in grid to perform first sort
            /// </summary>
            sortname,
            /// <summary>
            /// If true, jqGrid displays the beginning and ending record number in the grid, out of the total number of records in the query.
            /// </summary>
            viewrecords,
            /// <summary>
            /// Grid width in pixels
            /// </summary>
            width
        };

        public enum JqGridDataOptions
        {
            Unknown,
            /// <summary>
            /// Row data for first/selected page
            /// </summary>
            rows,
            /// <summary>
            /// For paging, first selected page
            /// </summary>
            page,
            /// <summary>
            /// For paging, total number of pages
            /// </summary>
            total,
            /// <summary>
            /// For paging, total # of records
            /// </summary>
            records,
            row
        };

        private const string _NoResultsDisplayString = "No Results";

        public JObject DataTableToJSON( DataTable Data )
        {
            // Columns
            JArray JColumnNames = new JArray();
            JArray JColumnDefs = new JArray();
            foreach( DataColumn Column in Data.Columns )
            {
                bool IsKey = ( Column.ColumnName.ToLower() == PkColumn.ToLower() );
                dataColumnToJson( Column, JColumnNames, JColumnDefs, IsKey );
            } // foreach( DataColumn Column in Data.Columns )

            // Rows
            JArray JRows = new JArray();
            foreach( DataRow Row in Data.Rows )
            {
                dataRowToJson( Row, JRows );
            }

            JObject FinalGrid = makeJqGridJSON( JColumnNames, JColumnDefs, JRows );

            return FinalGrid;

        } // _mapDataTable()

        public void dataColumnToJson( DataColumn Column, JArray JColumnNames, JArray JColumnDefs, bool IsKey = false, bool IsHidden = false )
        {
            string ColumnName = Column.ColumnName.ToUpperInvariant();
            makeJqColumn( ColumnName, JColumnNames, JColumnDefs, IsKey, IsHidden );
        }

        public void makeJqColumn( String ColumnName, JArray JColumnNames, JArray JColumnDefs, bool IsKey = false, bool IsHidden = false )
        {
            JColumnNames.Add( ColumnName.ToUpperInvariant() );
            JObject ThisColumnDef = new JObject();
            ThisColumnDef["name"] = ColumnName.ToUpperInvariant();
            ThisColumnDef["index"] = ColumnName.ToUpperInvariant();
            if( IsKey )
            {
                ThisColumnDef["key"] = true;
            }
            if( ( IsKey && HidePkColumn ) || IsHidden )
            {
                ThisColumnDef["hidden"] = true;
            }
            _ColumnsWidth += ColumnName.Length;
            JColumnDefs.Add( ThisColumnDef );
        }

        public void dataRowToJson( DataRow Row, JArray JRows )
        {

            JObject RowObj = new JObject();
            foreach( DataColumn Column in Row.Table.Columns )
            {
                makeJqCell( RowObj, Column.ColumnName.ToUpperInvariant(), Row[Column].ToString() );
            }
            JRows.Add( RowObj );
        } // _mapDataTable()

        public void makeJqCell( JObject RowObj, String ColumnName, String Value )
        {
            RowObj[ColumnName] = Value;
        }

        /// <summary>
        /// Returns a JSON Array representing grid rows (a row as a JObject of JProperty key/value pairs);
        /// This anticipates XElements as derived from a Tree from a View
        /// </summary>
        public JArray getGridRowsJSON( IEnumerable<XElement> GridNodes )
        {
            JArray RowsArray = new JArray(
                                from Element in GridNodes //not recursive
                                select new JObject(
                                    new JProperty( "nodeid", Element.Attribute( "nodeid" ).Value ),
                                    new JProperty( "nodepk", new CswPrimaryKey( "nodes", CswConvert.ToInt32( Element.Attribute( "nodeid" ).Value ) ).ToString() ),
                                    new JProperty( "cswnbtnodekey", wsTools.ToSafeJavaScriptParam( Element.Attribute( "key" ).Value ) ),
                                    new JProperty( "nodename", Element.Attribute( "nodename" ).Value ),
                                        from DirtyElement in Element.DescendantNodes().OfType<XElement>() // recursive
                                        where DirtyElement.Name == ( "NbtNodeProp" )
                                        select _massageGridCell( DirtyElement )
                                    )
                                );

            return RowsArray;
        } // getGridRowsJSON()

        /// <summary>
        /// Translates property value into human readable text.
        /// Currently only handles Logical fieldtype.
        /// </summary>
        private static JProperty _massageGridCell( XElement DirtyElement )
        {
            string CleanPropName = DirtyElement.Attribute( "name" ).Value.ToLower().Replace( " ", "_" );
            string CleanValue = string.Empty;
            string DirtyValue = DirtyElement.Attribute( "gestalt" ).Value;
            string PropFieldTypeString = DirtyElement.Attribute( "fieldtype" ).Value;
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
            JProperty CleanProp = new JProperty( CleanPropName, CleanValue );
            return CleanProp;
        }

        /// <summary>
        /// Generates a JSON array of friendly Column Names
        /// </summary>
        public JArray getGridColumnNamesJson( IEnumerable<CswNbtViewProperty> PropCollection )
        {
            JArray ColumnArray = new JArray(
                from ViewProp in PropCollection.Select( Prop => new CswViewBuilderProp( Prop ) )
                select new JValue( ViewProp.PropName )
                );
            return ColumnArray;
        }

        /// <summary>
        /// Generates a JSON property with the definitional data for a jqGrid Column Array
        /// </summary>
        public JArray getGridColumnDefinitionJson( IEnumerable<CswNbtViewProperty> PropCollection )
        {
            JArray ColumnDefArray = new JArray(
                from ViewProp in PropCollection.Select( Prop => new CswViewBuilderProp( Prop ) )
                where ViewProp != null
                select JqGridViewProperty.getJqGridAttributesForViewProp( ViewProp )
                );
            return ColumnDefArray;
        }

        /// <summary>
        /// Combines required jqGrid options into jqGrid consumable JObject
        /// </summary>
        public JObject makeJqGridJSON( JArray ColumnNames, JArray ColumnDefinition, JArray Rows )
        {
            JObject Ret = new JObject();

            Ret[JqGridJsonOptions.datatype.ToString()] = "local";
            Ret[JqGridJsonOptions.colNames.ToString()] = ColumnNames;
            Ret[JqGridJsonOptions.colModel.ToString()] = ColumnDefinition;
            Ret[JqGridJsonOptions.data.ToString()] = Rows;
            Ret[JqGridJsonOptions.rowNum.ToString()] = PageSize;
            Ret[JqGridJsonOptions.viewrecords.ToString()] = true;
            Ret[JqGridJsonOptions.emptyrecords.ToString()] = _NoResultsDisplayString;
            if( GridWidth == Int32.MinValue )
            {
                Int32 ColWidth = _ColumnsWidth * 14;
                GridWidth = ColWidth < 800 ? ColWidth : 450;
            }
            Ret[JqGridJsonOptions.width.ToString()] = GridWidth;
            //Ret[JqGridJsonOptions.autowidth.ToString()] = true;
            Ret[JqGridJsonOptions.sortname.ToString()] = GridSortName;
            Ret[JqGridJsonOptions.autoencode.ToString()] = GridAutoEncode;
            Ret[JqGridJsonOptions.height.ToString()] = GridHeight;
            Ret[JqGridJsonOptions.caption.ToString()] = GridTitle;
            Ret["CanEdit"] = CanEdit.ToString().ToLower();
            Ret["CanDelete"] = CanDelete.ToString().ToLower();

            return Ret;
        }

    } // class CswGridData

    /// <summary>
    /// For the transformation of XElement Attribute types into valid JProperty and JObject types
    /// </summary>
    public class JqGridViewProperty
    {
        private readonly bool _DoCssOverride = false;
        private readonly string _LiteralColumnName = string.Empty;
        private readonly string _FriendlyColumnName = string.Empty;
        private readonly Int32 _ColumnWidth = Int32.MinValue;

        public enum JqFieldType
        {
            Unknown,
            actions,
            checkbox,
            currency,
            date,
            /// <summary>
            /// This is not a jqGrid option, but we will (later) need it to distinguish from date and time
            /// </summary>
            datetime,
            email,
            function,
            integer,
            link,
            number,
            showlink,
            select,
            text,
            /// <summary>
            /// Also not a jqGrid option, but we need it to distinguish from datetime and date
            /// </summary>
            time

        };
        private readonly JqFieldType _JqFieldType = JqFieldType.Unknown;


        public enum JqGridSortBy
        {
            Unknown,
            asc,
            desc
        }

        private JqGridSortBy _SortBy = JqGridSortBy.Unknown;

        public JqGridViewProperty( CswViewBuilderProp ViewProperty, bool DoCssOverride = false )
        {
            switch( ViewProperty.FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    _JqFieldType = JqFieldType.number;
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    _JqFieldType = JqFieldType.date;
                    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                //    //not implemented yet
                //    _JqFieldType = JqFieldType.datetime;
                //    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.Time:
                //    _JqFieldType = JqFieldType.time;
                //    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    _JqFieldType = JqFieldType.link;
                    break;
                default:
                    _JqFieldType = JqFieldType.text;
                    break;
            }

            if( ViewProperty.SortBy )
            {
                switch( ViewProperty.SortMethod )
                {
                    case CswNbtViewProperty.PropertySortMethod.Ascending:
                        _SortBy = JqGridSortBy.asc;
                        break;
                    case CswNbtViewProperty.PropertySortMethod.Descending:
                        _SortBy = JqGridSortBy.desc;
                        break;
                }
            }

            _LiteralColumnName = ViewProperty.PropName.ToLower();
            _FriendlyColumnName = ViewProperty.PropName.ToLower().Replace( " ", "_" );
            _ColumnWidth = ViewProperty.Width;

            _DoCssOverride = DoCssOverride;
        } //ctor

        /// <summary>
        /// Transforms an Nbt View Property into well-form JSON for consumption by a jqGrid
        /// </summary>
        public static JObject getJqGridAttributesForViewProp( CswViewBuilderProp ViewProperty )
        {
            var JqGridViewProp = new JqGridViewProperty( ViewProperty );
            var ReturnObj = new JObject();
            const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;
            Type JType = JqGridViewProp.GetType();

            //foreach public/instanced property in the JqGridViewProperty class of type JProperty which is not null, 
            //add the property value to the return JObject
            foreach( JProperty ThisPropAttribute in ( from PropertyInfo in JType.GetProperties( Flags )
                                                      where PropertyInfo.PropertyType == typeof( JProperty )
                                                      select (JProperty) PropertyInfo.GetValue( JqGridViewProp, null ) )
                                                        .Where( ThisPropAttribute => null != ThisPropAttribute ) )
            {
                if( null != ReturnObj.Property( ThisPropAttribute.Name ) )
                {
                    throw new CswDniException( ErrorType.Error, "Error attempting to add duplicate property to collection", "Property: " + ThisPropAttribute + " already exists in the JObject: " + ReturnObj.ToString() );
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
                switch( _JqFieldType )
                {
                    case JqFieldType.date:
                        ReturnProp = new JProperty( "datefmt", "m/d/Y" );
                        break;
                    case JqFieldType.time:
                        ReturnProp = new JProperty( "datefmt", "h:i A" );
                        break;
                    //case JqFieldType.datetime:
                    //    //this fieldtype is not yet implemented
                    //    ReturnProp = new JProperty( "datefmt", "m/d/Y h:i A" );
                    //    break;
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
                if( _SortBy != JqGridSortBy.Unknown )
                {
                    ReturnProp = new JProperty( "firstsortorder", _SortBy.ToString() );
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
                switch( _JqFieldType )
                {
                    // jqGrid handles all date/time formats as the same type == date; 
                    // we must specify a datefmt to define the display template
                    case JqFieldType.date:
                        ReturnProp = new JProperty( "formatter", JqFieldType.date.ToString() );
                        break;
                    case JqFieldType.time:
                        ReturnProp = new JProperty( "formatter", JqFieldType.date.ToString() );
                        break;
                    //case JqFieldType.datetime:
                    //    //not implemented
                    //    ReturnProp = new JProperty( "formatter", JqFieldType.date.ToString() );
                    //    break;
                    case JqFieldType.number:
                        ReturnProp = new JProperty( "formatter", JqFieldType.number.ToString() );
                        break;
                    case JqFieldType.link:
                        ReturnProp = new JProperty( "formatter", JqFieldType.link.ToString() );
                        break;

                }
                return ReturnProp;
            }
        }

        /// <summary>
        /// Format options can be defined for particular columns, overwriting the defaults from the language file.
        /// http://www.trirand.com/jqgridwiki/doku.php?id=wiki:predefined_formatter
        /// </summary>
        public JProperty FormatOptions
        {
            get
            {
                JProperty ReturnProp = null;
                switch( _JqFieldType )
                {
                    // jqGrid handles all date/time formats as the same type == date; 
                    // we must specify a datefmt to define the display template
                    case JqFieldType.date:
                        ReturnProp = new JProperty( "formatoptions",
                                            new JObject(
                                                new JProperty( "srcformat", "Y-m-d" ),
                                                new JProperty( "newformat", "m/d/Y" ) )
                                        );
                        break;
                    case JqFieldType.time:
                        ReturnProp = new JProperty( "formatoptions",
                                            new JObject(
                                                new JProperty( "srcformat", "H:i:s" ),
                                                new JProperty( "newformat", "H:i:s" ) )
                                        );
                        break;
                    //case JqFieldType.datetime:
                    //    //not implemented yet
                    //    ReturnProp = new JProperty ( "formatoptions",
                    //                        new JObject( new JProperty( "srcformat", "Y-m-d H:i:s" ),
                    //                              new JProperty( "newformat", "m/d/Y H:i:s" ) )
                    //                    );
                    //    break;
                    case JqFieldType.number:
                        ReturnProp = new JProperty( "formatoptions",
                                            new JObject(
                                                new JProperty( "decimalSeparator", "." ),
                                                new JProperty( "thousandsSeparator", "," ),
                                                new JProperty( "decimalPlaces", "2" ),
                                                new JProperty( "defaultValue", "" ) )
                                        );
                        break;
                    case JqFieldType.link:
                        ReturnProp = new JProperty( "formatoptions",
                                            new JObject(
                                                new JProperty( "target", "" ) )
                                        );
                        break;
                }
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
                string ColumnName = _FriendlyColumnName;
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
                JProperty ReturnProp = new JProperty( "label", _LiteralColumnName );
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

                switch( _JqFieldType )
                {
                    // jqGrid treats all date/time fields the same.
                    case JqFieldType.date:
                        ReturnProp = new JProperty( "sorttype", JqFieldType.date.ToString() );
                        break;
                    case JqFieldType.datetime:
                        ReturnProp = new JProperty( "sorttype", JqFieldType.date.ToString() );
                        break;
                    case JqFieldType.time:
                        ReturnProp = new JProperty( "sorttype", JqFieldType.date.ToString() );
                        break;
                    case JqFieldType.number:
                        ReturnProp = new JProperty( "sorttype", JqFieldType.number.ToString() );
                        break;
                    default:
                        ReturnProp = new JProperty( "sorttype", JqFieldType.text.ToString() );
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
                if( 0 < _ColumnWidth )
                {
                    string RetWidth = _ColumnWidth.ToString();
                    ReturnProp = new JProperty( "width", RetWidth );
                }
                return ReturnProp;
            }
        }
        #endregion
    } // public class JqGridViewProperty

} // namespace ChemSW.Nbt.WebServices
