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

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
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
            get { return ( NbtScheduleRuleNames.TierII ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //This rule always has 'work' to do, but the stored procedure won't do anything if it already ran in the same day
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = 1;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = LogicRunStatus.Idle;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                    {
                        CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                        _LogicRunStatus = LogicRunStatus.Succeeded;
                    }
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtTierII exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtTierII
}//namespace ChemSW.Nbt.Sched
