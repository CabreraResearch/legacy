using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using NbtWebApp.Services;

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
            RouteTable.Routes.Add( new ServiceRoute( "Services/Labels", Factory, typeof( Labels ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Labels2", Factory, typeof( Labels2 ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Containers", Factory, typeof( Containers ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/ChemCatCentral", Factory, typeof( ChemCatCentral ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/LandingPages", Factory, typeof( LandingPages ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Materials", Factory, typeof( Materials ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Menus", Factory, typeof( Menus ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Mol", Factory, typeof( Mol ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Nodes", Factory, typeof( Nodes ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/RegulatoryReporting", Factory, typeof( RegulatoryReporting ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Reports", Factory, typeof( Reports ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Requests", Factory, typeof( Requests ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Session", Factory, typeof( Session ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Trees", Factory, typeof( Trees ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/Views", Factory, typeof( Views ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/CISProNbtMobile", Factory, typeof( CISProNbtMobile ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/LegacyMobile", Factory, typeof( LegacyMobile ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Services/KioskMode", Factory, typeof( KioskMode ) ) );
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
                HttpContext.Current.Response.AddHeader( "Access-Control-Allow-Methods", "GET, POST, DELETE, PUT" );
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