using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Conversion;
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

        public sealed class PropertyName
        {
            public const string SourceContainer = "Source Container";
            public const string DestinationContainer = "Destination Container";
            public const string QuantityDispensed = "Quantity Dispensed";
            public const string Type = "Dispense Type";
            public const string DispensedDate = "Dispensed Date";
            public const string RemainingSourceContainerQuantity = "Remaining Source Container Quantity";
            public const string RequestItem = "Request Item";
        }

        public sealed class DispenseType : CswEnum<DispenseType>
        {
            private DispenseType( string Name ) : base( Name ) { }
            public static IEnumerable<DispenseType> _All { get { return All; } }
            public static implicit operator DispenseType( string str )
            {
                DispenseType ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly DispenseType Unknown = new DispenseType( "Unknown" );
            /// <summary>
            /// Add new (child) containers with material specified in existing source container (no parent container)
            /// </summary>
            public static readonly DispenseType Receive = new DispenseType( "Receive" );

            /// <summary>
            /// Transfer material from a source (parent) container to zero or more destination (child) containers
            /// </summary>
            public static readonly DispenseType Dispense = new DispenseType( "Dispense" );

            /// <summary>
            /// Transfer material from a source (parent) container to an undocumented location (no child containers)
            /// </summary>
            public static readonly DispenseType Waste = new DispenseType( "Waste" );

            /// <summary>
            /// Empty material from a source (parent) container and mark as disposed (no child containers)
            /// </summary>
            public static readonly DispenseType Dispose = new DispenseType( "Dispose" );

            /// <summary>
            /// Add material to an existing source container (no parent container, no child containers)
            /// </summary>
            public static readonly DispenseType Add = new DispenseType( "Add" );

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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerDispenseTransaction
        /// </summary>
        public static implicit operator CswNbtObjClassContainerDispenseTransaction( CswNbtNode Node )
        {
            CswNbtObjClassContainerDispenseTransaction ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass ) )
            {
                ret = (CswNbtObjClassContainerDispenseTransaction) Node.ObjClass;
            }
            return ret;
        }

        #endregion

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtNode.setReadOnly( value: true, SaveToDb: true ); //case 24508

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            RequestItem.SetOnPropChange( OnRequestItemPropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
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
        private void OnRequestItemPropChange( CswNbtNodeProp Prop )
        {
            if( null != RequestItem.RelatedNodeId &&
                Int32.MinValue != RequestItem.RelatedNodeId.PrimaryKey )
            {
                Int32 RequestItemOriginalValue = CswConvert.ToInt32( RequestItem.GetOriginalPropRowValue( CswNbtSubField.SubFieldName.NodeID ) );
                if( RequestItemOriginalValue != RequestItem.RelatedNodeId.PrimaryKey )
                {
                    CswNbtObjClassRequestItem NodeAsRequestItem = _CswNbtResources.Nodes[RequestItem.RelatedNodeId];
                    if( null != NodeAsRequestItem )
                    {
                        if( null == NodeAsRequestItem.TotalDispensed.UnitId )
                        {
                            NodeAsRequestItem.TotalDispensed.UnitId = QuantityDispensed.UnitId;
                        }
                        CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, QuantityDispensed.UnitId, NodeAsRequestItem.TotalDispensed.UnitId, NodeAsRequestItem.Material.RelatedNodeId );
                        if( Type.Value == DispenseType.Dispense.ToString() )
                        {
                            NodeAsRequestItem.setNextStatus( CswNbtObjClassRequestItem.Statuses.Dispensed );
                            NodeAsRequestItem.TotalDispensed.Quantity -= Conversion.convertUnit( QuantityDispensed.Quantity );  // Subtracting a negative number in order to add
                        }
                        else if( Type.Value == DispenseType.Dispose.ToString() )
                        {
                            NodeAsRequestItem.setNextStatus( CswNbtObjClassRequestItem.Statuses.Disposed );
                        }
                        else if( Type.Value == DispenseType.Receive.ToString() )
                        {
                            NodeAsRequestItem.setNextStatus( CswNbtObjClassRequestItem.Statuses.Received );
                        }
                        NodeAsRequestItem.postChanges( true );
                    }
                }
            }
        }
        #endregion

    }//CswNbtObjClassContainerDispenseTransaction

}//namespace ChemSW.Nbt.ObjClasses

