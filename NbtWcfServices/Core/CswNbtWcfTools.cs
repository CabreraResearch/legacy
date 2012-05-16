using System.Web;

namespace NbtWebAppServices.Core
{
    public class CswNbtWcfTools
    {
        public static string getIpAddress()
        {
            string IPAddress = default( string );

            if( HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null )
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if( IPAddress == string.Empty && HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null )
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return IPAddress;
        }

    }
}