using System;
using System.Web.UI.WebControls;
using ChemSW.Core;

namespace ChemSW.Nbt.WebPages
{
    public partial class ExternalLogin : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string accessid = Request.Form["accessid"];

            string logoutpath = string.Empty;
            if( false == String.IsNullOrEmpty( Request.Form["logoutpath"] ) )
            {
                logoutpath = Request.Form["logoutpath"];
            }
            else if( null != Request.UrlReferrer )
            {
                logoutpath = Request.UrlReferrer.ToString();
            }
            else
            {
                logoutpath = Request.Path;
            }

            if( false == String.IsNullOrEmpty( username ) &&
                false == String.IsNullOrEmpty( password ) &&
                false == String.IsNullOrEmpty( accessid ) )
            {
                Literal JSLiteral = new Literal();
                JSLiteral.Text = @"
                            $(document).ready(function () {
                                Csw.clientSession.login({
                                    AccessId: '" + CswTools.SafeJavascriptParam( accessid ) + @"', 
                                    UserName: '" + CswTools.SafeJavascriptParam( username ) + @"', 
                                    Password: '" + CswTools.SafeJavascriptParam( password ) + @"',
                                    ForMobile: false,
                                    onAuthenticate: function() { window.location = 'Main.html'; },
                                    onFail: function(error) { window.location = '" + CswTools.SafeJavascriptParam( logoutpath ) + @"?error=' + error; },
                                    logoutpath: '" + CswTools.SafeJavascriptParam( logoutpath ) + @"'
                                });
                            });";
                JSPlaceHolder.Controls.Add( JSLiteral );
            }
            else
            {
                Response.Redirect( "Main.html" );
            }
        } // Page_Load()
    }
}