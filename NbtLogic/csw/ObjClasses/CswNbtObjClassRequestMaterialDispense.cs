using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Material Dispense Request Item
    /// </summary>
    public class CswNbtObjClassRequestMaterialDispense : CswNbtPropertySetRequestItem
    {
        #region Enums

        /// <summary>
        /// Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
        {
            /// <summary>
            /// Batch (<see cref="CswNbtNodePropRelationship"/>)
            /// </summary>
            public const string Batch = "Batch";

            /// <summary>
            /// For "Request By Size" items, the number(<see cref="CswNbtNodePropNumber"/>) of sizes(<see cref="Size"/>) to request. 
            /// </summary>
            public const string Count = "Count";

            /// <summary>
            /// True if Goods received (<see cref="CswNbtNodePropLogical"/>)
            /// </summary>
            public const string GoodsReceived = "Goods Received";

            /// <summary>
            /// Is Batch (<see cref="CswNbtNodePropLogical"/>)
            /// </summary>
            public const string IsBatch = "Is Batch";

            /// <summary>
            /// Level (<see cref="CswNbtNodePropRelationship"/>)
            /// </summary>
            public const string Level = "Level";

            /// <summary>
            /// Next date to reorder(<see cref="CswNbtNodePropDateTime"/> )
            /// </summary>
            public const string NextReorderDate = "Next Reorder Date";

            /// <summary>
            /// For "Request By Bulk" items, the Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
            /// </summary>
            public const string Quantity = "Quantity";

            /// <summary>
            /// Receipt Lot (<see cref="CswNbtNodePropRelationship"/>) to dispense from. 
            /// </summary>
            public const string ReceiptLotToDispense = "Receipt Lot to Dispense";

            /// <summary>
            /// Link grid of Receipt Lots received (<see cref="CswNbtNodePropGrid"/>) for this request. 
            /// </summary>
            public const string ReceiptLotsReceived = "Receipt Lots Received";

            /// <summary>
            /// Whether or no to reorder this item
            /// </summary>
            public const string Reorder = "Reorder";

            /// <summary>
            /// The frequency to reorder this item(<see cref="CswNbtNodePropTimeInterval"/>) to request. 
            /// </summary>
            public const string ReorderFrequency = "Reorder Frequency";

            /// <summary>
            /// For "Request By Size" items, a relationship(<see cref="CswNbtNodePropRelationship"/>) to the Size(<see cref="CswNbtObjClassSize"/>) to request. 
            /// </summary>
            public const string Size = "Size";

            /// <summary>
            /// For Dispense requests, the total amount(<see cref="CswNbtNodePropQuantity"/>) dispensed.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalDispensed = "Total Dispensed";

            public static CswCommaDelimitedString MLMCmgTabProps = new CswCommaDelimitedString
            {
                Level, IsBatch, Batch
            };

            public static CswCommaDelimitedString MLMReceiveTabProps = new CswCommaDelimitedString
            {
                ReceiptLotsReceived, GoodsReceived, ReceiptLotToDispense
            };
        }

        /// <summary>
        /// Types: Bulk or Size
        /// </summary>
        public new sealed class Types : CswNbtPropertySetRequestItem.Types
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        /// <summary>
        /// Statuses
        /// </summary>
        public new sealed class Statuses : CswNbtPropertySetRequestItem.Statuses
        {
            public const string Ordered = "Ordered";
            public const string Received = "Received";
            public const string Dispensed = "Dispensed";
            public const string Moved = "Moved";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Ordered, Received, Moved, Dispensed, Completed, Cancelled
                };
        }

        /// <summary>
        /// Fulfill menu options
        /// </summary>
        public new sealed class FulfillMenu : CswNbtPropertySetRequestItem.FulfillMenu
        {
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense from Container";
            public const string Move = "Move Containers";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Complete, Cancel
                };
        }

        #endregion Enums

        #region Base

        public static implicit operator CswNbtObjClassRequestMaterialDispense( CswNbtNode Node )
        {
            CswNbtObjClassRequestMaterialDispense ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestMaterialDispenseClass ) )
            {
                ret = (CswNbtObjClassRequestMaterialDispense) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassRequestMaterialDispense fromPropertySet( CswNbtPropertySetRequestItem PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetRequestItem toPropertySet( CswNbtObjClassRequestMaterialDispense ObjClass )
        {
            return ObjClass;
        }

        public CswNbtObjClassRequestMaterialDispense( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass ); }
        }

        #endregion Base

        #region Inherited Events

        /// <summary>
        /// 
        /// </summary>
        public override void toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance )
        {
            if( null != ItemInstance )
            {
                CswNbtObjClassRequestMaterialDispense ThisRequest = (CswNbtObjClassRequestMaterialDispense) ItemInstance;
                ThisRequest.Type.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Quantity.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Size.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Count.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Material.setReadOnly( value: IsReadOnly, SaveToDb: true );
            }
        }

        public override string setRequestDescription()
        {
            string Ret = "Dispense ";
            switch( Type.Value )
            {
                case Types.Bulk:
                    Ret += Quantity.Gestalt;
                    break;
                case Types.Size:
                    Ret += Count.Gestalt + " " + Size.Gestalt;
                    break;
            }
            Ret += " of " + Material.Gestalt;
            return Ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            //case 2753 - naming logic
            if( false == IsTemp )
            {
                if( Type.Value.Equals( Types.Size ) && CswTools.IsPrimaryKey( Size.RelatedNodeId ) ) //request material by size
                {
                    CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.GetNode( Size.RelatedNodeId );
                    if( null != sizeNode )
                    {
                        Name.Text = "Request " + Count.Value + " x " + sizeNode.Node.NodeName;
                    }
                }
                else //request material by bulk
                {
                    Name.Text = "Request " + Quantity.Quantity + Quantity.CachedUnitName;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetWriteNode()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetPopulateProps()
        {
            Quantity.SetOnPropChange( onQuantityPropChange );
            TotalDispensed.SetOnPropChange( onTotalDispensedPropChange );
            Material.SetOnPropChange( onMaterialPropChange );
        }//afterPopulateProps()

        /// <summary>
        /// 
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
                            case FulfillMenu.Order:
                                ButtonData.Action = NbtButtonAction.editprop;
                                ButtonData.Data["nodeid"] = NodeId.ToString();
                                CswPropIdAttr OrdIdAttr = new CswPropIdAttr( Node, ExternalOrderNumber.NodeTypeProp );
                                ButtonData.Data["propidattr"] = OrdIdAttr.ToString();
                                ButtonData.Data["title"] = "Enter the External Order Number";
                                break;
                            case FulfillMenu.Receive:
                                CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                                if( null != NodeAsMaterial )
                                {
                                    if( null != NodeAsMaterial.Receive.NodeTypeProp )
                                    {
                                        NbtButtonData ReceiveData = new NbtButtonData( NodeAsMaterial.Receive.NodeTypeProp );
                                        NodeAsMaterial.onButtonClick( ReceiveData );
                                        ButtonData.clone( ReceiveData );
                                        Int32 DocumentNodeTypeId = CswNbtActReceiving.getMaterialDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
                                        if( Int32.MinValue != DocumentNodeTypeId )
                                        {
                                            ButtonData.Data["documenttypeid"] = DocumentNodeTypeId;
                                        }
                                    }
                                }
                                break;

                            case FulfillMenu.Dispense:
                                //TODO: Someone will need to provide this container data
                                NodeAsContainer = _CswNbtResources.Nodes[CswConvert.ToString( ButtonData.Data["containerid"] )];
                                if( null != NodeAsContainer && null != NodeAsContainer.Dispense.NodeTypeProp )
                                {
                                    NbtButtonData DispenseData = new NbtButtonData( NodeAsContainer.Dispense.NodeTypeProp );
                                    NodeAsContainer.onButtonClick( DispenseData );
                                    ButtonData.clone( DispenseData );
                                }
                                else
                                {
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
                                    string Title = "Fulfill Request for " + Quantity.Gestalt + " of " + Material.Gestalt;
                                    if( TotalDispensed.Quantity > 0 )
                                    {
                                        Title += " (" + TotalDispensed.Gestalt + ") dispensed.";
                                    }
                                    ButtonData.Data["title"] = Title;
                                    ButtonData.Action = NbtButtonAction.dispense;
                                }
                                break;

                            case FulfillMenu.Move:
                                ButtonData.Data["sizeid"] = Size.RelatedNodeId.ToString();
                                ButtonData.Action = NbtButtonAction.move;
                                break;
                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["inventorygroupid"] = InventoryGroup.RelatedNodeId.ToString();
                        ButtonData.Data["requestitem"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
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
                case FulfillMenu.Move:
                    setNextStatus( Statuses.Moved );
                    break;
                case FulfillMenu.Order:
                    setNextStatus( Statuses.Ordered );
                    break;
                case FulfillMenu.Receive:
                    setNextStatus( Statuses.Received );
                    break;
            }
        }

        public void setNextStatus( string StatusVal )
        {
            switch( Status.Value )
            {
                case Statuses.Submitted:
                    if( StatusVal == Statuses.Dispensed || StatusVal == Statuses.Received || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;
                case Statuses.Received:
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
                case Statuses.Moved:
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
        /// 
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop )
        {
            if( Status.Value == Statuses.Pending )
            {
                TotalDispensed.setHidden( value: true, SaveToDb: true );
                Type.setHidden( value: true, SaveToDb: true );
                Quantity.setReadOnly( value: false, SaveToDb: true );
                Size.setReadOnly( value: false, SaveToDb: true );
                Count.setReadOnly( value: false, SaveToDb: true );

                //MLM
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
                {
                    Reorder.setHidden( value: true, SaveToDb: true );
                    foreach( string PropName in PropertyName.MLMCmgTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value: true, SaveToDb: true );
                    }
                    foreach( string PropName in PropertyName.MLMReceiveTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value: true, SaveToDb: true );
                    }
                }
            }
            else
            {
                if( Type.Value != Types.Size )
                {
                    TotalDispensed.setHidden( value: false, SaveToDb: true );
                }
                Type.setHidden( value: false, SaveToDb: true );
                Quantity.setReadOnly( value: true, SaveToDb: true );
                Size.setReadOnly( value: true, SaveToDb: true );
                Count.setReadOnly( value: true, SaveToDb: true );
                //MLM
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
                {
                    Reorder.setHidden( value: false, SaveToDb: true );
                    foreach( string PropName in PropertyName.MLMCmgTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value: false, SaveToDb: true );
                    }
                    foreach( string PropName in PropertyName.MLMReceiveTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value: false, SaveToDb: true );
                    }
                }
            }
            switch( Status.Value )
            {
                case Statuses.Received:
                    Fulfill.State = FulfillMenu.Dispense;
                    break;
                case Statuses.Dispensed:
                    if( TotalDispensed.Quantity >= Quantity.Quantity )
                    {
                        Fulfill.State = FulfillMenu.Complete;
                    }
                    break;
                case Statuses.Moved:
                    Fulfill.State = FulfillMenu.Complete;
                    break;
                case Statuses.Ordered:
                    Fulfill.State = FulfillMenu.Receive;
                    break;
            }
        }

        public override void onTypePropChange( CswNbtNodeProp Prop )
        {
            switch( Type.Value )
            {
                case Types.Size:
                    TotalDispensed.setHidden( value: true, SaveToDb: true );
                    Fulfill.MenuOptions = FulfillMenu.Options.Remove( FulfillMenu.Dispense ).ToString();
                    Quantity.clearQuantity( ForceClear: true );
                    break;
                case Types.Bulk:
                    TotalDispensed.setHidden( value: false, SaveToDb: true );
                    Fulfill.MenuOptions = FulfillMenu.Options.Remove( FulfillMenu.Move ).ToString();
                    Size.clearRelationship();
                    Count.Value = Double.NaN;
                    break;
            }

            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            bool QuantityDisabled = Type.Value == Types.Size;

            Quantity.setHidden( value: QuantityDisabled, SaveToDb: true );
            Size.setHidden( value: false == QuantityDisabled, SaveToDb: true );
            Count.setHidden( value: false == QuantityDisabled, SaveToDb: true );

            Type.setReadOnly( value: true, SaveToDb: true );

            Fulfill.State = FulfillMenu.Order;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }
        private void onQuantityPropChange( CswNbtNodeProp Prop )
        {
            if( CswTools.IsPrimaryKey( Quantity.UnitId ) && TotalDispensed.UnitId != Quantity.UnitId )
            {
                TotalDispensed.UnitId = Quantity.UnitId;
            }
        }

        public CswNbtNodePropRelationship Size
        {
            get { return _CswNbtNode.Properties[PropertyName.Size]; }
        }

        public CswNbtNodePropNumber Count
        {
            get { return _CswNbtNode.Properties[PropertyName.Count]; }
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

        public CswNbtNodePropTimeInterval ReorderFrequency { get { return _CswNbtNode.Properties[PropertyName.ReorderFrequency]; } }
        public CswNbtNodePropDateTime NextReorderDate { get { return _CswNbtNode.Properties[PropertyName.NextReorderDate]; } }
        public CswNbtNodePropRelationship Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        public CswNbtNodePropLogical IsBatch { get { return _CswNbtNode.Properties[PropertyName.IsBatch]; } }
        public CswNbtNodePropLogical Batch { get { return _CswNbtNode.Properties[PropertyName.Batch]; } }
        public CswNbtNodePropLogical Reorder { get { return _CswNbtNode.Properties[PropertyName.Reorder]; } }
        public CswNbtNodePropGrid ReceiptLotsReceived { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotsReceived]; } }
        public CswNbtNodePropLogical GoodsReceived { get { return _CswNbtNode.Properties[PropertyName.GoodsReceived]; } }
        public CswNbtNodePropRelationship ReceiptLotToDispense { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotToDispense]; } }

        #endregion
    }//CswNbtObjClassRequestMaterialDispense

}//namespace ChemSW.Nbt.ObjClasses
