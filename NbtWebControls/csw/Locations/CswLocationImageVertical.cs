using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.CswWebControls;
using ChemSW.Core;

namespace ChemSW.NbtWebControls
{
    public class CswLocationImageVertical : CswLocationImage
    {
        private Int32 _ElementCount = 0;
        private Int32 _ElementCountPad = 4;

        public CswLocationImageVertical( CswNbtResources CswNbtResources, XmlNode ParentNode, CswPrimaryKey SelectedNodeId )
            : base( CswNbtResources, ParentNode, SelectedNodeId )
        {
            _ElementCount = _ParentNodeSet.ChildNodes.Count;
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();
            base.OnLoad(e);
        }

        private CswAutoTable _Table;
        protected override void CreateChildControls()
        {
            _Table = new CswAutoTable();
            _Table.CellSpacing = 0;
            _Table.CellPadding = 0;
            this.Controls.Add(_Table);

            CswImageOverlay Title = new CswImageOverlay();
            Title.ID = "title";
            Title.ButtonText = KeyPrefix + _ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Key).InnerText;
            Title.ImageUrl = ImageUrlBase + "v_title.gif";
            Title.ImageHeight = 34;
            Title.ImageWidth = 180;
            Title.IsButton = true;
            Title.Click += new EventHandler(Image_Click);
            _Table.addControl(0, 0, Title);
            
            Label TitleLabel = new Label();
            TitleLabel.ID = this.ID + "_label";
            TitleLabel.Text = _ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Display).InnerText;
            if (_SelectedNodeId.PrimaryKey == Convert.ToInt32(_ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Key).InnerText))
                TitleLabel.CssClass = "LocationTitleTextSelected";
            else
                TitleLabel.CssClass = "LocationTitleText";
            Title.Content.Add(TitleLabel);

            Int32 ElementsMade = 0;
            Int32 ImagesMade = 0;
            if (_MoveMode)
            {
                _Table.addControl((ImagesMade * 2) + 1, 0, makeSpacer());
                _Table.addControl((ImagesMade * 2) + 2, 0, makeImage(KeyPrefix, new CswPrimaryKey("nodes", Convert.ToInt32(_ParentNode.SelectSingleNode(CswNbtLocationTree.XmlNodeName_Key).InnerText)),
                                                            "", "", CswNbtLocationTree.VerticalLocationTemplate.Slot, true));
                ImagesMade++;
            }

            while (ElementsMade < _ElementCount)
            {



                if (_ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_ObjectClass).InnerText == "LocationClass")
                {
                    //Locations
                    _Table.addControl((ImagesMade * 2) + 1, 0, makeImage(KeyPrefix,
                                                                          new CswPrimaryKey("nodes", Convert.ToInt32(_ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_Key).InnerText)),
                                                                         _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_Display).InnerText,
                                                                         //_ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_IconFileName).InnerText,
                                                                         string.Empty,
                                                                         (CswNbtLocationTree.VerticalLocationTemplate)Enum.Parse(typeof(CswNbtLocationTree.VerticalLocationTemplate), _ParentNodeSet.ChildNodes[ElementsMade].Attributes[CswNbtLocationTree.XmlAttrName_LocationTemplate].Value),
                                                                         true));
                    _Table.addControl((ImagesMade * 2) + 2, 0, makeSpacer());
                    ElementsMade++;
                    ImagesMade++;
                }
                else
                {
                    //Non-Locations
                    _Table.addControl((ImagesMade * 2) + 1, 0, makeImage(NodePrefix, 
                                                                         new CswPrimaryKey("nodes", Convert.ToInt32(_ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_Key).InnerText)),
                                                                         _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_Display).InnerText,
                                                                         _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTree.XmlNodeName_IconFileName).InnerText,
                                                                         CswNbtLocationTree.VerticalLocationTemplate.Empty,
                                                                         !_MoveMode));
                    _Table.addControl((ImagesMade * 2) + 2, 0, makeSpacer());
                    ElementsMade++;
                    ImagesMade++;
                }
            }

            while (ImagesMade < _ElementCountPad)
            {
                _Table.addControl((ImagesMade * 2) + 1, 0, makeImage(KeyPrefix, null, "", "", CswNbtLocationTree.VerticalLocationTemplate.Empty, false));
                _Table.addControl((ImagesMade * 2) + 2, 0, makeSpacer());
                ImagesMade++;
            }

            Image Bottom = new Image();
            Bottom.ImageUrl = ImageUrlBase + "v_bottom.gif";
            Bottom.ID = "bottom";
            Bottom.Width = Unit.Parse("180px");
            Bottom.Height = Unit.Parse("34px");
            _Table.addControl((ImagesMade * 2) + 1, 0, Bottom);

            base.CreateChildControls();
        }

        private Image makeSpacer()
        {
            Image Spacer = new Image();
            Spacer.ImageUrl = ImageUrlBase + "v_spacer.gif";
            Spacer.Width = Unit.Parse("180px");
            Spacer.Height = Unit.Parse("10px");
            return Spacer;
        }

        private CswImageOverlay makeImage( string Prefix, CswPrimaryKey NodeId, string Name, string IconFileName, CswNbtLocationTree.VerticalLocationTemplate Template, bool IsButton )
        {
            CswImageOverlay Image = new CswImageOverlay();
            if (IsButton && NodeId != null)
            {
                Image.ID = Prefix + NodeId;
                Image.ButtonText = Prefix + NodeId;
            }

            Image.ImageWidth = 180;
            Image.ImageHeight = 34;
            Image.IsButton = (IsButton && NodeId != _PropOwnerNodeId);

            switch (Template)
            {
                case CswNbtLocationTree.VerticalLocationTemplate.Shelf:
                    Image.ImageUrl = "v_shelf";
                    break;
                case CswNbtLocationTree.VerticalLocationTemplate.Slot:
                    Image.ImageUrl = "v_slot";
                    break;
                case CswNbtLocationTree.VerticalLocationTemplate.Empty:
                    Image.ImageUrl = "v_empty";
                    break;
            }
            if (Template != CswNbtLocationTree.VerticalLocationTemplate.Empty)
            {
                Image.HoverImageUrl = Image.ImageUrl + "_hover";
                if (_SelectedNodeId == NodeId)
                {
                    Image.ImageUrl += "_selected";
                    Image.HoverImageUrl += "_selected";
                }
            }

            Image.ImageUrl = ImageUrlBase + Image.ImageUrl + ".gif";
            if (Image.HoverImageUrl != String.Empty)
                Image.HoverImageUrl = ImageUrlBase + Image.HoverImageUrl + ".gif";

            if (IconFileName != String.Empty)
            {
                Image Icon = new Image();
                Icon.ImageUrl = "Images/icons/" + IconFileName;
                Icon.Style.Add(HtmlTextWriterStyle.Position, "relative");
                Icon.Style.Add(HtmlTextWriterStyle.Top, "40%");
                Image.Content.Add(Icon);
            }

            Label Label = new Label();
            Label.ID = this.ID + "_label";
            Label.Text = Name;
            if (NodeId == _SelectedNodeId)
                Label.CssClass = "LocationTextSelected";
            else
                Label.CssClass = "LocationText";
            Label.Style.Add(HtmlTextWriterStyle.Position, "relative");
            Label.Style.Add(HtmlTextWriterStyle.Top, "40%");
            Image.Content.Add(Label);

            Image.Click += new EventHandler(Image_Click);
            return Image;
        }
    }
}