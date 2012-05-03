using System;
using System.Web;

namespace ChemSW.Nbt
{
    public class NbtApplication : HttpApplication
    {
        //public static void RegisterRoutes( RouteCollection routes )
        //{
        //    routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

        //    routes.MapRoute(
        //        "Default", // Route name
        //        "{controller}/{action}/{id}", // URL with parameters
        //        new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
        //    );
        //}

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //RegisterRoutes( RouteTable.Routes );
        }

        protected void Application_BeginRequest( object sender, EventArgs e )
        {
            HttpContext.Current.Response.Cache.SetCacheability( HttpCacheability.NoCache );
            HttpContext.Current.Response.Cache.SetNoStore();
        }

        protected void Application_End( object sender, EventArgs e )
        {
        }

        protected void Application_Error( object sender, EventArgs e )
        {
            // Code that runs when an unhandled error occurs
        }

        protected void Session_Start( object sender, EventArgs e )
        {
            // Code that runs when a new session is started
        }
    }

}
