using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using Telerik.Web.UI;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    [ToolboxData( "<{0}:CswViewParameterEditor runat=server></{0}:CswViewParameterEditor>" )]
    public class CswViewFilterEditor : CompositeControl, INamingContainer
    {
        #region Properties

        public bool ShowAdvanced
        {
            get { return _ShowAdvancedHF.Value == "true"; }
            set { _ShowAdvancedHF.Value = value.ToString().ToLower(); }
        }

        private bool _Cleared { get { return _ClearedHF.Value == "true"; } }

        #endregion Properties

        #region Lifecycle

        private CswNbtResources _CswNbtResources = null;
        private CswNbtView _View;
        private RadAjaxManager _AjaxMgr;

        public CswViewFilterEditor( CswNbtResources CswNbtResources, RadAjaxManager AjaxMgr )
        {
            _CswNbtResources = CswNbtResources;
            _AjaxMgr = AjaxMgr;
        }

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                base.OnInit( e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

        }

        private Button _FilterButton;
        private CswAutoTable _FilterItemTable;
        private LinkButton _AdvancedLink;
        private Button _ClearButton;
        private HiddenField _ShowAdvancedHF;
        private HiddenField _ClearedHF;

        protected override void CreateChildControls()
        {
            try
            {
                HtmlGenericControl FilterDiv = new HtmlGenericControl( "div" );
                FilterDiv.Attributes.Add( "class", "FiltersTableDiv" );
                this.Controls.Add( FilterDiv );

                _FilterItemTable = new CswAutoTable();
                _FilterItemTable.ID = "FiltersTable";
                _FilterItemTable.CssClass = "FiltersTable";
                //_FilterItemTable.AllCellRightAlign = true;
                FilterDiv.Controls.Add( _FilterItemTable );

                _FilterItemTable.addControl( 0, 0, new CswLiteralText( "_FilterItemTable" ) );

                _ClearButton = new Button();
                _ClearButton.ID = "ClearLink";
                _ClearButton.Text = "Clear";
                _ClearButton.Click += new EventHandler( ClearLink_Click );

                _AdvancedLink = new LinkButton();
                _AdvancedLink.ID = "AdvancedLink";
                _AdvancedLink.Text = "Advanced";
                _AdvancedLink.Click += new EventHandler( AdvancedLink_Click );

                _ShowAdvancedHF = new HiddenField();
                _ShowAdvancedHF.ID = "ShowAdvancedHF";
                _ShowAdvancedHF.Value = "false";

                _ClearedHF = new HiddenField();
                _ClearedHF.ID = "ClearHF";
                _ClearedHF.Value = "false";

                _FilterButton = new Button();
                _FilterButton.ID = "SetFilters";
                _FilterButton.Text = "Search";
                _FilterButton.CssClass = "Button";
                _FilterButton.Visible = false;
                _FilterButton.Click += new EventHandler( FilterButton_Click );
                //this.Controls.Add( _FilterButton );  // this is done below

                base.CreateChildControls();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                if( _Cleared )
                {
                    _View = null;
                    LoadSearch( null, null );
                }

                _AjaxMgr.AjaxSettings.AddAjaxSetting( _AdvancedLink, _FilterItemTable );
                _AjaxMgr.AjaxSettings.AddAjaxSetting( _ClearButton, _FilterItemTable );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                _AdvancedLink.OnClientClick = "CswViewFilterEditor_AdvancedLink_Click('" + _ShowAdvancedHF.ClientID + "');";
                _ClearButton.OnClientClick = "CswViewFilterEditor_ClearButton_Click('" + _ClearedHF.ClientID + "');";

                if( _FilterCount > 0 )
                {
                    _FilterButton.Visible = true;
                    _FilterItemTable.Visible = true;
                }
                else
                {
                    _FilterButton.Visible = false;
                    _FilterItemTable.Visible = false;
                }

                foreach( CswPropertyFilter Filter in _Filters )
                {
                    Filter.ShowSubFieldAndMode = ShowAdvanced;
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }

        #endregion Lifecycle

        #region Events

        void ClearLink_Click( object sender, EventArgs e )
        {
        }

        void AdvancedLink_Click( object sender, EventArgs e )
        {
            //ShowAdvanced = true;
        }

        public event CswErrorHandler OnError;
        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }

        public void LoadView( CswNbtView View )
        {
            EnsureChildControls();
            _View = View;
            _initFromView();
        }

        public void LoadSearch( CswNbtMetaDataNodeType DefaultNodeType, CswNbtMetaDataNodeTypeProp DefaultProp )
        {
            EnsureChildControls();

            _clearFilters();

            CswPropertyFilter Filter;
            if( DefaultNodeType != null )
            {
                if( DefaultProp != null )
                    Filter = new CswPropertyFilter( _CswNbtResources, _AjaxMgr, DefaultNodeType.NodeTypeId, DefaultProp.PropName, true, true, true, false );
                else
                    Filter = new CswPropertyFilter( _CswNbtResources, _AjaxMgr, DefaultNodeType.NodeTypeId, string.Empty, true, true, true, false );
            }
            else
                Filter = new CswPropertyFilter( _CswNbtResources, _AjaxMgr, Int32.MinValue, string.Empty, true, true, true, false );

            Filter.OnError += new CswErrorHandler( HandleError );
            _FilterItemTable.Rows.Add( Filter ); // addControl( CurrentFilterRow, 0, Filter );
            _Filters.Add( Filter );
            _FilterCount++;

            _placeButtons();
        }

        private void _placeButtons()
        {
            TableCell AdvancedCell = _FilterItemTable.getCell( _FilterCount + 1, 0 );
            AdvancedCell.ColumnSpan = 2;
            AdvancedCell.Controls.Add( _ClearButton );
            AdvancedCell.Controls.Add( new CswLiteralNbsp( 3 ) );
            AdvancedCell.Controls.Add( _AdvancedLink );
            AdvancedCell.Controls.Add( _ShowAdvancedHF );
            AdvancedCell.Controls.Add( _ClearedHF );

            TableCell ButtonCell = _FilterItemTable.getCell( _FilterCount + 1, 5 );
            ButtonCell.Style.Add( HtmlTextWriterStyle.TextAlign, "right" );
            ButtonCell.Controls.Add( _FilterButton );
        }

        public class ViewChangedEventArgs : EventArgs
        {
            public CswNbtView OldView;
            public CswNbtView NewView;

            public ViewChangedEventArgs()
            {
            }
            public ViewChangedEventArgs( CswNbtView OldViewParam, CswNbtView NewViewParam )
            {
                OldView = OldViewParam;
                NewView = NewViewParam;
            }
        }

        public delegate void ViewChangedEventHandler( object sender, ViewChangedEventArgs e );
        public event ViewChangedEventHandler ViewChanged = null;

        void FilterButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswNbtView OldView = new CswNbtView( _CswNbtResources );
                if( _View != null )
                {
                    OldView.LoadXml( _View.ToXml() );
                    _resetViewFromForm();
                }
                else
                {
                    // Make a view with the search parameters
                    _View = new CswNbtView( _CswNbtResources );
                    _View.ViewMode = NbtViewRenderingMode.List;

                    CswPropertyFilter CswPropertyFilter = _Filters[0];

                    CswNbtViewRelationship SearchRelationship;
                    if( CswPropertyFilter.SelectedNodeTypeLatestVersion != null )
                    {
                        _View.ViewName = CswPropertyFilter.SelectedNodeTypeLatestVersion.NodeTypeName + " Search";
                        SearchRelationship = _View.AddViewRelationship( CswPropertyFilter.SelectedNodeTypeLatestVersion, true );
                    }
                    else
                    {
                        _View.ViewName = "All " + CswPropertyFilter.SelectedObjectClass.ObjectClass.ToString() + " Search";
                        SearchRelationship = _View.AddViewRelationship( CswPropertyFilter.SelectedObjectClass, true );
                    }

                    CswNbtViewProperty SearchProperty;
                    if( CswPropertyFilter.SelectedNodeTypePropFirstVersionId != Int32.MinValue )
                    {
                        SearchProperty = _View.AddViewProperty( SearchRelationship, _CswNbtResources.MetaData.getNodeTypeProp( CswPropertyFilter.SelectedPropLatestVersion.PropId ) );
                        CswNbtViewPropertyFilter SearchPropFilter = _View.AddViewPropertyFilter( SearchProperty, CswPropertyFilter.SelectedSubField.Name, CswPropertyFilter.SelectedFilterMode, CswPropertyFilter.FilterValue.ToString(), false );
                    }
                    //else
                    //{
                    //    SearchProperty = _View.AddViewProperty( SearchRelationship, _CswNbtResources.MetaData.getObjectClassProp( CswPropertyFilter.SelectedObjectClassPropId ) );
                    //}

                }

                if( ViewChanged != null )
                    ViewChanged( this, new ViewChangedEventArgs( OldView, _View ) );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        #endregion Events

        private void _clearFilters()
        {
            _FilterItemTable.Controls.Clear();  // ...Beware!  If there are view state problems, this is probably why.
            _Filters = new Collection<CswPropertyFilter>();
            _FilterCount = 0;
        }

        private Int32 _FilterCount = 0;
        private void _initFromView()
        {
            EnsureChildControls();

            _clearFilters();

            foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships )
                _initFromViewRecursive( Relationship );

            //CswAutoTable ButtonTable = new CswAutoTable();
            //_FilterItemTable.addControl( _FilterCount + 1, 0, ButtonTable );
            //ButtonTable.addControl( 0, 0, _AdvancedLink );
            //ButtonTable.addControl( 0, 0, new CswLiteralNbsp() );
            //ButtonTable.addControl( 0, 1, _ShowAdvancedHF );
            //ButtonTable.addControl( 0, 2, _FilterButton );

            _placeButtons();
        }
        private void _initFromViewRecursive( CswNbtViewRelationship Relationship )
        {
            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    makeFilter( //_FilterCount, 
                                Filter );
                    _FilterCount++;
                }
            }

            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                _initFromViewRecursive( ChildRelationship );
        }

        private Collection<CswPropertyFilter> _Filters;
        protected void makeFilter( //Int32 CurrentFilterRow, 
                                   CswNbtViewPropertyFilter CswNbtViewPropertyFilter )
        {
            CswPropertyFilter Filter = new CswPropertyFilter( _CswNbtResources, _AjaxMgr, CswNbtViewPropertyFilter, false, false, false, ( _Filters.Count > 0 ) );
            Filter.OnError += new CswErrorHandler( HandleError );
            _FilterItemTable.Rows.Add( Filter ); // addControl( CurrentFilterRow, 0, Filter );
            _Filters.Add( Filter );
            //Filter.SetFromView( CswNbtViewPropertyFilter );
        }


        private void _resetViewFromForm()
        {
            foreach( CswNbtViewRelationship Relationship in _View.Root.ChildRelationships )
                _resetViewFromFormRecursive( Relationship );
        }
        private void _resetViewFromFormRecursive( CswNbtViewRelationship Relationship )
        {
            foreach( CswNbtViewProperty Prop in Relationship.Properties )
            {
                foreach( CswNbtViewPropertyFilter Filter in Prop.Filters )
                {
                    foreach( TableRow CurrentRow in _FilterItemTable.Rows )
                    {
                        //if( CurrentRow.Cells.Count > 0 &&
                        //    CurrentRow.Cells[0] != null &&
                        //    CurrentRow.Cells[0].Controls[0] != null &&
                        //    CurrentRow.Cells[0].Controls[0] is CswPropertyFilter )
                        //{
                        //    CswPropertyFilter PropFilter = (CswPropertyFilter) CurrentRow.Cells[0].Controls[0];
                        if( CurrentRow is CswPropertyFilter )
                        {
                            CswPropertyFilter PropFilter = (CswPropertyFilter) CurrentRow;
                            if( PropFilter.ArbitraryId == Filter.ArbitraryId )
                            {
                                Filter.SubfieldName = PropFilter.SelectedSubField.Name;
                                Filter.FilterMode = PropFilter.SelectedFilterMode;
                                Filter.Value = PropFilter.FilterValue.ToString();
                            }
                        }

                    }//iterate filter item table

                }//iterate prop filters

            }//iterate properties

            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
                _resetViewFromFormRecursive( ChildRelationship );
        }

    } // class CswViewFilterEditer

}

