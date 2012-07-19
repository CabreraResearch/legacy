using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Conversion;
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
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Dispense action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
        }

        #endregion Constructor

        #region Public Methods

        public JObject dispenseSourceContainer( string DispenseType, string Quantity, string UnitId, string RequestItemId )
        {
            JObject ret = new JObject();
            CswPrimaryKey UnitOfMeasurePk = new CswPrimaryKey();
            UnitOfMeasurePk.FromString( UnitId );
            CswPrimaryKey RequestItemPk = new CswPrimaryKey();
            RequestItemPk.FromString( RequestItemId );
            Double RealQuantity = CswConvert.ToDouble( Quantity );

            if( DispenseType.Contains( CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() ) )
            {
                RealQuantity = -RealQuantity; // deducting negative quantity is adding quantity
            }

            _SourceContainer.DispenseOut( (CswNbtObjClassContainerDispenseTransaction.DispenseType) DispenseType, RealQuantity, UnitOfMeasurePk, RequestItemPk );
            _SourceContainer.postChanges( false );
            
            string ViewId = _getViewForAllDispenseContainers();
            ret["viewId"] = ViewId;

            return ret;
        }

        public JObject dispenseIntoChildContainers( string ContainerNodeTypeId, string DispenseGrid, string RequestItemId )
        {
            JObject ret = new JObject();
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
                        _SourceContainer.DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense, UnitOfMeasurePK, RequestItemPk );
                        _SourceContainer.postChanges( false );
                    }
                    else
                    {
                        for( Int32 c = 0; c < NumOfContainers; c += 1 )
                        {
                            CswNbtObjClassContainer ChildContainer = _createChildContainer( ContainerNodeTypeId, UnitOfMeasurePK, Barcode );
                            _SourceContainer.DispenseOut( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense, UnitOfMeasurePK, RequestItemPk, ChildContainer );
                            ChildContainer.DispenseIn( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityToDispense, UnitOfMeasurePK, RequestItemPk, _SourceContainer );
                            ChildContainer.postChanges( false );
                        }
                        _SourceContainer.postChanges( false );
                    }
                }
            }
            _SourceContainer.postChanges( false );

            string ViewId = _getViewForAllDispenseContainers();
            ret["viewId"] = ViewId;
            ret["barcodeId"] = _SourceContainer.NodeId.ToString() + "_" + _SourceContainer.Barcode.NodeTypePropId.ToString();

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

        #endregion Public Methods
    }
}
