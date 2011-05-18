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
using Telerik.Web.UI;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.WebPages
{
    public partial class Design : System.Web.UI.Page
    {
        private string _IconImageRoot = "Images/icons";
        private NbtDesignMode _Mode = NbtDesignMode.Standard;

        public string LabelNodeType = "NodeType";
        public string LabelNodeTypeTab = "Tab";
        public string LabelNodeTypeProp = "Property";

        public static string QueryStringVarName_Mode = "mode";

        public enum DesignPage
        {
            BlankPage,
            EditCategoryPage,
            AddNodeTypePage,
            EditNodeTypePage,
            AddTabPage,
            EditTabPage,
            AddPropertyPage,
            EditPropertyPage
        }


        #region Selected Value Properties

        private CswNodeTypeTree.NodeTypeTreeSelectedType _SelectedType
        {
            get { EnsureChildControls(); return NodeTypeTree.SelectedType; }
        }
        private string _SelectedValue
        {
            get { EnsureChildControls(); return NodeTypeTree.SelectedValue; }
        }

        private void setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType Type, string Value, bool reinitTabs )
        {
            if( Type != _SelectedType || Value != _SelectedValue )
            {
                NodeTypeTree.setSelectedNode( Type, Value );
                NodeTypeTree.DataBind();
                _SelectedNodeType = null;
                _SelectedNodeTypeProp = null;
                _SelectedNodeTypeTab = null;
            }

            if( reinitTabs )
            {
                switch( _SelectedType )
                {
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Category:
                        InitTab( DesignPage.EditCategoryPage );
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeTypeBaseVersion:
                        InitTab( DesignPage.BlankPage );
                        BlankPageLabel.Text = "Please select a version of the selected " + LabelNodeType;
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.PropertyFilter:
                        InitTab( DesignPage.BlankPage );
                        BlankPageLabel.Text = "Please select a " + LabelNodeTypeProp;
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Root:
                        InitTab( DesignPage.BlankPage );
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                        InitTab( DesignPage.EditNodeTypePage );
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                        InitTab( DesignPage.EditPropertyPage );
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Tab:
                        InitTab( DesignPage.EditTabPage );
                        break;
                }
            }
            DesignMenu.DesignSelectedType = Type;
            DesignMenu.DesignSelectedValue = Value;
            DesignMenu.DataBind();
        }

        private CswNbtMetaDataNodeType _SelectedNodeType = null;
        private CswNbtMetaDataNodeTypeTab _SelectedNodeTypeTab = null;
        private CswNbtMetaDataNodeTypeProp _SelectedNodeTypeProp = null;
        private void InitSelectedMetaDataObjects()
        {
            EnsureChildControls();
            switch( _SelectedType )
            {
                case CswNodeTypeTree.NodeTypeTreeSelectedType.Category:
                    _SelectedNodeType = null;
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                    if( CswConvert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( _SelectedValue ) );
                    }
                    else
                    {
                        _SelectedNodeType = null;
                    }
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                    if( CswConvert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeTypeProp = Master.CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( _SelectedValue ) );
                        _SelectedNodeType = _SelectedNodeTypeProp.NodeType;
                        _SelectedNodeTypeTab = _SelectedNodeTypeProp.NodeTypeTab;
                    }
                    else
                    {
                        _SelectedNodeType = null;
                        _SelectedNodeTypeTab = null;
                        _SelectedNodeTypeProp = null;
                    }
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Root:
                    _SelectedNodeType = null;
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Tab:
                    if( CswConvert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeTypeTab = Master.CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( _SelectedValue ) );
                        _SelectedNodeType = _SelectedNodeTypeTab.NodeType;
                    }
                    else
                    {
                        _SelectedNodeType = null;
                        _SelectedNodeTypeTab = null;
                    }
                    _SelectedNodeTypeProp = null;
                    break;
            }
        }

        private CswNbtMetaDataNodeType SelectedNodeType
        {
            get
            {
                if( _SelectedNodeType == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeType;
            }
        }
        private CswNbtMetaDataNodeTypeTab SelectedNodeTypeTab
        {
            get
            {
                if( _SelectedNodeTypeTab == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeTab;
            }
        }
        private CswNbtMetaDataNodeTypeProp SelectedNodeTypeProp
        {
            get
            {
                if( _SelectedNodeTypeProp == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeProp;
            }
        }

        #endregion Selected Value Properties

        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                //this.EnableViewState = false;

                if( Request.QueryString[QueryStringVarName_Mode] != null )
                {
                    switch( Request.QueryString[QueryStringVarName_Mode].ToString() )
                    {
                        case "i":
                            _Mode = NbtDesignMode.Inspection;
                            LabelNodeType = "Inspection Design";
                            LabelNodeTypeTab = "Section";
                            LabelNodeTypeProp = "Question";
                            break;
                    }
                }

                EnsureChildControls();
                if( !( (CswNbtObjClassUser) Master.CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Design ) )
                {
                    Master.GoHome();
                }
                NodeTypeTree.DataBind();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }


        private CswNodeTypeTree NodeTypeTree = null;
        private CswMainMenu DesignMenu = null;
        private CswAutoTable _LeftTable;
        private RadTabStrip DesignTabStrip;
        private CswAutoTable DesignTabTable;
        private HtmlGenericControl HiddenButtonDiv;

        private CswPropertyFilter _ConditionalFilter;
        private CswFieldTypeWebControl _DefaultValueControl = null;
        private CheckBox _RequiredValue;
        private CheckBox _IsUnique;
        private CheckBox _SetValueOnAddValue;

        private Button HiddenDeleteButton;
        private Button HiddenRefreshButton;
        private Button _SaveButton;
        private Button _ChangeObjectClassButton;
        private Button _CopyNodeTypeButton;

        protected override void CreateChildControls()
        {
            try
            {
                //DesignMenu = new CswMainMenu( Master.CswNbtResources );
                DesignMenu = Master.MainMenu;
                DesignMenu.OnError += new CswErrorHandler( Master.HandleError );
                DesignMenu.ID = "DesignMenu";
                DesignMenu.AllowEditView = true;
                DesignMenu.AllowAdd = true;
                DesignMenu.AllowDelete = true;
                DesignMenu.IsDesignMode = true;
                DesignMenu.DesignMode = _Mode;
                DesignMenu.AllowExport = true;
                DesignMenu.AllowChangeView = true;
                //leftph.Controls.Add( DesignMenu );

                HtmlGenericControl LeftDiv = new HtmlGenericControl( "div" );
                LeftDiv.Attributes.Add( "class", "treediv" );
                leftph.Controls.Add( LeftDiv );

                _LeftTable = new CswAutoTable();
                _LeftTable.ID = "lefttable";
                //_LeftTable.EnableViewState = false;
                LeftDiv.Controls.Add( _LeftTable );

                NodeTypeTree = new CswNodeTypeTree( Master.CswNbtResources );
                NodeTypeTree.ID = "NodeTypeTree";
                NodeTypeTree.ShowTabsAndProperties = true;
                NodeTypeTree.PropertySort = CswNodeTypeTree.PropertySortSetting.DisplayOrder;
                NodeTypeTree.ShowConditionalPropertiesBeneath = true;
                if( _Mode == NbtDesignMode.Inspection )
                {
                    NodeTypeTree.ShowQuestionNumbers = true;
                    NodeTypeTree.ObjectClassIdsToInclude = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ).ObjectClassId.ToString();
                    NodeTypeTree.TreeName = "Inspection Design";
                    NodeTypeTree.TreeView.OnClientNodePopulating = "NodeTypeTree_OnNodePopulating_InspectionMode";
                }
                _LeftTable.addControl( 1, 0, NodeTypeTree );

                DesignTabStrip = new RadTabStrip();
                DesignTabStrip.ID = "designtabstrip";
                DesignTabStrip.EnableEmbeddedSkins = false;
                DesignTabStrip.Skin = "ChemSW";
                DesignTabStrip.Tabs.Add( new RadTab( "Design", "designtab" ) );
                DesignTabStrip.EnableViewState = false;
                rightph.Controls.Add( DesignTabStrip );

                CswTabOuterTable _TabOuterTable = new CswTabOuterTable();
                _TabOuterTable.ID = "TabOuterTable";
                rightph.Controls.Add( _TabOuterTable );

                DesignTabTable = new CswAutoTable();
                DesignTabTable.ID = "designtabtable";
                DesignTabTable.FirstCellRightAlign = true;
                DesignTabTable.EnableViewState = false;
                DesignTabTable.CssClass = "TabTable";
                _TabOuterTable.ContentCell.Controls.Add( DesignTabTable );

                HiddenButtonDiv = new HtmlGenericControl( "div" );
                HiddenButtonDiv.ID = "HiddenDeleteButtonDiv";
                HiddenButtonDiv.Style.Add( "display", "none" );
                _LeftTable.addControl( 2, 0, HiddenButtonDiv );

                HiddenDeleteButton = new Button();
                HiddenDeleteButton.ID = "HiddenDeleteButton";
                HiddenDeleteButton.CssClass = "Button";
                HiddenDeleteButton.Text = "Delete";
                HiddenDeleteButton.Click += new EventHandler( HiddenDeleteButton_Click );
                HiddenButtonDiv.Controls.Add( HiddenDeleteButton );

                HiddenRefreshButton = new Button();
                HiddenRefreshButton.ID = "HiddenRefreshButton";
                HiddenRefreshButton.CssClass = "Button";
                HiddenRefreshButton.Text = "Refresh";
                HiddenRefreshButton.Click += new EventHandler( HiddenRefreshButton_Click );
                HiddenButtonDiv.Controls.Add( HiddenRefreshButton );

                // In order for Ajax to work correctly, we need all buttons to be instanced here, rather than later.
                // But these buttons are added to the Controls collection later
                _SaveButton = new Button();
                _SaveButton.ID = "DesignSaveButton";
                _SaveButton.CssClass = "Button";
                _SaveButton.Text = "Save";
                _SaveButton.OnClientClick = "if(!CswPropertyTable_SaveButton_PreClick()) return false;";
                _SaveButton.Click += new EventHandler( _SaveButton_Click );
                _SaveButton.ValidationGroup = "Design";
                HiddenButtonDiv.Controls.Add( _SaveButton );  // this is necessary for Ajax, but overridden below

                _ChangeObjectClassButton = new Button();
                _ChangeObjectClassButton.ID = "ChangeObjectClassButton";
                _ChangeObjectClassButton.Text = "Convert " + LabelNodeType;
                _ChangeObjectClassButton.Click += new EventHandler( _ConvertNodeTypeButton_Click );
                _ChangeObjectClassButton.CssClass = "Button";
                HiddenButtonDiv.Controls.Add( _ChangeObjectClassButton );  // this is necessary for Ajax, but overridden below

                _CopyNodeTypeButton = new Button();
                _CopyNodeTypeButton.ID = "CopyNodeTypeButton";
                _CopyNodeTypeButton.Text = "Copy " + LabelNodeType;
                _CopyNodeTypeButton.Click += new EventHandler( _CopyNodeTypeButton_Click );
                _CopyNodeTypeButton.CssClass = "Button";
                HiddenButtonDiv.Controls.Add( _CopyNodeTypeButton );  // this is necessary for Ajax, but overridden below
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }


            base.CreateChildControls();
        }


        protected override void OnLoad( EventArgs e )
        {
            try
            {
                //if( !Page.IsPostBack )

                //if( ( !Page.IsPostBack ||
                //    ( Session["Design_ForceReselect"] != null && Session["Design_ForceReselect"].ToString() == "true" ) ) )
                //{
                //    // This is required for below
                //    NodeTypeTree.DataBind();
                //}

                if( ( !Page.IsPostBack ||
                      ( Session["Design_ForceReselect"] != null && Session["Design_ForceReselect"].ToString() == "true" ) ) &&
                    ( Session["Design_SelectedType"] != null && Session["Design_SelectedType"].ToString() != string.Empty ) &&
                    ( Session["Design_SelectedValue"] != null && Session["Design_SelectedValue"].ToString() != string.Empty ) )
                {
                    setSelected( (CswNodeTypeTree.NodeTypeTreeSelectedType) Enum.Parse( typeof( CswNodeTypeTree.NodeTypeTreeSelectedType ), Session["Design_SelectedType"].ToString() ), Session["Design_SelectedValue"].ToString(), true );
                    Session["Design_ForceReselect"] = string.Empty;
                }
                else
                {
                    setSelected( _SelectedType, _SelectedValue, true );
                }

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( NodeTypeTree, DesignMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( NodeTypeTree, DesignTabStrip );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( NodeTypeTree, DesignTabTable );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( NodeTypeTree, Master.ErrorBox );

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( HiddenRefreshButton, NodeTypeTree );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( HiddenRefreshButton, DesignMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( HiddenRefreshButton, DesignTabStrip );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( HiddenRefreshButton, DesignTabTable );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( HiddenRefreshButton, Master.ErrorBox );

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _SaveButton, NodeTypeTree );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _SaveButton, DesignMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _SaveButton, DesignTabStrip );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _SaveButton, DesignTabTable );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _SaveButton, Master.ErrorBox );

                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CopyNodeTypeButton, DesignMenu );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CopyNodeTypeButton, NodeTypeTree );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CopyNodeTypeButton, DesignTabStrip );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CopyNodeTypeButton, DesignTabTable );
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _CopyNodeTypeButton, Master.ErrorBox );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                if( AddToNameTemplatePropSelect != null )
                    AddToNameTemplatePropSelect.Attributes.Add( "onchange", "addPropToTemplate(this, '" + NameTemplate.ClientID + "', '" + CswNbtMetaData._TemplateLeftDelimiter.ToString() + "', '" + CswNbtMetaData._TemplateRightDelimiter.ToString() + "');" );

                if( Master.DesignDeleteDialogWindow != null && DesignMenu != null && DesignMenu.DeleteMenuItem != null )
                    Master.DesignDeleteDialogWindow.OpenerElementID = DesignMenu.DeleteMenuItem.ID;

                // Case 20480
                if( _SelectedNodeTypeProp != null )
                {
                    if( _SetValueOnAddValue != null &&
                        !_SelectedNodeTypeProp.SetValueOnAddEnabled )
                    {
                        _SetValueOnAddValue.InputAttributes.Add( "disabled", "disabled" ); //avoid problem of the enclosing span of the input tag being "disabled" (http://geekswithblogs.net/jonasb/archive/2006/07/27/86498.aspx)
                    }

                    if( _ConditionalFilter != null && !_SelectedNodeTypeProp.FilterEnabled )
                        _ConditionalFilter.Attributes.Add( "disabled", "true" );

                    if( _RequiredValue != null && !_SelectedNodeTypeProp.IsRequiredEnabled )
                    {
                        _RequiredValue.Checked = false;
                        _RequiredValue.InputAttributes.Add( "disabled", "disabled" );
                    }


                    if( _RequiredValue != null )
                    {
                        string onclick = "return onSetPropRequired('" + _RequiredValue.ClientID + "','";
                        if( _SetValueOnAddValue != null && _SelectedNodeTypeProp.DefaultValue.Empty ) onclick += _SetValueOnAddValue.ClientID;
                        onclick += "','";
                        if( _ConditionalFilter != null ) onclick += _ConditionalFilter.ClientID;
                        onclick += "','";
                        if( _ConditionalFilter != null && _ConditionalFilter.PropSelectBox != null ) onclick += _ConditionalFilter.PropSelectBox.ClientID;
                        onclick += "');";
                        _RequiredValue.Attributes.Add( "onclick", onclick );

                        if( _DefaultValueControl != null &&
                            !_SelectedNodeTypeProp.SetValueOnAdd &&
                            _SelectedNodeTypeProp.IsRequired &&
                            _SelectedNodeTypeProp.DefaultValue.Empty )
                        {
                            throw new CswDniException( "Required properties must have a default value if not Set Value on Add", "Default value was empty, with required true and setvalonadd false" );
                        }
                    } //if( _RequiredValue != null )

                    // BZ 4868
                    if( SelectedNodeTypeProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                        SelectedNodeTypeProp.FKValue == Int32.MinValue )
                    {
                        _ViewXmlRow.Visible = false;
                    }
                }//if( _SelectedNodeTypeProp != null )

                // BZ 7389
                if( SelectedNodeTypeProp != null && _WarningLabel != null && SelectedNodeTypeProp.IsUnique && SelectedNodeTypeProp.IsRequired )
                    _WarningLabel.Text = "Warning: A Property that is both Unique and Required will prevent Multi-Add and Copy";


                Session["Design_SelectedType"] = _SelectedType.ToString();
                Session["Design_SelectedValue"] = _SelectedValue;

                if( _SelectedNodeType != null )
                {
                    if( !_SelectedNodeType.CanSave )
                    {
                        if( _SaveButton != null )
                        {
                            // BZ 7936
                            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab )
                            {
                                if( EditTabNameTextBox != null )
                                    EditTabNameTextBox.ReadOnly = true;
                                if( EditTabOrderTextBox != null )
                                    EditTabOrderTextBox.ReadOnly = true;
                                _SaveButton.Visible = true;
                            }
                            else
                            {
                                _SaveButton.Visible = false;
                            }
                        }
                        if( _ChangeObjectClassButton != null )
                            _ChangeObjectClassButton.Visible = false;
                        if( ChangeObjectClassSelect != null )
                            ChangeObjectClassSelect.Visible = false;
                        if( ChangeObjectClassLabel != null )
                            ChangeObjectClassLabel.Visible = false;
                        if( _LayoutLink != null )
                            _LayoutLink.Visible = false;
                    }
                    else
                    {
                        _SaveButton.Visible = true;   // BZ 7877
                    }

                    if( !_SelectedNodeType.CanDelete )
                    {
                        DesignMenu.AllowDelete = false;
                    }
                }

                if( LockedCheckbox != null )
                {
                    if( LockedCheckbox.Checked )
                        LockedCheckbox.Attributes.Add( "disabled", "1" );
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

        protected void _SaveButton_Click( object sender, EventArgs e )
        {
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType )
            {
                _SaveSelectedNodeType();
            }
            else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property )
            {
                _SaveSelectedProp();
            }
            else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab )
            {
                _SaveSelectedTab();
            }
            else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Category )
            {
                _SaveSelectedCategory();
            }
            NodeTypeTree.DataBind();
        }


        protected void HiddenRefreshButton_Click( object sender, EventArgs e )
        {
            NodeTypeTree.DataBind();
        }


        protected void HiddenDeleteButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( SelectedNodeType != null )
                {
                    if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType && CswTools.IsInteger( _SelectedValue ) )
                    {
                        Master.CswNbtResources.MetaData.DeleteNodeType( _SelectedNodeType );

                        // BZ 8511
                        Master.CswNbtResources.ViewSelect.clearCache();
                        //Master.ViewTree.DataBind();  

                        NodeTypeTree.DataBind();


                        Int32 FirstNodeTypeId = NodeTypeTree.getFirstNodeTypeId();
                        if( FirstNodeTypeId > 0 )
                        {
                            if( FirstNodeTypeId.ToString() == _SelectedValue )
                                setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, NodeTypeTree.getSecondNodeTypeId().ToString(), true );
                            else
                                setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, FirstNodeTypeId.ToString(), true );
                        }
                        else
                        {
                            setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.Root, string.Empty, true );
                        }
                    }
                    else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property && CswTools.IsInteger( _SelectedValue ) )
                    {
                        CswNbtMetaDataNodeTypeTab TabToSelect = Master.CswNbtResources.MetaData.DeleteNodeTypeProp( _SelectedNodeTypeProp );

                        setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.Tab, TabToSelect.TabId.ToString(), true );
                    }
                    else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab && CswTools.IsInteger( _SelectedValue ) )
                    {
                        CswNbtMetaDataNodeType NodeTypeToSelect = Master.CswNbtResources.MetaData.DeleteNodeTypeTab( _SelectedNodeTypeTab );

                        setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, NodeTypeToSelect.NodeTypeId.ToString(), true );
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected void _ConvertNodeTypeButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( SelectedNodeType.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass )
                {
                    string ObjectClass = SelectedNodeType.ObjectClass.ObjectClass.ToString();
                    string NodeTypeName = SelectedNodeType.NodeTypeName;
                    throw ( new CswDniException( "Only " + LabelNodeType + "s of the GenericObject class can be changed to other " + LabelNodeType + "s; the object class of the current " + LabelNodeType + " (" + NodeTypeName + ") is " + ObjectClass ) );
                }

                Int32 DestinationObjectClassID = CswConvert.ToInt32( ChangeObjectClassSelect.SelectedItem.Value );
                // BZ 7543 - This syntax is a little strange, but it's because CswNbtMetaData might decide to version the nodetype
                CswNbtMetaDataNodeType PossiblyNewNodeType = Master.CswNbtResources.MetaData.ConvertObjectClass( SelectedNodeType, Master.CswNbtResources.MetaData.getObjectClass( DestinationObjectClassID ) );

                //re-init the tree for changes
                setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, PossiblyNewNodeType.NodeTypeId.ToString(), false );

                init_EditNodeTypePage();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _SaveSelectedCategory()
        {
            try
            {
                if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Category )
                {
                    string OldCategory = _SelectedValue;
                    string NewCategory = EditCategoryTextBox.Text;
                    if( OldCategory != NewCategory )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.NodeTypes )
                        {
                            if( NodeType.Category == OldCategory )
                                NodeType.Category = NewCategory;
                        }

                        //re-init the tree for changes
                        setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.Category, NewCategory, true );
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _SaveSelectedNodeType()
        {
            try
            {
                if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType )
                {
                    // BZ 8372 - Do this first, since after versioning we don't want the new version to be locked
                    if( LockedCheckbox.Checked && !SelectedNodeType.IsLocked )
                        SelectedNodeType.IsLocked = true;

                    SelectedNodeType.NodeTypeName = EditNodeTypeName.Text;
                    SelectedNodeType.Category = EditNodeTypeCategory.Text;
                    SelectedNodeType.IconFileName = IconSelect.SelectedValue;
                    SelectedNodeType.NameTemplateText = NameTemplate.Text;

                    //re-init the tree for changes
                    setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, SelectedNodeType.NodeTypeId.ToString(), true );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected void _CopyNodeTypeButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswNbtMetaDataNodeType NewNodeType = Master.CswNbtResources.MetaData.CopyNodeType( SelectedNodeType, CopiedNodeTypeName.Text );
                CopiedNodeTypeName.Text = "";

                //re-init the tree for changes
                setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType, NewNodeType.NodeTypeId.ToString(), true );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _SaveSelectedTab()
        {
            try
            {
                Int32 NewTabOrder = SelectedNodeTypeTab.TabOrder;
                if( CswTools.IsInteger( EditTabOrderTextBox.Text ) )
                    NewTabOrder = CswConvert.ToInt32( EditTabOrderTextBox.Text );

                SelectedNodeTypeTab.IncludeInNodeReport = EditTabIncludeInNodeReport.Checked;
                SelectedNodeTypeTab.TabName = EditTabNameTextBox.Text;
                SelectedNodeTypeTab.TabOrder = NewTabOrder;


                //re-init the tree for changes
                setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.Tab, SelectedNodeTypeTab.TabId.ToString(), true );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _SaveSelectedProp()
        {
            try
            {
                CswNbtMetaDataNodeTypeProp PropToSave = SelectedNodeTypeProp;
                Int32 OldSelectedNodeTypePropId = CswConvert.ToInt32( _SelectedValue );
                if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property )
                {
                    CswNbtMetaDataNodeTypeTab OriginalTab = SelectedNodeTypeProp.NodeTypeTab;

                    string MultiString = getPropAttributeValue( "EditProp_MultiValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    if( MultiString == string.Empty )
                    {
                        if( PropToSave.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect )
                            MultiString = "Single";  // Default for NodeTypeSelect
                        else
                            MultiString = "Blank";
                    }

                    // For Relationship and MultiRelationship properties:
                    bool NewIsFk = false;
                    string NewFKType = CswNbtViewRelationship.RelatedIdType.Unknown.ToString();
                    Int32 NewFKValue = Int32.MinValue;
                    if( PropToSave.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                    {
                        string TargetValue = getPropAttributeValue( "EditProp_TargetValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                        if( TargetValue != String.Empty )
                        {
                            // Get settings from the form
                            NewIsFk = true;
                            if( TargetValue.Substring( 0, "nt_".Length ) == "nt_" )
                            {
                                NewFKType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                                NewFKValue = CswConvert.ToInt32( TargetValue.Substring( "nt_".Length ) );
                            }
                            else
                            {
                                NewFKType = CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString();
                                NewFKValue = CswConvert.ToInt32( TargetValue.Substring( "oc_".Length ) );
                            }
                        }
                    }
                    else
                    {
                        NewIsFk = false;
                        NewFKType = getPropAttributeValue( "EditProp_FkTypeValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                        NewFKValue = CswConvert.ToInt32( getPropAttributeValue( "EditProp_FkValueValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    }


                    // BZ 7954 - this should be set first

                    PropToSave.IsRequired = Convert.ToBoolean( getPropAttributeValue( "EditProp_RequiredValue" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );

                    // Case 20297 - this should be set second
                    String ValueOptionsString = getPropAttributeValue( "EditProp_ValueOptionsValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    if( PropToSave.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question &&
                        String.Empty == ValueOptionsString )
                    {
                        throw new CswDniException( "Compliant Answer is a required field", "Value option string is null" );
                    }

					// case 21178 - trim options
					CswCommaDelimitedString ValueOptionsCDS = new CswCommaDelimitedString();
					ValueOptionsCDS.FromString( ValueOptionsString );
					for( Int32 i = 0; i < ValueOptionsCDS.Count; i++ )
					{
						ValueOptionsCDS[i] = ValueOptionsCDS[i].Trim();
					}
					PropToSave.ValueOptions = ValueOptionsCDS.ToString();

					string ListOptionsString = getPropAttributeValue( "EditProp_OptionsValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
					CswCommaDelimitedString ListOptionsCDS = new CswCommaDelimitedString();
					ListOptionsCDS.FromString( ListOptionsString );
					for( Int32 i = 0; i < ListOptionsCDS.Count; i++ )
					{
						ListOptionsCDS[i] = ListOptionsCDS[i].Trim();
					}
					PropToSave.ListOptions = ListOptionsCDS.ToString();
					
					PropToSave.PropName = getPropAttributeValue( "EditProp_NameValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    PropToSave.NodeTypeTab = SelectedNodeType.getNodeTypeTab( EditPropTabSelect.SelectedValue.ToString() );   // BZ 8014 - need to use tabname, not tabid, for versioning
                    PropToSave.DisplayRow = CswConvert.ToInt32( getPropAttributeValue( "EditProp_DisplayRowValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.DisplayColumn = CswConvert.ToInt32( getPropAttributeValue( "EditProp_DisplayColValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.DisplayRowAdd = CswConvert.ToInt32( getPropAttributeValue( "EditProp_DisplayRowAddValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.DisplayColAdd = CswConvert.ToInt32( getPropAttributeValue( "EditProp_DisplayColAddValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.SetValueOnAdd = Convert.ToBoolean( getPropAttributeValue( "EditProp_SetValueOnAddValue" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.DateToday = Convert.ToBoolean( getPropAttributeValue( "EditProp_DateTodayValue" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.Length = CswConvert.ToInt32( getPropAttributeValue( "EditProp_LengthValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.TextAreaRows = CswConvert.ToInt32( getPropAttributeValue( "EditProp_RowsValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.TextAreaColumns = CswConvert.ToInt32( getPropAttributeValue( "EditProp_ColsValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.CompositeTemplateText = getPropAttributeValue( "EditProp_TemplateValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    PropToSave.NumberPrecision = CswConvert.ToInt32( getPropAttributeValue( "EditProp_PrecisionValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) );
                    PropToSave.MinValue = Convert.ToDouble( getPropAttributeValue( "EditProp_MinValue" + OldSelectedNodeTypePropId.ToString(), typeof( Double ), EditPropPlaceHolder ) );
                    PropToSave.MaxValue = Convert.ToDouble( getPropAttributeValue( "EditProp_MaxValue" + OldSelectedNodeTypePropId.ToString(), typeof( Double ), EditPropPlaceHolder ) );
                    PropToSave.IsUnique = Convert.ToBoolean( getPropAttributeValue( "EditProp_IsUnique" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.StaticText = getPropAttributeValue( "EditProp_TextValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    PropToSave.ReadOnly = Convert.ToBoolean( getPropAttributeValue( "EditProp_ReadOnlyValue" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.UseNumbering = Convert.ToBoolean( getPropAttributeValue( "EditProp_UseNumbering" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.HideInMobile = Convert.ToBoolean( getPropAttributeValue( "EditProp_HideInMobile" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.MobileSearch = Convert.ToBoolean( getPropAttributeValue( "EditProp_MobileSearch" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.SetFK( NewIsFk, NewFKType, NewFKValue,
                                      getPropAttributeValue( "EditProp_RelatedPropType" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder ),
                                      CswConvert.ToInt32( getPropAttributeValue( "EditProp_RelatedPropValue" + OldSelectedNodeTypePropId.ToString(), typeof( Int32 ), EditPropPlaceHolder ) ) );
                    PropToSave.Multi = (PropertySelectMode) Enum.Parse( typeof( PropertySelectMode ), MultiString );
                    PropToSave.HelpText = getPropAttributeValue( "EditProp_HelpText" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );
                    PropToSave.IsQuickSearch = Convert.ToBoolean( getPropAttributeValue( "EditProp_IsQuickSearch" + OldSelectedNodeTypePropId.ToString(), typeof( bool ), EditPropPlaceHolder ) );
                    PropToSave.Extended = getPropAttributeValue( "EditProp_ExtendedValue" + OldSelectedNodeTypePropId.ToString(), EditPropPlaceHolder );


                    // Default Value
                    Control Control = EditPropPlaceHolder.FindControl( "EditProp_DefaultValue" + OldSelectedNodeTypePropId.ToString() );
                    if( Control != null && Control is CswFieldTypeWebControl )
                    {
                        ( (CswFieldTypeWebControl) Control ).Save();
                        // BZ 8496 - Above changes may have changed the default value control, so we need to re-DataBind
                        if( Control is CswLogicalSet )
                            ( (CswLogicalSet) Control ).Prop.AsLogicalSet.ResetCachedXYValues();
                        Control.DataBind();
                    }

                    // Conditional Filter
                    if( _ConditionalFilter != null && _ConditionalFilter.SelectedPropLatestVersion != null &&
                        PropToSave.NodeTypeTab.TabName == OriginalTab.TabName ) //BZ 7415
                    {
                        PropToSave.setFilter( _ConditionalFilter.SelectedNodeTypePropFirstVersionId, _ConditionalFilter.SelectedSubField, _ConditionalFilter.SelectedFilterMode, _ConditionalFilter.FilterValue );
                    }
                    else
                    {
                        PropToSave.clearFilter();

                        if( _ConditionalFilter != null )
                        {
                            _ConditionalFilter.FilterPropertiesToTabId = PropToSave.NodeTypeTab.TabId;
                            _ConditionalFilter.Set( SelectedNodeType.NodeTypeId, string.Empty );
                        }
                    }

                    // For Sequences:
                    CswSequencesEditor SequencesEditor = (CswSequencesEditor) EditPropPlaceHolder.FindControl( "EditProp_SequenceValue" + OldSelectedNodeTypePropId.ToString() );
                    if( SequencesEditor != null )
                        PropToSave.setSequence( SequencesEditor.SelectedSequenceId );

                    // For Relationships:
                    if( PropToSave.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                    {
                        // Reload the view
						CswNbtView View = Master.CswNbtResources.ViewSelect.restoreView( PropToSave.ViewId );
                        _RelationshipViewTree.reinitTreeFromView( View, null, null, CswViewStructureTree.ViewTreeSelectType.None );
                    }

                    //re-init the tree for changes
                    setSelected( CswNodeTypeTree.NodeTypeTreeSelectedType.Property, PropToSave.PropId.ToString(), true );

                } // if (_SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property)
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected void _LayoutLink_Click( object sender, EventArgs e )
        {
            if( _Mode == NbtDesignMode.Inspection )
                Master.Redirect( "Design_Layout.aspx?mode=Inspection&nodetypeid=" + _SelectedNodeType.NodeTypeId );
            else
                Master.Redirect( "Design_Layout.aspx?nodetypeid=" + _SelectedNodeType.NodeTypeId );
        }

        protected void _DefaultViewLink_Click( object sender, EventArgs e )
        {
            // Make default view
            CswNbtView DefaultView = SelectedNodeType.CreateDefaultView();
            Master.setViewXml( DefaultView.ToXml().InnerXml );
            Master.GoMain();
        }

        #endregion Events

        #region Tab Contents

        private void InitTab( DesignPage PageName )
        {
            DesignTabTable.Controls.Clear();  // Beware!  This caused BZ 7649, and potentially other problems with ajax
            //foreach( WebControl Control in DesignTabTable.Controls )
            //{
            //    Control.Visible = false;
            //}

            switch( PageName )
            {
                case DesignPage.EditNodeTypePage:
                    DesignTabStrip.Tabs[0].Text = "Edit " + LabelNodeType;
                    create_EditNodeTypePage( DesignTabTable );
                    init_EditNodeTypePage();
                    break;
                case DesignPage.EditTabPage:
                    DesignTabStrip.Tabs[0].Text = "Edit " + LabelNodeTypeTab;
                    create_EditTabPage( DesignTabTable );
                    init_EditTabPage();
                    break;
                case DesignPage.EditPropertyPage:
                    DesignTabStrip.Tabs[0].Text = "Edit " + LabelNodeTypeProp;
                    create_EditPropertyPage( DesignTabTable );
                    init_EditPropertyPage();
                    break;
                case DesignPage.BlankPage:
                    DesignTabStrip.Tabs[0].Text = "Design";
                    create_BlankPage( DesignTabTable );
                    init_BlankPage();
                    break;
                case DesignPage.EditCategoryPage:
                    DesignTabStrip.Tabs[0].Text = "Edit Category";
                    create_EditCategoryPage( DesignTabTable );
                    init_EditCategoryPage();
                    break;
            }
        }

        private Literal BlankPageLabel;
        private void create_BlankPage( CswAutoTable TabTable )
        {
            BlankPageLabel = new Literal();
            //BlankPageLabel.ID = "BlankPageLabel";
            BlankPageLabel.Text = "Please select a " + LabelNodeType + ", " + LabelNodeTypeTab + ", or " + LabelNodeTypeProp;
            TabTable.addControl( 0, 1, BlankPageLabel );
        }

        private void init_BlankPage() { }


        private Literal EditCategoryLabel;
        private TextBox EditCategoryTextBox;
        private void create_EditCategoryPage( CswAutoTable TabTable )
        {
            EditCategoryLabel = new Literal();
            EditCategoryLabel.Text = "Edit Category: ";
            TabTable.addControl( 1, 0, EditCategoryLabel );

            EditCategoryTextBox = new TextBox();
            EditCategoryTextBox.ID = "EditCategoryTextBox_" + _SelectedValue;
            TabTable.addControl( 1, 1, EditCategoryTextBox );

            TabTable.addControl( 2, 1, _SaveButton );
        }

        private void init_EditCategoryPage()
        {
            EditCategoryTextBox.Text = _SelectedValue;
        }


        private Literal NodeTypeName;
        private TextBox EditNodeTypeName;
        private Literal NodeTypeObjectClassLabel;
        private Literal NodeTypeObjectClass;
        private Literal NodeTypeCategoryLabel;
        private TextBox EditNodeTypeCategory;
        private Literal IconLabel;
        private DropDownList IconSelect;
        private Image IconImage;
        private Literal NameTemplateLabel;
        private TextBox NameTemplate;
        private DropDownList AddToNameTemplatePropSelect;
        private Literal ChangeObjectClassLabel;
        private DropDownList ChangeObjectClassSelect;
        //private Literal Spacer1;
        private Literal Spacer2;
        //private Literal Spacer3;
        //private Literal Spacer4;
        private Literal CopiedNodeTypeNameLabel;
        private TextBox CopiedNodeTypeName;
        private CheckBox LockedCheckbox;
        private TableRow _ViewXmlRow;
        private CswViewStructureTree _RelationshipViewTree;
        private Label _WarningLabel;
        private LinkButton _LayoutLink;
        private LinkButton _DefaultViewLink;

        private void create_EditNodeTypePage( CswAutoTable TabTable )
        {
            NodeTypeName = new Literal();
            NodeTypeName.Text = LabelNodeType + " Name:";

            EditNodeTypeName = new TextBox();
            EditNodeTypeName.ID = "EditNodeTypeName_" + _SelectedValue;
            EditNodeTypeName.CssClass = "textinput";

            NodeTypeObjectClassLabel = new Literal();
            NodeTypeObjectClassLabel.Text = "Object Class:";

            NodeTypeObjectClass = new Literal();
            NodeTypeObjectClass.Text = "";

            NodeTypeCategoryLabel = new Literal();
            NodeTypeCategoryLabel.Text = "Category:";

            EditNodeTypeCategory = new TextBox();
            EditNodeTypeCategory.ID = "EditNodeTypeCategory_" + _SelectedValue;
            EditNodeTypeCategory.CssClass = "textinput";

            IconLabel = new Literal();
            IconLabel.Text = "Icon:";

            IconSelect = new DropDownList();
            IconSelect.ID = "IconSelect_" + _SelectedValue;
            IconSelect.CssClass = "selectinput";
            IconSelect.AutoPostBack = false;

            IconImage = new Image();
            IconImage.ID = "IconImage_" + _SelectedValue;

            NameTemplateLabel = new Literal();
            NameTemplateLabel.Text = "Name Template:";

            NameTemplate = new TextBox();
            NameTemplate.ID = "NameTemplate_" + _SelectedValue;
            NameTemplate.CssClass = "textinput";

            AddToNameTemplatePropSelect = new DropDownList();
            AddToNameTemplatePropSelect.ID = "AddToNameTemplatePropSelect_" + _SelectedValue;
            AddToNameTemplatePropSelect.CssClass = "selectinput";
            AddToNameTemplatePropSelect.AutoPostBack = false;

            LockedCheckbox = new CheckBox();
            LockedCheckbox.ID = "locked" + _SelectedValue;
            LockedCheckbox.Text = "Locked";
            LockedCheckbox.EnableViewState = false;

            //Spacer1 = new Literal();
            //Spacer1.Text = "&nbsp;";

            Spacer2 = new Literal();
            Spacer2.Text = "<hr class=\"designhr\">";

            //Spacer3 = new Literal();
            //Spacer3.Text = "&nbsp;";

            ChangeObjectClassLabel = new Literal();
            ChangeObjectClassLabel.Text = "Convert to Object Class:";

            ChangeObjectClassSelect = new DropDownList();
            ChangeObjectClassSelect.ID = "ChangeObjectClassSelect_" + _SelectedValue;
            ChangeObjectClassSelect.CssClass = "selectinput";
            ChangeObjectClassSelect.AutoPostBack = false;

            CopiedNodeTypeNameLabel = new Literal();
            CopiedNodeTypeNameLabel.Text = "Copy " + LabelNodeType + " As:";

            CopiedNodeTypeName = new TextBox();
            CopiedNodeTypeName.ID = "CopiedNodeTypeName_" + _SelectedValue;
            CopiedNodeTypeName.CssClass = "textinput";

            //Spacer4 = new Literal();
            //Spacer4.Text = "&nbsp;";

            _LayoutLink = new LinkButton();
            _LayoutLink.ID = "layoutlink_" + _SelectedValue;
            _LayoutLink.Text = "Edit Property Layout";
            _LayoutLink.Click += new EventHandler( _LayoutLink_Click );

            _DefaultViewLink = new LinkButton();
            _DefaultViewLink.ID = "defaultviewlink_" + _SelectedValue;
            _DefaultViewLink.Text = "Create and Load Default View";
            _DefaultViewLink.Click += new EventHandler( _DefaultViewLink_Click );

            TabTable.addControl( 0, 0, NodeTypeName );
            TabTable.addControl( 0, 1, EditNodeTypeName );
            TabTable.addControl( 1, 0, NodeTypeObjectClassLabel );
            TabTable.addControl( 1, 1, NodeTypeObjectClass );
            TabTable.addControl( 2, 0, NodeTypeCategoryLabel );
            TabTable.addControl( 2, 1, EditNodeTypeCategory );
            TabTable.addControl( 3, 0, IconLabel );
            TabTable.addControl( 3, 1, IconSelect );
            TabTable.addControl( 3, 1, IconImage );
            TabTable.addControl( 4, 0, NameTemplateLabel );
            TabTable.addControl( 4, 1, NameTemplate );
            TabTable.addControl( 4, 1, AddToNameTemplatePropSelect );
            TabTable.addControl( 5, 1, LockedCheckbox );
            TabTable.addControl( 6, 1, _SaveButton );
            TabTable.addControl( 7, 0, new CswLiteralNbsp() );
            TableCell SpacerCell = TabTable.getCell( 8, 0 );
            SpacerCell.ColumnSpan = 2;
            SpacerCell.Controls.Add( Spacer2 );
            TabTable.addControl( 9, 0, CopiedNodeTypeNameLabel );
            TabTable.addControl( 9, 1, CopiedNodeTypeName );
            TabTable.addControl( 10, 1, _CopyNodeTypeButton );
            TabTable.addControl( 11, 0, new CswLiteralNbsp() );
            TabTable.addControl( 12, 0, ChangeObjectClassLabel );
            TabTable.addControl( 12, 1, ChangeObjectClassSelect );
            TabTable.addControl( 13, 1, _ChangeObjectClassButton );
            TabTable.addControl( 14, 0, new CswLiteralNbsp() );
            TabTable.addControl( 15, 1, _LayoutLink );
            TabTable.addControl( 16, 1, _DefaultViewLink );
        }



        private void init_EditNodeTypePage()
        {
            ChangeObjectClassSelect.Items.Clear();
            foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
            {
                ListItem ObjectClassItem = new ListItem( ObjectClass.ObjectClass.ToString(),
                                                         ObjectClass.ObjectClassId.ToString() );
                ChangeObjectClassSelect.Items.Add( ObjectClassItem );
            }

            // Icon select box
            IconSelect.Items.Clear();
            ListItem BlankIconItem = new ListItem( "", "blank.gif" );
            IconSelect.Items.Add( BlankIconItem );
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo( HttpContext.Current.Request.PhysicalApplicationPath + _IconImageRoot );
            System.IO.FileInfo[] IconFiles = d.GetFiles();
            foreach( System.IO.FileInfo IconFile in IconFiles )
            {
                ListItem IconItem = new ListItem( IconFile.Name, IconFile.Name );
                IconSelect.Items.Add( IconItem );
            }
            IconSelect.Attributes.Add( "onchange", IconImage.ClientID + ".src = '" + _IconImageRoot + "/'+ this.value;" );

            ChangeObjectClassLabel.Visible = false;
            ChangeObjectClassSelect.Visible = false;
            _ChangeObjectClassButton.Visible = false;
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType && CswConvert.ToInt32( _SelectedValue ) > 0 )
            {
                CswNbtMetaDataObjectClass ObjectClass = SelectedNodeType.ObjectClass;
                if( ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass )
                {
                    ChangeObjectClassLabel.Visible = true;
                    ChangeObjectClassSelect.Visible = true;
                    _ChangeObjectClassButton.Visible = true;
                    ChangeObjectClassSelect.SelectedValue = ObjectClass.ObjectClassId.ToString();
                }
                // NodeTypeName Edit Box
                EditNodeTypeName.Text = SelectedNodeType.NodeTypeName;

                // EditNodeTypeCategory Edit Box
                EditNodeTypeCategory.Text = SelectedNodeType.Category;

                // Object Class Label
                NodeTypeObjectClass.Text = ObjectClass.ObjectClass.ToString();


                // Selected Icon
                string SelectedIcon = SelectedNodeType.IconFileName;
                if( IconSelect.Items.Count > 0 && SelectedIcon != String.Empty )
                    IconSelect.SelectedIndex = IconSelect.Items.IndexOf( IconSelect.Items.FindByValue( SelectedIcon ) );
                else
                    IconSelect.SelectedIndex = 0;
                if( IconSelect.SelectedValue != String.Empty )
                    IconImage.ImageUrl = _IconImageRoot + "/" + IconSelect.SelectedValue;
                else
                    IconImage.ImageUrl = _IconImageRoot + "/" + "blank.gif";

                // NameTemplate Textbox Value
                NameTemplate.Text = SelectedNodeType.NameTemplateText;

                // Prop select options
                AddToNameTemplatePropSelect.Items.Clear();
                AddToNameTemplatePropSelect.Items.Add( new ListItem( "Select " + LabelNodeTypeProp + " To Add...", "" ) );
                foreach( CswNbtMetaDataNodeTypeProp Prop in SelectedNodeType.NodeTypeProps )
                {
                    ListItem AddToNameTemplatePropItem = new ListItem( Prop.PropName, Prop.PropName );
                    AddToNameTemplatePropSelect.Items.Add( AddToNameTemplatePropItem );
                }

                if( AddToNameTemplatePropSelect.Items.Count == 1 )  // no properties!
                    AddToNameTemplatePropSelect.Visible = false;
                else
                    AddToNameTemplatePropSelect.Visible = true;

                if( _Mode == NbtDesignMode.Inspection )
                {
                    // BZ 7742
                    CopiedNodeTypeNameLabel.Visible = false;
                    CopiedNodeTypeName.Visible = false;
                    _CopyNodeTypeButton.Visible = false;
                    Spacer2.Visible = false;
                }

                LockedCheckbox.Checked = SelectedNodeType.IsLocked;
            }

            if( _Mode == NbtDesignMode.Inspection )
            {
                NodeTypeObjectClassLabel.Visible = false;
                NodeTypeObjectClass.Visible = false;
            }
        }

        private string _ReturnURLForQueryString()
        {
            string returnurl = "Design.aspx";
            if( Request.QueryString[QueryStringVarName_Mode] != null )
                returnurl = CswTools.UrlToQueryStringParam( "Design.aspx?" + QueryStringVarName_Mode + "=" + Request.QueryString[QueryStringVarName_Mode] );
            return returnurl;
        }

        private Literal EditTabNameLabel;
        private TextBox EditTabNameTextBox;
        private Literal EditTabOrderLabel;
        private TextBox EditTabOrderTextBox;
        private Literal EditTabIncludeInNodeReportLabel;
        private CheckBox EditTabIncludeInNodeReport;

        private void create_EditTabPage( CswAutoTable TabTable )
        {
            EditTabNameLabel = new Literal();
            EditTabNameLabel.Text = "Name:";

            EditTabNameTextBox = new TextBox();
            EditTabNameTextBox.ID = "EditTabNameTextBox_" + _SelectedValue;
            EditTabNameTextBox.CssClass = "textinput";

            EditTabOrderLabel = new Literal();
            if( _Mode == NbtDesignMode.Inspection )
                EditTabOrderLabel.Text = "Number:";
            else
                EditTabOrderLabel.Text = "Order:";

            EditTabOrderTextBox = new TextBox();
            EditTabOrderTextBox.ID = "EditTabOrderTextBox_" + _SelectedValue;
            EditTabOrderTextBox.CssClass = "textinput";

            EditTabIncludeInNodeReportLabel = new Literal();
            EditTabIncludeInNodeReportLabel.Text = "Include in Node Report:";

            EditTabIncludeInNodeReport = new CheckBox();
            EditTabIncludeInNodeReport.ID = "EditTabIncludeInNodeReport_" + _SelectedValue;

            TabTable.addControl( 0, 0, EditTabNameLabel );
            TabTable.addControl( 0, 1, EditTabNameTextBox );
            TabTable.addControl( 1, 0, EditTabOrderLabel );
            TabTable.addControl( 1, 1, EditTabOrderTextBox );
            TabTable.addControl( 2, 0, EditTabIncludeInNodeReportLabel );
            TabTable.addControl( 2, 1, EditTabIncludeInNodeReport );
            TabTable.addControl( 3, 1, _SaveButton );
        }

        private void init_EditTabPage()
        {
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab &&
                CswConvert.ToInt32( _SelectedValue ) > 0 )
            {
                EditTabNameTextBox.Text = SelectedNodeTypeTab.TabName;
                if( SelectedNodeTypeTab.TabOrder != Int32.MinValue )
                    EditTabOrderTextBox.Text = SelectedNodeTypeTab.TabOrder.ToString();
                else
                    EditTabOrderTextBox.Text = string.Empty;
                EditTabIncludeInNodeReport.Checked = SelectedNodeTypeTab.IncludeInNodeReport;
            }
        }


        private Literal EditPropTabLabel;
        private DropDownList EditPropTabSelect;
        private PlaceHolder EditPropPlaceHolder;

        private void create_EditPropertyPage( CswAutoTable TabTable )
        {
            EditPropTabLabel = new Literal();
            EditPropTabLabel.Text = "Display On " + LabelNodeTypeTab + ":";

            EditPropTabSelect = new DropDownList();
            EditPropTabSelect.ID = "EditPropTabSelect" + SelectedNodeTypeProp.PropId;
            EditPropTabSelect.CssClass = "selectinput";
            EditPropTabSelect.AutoPostBack = false;

            EditPropPlaceHolder = new PlaceHolder();
            EditPropPlaceHolder.ID = "EditPropPlaceHolder";

            TabTable.addControl( 0, 0, EditPropTabLabel );
            TabTable.addControl( 0, 1, EditPropTabSelect );
            TabTable.addControl( 1, 0, EditPropPlaceHolder );
            TabTable.addControl( 2, 1, _SaveButton );
        }

        private const String ChkBoxArrayValueColumnName = "Compliant";

        private void init_EditPropertyPage()
        {
            _SaveButton.Visible = false;
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property )
            {
                if( CswConvert.ToInt32( _SelectedValue ) > 0 )
                {
                    // Tab Select
                    EditPropTabSelect.Items.Clear();
                    foreach( CswNbtMetaDataNodeTypeTab Tab in SelectedNodeType.NodeTypeTabs )
                    {
                        EditPropTabSelect.Items.Add( new ListItem( Tab.TabName, Tab.TabName ) );
                    }

                    if( CswConvert.ToInt32( _SelectedValue ) > 0 )
                    {
                        // Edit Property Select box
                        if( SelectedNodeTypeProp != null && SelectedNodeTypeProp.NodeTypeTab != null )
                        {
                            EditPropTabSelect.SelectedValue = SelectedNodeTypeProp.NodeTypeTab.TabName;
                        }
                    }
                    _SaveButton.Visible = true;

                    // BZ 7415
                    if( !SelectedNodeTypeProp.EditTabEnabled )
                        EditPropTabSelect.Attributes.Add( "disabled", "true" );

                    // TODO: IS THIS NECESSARY?
                    EditPropPlaceHolder.Controls.Clear();      // DANGER!

                    CswNbtMetaDataFieldType FieldType = SelectedNodeTypeProp.FieldType;
                    CswNbtMetaDataObjectClassProp ObjectClassProp = SelectedNodeTypeProp.ObjectClassProp;
                    bool DerivesFromObjectClassProp = false;
                    if( SelectedNodeTypeProp.ObjectClassProp != null )
                        DerivesFromObjectClassProp = true;

                    if( DerivesFromObjectClassProp )
                    {
                        TableRow OrigNameRow = makeEditPropTableRow( EditPropPlaceHolder );
                        if( _Mode == NbtDesignMode.Inspection )
                            ( (Literal) OrigNameRow.Cells[0].Controls[0] ).Text = "Original Question:";
                        else
                            ( (Literal) OrigNameRow.Cells[0].Controls[0] ).Text = "Original Name:";
                        Literal OrigNameValue = new Literal();
                        //OrigNameValue.ID = "EditProp_OrigNameValue" + SelectedNodeTypeProp.PropId.ToString();
                        OrigNameValue.Text = SelectedNodeTypeProp.ObjectClassProp.PropName;
                        OrigNameRow.Cells[1].Controls.Add( OrigNameValue );
                    }

                    TableRow NameRow = makeEditPropTableRow( EditPropPlaceHolder );
                    if( _Mode == NbtDesignMode.Inspection )
                        ( (Literal) NameRow.Cells[0].Controls[0] ).Text = "Question:";
                    else
                        ( (Literal) NameRow.Cells[0].Controls[0] ).Text = "Name:";
                    TextBox NameValue = new TextBox();
                    NameValue.CssClass = "textinput";
                    NameValue.ID = "EditProp_NameValue" + SelectedNodeTypeProp.PropId.ToString();
                    NameValue.Text = SelectedNodeTypeProp.PropName;
                    NameValue.Width = Unit.Parse( "400px" );
                    NameValue.MaxLength = 512;
                    NameRow.Cells[1].Controls.Add( NameValue );

                    TableRow DisplayColRow = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) DisplayColRow.Cells[0].Controls[0] ).Text = LabelNodeTypeTab + " Display Column:";
                    TextBox DisplayColValue = new TextBox();
                    DisplayColValue.CssClass = "textinput";
                    DisplayColValue.ID = "EditProp_DisplayColValue" + SelectedNodeTypeProp.PropId.ToString();
                    DisplayColValue.Text = SelectedNodeTypeProp.DisplayColumn.ToString();
                    DisplayColRow.Cells[1].Controls.Add( DisplayColValue );

                    TableRow DisplayRowRow = makeEditPropTableRow( EditPropPlaceHolder );
                    if( _Mode == NbtDesignMode.Inspection )
                        ( (Literal) DisplayRowRow.Cells[0].Controls[0] ).Text = "Question Order:";
                    else
                        ( (Literal) DisplayRowRow.Cells[0].Controls[0] ).Text = LabelNodeTypeTab + "  Display Row:";
                    TextBox DisplayRowValue = new TextBox();
                    DisplayRowValue.CssClass = "textinput";
                    DisplayRowValue.ID = "EditProp_DisplayRowValue" + SelectedNodeTypeProp.PropId.ToString();
                    DisplayRowValue.Text = SelectedNodeTypeProp.DisplayRow.ToString();
                    DisplayRowRow.Cells[1].Controls.Add( DisplayRowValue );

                    TableRow DisplayColAdd = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) DisplayColAdd.Cells[0].Controls[0] ).Text = "Add Display Column:";
                    TextBox DisplayColAddValue = new TextBox();
                    DisplayColAddValue.CssClass = "textinput";
                    DisplayColAddValue.ID = "EditProp_DisplayColAddValue" + SelectedNodeTypeProp.PropId.ToString();
                    if( SelectedNodeTypeProp.DisplayColAdd != Int32.MinValue )
                        DisplayColAddValue.Text = SelectedNodeTypeProp.DisplayColAdd.ToString();
                    DisplayColAdd.Cells[1].Controls.Add( DisplayColAddValue );

                    TableRow DisplayRowAdd = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) DisplayRowAdd.Cells[0].Controls[0] ).Text = "Add Display Row:";
                    TextBox DisplayRowAddValue = new TextBox();
                    DisplayRowAddValue.CssClass = "textinput";
                    DisplayRowAddValue.ID = "EditProp_DisplayRowAddValue" + SelectedNodeTypeProp.PropId.ToString();
                    if( SelectedNodeTypeProp.DisplayRowAdd != Int32.MinValue )
                        DisplayRowAddValue.Text = SelectedNodeTypeProp.DisplayRowAdd.ToString();
                    DisplayRowAdd.Cells[1].Controls.Add( DisplayRowAddValue );

                    if( _Mode == NbtDesignMode.Inspection )
                    {
                        ( (Literal) DisplayColRow.Cells[0].Controls[0] ).Visible = false;
                        ( (Literal) DisplayColAdd.Cells[0].Controls[0] ).Visible = false;
                        ( (Literal) DisplayRowAdd.Cells[0].Controls[0] ).Visible = false;
                        DisplayColValue.Style.Add( HtmlTextWriterStyle.Display, "none" );
                        DisplayColAddValue.Style.Add( HtmlTextWriterStyle.Display, "none" );
                        DisplayRowAddValue.Style.Add( HtmlTextWriterStyle.Display, "none" );
                    }

                    TableRow FieldRow = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) FieldRow.Cells[0].Controls[0] ).Text = "Field Type:";
                    Literal FieldTypeValueLabel = new Literal();
                    FieldTypeValueLabel.Text = FieldType.FieldType.ToString();
                    FieldRow.Cells[1].Controls.Add( FieldTypeValueLabel );

                    switch( FieldType.FieldType )
                    {

                        case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                            TableRow BarcodeRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) BarcodeRow.Cells[0].Controls[0] ).Text = "Sequence:";
                            CswSequencesEditor BarcodeEditor = new CswSequencesEditor( Master.CswNbtResources, Master.AjaxManager, SelectedNodeTypeProp.PropId );
                            BarcodeEditor.ID = "EditProp_SequenceValue" + SelectedNodeTypeProp.PropId.ToString();
                            BarcodeEditor.DataBind();
                            BarcodeRow.Cells[1].Controls.Add( BarcodeEditor );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                            TableRow TemplateRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) TemplateRow.Cells[0].Controls[0] ).Text = "Template:";

                            TextBox TemplateValue = new TextBox();
                            TemplateValue.CssClass = "textinput";
                            TemplateValue.ID = "EditProp_TemplateValue" + SelectedNodeTypeProp.PropId.ToString();
                            TemplateValue.Text = SelectedNodeTypeProp.CompositeTemplateText;
                            TemplateRow.Cells[1].Controls.Add( TemplateValue );

                            DropDownList AddPropToCompositeTemplateSelect = new DropDownList();
                            AddPropToCompositeTemplateSelect.CssClass = "selectinput";
                            AddPropToCompositeTemplateSelect.ID = "EditProp_TemplatePropSelect" + SelectedNodeTypeProp.PropId.ToString();
                            AddPropToCompositeTemplateSelect.Items.Clear();
                            AddPropToCompositeTemplateSelect.Items.Add( new ListItem( "Select " + LabelNodeTypeProp + " To Add...", "" ) );
                            AddPropToCompositeTemplateSelect.Attributes.Add( "onchange", "addPropToTemplate(this, '" + TemplateValue.ClientID + "', '" + CswNbtMetaData._TemplateLeftDelimiter.ToString() + "', '" + CswNbtMetaData._TemplateRightDelimiter.ToString() + "');" );
                            foreach( CswNbtMetaDataNodeTypeProp OtherProp in SelectedNodeTypeProp.NodeType.NodeTypeProps )
                            {
                                if( OtherProp.PropId != SelectedNodeTypeProp.PropId )
                                    AddPropToCompositeTemplateSelect.Items.Add( new ListItem( OtherProp.PropName, OtherProp.PropName ) );
                            }
                            TemplateRow.Cells[1].Controls.Add( AddPropToCompositeTemplateSelect );

                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Date:
                            TableRow DateTodayRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) DateTodayRow.Cells[0].Controls[0] ).Text = "";
                            CheckBox DateTodayValue = new CheckBox();
                            DateTodayValue.ID = "EditProp_DateTodayValue" + SelectedNodeTypeProp.PropId.ToString();
                            DateTodayValue.Text = "Default to Today";
                            DateTodayValue.Checked = SelectedNodeTypeProp.DateToday;
                            DateTodayRow.Cells[1].Controls.Add( DateTodayValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.External:
                            /*
                            < 0.000000e+000lseif(lcase(prop.fieldtype) = "external") then %>
                                <tr class="details">
                                    <td align="right">URL:</td>
                                    <td><input type="text" name="external_url" id="external_url" value="<%=prop.url%>" /></td>
                                </tr>
                                <tr class="details">
                                    <td align="right">Length:</td>
                                    <td><input type="text" name="length" id="length" size=4 value="<%=prop.length%>"/></td>
                                </tr>
                                <tr class="details">
                                    <td align="right">Width:</td>
                                    <td><input type="text" name="width" id="width" size=4 value="<%=prop.width%>"/></td>
                                </tr>
                                <tr class="details"
                                    <td align="right">Field Type:</td>
                                    <td><%=prop.fieldtype%></td>
                                </tr>
                            */
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.File:
                            TableRow FileLengthRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) FileLengthRow.Cells[0].Controls[0] ).Text = "Length:";
                            TextBox FileLengthValue = new TextBox();
                            FileLengthValue.CssClass = "textinput";
                            FileLengthValue.ID = "EditProp_LengthValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.Length != Int32.MinValue )
                                FileLengthValue.Text = SelectedNodeTypeProp.Length.ToString();
                            FileLengthRow.Cells[1].Controls.Add( FileLengthValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                            //TableRow GridWidthRow = makeEditPropTableRow(EditPropHolder);
                            //((Literal)GridWidthRow.Cells[0].Controls[0]).Text = "Width (pixels):";
                            //TextBox GridWidthValue = new TextBox();
                            //GridWidthValue.CssClass = "textinput";
                            //GridWidthValue.ID = "EditProp_ColsValue" + SelectedNodeTypeProp.PropId.ToString();
                            //GridWidthValue.Text = PropRow["textareacols"].ToString();
                            //GridWidthRow.Cells[1].Controls.Add(GridWidthValue);

                            TableRow GridViewXmlRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) GridViewXmlRow.Cells[0].Controls[0] ).Text = "View:";
                            //UpdatePanel GridViewXmlUpdatePanel = new UpdatePanel();
                            //GridViewXmlUpdatePanel.ID = "GridViewXmlUpdatePanel";
                            //GridViewXmlRow.Cells[1].Controls.Add(GridViewXmlUpdatePanel);

                            //CswNbtView GridView = new CswNbtView( Master.CswNbtResources );
                            //if( SelectedNodeTypeProp.ViewId != Int32.MinValue )
                            //GridView.LoadXml(SelectedNodeTypeProp.ViewId);
							CswNbtView GridView = Master.CswNbtResources.ViewSelect.restoreView( SelectedNodeTypeProp.ViewId );
                            //else
                            //{
                            //    // This property is missing a view -- make a new one
                            //    GridView.makeNew( SelectedNodeTypeProp.PropName, NbtViewVisibility.Property, Int32.MinValue, Int32.MinValue, Int32.MinValue );
                            //    setPropertyViewId( CswConvert.ToInt32( SelectedNodeTypeProp.PropId.ToString() ), GridView.ViewId );
                            //}

                            CswViewStructureTree GridViewTree = new CswViewStructureTree( Master.CswNbtResources );
                            GridViewTree.ID = "GridViewTree";
                            GridViewTree.OnError += new CswErrorHandler( Master.HandleError );
                            GridViewTree.reinitTreeFromView( GridView, null, null, CswViewStructureTree.ViewTreeSelectType.None );
                            GridViewXmlRow.Cells[1].Controls.Add( GridViewTree );

                            Button EditGridViewButton = new Button();
                            EditGridViewButton.ID = "EditGridViewButton";
                            EditGridViewButton.CssClass = "Button";
                            EditGridViewButton.OnClientClick = "window.location='EditView.aspx?step=2&viewid=" + GridView.ViewId + "&return=" + _ReturnURLForQueryString() + "';";
                            EditGridViewButton.Text = "Edit View";
                            GridViewXmlRow.Cells[1].Controls.Add( EditGridViewButton );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Image:
                            TableRow HeightRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) HeightRow.Cells[0].Controls[0] ).Text = "Height (pixels):";
                            TextBox HeightValue = new TextBox();
                            HeightValue.CssClass = "textinput";
                            HeightValue.ID = "EditProp_RowsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaRows != Int32.MinValue )
                                HeightValue.Text = SelectedNodeTypeProp.TextAreaRows.ToString();
                            HeightRow.Cells[1].Controls.Add( HeightValue );

                            TableRow WidthRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) WidthRow.Cells[0].Controls[0] ).Text = "Width (pixels):";
                            TextBox WidthValue = new TextBox();
                            WidthValue.CssClass = "textinput";
                            WidthValue.ID = "EditProp_ColsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaColumns != Int32.MinValue )
                                WidthValue.Text = SelectedNodeTypeProp.TextAreaColumns.ToString();
                            WidthRow.Cells[1].Controls.Add( WidthValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.List:
                            TableRow OptionsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) OptionsRow.Cells[0].Controls[0] ).Text = "Options:";
                            TextBox OptionsValue = new TextBox();
                            if( DerivesFromObjectClassProp && ObjectClassProp.ListOptions != String.Empty )
                                OptionsValue.Enabled = false;
                            OptionsValue.CssClass = "textinput";
                            OptionsValue.ID = "EditProp_OptionsValue" + SelectedNodeTypeProp.PropId.ToString();
                            OptionsValue.Text = SelectedNodeTypeProp.ListOptions;
                            OptionsRow.Cells[1].Controls.Add( OptionsValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Location:

                            HiddenField LocationIsFk = new HiddenField();
                            LocationIsFk.ID = "EditProp_IsFkValue" + SelectedNodeTypeProp.PropId.ToString();
                            LocationIsFk.Value = "true";
                            EditPropPlaceHolder.Controls.Add( LocationIsFk );

                            HiddenField LocationFkType = new HiddenField();
                            LocationFkType.ID = "EditProp_FkTypeValue" + SelectedNodeTypeProp.PropId.ToString();
                            LocationFkType.Value = "ObjectClassId";
                            EditPropPlaceHolder.Controls.Add( LocationFkType );

                            HiddenField LocationFkValue = new HiddenField();
                            LocationFkValue.ID = "EditProp_FkValueValue" + SelectedNodeTypeProp.PropId.ToString();
                            LocationFkValue.Value = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ).ObjectClassId.ToString();
                            EditPropPlaceHolder.Controls.Add( LocationFkValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                            TableRow LCViewXmlRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) LCViewXmlRow.Cells[0].Controls[0] ).Text = "View:";
							CswNbtView LCView = Master.CswNbtResources.ViewSelect.restoreView( SelectedNodeTypeProp.ViewId );

                            CswViewStructureTree LCViewTree = new CswViewStructureTree( Master.CswNbtResources );
                            LCViewTree.ID = "LCViewTree";
                            LCViewTree.OnError += new CswErrorHandler( Master.HandleError );
                            LCViewTree.reinitTreeFromView( LCView, null, null, CswViewStructureTree.ViewTreeSelectType.None );
                            LCViewXmlRow.Cells[1].Controls.Add( LCViewTree );

                            Button EditLCViewButton = new Button();
                            EditLCViewButton.ID = "EditLCViewButton";
                            EditLCViewButton.CssClass = "Button";
                            EditLCViewButton.OnClientClick = "window.location='EditView.aspx?step=2&viewid=" + LCView.ViewId + "&return=" + _ReturnURLForQueryString() + "';";
                            EditLCViewButton.Text = "Edit View";
                            LCViewXmlRow.Cells[1].Controls.Add( EditLCViewButton );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                            TableRow LSRowsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) LSRowsRow.Cells[0].Controls[0] ).Text = "Rows:";
                            TextBox LSRowsValue = new TextBox();
                            LSRowsValue.CssClass = "textinput";
                            LSRowsValue.ID = "EditProp_RowsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaRows != Int32.MinValue )
                                LSRowsValue.Text = SelectedNodeTypeProp.TextAreaRows.ToString();
                            LSRowsRow.Cells[1].Controls.Add( LSRowsValue );

                            TableRow LSOptionsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) LSOptionsRow.Cells[0].Controls[0] ).Text = "Y Options:";
                            TextBox LSOptionsValue = new TextBox();
                            if( DerivesFromObjectClassProp && ( ObjectClassProp.ServerManaged ||
                                                                ObjectClassProp.ListOptions != String.Empty ||
                                                                ObjectClassProp.FKType == "fkeydefid" ) )
                            {
                                LSOptionsValue.Enabled = false;
                            }
                            LSOptionsValue.CssClass = "textinput";
                            LSOptionsValue.ID = "EditProp_OptionsValue" + SelectedNodeTypeProp.PropId.ToString();
                            LSOptionsValue.Text = SelectedNodeTypeProp.ListOptions;
                            LSOptionsRow.Cells[1].Controls.Add( LSOptionsValue );

                            TableRow LSValueOptionsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) LSValueOptionsRow.Cells[0].Controls[0] ).Text = "X Options:";
                            TextBox LSValueOptionsValue = new TextBox();
                            if( DerivesFromObjectClassProp && ( ObjectClassProp.ServerManaged || ObjectClassProp.ValueOptions != String.Empty ) )
                                LSValueOptionsValue.Enabled = false;
                            LSValueOptionsValue.CssClass = "textinput";
                            LSValueOptionsValue.ID = "EditProp_ValueOptionsValue" + SelectedNodeTypeProp.PropId.ToString();
                            LSValueOptionsValue.Text = SelectedNodeTypeProp.ValueOptions;
                            LSValueOptionsRow.Cells[1].Controls.Add( LSValueOptionsValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                            TableRow RowsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) RowsRow.Cells[0].Controls[0] ).Text = "Rows:";
                            TextBox RowsValue = new TextBox();
                            RowsValue.CssClass = "textinput";
                            RowsValue.ID = "EditProp_RowsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaRows != Int32.MinValue )
                                RowsValue.Text = SelectedNodeTypeProp.TextAreaRows.ToString();
                            RowsRow.Cells[1].Controls.Add( RowsValue );

                            TableRow ColsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) ColsRow.Cells[0].Controls[0] ).Text = "Columns:";
                            TextBox ColsValue = new TextBox();
                            ColsValue.CssClass = "textinput";
                            ColsValue.ID = "EditProp_ColsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaColumns != Int32.MinValue )
                                ColsValue.Text = SelectedNodeTypeProp.TextAreaColumns.ToString();
                            ColsRow.Cells[1].Controls.Add( ColsValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                            TableRow MTBFDateTodayRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) MTBFDateTodayRow.Cells[0].Controls[0] ).Text = "";
                            CheckBox MTBFDateTodayValue = new CheckBox();
                            MTBFDateTodayValue.ID = "EditProp_DateTodayValue" + SelectedNodeTypeProp.PropId.ToString();
                            MTBFDateTodayValue.Text = "Default to Today";
                            MTBFDateTodayValue.Checked = SelectedNodeTypeProp.DateToday;
                            MTBFDateTodayRow.Cells[1].Controls.Add( MTBFDateTodayValue );
                            break;

                        //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                        //    TableRow MultiTargetRow = makeEditPropTableRow( EditPropPlaceHolder );
                        //    ( (Literal) MultiTargetRow.Cells[0].Controls[0] ).Text = "Target:";
                        //    DropDownList MultiTargetValue = new DropDownList();
                        //    if( DerivesFromObjectClassProp && ObjectClassProp.FKType != String.Empty )
                        //        MultiTargetValue.Enabled = false;
                        //    MultiTargetValue.CssClass = "selectinput";
                        //    MultiTargetValue.ID = "EditProp_TargetValue" + SelectedNodeTypeProp.PropId.ToString();
                        //    foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.NodeTypes )
                        //    {
                        //        ListItem Item = new ListItem( NodeType.NodeTypeName, "nt_" + NodeType.NodeTypeId.ToString() );
                        //        if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() && SelectedNodeTypeProp.FKValue == NodeType.NodeTypeId )
                        //            Item.Selected = true;
                        //        MultiTargetValue.Items.Add( Item );
                        //    }
                        //    foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
                        //    {
                        //        ListItem Item = new ListItem( "{" + ObjectClass.ObjectClass + "}", "oc_" + ObjectClass.ObjectClassId.ToString() );
                        //        if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && SelectedNodeTypeProp.FKValue == ObjectClass.ObjectClassId )
                        //            Item.Selected = true;
                        //        MultiTargetValue.Items.Add( Item );
                        //    }
                        //    MultiTargetRow.Cells[1].Controls.Add( MultiTargetValue );

                        //    TableRow MultiViewXmlRow = makeEditPropTableRow( EditPropPlaceHolder );
                        //    ( (Literal) MultiViewXmlRow.Cells[0].Controls[0] ).Text = "View:";
                        //    //UpdatePanel ViewXmlUpdatePanel = new UpdatePanel();
                        //    //ViewXmlUpdatePanel.ID = "ViewXmlUpdatePanel";
                        //    //ViewXmlRow.Cells[1].Controls.Add(ViewXmlUpdatePanel);
                        //    CswNbtView MultiRelationshipView = new CswNbtView( Master.CswNbtResources );
                        //    if( SelectedNodeTypeProp.ViewId != Int32.MinValue )
                        //        //RelationshipView.LoadXml(SelectedNodeTypeProp.ViewId);
                        //        MultiRelationshipView = (CswNbtView) CswNbtViewFactory.restoreView( Master.CswNbtResources, SelectedNodeTypeProp.ViewId );
                        //    else
                        //    {
                        //        // This property is missing a view -- make a new one
                        //        MultiRelationshipView.makeNew( SelectedNodeTypeProp.PropName, NbtViewVisibility.Property, Int32.MinValue, Int32.MinValue, Int32.MinValue );
                        //        setPropertyViewId( SelectedNodeTypeProp.PropId, MultiRelationshipView.ViewId );
                        //    }

                        //    CswViewStructureTree MultiRelationshipViewTree = new CswViewStructureTree( Master.CswNbtResources );
                        //    MultiRelationshipViewTree.ID = "RelationshipViewTree";
                        //    MultiRelationshipViewTree.reinitTreeFromView( MultiRelationshipView, null, null, CswViewStructureTree.ViewTreeSelectType.None );
                        //    MultiViewXmlRow.Cells[1].Controls.Add( MultiRelationshipViewTree );

                        //    Button MultiEditRelationshipViewButton = new Button();
                        //    MultiEditRelationshipViewButton.ID = "EditRelationshipViewButton";
                        //    MultiEditRelationshipViewButton.CssClass = "Button";
                        //    MultiEditRelationshipViewButton.OnClientClick = "window.location='EditView.aspx?step=2&viewid=" + MultiRelationshipView.ViewId + "&return=Design.aspx';";
                        //    MultiEditRelationshipViewButton.Text = "Edit View";
                        //    MultiViewXmlRow.Cells[1].Controls.Add( MultiEditRelationshipViewButton );
                        //    break;


                        case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                            TableRow SelectModeRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) SelectModeRow.Cells[0].Controls[0] ).Text = "Select Mode:";
                            DropDownList SelectModeValue = new DropDownList();
                            foreach( string NodeTypeSelectModeName in Enum.GetNames( typeof( PropertySelectMode ) ) )
                            {
                                if( NodeTypeSelectModeName != PropertySelectMode.Blank.ToString() )
                                    SelectModeValue.Items.Add( new ListItem( NodeTypeSelectModeName, NodeTypeSelectModeName ) );
                            }
                            SelectModeValue.CssClass = "selectinput";
                            SelectModeValue.ID = "EditProp_MultiValue" + SelectedNodeTypeProp.PropId.ToString();
                            SelectModeValue.SelectedValue = SelectedNodeTypeProp.Multi.ToString();
                            if( DerivesFromObjectClassProp && ObjectClassProp.Multi.ToString() != String.Empty )
                                SelectModeValue.Enabled = false;
                            SelectModeRow.Cells[1].Controls.Add( SelectModeValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                            TableRow PrecisionRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) PrecisionRow.Cells[0].Controls[0] ).Text = "Precision:";
                            TextBox PrecisionValue = new TextBox();
                            PrecisionValue.CssClass = "textinput";
                            PrecisionValue.ID = "EditProp_PrecisionValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.NumberPrecision != Int32.MinValue )
                                PrecisionValue.Text = SelectedNodeTypeProp.NumberPrecision.ToString();
                            PrecisionRow.Cells[1].Controls.Add( PrecisionValue );

                            TableRow MinValueRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) MinValueRow.Cells[0].Controls[0] ).Text = "Minimum Value:";
                            TextBox MinValue = new TextBox();
                            MinValue.CssClass = "textinput";
                            MinValue.ID = "EditProp_MinValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( !Double.IsNaN( SelectedNodeTypeProp.MinValue ) )
                                MinValue.Text = SelectedNodeTypeProp.MinValue.ToString();
                            MinValueRow.Cells[1].Controls.Add( MinValue );

                            TableRow MaxValueRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) MaxValueRow.Cells[0].Controls[0] ).Text = "Maximum Value:";
                            TextBox MaxValue = new TextBox();
                            MaxValue.CssClass = "textinput";
                            MaxValue.ID = "EditProp_MaxValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( !Double.IsNaN( SelectedNodeTypeProp.MaxValue ) )
                                MaxValue.Text = SelectedNodeTypeProp.MaxValue.ToString();
                            MaxValueRow.Cells[1].Controls.Add( MaxValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                            TableRow RelationshipRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) RelationshipRow.Cells[0].Controls[0] ).Text = "Relationship:";

                            HiddenField PropertyRefIsFk = new HiddenField();
                            PropertyRefIsFk.ID = "EditProp_IsFkValue" + SelectedNodeTypeProp.PropId.ToString();
                            PropertyRefIsFk.Value = "true";
                            EditPropPlaceHolder.Controls.Add( PropertyRefIsFk );

                            HiddenField PropertyRefFkType = new HiddenField();
                            PropertyRefFkType.ID = "EditProp_FkTypeValue" + SelectedNodeTypeProp.PropId.ToString();
                            PropertyRefFkType.Value = "NodeTypePropId";
                            RelationshipRow.Cells[1].Controls.Add( PropertyRefFkType );

                            DropDownList RelationshipValue = new DropDownList();
                            RelationshipValue.Items.Add( new ListItem( string.Empty, string.Empty ) );
                            foreach( CswNbtMetaDataNodeTypeProp OtherProp in SelectedNodeTypeProp.NodeType.NodeTypeProps )
                            {
                                if( OtherProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                                    RelationshipValue.Items.Add( new ListItem( OtherProp.PropName, OtherProp.PropId.ToString() ) );
                            }
                            RelationshipValue.CssClass = "selectinput";
                            RelationshipValue.ID = "EditProp_FkValueValue" + SelectedNodeTypeProp.PropId.ToString();
                            RelationshipRow.Cells[1].Controls.Add( RelationshipValue );

                            if( SelectedNodeTypeProp.FKValue != Int32.MinValue )
                            {
                                RelationshipValue.SelectedValue = SelectedNodeTypeProp.FKValue.ToString();

                                CswNbtMetaDataNodeTypeProp RelationshipProp = Master.CswNbtResources.MetaData.getNodeTypeProp( SelectedNodeTypeProp.FKValue );

                                TableRow RelatedPropRow = makeEditPropTableRow( EditPropPlaceHolder );
                                ( (Literal) RelatedPropRow.Cells[0].Controls[0] ).Text = "Related " + LabelNodeTypeProp + ":";
                                DropDownList RelatedPropValue = new DropDownList();

                                HiddenField RelatedPropType = new HiddenField();
                                RelatedPropType.ID = "EditProp_RelatedPropType" + SelectedNodeTypeProp.PropId.ToString();
                                RelatedPropRow.Cells[1].Controls.Add( RelatedPropType );

                                ICollection RelatedProps = null;
                                if( RelationshipProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                                {
                                    CswNbtMetaDataNodeType RelatedNodeType = Master.CswNbtResources.MetaData.getNodeType( RelationshipProp.FKValue );
                                    if( RelatedNodeType != null )
                                    {
                                        RelatedPropType.Value = CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString();
                                        RelatedProps = RelatedNodeType.NodeTypeProps;
                                    }
                                }
                                else
                                {
                                    CswNbtMetaDataObjectClass RelatedObjectClass = Master.CswNbtResources.MetaData.getObjectClass( RelationshipProp.FKValue );
                                    if( RelatedObjectClass != null )
                                    {
                                        RelatedPropType.Value = CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString();
                                        RelatedProps = RelatedObjectClass.ObjectClassProps;
                                    }
                                }
                                RelatedPropValue.Items.Add( new ListItem( string.Empty, string.Empty ) );
                                if( RelatedProps != null )
                                {
                                    foreach( ICswNbtMetaDataProp RelatedProp in RelatedProps )
                                        RelatedPropValue.Items.Add( new ListItem( RelatedProp.PropName, RelatedProp.PropId.ToString() ) );
                                }
                                RelatedPropValue.CssClass = "selectinput";
                                RelatedPropValue.ID = "EditProp_RelatedPropValue" + SelectedNodeTypeProp.PropId.ToString();
                                if( SelectedNodeTypeProp.ValuePropId != Int32.MinValue )
                                    RelatedPropValue.SelectedValue = SelectedNodeTypeProp.ValuePropId.ToString();
                                RelatedPropRow.Cells[1].Controls.Add( RelatedPropValue );
                            }
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                            TableRow QtyPrecisionRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) QtyPrecisionRow.Cells[0].Controls[0] ).Text = "Precision:";
                            TextBox QtyPrecisionValue = new TextBox();
                            QtyPrecisionValue.CssClass = "textinput";
                            QtyPrecisionValue.ID = "EditProp_PrecisionValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.NumberPrecision != Int32.MinValue )
                                QtyPrecisionValue.Text = SelectedNodeTypeProp.NumberPrecision.ToString();
                            QtyPrecisionRow.Cells[1].Controls.Add( QtyPrecisionValue );

                            TableRow QtyMinValueRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) QtyMinValueRow.Cells[0].Controls[0] ).Text = "Minimum Value:";
                            TextBox QtyMinValue = new TextBox();
                            QtyMinValue.CssClass = "textinput";
                            QtyMinValue.ID = "EditProp_MinValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( !Double.IsNaN( SelectedNodeTypeProp.MinValue ) )
                                QtyMinValue.Text = SelectedNodeTypeProp.MinValue.ToString();
                            QtyMinValueRow.Cells[1].Controls.Add( QtyMinValue );

                            TableRow QtyMaxValueRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) QtyMaxValueRow.Cells[0].Controls[0] ).Text = "Maximum Value:";
                            TextBox QtyMaxValue = new TextBox();
                            QtyMaxValue.CssClass = "textinput";
                            QtyMaxValue.ID = "EditProp_MaxValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( !Double.IsNaN( SelectedNodeTypeProp.MaxValue ) )
                                QtyMaxValue.Text = SelectedNodeTypeProp.MaxValue.ToString();
                            QtyMaxValueRow.Cells[1].Controls.Add( QtyMaxValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Question:

                            //Textbox: Possible Answers List
                            TableRow QstnPossibleAnswersRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) QstnPossibleAnswersRow.Cells[0].Controls[0] ).Text = "Possible Answers:";
                            TextBox QstnPossibleAnswersText = new TextBox();
                            QstnPossibleAnswersText.CssClass = "textinput";
                            QstnPossibleAnswersText.ID = "EditProp_OptionsValue" + SelectedNodeTypeProp.PropId.ToString();
                            QstnPossibleAnswersText.Text = SelectedNodeTypeProp.ListOptions;
                            QstnPossibleAnswersRow.Cells[1].Controls.Add( QstnPossibleAnswersText );



                            //Text: Compliant Answers
                            TableRow QstnCompliantAnswerRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) QstnCompliantAnswerRow.Cells[0].Controls[0] ).Text = "Compliant Answers:";
                            CswCheckBoxArray QstnCompliantAnswerList = new CswCheckBoxArray( Master.CswNbtResources );
                            DataTable CompliantAnswersTable = new DataTable();
                            DataColumn AnswersColumn = CompliantAnswersTable.Columns.Add( "Answers" );
                            DataColumn CompliantColumn = CompliantAnswersTable.Columns.Add( ChkBoxArrayValueColumnName, typeof( bool ) );
                            CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                            PossibleAnswers.FromString( SelectedNodeTypeProp.ListOptions );
                            CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                            CompliantAnswers.FromString( SelectedNodeTypeProp.ValueOptions );
                            for( Int32 i = 0; i < PossibleAnswers.Count; i++ )
                            {
                                DataRow AnswerRow = CompliantAnswersTable.Rows.Add();
                                AnswerRow[0] = PossibleAnswers[i];
                                AnswerRow[1] = ( CompliantAnswers.Contains( PossibleAnswers[i] ) );
                            }

                            QstnCompliantAnswerList.CreateCheckBoxes( CompliantAnswersTable, AnswersColumn.ColumnName, AnswersColumn.ColumnName );
                            //TextBox QstnCompliantAnswerList = new TextBox();
                            //QstnCompliantAnswerList.CssClass = "textinput";
                            QstnCompliantAnswerList.ID = "EditProp_ValueOptionsValue" + SelectedNodeTypeProp.PropId.ToString();
                            //QstnCompliantAnswerList.Text = SelectedNodeTypeProp.ValueOptions;
                            QstnCompliantAnswerRow.Cells[1].Controls.Add( QstnCompliantAnswerList );

                            //RequiredFieldValidator QstnCompliantAnswerRFV = new RequiredFieldValidator();
                            //QstnCompliantAnswerRFV.ControlToValidate = QstnCompliantAnswerList.ID;
                            //QstnCompliantAnswerRFV.ID = "EditProp_ValueOptionsValue_RFV" + SelectedNodeTypeProp.PropId.ToString();
                            //QstnCompliantAnswerRFV.Display = ValidatorDisplay.Dynamic;
                            //QstnCompliantAnswerRFV.EnableClientScript = true;
                            //QstnCompliantAnswerRFV.Text = "&nbsp;<img src=\"Images/vld/bad.gif\" alt=\"Value is required\" />";
                            //QstnCompliantAnswerRFV.ValidationGroup = "Design";
                            //QstnCompliantAnswerRow.Cells[1].Controls.Add( QstnCompliantAnswerRFV );
                            break;


                        case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                            TableRow TargetRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) TargetRow.Cells[0].Controls[0] ).Text = "Target:";

                            //DropDownList TargetValue = new DropDownList();
                            CswNodeTypeDropDown TargetValue = new CswNodeTypeDropDown( Master.CswNbtResources );
                            // Target is only editable if no nodes have values for this property
                            // See BZs 7176, 7600, 8025
                            if( SelectedNodeTypeProp.InUse )
                            {
                                TargetValue.Enabled = false;
                            }
                            TargetValue.CssClass = "selectinput";
                            TargetValue.ID = "EditProp_TargetValue" + SelectedNodeTypeProp.PropId.ToString();

                            if( DerivesFromObjectClassProp &&
                                ObjectClassProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                                ObjectClassProp.FKValue != Int32.MinValue )
                            {
                                // BZ 8051 - Constrain options to this object class
                                //CswNbtMetaDataObjectClass ConstrainToObjectClass = Master.CswNbtResources.MetaData.getObjectClass( ObjectClassProp.FKValue );
                                //foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.NodeTypesByObjectClass( ConstrainToObjectClass.ObjectClass ) )
                                //{
                                //    if( NodeType.IsLatestVersion )
                                //    {
                                //        ListItem Item = new ListItem( NodeType.NodeTypeName, "nt_" + NodeType.FirstVersionNodeTypeId.ToString() );
                                //        if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() && SelectedNodeTypeProp.FKValue == NodeType.FirstVersionNodeTypeId )
                                //            Item.Selected = true;
                                //        TargetValue.Items.Add( Item );
                                //    }
                                //}

                                //// Object Class
                                //ListItem OCItem = new ListItem( "{" + ConstrainToObjectClass.ObjectClass.ToString() + "}", "oc_" + ConstrainToObjectClass.ObjectClassId.ToString() );
                                //if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && SelectedNodeTypeProp.FKValue == ConstrainToObjectClass.ObjectClassId )
                                //    OCItem.Selected = true;
                                //TargetValue.Items.Add( OCItem );
                                TargetValue.ConstrainToObjectClassId = ObjectClassProp.FKValue;
                            }
                            //else
                            //{
                            //    // Blank option
                            //    if( SelectedNodeTypeProp.FKValue == Int32.MinValue )
                            //    {
                            //        ListItem BlankItem = new ListItem( "[Select...]", string.Empty );
                            //        BlankItem.Selected = true;
                            //        TargetValue.Items.Add( BlankItem );
                            //    }

                            //    // Nodetypes
                            //    foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.LatestVersionNodeTypes )
                            //    {
                            //        ListItem Item = new ListItem( NodeType.NodeTypeName, "nt_" + NodeType.FirstVersionNodeTypeId.ToString() );
                            //        if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() && SelectedNodeTypeProp.FKValue == NodeType.FirstVersionNodeTypeId )
                            //            Item.Selected = true;
                            //        TargetValue.Items.Add( Item );
                            //    }

                            //    // Object Classes
                            //    foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
                            //    {
                            //        ListItem Item = new ListItem( "{" + ObjectClass.ObjectClass + "}", "oc_" + ObjectClass.ObjectClassId.ToString() );
                            //        if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && SelectedNodeTypeProp.FKValue == ObjectClass.ObjectClassId )
                            //            Item.Selected = true;
                            //        TargetValue.Items.Add( Item );
                            //    }
                            //}

                            TargetValue.DataBind();

                            if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                                TargetValue.SelectedNodeTypeFirstVersionId = SelectedNodeTypeProp.FKValue;
                            else if( SelectedNodeTypeProp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                                TargetValue.SelectedObjectClassId = SelectedNodeTypeProp.FKValue;

                            TargetRow.Cells[1].Controls.Add( TargetValue );

                            _ViewXmlRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) _ViewXmlRow.Cells[0].Controls[0] ).Text = "View:";
                            //CswNbtView RelationshipView = new CswNbtView( Master.CswNbtResources );
                            //if( SelectedNodeTypeProp.ViewId != Int32.MinValue )
                            //{
							CswNbtView RelationshipView = Master.CswNbtResources.ViewSelect.restoreView( SelectedNodeTypeProp.ViewId );
                            //}
                            //else
                            //{
                            //    // This property is missing a view -- make a new one
                            //    RelationshipView.makeNew( SelectedNodeTypeProp.PropName, NbtViewVisibility.Property, Int32.MinValue, Int32.MinValue, Int32.MinValue );
                            //    setPropertyViewId( SelectedNodeTypeProp.PropId, RelationshipView.ViewId );
                            //}

                            _RelationshipViewTree = new CswViewStructureTree( Master.CswNbtResources );
                            _RelationshipViewTree.ID = "RelationshipViewTree";
                            _RelationshipViewTree.OnError += new CswErrorHandler( Master.HandleError );
                            _RelationshipViewTree.reinitTreeFromView( RelationshipView, null, null, CswViewStructureTree.ViewTreeSelectType.None );
                            _ViewXmlRow.Cells[1].Controls.Add( _RelationshipViewTree );

                            Button EditRelationshipViewButton = new Button();
                            EditRelationshipViewButton.ID = "EditRelationshipViewButton";
                            EditRelationshipViewButton.CssClass = "Button";
                            EditRelationshipViewButton.OnClientClick = "window.location='EditView.aspx?step=2&viewid=" + SelectedNodeTypeProp.ViewId + "&return=" + _ReturnURLForQueryString() + "';";
                            EditRelationshipViewButton.Text = "Edit View";
                            _ViewXmlRow.Cells[1].Controls.Add( EditRelationshipViewButton );

                            // BZ 10078
                            //TableRow RelRowsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            //( (Literal) RelRowsRow.Cells[0].Controls[0] ).Text = "Rows:";
                            //TextBox RelRowsValue = new TextBox();
                            //RelRowsValue.CssClass = "textinput";
                            //RelRowsValue.ID = "EditProp_RowsValue" + SelectedNodeTypeProp.PropId.ToString();
                            //if( SelectedNodeTypeProp.TextAreaRows != Int32.MinValue )
                            //    RelRowsValue.Text = SelectedNodeTypeProp.TextAreaRows.ToString();
                            //RelRowsRow.Cells[1].Controls.Add( RelRowsValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                            TableRow SequenceRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) SequenceRow.Cells[0].Controls[0] ).Text = "Sequence:";
                            CswSequencesEditor SequencesEditor = new CswSequencesEditor( Master.CswNbtResources, Master.AjaxManager, SelectedNodeTypeProp.PropId );
                            SequencesEditor.ID = "EditProp_SequenceValue" + SelectedNodeTypeProp.PropId.ToString();
                            SequencesEditor.DataBind();
                            SequenceRow.Cells[1].Controls.Add( SequencesEditor );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Static:
                            TableRow TextRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) TextRow.Cells[0].Controls[0] ).Text = "Text:";
                            TextBox TextValue = new TextBox();
                            TextValue.CssClass = "textinput";
                            TextValue.ID = "EditProp_TextValue" + SelectedNodeTypeProp.PropId.ToString();
                            TextValue.Text = SelectedNodeTypeProp.StaticText;
                            TextRow.Cells[1].Controls.Add( TextValue );

                            TableRow StaticRowsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) StaticRowsRow.Cells[0].Controls[0] ).Text = "Rows:";
                            TextBox StaticRowsValue = new TextBox();
                            StaticRowsValue.CssClass = "textinput";
                            StaticRowsValue.ID = "EditProp_RowsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaRows != Int32.MinValue )
                                StaticRowsValue.Text = SelectedNodeTypeProp.TextAreaRows.ToString();
                            StaticRowsRow.Cells[1].Controls.Add( StaticRowsValue );

                            TableRow StaticColsRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) StaticColsRow.Cells[0].Controls[0] ).Text = "Columns:";
                            TextBox StaticColsValue = new TextBox();
                            StaticColsValue.CssClass = "textinput";
                            StaticColsValue.ID = "EditProp_ColsValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.TextAreaColumns != Int32.MinValue )
                                StaticColsValue.Text = SelectedNodeTypeProp.TextAreaColumns.ToString();
                            StaticColsRow.Cells[1].Controls.Add( StaticColsValue );

                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Text:
                            TableRow LengthRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) LengthRow.Cells[0].Controls[0] ).Text = "Length:";
                            TextBox LengthValue = new TextBox();
                            LengthValue.CssClass = "textinput";
                            LengthValue.ID = "EditProp_LengthValue" + SelectedNodeTypeProp.PropId.ToString();
                            if( SelectedNodeTypeProp.Length != Int32.MinValue )
                                LengthValue.Text = SelectedNodeTypeProp.Length.ToString();
                            LengthRow.Cells[1].Controls.Add( LengthValue );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                            TableRow ViewSelectModeRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) ViewSelectModeRow.Cells[0].Controls[0] ).Text = "Select Mode:";
                            DropDownList ViewSelectModeValue = new DropDownList();
                            foreach( string ViewSelectModeName in Enum.GetNames( typeof( PropertySelectMode ) ) )
                            {
                                if( ViewSelectModeName != PropertySelectMode.Blank.ToString() )
                                    ViewSelectModeValue.Items.Add( new ListItem( ViewSelectModeName, ViewSelectModeName ) );
                            }
                            ViewSelectModeValue.CssClass = "selectinput";
                            ViewSelectModeValue.ID = "EditProp_MultiValue" + SelectedNodeTypeProp.PropId.ToString();
                            ViewSelectModeValue.SelectedValue = SelectedNodeTypeProp.Multi.ToString();
                            if( DerivesFromObjectClassProp && ObjectClassProp.Multi.ToString() != String.Empty )
                                ViewSelectModeValue.Enabled = false;
                            ViewSelectModeRow.Cells[1].Controls.Add( ViewSelectModeValue );
                            break;

                    } // switch (ThisFieldType)

                    // BZ 7121 - Conditional Filtering
                    if( !SelectedNodeTypeProp.HasConditionalProperties() )
                    {
                        TableRow ConditionalRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) ConditionalRow.Cells[0].Controls[0] ).Text = "Display Condition:";

                        if( SelectedNodeTypeProp.hasFilter() )
                        {
                            CswNbtSubField SubField = SelectedNodeTypeProp.FieldTypeRule.SubFields.Default;
                            CswNbtPropFilterSql.PropertyFilterMode FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
                            string FilterValue = null;
                            SelectedNodeTypeProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );
                            _ConditionalFilter = new CswPropertyFilter( Master.CswNbtResources, Master.AjaxManager, SelectedNodeType.FirstVersionNodeTypeId, SelectedNodeTypeProp.PropName, SubField.Column.ToString(), FilterMode, FilterValue, false, true, false, false );
                            _ConditionalFilter.ShowSubFieldAndMode = true;
                        }
                        else
                        {
                            _ConditionalFilter = new CswPropertyFilter( Master.CswNbtResources, Master.AjaxManager, SelectedNodeType.FirstVersionNodeTypeId, string.Empty, false, true, false, false );
                            _ConditionalFilter.ShowSubFieldAndMode = true;
                        }
                        _ConditionalFilter.OnError += new CswErrorHandler( Master.HandleError );
                        _ConditionalFilter.ShowNodeType = false;
                        _ConditionalFilter.ID = "EditProp_ConditionalFilter" + SelectedNodeTypeProp.PropId.ToString();
                        _ConditionalFilter.FilterPropertiesToTabId = _SelectedNodeTypeTab.TabId;
                        _ConditionalFilter.FilterOutPropertyId = _SelectedNodeTypeProp.PropId;
                        _ConditionalFilter.FilterOutConditionalProperties = true;
                        _ConditionalFilter.ShowBlankOptions = true;
                        _ConditionalFilter.AllowedFieldTypes.Add( CswNbtMetaDataFieldType.NbtFieldType.List );
                        _ConditionalFilter.AllowedFieldTypes.Add( CswNbtMetaDataFieldType.NbtFieldType.Logical );
                        _ConditionalFilter.AllowedFieldTypes.Add( CswNbtMetaDataFieldType.NbtFieldType.Text );
                        _ConditionalFilter.AllowedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
                        _ConditionalFilter.AllowedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
                        _ConditionalFilter.AllowedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
                        _ConditionalFilter.AllowedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
                        //ConditionalRow.Cells[1].Controls.Add( _ConditionalFilter );
                        CswAutoTable ConditionalTable = new CswAutoTable();
                        ConditionalRow.Cells[1].Controls.Add( ConditionalTable );
                        ConditionalTable.Rows.Add( _ConditionalFilter );
                    }

                    // Batch Entry
                    //switch( ThisFieldType )
                    //{
                    //    case FieldType.Text:
                    //    case FieldType.Date:
                    //    case FieldType.List:
                    //    case FieldType.Quantity:
                    //        TableRow IsBatchRow = makeEditPropTableRow( EditPropHolder );
                    //        ( ( Label ) IsBatchRow.Cells[ 0 ].Controls[ 0 ] ).Text = "";
                    //        CheckBox IsBatchValue = new CheckBox();
                    //        IsBatchValue.ID = "EditProp_IsBatchValue" + SelectedNodeTypeProp.PropId.ToString();
                    //        IsBatchValue.Text = "Include on Batch Entry Form";
                    //        if( PropRow[ "isbatchentry" ].ToString() == "1" )
                    //            IsBatchValue.Checked = true;
                    //        else
                    //            IsBatchValue.Checked = false;
                    //        IsBatchRow.Cells[ 1 ].Controls.Add( IsBatchValue );
                    //        break;
                    //}

                    // BZ 8058 - Default Value
                    if( FieldType.CanHaveDefaultValue() )
                    {
                        TableRow DefaultValueRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) DefaultValueRow.Cells[0].Controls[0] ).Text = "Default Value:";

                        // This spawns a row whenever we click on a property, but it's unavoidable at the moment, and it's only one row per property.
                        _DefaultValueControl = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources,
                                            DefaultValueRow.Cells[1].Controls, "EditProp_DefaultValue" + SelectedNodeTypeProp.PropId.ToString(),
                                            SelectedNodeTypeProp.DefaultValue, NodeEditMode.DefaultValue, Master.HandleError );
                        // BZ 8307 - Even if the property is readonly, the default value is not.
                        _DefaultValueControl.DataBind();
                    }

                    //Required
                    switch( FieldType.FieldType )
                    {
                        //case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                        case CswNbtMetaDataFieldType.NbtFieldType.Date:
                        case CswNbtMetaDataFieldType.NbtFieldType.Time:
                        //case CswNbtMetaDataFieldType.NbtFieldType.File:
                        //case CswNbtMetaDataFieldType.NbtFieldType.Image:
                        case CswNbtMetaDataFieldType.NbtFieldType.Link:
                        case CswNbtMetaDataFieldType.NbtFieldType.List:
                        case CswNbtMetaDataFieldType.NbtFieldType.Location:
                        case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                        case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                        case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                        case CswNbtMetaDataFieldType.NbtFieldType.Password:
                        case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                        case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                        //case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                        case CswNbtMetaDataFieldType.NbtFieldType.Text:
                        //case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                        case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                            TableRow RequiredRow = makeEditPropTableRow( EditPropPlaceHolder );
                            ( (Literal) RequiredRow.Cells[0].Controls[0] ).Text = "";
                            _RequiredValue = new CheckBox();
                            if( DerivesFromObjectClassProp && ObjectClassProp.IsRequired )
                                _RequiredValue.Enabled = false;
                            _RequiredValue.ID = "EditProp_RequiredValue" + SelectedNodeTypeProp.PropId.ToString();
                            _RequiredValue.Text = "Required";
                            _RequiredValue.Checked = SelectedNodeTypeProp.IsRequired;
                            RequiredRow.Cells[1].Controls.Add( _RequiredValue );
                            break;
                    }

                    //bz # 6686, 7994
                    //IsUnique
                    if( _Mode == NbtDesignMode.Standard )
                    {
                        switch( FieldType.FieldType )
                        {
                            //                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                            case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                            case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                            case CswNbtMetaDataFieldType.NbtFieldType.Text:
                                //case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                                TableRow IsUniqueRow = makeEditPropTableRow( EditPropPlaceHolder );
                                ( (Literal) IsUniqueRow.Cells[0].Controls[0] ).Text = "";
                                _IsUnique = new CheckBox();
                                if( DerivesFromObjectClassProp && ObjectClassProp.IsUnique )
                                    _IsUnique.Enabled = false;
                                _IsUnique.ID = "EditProp_IsUnique" + SelectedNodeTypeProp.PropId.ToString();
                                _IsUnique.Text = "Unique";
                                _IsUnique.Checked = SelectedNodeTypeProp.IsUnique;
                                IsUniqueRow.Cells[1].Controls.Add( _IsUnique );

                                TableRow WarningRow = makeEditPropTableRow( EditPropPlaceHolder );
                                ( (Literal) WarningRow.Cells[0].Controls[0] ).Text = "";
                                _WarningLabel = new Label();
                                _WarningLabel.CssClass = "warning";
                                WarningRow.Cells[1].Controls.Add( _WarningLabel );
                                break;
                        }
                    }
                    // BZ 7594, 7967, 7975, 7982, 7984
                    if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.File &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Image &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.PropertyReference &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Static &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Composite &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.MOL )
                    {
                        //bz # 5641
                        TableRow ReadOnlyRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) ReadOnlyRow.Cells[0].Controls[0] ).Text = "";
                        CheckBox ReadOnlyValue = new CheckBox();
                        if( DerivesFromObjectClassProp && ObjectClassProp.ReadOnly )
                            ReadOnlyValue.Enabled = false;
                        ReadOnlyValue.ID = "EditProp_ReadOnlyValue" + SelectedNodeTypeProp.PropId.ToString();
                        ReadOnlyValue.Text = "ReadOnly";
                        ReadOnlyValue.Checked = SelectedNodeTypeProp.ReadOnly;
                        ReadOnlyRow.Cells[1].Controls.Add( ReadOnlyValue );
                    }

                    if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.File &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Image &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.LocationContents &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.PropertyReference &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Composite &&
                        FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.MOL )
                    {
                        //bz # 6157
                        TableRow SetValueOnAddRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) SetValueOnAddRow.Cells[0].Controls[0] ).Text = "";
                        _SetValueOnAddValue = new CheckBox();
                        //_SetValueOnAddValue.Enabled = false;
                        _SetValueOnAddValue.ID = "EditProp_SetValueOnAddValue" + SelectedNodeTypeProp.PropId.ToString();
                        _SetValueOnAddValue.Text = "Set Value On Add";
                        _SetValueOnAddValue.Checked = SelectedNodeTypeProp.SetValueOnAdd;
                        // BZ 7742
                        //if( _Mode == NbtDesignMode.Inspection )
                        //    _SetValueOnAddValue.Style.Add( HtmlTextWriterStyle.Display, "none" );
                        SetValueOnAddRow.Cells[1].Controls.Add( _SetValueOnAddValue );
                    }

                    if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Static )
                    {
                        TableRow UseNumberingRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) UseNumberingRow.Cells[0].Controls[0] ).Text = "";
                        CheckBox UseNumberingValue = new CheckBox();
                        UseNumberingValue.ID = "EditProp_UseNumbering" + SelectedNodeTypeProp.PropId.ToString();
                        UseNumberingValue.Text = "Enable Question Numbering";
                        UseNumberingValue.Checked = SelectedNodeTypeProp.UseNumbering;
                        UseNumberingRow.Cells[1].Controls.Add( UseNumberingValue );
                    }

                    TableRow HideInMobileRow = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) HideInMobileRow.Cells[0].Controls[0] ).Text = "";
                    CheckBox HideInMobileValue = new CheckBox();
                    HideInMobileValue.ID = "EditProp_HideInMobile" + SelectedNodeTypeProp.PropId.ToString();
                    HideInMobileValue.Text = "Hide in Mobile";
                    HideInMobileValue.Checked = SelectedNodeTypeProp.HideInMobile;
                    HideInMobileRow.Cells[1].Controls.Add( HideInMobileValue );

                    TableRow MobileSearchRow = makeEditPropTableRow( EditPropPlaceHolder );
                    ( (Literal) MobileSearchRow.Cells[0].Controls[0] ).Text = "";
                    CheckBox MobileSearchValue = new CheckBox();
                    MobileSearchValue.ID = "EditProp_MobileSearch" + SelectedNodeTypeProp.PropId.ToString();
                    MobileSearchValue.Text = "Use For Mobile Search";
                    MobileSearchValue.Checked = SelectedNodeTypeProp.MobileSearch;
                    MobileSearchRow.Cells[1].Controls.Add( MobileSearchValue );

                    // BZ 7957
                    if( FieldType.ShowLabel() )
                    {
                        TableRow HelpTextRow = makeEditPropTableRow( EditPropPlaceHolder );
                        ( (Literal) HelpTextRow.Cells[0].Controls[0] ).Text = "Help Text";
                        TextBox HelpTextBox = new TextBox();
                        HelpTextBox.ID = "EditProp_HelpText" + SelectedNodeTypeProp.PropId.ToString();
                        HelpTextBox.CssClass = "textinput";
                        HelpTextBox.Text = SelectedNodeTypeProp.HelpText;
                        HelpTextBox.TextMode = TextBoxMode.MultiLine;
                        HelpTextBox.Rows = 4;
                        HelpTextBox.Columns = 60;
                        HelpTextRow.Cells[1].Controls.Add( HelpTextBox );
                    }

                    // BZ 8139
                    if( SelectedNodeTypeProp.FieldTypeRule.SearchAllowed )
                    {
                        TableRow IsQuickSearchRow = makeEditPropTableRow( EditPropPlaceHolder );
                        CheckBox IsQuickSearchCheckBox = new CheckBox();
                        IsQuickSearchCheckBox.ID = "EditProp_IsQuickSearch" + SelectedNodeTypeProp.PropId.ToString();
                        IsQuickSearchCheckBox.Text = "Include in Quick Search";
                        IsQuickSearchCheckBox.Checked = SelectedNodeTypeProp.IsQuickSearch;
                        IsQuickSearchRow.Cells[1].Controls.Add( IsQuickSearchCheckBox );
                    }

                } // if (NodeTypePropId > 0)
            } // if (_SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property)

            //Master.AjaxManager.AjaxSettings.AddAjaxSetting( SavePropButton, DesignMenu );
            //Master.AjaxManager.AjaxSettings.AddAjaxSetting( SavePropButton, NodeTypeTree );
            //Master.AjaxManager.AjaxSettings.AddAjaxSetting( SavePropButton, DesignTabStrip );
            //Master.AjaxManager.AjaxSettings.AddAjaxSetting( SavePropButton, DesignTabTable );
            //Master.AjaxManager.AjaxSettings.AddAjaxSetting( SavePropButton, Master.ErrorBox );

            if( _ConditionalFilter != null )
            {
                if( _SetValueOnAddValue != null && _SetValueOnAddValue.Visible )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _ConditionalFilter, _SetValueOnAddValue );
                if( _RequiredValue != null && _RequiredValue.Visible )
                    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _ConditionalFilter, _RequiredValue );
            }
        } // init_EditPropertyPage()

        #endregion Tab Contents



        #region Helpers


        private String getPropAttributeValue( String ControlId, Control EditPropHolder )
        {
            return getPropAttributeValue( ControlId, typeof( String ), EditPropHolder );
        }

        private String getPropAttributeValue( String ControlId, Type ReturnType, Control EditPropHolder )
        {
            String ret = "";
            Control Control = EditPropHolder.FindControl( ControlId );
            if( Control != null )
            {
                if( Control is TextBox )
                {
                    ret = ( (TextBox) Control ).Text;
                }
                else if( Control is HiddenField )
                {
                    ret = ( (HiddenField) Control ).Value;
                }
                else if( Control is CheckBox )
                {
                    ret = ( (CheckBox) Control ).Checked.ToString().ToLower();
                }
                else if( Control is DropDownList )
                {
                    ret = ( (DropDownList) Control ).SelectedValue;
                }
                else if( Control is CswTriStateCheckBox )
                {
                    ret = ( (CswTriStateCheckBox) Control ).Checked.ToString().ToLower();
                }
                else if( Control is CswCheckBoxArray )
                {
                    ret = ( (CswCheckBoxArray) Control ).GetCheckedValues( ChkBoxArrayValueColumnName ).ToString();
                }
            }
            if( ret == "" )
            {
                if( ReturnType == typeof( Int32 ) )
                {
                    ret = Int32.MinValue.ToString();
                }
                else if( ReturnType == typeof( Double ) )
                {
                    ret = Double.NaN.ToString();
                }
                else if( ReturnType == typeof( bool ) )
                {
                    ret = "false";
                }
            }
            return ret;
        } // getPropAttributeValue()

        private TableRow makeEditPropTableRow( Control myHolder )
        {
            TableRow myRow = new TableRow();
            myHolder.Controls.Add( myRow );
            TableCell LabelCell = new TableCell();
            myRow.Cells.Add( LabelCell );
            TableCell ValueCell = new TableCell();
            myRow.Cells.Add( ValueCell );

            LabelCell.VerticalAlign = VerticalAlign.Top;
            LabelCell.HorizontalAlign = HorizontalAlign.Right;

            Literal myLabel = new Literal();
            LabelCell.Controls.Add( myLabel );

            return myRow;
        }

        #endregion Helpers

    }
}
