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
            CswNbtObjClassContainer InitialContainerNode = _CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["containernodeid"] )];
            if( null != InitialContainerNode )
            {
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
                        CswNbtNode ReceiptLot = _CswNbtResources.Nodes[RequestId];

                        for( int index = 0; index < Quantities.Count; index++ )
                        {
                            JObject QuantityDef = CswConvert.ToJObject( Quantities[index] );
                            Int32 NoContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );

                            CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
                            Barcodes.FromString( CswConvert.ToString( QuantityDef["barcodes"] ) );

                            double QuantityValue = CswConvert.ToDouble( QuantityDef["quantity"] );

                            CswPrimaryKey UnitId = new CswPrimaryKey();
                            UnitId.FromString( CswConvert.ToString( QuantityDef["unitid"] ) );

                            CswPrimaryKey SizeId = new CswPrimaryKey();
                            SizeId.FromString( CswConvert.ToString( QuantityDef["sizeid"] ) );

                            CswNbtObjClassSize AsSize = _CswNbtResources.Nodes.GetNode( SizeId );

                            CswCommaDelimitedString ContainerIds = new CswCommaDelimitedString();
                            ContainerIds.FromString( QuantityDef["containerids"].ToString() );

                            if( NoContainers > 0 && QuantityValue > 0 && Int32.MinValue != UnitId.PrimaryKey )
                            {
                                for( Int32 C = 0; C < NoContainers && _NodesProcessed >= _MaxNodeProcessed; C += 1 )
                                {
                                    //we promote the first container before the batch op starts, so there should always be at least one container id in the first set of quantities
                                    if( String.IsNullOrEmpty( ContainerIds[C] ) )
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
                            }
                        }//for( int index = 0; index < Quantities.Count; index++ )

                    }//if( null != ContainerNt )
                }//if( Int32.MinValue != ContainerNodeTypeId )

            }//if( null != InitialContainerNode )
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

            private Int32 _TotalContainers = Int32.MinValue;
            public Int32 TotalContainers
            {
                get
                {
                    if( Int32.MinValue == _TotalContainers )
                    {
                        _TotalContainers = _countContainers();
                    }
                    return _TotalContainers;
                }
            }

            /// <summary>
            /// Counts the number of containers this batch op needs to create
            /// </summary>
            private int _countContainers()
            {
                int Ret = Int32.MinValue;
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
                Int32 Ret = Int32.MinValue;

                JArray Quantities = CswConvert.ToJArray( _BatchData["quantities"].ToString(), true );
                for( int index = 0; index < Quantities.Count; index++ )
                {
                    JObject QuantityDef = CswConvert.ToJObject( Quantities[index].ToString(), true, "QuantityDefinition" );
                    if( QuantityDef.HasValues )
                    {
                        int NumContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );
                        CswCommaDelimitedString ContainerIds = new CswCommaDelimitedString();
                        ContainerIds.FromString( QuantityDef["containerids"].ToString() );
                        int ContainerIdsCount = ContainerIds.Count;

                        Ret += ( NumContainers - ContainerIdsCount );
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
