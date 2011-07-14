using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtUpdtInspection : ICswScheduleLogic
    {
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;



        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.UpdtInspection.ToString() ); }
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrance, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }//doesItemRunNow()

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private string _CompletionMessage = string.Empty;
        public string CompletionMessage
        {
            get { return ( _CompletionMessage ); }
        }


        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( _CswNbtResources );
			_CswNbtResources.AuditContext = "Scheduler Task: Update Inspections";
		}

        //private CswNbtNode _CswNbtNodeGenerator;
        private string _Pending = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Pending );
        private string _Overdue = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Overdue );
        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {

                    List<CswNbtObjClassInspectionDesign> InspectionDesigns = _CswScheduleLogicNodes.getInspectonDesigns();

                    for( Int32 idx = 0; ( idx < InspectionDesigns.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassInspectionDesign CurrentInspectionDesign = InspectionDesigns[idx];

                        DateTime DueDate = CurrentInspectionDesign.Date.DateValue;
                        CswNbtNode GeneratorNode = _CswNbtResources.Nodes.GetNode( CurrentInspectionDesign.Generator.RelatedNodeId );
                        if( null != GeneratorNode &&
                            _Pending == CurrentInspectionDesign.Status.Value &&
                            DateTime.Today >= DueDate &&
                            Tristate.True != CurrentInspectionDesign.IsFuture.Checked )
                        {
                            CurrentInspectionDesign.Status.Value = _Overdue;
                            CurrentInspectionDesign.postChanges( true );
                        }

                        if( LogicRunStatus.Stopping == _LogicRunStatus )
                        {
                            break;
                        }

                    }


                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CompletionMessage = "CswScheduleLogicNbtUpdtInspection::threadCallBack() exception: " + Exception.Message;
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;
                    _CswNbtResources.logError( new CswDniException( _CompletionMessage ) );

                }//catch

            }//if we're not shutting down

        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.LogicRunStatus.Idle;
        }

        public void releaseResources()
        {
            _CswNbtResources.release();
        }


    }//CswScheduleLogicNbtUpdtInspection


}//namespace ChemSW.Nbt.Sched
