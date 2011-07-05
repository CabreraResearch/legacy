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
        private CswNbtMetaDataEvents _CswNbtMetaDataEvents;
        public CswSessionManager CswSessionManager = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        private CswNbtStatistics _CswNbtStatistics = null;

		public CswSessionResourcesNbt( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, string LoginAccessId, string FilesPath, SetupMode SetupMode )
        {
            CswDbCfgInfoNbt CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode );
            CswSetupVblsNbt CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode );

            CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, FilesPath, true, false );

            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswSetupVblsNbt.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswSetupVblsNbt[RecordStatisticsVblName] );
            }

            CswSessionManager = new CswSessionManager( AppType.Nbt, 
													   new CswWebClientStorageCookies( HttpRequest, HttpResponse ), 
													   LoginAccessId, 
													   CswSetupVblsNbt, 
													   CswDbCfgInfoNbt, 
													   true, 
													   CswNbtResources, 
													   new CswNbtAuthenticator( CswNbtResources ), 
													   _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ), 
																								  new CswNbtStatisticsStorageStateServer(), 
																								  RecordStatistics ) );
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += new CswSessionManager.DeathenticationHandler( OnDeauthenticate );

            CswNbtResources.AccessId = CswSessionManager.AccessId;
        }//ctor()


        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }


        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess(); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void OnDeauthenticate(string SessionId)
        {
			CswNbtResources.SessionDataMgr.removeAllSessionData( SessionId );
		}//OnDeauthenticate()

	}//CswSessionResourcesNbt

}//ChemSW.Nbt
