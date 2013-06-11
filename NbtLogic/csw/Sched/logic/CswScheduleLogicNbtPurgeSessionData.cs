using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtPurgeSessionData : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.PurgeSessionData ); }
        }

        private bool _ExpiredSessionListRecsExist = false;
        private bool _ExpiredTempNodesExist = false;
        private string SessionListWhere = " where timeoutdate < sysdate";
        public Int32 getLoadCount( ICswResources CswResources )
        {

            Int32 ReturnVal = 0;

            CswArbitrarySelect CswArbitrarySelectSessionList = CswResources.makeCswArbitrarySelect( "expired_session_list_query", "select count(*) as \"cnt\" from sessionlist " + SessionListWhere );
            DataTable SessionListTable = CswArbitrarySelectSessionList.getTable();
            Int32 ExpiredSessionRecordCount = CswConvert.ToInt32( SessionListTable.Rows[0]["cnt"] );
            if( ExpiredSessionRecordCount > 0 )
            {

                ReturnVal = ExpiredSessionRecordCount;
                _ExpiredSessionListRecsExist = true;
            }

            _CswScheduleLogicDetail.LoadCount = ReturnVal;

            return ( ReturnVal );
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtSessionDataMgr CswNbtSessionDataMgr = new CswNbtSessionDataMgr( CswNbtResources );

                    if( _ExpiredSessionListRecsExist )
                    {
                        CswTableSelect SessionListSelect = CswNbtResources.makeCswTableSelect( "delete_expired_sessionlist_records", "sessionlist" );
                        DataTable SessionListTable = SessionListSelect.getTable( SessionListWhere );

                        foreach( DataRow CurrentRow in SessionListTable.Rows )
                        {
                            string CurrentSessionId = CurrentRow["sessionid"].ToString();
                            CswNbtSessionDataMgr.removeAllSessionData( CurrentSessionId );

                            CswTableUpdate CswTableUpdateSessionList = CswNbtResources.makeCswTableUpdate( "session_list_delete", "sessionlist" );
                            DataTable SessionListUpdateTable = CswTableUpdateSessionList.getTable( " where sessionid = '" + CurrentSessionId + "'" );
                            if( SessionListUpdateTable.Rows.Count > 0  )
                            {
                                SessionListUpdateTable.Rows[0].Delete();
                                CswTableUpdateSessionList.update( SessionListUpdateTable );
                            }
                            
                        }//iterate session records

                    }//_ExpiredSessionListRecsExist

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtPurgeSessionData exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

                finally
                {
                    _ExpiredSessionListRecsExist = false;
                    _ExpiredTempNodesExist = false;

                }

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtPurgeSessionData

}//namespace ChemSW.Nbt.Sched
