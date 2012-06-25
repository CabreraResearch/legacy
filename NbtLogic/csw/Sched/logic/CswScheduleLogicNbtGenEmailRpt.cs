using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;


enum MailRptFormatOptions { Link, CSV };

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenEmailRpt : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.GenEmailRpt.ToString() ); }
        }

        //CswNbtNode _CswNbtNodeMailReport = null;
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
        private CswScheduleNodeUpdater _CswScheduleNodeUpdater = null;
        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;
        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswNbtResources = (CswNbtResources) RuleResources;
            _CswScheduleLogicDetail = CswScheduleLogicDetail;
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( _CswNbtResources );
            _CswScheduleNodeUpdater = new CswScheduleNodeUpdater( _CswNbtResources );
            //_CswNbtResources.AuditContext = "Scheduler Task: Generate Email Reports";
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;
        }//init() 


        private string makeViewUrl( CswNbtViewId ViewId )
        {
            string ret = _CswNbtResources.SetupVbls["MailReportUrlStem"];
            if( !ret.EndsWith( "/" ) ) ret += "/";
            ret += "Main.html?viewid=";
            ret += ViewId.ToString();
            return ret;
        }
        private string makeReportUrl( CswNbtObjClassReport ReportObjClass )
        {
            string ret = _CswNbtResources.SetupVbls["MailReportUrlStem"];
            //if( !ret.EndsWith( "/" ) ) ret += "/";
            ret += ReportObjClass.ReportUrl;
            return ret;
        }

        public void threadCallBack()
        {
            _LogicRunStatus = LogicRunStatus.Running;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
                {
                    string InnerErrorMessage = string.Empty;
                    Int32 TotalReportsProcessed = 0;
                    List<CswNbtObjClassMailReport> MailReports = _CswScheduleLogicNodes.getMailReports();

                    for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
                    {

                        CswNbtObjClassMailReport CurrentMailReport = null;
                        try
                        {

                            CurrentMailReport = MailReports[idx];

                            if( CurrentMailReport.Enabled.Checked == Tristate.True )
                            {
                                DateTime ThisDueDateValue = CurrentMailReport.NextDueDate.DateTimeValue.Date;
                                DateTime InitialDueDateValue = CurrentMailReport.DueDateInterval.getStartDate().Date;
                                DateTime FinalDueDateValue = CurrentMailReport.FinalDueDate.DateTimeValue.Date;

                                // BZ 7866
                                if( ThisDueDateValue != DateTime.MinValue )
                                {
                                    // BZ 7124 - set runtime
                                    if( CurrentMailReport.RunTime.DateTimeValue != DateTime.MinValue )
                                        ThisDueDateValue = ThisDueDateValue.AddTicks( CurrentMailReport.RunTime.DateTimeValue.TimeOfDay.Ticks );

                                    Int32 WarnDays = (Int32) CurrentMailReport.WarningDays.Value;
                                    if( WarnDays > 0 )
                                    {
                                        TimeSpan WarningDaysSpan = new TimeSpan( WarnDays, 0, 0, 0, 0 );
                                        ThisDueDateValue = ThisDueDateValue.Subtract( WarningDaysSpan );
                                        InitialDueDateValue = InitialDueDateValue.Subtract( WarningDaysSpan );
                                    }

                                    CswNbtMailReportStatus CswNbtMailReportStatus = new CswNbtMailReportStatus();
                                    // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                                    if( ( DateTime.Now.Date >= InitialDueDateValue ) &&
                                        ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                                        ( DateTime.Now >= ThisDueDateValue ) )
                                    {
                                        CswMail CswMail = _CswNbtResources.CswMail;
                                        CurrentMailReport.LastProcessed.DateTimeValue = DateTime.Now;

                                        if( !CurrentMailReport.Type.Empty )
                                        {
                                            // BZ 10094 - Run report in context of recipient 
                                            if( !CurrentMailReport.Recipients.Empty )
                                            {
                                                Collection<Int32> RecipientUserIds = CurrentMailReport.Recipients.SelectedUserIds.ToIntCollection();
                                                foreach( Int32 UserId in RecipientUserIds )
                                                {
                                                    if( Int32.MinValue != UserId )
                                                    {
                                                        CswNbtNode UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
                                                        CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
                                                        string EmailAddy = UserNodeAsUser.Email.Trim();
                                                        if( EmailAddy != string.Empty )
                                                        {
                                                            DataTable ReportTable = null;
                                                            CswNbtNode ReportNode = null;
                                                            CswNbtObjClassReport ReportObjClass = null;

                                                            string ReportLink = string.Empty;
                                                            bool ContinueWithReport = false;
                                                            bool HasResults = false;
                                                            string Subject = string.Empty;
                                                            if( "View" == CurrentMailReport.Type.Value )
                                                            {
                                                                Int32 ViewIdInt = CswConvert.ToInt32( CurrentMailReport.ReportView.SelectedViewIds );
                                                                if( Int32.MinValue != ViewIdInt )
                                                                {
                                                                    CswNbtViewId ViewId = new CswNbtViewId( ViewIdInt );
                                                                    if( ViewId.isSet() )
                                                                    {
                                                                        ReportLink = makeViewUrl( ViewId );
                                                                        ContinueWithReport = true;

                                                                        CswNbtView ReportView = _CswNbtResources.ViewSelect.restoreView( ViewId );
                                                                        ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( UserNodeAsUser as ICswNbtUser, ReportView, true, true, false, false );
                                                                        Subject = CurrentMailReport.Type.Value + " Notification: " + ReportView.ViewName;
                                                                        HasResults = ( ReportTree.getChildNodeCount() > 0 );
                                                                    }
                                                                }
                                                            }
                                                            else if( "Report" == CurrentMailReport.Type.Value )
                                                            {
                                                                if( null != _CswNbtResources.Nodes[CurrentMailReport.Report.NodeId] )
                                                                {
                                                                    ReportNode = _CswNbtResources.Nodes[CurrentMailReport.Report.RelatedNodeId];
                                                                    //ViewId = ReportObjClass.View.ViewId;

                                                                    CswArbitrarySelect ReportSelect = _CswNbtResources.makeCswArbitrarySelect( "MailReport_" + ReportNode.NodeId.ToString() + "_Select", ReportObjClass.SQL.Text );
                                                                    ReportTable = ReportSelect.getTable();

                                                                    ContinueWithReport = true;

                                                                    Subject = CurrentMailReport.Type.Value + " Notification: " + ReportNode.NodeName;
                                                                    HasResults = ( ReportTable.Rows.Count > 0 );

                                                                }
                                                            }

                                                            if( ContinueWithReport )
                                                            {

                                                                if( CswNbtMailReportStatus.ReportReadyForQuery )
                                                                {

                                                                    MailRptFormatOptions MailRptFormat = (MailRptFormatOptions) Enum.Parse( typeof( MailRptFormatOptions ), CurrentMailReport.OutputFormat.Value.ToString() );
                                                                    ReportObjClass = (CswNbtObjClassReport) ReportNode;

                                                                    string Message = string.Empty;
                                                                    if( HasResults )
                                                                    {

                                                                        if( MailRptFormatOptions.Link == MailRptFormat )
                                                                        {
                                                                            ReportLink = makeReportUrl( ReportObjClass );
                                                                            CswNbtMailReportStatus.Link = ReportLink;

                                                                        }
                                                                        CswNbtMailReportStatus.ReportDataExist = true;
                                                                        Message = CurrentMailReport.Message.Text + "\r\n";
                                                                        Message += ReportLink;
                                                                        CswNbtMailReportStatus.ReportReason = "The report's view returned data ";
                                                                    }
                                                                    else
                                                                    {
                                                                        if( string.Empty != CurrentMailReport.NoDataNotification.Text )
                                                                        {
                                                                            CswNbtMailReportStatus.ReportDataExist = true;
                                                                            Message = CurrentMailReport.NoDataNotification.Text;
                                                                            CswNbtMailReportStatus.ReportReason = "Report sent with no data message ";
                                                                        }
                                                                        else
                                                                        {
                                                                            CswNbtMailReportStatus.ReportDataExist = false;
                                                                            CswNbtMailReportStatus.ReportNotMadeReason = "The report's view returned no data; a no-data message was not specified";
                                                                        } //if-else there was nodata notification


                                                                    } //if-else the report has any results


                                                                    if( CswNbtMailReportStatus.ReportDataExist )
                                                                    {

                                                                        CswNbtMailReportStatus.EmailSentReason = "Recipients: ";

                                                                        CswMailMessage MailMessage = new CswMailMessage();
                                                                        MailMessage.Recipient = EmailAddy;
                                                                        MailMessage.RecipientDisplayName = UserNodeAsUser.FirstName + " " + UserNodeAsUser.LastName;
                                                                        MailMessage.Subject = Subject;
                                                                        MailMessage.Content = Message;


                                                                        if( MailRptFormatOptions.CSV == MailRptFormat )
                                                                        {
                                                                            string TableAsCSV = ( ( CswDataTable ) ReportTable ).ToCsv();

                                                                            System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream( new System.Text.UTF8Encoding().GetBytes( TableAsCSV ), false );
                                                                            
                                                                                //System.IO.StringWriter StringWriter = new System.IO.StringWriter()
                                                                            MailMessage.Attachment = MemoryStream;
                                                                        }

                                                                        string StatusMessage = string.Empty;
                                                                        if( CswMail.send( MailMessage ) )
                                                                        {
                                                                            CswNbtMailReportStatus.EmailSentReason += UserNode.NodeName + " at " + EmailAddy + " (succeeded); ";
                                                                            StatusMessage = CswNbtMailReportStatus.EmailSentReason;
                                                                        }
                                                                        else
                                                                        {
                                                                            CswNbtMailReportStatus.EmailFailureReason += UserNode.NodeName + " at " + EmailAddy + " (failed: " + CswMail.Status + "); ";
                                                                            StatusMessage = CswNbtMailReportStatus.EmailFailureReason;
                                                                        }

                                                                        TotalReportsProcessed++;
                                                                        _CswScheduleNodeUpdater.update( CurrentMailReport.Node, StatusMessage );

                                                                    } // if( CswNbtMailReportStatus.ReportDataExist )

                                                                } // if( CswNbtMailReportStatus.ReportReadyForQuery )

                                                            } // if( ContinueWithReport )

                                                        } //if( EmailAddy != string.Empty )

                                                    } //if( Int32.MinValue != UserId )

                                                } // foreach( Int32 UserId in RecipientUserIds )

                                            } // if( !CurrentMailReport.Recipients.Empty )

                                        } // if( !CurrentMailReport.Type.Empty )
                                        else
                                        {
                                            CswNbtMailReportStatus.ReportFailureReason = "Unknown " + CurrentMailReport.Type.Value;
                                            InnerErrorMessage += CswNbtMailReportStatus.ReportFailureReason + ";";
                                        }
                                    } // if( ThisDueDateValue != DateTime.MinValue )
                                    else
                                    {
                                        CswNbtMailReportStatus.ReportFailureReason = "Report type is not specified";
                                        InnerErrorMessage += CswNbtMailReportStatus.ReportFailureReason + "; ";

                                        //if the report type is specified 

                                        // might be redundant with CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun()
                                        CurrentMailReport.RunStatus.AddComment( CswNbtMailReportStatus.Message );
                                        CurrentMailReport.postChanges( true );

                                    }
                                } // if( ThisDueDateValue != DateTime.MinValue )

                            }// if( CurrentMailReport.Enabled.Checked == Tristate.True )

                        }//try 

                        catch( Exception Exception )
                        {
                            if( null != CurrentMailReport )
                            {

                                InnerErrorMessage += "An exception occurred: " + Exception.Message + "; ";

                                CurrentMailReport.RunStatus.AddComment( InnerErrorMessage );
                                CurrentMailReport.postChanges( true );

                            }
                        }

                    }// for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )

                    if( string.Empty == InnerErrorMessage )
                    {
                        _CswScheduleLogicDetail.StatusMessage = TotalReportsProcessed.ToString() + " reports processed without error";
                    }
                    else
                    {
                        _CswScheduleLogicDetail.StatusMessage = TotalReportsProcessed.ToString() + " reports processed, and the following errors occured during processing: " + InnerErrorMessage;
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
