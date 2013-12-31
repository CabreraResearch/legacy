using System;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

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

        public void OverrideMaxProcessed( int NewMax )
        {
            _MaxNodeProcessed = NewMax;
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
                int totalContainers = BatchData.CountTotalContainers();
                ret = Math.Round( (Double) ( totalContainers - containersLeft ) / totalContainers * 100, 0 );
            }
            return ret;
        } // getPercentDone()

        public JArray getContainerIds( CswNbtObjClassBatchOp Op )
        {
            ReceivingBatchData Data = new ReceivingBatchData( Op.BatchData.Text );
            return Data.getContainerIds();
        }

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
                    _receiveContainers( BatchData );
                    
                    if( 0 == BatchData.getNumberContainersToCreate() || 0 == _NodesProcessed )
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

        private static CswPrimaryKey _getRequestId( JObject ReceiptObj )
        {
            CswPrimaryKey RequestId = null;
            if( ReceiptObj["requestitem"] != null )
            {
                RequestId = new CswPrimaryKey();
                RequestId.FromString( CswConvert.ToString( ReceiptObj["requestitem"]["requestitemid"] ) );
                if( false == CswTools.IsPrimaryKey( RequestId ) )
                {
                    RequestId = null;
                }
            }
            return RequestId;
        }

        private void _receiveContainers( ReceivingBatchData BatchData )
        {
            JObject ReceiptObj = BatchData.ReceiptDefinitionObj;
            JObject ContainerAddProps = CswConvert.ToJObject( ReceiptObj["props"] );
            Int32 ContainerNodeTypeId = CswConvert.ToInt32( ReceiptObj["containernodetypeid"] );
            if( Int32.MinValue != ContainerNodeTypeId )
            {
                CswNbtMetaDataNodeType ContainerNt = _CswNbtResources.MetaData.getNodeType( ContainerNodeTypeId );
                CswPrimaryKey MaterialId = new CswPrimaryKey();
                MaterialId.FromString( CswConvert.ToString( ReceiptObj["materialid"] ) );
                JArray Quantities = CswConvert.ToJArray( ReceiptObj["quantities"] );
                if( null != ContainerNt && CswTools.IsPrimaryKey( MaterialId ) && Quantities.HasValues )
                {
                    CswPrimaryKey RequestId = _getRequestId( ReceiptObj );
                    CswPrimaryKey ReceiptLotId = CswConvert.ToPrimaryKey( ReceiptObj["receiptLotId"].ToString() );
                    CswNbtNode ReceiptLot = _CswNbtResources.Nodes[ReceiptLotId];

                    for( int index = 0; index < Quantities.Count; index++ )
                    {
                        JObject QuantityDef = CswConvert.ToJObject( Quantities[index] );
                        Int32 NoContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );

                        CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
                        Barcodes.FromString( CswConvert.ToString( QuantityDef["barcodes"] ) );

                        JArray ContainerIds = CswConvert.ToJArray( QuantityDef["containerids"].ToString() );

                        for( Int32 C = 0; C < NoContainers; C += 1 )
                        {
                            //we promote the first container before the batch op starts, so there should always be at least one container id in the first set of quantities
                            if( C >= ContainerIds.Count && _NodesProcessed < _MaxNodeProcessed ) //only create a container where we haven't already
                            {
                                CswNbtActReceiving.HandleContainer( _CswNbtResources, MaterialId, RequestId, ReceiptLot.NodeId, QuantityDef, C, delegate( Action<CswNbtNode> After )
                                    {
                                        CswNbtNodeKey ContainerNodeKey;
                                        CswNbtObjClassContainer AsContainer = _CswNbtSdTabsAndProps.addNode( ContainerNt, null, ContainerAddProps, out ContainerNodeKey, After );
                                        CswNbtActReceiving.AddContainerIdToReceiptDefinition( BatchData.ReceiptDefinitionObj, index, AsContainer.NodeId.ToString() );
                                        _NodesProcessed++;
                                    } );
                            }
                        } //for( Int32 C = 0; C < NoContainers; C += 1 )
                    }//for( int index = 0; index < Quantities.Count; index++ )
                }//if( null != ContainerNt )
            }//if( Int32.MinValue != ContainerNodeTypeId )
        }

        #region ReceivingBatchData

        // This internal class is specific to this batch operation
        private class ReceivingBatchData
        {
            private readonly JObject _BatchData;

            public ReceivingBatchData( string ReceiptDefinition )
            {
                _BatchData = CswConvert.ToJObject( ReceiptDefinition, true, "ReceiptDefinitionForReceivingBatchOp" );
            }

            public JObject ReceiptDefinitionObj
            {
                get { return _BatchData; }
            }

            public Int32 CountTotalContainers()
            {
                int Ret = 0;
                JArray Quantities = CswConvert.ToJArray( _BatchData["quantities"].ToString(), true );
                for( int index = 0; index < Quantities.Count; index++ )
                {
                    JObject QuantityDef = CswConvert.ToJObject( Quantities[index].ToString(), true, "QuantifyDefinition" );
                    if( QuantityDef.HasValues )
                    {
                        int NumContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );
                        if( Int32.MinValue != NumContainers )
                        {
                            Ret += NumContainers;
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Cannot calculate number of containers to create", "Receipt Definition is missing number of containers for a quantity." );
                        }
                    }
                }
                return Ret;
            }

            public Int32 getNumberContainersToCreate()
            {
                Int32 Ret = 0;

                JArray Quantities = CswConvert.ToJArray( _BatchData["quantities"].ToString(), true );
                for( int index = 0; index < Quantities.Count; index++ )
                {
                    JObject QuantityDef = CswConvert.ToJObject( Quantities[index].ToString(), true, "QuantityDefinition" );
                    if( QuantityDef.HasValues )
                    {
                        int NumContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );
                        JArray ContainerIds = CswConvert.ToJArray( QuantityDef["containerids"].ToString() );
                        int ContainerIdsCount = ContainerIds.Count;

                        Ret += ( NumContainers - ContainerIdsCount );
                    }
                }

                return Ret;
            }

            public JArray getContainerIds()
            {
                JArray Ret = new JArray();
                JArray Quantities = CswConvert.ToJArray( _BatchData["quantities"].ToString(), true );
                for( int index = 0; index < Quantities.Count; index++ )
                {
                    JObject QuantityDef = CswConvert.ToJObject( Quantities[index].ToString(), true, "QuantityDefinition" );
                    if( QuantityDef.HasValues )
                    {
                        JArray ContainerIds = CswConvert.ToJArray( QuantityDef["containerids"].ToString() );
                        for( int i = 0; i < ContainerIds.Count; i++ )
                        {
                            Ret.Add( ContainerIds[i] );
                        }
                    }
                }

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
