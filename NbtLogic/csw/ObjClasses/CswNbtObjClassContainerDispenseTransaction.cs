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

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string SourceContainer = "Source Container";
            public const string DestinationContainer = "Destination Container";
            public const string QuantityDispensed = "Quantity Dispensed";
            public const string Type = "Dispense Type";
            public const string DispensedDate = "Dispensed Date";
            public const string RemainingSourceContainerQuantity = "Remaining Source Container Quantity";
            public const string RequestItem = "Request Item";
        }

        #endregion

        #region ctor

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainerDispenseTransaction( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

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

        #endregion

        #region Inherited Events

        private void _onCreateFromRequestItem()
        {
            if( CswTools.IsPrimaryKey( RequestItem.RelatedNodeId ) )
            {
                CswNbtPropertySetRequestItem NodeAsPropSet = _CswNbtResources.Nodes[ RequestItem.RelatedNodeId ];
                if( null != NodeAsPropSet )
                {
                    if( Type.Value == CswEnumNbtContainerDispenseType.Dispense.ToString() )
                    {
                        CswNbtUnitConversion Conversion = null;
                        switch( NodeAsPropSet.Type.Value )
                        {
                            case CswNbtObjClassRequestContainerDispense.Types.ContainerDispense:
                                CswNbtObjClassRequestContainerDispense NodeAsCd = CswNbtObjClassRequestContainerDispense.fromPropertySet( NodeAsPropSet );
                                if( false == CswTools.IsPrimaryKey( NodeAsCd.TotalDispensed.UnitId ) )
                                {
                                    NodeAsCd.TotalDispensed.UnitId = QuantityDispensed.UnitId;
                                }
                                NodeAsCd.setNextStatus( CswNbtObjClassRequestContainerDispense.Statuses.Dispensed );

                                Conversion = new CswNbtUnitConversion( _CswNbtResources, QuantityDispensed.UnitId, NodeAsCd.TotalDispensed.UnitId, NodeAsCd.Material.RelatedNodeId );
                                NodeAsCd.TotalDispensed.Quantity -= Conversion.convertUnit( QuantityDispensed.Quantity ); // Subtracting a negative number in order to add
                                break;

                            default:
                                CswNbtObjClassRequestMaterialDispense NodeAsMd = CswNbtObjClassRequestMaterialDispense.fromPropertySet( NodeAsPropSet );
                                if( false == CswTools.IsPrimaryKey( NodeAsMd.TotalDispensed.UnitId ) )
                                {
                                    NodeAsMd.TotalDispensed.UnitId = QuantityDispensed.UnitId;
                                }
                                NodeAsMd.setNextStatus( CswNbtObjClassRequestMaterialDispense.Statuses.Dispensed );

                                Conversion = new CswNbtUnitConversion( _CswNbtResources, QuantityDispensed.UnitId, NodeAsMd.TotalDispensed.UnitId, NodeAsMd.Material.RelatedNodeId );
                                NodeAsMd.TotalDispensed.Quantity -= Conversion.convertUnit( QuantityDispensed.Quantity ); // Subtracting a negative number in order to add
                                break;
                        }
                    }
                    else if( Type.Value == CswEnumNbtContainerDispenseType.Dispose.ToString() )
                    {
                        CswNbtObjClassRequestContainerUpdate NodeAsCu = CswNbtObjClassRequestContainerUpdate.fromPropertySet( NodeAsPropSet );
                        NodeAsCu.setNextStatus( CswNbtObjClassRequestContainerUpdate.Statuses.Disposed );
                    }
                    else if( Type.Value == CswEnumNbtContainerDispenseType.Receive.ToString() )
                    {
                        CswNbtObjClassRequestMaterialDispense NodeAsMd = CswNbtObjClassRequestMaterialDispense.fromPropertySet( NodeAsPropSet );
                        NodeAsMd.setNextStatus( CswNbtObjClassRequestMaterialDispense.Statuses.Received );
                    }
                    NodeAsPropSet.postChanges( true );
                }
            }
        }

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _onCreateFromRequestItem();
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtNode.setReadOnly( value: true, SaveToDb: true ); //case 24508

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship SourceContainer
        {
            get { return _CswNbtNode.Properties[PropertyName.SourceContainer]; }
        }
        public CswNbtNodePropRelationship DestinationContainer
        {
            get { return _CswNbtNode.Properties[PropertyName.DestinationContainer]; }
        }
        public CswNbtNodePropQuantity QuantityDispensed
        {
            get { return _CswNbtNode.Properties[PropertyName.QuantityDispensed]; }
        }
        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }
        public CswNbtNodePropDateTime DispensedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.DispensedDate]; }
        }
        public CswNbtNodePropQuantity RemainingSourceContainerQuantity
        {
            get { return _CswNbtNode.Properties[PropertyName.RemainingSourceContainerQuantity]; }
        }
        public CswNbtNodePropRelationship RequestItem
        {
            get { return _CswNbtNode.Properties[PropertyName.RequestItem]; }
        }
        
        #endregion

    }//CswNbtObjClassContainerDispenseTransaction

}//namespace ChemSW.Nbt.ObjClasses

