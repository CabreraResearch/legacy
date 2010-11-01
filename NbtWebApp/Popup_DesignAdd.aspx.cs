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
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using ChemSW.CswWebControls;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_DesignAdd : System.Web.UI.Page
    {
        private NbtDesignMode _Mode = NbtDesignMode.Standard;
        public string LabelNodeType = "NodeType";
        public string LabelNodeTypeTab = "Tab";
        public string LabelNodeTypeProp = "Property";

        #region Selected

        private CswNodeTypeTree.NodeTypeTreeSelectedType _AddType
        {
            get
            {
                if (Request.QueryString["add"] != null)
                    return (CswNodeTypeTree.NodeTypeTreeSelectedType)Enum.Parse(typeof(CswNodeTypeTree.NodeTypeTreeSelectedType), Request.QueryString["add"].ToString());
                else
                    return CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType;
            }
        }
        private CswNodeTypeTree.NodeTypeTreeSelectedType _SelectedType
        {
            get
            {
                if (Request.QueryString["type"] != null)
                    return (CswNodeTypeTree.NodeTypeTreeSelectedType)Enum.Parse(typeof(CswNodeTypeTree.NodeTypeTreeSelectedType), Request.QueryString["type"].ToString());
                else
                    return CswNodeTypeTree.NodeTypeTreeSelectedType.Root;
            }
        }
        private string _SelectedValue
        {
            get
            {
                if (Request.QueryString["value"] != null)
                    return Request.QueryString["value"].ToString();
                else
                    return string.Empty;
            }
        }


        private CswNbtMetaDataNodeType _SelectedNodeType = null;
        private CswNbtMetaDataNodeTypeTab _SelectedNodeTypeTab = null;
        private CswNbtMetaDataNodeTypeProp _SelectedNodeTypeProp = null;
        private void InitSelectedMetaDataObjects()
        {
            EnsureChildControls();
            switch (_SelectedType)
            {
                case CswNodeTypeTree.NodeTypeTreeSelectedType.Category:
                    _SelectedNodeType = null;
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                    if (Convert.ToInt32(_SelectedValue) > 0)
                    {
                        _SelectedNodeType = Master.CswNbtResources.MetaData.getNodeType(Convert.ToInt32(_SelectedValue));
                    }
                    else
                    {
                        _SelectedNodeType = null;
                    }
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                    if (Convert.ToInt32(_SelectedValue) > 0)
                    {
                        _SelectedNodeTypeProp = Master.CswNbtResources.MetaData.getNodeTypeProp(Convert.ToInt32(_SelectedValue));
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
                    if (Convert.ToInt32(_SelectedValue) > 0)
                    {
                        _SelectedNodeTypeTab = Master.CswNbtResources.MetaData.getNodeTypeTab(Convert.ToInt32(_SelectedValue));
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
                if (_SelectedNodeType == null)
                    InitSelectedMetaDataObjects();
                return _SelectedNodeType;
            }
        }
        private CswNbtMetaDataNodeTypeTab SelectedNodeTypeTab
        {
            get
            {
                if (_SelectedNodeTypeTab == null)
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeTab;
            }
        }
        private CswNbtMetaDataNodeTypeProp SelectedNodeTypeProp
        {
            get
            {
                if (_SelectedNodeTypeProp == null)
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeProp;
            }
        }

        #endregion Selected

        #region Page Lifecycle

        protected override void OnInit(EventArgs e)
        {
            try
            {
                if( Request.QueryString["mode"] != null )
                {
                    switch( Request.QueryString["mode"].ToString() )
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
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit(e);
        }

        private CswAutoTable AddTable;
        private Button CancelButton;

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                AddTable = new CswAutoTable();
                AddTable.ID = "AddTable";
                AddTable.FirstCellRightAlign = true;
                ph.Controls.Add( AddTable );

                Int32 LastRow = 0;
                switch( _AddType )
                {
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                        if( !Master.CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Create, SelectedNodeTypeTab.NodeType.NodeTypeId, null, null ) )
                            throw new CswDniException( "You do not have permission to add properties to this NodeType" );

                        create_AddPropertyPage();
                        AddTable.addControl( 0, 0, Label1 );
                        AddTable.addControl( 0, 1, AddPropNewFieldTypeIdSelect );
                        AddTable.addControl( 1, 0, AddPropTabLabel );
                        AddTable.addControl( 1, 1, AddPropTabSelect );
                        AddTable.addControl( 2, 0, AddPropNameLabel );
                        AddTable.addControl( 2, 1, AddPropName );
                        AddTable.addControl( 3, 1, AddPropButton );
                        LastRow = 3;
                        init_AddPropertyPage();
                        TitleContentLiteral.Text = "Add " + LabelNodeTypeProp;
                        //LeftHeaderContentLiteral.Text = "Add " + LabelNodeTypeProp + " to " + LabelNodeTypeTab + ": " + SelectedNodeTypeTab.TabName;
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.Tab:
                        if( !Master.CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Create, SelectedNodeType.NodeTypeId, null, null ) )
                            throw new CswDniException( "You do not have permission to add tabs to this NodeType" );

                        create_AddTabPage();
                        AddTable.addControl( 0, 0, AddTabNameLabel );
                        AddTable.addControl( 0, 1, AddTabNameTextBox );
                        AddTable.addControl( 1, 0, AddTabOrderLabel );
                        AddTable.addControl( 1, 1, AddTabOrderTextBox );
                        AddTable.addControl( 2, 1, AddNewTabButton );
                        LastRow = 2;
                        init_AddTabPage();
                        TitleContentLiteral.Text = "Add " + LabelNodeTypeTab;
                        //LeftHeaderContentLiteral.Text = "Add " + LabelNodeTypeTab + " to " + LabelNodeType + ": " + SelectedNodeType.NodeTypeName;
                        break;
                    case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                        if( !( (CswNbtObjClassUser) Master.CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Design ) )
                                throw new CswDniException( "You do not have permission to add NodeTypes" );

                        create_AddNodeTypePage();
                        AddTable.addControl( 0, 0, ObjectClassLabel );
                        AddTable.addControl( 0, 1, ObjectClassSelect );
                        AddTable.addControl( 1, 0, NewNodeTypeNameLabel );
                        AddTable.addControl( 1, 1, NewNodeTypeName );
                        AddTable.addControl( 2, 0, NewNodeTypeCategoryLabel );
                        AddTable.addControl( 2, 1, NewNodeTypeCategory );
                        AddTable.addControl( 3, 1, NewNodeTypeButton );
                        LastRow = 3;
                        init_AddNodeTypePage();
                        TitleContentLiteral.Text = "Add " + LabelNodeType;
                        //LeftHeaderContentLiteral.Text = "Add " + LabelNodeType;
                        break;
                }

                CancelButton = new Button();
                CancelButton.ID = "CancelButton";
                CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";
                CancelButton.Text = "Cancel";
                CancelButton.CssClass = "Button";
                AddTable.addControl( LastRow, 1, CancelButton );
            }
            catch( Exception ex )
            {
                Master.HandleError(ex);
            }
        }

        #endregion Page Lifecycle

        #region Events


        protected void NewNodeTypeButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Don't have to worry about versioning here
                CswNbtMetaDataNodeType NewNodeType = Master.CswNbtResources.MetaData.makeNewNodeType(Convert.ToInt32(ObjectClassSelect.SelectedValue.ToString()), NewNodeTypeName.Text, NewNodeTypeCategory.Text);
                Session["Design_SelectedType"] = CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType.ToString();
                Session["Design_SelectedValue"] = NewNodeType.NodeTypeId.ToString();
                Session["Design_ForceReselect"] = "true";
                string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_JS", JS, false);

                //_DoFocus = false;
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }

        }

        protected void AddNewTabButton_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 NewTabOrder = Int32.MinValue;
                if (CswTools.IsInteger(AddTabOrderTextBox.Text))
                    NewTabOrder = Convert.ToInt32( AddTabOrderTextBox.Text );

                CswNbtMetaDataNodeTypeTab NewTab = Master.CswNbtResources.MetaData.makeNewTab( SelectedNodeType, AddTabNameTextBox.Text, NewTabOrder );
                // BZ 7543 - We might have just versioned the nodetype, but we don't care, since the tabid is correct and we're closing the popup
                Session["Design_SelectedType"] = CswNodeTypeTree.NodeTypeTreeSelectedType.Tab.ToString();
                Session["Design_SelectedValue"] = NewTab.TabId.ToString();
                Session["Design_ForceReselect"] = "true";
                string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_JS", JS, false);
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
        }

        protected void AddPropButton_Click(object sender, EventArgs e)
        {
            try
            {
                CswNbtMetaDataNodeTypeProp NewProp = Master.CswNbtResources.MetaData.makeNewProp( SelectedNodeType, 
                                                                                                  Convert.ToInt32( AddPropNewFieldTypeIdSelect.SelectedValue ),
                                                                                                  AddPropName.Text,
                                                                                                  Convert.ToInt32( AddPropTabSelect.SelectedValue ) );

                // BZ 7543 - We might have just versioned the nodetype, but we don't care, since the propid is correct and we're closing the popup

                Session["Design_SelectedType"] = CswNodeTypeTree.NodeTypeTreeSelectedType.Property.ToString();
                Session["Design_SelectedValue"] = NewProp.PropId.ToString();
                Session["Design_ForceReselect"] = "true";
                string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_JS", JS, false);
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
        }

        #endregion Events

        private Label ObjectClassLabel;
        private DropDownList ObjectClassSelect;
        private Label NewNodeTypeNameLabel;
        private TextBox NewNodeTypeName;
        private Button NewNodeTypeButton;
        private Label NewNodeTypeCategoryLabel;
        private TextBox NewNodeTypeCategory;
        private void create_AddNodeTypePage()
        {
            ObjectClassLabel = new Label();
            ObjectClassLabel.ID = "ObjectClassLabel";
            ObjectClassLabel.Text = "Object Class:";

            ObjectClassSelect = new DropDownList();
            ObjectClassSelect.ID = "ObjectClassSelect";
            ObjectClassSelect.CssClass = "selectinput";

            NewNodeTypeNameLabel = new Label();
            NewNodeTypeNameLabel.ID = "NewNodeTypeNameLabel";
            NewNodeTypeNameLabel.Text = "Name:";

            NewNodeTypeName = new TextBox();
            NewNodeTypeName.ID = "NewNodeTypeName";
            NewNodeTypeName.CssClass = "textinput";

            NewNodeTypeCategoryLabel = new Label();
            NewNodeTypeCategoryLabel.ID = "NodeTypeCategoryLabel";
            NewNodeTypeCategoryLabel.Text = "Category:";

            NewNodeTypeCategory = new TextBox();
            NewNodeTypeCategory.ID = "NewNodeTypeCategory";
            NewNodeTypeCategory.CssClass = "textinput";

            NewNodeTypeButton = new Button();
            NewNodeTypeButton.ID = "NewNodeTypeButton";
            NewNodeTypeButton.Text = "Create "+ LabelNodeType;
            NewNodeTypeButton.Click += new EventHandler(NewNodeTypeButton_Click);
            NewNodeTypeButton.CssClass = "Button";
        }

        private void init_AddNodeTypePage()
        {
            ObjectClassSelect.Items.Clear();
            foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
            {
                ListItem ClassItem = new ListItem( ObjectClass.ObjectClass.ToString(),
                                                  ObjectClass.ObjectClassId.ToString() );
                ObjectClassSelect.Items.Add( ClassItem );
            }
            if( _Mode == NbtDesignMode.Inspection )
            {
                ObjectClassSelect.SelectedValue = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass ).ObjectClassId.ToString();
                ObjectClassLabel.Style.Add( HtmlTextWriterStyle.Display, "none" );
                ObjectClassSelect.Style.Add( HtmlTextWriterStyle.Display, "none" );
            }
            else
            {
                ObjectClassSelect.SelectedValue = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass ).ObjectClassId.ToString();
            }
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Category && _SelectedValue != string.Empty )
                NewNodeTypeCategory.Text = _SelectedValue;


        }

        private Label AddTabNameLabel;
        private TextBox AddTabNameTextBox;
        private Label AddTabOrderLabel;
        private TextBox AddTabOrderTextBox;
        private Button AddNewTabButton;
        private void create_AddTabPage()
        {

            AddTabNameLabel = new Label();
            AddTabNameLabel.ID = "AddTabNameLabel";
            AddTabNameLabel.Text = "Name:";

            AddTabNameTextBox = new TextBox();
            AddTabNameTextBox.ID = "AddTabNameTextBox";
            AddTabNameTextBox.CssClass = "textinput";

            AddTabOrderLabel = new Label();
            AddTabOrderLabel.ID = "AddTabOrderLabel";
            if( _Mode == NbtDesignMode.Inspection )
                AddTabOrderLabel.Text = "Number:";
            else
                AddTabOrderLabel.Text = "Order:";

            AddTabOrderTextBox = new TextBox();
            AddTabOrderTextBox.ID = "AddTabOrderTextBox";
            AddTabOrderTextBox.Text = ( SelectedNodeType.GetMaximumTabOrder() + 1 ).ToString();
            AddTabOrderTextBox.CssClass = "textinput";

            AddNewTabButton = new Button();
            AddNewTabButton.ID = "AddNewTabButton";
            AddNewTabButton.Text = "Add " + LabelNodeTypeTab;
            AddNewTabButton.CssClass = "Button";
            AddNewTabButton.Click += new EventHandler(AddNewTabButton_Click);

        }
        private void init_AddTabPage() { }

        private CswLiteralText Label1;
        private DropDownList AddPropNewFieldTypeIdSelect;
        private CswLiteralText AddPropTabLabel;
        private DropDownList AddPropTabSelect;
        private Label AddPropNameLabel;
        private TextBox AddPropName;
        private Button AddPropButton;
        private void create_AddPropertyPage()
        {

            Label1 = new CswLiteralText("Field Type: ");
            if( _Mode == NbtDesignMode.Inspection )
                Label1.Visible = false;

            AddPropNewFieldTypeIdSelect = new DropDownList();
            AddPropNewFieldTypeIdSelect.ID = "AddPropNewFieldTypeIdSelect";
            AddPropNewFieldTypeIdSelect.CssClass = "selectinput";
            AddPropNewFieldTypeIdSelect.AutoPostBack = false;
            if( _Mode == NbtDesignMode.Inspection )
                AddPropNewFieldTypeIdSelect.Visible = false;

            AddPropTabLabel = new CswLiteralText("Display On " + LabelNodeTypeTab + ": ");

            AddPropTabSelect = new DropDownList();
            AddPropTabSelect.ID = "AddPropTabSelect";
            AddPropTabSelect.CssClass = "selectinput";
            AddPropTabSelect.AutoPostBack = false;

            AddPropNameLabel = new Label();
            AddPropNameLabel.ID = "AddPropNameLabel";
            if(_Mode == NbtDesignMode.Inspection)
                AddPropNameLabel.Text = "Question:";
            else
                AddPropNameLabel.Text = "Name:";

            AddPropName = new TextBox();
            AddPropName.ID = "AddPropName";
            AddPropName.CssClass = "textinput";
            AddPropName.Width = Unit.Parse( "250px" );
            AddPropName.MaxLength = 512;

            AddPropButton = new Button();
            AddPropButton.ID = "AddNewTabButton";
            AddPropButton.Text = "Add " + LabelNodeTypeProp;
            AddPropButton.CssClass = "Button";
            AddPropButton.Click += new EventHandler(AddPropButton_Click);

        }


        private void init_AddPropertyPage()
        {
            // Field Type select box
            AddPropNewFieldTypeIdSelect.Items.Clear();
            if( _Mode == NbtDesignMode.Inspection )
            {
                CswNbtMetaDataFieldType QuestionFT = Master.CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Question );
                AddPropNewFieldTypeIdSelect.Items.Add( new ListItem( QuestionFT.FieldType.ToString(),
                                                                             QuestionFT.FieldTypeId.ToString() ) );
                AddPropNewFieldTypeIdSelect.SelectedValue = QuestionFT.FieldTypeId.ToString();
            }
            else
            {

                foreach( CswNbtMetaDataFieldType FieldType in Master.CswNbtResources.MetaData.FieldTypes )
                {
                    // Temporarily skip unimplemented ones
                    // If Inspection, filter to allowed question field types
                    if( FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.External &&
                            FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.MOL ) 
                    {
                        AddPropNewFieldTypeIdSelect.Items.Add( new ListItem( FieldType.FieldType.ToString(),
                                                                             FieldType.FieldTypeId.ToString() ) );
                        if( FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Text )
                            AddPropNewFieldTypeIdSelect.SelectedValue = FieldType.FieldTypeId.ToString();
                    }
                }
            }
            // Tab Select
            AddPropTabSelect.Items.Clear();
            foreach (CswNbtMetaDataNodeTypeTab Tab in SelectedNodeType.NodeTypeTabs)
                AddPropTabSelect.Items.Add(new ListItem(Tab.TabName, Tab.TabId.ToString()));

            if (SelectedNodeTypeTab != null)
                AddPropTabSelect.SelectedValue = SelectedNodeTypeTab.TabId.ToString();

            if (SelectedNodeTypeProp != null)
            {
                // Edit Property Select box
                if (SelectedNodeTypeProp != null && SelectedNodeTypeProp.NodeTypeTab != null)
                    AddPropTabSelect.SelectedIndex = AddPropTabSelect.Items.IndexOf(AddPropTabSelect.Items.FindByValue(SelectedNodeTypeProp.NodeTypeTab.TabId.ToString()));
            }
        }
    }
}
