using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpMailReport : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.MailReport;
        CswScheduleLogicNbtGenEmailRpt MailReportProcessor = new CswScheduleLogicNbtGenEmailRpt();

        public CswNbtBatchOpMailReport( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            MailReportProcessor.init( _CswNbtResources, null );
        }

        /// <summary>
        /// Create a new batch operation to handle the execution of a single mail report
        /// </summary>
        /// <param name="MailReportNodeId"></param>
        public CswNbtObjClassBatchOp makeBatchOp( CswPrimaryKey MailReportNodeId )
        {
            Collection<CswPrimaryKey> MailReportPK = new Collection<CswPrimaryKey>();
            MailReportPK.Add( MailReportNodeId );
            return makeBatchOp( MailReportPK );
        }

        /// <summary>
        /// Create a new batch operation to handle the execution of one or more mail reports
        /// </summary>
        /// <param name="MailReportNodeIds"></param>
        public CswNbtObjClassBatchOp makeBatchOp( Collection<CswPrimaryKey> MailReportNodeIds )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            MailReportBatchData BatchData = new MailReportBatchData( string.Empty );
            BatchData.MailReportNodeIds = _pkArrayToJArray( MailReportNodeIds );
            BatchData.StartingCount = MailReportNodeIds.Count();
            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        }

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.MailReport )
            {
                MailReportBatchData BatchData = new MailReportBatchData( BatchNode.BatchData.Text );
                if( BatchData.StartingCount > 0 )
                {
                    ret = Math.Round( (Double) ( BatchData.StartingCount - BatchData.MailReportNodeIds.Count() ) / BatchData.StartingCount * 100, 0 );
                }
            }
            return ret;
        }

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
                    MailReportBatchData BatchData = new MailReportBatchData( BatchNode.BatchData.Text );
                    if( BatchData.MailReportNodeIds.Count > 0 )
                    {
                        string NodeIdStr = BatchData.MailReportNodeIds.First.ToString();
                        _processMailReportNode( NodeIdStr );

                        // Setup for next iteration
                        BatchData.MailReportNodeIds.RemoveAt( 0 );
                        BatchNode.BatchData.Text = BatchData.ToString();
                        BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    }
                    else
                    {
                        BatchNode.finish();
                    }
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }
        }

        #region Private Helper Functions

        private JArray _pkArrayToJArray( Collection<CswPrimaryKey> strArray )
        {
            JArray ret = new JArray();
            foreach( CswPrimaryKey k in strArray )
            {
                ret.Add( k.ToString() );
            }
            return ret;
        }

        private void _processMailReportNode( string NodePk )
        {
            CswPrimaryKey MailReportNodePk = new CswPrimaryKey();
            MailReportNodePk.FromString( NodePk );

            if( Int32.MinValue != MailReportNodePk.PrimaryKey )
            {
                CswNbtNode MailReportNode = _CswNbtResources.Nodes[MailReportNodePk];
                if( MailReportNode != null )
                {
                    CswNbtObjClassMailReport NodeAsMailReport = MailReportNode;
                    string statusMessage = MailReportProcessor.processMailReport( NodeAsMailReport, new CswNbtMailReportStatus() );
                    NodeAsMailReport.RunStatus.AddComment( statusMessage );
                    NodeAsMailReport.postChanges( false );
                }
            }
        }

        #endregion

        #region MailReportBatchData

        private class MailReportBatchData
        {
            private JObject _BatchData;

            public MailReportBatchData( string BatchData )
            {
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
            }

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
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        }

        #endregion MailReportBatchData
    }
}
