using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class DisplayLog : System.Web.UI.Page
    {
        private CswLogViewer _CswLogViewer;

        protected override void OnInit(EventArgs e)
        {
            //create the CswLogViewer
            try
            {
                _CswLogViewer = new CswLogViewer(Master.CswNbtResources);
                _CswLogViewer.ID = "logviewer";
                theGrid.Controls.Add(_CswLogViewer);
                _CswLogViewer.OnError += new CswErrorHandler(_CswLogViewer_OnError);
                //_CswLogViewer.DataBind();


            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
            base.OnInit(e);
        }
        protected override void OnLoadComplete(EventArgs e)
        {

            base.OnLoadComplete(e);
        }

        void _CswLogViewer_OnError(Exception ex)
        {
            Master.HandleError(ex);

        }

    } // class DisplayLog
} // namespace ChemSW.Nbt.WebPages

