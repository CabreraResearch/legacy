using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Security;
using ChemSW.Session;

// supports ScriptService attribute
// supports ScriptService attribute

// supports ScriptService attribute



namespace ChemSW.WebSvc
{
    public class CswWebSvcResourceInitializerNbt : ICswWebSvcResourceInitializer
    {
        private CswTimer _Timer = new CswTimer();
        private HttpContext _HttpContext = null;
        private CswNbtSessionAuthenticateData.Authentication.Request _AuthenticationRequest;
        private delegate void _OnDeInitDelegate();
        private _OnDeInitDelegate _OnDeInit;

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

        public CswWebSvcResourceInitializerNbt( HttpContext HttpContext, CswNbtSessionAuthenticateData.Authentication.Request AuthenticationRequest ) //TODO: add Username/Password
        {
            _HttpContext = HttpContext;
            _AuthenticationRequest = AuthenticationRequest ?? new CswNbtSessionAuthenticateData.Authentication.Request();
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
            _CswSessionResourcesNbt = new CswSessionResourcesNbt( _HttpContext.Application, _HttpContext.Request, _HttpContext.Response, _HttpContext, string.Empty, SetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResourcesNbt.CswNbtResources;
            _CswNbtResources.beginTransaction();
            _SessionAuthenticate = new CswNbtSessionAuthenticate( _CswNbtResources, _CswSessionResourcesNbt.CswSessionManager, _AuthenticationRequest );
            _OnDeInit = new _OnDeInitDelegate( _deInitResources );
            return ( _CswNbtResources );

        }//_initResources() 

        public AuthenticationStatus authenticate()
        {
            AuthenticationStatus Ret = _CswSessionResourcesNbt.attemptRefresh();
            if( Ret != AuthenticationStatus.Authenticated )
            {
                Ret = _SessionAuthenticate.authenticate();
            }

            _CswNbtResources.ServerInitTime = _Timer.ElapsedDurationInMilliseconds;

            return ( Ret );

        }//autheticate

        private void _deInitResources()
        {
            if( _CswSessionResourcesNbt != null )
            {
                _CswSessionResourcesNbt.endSession();

                //bury the overhead of nuking old sessions in the overhead of authenticating
                _CswSessionResourcesNbt.purgeExpiredSessions();

                _CswSessionResourcesNbt.finalize();
                _CswSessionResourcesNbt.release();
            }
        }

        public void deInitResources()
        {
            _OnDeInit.BeginInvoke( null, null );

            _CswNbtResources.TotalServerTime = _Timer.ElapsedDurationInMilliseconds;

        } // _deInitResources()

    } // class CswWebSvcResourceInitializerCommon

} // namespace ChemSW.Nbt.WebServices
