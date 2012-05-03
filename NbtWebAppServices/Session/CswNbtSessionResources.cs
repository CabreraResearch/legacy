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
    public class CswNbtSessionResources
    {
        public CswNbtResources CswNbtResources = null;
        public ICswResources CswResourcesMaster = null;
        //private CswNbtMetaDataEvents _CswNbtMetaDataEvents;
        public CswSessionManager CswSessionManager = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        private CswNbtStatistics _CswNbtStatistics = null;
        private HttpContext _Context;

        public CswNbtSessionResources( HttpContext Context, string LoginAccessId, SetupMode SetupMode )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
            _Context = Context;
            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtWeb );

            CswDbCfgInfo CswDbCfgInfo = new CswDbCfgInfo( SetupMode.NbtWeb );
            CswResourcesMaster = new CswResources( AppType.Nbt, SetupVbls, CswDbCfgInfo, false, new CswSuperCycleCacheDefault(), null );
            CswResourcesMaster.SetDbResources( PooledConnectionState.Open );
            CswResourcesMaster.AccessId = CswDbCfgInfo.MasterAccessId;

            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode, true, false,
                                                                          CswResourcesMaster: CswResourcesMaster,
                                                                          CswLogger: CswResourcesMaster.CswLogger );


            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswNbtResources.SetupVbls.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswNbtResources.SetupVbls[RecordStatisticsVblName] );
            }


            CswSessionManager = new CswSessionManager( AppType.Nbt,
                                                       new CswWebCookies( _Context.Request, _Context.Response ),
                                                       LoginAccessId,
                                                       CswNbtResources.SetupVbls,
                                                       CswNbtResources.CswDbCfgInfo,
                                                       false,
                                                       CswNbtResources,
                                                       CswResourcesMaster,
                                                       new CswNbtSchemaAuthenticator( CswNbtResources ),
                                                       _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ),
                                                                                                  RecordStatistics ) );
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += OnDeauthenticate;

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()

        public static CswNbtSessionResources initResources( HttpContext Context )
        {
            CswNbtSessionResources Ret = new CswNbtSessionResources( Context, string.Empty, SetupMode.NbtWeb );
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
                    //    CswNbtSessionResources.CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
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


    }//CswNbtSessionResources

}//ChemSW.Nbt
