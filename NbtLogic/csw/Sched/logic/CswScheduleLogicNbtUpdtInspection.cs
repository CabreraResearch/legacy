using System;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtUpdtInspection : ICswScheduleLogic
    {
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.UpdtInspection ); }
        }

        //Determine the number of inspections to update and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( ( CswNbtResources ) CswResources );
            Collection<CswNbtObjClassInspectionDesign> InspectionDesigns = _CswScheduleLogicNodes.getInspectonDesigns();
            return InspectionDesigns.Count;
        }

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

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        private string _Overdue = CswEnumNbtInspectionStatus.Overdue;
        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    Int32 TotalProcessed = 0;
                    string Names = string.Empty;

                    Collection<CswNbtObjClassInspectionDesign> InspectionDesigns = _CswScheduleLogicNodes.getInspectonDesigns();

                    for( Int32 idx = 0; ( idx < InspectionDesigns.Count ) && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassInspectionDesign CurrentInspectionDesign = InspectionDesigns[idx];
                        CurrentInspectionDesign.Status.Value = _Overdue;
                        CurrentInspectionDesign.postChanges( ForceUpdate: true );

                        TotalProcessed++;
                        Names += CurrentInspectionDesign.Name + "; ";
                    }

                    _CswScheduleLogicDetail.StatusMessage = TotalProcessed.ToString() + " inspections processed: " + Names;
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtInspection::threadCallBack() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

            _CswScheduleLogicDetail.StatusMessage = "Completed without error";

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtUpdtInspection

}//namespace ChemSW.Nbt.Sched
