using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Mail; 
namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenEmailRpt : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( "NbtGenEmailRpt" ); }
        }

        CswNbtNode _CswNbtNodeMailReport = null; 
        public bool doesItemRunNow()
        {
            bool ReturnVal = false;

            _CswNbtNodeMailReport = null; //THIS MUST BE QUERIED FOR (USED TO BE PASSED INTO CTOR)
            CswNbtObjClassMailReport MailReportNode = CswNbtNodeCaster.AsMailReport( _CswNbtNodeMailReport );
            if( MailReportNode.Enabled.Checked == Tristate.True )
            {
                DateTime ThisDueDateValue = MailReportNode.NextDueDate.DateValue.Date;
                DateTime InitialDueDateValue = MailReportNode.DueDateInterval.getStartDate().Date;
                DateTime FinalDueDateValue = MailReportNode.FinalDueDate.DateValue.Date;

                // BZ 7866
                if( ThisDueDateValue != DateTime.MinValue )
                {
                    // BZ 7124 - set runtime
                    if( MailReportNode.RunTime.TimeValue != DateTime.MinValue )
                        ThisDueDateValue = ThisDueDateValue.AddTicks( MailReportNode.RunTime.TimeValue.TimeOfDay.Ticks );

                    Int32 WarnDays = (Int32) MailReportNode.WarningDays.Value;
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

            } // if( MailReportNode.Enabled.Checked == Tristate.True )

            return ( ReturnVal );

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

                    CswMail CswMail = _CswNbtResources.CswMail;
                    CswNbtMailReportStatus CswNbtMailReportStatus = new CswNbtMailReportStatus();
                    if( null != _CswNbtNodeMailReport )
                    {
                        CswNbtObjClassMailReport MailReportObjClass = CswNbtNodeCaster.AsMailReport( _CswNbtNodeMailReport );
                        MailReportObjClass.LastProcessed.DateValue = DateTime.Now;

                        string ReportReference = _CswNbtResources.SetupVbls["MailReportUrlStem"] + "Login.aspx?destination=";

                        if( !MailReportObjClass.Type.Empty )
                        {
                            Int32 ViewId = Int32.MinValue;
                            string ReportParameter = string.Empty;
                            if( "View" == MailReportObjClass.Type.Value )
                            {
                                ViewId = CswConvert.ToInt32( MailReportObjClass.ReportView.SelectedViewIds );
                                ReportParameter = CswTools.UrlToQueryStringParam( "Main.aspx?ViewId=" + ViewId.ToString() );
                            }
                            else if( "Report" == MailReportObjClass.Type.Value )
                            {
                                if( null != _CswNbtResources.Nodes[MailReportObjClass.Report.NodeId] )
                                {
                                    CswNbtNode ReportNode = _CswNbtResources.Nodes[MailReportObjClass.Report.RelatedNodeId];
                                    CswNbtObjClassReport ReportObjClass = CswNbtNodeCaster.AsReport( ReportNode );
                                    ViewId = CswConvert.ToInt32( ReportObjClass.View.ViewId );
                                    ReportParameter = CswTools.UrlToQueryStringParam( "Report.aspx?reportid=" + ReportNode.NodeId.PrimaryKey.ToString() );
                                }
                            }
                            else
                            {
                                CswNbtMailReportStatus.ReportFailureReason = "Unknown " + MailReportObjClass.Type.Value;

                            }//if-else on format

                            if( string.Empty != ReportParameter )
                            {
                                ReportReference += ReportParameter;
                                CswNbtMailReportStatus.Link = ReportReference;

                            }

                            if( CswNbtMailReportStatus.ReportReadyForQuery )
                            {
                                // BZ 10094 - Run report in context of recipient 
                                if( !MailReportObjClass.Recipients.Empty )
                                {
                                    Collection<Int32> RecipientUserIds = MailReportObjClass.Recipients.SelectedUserIds.ToIntCollection();
                                    foreach( Int32 UserId in RecipientUserIds )
                                    {
                                        if( Int32.MinValue != UserId )
                                        {
                                            CswNbtNode UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
                                            CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) CswNbtNodeCaster.AsUser( UserNode );
                                            string EmailAddy = UserNodeAsUser.Email.Trim();
                                            if( EmailAddy != string.Empty )
                                            {
                                                CswNbtView ReportView = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );

                                                string Subject = MailReportObjClass.Type.Value + " Notification: " + ReportView.ViewName;

                                                ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( UserNodeAsUser as ICswNbtUser, ReportView, true, true, false, false );
                                                string Message = string.Empty;
                                                if( ReportTree.getChildNodeCount() > 0 )
                                                {
                                                    CswNbtMailReportStatus.ReportDataExist = true;
                                                    Message = MailReportObjClass.Message.Text + "\r\n";
                                                    Message += ReportReference;
                                                    CswNbtMailReportStatus.ReportReason = "The report's view returned data ";
                                                    //= "Message sent with report link: " + ReportReference;
                                                }
                                                else
                                                {
                                                    if( string.Empty != MailReportObjClass.NoDataNotification.Text )
                                                    {
                                                        CswNbtMailReportStatus.ReportDataExist = true;
                                                        Message = MailReportObjClass.NoDataNotification.Text;
                                                        CswNbtMailReportStatus.ReportReason = "Report sent with no data message ";
                                                    }
                                                    else
                                                    {
                                                        CswNbtMailReportStatus.ReportDataExist = false;
                                                        CswNbtMailReportStatus.ReportNotMadeReason = "The report's view returned no data; a no-data message was not specified";
                                                    }//if-else there was nodata notification


                                                }//if-else the report has any results


                                                if( CswNbtMailReportStatus.ReportDataExist )
                                                {

                                                    CswNbtMailReportStatus.EmailSentReason = "Recipients: ";

                                                    CswMailMessage MailMessage = new CswMailMessage();
                                                    MailMessage.Recipient = EmailAddy;
                                                    MailMessage.RecipientDisplayName = UserNodeAsUser.FirstName + " " + UserNodeAsUser.LastName;
                                                    MailMessage.Subject = Subject;
                                                    MailMessage.Content = Message;

                                                    if( CswMail.send( MailMessage ) )
                                                    {
                                                        CswNbtMailReportStatus.EmailSentReason += UserNode.NodeName + " at " + EmailAddy + " (succeeded); ";
                                                    }
                                                    else
                                                    {
                                                        CswNbtMailReportStatus.EmailFailureReason += UserNode.NodeName + " at " + EmailAddy + " (failed: " + CswMail.Status + "); ";
                                                    }

                                                } // if( CswNbtMailReportStatus.ReportDataExist )
                                            } // if( EmailAddy != string.Empty )
                                        }//if( Int32.MinValue != UserId )
                                    }//foreach( Int32 UserId in RecipientUserIds )
                                }//if-else report data exist

                            }//if report is ready for query

                        }
                        else
                        {
                            CswNbtMailReportStatus.ReportFailureReason = "Report type is not specified";

                        }//if the report type is specified 

                        // might be redundant with CswNbtDbBasedSchdEvents.handleOnSchdItemWasRun()
                        MailReportObjClass.RunStatus.StaticText = CswNbtMailReportStatus.Message;
                        MailReportObjClass.postChanges( true );

                    }//if there is a report node 


                    //SCHEDULE NODE MUST BE UPDATED; CF ALSO GEN NODE CLASS
                    //if( null != OnScheduleItemWasRun )
                    //{
                    //    OnScheduleItemWasRun( this, _CswNbtNodeMailReport );
                    //} 
                    
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

    }//CswScheduleLogicNbtGenEmailRpt

}//namespace ChemSW.Nbt.Sched
