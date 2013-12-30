using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpReceiving: ICswNbtBatchOp
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.Receiving;

        public CswNbtBatchOpReceiving( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Create a new batch operation to handle creation of Containers from the receiving wizard
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( string ReceiptDefinition )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            ReceivingBatchData BatchData = new ReceivingBatchData( ReceiptDefinition );

            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData.ToString() );
            return BatchNode;
        } // makeBatchOp()

        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 100;
            if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.Receiving )
            {
                ReceivingBatchData BatchData = new ReceivingBatchData( BatchNode.BatchData.Text );
                int containersLeft = BatchData.getNumberContainersToCreate();
                ret = Math.Round( (Double) ( BatchData.TotalContainers - containersLeft ) / BatchData.TotalContainers * 100, 0 );
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
                if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.Receiving )
                {
                    BatchNode.start();

                    ReceivingBatchData BatchData = new ReceivingBatchData( BatchNode.BatchData.Text );

                    //if( BatchData.NodePks.Count > 0 )
                    //{
                    //    int NodesProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    //    for( int i = 0; i < NodesProcessedPerIteration && BatchData.NodePks.Count > 0; i++ )
                    //    {
                    //        CswNbtNode Node = _CswNbtResources.Nodes[CswConvert.ToString( BatchData.NodePks[0] )];
                    //        BatchData.NodePks.RemoveAt( 0 );
                    //        bool LocationUpdated = false;
                    //        if( null != Node )
                    //        {
                    //            CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                    //            if( null != NodeType )
                    //            {
                    //                CswNbtMetaDataNodeTypeProp LocationNTP = NodeType.getLocationProperty();
                    //                if( null != LocationNTP )
                    //                {
                    //                    CswNbtNodePropLocation LocationProp = Node.Properties[LocationNTP];
                    //                    if( null != LocationProp )
                    //                    {
                    //                        LocationProp.SelectedNodeId = BatchData.LocationId;
                    //                        Node.postChanges( false );
                    //                        LocationUpdated = true;
                    //                    }
                    //                }
                    //            }
                    //        }//if( null != Node )
                    //
                    //        if( false == LocationUpdated )
                    //        {
                    //            BatchNode.appendToLog( "Unable to update the location of: " + Node.NodeName + " (" + Node.NodeId.ToString() + ")" );
                    //        }
                    //
                    //    }//for( int i = 0; i < NodesProcessedPerIteration && BatchData.NodePks.Count > 0; i++ )
                    //}
                    //else
                    //{
                    //    BatchNode.finish();
                    //}

                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                BatchNode.error( ex );
            }

        } // runBatchOp()

        #region ReceivingBatchData

        // This internal class is specific to this batch operation
        private class ReceivingBatchData
        {
            private readonly JObject _BatchData;

            public ReceivingBatchData( string ReceiptDefinition )
            {
                _BatchData = CswConvert.ToJObject( ReceiptDefinition, true, "ReceiptDefinitionForReceivingBatchOp" );
            }

            private Int32 _TotalContainers = Int32.MinValue;
            public Int32 TotalContainers
            {
                get
                {
                    if( Int32.MinValue == _TotalContainers )
                    {
                        //TODO: count number of containers to be created and set _TotalContainers
                    }
                    return _TotalContainers;
                }
            }

            public Int32 getNumberContainersToCreate()
            {
                Int32 Ret = Int32.MinValue;
                //TODO: calculate how many containers are left to create
                return Ret;
            }

            public override string ToString()
            {
                return _BatchData.ToString();
            }
        } // class ReceivingBatchData

        #endregion ReceivingBatchData

    } // class CswNbtBatchOpReceiving
} // namespace ChemSW.Nbt.Batch
