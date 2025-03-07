﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Actions.Receiving;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpReceiving: ICswNbtBatchOp
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.Receiving;

        private CswNbtSdTabsAndProps _CswNbtSdTabsAndProps;
        private int _NodesProcessed = 0; //the number of nodes processed this run of the batch op
        private int _MaxNodeProcessed = 10;

        public CswNbtBatchOpReceiving( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _MaxNodeProcessed = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
        }

        /// <summary>
        /// To override the maximum number of containers created per run (for Unit Tests)
        /// </summary>
        public void OverrideMaxProcessed( int NewMax )
        {
            _MaxNodeProcessed = NewMax;
        }

        public CswNbtReceivingDefinition GetBatchData( CswNbtObjClassBatchOp Op )
        {
            ReceivingBatchData BatchData = new ReceivingBatchData( Op.BatchData.Text );
            return BatchData.ReceiptDef;
        }

        /// <summary>
        /// Create a new batch operation to handle creation of Containers from the receiving wizard
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswNbtReceivingDefinition ReceiptDefinition )
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
                int containersLeft = BatchData.CountNumberContainersToCreate();
                int totalContainers = BatchData.CountTotalContainers();
                ret = Math.Round( (Double) ( totalContainers - containersLeft ) / totalContainers * 100, 0 );
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
                if( BatchNode.OpNameValue == CswEnumNbtBatchOpName.Receiving )
                {
                    _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );

                    BatchNode.start();

                    ReceivingBatchData BatchData = new ReceivingBatchData( BatchNode.BatchData.Text );

                    CswNbtActReceiving ActReceiving = new CswNbtActReceiving( _CswNbtResources );
                    CswNbtReceivingDefinition UpdatedReceiptDef = ActReceiving.receiveContainers( BatchData.ReceiptDef, ref _NodesProcessed, _MaxNodeProcessed );
                    
                    ReceivingBatchData UpdatedBatchData = new ReceivingBatchData( UpdatedReceiptDef );
                    BatchNode.BatchData.Text = UpdatedBatchData.ToString();

                    if( 0 == BatchData.CountNumberContainersToCreate() || 0 == _NodesProcessed )
                    {
                        BatchNode.finish();
                    }

                    _NodesProcessed = 0; //reset this for Unit Test purposes

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

        private class ReceivingBatchData
        {
            private CswNbtReceivingDefinition _receiptDef;
            private readonly Type _receiptDefType = typeof( CswNbtReceivingDefinition );

            public ReceivingBatchData( CswNbtReceivingDefinition ReceiptDefinition )
            {
                _receiptDef = ReceiptDefinition;
            }

            public ReceivingBatchData( string ReceiptDefinitionStr )
            {
                _receiptDef = CswSerialize<CswNbtReceivingDefinition>.ToObject( ReceiptDefinitionStr );
            }

            public CswNbtReceivingDefinition ReceiptDef { get { return _receiptDef; } }

            public int CountNumberContainersToCreate()
            {
                int ret = 0;
                foreach( CswNbtAmountsGridQuantity Quantity in _receiptDef.Quantities )
                {
                    ret += Quantity.NumContainers - ( Quantity.ContainerIds.Count );
                }
                return ret;
            }

            public int CountTotalContainers()
            {
                int ret = 0;
                foreach( CswNbtAmountsGridQuantity Quantity in _receiptDef.Quantities )
                {
                    ret += Quantity.NumContainers;
                }
                return ret;
            }

            public override string ToString()
            {
                string Ret = string.Empty;
                using( MemoryStream ms = new MemoryStream() )
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer( _receiptDefType );
                    serializer.WriteObject( ms, _receiptDef );
                    Ret = Encoding.UTF8.GetString( ms.GetBuffer(), 0, CswConvert.ToInt32( ms.Length ) );
                }
                return Ret;
            }

        }

        #endregion ReceivingBatchData

    } // class CswNbtBatchOpReceiving
} // namespace ChemSW.Nbt.Batch
