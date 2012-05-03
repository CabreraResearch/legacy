using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Security;
using NbtWebAppServices.Core;
using NbtWebAppServices.Session;

namespace NbtWebAppServices.WebServices
{
    [ServiceContract]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Session
    {
        CswTimer Timer = new CswTimer();
        double ServerInitTime = 0;
        private HttpContext Context = HttpContext.Current;
        public CswNbtSessionResources CswNbtSessionResources;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Session/init" )]
        public CswSessionAuthentication init( CswSessionRequest request )//string CustomerId, string UserName, string Password )
        {
            CswSessionAuthentication Ret = new CswSessionAuthentication();
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

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
            Ret = _AddAuthenticationStatus( AuthenticationStatus );
            return Ret; //_AddAuthenticationStatus( AuthenticationStatus.Authenticated );
        }

        private CswSessionAuthentication _AddAuthenticationStatus( AuthenticationStatus AuthenticationStatusIn )
        {
            CswSessionAuthentication CswSessionAuthentication = new CswSessionAuthentication
                                                              {
                                                                  AuthenticationStatus = AuthenticationStatusIn.ToString()
                                                              };

            if( null != CswNbtSessionResources &&
                null != CswNbtSessionResources.CswSessionManager )
            {
                CswDateTime CswTimeout = new CswDateTime( CswNbtSessionResources.CswNbtResources, CswNbtSessionResources.CswSessionManager.TimeoutDate );
                CswSessionAuthentication.TimeOut = CswTimeout.ToClientAsJavascriptString();
            }
            CswSessionAuthentication.CswPerfTimer = new CswPerfTimer
                                              {
                                                  ServerInit = ServerInitTime
                                              };
            if( null != CswNbtSessionResources &&
                null != CswNbtSessionResources.CswNbtResources )
            {
                CswSessionAuthentication.CswPerfTimer.DbInit = CswNbtSessionResources.CswNbtResources.CswLogger.DbInitTime;
                CswSessionAuthentication.CswPerfTimer.DbQuery = CswNbtSessionResources.CswNbtResources.CswLogger.DbQueryTime;
                CswSessionAuthentication.CswPerfTimer.DbCommit = CswNbtSessionResources.CswNbtResources.CswLogger.DbCommitTime;
                CswSessionAuthentication.CswPerfTimer.DbDeinit = CswNbtSessionResources.CswNbtResources.CswLogger.DbDeInitTime;
                CswSessionAuthentication.CswPerfTimer.TreeLoaderSql = CswNbtSessionResources.CswNbtResources.CswLogger.TreeLoaderSQLTime;
            }
            CswSessionAuthentication.CswPerfTimer.ServerTotal = Timer.ElapsedDurationInMilliseconds;

            return CswSessionAuthentication;
        }
    }
}