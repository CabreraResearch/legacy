using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActDispenseContainer
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtObjClassContainer _SourceContainer = null;
        private Collection<CswPrimaryKey> _ContainersToView = new Collection<CswPrimaryKey>();

        #region Constructor

        public CswNbtActDispenseContainer( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public CswNbtActDispenseContainer( CswNbtResources CswNbtResources, string SourceContainerNodeId )
        {
            _CswNbtResources = CswNbtResources;

            if( false == String.IsNullOrEmpty( SourceContainerNodeId ) )
            {
                CswPrimaryKey SourceContainerPK = new CswPrimaryKey();
                SourceContainerPK.FromString( SourceContainerNodeId );
                _SourceContainer = _CswNbtResources.Nodes.GetNode( SourceContainerPK );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot execute dispense contianer action with an undefined Source Container.", "Attempted to constuct CswNbtActDispenseContainer without a valid Source Container." );
            }
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot use the Dispense action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
        }

        #endregion Constructor

        #region Public Methods

        public JObject dispenseSourceContainer( string DispenseType, string Quantity, string UnitId, string RequestItemId )
        {
            JObject ret = new JObject();
            if( null != _SourceContainer )
            {
                CswEnumNbtContainerDispenseType DispenseTypeEnum = _getDispenseTypeFromAction( DispenseType );
                CswPrimaryKey UnitOfMeasurePk = new CswPrimaryKey();
                UnitOfMeasurePk.FromString( UnitId );
                CswPrimaryKey RequestItemPk = new CswPrimaryKey();
                RequestItemPk.FromString( RequestItemId );
                Double RealQuantity = CswConvert.ToDouble( Quantity );

                if( DispenseTypeEnum == CswEnumNbtContainerDispenseType.Add )
                {
                    RealQuantity = -RealQuantity; // deducting negative quantity is adding quantity
                }

                _SourceContainer.DispenseOut( DispenseTypeEnum, RealQuantity, UnitOfMeasurePk, RequestItemPk );
                _SourceContainer.postChanges( false );

                string ViewId = _getViewForAllDispenseContainers();
                ret["viewId"] = ViewId;
            }
            return ret;
        }

        private CswEnumNbtContainerDispenseType _getDispenseTypeFromAction( string DispenseTypeDescription )
        {
            CswEnumNbtContainerDispenseType DispenseType = DispenseTypeDescription;
            if( DispenseTypeDescription.Contains( CswEnumNbtContainerDispenseType.Add.ToString() ) )
            {
                DispenseType = CswEnumNbtContainerDispenseType.Add;
            }
            else if( DispenseTypeDescription.Contains( CswEnumNbtContainerDispenseType.Waste.ToString() ) )
            {
                DispenseType = CswEnumNbtContainerDispenseType.Waste;
            }
            else if( DispenseTypeDescription.Contains( CswEnumNbtContainerDispenseType.Dispense.ToString() ) )
            {
                DispenseType = CswEnumNbtContainerDispenseType.Dispense;
            }
            return DispenseType;
        }

        public JObject dispenseIntoChildContainers( string ContainerNodeTypeId, string DispenseGrid, string RequestItemId )
        {
            JObject ret = new JObject();
            if( null != _SourceContainer )
            {
                CswPrimaryKey RequestItemPk = new CswPrimaryKey();
                RequestItemPk.FromString( RequestItemId );
                JArray GridArray = JArray.Parse( DispenseGrid );
                JObject jBarcodes = new JObject();
                ret["barcodes"] = jBarcodes;
                for( Int32 i = 0; i < GridArray.Count; i += 1 )
                {
                    if( GridArray[i].Type == JTokenType.Object )
                    {
                        JObject CurrentRow = (JObject) GridArray[i];
                        int NumOfContainers = CswConvert.ToInt32( CurrentRow["containerNo"] );
                        double QuantityToDispense = CswConvert.ToDouble( CurrentRow["quantity"] );
                        string UnitId = CswConvert.ToString( CurrentRow["unitid"] );
                        CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
                        Barcodes.FromString( CswConvert.ToString( CurrentRow["barcodes"] ) );

                        CswPrimaryKey UnitOfMeasurePK = new CswPrimaryKey();
                        UnitOfMeasurePK.FromString( UnitId );

                        if( NumOfContainers == 0 )
                        {
                            _SourceContainer.DispenseOut(
                                CswEnumNbtContainerDispenseType.Dispense, QuantityToDispense,
                                UnitOfMeasurePK, RequestItemPk );
                            _SourceContainer.postChanges( false );
                        }
                        else
                        {
                            for( Int32 c = 0; c < NumOfContainers; c += 1 )
                            {
                                CswNbtObjClassContainer ChildContainer = _createChildContainer( ContainerNodeTypeId,
                                                                                               UnitOfMeasurePK, Barcodes[c] );
                                _SourceContainer.DispenseOut(
                                    CswEnumNbtContainerDispenseType.Dispense, QuantityToDispense,
                                    UnitOfMeasurePK, RequestItemPk, ChildContainer );
                                //ChildContainer.DispenseIn( CswEnumNbtContainerDispenseType.Dispense, QuantityToDispense, UnitOfMeasurePK, RequestItemPk, _SourceContainer );
                                ChildContainer.postChanges( false );

                                JObject BarcodeNode = new JObject();
                                jBarcodes[ChildContainer.NodeId.ToString()] = BarcodeNode;
                                BarcodeNode["nodeid"] = ChildContainer.NodeId.ToString();
                                BarcodeNode["nodename"] = ChildContainer.NodeName;
                            }
                            _SourceContainer.postChanges( false );
                        }
                    }
                }
                _SourceContainer.postChanges( false );

                string ViewId = _getViewForAllDispenseContainers();
                ret["viewId"] = ViewId;
            }
            return ret;
        }

        private CswNbtObjClassContainer _createChildContainer( string ContainerNodeTypeId, CswPrimaryKey UnitId, string Barcode )
        {
            CswNbtObjClassContainer ChildContainer = null;
            CswNbtMetaDataNodeType ContainerNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( ContainerNodeTypeId ) );
            if( ContainerNT != null )
            {
                CswNbtNode CopyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, CswEnumNbtMakeNodeOperation.DoNothing );
                CopyNode.copyPropertyValues( _SourceContainer.Node );
                ChildContainer = CopyNode;
                if( false == String.IsNullOrEmpty( Barcode ) )
                {
                    ChildContainer.Barcode.setBarcodeValueOverride( Barcode, false );
                }
                else
                {
                    ChildContainer.Barcode.setBarcodeValue();
                }
                ChildContainer.SourceContainer.RelatedNodeId = _SourceContainer.NodeId;
                ChildContainer.Quantity.Quantity = 0;
                ChildContainer.Quantity.UnitId = UnitId;
                ChildContainer.Disposed.Checked = CswEnumTristate.False;
                ChildContainer.postChanges( false );
                ChildContainer.Undispose.setHidden( value: true, SaveToDb: true );
                _ContainersToView.Add( ChildContainer.NodeId );
            }
            return ChildContainer;
        }

        private string _getViewForAllDispenseContainers()
        {
            Collection<CswPrimaryKey> SourceContainerRoot = new Collection<CswPrimaryKey>();
            SourceContainerRoot.Add( _SourceContainer.NodeId );
            CswNbtView DispenseContainerView = new CswNbtView( _CswNbtResources );
            DispenseContainerView.ViewName = "Containers Dispensed at " + DateTime.Now.ToShortTimeString();

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtViewRelationship RootRelationship = DispenseContainerView.AddViewRelationship( ContainerOc, false );
            RootRelationship.NodeIdsToFilterIn = SourceContainerRoot;

            if( _ContainersToView.Count > 0 )
            {
                CswNbtMetaDataObjectClassProp SourceContainerProp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.SourceContainer );
                CswNbtViewRelationship ChildRelationship = DispenseContainerView.AddViewRelationship( RootRelationship, CswEnumNbtViewPropOwnerType.Second, SourceContainerProp, false );
                ChildRelationship.NodeIdsToFilterIn = _ContainersToView;
            }

            DispenseContainerView.SaveToCache( false );
            return DispenseContainerView.SessionViewId.ToString();
        }

        public CswNbtView getDispensibleContainersView( CswPrimaryKey RequestItemId )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );

            CswNbtPropertySetRequestItem NodeAsRequestItem = _CswNbtResources.Nodes[RequestItemId];
            if( null != NodeAsRequestItem )
            {
                CswNbtObjClassRequestMaterialDispense RequestMaterialDispense = CswNbtObjClassRequestMaterialDispense.fromPropertySet( NodeAsRequestItem );
                if( null != RequestMaterialDispense )
                {
                    CswNbtPropertySetMaterial NodeAsMaterial = _CswNbtResources.Nodes[RequestMaterialDispense.Material.RelatedNodeId];
                    if( null != NodeAsMaterial )
                    {
                        Ret.ViewName = "Containers of " + NodeAsMaterial.TradeName.Text;
                        Ret.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                        Ret.Category = "Dispensing";

                        CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                        CswNbtViewRelationship ContainerRel = Ret.AddViewRelationship( ContainerOc, true );
                        CswNbtViewProperty BarcodeVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode ) );
                        CswNbtViewProperty MaterialVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material ) );
                        Ret.AddViewPropertyFilter( MaterialVp, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: NodeAsMaterial.NodeId.PrimaryKey.ToString() );
                        MaterialVp.ShowInGrid = false;

                        CswNbtViewProperty MissingVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Missing ) );
                        Ret.AddViewPropertyFilter( MissingVp, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumTristate.True.ToString() );
                        MissingVp.ShowInGrid = false;

                        CswNbtViewProperty QuantityVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity ) );
                        Ret.AddViewPropertyFilter( QuantityVp, CswEnumNbtSubFieldName.Value, FilterMode: CswEnumNbtFilterMode.GreaterThan, Value: "0" );

                        CswNbtViewProperty StatusVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status ) );
                        Ret.AddViewPropertyFilter( StatusVp, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumNbtContainerStatuses.Expired );
                        StatusVp.ShowInGrid = false;

                        Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location ) );

                        CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                        CswNbtMetaDataObjectClassProp InventoryGroupOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                        CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( ContainerRel, CswEnumNbtViewPropOwnerType.First, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location ), IncludeDefaultFilters: true );

                        if( CswTools.IsPrimaryKey( RequestMaterialDispense.InventoryGroup.RelatedNodeId ) )
                        {
                            Ret.AddViewPropertyAndFilter( LocationVr, InventoryGroupOcp, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: RequestMaterialDispense.InventoryGroup.RelatedNodeId.PrimaryKey.ToString(), ShowInGrid: false );
                        }
                        else
                        {
                            Ret.AddViewPropertyAndFilter( LocationVr, InventoryGroupOcp, SubFieldName: CswEnumNbtSubFieldName.NodeID, FilterMode: CswEnumNbtFilterMode.Null, ShowInGrid: false );
                        }

                        if( RequestMaterialDispense.Type.Value == CswNbtObjClassRequestMaterialDispense.Types.Size )
                        {
                            Ret.AddViewPropertyAndFilter( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Size ), SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: RequestMaterialDispense.Size.RelatedNodeId.PrimaryKey.ToString(), ShowInGrid: false );
                        }
                    }
                }
            }
            return Ret;
        }

        #endregion Public Methods
    }
}
