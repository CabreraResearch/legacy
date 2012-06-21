using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt;

namespace ChemSW.NbtWebControls
{

    public class CswLocationImageHorizontal : CswLocationImage
    {
        private Int32 _ElementCount = 0;
        private Int32 _ElementCountPad = 4;

        public CswLocationImageHorizontal( CswNbtResources CswNbtResources, XmlNode ParentNode, CswPrimaryKey SelectedNodeId )
            : base( CswNbtResources, ParentNode, SelectedNodeId )
        {
            _ElementCount = _ParentNodeSet.ChildNodes.Count;
        }

        protected override void OnLoad( EventArgs e )
        {
            EnsureChildControls();
            base.OnLoad( e );
        }

        private CswAutoTable _Table;
        protected override void CreateChildControls()
        {
            _Table = new CswAutoTable();
            _Table.CellSpacing = 0;
            _Table.CellPadding = 0;
            _Table.ID = "horiztable";
            this.Controls.Add( _Table );

            Image TitleLeft = new Image();
            TitleLeft.ImageUrl = ImageUrlBase + "h_title_left.gif";
            TitleLeft.ID = "title_left";
            TitleLeft.Width = Unit.Parse( "34px" );
            TitleLeft.Height = Unit.Parse( "30px" );
            _Table.addControl( 0, 0, TitleLeft );

            Image Left = new Image();
            Left.ImageUrl = ImageUrlBase + "h_left.gif";
            Left.ID = "left";
            Left.Width = Unit.Parse( "34px" );
            Left.Height = Unit.Parse( "180px" );
            _Table.addControl( 1, 0, Left );

            TableCell TitleCell = _Table.getCell( 0, 1 );
            TitleCell.Attributes.Add( "colspan", "100%" );

            CswImageOverlay Title = new CswImageOverlay();
            Title.ID = "title";
            Title.ButtonText = KeyPrefix + _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText;
            Title.ImageUrl = ImageUrlBase + "h_title.gif";
            Title.ImageHeight = 30;
            Title.IsButton = true;
            Title.Click += new EventHandler( Image_Click );
            _Table.addControl( 0, 1, Title );

            Label TitleLabel = new Label();
            TitleLabel.ID = this.ID + "_label";
            TitleLabel.Text = _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Display ).InnerText;
            if( _SelectedNodeId != null && _SelectedNodeId.PrimaryKey == CswConvert.ToInt32( _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText ) )
                TitleLabel.CssClass = "LocationTitleTextSelected";
            else
                TitleLabel.CssClass = "LocationTitleText";
            Title.Content.Add( TitleLabel );

