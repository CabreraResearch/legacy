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
    public class CswMemo : CswFieldTypeWebControl, INamingContainer
    {
        public CswMemo( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswMemo_DataBinding);
        }

        private void CswMemo_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                Text = Prop.AsMemo.Text;
                _TextBox.Rows = Prop.AsMemo.Rows;
                _TextBox.Columns = Prop.AsMemo.Columns;
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
                Prop.AsMemo.Text = Text;
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            Text = string.Empty;
        }

        public string Text
        {
            get
            {
                EnsureChildControls();
                return _TextBox.Text;
            }
            set
            {
                EnsureChildControls();
                _TextBox.Text = value;
            }
        }

        private TextBox _TextBox;
        //private Sample.Web.UI.Compatibility.CustomValidator _Validator;

        protected override void CreateChildControls()
        {
            _TextBox = new TextBox();
            _TextBox.ID = "text";
            _TextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            this.Controls.Add(_TextBox);

            //_Validator = new Sample.Web.UI.Compatibility.CustomValidator();
            //_Validator.ID = "vld";
            //_Validator.ValidateEmptyText = true;
            //_Validator.ControlToValidate = _TextBox.ID;
            //_Validator.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            //this.Controls.Add(_Validator);


            _TextBox.TextMode = TextBoxMode.MultiLine;
            _TextBox.CssClass = "textinput";
            base.CreateChildControls();

            if( Required && _EditMode != NodeEditMode.LowRes)
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _TextBox.ID;
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( _EditMode != NodeEditMode.LowRes )
            {
                _TextBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange();" );
                _TextBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
            }
            base.OnPreRender( e );
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (ReadOnly)
            {
                writer.Write(this.Text);
            }
            else
            {
                base.RenderControl(writer);
            }
        }

    }
}
