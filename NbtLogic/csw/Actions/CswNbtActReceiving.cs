using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Actions.Receiving;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReceiving
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources;
        private CswNbtSdTabsAndProps _CswNbtSdTabsAndProps;
        private CswNbtMetaDataObjectClass _ContainerOc = null;
        private CswPrimaryKey _MaterialId = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActReceiving( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
        }

        public CswNbtActReceiving( CswNbtResources CswNbtResources, CswPrimaryKey MaterialNodeId )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot use the Receive Material action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
        }

        #endregion Constructor

        #region Public methods and props

        ///<summary>
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

        public JObject receiveMaterial( CswNbtReceivingDefiniton ReceiptDefinition )
        {
            JObject Ret = new JObject();


            CswNbtObjClassContainer InitialContainerNode = _CswNbtResources.Nodes[ReceiptDefinition.ContainerNodeId];
            if( null != InitialContainerNode )
            {
                HandleInitialContainer( InitialContainerNode, ReceiptDefinition );

                CswNbtBatchOpReceiving ReceivingBatchOp = new CswNbtBatchOpReceiving( _CswNbtResources );
                //ReceivingBatchOp.makeBatchOp( ReceiptObj.ToString() ); //TODO: fix batch op to use receipt def

                //TODO: spawn print jobs
                //TODO: get landing page data
            }

            return Ret;
        }

        //Abstracted outside receiveMaterial() for Unit Test purposes
        public void HandleInitialContainer( CswNbtObjClassContainer InitialContainerNode, CswNbtReceivingDefiniton ReceiptObj )
        {
            JObject ContainerAddProps = ReceiptObj.ContainerProps;
            _CswNbtSdTabsAndProps.saveNodeProps( InitialContainerNode.Node, ContainerAddProps );
            ReceiptObj.Quantities[0].ContainerIds.Add( InitialContainerNode.NodeId.ToString() );

            commitSDSDocNode( ReceiptObj.MaterialNodeId, ReceiptObj.SDSNodeId, ReceiptObj.SDSProps );
            CswNbtNode ReceiptLot = _makeReceiptLot( ReceiptObj.MaterialNodeId, ReceiptObj, InitialContainerNode.ExpirationDate.DateTimeValue );
            if( null == ReceiptObj.ReceiptLotNodeId )
            {
                ReceiptObj.ReceiptLotNodeId = ReceiptLot.NodeId;
            }

            Collection<CswNbtAmountsGridQuantity> Quantities = ReceiptObj.Quantities;
            HandleContainer( _CswNbtResources,
                             ReceiptObj,
                             QuantityDef : Quantities[0],
                             Barcode : Quantities[0].Barcodes.FirstOrDefault(),
                             Apply : delegate( Action<CswNbtNode> After )
                             {
                                 After( InitialContainerNode.Node );
                                 InitialContainerNode.PromoteTempToReal();
                             }
                );
        }

        public static void HandleContainer( CswNbtResources NbtResources, CswNbtReceivingDefiniton ReceiptDef, CswNbtAmountsGridQuantity QuantityDef, string Barcode, Action<Action<CswNbtNode>> Apply )
        {
            CswNbtObjClassSize AsSize = NbtResources.Nodes.GetNode( QuantityDef.SizeNodeId );

            Action<CswNbtNode> After = delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassContainer thisContainer = NewNode;
                if( QuantityDef.Barcodes.Count <= QuantityDef.NumContainers && false == string.IsNullOrEmpty( Barcode ) )
                {
                    thisContainer.Barcode.setBarcodeValueOverride( Barcode, false );
                }
                thisContainer.Size.RelatedNodeId = QuantityDef.SizeNodeId;
                thisContainer.Material.RelatedNodeId = ReceiptDef.MaterialNodeId;
                if( AsSize.QuantityEditable.Checked != CswEnumTristate.True )
                {
                    QuantityDef.Quantity = AsSize.InitialQuantity.Quantity;
                    QuantityDef.UnitNodeId = AsSize.InitialQuantity.UnitId;
                }
                if( null == thisContainer.Quantity.UnitId || Int32.MinValue == thisContainer.Quantity.UnitId.PrimaryKey )
                {
                    thisContainer.Quantity.UnitId = QuantityDef.UnitNodeId;
                }
                thisContainer.DispenseIn( CswEnumNbtContainerDispenseType.Receive, QuantityDef.Quantity, QuantityDef.UnitNodeId, ReceiptDef.RequestItemtNodeId );
                thisContainer.Disposed.Checked = CswEnumTristate.False;
                thisContainer.ReceiptLot.RelatedNodeId = ReceiptDef.ReceiptLotNodeId;
            };

            if( QuantityDef.NumContainers > 0 && QuantityDef.Quantity > 0 && Int32.MinValue != QuantityDef.UnitNodeId.PrimaryKey )
            {
                Apply( After );
            }
        }

        /// <summary>
        /// Persist the SDS Document
        /// </summary>
        public void commitSDSDocNode( CswPrimaryKey MaterialId, CswPrimaryKey SDSNodeId, JObject SDSProps )
        {
            CswNbtObjClassSDSDocument SDSDoc = _CswNbtResources.Nodes.GetNode( SDSNodeId );
            if( null != SDSDoc )
            {
                _CswNbtSdTabsAndProps.saveProps( SDSDoc.NodeId, Int32.MinValue, SDSProps, SDSDoc.NodeTypeId, null, IsIdentityTab : false, setIsTempToFalse : false );
                if( ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( SDSDoc.File.FileName ) ) ||
                    ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( SDSDoc.Link.Href ) ) )
                {
                    SDSDoc.Owner.RelatedNodeId = MaterialId;
                    SDSDoc.PromoteTempToReal();
                }
            }
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

        #endregion Public methods and props

        #region Private Helper Functions

        private CswNbtNode _makeReceiptLot( CswPrimaryKey MaterialId, CswNbtReceivingDefiniton ReceiptDef, DateTime ExpirationDate )
        {
            Action<CswNbtNode> AfterReceiptLot = delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassReceiptLot thisReceiptLot = NewNode;
                thisReceiptLot.Material.RelatedNodeId = MaterialId;
                thisReceiptLot.RequestItem.RelatedNodeId = ReceiptDef.RequestItemtNodeId;
                thisReceiptLot.ExpirationDate.DateTimeValue = ExpirationDate;
            };

            CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.GetNode( ReceiptDef.ReceiptLotNodeId );
            if( null != ReceiptDef.ReceiptLotNodeId && null != ReceiptLot )
            {
                _CswNbtSdTabsAndProps.saveProps( ReceiptLot.NodeId, Int32.MinValue, ReceiptDef.ReceiptLotProps, ReceiptLot.NodeTypeId, null, IsIdentityTab : false );
                AfterReceiptLot( ReceiptLot.Node );
                ReceiptLot.postChanges( false );
            }
            else
            {
                CswNbtMetaDataObjectClass ReceiptLotClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
                ReceiptLot = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ReceiptLotClass.FirstNodeType.NodeTypeId, AfterReceiptLot );
            }
            _attachCofA( ReceiptLot.NodeId, ReceiptDef );
            return ReceiptLot.Node;
        }

        private void _attachCofA( CswPrimaryKey ReceiptLotId, CswNbtReceivingDefiniton ReceiptDef )
        {
            CswNbtObjClassCofADocument CofADoc = _CswNbtResources.Nodes.GetNode( ReceiptDef.CofADocNodeId );
            if( null != CofADoc )
            {
                _CswNbtSdTabsAndProps.saveProps( CofADoc.NodeId, Int32.MinValue, ReceiptDef.CofAPropsJSON, CofADoc.NodeTypeId, null, IsIdentityTab : false );
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