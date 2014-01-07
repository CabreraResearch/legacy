using System;
using ChemSW.Core;
using ChemSW.Nbt.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Dispense Transaction Object Class
    /// </summary>
    public class CswNbtObjClassContainerDispenseTransaction : CswNbtObjClass
    {
        #region Static Properties

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string SourceContainer = "Source Container";
            public const string DestinationContainer = "Destination Container";
            public const string QuantityDispensed = "Quantity Dispensed";
            public const string Type = "Dispense Type";
            public const string DispensedDate = "Dispensed Date";
            public const string RemainingSourceContainerQuantity = "Remaining Source Container Quantity";
            public const string RequestItem = "Request Item";
            public const string Dispenser = "Dispenser";
        }

        #endregion Static Properties

        #region ctor

        public CswNbtObjClassContainerDispenseTransaction( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerDispenseTransaction
        /// </summary>
        public static implicit operator CswNbtObjClassContainerDispenseTransaction( CswNbtNode Node )
        {
            CswNbtObjClassContainerDispenseTransaction ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ContainerDispenseTransactionClass ) )
            {
                ret = (CswNbtObjClassContainerDispenseTransaction) Node.ObjClass;
            }
            return ret;
        }

        #endregion ctor

        #region Inherited Events

        //updates total dispensed for all dispense request fulfillment actions 
        private void _onCreateFromRequestItem()
        {
            if( CswTools.IsPrimaryKey( RequestItem.RelatedNodeId ) )
            {
                CswNbtObjClassRequestItem RequestItemNode = _CswNbtResources.Nodes[RequestItem.RelatedNodeId];
                if( null != RequestItemNode )
                {
                    if( Type.Value == CswEnumNbtContainerDispenseType.Dispense.ToString() )
                    {
                        CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, QuantityDispensed.UnitId, RequestItemNode.TotalDispensed.UnitId, RequestItemNode.Material.RelatedNodeId );
                        double DispensedQuantity = Conversion.convertUnit( QuantityDispensed.Quantity );
                        RequestItemNode.TotalDispensed.Quantity -= DispensedQuantity; // Subtracting a negative number in order to add
                        RequestItemNode.FulfillmentHistory.AddComment( "Dispensed " + QuantityDispensed.Gestalt + " into " + CswNbtNode.getNodeLink( DestinationContainer.RelatedNodeId, DestinationContainer.Gestalt ) );
                        RequestItemNode.Status.Value = CswNbtObjClassRequestItem.Statuses.Dispensed;
                    }
                    RequestItemNode.postChanges( false );
                }
            }
        }

        public override void beforePromoteNode( bool OverrideUniqueValidation = false )
        {
            _CswNbtNode.setReadOnly( value: true, SaveToDb: true ); //case 24508
        }

        public override void afterPromoteNode()
        {
            _onCreateFromRequestItem();
        }

        #endregion Inherited Events

        #region Object class specific properties

        public CswNbtNodePropRelationship SourceContainer { get { return _CswNbtNode.Properties[PropertyName.SourceContainer]; } }
        public CswNbtNodePropRelationship DestinationContainer { get { return _CswNbtNode.Properties[PropertyName.DestinationContainer]; } }
        public CswNbtNodePropQuantity QuantityDispensed { get { return _CswNbtNode.Properties[PropertyName.QuantityDispensed]; } }
        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropDateTime DispensedDate { get { return _CswNbtNode.Properties[PropertyName.DispensedDate]; } }
        public CswNbtNodePropQuantity RemainingSourceContainerQuantity { get { return _CswNbtNode.Properties[PropertyName.RemainingSourceContainerQuantity]; } }
        public CswNbtNodePropRelationship Dispenser { get { return _CswNbtNode.Properties[PropertyName.Dispenser]; } }
        public CswNbtNodePropRelationship RequestItem { get { return _CswNbtNode.Properties[PropertyName.RequestItem]; } }

        #endregion

    }//CswNbtObjClassContainerDispenseTransaction

}//namespace ChemSW.Nbt.ObjClasses