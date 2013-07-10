using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _ContainerOc = null;
        private CswNbtMetaDataObjectClass _MaterialOc = null;
        private CswPrimaryKey _MaterialId = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActReceiving( CswNbtResources CswNbtResources, CswNbtMetaDataObjectClass MaterialOc, CswPrimaryKey MaterialNodeId )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }

            _MaterialOc = MaterialOc;
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
        }

        #endregion Constructor

        #region Public methods and props

        public CswNbtView SizesView
        {
            get
            {
                CswNbtView SizeView = new CswNbtView( _CswNbtResources );
                SizeView.Visibility = CswEnumNbtViewVisibility.Property;
                SizeView.ViewMode = CswEnumNbtViewRenderingMode.Grid;

                CswNbtViewRelationship MaterialRel = SizeView.AddViewRelationship( _MaterialOc, true );
                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp InitialQuantityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                CswNbtMetaDataObjectClassProp CatalogNoOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );

                CswNbtViewRelationship SizeRel = SizeView.AddViewRelationship( MaterialRel, CswEnumNbtViewPropOwnerType.Second, MaterialOcp, true );
                SizeView.AddViewProperty( SizeRel, InitialQuantityOcp );
                SizeView.AddViewProperty( SizeRel, CatalogNoOcp );
                SizeView.SaveToCache( false );
                return SizeView;
            }
        }

        /// <summary>
        /// Instance a new container according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassContainer makeContainer( CswNbtMetaDataNodeType ContainerNt = null )
        {
            CswNbtObjClassContainer RetAsContainer = null;
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );

            ContainerNt = ContainerNt ?? _ContainerOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != ContainerNt )
            {
                RetAsContainer = PropsAction.getAddNode( ContainerNt, CswEnumNbtMakeNodeOperation.MakeTemp );
                if( null == RetAsContainer )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Could not create a new container.", "Failed to create a new Container node." );
                }
                RetAsContainer.Material.RelatedNodeId = _MaterialId;
                RetAsContainer.Material.setHidden( value: true, SaveToDb: false );
                RetAsContainer.Size.setHidden( value: true, SaveToDb: false );
            }
            return RetAsContainer;
        }

        /// <summary>
        /// Get the Add Layout properties for a container
        /// </summary>
        public JObject getContainerAddProps( CswNbtObjClassContainer Container )
        {
            JObject Ret = new JObject();
            if( null != Container )
            {
                CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
                Ret = PropsAction.getProps( Container.Node, "", null, CswEnumNbtLayoutType.Add );
            }
            return Ret;
        }

        /// <summary>
        /// Create new containers and return the number of containers succcesfully created and a view of said containers. 
        /// </summary>
        public static JObject receiveMaterial( string ReceiptDefinition, CswNbtResources CswNbtResources )
        {

            JObject Ret = new JObject();
            JObject ReceiptObj = CswConvert.ToJObject( ReceiptDefinition );
            Collection<CswPrimaryKey> ContainerIds = new Collection<CswPrimaryKey>();
            Debug.Assert( ReceiptObj.HasValues, "The request was not provided a parsable JSON string." );
            if( ReceiptObj.HasValues )
            {
                CswNbtObjClassContainer InitialContainerNode = CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["containernodeid"] )];
                Debug.Assert( ( null != InitialContainerNode ), "The request did not specify a valid materialid." );
                if( null != InitialContainerNode )
                {
                    //Convert to real node
                    InitialContainerNode.IsTemp = false;

                    Int32 ContainerNodeTypeId = CswConvert.ToInt32( ReceiptObj["containernodetypeid"] );
                    Debug.Assert( ( Int32.MinValue != ContainerNodeTypeId ), "The request did not specify a valid container nodetypeid." );
                    if( Int32.MinValue != ContainerNodeTypeId )
                    {
                        CswNbtMetaDataNodeType ContainerNt = CswNbtResources.MetaData.getNodeType( ContainerNodeTypeId );
                        Debug.Assert( ( null != ContainerNt ), "The request specified an invalid container nodetypeid." );
                        if( null != ContainerNt )
                        {
                            CswNbtPropertySetMaterial NodeAsMaterial = CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["materialid"] )];
                            Debug.Assert( ( null != NodeAsMaterial ), "The request did not specify a valid materialid." );
                            if( null != NodeAsMaterial )
                            {
                                commitSDSDocNode( CswNbtResources, NodeAsMaterial, ReceiptObj );
                                JArray Quantities = CswConvert.ToJArray( ReceiptObj["quantities"] );
                                Debug.Assert( Quantities.HasValues, "The request did not specify any valid container amounts." );
                                if( Quantities.HasValues )
                                {
                                    CswNbtNode ReceiptLot = _makeReceiptLot( CswNbtResources, NodeAsMaterial.NodeId );
                                    _attachCofA( CswNbtResources, ReceiptLot.NodeId, ReceiptObj );
                                    JObject ContainerAddProps = CswConvert.ToJObject( ReceiptObj["props"] );

                                    CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( CswNbtResources );
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
                                        CswNbtObjClassSize AsSize = CswNbtResources.Nodes.GetNode( SizeId );

                                        Debug.Assert( ( NoContainers > 0 ), "The request did not specify at least one container." );
                                        Debug.Assert( ( QuantityValue > 0 ), "The request did not specify a valid quantity." );
                                        Debug.Assert( ( Int32.MinValue != UnitId.PrimaryKey ), "The request did not specify a valid unit." );
                                        if( NoContainers > 0 && QuantityValue > 0 && Int32.MinValue != UnitId.PrimaryKey )
                                        {
                                            JObject jBarcodes = new JObject();
                                            Ret["barcodes"] = jBarcodes;
                                            for( Int32 C = 0; C < NoContainers; C += 1 )
                                            {
                                                // This includes the initial Container node that was created at 
                                                // the start of the receive wizard -- this is done so the barcode isn't
                                                // thrown out.
                                                CswNbtNodeKey ContainerNodeKey;
                                                CswNbtObjClassContainer AsContainer;
                                                if( C == 0 && index == 0 )
                                                {
                                                    AsContainer = InitialContainerNode;
                                                    SdTabsAndProps.saveNodeProps( AsContainer.Node, ContainerAddProps ); //case 29387

                                                    if( false == CswTools.IsPrimaryKey(AsContainer.Location.SelectedNodeId) )
                                                    {
                                                        throw new CswDniException( CswEnumErrorType.Warning, "You cannot Receive a Container without picking a Location.", "You cannot Receive a Container without picking a Location." );
                                                    }
                                                    if( false == AsContainer.isLocationInAccessibleInventoryGroup( AsContainer.Location.SelectedNodeId ) )
                                                    {
                                                        throw new CswDniException( CswEnumErrorType.Warning, "You do not have Inventory Group permission to receive Containers into this Location: " + AsContainer.Location.CachedPath, "You do not have Inventory Group permission to receive Containers into this Location: " + AsContainer.Location.CachedPath );
                                                    }

                                                }
                                                else
                                                {
                                                    AsContainer = SdTabsAndProps.addNode( ContainerNt, null, ContainerAddProps, out ContainerNodeKey );
                                                }

                                                if( null != AsContainer )
                                                {
                                                    if( Barcodes.Count <= NoContainers && false == string.IsNullOrEmpty( Barcodes[C] ) )
                                                    {
                                                        AsContainer.Barcode.setBarcodeValueOverride( Barcodes[C], false );
                                                    }
                                                    AsContainer.Size.RelatedNodeId = SizeId;
                                                    AsContainer.Material.RelatedNodeId = NodeAsMaterial.NodeId;
                                                    if( AsSize.QuantityEditable.Checked != CswEnumTristate.True )
                                                    {
                                                        QuantityValue = AsSize.InitialQuantity.Quantity;
                                                        UnitId = AsSize.InitialQuantity.UnitId;
                                                    }
                                                    if( null == AsContainer.Quantity.UnitId || Int32.MinValue == AsContainer.Quantity.UnitId.PrimaryKey )
                                                    {
                                                        AsContainer.Quantity.UnitId = UnitId;
                                                    }
                                                    AsContainer.DispenseIn( CswEnumNbtContainerDispenseType.Receive, QuantityValue, UnitId );
                                                    AsContainer.Disposed.Checked = CswEnumTristate.False;
                                                    AsContainer.Undispose.setHidden( value: true, SaveToDb: true );
                                                    AsContainer.ReceiptLot.RelatedNodeId = ReceiptLot.NodeId;
                                                    AsContainer.postChanges( true );
                                                    ContainerIds.Add( AsContainer.NodeId );

                                                    JObject BarcodeNode = new JObject();
                                                    jBarcodes[AsContainer.NodeId.ToString()] = BarcodeNode;
                                                    BarcodeNode["nodeid"] = AsContainer.NodeId.ToString();
                                                    BarcodeNode["nodename"] = AsContainer.NodeName;
                                                    
                                                }
                                            } //for( Int32 C = 0; C < NoContainers; C += 1 )
                                        }
                                    }
                                }//if( Quantities.HasValues )

                            }//if( null != NodeAsMaterial )


                        }//if( null != ContainerNt )

                        if( ContainerIds.Count > 0 )
                        {
                            CswNbtView NewContainersView = new CswNbtView( CswNbtResources );
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
        public static void commitSDSDocNode( CswNbtResources CswNbtResources, CswNbtPropertySetMaterial NodeAsMaterial, JObject Obj )
        {
            CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( CswNbtResources );
            CswNbtObjClassSDSDocument SDSDoc = CswNbtResources.Nodes[CswConvert.ToString( Obj["sdsDocId"] )];
            if( null != SDSDoc )
            {
                SdTabsAndProps.saveProps( SDSDoc.NodeId, Int32.MinValue, (JObject) Obj["sdsDocProperties"], SDSDoc.NodeTypeId, null, IsIdentityTab: false, setIsTempToFalse: false );
                if( ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( SDSDoc.File.FileName ) ) ||
                    ( SDSDoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( SDSDoc.Link.Href ) ) )
                {
                    SDSDoc.IsTemp = false;
                    SDSDoc.Owner.RelatedNodeId = NodeAsMaterial.NodeId;
                    SDSDoc.postChanges( ForceUpdate: false );
                }

            }
        }

        #endregion Public methods and props

        #region Private Helper Functions

        private static CswNbtNode _makeReceiptLot( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId )
        {
            CswNbtMetaDataObjectClass ReceiptLotClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            CswNbtObjClassReceiptLot ReceiptLot = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ReceiptLotClass.FirstNodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
            ReceiptLot.Material.RelatedNodeId = MaterialId;
            ReceiptLot.postChanges( false );
            return ReceiptLot.Node;
        }

        private static void _attachCofA( CswNbtResources _CswNbtResources, CswPrimaryKey ReceiptLotId, JObject Obj )
        {
            CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
            CswNbtObjClassCofADocument CofADoc = _CswNbtResources.Nodes[CswConvert.ToString( Obj["cofaDocId"] )];
            if( null != CofADoc )
            {
                SdTabsAndProps.saveProps( CofADoc.NodeId, Int32.MinValue, (JObject) Obj["cofaDocProperties"], CofADoc.NodeTypeId, null, IsIdentityTab: false );
                if( ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File && false == string.IsNullOrEmpty( CofADoc.File.FileName ) ) ||
                    ( CofADoc.FileType.Value == CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link && false == string.IsNullOrEmpty( CofADoc.Link.Href ) ) )
                {
                    CofADoc.IsTemp = false;
                    CofADoc.Owner.RelatedNodeId = ReceiptLotId;
                    CofADoc.postChanges( ForceUpdate: false );
                }
            }
        }

        #endregion Private Helper Functions
    }
}
