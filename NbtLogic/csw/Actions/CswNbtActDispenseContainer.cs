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
                throw new CswDniException( ErrorType.Error, "Cannot execute dispense contianer action with an undefined Source Container.", "Attempted to constuct CswNbtActDispenseContainer without a valid Source Container." );
            }
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Dispense action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
        }

        #endregion Constructor

        #region Public Methods

        public JObject dispenseSourceContainer( string DispenseType, string Quantity, string UnitId, string RequestItemId )
        {
            JObject ret = new JObject();
            if( null != _SourceContainer )
            {
                CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseTypeEnum = _getDispenseTypeFromAction( DispenseType );
                CswPrimaryKey UnitOfMeasurePk = new CswPrimaryKey();
                UnitOfMeasurePk.FromString( UnitId );
                CswPrimaryKey RequestItemPk = new CswPrimaryKey();
                RequestItemPk.FromString( RequestItemId );
                Double RealQuantity = CswConvert.ToDouble( Quantity );

                if( DispenseTypeEnum == CswNbtObjClassContainerDispenseTransaction.DispenseType.Add )
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

        private CswNbtObjClassContainerDispenseTransaction.DispenseType _getDispenseTypeFromAction( string DispenseTypeDescription )
        {
            CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType = DispenseTypeDescription;
            if( DispenseTypeDescription.Contains( CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() ) )
            {
                DispenseType = CswNbtObjClassContainerDispenseTransaction.DispenseType.Add;
            }
            else if( DispenseTypeDescription.Contains( CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste.ToString() ) )
            {
                DispenseType = CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste;
            }
            else if( DispenseTypeDescription.Contains( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense.ToString() ) )
            {
                DispenseType = CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense;
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
                for( Int32 i = 0; i < GridArray.Count; i += 1 )
                {
                    if( GridArray[i].Type == JTokenType.Object )
                    {
                        JObject CurrentRow = (JObject) GridArray[i];
                        int NumOfContainers = CswConvert.ToInt32( CurrentRow["containerNo"] );
                        double QuantityToDispense = CswConvert.ToDouble( CurrentRow["quantity"] );
                        string UnitId = CswConvert.ToString( CurrentRow["unitid"] );
                        string Barcode = CswConvert.ToString( CurrentRow["barcodes"] );

                        CswPrimaryKey UnitOfMeasurePK = new CswPrimaryKey();
                        UnitOfMeasurePK.FromString( UnitId );

                        if( NumOfContainers == 0 )
                        {
                            _SourceContainer.DispenseOut(
                                CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense,
                                UnitOfMeasurePK, RequestItemPk );
                            _SourceContainer.postChanges( false );
                        }
                        else
                        {
                            for( Int32 c = 0; c < NumOfContainers; c += 1 )
                            {
                                CswNbtObjClassContainer ChildContainer = _createChildContainer( ContainerNodeTypeId,
                                                                                               UnitOfMeasurePK, Barcode );
                                _SourceContainer.DispenseOut(
                                    CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense,
                                    UnitOfMeasurePK, RequestItemPk, ChildContainer );
                                //ChildContainer.DispenseIn( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense, UnitOfMeasurePK, RequestItemPk, _SourceContainer );
                                ChildContainer.postChanges( false );
                            }
                            _SourceContainer.postChanges( false );
                        }
                    }
                }
                _SourceContainer.postChanges( false );

                string ViewId = _getViewForAllDispenseContainers();
                ret["viewId"] = ViewId;
                ret["barcodeId"] = _SourceContainer.NodeId.ToString() + "_" +
                                   _SourceContainer.Barcode.NodeTypePropId.ToString();
            }
            return ret;
        }

        private CswNbtObjClassContainer _createChildContainer( string ContainerNodeTypeId, CswPrimaryKey UnitId, string Barcode )
        {
            CswNbtObjClassContainer ChildContainer = null;
            CswNbtMetaDataNodeType ContainerNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( ContainerNodeTypeId ) );
            if( ContainerNT != null )
            {
                CswNbtNode CopyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
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
                ChildContainer.postChanges( false );
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

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtViewRelationship RootRelationship = DispenseContainerView.AddViewRelationship( ContainerOc, false );
            RootRelationship.NodeIdsToFilterIn = SourceContainerRoot;

            if( _ContainersToView.Count > 0 )
            {
                CswNbtMetaDataObjectClassProp SourceContainerProp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.SourceContainerPropertyName );
                CswNbtViewRelationship ChildRelationship = DispenseContainerView.AddViewRelationship( RootRelationship, NbtViewPropOwnerType.Second, SourceContainerProp, false );
                ChildRelationship.NodeIdsToFilterIn = _ContainersToView;
            }

            DispenseContainerView.SaveToCache( false );
            return DispenseContainerView.SessionViewId.ToString();
        }

        public CswNbtView getDispensibleContainersView( CswPrimaryKey RequestItemId )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );

            CswNbtObjClassRequestItem NodeAsRequestItem = _CswNbtResources.Nodes[RequestItemId];
            if( null != NodeAsRequestItem )
            {
                CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes[NodeAsRequestItem.Material.RelatedNodeId];
                if( null != NodeAsMaterial )
                {
                    Ret.ViewName = "Containers of " + NodeAsMaterial.TradeName.Text;
                    Ret.ViewMode = NbtViewRenderingMode.Grid;
                    Ret.Category = "Dispensing";

                    CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
                    CswNbtViewRelationship ContainerRel = Ret.AddViewRelationship( ContainerOc, true );
                    CswNbtViewProperty BarcodeVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.BarcodePropertyName ) );
                    CswNbtViewProperty MaterialVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName ) );
                    Ret.AddViewPropertyFilter( MaterialVp, SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: NodeAsMaterial.NodeId.PrimaryKey.ToString() );
                    MaterialVp.ShowInGrid = false;

                    CswNbtViewProperty MissingVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MissingPropertyName ) );
                    Ret.AddViewPropertyFilter( MissingVp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: Tristate.True.ToString() );
                    MissingVp.ShowInGrid = false;

                    CswNbtViewProperty LocationVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName ) );

                    CswNbtViewProperty QuantityVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.QuantityPropertyName ) );
                    Ret.AddViewPropertyFilter( QuantityVp, CswNbtSubField.SubFieldName.Value, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThan, Value: "0" );

                    CswNbtViewProperty ExpirationDateVp = Ret.AddViewProperty( ContainerRel, ContainerOc.getObjectClassProp( CswNbtObjClassContainer.ExpirationDatePropertyName ) );
                    Ret.AddViewPropertyFilter( ExpirationDateVp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals, Value: "today" );
                }
            }

            return Ret;
        }

        #endregion Public Methods
    }
}
