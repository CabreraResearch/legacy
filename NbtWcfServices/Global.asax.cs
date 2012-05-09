using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using NbtWebAppServices.WebServices;

namespace NbtWebAppServices
{
    public class Global : HttpApplication
    {
        protected void Application_Start( object sender, EventArgs e )
        {
            WebServiceHostFactory Factory = new WebServiceHostFactory();
            RouteTable.Routes.Add( new ServiceRoute( "Session", Factory, typeof( CswNbtWsSession ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Locations", Factory, typeof( CswNbtWsLocations ) ) );
            RouteTable.Routes.Add( new ServiceRoute( "Inspections", Factory, typeof( CswNbtWebServiceInspectionsUriMethods ) ) );
        }

        protected void Application_BeginRequest( object sender, EventArgs e )
        {
            HttpContext.Current.Response.Cache.SetCacheability( HttpCacheability.NoCache );
            HttpContext.Current.Response.Cache.SetNoStore();

            EnableCrossDmainAjaxCall();
        }

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
    }
}