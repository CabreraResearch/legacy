using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenNode : ICswScheduleLogic
    {
        private Int32 _GeneratorLimit = 1;

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenNode.ToString() ); }
        }

        public bool doesItemRunNow()
        {
            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );
        }

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswScheduleNodeUpdater _CswScheduleNodeUpdater = null;
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( _CswNbtResources );
            _CswScheduleNodeUpdater = new CswScheduleNodeUpdater( _CswNbtResources );
            _CswNbtResources.AuditContext = "Scheduler Task: Generate Nodes";

        }//init()

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    
                    List<CswNbtObjClassGenerator> ObjectGenerators = _CswScheduleLogicNodes.getGenerators();

                    for( Int32 idx = 0; ( idx < ObjectGenerators.Count && idx < _GeneratorLimit ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassGenerator CurrentGenerator = ObjectGenerators[idx];
                        if( CurrentGenerator.Enabled.Checked == Tristate.True )
                        {
                            DateTime ThisDueDateValue = CurrentGenerator.NextDueDate.DateTimeValue.Date;
                            DateTime InitialDueDateValue = CurrentGenerator.DueDateInterval.getStartDate().Date;
                            if( InitialDueDateValue == DateTime.MinValue )
                            {
                                InitialDueDateValue = ThisDueDateValue;
                            }
                            DateTime FinalDueDateValue = CurrentGenerator.FinalDueDate.DateTimeValue.Date;

                            // BZ 7866
                            if( ThisDueDateValue != DateTime.MinValue )
                            {
                                // BZ 7124 - set runtime
                                if( CurrentGenerator.RunTime.DateTimeValue != DateTime.MinValue )
                                {
                                    ThisDueDateValue = ThisDueDateValue.AddTicks( CurrentGenerator.RunTime.DateTimeValue.TimeOfDay.Ticks );
                                }

                                Int32 WarnDays = CswConvert.ToInt32( CurrentGenerator.WarningDays.Value );
                                if( WarnDays > 0 )
                                {
                                    TimeSpan WarningDaysSpan = new TimeSpan( WarnDays, 0, 0, 0, 0 );
                                    ThisDueDateValue = ThisDueDateValue.Subtract( WarningDaysSpan );
                                    InitialDueDateValue = InitialDueDateValue.Subtract( WarningDaysSpan );
                                }

                                // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                                if( ( DateTime.Now.Date >= InitialDueDateValue ) &&
                                    ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                                    ( DateTime.Now >= ThisDueDateValue ) )
                                {
                                    string Message = _runGenerator( CurrentGenerator );
                                    _CswScheduleNodeUpdater.update( CurrentGenerator.Node, Message );
                                }
                            } // if( ThisDueDateValue != DateTime.MinValue )

                        } // if( CurrentGenerator.Enabled.Checked == Tristate.True )

                    }//iterate generators

                    _LogicRunStatus = LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenNode::GetUpdatedItems() exception: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = LogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

            _CswScheduleLogicDetail.StatusMessage = "Completed without error";

        }//threadCallBack()

        private string _runGenerator( CswNbtObjClassGenerator CurrentGenerator )
        {
            string RetMessage;
            CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( _CswNbtResources );
            Int32 NodesCreated = CswNbtActGenerateNodes.makeNode( CurrentGenerator.Node );
            if( NodesCreated > 0 )
            {
                RetMessage = "Created " + NodesCreated.ToString() + " " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " target(s) from: " + CurrentGenerator.Node.NodeName + ", " + CurrentGenerator.Node.getNodeType().NodeTypeName;
            }
            else
            {
                RetMessage = "No " + CurrentGenerator.TargetType.SelectedNodeTypeNames() + " targets created from: " + CurrentGenerator.Node.NodeName + ", " + CurrentGenerator.Node.getNodeType().NodeTypeName;
            }
            return RetMessage;
        }

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

    }//CswScheduleLogicNbtGenNode


}//namespace ChemSW.Nbt.Sched
