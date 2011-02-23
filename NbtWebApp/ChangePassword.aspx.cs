using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;

using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Encryption;

namespace ChemSW.Nbt.WebPages
{
    public partial class ChangePassword : System.Web.UI.Page
    {

        private CswAutoTable Table;
        private Label ExpiredLabel;
        private Literal NewPasswordLiteral;
        private CswPassword PasswordControl;
        private Button SaveButton;
        private CswNbtNode UserNode;
        private CswNbtObjClassUser UserNodeAsUser;
        protected override void OnInit( EventArgs e )
        {
            Master.HideContent = true;
            
            HtmlGenericControl Div = new HtmlGenericControl( "div" );
            Div.Attributes.Add( "class", "changepassworddiv" );
            ph.Controls.Add( Div );

            Table = new CswAutoTable();
            Table.CssClass = "changepasswordtable";
            Div.Controls.Add( Table );

            ExpiredLabel = new Label();
            ExpiredLabel.CssClass = "ErrorContent";
            TableCell ExpiredCell = Table.getCell( 0, 0 );
            ExpiredCell.ColumnSpan = 2;
            ExpiredCell.HorizontalAlign = HorizontalAlign.Center;
            ExpiredCell.Controls.Add( ExpiredLabel );

            Table.addControl( 1, 0, new CswLiteralNbsp() );

            NewPasswordLiteral = new Literal();
            NewPasswordLiteral.Text = "New Password:&nbsp;";
            Table.addControl( 2, 0, NewPasswordLiteral );

            TableCell PasswordControlCell = Table.getCell( 2, 1 );
            UserNode = Master.CswNbtResources.Nodes.GetNode( Master.CswNbtResources.CurrentUser.UserId );
            UserNodeAsUser = CswNbtNodeCaster.AsUser( UserNode );
            PasswordControl = (CswPassword) CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, 
                                                                                       PasswordControlCell.Controls, 
                                                                                       string.Empty, 
                                                                                       UserNode.Properties[CswNbtObjClassUser.PasswordPropertyName],
                                                                                       NodeEditMode.Edit, 
                                                                                       new CswErrorHandler( Master.HandleError ) );
            // Guarantee the user can edit his own password
            PasswordControl.ReadOnly = false;
            
            SaveButton = new Button();
            SaveButton.ID = "SaveButton";
            SaveButton.Text = "Save";
            SaveButton.UseSubmitBehavior = true;
            SaveButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            SaveButton.Click += new EventHandler( SaveButton_Click );
            SaveButton.OnClientClick = "return CswPropertyTable_SaveButton_PreClick();";
            Table.addControl( 2, 1, SaveButton );

            base.OnInit( e );
        }
        protected override void OnLoad( EventArgs e )
        {
            ExpiredLabel.Text = "Your password has expired.";

            base.OnLoad( e );
        }

        void SaveButton_Click( object sender, EventArgs e )
        {
            CswEncryption CswEncryption = new CswEncryption( Master.CswNbtResources.MD5Seed );
            string EncryptedNewPassword = CswEncryption.getMd5Hash( PasswordControl.Text );
            if( EncryptedNewPassword == UserNodeAsUser.PasswordProperty.EncryptedPassword )
            {
                ExpiredLabel.Text += "<BR/>You must choose a new password";
            }
            else
            {
                PasswordControl.Save();
                UserNode.postChanges( true );
                // Master.GoMain();
                Master.GoHome();
            }
        }
    }
}