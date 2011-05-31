using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls;
using Telerik.Web.UI;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebPages
{
    public partial class Design_Layout : System.Web.UI.Page
    {
        private enum LayoutMode
        {
            Edit,
            Add,
            Inspection
        }
        private LayoutMode _Mode = LayoutMode.Edit;

        private string SelectedTabId
        {
            get
            {
                string ret = string.Empty;
                if( PropertyTable != null && PropertyTable.TabStrip.SelectedTab != null )
                    ret = PropertyTable.TabStrip.SelectedTab.Value;
                else if( Session["DesignLayout_SelectedTabId"] != null && Session["DesignLayout_SelectedTabId"].ToString() != string.Empty )
                    ret = Session["DesignLayout_SelectedTabId"].ToString();
                return ret;
            }
            set
            {
                foreach( RadTab Tab in PropertyTable.TabStrip.Tabs )
                {
                    if( Tab.Value == value )
                    {
                        Tab.Selected = true;
                        break;
                    }
                }
            }
        }


        private CswAutoTable PropDataTable;
        private CswNbtMetaDataNodeType NodeType;
        private DropDownList ModeList;
        private CswPropertyTable PropertyTable;
        private CswNbtNode FakeNode;
        private CswNbtNodeKey FakeNodeKey;
        private Button SaveButton;
        private TextBox TabOrderTextBox;

        private Hashtable LayoutControlsHash;
        private class LayoutControlSet
        {
            public TextBox DisplayColTextBox;
            public TextBox DisplayRowTextBox;
            public CheckBox SetValOnAddCheckBox;

            public LayoutControlSet( TextBox inDisplayColTextBox,
                                     TextBox inDisplayRowTextBox,
                                     CheckBox inSetValOnAddCheckBox )
            {
                DisplayColTextBox = inDisplayColTextBox;
                DisplayRowTextBox = inDisplayRowTextBox;
                SetValOnAddCheckBox = inSetValOnAddCheckBox;
            }
        }

        protected override void OnInit( EventArgs e )
        {
            try
            {
                if( Request.QueryString["mode"] != null && Request.QueryString["mode"] != string.Empty )
                    _Mode = (LayoutMode) Enum.Parse( typeof( LayoutMode ), Request.QueryString["mode"] );

                if( Request.QueryString["nodetypeid"] != string.Empty )
                    NodeType = Master.CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( Request.QueryString["nodetypeid"] ) );

                if( NodeType == null )
                    Master.Redirect( "Design.aspx" );
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
                FakeNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                FakeNodeKey = new CswNbtNodeKey( Master.CswNbtResources, null, string.Empty, FakeNode.NodeId, NodeSpecies.Plain, NodeType.NodeTypeId, NodeType.ObjectClass.ObjectClassId, string.Empty, string.Empty );

                PropertyTable = new CswPropertyTable( Master.CswNbtResources, Master.AjaxManager );
                PropertyTable.EnableViewState = false;
                PropertyTable.OnError += new CswErrorHandler( Master.HandleError );
                PropertyTable.ID = "exampletable";
                //PropertyTable.SelectedNodeTypeId = NodeType.NodeTypeId;
                if( _Mode == LayoutMode.Add )
                    PropertyTable.EditMode = NodeEditMode.AddInPopup;
                else if( _Mode == LayoutMode.Edit || _Mode == LayoutMode.Inspection )
                    PropertyTable.EditMode = NodeEditMode.Demo;
                //PropertyTable.SelectedNodeKey = FakeNodeKey;
                //PropertyTable.SelectedNodeText = FakeNode.NodeName;
                PropertyTable.SelectedNode = FakeNode;
                //PropertyTable.SelectedNodeSpecies = FakeNode.NodeSpecies;
                PropertyTable.SelectedTabId = SelectedTabId;
                PropertyTable.TabStrip.TabClick += new Telerik.Web.UI.RadTabStripEventHandler( TabStrip_TabClick );
                PropertyTable.SaveButton.Visible = false;

                PropertyTable.initTabStrip();
                SelectedTabId = PropertyTable.initTabContents();

                rightph.Controls.Add( PropertyTable );

                initLeftContents();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnLoad( e );
        }

        private string DisplayRowIdPrefix = "display_row_";
        private string DisplayColIdPrefix = "display_col_";
        private string SetValOnAddIdPrefix = "setvalonadd_";
        private string TabOrderIdPrefix = "taborder_";

        private void initLeftContents()
        {
            leftph.Controls.Clear();
            LayoutControlsHash = new Hashtable();

            if( _Mode != LayoutMode.Inspection )
            {
                ModeList = new DropDownList();
                ModeList.ID = "modelist";
                ModeList.CssClass = "selectinput";
                ModeList.Items.Add( new ListItem( LayoutMode.Edit.ToString() ) );
                ModeList.Items.Add( new ListItem( LayoutMode.Add.ToString() ) );
                ModeList.SelectedValue = _Mode.ToString();
                ModeList.AutoPostBack = true;
                ModeList.SelectedIndexChanged += new EventHandler( ModeList_SelectedIndexChanged );
                leftph.Controls.Add( ModeList );
            }

            PropDataTable = new CswAutoTable();
            PropDataTable.ID = "proptable";
            leftph.Controls.Add( PropDataTable );

            Int32 NameColumn = 0;
            Int32 SetValOnAddColumn = 1;
            Int32 DisplayRowColumn = 2;
            Int32 DisplayColColumn = 3;

            Int32 row = 0;

            CswNbtMetaDataNodeTypeTab Tab = null;
            if( _Mode != LayoutMode.Add )
            {
                Tab = Master.CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( SelectedTabId ) );

                // Tab data
                Literal TabNameLiteral = new Literal();
                if( _Mode == LayoutMode.Inspection )
                    TabNameLiteral.Text = "<b>Section: " + Tab.TabName + "</b>, Number:";
                else
                    TabNameLiteral.Text = "<b>Tab: " + Tab.TabName + "</b>, Order:";
                PropDataTable.addControl( row, NameColumn, TabNameLiteral );

                TabOrderTextBox = new TextBox();
                TabOrderTextBox.ID = TabOrderIdPrefix + Tab.TabId.ToString();
                TabOrderTextBox.Width = Unit.Parse( "30px" );
                if( Tab.TabOrder != Int32.MinValue )
                    TabOrderTextBox.Text = Tab.TabOrder.ToString();
                PropDataTable.addControl( row, NameColumn, TabOrderTextBox );
                row++;
            }

            // Heading
            Literal HeadingPropNameLiteral = new Literal();
            //if( _Mode == LayoutMode.Inspection )
            //    HeadingPropNameLiteral.Text = "<b>Question</b>";
            //else
            HeadingPropNameLiteral.Text = "<b>Property Name</b>";
            PropDataTable.addControl( row, NameColumn, HeadingPropNameLiteral );

            if( _Mode == LayoutMode.Add )
            {
                Literal HeadingSetValOnAddLiteral = new Literal();
                HeadingSetValOnAddLiteral.Text = "<b>Set Value On Add</b>";
                PropDataTable.addControl( row, SetValOnAddColumn, HeadingSetValOnAddLiteral );
            }

            Literal HeadingDisplayRowLiteral = new Literal();
            if( _Mode == LayoutMode.Inspection )
                HeadingDisplayRowLiteral.Text = "<b>Order</b>";
            else
                HeadingDisplayRowLiteral.Text = "<b>Row</b>";
            PropDataTable.addControl( row, DisplayRowColumn, HeadingDisplayRowLiteral );
            
            if( _Mode == LayoutMode.Edit || _Mode == LayoutMode.Add )
            {
                Literal HeadingDisplayColLiteral = new Literal();
                HeadingDisplayColLiteral.Text = "<b>Column</b>";
                PropDataTable.addControl( row, DisplayColColumn, HeadingDisplayColLiteral );
            }
            row++;

            // Data

            ICollection Props = null;
            if( _Mode == LayoutMode.Add )
                Props = NodeType.NodeTypeProps;
            else
                Props = Tab.NodeTypePropsByDisplayOrder;

            foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
            {
                Literal PropNameLiteral = new Literal();
                PropNameLiteral.Text = Prop.PropNameWithQuestionNo;
                PropDataTable.addControl( row, NameColumn, PropNameLiteral );

                TextBox DisplayRowTextBox = null;
                TextBox DisplayColTextBox = null;
                CheckBox SetValOnAddCheckBox = null;

                DisplayRowTextBox = new TextBox();
                DisplayRowTextBox.ID = DisplayRowIdPrefix + Prop.PropId.ToString();
                DisplayRowTextBox.Width = Unit.Parse( "30px" );
                PropDataTable.addControl( row, DisplayRowColumn, DisplayRowTextBox );

                DisplayColTextBox = new TextBox();
                DisplayColTextBox.ID = DisplayColIdPrefix + Prop.PropId.ToString();
                DisplayColTextBox.Width = Unit.Parse( "30px" );
                PropDataTable.addControl( row, DisplayColColumn, DisplayColTextBox );

                if( _Mode == LayoutMode.Add )
                {
                    SetValOnAddCheckBox = new CheckBox();
                    SetValOnAddCheckBox.ID = SetValOnAddIdPrefix + Prop.PropId.ToString();
                    SetValOnAddCheckBox.Checked = Prop.SetValueOnAdd;
                    PropDataTable.addControl( row, SetValOnAddColumn, SetValOnAddCheckBox );

                    if( Prop.DisplayRowAdd != Int32.MinValue )
                        DisplayRowTextBox.Text = Prop.DisplayRowAdd.ToString();
                    if( Prop.DisplayColAdd != Int32.MinValue )
                        DisplayColTextBox.Text = Prop.DisplayColAdd.ToString();
                }
                else
                {
                    if( Prop.DisplayRow != Int32.MinValue )
                        DisplayRowTextBox.Text = Prop.DisplayRow.ToString();
                    if( Prop.DisplayColumn != Int32.MinValue )
                        DisplayColTextBox.Text = Prop.DisplayColumn.ToString();
                }

                if( _Mode == LayoutMode.Inspection )
                    DisplayColTextBox.Visible = false;

                LayoutControlsHash.Add( Prop.FirstPropVersionId, new LayoutControlSet( DisplayColTextBox, DisplayRowTextBox, SetValOnAddCheckBox ) );
                row++;
            }

            SaveButton = new Button();
            SaveButton.ID = "savebutton";
            SaveButton.CssClass = "button";
            SaveButton.Text = "Save";
            SaveButton.Click += new EventHandler( SaveButton_Click );
            leftph.Controls.Add( SaveButton );

            leftph.Controls.Add( new CswLiteralBr() );

            LinkButton BackToDesignLink = new LinkButton();
            BackToDesignLink.ID = "BackToDesignLink";
            BackToDesignLink.Text = "Back To Design";
            BackToDesignLink.Click += new EventHandler( BackToDesignLink_Click );
            leftph.Controls.Add( BackToDesignLink );
        }

        void TabStrip_TabClick( object sender, Telerik.Web.UI.RadTabStripEventArgs e )
        {
            try
            {
                PropertyTable.SelectedTabId = e.Tab.Value;
                SelectedTabId = PropertyTable.initTabContents();
                initLeftContents();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void ModeList_SelectedIndexChanged( object sender, EventArgs e )
        {
            try
            {
                Master.Redirect( "Design_Layout.aspx?mode=" + ModeList.SelectedValue + "&nodetypeid=" + NodeType.NodeTypeId );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void BackToDesignLink_Click( object sender, EventArgs e )
        {
            try
            {
                if( _Mode == LayoutMode.Inspection )
                    Master.Redirect( "Design.aspx?mode=i" );
                else
                    Master.Redirect( "Design.aspx" );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void SaveButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( _Mode != LayoutMode.Add )
                {
                    if( CswTools.IsInteger( TabOrderTextBox.Text ) )
                    {
                        CswNbtMetaDataNodeTypeTab Tab = Master.CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( SelectedTabId ) );
                        Tab.TabOrder = CswConvert.ToInt32( TabOrderTextBox.Text );
                    }
                }

                foreach( Int32 FirstPropVersionId in LayoutControlsHash.Keys )
                {
                    CswNbtMetaDataNodeTypeProp Prop = NodeType.getNodeTypePropByFirstVersionId( FirstPropVersionId );
                    LayoutControlSet LCS = LayoutControlsHash[FirstPropVersionId] as LayoutControlSet;

                    bool SetValOnAdd = Prop.SetValueOnAdd;
                    Int32 DisplayRow = Prop.DisplayRow;
                    Int32 DisplayColumn = Prop.DisplayColumn;
                    Int32 DisplayRowAdd = Prop.DisplayRowAdd;
                    Int32 DisplayColAdd = Prop.DisplayColAdd;

                    if( _Mode == LayoutMode.Inspection )
                    {
                        if( CswTools.IsInteger( LCS.DisplayRowTextBox.Text ) )
                        {
                            DisplayRow = CswConvert.ToInt32( LCS.DisplayRowTextBox.Text );
                        }
                    }
                    else if( _Mode == LayoutMode.Add )
                    {
                        SetValOnAdd = LCS.SetValOnAddCheckBox.Checked;
                        if( CswTools.IsInteger( LCS.DisplayRowTextBox.Text ) )
                            DisplayRowAdd = CswConvert.ToInt32( LCS.DisplayRowTextBox.Text );
                        if( CswTools.IsInteger( LCS.DisplayColTextBox.Text ) )
                            DisplayColAdd = CswConvert.ToInt32( LCS.DisplayColTextBox.Text );
                    }
                    else
                    {
                        if( CswTools.IsInteger( LCS.DisplayRowTextBox.Text ) )
                            DisplayRow = CswConvert.ToInt32( LCS.DisplayRowTextBox.Text );
                        if( CswTools.IsInteger( LCS.DisplayColTextBox.Text ) )
                            DisplayColumn = CswConvert.ToInt32( LCS.DisplayColTextBox.Text );
                    }

                    Prop.DisplayRow = DisplayRow;
                    Prop.DisplayColumn = DisplayColumn;
                    Prop.DisplayRowAdd = DisplayRowAdd;
                    Prop.DisplayColAdd = DisplayColAdd;
                    Prop.SetValueOnAdd = SetValOnAdd;
                }

                initLeftContents();
                SelectedTabId = PropertyTable.initTabContents();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                Session["DesignLayout_SelectedTabId"] = SelectedTabId;
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnPreRender( e );
        }

    }
}
