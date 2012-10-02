using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using API.Test.Services;

namespace API.Test
{

    /// <summary>
    /// Global API.TEST
    /// </summary>
    public class Global : HttpApplication
    {



        /// <summary>
        /// Application Start
        /// </summary>
        protected void Application_Start( object sender, EventArgs e )
        {
            WebServiceHostFactory Factory = new WebServiceHostFactory();
            RouteTable.Routes.Add( new ServiceRoute( "Properties", Factory, typeof( Properties ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Foo", Factory, typeof( Foo ) ) );
        }

        /// <summary>
        /// Session Start
        /// </summary>
        protected void Session_Start( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Application Begin Request
        /// </summary>
        protected void Application_BeginRequest( object sender, EventArgs e )
        {
            HttpContext.Current.Response.Cache.SetCacheability( HttpCacheability.NoCache );
            HttpContext.Current.Response.Cache.SetNoStore();

            EnableCrossDmainAjaxCall();
        }

        /// <summary>
        /// Enable CDA
        /// </summary>
        private void EnableCrossDmainAjaxCall()
        {
            HttpContext.Current.Response.AddHeader( "Access-Control-Allow-Origin", "*" );
            if( HttpContext.Current.Request.HttpMethod == "OPTIONS" )
            {
                HttpContext.Current.Response.AddHeader( "Access-Control-Allow-Methods", "GET, POST" );
                HttpContext.Current.Response.AddHeader( "Access-Control-Allow-Headers", "Content-Type, Accept" );
                HttpContext.Current.Response.AddHeader( "Access-Control-Max-Age", "1728000" );
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// Application Authenticate Request
        /// </summary>
        protected void Application_AuthenticateRequest( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Application Error
        /// </summary>
        protected void Application_Error( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Session End
        /// </summary>
        protected void Session_End( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Application End
        /// </summary>
        protected void Application_End( object sender, EventArgs e )
        {

        }
    }
}