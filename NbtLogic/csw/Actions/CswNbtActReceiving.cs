using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReceiving
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources;
        private CswNbtSdTabsAndProps _CswNbtSdTabsAndProps;
        private CswNbtMetaDataObjectClass _ContainerOc = null;
        private CswNbtMetaDataObjectClass _MaterialOc = null;
        private CswPrimaryKey _MaterialId = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActReceiving( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
        }

        public CswNbtActReceiving( CswNbtResources CswNbtResources, CswNbtMetaDataObjectClass MaterialOc, CswPrimaryKey MaterialNodeId )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot use the Receive Material action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }
            _MaterialOc = MaterialOc;
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
        }

        #endregion Constructor

        #region Public methods and props

        /// <summary>
        /// Instance a new container according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassContainer makeContainer( Action<CswNbtNode> After )
        {
            CswNbtObjClassContainer ret = null;

            CswNbtMetaDataNodeType ContainerNt = _ContainerOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != ContainerNt )
            {
                Action<CswNbtNode> After2 = delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassContainer RetAsContainer = NewNode;
                        RetAsContainer.Material.RelatedNodeId = _MaterialId;
                        //cases 30647 and 31079
                        //RetAsContainer.Material.setHidden( value: true, SaveToDb: false );
                        //RetAsContainer.Size.setHidden( value: true, SaveToDb: false );
                        if( null != After )
                        {
                            After( NewNode );
                        }
                    };
                ret = _CswNbtSdTabsAndProps.getAddNodeAndPostChanges( ContainerNt, After2 );
                if( null == ret )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Could not create a new container.", "Failed to create a new Container node." );
                }
            }
            return ret;
        } // makeContainer()

        /// <summary>
        /// Get the Add Layout properties for a container
        /// </summary>
        public JObject getContainerAddProps( CswNbtObjClassContainer Container )
        {
            JObject Ret = new JObject();
            if( null != Container )
            {
                Ret = _CswNbtSdTabsAndProps.getProps( Container.Node, "", null, CswEnumNbtLayoutType.Add );
            }
            return Ret;
        }

        /// <summary>
        /// Updates the default Expiration Date on containers to receive based on Receipt Lot's Manufactured Date
        /// </summary>
        public ContainerData.ReceivingData updateExpirationDate( ContainerData.ReceiptLotRequest Request )
        {
            ContainerData.ReceivingData ReceiveData = new ContainerData.ReceivingData();
            JObject ReceiptLotPropsObj = CswConvert.ToJObject( Request.ReceiptLotProps );
            if( ReceiptLotPropsObj.HasValues )
            {
                CswPrimaryKey ReceiptLotId = CswConvert.ToPrimaryKey( Request.ReceiptLotId );
                if( CswTools.IsPrimaryKey( ReceiptLotId ) )
                {
                    CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.GetNode( ReceiptLotId );
                    _CswNbtSdTabsAndProps.saveNodeProps( ReceiptLot.Node, ReceiptLotPropsObj );
                    CswPrimaryKey ContainerId = CswConvert.ToPrimaryKey( Request.ContainerId );
                    if( CswTools.IsPrimaryKey( ContainerId ) &&
                        ReceiptLot.ManufacturedDate.DateTimeValue != DateTime.MinValue )
                    {
                        CswNbtObjClassContainer Container = _CswNbtResources.Nodes.GetNode( ContainerId );
                        CswNbtPropertySetMaterial Material = _CswNbtResources.Nodes.GetNode( Container.Material.RelatedNodeId );
                        Container.ExpirationDate.DateTimeValue = Material.getDefaultExpirationDate( ReceiptLot.ManufacturedDate.DateTimeValue );
                        Container.postChanges( false );
                        JObject ContainerProps = getContainerAddProps( Container );
                        ReceiveData.ContainerProps = ContainerProps.ToString();
                    }
                }
            }
            return ReceiveData;
        }

        public static void AddContainerIdToReceiptDefinition( JObject ReceiptDefinitionObj, int QuantityIdx, string ContainerNodeId )
        {
            try
            {
                CswConvert.ToJArray( ReceiptDefinitionObj["quantities"][QuantityIdx]["containerids"] ).Add( ContainerNodeId );
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Error adding container id to quantity", ex.Message );
            }
        }

        public JObject receiveMaterial( string ReceiptDefinition )
        {
            JObject ReceiptObj = CswConvert.ToJObject( ReceiptDefinition );
            if( ReceiptObj.HasValues )
            {
                CswNbtObjClassContainer InitialContainerNode = _CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["containernodeid"] )];
                if( null != InitialContainerNode )
                {
                    HandleInitialContainer( InitialContainerNode, ReceiptObj );

                    CswNbtBatchOpReceiving ReceivingBatchOp = new CswNbtBatchOpReceiving( _CswNbtResources );
                    ReceivingBatchOp.makeBatchOp( ReceiptObj.ToString() );
                }
            }

            //TODO: return something to the client to do something
            return null;
        }

        //Abstracted outside receiveMaterial() for Unit Test purposes
        public void HandleInitialContainer( CswNbtObjClassContainer InitialContainerNode, JObject ReceiptObj )
        {
            JObject ContainerAddProps = CswConvert.ToJObject( ReceiptObj["props"] );
            _CswNbtSdTabsAndProps.saveNodeProps( InitialContainerNode.Node, ContainerAddProps );
            AddContainerIdToReceiptDefinition( ReceiptObj, 0, InitialContainerNode.NodeId.ToString() );

            CswPrimaryKey MaterialId = new CswPrimaryKey();
            MaterialId.FromString( CswConvert.ToString( ReceiptObj["materialid"] ) );
            commitSDSDocNode( MaterialId, ReceiptObj );
            CswPrimaryKey RequestId = _getRequestId( ReceiptObj );
            CswNbtNode ReceiptLot = _makeReceiptLot( MaterialId, RequestId, ReceiptObj, InitialContainerNode.ExpirationDate.DateTimeValue );
            if( null == ReceiptObj["receiptLotId"] )
            {
                ReceiptObj["receiptLotId"] = ReceiptLot.NodeId.ToString();
            }

            JArray Quants = CswConvert.ToJArray( ReceiptObj["quantities"] );
            JObject QuantityDef = CswConvert.ToJObject( Quants[0] );
            HandleContainer( _CswNbtResources,
                             MaterialId : MaterialId,
                             RequestId : RequestId,
                             ReceiptLotId : ReceiptLot.NodeId,
                             QuantityDef : QuantityDef,
                             ContainerIdx : 0,
                             Apply : delegate( Action<CswNbtNode> After )
                             {
                                 After( InitialContainerNode.Node );
                                 InitialContainerNode.PromoteTempToReal();
                             }
                );
        }

        public static void HandleContainer( CswNbtResources NbtResources, CswPrimaryKey MaterialId, CswPrimaryKey RequestId, CswPrimaryKey ReceiptLotId, JObject QuantityDef, int ContainerIdx, Action<Action<CswNbtNode>> Apply )
        {
            Int32 NoContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );

            CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
            Barcodes.FromString( CswConvert.ToString( QuantityDef["barcodes"] ) );

            double QuantityValue = CswConvert.ToDouble( QuantityDef["quantity"] );

            CswPrimaryKey UnitId = new CswPrimaryKey();
            UnitId.FromString( CswConvert.ToString( QuantityDef["unitid"] ) );

            CswPrimaryKey SizeId = new CswPrimaryKey();
            SizeId.FromString( CswConvert.ToString( QuantityDef["sizeid"] ) );

            CswNbtObjClassSize AsSize = NbtResources.Nodes.GetNode( SizeId );

            CswCommaDelimitedString ContainerIds = new CswCommaDelimitedString();
            ContainerIds.FromString( QuantityDef["containerids"].ToString() );

            Action<CswNbtNode> After = delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassContainer thisContainer = NewNode;
                if( Barcodes.Count <= NoContainers && false == string.IsNullOrEmpty( Barcodes[ContainerIdx] ) )
                {
                    thisContainer.Barcode.setBarcodeValueOverride( Barcodes[ContainerIdx], false );
                }
                thisContainer.Size.RelatedNodeId = SizeId;
                thisContainer.Material.RelatedNodeId = MaterialId;
                if( AsSize.QuantityEditable.Checked != CswEnumTristate.True )
                {
                    QuantityValue = AsSize.InitialQuantity.Quantity;
                    UnitId = AsSize.InitialQuantity.UnitId;
                }
                if( null == thisContainer.Quantity.UnitId || Int32.MinValue == thisContainer.Quantity.UnitId.PrimaryKey )
                {
                    thisContainer.Quantity.UnitId = UnitId;
                }
                thisContainer.DispenseIn( CswEnumNbtContainerDispenseType.Receive, QuantityValue, UnitId, RequestId );
                thisContainer.Disposed.Checked = CswEnumTristate.False;
                thisContainer.ReceiptLot.RelatedNodeId = ReceiptLotId;
            };

            if( NoContainers > 0 && QuantityValue > 0 && Int32.MinValue != UnitId.PrimaryKey )
            {
                Apply( After );
            }
        }

        /// <summary>
        /// Persist the SDS Document
        /// </summary>
        public void commitSDSDocNode( CswPrimaryKey MaterialId, JObject Obj )
        {
            string sdsDocId = CswConvert.ToString( Obj["sdsDocId"] );
            CswNbtObjClassSDSDocument SDSDoc = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( sdsDocId ) );
            if( null != SDSDoc )
            {
                _CswNbtSdTabsAndProps.saveProps( SDSDoc.NodeId, Int32.MinValue, (JObject) Obj["sdsDocProperties"], SDSDoc.NodeTypeId, null, IsIdentityTab : false, setIsTempToFalse : false );
                if( ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( SDSDoc.File.FileName ) ) ||
                    ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( SDSDoc.Link.Href ) ) )
                {
                    SDSDoc.Owner.RelatedNodeId = MaterialId;
                    SDSDoc.PromoteTempToReal();
                }
            }
        }

        #endregion Public methods and props

        #region Private Helper Functions

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

        private CswNbtNode _makeReceiptLot( CswPrimaryKey MaterialId, CswPrimaryKey RequestId, JObject ReceiptObj, DateTime ExpirationDate )
        {
            Action<CswNbtNode> AfterReceiptLot = delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassReceiptLot thisReceiptLot = NewNode;
                    thisReceiptLot.Material.RelatedNodeId = MaterialId;
                    thisReceiptLot.RequestItem.RelatedNodeId = RequestId;
                    thisReceiptLot.ExpirationDate.DateTimeValue = ExpirationDate;
                    //ReceiptLot.postChanges( false );
                };

            string receiptLotId = CswConvert.ToString( ReceiptObj["receiptLotId"] );
            CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( receiptLotId ) );
            if( null != ReceiptLot )
            {
                _CswNbtSdTabsAndProps.saveProps( ReceiptLot.NodeId, Int32.MinValue, (JObject) ReceiptObj["receiptLotProperties"], ReceiptLot.NodeTypeId, null, IsIdentityTab : false );
                AfterReceiptLot( ReceiptLot.Node );
                ReceiptLot.postChanges( false );
            }
            else
            {
                CswNbtMetaDataObjectClass ReceiptLotClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
                ReceiptLot = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ReceiptLotClass.FirstNodeType.NodeTypeId, AfterReceiptLot );
            }
            _attachCofA( ReceiptLot.NodeId, ReceiptObj );
            return ReceiptLot.Node;
        }

        private void _attachCofA( CswPrimaryKey ReceiptLotId, JObject Obj )
        {
            string cofaDocId = CswConvert.ToString( Obj["cofaDocId"] );
            CswNbtObjClassCofADocument CofADoc = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( cofaDocId ) );
            if( null != CofADoc )
            {
                _CswNbtSdTabsAndProps.saveProps( CofADoc.NodeId, Int32.MinValue, (JObject) Obj["cofaDocProperties"], CofADoc.NodeTypeId, null, IsIdentityTab : false );
                if( ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( CofADoc.File.FileName ) ) ||
                    ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( CofADoc.Link.Href ) ) )
                {
                    CofADoc.Owner.RelatedNodeId = ReceiptLotId;
                    CofADoc.PromoteTempToReal();
                }
            }
        }

        #endregion Private Helper Functions
    }
}
