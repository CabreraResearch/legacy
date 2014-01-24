using System;
using System.Web.UI.WebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            if( Request.Url.AbsoluteUri.Contains( "dev" ) )
            {
                html.Attributes.Add( "manifest", "release/Dev.appcache" );
            }
            else
            {
                html.Attributes.Add( "manifest", "release/Main.appcache" );
            }


            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string accessid = Request.Form["accessid"];

            string logoutpath = string.Empty;
            if( false == String.IsNullOrEmpty( Request.Form["logoutpath"] ) )
            {
                logoutpath = Request.Form["logoutpath"];
            }
            else
            {
                logoutpath = Request.Path;
            }

            //pass any present login credentials into the client
            Literal JSLiteral = new Literal();

            JSLiteral.Text = @"
                var username = '" + username + @"';
                var password = '" + password + @"';
                var accessid = '" + accessid + @"';
                var logoutpath = '" + logoutpath + @"';
            ";

            JSPlaceHolder.Controls.Add( JSLiteral );

        } // Page_Load()
    }
}