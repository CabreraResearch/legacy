using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    [ToolboxData("<{0}:CswSequence runat=server></{0}:CswSequence>")]
    public class CswSequence : CswFieldTypeWebControl
    {
        public CswSequence( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswSequence_DataBinding);
        }

        private Label _Label;
        private TextBox _ValueTextBox;
        protected override void CreateChildControls()
        {
            _Label = new Label();
            _Label.ID = "label";
            _Label.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            this.Controls.Add( _Label );

            _ValueTextBox = new TextBox();
            _ValueTextBox.ID = "ValueTextBox";
            _ValueTextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            this.Controls.Add( _ValueTextBox );

            base.CreateChildControls();
        }

        private void CswSequence_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _Label.Text = Prop.AsSequence.Sequence;
                _ValueTextBox.Text = Prop.AsSequence.Sequence;
            }
        }

        public override void Save()
        {
            if( _EditMode == NodeEditMode.DefaultValue )
                Prop.AsSequence.setSequenceValueOverride( _ValueTextBox.Text, false );
            else if( !ReadOnly && _ValueTextBox.Text != CswNbtNodePropBarcode.AutoSignal && _ValueTextBox.Text != string.Empty )
                Prop.AsSequence.setSequenceValueOverride( _ValueTextBox.Text, false );
            else
                Prop.AsSequence.setSequenceValue();  // this will not overwrite
        }
        public override void AfterSave()
        {
            this.DataBind();
        }
        public override void Clear()
        {
            _Label.Text = string.Empty;
            _ValueTextBox.Text = string.Empty;
        }

        protected override void OnPreRender( EventArgs e )
        {
            _Label.Text += "&nbsp;";

            if( ReadOnly )
            {
                _Label.Visible = true;
                _ValueTextBox.Visible = false;
            }
            else
            {
                _Label.Visible = false;
                _ValueTextBox.Visible = true;
                _ValueTextBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange();" );
                _ValueTextBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
                if( _ValueTextBox.Text == string.Empty )
                    _ValueTextBox.Text = CswNbtNodePropBarcode.AutoSignal;
            }
            base.OnPreRender( e );
        }
    }
}
