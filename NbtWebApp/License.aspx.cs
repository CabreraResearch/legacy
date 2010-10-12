using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Security;

namespace ChemSW.Nbt.WebPages
{
    public partial class License : System.Web.UI.Page
    {
        private CswLicenseManager _LicenseManager = null;
        protected override void OnInit( EventArgs e )
        {
            _LicenseManager = new CswLicenseManager( Master.CswNbtResources );
            LicenseMemo.Text = _LicenseManager.LatestLicenseText;

            AcceptButton.Click += new EventHandler( AcceptButton_Click );
            DeclineButton.Click += new EventHandler( DeclineButton_Click );

            base.OnInit( e );
        }

        public string QueryStringRedirect
        {
            get
            {
                string ret = string.Empty;
                if( Request.QueryString["destination"] != null && Request.QueryString["destination"].ToString() != string.Empty )
                    ret = CswTools.UrlToQueryStringParam( Request.QueryString["destination"].ToString() );
                return ret;
            }
        }

        void DeclineButton_Click( object sender, EventArgs e )
        {
            Master.Logout();
        }

        void AcceptButton_Click( object sender, EventArgs e )
        {
            _LicenseManager.RecordLicenseAcceptance( Master.CswNbtResources.CurrentUser );

            if( string.Empty == QueryStringRedirect )
            {
                //Master.Redirect( "Main.aspx" );
                Master.GoHome();
            }
            else
            {
                Master.Redirect( QueryStringRedirect );
            }
        }
    }
}