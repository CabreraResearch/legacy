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

        public JObject dispenseSourceContainer( string DispenseType, string Quantity, string UnitId )
        {
            JObject ret = new JObject();
            CswPrimaryKey UnitOfMeasurePK = new CswPrimaryKey();
            UnitOfMeasurePK.FromString( UnitId );
            if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() )
            {
                _addMaterialToContainer( CswConvert.ToDouble( Quantity ), UnitOfMeasurePK );
            }
            else if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste.ToString() )
            {
                _wasteMaterialFromContainer( CswConvert.ToDouble( Quantity ), UnitOfMeasurePK );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Failed to dispense container: Dispense type not supported.", "Invalid Dispense Type." );
            }
            string ViewId = _getViewForAllDispenseContainers();
            ret["viewId"] = ViewId;

            return ret;
        }

        public JObject dispenseIntoChildContainers( string ContainerNodeTypeId, string DispenseGrid )
        {
            JObject ret = new JObject();
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
                    _dispenseMaterialFromContainer( ContainerNodeTypeId, NumOfContainers, QuantityToDispense, UnitOfMeasurePK, Barcode );
                }
            }
            _SourceContainer.postChanges( false );

            string ViewId = _getViewForAllDispenseContainers();
            ret["viewId"] = ViewId;
            ret["barcodeId"] = _SourceContainer.NodeId.ToString() + "_" + _SourceContainer.Barcode.NodeTypePropId.ToString();

            return ret;
        }

        private void _addMaterialToContainer( double Quantity, CswPrimaryKey UnitId )
        {
            double QuantityToAdd = _getDispenseAmountInProperUnits( Quantity, UnitId, _SourceContainer.Quantity.UnitId );
            _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity + QuantityToAdd;
            _SourceContainer.postChanges( false );
            _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Add, QuantityToAdd, _SourceContainer.NodeId );
        }

        private void _wasteMaterialFromContainer( double Quantity, CswPrimaryKey UnitId )
        {
            double QuantityToWaste = _getDispenseAmountInProperUnits( Quantity, UnitId, _SourceContainer.Quantity.UnitId );
            _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity - QuantityToWaste;
            _SourceContainer.postChanges( false );
            _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste, QuantityToWaste );
        }

        private void _dispenseMaterialFromContainer( string ContainerNodeTypeId, int NumOfContainers, double QuantityToDispense, CswPrimaryKey UnitId, string Barcode )
        {
            if( NumOfContainers == 0 )
            {
                double QuantityDispensed = _getDispenseAmountInProperUnits( QuantityToDispense, UnitId, _SourceContainer.Quantity.UnitId );
                _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity - QuantityDispensed;
                _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, QuantityDispensed );
            }
            else
            {
                for( Int32 i = 0; i < NumOfContainers; i += 1 )
                {
                    double SourceQuantityDispensed = _getDispenseAmountInProperUnits( QuantityToDispense, UnitId, _SourceContainer.Quantity.UnitId );
                    CswNbtObjClassContainer ChildContainer = _createChildContainer( ContainerNodeTypeId, QuantityToDispense, UnitId, Barcode );
                    _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity - SourceQuantityDispensed;
                    _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Dispense, SourceQuantityDispensed, ChildContainer.NodeId );
                }
            }
        }

        private double _getDispenseAmountInProperUnits( double Quantity, CswPrimaryKey OldUnitId, CswPrimaryKey NewUnitId )
        {
            double ValueToConvert = Quantity;
            double convertedValue = ValueToConvert;
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, _SourceContainer.Material.RelatedNodeId );
            convertedValue = ConversionObj.convertUnit( ValueToConvert );
            return convertedValue;
        }

        private CswNbtObjClassContainer _createChildContainer( string ContainerNodeTypeId, double QuantityDispensed, CswPrimaryKey UnitId, string Barcode )
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
                ChildContainer.Quantity.Quantity = QuantityDispensed;
                ChildContainer.Quantity.UnitId = UnitId;
                ChildContainer.postChanges( false );
                _ContainersToView.Add( ChildContainer.NodeId );
            }
            return ChildContainer;
        }

        private void _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityDispensed, CswPrimaryKey DestinationId = null )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                ContDispTransNode.SourceContainer.RelatedNodeId = _SourceContainer.NodeId;
                ContDispTransNode.DestinationContainer.RelatedNodeId = DestinationId;
                ContDispTransNode.QuantityDispensed.Quantity = QuantityDispensed;
                ContDispTransNode.QuantityDispensed.UnitId = _SourceContainer.Quantity.UnitId;
                ContDispTransNode.Type.Value = DispenseType.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Today;
                ContDispTransNode.RemainingSourceContainerQuantity.Quantity = _SourceContainer.Quantity.Quantity;
                ContDispTransNode.RemainingSourceContainerQuantity.UnitId = _SourceContainer.Quantity.UnitId;

                ContDispTransNode.postChanges( false );
            }
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
