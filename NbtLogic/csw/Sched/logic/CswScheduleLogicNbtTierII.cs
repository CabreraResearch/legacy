using System;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtTierII : ICswScheduleLogic
    {
        #region Properties

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.TierII ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //In the case where the rule always has 'work' to do, the rule should only have load when the rule is scheduled to run.
        //This is necessary to stop the rule from running once it has completed its job.
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = _CswScheduleLogicDetail.doesItemRunNow() ? 1 : 0;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
                    {
                        CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                        _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                    }
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtTierII exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtTierII
}//namespace ChemSW.Nbt.Sched
