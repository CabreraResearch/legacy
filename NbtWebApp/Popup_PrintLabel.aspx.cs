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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_PrintLabel : System.Web.UI.Page
    {
        private string _ActiveXString = "<OBJECT ID='labelx' Name='labelx' classid='clsid:A8926827-7F19-48A1-A086-B1A5901DB7F0' codebase='CafLabelPrintUtil.cab#version=0,1,6,0' width=500 height=300 align=center hspace=0 vspace=0></OBJECT>";
        public string CheckedNodeIds = string.Empty;
        private CswNbtNode _Node = null;
                
        protected override void OnInit(EventArgs e)
        {
            try
            {
                //CswNbtNodeKey NodeKey = null;
                CswPrimaryKey NodeId = null;
                Int32 PropId = Int32.MinValue;
                if( Request.QueryString["nodeid"] != null && Request.QueryString["propid"] != null )
                {
                    //NodeKey = new CswNbtNodeKey(Master.CswNbtResources, Request.QueryString["nodekey"].ToString());
                    NodeId = new CswPrimaryKey();
                    NodeId.FromString( Request.QueryString["nodeid"] );
                    _Node = Master.CswNbtResources.Nodes.GetNode( NodeId );
                    PropId = CswConvert.ToInt32( Request.QueryString["propid"] );
                }
                else
                {
                    throw new CswDniException( "Invalid parameters for Print Label", "NodeId and PropId are required for Popup_PrintLabel.aspx" );
                }

                EnsureChildControls();

                CswNbtMetaDataNodeTypeProp MetaDataProp = Master.CswNbtResources.MetaData.getNodeTypeProp(PropId);
                if (MetaDataProp == null)
                    throw new CswDniException("Invalid PropID parameter for Print Label", "Invalid PropID parameter for Print Label: " + PropId);

                _NodeNameValueLiteral.Text = _Node.NodeName;
                _PropNameValueLiteral.Text = _Node.Properties[MetaDataProp].PropName;
                _NodeNameLiteral.Text = _Node.NodeType.NodeTypeName + ":";

                if( Request.QueryString["checkednodeids"] != null )
                    CheckedNodeIds = Request.QueryString["checkednodeids"].ToString();
                if( CheckedNodeIds != string.Empty )
                {
                    Int32 resultcount = 1;  // because we already did one above
                    foreach( string NodeIdToPrintString in CheckedNodeIds.Split( ',' ) )
                    {
                        CswPrimaryKey NodeIdToPrint = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeIdToPrintString ) );
                        if( NodeIdToPrint != _Node.NodeId )
                        {
                            if( resultcount < 5 )
                            {
                                CswNbtNode NodeToPrint = Master.CswNbtResources.Nodes.GetNode( NodeIdToPrint );
                                _NodeNameValueLiteral.Text += ", " + NodeToPrint.NodeName;
                            }
                            else if( resultcount == 5 )
                            {
                                _NodeNameValueLiteral.Text += "...";
                            }
                            resultcount++;
                        }
                    }
                    _NodeNameValueLiteral.Text += " (" + resultcount.ToString() + " total)";
                }


                if (!Page.IsPostBack)
                {
                    ICswNbtTree PrintLabelsTree = getPrintLabelsForNodeType(_Node.NodeTypeId);
                    PrintLabelsTree.goToRoot();
                    _PrintLabelDropDown.Items.Clear();
                    for (int i = 0; i < PrintLabelsTree.getChildNodeCount(); i++)
                    {
                        PrintLabelsTree.goToNthChild(i);
                        _PrintLabelDropDown.Items.Add(new ListItem(PrintLabelsTree.getNodeNameForCurrentPosition(), PrintLabelsTree.getNodeIdForCurrentPosition().ToString()));
                        PrintLabelsTree.goToParentNode();
                    }
                }
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }

            base.OnInit(e);
        }
        

        private TextBox _EPLBox;
        private DropDownList _PrintLabelDropDown;
        private Button _PrintButton;
        private Literal _ActiveXLiteral;
        private Button _CancelButton;

        private Literal _NodeNameLiteral;
        private Literal _NodeNameValueLiteral;
        private Literal _PropNameLiteral;
        private Literal _PropNameValueLiteral;
        private Literal _PrintLabelLiteral;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            Table.ID = "Table";
            Table.FirstCellRightAlign = true;
            ph.Controls.Add(Table);

            _NodeNameLiteral = new Literal();
            _NodeNameLiteral.ID = "nodenameliteral";
            Table.addControl(0, 0, _NodeNameLiteral);

            _NodeNameValueLiteral = new Literal();
            _NodeNameValueLiteral.ID = "nodenamevalueliteral";
            Table.addControl(0, 1, _NodeNameValueLiteral);

            _PropNameLiteral = new Literal();
            _PropNameLiteral.ID = "propnameliteral";
            _PropNameLiteral.Text = "Property:";
            Table.addControl(1, 0, _PropNameLiteral);

            _PropNameValueLiteral = new Literal();
            _PropNameValueLiteral.ID = "propnamevalueliteral";
            Table.addControl(1, 1, _PropNameValueLiteral);
            
            _PrintLabelLiteral = new Literal();
            _PrintLabelLiteral.ID = "printlabelliteral";
            _PrintLabelLiteral.Text = "Label:";
            Table.addControl(2, 0, _PrintLabelLiteral);

            _PrintLabelDropDown = new DropDownList();
            _PrintLabelDropDown.ID = "printlabel";
            _PrintLabelDropDown.CssClass = "selectinput";
            //_PrintLabelDropDown.SelectedIndexChanged += new EventHandler(_PrintLabelDropDown_SelectedIndexChanged);
            _PrintLabelDropDown.AutoPostBack = true;
            Table.addControl(2, 1, _PrintLabelDropDown);

            _PrintButton = new Button();
            _PrintButton.ID = "print";
            _PrintButton.Text = "Print";
            _PrintButton.CssClass = "Button";
            Table.addControl(3, 1, _PrintButton);

            _CancelButton = new Button();
            _CancelButton.ID = "cancel";
            _CancelButton.Text = "Cancel";
            _CancelButton.CssClass = "Button";
            Table.addControl(3, 1, _CancelButton);

            HtmlGenericControl HiddenDiv = new HtmlGenericControl("div");
            HiddenDiv.ID = "hidden";
            HiddenDiv.Style.Add("display", "none");
            ph.Controls.Add(HiddenDiv);

            _EPLBox = new TextBox();
            _EPLBox.ID = "eplscript";
            _EPLBox.TextMode = TextBoxMode.MultiLine;
            HiddenDiv.Controls.Add(_EPLBox);

            _ActiveXLiteral = new Literal();
            _ActiveXLiteral.Text = _ActiveXString;
            HiddenDiv.Controls.Add(_ActiveXLiteral);

            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                if( _PrintLabelDropDown.Items.Count > 0 )
                {
                    CswPrimaryKey PrintLabelKey = new CswPrimaryKey();
                    PrintLabelKey.FromString( _PrintLabelDropDown.SelectedValue );
                    _initEplText( PrintLabelKey );
                }
                else
                {
                    _EPLBox.Visible = false;
                    _PrintLabelDropDown.Visible = false;
                    _PrintButton.Visible = false;
                    _ActiveXLiteral.Visible = false;

                    _NodeNameLiteral.Visible = false;
                    _NodeNameValueLiteral.Visible = false;
                    _PropNameLiteral.Visible = false;
                    _PropNameValueLiteral.Visible = false;
                    _PrintLabelLiteral.Visible = false;

                    Label ErrorLiteral = new Label();
                    ErrorLiteral.CssClass = "ErrorContent";
                    ErrorLiteral.Text = "There are no print labels assigned to " + _Node.NodeType.NodeTypeName + ".";
                    ph.Controls.AddAt(0, ErrorLiteral );
                }
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                _PrintButton.OnClientClick = "return Popup_PrintLabel_Print('" + _EPLBox.ClientID + "');";
                _CancelButton.OnClientClick = "Popup_Cancel_Clicked();";

            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }

            base.OnPreRender(e);
        }


        private void _initEplText(CswPrimaryKey PrintLabelId)
        {
            CswNbtNode PrintLabelNode = Master.CswNbtResources.Nodes.GetNode(PrintLabelId);
            CswNbtObjClassPrintLabel NodeAsPrintLabel = (CswNbtObjClassPrintLabel) CswNbtNodeCaster.AsPrintLabel( PrintLabelNode );

            string EPLText = NodeAsPrintLabel.epltext.Text;
            string Params = NodeAsPrintLabel.Params.Text;

            // BZ 6118 - this prevents " from being turned into &quot;
            // BUT SEE BZ 7881!
            _EPLBox.Text = GenerateEPLScript(EPLText, Params, _Node) + "\n";
            if( CheckedNodeIds != string.Empty )
            {
                foreach( string NodeIdToPrintString in CheckedNodeIds.Split( ',' ) )
                {
                    CswPrimaryKey NodeIdToPrint = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeIdToPrintString ) );
                    if( NodeIdToPrint != _Node.NodeId )
                    {
                        CswNbtNode NodeToPrint = Master.CswNbtResources.Nodes.GetNode( NodeIdToPrint );
                        _EPLBox.Text += GenerateEPLScript( EPLText, Params, NodeToPrint ) + "\n";
                    }
                }
            }
        }

        private string GenerateEPLScript(string EPLText, string Params, CswNbtNode Node)
        {
            string EPLScript = EPLText;
            string[] ParamsArray = Params.Split('\n');

            while (EPLScript.Contains("{"))
            {
                Int32 ParamStartIndex = EPLScript.IndexOf("{");
                Int32 ParamEndIndex = EPLScript.IndexOf("}");

                string PropertyParamString = EPLScript.Substring(ParamStartIndex, ParamEndIndex - ParamStartIndex + 1);
                string PropertyParamName = PropertyParamString.Substring(1, PropertyParamString.Length - 2);
                // Find the property
                CswNbtMetaDataNodeType MetaDataNodeType = Master.CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
                CswNbtMetaDataNodeTypeProp MetaDataProp = MetaDataNodeType.getNodeTypeProp(PropertyParamName);
                string PropertyValue = ( (CswNbtNodePropWrapper) Node.Properties[MetaDataProp] ).Gestalt;

                bool FoundMatch = false;
                foreach (string ParamNVP in ParamsArray)
                {
                    string[] ParamSplit = ParamNVP.Split('=');
                    if (ParamSplit[0] == PropertyParamName && CswTools.IsInteger(ParamSplit[1]))
                    {
                        FoundMatch = true;
                        Int32 MaxLength = CswConvert.ToInt32(ParamSplit[1]);
                        Int32 CurrentIteration = 1;
                        while (ParamStartIndex >= 0)
                        {
                            if (PropertyValue.Length > MaxLength)
                            {
                                EPLScript = EPLScript.Substring(0, ParamStartIndex) + PropertyValue.Substring(0, MaxLength) + EPLScript.Substring(ParamEndIndex + 1);
                                PropertyValue = PropertyValue.Substring(MaxLength + 1);
                            }
                            else
                            {
                                EPLScript = EPLScript.Substring(0, ParamStartIndex) + PropertyValue + EPLScript.Substring(ParamEndIndex + 1);
                                PropertyValue = "";
                            }
                            CurrentIteration++;
                            ParamStartIndex = EPLScript.IndexOf("{" + PropertyParamName + "_" + CurrentIteration + "}");
                            ParamEndIndex = ParamStartIndex + ("{" + PropertyParamName + "_" + CurrentIteration + "}").Length - 1;
                        }
                    }
                }
                if (!FoundMatch)
                {
                    EPLScript = EPLScript.Substring(0, ParamStartIndex) + PropertyValue + EPLScript.Substring(ParamEndIndex + 1);
                }
            }

            return EPLScript;
        }

        private ICswNbtTree getPrintLabelsForNodeType(Int32 NodeTypeId)
        {
            CswTimer Timer = new CswTimer();

            string PrintLabelNodeTypesPropertyName = "NodeTypes";
            CswNbtMetaDataObjectClass PrintLabelObjectClass = Master.CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass);
            CswNbtMetaDataObjectClassProp NodeTypesProperty = PrintLabelObjectClass.getObjectClassProp(PrintLabelNodeTypesPropertyName);

            CswNbtView PrintLabelView = new CswNbtView(Master.CswNbtResources);
            PrintLabelView.ViewName = "getPrintLabelsForNodeType(" + NodeTypeId.ToString() + ")";
            CswNbtViewRelationship PrintLabelRelationship = PrintLabelView.AddViewRelationship( PrintLabelObjectClass, true );
            CswNbtViewProperty PrintLabelNodeTypesProperty = PrintLabelView.AddViewProperty( PrintLabelRelationship, NodeTypesProperty );
            CswNbtViewPropertyFilter PrintLabelNodeTypesPropertyFilter = PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Contains, NodeTypeId.ToString(), false );

            ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView(PrintLabelView, true, true, false, false);

            Master.CswNbtResources.logTimerResult("Running getPrintLabelsForNodeType", Timer.ElapsedDurationInSecondsAsString);

            return Tree;
        }

    } // class Popup_PrintLabel
} // namespace ChemSW.Nbt.WebPages