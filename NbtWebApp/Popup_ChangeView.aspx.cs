using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_ChangeView : System.Web.UI.Page
    {
        private CswViewListTree ViewTree;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                //Master.CswNbtResources.ViewSelect.clearCache();  

                CswAutoTable ViewTreeTable = new CswAutoTable();
                ViewTreeTable.ID = "ViewTreeTable";
                ph.Controls.Add( ViewTreeTable );

                Literal ViewTreeLabel = new Literal();
                ViewTreeLabel.Text = "Choose a View:";
                ViewTreeTable.addControl( 0, 0, ViewTreeLabel );

                ViewTree = new CswViewListTree( Master.CswNbtResources, false );
                ViewTree.ID = "viewtree";
                ViewTree.IncludeActions = true;
                ViewTree.IncludeReports = true;
                ViewTree.ViewSelected += new CswViewListTree.ViewSelectedEventHandler( _ViewTree_ViewSelected );
                ViewTree.OnError += new CswErrorHandler( Master.HandleError );
                if( Master.CswNbtView != null )
                    ViewTree.ViewIdToSelect = Master.CswNbtView.ViewId;
                else if( Master.ActionId > 0 )
                    ViewTree.ActionIdToSelect = Master.ActionId;
                ViewTreeTable.addControl( 1, 0, ViewTree );

                ViewTree.PreviousView1 = Master.PreviousView1;
                ViewTree.PreviousView2 = Master.PreviousView2;
                ViewTree.PreviousView3 = Master.PreviousView3;
                ViewTree.PreviousView4 = Master.PreviousView4;
                ViewTree.PreviousView5 = Master.PreviousView5;
                ViewTree.DataBind();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnInit( e );
        } // OnInit()


        //public event CswViewListTree.ViewSelectedEventHandler ViewSelected = null;

        protected void _ViewTree_ViewSelected( object sender, RadTreeNodeEventArgs e )
        {
            try
            {
                string JS = @"  <script>
                                    Popup_OK_Clicked('" + e.Node.Value + @"');
                                </script>";
                System.Web.UI.ScriptManager.RegisterStartupScript( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // _ViewTree_ViewSelected()
    }
}