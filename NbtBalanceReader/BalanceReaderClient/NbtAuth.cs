using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Windows.Forms;
using BalanceReaderClient.NbtPublic;

namespace BalanceReaderClient
{
    public class NbtAuth
    {
        public string AccessId;
        public string UserId;
        public string Password;
        public bool useSSL;
        public string baseURL;
        private string sessionId;
        public Timer announceBalanceTimer;

        /// <summary>
        /// This delegate is used to specify what an NbtPublicClient should do after an authentication attempt.
        /// Passed as a parameter to PerformAction, which handles the entire authentication process.
        /// </summary>
        /// <param name="Client">an NbtPublicClient that has been initialized</param>
        public delegate string SessionCompleteEvent( NbtPublicClient Client );




        /// <summary>
        /// Starts a new NBT session and performs the action specified.
        /// </summary>
        /// <param name="RequestCallback">The delegate to be executed when the authentication attempt succeeds or fails</param>
        public string PerformAction( SessionCompleteEvent RequestCallback )
        {
            string StatusText = "";

            NbtPublicClient NbtClient = new NbtPublicClient();
            try
            {
                string EndpointUrl = _formatUrl(baseURL);
                useSSL = ( EndpointUrl.ToLower().IndexOf( "https:" ) > -1 );
                NbtClient.Endpoint.Address = new EndpointAddress( EndpointUrl );
                NbtClient.Endpoint.Binding = new WebHttpBinding
                    {
                        AllowCookies = true,
                        Security = new WebHttpSecurity
                            {
                                Mode = useSSL ? WebHttpSecurityMode.Transport : WebHttpSecurityMode.None
                            }
                    };
                //create a scope so that we can send the sessionId header
                using( OperationContextScope Scope = new OperationContextScope( NbtClient.InnerChannel ) )
                {
                    WebOperationContext.Current.OutgoingRequest.Headers.Add( "X-NBT-SessionId", sessionId );
                    StatusText = RequestCallback( NbtClient );
                }


                if( StatusText == "NonExistentSession" )
                {//if the session doesn't exist, make an authentication request to get a new session ID

                    //create a scope so we can extract the session id header
                    using( OperationContextScope Scope = new OperationContextScope( NbtClient.InnerChannel ) )
                    {
                        CswNbtWebServiceSessionCswNbtAuthReturn AuthenticationRequest = NbtClient.SessionInit( new CswWebSvcSessionAuthenticateDataAuthenticationRequest
                            {
                                CustomerId = AccessId,
                                UserName = UserId,
                                Password = Password,
                                IsMobile = true,
                                SuppressLog = true
                            } );

                        StatusText = AuthenticationRequest.Authentication.AuthenticationStatus;
                        if( StatusText == "Authenticated" )
                        {//if the authentication was successful, re-send the web request
                            sessionId = WebOperationContext.Current.IncomingResponse.Headers["X-NBT-SessionId"];
                            PerformAction( RequestCallback );
                        }
                    }//operationContextScope
                }//if the session doesn't exist
               

            } //try

            catch( Exception Ex )
            {
                StatusText += Ex.Message;
            }

            if( StatusText != "Authenticated" )
            {
                announceBalanceTimer.Stop();
            }
            NbtClient.Close();

            return StatusText;

        }//performAction()


        
        /// <summary>
        /// An asynchronous version of PerformAction, to be called by a BackgroundWorker
        /// Used for NBT requests that are sent directly from the form
        /// </summary>
        /// <param name="Sender">the BackgroundWorker object which called testConnection</param>
        /// <param name="Arguments">A wrapper for the SessionCompleteEvent passed in by the BackgroundWorker</param>
        /// <returns></returns>
        public void PerformActionAsync( object Sender, DoWorkEventArgs Arguments )
        {
            SessionCompleteEvent CompletionCallback = (SessionCompleteEvent) Arguments.Argument;
            PerformAction( CompletionCallback );

        }


        private string _formatUrl( string Url )
        {
            if( false == Url.EndsWith( "NbtPublic.svc" ) )
            {
                //the user didn't give us the right endpoint, try to detect the correct one

                //anything that isn't the end will need to at least end with /
                if( false == Url.EndsWith( "/" ) )
                {
                    Url += "/";
                }

                //if they found the services directory, all that is needed is pointing to NbtPublic
                if( Url.EndsWith( "Services/" ) )
                {
                    Url += "NbtPublic.svc";
                }
                //otherwise, it seems most likely that they gave the root of the NBT web app
                else
                {
                    Url += "Services/NbtPublic.svc";
                }

            }//if false == Url.EndsWith( "NbtPublic.svc" )

            return Url;
        }


    }//class NbtAuth
}//namespace
