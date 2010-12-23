using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_NewView : System.Web.UI.Page
    {
        private Int32 SessionViewId;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                SessionViewId = Int32.MinValue;
                if( Request.QueryString["sessionviewid"] != null && CswTools.IsInteger( Request.QueryString["sessionviewid"] ) )
                {
                    SessionViewId = CswConvert.ToInt32( Request.QueryString["sessionviewid"] );
                }

                _CopyList.Visible = false;
                _NewViewCopyLabel.Visible = false;

                if( SessionViewId == Int32.MinValue )
                {
                    _CopyList.Items.Clear();
                    _CopyList.Items.Add( new ListItem( "[New Blank View]", "" ) );

                    DataTable Views = null;
                    if( Master.CswNbtResources.CurrentNbtUser.IsAdministrator() )
                        Views = Master.CswNbtResources.ViewSelect.getAllViews();
                    else
                        Views = Master.CswNbtResources.ViewSelect.getVisibleViews( false );

                    if( Views.Rows.Count > 0 )
                    {
                        _CopyList.Visible = true;
                        _NewViewCopyLabel.Visible = true;

                        foreach( DataRow Row in Views.Rows )
                        {
                            string ViewListItemName = Row["viewname"].ToString();
                            if( Master.CswNbtResources.CurrentNbtUser.IsAdministrator() )
                            {
                                switch( (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), Row["visibility"].ToString() ) )
                                {
                                    case NbtViewVisibility.User:
                                        ViewListItemName += " (User: " + Row["username"].ToString() + ")";
                                        break;
                                    case NbtViewVisibility.Role:
                                        ViewListItemName += " (Role: " + Row["rolename"].ToString() + ")";
                                        break;
                                    case NbtViewVisibility.Global:
                                        ViewListItemName += " (Global)";
                                        break;
                                }
                            }
                            _CopyList.Items.Add( new ListItem( ViewListItemName, Row["nodeviewid"].ToString() ) );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }
        protected override void OnLoad( EventArgs e )
        {
            try
            {
                if( !Master.CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    ViewVisibilityEditor._VisibilityDropDown.Items.FindByValue( NbtViewVisibility.User.ToString() ).Selected = true;
                    ViewVisibilityEditor._VisibilityUserDropDown.Items.FindByValue( Master.CswNbtResources.CurrentUser.UserId.PrimaryKey.ToString() ).Selected = true;
                    _NewViewVisibilityLabel.Visible = false;
                    ViewVisibilityEditor.Style.Add( HtmlTextWriterStyle.Display, "none" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnLoad( e );
        }

        private TextBox _NewViewNameBox = null;
        private Label _NewViewNameLabel = null;
        private Label _NewViewCopyLabel = null;
        private Label _NewViewVisibilityLabel = null;
        private DropDownList _CopyList = null;
        private Label _ModeLabel = null;
        private DropDownList _ModeDropDown = null;
        private CswViewVisibilityEditor ViewVisibilityEditor;

        protected override void CreateChildControls()
        {

            HtmlGenericControl HeaderDivOfDialog = new HtmlGenericControl( "div" );
            HeaderDivOfDialog.ID = "HeaderDivOfDialog";
            HeaderDivOfDialog.Attributes.Add( "class", "DialogTitle" );
            ph.Controls.Add( HeaderDivOfDialog );

            HtmlGenericControl MainDivContentOfDialog = new HtmlGenericControl( "div" );
            MainDivContentOfDialog.ID = "MainDivContentOfDialog";
            MainDivContentOfDialog.Attributes.Add( "class", "DialogText" );
            ph.Controls.Add( MainDivContentOfDialog );

            // New View Dialog - Inner Controls
            CswAutoTable ManagerControlsTable = new CswAutoTable();
            ManagerControlsTable.FirstCellRightAlign = true;
            ManagerControlsTable.ID = "ManagerControlsTable";
            MainDivContentOfDialog.Controls.Add( ManagerControlsTable );

            _NewViewNameLabel = new Label();
            _NewViewNameLabel.ID = "NewViewNameLabel";
            _NewViewNameLabel.Text = "Name:";

            _NewViewNameBox = new TextBox();
            _NewViewNameBox.ID = "NewViewNameBox";
            _NewViewNameBox.CssClass = "textinput";

            _ModeLabel = new Label();
            _ModeLabel.Text = "Display Mode:";

            _ModeDropDown = new DropDownList();
            _ModeDropDown.ID = "mode";
            _ModeDropDown.CssClass = "selectinput";
            _ModeDropDown.Items.Add( new ListItem( "List", NbtViewRenderingMode.List.ToString() ) );
            _ModeDropDown.Items.Add( new ListItem( "Tree", NbtViewRenderingMode.Tree.ToString() ) );
            _ModeDropDown.Items.Add( new ListItem( "Grid", NbtViewRenderingMode.Grid.ToString() ) );

            _NewViewVisibilityLabel = new Label();
            _NewViewVisibilityLabel.ID = "NewViewVisibilityLabel";
            _NewViewVisibilityLabel.Text = "Available to:";

            ViewVisibilityEditor = new CswViewVisibilityEditor( Master.CswNbtResources );
            ViewVisibilityEditor.ID = "ViewVisibilityEditor";


            _NewViewCopyLabel = new Label();
            _NewViewCopyLabel.ID = "NewViewCopyLabel";
            _NewViewCopyLabel.Text = "Copy View:";

            _CopyList = new DropDownList();
            _CopyList.ID = "CopyList";
            _CopyList.CssClass = "selectinput";

            Button CreateButton = new Button();
            CreateButton.ID = "CreateButton";
            CreateButton.CssClass = "Button";
            CreateButton.Text = "Create";
            CreateButton.Click += CreateButton_Click;

            Button CancelButton = new Button();
            CancelButton.ID = "CancelButton";
            CancelButton.CssClass = "Button";
            CancelButton.Text = "Cancel";
            CancelButton.OnClientClick = "Popup_Cancel_Clicked();";

            //Assign controls to rows and columns
            ManagerControlsTable.addControl( 0, 0, _NewViewNameLabel );
            ManagerControlsTable.addControl( 0, 1, _NewViewNameBox );
            ManagerControlsTable.addControl( 1, 0, _ModeLabel );
            ManagerControlsTable.addControl( 1, 1, _ModeDropDown );
            ManagerControlsTable.addControl( 2, 0, _NewViewVisibilityLabel );
            ManagerControlsTable.addControl( 2, 1, ViewVisibilityEditor );
            ManagerControlsTable.addControl( 3, 0, _NewViewCopyLabel );
            ManagerControlsTable.addControl( 3, 1, _CopyList );
            ManagerControlsTable.addControl( 4, 1, CreateButton );
            ManagerControlsTable.addControl( 4, 1, CancelButton );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
        }

        protected void CreateButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswNbtView View = new CswNbtView( Master.CswNbtResources );
                if( SessionViewId != Int32.MinValue )
                {
                    CswNbtView ViewToCopy = Master.CswNbtResources.ViewCache.getView( SessionViewId ) as CswNbtView;
                    View.makeNew( _NewViewNameBox.Text,
                                  ViewVisibilityEditor.SelectedVisibility,
                                  ViewVisibilityEditor.SelectedRoleId,
                                  ViewVisibilityEditor.SelectedUserId,
                                  ViewToCopy
                                 );

                }
                else
                {
                    Int32 CopyViewId = Int32.MinValue;
                    if( CswTools.IsInteger( _CopyList.SelectedValue ) )
                        CopyViewId = CswConvert.ToInt32( _CopyList.SelectedValue );
                    View.makeNew( _NewViewNameBox.Text,
                                  ViewVisibilityEditor.SelectedVisibility,
                                  ViewVisibilityEditor.SelectedRoleId,
                                  ViewVisibilityEditor.SelectedUserId,
                                  CopyViewId
                                 );
                }
                View.SetViewMode( (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), _ModeDropDown.SelectedValue ) );
                View.save();

                string JSAction = "?viewid=" + View.ViewId;

                if( Master.CswNbtResources.CurrentNbtUser.IsAdministrator() &&
                    ( ViewVisibilityEditor.SelectedVisibility == NbtViewVisibility.Role &&
                      ViewVisibilityEditor.SelectedRoleId != Master.CswNbtResources.CurrentUser.RoleId ) ||
                    ( ViewVisibilityEditor.SelectedVisibility == NbtViewVisibility.User &&
                      ViewVisibilityEditor.SelectedUserId != Master.CswNbtResources.CurrentUser.UserId ) )
                {
                    JSAction += "&sa=1";
                }

                string JS = @"<script language=""Javascript"">Popup_OK_Clicked('" + JSAction + "');</script>";
                ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }
    }
}
