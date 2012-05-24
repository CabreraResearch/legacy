using System.Web;
using ChemSW;
using ChemSW.Config;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.RscAdo;
using ChemSW.Security;
using ChemSW.Session;
using NbtWebAppServices.Core;

namespace NbtWebAppServices.Session
{
    public class CswNbtWcfSessionResources
    {
        public CswNbtResources CswNbtResources = null;
        public ICswResources CswResourcesMaster = null;
        //private CswNbtMetaDataEvents _CswNbtMetaDataEvents;
        public CswSessionManager CswSessionManager = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        private CswNbtWcfStatistics _CswNbtWcfStatistics = null;
        private HttpContext _Context;
        private static bool _IsMobile;
        AppType _AppType = AppType.Nbt;

        public CswNbtWcfSessionResources( HttpContext Context, string LoginAccessId, SetupMode SetupMode, bool IsMobile )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
            _Context = Context;
            _IsMobile = IsMobile;
            if( _IsMobile )
            {
                _AppType = AppType.Mobile;
            }

            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtWeb );

            CswDbCfgInfo CswDbCfgInfo = new CswDbCfgInfo( SetupMode.NbtWeb, IsMobile );
            CswResourcesMaster = new CswResources( _AppType, SetupVbls, CswDbCfgInfo, false, new CswSuperCycleCacheDefault(), null );
            CswResourcesMaster.SetDbResources( PooledConnectionState.Open );
            CswResourcesMaster.AccessId = CswDbCfgInfo.MasterAccessId;

            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( _AppType, SetupMode, true, false,
                                                                          CswResourcesMaster: CswResourcesMaster,
                                                                          CswLogger: CswResourcesMaster.CswLogger );


            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswNbtResources.SetupVbls.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswNbtResources.SetupVbls[RecordStatisticsVblName] );
            }
            
            CswSessionManager = new CswSessionManager( _AppType,
                                                       new CswNbtWcfCookies( _Context.Request, _Context.Response ),
                                                       LoginAccessId,
                                                       CswNbtResources.SetupVbls,
                                                       CswNbtResources.CswDbCfgInfo,
                                                       false,
                                                       CswNbtResources,
                                                       CswResourcesMaster,
                                                       new CswNbtSchemaAuthenticator( CswNbtResources ),
                                                       _CswNbtWcfStatistics = new CswNbtWcfStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ),
                                                                                                  RecordStatistics ) );
            CswNbtStatisticsEvents = _CswNbtWcfStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += OnDeauthenticate;

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()

        public static CswNbtWcfSessionResources initResources( HttpContext Context, bool IsMobile )
        {
            CswNbtWcfSessionResources Ret = new CswNbtWcfSessionResources( Context, string.Empty, SetupMode.NbtWeb, IsMobile );
            Ret.CswNbtResources.beginTransaction();
            Ret.CswNbtResources.logMessage( "WebServices: CswSession Started (_initResources called)" );
            return Ret;
        }

        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }

        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess( false ); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate( string SessionId )
        {
            CswNbtResources.SessionDataMgr.removeAllSessionData( SessionId );
        }//OnDeauthenticate()

        public void deInitResources( CswNbtResources OtherResources = null )
        {
            endSession();
            if( null != CswNbtResources )
            {
                CswNbtResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                CswNbtResources.finalize();
                CswNbtResources.release();
            }
            if( null != OtherResources )
            {
                OtherResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );

                OtherResources.finalize();
                OtherResources.release();
            }
        } // _deInitResources()

        public AuthenticationStatus attemptRefresh( bool ThrowOnError = false )
        {
            AuthenticationStatus ret = attemptRefresh();

            if( ThrowOnError &&
                ret != AuthenticationStatus.Authenticated )
            {
                throw new CswDniException( ErrorType.Warning, "Current session is not authenticated, please login again.", "Cannot execute web method without a valid session." );
            }

            if( ret == AuthenticationStatus.Authenticated )
            {
                // Set audit context
                string ContextViewId = string.Empty;
                string ContextActionName = string.Empty;
                if( _Context.Request.Cookies["csw_currentviewid"] != null )
                {
                    HttpCookie HttpCookie = _Context.Request.Cookies["csw_currentviewid"];
                    if( HttpCookie != null )
                    {
                        ContextViewId = HttpCookie.Value;
                    }
                }
                if( _Context.Request.Cookies["csw_currentactionname"] != null )
                {
                    HttpCookie HttpCookie = _Context.Request.Cookies["csw_currentactionname"];
                    if( HttpCookie != null )
                    {
                        ContextActionName = HttpCookie.Value;
                    }
                }

                if( ContextViewId != string.Empty )
                {
                    //CswNbtView ContextView = _getView( ContextViewId );
                    //if( ContextView != null )
                    //{
                    //    CswNbtWcfSessionResources.CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    //}
                }
                else if( ContextActionName != string.Empty )
                {
                    CswNbtAction ContextAction = CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( ContextAction != null )
                    {
                        CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }

            return ret;
        } // _attemptRefresh()


    }//CswNbtWcfSessionResources

}//ChemSW.Nbt
