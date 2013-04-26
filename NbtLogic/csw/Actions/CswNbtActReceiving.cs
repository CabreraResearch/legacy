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
        private CswNbtMetaDataObjectClass _SizeOc = null;
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
            _SizeOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
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
                CswNbtMetaDataObjectClassProp DispensableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable );

                CswNbtViewRelationship SizeRel = SizeView.AddViewRelationship( MaterialRel, CswEnumNbtViewPropOwnerType.Second, MaterialOcp, true );
                SizeView.AddViewProperty( SizeRel, InitialQuantityOcp );
                //CswNbtViewProperty DispensableVp = SizeView.AddViewProperty( SizeRel, DispensableOcp );
                //DispensableVp.ShowInGrid = false;
                //SizeView.AddViewPropertyFilter( DispensableVp, DispensableOcp.getFieldTypeRule().SubFields.Default.Name, Value: Tristate.True.ToString() );
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
                _CswNbtResources.EditMode = CswEnumNbtNodeEditMode.Add;
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
                _CswNbtResources.EditMode = CswEnumNbtNodeEditMode.Add;
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
                            CswNbtObjClassMaterial NodeAsMaterial = CswNbtResources.Nodes[CswConvert.ToString( ReceiptObj["materialid"] )];
                            Debug.Assert( ( null != NodeAsMaterial ), "The request did not specify a valid materialid." );
                            if( null != NodeAsMaterial )
                            {
                                commitDocumentNode( CswNbtResources, NodeAsMaterial, ReceiptObj );
                                JArray Quantities = CswConvert.ToJArray( ReceiptObj["quantities"] );
                                Debug.Assert( Quantities.HasValues, "The request did not specify any valid container amounts." );
                                if( Quantities.HasValues )
                                {

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
        /// Gets the SDS Document NodeTypeId.
        /// </summary>
        public static Int32 getSDSDocumentNodeTypeId( CswNbtResources CswNbtResources )
        {
            Int32 Ret = Int32.MinValue;
            CswNbtMetaDataObjectClass DocumentOc = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DocumentClass );
            foreach( CswNbtMetaDataNodeType DocumentNt in DocumentOc.getLatestVersionNodeTypes() )
            {
                if( DocumentNt.NodeTypeName == "SDS Document" )
                {
                    Ret = DocumentNt.NodeTypeId;
                    break;
                }
            }
            return Ret;
        }

        /// <summary>
        /// Upversion a Document node
        /// </summary>
        public static CswNbtObjClassDocument commitDocumentNode( CswNbtResources CswNbtResources, CswNbtObjClassMaterial NodeAsMaterial, JObject Obj )
        {
            CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( CswNbtResources );
            CswNbtObjClassDocument Doc = CswNbtResources.Nodes[CswConvert.ToString( Obj["documentid"] )];
            if( null != Doc )
            {
                SdTabsAndProps.saveProps( Doc.NodeId, Int32.MinValue, (JObject) Obj["documentProperties"], Doc.NodeTypeId, null, IsIdentityTab: false );
                if( ( Doc.FileType.Value == CswNbtObjClassDocument.FileTypes.File && false == string.IsNullOrEmpty( Doc.File.FileName ) ) ||
                    ( Doc.FileType.Value == CswNbtObjClassDocument.FileTypes.Link && false == string.IsNullOrEmpty( Doc.Link.Href ) ) )
                {
                    Doc.IsTemp = false;
                    Doc.Owner.RelatedNodeId = NodeAsMaterial.NodeId;
                    Doc.postChanges( ForceUpdate: false );
                }

            }
            return Doc;
        }

        #endregion Public methods and props
    }


}
