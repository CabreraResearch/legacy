using System;
using System.IO;
using System.Web;


namespace NbtWebApp
{

    /*
     * Without this module we are unable to access the Files collection from request - it throws an error otherwise
     * 
     * FROM: http://blogs.msdn.com/b/praburaj/archive/2012/09/13/accessing-httpcontext-current-request-inputstream-property-in-aspnetcompatibility-mode-throws-exception-this-method-or-property-is-not-supported-after-httprequest-getbufferlessinputstream-has-been-invoked.aspx
     */

    public class WcfReadEntityBodyModeWorkaroundModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init( HttpApplication context )
        {
            context.BeginRequest += context_BeginRequest;
        }

        private void context_BeginRequest( object sender, EventArgs e )
        {
            //This will force the HttpContext.Request.ReadEntityBody to be "Classic" and will ensure compatibility..
            Stream stream = ( sender as HttpApplication ).Request.InputStream;
        }
    }
}