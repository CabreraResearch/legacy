using System;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtTierII : ICswScheduleLogic
    {
        #region Properties

        private CswNbtResources _CswNbtResources;
        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.ExpiredContainers.ToString() ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetailIn )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetailIn;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;
            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        _CswNbtResources.execStoredProc("TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>());
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                        _LogicRunStatus = LogicRunStatus.Succeeded;
                    }
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtTierII exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtTierII
}//namespace ChemSW.Nbt.Sched
