using System.Web;
using ChemSW;
using ChemSW.Config;
using ChemSW.Nbt;
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

        public CswNbtSessionResources( HttpContext Context, string LoginAccessId, SetupMode SetupMode )
        {

            //SuperCycleCache configuraiton has to happen here because here is where we can stash the cache,
            //so to speak, in the wrapper class -- the resource factory is agnostic with respect to cache type
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
                                                       new CswWebCookies( Context.Request, Context.Response ),
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

        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }

        public static CswNbtSessionResources initResources( HttpContext Context )
        {
            CswNbtSessionResources Ret = new CswNbtSessionResources( Context, string.Empty, SetupMode.NbtWeb );
            Ret.CswNbtResources.beginTransaction();
            Ret.CswNbtResources.logMessage( "WebServices: CswSession Started (_initResources called)" );
            return Ret;
        }

        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess( false ); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate( string SessionId )
        {
            CswNbtResources.SessionDataMgr.removeAllSessionData( SessionId );
        }//OnDeauthenticate()

    }//CswNbtSessionResources

}//ChemSW.Nbt
