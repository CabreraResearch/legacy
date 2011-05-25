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
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Security;
using ChemSW.NbtWebControls;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class PrintableLayout : System.Web.UI.MasterPage
    {
        protected override void OnLoad( EventArgs e )
        {
            Master.TimeReportDiv.Visible = false;
            base.OnLoad( e );
        }
        
        public CswNbtResources CswNbtResources
        {
            get { return Master.CswNbtResources; }
        }

        public CswNbtView CswNbtView
        {
            get { return Master.CswNbtView; }
        }
        public string LogoutPath
        {
            get { return Master.LogoutPath; }
            set { Master.LogoutPath = value; }
        }
        public RadAjaxManager AjaxManager
        {
            get { return Master.AjaxManager; }
        }

        //public CswAuthenticator CswAuthenticator
        //{
        //    get { return Master.CswAuthenticator; }
        //}

		public void setViewId( CswNbtViewId ViewId )
		{
			Master.setViewId( ViewId );
		}
		public void setViewId( CswNbtViewId ViewId, bool ForceReload )
        {
            Master.setViewId(ViewId, ForceReload);
        }
		public void setSessionViewId( CswNbtSessionDataId SessionViewId, bool ForceReload )
        {
            Master.setSessionViewId( SessionViewId, ForceReload );
        }//setSessionViewId()

		public void setSessionViewId( CswNbtSessionDataId SessionViewId )
        {
            Master.setSessionViewId( SessionViewId );
        }//setSessionViewId()
        public void setViewXml(string ViewXml)
        {
            Master.setViewXml(ViewXml);
        }
        public void setViewXml(string ViewXml, bool ForceReload)
        {
            Master.setViewXml(ViewXml, ForceReload);
        }

        public void ReleaseAll()
        {
            Master.ReleaseAll();
        }

        public void Redirect(string url)
        {
            Master.Redirect(url);
        }

        public void GoHome()
        {
            Master.GoHome();
        }

        public void LogMessage(string Message)
        {
            Master.LogMessage(Message);
        }
        public void LogTimerResult(string Message, string TimerResult)
        {
            Master.LogTimerResult(Message, TimerResult);
        }

        public void HandleError(Exception ex)
        {
            Master.HandleError(ex);
        }
    }
}