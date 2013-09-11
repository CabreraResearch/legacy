using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
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
        private CswEnumAuthenticationStatus _AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;

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
            if( _AuthenticationStatus == CswEnumAuthenticationStatus.Authenticated )
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

        private CswEnumAuthenticationStatus _authenticate()
        {
            CswEnumAuthenticationStatus AuthenticationStatus = CswEnumAuthenticationStatus.Unknown;

            try
            {
                string ParsedAccessId = _AuthenticationRequest.CustomerId.ToLower().Trim();
                if( false == string.IsNullOrEmpty( ParsedAccessId ) )
                {
                    _CswSessionManager.setAccessId( ParsedAccessId );
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "There is no configuration information for this AccessId", "AccessId is null or empty." );
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
                    AuthenticationStatus = CswEnumAuthenticationStatus.NonExistentAccessId;
                }
            }

            if( AuthenticationStatus == CswEnumAuthenticationStatus.Unknown )
            {
                AuthenticationStatus = _CswSessionManager.beginSession( _AuthenticationRequest );
            }

            // case 21211
            if( AuthenticationStatus == CswEnumAuthenticationStatus.Authenticated )
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
                if( LicenseManager.MustShowLicense( _CswNbtResources.CurrentUser ) )
                {
                    if( LicenseManager.AllowShowLicense( _CswNbtResources.CurrentUser ) )
                    {
                        // BZ 8133 - make sure they've seen the License
                        AuthenticationStatus = CswEnumAuthenticationStatus.ShowLicense;
                    }
                    else
                    {
                        // case 30086 - prevent login if admin hasn't accepted the license yet
                        AuthenticationStatus = CswEnumAuthenticationStatus.NoLicense;
                        _CswSessionManager.clearSession();
                    }
                }
                else if( _CswNbtResources.CurrentNbtUser.PasswordIsExpired )
                {
                    // BZ 9077 - Password expired
                    AuthenticationStatus = CswEnumAuthenticationStatus.ExpiredPassword;
                }
                else if( 1 < _CswNbtResources.CswSessionManager.SessionsList.getSessionCountForUser( _CswNbtResources.AccessId, _AuthenticationRequest.UserName ) 
                      && false == _AuthenticationRequest.IsMobile
                      && CswNbtObjClassUser.ChemSWAdminUsername  != _CswNbtResources.CurrentUser.Username )
                {
                    AuthenticationStatus = CswEnumAuthenticationStatus.AlreadyLoggedIn;
                }
            }

            return AuthenticationStatus;
        }

        public CswEnumAuthenticationStatus authenticate()
        {
            CswEnumAuthenticationStatus Ret = _AuthenticationStatus;
            if( Ret != CswEnumAuthenticationStatus.Authenticated )
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
