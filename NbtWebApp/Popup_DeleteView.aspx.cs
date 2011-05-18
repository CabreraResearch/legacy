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

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_DeleteView : System.Web.UI.Page
    {
        //private CswViewEditorManager _CswViewEditorManager = null;
        private Int32 ViewId;
        private CswNbtView View;

        protected override void OnInit(EventArgs e)
        {
            if (Request.QueryString["viewid"] != null)
            {
                ViewId = Convert.ToInt32(Request.QueryString["viewid"].ToString());

                //null will occur after we've deleted
				if( null != ( View = Master.CswNbtResources.ViewSelect.restoreView( ViewId ) ) )
                {
                    //ViewNameHolder.Text = View.ViewName;
                    DeleteViewNameLiteral.Text = View.ViewName;
                }//if there is a current view
            }

            base.OnInit(e);
        }
    } // Popup_DeleteView
} // namespace ChemSW.Nbt.WebPages
