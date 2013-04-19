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
            get { return ( CswEnumNbtScheduleRuleNames.BatchOp ); }
        }

        //Determine the number of batchops that need to run and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = CswNbtBatchManager.getBatchNodeCount( (CswNbtResources) CswResources );
            return _CswScheduleLogicDetail.LoadCount;
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
        }//initScheduleLogicDetail()

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    CswNbtBatchManager.runNextBatchOp( CswNbtResources );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line
                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

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
    }//CswScheduleLogicNbtGenNode

}//namespace ChemSW.Nbt.Sched
