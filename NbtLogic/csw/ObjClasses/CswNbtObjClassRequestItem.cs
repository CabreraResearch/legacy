using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequestItem: CswNbtObjClass
    {
        #region Properties

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            #region Core Properties
            /// <summary>
            /// The status of the Request Item based on actions performed.
            /// <para>PropType: List</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Status = "Status";
            /// <summary>
            /// The Type of Request Item with respect to the most recent action required for fulfillment.
            /// <para>PropType: List</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Type = "Type";
            /// <summary>
            /// Relationship to the Request to which this Item belongs.
            /// <para>PropType: Relationship (Request)</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Request = "Request";
            /// <summary>
            /// Name of this Item's Request
            /// <para>PropType: PropertyReference (Request.Name)</para>
            /// </summary>
            public const string Name = "Name";
            /// <summary>
            /// Unique Identifier of this Item, Sequence
            /// <para>PropType: Sequence</para>
            /// <para>SetValOnAdd</para>
            /// </summary>
            public const string ItemNumber = "Item Number";
            /// <summary>
            /// A composite description of the item based on its Type and Material/Container.
            /// <para>PropType: Static</para>
            /// </summary>
            public const string Description = "Description";
            /// <summary>
            /// The User who initiated the Request.
            /// <para>PropType: PropertyReference (Request.Requestor)</para>
            /// </summary>
            public const string Requestor = "Requestor";
            /// <summary>
            /// The User for whom the Request is intended.
            /// <para>PropType: Relationship (User)</para>
            /// <para>SetValOnAdd</para>
            /// </summary>
            public const string RequestedFor = "Requested For";
            /// <summary>
            /// The date the item is needed.
            /// <para>PropType: DateTime</para>
            /// <para>SetValOnAdd</para>
            /// </summary>
            public const string NeededBy = "Needed By";
            /// <summary>
            /// External Order Number
            /// <para>PropType: Text</para>
            /// <para>SetValOnAdd</para>
            /// </summary>
            public const string ExternalOrderNumber = "External Order Number";
            /// <summary>
            /// User to whom the Request Item is assigned for fulfillment.
            /// <para>PropType: Relationship (User)</para>
            /// </summary>
            public const string AssignedTo = "Assigned To";
            /// <summary>
            /// Comments on this Request Item
            /// <para>PropType: Comments</para>
            /// </summary>
            public const string Comments = "Comments";
            /// <summary>
            /// Numeric priority of this Item (higher number means higher priority)
            /// <para>PropType: Number</para>
            /// <para>MinVal: 0</para>
            /// <para>Default Value: 0</para>
            /// </summary>
            public const string Priority = "Priority";
            /// <summary>
            /// Log of actions performed on this Item
            /// <para>PropType: Comments</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string FulfillmentHistory = "Fulfillment History";
            /// <summary>
            /// Menu button to fulfill request.
            /// <para>PropType: Button</para>
            /// </summary>
            public const string Fulfill = "Fulfill";
            /// <summary>
            /// Location to which the requested item should be delivered
            /// <para>PropType: Location</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string Location = "Location";
            /// <summary>
            /// The Inventory Group from which the Request Item will be Fulfilled. Selected Inventory Group must be Central.
            /// <para>PropType: Relationship (InventoryGroup)</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string InventoryGroup = "Inventory Group";
            #endregion Core Properties
            #region Target Properties
            /// <summary>
            /// The EnterprisePart from which the Request Item will be Fulfilled.
            /// <para>PropType: Relationship (EnterprisePart)</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string EnterprisePart = "Enterprise Part";
            /// <summary>
            /// The Material from which the Request Item will be Fulfilled.
            /// <para>PropType: Relationship (Material)</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Material = "Material";
            /// <summary>
            /// The Container from which the Request Item will be Fulfilled.
            /// <para>PropType: Relationship (Container)</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Container = "Container";
            #endregion Target Properties
            #region Amount Properties
            #region Bulk Properties
            /// <summary>
            /// For Bulk Request Items, the Quantity to request. 
            /// <para>PropType: Quantity (Weight, Volume, Each)</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string Quantity = "Quantity";
            /// <summary>
            /// For Bulk Request Items, the total amount dispensed.
            /// <para>PropType: Quantity (Weight, Volume, Each)</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalDispensed = "Total Dispensed";
            #endregion Bulk Properties
            #region Size Properties
            /// <summary>
            /// For Size Request Items, the Material Size of the Container from which the Request Item will be Fulfilled.
            /// <para>PropType: Relationship (Size)</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string Size = "Size";
            /// <summary>
            /// For Size Request Items, the number of Sizes to request. 
            /// <para>PropType: Number</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string SizeCount = "Size Count";
            /// <summary>
            /// For Size Request Items, the total number of Sizes moved.
            /// <para>PropType: Number</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalMoved = "Total Moved";
            #endregion Size Properties
            #endregion Amount Properties
            #region MaterialCreate-Specific Props
            /// <summary>
            /// The Type of the new material 
            /// <para>PropType: NodeTypeSelect (Material)</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string NewMaterialType = "New Material Type";
            /// <summary>
            /// The Tradeame of the new material 
            /// <para>PropType: Text</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string NewMaterialTradename = "New Material Tradename";
            /// <summary>
            /// The Supplier of the new material
            /// <para>PropType: Text</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string NewMaterialSupplier = "New Material Supplier";
            /// <summary>
            /// The Part No of the new material
            /// <para>PropType: Text</para>
            /// <para>SetValOnAdd</para>
            /// <para>Required</para>
            /// </summary>
            public const string NewMaterialPartNo = "New Material Part No";
            #endregion Material-Create-Specific Props
            #region Favorite/Recurring Props
            /// <summary>
            /// True if this Request Item is a member of a Favorites Request
            /// <para>PropType: PropertyReference</para>
            /// </summary>
            public const string IsFavorite = "Is Favorite";
            /// <summary>
            /// Whether or not this item is scheduled to be reordered
            /// <para>PropType: Logical</para>
            /// <para>Default Value: False</para>
            /// </summary>
            public const string IsRecurring = "Is Recurring";
            /// <summary>
            /// The frequency to reorder this item to request. 
            /// <para>PropType: TimeInterval</para>
            /// </summary>
            public const string RecurringFrequency = "Recurring Frequency";
            /// <summary>
            /// Next date to reorder
            /// <para>PropType: DateTime</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string NextReorderDate = "Next Reorder Date";
            #endregion Favorite/Recurring Props
            #region MLM-specific props
            /// <summary>
            /// Certification Level 
            /// <para>PropType: List</para>
            /// </summary>
            public const string CertificationLevel = "Certification Level";//See CswEnumNbtMaterialRequestApprovalLevel
            /// <summary>
            /// Is Batch
            /// <para>PropType: Logical</para>
            /// </summary>
            public const string IsBatch = "Is Batch";
            /// <summary>
            /// Batch Number
            /// <para>PropType: Text</para>
            /// </summary>
            public const string BatchNumber = "Batch Number";
            /// <summary>
            /// True if all goods have been received
            /// <para>PropType: Logical</para>
            /// </summary>
            public const string GoodsReceived = "Goods Received";
            /// <summary>
            /// Receipt Lot to dispense from. 
            /// <para>PropType: Relationship (ReceiptLot)</para>
            /// </summary>
            public const string ReceiptLotToDispense = "Receipt Lot to Dispense";
            /// <summary>
            /// Link grid of Receipt Lots received for this request. 
            /// <para>PropType: Grid</para>
            /// </summary>
            public const string ReceiptLotsReceived = "Receipt Lots Received";
            #endregion MLM-Specific Props
        }

        #endregion Properties

        #region Enums

        /// <summary>
        /// Request Item Statuses - actions performed within the context of Request Items have the ability to change its Status
        /// </summary>
        public class Statuses
        {
            public const string Pending = "Pending";
            public const string Submitted = "Submitted";
            public const string Created = "Created";
            public const string Ordered = "Ordered";
            public const string Received = "Received";
            public const string Dispensed = "Dispensed";
            public const string Moved = "Moved";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string NonRequestableStatus = "No Status";//this is used as a placeholder for recurring/favorite request items
            public static readonly CswCommaDelimitedString Options = 
                new CswCommaDelimitedString { Pending, Submitted, Created, Ordered, Received, Dispensed, Moved, Completed, Cancelled };
        }

        /// <summary>
        /// Fulfill Button Menu Options - each Type uses a distinct subset of these options
        /// </summary>
        public class FulfillMenu
        {
            public const string Create = "Create Material";
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense this Container";//Dispense from Container
            public const string Move = "Move this Container";//Move Containers
            public const string Dispose = "Dispose this Container";
            public const string Complete = "Complete Request";
            public const string Cancel = "Cancel Request";
            public static readonly CswCommaDelimitedString Options =
                new CswCommaDelimitedString { Create, Order, Receive, Dispense, Move, Dispose, Complete, Cancel };
        }

        /// <summary>
        /// The types of Request Items - some Request Items may change Type depending on which actions have been fulfilled
        /// </summary>
        public class Types
        {
            public const string Create = "Request Material Create";
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";
            public const string Dispense = "Request Container Dispense";
            public const string Dispose = "Request Container Dispose";
            public const string Move = "Request Container Move";
            public static readonly CswCommaDelimitedString Options =
                new CswCommaDelimitedString { Create, Bulk, Size, Dispense, Dispose, Move };
        }

        #endregion Enums

        #region ctor

        public static implicit operator CswNbtObjClassRequestItem( CswNbtNode Node )
        {
            CswNbtObjClassRequestItem ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RequestItemClass ) )
            {
                ret = (CswNbtObjClassRequestItem) Node.ObjClass;
            }
            return ret;
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRequestItem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass ); }
        }

        #endregion ctor

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //Only show legitimate requests the current user created
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp,
                                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                                Value: "me",
                                                                ShowInGrid: false );
            CswNbtMetaDataObjectClassProp IsFavoriteOcp = ObjectClass.getObjectClassProp( PropertyName.IsFavorite );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsFavoriteOcp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True,
                                                                ShowInGrid: false );
            CswNbtMetaDataObjectClassProp IsRecurringOcp = ObjectClass.getObjectClassProp( PropertyName.IsRecurring );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, IsRecurringOcp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True,
                                                                ShowInGrid: false );
            CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: Statuses.NonRequestableStatus,
                                                                ShowInGrid: false );
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Fulfill:
                        switch( ButtonData.SelectedText )//TODO - subclass and fill in logic
                        {
                            case FulfillMenu.Cancel:
                                Status.Value = Statuses.Cancelled;
                                Fulfill.State = FulfillMenu.Cancel;
                                Fulfill.MenuOptions = "";
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Complete:
                                Status.Value = Statuses.Completed;
                                Fulfill.State = FulfillMenu.Complete;
                                Fulfill.MenuOptions = "";
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Create:
                                ButtonData.Action = CswEnumNbtButtonAction.creatematerial;
                                break;
                            case FulfillMenu.Order:
                                ButtonData.Action = CswEnumNbtButtonAction.editprop;
                                break;
                            case FulfillMenu.Receive:
                                CswNbtPropertySetMaterial NodeAsMaterial = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                                if( null != NodeAsMaterial )
                                {
                                    if( null != NodeAsMaterial.Receive.NodeTypeProp )
                                    {
                                        NbtButtonData ReceiveData = new NbtButtonData( NodeAsMaterial.Receive.NodeTypeProp );
                                        NodeAsMaterial.triggerOnButtonClick( ReceiveData );
                                        ButtonData.clone( ReceiveData );
                                    }
                                }
                                break;
                            case FulfillMenu.Dispense:
                                ButtonData.Action = CswEnumNbtButtonAction.dispense;
                                break;
                            case FulfillMenu.Move:
                                ButtonData.Action = CswEnumNbtButtonAction.move;
                                break;
                        } //switch( ButtonData.SelectedText )
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

        #endregion Inherited Events

        #region Custom Logic



        #endregion Custom Logic

        #region ObjectClass-specific properties

        private CswNbtObjClassUser.Cache _UserCache;
        public CswNbtObjClassUser.Cache UserCache 
        { 
            get { return _UserCache ?? ( _UserCache = CswNbtObjClassUser.getCurrentUserCache( _CswNbtResources ) ); } 
        }

        //Core Properties (All Request Items use these)
        public CswNbtNodePropList Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropRelationship Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
        public CswNbtNodePropPropertyReference Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropSequence ItemNumber { get { return _CswNbtNode.Properties[PropertyName.ItemNumber]; } }
        public CswNbtNodePropStatic Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropRelationship Requestor { get { return _CswNbtNode.Properties[PropertyName.Requestor]; } }
        public CswNbtNodePropRelationship RequestedFor { get { return _CswNbtNode.Properties[PropertyName.RequestedFor]; } }
        public CswNbtNodePropDateTime NeededBy { get { return _CswNbtNode.Properties[PropertyName.NeededBy]; } }
        public CswNbtNodePropText ExternalOrderNumber { get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber]; } }
        public CswNbtNodePropRelationship AssignedTo { get { return _CswNbtNode.Properties[PropertyName.AssignedTo]; } }
        public CswNbtNodePropComments Comments { get { return _CswNbtNode.Properties[PropertyName.Comments]; } }
        public CswNbtNodePropNumber Priority { get { return _CswNbtNode.Properties[PropertyName.Priority]; } }
        public CswNbtNodePropComments FulfillmentHistory { get { return _CswNbtNode.Properties[PropertyName.FulfillmentHistory]; } }
        public CswNbtNodePropButton Fulfill { get { return _CswNbtNode.Properties[PropertyName.Fulfill]; } }
        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }
        //Target Item Type (Only one of these will be used for a given Request Item)
        public CswNbtNodePropRelationship EnterprisePart { get { return _CswNbtNode.Properties[PropertyName.EnterprisePart]; } }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropRelationship Container { get { return _CswNbtNode.Properties[PropertyName.Container]; } }
        //Bulk Request Properties (all Request Items except Move/Dispose use either Bulk or Size)
        public CswNbtNodePropQuantity Quantity { get { return _CswNbtNode.Properties[PropertyName.Quantity]; } }
        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
        //Size Request Properties (all Request Items except Move/Dispose use either Bulk or Size)
        public CswNbtNodePropRelationship Size { get { return _CswNbtNode.Properties[PropertyName.Size]; } }
        public CswNbtNodePropNumber SizeCount { get { return _CswNbtNode.Properties[PropertyName.SizeCount]; } }
        public CswNbtNodePropNumber TotalMoved { get { return _CswNbtNode.Properties[PropertyName.TotalMoved]; } }
        //Create Request Properties (only the Create Material Request Item uses these)
        public CswNbtNodePropNodeTypeSelect NewMaterialType { get { return _CswNbtNode.Properties[PropertyName.NewMaterialType]; } }
        public CswNbtNodePropText NewMaterialTradename { get { return _CswNbtNode.Properties[PropertyName.NewMaterialTradename]; } }
        public CswNbtNodePropText NewMaterialPartNo { get { return _CswNbtNode.Properties[PropertyName.NewMaterialPartNo]; } }
        public CswNbtNodePropText NewMaterialSupplier { get { return _CswNbtNode.Properties[PropertyName.NewMaterialSupplier]; } }
        //Placeholder properties (only Request Items whose Target is not container uses these)
        public CswNbtNodePropPropertyReference IsFavorite { get { return _CswNbtNode.Properties[PropertyName.IsFavorite]; } }
        public CswNbtNodePropLogical IsRecurring { get { return _CswNbtNode.Properties[PropertyName.IsRecurring]; } }
        public CswNbtNodePropTimeInterval RecurringFrequency { get { return _CswNbtNode.Properties[PropertyName.RecurringFrequency]; } }
        public CswNbtNodePropDateTime NextReorderDate { get { return _CswNbtNode.Properties[PropertyName.NextReorderDate]; } }
        //MLM properties
        public CswNbtNodePropList CertificationLevel { get { return _CswNbtNode.Properties[PropertyName.CertificationLevel]; } }
        public CswNbtNodePropLogical IsBatch { get { return _CswNbtNode.Properties[PropertyName.IsBatch]; } }
        public CswNbtNodePropText BatchNumber { get { return _CswNbtNode.Properties[PropertyName.BatchNumber]; } }
        public CswNbtNodePropLogical GoodsReceived { get { return _CswNbtNode.Properties[PropertyName.GoodsReceived]; } }
        public CswNbtNodePropRelationship ReceiptLotToDispense { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotToDispense]; } }
        public CswNbtNodePropGrid ReceiptLotsReceived { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotsReceived]; } }

        #endregion ObjectClass-specific properties
    }//CswNbtObjClassRequestItem

}//namespace ChemSW.Nbt.ObjClasses
