using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.csw.Actions
{
    public class CswNbtActDispenseContainer
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtObjClassContainer _SourceContainer = null;

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

        public JObject updateDispensedContainer( string DispenseType, string Quantity, string UnitId )
        {
            JObject ret = new JObject();
            if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Add.ToString() )
            {
                ret = _addMaterialToContainer( Quantity, UnitId );
            }
            else if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste.ToString() )
            {
                ret = _wasteMaterialFromContainer( Quantity, UnitId );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Failed to dispense container: Dispense type not supported.", "Invalid Dispense Type." );
            }
            return ret;
        }

        private JObject _addMaterialToContainer( string Quantity, string UnitId )
        {
            double QuantityToAdd = _getDispenseAmountInProperUnits( Quantity, UnitId );
            _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity + QuantityToAdd;
            _SourceContainer.postChanges( false );
            _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Add, QuantityToAdd );
            //TODO - create view with the container and return it
            return new JObject();
        }

        private JObject _wasteMaterialFromContainer( string Quantity, string UnitId )
        {
            double QuantityToWaste = _getDispenseAmountInProperUnits( Quantity, UnitId );
            _SourceContainer.Quantity.Quantity = _SourceContainer.Quantity.Quantity - QuantityToWaste;
            _SourceContainer.postChanges( false );
            _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType.Waste, QuantityToWaste );
            //TODO - waste quantity from source container, create transaction node
            return new JObject();
        }

        private double _getDispenseAmountInProperUnits( string Quantity, string UnitId )
        {
            double ValueToConvert = CswConvert.ToDouble( Quantity );
            double convertedValue = ValueToConvert;
            CswPrimaryKey UnitOfMeasurePK = new CswPrimaryKey();
            UnitOfMeasurePK.FromString( UnitId );
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, UnitOfMeasurePK, _SourceContainer.Quantity.UnitId, _SourceContainer.Material.RelatedNodeId );
            try
            {
                convertedValue = ConversionObj.convertUnit( ValueToConvert );
            }
            catch( Exception e )
            {
                throw new CswDniException( ErrorType.Error, "Failed to dispense container: Source Container has unknown Quantity.", "Dispense failed - Source Container Quantity is null: " + e.StackTrace );
            }
            return convertedValue;
        }

        public JObject upsertDispenseContainers( string ContainerNodeTypeId, string DesignGrid )
        {
            JArray GridArray = JArray.Parse( DesignGrid );
            //TODO - create distination containers with respective quantities, create transaction nodes for each dispense instance, update source container
            //TODO - create view with the container and return it
            return new JObject();
        }

        private void _createContainerTransactionNode( CswNbtObjClassContainerDispenseTransaction.DispenseType DispenseType, double QuantityDispensed )
        {
            CswNbtMetaDataNodeType ContDispTransNT = _CswNbtResources.MetaData.getNodeType( "Container Dispense Transaction" );
            if( ContDispTransNT != null )
            {
                CswNbtObjClassContainerDispenseTransaction ContDispTransNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContDispTransNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                ContDispTransNode.SourceContainer.RelatedNodeId = _SourceContainer.NodeId;
                if( DispenseType == CswNbtObjClassContainerDispenseTransaction.DispenseType.Add )
                {
                    ContDispTransNode.DestinationContainer.RelatedNodeId = _SourceContainer.NodeId;
                }
                ContDispTransNode.QuantityDispensed.Quantity = QuantityDispensed;
                ContDispTransNode.QuantityDispensed.UnitId = _SourceContainer.Quantity.UnitId;
                ContDispTransNode.Type.Value = DispenseType.ToString();
                ContDispTransNode.DispensedDate.DateTimeValue = DateTime.Today;
                ContDispTransNode.RemainingSourceContainerQuantity.Quantity = _SourceContainer.Quantity.Quantity;
                ContDispTransNode.RemainingSourceContainerQuantity.UnitId = _SourceContainer.Quantity.UnitId;

                ContDispTransNode.postChanges( false );
            }
        }

        #endregion Public Methods
    }
}
