using ChemSW.Nbt.Statistics;
using ChemSW.Session;

namespace NbtWebAppServices.Session
{
    public class CswNbtStatistics : ICswStatisticsStorage
    {
        private CswNbtStatisticsStorageDb _CswNbtStatisticsStorageDb = null;
        private bool _RecordStatistics = false;
        public CswNbtStatistics( CswNbtStatisticsStorageDb CswNbtStatisticsStorageDb, bool RecordStatistics )
        {
            _RecordStatistics = RecordStatistics;

            _CswNbtStatisticsStorageDb = CswNbtStatisticsStorageDb;
            _CswNbtStatisticsEvents = new CswNbtStatisticsEvents( RecordStatistics );
            _CswNbtStatisticsEvents.CswNbtStatisticsEntry = new CswNbtStatisticsEntry( "" );
            //_CswNbtStatisticsEvents.CswNbtStatisticsEntry = _CswNbtStatisticsEvents.CswNbtStatisticsEntry;

        }//ctor

        //        private CswNbtStatisticsEntry _CswNbtStatisticsEvents.CswNbtStatisticsEntry = null;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents
        {
            get
            {
                return ( _CswNbtStatisticsEvents );
            }
        }

        #region ICswStatisticsStorage Members

        public void OnMakeNewSession( CswSessionsListEntry CswSessionsListEntry )
        {
            if( _RecordStatistics )
            {


                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.SessionId = CswSessionsListEntry.SessionId;
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.UserId = CswSessionsListEntry.UserId;
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.UserName = CswSessionsListEntry.UserName;
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.SessionId = CswSessionsListEntry.SessionId;
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.AccessId = CswSessionsListEntry.AccessId;
            }
        }

        public void OnLoadExistingSession( string SessionId )
        {
            if( _RecordStatistics )
            {
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.SessionId = SessionId;
                _CswNbtStatisticsEvents.CswNbtStatisticsEntry.StatisticsDataWasLost = true;
            }
        }

        public void OnEndRequestCycle()
        {
        }

        public void OnEndSession()
        {
            if( _RecordStatistics )
            {
                _CswNbtStatisticsStorageDb.save( _CswNbtStatisticsEvents.CswNbtStatisticsEntry );
            }
        }

        #endregion

    }//class CswSessionListEntry

}//ChemSW.CswSession