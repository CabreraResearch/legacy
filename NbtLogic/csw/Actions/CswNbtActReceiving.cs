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
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }

            _MaterialOc = MaterialOc;
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            _SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
        }

        #endregion Constructor

        #region Public methods and props

        public CswNbtView SizesView
        {
            get
            {
                CswNbtView SizeView = new CswNbtView( _CswNbtResources );
                SizeView.Visibility = NbtViewVisibility.Property;
                SizeView.ViewMode = NbtViewRenderingMode.Grid;

                CswNbtViewRelationship MaterialRel = SizeView.AddViewRelationship( _MaterialOc, true );
                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp CapacityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.InitialQuantityPropertyName );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                CswNbtMetaDataObjectClassProp CatalogNoOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CatalogNoPropertyName );
                CswNbtMetaDataObjectClassProp DispensableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );

                CswNbtViewRelationship SizeRel = SizeView.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, true );
                SizeView.AddViewProperty( SizeRel, CapacityOcp );
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
                RetAsContainer = PropsAction.getAddNode( ContainerNt );
                if( null == RetAsContainer )
                {
                    throw new CswDniException( ErrorType.Error, "Could not create a new container.", "Failed to create a new Container node." );
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
                _CswNbtResources.EditMode = NodeEditMode.Add;
                Ret = PropsAction.getProps( Container.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
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
                Int32 ContainerNodeTypeId = CswConvert.ToInt32( ReceiptObj["containernodetypeid"] );
                Debug.Assert( ( Int32.MinValue != ContainerNodeTypeId ), "The request did not specify a valid container nodetypeid." );
                if( Int32.MinValue != ContainerNodeTypeId )
                {
                    CswNbtMetaDataNodeType ContainerNt = CswNbtResources.MetaData.getNodeType( ContainerNodeTypeId );
                    Debug.Assert( ( null != ContainerNt ), "The request specified an invalid container nodetypeid." );
                    if( null != ContainerNt )
                    {
                        CswPrimaryKey MaterialId = new CswPrimaryKey();
                        MaterialId.FromString( CswConvert.ToString( ReceiptObj["materialid"] ) );
                        Debug.Assert( ( Int32.MinValue != MaterialId.PrimaryKey ), "The request did not specify a valid materialid." );
                        if( Int32.MinValue != MaterialId.PrimaryKey )
                        {
                            JArray Quantities = CswConvert.ToJArray( ReceiptObj["quantities"] );
                            Debug.Assert( Quantities.HasValues, "The request did not specify any valid container amounts." );
                            if( Quantities.HasValues )
                            {
                                JObject ContainerAddProps = CswConvert.ToJObject( ReceiptObj["props"] );

                                CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( CswNbtResources );
                                foreach( JObject QuantityDef in Quantities )
                                {
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
                                        for( Int32 C = 0; C < NoContainers; C += 1 )
                                        {
                                            CswNbtNode Container;
                                            CswNbtNodeKey ContainerNodeKey;
                                            SdTabsAndProps.addNode( ContainerNt, out Container, ContainerAddProps, out ContainerNodeKey );
                                            CswNbtObjClassContainer AsContainer = Container;
                                            if( Barcodes.Count <= NoContainers && false == string.IsNullOrEmpty( Barcodes[C] ) )
                                            {
                                                AsContainer.Barcode.setBarcodeValueOverride( Barcodes[C], false );
                                            }
                                            AsContainer.Size.RelatedNodeId = SizeId;
                                            AsContainer.Material.RelatedNodeId = MaterialId;
                                            if( AsSize.QuantityEditable.Checked != Tristate.True )
                                            {
                                                    QuantityValue = AsSize.InitialQuantity.Quantity;
                                                    UnitId = AsSize.InitialQuantity.UnitId;
                                            }
                                            if( null == AsContainer.Quantity.UnitId || Int32.MinValue == AsContainer.Quantity.UnitId.PrimaryKey )
                                            {
                                                AsContainer.Quantity.UnitId = UnitId;
                                            }
                                            AsContainer.DispenseIn( CswNbtObjClassContainerDispenseTransaction.DispenseType.Receive, QuantityValue, UnitId );
                                            AsContainer.postChanges( true );
                                            ContainerIds.Add( AsContainer.NodeId );
                                            Ret["barcodeId"] = AsContainer.NodeId.ToString() + "_" + AsContainer.Barcode.NodeTypePropId.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if( ContainerIds.Count > 0 )
                    {
                        CswNbtView NewContainersView = new CswNbtView( CswNbtResources );
                        NewContainersView.ViewName = "New Containers";
                        CswNbtViewRelationship ContainerVr = NewContainersView.AddViewRelationship( ContainerNt, true );
                        ContainerVr.NodeIdsToFilterIn = ContainerIds;
                        NewContainersView.SaveToCache( false );
                        Ret["viewid"] = NewContainersView.SessionViewId.ToString();
                    }
                }
            }
            Ret["containerscreated"] = ContainerIds.Count;
            return Ret;
        }

        #endregion Public methods and props
    }


}
