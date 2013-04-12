using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtUpdtMTBF : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.UpdtMTBF ); }
        }

        //In the case where the rule always has 'work' to do, the rule should only have load when the rule is scheduled to run.
        //This is necessary to stop the rule from running once it has completed its job.
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = _CswScheduleLogicDetail.doesItemRunNow() ? 1 : 0;
            return _CswScheduleLogicDetail.LoadCount;
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
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
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            //CswNbtResources.AuditContext = "Scheduler Task: Update MTBF";
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    // BZ 6779
                    // Set all MTBF fields pendingupdate = 1
                    Int32 MTBFId = CswNbtResources.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId;

                    CswTableSelect NTPSelect = CswNbtResources.makeCswTableSelect( "UpdateMTBF_NTP_Select", "nodetype_props" );
                    DataTable NTPTable = NTPSelect.getTable( "fieldtypeid", MTBFId );
                    string NTPIds = string.Empty;
                    foreach( DataRow NTPRow in NTPTable.Rows )
                    {
                        if( NTPIds != string.Empty ) NTPIds += ",";
                        NTPIds += CswConvert.ToInt32( NTPRow["nodetypepropid"] );
                    }

                    if( NTPIds != string.Empty )
                    {
                        CswTableUpdate JNPUpdate = CswNbtResources.makeCswTableUpdate( "UpdateMTBF_JNP_Update", "jct_nodes_props" );
                        DataTable JNPTable = JNPUpdate.getTable( "where nodetypepropid in (" + NTPIds + ")" );
                        foreach( DataRow JNPRow in JNPTable.Rows )
                        {
                            JNPRow["pendingupdate"] = CswConvert.ToDbVal( true );
                        }
                        JNPUpdate.update( JNPTable );
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtMTBF::GetUpdatedItems() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtUpdtMTBF

}//namespace ChemSW.Nbt.Sched
