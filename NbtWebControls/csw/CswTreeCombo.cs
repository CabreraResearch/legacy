using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Core;
using Telerik.Web.UI;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    [ToolboxData("<{0}:CswTreeCombo runat=server></{0}:CswTreeCombo>")]
    public class CswTreeCombo : CompositeControl, INamingContainer//, IPostBackDataHandler
    {
        public string TextBoxCssClass = "textinput";
        public string TextBoxCssClassInvalid = "textinput_invalid";
        public string DropDownCssClass = "selectinput";
        public string DropDownCssClassInvalid = "selectinput_invalid";
        public string ClientSideOnInvalid = string.Empty;
        //public string ValidationGroup = string.Empty;
        public bool Required = false;
        public bool ReadOnly = false;
        public bool ShowEdit = true;
        public bool ShowClear = true;

        public CswTreeCombo(RadTreeView TreeView)
        {
            _TreeView = TreeView;
        }


        public string SelectedTreeViewNodeText
        {
            get
            {
                EnsureChildControls();
                RadTreeNode Node = _TreeView.SelectedNode;
                if (Node != null)
                    return Node.Text;
                else
                    return string.Empty;
            }
        }
        public string SelectedTreeViewNodeValue
        {
            get
            {
                EnsureChildControls();
                RadTreeNode Node = _TreeView.SelectedNode;
                if (Node != null)
                    return Node.Value;
                else
                    return string.Empty;
            }
            set
            {
                EnsureChildControls();
                if (value == string.Empty)
                {
                    // clear selected
                    RadTreeNode Node = _TreeView.SelectedNode;
                    if (Node != null)
                        Node.Selected = false;
                }
                else
                {
                    RadTreeNode Node = _TreeView.FindNodeByValue(value);
                    if (Node != null)
                        Node.Selected = true;
                }
                synchControls(value);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        private RadTreeView _TreeView;
        private RadComboBox _Combo;
        //private HiddenField _HiddenNodeValue;
        private CswImageButton _ClearButton;
        private CswImageButton _EditButton;
        //private Sample.Web.UI.Compatibility.CustomValidator _Validator;

        public RadTreeView TreeView
        {
            get { EnsureChildControls(); return _TreeView; }
        }

        public RadComboBox ComboBox
        {
            get { EnsureChildControls(); return _Combo; }
        }

        public CswImageButton EditButton
        {
            get
            {
                EnsureChildControls();
                return _EditButton;
            }
        }

        //public HiddenField HiddenFieldSelectedNodeValue
        //{
        //    get 
        //    {
        //        EnsureChildControls();
        //        return _HiddenNodeValue;
        //    }
        //}

        private class ComboTreeTemplate : ITemplate
        {
            private RadTreeView _TreeView;
            public ComboTreeTemplate(RadTreeView TreeView)
            {
                _TreeView = TreeView;
            }
            public void InstantiateIn(Control Parent)
            {
                HtmlGenericControl TreeDiv = new HtmlGenericControl();
                TreeDiv.ID = "treediv";
                TreeDiv.Attributes.Add("onclick", "StopPropagation");
                Parent.Controls.Add(TreeDiv);

                TreeDiv.Controls.Add(_TreeView);
            }
        }

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add(Table);

            HtmlGenericControl ComboDiv = new HtmlGenericControl();
            ComboDiv.ID = "cd";
            Table.addControl(0, 0, ComboDiv);

            _Combo = new RadComboBox();
            _Combo.AutoPostBack = false;
            _Combo.AllowCustomText = true;   // setting this to false means the box is always empty
            _Combo.EnableTextSelection = false;
            _Combo.EnableLoadOnDemand = false;
            _Combo.Width = Unit.Parse("270px");
            _Combo.ID = "combo";
            _Combo.EnableEmbeddedSkins = false;
            _Combo.Skin = "ChemSW";
            _TreeView.Height = 300;
            ComboDiv.Controls.Add(_Combo);

            _Combo.ItemTemplate = new ComboTreeTemplate(_TreeView);
            _Combo.Items.Add(new RadComboBoxItem());

            //HtmlGenericControl ImageDiv = new HtmlGenericControl("div");
            //ImageDiv.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
            //_Combo.DropDownFooter.Controls.Add(ImageDiv);
            //Image DropDownResizeImage = new Image();
            //DropDownResizeImage.ID = "comboresize";
            //DropDownResizeImage.ImageUrl = "Images/combo/resize.gif";
            //ImageDiv.Controls.Add(DropDownResizeImage);

            //_HiddenNodeValue = new HiddenField();
            //_HiddenNodeValue.ID = "value";
            //Table.addControl(0, 0, _HiddenNodeValue);

            //_Validator = new Sample.Web.UI.Compatibility.CustomValidator();
            //_Validator.ID = "vld";
            //_Validator.ValidateEmptyText = true;
            //_Validator.ValidationGroup = ValidationGroup;
            //Table.addControl(0, 0, _Validator);

            _ClearButton = new CswImageButton(CswImageButton.ButtonType.Clear);
            _ClearButton.ID = "clear";
            Table.addControl(0, 1, _ClearButton);

            _EditButton = new CswImageButton(CswImageButton.ButtonType.View);
            _EditButton.ID = "edit";
            //_EditButton.Click += new ImageClickEventHandler(_EditButton_Click);
            _EditButton.Click += new EventHandler(_EditButton_Click);
            Table.addControl(0, 1, _EditButton);

            base.CreateChildControls();
        }

        void _TreeView_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (TreeViewNodeSelected != null)
                TreeViewNodeSelected(sender, e);
        }

        public event EventHandler EditButtonClick = null;
        protected void _EditButton_Click(object sender, EventArgs e)
        {
            if (EditButtonClick != null)
                EditButtonClick(sender, e);
        }

        public event RadTreeViewEventHandler TreeViewNodeSelected = null;


        private void synchControls(string TreeViewNodeKey)
        {
            EnsureChildControls();

            if (TreeViewNodeKey != string.Empty)
            {
                //_HiddenNodeValue.Value = TreeViewNodeId.ToString();
                RadTreeNode Node = TreeView.FindNodeByValue(TreeViewNodeKey);
                if (Node != null)
                    Node.Selected = true;

                if (TreeView.SelectedNode != null)
                {
                    _Combo.Text = TreeView.SelectedNode.Text;
                    //_Combo.SelectedValue = TreeView.SelectedNode.Value;
                }
                else
                {
                    _Combo.Text = string.Empty;
                }
            }
            else
            {
                //_HiddenNodeValue.Value = string.Empty;
                _Combo.Text = string.Empty;
            }
        }

        public bool KeepExpanded = true;

        protected override void OnPreRender(EventArgs e)
        {
            if (TreeView.SelectedNode != null)
                _Combo.Text = TreeView.SelectedNode.Text;
            else
                _Combo.Text = string.Empty;

            if (!ShowEdit)
                EditButton.Visible = false;
            else
                EditButton.Visible = true;

            if (!ShowClear)
                _ClearButton.Visible = false;
            else
                _ClearButton.Visible = true;

            //Page.RegisterRequiresPostBack(this);

            if (KeepExpanded)
                TreeView.ExpandAllNodes();

            _ClearButton.OnClientClick = "return CswTreeCombo_clear('" + _Combo.ClientID + "');";
            if (TreeView.OnClientNodeClicked == string.Empty)
                TreeView.OnClientNodeClicked = "CswTreeCombo_TreeNodeSelect";

            //            String JS = @"  <script language=""Javascript""> 
            //
            //                            function " + this.ClientID + @"_clear()
            //                            {
            //                                " + _Combo.ClientID + @".set_text('');
            //                                document.getElementById('" + _HiddenNodeValue.ClientID + @"').value = '';
            //                                return false;
            //                            }
            //
            //                            function " + this.ClientID + @"_onComboTreeNodeSelect(sender, args)
            //                            {
            //                                var comboBox = $find('" + _Combo.ClientID + @"');
            //                                var node = args.get_node()
            //                                
            //                                comboBox.set_text(node.get_text());
            //                                
            //                                comboBox.hideDropDown();
            //                                document.getElementById('" + _HiddenNodeValue.ClientID + @"').value = node.Value;
            //                 
            //                                " + ClientOnComboTreeNodeSelect + @"
            //
            //                                return false;
            //                            }
            //
            //                            </script> ";

            //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_JS", JS, false);



            //initValidationJS(); //bz # 5690

            base.OnPreRender(e);
        }

        //public string ClientOnComboTreeNodeSelect = string.Empty;

        //        protected void initValidationJS()
        //        {
        //            _Validator.ClientValidationFunction = this.ClientID + "_validate";

        //            string JS = @"  <script language='Javascript'>
        //
        //                            function " + this.ClientID + @"_validate(sender, args)
        //                            {
        //                                var combo = document.getElementById('" + _Combo.ClientID + @"_Input');
        //                                args.IsValid = true;
        //                                var regex;
        //                                var msg;
        //                                var invalidMsg;
        //                            ";
        //                                        if (Required)
        //                                        {
        //                                            JS += @"
        //                                regex = /[^\s]/g;
        //                                msg = 'Value is required';
        //                                if(args.IsValid && !regex.test(combo.value))
        //                                {
        //                                    args.IsValid = false; 
        //                                    invalidMsg = msg;
        //                                }
        //                            ";
        //                                        }

        //                                        JS += @"
        //                                if(!args.IsValid)
        //                                {
        //                                    combo.className = '" + DropDownCssClassInvalid + @"';
        //                                    combo.title = invalidMsg;
        //                                    " + ClientSideOnInvalid + @"
        //                                } else {
        //                                    combo.className = '" + DropDownCssClass + @"';
        //                                    combo.title = '" + _Combo.ToolTip + @"';
        //                                }
        //                            }
        //                            </script>
        //                            ";

        //            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_vldJS", JS, false);
        //        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (ReadOnly)
            {
                _ClearButton.Visible = false;
                _EditButton.Visible = false;
                _Combo.Visible = false;
                TreeView.Visible = false;
                if (TreeView.SelectedNode != null)
                    writer.Write(TreeView.SelectedNode.Text);
            }
            //else
            //{
            //}
            base.RenderControl(writer);
        }

        public string Text
        {
            get
            {
                EnsureChildControls();
                return _Combo.Text;
            }
            set
            {
                EnsureChildControls();
                _Combo.Text = value;
            }
        }
    }
}
