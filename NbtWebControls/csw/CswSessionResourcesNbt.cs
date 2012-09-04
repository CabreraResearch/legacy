using System;
using System.Collections.Generic;
using System.Web;
using ChemSW.Config;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.Security;
using ChemSW.Session;


namespace ChemSW.Nbt
{
    public class CswSessionResourcesNbt
    {
        public CswNbtResources CswNbtResources = null;
        public CswResources CswResourcesMaster = null;
        //private CswNbtMetaDataEvents _CswNbtMetaDataEvents;
        public CswSessionManager CswSessionManager = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        private CswNbtStatistics _CswNbtStatistics = null;

        public CswSessionResourcesNbt( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, HttpContext Context, string LoginAccessId, SetupMode SetupMode )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtWeb );

            ICswSuperCycleCache CswSuperCycleCache = new CswSuperCycleCacheWeb( Context.Cache );
            // Set the cache to drop anything 10 minutes old
            CswSuperCycleCache.CacheDirtyThreshold = DateTime.Now.Subtract( new TimeSpan( 0, 10, 0 ) );


            CswDbCfgInfo CswDbCfgInfo = new CswDbCfgInfo( SetupMode.NbtWeb, IsMobile: false );
            CswResourcesMaster = new CswResources( AppType.Nbt, SetupVbls, CswDbCfgInfo, false, new CswSuperCycleCacheDefault(), null );
            CswResourcesMaster.SetDbResources( ChemSW.RscAdo.PooledConnectionState.Open );
            CswResourcesMaster.AccessId = CswDbCfgInfo.MasterAccessId;

            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode, true, false, CswSuperCycleCache, RscAdo.PooledConnectionState.Open, CswResourcesMaster, CswResourcesMaster.CswLogger );



            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswNbtResources.SetupVbls.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswNbtResources.SetupVbls[RecordStatisticsVblName] );
            }

            Dictionary<string, string> Cookies = new Dictionary<string, string>();
            foreach( string CookieName in Context.Request.Cookies )
            {
                Cookies[CookieName] = Context.Request.Cookies[CookieName].Value;
            }

            CswSessionManager = new CswSessionManager( AppType.Nbt,
                                                       new CswWebClientStorageCookies( HttpRequest, HttpResponse ),
                                                       LoginAccessId,
                                                       CswNbtResources.SetupVbls,
                                                       CswNbtResources.CswDbCfgInfo,
                                                       false,
                                                       CswNbtResources,
                                                       CswResourcesMaster,
                                                       new CswNbtSchemaAuthenticator( CswNbtResources ),
                                                       Cookies,
                                                       _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ),
                                                                                                  new CswNbtStatisticsStorageStateServer(),
                                                                                                  RecordStatistics ) );
            CswNbtResources.CswSessionManager = CswSessionManager;
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += new CswSessionManager.DeathenticationHandler( OnDeauthenticate );

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()




        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }


        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess( false ); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate( string SessionId )
        {
            CswNbtResources.SessionDataMgr.removeAllSessionData( SessionId );
        }//OnDeauthenticate()

        public void finalize()
        {
            if( null != CswNbtResources )
            {
                CswNbtResources.finalize();
            }
            if( null != CswResourcesMaster )
            {
                CswResourcesMaster.finalize( true );
            }
        }

        public void release()
        {
            if( null != CswNbtResources )
            {
                CswNbtResources.release();
            }
            if( null != CswResourcesMaster )
            {
                CswResourcesMaster.release();
            }
        }

    }//CswSessionResourcesNbt

}//ChemSW.Nbt
