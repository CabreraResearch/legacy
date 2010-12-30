using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;
using ChemSW.Core;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_EditProp : System.Web.UI.Page
    {
        //private CswFieldTypeWebControlFactory _CswFieldTypeWebControlFactory;
        private CswNbtNode _Node = null;
        //private CswNbtNodeKey _NodeKey = null;
        //private CswNbtNodeKey NodeKey
        //{
        //    get { return _NodeKey; }
        //    set { _NodeKey = value; }
        //}

        private Int32 PropId = Int32.MinValue;
        
        private CswNbtMetaDataNodeTypeProp MetaDataNodeTypeProp
        {
            get { return Master.CswNbtResources.MetaData.getNodeTypeProp( PropId ); }
        }
        //private Button _SaveButton;

        private CswFieldTypeWebControl PropControl;
        private Button SaveButton;
        private Button CancelButton;

        public string CheckedNodeIds = string.Empty;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                this.EnableViewState = false;

                if( Request.QueryString["nodepk"] != null && Request.QueryString["propid"] != null )
                {
                    CswPrimaryKey NodePk = new CswPrimaryKey();
                    NodePk.FromString( Request.QueryString["nodepk"].ToString() );
                    if( NodePk != null )
                    {
                        // Edit property value for node
                        _Node = Master.CswNbtResources.Nodes.GetNode( NodePk );
                        PropId = CswConvert.ToInt32( Request.QueryString["propid"].ToString() );
                        CswNbtMetaDataNodeTypeProp MetaDataProp = Master.CswNbtResources.MetaData.getNodeTypeProp( PropId );
                        PropControl = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, PropPlaceHolder.Controls, string.Empty, MetaDataProp, _Node, NodeEditMode.EditInPopup, new CswErrorHandler( Master.HandleError ) );
                    }
                    else
                    {
                        // Edit default value for property
                        PropId = CswConvert.ToInt32( Request.QueryString["propid"].ToString() );
                        CswNbtMetaDataNodeTypeProp MetaDataProp = Master.CswNbtResources.MetaData.getNodeTypeProp( PropId );
                        PropControl = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, PropPlaceHolder.Controls, string.Empty, MetaDataProp.DefaultValue, NodeEditMode.EditInPopup, new CswErrorHandler( Master.HandleError ) );
                    }
                }
                ( (System.Web.UI.WebControls.WebControl) PropControl ).DataBind();

                CheckedNodeIds = Request.QueryString["checkednodeids"].ToString();

                PropPlaceHolder.Controls.Add( new CswLiteralBr() );

                SaveButton = new Button();
                SaveButton.ID = "SaveButton";
                SaveButton.CssClass = "Button";
                SaveButton.Text = "Save";
                SaveButton.Click += new EventHandler( SaveButton_Click );
                SaveButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                PropPlaceHolder.Controls.Add( SaveButton );

                CancelButton = new Button();
                CancelButton.ID = "CancelButton";
                CancelButton.CssClass = "Button";
                CancelButton.Text = "Cancel";
                CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";
                PropPlaceHolder.Controls.Add( CancelButton );

                base.OnInit( e );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if( _Node != null )
                {
                    if( Page.IsValid && PropPlaceHolder.HasControls() )
                    {
                        SaveFieldTypeWebControls( PropPlaceHolder.Controls );

                        //Master.CswNbtResources.Nodes.Write(Master.CswNbtResources.Nodes[NodeKey]);
                        _Node.postChanges( true );

                        if( CheckedNodeIds != string.Empty )
                        {
                            foreach( string OtherNodeIdString in CheckedNodeIds.Split( ',' ) )
                            {
                                //CswPrimaryKey OtherNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OtherNodeIdString ) );
                                CswPrimaryKey OtherNodeId = new CswPrimaryKey();
                                OtherNodeId.FromString( OtherNodeIdString );
                                if( OtherNodeId != _Node.NodeId )
                                {
                                    CswNbtNode OtherNode = Master.CswNbtResources.Nodes[OtherNodeId];
                                    OtherNode.Properties[MetaDataNodeTypeProp].copy( _Node.Properties[MetaDataNodeTypeProp] );
                                    OtherNode.postChanges( true );
                                }
                            }
                        }

                        AfterSaveFieldTypeWebControls( PropPlaceHolder.Controls );
                    }
                }
                else
                {
                    if( Page.IsValid && PropPlaceHolder.HasControls() )
                    {
                        SaveFieldTypeWebControls( PropPlaceHolder.Controls );
                        AfterSaveFieldTypeWebControls( PropPlaceHolder.Controls );
                    }
                }

                // Commit any transactions
                Master.CswNbtResources.finalize();

                string JS = @"  <script>
                                    Popup_OK_Clicked();
                                </script>";

                System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), this.UniqueID + "_JS", JS, false);
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
        }//SaveButton_Click()

        private void SaveFieldTypeWebControls(ControlCollection Controls)
        {
            foreach (System.Web.UI.Control thisControl in Controls)
            {
                if (thisControl is CswFieldTypeWebControl)
                {
                    if (!((CswFieldTypeWebControl)thisControl).ReadOnly)
                        ((CswFieldTypeWebControl)thisControl).Save();
                }

                if (thisControl.Controls.Count > 0)
                    SaveFieldTypeWebControls(thisControl.Controls);
            }
        }
        private void AfterSaveFieldTypeWebControls(ControlCollection Controls)
        {
            foreach (System.Web.UI.Control thisControl in Controls)
            {
                if (thisControl is CswFieldTypeWebControl)
                {
                    // We need to update readonly controls here too!  BZ 6122
                    //if (!((CswFieldTypeWebControl)thisControl).ReadOnly)
                    ((CswFieldTypeWebControl)thisControl).AfterSave();
                }

                if (thisControl.Controls.Count > 0)
                    AfterSaveFieldTypeWebControls(thisControl.Controls);
            }
        }

    }
}
