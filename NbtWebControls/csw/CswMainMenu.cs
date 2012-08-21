using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// Defines the modes that the Design page supports
    /// </summary>
    public enum NbtDesignMode
    {
        /// <summary>
        /// Allows access to design all nodetypes and properties
        /// </summary>
        Standard,
        /// <summary>
        /// Limited design mode for creation of inspection designs.
        /// </summary>
        Inspection
    }



    /// <summary>
    /// Web Control to render a Menu
    /// </summary>
    [ToolboxData( "<{0}:CswMainMenu runat=server></{0}:CswMainMenu>" )]
    public class CswMainMenu : CompositeControl, INamingContainer
    {
        /// <summary>
        /// Supported Export formats
        /// </summary>
        public enum ExportOutputFormat
        {
            CSV,
            Excel,
            PDF,
            Word,
            MobileXML,
            ReportXML
        }

        /// <summary>
        /// If true, using the Add Menu won't alter the view
        /// </summary>
        public bool AddMenuDoesntChangeView = false;
        /// <summary>
        /// If true, adding a node to the tree or grid won't change the selected node
        /// </summary>
        public bool AddMenuDoesntChangeSelectedNode = false;

        /// <summary>
        /// Enable the Add Menu
        /// </summary>
        public bool AllowAdd
        {
            get
            {
                if( ViewState["AllowAdd"] == null )
                    ViewState["AllowAdd"] = "true";
                return Convert.ToBoolean( ViewState["AllowAdd"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowAdd"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Batch Menu Item
        /// </summary>
        public bool AllowBatch
        {
            get
            {
                if( ViewState["AllowBatch"] == null )
                    ViewState["AllowBatch"] = "false";
                return Convert.ToBoolean( ViewState["AllowBatch"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowBatch"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Mobile Item
        /// </summary>
        public bool AllowMobile
        {
            get
            {
                if( ViewState["AllowMobile"] == null )
                    ViewState["AllowMobile"] = "false";
                return Convert.ToBoolean( ViewState["AllowMobile"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowMobile"] = value.ToString().ToLower();
            }
        }

        /// <summary>
        /// Set true if batch mode is already enabled
        /// </summary>
        public bool BatchEnabled
        {
            get
            {
                if( ViewState["BatchEnabled"] == null )
                    ViewState["BatchEnabled"] = "false";
                return Convert.ToBoolean( ViewState["BatchEnabled"].ToString().ToLower() );
            }
            set
            {
                ViewState["BatchEnabled"] = value.ToString().ToLower();
            }
        }

        /// <summary>
        /// Enable the Delete menu option
        /// </summary>
        ///
        private bool _AllowDelete = true;
        public bool AllowDelete
        {
            get { return ( _AllowDelete ); }
            set { _AllowDelete = value; }
        }

        public NbtViewRenderingMode NbtViewRenderingMode //bz # 7129
        {
            get
            {
                NbtViewRenderingMode ReturnVal = NbtViewRenderingMode.Unknown;
                if( null != ViewState["NbtViewRenderingMode"] )
                {
                    //ReturnVal = (NbtViewRenderingMode) Enum.Parse( typeof( NbtViewRenderingMode ), ViewState["NbtViewRenderingMode"].ToString() );
                    ReturnVal = (NbtViewRenderingMode) ViewState["NbtViewRenderingMode"].ToString();
                }

                return ( ReturnVal );
            }//get

            set
            {
                ViewState["NbtViewRenderingMode"] = value.ToString();
            }//set

        }//NbtViewRenderingMode


        /// <summary>
        /// Enable the Print menu option
        /// </summary>
        public bool AllowPrint
        {
            get
            {
                if( ViewState["AllowPrint"] == null )
                    ViewState["AllowPrint"] = "false";
                return Convert.ToBoolean( ViewState["AllowPrint"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowPrint"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Print Label option
        /// </summary>
        public bool AllowPrintLabel
        {
            get
            {
                if( ViewState["AllowPrintLabel"] == null )
                    ViewState["AllowPrintLabel"] = "true";
                return Convert.ToBoolean( ViewState["AllowPrintLabel"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowPrintLabel"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Export menu option
        /// </summary>
        public bool AllowExport
        {
            get
            {
                if( ViewState["AllowExport"] == null )
                    ViewState["AllowExport"] = "true";
                return Convert.ToBoolean( ViewState["AllowExport"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowExport"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Edit View menu option
        /// </summary>
        public bool AllowEditView
        {
            get
            {
                if( ViewState["AllowEditView"] == null )
                    ViewState["AllowEditView"] = "true";
                return Convert.ToBoolean( ViewState["AllowEditView"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowEditView"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Change View menu option
        /// </summary>
        public bool AllowChangeView
        {
            get
            {
                if( ViewState["AllowChangeView"] == null )
                    ViewState["AllowChangeView"] = "false";
                return Convert.ToBoolean( ViewState["AllowChangeView"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowChangeView"] = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// Enable the Copy menu option
        /// </summary>
        public bool AllowCopy
        {
            get
            {
                if( ViewState["AllowCopy"] == null )
                    ViewState["AllowCopy"] = "true";
                return Convert.ToBoolean( ViewState["AllowCopy"].ToString().ToLower() );
            }
            set
            {
                ViewState["AllowCopy"] = value.ToString().ToLower();
            }
        }
        ///// <summary>
        ///// Enable the Search menu option
        ///// </summary>
        //public bool AllowSearch
        //{
        //    get
        //    {
        //        if( ViewState["AllowSearch"] == null )
        //            ViewState["AllowSearch"] = "true";
        //        return Convert.ToBoolean( ViewState["AllowSearch"].ToString().ToLower() );
        //    }
        //    set
        //    {
        //        ViewState["AllowSearch"] = value.ToString().ToLower();
        //    }
        //}

        private CswNbtResources _CswNbtResources;
        /// <summary>
        /// CswNbtResources object
        /// </summary>
        public CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
            set { _CswNbtResources = value; }
        }

        private CswNbtNodeKey _ParentNodeKey;
        /// <summary>
        /// NodeKey of parent node, for Add
        /// </summary>
        public CswNbtNodeKey ParentNodeKey
        {
            get { return _ParentNodeKey; }
            set
            {
                _ParentNodeKey = value;
                _ParentNodeKeyViewNode = null;
            }
        }
        private CswNbtNodeKey _SelectedNodeKey;
        /// <summary>
        /// NodeKey of selected node, for Delete
        /// </summary>
        public CswNbtNodeKey SelectedNodeKey
        {
            get { return _SelectedNodeKey; }
            set
            {
                _SelectedNodeKey = value;
                _SelectedNodeKeyViewNode = null;
            }
        }

        private CswNbtViewNode _ParentNodeKeyViewNode = null;
        private CswNbtViewNode ParentNodeKeyViewNode
        {
            get
            {
                if( ParentNodeKey != null && _ParentNodeKeyViewNode == null )
                {
                    if( _View != null )
                    {
                        if( ParentNodeKey.ViewNodeUniqueId != string.Empty )
                            _ParentNodeKeyViewNode = ( (CswNbtView) _View ).FindViewNodeByUniqueId( ParentNodeKey.ViewNodeUniqueId );
                    }
                }
                return _ParentNodeKeyViewNode;
            }
        }
        private CswNbtViewNode _SelectedNodeKeyViewNode = null;
        private CswNbtViewNode SelectedNodeKeyViewNode
        {
            get
            {
                if( SelectedNodeKey != null && _SelectedNodeKeyViewNode == null )
                {
                    if( _View != null )
                    {
                        if( SelectedNodeKey.ViewNodeUniqueId != string.Empty )
                            _SelectedNodeKeyViewNode = ( (CswNbtView) _View ).FindViewNodeByUniqueId( SelectedNodeKey.ViewNodeUniqueId );
                    }
                }
                return _SelectedNodeKeyViewNode;
            }
        }

        /// <summary>
        /// CSS Class for the Menu Div
        /// </summary>
        public string MenuDivCssClass = "MenuDiv";
        /// <summary>
        /// CSS Class for the top Menu Group
        /// </summary>
        public string MenuGroupCssClass = "MenuGroup";
        /// <summary>
        /// CSS Class for the second Menu Group
        /// </summary>
        public string SubMenuGroupCssClass = "SubMenuGroup";

        private CswNbtView _View = null;
        /// <summary>
        /// View which rendered the grid or tree to which this menu is attached
        /// </summary>
        public CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
        }

        /// <summary>
        /// True if the menu is used in Design Mode (which changes the Add menu)
        /// </summary>
        public bool IsDesignMode = false;
        public new NbtDesignMode DesignMode = NbtDesignMode.Standard;

        /// <summary>
        /// Type of node selected, for Design Mode
        /// </summary>
        public CswNodeTypeTree.NodeTypeTreeSelectedType DesignSelectedType;

        /// <summary>
        /// Value of node selected, for Design Mode
        /// </summary>
        public string DesignSelectedValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswMainMenu( CswNbtResources Rsc )
        {
            CswNbtResources = Rsc;
            this.DataBinding += new EventHandler( CswMainMenu_DataBinding );
        }

        private CswNbtNode _Node = null;

        /// <summary>
        /// Sets up the Add menu and the Edit View menu item
        /// </summary>
        void CswMainMenu_DataBinding( object sender, EventArgs e )
        {
            try
            {
                EnsureChildControls();

                if( BatchEnabled )
                    _BatchOpsMenuItem.Text = "Single-Edit";
                else
                    _BatchOpsMenuItem.Text = "Multi-Edit";


                // Setup Export Menu
                ExportMenuItem.Visible = false;
                if( AllowExport )
                {
                    if( _View != null )
                    {
                        ExportMenuItem.Visible = true;
                        ExportMenuItem.Items.Clear();
                        if( NbtViewRenderingMode.Grid == NbtViewRenderingMode )
                        {
                            foreach( ExportOutputFormat FormatType in Enum.GetValues( typeof( ExportOutputFormat ) ) )
                            {
                                if( ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                                {
                                    _addExportMenuItem( _View, FormatType, NbtViewRenderingMode );
                                }
                            }
                        }
                        else // tree or list
                        {
                            _addExportMenuItem( _View, ExportOutputFormat.ReportXML, NbtViewRenderingMode );
                            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                                _addExportMenuItem( _View, ExportOutputFormat.MobileXML, NbtViewRenderingMode );
                        }
                    }
                    else if( IsDesignMode && _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                    {
                        ExportMenuItem.Visible = false;
                        CswNbtMetaDataNodeType NodeType = null;
                        switch( DesignSelectedType )
                        {
                            case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                                NodeType = _CswNbtResources.MetaData.getNodeType( Convert.ToInt32( DesignSelectedValue ) );
                                break;
                            case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( Convert.ToInt32( DesignSelectedValue ) );
                                NodeType = NodeTypeProp.getNodeType();
                                break;
                            case CswNodeTypeTree.NodeTypeTreeSelectedType.Tab:
                                CswNbtMetaDataNodeTypeTab NodeTypeTab = _CswNbtResources.MetaData.getNodeTypeTab( Convert.ToInt32( DesignSelectedValue ) );
                                NodeType = NodeTypeTab.getNodeType();
                                break;
                        }

                        ExportMenuItem.Visible = true;
                        ExportMenuItem.Items.Clear();
                        if( NodeType != null )
                        {

                            RadMenuItem DesignExportItem = new RadMenuItem();
                            DesignExportItem.Text = "Export NodeType XML";
                            DesignExportItem.Value = "export_" + ExportOutputFormat.MobileXML.ToString().ToLower();
                            DesignExportItem.CssClass = SubMenuGroupCssClass;
                            DesignExportItem.Attributes.Add( "onclick", "openExportPopup('nodetypeid=" + NodeType.NodeTypeId.ToString() + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + NbtViewRenderingMode.ToString() + "');" );
                            ExportMenuItem.Items.Add( DesignExportItem );
                        }
                        }
                } // if( AllowExport )

                if( AllowMobile && _View != null && _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                {
                    _MobileMenuItem.Visible = true;
                    _MobileMenuItem.Items.Clear();

                    RadMenuItem MobileExportItem = new RadMenuItem();
                    MobileExportItem.Text = "Export Mobile XML";
                    MobileExportItem.Value = "export_" + ExportOutputFormat.MobileXML.ToString().ToLower();
                    MobileExportItem.CssClass = SubMenuGroupCssClass;
                    MobileExportItem.Attributes.Add( "onclick", "openExportPopup('sessionviewid=" + _View.SessionViewId + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + NbtViewRenderingMode.ToString() + "');" );
                    _MobileMenuItem.Items.Add( MobileExportItem );
                }
                else
                {
                    _MobileMenuItem.Visible = false;
                }

                // Setup Add Menu
                _AddMenuItem.Visible = false;
                if( ParentNodeKey != null )
                {
                    if( ParentNodeKey.NodeSpecies == NodeSpecies.Plain )
                        _Node = _CswNbtResources.Nodes[ParentNodeKey];

                    if( AllowAdd )
                    {
                        if( ParentNodeKeyViewNode != null )
                        {
                            bool LimitToFirstGeneration = ( View.ViewMode == NbtViewRenderingMode.Grid );
                            Collection<CswNbtViewNode.CswNbtViewAddNodeTypeEntry> AllowedChildNodeTypes = ParentNodeKeyViewNode.AllowedChildNodeTypes( LimitToFirstGeneration );
                            if( AllowedChildNodeTypes.Count > 0 )
                            {
                                _AddMenuItem.Visible = true;
                                _AddMenuItem.Items.Clear();

                                string AddMenuDoesntChangeViewString = "0";
                                if( AddMenuDoesntChangeView )
                                    AddMenuDoesntChangeViewString = "1";
                                string AddMenuDoesntChangeSelectedNodeString = "0";
                                if( AddMenuDoesntChangeSelectedNode )
                                    AddMenuDoesntChangeSelectedNodeString = "1";

                                foreach( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry in AllowedChildNodeTypes )
                                {
                                    RadMenuItem TypeMenuItem = new RadMenuItem();
                                    TypeMenuItem.Text = Entry.NodeType.NodeTypeName;
                                    TypeMenuItem.Value = "addnode_" + Entry.NodeType.NodeTypeId.ToString();
                                    TypeMenuItem.CssClass = SubMenuGroupCssClass;
                                    //TypeMenuItem.Attributes.Add( "onclick", "openNewNodePopup('" + Entry.NodeType.NodeTypeId.ToString() + "', '" + ParentNodeKey.ToJavaScriptParam() + "', '" + ParentNodeKeyViewNode.View.SessionViewId.ToString() + "', '" + Entry.ViewRelationship.UniqueId + "', '" + AddMenuDoesntChangeViewString + "', '" + AddMenuDoesntChangeSelectedNodeString + "');" );
                                    // Case 20544 - Add viewid to querystring
                                    TypeMenuItem.Attributes.Add( "onclick", "openNewNodePopup('" + Entry.NodeType.NodeTypeId.ToString() + "', '" + ParentNodeKey.ToString() + "', '" + ParentNodeKeyViewNode.View.SessionViewId.ToString() + "', '" + AddMenuDoesntChangeViewString + "', '" + AddMenuDoesntChangeSelectedNodeString + "', '" + _View.ViewId.ToString() + "');" );
                                    _AddMenuItem.Items.Add( TypeMenuItem );
                                }
                            }
                        }
                    }
                }
                else if( IsDesignMode )
                {
                    _AddMenuItem.Items.Clear();

                    if( DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property ||
                         DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeTypeBaseVersion ||
                         DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.PropertyFilter )
                    {
                        // No Add Menu
                    }
                    else if( DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab )
                    {
                        CswNbtMetaDataNodeTypeTab SelectedTab = CswNbtResources.MetaData.getNodeTypeTab( Convert.ToInt32( DesignSelectedValue ) );
						if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, SelectedTab.getNodeType() ) )
                        {
                            if( SelectedTab.getNodeType().IsLatestVersion() )
                            {
                                _AddMenuItem.Visible = true;
                                RadMenuItem AddPropMenuItem = new RadMenuItem();
                                if( DesignMode == NbtDesignMode.Inspection )
                                    AddPropMenuItem.Text = "Question";
                                else
                                    AddPropMenuItem.Text = "Property";
                                AddPropMenuItem.Value = "addprop_" + DesignSelectedValue;
                                AddPropMenuItem.CssClass = SubMenuGroupCssClass;
                                string js = string.Empty;
                                if( DesignMode == NbtDesignMode.Inspection )
                                    js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.Property + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Tab + "','" + DesignSelectedValue + "', 'i');";
                                else
                                    js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.Property + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Tab + "','" + DesignSelectedValue + "', '');";
                                AddPropMenuItem.Attributes.Add( "onclick", js );
                                AddPropMenuItem.PostBack = false;
                                _AddMenuItem.Items.Add( AddPropMenuItem );
                            }
                        }
                    }
                    else if( DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType )
                    {
                        CswNbtMetaDataNodeType SelectedNodeType = CswNbtResources.MetaData.getNodeType( Convert.ToInt32( DesignSelectedValue ) );
						if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, SelectedNodeType ) )
                        {
                            if( SelectedNodeType != null && SelectedNodeType.IsLatestVersion() )
                            {
                                _AddMenuItem.Visible = true;
                                RadMenuItem AddTabMenuItem = new RadMenuItem();
                                if( DesignMode == NbtDesignMode.Inspection )
                                    AddTabMenuItem.Text = "Section";
                                else
                                    AddTabMenuItem.Text = "Tab";
                                AddTabMenuItem.Value = "addtab_" + DesignSelectedValue;
                                AddTabMenuItem.CssClass = SubMenuGroupCssClass;
                                string js = string.Empty;
                                if( DesignMode == NbtDesignMode.Inspection )
                                    js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.Tab + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType + "','" + DesignSelectedValue + "', 'i');";
                                else
                                    js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.Tab + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType + "','" + DesignSelectedValue + "', '');";
                                AddTabMenuItem.Attributes.Add( "onclick", js );
                                AddTabMenuItem.PostBack = false;
                                _AddMenuItem.Items.Add( AddTabMenuItem );
                            }
                        }
                    }
                    else
                    {
                        _AddMenuItem.Visible = true;
                        RadMenuItem AddTypeMenuItem = new RadMenuItem();
                        if( DesignMode == NbtDesignMode.Inspection )
                            AddTypeMenuItem.Text = "Inspection Design";
                        else
                            AddTypeMenuItem.Text = "NodeType";
                        AddTypeMenuItem.Value = "addtype";
                        AddTypeMenuItem.CssClass = SubMenuGroupCssClass;
                        string js = string.Empty;
                        if( DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Category )
                        {
                            if( DesignMode == NbtDesignMode.Inspection )
                                js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString() + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Category.ToString() + "','" + DesignSelectedValue + "', 'i');";
                            else
                                js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString() + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Category.ToString() + "','" + DesignSelectedValue + "', '');";
                        }
                        else
                        {
                            if( DesignMode == NbtDesignMode.Inspection )
                                js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString() + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Root.ToString() + "','', 'i');";
                            else
                                js = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString() + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Root.ToString() + "','', '');";
                        }
                        AddTypeMenuItem.Attributes.Add( "onclick", js );
                        AddTypeMenuItem.PostBack = false;
                        _AddMenuItem.Items.Add( AddTypeMenuItem );
                    }
                }

                // Edit View
                if( AllowEditView && _EditViewMenuItem != null && View != null )
                {
                    _EditViewMenuItem.NavigateUrl = "EditView.aspx?viewid=" + View.ViewId.ToString();
                    if( View.Visibility == NbtViewVisibility.Property )
                        _EditViewMenuItem.NavigateUrl += "&step=2";
                }
                else
                {
                    _EditViewMenuItem.NavigateUrl = "EditView.aspx";
                }

                // Search
                //if( View is CswNbtView && View != null && View.IsSearchable() )
                //{
                _SearchMenuItem.Visible = true;
                // Handle searching from grid properties:
                if( View != null && ParentNodeKey != null )
                    // Case 20715 - need to check NodeSpecies first
                    if( ParentNodeKey.NodeSpecies == NodeSpecies.Plain && null != ParentNodeKey.NodeId )
                        _SearchMenuItem.NavigateUrl = "Search.aspx?nodeid=" + ParentNodeKey.NodeId.ToString() + "&viewid=" + View.ViewId.ToString();
                    else
                        _SearchMenuItem.NavigateUrl = "Search.aspx?nodeid=" + "&viewid=" + View.ViewId.ToString();
                else
                    _SearchMenuItem.NavigateUrl = "Search.aspx";
                //}
                //else
                //{
                //    _SearchMenuItem.Visible = false;
                //}


                // Copy
                if( AllowCopy && SelectedNodeKey != null &&
                    SelectedNodeKey.NodeSpecies == NodeSpecies.Plain &&
                    _CswNbtResources.Permit.can( Nbt.Security.CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType(SelectedNodeKey.NodeTypeId )) )
                {
                    if( SelectedNodeKeyViewNode != null && SelectedNodeKeyViewNode is CswNbtViewRelationship &&
                      ( (CswNbtViewRelationship) SelectedNodeKeyViewNode ).NodeIdsToFilterIn.Count == 0 )   // BZ 8022
                    {
                        CswNbtMetaDataNodeType CopyNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeKey.NodeTypeId );
                        string badproperty = string.Empty;
                        if( !CopyNodeType.IsUniqueAndRequired( ref badproperty ) )
                        {
                            _CopyMenuItem.Visible = true;
                            _CopyMenuItem.Attributes.Add( "onclick", "openCopyPopup('" + SelectedNodeKey.ToString() + "');" );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        } // CswMainMenu_DataBinding()

        private void _addExportMenuItem( CswNbtView View, ExportOutputFormat Format, NbtViewRenderingMode RenderingMode )
        {
            RadMenuItem ExportAsItem = new RadMenuItem();
            ExportAsItem.Text = Format.ToString();
            ExportAsItem.Value = "export_" + Format.ToString().ToLower();
            ExportAsItem.CssClass = SubMenuGroupCssClass;
            ExportAsItem.Attributes.Add( "onclick", "openExportPopup('sessionviewid=" + _View.SessionViewId + "&format=" + Format.ToString().ToLower() + "&renderingmode=" + RenderingMode.ToString() + "');" );
            ExportMenuItem.Items.Add( ExportAsItem );
        }

        /// <summary>
        /// The underlying Menu Items collection
        /// </summary>
        public RadMenuItemCollection Items
        {
            get
            {
                EnsureChildControls();
                return _Menu.Items;
            }
        }

        private HtmlGenericControl _MenuDiv;
        private RadMenu _Menu;
        private RadMenuItem _AddMenuItem;
        private RadMenuItem _CopyMenuItem;
        private RadMenuItem _SearchMenuItem;
        private RadMenuItem _EditViewMenuItem;
        //private RadMenuItem _ViewMenuItem;
        private RadMenuItem _ChangeViewMenuItem;
        private RadMenuItem _BatchOpsMenuItem;
        private RadMenuItem _MobileMenuItem;
        private RadMenuItem SaveViewAsMenuItem;

        /// <summary>
        /// The Delete menu item, for hooking up events
        /// </summary>
        public RadMenuItem DeleteMenuItem;
        /// <summary>
        /// The Print menu item, for hooking up events
        /// </summary>
        public RadMenuItem PrintMenuItem;
        /// <summary>
        /// The Print Label menu item, for hooking up events
        /// </summary>
        public RadMenuItem PrintLabelMenuItem;
        /// <summary>
        /// The Export menu item, for hooking up events
        /// </summary>
        public RadMenuItem ExportMenuItem;

        /// <summary>
        /// Creates the Menu control and all menu items
        /// </summary>
        protected override void CreateChildControls()
        {
            _MenuDiv = new HtmlGenericControl( "div" );
            _MenuDiv.ID = "menudiv";
            this.Controls.Add( _MenuDiv );

            _Menu = new RadMenu();
            _Menu.EnableViewState = false;   // do not restore menu items from previous loads
            //_Menu.ImagesBaseUrl = "Images/menu/";
            //_Menu.AutoPostBackOnSelect = true;
            _Menu.ItemClick += new RadMenuEventHandler( _Menu_ItemClick );
            //+= new RadMenu.ItemSelectedEventHandler(OnMainMenuItemSelected);
            _Menu.CollapseDelay = 500;
            _Menu.CssClass = "Menu";
            _Menu.EnableEmbeddedSkins = false;
            _Menu.Skin = "ChemSW";
            _MenuDiv.Controls.Add( _Menu );

            _SearchMenuItem = new RadMenuItem();
            _SearchMenuItem.Text = "Search";
            _Menu.Items.Add( _SearchMenuItem );

            _AddMenuItem = new RadMenuItem();
            _AddMenuItem.Text = "Add";
            _AddMenuItem.PostBack = false;
            _AddMenuItem.Visible = false;  // if we never databind...
            _Menu.Items.Add( _AddMenuItem );

            _CopyMenuItem = new RadMenuItem();
            _CopyMenuItem.Text = "Copy";
            _CopyMenuItem.PostBack = false;
            _CopyMenuItem.Visible = false;  // if we never databind...
            _Menu.Items.Add( _CopyMenuItem );

            DeleteMenuItem = new RadMenuItem();
            DeleteMenuItem.Text = "Delete";
            DeleteMenuItem.PostBack = false;
            _Menu.Items.Add( DeleteMenuItem );

            SaveViewAsMenuItem = new RadMenuItem();
            SaveViewAsMenuItem.Text = "Save As";
            SaveViewAsMenuItem.PostBack = false;
            _Menu.Items.Add( SaveViewAsMenuItem );

            PrintLabelMenuItem = new RadMenuItem();
            PrintLabelMenuItem.Value = "printlabelmenuitem";
            PrintLabelMenuItem.Text = "Print Label";
            PrintLabelMenuItem.PostBack = false;
            _Menu.Items.Add( PrintLabelMenuItem );

            PrintMenuItem = new RadMenuItem();
            PrintMenuItem.Value = "printmenuitem";
            PrintMenuItem.Text = "Print";
            PrintMenuItem.PostBack = false;
            _Menu.Items.Add( PrintMenuItem );

            ExportMenuItem = new RadMenuItem();
            ExportMenuItem.Value = "exportmenuitem";
            ExportMenuItem.Text = "Export";
            ExportMenuItem.PostBack = false;
            _Menu.Items.Add( ExportMenuItem );

            _MobileMenuItem = new RadMenuItem();
            _MobileMenuItem.PostBack = false;
            _MobileMenuItem.Text = "Mobile";
            _Menu.Items.Add( _MobileMenuItem );

            _ChangeViewMenuItem = new RadMenuItem();
            _ChangeViewMenuItem.Text = "Switch View";
            _ChangeViewMenuItem.Attributes.Add( "onclick", "return openChangeViewPopup();" );
            _ChangeViewMenuItem.PostBack = false;
            _Menu.Items.Add( _ChangeViewMenuItem );

            _EditViewMenuItem = new RadMenuItem();
            _EditViewMenuItem.Text = "Edit View";
            _Menu.Items.Add( _EditViewMenuItem );

            _BatchOpsMenuItem = new RadMenuItem();
            _BatchOpsMenuItem.PostBack = false;
            _BatchOpsMenuItem.Attributes.Add( "onclick", "BatchOpsMenuItem_Click();" );
            _Menu.Items.Add( _BatchOpsMenuItem );

            base.CreateChildControls();
        }

        /// <summary>
        /// Event when a menu item is selected
        /// </summary>
        public event RadMenuEventHandler MainMenuItemSelected = null;

        private void _Menu_ItemClick( object sender, RadMenuEventArgs e )
        {
            try
            {
                if( MainMenuItemSelected != null )
                    MainMenuItemSelected( sender, e );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        /// <summary>
        /// PreRender
        /// </summary>
        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                //_Menu.DefaultItemLookId = TopItemLookId;
                //_Menu.DefaultGroupCssClass = MenuGroupCssClass;
                _MenuDiv.Attributes.Add( "class", MenuDivCssClass );

                if( _View != null )
                    PrintMenuItem.Attributes.Add( "onclick", "openPrintGridPopup('sessionviewid=" + _View.SessionViewId + "');" );
                else
                    PrintMenuItem.Attributes.Add( "onclick", "openPrintGridPopup('');" );

                _ChangeViewMenuItem.Visible = false;
                _EditViewMenuItem.Visible = false;
                //_SearchMenuItem.Visible = false;
                //ExportMenuItem.Visible = false;
                PrintMenuItem.Visible = false;
                PrintLabelMenuItem.Visible = false;
                _BatchOpsMenuItem.Visible = false;
                SaveViewAsMenuItem.Visible = false;
                //if( AllowSearch )
                //_SearchMenuItem.Visible = true;
                if( AllowChangeView )
                    _ChangeViewMenuItem.Visible = true;
                if( AllowBatch )
                    _BatchOpsMenuItem.Visible = true;
                if( AllowMobile && _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Mobile ) )
                    _MobileMenuItem.Visible = true;
                //if( AllowEditView && ( (CswNbtObjClassRole) _CswNbtResources.CurrentNbtUser.RoleNode ).ActionPermissions.CheckValue( CswNbtAction.PermissionXValue, CswNbtAction.ActionNameEnumToString( _CswNbtResources.Actions[CswNbtActionName.Edit_View].Name ) ) )
				if( AllowEditView && _CswNbtResources.Permit.can( CswNbtActionName.Edit_View ) )
					_EditViewMenuItem.Visible = true;
                //if ( AllowExport && NbtViewRenderingMode.Unknown != NbtViewRenderingMode )
                //    ExportMenuItem.Visible = true;
                //if( AllowExportXml )
                //    ExportAsXmlMenuItem.Visible = true;
                if( AllowPrint )
                    PrintMenuItem.Visible = true;

                if( AllowPrintLabel && SelectedNodeKey != null )
                {
                    CswNbtMetaDataNodeType SelectedNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeKey.NodeTypeId );
                    if( SelectedNodeType != null )
                    {
                        CswNbtMetaDataNodeTypeProp BarcodeProperty = SelectedNodeType.getBarcodeProperty();
                        if( BarcodeProperty != null )
                        {
                            PrintLabelMenuItem.Visible = true;
                            PrintLabelMenuItem.Attributes.Add( "onclick", "openPrintLabelPopup('" + SelectedNodeKey.NodeId.ToString() + "','" + BarcodeProperty.PropId.ToString() + "');" );
                        }
                    }
                }

                if( this.View is CswNbtView && !( (CswNbtView) View ).ViewId.isSet() )
                {
                    SaveViewAsMenuItem.Visible = true;
                    SaveViewAsMenuItem.Attributes.Add( "onclick", "openSaveViewAsPopup('" + View.SessionViewId + "');" );
                }

                // Setup delete menu
                if( DeleteMenuItem != null )
                {
                    DeleteMenuItem.Visible = false;
                    if( SelectedNodeKey != null &&
                        //_Node != null &&
                        SelectedNodeKey.NodeSpecies == NodeSpecies.Plain &&
                        this.AllowDelete &&
                        SelectedNodeKeyViewNode != null )
                    {
                        if( SelectedNodeKeyViewNode is CswNbtViewRelationship &&
                            ( (CswNbtViewRelationship) SelectedNodeKeyViewNode ).AllowDelete &&
                            _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, _CswNbtResources.MetaData.getNodeType( SelectedNodeKey.NodeTypeId ), false, null, null, CswNbtResources.Nodes[SelectedNodeKey].NodeId, null ) )
                        {
                            DeleteMenuItem.Visible = true;

                            // This lives in MainLayout.aspx, because the dialog cannot be inside an UpdatePanel.
                            DeleteMenuItem.Attributes.Add( "onclick", "openDeleteNodePopup('" + SelectedNodeKey.ToString() + "');" );
                        }
                    }
                    else if( IsDesignMode &&
                              this.AllowDelete &&
                              ( DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType ||
                                DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab ||
                                DesignSelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property ) &&
                              DesignSelectedValue != string.Empty )
                    {
                        // Can't delete the first tab
                        if( ( DesignSelectedType != CswNodeTypeTree.NodeTypeTreeSelectedType.Tab ) ||
                            ( _CswNbtResources.MetaData.getNodeTypeTab( Convert.ToInt32( DesignSelectedValue ) ).getNodeType().getNodeTypeTabs().Count() > 1 ) )
                        {
                            DeleteMenuItem.Visible = true;

                            // This lives in main.js
                            DeleteMenuItem.Attributes.Add( "onclick", "openDesignDeletePopup('" + DesignSelectedType.ToString() + "','" + DesignSelectedValue + "');" );
                        }
                    }
                }

                // case 21158
                _SearchMenuItem.Visible = false;
                _ChangeViewMenuItem.Visible = false;
                _EditViewMenuItem.Visible = false;


            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }


        /// <summary>
        /// Event which occurs if the control encounters an error
        /// </summary>
        public event CswErrorHandler OnError;

        private void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }



    } // class CswMainMenu
} // namespace ChemSW.NbtWebControls
