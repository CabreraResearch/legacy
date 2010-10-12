using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Session;


namespace ChemSW.Nbt.Statistics
{
    public class CswNbtStatistics : ICswStatisticsStorage
    {

        private CswNbtStatisticsStorageDb _CswNbtStatisticsStorageDb = null;
        private CswNbtStatisticsStorageStateServer _CswNbtStatisticsStorageStateServer = null;
        private bool _RecordStatistics = false; 
        public CswNbtStatistics( CswNbtStatisticsStorageDb CswNbtStatisticsStorageDb, CswNbtStatisticsStorageStateServer CswNbtStatisticsStorageStateServer , bool RecordStatistics )
        {
            _RecordStatistics = RecordStatistics;

                _CswNbtStatisticsStorageDb = CswNbtStatisticsStorageDb;
                _CswNbtStatisticsStorageStateServer = CswNbtStatisticsStorageStateServer;
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
                return ( _CswNbtStatisticsEvents  );
            }
        }

        #region ICswStatisticsStorage Members

        public void OnMakeNewSession( CswSessionsListEntry CswSessionsListEntry )
        {
            if ( _RecordStatistics )
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
            if ( _RecordStatistics )
            {


                if ( null != _CswNbtStatisticsStorageStateServer.CswNbtStatisticsEntry )
                {
                    _CswNbtStatisticsEvents.CswNbtStatisticsEntry = _CswNbtStatisticsStorageStateServer.CswNbtStatisticsEntry;
                }
                else
                {
                    _CswNbtStatisticsEvents.CswNbtStatisticsEntry.SessionId = SessionId;
                    _CswNbtStatisticsEvents.CswNbtStatisticsEntry.StatisticsDataWasLost = true;
                }
            }

        }

        public void OnEndRequestCycle()
        {
            if ( _RecordStatistics )
            {
                _CswNbtStatisticsStorageStateServer.save( _CswNbtStatisticsEvents.CswNbtStatisticsEntry );
            }
        }

        public void OnEndSession()
        {
            if ( _RecordStatistics )
            {
                _CswNbtStatisticsStorageDb.save( _CswNbtStatisticsEvents.CswNbtStatisticsEntry );
                _CswNbtStatisticsStorageStateServer.remove();
            }
        }

        #endregion



    }//class CswSessionListEntry

}//ChemSW.Session