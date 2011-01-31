using System;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswBarcode : CswFieldTypeWebControl
    {
        public CswBarcode( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswBarcode_DataBinding);
        }

        private void CswBarcode_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _BarcodeLabel.Text = Prop.AsBarcode.Barcode;
                _ValueTextBox.Text = Prop.AsBarcode.Barcode;
            }
        }

        // Case 20783
        private bool _Succeeded = false;
        public override void Save()
        {
            if( _EditMode == NodeEditMode.DefaultValue )
                _Succeeded = Prop.AsBarcode.setBarcodeValueOverride( _ValueTextBox.Text, false );
            else if( !ReadOnly && _ValueTextBox.Text != CswNbtNodePropBarcode.AutoSignal && _ValueTextBox.Text != string.Empty )
                _Succeeded = Prop.AsBarcode.setBarcodeValueOverride( _ValueTextBox.Text, true );
            else
                _Succeeded = Prop.AsBarcode.setBarcodeValue();  // this will not overwrite
        }
        public override void AfterSave()
        {
            this.DataBind();
        }
        public override void Clear()
        {
            _BarcodeLabel.Text = string.Empty;
            _ValueTextBox.Text = string.Empty;
        }

        
        private Label _BarcodeLabel;
        private CswImageButton _PrintButton;
        private TextBox _ValueTextBox;
        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _BarcodeLabel = new Label();
            _BarcodeLabel.ID = "label";
            _BarcodeLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            Table.addControl( 0, 0, _BarcodeLabel );

            _ValueTextBox = new TextBox();
            _ValueTextBox.ID = "ValueTextBox";
            _ValueTextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            Table.addControl( 0, 0, _ValueTextBox );

            Table.addControl( 0, 1, new CswLiteralNbsp() );
            Table.addControl( 0, 1, new CswLiteralNbsp() );
            Table.addControl( 0, 1, new CswLiteralNbsp() );

            _PrintButton = new CswImageButton(CswImageButton.ButtonType.Print);
            _PrintButton.ID = "print";
            Table.addControl( 0, 2, _PrintButton );
        }
        protected override void OnPreRender( EventArgs e )
        {
            _BarcodeLabel.Text += "&nbsp;";

            if( ReadOnly )
            {
                _BarcodeLabel.Visible = true;
                _ValueTextBox.Visible = false;
            }
            else
            {
                _BarcodeLabel.Visible = false;
                _ValueTextBox.Visible = true;
                _ValueTextBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange();" );
                _ValueTextBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
                if( _ValueTextBox.Text == string.Empty )
                    _ValueTextBox.Text = CswNbtNodePropBarcode.AutoSignal;
            }

            if( Prop != null && Prop.NodeId != null )
                _PrintButton.OnClientClick = "openPrintLabelPopup('" + Prop.NodeId.ToString() + "','" + Prop.NodeTypePropId + "');";

            if( _EditMode == NodeEditMode.PrintReport )
                _PrintButton.Visible = false;

            base.OnPreRender( e );
        }
    }
}
