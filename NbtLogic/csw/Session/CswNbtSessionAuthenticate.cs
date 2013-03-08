using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Security;
using ChemSW.Session;
using ChemSW.WebSvc;

namespace ChemSW.Nbt
{
    public class CswNbtSessionAuthenticate
    {

        private CswNbtResources _CswNbtResources;
        private CswSessionManager _CswSessionManager;
        private CswWebSvcSessionAuthenticateData.Authentication.Request _AuthenticationRequest;
        private AuthenticationStatus _AuthenticationStatus = AuthenticationStatus.Unknown;

        public CswNbtSessionAuthenticate( CswNbtResources Resources, CswSessionManager SessionManager, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest )
        {
            _CswNbtResources = Resources;
            _CswSessionManager = SessionManager;
            _AuthenticationRequest = AuthenticationRequest;

        } // constructor

        private CswNbtView _getView( string ViewId )
        {
            CswNbtView ReturnVal = null;

            if( CswNbtViewId.isViewIdString( ViewId ) )
            {
                CswNbtViewId realViewid = new CswNbtViewId( ViewId );
                ReturnVal = _CswNbtResources.ViewSelect.restoreView( realViewid );
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
            {
                CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
                ReturnVal = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
            }

            return ( ReturnVal );
        } // _getView()

        private void _reauthenticate()
        {
            if( _AuthenticationStatus == AuthenticationStatus.Authenticated )
            {
                // Set audit context
                if( false == string.IsNullOrEmpty( _AuthenticationRequest.CurrentViewId ) )
                {

                    CswNbtView ContextView = _getView( _AuthenticationRequest.CurrentViewId );
                    if( ContextView != null )
                    {
                        _CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( false == string.IsNullOrEmpty( _AuthenticationRequest.CurrentActionName ) )
                {
                    CswNbtAction ContextAction = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( _AuthenticationRequest.CurrentActionName )];
                    if( ContextAction != null )
                    {
                        _CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }
        }

        private AuthenticationStatus _authenticate()
        {
            AuthenticationStatus AuthenticationStatus = AuthenticationStatus.Unknown;

            try
            {
                string ParsedAccessId = _AuthenticationRequest.CustomerId.ToLower().Trim();
                if( false == string.IsNullOrEmpty( ParsedAccessId ) )
                {
                    _CswSessionManager.setAccessId( ParsedAccessId );
                }
                else
                {
                    throw new CswDniException( ErrorType.Warning, "There is no configuration information for this AccessId", "AccessId is null or empty." );
                }
            }
            catch( CswDniException ex )
            {
                if( !ex.Message.Contains( "There is no configuration information for this AccessId" ) )
                {
                    throw ex;
                }
                else
                {
                    AuthenticationStatus = AuthenticationStatus.NonExistentAccessId;
                }
            }

            if( AuthenticationStatus == AuthenticationStatus.Unknown )
            {
                AuthenticationStatus = _CswSessionManager.beginSession( _AuthenticationRequest.UserName, _AuthenticationRequest.Password, _AuthenticationRequest.IpAddress, _AuthenticationRequest.IsMobile );
            }

            // case 21211
            if( AuthenticationStatus == AuthenticationStatus.Authenticated )
            {
                // Removed for case 28617.  See case 28621.
                //// case 21036
                //if( _AuthenticationRequest.IsMobile && 
                //    false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SI ) )
                //{
                //    AuthenticationStatus = AuthenticationStatus.ModuleNotEnabled;
                //    _CswSessionManager.clearSession();
                //}
                CswLicenseManager LicenseManager = new CswLicenseManager( _CswNbtResources );
                //Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );

                if( _CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                {
                    // BZ 9077 - Password expired
                    AuthenticationStatus = AuthenticationStatus.ExpiredPassword;
                }
                else if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
                {
                    // BZ 8133 - make sure they've seen the License
                    AuthenticationStatus = AuthenticationStatus.ShowLicense;
                }
            }

            return AuthenticationStatus;
        }

        public AuthenticationStatus authenticate()
        {
            AuthenticationStatus Ret = _AuthenticationStatus;
            if( Ret != AuthenticationStatus.Authenticated )
            {
                Ret = _authenticate();
            }
            return Ret;
        }

        public bool deauthenticate()
        {
            _CswSessionManager.clearSession();
            return true;
        }//deAuthenticate()

    } // CswNbtSessionDataItem
} // namespace ChemSW.Nbt
