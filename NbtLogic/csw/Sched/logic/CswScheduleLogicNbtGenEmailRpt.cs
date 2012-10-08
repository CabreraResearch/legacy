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
            if( !ret.EndsWith( "/" ) ) ret += "/";
            //ret += ReportObjClass.ReportUrl;
            ret += "Main.html?reportid=";
            ret += ReportObjClass.NodeId.ToString();
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
                                    {
                                        ThisDueDateValue = ThisDueDateValue.AddTicks( CurrentMailReport.RunTime.DateTimeValue.TimeOfDay.Ticks );
                                    }

                                    // Warning days makes no sense for mail reports

                                    //Int32 WarnDays = (Int32) CurrentMailReport.WarningDays.Value;
                                    //if( WarnDays > 0 )
                                    //{
                                    //    TimeSpan WarningDaysSpan = new TimeSpan( WarnDays, 0, 0, 0, 0 );
                                    //    ThisDueDateValue = ThisDueDateValue.Subtract( WarningDaysSpan );
                                    //    InitialDueDateValue = InitialDueDateValue.Subtract( WarningDaysSpan );
                                    //}

                                    CswNbtMailReportStatus CswNbtMailReportStatus = new CswNbtMailReportStatus();
                                    // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                                    if( ( DateTime.Now.Date >= InitialDueDateValue ) &&
                                        ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                                        ( DateTime.Now >= ThisDueDateValue ) )
                                    {
                                        //CswMail CswMail = _CswNbtResources.CswMail;
                                        CurrentMailReport.LastProcessed.DateTimeValue = DateTime.Now;

                                        if( false == CurrentMailReport.Type.Empty )
                                        {
                                            string EmailReportStatusMessage = processMailReport( CurrentMailReport, CswNbtMailReportStatus );
                                            TotalReportsProcessed++;
                                            _CswScheduleNodeUpdater.update( CurrentMailReport.Node, EmailReportStatusMessage );
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

        public string processMailReport( CswNbtObjClassMailReport CurrentMailReport, CswNbtMailReportStatus CswNbtMailReportStatus )
        {
            string EmailReportStatusMessage = string.Empty;//all conditions must give StatusMessage a value!

            if( false == CurrentMailReport.Recipients.Empty )
            {
                Collection<Int32> RecipientUserIds = CurrentMailReport.Recipients.SelectedUserIds.ToIntCollection();
                foreach( Int32 UserId in RecipientUserIds )
                {
                    if( Int32.MinValue != UserId )
                    {
                        CswNbtNode UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
                        CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
                        string CurrentEmailAddress = UserNodeAsUser.Email.Trim();
                        if( CurrentEmailAddress != string.Empty )
                        {
                            DataTable ReportTable = null;
                            CswNbtNode ReportNode = null;
                            CswNbtObjClassReport ReportObjClass = null;

                            string ViewLink = string.Empty;
                            string EmailMessageSubject = string.Empty;
                            string EmailMessageBody = string.Empty;

                            if( "View" == CurrentMailReport.Type.Value )
                            {
                                CswNbtViewId ViewId = CurrentMailReport.ReportView.ViewId;
                                if( ViewId.isSet() )
                                {
                                    ViewLink = makeViewUrl( ViewId );
                                    CswNbtView ReportView = _CswNbtResources.ViewSelect.restoreView( ViewId );
                                    ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( UserNodeAsUser as ICswNbtUser, ReportView, true, true, false, false );

                                    EmailMessageSubject = CurrentMailReport.NodeName;
                                    if( CswNbtObjClassMailReport.EventOption.Exists.ToString() != CurrentMailReport.Event.Value )
                                    {
                                        // case 27720 - check mail report events to find nodes that match the view results
                                        Collection<CswPrimaryKey> NodesToMail = new Collection<CswPrimaryKey>();
                                        foreach( Int32 NodeId in CurrentMailReport.GetNodesToReport().ToIntCollection() )
                                        {
                                            CswPrimaryKey ThisNodeId = new CswPrimaryKey( "nodes", NodeId );
                                            ReportTree.makeNodeCurrent( ThisNodeId );
                                            if( ReportTree.isCurrentNodeDefined() )
                                            {
                                                NodesToMail.Add( ThisNodeId );
                                            }
                                        }
                                        if( NodesToMail.Count > 0 )
                                        {
                                            EmailMessageBody = _setStatusHaveData( CurrentMailReport, CswNbtMailReportStatus, ViewLink );
                                        }
                                        else
                                        {
                                            EmailMessageBody = _setStatusDoNotHaveData( CurrentMailReport, CswNbtMailReportStatus );
                                        }//if-else the view got a result
                                    }
                                    else
                                    {
                                        if( ReportTree.getChildNodeCount() > 0 )
                                        {
                                            EmailMessageBody = _setStatusHaveData( CurrentMailReport, CswNbtMailReportStatus, ViewLink );
                                        }
                                        else
                                        {
                                            EmailMessageBody = _setStatusDoNotHaveData( CurrentMailReport, CswNbtMailReportStatus );
                                        }//if-else the view got a result
                                    }

                                    EmailReportStatusMessage = _sendMailMessage( CurrentMailReport, CswNbtMailReportStatus, EmailMessageBody, UserNodeAsUser.LastName, UserNodeAsUser.FirstName, UserNodeAsUser.Node.NodeName, EmailMessageSubject, CurrentEmailAddress, null );
                                }
                                else
                                {
                                    EmailReportStatusMessage = "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the associated view's ViewId is not set";
                                }
                            }
                            else if( "Report" == CurrentMailReport.Type.Value )
                            {
                                if( null != _CswNbtResources.Nodes[CurrentMailReport.Report.NodeId] )
                                {
                                    ReportNode = _CswNbtResources.Nodes[CurrentMailReport.Report.RelatedNodeId];
                                    ReportObjClass = (CswNbtObjClassReport) ReportNode;
                                    string ReportSql = ReportObjClass.getUserContextSql( UserNodeAsUser.Username );

                                    CswArbitrarySelect ReportSelect = _CswNbtResources.makeCswArbitrarySelect( "MailReport_" + ReportNode.NodeId.ToString() + "_Select", ReportSql );
                                    ReportTable = ReportSelect.getTable();

                                    EmailMessageSubject = CurrentMailReport.Type.Value + " Notification: " + ReportNode.NodeName;

                                    if( ReportTable.Rows.Count > 0 )
                                    {
                                        string ReportLink = string.Empty;
                                        MailRptFormatOptions MailRptFormat = (MailRptFormatOptions) Enum.Parse( typeof( MailRptFormatOptions ), CurrentMailReport.OutputFormat.Value.ToString() );
                                        if( MailRptFormatOptions.Link == MailRptFormat )
                                        {
                                            ReportLink = makeReportUrl( ReportObjClass );
                                            ReportTable = null; //so we don't end up attaching the CSV
                                        }

                                        EmailMessageBody = _setStatusHaveData( CurrentMailReport, CswNbtMailReportStatus, ReportLink );
                                    }
                                    else
                                    {
                                        EmailMessageBody = _setStatusDoNotHaveData( CurrentMailReport, CswNbtMailReportStatus );
                                    }//if-else the view got a result

                                    EmailReportStatusMessage = _sendMailMessage( CurrentMailReport, CswNbtMailReportStatus, EmailMessageBody, UserNodeAsUser.LastName, UserNodeAsUser.FirstName, UserNodeAsUser.Node.NodeName, EmailMessageSubject, CurrentEmailAddress, ReportTable );
                                }
                                else
                                {
                                    EmailReportStatusMessage = "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the associated report's NodeId is not set";
                                }//if-else report's node id is present
                            }
                            else
                            {
                                EmailReportStatusMessage = "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the report type " + CurrentMailReport.Type.Value + " is unknown";
                            }//if-else-if on report type

                        }//if( Email Address != string.Empty )

                    }//if( Int32.MinValue != UserId )

                }//foreach( Int32 UserId in RecipientUserIds )

            }//if( !CurrentMailReport.Recipients.Empty )

            // case 27720 - clear existing Mail Report Event records for this mail report
            CurrentMailReport.ClearNodesToReport();

            return EmailReportStatusMessage;
        }//processMailReport()

        private string _sendMailMessage( CswNbtObjClassMailReport CurrentMailReport, CswNbtMailReportStatus CswNbtMailReportStatus, string MailReportMessage, string LastName, string FirstName, string UserName, string Subject, string CurrentEmailAddress, DataTable ReportTable )
        {
            string ReturnVal = string.Empty;
            if( CswNbtMailReportStatus.ReportDataExist )
            {
                CswMail CswMail = _CswNbtResources.CswMail;

                CswNbtMailReportStatus.EmailSentReason = "Recipients: ";

                CswMailMessage MailMessage = new CswMailMessage();
                MailMessage.Recipient = CurrentEmailAddress;
                MailMessage.RecipientDisplayName = FirstName + " " + LastName;
                MailMessage.Subject = Subject;
                MailMessage.Content = MailReportMessage;


                if( null != ReportTable )
                {
                    string TableAsCSV = ( (CswDataTable) ReportTable ).ToCsv();

                    byte[] Buffer = new System.Text.UTF8Encoding().GetBytes( TableAsCSV );
                    System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream( Buffer, false );

                    MailMessage.Attachment = MemoryStream;
                    MailMessage.AttachmentDisplayName = CurrentMailReport.Node.NodeName + ".csv";
                }

                if( CswMail.send( MailMessage ) )
                {
                    CswNbtMailReportStatus.EmailSentReason += UserName + " at " + CurrentEmailAddress + " (succeeded); ";
                    ReturnVal = CswNbtMailReportStatus.EmailSentReason;
                }
                else
                {
                    CswNbtMailReportStatus.EmailFailureReason += UserName + " at " + CurrentEmailAddress + " (failed: " + CswMail.Status + "); ";
                    ReturnVal = CswNbtMailReportStatus.EmailFailureReason;
                }
            }
            return ( ReturnVal );

        }//_sendMailMessage()


        private string _setStatusHaveData( CswNbtObjClassMailReport CurrentMailReport, CswNbtMailReportStatus CswNbtMailReportStatus, string ViewLink )
        {
            string ReturnVal = string.Empty;


            CswNbtMailReportStatus.ReportDataExist = true;
            ReturnVal = CurrentMailReport.Message.Text + "\r\n";



            if( string.Empty != ViewLink )
            {
                ReturnVal += ViewLink;
            }

            CswNbtMailReportStatus.ReportReason = "The report's view returned data ";

            return ( ReturnVal );

        }//_handleYesDataCase()



        private string _setStatusDoNotHaveData( CswNbtObjClassMailReport CswNbtObjClassMailReport, CswNbtMailReportStatus CswNbtMailReportStatus )
        {
            string ReturnVal = string.Empty;

            if( string.Empty != CswNbtObjClassMailReport.NoDataNotification.Text )
            {

                CswNbtMailReportStatus.ReportDataExist = true;
                ReturnVal = CswNbtObjClassMailReport.NoDataNotification.Text;
                CswNbtMailReportStatus.ReportReason = "Report sent with no data message ";
            }
            else
            {
                CswNbtMailReportStatus.ReportDataExist = false;
                CswNbtMailReportStatus.ReportNotMadeReason = "The report's view returned no data; a no-data message was not specified";
            } //if-else there was nodata notification

            return ( ReturnVal );

        }//_handleNoDataCase()


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
