using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Core;
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
                    _SearchResultsLabel.Text = "No matching results found";
                }
                else
                {
                    _SearchResultsLabel.Visible = false;
                }

                _View = Master.CswNbtView;

                if( !_View.IsSearchable() )
                {
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
                } // if-else( !_View.IsSearchable() )
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
                _FilterEditor.OnFilterClear += new CswViewFilterEditor.FilterClearHandler( _FilterEditor_OnFilterClear );
                CenteredDiv.Controls.Add( _FilterEditor );

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

        void _FilterEditor_OnFilterClear()
        {
            _SearchLabel.Text = "Search";
        }

        void _DeleteButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( CswTools.IsInteger( _LoadDropDown.SelectedValue ) )
                {
                    CswNbtView DoomedView = CswNbtViewFactory.restoreView( Master.CswNbtResources, CswConvert.ToInt32( _LoadDropDown.SelectedValue ) );
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
                    Master.setViewId( CswConvert.ToInt32( _LoadDropDown.SelectedValue ) );
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
