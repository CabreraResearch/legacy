using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.Config;
using ChemSW.Core;
using ChemSW.NbtWebControls;
using ChemSW.Security;
using ChemSW.Nbt;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowRes_Login : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            try
            {
                // Handle Customer ID box


                Login1.CswResources = Master.CswNbtResources;

                string AccessId = string.Empty;
                if( Request.Form["accessid"] != null && Request.Form["accessid"] != string.Empty )
                    AccessId = Request.Form["accessid"];
                else if( Request.QueryString["accessid"] != null && Request.QueryString["accessid"] != string.Empty )
                    AccessId = Request.QueryString["accessid"];
                if( AccessId != string.Empty && Master.CswNbtResources.doesAccessIdExist( AccessId ) )
                    Master.AccessId = AccessId;
                
                if( Request.Form["logoutpath"] != null && Request.Form["logoutpath"] != string.Empty )
                    Master.LogoutPath = Request.Form["logoutpath"];
                else if( Request.QueryString["logoutpath"] != null && Request.Form["logoutpath"] != string.Empty )
                    Master.LogoutPath = Request.QueryString["logoutpath"];

                Login1.OnError += new CswErrorHandler( Master.HandleError );
                Login1.Authenticate += new CswLogin.CswNbtLoginHandler( Login1_Authenticate );
                Login1.OnPostAccessId += new CswLogin.CswNbtOnLoadHandler( _SetAccessId );

                EnsureChildControls();

                Master.LogoutButton.Visible = false;

                base.OnInit( e );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }//OnInit()

        protected override void OnLoad( EventArgs e )
        {
            // Support logins from other forms
            if ( Request.Form[ "accessid" ] != null &&
                 Request.Form[ "username" ] != null &&
                 Request.Form[ "password" ] != null )
            {
                AuthenticationStatus AuthenticationStatus = _Authenticate( Request.Form[ "accessid" ].ToString(), Request.Form[ "username" ].ToString(), Request.Form[ "password" ].ToString() );
                if ( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    _Proceed();
                }
                else
                {
                    string LogoutPath = Master.LogoutPath;
                    if ( LogoutPath.IndexOf( "?" ) > 0 )
                        LogoutPath += "&error=" + CswAuthenticator.getAuthenticationStatusMessage( AuthenticationStatus );
                    else
                        LogoutPath += "?error=" + CswAuthenticator.getAuthenticationStatusMessage( AuthenticationStatus );
                    Master.Redirect( LogoutPath );
                }
            }
            base.OnLoad( e );
        }


        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
        }

        private void _SetAccessId( EventArgs e )
        {
            if ( string.Empty != Login1.AccessId )
            {
                Master.AccessId = Login1.AccessId;
            }
        }//_SetAccessId()

        protected void Login1_Authenticate( object sender, AuthenticateEventArgs AuthenticateEventArgs )
        {
            try
            {
                AuthenticationStatus AuthenticationStatus = _Authenticate( Login1.AccessId, Login1.UserName, Login1.Password );
                if ( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    AuthenticateEventArgs.Authenticated = true;
                    _Proceed(); 
                }
            }

            catch ( Exception ex )
            {
                Master.HandleError( ex );
            }

        }//Login1_Authenticate()


        private AuthenticationStatus _Authenticate( string AccessId, string username, string password )
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            if( Master.CswNbtResources.doesAccessIdExist( AccessId ) )
            {
                Master.AccessId = AccessId;
                AuthenticationStatus = Master.Authenticate( username, password );
            }
            else
            { 
                // BZ 8217
                AuthenticationStatus = AuthenticationStatus.NonExistentAccessId;
            }
            Login1.Message = CswAuthenticator.getAuthenticationStatusMessage( AuthenticationStatus );
            return AuthenticationStatus;
        }

        private void _Proceed()
        {
            Master.Redirect( "LowRes_SelectView.aspx" );
        }
    }//_Login

}//namespace ChemSW.Nbt.WebPages
