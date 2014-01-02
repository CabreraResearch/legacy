using ChemSW.Core;
using ChemSW.Nbt.Conversion;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtContainerDispenser
    {
        private CswNbtObjClassContainer _Container;
        private CswNbtResources _CswNbtResources;
        private CswNbtContainerDispenseTransactionBuilder _ContainerDispenseTransactionBuilder;

        #region Constructor

        public CswNbtContainerDispenser( CswNbtResources CswNbtResources, CswNbtContainerDispenseTransactionBuilder ContainerDispenseTransactionBuilder, CswNbtObjClassContainer Container )
        {
            _CswNbtResources = CswNbtResources;
            _ContainerDispenseTransactionBuilder = ContainerDispenseTransactionBuilder;
            _Container = Container;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Dispense out of this container.
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToDeduct">Positive quantity to subtract</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="DestinationContainer"></param>
        public void DispenseOut( CswEnumNbtContainerDispenseType DispenseType, double QuantityToDeduct, CswPrimaryKey UnitId,
                                 CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer DestinationContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToDeduct = _getDispenseAmountInProperUnits( QuantityToDeduct, UnitId, _Container.Quantity.UnitId );
            double CurrentQuantity = 0;
            if( CswTools.IsDouble( _Container.Quantity.Quantity ) )
            {
                CurrentQuantity = _Container.Quantity.Quantity;
            }
            _Container.Quantity.Quantity = CurrentQuantity - RealQuantityToDeduct;

            if( DestinationContainer != null )
            {
                DestinationContainer.DispenseIn( DispenseType, QuantityToDeduct, UnitId, RequestItemId, _Container, false );  // false, because we do not want another duplicate transaction record
            }
            if( RecordTransaction )
            {
                _ContainerDispenseTransactionBuilder.create( DispenseType, -RealQuantityToDeduct, _Container.Quantity.UnitId, RequestItemId, _Container, DestinationContainer );
            }
            _Container.CreateContainerLocationNode( CswEnumNbtContainerLocationTypeOptions.Dispense );
        } // DispenseOut()

        /// <summary>
        /// Dispense into this container.  
        /// </summary>
        /// <param name="DispenseType"></param>
        /// <param name="QuantityToAdd">Positive quantity to add</param>
        /// <param name="UnitId"></param>
        /// <param name="RequestItemId"></param>
        /// <param name="SourceContainer"></param>
        public void DispenseIn( CswEnumNbtContainerDispenseType DispenseType, double QuantityToAdd, CswPrimaryKey UnitId,
                                CswPrimaryKey RequestItemId = null, CswNbtObjClassContainer SourceContainer = null, bool RecordTransaction = true )
        {
            double RealQuantityToAdd = _getDispenseAmountInProperUnits( QuantityToAdd, UnitId, _Container.Quantity.UnitId );
            double CurrentQuantity = 0;
            if( CswTools.IsDouble( _Container.Quantity.Quantity ) )
            {
                CurrentQuantity = _Container.Quantity.Quantity;
            }
            _Container.Quantity.Quantity = CurrentQuantity + RealQuantityToAdd;
            if( RecordTransaction )
            {
                _ContainerDispenseTransactionBuilder.create( DispenseType, RealQuantityToAdd, _Container.Quantity.UnitId, RequestItemId, SourceContainer, _Container );
            }
            CswEnumNbtContainerLocationTypeOptions ContainerLocationType =
                SourceContainer == null ? CswEnumNbtContainerLocationTypeOptions.Receipt
                                        : CswEnumNbtContainerLocationTypeOptions.Dispense;
            _Container.CreateContainerLocationNode( ContainerLocationType );
        } // DispenseIn()

        #endregion Public Methods

        #region Private Methods

        private double _getDispenseAmountInProperUnits( double Amount, CswPrimaryKey OldUnitId, CswPrimaryKey NewUnitId )
        {
            double convertedValue = Amount;
            if( OldUnitId != NewUnitId )
            {
                CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, OldUnitId, NewUnitId, _Container.Material.RelatedNodeId );
                convertedValue = ConversionObj.convertUnit( Amount );
            }
            return convertedValue;
        }
        
        #endregion Private Methods
    }
}
