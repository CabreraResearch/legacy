using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequestItem : CswNbtObjClass
    {

        public sealed class PropertyName
        {
            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the User (<see cref="CswNbtObjClassUser"/>) to whom the Request Item is assigned.
            /// </summary>
            public const string AssignedTo = "Assigned To";

            /// <summary>
            /// Comments(<see cref="CswNbtNodePropComments"/>) on this Item
            /// </summary>
            public const string Comments = "Comments";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Container (<see cref="CswNbtObjClassContainer"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Container = "Container";

            /// <summary>
            /// For "Request By Size" items, the number(<see cref="CswNbtNodePropNumber"/>) of sizes(<see cref="Size"/>) to request. 
            /// </summary>
            public const string Count = "Count";

            /// <summary>
            /// External Order Number(<see cref="CswNbtNodePropText"/>)
            /// </summary>
            public const string ExternalOrderNumber = "External Order Number";

            /// <summary>
            /// Menu button(<see cref="CswNbtNodePropButton"/>) to fulfill request.
            /// </summary>
            public const string Fulfill = "Fulfill";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Inventory Group (<see cref="CswNbtObjClassInventoryGroup"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string InventoryGroup = "Inventory Group";

            /// <summary>
            /// Location(<see cref="CswNbtNodePropLocation"/> ) to which the request should be delivered
            /// </summary>
            public const string Location = "Location";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material (<see cref="CswNbtObjClassMaterial"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Material = "Material";

            /// <summary>
            /// Name(<see cref="CswNbtNodePropText"/>) of this Item
            /// </summary>
            public const string Name = "Name";

            /// <summary>
            /// The date(<see cref="CswNbtNodePropDateTime"/>) the item is needed.
            /// </summary>
            public const string NeededBy = "Needed By";

            /// <summary>
            /// Unique Identified of this Item, Sequence(<see cref="CswNbtNodePropSequence"/>)
            /// </summary>
            public const string Number = "Number";

            /// <summary>
            /// For "Request By Bulk" items, the Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
            /// </summary>
            public const string Quantity = "Quantity";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Request(<see cref="CswNbtObjClassRequest"/>) to which this Item belongs. 
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Request = "Request";

            /// <summary>
            /// For Material(<see cref="CswNbtObjClassMaterial"/>) requests, Bulk or Size List(<see cref="CswNbtNodePropList"/>) options.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string RequestBy = "Request By";

            /// <summary>
            /// The User (<see cref="CswNbtObjClassUser"/>) who initiated the Request(<see cref="CswNbtObjClassRequest"/>) as a Property Ref(<see cref="CswNbtNodePropPropertyReference"/>).
            /// </summary>
            public const string Requestor = "Requestor";

            /// <summary>
            /// A relationship(<see cref="CswNbtNodePropRelationship"/>) to the User (<see cref="CswNbtObjClassUser"/>) for whom the Request(<see cref="CswNbtObjClassRequest"/>) is intended.
            /// </summary>
            public const string RequestedFor = "RequestedFor";

            /// <summary>
            /// For "Request By Size" items, a relationship(<see cref="CswNbtNodePropRelationship"/>) to the Size(<see cref="CswNbtObjClassSize"/>) to request. 
            /// </summary>
            public const string Size = "Size";

            /// <summary>
            /// The status(<see cref="CswNbtNodePropList"/>) of the item.
            /// </summary>
            public const string Status = "Status";

            /// <summary>
            /// For Dispense requests, the total amount(<see cref="CswNbtNodePropQuantity"/>) dispensed.
            /// <para>ServerManaged</para>
            /// </summary>
            public const string TotalDispensed = "Total Dispensed";

            /// <summary>
            /// The request's type(<see cref="CswNbtNodePropList"/>)
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Type = "Type";
        }

        public sealed class Types
        {
            public const string CreateMaterial = "Create Material";
            public const string Dispense = "Dispense Container";
            public const string Dispose = "Dispose Container";
            public const string Move = "Move Container";
            public const string Request = "Request Material";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { CreateMaterial, Dispense, Dispose, Move, Request };
        }

        public sealed class RequestsBy
        {
            public const string Bulk = "Request By Bulk";
            public const string Size = "Request By Size";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
        }

        public sealed class Statuses
        {
            public const string Pending = "Pending";
            public const string Submitted = "Submitted";
            public const string Ordered = "Ordered";
            public const string Received = "Received";
            public const string Dispensed = "Dispensed";
            public const string Disposed = "Disposed";
            public const string Moved = "Moved";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Ordered, Received, Dispensed, Completed, Cancelled
                };
        }

        public sealed class FulfillMenu
        {
            public const string CreateMaterial = "Create Material";
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense this Container";
            public const string Move = "Move";
            public const string Dispose = "Dispose this Container";
            public const string Complete = "Complete Request";
            public const string Cancel = "Cancel Request";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Dispose, Move, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString CreateMaterialOptions = new CswCommaDelimitedString
                {
                    CreateMaterial, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString DispenseOptions = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString DisposeOptions = new CswCommaDelimitedString
                {
                    Dispose, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString MoveOptions = new CswCommaDelimitedString
                {
                    Move, Complete, Cancel
                };


        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public static implicit operator CswNbtObjClassRequestItem( CswNbtNode Node )
        {
            CswNbtObjClassRequestItem ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestItemClass ) )
            {
                ret = (CswNbtObjClassRequestItem) Node.ObjClass;
            }
            return ret;
        }

        public CswNbtObjClassRequestItem copyNode()
        {
            CswNbtNode CopyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopyNode.copyPropertyValues( Node );
            CswNbtObjClassRequestItem RetCopy = CopyNode;
            RetCopy.Status.Value = Statuses.Pending;
            RetCopy.Request.RelatedNodeId = null;
            _toggleReadOnlyProps( false, RetCopy );
            RetCopy.postChanges( true );
            return RetCopy;
        }

        public CswNbtObjClassRequestItem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestItemClass ); }
        }

        #region Inherited Events

        private string _makeNotificationMessage()
        {
            string MessageText = "The Status for this Request Item has changed to: " + Status.Value + ". \n";

            CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( null != NodeAsMaterial )
            {
                MessageText += "Material: " + NodeAsMaterial.TradeName.Text + "\n";
            }

            CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
            if( null != NodeAsContainer )
            {
                MessageText += "Container: " + NodeAsContainer.Node.NodeName + "\n";
            }

            if( Quantity.Quantity > 0 )
            {
                MessageText += "Quantity: " + Quantity.Quantity + ", Unit: " + Quantity.CachedUnitName + "\n";
            }
            if( false == string.IsNullOrEmpty( Size.CachedNodeName ) )
            {
                MessageText += "Size: " + Size.CachedNodeName + "\n";
            }
            if( Count.Value > 0 )
            {
                MessageText += "Count: " + Count.Value + "\n";
            }
            CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes.GetNode( Location.SelectedNodeId );
            if( null != NodeAsLocation )
            {
                MessageText += "Location: " + NodeAsLocation.Location.CachedFullPath;
            }


            return MessageText;
        }

        private void _toggleReadOnlyProps( bool IsReadOnly, CswNbtObjClassRequestItem ItemInstance )
        {
            ItemInstance.Request.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.RequestBy.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Type.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Quantity.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Size.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Container.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Count.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Material.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Location.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Number.setReadOnly( value: IsReadOnly, SaveToDb: true );
        }

        private void _setDefaultValues()
        {
            if( false == CswTools.IsPrimaryKey( Request.RelatedNodeId ) )
            {
                CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true );
                Request.RelatedNodeId = RequestAct.CurrentRequestNode().NodeId;
                Request.setReadOnly( value: true, SaveToDb: true );
                Request.setHidden( value: true, SaveToDb: false );
            }
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setDefaultValues();

            /* Container-specific logic */
            if( ( Type.Value == Types.Dispense ||
                Type.Value == Types.Move ||
                Type.Value == Types.Dispose ) &&
                null != Container.RelatedNodeId )
            {
                if( CswTools.IsPrimaryKey( InventoryGroup.RelatedNodeId ) )
                {
                    CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes[Container.RelatedNodeId];
                    if( null == NodeAsContainer )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                  "A " + Type.Value +
                                                  " type of Request Item requires a valid Container.",
                                                  "Attempted to edit node without a valid Container relationship." );
                    }
                    CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes[NodeAsContainer.Location.SelectedNodeId];
                    if( null != NodeAsLocation &&
                        InventoryGroup.RelatedNodeId != NodeAsLocation.InventoryGroup.RelatedNodeId )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                  "For a " + Type.Value +
                                                  " type of Request Item, the Inventory Group of the Request must match the Inventory Group of the Container's Location.",
                                                  "Attempted to edit node without matching Container and Request Inventory Group relationships." );
                    }
                }
            }

            //case 2753 - naming logic
            if( false == IsTemp )
            {
                switch( Type.Value )
                {
                    case Types.Request:
                        if( RequestBy.Value.Equals( RequestsBy.Size ) && CswTools.IsPrimaryKey( Size.RelatedNodeId ) ) //request material by size
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
                        break;
                    case Types.Dispense:
                        if( null != Container.RelatedNodeId )
                        {
                            CswNbtObjClassContainer containerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                            Name.Text = "Dispense " + containerNode.Quantity.Quantity + containerNode.Quantity.CachedUnitName + " from Container " + containerNode.Barcode.Barcode;
                        }
                        break;
                    case Types.Dispose:
                        if( null != Container.RelatedNodeId )
                        {
                            CswNbtObjClassContainer containerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                            Name.Text = "Dispose Container " + containerNode.Barcode.Barcode;
                        }
                        break;
                    case Types.Move:
                        if( null != Container.RelatedNodeId )
                        {
                            CswNbtObjClassContainer containerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                            Name.Text = "Move Container " + containerNode.Barcode.Barcode;
                        }
                        break;
                    default:
                        throw new CswDniException( ErrorType.Warning,
                            "An invalid request type was encountered",
                            "An invald request type of " + Type.Value + " was encountered." );
                }
            }

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

        private void _setFulfillVisibility()
        {
            bool HideMenuButton = ( Status.Value == Statuses.Pending );
            if( false == HideMenuButton &&
                CswTools.IsPrimaryKey( Request.RelatedNodeId ) &&
                Status.Value != Statuses.Cancelled &&
                Status.Value != Statuses.Completed )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
                if( null != NodeAsRequest &&
                    _CswNbtResources.CurrentNbtUser.UserId == NodeAsRequest.Requestor.RelatedNodeId )
                {
                    HideMenuButton = true;
                }
            }
            Fulfill.setHidden( value: HideMenuButton, SaveToDb: false );
        }

        public override void afterPopulateProps()
        {
            Quantity.SetOnPropChange( OnQuantityPropChange );
            TotalDispensed.SetOnPropChange( OnTotalDispensedPropChange );
            RequestBy.SetOnPropChange( OnRequestByPropChange );
            Type.SetOnPropChange( OnTypePropChange );
            Material.SetOnPropChange( OnMaterialPropChange );
            Container.SetOnPropChange( OnContainerPropChange );
            Status.SetOnPropChange( OnStatusPropChange );
            _setFulfillVisibility();
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status.ToString() );
            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp, Statuses.Pending.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                switch( OCP.PropName )
                {
                    case PropertyName.Fulfill:
                        CswNbtObjClassContainer NodeAsContainer = null;
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Cancel:
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Complete:
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
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
                            case FulfillMenu.Dispose:
                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != NodeAsContainer )
                                {
                                    NodeAsContainer.DisposeContainer();
                                    NodeAsContainer.postChanges( true );
                                }
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Move:
                                if( null != Container.RelatedNodeId )
                                {
                                    NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                    if( null != NodeAsContainer )
                                    {
                                        NodeAsContainer.Location.SelectedNodeId = Location.SelectedNodeId;
                                        NodeAsContainer.Location.CachedNodeName = Location.CachedNodeName;
                                        NodeAsContainer.Location.CachedPath = Location.CachedPath;
                                        NodeAsContainer.postChanges( false );
                                        Status.Value = Statuses.Completed;
                                        ButtonData.Action = NbtButtonAction.refresh;
                                        //ButtonData.Data["nodeid"] = Container.RelatedNodeId.ToString();
                                        //CswPropIdAttr LocIdAttr = new CswPropIdAttr( NodeAsContainer.Node, NodeAsContainer.Location.NodeTypeProp );
                                        //ButtonData.Data["propidattr"] = LocIdAttr.ToString();
                                        //Status.Value = Statuses.Moved;
                                        //ButtonData.Action = NbtButtonAction.editprop;
                                        //ButtonData.Data["title"] = "Set " + NodeAsContainer.Node.NodeName + " Container's Location";
                                    }
                                }
                                break;
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
                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );
                        postChanges( true );
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
                case FulfillMenu.Dispose:
                    setNextStatus( Statuses.Disposed );
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
                    if( StatusVal == Statuses.Dispensed || StatusVal == Statuses.Disposed || StatusVal == Statuses.Moved || StatusVal == Statuses.Received || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
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
                case Statuses.Moved:
                case Statuses.Disposed:
                    if( StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;
            }
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Request
        {
            get { return _CswNbtNode.Properties[PropertyName.Request]; }
        }

        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }
        private void OnTypePropChange( CswNbtNodeProp Prop )
        {
            /* Spec W1010: Location applies to all but Dispose */
            Location.setHidden( value: ( Types.Dispose == Type.Value ), SaveToDb: true );
            Location.setReadOnly( value: ( Status.Value != Statuses.Pending || Types.Dispose == Type.Value ), SaveToDb: true );

            /* Spec W1010: Container applies only to Dispense, Dispose and Move */
            RequestBy.setReadOnly( value: ( Status.Value != Statuses.Pending || ( Types.Request != Type.Value && Types.Dispense != Type.Value ) ), SaveToDb: true );
            RequestBy.setHidden( value: ( Types.Request != Type.Value && Types.Dispense != Type.Value ), SaveToDb: true );
            Container.setHidden( value: ( Types.Request == Type.Value ), SaveToDb: true );

            /* Spec W1010: Material applies only to Request and Dispense */
            Material.setHidden( value: ( Types.Request != Type.Value && Types.Dispense != Type.Value ), SaveToDb: true );
            //AssignedTo.setHidden( value: ( AssignedTo.Hidden && Types.Dispense != Type.Value ), SaveToDb: true );

            switch( Type.Value )
            {
                case Types.Request: //This fall through is intentional
                case Types.Dispense:
                    Fulfill.MenuOptions = FulfillMenu.DispenseOptions.ToString();
                    Fulfill.State = FulfillMenu.Dispense;
                    break;
                case Types.Dispose:
                    Fulfill.MenuOptions = FulfillMenu.DisposeOptions.ToString();
                    Fulfill.State = FulfillMenu.Dispose;
                    break;
                case Types.Move:
                    Fulfill.MenuOptions = FulfillMenu.MoveOptions.ToString();
                    Fulfill.State = FulfillMenu.Move;
                    break;
            }
        }

        public CswNbtNodePropList RequestBy
        {
            get { return _CswNbtNode.Properties[PropertyName.RequestBy]; }
        }
        private void OnRequestByPropChange( CswNbtNodeProp Prop )
        {
            switch( RequestBy.Value )
            {
                case RequestsBy.Size:
                    Quantity.CachedUnitName = "";
                    Quantity.UnitId = null;
                    Quantity.Quantity = Double.NaN;
                    break;
                case RequestsBy.Bulk:
                    Size.CachedNodeName = "";
                    Size.RelatedNodeId = null;
                    Count.Value = Double.NaN;
                    break;
            }
            /* Spec W1010: Size and Count apply only to Request */
            Size.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Count.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Size.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Count.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value != RequestsBy.Size ), SaveToDb: true );

            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            Quantity.setHidden( value: ( RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
            Quantity.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
        }

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }
        private void OnQuantityPropChange( CswNbtNodeProp Prop )
        {
            TotalDispensed.UnitId = Quantity.UnitId;
        }

        public CswNbtNodePropRelationship Size
        {
            get { return _CswNbtNode.Properties[PropertyName.Size]; }
        }

        public CswNbtNodePropNumber Count
        {
            get { return _CswNbtNode.Properties[PropertyName.Count]; }
        }

        public CswNbtNodePropRelationship Material
        {
            get { return _CswNbtNode.Properties[PropertyName.Material]; }
        }
        private void OnMaterialPropChange( CswNbtNodeProp Prop )
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
        private void OnContainerPropChange( CswNbtNodeProp Prop )
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
        public CswNbtNodePropLocation Location
        {
            get { return _CswNbtNode.Properties[PropertyName.Location]; }
        }

        public CswNbtNodePropComments Comments
        {
            get { return _CswNbtNode.Properties[PropertyName.Comments]; }
        }

        public CswNbtNodePropList Status
        {
            get { return _CswNbtNode.Properties[PropertyName.Status]; }
        }

        private void OnStatusPropChange( CswNbtNodeProp Prop )
        {
            AssignedTo.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );
            Fulfill.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );
            TotalDispensed.setHidden( value: ( Status.Value == Statuses.Pending || ( Type.Value != Types.Dispense && Type.Value != Types.Request ) ), SaveToDb: true );

            //27800 - don't show redundant props when status is pending
            Request.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            RequestBy.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Name.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Requestor.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Status.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Submitted:
                    _toggleReadOnlyProps( true, this );
                    break;
                case Statuses.Received:
                    Fulfill.State = FulfillMenu.Dispense;
                    break;
                case Statuses.Dispensed:
                    if( TotalDispensed.Quantity >= Quantity.Quantity )
                    {
                        Fulfill.State = FulfillMenu.Complete;
                    }
                    break;
                case Statuses.Moved: //This fallthrough is intentional
                case Statuses.Disposed:
                    Fulfill.State = FulfillMenu.Complete;
                    break;
                case Statuses.Ordered:
                    Fulfill.State = FulfillMenu.Receive;
                    break;
                case Statuses.Cancelled: //This fallthrough is intentional
                case Statuses.Completed:
                    CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
                    if( null != NodeAsRequest )
                    {
                        NodeAsRequest.setCompletedDate();
                    }
                    Node.setReadOnly( true, true );
                    break;
            }

            if( Status.Value != Statuses.Pending &&
                Status.Value != Status.GetOriginalPropRowValue() )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );
                /* Email notification logic */
                if( null != NodeAsRequest &&
                    null != NodeAsRequest.Requestor.RelatedNodeId )
                {
                    CswNbtObjClassUser RequestorAsUser = _CswNbtResources.Nodes.GetNode( NodeAsRequest.Requestor.RelatedNodeId );
                    if( null != RequestorAsUser )
                    {
                        string Subject = Node.NodeName + "'s Request Item Status has Changed to " + Status.Value;
                        string Message = _makeNotificationMessage();
                        string Recipient = RequestorAsUser.Email;
                        Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message, Recipient );
                        _CswNbtResources.sendEmailNotification( EmailMessage );
                    }
                }
            }
        }

        public CswNbtNodePropSequence Number
        {
            get { return _CswNbtNode.Properties[PropertyName.Number]; }
        }

        public CswNbtNodePropText ExternalOrderNumber
        {
            get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber]; }
        }

        public CswNbtNodePropRelationship AssignedTo { get { return _CswNbtNode.Properties[PropertyName.AssignedTo]; } }

        public CswNbtNodePropButton Fulfill { get { return _CswNbtNode.Properties[PropertyName.Fulfill]; } }

        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }

        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
        private void OnTotalDispensedPropChange( CswNbtNodeProp Prop )
        {
            if( Status.Value != Statuses.Pending &&
                Status.Value != Statuses.Cancelled &&
                Status.Value != Statuses.Completed )
            {
                if( TotalDispensed.Quantity >= Quantity.Quantity )
                {
                    Fulfill.State = FulfillMenu.Complete;
                }
                else if( TotalDispensed.Quantity > 0 && ( Type.Value == Types.Request || Type.Value == Types.Dispense ) )
                {
                    Fulfill.State = FulfillMenu.Dispense;
                    Status.Value = Statuses.Dispensed;
                }
            }
        }

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }

        public CswNbtNodePropPropertyReference Requestor { get { return _CswNbtNode.Properties[PropertyName.Requestor]; } }

        #endregion
    }//CswNbtObjClassRequestItem

}//namespace ChemSW.Nbt.ObjClasses
