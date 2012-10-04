using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// Represents a table of components for the Welcome page
    /// </summary>
    public class CswWelcomeTable : CompositeControl
    {
        /// <summary>
        /// Folder Path for Button Images
        /// </summary>
        public static string IconImageRoot = "Images/biggerbuttons";

        #region Private Variables

        /// <summary>
        /// Types of Welcome Page Components
        /// </summary>
        public enum WelcomeComponentType
        {
            /// <summary>
            /// Link to a View, Report, or Action
            /// </summary>
            Link,
            /// <summary>
            /// Search on a View
            /// </summary>
            Search,
            /// <summary>
            /// Static text
            /// </summary>
            Text,
            /// <summary>
            /// Add link for new node
            /// </summary>
            Add
        }
        //public enum WelcomeForType
        //{
        //    User,
        //    Role,
        //    Global
        //}

        private CswNbtResources _CswNbtResources;
        private RadAjaxManager _AjaxManager;
        //private PlaceHolder _ComponentPlaceHolder;
        private CswLayoutTable _LayoutTable;
        private LinkButton _ResetButton;
        private Button _AddedButton;

        private string ViewLinkPrefix = "ViewLink_";
        private string SearchButtonPrefix = "SearchButton_";
        private string SearchLinkPrefix = "Search_";
        private string AddLinkPrefix = "AddLink_";
        private string ButtonLinkPrefix = "ButtonLink_";
        private string AddButtonLinkPrefix = "AddButton_";

        private CswPrimaryKey SelectedRoleId
        {
            get
            {
                CswPrimaryKey ret = null;
                if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    ret = new CswPrimaryKey();
                    ret.FromString( _RoleHiddenField.Value );
                }
                else
                    ret = _CswNbtResources.CurrentNbtUser.RoleId;
                return ret;
            }
        }

        #endregion Private Variables

        #region Lifecycle

        /// <summary>
        /// Constructor
        /// </summary>
        public CswWelcomeTable( CswNbtResources CswNbtResources, RadAjaxManager AjaxManager )
        {
            _CswNbtResources = CswNbtResources;
            _AjaxManager = AjaxManager;
        }

        /// <summary>
        /// Init
        /// </summary>
        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();

                if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    //ICswNbtTree RolesTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass );
                    //for( Int32 r = 0; r < RolesTree.getChildNodeCount(); r++ )
                    //{
                    //    RolesTree.goToNthChild( r );
                    CswNbtMetaDataObjectClass RoleOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass );
                    foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, false ) )
                    {
                        //_RoleSelect.Items.Add( new ListItem( RolesTree.getNodeNameForCurrentPosition(), RolesTree.getNodeIdForCurrentPosition().ToString() ) );
                        _RoleSelect.Items.Add( new ListItem( RoleNode.NodeName, RoleNode.NodeId.ToString() ) );
                        //RolesTree.goToParentNode();
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnInit( e );
        } // OnInit()

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                _initWelcomeComponents();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnLoad( e );
        }

        private CswImageButton _ConfigButton;
        private CswImageButton _AddButton;
        private DropDownList _RoleSelect;
        private CswLiteralText _RoleLiteral;
        private HiddenField _RoleHiddenField;

        /// <summary>
        /// CreateChildControls
        /// </summary>
        protected override void CreateChildControls()
        {
            HtmlGenericControl WelcomeContentDiv = new HtmlGenericControl( "div" );
            WelcomeContentDiv.Attributes.Add( "class", "WelcomeDiv" );
            this.Controls.Add( WelcomeContentDiv );

            CswAutoTable TopTable = new CswAutoTable();
            TopTable.Width = Unit.Parse( "99%" );
            WelcomeContentDiv.Controls.Add( TopTable );

            CswAutoTable ButtonTable = new CswAutoTable();
            ButtonTable.Style.Add( HtmlTextWriterStyle.PaddingBottom, "5px" );
            TopTable.addControl( 0, 0, ButtonTable );
            TopTable.getCell( 0, 0 ).HorizontalAlign = HorizontalAlign.Right;
            //TopTable.getCell( 0, 0 ).Style.Add( HtmlTextWriterStyle.TextAlign, "right" );
            TopTable.getCell( 0, 0 ).Style.Add( HtmlTextWriterStyle.Width, "100%" );

            _RoleLiteral = new CswLiteralText( "Role: " );
            ButtonTable.addControl( 0, 0, _RoleLiteral );

            _RoleSelect = new DropDownList();
            _RoleSelect.ID = "RoleSelect";
            _RoleSelect.SelectedIndexChanged += new EventHandler( _RoleSelect_SelectedIndexChanged );
            _RoleSelect.AutoPostBack = true;
            ButtonTable.addControl( 0, 0, _RoleSelect );

            _RoleHiddenField = new HiddenField();
            _RoleHiddenField.ID = "RoleHiddenField";
            _RoleHiddenField.Value = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            ButtonTable.addControl( 0, 0, _RoleHiddenField );

            ButtonTable.addControl( 0, 1, new CswLiteralNbsp( 5 ) );

            _ResetButton = new LinkButton();
            _ResetButton.ID = "ResetButton";
            _ResetButton.Text = "Reset to Default";
            _ResetButton.CssClass = "ResetButton";
            _ResetButton.Click += new EventHandler( ResetButton_Click );
            ButtonTable.addControl( 0, 2, _ResetButton );

            ButtonTable.addControl( 0, 3, new CswLiteralNbsp( 5 ) );

            _ConfigButton = new CswImageButton( CswImageButton.ButtonType.Configure );
            _ConfigButton.Click += new EventHandler( ConfigButton_Click );
            ButtonTable.addControl( 0, 4, _ConfigButton );

            _AddButton = new CswImageButton( CswImageButton.ButtonType.Add );
            ButtonTable.addControl( 0, 5, _AddButton );

            _LayoutTable = new CswLayoutTable( _CswNbtResources, _AjaxManager );
            _LayoutTable.CssClass = "CswLayoutTable";
            _LayoutTable.OnError += new CswErrorHandler( this.HandleError );
            _LayoutTable.OnMoveComponent += new CswLayoutTable.MoveComponentEventHandler( _LayoutTable_OnMoveComponent );
            _LayoutTable.OnDeleteComponent += new CswLayoutTable.DeleteComponentEventHandler( _LayoutTable_OnDeleteComponent );
            TopTable.addControl( 1, 0, _LayoutTable );

            HtmlGenericControl HiddenDiv = new HtmlGenericControl( "div" );
            HiddenDiv.Style.Add( HtmlTextWriterStyle.Display, "none" );
            this.Controls.Add( HiddenDiv );

            _AddedButton = new Button();
            _AddedButton.ID = "AddedButton";
            _AddedButton.Click += new EventHandler( _AddedButton_Click );
            HiddenDiv.Controls.Add( _AddedButton );

            base.CreateChildControls();
        } // CreateChildControls()

        /// <summary>
        /// Prerender
        /// </summary>
        protected override void OnPreRender( EventArgs e )
        {
            _RoleSelect.Visible = false;
            _RoleLiteral.Visible = false;
            _ResetButton.Visible = false;

            if( _LayoutTable.EditMode )
            {
                _ResetButton.Visible = true;
            }

            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() &&
                ( _LayoutTable.EditMode || SelectedRoleId != _CswNbtResources.CurrentNbtUser.RoleId ) )
            {
                _RoleSelect.Visible = true;
                _RoleLiteral.Visible = true;
            }

            if( _CswNbtResources.Permit.can( CswNbtActionName.Design ) )
            {
                _ConfigButton.Visible = true;
                _AddButton.Visible = true;
            }
            else
            {
                _ConfigButton.Visible = false;
                _AddButton.Visible = false;
            }

            _ResetButton.OnClientClick = "return confirm(\"Are you sure you want to reset the content of this page?\");";
            _AddButton.OnClientClick = "WelcomeAddComponentDialog_openPopup(document.getElementById('" + _RoleSelect.ClientID + "')); return false;";


            base.OnPreRender( e );

        } // OnPreRender()

        #endregion Page Lifecycle


        #region Events


        protected void _RoleSelect_SelectedIndexChanged( object sender, EventArgs e )
        {
            _RoleHiddenField.Value = _RoleSelect.SelectedValue;
            _initWelcomeComponents();
        }


        /// <summary>
        /// Delegate for View link handling
        /// </summary>
        public delegate void ViewLinkClickHandler( CswViewListTree.ViewType ViewType, Int32 Pk );
        /// <summary>
        /// View link click event
        /// </summary>
        public event ViewLinkClickHandler OnViewLinkClick = null;

        void ViewLink_Click( object sender, EventArgs e )
        {
            try
            {
                string[] SplitID = ( (LinkButton) sender ).ID.Split( '_' );
                CswViewListTree.ViewType ThisViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), SplitID[1] );
                Int32 ThisPk = CswConvert.ToInt32( SplitID[2] );
                if( OnViewLinkClick != null )
                    OnViewLinkClick( ThisViewType, ThisPk );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        void ButtonLink_Click( object sender, ImageClickEventArgs e )
        {
            try
            {
                string[] SplitID = ( (ImageButton) sender ).ID.Split( '_' );
                CswViewListTree.ViewType ThisViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), SplitID[1] );
                Int32 ThisPk = CswConvert.ToInt32( SplitID[2] );
                if( OnViewLinkClick != null )
                    OnViewLinkClick( ThisViewType, ThisPk );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        /// <summary>
        /// Delegate for handling searches
        /// </summary>
        public delegate void SearchClickHandler( string ViewString );
        /// <summary>
        /// Search event
        /// </summary>
        public event SearchClickHandler OnSearchClick = null;

        void SearchLink_Click( object sender, EventArgs e )
        {
            try
            {
                string[] SplitID = ( (LinkButton) sender ).ID.Split( '_' );
                //CswViewListTree.ViewType ThisViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), SplitID[1] );
                CswNbtViewId ViewId = new CswNbtViewId( CswConvert.ToInt32( SplitID[2] ) );
                CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( ViewId );
                //_setupSearchViewRecursive( _LayoutTable, ThisView.Root );
                if( OnSearchClick != null )
                    OnSearchClick( ThisView.ToString() );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }
        void SearchButtonLink_Click( object sender, EventArgs e )
        {
            try
            {
                string[] SplitID = ( (ImageButton) sender ).ID.Split( '_' );
                //CswViewListTree.ViewType ThisViewType = (CswViewListTree.ViewType) Enum.Parse( typeof( CswViewListTree.ViewType ), SplitID[1] );
                CswNbtViewId ViewId = new CswNbtViewId( CswConvert.ToInt32( SplitID[2] ) );
                CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( ViewId );
                //_setupSearchViewRecursive( _LayoutTable, ThisView.Root );
                if( OnSearchClick != null )
                    OnSearchClick( ThisView.ToString() );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        /// <summary>
        /// Delegate for handling add new 
        /// </summary>
        public delegate void AddedClickHandler();
        /// <summary>
        /// Add new event
        /// </summary>
        public event AddedClickHandler OnAddedClick = null;

        void _AddedButton_Click( object sender, EventArgs e )
        {
            if( OnAddedClick != null )
                OnAddedClick();
        }

        void ConfigButton_Click( object sender, EventArgs e )
        {
            _LayoutTable.EditMode = !_LayoutTable.EditMode;
            _LayoutTable.ReinitComponents();
        }

        void _LayoutTable_OnMoveComponent( Int32 LayoutComponentId, Int32 NewDisplayRow, Int32 NewDisplayColumn )
        {
            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateMove", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", LayoutComponentId );
            DataRow WelcomeRow = WelcomeTable.Rows[0];
            WelcomeRow["display_row"] = CswConvert.ToDbVal( NewDisplayRow );
            WelcomeRow["display_col"] = CswConvert.ToDbVal( NewDisplayColumn );
            WelcomeUpdate.update( WelcomeTable );
        }

        void _LayoutTable_OnDeleteComponent( Int32 LayoutComponentId )
        {
            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateDelete", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", LayoutComponentId );
            DataRow WelcomeRow = WelcomeTable.Rows[0];
            WelcomeRow.Delete();
            WelcomeUpdate.update( WelcomeTable );
        }



        void ResetButton_Click( object sender, EventArgs e )
        {
            _Reset( SelectedRoleId.PrimaryKey );
            _initWelcomeComponents();
        }


        /// <summary>
        /// Error handling event
        /// </summary>
        public event CswErrorHandler OnError;

        /// <summary>
        /// Error handler
        /// </summary>
        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }


        #endregion Events


        private void _Reset( Int32 RoleId )
        {
            // Reset the contents for this role to factory default
            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateReset", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "roleid", RoleId );
            for( Int32 i = 0; i < WelcomeTable.Rows.Count; i++ )
            {
                WelcomeTable.Rows[i].Delete();
            }

            CswNbtViewId EquipmentByTypeViewId = new CswNbtViewId();
            CswNbtViewId TasksOpenViewId = new CswNbtViewId();
            CswNbtViewId ProblemsOpenViewId = new CswNbtViewId();
            CswNbtViewId FindEquipmentViewId = new CswNbtViewId();

            Dictionary<CswNbtViewId, CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( false );
            foreach( CswNbtView View in Views.Values )
            {
                if( View.ViewName == "All Equipment" )
                    EquipmentByTypeViewId = View.ViewId;
                if( View.ViewName == "Tasks: Open" )
                    TasksOpenViewId = View.ViewId;
                if( View.ViewName == "Problems: Open" )
                    ProblemsOpenViewId = View.ViewId;
                if( View.ViewName == "Find Equipment" )
                    FindEquipmentViewId = View.ViewId;
            }

            Int32 ProblemNodeTypeId = Int32.MinValue;
            Int32 TaskNodeTypeId = Int32.MinValue;
            Int32 ScheduleNodeTypeId = Int32.MinValue;
            Int32 EquipmentNodeTypeId = Int32.MinValue;
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
            {
                if( NodeType.NodeTypeName == "Equipment Problem" )
                    ProblemNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment Task" )
                    TaskNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment Schedule" )
                    ScheduleNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment" )
                    EquipmentNodeTypeId = NodeType.FirstVersionNodeTypeId;
            }

            // Equipment
            if( FindEquipmentViewId.isSet() )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Search, CswViewListTree.ViewType.View, FindEquipmentViewId.get(), Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
            if( EquipmentNodeTypeId != Int32.MinValue )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
            if( EquipmentByTypeViewId.isSet() )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, EquipmentByTypeViewId.get(), Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

            // Problems
            if( ProblemsOpenViewId.isSet() )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, ProblemsOpenViewId.get(), Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
            if( ProblemNodeTypeId != Int32.MinValue )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

            // Schedules and Tasks
            if( TasksOpenViewId.isSet() )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, TasksOpenViewId.get(), Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
            if( TaskNodeTypeId != Int32.MinValue )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
            if( ScheduleNodeTypeId != Int32.MinValue )
                AddWelcomeComponent( _CswNbtResources, WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

            WelcomeUpdate.update( WelcomeTable );
        }

        /// <summary>
        /// Adds a welcome component to the welcome page
        /// </summary>
        public static void AddWelcomeComponent( CswNbtResources CswNbtResources, DataTable WelcomeTable, WelcomeComponentType ComponentType,
                                                CswViewListTree.ViewType ViewType, Int32 ViewValue,
                                                Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, Int32 RoleId )
        {
            if( Row == Int32.MinValue )
            {
                string SqlText = @"select max(display_row) maxcol
                                     from welcome
                                    where display_col = 1
                                      and (roleid = " + RoleId.ToString() + @")";
                CswArbitrarySelect WelcomeSelect = CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_WelcomeSelect", SqlText );
                DataTable WelcomeSelectTable = WelcomeSelect.getTable();
                Int32 MaxRow = 0;
                if( WelcomeSelectTable.Rows.Count > 0 )
                {
                    MaxRow = CswConvert.ToInt32( WelcomeSelectTable.Rows[0]["maxcol"] );
                    if( MaxRow < 0 ) MaxRow = 0;
                }
                Row = MaxRow + 1;
                Column = 1;
            }

            if( ButtonIcon == "blank.gif" )
                ButtonIcon = string.Empty;

            DataRow NewWelcomeRow = WelcomeTable.NewRow();
            NewWelcomeRow["roleid"] = RoleId;
            NewWelcomeRow["componenttype"] = ComponentType.ToString();
            NewWelcomeRow["display_col"] = Column;
            NewWelcomeRow["display_row"] = Row;

            switch( ComponentType )
            {
                case CswWelcomeTable.WelcomeComponentType.Add:
                    NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case CswWelcomeTable.WelcomeComponentType.Link:
                    switch( ViewType )
                    {
                        case CswViewListTree.ViewType.View:
                            NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        case CswViewListTree.ViewType.Action:
                            NewWelcomeRow["actionid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        case CswViewListTree.ViewType.Report:
                            NewWelcomeRow["reportid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        default:
                            throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Welcome Page Component" );
                    }
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case CswWelcomeTable.WelcomeComponentType.Search:
                    if( ViewType == CswViewListTree.ViewType.View )
                    {
                        NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( ViewValue );
                        NewWelcomeRow["buttonicon"] = ButtonIcon;
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Welcome Page Component" );
                    break;
                case CswWelcomeTable.WelcomeComponentType.Text:
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
            }
            WelcomeTable.Rows.Add( NewWelcomeRow );
        }

        private DataTable _getWelcomeTable( Int32 RoleId )
        {
            CswTableSelect WelcomeSelect = _CswNbtResources.makeCswTableSelect( "WelcomeSelect", "welcome" );
            string WhereClause = "where roleid = '" + RoleId.ToString() + "'";
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "welcomeid", OrderByType.Ascending ) );
            return WelcomeSelect.getTable( WhereClause, OrderBy );
        }

        private void _initWelcomeComponents()
        {
            _LayoutTable.Components.Clear();

            // Welcome components from database
            DataTable WelcomeTable = _getWelcomeTable( SelectedRoleId.PrimaryKey );

            // see BZ 10234
            if( WelcomeTable.Rows.Count == 0 && !Page.IsPostBack )
            {
                _Reset( SelectedRoleId.PrimaryKey );
                WelcomeTable = _getWelcomeTable( SelectedRoleId.PrimaryKey );
            }

            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                WelcomeComponentType ThisComponentType = (WelcomeComponentType) Enum.Parse( typeof( WelcomeComponentType ), WelcomeRow["componenttype"].ToString() );
                CswAutoTable ComponentTable = new CswAutoTable();
                ComponentTable.Width = Unit.Parse( "100%" );

                string IDSuffix = string.Empty;
                string LinkText = string.Empty;
                CswNbtView ThisView = null;
                CswNbtAction ThisAction = null;
                CswNbtNode ThisReportNode = null;
                CswNbtMetaDataNodeType NodeType = null;
                if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                {
                    ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
                    if( ThisView.IsFullyEnabled() )
                    {
                        IDSuffix += CswViewListTree.ViewType.View.ToString() + "_" + ThisView.ViewId.ToString() + "_" + WelcomeRow["welcomeid"].ToString();
                        if( WelcomeRow["displaytext"].ToString() != string.Empty )
                            LinkText = WelcomeRow["displaytext"].ToString();
                        else
                            LinkText = ThisView.ViewName;
                    }
                }
                if( CswConvert.ToInt32( WelcomeRow["actionid"] ) != Int32.MinValue )
                {
                    ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( WelcomeRow["actionid"] )];
                    if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                    {
                        IDSuffix += CswViewListTree.ViewType.Action.ToString() + "_" + ThisAction.ActionId.ToString() + "_" + WelcomeRow["welcomeid"].ToString();
                        if( WelcomeRow["displaytext"].ToString() != string.Empty )
                            LinkText = WelcomeRow["displaytext"].ToString();
                        else
                            LinkText = ThisAction.Name.ToString();
                    }
                }
                if( CswConvert.ToInt32( WelcomeRow["reportid"] ) != Int32.MinValue )
                {
                    ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( WelcomeRow["reportid"] ) )];
                    IDSuffix += CswViewListTree.ViewType.Report.ToString() + "_" + ThisReportNode.NodeId.PrimaryKey.ToString() + "_" + WelcomeRow["welcomeid"].ToString();
                    if( WelcomeRow["displaytext"].ToString() != string.Empty )
                        LinkText = WelcomeRow["displaytext"].ToString();
                    else
                        LinkText = ThisReportNode.NodeName;
                }
                if( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) != Int32.MinValue )
                {
                    NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) );
                    if( NodeType != null )
                        IDSuffix += NodeType.NodeTypeId.ToString() + "_" + WelcomeRow["welcomeid"].ToString();
                }

                if( IDSuffix != string.Empty || ThisComponentType == WelcomeComponentType.Text )
                {
                    switch( ThisComponentType )
                    {
                        case WelcomeComponentType.Link:
                            if( WelcomeRow["buttonicon"].ToString() != string.Empty )
                            {
                                ImageButton ButtonLink = new ImageButton();
                                ButtonLink.ID = ButtonLinkPrefix + IDSuffix;
                                ButtonLink.ImageUrl = IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString();
                                ButtonLink.CssClass = "WelcomeButton";
                                ButtonLink.AlternateText = LinkText;
                                ButtonLink.Click += new ImageClickEventHandler( ButtonLink_Click );
                                ComponentTable.getCell( 0, 0 ).HorizontalAlign = HorizontalAlign.Center;
                                ComponentTable.addControl( 0, 0, ButtonLink );
                            }

                            LinkButton ViewLink = new LinkButton();
                            ViewLink.ID = ViewLinkPrefix + IDSuffix;
                            ViewLink.Text = LinkText;
                            ViewLink.Click += new EventHandler( ViewLink_Click );
                            ViewLink.CssClass = "WelcomeText";
                            ComponentTable.getCell( 1, 0 ).HorizontalAlign = HorizontalAlign.Center;
                            ComponentTable.addControl( 1, 0, ViewLink );
                            break;

                        case WelcomeComponentType.Search:
                            if( WelcomeRow["buttonicon"].ToString() != string.Empty )
                            {
                                ImageButton ButtonLink = new ImageButton();
                                ButtonLink.ID = SearchButtonPrefix + IDSuffix;
                                ButtonLink.ImageUrl = IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString();
                                ButtonLink.CssClass = "WelcomeButton";
                                ButtonLink.AlternateText = LinkText;
                                ButtonLink.Click += new ImageClickEventHandler( SearchButtonLink_Click );
                                ComponentTable.getCell( 0, 0 ).HorizontalAlign = HorizontalAlign.Center;
                                ComponentTable.addControl( 0, 0, ButtonLink );
                            }

                            LinkButton SearchLink = new LinkButton();
                            SearchLink.ID = SearchLinkPrefix + IDSuffix;
                            SearchLink.Text = LinkText;
                            SearchLink.Click += new EventHandler( SearchLink_Click );
                            SearchLink.CssClass = "WelcomeText";
                            ComponentTable.getCell( 1, 0 ).HorizontalAlign = HorizontalAlign.Center;
                            ComponentTable.addControl( 1, 0, SearchLink );
                            break;

                        //case WelcomeComponentType.Search:
                        //    _setupSearchFormRecursive( ComponentTable.getCell( 0, 0 ), ThisView.Root, 1 );

                        //    Button SearchSubmitButton = new Button();
                        //    SearchSubmitButton.ID = SearchButtonPrefix + IDSuffix;
                        //    SearchSubmitButton.Text = "Search";
                        //    SearchSubmitButton.CssClass = "Button";
                        //    SearchSubmitButton.Click += new EventHandler( SearchSubmitButton_Click );

                        //    TableCell SearchSubmitCell = ComponentTable.getCell( 0, 1 );
                        //    SearchSubmitCell.Style.Add( HtmlTextWriterStyle.TextAlign, "right" );
                        //    SearchSubmitCell.Style.Add( HtmlTextWriterStyle.VerticalAlign, "bottom" );
                        //    SearchSubmitCell.Controls.Add( SearchSubmitButton );
                        //    break;

                        case WelcomeComponentType.Add:
                            if( WelcomeRow["buttonicon"].ToString() != string.Empty )
                            {
                                ImageButton AddButtonLink = new ImageButton();
                                AddButtonLink.ID = AddButtonLinkPrefix + IDSuffix;
                                AddButtonLink.ImageUrl = IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString();
                                AddButtonLink.CssClass = "WelcomeButton";
                                //AddButtonLink.Click += new ImageClickEventHandler( AddButtonLink_Click );
                                AddButtonLink.OnClientClick = "return WelcomeAddNodeDialog_openPopup('" + NodeType.NodeTypeId.ToString() + "');";
                                ComponentTable.getCell( 0, 0 ).HorizontalAlign = HorizontalAlign.Center;
                                ComponentTable.addControl( 0, 0, AddButtonLink );
                            }

                            LinkButton AddLink = new LinkButton();
                            AddLink.ID = AddLinkPrefix + IDSuffix;
                            if( WelcomeRow["displaytext"].ToString() != string.Empty )
                                AddLink.Text = WelcomeRow["displaytext"].ToString();
                            else
                                AddLink.Text = "Add New " + NodeType.NodeTypeName;
                            AddLink.CssClass = "WelcomeText";
                            //AddLink.Click += new EventHandler( AddLink_Click );
                            AddLink.OnClientClick = "return WelcomeAddNodeDialog_openPopup('" + NodeType.NodeTypeId.ToString() + "');";
                            ComponentTable.getCell( 1, 0 ).HorizontalAlign = HorizontalAlign.Center;
                            ComponentTable.addControl( 1, 0, AddLink );
                            break;

                        case WelcomeComponentType.Text:
                            Label TextLabel = new Label();
                            TextLabel.Text = WelcomeRow["displaytext"].ToString();
                            TextLabel.CssClass = "WelcomeText";
                            ComponentTable.addControl( 0, 0, TextLabel );
                            break;
                    } // switch( ThisComponentType )

                    CswLayoutTable.LayoutComponent ThisComponent = new CswLayoutTable.LayoutComponent(
                                                                        CswConvert.ToInt32( WelcomeRow["welcomeid"] ),
                                                                        CswConvert.ToInt32( WelcomeRow["display_row"] ),
                                                                        CswConvert.ToInt32( WelcomeRow["display_col"] ),
                                                                        null,
                                                                        ComponentTable,
                                                                        true );
                    _LayoutTable.Components.Add( CswConvert.ToInt32( WelcomeRow["welcomeid"] ), ThisComponent );
                } //  if( IDSuffix != string.Empty )

            } // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

            _LayoutTable.ReinitComponents();

        } // _initWelcomeComponents()


        //// Similar, but not the same as CswPropertyTable
        //private bool _setupSearchFormRecursive( TableCell SearchCell, CswNbtViewNode ViewNode, Int32 Count )
        //{
        //    bool ret = false;
        //    if( ViewNode is CswNbtViewRoot )
        //    {
        //        if( Count > 0 )
        //        {
        //            foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
        //                ret = _setupSearchFormRecursive( SearchCell, ChildRel, Count ) || ret;
        //        }
        //    }
        //    else if( ViewNode is CswNbtViewRelationship )
        //    {
        //        foreach( CswNbtViewProperty ViewProp in ( (CswNbtViewRelationship) ViewNode ).Properties )
        //        {
        //            CswPropertyFilter Searcher = new CswPropertyFilter( _CswNbtResources, _AjaxManager, ViewProp, false, false, true, false );
        //            Searcher.ID = SearchFilterPrefix + ViewProp.View.ViewId.ToString() + "_" + ViewProp.UniqueId;
        //            SearchCell.Controls.Add( Searcher );
        //            //Searcher.SetFromView( ViewProp );
        //            ret = true;
        //            Count--;
        //            if( Count <= 0 ) break;
        //        }
        //        if( Count > 0 )
        //        {
        //            foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
        //                ret = _setupSearchFormRecursive( SearchCell, ChildRel, Count ) || ret;
        //        }
        //    }
        //    return ret;
        //} // _setupSearchFormRecursive()

        //// Similar, but not the same as CswPropertyTable
        //private void _setupSearchViewRecursive( WebControl SearchPH, CswNbtViewNode ViewNode )
        //{
        //    if( ViewNode is CswNbtViewRoot )
        //    {
        //        foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
        //        {
        //            _setupSearchViewRecursive( SearchPH, ChildRel );
        //        }
        //    }
        //    else if( ViewNode is CswNbtViewRelationship )
        //    {
        //        foreach( CswNbtViewProperty ViewProp in ( (CswNbtViewRelationship) ViewNode ).Properties )
        //        {
        //            CswPropertyFilter Searcher = (CswPropertyFilter) SearchPH.FindControl( SearchFilterPrefix + ViewProp.View.ViewId.ToString() + "_" + ViewProp.UniqueId );
        //            if( Searcher != null && Searcher.HasValidFilter )
        //            {
        //                // Clear existing filters (in case this is a subsequent search)
        //                ViewProp.Filters.Clear();

        //                CswNbtViewPropertyFilter SearchPropFilter = ViewProp.View.AddViewPropertyFilter( ViewProp, Searcher.SelectedSubField.Name, Searcher.SelectedFilterMode, Searcher.FilterValue.ToString(), false );
        //                //SearchPropFilter.SubfieldName = Searcher.SelectedSubField.Name;
        //                //SearchPropFilter.FilterMode = Searcher.SelectedFilterMode;
        //                //SearchPropFilter.Value = Searcher.FilterValue.ToString();
        //                //SearchPropFilter.ArbitraryId = ViewProp.ArbitraryId + SearchPropFilter.FilterMode.ToString() + SearchPropFilter.Value.ToString();  // Important!
        //                //ViewProp.addFilter( SearchPropFilter );
        //            }
        //            //HandleSearchSetup( ViewProp );
        //        }
        //        foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
        //        {
        //            _setupSearchViewRecursive( SearchPH, ChildRel );
        //        }
        //    }
        //} // _setupSearchViewRecursive()

    } // class CswWelcomeTable
} // namespace ChemSW.NbtWebControls

