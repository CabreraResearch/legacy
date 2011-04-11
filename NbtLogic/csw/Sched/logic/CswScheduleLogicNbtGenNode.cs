using System;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;


namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenNode : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenNode.ToString() ); }
        }

        CswNbtNode _CswNbtNodeGenerator = null;
        public bool doesItemRunNow()
        {
            bool ReturnVal = _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrance, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow();

            if( ReturnVal )
            {

                _CswNbtNodeGenerator = null; //MUST BE QUIERED FOR ; USED TO PASSED IN TO CTOR
                CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( _CswNbtNodeGenerator );
                if( GeneratorNode.Enabled.Checked == Tristate.True )
                {
                    DateTime ThisDueDateValue = GeneratorNode.NextDueDate.DateValue.Date;
                    DateTime InitialDueDateValue = GeneratorNode.DueDateInterval.getStartDate().Date;
                    DateTime FinalDueDateValue = GeneratorNode.FinalDueDate.DateValue.Date;

                    // BZ 7866
                    if( ThisDueDateValue != DateTime.MinValue )
                    {
                        // BZ 7124 - set runtime
                        if( GeneratorNode.RunTime.TimeValue != DateTime.MinValue )
                            ThisDueDateValue = ThisDueDateValue.AddTicks( GeneratorNode.RunTime.TimeValue.TimeOfDay.Ticks );

                        Int32 WarnDays = (Int32) GeneratorNode.WarningDays.Value;
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
                            ReturnVal = true;
                        }
                    } // if( ThisDueDateValue != DateTime.MinValue )
                } // if( GeneratorNode.Enabled.Checked == Tristate.True )

            }

            return ( ReturnVal );
        }

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
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        private CswNbtResources _CswNbtResources = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {

                    CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( _CswNbtResources );
                    CswNbtActGenerateNodes.makeNode( _CswNbtNodeGenerator );

                    // EQUIVALENT OF THIS EVENT NEEDS TO BE RUN HERE
                    /*
                    if( null != OnScheduleItemWasRun )
                    {
                        OnScheduleItemWasRun( this, _CswNbtNodeGenerator );
                    }


            public void handleOnSchdItemWasRun( CswNbtSchdItem CswNbtSchdItem, CswNbtNode CswNbtSchedulerNode )
            {
                ICswNbtPropertySetScheduler SchedulerNode = CswNbtNodeCaster.AsPropertySetScheduler( CswNbtSchedulerNode );
                SchedulerNode.RunStatus.StaticText = CswNbtSchdItem.StatusMessage;
                if( CswNbtSchdItem.Succeeded )
                {
                    DateTime CandidateNextDueDate = SchedulerNode.DueDateInterval.getNextOccuranceAfter( SchedulerNode.NextDueDate.DateValue );
                    if( SchedulerNode.FinalDueDate.DateValue.Date != DateTime.MinValue &&
                        ( SchedulerNode.FinalDueDate.DateValue.Date < DateTime.Now.Date ||
                          CandidateNextDueDate > SchedulerNode.FinalDueDate.DateValue.Date ) )
                    {
                        CandidateNextDueDate = DateTime.MinValue;
                    }
                    SchedulerNode.NextDueDate.DateValue = CandidateNextDueDate;
                }
                CswNbtSchedulerNode.postChanges( false );

            }//handleOnSchdItemWasRun
                     */


                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CompletionMessage = "Csw3ETasks::GetUpdatedItems() exception: " + Exception.Message;
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

    }//CswScheduleLogicNbtGenNode


}//namespace ChemSW.Nbt.Sched
