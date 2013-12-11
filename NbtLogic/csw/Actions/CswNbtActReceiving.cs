using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
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
                        After( NewNode );
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

        /// <summary>
        /// Create new containers and return the number of containers succcesfully created and a view of said containers. 
        /// </summary>
        public JObject receiveMaterial( string ReceiptDefinition )
        {
            JObject Ret = new JObject();
            JObject ReceiptObj = CswConvert.ToJObject( ReceiptDefinition );
            Collection<CswPrimaryKey> ContainerIds = new Collection<CswPrimaryKey>();
            if( ReceiptObj.HasValues )
            {
                CswNbtObjClassContainer InitialContainerNode = _CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["containernodeid"] )];
                if( null != InitialContainerNode )
                {
                    JObject ContainerAddProps = CswConvert.ToJObject( ReceiptObj["props"] );
                    _CswNbtSdTabsAndProps.saveNodeProps( InitialContainerNode.Node, ContainerAddProps );
                    Int32 ContainerNodeTypeId = CswConvert.ToInt32( ReceiptObj["containernodetypeid"] );
                    if( Int32.MinValue != ContainerNodeTypeId )
                    {
                        CswNbtMetaDataNodeType ContainerNt = _CswNbtResources.MetaData.getNodeType( ContainerNodeTypeId );
                        CswPrimaryKey MaterialId = new CswPrimaryKey();
                        MaterialId.FromString( CswConvert.ToString( ReceiptObj["materialid"] ) );
                        JArray Quantities = CswConvert.ToJArray( ReceiptObj["quantities"] );
                        if( null != ContainerNt && CswTools.IsPrimaryKey( MaterialId ) && Quantities.HasValues )
                        {
                            commitSDSDocNode( MaterialId, ReceiptObj );
                            CswPrimaryKey RequestId = _getRequestId( ReceiptObj );
                            CswNbtNode ReceiptLot = _makeReceiptLot( MaterialId, RequestId, ReceiptObj, InitialContainerNode.ExpirationDate.DateTimeValue );
                            JObject jBarcodes = new JObject();
                            Ret["barcodes"] = jBarcodes;
                            for( int index = 0; index < Quantities.Count; index++ )
                            {
                                JObject QuantityDef = CswConvert.ToJObject( Quantities[index] );
                                Int32 NoContainers = CswConvert.ToInt32( QuantityDef["containerNo"] );
                                CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
                                Barcodes.FromString( CswConvert.ToString( QuantityDef["barcodes"] ) );
                                Double QuantityValue = CswConvert.ToDouble( QuantityDef["quantity"] );
                                CswPrimaryKey UnitId = new CswPrimaryKey();
                                UnitId.FromString( CswConvert.ToString( QuantityDef["unitid"] ) );
                                CswPrimaryKey SizeId = new CswPrimaryKey();
                                SizeId.FromString( CswConvert.ToString( QuantityDef["sizeid"] ) );
                                CswNbtObjClassSize AsSize = _CswNbtResources.Nodes.GetNode( SizeId );
                                if( NoContainers > 0 && QuantityValue > 0 && Int32.MinValue != UnitId.PrimaryKey )
                                {
                                    for( Int32 C = 0; C < NoContainers; C += 1 )
                                    {
                                        // This includes the initial Container node that was created at the start of the receive wizard.
                                        // This is done so we can create Dispense Transaction and Location records, and persist custom barcodes.


                                        Action<CswNbtNode> After = delegate( CswNbtNode NewNode )
                                            {
                                                CswNbtObjClassContainer thisContainer = NewNode;
                                                if( Barcodes.Count <= NoContainers && false == string.IsNullOrEmpty( Barcodes[C] ) )
                                                {
                                                    thisContainer.Barcode.setBarcodeValueOverride( Barcodes[C], false );
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
                                                thisContainer.ReceiptLot.RelatedNodeId = ReceiptLot.NodeId;
                                                //thisContainer.postChanges( true );
                                            };

                                        CswNbtObjClassContainer AsContainer;
                                        if( C == 0 && index == 0 )
                                        {
                                            AsContainer = InitialContainerNode;
                                            //AsContainer.IsTemp = false;
                                            if( false == CswTools.IsPrimaryKey( AsContainer.Location.SelectedNodeId ) )
                                            {
                                                throw new CswDniException( CswEnumErrorType.Warning, "You cannot Receive a Container without picking a Location.", "You cannot Receive a Container without picking a Location." );
                                            }
                                            if( false == AsContainer.isLocationInAccessibleInventoryGroup( AsContainer.Location.SelectedNodeId ) )
                                            {
                                                throw new CswDniException( CswEnumErrorType.Warning, "You do not have Inventory Group permission to receive Containers into this Location: " + AsContainer.Location.CachedPath, "You do not have Inventory Group permission to receive Containers into this Location: " + AsContainer.Location.CachedPath );
                                            }
                                            After( AsContainer.Node );
                                            AsContainer.postChanges( false );
                                            AsContainer.PromoteTempToReal();
                                        }
                                        else
                                        {
                                            CswNbtNodeKey ContainerNodeKey;
                                            AsContainer = _CswNbtSdTabsAndProps.addNode( ContainerNt, null, ContainerAddProps, out ContainerNodeKey, After );
                                        }

                                        if( null != AsContainer )
                                        {
                                            ContainerIds.Add( AsContainer.NodeId );

                                            JObject BarcodeNode = new JObject();
                                            jBarcodes[AsContainer.NodeId.ToString()] = BarcodeNode;
                                            BarcodeNode["nodeid"] = AsContainer.NodeId.ToString();
                                            BarcodeNode["nodename"] = AsContainer.NodeName;
                                        }
                                    } //for( Int32 C = 0; C < NoContainers; C += 1 )
                                }
                            }//for( int index = 0; index < Quantities.Count; index++ )

                        }//if( null != ContainerNt )

                        if( ContainerIds.Count > 0 )
                        {
                            CswNbtView NewContainersView = new CswNbtView( _CswNbtResources );
                            NewContainersView.ViewName = "New Containers";
                            CswNbtViewRelationship ContainerVr = NewContainersView.AddViewRelationship( ContainerNt, true );
                            ContainerVr.NodeIdsToFilterIn = ContainerIds;
                            NewContainersView.SaveToCache( false );
                            Ret["viewid"] = NewContainersView.SessionViewId.ToString();
                        }
                    }//if( Int32.MinValue != ContainerNodeTypeId )

                }//if( null != InitialContainerNode )

            }//if( ReceiptObj.HasValues )

            Ret["containerscreated"] = ContainerIds.Count;

            return Ret;
        }

        /// <summary>
        /// Persist the SDS Document
        /// </summary>
        public void commitSDSDocNode( CswPrimaryKey MaterialId, JObject Obj )
        {
            CswNbtObjClassSDSDocument SDSDoc = _CswNbtResources.Nodes[CswConvert.ToString( Obj["sdsDocId"] )];
            if( null != SDSDoc )
            {
                _CswNbtSdTabsAndProps.saveProps( SDSDoc.NodeId, Int32.MinValue, (JObject) Obj["sdsDocProperties"], SDSDoc.NodeTypeId, null, IsIdentityTab: false, setIsTempToFalse: false );
                if( ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( SDSDoc.File.FileName ) ) ||
                    ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( SDSDoc.Link.Href ) ) )
                {
                    //SDSDoc.IsTemp = false;
                    SDSDoc.Owner.RelatedNodeId = MaterialId;
                    SDSDoc.postChanges( ForceUpdate: false );
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

            CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["receiptLotId"] )];
            if( null != ReceiptLot )
            {
                _CswNbtSdTabsAndProps.saveProps( ReceiptLot.NodeId, Int32.MinValue, (JObject) ReceiptObj["receiptLotProperties"], ReceiptLot.NodeTypeId, null, IsIdentityTab: false );
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
            CswNbtObjClassCofADocument CofADoc = _CswNbtResources.Nodes[CswConvert.ToString( Obj["cofaDocId"] )];
            if( null != CofADoc )
            {
                _CswNbtSdTabsAndProps.saveProps( CofADoc.NodeId, Int32.MinValue, (JObject) Obj["cofaDocProperties"], CofADoc.NodeTypeId, null, IsIdentityTab: false );
                if( ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( CofADoc.File.FileName ) ) ||
                    ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( CofADoc.Link.Href ) ) )
                {
                    //CofADoc.IsTemp = false;
                    CofADoc.Owner.RelatedNodeId = ReceiptLotId;
                    CofADoc.postChanges( ForceUpdate: false );
                    CofADoc.PromoteTempToReal();
                }
            }
        }

        #endregion Private Helper Functions
    }
}
