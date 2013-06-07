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
        private string TempNodestWhere = " where ( created  + 1 ) < sysdate and istemp = '1'";
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

            CswArbitrarySelect CswArbitrarySelectnodes = CswResources.makeCswArbitrarySelect( "expired_nodes_query", "select count(*) as \"cnt\" from nodes " + TempNodestWhere );
            DataTable nodesTable = CswArbitrarySelectnodes.getTable();
            Int32 ExpiredNodesRecordCount = CswConvert.ToInt32( nodesTable.Rows[0]["cnt"] );
            if( ExpiredNodesRecordCount > 0 )
            {
                ReturnVal += ExpiredNodesRecordCount;
                _ExpiredTempNodesExist = true;

            }

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
                        }


                    }//_ExpiredSessionListRecsExist

                    if( _ExpiredTempNodesExist )
                    {

                        CswTableUpdate NodesSelect = CswNbtResources.makeCswTableUpdate( "delete_expired_Nodes_records", "nodes" );
                        DataTable NodesTable = NodesSelect.getTable( TempNodestWhere );
                        foreach( DataRow CurrentRow in NodesTable.Rows )
                        {
                            string CurrentSessionId = CurrentRow["sessionid"].ToString();
                            CswNbtSessionDataMgr.removeAllSessionData( CurrentSessionId );
                        }

                    }//_ExpiredNodesRecsExist


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
