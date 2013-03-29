﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Mail;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMailReport : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MailReport;
        private Int32 NodeLimit = 10;   // TODO: change me in Titania to use NodesProcessedPerIteration

        public CswNbtBatchOpMailReport( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle the execution of one or more mail reports
        /// </summary>
        /// <param name="MailReportNodeIds"></param>
        public CswNbtObjClassBatchOp makeBatchOp( Collection<CswPrimaryKey> MailReportNodeIds )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            MailReportBatchData BatchData = new MailReportBatchData( _CswNbtResources, string.Empty );
            BatchData.MailReportNodeIds = _pkArrayToJArray( MailReportNodeIds );
            BatchData.StartingCount = MailReportNodeIds.Count();
            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        } // makeBatchOp()

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MailReport )
            {
                MailReportBatchData BatchData = new MailReportBatchData( _CswNbtResources, BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    if( 0 == BatchData.MailReportNodeIds.Count() )
                    {
                        if( BatchData.RecipientIds.Count() > 0 )
                        {
                            // Processing the last one, special case
                            ret = 99;
                        }
                        else
                        {
                            // Done
                            ret = 100;
                        }
                    }
                    else
                    {
                        ret = Math.Round( ( (Double) ( BatchData.StartingCount - BatchData.MailReportNodeIds.Count() ) / BatchData.StartingCount ) * 100, 0 );
                    }
                }
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MailReport )
                {
                    BatchNode.start();
                    MailReportBatchData BatchData = new MailReportBatchData( _CswNbtResources, BatchNode.BatchData.Text );

                    if( null != BatchData.CurrentMailReport )
                    {
                        if( BatchData.RecipientIds.Count > 0 )
                        {
                            processMailReport( BatchData );
                            BatchData.CurrentMailReport.postChanges( false );
                        }
                        else
                        {
                            // case 27720 - clear existing Mail Report Event records for this mail report
                            BatchData.CurrentMailReport.ClearNodesToReport();
                            BatchData.CurrentMailReport.LastProcessed.DateTimeValue = DateTime.Now;
                            BatchData.CurrentMailReport.postChanges( false );

                            BatchData.CurrentMailReportId = string.Empty;
                        }
                    }
                    else if( BatchData.MailReportNodeIds.Count > 0 )
                    {
                        BatchData.CurrentMailReportId = BatchData.MailReportNodeIds.First.ToString();
                        BatchData.MailReportNodeIds.RemoveAt( 0 );

                        if( null != BatchData.CurrentMailReport )
                        {
                            BatchData.RecipientIds = _intArrayToJArray( BatchData.CurrentMailReport.Recipients.SelectedUserIds.ToIntCollection() );
                        }
                    }
                    else
                    {
                        BatchNode.finish();
                    }

                    // Setup for next iteration
                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchNode.postChanges( false );
                } // if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MailReport )
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }
        } // runBatchOp()

        #region Private Helper Functions

        private JArray _pkArrayToJArray( Collection<CswPrimaryKey> pkColl )
        {
            JArray ret = new JArray();
            foreach( CswPrimaryKey k in pkColl )
            {
                ret.Add( k.ToString() );
            }
            return ret;
        }

        private JArray _intArrayToJArray( Collection<Int32> intColl )
        {
            JArray ret = new JArray();
            foreach( Int32 k in intColl )
            {
                ret.Add( k.ToString() );
            }
            return ret;
        }

        #endregion

        #region MailReportBatchData

        private class MailReportBatchData
        {
            private JObject _BatchData;
            private CswNbtResources _CswNbtResources;

            public MailReportBatchData( CswNbtResources Resources, string BatchData )
            {
                _CswNbtResources = Resources;
                if( BatchData != string.Empty )
                {
                    _BatchData = JObject.Parse( BatchData );
                }
                else
                {
                    _BatchData = new JObject();
                }
            }

            private JArray _MailReportNodeIds = null;
            public JArray MailReportNodeIds
            {
                get
                {
                    if( null == _MailReportNodeIds )
                    {
                        if( null != _BatchData["mailreportnodeids"] )
                        {
                            _MailReportNodeIds = (JArray) _BatchData["mailreportnodeids"];
                        }
                    }
                    return _MailReportNodeIds;
                }
                set
                {
                    _MailReportNodeIds = value;
                    _BatchData["mailreportnodeids"] = _MailReportNodeIds;
                }
            } // MailReportNodeIds

            private string _CurrentMailReportId = null;
            public string CurrentMailReportId
            {
                get
                {
                    if( null == _CurrentMailReportId )
                    {
                        if( null != _BatchData["currentmailreportid"] )
                        {
                            _CurrentMailReportId = _BatchData["currentmailreportid"].ToString();
                        }
                    }
                    return _CurrentMailReportId;
                }
                set
                {
                    _CurrentMailReportId = value;
                    _BatchData["currentmailreportid"] = _CurrentMailReportId;
                    _CurrentMailReport = null;
                }
            } // CurrentMailReportId

            private CswNbtObjClassMailReport _CurrentMailReport = null;
            public CswNbtObjClassMailReport CurrentMailReport
            {
                get
                {
                    if( null == _CurrentMailReport && false == String.IsNullOrEmpty( CurrentMailReportId ) )
                    {
                        CswPrimaryKey MailReportNodePk = new CswPrimaryKey();
                        MailReportNodePk.FromString( CurrentMailReportId );
                        if( Int32.MinValue != MailReportNodePk.PrimaryKey )
                        {
                            _CurrentMailReport = _CswNbtResources.Nodes[MailReportNodePk];
                        }
                    }
                    return _CurrentMailReport;
                }
            } // CurrentMailReport

            private JArray _RecipientIds = null;
            public JArray RecipientIds
            {
                get
                {
                    if( null == _RecipientIds )
                    {
                        if( null != _BatchData["recipientids"] )
                        {
                            _RecipientIds = (JArray) _BatchData["recipientids"];
                        }
                    }
                    return _RecipientIds;
                }
                set
                {
                    _RecipientIds = value;
                    _BatchData["recipientids"] = _RecipientIds;
                }
            } // RecipientIds

            private Int32 _StartingCount = Int32.MinValue;
            public Int32 StartingCount
            {
                get
                {
                    if( Int32.MinValue == _StartingCount )
                    {
                        if( null != _BatchData["startingcount"] )
                        {
                            _StartingCount = CswConvert.ToInt32( _BatchData["startingcount"].ToString() );
                        }
                    }
                    return _StartingCount;
                }
                set
                {
                    _StartingCount = value;
                    _BatchData["startingcount"] = _StartingCount;
                }
            } // StartingCount

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class MailReportBatchData

        #endregion MailReportBatchData



        #region Processing Mail Reports

        private void processMailReport( MailReportBatchData BatchData )
        {
            CswNbtObjClassMailReport CurrentMailReport = BatchData.CurrentMailReport;

            string EmailReportStatusMessage = string.Empty;

            if( false == CurrentMailReport.Recipients.Empty )
            {
                // Copy recipients
                JArray NewRecipientIds = new JArray();
                foreach(string UserId in BatchData.RecipientIds)
                {
                    NewRecipientIds.Add( UserId );
                }

                for( Int32 u = 0; u < BatchData.RecipientIds.Count() && u < NodeLimit; u++ )
                {
                    Int32 UserId = CswConvert.ToInt32( BatchData.RecipientIds[u].ToString() );
                    NewRecipientIds.RemoveAt( 0 );

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
                                        if( CswNbtObjClassMailReport.EventOption.Exists.ToString() != CurrentMailReport.Event.Value )
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
                                                EmailMessageBody = _makeEmailBody( CurrentMailReport, string.Empty, NodesToMail );
                                                SendMail = true;
                                            }
                                        }
                                        else
                                        {
                                            EmailMessageBody = _makeEmailBody( CurrentMailReport, _makeViewLink( ViewId, ReportView.ViewName ) );
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
                                        MailRptFormatOptions MailRptFormat = (MailRptFormatOptions) Enum.Parse( typeof( MailRptFormatOptions ), CurrentMailReport.OutputFormat.Value.ToString() );
                                        if( MailRptFormatOptions.Link == MailRptFormat )
                                        {
                                            ReportLink = _makeReportLink( ReportObjClass );
                                            ReportTable = null; //so we don't end up attaching the CSV
                                        }

                                        EmailMessageBody = _makeEmailBody( CurrentMailReport, ReportLink );
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
                                EmailReportStatusMessage += _sendMailMessage( CurrentMailReport, EmailMessageBody, UserNodeAsUser.LastName, UserNodeAsUser.FirstName, UserNodeAsUser.Node.NodeName, EmailMessageSubject, CurrentEmailAddress, ReportTable ) + "\r\n";
                            }
                        }//if( Email Address != string.Empty )

                    }//if( Int32.MinValue != UserId )

                }//for( Int32 u = 0; u < BatchData.RecipientIds.Count() && u < NodeLimit; u++ )

                BatchData.RecipientIds = NewRecipientIds;

                CurrentMailReport.RunStatus.AddComment( EmailReportStatusMessage );
                CurrentMailReport.postChanges( false );
            }//if( !CurrentMailReport.Recipients.Empty )
        }//processMailReport()

        private string _sendMailMessage( CswNbtObjClassMailReport CurrentMailReport, string MailReportMessage, string LastName, string FirstName, string UserName, string Subject, string CurrentEmailAddress, DataTable ReportTable )
        {
            string ReturnVal = string.Empty;
            CswMail CswMail = _CswNbtResources.CswMail;

            ReturnVal += "Recipients: ";

            CswMailMessage MailMessage = new CswMailMessage();
            MailMessage.Recipient = CurrentEmailAddress;
            MailMessage.RecipientDisplayName = FirstName + " " + LastName;
            MailMessage.Subject = Subject;
            MailMessage.Content = MailReportMessage;
            MailMessage.Format = CswMailMessageBodyFormat.HTML;

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


        private string _makeEmailBody( CswNbtObjClassMailReport CurrentMailReport, string Link, Dictionary<CswPrimaryKey, string> NodeDict = null )
        {
            string ReturnVal = string.Empty;
            ReturnVal = CurrentMailReport.Message.Text.Replace( "\r\n", "<br>" ) + "<br>";
            if( null != NodeDict )
            {
                foreach( CswPrimaryKey NodeId in NodeDict.Keys )
                {
                    ReturnVal += _makeNodeLink( NodeId, NodeDict[NodeId] ) + "<br>";
                }
            }
            ReturnVal += "<br>" + Link + "<br>";
            return ( ReturnVal );
        }//_makeEmailBody()


        private string _makeViewLink( CswNbtViewId ViewId, string ViewName )
        {
            return _makeLink( "Main.html?viewid=" + ViewId.ToString(), ViewName );
        }
        private string _makeNodeLink( CswPrimaryKey NodeId, string NodeName )
        {
            return _makeLink( "Main.html?nodeid=" + NodeId.ToString(), NodeName );
        }
        private string _makeReportLink( CswNbtObjClassReport ReportObjClass )
        {
            return _makeLink( "Main.html?reportid=" + ReportObjClass.NodeId.ToString(), ReportObjClass.ReportName.Text );
        }
        private string _makeLink( string Href, string Text )
        {
            string ret = "<a href=\"";
            ret += _CswNbtResources.SetupVbls["MailReportUrlStem"];
            if( !ret.EndsWith( "/" ) )
            {
                ret += "/";
            }
            ret += Href + "\">" + Text + "</a>";
            return ret;
        }

        #endregion Processing Mail Reports

    }
}
