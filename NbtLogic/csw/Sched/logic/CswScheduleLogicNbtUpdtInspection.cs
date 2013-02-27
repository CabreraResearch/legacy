using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtInspection : ICswScheduleLogic
    {
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;



        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.UpdtInspection.ToString() ); }
        }

        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
        }//hasLoad()

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


        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();

        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
        }


        //private CswNbtNode _CswNbtNodeGenerator;
        private string _Pending = CswNbtObjClassInspectionDesign.InspectionStatus.Pending;
        private string _Overdue = CswNbtObjClassInspectionDesign.InspectionStatus.Overdue;
        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );
            //CswNbtResources.AuditContext = "Scheduler Task: Update Inspections";
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {

                    Int32 TotalProcessed = 0;
                    string Names = string.Empty;

                    Collection<CswNbtObjClassInspectionDesign> InspectionDesigns = _CswScheduleLogicNodes.getInspectonDesigns();

                    for( Int32 idx = 0; ( idx < InspectionDesigns.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassInspectionDesign CurrentInspectionDesign = InspectionDesigns[idx];

                        DateTime DueDate = CurrentInspectionDesign.DueDate.DateTimeValue;
                        //CswNbtNode GeneratorNode = CswNbtResources.Nodes.GetNode( CurrentInspectionDesign.Generator.RelatedNodeId );
                        //if( null != GeneratorNode &&
                        if( _Pending == CurrentInspectionDesign.Status.Value &&
                            DateTime.Today >= DueDate &&
                            Tristate.True != CurrentInspectionDesign.IsFuture.Checked )
                        {
                            CurrentInspectionDesign.Status.Value = _Overdue;
                            CurrentInspectionDesign.postChanges( ForceUpdate: true );

                            TotalProcessed++;
                            Names += CurrentInspectionDesign.Name + "; ";
                        }

                        if( LogicRunStatus.Stopping == _LogicRunStatus )
                        {
                            break;
                        }

                    }

                    _CswScheduleLogicDetail.StatusMessage = TotalProcessed.ToString() + " inspections processed: " + Names;
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtUpdtInspection::threadCallBack() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;

                }//catch

            }//if we're not shutting down

            _CswScheduleLogicDetail.StatusMessage = "Completed without error";


        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.LogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtUpdtInspection


}//namespace ChemSW.Nbt.Sched
