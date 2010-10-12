using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswPassword : CswFieldTypeWebControl, INamingContainer
    {
        public CswPassword( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswPassword_DataBinding);
        }
        
        private void CswPassword_DataBinding(object sender, EventArgs e)
        {
            EnsureChildControls();
            if( Prop != null )
            {
                if( Prop.AsPassword.Empty )
                {
                    _PasswordLabel.Text = "Set";
                    if( Required )
                    {
                        _RequiredValidator.Visible = true;
                        _RequiredValidator.Enabled = true;
                        _RequiredValidator.ControlToValidate = _Password.ID;
                    }
                }
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                if( Text != "" )
                    Prop.AsPassword.Password = Text;
            }
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
                return _Password.Value;
            }
            set
            {
                _Password.Value = value;
            }
        }


        private HtmlInputPassword _Password;
        private HtmlInputPassword _Confirm;
        private CswInvalidImage _InvalidImg;
        private Label _PasswordLabel;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _PasswordLabel = new Label();
            _PasswordLabel.Text = "Change";

            _Password = new HtmlInputPassword();
            _Password.ID = "pw";
            _Password.Attributes.Add( "class", CswFieldTypeWebControl.TextBoxCssClass );

            _InvalidImg = new CswInvalidImage();
            _InvalidImg.ID = "InvalidImg";

            Label ConfirmLabel = new Label();
            ConfirmLabel.Text = "Confirm";

            _Confirm = new HtmlInputPassword();
            _Confirm.ID = "cf";
            _Confirm.Attributes.Add( "class", CswFieldTypeWebControl.TextBoxCssClass );

            // Validation
            //_PasswordValidator = new Sample.Web.UI.Compatibility.CustomValidator();
            //_PasswordValidator.ID = "vld";
            ////_PasswordValidator.Text = "Passwords do not match!";
            //_PasswordValidator.ServerValidate += new ServerValidateEventHandler(_PasswordValidator_ServerValidate);
            //_PasswordValidator.ValidationGroup = CswFieldTypeWebControlPropsImpl.FieldTypeValidationGroup;

            Table.addControl( 0, 0, _PasswordLabel );
            Table.addControl( 0, 1, _Password );
            Table.addControl( 0, 1, _InvalidImg );
            Table.addControl( 1, 0, ConfirmLabel );
            Table.addControl( 1, 1, _Confirm );
            //Table.addControl(0, 1, _PasswordValidator);

            base.CreateChildControls();

            Table.addControl( 0, 1, _RequiredValidator );
        }

        //protected void _PasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        //{
        //    if (_Password.Value != _Confirm.Value)
        //    {
        //        _Password.Attributes.Add("class", CswFieldTypeWebControlPropsImpl.TextBoxCssClassInvalid);
        //        _Confirm.Attributes.Add("class", CswFieldTypeWebControlPropsImpl.TextBoxCssClassInvalid);
        //        args.IsValid = false;
        //    }
        //    else
        //    {
        //        args.IsValid = true;
        //    }
        //}

        protected override void OnPreRender(EventArgs e)
        {
            //_PasswordValidator.ClientValidationFunction = this.ClientID + "_validatepassword";
            //initValidationJS();
            //initCheckChangesJS();

            string Length = _CswNbtResources.getConfigVariableValue("password_length");
            string Complexity = _CswNbtResources.getConfigVariableValue("password_complexity");
            _Password.Attributes.Add( "onkeypress", "CswPassword_onchange('" + _Password.ClientID + "','" + _Confirm.ClientID + "','" + _InvalidImg.ClientID + "'," + Length + "," + Complexity + ");" );
            //User must type password-no copy/paste
            //_Password.Attributes.Add( "onchange", "CswPassword_onchange('" + _Password.ClientID + "','" + _Confirm.ClientID + "','" + _InvalidImg.ClientID + "'," + Length + "," + Complexity + ");" );
            _Confirm.Attributes.Add( "onchange", "CswPassword_onchange('" + _Password.ClientID + "','" + _Confirm.ClientID + "','" + _InvalidImg.ClientID + "'," + Length + "," + Complexity + ");" );
            //_Confirm.Attributes.Add( "onkeypress", "CswPassword_onchange('" + _Password.ClientID + "','" + _Confirm.ClientID + "','" + _InvalidImg.ClientID + "'," + Length + "," + Complexity + ");" );
            base.OnPreRender(e);
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
