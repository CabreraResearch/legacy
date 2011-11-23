using System;
using System.Web;
using ChemSW.Config;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.Security;
using ChemSW.Session;

namespace ChemSW.Nbt
{
    public class CswSessionResourcesNbt
    {
        public CswNbtResources CswNbtResources = null;
        //private CswNbtMetaDataEvents _CswNbtMetaDataEvents;
        public CswSessionManager CswSessionManager = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        private CswNbtStatistics _CswNbtStatistics = null;

        public CswSessionResourcesNbt( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, HttpContext Context, string LoginAccessId, SetupMode SetupMode )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
            CswSetupVblsNbt SetupVbls = new CswSetupVblsNbt( SetupMode.NbtWeb );
            ICswSuperCycleCache CswSuperCycleCache = null;
            if( SetupVbls.doesSettingExist( "cachemetadata" ) && "1" == SetupVbls["cachemetadata"] )
            {
                CswSuperCycleCache = new CswSuperCycleCacheWeb( Context.Cache );
            }
            else
            {
                CswSuperCycleCache = new CswSuperCycleCacheDefault();
            }


            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode, true, false, CswSuperCycleCache );

            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswNbtResources.SetupVbls.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswNbtResources.SetupVbls[RecordStatisticsVblName] );
            }

            CswSessionManager = new CswSessionManager( AppType.Nbt, 
                                                       new CswWebClientStorageCookies( HttpRequest, HttpResponse ), 
                                                       LoginAccessId,
                                                       CswNbtResources.SetupVbls,
                                                       CswNbtResources.CswDbCfgInfo, 
                                                       true, 
                                                       CswNbtResources, 
                                                       new CswNbtSchemaAuthenticator( CswNbtResources ), 
                                                       _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ), 
                                                                                                  new CswNbtStatisticsStorageStateServer(), 
                                                                                                  RecordStatistics ) );
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += new CswSessionManager.DeathenticationHandler( OnDeauthenticate );

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()


        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }


        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess(false); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate(string SessionId)
        {
            CswNbtResources.SessionDataMgr.removeAllSessionData( SessionId );
        }//OnDeauthenticate()

    }//CswSessionResourcesNbt

}//ChemSW.Nbt
