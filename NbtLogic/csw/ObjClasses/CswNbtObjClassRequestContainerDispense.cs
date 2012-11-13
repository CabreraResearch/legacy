using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Dispense Request Item
    /// </summary>
    public class CswNbtObjClassRequestContainerDispense : CswNbtPropertySetRequestItem
    {
        #region Enums
        /// <summary>
        /// Property Names (Includes Property Names inherited from base class <see cref="CswNbtPropertySetRequestItem"/>)
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
        {
            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Container (<see cref="CswNbtObjClassContainer"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Container = "Container";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material (<see cref="CswNbtObjClassMaterial"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Material = "Material";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material Size of the Container (<see cref="CswNbtObjClassSize"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Size = "Size";

            /// <summary>
            /// For "Request By Bulk" items, the Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
            /// </summary>
            public const string Quantity = "Quantity";

            /// <summary>
            /// For Dispense requests, the total amount(<see cref="CswNbtNodePropQuantity"/>) dispensed.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalDispensed = "Total Dispensed";

        }

        public new sealed class Types : CswNbtPropertySetRequestItem.Types
        {
            public const string ContainerDispense = "Request Container Dispense";
        }

        /// <summary>
        /// Possible values (Includes Statuses inherited from base class <see cref="CswNbtPropertySetRequestItem"/>)
        /// </summary>
        public new sealed class Statuses : CswNbtPropertySetRequestItem.Statuses
        {
            /// <summary>
            /// The Request Item has been Dispensed
            /// </summary>
            public const string Dispensed = "Dispensed";

            /// <summary>
            /// Statuses Options as CswCommaDelimitedString
            /// </summary>
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Dispensed, Completed, Cancelled
                };
        }

        /// <summary>
        /// Possible Fulfill menu options (Includes menu inherited from base class <see cref="CswNbtPropertySetRequestItem"/>)
        /// </summary>
        public new sealed class FulfillMenu : CswNbtPropertySetRequestItem.FulfillMenu
        {
            /// <summary>
            /// Dispense for this Request Item
            /// </summary>
            public const string Dispense = "Dispense this Container";

            /// <summary>
            /// Fulfill Menu Options as CswCommaDelimitedString
            /// </summary>
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Dispense, Complete, Cancel
                };
        }

        #endregion Enums

        #region Base

        /// <summary>
        /// Implicit Node cast to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassRequestContainerDispense( CswNbtNode Node )
        {
            CswNbtObjClassRequestContainerDispense ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestContainerDispenseClass ) )
            {
                ret = (CswNbtObjClassRequestContainerDispense) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassRequestContainerDispense fromPropertySet( CswNbtPropertySetRequestItem PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetRequestItem toPropertySet( CswNbtObjClassRequestContainerDispense ObjClass )
        {
            return ObjClass;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtObjClassRequestContainerDispense( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {

        }//ctor()

        /// <summary>
        /// 
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass ); }
        }

        #endregion Base

        #region Inherited Events

        /// <summary>
        /// Abstract override to toggle read only properties
        /// </summary>
        public override void toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance )
        {
            if( null != ItemInstance )
            {
                CswNbtObjClassRequestContainerDispense ThisRequest = (CswNbtObjClassRequestContainerDispense) ItemInstance;
                ThisRequest.Quantity.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Container.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Material.setReadOnly( value: IsReadOnly, SaveToDb: true );
            }
        }

        public override string setRequestDescription()
        {
            return "Dispense " + Quantity.Gestalt + " from " + Container.Gestalt + " of " + Material.Gestalt;
        }

        /// <summary>
        /// Abstract override to be called on beforeWriteNode
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( CswTools.IsPrimaryKey( InventoryGroup.RelatedNodeId ) )
            {
                CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes[Container.RelatedNodeId];
                if( null == NodeAsContainer )
                {
                    throw new CswDniException( ErrorType.Warning,
                                              "A Container Dispense " +
                                              " type of Request Item requires a valid Container.",
                                              "Attempted to edit node without a valid Container relationship." );
                }
                CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes[NodeAsContainer.Location.SelectedNodeId];
                if( null != NodeAsLocation &&
                    InventoryGroup.RelatedNodeId != NodeAsLocation.InventoryGroup.RelatedNodeId )
                {
                    throw new CswDniException( ErrorType.Warning,
                                              "For a Container Dispense " +
                                              " type of Request Item, the Inventory Group of the Request must match the Inventory Group of the Container's Location.",
                                              "Attempted to edit node without matching Container and Request Inventory Group relationships." );
                }
            }

            //case 2753 - naming logic
            if( false == IsTemp )
            {
                if( null != Container.RelatedNodeId )
                {
                    CswNbtObjClassContainer containerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                    Name.Text = "Dispense " + containerNode.Quantity.Quantity + containerNode.Quantity.CachedUnitName + " from Container " + containerNode.Barcode.Barcode;
                }
            }
        }

        /// <summary>
        /// Abstract override to be called on afterWriteNode
        /// </summary>
        public override void afterPropertySetWriteNode()
        {

        }

        /// <summary>
        /// Abstract override to be called on afterPopulateProps
        /// </summary>
        public override void afterPropertySetPopulateProps()
        {
            Quantity.SetOnPropChange( onQuantityPropChange );
            TotalDispensed.SetOnPropChange( onTotalDispensedPropChange );
            Material.SetOnPropChange( onMaterialPropChange );
            Container.SetOnPropChange( onContainerPropChange );
            Status.SetOnPropChange( onStatusPropChange );
        }

        /// <summary>
        /// Abstract override to be called on onButtonClick
        /// </summary>
        public override bool onPropertySetButtonClick( CswNbtMetaDataObjectClassProp OCP, NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                switch( OCP.PropName )
                {
                    case PropertyName.Fulfill:
                        CswNbtObjClassContainer NodeAsContainer = null;
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Dispense:
                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != NodeAsContainer && null != NodeAsContainer.Dispense.NodeTypeProp )
                                {
                                    NbtButtonData DispenseData = new NbtButtonData( NodeAsContainer.Dispense.NodeTypeProp );
                                    NodeAsContainer.onButtonClick( DispenseData );
                                    ButtonData.clone( DispenseData );
                                }
                                else
                                {
                                    ButtonData.Data["containernodetypeid"] = Container.TargetId;
                                    ButtonData.Data["containerobjectclassid"] = Container.TargetId;
                                    JObject InitialQuantity = null;
                                    if( null != Size.RelatedNodeId && Int32.MinValue != Size.RelatedNodeId.PrimaryKey )
                                    {
                                        CswNbtObjClassSize NodeAsSize = _CswNbtResources.Nodes[Size.RelatedNodeId];
                                        if( null != NodeAsSize )
                                        {
                                            InitialQuantity = new JObject();
                                            NodeAsSize.InitialQuantity.ToJSON( InitialQuantity );
                                            ButtonData.Data["initialQuantity"] = InitialQuantity;
                                        }
                                    }
                                    else if( false == Quantity.Empty )
                                    {
                                        InitialQuantity = new JObject();
                                        Quantity.ToJSON( InitialQuantity );
                                    }
                                    if( null != InitialQuantity )
                                    {
                                        ButtonData.Data["initialQuantity"] = InitialQuantity;
                                    }
                                    ButtonData.Action = NbtButtonAction.dispense;
                                }
                                break;

                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["containerid"] = ( Container.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["locationid"] = ( Location.SelectedNodeId ?? new CswPrimaryKey() ).ToString();
                        break; //case PropertyName.Fulfill:
                }
            }
            return true;
        }

        private void _getNextStatus( string ButtonText )
        {
            switch( ButtonText )
            {
                case FulfillMenu.Cancel:
                    setNextStatus( Statuses.Cancelled );
                    break;
                case FulfillMenu.Complete:
                    setNextStatus( Statuses.Completed );
                    break;
            }
        }

        /// <summary>
        /// Move to the next permitted Status based on the current Status
        /// </summary>
        public void setNextStatus( string StatusVal )
        {
            switch( Status.Value )
            {
                case Statuses.Submitted:
                    if( StatusVal == Statuses.Dispensed || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;
                case Statuses.Dispensed:
                    if( StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;
            }
        }

        #endregion

        #region CswNbtPropertySetRequestItem Members

        /// <summary>
        /// Request-specific Status property change events
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop )
        {
            TotalDispensed.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Dispensed:
                    if( TotalDispensed.Quantity >= Quantity.Quantity )
                    {
                        Fulfill.State = FulfillMenu.Complete;
                    }
                    break;
            }
        }

        public override void onTypePropChange( CswNbtNodeProp Prop )
        {
            Fulfill.MenuOptions = FulfillMenu.Options.ToString();
            Fulfill.State = FulfillMenu.Dispense;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }
        private void onQuantityPropChange( CswNbtNodeProp Prop )
        {
            if( TotalDispensed.Precision != Quantity.Precision )
            {
                TotalDispensed.NodeTypeProp.NumberPrecision = Quantity.Precision;
            }
            if( TotalDispensed.UnitId != Quantity.UnitId )
            {
                TotalDispensed.UnitId = Quantity.UnitId;
            }
            TotalDispensed.UnitId = Quantity.UnitId;
        }

        public CswNbtNodePropRelationship Size
        {
            get { return _CswNbtNode.Properties[PropertyName.Size]; }
        }

        public CswNbtNodePropRelationship Material
        {
            get { return _CswNbtNode.Properties[PropertyName.Material]; }
        }
        private void onMaterialPropChange( CswNbtNodeProp Prop )
        {
            if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
            {
                Material.setReadOnly( value: true, SaveToDb: true );
                CswNbtNode MaterialNode = _CswNbtResources.Nodes[Material.RelatedNodeId];
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                Vb.setQuantityUnitOfMeasureView( MaterialNode, Quantity );
                Vb.setQuantityUnitOfMeasureView( MaterialNode, TotalDispensed );
                TotalDispensed.Quantity = 0;
            }
        }
        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container]; }
        }
        private void onContainerPropChange( CswNbtNodeProp Prop )
        {
            if( null != Container.RelatedNodeId )
            {
                Container.setReadOnly( value: true, SaveToDb: true );
                if( null == Material.RelatedNodeId )
                {
                    CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes[Container.RelatedNodeId];
                    if( null != NodeAsContainer )
                    {
                        Material.RelatedNodeId = NodeAsContainer.Material.RelatedNodeId;
                    }
                }
            }
        }

        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
        private void onTotalDispensedPropChange( CswNbtNodeProp Prop )
        {
            if( Status.Value != Statuses.Pending &&
                Status.Value != Statuses.Cancelled &&
                Status.Value != Statuses.Completed )
            {
                if( TotalDispensed.Quantity >= Quantity.Quantity )
                {
                    Fulfill.State = FulfillMenu.Complete;
                }
                else if( TotalDispensed.Quantity > 0 )
                {
                    Fulfill.State = FulfillMenu.Dispense;
                    Status.Value = Statuses.Dispensed;
                }
            }
        }



        #endregion

    }//CswNbtObjClassRequestContainerDispense

}//namespace ChemSW.Nbt.ObjClasses
