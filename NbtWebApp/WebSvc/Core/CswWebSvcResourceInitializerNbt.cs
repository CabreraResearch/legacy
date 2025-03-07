using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Security;

namespace ChemSW.WebSvc
{
    public class CswWebSvcResourceInitializerNbt: ICswWebSvcResourceInitializer
    {
        private CswTimer _Timer = new CswTimer();
        private CswWebSvcSessionAuthenticateData.Authentication.Request _AuthenticationRequest;

        public HttpContext _HttpContext = null;
        public HttpContext HttpContext
        {
            get { return _HttpContext; }
        }

        private void _setHttpContextOnRequest()
        {
            if( null != _HttpContext.Request.Cookies["csw_currentviewid"] )
            {
                _AuthenticationRequest.CurrentViewId = _HttpContext.Request.Cookies["csw_currentviewid"].Value;
            }
            if( null != _HttpContext.Request.Cookies["csw_currentactionname"] )
            {
                _AuthenticationRequest.CurrentActionName = _HttpContext.Request.Cookies["csw_currentactionname"].Value;
            }
            _AuthenticationRequest.IpAddress = CswWebSvcCommonMethods.getIpAddress();
        }

        public CswWebSvcResourceInitializerNbt( HttpContext HttpContext, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest = null ) //TODO: add Username/Password
        {
            _HttpContext = HttpContext;
            _AuthenticationRequest = AuthenticationRequest ?? new CswWebSvcSessionAuthenticateData.Authentication.Request();
            _setHttpContextOnRequest();
        }

        private CswSessionResourcesNbt _CswSessionResourcesNbt = null;

        private CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {
            get
            {
                return ( _CswNbtResources );
            }
        }

        private CswNbtSessionAuthenticate _SessionAuthenticate = null;

        public ICswResources initResources()
        {
            _CswSessionResourcesNbt = new CswSessionResourcesNbt( _HttpContext.Application, _HttpContext.Request, _HttpContext.Response, _HttpContext, string.Empty, CswEnumSetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResourcesNbt.CswNbtResources;
            _CswNbtResources.beginTransaction();

            if( null != _AuthenticationRequest.RequiredModules )
            {
                foreach( string RequiredModule in _AuthenticationRequest.RequiredModules )
                {
                    if( false == _CswNbtResources.Modules.IsModuleEnabled( RequiredModule ) )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Not all required modules are enabled. Please contact your administrator.", "The " + RequiredModule + " module is required in order to complete this request." );
                    }
                }
            }
            _SessionAuthenticate = new CswNbtSessionAuthenticate( _CswNbtResources, _CswSessionResourcesNbt.CswSessionManager, _AuthenticationRequest );
            return ( _CswNbtResources );

        }//_initResources() 

        public CswEnumAuthenticationStatus authenticate()
        {
            CswEnumAuthenticationStatus Ret = CswEnumAuthenticationStatus.Unknown;
            //We're keeping this logic here, because we don't want to contaminate NbtLogic with the necessary web libraries required to support CswSessionResourcesNbt
            if( null != _AuthenticationRequest && _AuthenticationRequest.IsValid() )
            {
                if( false == CswTools.IsValidUsername( _AuthenticationRequest.CustomerId ) )
                {
                    Ret = CswEnumAuthenticationStatus.NonExistentAccessId;
                }
                else
                {
                    Ret = _SessionAuthenticate.authenticate();
                }
            }
            else
            {
                Ret = _CswSessionResourcesNbt.attemptRefresh();
            }

            //Set audit context
            if( Ret == CswEnumAuthenticationStatus.Authenticated && null != _HttpContext.Request.Cookies )
            {
                string ContextViewId = string.Empty;
                string ContextActionName = string.Empty;
                
                if( null != _HttpContext.Request.Cookies["csw_currentviewid"] )
                {
                    ContextViewId = _HttpContext.Request.Cookies["csw_currentviewid"].Value;
                }
                if( null != _HttpContext.Request.Cookies["csw_currentactionname"] )
                {
                    ContextActionName = _HttpContext.Request.Cookies["csw_currentactionname"].Value;
                }

                if( string.Empty != ContextViewId )
                {
                    CswNbtView ContextView = null;
                    if( CswNbtViewId.isViewIdString( ContextViewId ) )
                    {
                        CswNbtViewId realViewid = new CswNbtViewId( ContextViewId );
                        ContextView = _CswNbtResources.ViewSelect.restoreView( realViewid );
                    }
                    else if( CswNbtSessionDataId.isSessionDataIdString( ContextViewId ) )
                    {
                        CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ContextViewId );
                        ContextView = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
                    }
                    if( null != ContextView )
                    {
                        _CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( string.Empty != ContextActionName )
                {
                    CswNbtAction ContextAction = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( null != ContextAction )
                    {
                        _CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }

            _CswNbtResources.ServerInitTime = _Timer.ElapsedDurationInMilliseconds;

            return ( Ret );

        }//autheticate

        public void deauthenticate()
        {
            _SessionAuthenticate.deauthenticate();
        }//autheticate

        public void deInitResources()
        {
            if( _CswSessionResourcesNbt != null )
            {
                _CswSessionResourcesNbt.endSession();
                _CswSessionResourcesNbt.finalize();
                _CswSessionResourcesNbt.release();
            }
            if( null != _CswNbtResources )
            {
                _CswNbtResources.TotalServerTime = _Timer.ElapsedDurationInMilliseconds;
            }
        } // _deInitResources()

    } // class CswWebSvcResourceInitializerCommon

} // namespace ChemSW.Nbt.WebServices
