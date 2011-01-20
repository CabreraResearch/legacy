using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswFile : CswFieldTypeWebControl, INamingContainer
    {
        public CswFile(CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswFile_DataBinding);
        }

        private void CswFile_DataBinding(object sender, EventArgs e)
        {
            // We don't do any loading directly on Files -- we use GetBlob.Aspx
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                // We don't do any saving directly on Files -- we use PutBlob.Aspx
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
   
        //private Literal _Spacer = null;
        private HyperLink _FileLink = null;
        private CswImageButton _UploadButton;
        private CswImageButton _ClearButton;
        private HiddenField _hiddenClear;


        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _FileLink = new HyperLink();

            //_Spacer = new Literal();
            //_Spacer.Text = "&nbsp;&nbsp;&nbsp;";

            _ClearButton = new CswImageButton( CswImageButton.ButtonType.Clear );
            _ClearButton.ID = "clear";

            _hiddenClear = new HiddenField();
            _hiddenClear.ID = "hiddenclear";

            _UploadButton = new CswImageButton( CswImageButton.ButtonType.Edit );
            _UploadButton.ID = "upload";

            Table.addControl( 0, 0, _FileLink );
            Table.addControl( 0, 1, new CswLiteralNbsp() );
            Table.addControl( 0, 1, new CswLiteralNbsp() );
            Table.addControl( 0, 1, new CswLiteralNbsp() );
            Table.addControl( 0, 2, _ClearButton );
            Table.addControl( 0, 3, _UploadButton );
            Table.addControl( 0, 4, _hiddenClear );

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            _UploadButton.Visible = false;
            _ClearButton.Visible = false;
            if (Prop != null && Prop.AsBlob != null)
            {
                if (Prop.AsBlob.JctNodePropId != Int32.MinValue)
                {
                    _FileLink.Text = Prop.AsBlob.FileName;
                    _FileLink.Target = "_blank";
                    _FileLink.NavigateUrl = "GetBlob.Aspx?mode=doc&jctnodepropid=" + Prop.AsBlob.JctNodePropId + "&nodeid=" + Prop.NodeId.ToString() + "&propid=" + PropId.ToString();
                    _UploadButton.OnClientClick = "CswFile_editDocument('" + _hiddenClear.ClientID + "', '" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                    _ClearButton.OnClientClick = "return CswFile_clearDocument('" + _hiddenClear.ClientID + "', '" + _FileLink.ClientID + "');";
                    _UploadButton.Visible = true;
                    _ClearButton.Visible = true;
                }
                else if( Prop.NodeId != null )
                {
                    _UploadButton.OnClientClick = "CswFile_editDocument('" + _hiddenClear.ClientID + "', '" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                    _UploadButton.Visible = true;
                }

            } // if (_PropAsBlob != null)

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
