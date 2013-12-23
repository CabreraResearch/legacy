using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.Requesting;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequestItem : CswNbtObjClass
    {
        #region Properties

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            #region Core Properties
            /// <summary>
            /// The Type of Request Item submitted by the User.
            /// <para>PropType: List</para>
            /// <para>ServerManaged</para>
            /// </summary>
            public const string RequestType = "Request Type";
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
            /// <para>ServerManaged</para>
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
            /// <summary>
            /// Create the requested Material
            /// </summary>
            public const string Create = "Create Material";
            /// <summary>
            /// Make an external Order for the requested Material
            /// </summary>
            public const string Order = "Order";
            /// <summary>
            /// Receive the requested Material (or, for EP Requests, select an EP Material to Receive)
            /// </summary>
            public const string Receive = "Receive";
            /// <summary>
            /// Dispense the requested Container
            /// </summary>
            public const string DispenseContainer = "Dispense Container";
            /// <summary>
            /// Select a Container of the given Material (or EP) to dispense from
            /// </summary>
            public const string DispenseMaterial = "Dispense from Container";
            /// <summary>
            /// Move the requested Container to the requested Location
            /// </summary>
            public const string MoveContainer = "Move Container";
            /// <summary>
            /// Select containers of the requested Target to move to the requested Location
            /// </summary>
            public const string MoveContainers = "Move Containers";
            /// <summary>
            /// Dispose the requested Container
            /// </summary>
            public const string Dispose = "Dispose this Container";
            /// <summary>
            /// Complete the Request Item
            /// </summary>
            public const string Complete = "Complete Request";
            /// <summary>
            /// Cancel the Request Item
            /// </summary>
            public const string Cancel = "Cancel Request";
            public static readonly CswCommaDelimitedString Options =
                new CswCommaDelimitedString { Create, Order, Receive, DispenseContainer, DispenseMaterial, MoveContainer, MoveContainers, Dispose, Complete, Cancel };
        }

        /// <summary>
        /// The types of Request Items - some Request Items may change Type depending on which actions have been fulfilled
        /// </summary>
        public class Types
        {
            public const string EnterprisePart = "Request Enterprise Part";
            public const string MaterialCreate = "Request Material Create";
            public const string MaterialBulk = "Request Material By Bulk";
            public const string MaterialSize = "Request Material By Size";
            public const string ContainerDispense = "Request Container Dispense";
            public const string ContainerDispose = "Request Container Dispose";
            public const string ContainerMove = "Request Container Move";
            public static readonly CswCommaDelimitedString Options =
                new CswCommaDelimitedString { EnterprisePart, MaterialCreate, MaterialBulk, MaterialSize, ContainerDispense, ContainerMove, ContainerDispose };
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

        private CswNbtRequestItemType _TypeDef;//Do not access the private variable directly
        public CswNbtRequestItemType TypeDef
        {
            get
            {
                //Always make sure we're using the right RequestItem Type Definition (in case the Type changes)
                if( null == _TypeDef || _TypeDef.RequestItemType != Type.Value )
                {
                    //We use _TypeDef before the default value for Type is set, so set it here in that case
                    if( String.IsNullOrEmpty( Type.Value ) )
                    {
                        Type.Value = Types.MaterialCreate;
                    }
                    _TypeDef = CswNbtRequestItemTypeFactory.makeRequestItemType( _CswNbtResources, this );
                }
                return _TypeDef;
            }
        }

        #endregion ctor

        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            RequestType.Value = Type.Value;
            TotalDispensed.UnitId = Quantity.UnitId;
            if( false == CswTools.IsDouble( Quantity.Quantity ) )
            {
                Quantity.Quantity = 0;
                if( null != Size.RelatedNodeId )
                {
                    CswNbtObjClassSize SizeNode = _CswNbtResources.Nodes[Size.RelatedNodeId];
                    TotalDispensed.UnitId = SizeNode.InitialQuantity.UnitId;
                }
            }
            _CswNbtObjClassDefault.beforePromoteNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            if( Creating )
            {
                _setUIVisibility();//This sets the Request Item's add layout based on its Type
                TypeDef.setQuantityOptions();
                if( null != Container.RelatedNodeId )//Set Inventory Group to Container's Inventory Group (if applicable)
                {
                    CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes[Container.RelatedNodeId];
                    InventoryGroup.RelatedNodeId = ContainerNode.getPermissionGroupId();
                }
            }
            _setDefaultValues();
            TypeDef.setDescription();
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _updateCartCounts( -1 );
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );
        }

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            _setUIVisibility();
            Request.SetOnPropChange( _onRequestPropChange );
            EnterprisePart.SetOnPropChange( _onEnterprisePartPropChange );
            Material.SetOnPropChange( _onMaterialPropChange );
            Status.SetOnPropChange( _onStatusPropChange );
            Type.SetOnPropChange( _onTypePropChange );
            ExternalOrderNumber.SetOnPropChange( _onExternalOrderNumberPropChange );
            RecurringFrequency.SetOnPropChange( _onRecurringFrequencyPropChange );
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
                                                                Value: CswNbtNodePropLogical.toLogicalGestalt( CswEnumTristate.True ),
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
            CswNbtObjClassContainer ContainerNode;
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Fulfill:
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
                        switch( ButtonData.SelectedText )//TODO - subclass and fill in logic
                        {
                            case FulfillMenu.Cancel:
                                Status.Value = Statuses.Cancelled;
                                Fulfill.State = FulfillMenu.Cancel;
                                Node.setReadOnly( true, true );
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Complete:
                                Status.Value = Statuses.Completed;
                                Fulfill.State = FulfillMenu.Complete;
                                Node.setReadOnly( true, true );
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Create:
                                ButtonData.Action = CswEnumNbtButtonAction.creatematerial;
                                // Create the temporary material node
                                Int32 SelectedNodeTypeId = NewMaterialType.SelectedNodeTypeIds.ToIntCollection().FirstOrDefault();
                                CswNbtPropertySetMaterial NewMaterial = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( SelectedNodeTypeId, IsTemp: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                                {
                                    ( (CswNbtPropertySetMaterial) NewNode ).TradeName.Text = NewMaterialTradename.Text;
                                    ( (CswNbtPropertySetMaterial) NewNode ).PartNumber.Text = NewMaterialPartNo.Text;
                                } );
                                Material.RelatedNodeId = NewMaterial.NodeId;
                                ButtonData.Data["state"] = new JObject();
                                ButtonData.Data["state"]["materialType"] = new JObject();
                                ButtonData.Data["state"]["materialType"]["name"] = NewMaterial.NodeType.NodeTypeName;
                                ButtonData.Data["state"]["materialType"]["val"] = NewMaterial.NodeType.NodeTypeId;
                                ButtonData.Data["state"]["materialId"] = NewMaterial.NodeId.ToString();
                                ButtonData.Data["state"]["tradeName"] = NewMaterial.TradeName.Text;
                                ButtonData.Data["state"]["addNewC3Supplier"] = true;
                                ButtonData.Data["state"]["supplier"] = new JObject();
                                ButtonData.Data["state"]["supplier"]["name"] = NewMaterialSupplier.Text;
                                ButtonData.Data["state"]["partNo"] = NewMaterialPartNo.Text;
                                ButtonData.Data["state"]["request"] = new JObject();
                                ButtonData.Data["state"]["request"]["requestitemid"] = NodeId.ToString();
                                ButtonData.Data["state"]["request"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                                ButtonData.Data["success"] = true;
                                break;
                            case FulfillMenu.Order://TODO - Case 31271 - Implement Ordering for Realz
                                ButtonData.Action = CswEnumNbtButtonAction.editprop;
                                ButtonData.Data["nodeid"] = NodeId.ToString();
                                CswPropIdAttr OrdIdAttr = new CswPropIdAttr( Node, ExternalOrderNumber.NodeTypeProp );
                                ButtonData.Data["propidattr"] = OrdIdAttr.ToString();
                                ButtonData.Data["title"] = "Enter the External Order Number";
                                break;
                            case FulfillMenu.Receive:
                                CswNbtPropertySetMaterial MaterialNode = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
                                if( null != MaterialNode )
                                {
                                    NbtButtonData ReceiveData = new NbtButtonData( MaterialNode.Receive.NodeTypeProp );
                                    MaterialNode.triggerOnButtonClick( ReceiveData );
                                    ButtonData.clone( ReceiveData );
                                }
                                else
                                {
                                    throw new CswDniException( CswEnumErrorType.Warning, "A Material must be selected in order to Receive.", "User attempted to Receive without a material defined." );
                                }
                                //TODO - if it's a Material Size Request, we should select the requested Size and Unit Count by default.
                                break;
                            case FulfillMenu.DispenseMaterial:
                                if( false == Quantity.Empty )
                                {
                                    JObject InitialQuantity = new JObject();
                                    Quantity.ToJSON( InitialQuantity );
                                    ButtonData.Data["initialQuantity"] = InitialQuantity;
                                }
                                string Title = "Fulfill Request for " + Quantity.Gestalt + " of " + Material.Gestalt;
                                if( TotalDispensed.Quantity > 0 )//Letting the user know how much has already been dispensed...
                                {
                                    Title += " (" + TotalDispensed.Gestalt + ") dispensed.";
                                }
                                ButtonData.Data["title"] = Title;
                                ButtonData.Action = CswEnumNbtButtonAction.dispense;
                                break;
                            case FulfillMenu.DispenseContainer:
                                ContainerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != ContainerNode )
                                {
                                    NbtButtonData DispenseData = new NbtButtonData( ContainerNode.Dispense.NodeTypeProp );
                                    ContainerNode.triggerOnButtonClick( DispenseData );
                                    ButtonData.clone( DispenseData );
                                }
                                break;
                            case FulfillMenu.MoveContainers:
                                ButtonData.Action = CswEnumNbtButtonAction.move;
                                //TODO - see if we need these propertes (or others) depending on the Request Type
                                //ButtonData.Data["title"] = "Fulfill " + Description.StaticText;//Defaults to 'Move Containers'
                                //ButtonData.Data["sizeid"] = Size.RelatedNodeId.ToString();
                                ButtonData.Data["location"] = Location.Gestalt;
                                break;
                            case FulfillMenu.MoveContainer:
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                ContainerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != ContainerNode )
                                {
                                    ContainerNode.Location.SelectedNodeId = Location.SelectedNodeId;
                                    ContainerNode.Location.CachedNodeName = Location.CachedNodeName;
                                    ContainerNode.Location.CachedPath = Location.CachedPath;
                                    ContainerNode.postChanges( false );
                                    //Note: The Container will mark all related Move request items (including this one) as Completed
                                }
                                break;
                            case FulfillMenu.Dispose:
                                ButtonData.Action = CswEnumNbtButtonAction.refresh;
                                ContainerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != ContainerNode )
                                {
                                    ContainerNode.DisposeContainer();
                                    ContainerNode.postChanges( true );
                                    FulfillmentHistory.AddComment( "Disposed Container " + CswNbtNode.getNodeLink( ContainerNode.NodeId, ContainerNode.Barcode.Barcode ) );
                                    Status.Value = Statuses.Completed;
                                }
                                break;
                        } //switch( ButtonData.SelectedText )
                        //TODO - we use 3 different RequestItem objects for 3 different wizards - can we clean this up?
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestMode"] = Type.Value.ToLower();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["inventorygroupid"] = ( InventoryGroup.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["containerid"] = ( Container.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["locationid"] = ( Location.SelectedNodeId ?? new CswPrimaryKey() ).ToString();
                        if( ButtonData.Data["state"] != null )
                        {
                            ButtonData.Data["state"]["requestitem"] = ButtonData.Data["requestitem"];
                        }
                        postChanges( ForceUpdate: false );
                        break; //case PropertyName.Fulfill:
                }
            }
            return true;
        }

        #endregion Inherited Events

        #region UI Logic

        private void _setUIVisibility()
        {
            //This loop is a bit expensive - perhaps revisit later and come up with a better way to invoke this hide logic
            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                Prop.SetOnBeforeRender( TypeDef.setUIVisibility );
            }
            RecurringFrequency.SetOnBeforeRender( _hideRecurringProps );
            NextReorderDate.SetOnBeforeRender( _hideRecurringProps );
            //This can't be done in OnBeforeRender because Search ignores it
            if( Status.Value == Statuses.Pending ||
                Status.Value == Statuses.Completed ||
                Status.Value == Statuses.Cancelled )
            {
                Fulfill.setHidden( true, SaveToDb: false );
                Fulfill.MenuOptions = "";
            }
            else
            {
                TypeDef.setFulfillOptions();
            }
        }

        private void _hideRecurringProps( CswNbtNodeProp Prop )
        {
            Prop.setHidden( IsRecurring.Checked != CswEnumTristate.True, SaveToDb: false );
        }

        #endregion UI Logic

        #region Custom Logic

        /// <summary>
        /// Copy the Request Item
        /// </summary>
        public CswNbtObjClassRequestItem copyNode( bool PostChanges = true, bool ClearRequest = true )
        {
            CswNbtObjClassRequestItem RetCopy = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, delegate( CswNbtNode NewNode )
            {
                NewNode.copyPropertyValues( Node );
                ( (CswNbtObjClassRequestItem) NewNode ).Status.Value = Statuses.Pending;
                if( ClearRequest )
                {
                    ( (CswNbtObjClassRequestItem) NewNode ).Request.RelatedNodeId = null;
                }
            } );

            return RetCopy;
        }

        private void _setDefaultValues()
        {
            //Set Request to current active Cart Request
            if( false == CswTools.IsPrimaryKey( Request.RelatedNodeId ) )
            {
                CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );
                CswNbtObjClassRequest CurrentRequest = RequestAct.getCurrentRequestNode();
                if( null != CurrentRequest )
                {
                    // In sched rule(s), no Current Cart will exist
                    Request.RelatedNodeId = CurrentRequest.NodeId;
                }
                Request.setReadOnly( value: true, SaveToDb: true );
                Request.setHidden( value: true, SaveToDb: false );
            }
            //Set Requestor and RequestedFor to current user
            if( false == CswTools.IsPrimaryKey( Requestor.RelatedNodeId ) )
            {
                CswNbtObjClassRequest ThisRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
                if( null != ThisRequest )
                {
                    Requestor.RelatedNodeId = ThisRequest.Requestor.RelatedNodeId;
                    RequestedFor.RelatedNodeId = ThisRequest.Requestor.RelatedNodeId;
                }
            }
        }

        //TODO - This updates the Cart Count when creating the temp - we should really change this so it only increments after PromoteTempToReal
        private void _updateCartCounts( Int32 Incrementer = 1 )
        {
            switch( Status.Value )
            {
                case Statuses.Pending:
                    UserCache.CartCounts.PendingRequestItems += Incrementer;
                    UserCache.update( _CswNbtResources );
                    break;
                case Statuses.Submitted:
                    UserCache.CartCounts.SubmittedRequestItems += Incrementer;
                    UserCache.update( _CswNbtResources );
                    break;
            }

            //If the Item is moving from Pending to something else
            string LastStatus = Status.GetOriginalPropRowValue();
            if( Status.Value != Statuses.Pending &&
                LastStatus == Statuses.Pending )
            {
                UserCache.CartCounts.PendingRequestItems -= 1;
                UserCache.update( _CswNbtResources );
            }
        }

        #endregion Custom Logic

        #region ObjectClass-specific properties

        private CswNbtObjClassUser.Cache _UserCache;
        public CswNbtObjClassUser.Cache UserCache
        {
            get { return _UserCache ?? ( _UserCache = CswNbtObjClassUser.getCurrentUserCache( _CswNbtResources ) ); }
        }

        //Core Properties (All Request Items use these)
        public CswNbtNodePropList RequestType { get { return _CswNbtNode.Properties[PropertyName.RequestType]; } }
        public CswNbtNodePropList Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        private void _onStatusPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( Status.Value != Status.GetOriginalPropRowValue() )
            {
                switch( Status.Value )
                {
                    case Statuses.Submitted:
                        FulfillmentHistory.AddComment( "Request Item Submitted." );
                        break;
                    case Statuses.Completed:
                        FulfillmentHistory.AddComment( "Request Item Completed." );
                        if( null != Request.RelatedNodeId )
                        {
                            CswNbtObjClassRequest ParentRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
                            ParentRequest.setCompletedDate();
                        }
                        break;
                    case Statuses.Cancelled:
                        FulfillmentHistory.AddComment( "Request Item Cancelled." );
                        break;
                }
            }
            _updateCartCounts();
        }
        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        private void _onTypePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            TypeDef.setFulfillOptions();
        }
        public CswNbtNodePropRelationship Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
        private void _onRequestPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            IsFavorite.RecalculateReferenceValue();
            Name.RecalculateReferenceValue();
        }
        public CswNbtNodePropPropertyReference Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropSequence ItemNumber { get { return _CswNbtNode.Properties[PropertyName.ItemNumber]; } }
        public CswNbtNodePropStatic Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropRelationship Requestor { get { return _CswNbtNode.Properties[PropertyName.Requestor]; } }
        public CswNbtNodePropRelationship RequestedFor { get { return _CswNbtNode.Properties[PropertyName.RequestedFor]; } }
        public CswNbtNodePropDateTime NeededBy { get { return _CswNbtNode.Properties[PropertyName.NeededBy]; } }
        public CswNbtNodePropText ExternalOrderNumber { get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber]; } }
        private void _onExternalOrderNumberPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( false == String.IsNullOrEmpty( ExternalOrderNumber.Text ) && Status.Value != Statuses.Pending )
            {
                String ActionChange = String.IsNullOrEmpty( ExternalOrderNumber.GetOriginalPropRowValue() ) ? "Added" : "Modified";
                FulfillmentHistory.AddComment( ActionChange + " External Order Number: " + ExternalOrderNumber.Text );
            }
        }
        public CswNbtNodePropRelationship AssignedTo { get { return _CswNbtNode.Properties[PropertyName.AssignedTo]; } }
        public CswNbtNodePropComments Comments { get { return _CswNbtNode.Properties[PropertyName.Comments]; } }
        public CswNbtNodePropNumber Priority { get { return _CswNbtNode.Properties[PropertyName.Priority]; } }
        public CswNbtNodePropComments FulfillmentHistory { get { return _CswNbtNode.Properties[PropertyName.FulfillmentHistory]; } }
        public CswNbtNodePropButton Fulfill { get { return _CswNbtNode.Properties[PropertyName.Fulfill]; } }
        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }
        //Target Item Type (will return EnterprisePart, Material, or Container depending on the RequestItem type)
        public CswNbtNodePropRelationship Target { get { return _TypeDef.Target; } }
        public CswNbtNodePropRelationship EnterprisePart { get { return _CswNbtNode.Properties[PropertyName.EnterprisePart]; } }
        private void _onEnterprisePartPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            //TODO - Case 31176 - is it possible to scope Material picklist to materials belonging to EP?
        }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        private void _onMaterialPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( Type.Value == Types.MaterialCreate && String.IsNullOrEmpty( Material.GetOriginalPropRowValue() ) )
            {
                Type.Value = Types.MaterialBulk;
                FulfillmentHistory.AddComment( "Selected existing Material: " + CswNbtNode.getNodeLink( Material.RelatedNodeId, Material.CachedNodeName ) );
            }
            else if( Type.Value == Types.EnterprisePart )//TODO - remove this block once Case 31242 is complete
            {
                bool materialMatchesEP = false;
                CswNbtMetaDataObjectClass ManufacturerEquivalentPartOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass );
                CswNbtMetaDataObjectClassProp EPOCP = ManufacturerEquivalentPartOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart );
                CswNbtMetaDataObjectClassProp MaterialOCP = ManufacturerEquivalentPartOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material );

                CswNbtView EPMatsView = new CswNbtView( _CswNbtResources );
                EPMatsView.ViewName = "Materials under " + EnterprisePart.RelatedNodeId;
                CswNbtViewRelationship MEPVR = EPMatsView.AddViewRelationship( ManufacturerEquivalentPartOC, false );
                EPMatsView.AddViewPropertyAndFilter( MEPVR, EPOCP, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: EnterprisePart.RelatedNodeId.PrimaryKey.ToString() );
                CswNbtViewRelationship MatVR = EPMatsView.AddViewRelationship( MEPVR, CswEnumNbtViewPropOwnerType.First, MaterialOCP, false );

                ICswNbtTree EPMatsTree = _CswNbtResources.Trees.getTreeFromView( EPMatsView, false, true, true );
                for( int i = 0; i < EPMatsTree.getChildNodeCount(); i++ )
                {
                    EPMatsTree.goToNthChild( i ); //EP's MEPs
                    if( EPMatsTree.getChildNodeCount() > 0 )
                    {
                        EPMatsTree.goToNthChild( 0 ); //MEP's Material
                        if( EPMatsTree.getNodeIdForCurrentPosition() == Material.RelatedNodeId )
                        {
                            materialMatchesEP = true;
                            break;
                        }
                    }
                    EPMatsTree.goToParentNode();
                }
                if( false == materialMatchesEP )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Selected Material does not belong to requested Enterprise Part.", "User selected an invalid Material." );
                }
            }
        }
        public CswNbtNodePropRelationship Container { get { return _CswNbtNode.Properties[PropertyName.Container]; } }
        //Bulk Request Properties (all Request Items except Move/Dispose/Size use Bulk)
        public CswNbtNodePropQuantity Quantity { get { return _CswNbtNode.Properties[PropertyName.Quantity]; } }
        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
        //Size Request Properties (used by Request By Size Request Items)
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
        private void _onRecurringFrequencyPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            NextReorderDate.DateTimeValue = CswNbtPropertySetSchedulerImpl.getNextDueDate( Node, NextReorderDate, RecurringFrequency );
        }
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