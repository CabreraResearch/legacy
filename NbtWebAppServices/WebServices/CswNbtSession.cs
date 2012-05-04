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
    public class CswNbtSession
    {
        private HttpContext Context = HttpContext.Current;
        public static CswNbtSessionResources CswNbtSessionResources = null;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Session/init" )]
        public CswNbtWebServiceResponseNoData init( CswNbtSessionRequest request )//string CustomerId, string UserName, string Password )
        {
            CswNbtWebServiceResponseNoData Ret = new CswNbtWebServiceResponseNoData();
            Ret.AuthenticationStatus = new CswNbtSessionAuthenticationStatus();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                CswNbtSessionResources = CswNbtSessionResources.initResources( Context );
                try
                {
                    string ParsedAccessId = request.CustomerId.ToLower().Trim();
                    if( false == string.IsNullOrEmpty( ParsedAccessId ) )
                    {
                        CswNbtSessionResources.CswSessionManager.setAccessId( ParsedAccessId );
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Warning, "There is no configuration information for this CustomerId", "CustomerId is null or empty." );
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
                    AuthenticationStatus = CswNbtSessionResources.CswSessionManager.beginSession( request.UserName, request.Password, CswWebserviceTools.getIpAddress() );
                }

                // case 21211
                if( AuthenticationStatus == AuthenticationStatus.Authenticated )
                {
                    // case 21036
                    if( request.IsMobile &&
                         false == CswNbtSessionResources.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                    {
                        AuthenticationStatus = AuthenticationStatus.ModuleNotEnabled;
                        CswNbtSessionResources.CswSessionManager.clearSession();
                    }
                    CswLicenseManager LicenseManager = new CswLicenseManager( CswNbtSessionResources.CswNbtResources );

                    if( CswNbtSessionResources.CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                    {
                        // BZ 9077 - Password expired
                        AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
                    }
                    else if( LicenseManager.MustShowLicense( CswNbtSessionResources.CswNbtResources.CurrentUser ) )
                    {
                        // BZ 8133 - make sure they've seen the License
                        AuthenticationStatus = AuthenticationStatus.ShowLicense;
                    }
                }

                //bury the overhead of nuking old sessions in the overhead of authenticating
                CswNbtSessionResources.purgeExpiredSessions();
            }
            catch( Exception ex )
            {
                Ret.addError( ex, CswNbtSessionResources );
            }
            Ret.finalizeResponse( AuthenticationStatus, CswNbtSessionResources, null );

            return Ret; //_AddAuthenticationStatus( AuthenticationStatus.Authenticated );
        }

        //[OperationContract]
        //[WebInvoke( Method = "POST", UriTemplate = "Session/all" )]
        //public string getSessions()
        //{
        //    JObject ReturnVal = new JObject();
        //    AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
        //    try
        //    {
        //        _initResources();
        //        AuthenticationStatus = _attemptRefresh();
        //        if( AuthenticationStatus.Authenticated == AuthenticationStatus )
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

        //    _jAddAuthenticationStatus( ReturnVal, AuthenticationStatus );

        //    return ReturnVal.ToString();

        //} // getSessions()

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Session/end" )]
        public CswNbtWebServiceResponseNoData end( string SessionId )
        {
            CswNbtWebServiceResponseNoData Ret = new CswNbtWebServiceResponseNoData();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;
            CswNbtSessionResources Resources = null;
            try
            {
                Resources = CswNbtSessionResources.initResources( Context );
                AuthenticationStatus = Resources.attemptRefresh();
                if( AuthenticationStatus.Authenticated == AuthenticationStatus )
                {
                    CswNbtSessionResources.CswSessionManager.clearSession( SessionId );
                }
                Resources.deInitResources();
            }
            catch( Exception Ex )
            {
                Ret.addError( Ex, Resources );
            }
            Ret.finalizeResponse( AuthenticationStatus, Resources );
            return Ret;

        } // endSession()
    }
}