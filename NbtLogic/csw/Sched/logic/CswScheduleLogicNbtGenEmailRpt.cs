using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenEmailRpt : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenEmailRpt.ToString() ); }
        }

        public bool doesItemRunNow()
        {

            return ( _CswSchedItemTimingFactory.makeReportTimer( _CswScheduleLogicDetail.Recurrence, _CswScheduleLogicDetail.RunEndTime, _CswScheduleLogicDetail.Interval ).doesItemRunNow() );

        }//doesItemRunNow()

        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        private CswNbtResources _CswNbtResources = null;
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        private CswScheduleNodeUpdater _CswScheduleNodeUpdater;

        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( _CswNbtResources );
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
            _CswScheduleNodeUpdater = new CswScheduleNodeUpdater( _CswNbtResources );
        }//init() 

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    string InnerErrorMessage = string.Empty;
                    Collection<CswNbtObjClassMailReport> MailReports = _CswScheduleLogicNodes.getMailReports();
                    Collection<CswPrimaryKey> MailReportIdsToRun = new Collection<CswPrimaryKey>();

                    for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassMailReport CurrentMailReport = CurrentMailReport = MailReports[idx];
                        if( null != CurrentMailReport )
                        {
                            try
                            {
                                if( CurrentMailReport.Enabled.Checked == Tristate.True &&
                                    CurrentMailReport.Recipients.SelectedUserIds.Count > 0 &&
                                    ( CurrentMailReport.Type.Value != CswNbtObjClassMailReport.TypeOptionView ||                    // for notifications, 
                                      CurrentMailReport.Event.Value != CswNbtObjClassMailReport.EventOption.Edit.ToString() ||      // make sure at least one
                                      false == String.IsNullOrEmpty( CurrentMailReport.NodesToReport.Text ) ) )                     // node has changed
                                {
                                    if( false == CurrentMailReport.Type.Empty )
                                    {
                                        DateTime ThisDueDateValue = CurrentMailReport.NextDueDate.DateTimeValue;
                                        DateTime InitialDueDateValue = CurrentMailReport.DueDateInterval.getStartDate();
                                        DateTime FinalDueDateValue = CurrentMailReport.FinalDueDate.DateTimeValue;
                                        DateTime NowDateValue = DateTime.Now;
                                        DateTime MinDateValue = DateTime.MinValue;

                                        // BZ 7866
                                        if( DateTime.MinValue != ThisDueDateValue )
                                        {
                                            if( CswRateInterval.RateIntervalType.Hourly != CurrentMailReport.DueDateInterval.RateInterval.RateType )  // Ignore runtime for hourly reports
                                            {
                                                // Trim times
                                                ThisDueDateValue = ThisDueDateValue.Date;
                                                InitialDueDateValue = InitialDueDateValue.Date;
                                                FinalDueDateValue = FinalDueDateValue.Date;
                                                NowDateValue = NowDateValue.Date;
                                                MinDateValue = MinDateValue.Date;

                                                // BZ 7124 - set runtime
                                                if( CurrentMailReport.RunTime.DateTimeValue != DateTime.MinValue )
                                                {
                                                    ThisDueDateValue = ThisDueDateValue.AddTicks( CurrentMailReport.RunTime.DateTimeValue.TimeOfDay.Ticks );
                                                }
                                            }

                                            // Note: Warning days makes no sense for mail reports

                                            // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                                            if( ( NowDateValue >= InitialDueDateValue ) &&
                                                ( NowDateValue <= FinalDueDateValue || MinDateValue == FinalDueDateValue ) &&
                                                ( NowDateValue >= ThisDueDateValue ) )
                                            {
                                                // Add to batch operation
                                                MailReportIdsToRun.Add( CurrentMailReport.NodeId );

                                                // Cycle the next due date so we don't make another batch op while this one is running
                                                _CswScheduleNodeUpdater.update( CurrentMailReport.Node, string.Empty );
                                            }
                                        } // if( ThisDueDateValue != DateTime.MinValue )

                                    } // if( false == CurrentMailReport.Type.Empty )
                                    else
                                    {
                                        //if the report type is specified 

                                        // might be redundant with CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun()
                                        InnerErrorMessage = "Report type is not specified";
                                        CurrentMailReport.RunStatus.AddComment( InnerErrorMessage );
                                        CurrentMailReport.postChanges( true );
                                    }
                                }// if( CurrentMailReport.Enabled.Checked == Tristate.True )

                            }//try 

                            catch( Exception Exception )
                            {
                                InnerErrorMessage += "An exception occurred: " + Exception.Message + "; ";
                                CurrentMailReport.RunStatus.AddComment( InnerErrorMessage );
                                CurrentMailReport.postChanges( true );
                            }
                        } // if( null != CurrentMailReport )
                    } // for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )

                    if( false == String.IsNullOrEmpty( InnerErrorMessage ) )
                    {
                        _CswScheduleLogicDetail.StatusMessage = "The following errors occurred during processing: " + InnerErrorMessage;
                    }

                    if( MailReportIdsToRun.Count > 0 )
                    {
                        CswNbtBatchOpMailReport BatchOp = new CswNbtBatchOpMailReport( _CswNbtResources );
                        BatchOp.makeBatchOp( MailReportIdsToRun );
                    }
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "An exception occurred: " + Exception.Message;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;

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

    }//CswScheduleLogicNbtGenEmailRpt

}//namespace ChemSW.Nbt.Sched
