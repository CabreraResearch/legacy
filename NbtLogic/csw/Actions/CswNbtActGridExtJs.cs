using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Logic
{
    /// <summary>
    /// Options for an ExtJs-compatible grid
    /// </summary>
    public class CswNbtActGridExtJs
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtActGridExtJs( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        #region extJsGrid Definitions

        /// <summary>
        /// extJs Object Types
        /// </summary>
        public sealed class extJsXType : CswEnum<extJsXType>
        {
            private extJsXType( string Name ) : base( Name ) { }
            public static IEnumerable<extJsXType> _All { get { return CswEnum<extJsXType>.All; } }
            public static explicit operator extJsXType( string str )
            {
                extJsXType ret = Parse( str );
                return ( ret != null ) ? ret : extJsXType.Unknown;
            }
            public static readonly extJsXType Unknown = new extJsXType( "Unknown" );

            public static readonly extJsXType button = new extJsXType( "button" );               //Ext.button.Button      
            public static readonly extJsXType buttongroup = new extJsXType( "buttongroup" );     //Ext.container.ButtonGroup
            public static readonly extJsXType colorpalette = new extJsXType( "colorpalette" );   //Ext.picker.Color       
            public static readonly extJsXType component = new extJsXType( "component" );         //Ext.Component          
            public static readonly extJsXType container = new extJsXType( "container" );         //Ext.container.Container
            public static readonly extJsXType cycle = new extJsXType( "cycle" );                 //Ext.button.Cycle       
            public static readonly extJsXType dataview = new extJsXType( "dataview" );           //Ext.view.View          
            public static readonly extJsXType datepicker = new extJsXType( "datepicker" );       //Ext.picker.Date        
            public static readonly extJsXType editor = new extJsXType( "editor" );               //Ext.Editor             
            public static readonly extJsXType editorgrid = new extJsXType( "editorgrid" );       //Ext.grid.plugin.Editing
            public static readonly extJsXType grid = new extJsXType( "grid" );                   //Ext.grid.Panel         
            public static readonly extJsXType multislider = new extJsXType( "multislider" );     //Ext.slider.Multi       
            public static readonly extJsXType panel = new extJsXType( "panel" );                 //Ext.panel.Panel        
            public static readonly extJsXType progressbar = new extJsXType( "progressbar" );     //Ext.ProgressBar        
            public static readonly extJsXType slider = new extJsXType( "slider" );               //Ext.slider.Single      
            public static readonly extJsXType splitbutton = new extJsXType( "splitbutton" );     //Ext.button.Split       
            public static readonly extJsXType tabpanel = new extJsXType( "tabpanel" );           //Ext.tab.Panel          
            public static readonly extJsXType treepanel = new extJsXType( "treepanel" );         //Ext.tree.Panel         
            public static readonly extJsXType viewport = new extJsXType( "viewport" );           //Ext.container.Viewport 
            public static readonly extJsXType window = new extJsXType( "window" );               //Ext.window.Window      
            //Toolbar components
            public static readonly extJsXType pagingtoolbar = new extJsXType( "pagingtoolbar" ); //Ext.toolbar.Paging     
            public static readonly extJsXType toolbar = new extJsXType( "toolbar" );             //Ext.toolbar.Toolbar    
            public static readonly extJsXType tbfill = new extJsXType( "tbfill" );               //Ext.toolbar.Fill       
            public static readonly extJsXType tbitem = new extJsXType( "tbitem" );               //Ext.toolbar.Item       
            public static readonly extJsXType tbseparator = new extJsXType( "tbseparator" );     //Ext.toolbar.Separator  
            public static readonly extJsXType tbspacer = new extJsXType( "tbspacer" );           //Ext.toolbar.Spacer     
            public static readonly extJsXType tbtext = new extJsXType( "tbtext" );               //Ext.toolbar.TextItem   
            //Menu components
            public static readonly extJsXType menu = new extJsXType( "menu" );                   //Ext.menu.Menu          
            public static readonly extJsXType menucheckitem = new extJsXType( "menucheckitem" ); //Ext.menu.CheckItem     
            public static readonly extJsXType menuitem = new extJsXType( "menuitem" );           //Ext.menu.Item          
            public static readonly extJsXType menuseparator = new extJsXType( "menuseparator" ); //Ext.menu.Separator     
            public static readonly extJsXType menutextitem = new extJsXType( "menutextitem" );   //Ext.menu.Item          
            //Form components
            public static readonly extJsXType form = new extJsXType( "form" );                   //Ext.form.Panel         
            public static readonly extJsXType checkbox = new extJsXType( "checkbox" );           //Ext.form.field.Checkbox
            public static readonly extJsXType combo = new extJsXType( "combo" );                 //Ext.form.field.ComboBox
            public static readonly extJsXType datefield = new extJsXType( "datefield" );         //Ext.form.field.Date    
            public static readonly extJsXType displayfield = new extJsXType( "displayfield" );   //Ext.form.field.Display 
            public static readonly extJsXType field = new extJsXType( "field" );                 //Ext.form.field.Base    
            public static readonly extJsXType fieldset = new extJsXType( "fieldset" );           //Ext.form.FieldSet      
            public static readonly extJsXType hidden = new extJsXType( "hidden" );               //Ext.form.field.Hidden  
            public static readonly extJsXType htmleditor = new extJsXType( "htmleditor" );       //Ext.form.field.HtmlEdit
            public static readonly extJsXType label = new extJsXType( "label" );                 //Ext.form.Label         
            public static readonly extJsXType numberfield = new extJsXType( "numberfield" );     //Ext.form.field.Number  
            public static readonly extJsXType radio = new extJsXType( "radio" );                 //Ext.form.field.Radio   
            public static readonly extJsXType radiogroup = new extJsXType( "radiogroup" );       //Ext.form.RadioGroup    
            public static readonly extJsXType textarea = new extJsXType( "textarea" );           //Ext.form.field.TextArea
            public static readonly extJsXType textfield = new extJsXType( "textfield" );         //Ext.form.field.Text    
            public static readonly extJsXType timefield = new extJsXType( "timefield" );         //Ext.form.field.Time    
            public static readonly extJsXType trigger = new extJsXType( "trigger" );             //Ext.form.field.Trigger 
            //Chart components
            public static readonly extJsXType chart = new extJsXType( "chart" );                 //Ext.chart.Chart        
            public static readonly extJsXType barchart = new extJsXType( "barchart" );           //Ext.chart.series.Bar   
            public static readonly extJsXType columnchart = new extJsXType( "columnchart" );     //Ext.chart.series.Column
            public static readonly extJsXType linechart = new extJsXType( "linechart" );         //Ext.chart.series.Line  
            public static readonly extJsXType piechart = new extJsXType( "piechart" );           //Ext.chart.series.Pie   
            //Grid Column components
            public static readonly extJsXType actioncolumn = new extJsXType( "actioncolumn" );     //Ext.grid.column.Action
            public static readonly extJsXType booleancolumn = new extJsXType( "booleancolumn" );   //Ext.grid.column.Boolean
            public static readonly extJsXType datecolumn = new extJsXType( "datecolumn" );         //Ext.grid.column.Date
            public static readonly extJsXType numbercolumn = new extJsXType( "numbercolumn" );     //Ext.grid.column.Number
            public static readonly extJsXType templatecolumn = new extJsXType( "templatecolumn" ); //Ext.grid.column.Template
        }


        // Example JSON:
        // { 
        //     grid: {
        //         fields:['name', 'email', 'phone'],
        //         columns:  [
        //             { header: 'Name',  dataIndex: 'name' },
        //             { header: 'Email', dataIndex: 'email', flex: 1 },
        //             { header: 'Phone', dataIndex: 'phone' }
        //         ],
        //         data:{'items':[
        //             { 'name': 'Lisa',  'email':'lisa@simpsons.com',  'phone':'555-111-1224'  },
        //             { 'name': 'Bart',  'email':'bart@simpsons.com',  'phone':'555-222-1234' },
        //             { 'name': 'Homer', 'email':'home@simpsons.com',  'phone':'555-222-1244'  },
        //             { 'name': 'Marge', 'email':'marge@simpsons.com', 'phone':'555-222-1254'  }
        //         ]}
        //     }
        // }

        private const string _DataIndexPrefix = "val_";

        private class extJsGridFieldDef
        {
            /// <summary>
            /// Name for the field (matches column definition's dataIndex)
            /// </summary>
            public string name;
            /// <summary>
            /// Data type
            /// string, date, number
            /// </summary>
            public string type = "string";

            public JObject ToJson()
            {
                JObject Jfield = new JObject();
                Jfield["name"] = _DataIndexPrefix + name;
                if( type != string.Empty )
                {
                    Jfield["type"] = type;
                }
                return Jfield;
            } // ToJson()
        }

        private class extJsGridColumnDef
        {
            /// <summary>
            /// Display name for the column
            /// </summary>
            public string header;
            /// <summary>
            /// Internal column name
            /// </summary>
            public string dataIndex;
            /// <summary>
            /// Width in pixels (default is 100)
            /// </summary>
            public Int32 width;
            /// <summary>
            /// Whether this column will widen to encompass all extra space in the grid
            /// </summary>
            public bool flex = false;
            /// <summary>
            /// Hide this column (can be manually displayed on client)
            /// </summary>
            public bool hidden = false;
            /// <summary>
            /// Data type for grid column
            /// </summary>
            public extJsXType xtype = extJsXType.Unknown;
            /// <summary>
            /// Date format (if xtype == datecolumn)
            /// </summary>
            public string dateformat;

            public JObject ToJson()
            {
                JObject Jcol = new JObject();
                Jcol["id"] = dataIndex;
                Jcol["header"] = header;
                Jcol["dataIndex"] = _DataIndexPrefix + dataIndex;
                if( width > 0 )
                {
                    Jcol["width"] = width;
                }
                if( flex )
                {
                    Jcol["flex"] = flex;
                }
                if( hidden )
                {
                    Jcol["hidden"] = hidden;
                }
                if( xtype != extJsXType.Unknown )
                {
                    Jcol["xtype"] = xtype.ToString();
                }
                if( dateformat != string.Empty )
                {
                    Jcol["format"] = dateformat;
                }
                return Jcol;
            } // ToJson()

        } // class extJsGridColumnDef

        private class extJsGridRow
        {
            /// <summary>
            /// Name - value pairs that encompass the row data
            /// </summary>
            public Dictionary<string, string> data = new Dictionary<string, string>();

            public JObject ToJson()
            {
                JObject Jrow = new JObject();
                foreach( string Key in data.Keys )
                {
                    Jrow[_DataIndexPrefix + Key] = data[Key];
                }
                return Jrow;
            } // ToJson()

        } // class extJsGridRow

        private class extJsGrid
        {
            /// <summary>
            /// Header Title for Grid
            /// </summary>
            public string title = string.Empty;
            /// <summary>
            /// Field definitions
            /// </summary>
            public Collection<extJsGridFieldDef> fields = new Collection<extJsGridFieldDef>();
            /// <summary>
            /// Column definitions
            /// </summary>
            public Collection<extJsGridColumnDef> columns = new Collection<extJsGridColumnDef>();
            /// <summary>
            /// Row data
            /// </summary>
            public Collection<extJsGridRow> rows = new Collection<extJsGridRow>();
            /// <summary>
            /// Page size
            /// </summary>
            public Int32 PageSize = 50;

            public bool columnsContains( string header )
            {
                bool ret = false;
                foreach( extJsGridColumnDef col in columns )
                {
                    ret = ret || ( col.header.ToLower() == header.ToLower() );
                }
                return ret;
            } // columnsContains

            public JObject ToJson()
            {
                JArray Jfields = new JArray();
                JArray Jcolumns = new JArray();
                JArray Jdataitems = new JArray();

                foreach( extJsGridFieldDef fld in fields )
                {
                    Jfields.Add( fld.ToJson() );
                }
                foreach( extJsGridColumnDef col in columns )
                {
                    Jcolumns.Add( col.ToJson() );
                }
                foreach( extJsGridRow Row in rows )
                {
                    Jdataitems.Add( Row.ToJson() );
                }

                JObject Jret = new JObject();
                Jret["grid"] = new JObject();
                Jret["grid"]["title"] = title;
                Jret["grid"]["fields"] = Jfields;
                Jret["grid"]["columns"] = Jcolumns;
                Jret["grid"]["pageSize"] = PageSize;
                Jret["grid"]["data"] = new JObject();
                Jret["grid"]["data"]["items"] = Jdataitems;
                return Jret;
            } // ToJson()

        } // class extJsGrid
        #endregion Grid Definitions


        public JObject TreeToJson( CswNbtView View, ICswNbtTree Tree, bool IsPropertyGrid = false )
        {
            extJsGrid grid = new extJsGrid();
            grid.title = View.ViewName;
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            if( IsPropertyGrid && Tree.getChildNodeCount() > 0 )
            {
                Tree.goToNthChild( 0 );
            }

            extJsGridFieldDef nodeIdFldDef = new extJsGridFieldDef();
            nodeIdFldDef.name = "nodeid";
            grid.fields.Add( nodeIdFldDef );
            extJsGridColumnDef nodeIdColDef = new extJsGridColumnDef();
            nodeIdColDef.header = "Internal ID";
            nodeIdColDef.dataIndex = "nodeid";
            nodeIdColDef.hidden = true;
            grid.columns.Add( nodeIdColDef );

            extJsGridFieldDef nodekeyFldDef = new extJsGridFieldDef();
            nodekeyFldDef.name = "nodekey";
            grid.fields.Add( nodekeyFldDef );
            extJsGridColumnDef nodekeyColDef = new extJsGridColumnDef();
            nodekeyColDef.header = "Internal Key";
            nodekeyColDef.dataIndex = "nodekey";
            nodekeyColDef.hidden = true;
            grid.columns.Add( nodekeyColDef );

            extJsGridFieldDef nodenameFldDef = new extJsGridFieldDef();
            nodenameFldDef.name = "nodename";
            grid.fields.Add( nodenameFldDef );
            extJsGridColumnDef nodenameColDef = new extJsGridColumnDef();
            nodenameColDef.header = "Internal Name";
            nodenameColDef.dataIndex = "nodename";
            nodenameColDef.hidden = true;
            grid.columns.Add( nodenameColDef );

            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                extJsGridRow gridrow = new extJsGridRow();
                Tree.goToNthChild( c );

                gridrow.data.Add( "nodeid", Tree.getNodeIdForCurrentPosition().ToString() );
                gridrow.data.Add( "nodekey", Tree.getNodeKeyForCurrentPosition().ToString() );
                gridrow.data.Add( "nodename", Tree.getNodeNameForCurrentPosition().ToString() );

                _TreeNodeToGrid( View, Tree, grid, gridrow );

                Tree.goToParentNode();
                grid.rows.Add( gridrow );
            }

            return grid.ToJson();
        } // TreeToJson()

        private void _TreeNodeToGrid( CswNbtView View, ICswNbtTree Tree, extJsGrid grid, extJsGridRow gridrow )
        {
            JArray ChildProps = Tree.getChildNodePropsOfNode();
            foreach( JObject Prop in ChildProps )
            {
                string header = Prop[CswNbtTreeNodes._AttrName_NodePropName].ToString();
                string dataIndex = Prop[CswNbtTreeNodes._AttrName_NodePropId].ToString();
                string value = Prop[CswNbtTreeNodes._AttrName_NodePropGestalt].ToString();
                Int32 propid = CswConvert.ToInt32( Prop[CswNbtTreeNodes._AttrName_NodePropId].ToString() );

                if( false == grid.columnsContains( header ) )
                {
                    // Potential bug here
                    // If the same property is added to the view more than once, we'll only use the grid definition for the first instance

                    CswNbtViewProperty ViewProp = View.findPropertyById( NbtViewPropType.NodeTypePropId, propid );
                    if( ViewProp == null )
                    {
                        CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( propid );
                        if( MetaDataProp != null && MetaDataProp.ObjectClassPropId != Int32.MinValue )
                        {
                            ViewProp = View.findPropertyById( NbtViewPropType.ObjectClassPropId, MetaDataProp.ObjectClassPropId );
                        }
                    }

                    extJsGridFieldDef flddef = new extJsGridFieldDef();
                    flddef.name = dataIndex;
                    extJsGridColumnDef coldef = new extJsGridColumnDef();
                    coldef.header = header;
                    coldef.dataIndex = dataIndex;
                    coldef.hidden = ( false == ViewProp.ShowInGrid );
                    switch( ViewProp.FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                            coldef.xtype = extJsXType.numbercolumn;
                            flddef.type = "number";
                            break;
                        case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                            coldef.xtype = extJsXType.datecolumn;
                            coldef.dateformat = "m/d/Y";
                            flddef.type = "date";
                            break;
                    }
                    if( ViewProp.Width > 0 )
                    {
                        coldef.width = ViewProp.Width * 7; // approx. characters to pixels
                    }
                    grid.columns.Add( coldef );
                    grid.fields.Add( flddef );
                }
                gridrow.data[dataIndex] = value;
            } // foreach( JObject Prop in ChildProps )

            // Recurse, but add properties of child nodes to the same gridrow
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                _TreeNodeToGrid( View, Tree, grid, gridrow );

                Tree.goToParentNode();
            }
        } // _TreeNodeToGrid

        public JObject DataTableToJSON( DataTable DT, bool Editable = false )
        {
            extJsGrid grid = new extJsGrid();
            if( _CswNbtResources.CurrentNbtUser != null && _CswNbtResources.CurrentNbtUser.PageSize > 0 )
            {
                grid.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
            }

            foreach( DataColumn Column in DT.Columns )
            {
                extJsGridColumnDef gridcol = new extJsGridColumnDef();
                gridcol.header = Column.ColumnName;
                gridcol.dataIndex = Column.ColumnName;
                grid.columns.Add( gridcol );
            }

            foreach( DataRow Row in DT.Rows )
            {
                extJsGridRow gridrow = new extJsGridRow();
                foreach( DataColumn Column in DT.Columns )
                {
                    gridrow.data[Column.ColumnName] = Row[Column].ToString();
                }
                grid.rows.Add( gridrow );
            } // foreach( DataRow Row in DT.Rows )

            return grid.ToJson();
        } // DataTableToJSON


    } // class CswNbtActGridExtJs
} // namespace ChemSW.Nbt.Logic
