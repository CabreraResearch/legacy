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
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;

        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
        }//initScheduleLogicDetail() 




        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
                try
                {
                    string InnerErrorMessage = string.Empty;
                    _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );
                    CswResources.AuditContext = "Scheduler Task: " + RuleName;

                    //Review K4566: limit iteration and always increment https://fogbugz.chemswlive.com/kiln/Review/K4566
                    Int32 MailReportLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.generatorlimit.ToString() ) );
                    if( MailReportLimit < 1 )
                    {
                        MailReportLimit = 1;
                    }
                    Int32 TotalMailReportsProcessed = 0; 

                    Collection<CswNbtObjClassMailReport> MailReports = _CswScheduleLogicNodes.getMailReports();
                    Collection<CswPrimaryKey> MailReportIdsToRun = new Collection<CswPrimaryKey>();

                    for( Int32 idx = 0; ( idx < MailReports.Count && TotalMailReportsProcessed < MailReportLimit ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {
                        CswNbtObjClassMailReport CurrentMailReport = MailReports[idx];
                        if( null != CurrentMailReport )
                        {
                            try
                            {
                                // The query already checks if the reports are enabled
                                //if( CurrentMailReport.Enabled.Checked == Tristate.True )
                                //{
                                    if( CurrentMailReport.Recipients.SelectedUserIds.Count > 0 )
                                    {
                                        // for notifications, make sure at least one node has changed
                                        if( CurrentMailReport.Type.Value != CswNbtObjClassMailReport.TypeOptionView ||
                                            CurrentMailReport.Event.Value != CswNbtObjClassMailReport.EventOption.Edit.ToString() ||
                                            false == String.IsNullOrEmpty( CurrentMailReport.NodesToReport.Text ) )
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
                                                    if( CswRateInterval.RateIntervalType.Hourly != CurrentMailReport.DueDateInterval.RateInterval.RateType ) // Ignore runtime for hourly reports
                                                    {
                                                        // Trim times
                                                        ThisDueDateValue = ThisDueDateValue.Date;
                                                        InitialDueDateValue = InitialDueDateValue.Date;
                                                        FinalDueDateValue = FinalDueDateValue.Date;
                                                        //NowDateValue = NowDateValue.Date;
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
                                                        CurrentMailReport.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                                                        CurrentMailReport.postChanges( false );
                                                    }
                                                } // if( ThisDueDateValue != DateTime.MinValue )

                                            } // if( false == CurrentMailReport.Type.Empty )
                                            else
                                            {
                                                // might be redundant with CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun()
                                                CurrentMailReport.RunStatus.AddComment( "Report type is not specified" );
                                                CurrentMailReport.Enabled.Checked = Tristate.False;
                                                CurrentMailReport.postChanges( true );
                                            }
                                        } // if there's something to report
                                        else
                                        {
                                            CurrentMailReport.RunStatus.AddComment( "No reportable changes made" );
                                            CurrentMailReport.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false ); 
                                            CurrentMailReport.postChanges( false );
                                        }
                                    } // if( CurrentMailReport.Recipients.SelectedUserIds.Count > 0 )
                                    else
                                    {
                                        CurrentMailReport.RunStatus.AddComment( "No recipients selected" );
                                        CurrentMailReport.Enabled.Checked = Tristate.False;
                                        CurrentMailReport.postChanges( false );
                                    }
                                //} // if( CurrentMailReport.Enabled.Checked == Tristate.True )
                            } //try 

                            catch( Exception Exception )
                            {
                                InnerErrorMessage += "An exception occurred: " + Exception.Message + "; ";
                                CurrentMailReport.Enabled.Checked = Tristate.False; 
                                CurrentMailReport.RunStatus.AddComment( InnerErrorMessage );
                                CurrentMailReport.postChanges( true );
                            }
                            finally
                            {
                                TotalMailReportsProcessed += 1;
                            }
                        } // if( null != CurrentMailReport )
                    } // for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )

                    if( false == String.IsNullOrEmpty( InnerErrorMessage ) )
                    {
                        _CswScheduleLogicDetail.StatusMessage = "The following errors occurred during processing: " + InnerErrorMessage;
                    }
                    else
                    {
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    }

                    if( MailReportIdsToRun.Count > 0 )
                    {
                        CswNbtBatchOpMailReport BatchOp = new CswNbtBatchOpMailReport( CswNbtResources );
                        BatchOp.makeBatchOp( MailReportIdsToRun );
                    }
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "An exception occurred: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
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
    }//CswScheduleLogicNbtGenEmailRpt

}//namespace ChemSW.Nbt.Sched
