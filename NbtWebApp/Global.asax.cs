using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace NbtWebApp
{
    /// <summary>
    /// Global ASAX
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// On Application Start
        /// </summary>
        protected void Application_Start( object sender, EventArgs e )
        {
            WebServiceHostFactory Factory = new WebServiceHostFactory();
            RouteTable.Routes.Add( new ServiceRoute( "Labels", Factory, typeof( Labels ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Views", Factory, typeof( Views ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Session", Factory, typeof( Session ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Reports", Factory, typeof( Reports ) ) );
        }

        /// <summary>
        /// On Application Begin Request
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
        /// On Application End
        /// </summary>
        protected void Application_End( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// On Application Error
        /// </summary>
        protected void Application_Error( object sender, EventArgs e )
        {
            // Code that runs when an unhandled error occurs
        }

        /// <summary>
        /// On Session Start
        /// </summary>
        protected void Session_Start( object sender, EventArgs e )
        {
            // Code that runs when a new session is started
        }
    }
}