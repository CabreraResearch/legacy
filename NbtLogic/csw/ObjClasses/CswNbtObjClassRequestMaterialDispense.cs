using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Material Dispense Request Item
    /// </summary>
    public class CswNbtObjClassRequestMaterialDispense: CswNbtPropertySetRequestItem
    {
        #region Enums

        /// <summary>
        /// Property Names
        /// </summary>
        public new sealed class PropertyName: CswNbtPropertySetRequestItem.PropertyName
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
            /// True (<see cref="CswNbtNodePropLogical"/>) if this Request Item is a member of a Favorites Request
            /// </summary>
            public const string IsFavorite = "Is Favorite";

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
            public const string IsRecurring = "Is Recurring";

            /// <summary>
            /// The frequency to reorder this item(<see cref="CswNbtNodePropTimeInterval"/>) to request. 
            /// </summary>
            public const string RecurringFrequency = "Recurring Frequency";

            /// <summary>
            /// For "Request By Size" items, a relationship(<see cref="CswNbtNodePropRelationship"/>) to the Size(<see cref="CswNbtObjClassSize"/>) to request. 
            /// </summary>
            public const string Size = "Size";

            /// <summary>
            /// For By Bulk requests, the total amount(<see cref="CswNbtNodePropQuantity"/>) dispensed.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalDispensed = "Total Dispensed";

            /// <summary>
            /// For By Size requests, the total number(<see cref="CswNbtNodePropNumber"/>) moved.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalMoved = "Total Moved";

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
        public new sealed class Types: CswNbtPropertySetRequestItem.Types
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        /// <summary>
        /// Statuses
        /// </summary>
        public new sealed class Statuses: CswNbtPropertySetRequestItem.Statuses
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
        public new sealed class FulfillMenu: CswNbtPropertySetRequestItem.FulfillMenu
        {
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense from Container";
            public const string Move = "Move Containers";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Order, Receive, Move, Dispense, Complete, Cancel
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
                ThisRequest.Type.setReadOnly( value : IsReadOnly, SaveToDb : true );
                ThisRequest.Quantity.setReadOnly( value : IsReadOnly, SaveToDb : true );
                ThisRequest.Size.setReadOnly( value : IsReadOnly, SaveToDb : true );
                ThisRequest.Count.setReadOnly( value : IsReadOnly, SaveToDb : true );
                ThisRequest.Material.setReadOnly( value : IsReadOnly, SaveToDb : true );
            }
        }

        public override string setRequestDescription()
        {
            string Ret = "";
            if( _IsRecurring && false == RecurringFrequency.Empty )
            {
                Ret = "Recurring " + RecurringFrequency.RateInterval.RateType + ": ";
            }
            Ret += "Dispense ";
            switch( Type.Value )
            {
                case Types.Bulk:
                    Ret += Quantity.Gestalt;
                    break;
                case Types.Size:
                    Ret += Count.Gestalt + " x " + Size.Gestalt;
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
            IsFavorite.setHidden( value : true, SaveToDb : true );
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
            TotalMoved.SetOnPropChange( onTotalMovedPropChange );
            IsFavorite.SetOnPropChange( onIsFavoritePropChange );
            IsRecurring.SetOnPropChange( onIsRecurringChange );
            RecurringFrequency.SetOnPropChange( onRecurringFrequencyPropChange );
        }//afterPopulateProps()

        /// <summary>
        /// 
        /// </summary>
        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Fulfill:
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
                                        Int32 DocumentNodeTypeId = CswNbtActReceiving.getSDSDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
                                        if( Int32.MinValue != DocumentNodeTypeId )
                                        {
                                            ButtonData.Data["documenttypeid"] = DocumentNodeTypeId;
                                        }
                                    }
                                }
                                break;

                            case FulfillMenu.Dispense:
                                JObject InitialQuantity = null;
                                if( false == Quantity.Empty )
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
                                break;

                            case FulfillMenu.Move:
                                ButtonData.Data["title"] = "Fulfill Request for " + Count.Value + " x " + Size.Gestalt + " of " + Material.Gestalt;
                                ButtonData.Data["sizeid"] = Size.RelatedNodeId.ToString();
                                ButtonData.Data["location"] = Location.Gestalt;
                                ButtonData.Action = NbtButtonAction.move;
                                break;
                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestMode"] = Type.Value.ToLower();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["inventorygroupid"] = ( InventoryGroup.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
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
                TotalDispensed.setHidden( value : true, SaveToDb : true );
                TotalMoved.setHidden( value : true, SaveToDb : true );
                Type.setHidden( value : true, SaveToDb : true );
                Quantity.setReadOnly( value : false, SaveToDb : true );
                Size.setReadOnly( value : false, SaveToDb : true );
                Count.setReadOnly( value : false, SaveToDb : true );

                //MLM
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
                {
                    foreach( string PropName in PropertyName.MLMCmgTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value : true, SaveToDb : true );
                    }
                    foreach( string PropName in PropertyName.MLMReceiveTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value : true, SaveToDb : true );
                    }
                }
            }
            else
            {
                if( Type.Value == Types.Size )
                {
                    TotalDispensed.setHidden( value : true, SaveToDb : true );
                    TotalMoved.setHidden( value : false, SaveToDb : true );
                }
                else
                {
                    TotalDispensed.setHidden( value : false, SaveToDb : true );
                    TotalMoved.setHidden( value : true, SaveToDb : true );
                }
                Type.setHidden( value : false, SaveToDb : true );
                Quantity.setReadOnly( value : true, SaveToDb : true );
                Size.setReadOnly( value : true, SaveToDb : true );
                Count.setReadOnly( value : true, SaveToDb : true );
                //MLM
                if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
                {
                    IsRecurring.setHidden( value : false, SaveToDb : true );
                    foreach( string PropName in PropertyName.MLMCmgTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value : false, SaveToDb : true );
                    }
                    foreach( string PropName in PropertyName.MLMReceiveTabProps )
                    {
                        _CswNbtNode.Properties[PropName].setHidden( value : false, SaveToDb : true );
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
                    if( TotalMoved.Value >= Count.Value )
                    {
                        Fulfill.State = FulfillMenu.Complete;
                    }
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
                    Fulfill.MenuOptions = FulfillMenu.Options.Remove( FulfillMenu.Dispense ).ToString();
                    Fulfill.State = FulfillMenu.Move;
                    Quantity.clearQuantity( ForceClear : true );
                    break;
                case Types.Bulk:
                    Fulfill.MenuOptions = FulfillMenu.Options.Remove( FulfillMenu.Move ).ToString();
                    Fulfill.State = FulfillMenu.Dispense;
                    Size.clearRelationship();
                    Count.Value = Double.NaN;
                    break;
            }

            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            bool QuantityDisabled = Type.Value == Types.Size;

            Quantity.setHidden( value : QuantityDisabled, SaveToDb : true );
            Size.setHidden( value : false == QuantityDisabled, SaveToDb : true );
            Count.setHidden( value : false == QuantityDisabled, SaveToDb : true );

            Type.setReadOnly( value : true, SaveToDb : true );
        }

        public override void onRequestPropChange( CswNbtNodeProp Prop )
        {
            IsFavorite.RecalculateReferenceValue();
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp,
                FilterMode : CswNbtPropFilterSql.PropertyFilterMode.Equals,
                Value : "me",
                ShowInGrid : false );

            CswNbtMetaDataObjectClassProp IsFavoriteOcp = ObjectClass.getObjectClassProp( PropertyName.IsFavorite );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsFavoriteOcp,
                FilterMode : CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                Value : CswNbtNodePropLogical.toLogicalGestalt( Tristate.True ),
                ShowInGrid : false );

            CswNbtMetaDataObjectClassProp IsRecurringOcp = ObjectClass.getObjectClassProp( PropertyName.IsRecurring );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsRecurringOcp,
                FilterMode : CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                Value : Tristate.True.ToString(),
                ShowInGrid : false );
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
                Material.setReadOnly( value : true, SaveToDb : true );
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
            if( Type.Value == Types.Bulk &&
                Status.Value != Statuses.Pending &&
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

        public CswNbtNodePropNumber TotalMoved { get { return _CswNbtNode.Properties[PropertyName.TotalMoved]; } }
        private void onTotalMovedPropChange( CswNbtNodeProp Prop )
        {
            if( Type.Value == Types.Size &&
                TotalMoved.Value >= Count.Value )
            {
                Status.Value = Statuses.Moved;
                Fulfill.State = FulfillMenu.Complete;
            }
        }

        private void _hideFakeItemProps()
        {
            //Neither favs nor recurs represent real (aka Fulfillable) Items
            if( _IsFavorite || _IsRecurring ) 
            {
                Status.Value = "";
                Status.setHidden( value : true, SaveToDb : true );
                Fulfill.setHidden( value : true, SaveToDb : true );
                AssignedTo.setHidden( value : true, SaveToDb : true );
                Number.setHidden( value : true, SaveToDb : true );
                NeededBy.setHidden( value : true, SaveToDb : true );
                TotalMoved.setHidden( value : true, SaveToDb : true );
                TotalDispensed.setHidden( value : true, SaveToDb : true );
                ReceiptLotToDispense.setHidden( value : true, SaveToDb : true );
                ReceiptLotsReceived.setHidden( value : true, SaveToDb : true );
                GoodsReceived.setHidden( value : true, SaveToDb : true );
            }
        }

        public CswNbtNodePropGrid ReceiptLotsReceived { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotsReceived]; } }
        public CswNbtNodePropDateTime NextReorderDate { get { return _CswNbtNode.Properties[PropertyName.NextReorderDate]; } }
        public CswNbtNodePropLogical IsBatch { get { return _CswNbtNode.Properties[PropertyName.IsBatch]; } }
        public CswNbtNodePropLogical Batch { get { return _CswNbtNode.Properties[PropertyName.Batch]; } }
        public CswNbtNodePropLogical IsRecurring { get { return _CswNbtNode.Properties[PropertyName.IsRecurring]; } }
        private bool _IsRecurring { get { return Tristate.True == IsRecurring.Checked; } } //&& _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ); } }
        private void onIsRecurringChange( CswNbtNodeProp NodeProp )
        {
            IsRecurring.setHidden( value : true, SaveToDb : true );
            // No "else": like favorites, recurring items never transition out of this state--they can only be deleted.
            if( _IsRecurring )
            {
                _hideFakeItemProps();
                RecurringFrequency.setHidden( value: false, SaveToDb: true );
                NextReorderDate.setHidden( value: false, SaveToDb: true );
                Name.setHidden( value: true, SaveToDb: true );
            }
            else
            {
                RecurringFrequency.setHidden( value : true, SaveToDb : true );
                NextReorderDate.setHidden( value : true, SaveToDb : true );
            }
        }
        
        public CswNbtNodePropLogical GoodsReceived { get { return _CswNbtNode.Properties[PropertyName.GoodsReceived]; } }
        public CswNbtNodePropPropertyReference IsFavorite { get { return _CswNbtNode.Properties[PropertyName.IsFavorite]; } }
        private bool _IsFavorite { get { return CswConvert.ToBoolean( IsFavorite.Gestalt ); } }
        private void onIsFavoritePropChange( CswNbtNodeProp NodeProp )
        {
            // No "else": like recurring, favorite items never transition out of this state--they can only be deleted.
            if( _IsFavorite ) 
            {
                _hideFakeItemProps();

                //Name is normally shown on status change, which doesn't happen for "fake" request items
                Name.setHidden( value : false, SaveToDb : true );
                IsRecurring.setHidden( value : true, SaveToDb : true );
                NextReorderDate.setHidden( value : true, SaveToDb : true );
                RecurringFrequency.setHidden( value : true, SaveToDb : true );
            }
        }
        public CswNbtNodePropRelationship ReceiptLotToDispense { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotToDispense]; } }
        public CswNbtNodePropRelationship Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        public CswNbtNodePropTimeInterval RecurringFrequency { get { return _CswNbtNode.Properties[PropertyName.RecurringFrequency]; } }
        private void onRecurringFrequencyPropChange( CswNbtNodeProp NodeProp )
        {
            NextReorderDate.DateTimeValue = CswNbtPropertySetSchedulerImpl.getNextDueDate( this.Node, NextReorderDate, RecurringFrequency );
        }

        #endregion
    }//CswNbtObjClassRequestMaterialDispense

}//namespace ChemSW.Nbt.ObjClasses
