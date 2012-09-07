using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Security;
// supports ScriptService attribute
// supports ScriptService attribute

// supports ScriptService attribute



namespace ChemSW.WebSvc
{
    public class CswWebSvcResourceInitializerNbt : ICswWebSvcResourceInitializer
    {
        private CswTimer _Timer = new CswTimer();
        HttpContext _HttpContext = null;
        public CswWebSvcResourceInitializerNbt( HttpContext HttpContext )
        {
            _HttpContext = HttpContext;
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


        public ICswResources initResources()
        {
            _CswSessionResourcesNbt = new CswSessionResourcesNbt( _HttpContext.Application, _HttpContext.Request, _HttpContext.Response, _HttpContext, string.Empty, SetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResourcesNbt.CswNbtResources;
            _CswNbtResources.beginTransaction();

            return ( _CswNbtResources );

        }//_initResources() 

        public AuthenticationStatus authenticate()
        {
            AuthenticationStatus ReturnVal = _CswSessionResourcesNbt.attemptRefresh();

            if( ReturnVal == AuthenticationStatus.Authenticated )
            {
                // Set audit context
                string ContextViewId = string.Empty;
                string ContextActionName = string.Empty;
                if( _HttpContext.Request.Cookies["csw_currentviewid"] != null )
                {
                    ContextViewId = _HttpContext.Request.Cookies["csw_currentviewid"].Value;
                }
                if( _HttpContext.Request.Cookies["csw_currentactionname"] != null )
                {
                    ContextActionName = _HttpContext.Request.Cookies["csw_currentactionname"].Value;
                }

                if( ContextViewId != string.Empty )
                {

                    CswNbtView ContextView = CswWebSvcCommonMethods.getView( _CswNbtResources, ContextViewId );
                    if( ContextView != null )
                    {
                        _CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( ContextActionName != string.Empty )
                {
                    CswNbtAction ContextAction = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( ContextAction != null )
                    {
                        _CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }

            _CswNbtResources.ServerInitTime = _Timer.ElapsedDurationInMilliseconds;

            return ( ReturnVal );

        }//autheticate

        public void deInitResources()
        {
            if( _CswSessionResourcesNbt != null )
            {
                _CswSessionResourcesNbt.endSession();

                _CswSessionResourcesNbt.finalize();
                _CswSessionResourcesNbt.release();
            }

            _CswNbtResources.TotalServerTime = _Timer.ElapsedDurationInMilliseconds;

        } // _deInitResources()

    } // class CswWebSvcResourceInitializerCommon

} // namespace ChemSW.Nbt.WebServices
