using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class Main : System.Web.UI.Page
    {
        #region Private Variables

        private RadTreeView _MainTreeView;
        private CswNodesGrid _MainGrid;
        private CswNodesList _MainList;

        //private CswViewFilterEditor _MainFilterEditor;
        private HtmlGenericControl _InvisiDiv;
        private Button _MainAjaxTrigger;
        private Button _MainNodeTreeAjaxTrigger;
        private RadTextBox _SelectedNodeKeyBox;
        private CswPropertyTable PropTable;
        private HiddenField _SessionViewid;
        private CheckBox _MultiEditCheck;
        private LinkButton _CheckAllLink;

        private CswMainMenu _MainMenu { get { return Master.MainMenu; } }

        private CswNbtNodeKey _SelectedNodeKey = null;
        private CswNbtNodeKey SelectedNodeKey
        {
            get
            {
                EnsureChildControls();

                if( _SelectedNodeKeyBox.Text == string.Empty && Session["Main_SelectedNodeKey"] != null && Session["Main_SelectedNodeKey"].ToString() != string.Empty )
                {
                    _SelectedNodeKeyBox.Text = Session["Main_SelectedNodeKey"].ToString();
                }
                if( _SelectedNodeKeyBox.Text != string.Empty )
                {
                    if( _SelectedNodeKey == null || _SelectedNodeKeyBox.Text != _SelectedNodeKey.ToString() )
                        _SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, _SelectedNodeKeyBox.Text );
                }
                else
                {
                    _SelectedNodeKey = null;
                }
                return _SelectedNodeKey;
            }
            set
            {
                EnsureChildControls();
                _SelectedNodeKey = value;
                if( value != null )
                {
                    _SelectedNodeKeyBox.Text = value.ToString();
                    Session["Main_SelectedNodeKey"] = value.ToString();
                    if( _ViewMode == NbtViewRenderingMode.Tree )
                    {
                        RadTreeNode Node = _findTreeNodeByNodeKey( _MainTreeView, SelectedNodeKey );
                        if( Node != null )
                            Node.Selected = true;
                    }
                    else if( _ViewMode == NbtViewRenderingMode.Grid )
                    {
                        //GridDataItem Row = _findGridRowByNodeKey(_MainGrid.Grid, SelectedNodeKey);
                        //if (Row != null)
                        //    Row.Selected = true;
                        SelectedNodeKey = null;
                    }
                    else if( _ViewMode == NbtViewRenderingMode.List )
                    {
                        _MainList.SelectedNodeKey = SelectedNodeKey;
                    }
                }
                else
                {
                    _SelectedNodeKeyBox.Text = string.Empty;
                    Session["Main_SelectedNodeKey"] = string.Empty;
                    if( _ViewMode == NbtViewRenderingMode.Tree )
                    {
                        RadTreeNode Node = _MainTreeView.SelectedNode;
                        if( Node != null )
                            Node.Selected = false;
                    }
                    else if( _ViewMode == NbtViewRenderingMode.Grid && _MainGrid.Grid.SelectedItems.Count > 0 )
                    {
                        GridItem Row = _MainGrid.Grid.SelectedItems[0];
                        if( Row != null )
                            Row.Selected = false;
                    }
                    else if( _ViewMode == NbtViewRenderingMode.List )
                    {
                        _MainList.SelectedNodeKey = null;
                    }
                }
            }
        }

        private string SelectedTabId
        {
            get
            {
                string ret = string.Empty;
                EnsureChildControls();
                if( PropTable != null && PropTable.TabStrip.SelectedTab != null )
                    ret = PropTable.TabStrip.SelectedTab.Value;
                else if( Session["Main_SelectedTabId"] != null && Session["Main_SelectedTabId"].ToString() != string.Empty )
                    ret = Session["Main_SelectedTabId"].ToString();
                return ret;
            }
            set
            {
                EnsureChildControls();
                foreach( RadTab Tab in PropTable.TabStrip.Tabs )
                {
                    if( Tab.Value == value )
                    {
                        Tab.Selected = true;
                        break;
                    }
                }
            }
        }

        private NbtViewRenderingMode _ViewMode
        {
            get
            {
                NbtViewRenderingMode ret = NbtViewRenderingMode.Tree;
                if( Master.CswNbtView != null )
                    ret = Master.CswNbtView.ViewMode;
                return ret;
            }
        }

        #endregion Private Variables

        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                Master.Redirect( "Main.html?clear=y" );

                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnInit( e );
        }

        protected override void CreateChildControls()
        {
            rightph.EnableViewState = false;

            //_MainMenu = new CswMainMenu( Master.CswNbtResources );
            //_MainMenu.ID = "mainmenu";
            //_MainMenu.OnError += new CswErrorHandler( Master.HandleError );
            _MainMenu.AllowAdd = true;
            _MainMenu.AllowDelete = true;
            _MainMenu.AllowEditView = true;
            _MainMenu.AllowPrintLabel = true;

            //_NavTable = new CswAutoTable();
            //_NavTable.ID = "navtable";
            //_NavTable.CssClass = "navtable";
            //LeftDiv.Controls.Add( _NavTable );

            //TableCell MenuCell = _NavTable.getCell( 0, 0 );
            //MenuCell.CssClass = "MenuCell";
            //MenuCell.Controls.Add( _MainMenu );

            //_MainFilterEditor = new CswViewFilterEditor(Master.CswNbtResources, Master.AjaxManager);
            //_MainFilterEditor.ID = "mainfiltereditor";
            //_MainFilterEditor.OnError += new CswErrorHandler(Master.HandleError);
            //_MainFilterEditor.ViewChanged += new CswViewFilterEditor.ViewChangedEventHandler( _MainFilterEditor_ViewChanged );


            Control PropTableParent = null;
            HtmlGenericControl NavDiv = new HtmlGenericControl( "div" );

            if( _ViewMode == NbtViewRenderingMode.Tree )
            {
                NavDiv.Attributes.Add( "class", "treediv" );

                CswAutoTable LeftTable = new CswAutoTable();
                leftph.Controls.Add( LeftTable );

                //LeftTable.addControl( 0, 0, _MainMenu );
                //LeftTable.addControl( 1, 0, _MainFilterEditor );
                LeftTable.addControl( 2, 0, NavDiv );

                _MainMenu.NbtViewRenderingMode = NbtViewRenderingMode.Tree;
                _MainMenu.AllowPrint = false;

                _CheckAllLink = new LinkButton();
                _CheckAllLink.ID = "CheckAllLink";
                _CheckAllLink.Text = "Check All";
                _CheckAllLink.Visible = false;
                NavDiv.Controls.Add( _CheckAllLink );

                _MainTreeView = new RadTreeView();
                _MainTreeView.ID = "MainTreeView";
                _MainTreeView.Height = 350;
                _MainTreeView.Width = 270;
                _MainTreeView.WebServiceSettings.Method = "GetNodeChildren";
                _MainTreeView.WebServiceSettings.Path = "TreeViewService.asmx";
                _MainTreeView.EnableEmbeddedSkins = false;
                _MainTreeView.Skin = "ChemSW";
                _MainTreeView.OnClientNodePopulating = "MainNodeTree_OnNodePopulating";
                _MainTreeView.OnClientNodeClicked = "MainNodeTree_OnNodeClicked";
                _MainTreeView.OnClientNodeClicking = "MainNodeTree_OnNodeClicking";
                _MainTreeView.OnClientNodePopulated = "MainNodeTree_OnNodePopulated";
                _MainTreeView.OnClientNodeChecking = "MainNodeTree_OnNodeChecking";
                NavDiv.Controls.Add( _MainTreeView );

                PropTableParent = (Control) rightph;
            }
            else if( _ViewMode == NbtViewRenderingMode.Grid )
            {
                CswAutoTable CenterTable = new CswAutoTable();
                centerph.Controls.Add( CenterTable );

                //CenterTable.addControl( 0, 0, _MainMenu );
                //CenterTable.addControl( 1, 0, _MainFilterEditor );

                NavDiv.Attributes.Add( "class", "GridDiv" );
                CenterTable.addControl( 2, 0, NavDiv );

                _MainMenu.NbtViewRenderingMode = NbtViewRenderingMode.Grid;
                _MainMenu.AllowPrint = true;

                _MainGrid = new CswNodesGrid( Master.CswNbtResources );
                _MainGrid.OnError += new CswErrorHandler( Master.HandleError );
                //_MainGrid.Grid.ClientSettings.Selecting.AllowRowSelect = true;
                //_MainGrid.Grid.ClientSettings.ClientEvents.OnRowClick = "MainGrid_RowClick";
                _MainGrid.ShowActionColumns = true;
                NavDiv.Controls.Add( _MainGrid );

                NavDiv.Controls.Add( new CswLiteralBr() );

                PropTableParent = null; // (Control) CenterTable.getCell( 3, 0 );
            }
            else if( _ViewMode == NbtViewRenderingMode.List )
            {
                NavDiv.Attributes.Add( "class", "treediv" );

                CswAutoTable LeftTable = new CswAutoTable();
                leftph.Controls.Add( LeftTable );

                //LeftTable.addControl( 0, 0, _MainMenu );
                //LeftTable.addControl( 1, 0, _MainFilterEditor );
                LeftTable.addControl( 2, 0, NavDiv );

                _MainMenu.NbtViewRenderingMode = NbtViewRenderingMode.List;
                _MainMenu.AllowPrint = false;

                _MainList = new CswNodesList( Master.CswNbtResources );
                _MainList.ID = "MainList";
                _MainList.EnableViewState = false;
                _MainList.OnError += new CswErrorHandler( Master.HandleError );
                _MainList.ClientClickFunctionName = "MainList_OnClick";
                NavDiv.Controls.Add( _MainList );

                PropTableParent = (Control) rightph;
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Invalid View", "Unhandled View Rendering Mode: " + _ViewMode.ToString() );
            }

            if( PropTableParent != null )
            {
                PropTable = new CswPropertyTable( Master.CswNbtResources, Master.AjaxManager );
                PropTable.ID = "proptable";
                PropTable.OnError += new CswErrorHandler( Master.HandleError );
                PropTable.TabStrip.TabClick += new RadTabStripEventHandler( _MainTabStrip_TabClick );
                PropTable.SaveButton.Click += new EventHandler( SaveButton_Click );
                //PropTable.OnHandleSearchSetup += new CswPropertyTable.SearchSetupHandler( Master.HandleSearch );
                //PropTable.OnSearch += new CswPropertyTable.SearchHandler( PropTable_OnSearch );
                //PropTable.OnViewClick += new CswPropertyTable.ViewClickHandler( PropTable_OnViewClick );
                PropTableParent.Controls.Add( PropTable );
            }

            _InvisiDiv = new HtmlGenericControl( "div" );
            _InvisiDiv.ID = "invisidiv";
            _InvisiDiv.Attributes.Add( "style", "display: none;" );
            NavDiv.Controls.Add( _InvisiDiv );

            _SessionViewid = new HiddenField();
            _SessionViewid.ID = "SessionViewId";
            _InvisiDiv.Controls.Add( _SessionViewid );

            _MainAjaxTrigger = new Button();
            _MainAjaxTrigger.ID = "MainAjaxTrigger";
            _MainAjaxTrigger.CssClass = "button";
            _MainAjaxTrigger.Text = "AjaxGo!";
            _MainAjaxTrigger.Click += new EventHandler( _MainAjaxTrigger_Click );
            _MainAjaxTrigger.OnClientClick = "if(!MainAjaxTrigger_PreClick()) return false;";
            _InvisiDiv.Controls.Add( _MainAjaxTrigger );

            _MainNodeTreeAjaxTrigger = new Button();
            _MainNodeTreeAjaxTrigger.ID = "MainNodeTreeAjaxTrigger";
            _MainNodeTreeAjaxTrigger.CssClass = "button";
            _MainNodeTreeAjaxTrigger.Text = "TreeAjaxGo!";
            _MainNodeTreeAjaxTrigger.Click += new EventHandler( _MainNodeTreeAjaxTrigger_Click );
            _MainNodeTreeAjaxTrigger.OnClientClick = "if(!MainNodeTreeAjaxTrigger_PreClick()) return false;";
            _InvisiDiv.Controls.Add( _MainNodeTreeAjaxTrigger );

            _SelectedNodeKeyBox = new RadTextBox();
            _SelectedNodeKeyBox.ID = "SelectedNodeKeyBox";
            _SelectedNodeKeyBox.Attributes.Add( "visibility", "hidden" );
            _InvisiDiv.Controls.Add( _SelectedNodeKeyBox );

            _MultiEditCheck = new CheckBox();
            _MultiEditCheck.ID = "MultiEditCheck";
            _MultiEditCheck.Text = "Enable Multi-Edit";
            _MultiEditCheck.AutoPostBack = true;
            _MultiEditCheck.CheckedChanged += new EventHandler( _MultiEditCheck_CheckedChanged );
            //_MultiEditCheck.Style.Add( HtmlTextWriterStyle.Display, "none" );
            _InvisiDiv.Controls.Add( _MultiEditCheck );

            base.CreateChildControls();
        } // CreateChildControls()

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                CswTimer OnLoadTimer = new CswTimer();
                Master.CswNbtResources.logTimerResult( "Main.OnLoad() started", OnLoadTimer.ElapsedDurationInSecondsAsString );

                CswTimer thisTimer = new CswTimer();
                initNavigator( !Page.IsPostBack );
                //initNavigator( true );  // BZ 8117
                Master.CswNbtResources.logTimerResult( "Main.OnLoad().initNavigator()", thisTimer.ElapsedDurationInSecondsAsString );
                thisTimer = new CswTimer();
                initPropTable( true );
                Master.CswNbtResources.logTimerResult( "Main.OnLoad().initPropTable()", thisTimer.ElapsedDurationInSecondsAsString );
                initMainMenu();

                // This caused BZ 10044
                //if( _ViewMode == NbtViewRenderingMode.Tree )
                //    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, _MainTreeView );
                //else if( _ViewMode == NbtViewRenderingMode.Grid )
                //    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, _MainGrid );

                // but we need this one for BZ 10230
                if( _ViewMode == NbtViewRenderingMode.List )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, _MainList );

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, rightph );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, centerph );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, _MainMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, _SelectedNodeKeyBox );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, Master.ErrorBox );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainAjaxTrigger, ContextSensitivePlaceHolder );

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, rightph );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, centerph );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, _MainMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, _SelectedNodeKeyBox );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, ContextSensitivePlaceHolder );
                if( _ViewMode == NbtViewRenderingMode.Tree )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, _MainTreeView );
                else if( _ViewMode == NbtViewRenderingMode.List )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, _MainList );
                else if( _ViewMode == NbtViewRenderingMode.Grid )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, _MainGrid );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _MainNodeTreeAjaxTrigger, Master.ErrorBox );

                if( PropTable != null && PropTable.SaveButton != null )
                {
                    Master.AjaxManager.ClientEvents.OnResponseEnd = "MainAjaxManager_OnResponseEnd";
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, rightph );
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, centerph );
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, _MainMenu );
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, _SelectedNodeKeyBox );
                    if( _ViewMode == NbtViewRenderingMode.Tree )
                        Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, _MainTreeView );
                    else if( _ViewMode == NbtViewRenderingMode.List )
                        Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, _MainList );
                    else if( _ViewMode == NbtViewRenderingMode.Grid )
                        Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, _MainGrid );
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, Master.ErrorBox );
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.SaveButton, ContextSensitivePlaceHolder );

                    // BZ 7549
                    if( PropTable.TabStrip != null )
                    {
                        Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.TabStrip, rightph );
                        Master.AjaxManager.AjaxSettings.AddAjaxSetting( PropTable.TabStrip, centerph );
                    }
                }

                Master.CswNbtResources.logTimerResult( "Main.OnLoad() finished", OnLoadTimer.ElapsedDurationInSecondsAsString );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnLoad( e );
        } // OnLoad()

        private ICswNbtTree CswNbtTree;

        private void initNavigator( bool ReloadTree )
        {
            if( _ViewMode == NbtViewRenderingMode.Tree )
            {
                CswNbtNodeKey PriorSelectedNodeKey = null;
                if( _MainTreeView.SelectedNode != null )
                    PriorSelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, _MainTreeView.SelectedNode.Value );

                bool RestoreChecked = true;
                if( SelectedNodeKey != PriorSelectedNodeKey )
                    RestoreChecked = false;

                // Get View
                if( Master.CswNbtView != null )
                {
                    CswNbtTree = Master.CswNbtResources.Trees.getTreeFromView( Master.CswNbtResources.CurrentNbtUser, Master.CswNbtView, true, false, false );
                    //_MainFilterEditor.LoadView( Master.CswNbtView );
                    _MainMenu.View = Master.CswNbtView;
                }
                else
                    throw ( new CswDniException( ErrorType.Error, "Invalid View", "Unable to build tree without a View" ) );

                // BZ 10045 - This was a bad idea...
                //if( CswNbtTree.getChildNodeCount() == 0 )
                //{
                //    Master.Redirect( "Search.aspx?msg=nomatch" );
                //}

                _SessionViewid.Value = _MainMenu.View.SessionViewId.ToString();

                if( ReloadTree )
                {
                    ArrayList CheckedNodes = new ArrayList();
                    if( RestoreChecked )
                    {
                        // Cache the checked nodes
                        foreach( RadTreeNode CheckedNode in _MainTreeView.CheckedNodes )
                            CheckedNodes.Add( CheckedNode.Value );
                    }

                    // Load tree
                    // BROKEN BY case 24709
                    //string XmlStr = CswNbtTree.getTreeAsXml();
                    //_MainTreeView.LoadXml( XmlStr );

                    if( RestoreChecked )
                    {
                        // Restore checked nodes
                        foreach( string CheckedNodeValue in CheckedNodes )
                        {
                            RadTreeNode NodeToCheck = _MainTreeView.FindNodeByValue( CheckedNodeValue );
                            if( NodeToCheck != null )
                                NodeToCheck.Checked = true;
                        }
                    }
                }

                // Set selected node
                if( SelectedNodeKey != null )
                {
                    RadTreeNode Node = _findTreeNodeByNodeKey( _MainTreeView, SelectedNodeKey );
                    if( Node != null )
                        Node.Selected = true;
                }

                if( _MainTreeView.SelectedNode != null )
                {
                    // BZ 10061 - Do this only if the SelectedNodeKey was set when the tree was populated
                    _MainTreeView.SelectedNode.Expanded = true;
                }

                RadTreeNode NodeToSelect = null;
                if( _MainTreeView.SelectedNode == null )
                {
                    if( _MainTreeView.Nodes[0] != null )
                    {
                        // BZ 8128 - Root is default now
                        // BZ 10026 - Not anymore
                        if( _MainTreeView.Nodes[0].Nodes.Count > 0 )
                        {
                            NodeToSelect = _MainTreeView.Nodes[0].Nodes[0];
                        }
                        else
                        {
                            NodeToSelect = _MainTreeView.Nodes[0];
                        }
                    }
                }
                if( NodeToSelect != null )
                {
                    SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, NodeToSelect.Value );
                    //Node.Selected = true;
                }

                //_setTreeCheckable( _MainTreeView.Nodes, SelectedNodeKey );

                _MainMenu.AllowExport = false;
                _MainMenu.SelectedNodeKey = SelectedNodeKey;
                _MainMenu.ParentNodeKey = SelectedNodeKey;
            } // if( _ViewMode == NbtViewRenderingMode.Tree )
            else if( _ViewMode == NbtViewRenderingMode.Grid )
            {
                _MainGrid.View = Master.CswNbtView;
                _MainMenu.View = Master.CswNbtView;
                _MainGrid.DataBind();

                CswNbtTree = _MainGrid.CswNbtTree;
                //_MainFilterEditor.LoadView( Master.CswNbtView );

                //if( SelectedNodeKey != null )
                //{
                //    GridDataItem Row = _findGridRowByNodeKey( _MainGrid.Grid, SelectedNodeKey );
                //    if( Row != null )
                //        Row.Selected = true;
                //}

                //if( _MainGrid.Grid.SelectedValue == null )
                //{
                SelectedNodeKey = null;
                if( _MainGrid.Grid.Items.Count > 0 )
                {
                    GridDataItem Row = _MainGrid.Grid.Items[0];
                    if( Row != null )
                    {
                        SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, Row.GetDataKeyValue( "NodeKey" ).ToString() );
                    }
                }
                else
                {
                    //PropTable.Visible = false;
                    _MainMenu.AllowExport = false;
                    _MainMenu.AllowPrint = false;
                }
                //}
                _MainMenu.AllowCopy = false;
                _MainMenu.AllowDelete = false;
                _MainMenu.SelectedNodeKey = SelectedNodeKey;

                // BZ 10040
                //_MainMenu.ParentNodeKey = null;
                _MainGrid.CswNbtTree.goToRoot();
                _MainMenu.ParentNodeKey = _MainGrid.CswNbtTree.getNodeKeyForCurrentPosition();

                if( CswNbtTree != null && SelectedNodeKey != null )
                {
                    CswNbtTree.makeNodeCurrent( SelectedNodeKey );
                    if( !CswNbtTree.isCurrentPositionRoot() )
                    {
                        CswNbtTree.goToParentNode();
                        _MainMenu.ParentNodeKey = CswNbtTree.getNodeKeyForCurrentPosition();
                    }
                }
            } // else if( _ViewMode == NbtViewRenderingMode.Grid )
            else if( _ViewMode == NbtViewRenderingMode.List )
            {
                // Get View
                if( Master.CswNbtView != null )
                {
                    CswNbtTree = Master.CswNbtResources.Trees.getTreeFromView(
                        RunAsUser: Master.CswNbtResources.CurrentNbtUser,
                        View: Master.CswNbtView,
                        RequireViewPermissions: true,
                        IncludeSystemNodes: false,
                        IncludeHiddenNodes: false );

                    //// Select Root by default
                    //if( SelectedNodeKey == null )
                    //    SelectedNodeKey = CswNbtTree.getNodeKeyForCurrentPosition();

                    if( SelectedNodeKey != null )
                    {
                        // See BZ 10110
                        //CswNbtNode TestNode = CswNbtTree.getNode( SelectedNodeKey );
                        //if( TestNode == null ) 
                        //    SelectedNodeKey = null;

                        CswNbtTree.makeNodeCurrent( SelectedNodeKey );
                        if( !CswNbtTree.isCurrentNodeDefined() || SelectedNodeKey.NodeSpecies == NodeSpecies.Root )
                            SelectedNodeKey = null;
                        CswNbtTree.goToRoot();
                    }

                    // BZ 10026 - Select first node by default
                    if( SelectedNodeKey == null )
                    {
                        if( CswNbtTree.getChildNodeCount() > 0 )
                        {
                            // First node
                            CswNbtTree.goToNthChild( 0 );
                            SelectedNodeKey = CswNbtTree.getNodeKeyForCurrentPosition();
                        }
                        else
                        {
                            // Root
                            SelectedNodeKey = CswNbtTree.getNodeKeyForCurrentPosition();
                        }
                    }

                    _MainMenu.View = Master.CswNbtView;
                    _MainMenu.AllowExport = false;
                    _MainMenu.SelectedNodeKey = SelectedNodeKey;
                    _MainMenu.ParentNodeKey = SelectedNodeKey;

                    _MainList.View = Master.CswNbtView;
                    _MainList.ForceReload = ReloadTree;
                    _MainList.SelectedNodeKey = SelectedNodeKey;
                    _MainList.DataBind();

                } // if( Master.CswNbtView != null )
                else
                    throw ( new CswDniException( ErrorType.Error, "Invalid View", "Unable to build list without a View" ) );


            } // else if( _ViewMode == NbtViewRenderingMode.List )
        } // initNavigator()

        //// Set checkable to nodes of the same nodetype as the selected node
        //private void _setTreeCheckable(RadTreeNodeCollection Nodes, CswNbtNodeKey SelectedNodeKey)
        //{
        //    foreach(RadTreeNode TreeNode in Nodes)
        //    {
        //        CswNbtNodeKey ThisKey = new CswNbtNodeKey(Master.CswNbtResources, TreeNode.Value);
        //        TreeNode.Checkable = (ThisKey.NodeTypeId == SelectedNodeKey.NodeTypeId);
        //        _setTreeCheckable( TreeNode.Nodes, SelectedNodeKey );
        //    }
        //}



        private void initPropTable( bool ReloadTable )
        {
            if( PropTable != null )
            {
                if( CswNbtTree != null && SelectedNodeKey != null &&
                    ( SelectedNodeKey.NodeSpecies != NodeSpecies.Plain || CswNbtTree.getNode( SelectedNodeKey ) != null ) )
                {
                    CswNbtNode Node = CswNbtTree.getNode( SelectedNodeKey );
                    //PropTable.SelectedNodeKey = SelectedNodeKey;
                    if( Node != null )
                    {
                        PropTable.SelectedNode = Node;
                        //PropTable.SelectedNodeTypeId = Node.NodeTypeId;
                        PropTable.SelectedTabId = SelectedTabId;
                    }
                    else if( _MainTreeView != null )
                    {
                        PropTable.SelectedNodeSpecies = SelectedNodeKey.NodeSpecies;
                        RadTreeNode SelectedNode = _findTreeNodeByNodeKey( _MainTreeView, SelectedNodeKey );
                        if( SelectedNode != null )
                        {
                            PropTable.SelectedNodeText = SelectedNode.Text;
                        }
                    }
                    else if( _MainList != null )
                    {
                        PropTable.SelectedNodeSpecies = SelectedNodeKey.NodeSpecies;
                        PropTable.SelectedNodeText = Master.CswNbtView.ViewName;
                    }
                }
                PropTable.View = Master.CswNbtView;

                if( _ViewMode == NbtViewRenderingMode.Tree )
                {
                    PropTable.BatchMode = _MultiEditCheck.Checked;
                    ArrayList CheckedKeys = new ArrayList();
                    foreach( RadTreeNode TreeNode in _MainTreeView.CheckedNodes )
                    {
                        CheckedKeys.Add( new CswNbtNodeKey( Master.CswNbtResources, TreeNode.Value ) );
                    }
                    PropTable.CheckedKeys = CheckedKeys;
                }
                else
                {
                    PropTable.BatchMode = false;
                }

                if( ReloadTable && SelectedNodeKey != null )
                {
                    PropTable.initTabStrip();
                    SelectedTabId = PropTable.initTabContents();
                }
            } // if( PropTable != null )
        } // initPropTable()

        private void initMainMenu()
        {
            _MainMenu.AllowMobile = true;
            if( _ViewMode == NbtViewRenderingMode.Tree )
            {
                _MainMenu.AllowBatch = true;
                _MainMenu.BatchEnabled = _MultiEditCheck.Checked;
                //if( _MultiEditCheck.Checked )
                //{
                //    string CheckedNodeIds = string.Empty;
                //    foreach( RadTreeNode TreeNode in _MainTreeView.CheckedNodes )
                //    {
                //        CswNbtNodeKey NodeKey = new CswNbtNodeKey( Master.CswNbtResources, TreeNode.Value );
                //        if( CheckedNodeIds != string.Empty ) CheckedNodeIds += ",";
                //        CheckedNodeIds += NodeKey.NodeId.PrimaryKey.ToString();
                //    }
                //    _MainMenu.CheckedNodeIds = CheckedNodeIds;
                //}
            }
            else
            {
                _MainMenu.AllowBatch = false;
                _MainMenu.BatchEnabled = false;
            }

            _MainMenu.DataBind();
        } // initMainMenu()

        //protected void _MainFilterEditor_ViewChanged(object sender, CswViewFilterEditor.ViewChangedEventArgs e)
        //{
        //    //bz # 6629
        //    Master.HandleModifyViewFilters( e.OldView, e.NewView );
        //    Master.setViewXml(e.NewView.ToString(), true);
        //    Master.Redirect("Main.aspx");
        //}

        void SaveButton_Click( object sender, EventArgs e )
        {
            //Master.HandleSaveNode();

            if( !_MultiEditCheck.Checked )
            {
                initNavigator( true );
                initPropTable( true );
            }
            initMainMenu();
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                SetupContextSensitiveHelp();

                if( _MainTreeView != null && _MainTreeView.SelectedNode != null )
                    _MainTreeView.SelectedNode.ExpandParentNodes();

                if( SelectedNodeKey != null )
                {
                    Session["Main_SelectedNodeKey"] = SelectedNodeKey.ToString();
                }

                Session["Main_SelectedTabId"] = SelectedTabId;

                if( _CheckAllLink != null )
                {
                    if( _MultiEditCheck.Checked )
                    {
                        _CheckAllLink.Visible = true;
                        _CheckAllLink.OnClientClick = "Main_SetCheckAll('" + _MainTreeView.ClientID + "', '" + _CheckAllLink.ClientID + "'); return false;";
                    }
                    else
                    {
                        _CheckAllLink.Visible = false;
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnPreRender( e );
        }


        #endregion Page Lifecycle

        #region Events

        public void HandleMultiModeEnabled( CswNbtView View )
        {
            Master.HandleMultiModeEnabled( View );
        }

        void _MultiEditCheck_CheckedChanged( object sender, EventArgs e )
        {
            try
            {
                if( _MultiEditCheck.Checked )
                {
                    _MainTreeView.CheckBoxes = true;
                    HandleMultiModeEnabled( Master.CswNbtView );
                }
                else
                {
                    _MainTreeView.CheckBoxes = false;
                    initNavigator( true );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _MainTabStrip_TabClick( object sender, RadTabStripEventArgs e )
        {
            try
            {
                PropTable.SelectedTabId = e.Tab.Value;
                SelectedTabId = PropTable.initTabContents();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }

        //void _MainTreeView_NodeClick(object sender, RadTreeNodeEventArgs e)
        //{
        //}

        //void ListLink_Click( object sender, EventArgs e )
        //{
        //    //SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, ( (LinkButton) sender ).ID );
        //    CswPrimaryKey SelectedPk = new CswPrimaryKey();
        //    SelectedPk.FromString( ( (LinkButton) sender ).ID );
        //    SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, CswNbtTree.Key, string.Empty, SelectedPk, NodeSpecies.Plain, Int32.MinValue, Int32.MinValue, string.Empty, string.Empty );
        //}


        void _MainAjaxTrigger_Click( object sender, EventArgs e )
        {
            try
            {
                initPropTable( true );
                initMainMenu();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }
        void _MainNodeTreeAjaxTrigger_Click( object sender, EventArgs e )
        {
            try
            {
                initNavigator( true );
                initPropTable( true );
                initMainMenu();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        //void PropTable_OnSearch( CswNbtView SearchView )
        //{
        //    Master.setViewXml( SearchView.ToString() );
        //    Master.GoMain();
        //}
        //void PropTable_OnViewClick( Int32 ViewId )
        //{
        //    Master.setViewId( ViewId );
        //    Master.GoMain();
        //}



        #endregion Events

        #region Helpers

        class CswMainTreeNodeTemplate : ITemplate
        {
            public void InstantiateIn( Control Container )
            {
                CswAutoTable NodeTable = new CswAutoTable();
                TableCell LeftCell = NodeTable.getCell( 0, 0 );
                TableCell RightCell = NodeTable.getCell( 0, 1 );
                Image Icon = new Image();

            }
        } // class CswMainTreeNodeTemplate

        private RadTreeNode _findTreeNodeByNodeKey( RadTreeView Tree, CswNbtNodeKey NodeKey )
        {
            RadTreeNode ret = null;
            foreach( RadTreeNode ChildNode in Tree.Nodes )
            {
                if( ret == null )
                    ret = _findTreeNodeByNodeKeyRecursive( ChildNode, NodeKey );
                else
                    break;
            }
            return ret;
        }
        private RadTreeNode _findTreeNodeByNodeKeyRecursive( RadTreeNode TreeNode, CswNbtNodeKey NodeKey )
        {
            RadTreeNode ret = null;
            CswNbtNodeKey TreeNodeKey = new CswNbtNodeKey( Master.CswNbtResources, TreeNode.Value );
            if( TreeNodeKey == NodeKey )
            {
                ret = TreeNode;
            }
            else
            {
                foreach( RadTreeNode ChildNode in TreeNode.Nodes )
                {
                    if( ret == null )
                        ret = _findTreeNodeByNodeKeyRecursive( ChildNode, NodeKey );
                    else
                        break;
                }
            }
            return ret;
        }

        private GridDataItem _findGridRowByNodeKey( RadGrid Grid, CswNbtNodeKey NodeKey )
        {
            GridDataItem ret = null;
            foreach( GridDataItem Row in Grid.Items )
            {
                if( Row.GetDataKeyValue( "NodeKey" ).ToString() == NodeKey.ToString() )
                {
                    ret = Row;
                }
            }
            return ret;
        }

        private void SetupContextSensitiveHelp()
        {
            // We're not really ready for this yet

            //CswMessageBox ContentBox = new CswMessageBox();
            //ContentBox.ID = "ContextSensitiveContentBox";
            //ContentBox.DivCssClass = "ContextSensitiveContentDiv";
            //ContentBox.MessageCssClass = "ContextSensitiveMessage";
            //ContentBox.UseTitles = false;
            //ContextSensitivePlaceHolder.Controls.Add( ContentBox );

            //CswSessionsListEntry SessionsListEntry = Master.CswSession.SessionsListEntry;
            //if( SessionsListEntry.Stats_count_multiedit == 0 ) 
            //{
            //    foreach( string NodeTypeId in SessionsListEntry.NodeTypesSaved.Keys )
            //    {
            //        if( CswConvert.ToInt32( SessionsListEntry.NodeTypesSaved[NodeTypeId] ) > 10 )
            //        {
            //            CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            //            ContentBox.addMessage( "Multi1", "Have you considered using Multi-Edit to save changes to multiple " + NodeType.NodeTypeName + " nodes at once?" );
            //            break;
            //        }
            //    }
            //}

        } // SetupContextSensitiveHelp()

        #endregion Helpers

    } // class Main
} // namespace ChemSW.Nbt.WebPages
