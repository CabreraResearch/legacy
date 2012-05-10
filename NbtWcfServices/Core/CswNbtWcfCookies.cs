using System;
// This is why this class is in the web controls package
using System.Web;
using ChemSW.Session;

namespace NbtWebAppServices.Core
{
    public class CswNbtWcfCookies : ICswWebClientStorage
    {
        private HttpRequest _HttpRequest;
        private HttpResponse _HttpResponse;

        private string _SessionCookieName = "CswSessionId";

        public CswNbtWcfCookies( HttpRequest HttpRequest, HttpResponse HttpResponse )
        {
            _HttpRequest = HttpRequest;
            _HttpResponse = HttpResponse;
        }

        //Sergei:
        //If C# had an assignment operator I'd use it here; as it is, since saying
        //things like 
        //                      CachedSessionId.ID = nu
        //seems dundant, I'm using good old fashioned get() put() semantics
        //--Dimitri
        public string getSessionId()
        {
            string ReturnVal = string.Empty;
            HttpCookie SessionCookie = _HttpRequest.Cookies.Get( _SessionCookieName );

            if( null != SessionCookie )
            {
                ReturnVal = SessionCookie.Value;
                SessionCookie.HttpOnly = true;
            }
            return ( ReturnVal );
        }//load() 

        public void putSession( CswSessionsListEntry SessionsListEntry )
        {

            HttpCookie SessionCookie = _HttpResponse.Cookies.Get( _SessionCookieName );
            if( null == SessionCookie )
            {
                SessionCookie = new HttpCookie( _SessionCookieName ) { HttpOnly = true };
                _HttpResponse.Cookies.Add( SessionCookie );
            }
            if( SessionsListEntry.IsMobile )
            {
                SessionCookie.Expires = DateTime.Now.AddMonths( 3 );
            }
            SessionCookie.Value = SessionsListEntry.SessionId;
        }

        public void delete()
        {
            if( null != _HttpResponse.Cookies[_SessionCookieName] )
            {
                HttpCookie HttpCookie = _HttpResponse.Cookies[_SessionCookieName];
                if( HttpCookie != null )
                {
                    HttpCookie.Expires = DateTime.Now.AddDays( -5 );
                }
            }
        }

    }//ICswSession

}//ChemSW.Core
