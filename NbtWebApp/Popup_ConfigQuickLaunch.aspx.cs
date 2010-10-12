using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_ConfigQuickLaunch : System.Web.UI.Page
    {
        private CswFieldTypeWebControl QuickLaunchViewsControl;
        private CswFieldTypeWebControl QuickLaunchActionsControl;
        private Button SaveButton;
        private Button CancelButton;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                this.EnableViewState = false;

                CswNbtObjClassUser UserNode = Master.CswNbtResources.CurrentNbtUser.UserNode as CswNbtObjClassUser;
                CswNbtMetaDataNodeType UserNodeType = Master.CswNbtResources.MetaData.getNodeType( UserNode.NodeTypeId );

                CswNbtMetaDataNodeTypeProp QuickLaunchViewsProp = UserNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.QuickLaunchViewsPropertyName );
                CswNbtMetaDataNodeTypeProp QuickLaunchActionsProp = UserNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.QuickLaunchActionsPropertyName );

                CswAutoTable Table = new CswAutoTable();
                Table.OddCellRightAlign = true;
                ph.Controls.Add( Table );

                TableCell ExplanationCell = Table.getCell( 0, 0 );
                ExplanationCell.ColumnSpan = 2;

                Literal ExplanationLiteral = new Literal();
                ExplanationLiteral.Text = "These Views and Actions will always appear in the Quick Launch:";
                ExplanationCell.Controls.Add( ExplanationLiteral );

                ExplanationCell.Controls.Add( new CswLiteralBr() );
                ExplanationCell.Controls.Add( new CswLiteralBr() );

                Literal QuickLaunchViewsLiteral = new Literal();
                QuickLaunchViewsLiteral.Text = QuickLaunchViewsProp.PropName;
                Table.addControl( 1, 0, QuickLaunchViewsLiteral );

                TableCell QuickLaunchViewsCell = Table.getCell( 1, 1 );
                QuickLaunchViewsControl = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, QuickLaunchViewsCell.Controls, string.Empty, QuickLaunchViewsProp, UserNode.Node, NodeEditMode.EditInPopup, new CswErrorHandler( Master.HandleError ) );

                Literal QuickLaunchActionsLiteral = new Literal();
                QuickLaunchActionsLiteral.Text = QuickLaunchActionsProp.PropName;
                Table.addControl( 2, 0, QuickLaunchActionsLiteral );

                TableCell QuickLaunchActionsCell = Table.getCell( 2, 1 );
                QuickLaunchActionsControl = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, QuickLaunchActionsCell.Controls, string.Empty, QuickLaunchActionsProp, UserNode.Node, NodeEditMode.EditInPopup, new CswErrorHandler( Master.HandleError ) );

                ( (System.Web.UI.WebControls.WebControl) QuickLaunchViewsControl ).DataBind();
                ( (System.Web.UI.WebControls.WebControl) QuickLaunchActionsControl ).DataBind();

                ph.Controls.Add( new CswLiteralBr() );

                SaveButton = new Button();
                SaveButton.ID = "SaveButton";
                SaveButton.CssClass = "Button";
                SaveButton.Text = "Save";
                SaveButton.Click += new EventHandler( SaveButton_Click );
                SaveButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                ph.Controls.Add( SaveButton );

                CancelButton = new Button();
                CancelButton.ID = "CancelButton";
                CancelButton.CssClass = "Button";
                CancelButton.Text = "Cancel";
                CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";
                ph.Controls.Add( CancelButton );

            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        protected void SaveButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid && ph.HasControls() )
                {
                    SaveFieldTypeWebControls( ph.Controls );
                    Master.CswNbtResources.CurrentNbtUser.postChanges( true );
                    AfterSaveFieldTypeWebControls( this.Controls );
                }

                // Commit any transactions
                Master.CswNbtResources.finalize();

                string JS = @"  <script>
                                    Popup_OK_Clicked();
                                </script>";

                System.Web.UI.ScriptManager.RegisterStartupScript( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//SaveButton_Click()

        private void SaveFieldTypeWebControls( ControlCollection Controls )
        {
            foreach( System.Web.UI.Control thisControl in Controls )
            {
                if( thisControl is CswFieldTypeWebControl )
                {
                    if( !( (CswFieldTypeWebControl) thisControl ).ReadOnly )
                        ( (CswFieldTypeWebControl) thisControl ).Save();
                }

                if( thisControl.Controls.Count > 0 )
                    SaveFieldTypeWebControls( thisControl.Controls );
            }
        }
        private void AfterSaveFieldTypeWebControls( ControlCollection Controls )
        {
            foreach( System.Web.UI.Control thisControl in Controls )
            {
                if( thisControl is CswFieldTypeWebControl )
                {
                    // We need to update readonly controls here too!  BZ 6122
                    //if (!((CswFieldTypeWebControl)thisControl).ReadOnly)
                    ( (CswFieldTypeWebControl) thisControl ).AfterSave();
                }

                if( thisControl.Controls.Count > 0 )
                    AfterSaveFieldTypeWebControls( thisControl.Controls );
            }
        }

    }
}
