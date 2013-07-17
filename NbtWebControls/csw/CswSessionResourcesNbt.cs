using ChemSW.Config;
using ChemSW.Nbt.csw.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.Security;
using ChemSW.Session;
using System;
using System.Collections.Generic;
using System.Web;


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

        public CswSessionResourcesNbt( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, HttpContext Context, string LoginAccessId, CswEnumSetupMode SetupMode )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
            CswSetupVbls SetupVbls = new CswSetupVbls( CswEnumSetupMode.NbtWeb );

            ICswSuperCycleCache CswSuperCycleCache = new CswSuperCycleCacheWeb( Context.Cache );
            // Set the cache to drop anything 10 minutes old
            CswSuperCycleCache.CacheDirtyThreshold = DateTime.Now.Subtract( new TimeSpan( 0, 10, 0 ) );


            CswDbCfgInfo CswDbCfgInfo = new CswDbCfgInfo( CswEnumSetupMode.NbtWeb );
            CswResourcesMaster = new CswResources( CswEnumAppType.Nbt, SetupVbls, CswDbCfgInfo, false, new CswSuperCycleCacheDefault(), null );
            CswResourcesMaster.SetDbResources( ChemSW.RscAdo.CswEnumPooledConnectionState.Open );
            CswResourcesMaster.AccessId = CswDbCfgInfo.MasterAccessId;

            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, SetupMode, true, false, CswSuperCycleCache, RscAdo.CswEnumPooledConnectionState.Open, CswResourcesMaster, CswResourcesMaster.CswLogger );



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

            CswNbtSchemaAuthenticatorFactory AuthenticatorFactory = new CswNbtSchemaAuthenticatorFactory( CswNbtResources );
            ICswSchemaAuthenticater Authenticator = AuthenticatorFactory.Make( CswNbtResources.SetupVbls );

            CswSessionManager = new CswSessionManager( CswEnumAppType.Nbt,
                                                       new CswWebClientStorageCookies( HttpRequest, HttpResponse ),
                                                       LoginAccessId,
                                                       CswNbtResources.SetupVbls,
                                                       CswNbtResources.CswDbCfgInfo,
                                                       false,
                                                       CswNbtResources,
                                                       CswResourcesMaster,
                                                       Authenticator,
                                                       Cookies,
                                                       _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ),
                                                                                                  new CswNbtStatisticsStorageStateServer(),
                                                                                                  RecordStatistics ) );
            CswNbtResources.CswSessionManager = CswSessionManager;
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;

            CswSessionManager.OnDeauthenticate += new CswSessionManager.DeathenticationHandler( OnDeauthenticate );

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()




        public CswEnumAuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }


        public CswEnumAuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess( false ); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate( string SessionId )
        {
            //CAUTION: don't do anything to this method without giving due consideration 
            //to the CAUTION noted in CswSessionManager::clearSession()
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
