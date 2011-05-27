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
        //        private CswNbtSession _CswSession = null;
        public CswSessionManager CswSessionManager = null;

        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;

        //private HttpSessionState _HttpSessionState = null;
        //private SessionRetrievalType _SessionRetrievalType = SessionRetrievalType.CrossProcess;


        private CswNbtStatistics _CswNbtStatistics = null;

        private bool _UsedCachedResources = false;
        public CswSessionResourcesNbt( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, string LoginAccessId, string FilesPath, SetupMode SetupMode )
        {
            //_HttpSessionState = HttpSessionState;
            //            _CswSession = new CswNbtSession( HttpApplicationState, HttpSessionState, HttpRequest, HttpResponse );
            CswDbCfgInfoNbt CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode );
            CswSetupVblsNbt CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode );

            //            CswSessionManager = new CswSessionManager(App
            //string AccessId = CswSessionManager.AccessId;
            //if ( string.Empty == AccessId )
            //    throw ( new CswDniException( "There is no AccessId for this session" ) );


            //CswNbtObjClassFactory CswNbtObjClassFactory = new CswNbtObjClassFactory();

            if( CachedResources != null )
            {
                CswNbtResources = CachedResources as CswNbtResources;
                CswNbtResources.AfterRestoreFromCache();
                _UsedCachedResources = true;
            }
            else
            {
                CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, FilesPath, true, false );

                /*
                CswNbtResources = new CswNbtResources( AppType.Nbt, CswSetupVblsNbt, CswDbCfgInfoNbt, //CswNbtObjClassFactory, 
                                                       true, false );
                CswNbtResources.SetDbResources( new CswNbtTreeFactory( FilesPath ) );

                //bz # 9896: This events must only be assigned when we first instance the class;
                //if we also assign them to cached resources, we get duplicate events occuring :-(
                _CswNbtMetaDataEvents = new CswNbtMetaDataEvents( CswNbtResources );
                CswNbtResources.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeType );
                CswNbtResources.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( _CswNbtMetaDataEvents.OnCopyNodeType );
                CswNbtResources.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeTypeProp );
                CswNbtResources.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypePropName );
                CswNbtResources.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( _CswNbtMetaDataEvents.OnDeleteNodeTypeProp );
                CswNbtResources.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypeName );
                 * */

                _UsedCachedResources = false;
            }


            string RecordStatisticsVblName = "RecordUserStatistics";
            bool RecordStatistics = false;
            if( CswSetupVblsNbt.doesSettingExist( RecordStatisticsVblName ) )
            {
                RecordStatistics = ( "1" == CswSetupVblsNbt[RecordStatisticsVblName] );
            }


            //System.Enum.TryParse<SessionRetrievalType>( CswSetupVblsNbt.readSetting( "SessionRetrievalType" ), true, out _SessionRetrievalType );
            //ICswSessionStorage SoCalledFastSessionStorage = SessionRetrievalType.Optimized == _SessionRetrievalType ? (ICswSessionStorage) new CswSessionStorageStateServer( _HttpSessionState ) : (ICswSessionStorage) new CswSessionStorageStateServerDummy();


            CswSessionManager = new CswSessionManager( AppType.Nbt, new CswWebClientStorageCookies( HttpRequest, HttpResponse ), LoginAccessId, CswSetupVblsNbt, CswDbCfgInfoNbt, true, _UsedCachedResources, CswNbtResources, new CswNbtAuthenticator( CswNbtResources ), _CswNbtStatistics = new CswNbtStatistics( new CswNbtStatisticsStorageDb( CswNbtResources ), new CswNbtStatisticsStorageStateServer(), RecordStatistics ) );
            CswNbtStatisticsEvents = _CswNbtStatistics.CswNbtStatisticsEvents;
            CswSessionManager.OnDeauthenticate += new CswSessionManager.DeathenticationHandler( OnDeauthenticate );


            CswNbtResources.AccessId = CswSessionManager.AccessId;
            //            CswNbtResources = CswNbtResources;



            //, new CswAuthenticator.AuthenticationHandler( _CswSession.OnAfterLogin ), new CswAuthenticator.AuthenticationHandler( _CswSession.OnBeforeLogout ) 
        }//ctor()


        public AuthenticationStatus AuthenticationStatus { get { return ( CswSessionManager.AuthenticationStatus ); } }


        public AuthenticationStatus attemptRefresh() { return ( CswSessionManager.attemptRefresh() ); }
        public void endSession() { CswSessionManager.updateLastAccess(); }

        public void purgeExpiredSessions() { CswSessionManager.SessionsList.purgeExpiredSessions(); }

        public void setCache()
        {
            //if( SessionRetrievalType.Optimized == _SessionRetrievalType )
            //{
            //    if( _HttpSessionState != null )
            //    {
            //        if( "1" == CswNbtResources.SetupVbls["cachemetadata"] )
            //        {
            //            if( ( CswNbtResources != null ) && ( false == _CacheCleared ) )
            //            {
            //                //Session[ "ViewCache" ] = CswNbtResources.ViewCache.ToString();
            //                CswNbtResources.BeforeStoreInCache();
            //                _HttpSessionState["CswNbtResources"] = CswNbtResources;
            //            }
            //        }
            //        else
            //        {
            //            _HttpSessionState["CswNbtResources"] = null;
            //        }
            //    }
            //}//

        }//setCache()

        private bool _CacheCleared = false;
        public void OnDeauthenticate()
        {

            //if( SessionRetrievalType.Optimized == _SessionRetrievalType )
            //{

            //    if( null != _HttpSessionState && null != _HttpSessionState["CswNbtResources"] )
            //    {
            //        _HttpSessionState.Remove( "CswNbtResources" );
            //        _CacheCleared = true;

            //        // BZ 9932,10341,10342,10266
            //        _HttpSessionState.Clear();

            //    }
            //}

            ////bz # 9932 but cf. also 10266
            //List<string> KeysToRemove = new List<string>(); 
            //foreach ( string CurrentKey in _HttpSessionState.Keys )
            //{
            //    if ( CurrentKey.Contains( "PreviousView" ) )
            //    {
            //        KeysToRemove.Add( CurrentKey ); 
            //    }
            //}

            //foreach ( string CurrentKeyToRemove in KeysToRemove )
            //{
            //    _HttpSessionState.Remove( CurrentKeyToRemove ); 
            //}


        }//clearCache()

        public ICswResources CachedResources
        {
            get
            {
                ICswResources ret = null;

                //if( null != _HttpSessionState && null != _HttpSessionState["CswNbtResources"] )
                //{

                //    if( ( null != _HttpSessionState ) && ( _HttpSessionState["CswNbtResources"] != null ) )
                //        ret = _HttpSessionState["CswNbtResources"] as ICswResources;
                //}

                return ret;
            }
        }

        //public void setUserResources()
        //{

        //    CswSessionManager.setUserResources();


        //}//


    }//CswInitialization

}//ChemSW.Nbt
