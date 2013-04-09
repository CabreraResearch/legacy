using System;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtBatchOps : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.BatchOp ); }
        }

        //Determine the number of batchops that need to run and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = CswNbtBatchManager.getBatchNodeCount( (CswNbtResources) CswResources );
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
        }//initScheduleLogicDetail()

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtBatchManager.runNextBatchOp( CswNbtResources );
                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line
                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message;
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
    }//CswScheduleLogicNbtGenNode

}//namespace ChemSW.Nbt.Sched
