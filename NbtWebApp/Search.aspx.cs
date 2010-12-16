using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Search : System.Web.UI.Page
    {

        private CswNbtView _View;

        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                if( Request.QueryString["msg"] == "nomatch" )
                {
                    //Master.ErrorBox.addMessage( "No matching results found", "Search returned 0 results" );
                    _SearchResultsLabel.Text = "No matching results found";
                }

                _View = Master.CswNbtView;

                if( !_View.IsSearchable() )
                {
                    //_CswPropertyFilter.Visible = false;
                    //_SearchButton.Visible = false;
                    //_FilterEditor.Visible = true;
                    //_SearchLabel.Visible = true;
                    CswNbtMetaDataObjectClass SearchOC = null;
                                    
                    if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.IMCS ) )
                    {
                        SearchOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
                    }
                    else if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.FE ) )
                    {
                        SearchOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass );
                    }
                    else if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
                    {
                        SearchOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                    }
                    else if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.BioSafety ) )
                    {
                        SearchOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BiologicalClass );
                    }
                    else if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.SI ) )
                    {
                        SearchOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                    }

                    if( null != SearchOC )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in SearchOC.NodeTypes )
                        {
                            _FilterEditor.LoadSearch( NodeType, null );
                        }
                    }
                    else
                    {
                        _FilterEditor.LoadSearch( null, null );
                    }

                    _SearchLabel.Text = "Search";
                }
                else
                {
                    //_FilterEditor.Visible = false;
                    //_SearchLabel.Visible = false;
                    //_CswPropertyFilter.Visible = true;
                    //_SearchButton.Visible = true;


                    if( _View.Visibility == NbtViewVisibility.Property &&
                        _View.ViewMode == NbtViewRenderingMode.Grid &&
                        Request.QueryString["nodeid"] != null && Request.QueryString["nodeid"] != string.Empty )
                    {
                        // Special handling for grid properties:
                        // Since the Property Grid views have a parent relationship that normal grid views do not, 
                        // we have to add the root relationship as a property filter to the second relationship 
                        // instead of using NodeIdsToFilterIn

                        CswNbtViewRelationship ParentRelationship = _View.Root.ChildRelationships[0];
                        CswNbtViewRelationship GridRowRelationship = ParentRelationship.ChildRelationships[0];
                        CswNbtViewRelationship.RelatedIdType SecondType = GridRowRelationship.SecondType;
                        Int32 SecondId = GridRowRelationship.SecondId;
                        CswNbtViewRelationship.PropIdType RelationshipPropType = GridRowRelationship.PropType;
                        Int32 RelationshipPropId = GridRowRelationship.PropId;

                        GridRowRelationship.overrideFirst( (CswNbtMetaDataNodeType) null );
                        _View.Root.removeChildRelationship( ParentRelationship );
                        _View.Root.addChildRelationship( GridRowRelationship );

                        //_View = new CswNbtView( Master.CswNbtResources );
                        //CswNbtViewRelationship NewRel = null;
                        //if( OldSecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
                        //    NewRel = _View.AddViewRelationship( Master.CswNbtResources.MetaData.getObjectClass( OldSecondId ) );
                        //else if( OldSecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                        //    NewRel = _View.AddViewRelationship( Master.CswNbtResources.MetaData.getNodeType( OldSecondId ) );

                        CswNbtViewProperty NewRelationshipProp = null;
                        if( RelationshipPropType == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
                            NewRelationshipProp = _View.AddViewProperty( GridRowRelationship, Master.CswNbtResources.MetaData.getObjectClassProp( RelationshipPropId ) );
                        else if( RelationshipPropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                            NewRelationshipProp = _View.AddViewProperty( GridRowRelationship, Master.CswNbtResources.MetaData.getNodeTypeProp( RelationshipPropId ) );

                        CswPrimaryKey ParentNodeId = new CswPrimaryKey();
                        ParentNodeId.FromString( Request.QueryString["nodeid"] );

                        _View.AddViewPropertyFilter( NewRelationshipProp,
                                                     CswNbtSubField.SubFieldName.NodeID,
                                                     CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                     ParentNodeId.PrimaryKey.ToString(),
                                                     false );
                    }

                    _FilterEditor.LoadView( _View );
                    _SearchLabel.Text = _View.ViewName;
                }
                //    Master.Redirect( "SearchCustom.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        private CswViewFilterEditor _FilterEditor;
        private Label _SearchLabel;
        private LinkButton _NewCustomLink;
        private DropDownList _LoadDropDown;
        private Button _LoadButton;
        private Button _DeleteButton;
        private Label _SearchResultsLabel;
        //private CswPropertyFilter _CswPropertyFilter;
        //private CswAutoTable _PropFilterTable;
        //private Button _SearchButton;

        protected override void CreateChildControls()
        {
            try
            {
                CswCenteredDiv CenteredDiv = new CswCenteredDiv();
                centerph.Controls.Add( CenteredDiv );

                _SearchLabel = new Label();
                _SearchLabel.CssClass = "SearchLabel";
                CenteredDiv.Controls.Add( _SearchLabel );

                CenteredDiv.Controls.Add( new CswLiteralBr() );

                _FilterEditor = new CswViewFilterEditor( Master.CswNbtResources, Master.AjaxManager );
                _FilterEditor.ViewChanged += new CswViewFilterEditor.ViewChangedEventHandler( _FilterEditor_ViewChanged );
                _FilterEditor.OnError += new CswErrorHandler( Master.HandleError );
                CenteredDiv.Controls.Add( _FilterEditor );

                //// Default search for IMCS
                //_PropFilterTable = new CswAutoTable();
                //CenteredDiv.Controls.Add( _PropFilterTable );

                //if( Master.CswNbtResources.MetaData != null ) // login page
                //{
                //    CswNbtMetaDataNodeType EquipmentNodeType = Master.CswNbtResources.MetaData.getNodeType( "Equipment" );
                //    if( EquipmentNodeType != null && EquipmentNodeType.BarcodeProperty != null )
                //        _CswPropertyFilter = new CswPropertyFilter( Master.CswNbtResources, Master.AjaxManager, EquipmentNodeType.NodeTypeId, EquipmentNodeType.BarcodeProperty.PropId, true, true, true, false );
                //}
                //else
                //{
                //    _CswPropertyFilter = new CswPropertyFilter( Master.CswNbtResources, Master.AjaxManager, null, true, true, true, false );
                //}
                //_CswPropertyFilter.OnError += new CswErrorHandler( Master.HandleError );
                //_CswPropertyFilter.ID = "SearchPropFilter";
                //_CswPropertyFilter.UseCheckChanges = false;
                //_PropFilterTable.addControl( 0, 0, _CswPropertyFilter );

                //_SearchButton = new Button();
                //_SearchButton.ID = "SearchButton";
                //_SearchButton.Text = "Search";
                //_SearchButton.CssClass = "Button";
                //_SearchButton.Click += new EventHandler( SearchButton_Click );
                //TableCell SearchButtonCell = _PropFilterTable.getCell( 1, 0 );
                //SearchButtonCell.ColumnSpan = 10;
                //SearchButtonCell.HorizontalAlign = HorizontalAlign.Right;
                //SearchButtonCell.Controls.Add( _SearchButton );

                _SearchResultsLabel = new Label();
                _SearchResultsLabel.CssClass = "ErrorContent";
                CenteredDiv.Controls.Add( _SearchResultsLabel );

                CenteredDiv.Controls.Add( new CswLiteralBr() );
                CenteredDiv.Controls.Add( new CswLiteralBr() );
                CenteredDiv.Controls.Add( new CswLiteralBr() );
                CenteredDiv.Controls.Add( new CswLiteralBr() );

                Literal LoadLiteral = new Literal();
                LoadLiteral.Text = "Load a Search:&nbsp";
                CenteredDiv.Controls.Add( LoadLiteral );

                _LoadDropDown = new DropDownList();
                _LoadDropDown.ID = "LoadDropDown";
                _initLoadSearch();
                //_LoadDropDown.SelectedIndexChanged += new EventHandler( _LoadDropDown_SelectedIndexChanged );
                //_LoadDropDown.AutoPostBack = true;
                CenteredDiv.Controls.Add( _LoadDropDown );

                CenteredDiv.Controls.Add( new CswLiteralNbsp() );

                _LoadButton = new Button();
                _LoadButton.ID = "LoadButton";
                _LoadButton.Text = "Load";
                _LoadButton.Click += new EventHandler( _LoadButton_Click );
                CenteredDiv.Controls.Add( _LoadButton );

                CenteredDiv.Controls.Add( new CswLiteralNbsp() );

                _DeleteButton = new Button();
                _DeleteButton.ID = "DeleteButton";
                _DeleteButton.Text = "Delete";
                _DeleteButton.Click += new EventHandler( _DeleteButton_Click );
                CenteredDiv.Controls.Add( _DeleteButton );


                CenteredDiv.Controls.Add( new CswLiteralBr() );
                CenteredDiv.Controls.Add( new CswLiteralBr() );

                _NewCustomLink = new LinkButton();
                _NewCustomLink.ID = "NewCustomLink";
                _NewCustomLink.Text = "New Custom Search";
                _NewCustomLink.Click += new EventHandler( _NewCustomLink_Click );
                CenteredDiv.Controls.Add( _NewCustomLink );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.CreateChildControls();
        }


        protected override void OnPreRender( EventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events

        void _NewCustomLink_Click( object sender, EventArgs e )
        {
            try
            {
                Master.Redirect( "SearchCustom.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _FilterEditor_ViewChanged( object sender, CswViewFilterEditor.ViewChangedEventArgs e )
        {
            try
            {
                Master.setViewXml( e.NewView.ToString() );
                Master.Redirect( "Main.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        //protected void SearchButton_Click( object sender, EventArgs e )
        //{
        //    try
        //    {
        //        // Make a view with the search parameters
        //        CswNbtView SearchView = new CswNbtView( Master.CswNbtResources );
        //        SearchView.ViewMode = NbtViewRenderingMode.List;

        //        CswNbtViewRelationship SearchRelationship;
        //        if( _CswPropertyFilter.SelectedNodeTypeLatestVersion != null )
        //        {
        //            SearchView.ViewName = _CswPropertyFilter.SelectedNodeTypeLatestVersion.NodeTypeName + " Search";
        //            SearchRelationship = SearchView.AddViewRelationship( _CswPropertyFilter.SelectedNodeTypeLatestVersion );
        //        }
        //        else
        //        {
        //            SearchView.ViewName = "All " + _CswPropertyFilter.SelectedObjectClass.ObjectClass.ToString() + " Search";
        //            SearchRelationship = SearchView.AddViewRelationship( _CswPropertyFilter.SelectedObjectClass );
        //        }

        //        CswNbtViewProperty SearchProperty;
        //        if( _CswPropertyFilter.SelectedNodeTypePropFirstVersionId != Int32.MinValue )
        //        {
        //            SearchProperty = SearchView.AddViewProperty( SearchRelationship, Master.CswNbtResources.MetaData.getNodeTypeProp( _CswPropertyFilter.SelectedPropLatestVersion.PropId ) );
        //        }
        //        else
        //        {
        //            SearchProperty = SearchView.AddViewProperty( SearchRelationship, Master.CswNbtResources.MetaData.getObjectClassProp( _CswPropertyFilter.SelectedObjectClassPropId ) );
        //        }

        //        CswNbtViewPropertyFilter SearchPropFilter = SearchView.AddViewPropertyFilter( SearchProperty, _CswPropertyFilter.SelectedSubField.Name, _CswPropertyFilter.SelectedFilterMode, _CswPropertyFilter.FilterValue.ToString(), false );

        //        Master.setViewXml( SearchView.ToString() );
        //        Master.HandleSearch( SearchProperty );
        //        Master.Redirect( "Main.aspx" );
        //    }
        //    catch( Exception ex )
        //    {
        //        Master.HandleError( ex );
        //    }
        //}

        //void _LoadDropDown_SelectedIndexChanged( object sender, EventArgs e )
        //{
        //    Master.setViewId( Convert.ToInt32( _LoadDropDown.SelectedValue ) );
        //    Master.Redirect( "Search.aspx" );
        //}

        void _DeleteButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( CswTools.IsInteger( _LoadDropDown.SelectedValue ) )
                {
                    CswNbtView DoomedView = CswNbtViewFactory.restoreView( Master.CswNbtResources, Convert.ToInt32( _LoadDropDown.SelectedValue ) );
                    if( DoomedView.Visibility == NbtViewVisibility.User )
                        DoomedView.Delete();
                    else
                        throw new CswDniException( "Cannot Delete View: " + DoomedView.ViewName, "User attempted to delete a non-User view: " + DoomedView.ViewId.ToString() );
                    _initLoadSearch();
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _LoadButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( CswTools.IsInteger( _LoadDropDown.SelectedValue ) )
                {
                    Master.setViewId( Convert.ToInt32( _LoadDropDown.SelectedValue ) );
                    Master.Redirect( "Search.aspx" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        #endregion Events

        private void _initLoadSearch()
        {
            DataTable OriginalViewsTable = Master.CswNbtResources.ViewSelect.getVisibleViews( false );
            DataTable ViewsTable = OriginalViewsTable.Copy();
            Collection<DataRow> RowsToRemove = new Collection<DataRow>();
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                // Searchable?
                if( !ViewRow["viewxml"].ToString().Contains( "<Filter" ) )
                {
                    RowsToRemove.Add( ViewRow );
                }
            }
            foreach( DataRow DoomedRow in RowsToRemove )
                DoomedRow.Delete();

            ViewsTable.Rows.InsertAt( ViewsTable.NewRow(), 0 );

            _LoadDropDown.DataSource = ViewsTable;
            _LoadDropDown.DataTextField = "viewname";
            _LoadDropDown.DataValueField = "nodeviewid";
            _LoadDropDown.DataBind();
        }



    } // class Search
} // namespace ChemSW.Nbt.WebPages
