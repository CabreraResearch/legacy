using System;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;

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