            Int32 ElementsMade = 0;
            while( ElementsMade < _ElementCount )
            {
                if( _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_ObjectClass ).InnerText == "LocationClass" )
                {
                    //Locations
                    _Table.addControl( 1, ( ElementsMade * 2 ) + 1, makeSpacer() );
                    _Table.addControl( 1, ( ElementsMade * 2 ) + 2, makeImage( KeyPrefix, new CswPrimaryKey( "nodes", CswConvert.ToInt32( _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText ) ),
                                                                                   _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Display ).InnerText,
                        //_ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode(CswNbtLocationTreeDeprecatedDeprecated.XmlNodeName_IconFileName).InnerText,
                                                                                   string.Empty,
                                                                                   (CswNbtLocationTreeDeprecated.HorizontalLocationTemplate) Enum.Parse( typeof( CswNbtLocationTreeDeprecated.HorizontalLocationTemplate ), _ParentNodeSet.ChildNodes[ElementsMade].Attributes[CswNbtLocationTreeDeprecated.XmlAttrName_LocationTemplate].Value ),
                                                                                   true ) );
                }
                else
                {
                    //Non-Locations
                    _Table.addControl( 1, ( ElementsMade * 2 ) + 1, makeSpacer() );
                    _Table.addControl( 1, ( ElementsMade * 2 ) + 2, makeImage( NodePrefix, new CswPrimaryKey( "nodes", CswConvert.ToInt32( _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText ) ),
                                                                                   _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Display ).InnerText,
                                                                                   _ParentNodeSet.ChildNodes[ElementsMade].SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_IconFileName ).InnerText,
                                                                                   CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Empty,
                                                                                   !_MoveMode ) );

                }
                ElementsMade++;
            }

            if( _MoveMode )
            {
                _Table.addControl( 1, ( ElementsMade * 2 ) + 1, makeSpacer() );
                Int32 SelectSingleNodeId = Int32.MinValue;
                if( _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText != string.Empty )
                    SelectSingleNodeId = CswConvert.ToInt32( _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText );
                _Table.addControl( 1, ( ElementsMade * 2 ) + 2, makeImage( KeyPrefix, new CswPrimaryKey( "nodes", SelectSingleNodeId ),
                                                                               "", "", CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Slot, true ) );
                ElementsMade++;
            }

            while( ElementsMade < _ElementCountPad )
            {
                _Table.addControl( 1, ( ElementsMade * 2 ) + 1, makeSpacer() );
                _Table.addControl( 1, ( ElementsMade * 2 ) + 2, makeImage( KeyPrefix, null, "", "", CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Empty, false ) );
                ElementsMade++;
            }

            base.CreateChildControls();
        }

        private Image makeSpacer()
        {
            Image Spacer = new Image();
            Spacer.ImageUrl = ImageUrlBase + "h_spacer.gif";
            Spacer.Width = Unit.Parse( "10px" );
            Spacer.Height = Unit.Parse( "180px" );
            return Spacer;
        }

        private CswImageOverlay makeImage( string Prefix, CswPrimaryKey NodeId, string Name, string IconFileName, CswNbtLocationTreeDeprecated.HorizontalLocationTemplate Template, bool IsButton )
        {
            CswImageOverlay Image = new CswImageOverlay();
            if( IsButton && NodeId != null )
            {
                Image.ID = Prefix + NodeId;
                Image.ButtonText = Prefix + NodeId;
            }

            Image.ImageWidth = 80;
            Image.ImageHeight = 180;
            Image.IsButton = ( IsButton && NodeId != _PropOwnerNodeId );

            if( Template == CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Unknown )
                Template = CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Empty;

            switch( Template )
            {
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.ShortBox:
                    Image.ImageUrl = "h_box_short";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.MediumBox:
                    Image.ImageUrl = "h_box_medium";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.TallBox:
                    Image.ImageUrl = "h_box_tall";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Door:
                    Image.ImageUrl = "h_door";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Fridge:
                    Image.ImageUrl = "h_fridge";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Building:
                    Image.ImageUrl = "h_building";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Cabinet:
                    Image.ImageUrl = "h_cabinet";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.SafetyCabinet:
                    Image.ImageUrl = "h_safetycabinet";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Slot:
                    Image.ImageUrl = "h_slot";
                    break;
                case CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Empty:
                    Image.ImageUrl = "h_empty";
                    break;
            }
            if( Template != CswNbtLocationTreeDeprecated.HorizontalLocationTemplate.Empty )
            {
                Image.HoverImageUrl = Image.ImageUrl + "_hover";
                if( _SelectedNodeId == NodeId )
                {
                    Image.ImageUrl += "_selected";
                    Image.HoverImageUrl += "_selected";
                }
            }

            Image.ImageUrl = ImageUrlBase + Image.ImageUrl + ".gif";
            if( Image.HoverImageUrl != String.Empty )
                Image.HoverImageUrl = ImageUrlBase + Image.HoverImageUrl + ".gif";

            if( IconFileName != String.Empty )
            {
                Image Icon = new Image();
                Icon.ImageUrl = "Images/icons/" + IconFileName;
                Icon.Style.Add( HtmlTextWriterStyle.Position, "relative" );
                Icon.Style.Add( HtmlTextWriterStyle.Top, "60px" );
                Image.Content.Add( Icon );
            }

            Label Label = new Label();
            Label.ID = this.ID + "_label";
            Label.Text = Name;
            if( NodeId == _SelectedNodeId )
                Label.CssClass = "LocationTextSelected";
            else
                Label.CssClass = "LocationText";
            Label.Style.Add( HtmlTextWriterStyle.Position, "relative" );
            Label.Style.Add( HtmlTextWriterStyle.Top, "60px" );
            Image.Content.Add( Label );

            Image.Click += new EventHandler( Image_Click );
            return Image;
        }
    }
}