using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Security;
using NbtWebAppServices.Core;
using NbtWebAppServices.Response;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class CswNbtWcfSessionUriMethods
    {
        private HttpContext _Context = HttpContext.Current;
        private CswNbtWcfSessionResources _CswNbtWcfSessionResources = null;

        [OperationContract]
        [WebGet( RequestFormat = WebMessageFormat.Json )]
        public CswNbtWcfRequest.CswNbtSessionRequest template()
        {
            return new CswNbtWcfRequest.CswNbtSessionRequest();
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        public CswNbtWcfResponseBase init( CswNbtWcfRequest.CswNbtSessionRequest request )
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            CswNbtWcfResponseBase Ret = new CswNbtWcfResponseBase();
            if( false == string.IsNullOrEmpty( request.CustomerId ) &&
                false == string.IsNullOrEmpty( request.UserName ) &&
                false == string.IsNullOrEmpty( request.Password ) )
            {
                Ret = new CswNbtWcfResponseBase( _Context, request.IsMobile );


                try
                {
                    _CswNbtWcfSessionResources = Ret.CswNbtWcfSessionResources;
                    try
                    {
                        string ParsedAccessId = request.CustomerId.ToLower().Trim();
                        if( false == string.IsNullOrEmpty( ParsedAccessId ) )
                        {
                            _CswNbtWcfSessionResources.CswSessionManager.setAccessId( ParsedAccessId );
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Warning,
                                                      "There is no configuration information for this CustomerId",
                                                      "CustomerId is null or empty." );
                        }
                    }
                    catch( CswDniException ex )
                    {
                        if( false == ex.Message.Contains( "There is no configuration information for this CustomerId" ) )
                        {
                            throw ex;
                        }
                        AuthenticationStatus = AuthenticationStatus.NonExistentAccessId;
                    }

                    if( AuthenticationStatus == AuthenticationStatus.Unknown )
                    {
                        AuthenticationStatus =
                            _CswNbtWcfSessionResources.CswSessionManager.beginSession( request.UserName, request.Password,
                                                                                      CswNbtWcfTools.getIpAddress(),
                                                                                      request.IsMobile );
                    }

                    // case 21211
                    if( AuthenticationStatus == AuthenticationStatus.Authenticated )
                    {
                        // case 21036
                        if( request.IsMobile &&
                            false == _CswNbtWcfSessionResources.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) )
                        {
                            AuthenticationStatus = AuthenticationStatus.ModuleNotEnabled;
                            _CswNbtWcfSessionResources.CswSessionManager.clearSession();
                        }
                        CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtWcfSessionResources.CswNbtResources );

                        if( _CswNbtWcfSessionResources.CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                        {
                            // BZ 9077 - Password expired
                            AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
                        }
                        else if( LicenseManager.MustShowLicense( _CswNbtWcfSessionResources.CswNbtResources.CurrentUser ) )
                        {
                            // BZ 8133 - make sure they've seen the License
                            AuthenticationStatus = AuthenticationStatus.ShowLicense;
                        }
                    }

                    //bury the overhead of nuking old sessions in the overhead of authenticating
                    _CswNbtWcfSessionResources.purgeExpiredSessions();
                }
                catch( Exception ex )
                {
                    Ret.addError( ex );
                }
                Ret.SessionAuthenticationStatus.AuthenticationStatus = AuthenticationStatus.ToString();
            }
            Ret.finalizeResponse();

            return Ret; //_AddAuthenticationStatus( SessionAuthenticationStatus.Authenticated );
        }

        //[OperationContract]
        //[WebInvoke( Method = "POST", UriTemplate = "Session/all" )]
        //public string getSessions()
        //{
        //    JObject ReturnVal = new JObject();
        //    SessionAuthenticationStatus SessionAuthenticationStatus = SessionAuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        SessionAuthenticationStatus = _attemptRefresh();
        //        if( SessionAuthenticationStatus.Authenticated == SessionAuthenticationStatus )
        //        {

        //            SortedList<string, CswSessionsListEntry> SessionList = _CswSessionResources.CswSessionManager.SessionsList.AllSessions;
        //            foreach( CswSessionsListEntry Entry in SessionList.Values )
        //            {
        //                // Filter to the administrator's access id only
        //                if( Entry.AccessId == _CswNbtResources.AccessId || _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
        //                {
        //                    JObject JSession = new JObject();
        //                    JSession["sessionid"] = Entry.SessionId;
        //                    JSession["username"] = Entry.UserName;
        //                    JSession["logindate"] = Entry.LoginDate.ToString();
        //                    JSession["timeoutdate"] = Entry.TimeoutDate.ToString();
        //                    JSession["accessid"] = Entry.AccessId;
        //                    ReturnVal[Entry.SessionId] = JSession;
        //                } // if (Entry.AccessId == Master.AccessID)
        //            } // foreach (CswAuthenticator.SessionListEntry Entry in SessionList.Values)
        //        }
        //        _deInitResources();
        //    }
        //    catch( Exception Ex )
        //    {
        //        ReturnVal = jError( Ex );
        //    }

        //    _jAddAuthenticationStatus( ReturnVal, SessionAuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // getSessions()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        public CswNbtWcfResponseBase end( string CswSessionId )
        {
            CswNbtWcfResponseBase Ret = new CswNbtWcfResponseBase( _Context, false );
            try
            {
                _CswNbtWcfSessionResources = Ret.CswNbtWcfSessionResources;
                if( Ret.SessionAuthenticationStatus.AuthenticationStatus == AuthenticationStatus.Authenticated.ToString() )
                {
                    _CswNbtWcfSessionResources.CswSessionManager.clearSession( CswSessionId );
                }
            }
            catch( Exception Ex )
            {
                Ret.addError( Ex );
            }
            Ret.finalizeResponse();
            return Ret;

        } // endSession()
    }
}