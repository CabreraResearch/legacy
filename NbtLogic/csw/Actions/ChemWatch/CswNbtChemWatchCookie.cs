using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace NbtWebApp.WebSvc.Logic.Actions.ChemWatch
{
    public class CookieManagerMessageInspector: IClientMessageInspector
    {
        private string sharedCookie;

        public void AfterReceiveReply( ref Message reply, object correlationState )
        {
            HttpResponseMessageProperty httpResponse =
                reply.Properties[HttpResponseMessageProperty.Name]
                as HttpResponseMessageProperty;

            if( httpResponse != null )
            {
                string cookie = httpResponse.Headers[HttpResponseHeader.SetCookie];

                if( false == string.IsNullOrEmpty( cookie ) )
                {
                    cookie = cookie.Replace( "; path=/", "" );
                    cookie = cookie.Replace( "HttpOnly,", "" );
                    this.sharedCookie = cookie;
                }
            }
        }

        public object BeforeSendRequest( ref Message request, IClientChannel channel )
        {
            HttpRequestMessageProperty httpRequest;

            // The HTTP request object is made available in the outgoing message only
            // when the Visual Studio Debugger is attacched to the running process
            if( !request.Properties.ContainsKey( HttpRequestMessageProperty.Name ) )
            {
                request.Properties.Add( HttpRequestMessageProperty.Name,
                                        new HttpRequestMessageProperty() );
            }

            httpRequest = (HttpRequestMessageProperty)
                          request.Properties[HttpRequestMessageProperty.Name];
            httpRequest.Headers.Add( HttpRequestHeader.Cookie, this.sharedCookie );

            return null;
        }
    }

    public class CookieManagerBehavior: IEndpointBehavior
    {
        private CookieManagerMessageInspector c = new CookieManagerMessageInspector();

        public void AddBindingParameters( ServiceEndpoint endpoint, BindingParameterCollection bindingParameters )
        {
        }

        public void ApplyClientBehavior( ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime )
        {
            clientRuntime.MessageInspectors.Add( c );
        }

        public void ApplyDispatchBehavior( ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher )
        {
        }

        public void Validate( ServiceEndpoint endpoint )
        {
        }
    }
}