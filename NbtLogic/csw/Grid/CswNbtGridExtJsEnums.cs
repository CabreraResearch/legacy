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

namespace ChemSW.Nbt.Grid.ExtJs
{
    // Enums for an ExtJs-compatible grid

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
    } // extJsXType

} // namespace ChemSW.Nbt.Logic
