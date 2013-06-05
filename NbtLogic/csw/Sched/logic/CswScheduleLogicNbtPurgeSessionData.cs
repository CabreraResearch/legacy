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
        private string SessionListWhere = " where ( sysdate + 1 ) > timeoutdate";
        private string TempNodestWhere = " where ( sysdate + 1 ) > created and istemp = '1'";
        public Int32 getLoadCount( ICswResources CswResources )
        {

            Int32 ReturnVal = 0;

            CswArbitrarySelect CswArbitrarySelectSessionList = CswResources.makeCswArbitrarySelect( "expired_session_list_query", "select count(*) from sessionlist " + SessionListWhere );
            DataTable SessionListTable = CswArbitrarySelectSessionList.getTable();
            if( SessionListTable.Rows.Count > 0 )
            {
                ReturnVal = CswConvert.ToInt32( SessionListTable.Rows[0][0] );
                _ExpiredSessionListRecsExist = true;
            }

            CswArbitrarySelect CswArbitrarySelectnodes = CswResources.makeCswArbitrarySelect( "expired_nodes_query", "select count(*) from nodes " + TempNodestWhere );
            DataTable nodesTable = CswArbitrarySelectnodes.getTable();
            if( nodesTable.Rows.Count > 0 )
            {
                ReturnVal += CswConvert.ToInt32( nodesTable.Rows[0][0] );
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

            //CswNbtResources.AuditContext = "Scheduler Task: Update MTBF";
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _ExpiredSessionListRecsExist )
                    {
                        CswTableUpdate SessionDataUpdate = CswNbtResources.makeCswTableUpdate( "delete_expired_session_data_records", "session_data" );
                        DataTable SessionDataTable = SessionDataUpdate.getTable( " where sessionid in ( select sessionid from sessionlist " + SessionListWhere + " )" );
                        if( SessionDataTable.Rows.Count > 0 ) //in theory there could be no session_data records even thugh we know there are sessionlist records
                        {
                            foreach (DataRow CurrentRow in SessionDataTable.Rows)
                            {
                                CurrentRow.Delete();
                            }
                            SessionDataUpdate.update(SessionDataTable);
                        }

                        CswTableUpdate SessionListUpdate = CswNbtResources.makeCswTableUpdate( "delete_expired_sessionlist_records", "sessionlist" );
                        DataTable SessionListTable = SessionListUpdate.getTable( SessionListWhere );

                        foreach( DataRow CurrentRow in SessionListTable.Rows )
                        {
                            CurrentRow.Delete();
                        }
                        SessionListUpdate.update( SessionListTable );


                    }//_ExpiredSessionListRecsExist

                    if( _ExpiredTempNodesExist )
                    {

                        CswTableUpdate JctNodePropsUpdate = CswNbtResources.makeCswTableUpdate( "delete_expired_jct_nodes_props_records", "jct_nodes_props" );
                        DataTable JctNodesPropsTable = JctNodePropsUpdate.getTable( "where nodeid in (select nodeid from nodes " + TempNodestWhere + ") " );
                        if( JctNodesPropsTable.Rows.Count > 0 ) //in theory there could no jct_nodes_props records even though we know there are expired node temp node records
                        {
                            foreach (DataRow CurrentRow in JctNodesPropsTable.Rows)
                            {
                                CurrentRow.Delete();
                            }
                            JctNodePropsUpdate.update(JctNodesPropsTable);
                        }


                        CswTableUpdate NodesUpdate = CswNbtResources.makeCswTableUpdate( "delete_expired_Nodes_records", "nodes" );
                        DataTable NodesTable = NodesUpdate.getTable( TempNodestWhere );
                        foreach( DataRow CurrentRow in NodesTable.Rows )
                        {
                            CurrentRow.Delete();
                        }
                        NodesUpdate.update( NodesTable );
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
