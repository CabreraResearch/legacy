using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtGenEmailRpt : ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.GenEmailRpt ); }
        }

        //Determine the number of mail report nodes that need to run and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicNodes = new CswScheduleLogicNodes( ( CswNbtResources ) CswResources );
            Collection<CswNbtObjClassMailReport> MailReports = _CswScheduleLogicNodes.getMailReports();
            _CswScheduleLogicDetail.LoadCount = MailReports.Count;
            return _CswScheduleLogicDetail.LoadCount;
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private CswScheduleLogicNodes _CswScheduleLogicNodes = null;

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }//initScheduleLogicDetail() 

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {

                CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
                try
                {
                    string InnerErrorMessage = string.Empty;
                    _CswScheduleLogicNodes = new CswScheduleLogicNodes( CswNbtResources );
                    CswResources.AuditContext = "Scheduler Task: " + RuleName;

                    //Review K4566: limit iteration and always increment https://fogbugz.chemswlive.com/kiln/Review/K4566
                    Int32 MailReportLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.generatorlimit.ToString() ) );
                    if( MailReportLimit < 1 )
                    {
                        MailReportLimit = 1;
                    }
                    Int32 TotalMailReportsProcessed = 0; 

                    Collection<CswNbtObjClassMailReport> MailReports = _CswScheduleLogicNodes.getMailReports();
                    Collection<CswPrimaryKey> MailReportIdsToRun = new Collection<CswPrimaryKey>();

                    for( Int32 idx = 0; ( idx < MailReports.Count && TotalMailReportsProcessed < MailReportLimit ) && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ); idx++ )
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
                                            CurrentMailReport.Event.Value != CswEnumNbtMailReportEventOption.Edit.ToString() ||
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
                                                    if( CswEnumRateIntervalType.Hourly != CurrentMailReport.DueDateInterval.RateInterval.RateType ) // Ignore runtime for hourly reports
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
                                                CurrentMailReport.Enabled.Checked = CswEnumTristate.False;
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
                                        CurrentMailReport.Enabled.Checked = CswEnumTristate.False;
                                        CurrentMailReport.postChanges( false );
                                    }
                                //} // if( CurrentMailReport.Enabled.Checked == Tristate.True )
                            } //try 

                            catch( Exception Exception )
                            {
                                InnerErrorMessage += "An exception occurred: " + Exception.Message + "; ";
                                CurrentMailReport.Enabled.Checked = CswEnumTristate.False; 
                                CurrentMailReport.RunStatus.AddComment( InnerErrorMessage );
                                CurrentMailReport.postChanges( true );
                            }
                            finally
                            {
                                TotalMailReportsProcessed += 1;
                            }
                        } // if( null != CurrentMailReport )
                    } // for( Int32 idx = 0; ( idx < MailReports.Count ) && ( LogicRunStatus.Stopping != _LogicRunStatus ); idx++ )

                    foreach(CswPrimaryKey MailReportId in MailReportIdsToRun )
                    {
                        CswNbtObjClassMailReport MailReportNode = CswNbtResources.Nodes[MailReportId];
                        if( null != MailReportNode )
                        {
                            Collection<Int32> RecipientIds = MailReportNode.Recipients.SelectedUserIds.ToIntCollection();
                            if( RecipientIds.Count > 0 )
                            {
                                processMailReport( CswNbtResources, MailReportNode );
                                MailReportNode.postChanges( false );
                            }
                        }
                    }

                    if( false == String.IsNullOrEmpty( InnerErrorMessage ) )
                    {
                        _CswScheduleLogicDetail.StatusMessage = "The following errors occurred during processing: " + InnerErrorMessage;
                    }
                    else
                    {
                        _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    }
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "An exception occurred: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;

                }//catch
            }//if we're not shutting down
        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #region Processing Mail Reports

        private void processMailReport( CswNbtResources _CswNbtResources, CswNbtObjClassMailReport CurrentMailReport )
        {
            string EmailReportStatusMessage = string.Empty;

            if( false == CurrentMailReport.Recipients.Empty )
            {
                Collection<Int32> RecipientIds = CurrentMailReport.Recipients.SelectedUserIds.ToIntCollection();
                int NodeLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                for( Int32 u = 0; u < RecipientIds.Count && u < NodeLimit; u++ )
                {
                    Int32 UserId = CswConvert.ToInt32( RecipientIds[u].ToString() );

                    if( Int32.MinValue != UserId )
                    {
                        CswNbtNode UserNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", UserId )];
                        CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
                        string CurrentEmailAddress = UserNodeAsUser.Email.Trim();
                        if( CurrentEmailAddress != string.Empty )
                        {
                            DataTable ReportTable = null;
                            CswNbtObjClassReport ReportObjClass = null;

                            string EmailMessageSubject = CurrentMailReport.NodeName;
                            string EmailMessageBody = string.Empty;
                            bool SendMail = false;

                            if( "View" == CurrentMailReport.Type.Value )
                            {
                                CswNbtViewId ViewId = CurrentMailReport.ReportView.ViewId;
                                if( ViewId.isSet() )
                                {
                                    CswNbtView ReportView = _CswNbtResources.ViewSelect.restoreView( ViewId );
                                    ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView(
                                        RunAsUser: UserNodeAsUser as ICswNbtUser,
                                        View: ReportView,
                                        RequireViewPermissions: true,
                                        IncludeSystemNodes: false,
                                        IncludeHiddenNodes: false );
                                    //ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( UserNodeAsUser as ICswNbtUser, ReportView, true, true, false, false );

                                    if( ReportTree.getChildNodeCount() > 0 )
                                    {
                                        if( CswEnumNbtMailReportEventOption.Exists.ToString() != CurrentMailReport.Event.Value )
                                        {
                                            // case 27720 - check mail report events to find nodes that match the view results
                                            Dictionary<CswPrimaryKey, string> NodesToMail = new Dictionary<CswPrimaryKey, string>();
                                            foreach( Int32 NodeId in CurrentMailReport.GetNodesToReport().ToIntCollection() )
                                            {
                                                CswPrimaryKey ThisNodeId = new CswPrimaryKey( "nodes", NodeId );
                                                ReportTree.makeNodeCurrent( ThisNodeId );
                                                if( ReportTree.isCurrentNodeDefined() )
                                                {
                                                    NodesToMail.Add( ThisNodeId, ReportTree.getNodeNameForCurrentPosition() );
                                                }
                                            }
                                            if( NodesToMail.Count > 0 )
                                            {
                                                EmailMessageBody = _makeEmailBody( _CswNbtResources, CurrentMailReport, string.Empty, NodesToMail );
                                                SendMail = true;
                                            }
                                        }
                                        else
                                        {
                                            EmailMessageBody = _makeEmailBody( _CswNbtResources, CurrentMailReport, _makeViewLink( _CswNbtResources, ViewId, ReportView.ViewName ) );
                                            SendMail = true;
                                        }
                                    } // if( ReportTree.getChildNodeCount() > 0 )
                                } // if( ViewId.isSet() )
                                else
                                {
                                    EmailReportStatusMessage += "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the associated view's ViewId is not set\r\n";
                                }
                            } // if( "View" == CurrentMailReport.Type.Value )

                            else if( "Report" == CurrentMailReport.Type.Value )
                            {
                                ReportObjClass = (CswNbtObjClassReport) _CswNbtResources.Nodes[CurrentMailReport.Report.RelatedNodeId];
                                if( null != ReportObjClass )
                                {
                                    string ReportSql = CswNbtObjClassReport.ReplaceReportParams( ReportObjClass.SQL.Text, ReportObjClass.ExtractReportParams( UserNode ) );

                                    CswArbitrarySelect ReportSelect = _CswNbtResources.makeCswArbitrarySelect( "MailReport_" + ReportObjClass.NodeId.ToString() + "_Select", ReportSql );
                                    ReportTable = ReportSelect.getTable();

                                    if( ReportTable.Rows.Count > 0 )
                                    {
                                        string ReportLink = string.Empty;
                                        CswEnumNbtMailReportFormatOptions MailRptFormat = (CswEnumNbtMailReportFormatOptions) Enum.Parse( typeof( CswEnumNbtMailReportFormatOptions ), CurrentMailReport.OutputFormat.Value.ToString() );
                                        if( CswEnumNbtMailReportFormatOptions.Link == MailRptFormat )
                                        {
                                            ReportLink = _makeReportLink( _CswNbtResources, ReportObjClass );
                                            ReportTable = null; //so we don't end up attaching the CSV
                                        }

                                        EmailMessageBody = _makeEmailBody( _CswNbtResources, CurrentMailReport, ReportLink );
                                        SendMail = true;
                                    }
                                }
                                else
                                {
                                    EmailReportStatusMessage += "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the associated report's NodeId is not set\r\n";
                                }//if-else report's node id is present
                            } // else if( "Report" == CurrentMailReport.Type.Value )

                            else
                            {
                                EmailReportStatusMessage += "Unable to process email report " + CurrentMailReport.Node.NodeName + ": the report type " + CurrentMailReport.Type.Value + " is unknown\r\n";
                            }//if-else-if on report type


                            if( SendMail )
                            {
                                EmailReportStatusMessage += _sendMailMessage( _CswNbtResources, CurrentMailReport, EmailMessageBody, UserNodeAsUser.LastName, UserNodeAsUser.FirstName, UserNodeAsUser.Node.NodeName, EmailMessageSubject, CurrentEmailAddress, ReportTable ) + "\r\n";
                            }
                        }//if( Email Address != string.Empty )

                    }//if( Int32.MinValue != UserId )

                }//for( Int32 u = 0; u < BatchData.RecipientIds.Count() && u < NodeLimit; u++ )

                CurrentMailReport.RunStatus.AddComment( EmailReportStatusMessage );
                CurrentMailReport.postChanges( false );
            }//if( !CurrentMailReport.Recipients.Empty )
        }//processMailReport()

        private string _sendMailMessage( CswNbtResources _CswNbtResources, CswNbtObjClassMailReport CurrentMailReport, string MailReportMessage, string LastName, string FirstName, string UserName, string Subject, string CurrentEmailAddress, DataTable ReportTable )
        {
            string ReturnVal = string.Empty;
            CswMail CswMail = _CswNbtResources.CswMail;

            ReturnVal += "Recipients: ";

            CswMailMessage MailMessage = new CswMailMessage();
            MailMessage.Recipient = CurrentEmailAddress;
            MailMessage.RecipientDisplayName = FirstName + " " + LastName;
            MailMessage.Subject = Subject;
            MailMessage.Content = MailReportMessage;
            MailMessage.Format = CswEnumMailMessageBodyFormat.HTML;

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
                ReturnVal += UserName + " at " + CurrentEmailAddress + " (succeeded); ";
            }
            else
            {
                ReturnVal += UserName + " at " + CurrentEmailAddress + " (failed: " + CswMail.Status + "); ";
            }
            return ( ReturnVal );

        }//_sendMailMessage()


        private string _makeEmailBody( CswNbtResources _CswNbtResources, CswNbtObjClassMailReport CurrentMailReport, string Link, Dictionary<CswPrimaryKey, string> NodeDict = null )
        {
            string ReturnVal = string.Empty;
            ReturnVal = CurrentMailReport.Message.Text.Replace( "\r\n", "<br>" ) + "<br>";
            if( null != NodeDict )
            {
                foreach( CswPrimaryKey NodeId in NodeDict.Keys )
                {
                    ReturnVal += _makeNodeLink( _CswNbtResources, NodeId, NodeDict[NodeId] ) + "<br>";
                }
            }
            ReturnVal += "<br>" + Link + "<br>";
            return ( ReturnVal );
        }//_makeEmailBody()


        private string _makeViewLink( CswNbtResources _CswNbtResources, CswNbtViewId ViewId, string ViewName )
        {
            return _makeLink( _CswNbtResources, "Main.html?viewid=" + ViewId.ToString(), ViewName );
        }
        private string _makeNodeLink( CswNbtResources _CswNbtResources, CswPrimaryKey NodeId, string NodeName )
        {
            return _makeLink( _CswNbtResources, "Main.html?nodeid=" + NodeId.ToString(), NodeName );
        }
        private string _makeReportLink( CswNbtResources _CswNbtResources, CswNbtObjClassReport ReportObjClass )
        {
            return _makeLink( _CswNbtResources, "Main.html?reportid=" + ReportObjClass.NodeId.ToString(), ReportObjClass.ReportName.Text );
        }
        private string _makeLink( CswNbtResources _CswNbtResources, string Href, string Text )
        {
            string ret = "<a href=\"";
            ret += _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.MailReportUrlStem];
            if( !ret.EndsWith( "/" ) )
            {
                ret += "/";
            }
            ret += Href + "\">" + Text + "</a>";
            return ret;
        }

        #endregion Processing Mail Reports
    }//CswScheduleLogicNbtGenEmailRpt

}//namespace ChemSW.Nbt.Sched
