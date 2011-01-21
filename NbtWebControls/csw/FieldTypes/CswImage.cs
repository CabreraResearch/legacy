using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.CswWebControls;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswImage : CswFieldTypeWebControl, INamingContainer
    {
        public CswImage( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswImage_DataBinding);
            
        }

        private void CswImage_DataBinding(object sender, EventArgs e)
        {
            // We don't do any loading directly on Images -- we use GetBlob.Aspx
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                // We don't do any saving directly on Images -- we use PutBlob.Aspx
                if( _hiddenClear.Value == "1" )
                    Clear();
            }
        }
        public override void AfterSave()
        {
        }
        public override void Clear()
        {
            if( Prop != null )
            {
                Prop.ClearValue();
                Prop.ClearBlob();
            }
        }


        private ImageButton _Image = null;
        //private HyperLink _Link = null;
        private Label _Label = null;
        private CswImageButton _UploadButton;
        private CswImageButton _ClearButton;
        private HiddenField _hiddenClear;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            TableCell Cell11 = Table.getCell( 0, 0 );
            Cell11.ColumnSpan = 2;
            TableCell Cell22 = Table.getCell( 1, 1 );
            Cell22.HorizontalAlign = HorizontalAlign.Right;
            TableCell Cell23 = Table.getCell( 1, 2 );
            Cell22.HorizontalAlign = HorizontalAlign.Right;

            _Image = new ImageButton();
            _Image.ID = "image";
            Cell11.Controls.Add( _Image );

            _Label = new Label();
            Table.addControl( 1, 0, _Label );

            _ClearButton = new CswImageButton( CswImageButton.ButtonType.Clear );
            _ClearButton.ID = "clear";
            Cell22.Controls.Add( _ClearButton );

            _hiddenClear = new HiddenField();
            _hiddenClear.ID = "hiddenclear";
            Cell22.Controls.Add( _hiddenClear );

            _UploadButton = new CswImageButton( CswImageButton.ButtonType.Edit );
            _UploadButton.ID = "upload";
            Cell23.Controls.Add( _UploadButton );

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if( Prop != null && Prop.AsImage != null )
            {
                if( Prop.AsImage.Height > 0 )
                    _Image.Height = Prop.AsImage.Height;
                if( Prop.AsImage.Width > 0 )
                    _Image.Width = Prop.AsImage.Width;

                _Label.Visible = false;
                _Image.Visible = false;
                if( Prop.AsImage.JctNodePropId > 0 )
                {
                    if( Prop.AsImage.FileName != String.Empty )
                    {
                        _Label.Visible = true;
                        _Image.Visible = true;
                        _Label.Text = Prop.AsImage.FileName;
                        _Image.ImageUrl = "GetBlob.aspx?mode=image&jctnodepropid=" + Prop.AsImage.JctNodePropId + "&nodeid=" + Prop.NodeId.PrimaryKey.ToString() + "&propid=" + PropId.ToString();
                        _Image.OnClientClick = "openBlobPopup('mode=image&jctnodepropid=" + Prop.AsImage.JctNodePropId + "&nodeid=" + Prop.NodeId.PrimaryKey.ToString() + "&propid=" + PropId.ToString() + "');";
                        _Image.AlternateText = Prop.AsImage.FileName;
                    }
                    //_UploadButton.OnClientClick = "return " + this.ClientID + "_editImage('" + Prop.AsImage.JctNodePropId + "');";
                    _UploadButton.OnClientClick = "CswImage_editImage('" + _hiddenClear.ClientID + "', '" + Prop.NodeId.ToString() + "', '" + PropId + "');";
                    //_ClearButton.OnClientClick = "return " + this.ClientID + "_clearImage();";
                    _ClearButton.OnClientClick = "return CswImage_clearImage('" + _hiddenClear.ClientID + "', '" + _Image.ClientID + "', '" + _Label.ClientID + "');";
                }
                else
                {
                    //_UploadButton.OnClientClick = this.ClientID + "_editImage('');";
                    _UploadButton.OnClientClick = "CswImage_editImage('" + _hiddenClear.ClientID + "', '" + Prop.NodeId.ToString() + "', '" + PropId + "');";
                    _Image.ImageUrl = "GetBlob.Aspx?mode=image&nodeid=" + Prop.NodeId.ToString() + "&propid=" + PropId;
                    _Image.OnClientClick = "openBlobPopup('mode=image&nodeid=" + Prop.NodeId.ToString() + "&propid=" + PropId + "');";
                }
            }

            if( ReadOnly || _EditMode == NodeEditMode.AddInPopup || 
                            _EditMode == NodeEditMode.EditInPopup || 
                            _EditMode == NodeEditMode.Demo || 
                            _EditMode == NodeEditMode.PrintReport )
            {
                _ClearButton.Visible = false;
                _UploadButton.Visible = false;
            }

            base.OnPreRender(e);
        }
    }
}
