using System;
using System.Collections.Generic;
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
            public const string Request = "Request";
            public const string Type = "Type";
            public const string RequestBy = "Request By";
            public const string Quantity = "Quantity";
            public const string Size = "Size";
            public const string Count = "Count";
            public const string Material = "Material";
            public const string Container = "Container";
            public const string Comments = "Comments";
            public const string Status = "Status";
            public const string Number = "Number";
            public const string ExternalOrderNumber = "External Order Number";
            public const string Location = "Location";
            public const string AssignedTo = "Assigned To";
            public const string Fulfill = "Fulfill";
            public const string InventoryGroup = "Inventory Group";
            public const string TotalDispensed = "Total Dispensed";
        }

        public sealed class Types
        {
            public const string Dispense = "Dispense";
            public const string Request = "Request";
            public const string Move = "Move";
            public const string Dispose = "Dispose";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Dispense, Request, Move, Dispose };
        }

        public sealed class RequestsBy
        {
            public const string Bulk = "Bulk";
            public const string Size = "Size";
            public const string Quantity = "Quantity";

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
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense";
            public const string Move = "Move";
            public const string Dispose = "Dispose";
            public const string Complete = "Complete";
            public const string Cancel = "Cancel";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Dispose, Move, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString DispenseOptions = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Dispose, Move, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString MoveOptions = new CswCommaDelimitedString
                {
                    Move, Complete, Cancel
                };
            public static readonly CswCommaDelimitedString DisposeOptions = new CswCommaDelimitedString
                {
                    Dispose, Move, Complete, Cancel
                };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public static implicit operator CswNbtObjClassRequestItem( CswNbtNode Node )
        {
            CswNbtObjClassRequestItem ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ) )
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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );

            CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart );
            Request.RelatedNodeId = RequestAct.CurrentRequestNode().NodeId;
        } // beforeCreateNode()

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
                MessageText += "Location: " + NodeAsLocation.Location + CswNbtNodePropLocation.PathDelimiter +
                                NodeAsLocation.Name + "\n";
            }


            return MessageText;
        }

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

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

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );

            /* Container-specific logic */
            if( ( Type.Value == Types.Dispense ||
                Type.Value == Types.Move ||
                Type.Value == Types.Dispose ) &&
                null != Container.RelatedNodeId )
            {
                if( null != NodeAsRequest &&
                    null != NodeAsRequest.InventoryGroup.RelatedNodeId )
                {
                    CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                    if( null == NodeAsContainer )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                  "A " + Type.Value +
                                                  " type of Request Item requires a valid Container.",
                                                  "Attempted to edit node without a valid Container relationship." );
                    }
                    CswNbtObjClassLocation NodeAsLocation =
                        _CswNbtResources.Nodes.GetNode( NodeAsContainer.Location.NodeId );
                    if( null != NodeAsLocation &&
                        NodeAsRequest.InventoryGroup.RelatedNodeId != NodeAsLocation.InventoryGroup.RelatedNodeId )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                  "For a " + Type.Value +
                                                  " type of Request Item, the Inventory Group of the Request must match the Inventory Group of the Container's Location.",
                                                  "Attempted to edit node without matching Container and Request Inventory Group relationships." );
                    }
                }
            }


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
            Quantity.SetOnPropChange( OnQuantityPropChange );
            TotalDispensed.SetOnPropChange( OnTotalDispensedPropChange );
            Request.SetOnPropChange( OnRequestPropChange );
            RequestBy.SetOnPropChange( OnRequestByPropChange );
            Type.SetOnPropChange( OnTypePropChange );
            Material.SetOnPropChange( OnMaterialPropChange );
            Container.SetOnPropChange( OnContainerPropChange );
            Status.SetOnPropChange( OnStatusPropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status.ToString() );
            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp, Statuses.Pending.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        private string _getNextStatus( string ButtonText )
        {
            string Ret = Status.Value;
            switch( ButtonText )
            {
                case FulfillMenu.Cancel:
                    Ret = Statuses.Cancelled;
                    break;
                case FulfillMenu.Complete:
                    Ret = Statuses.Completed;
                    break;
                case FulfillMenu.Move:
                    Ret = Statuses.Moved;
                    break;
                case FulfillMenu.Dispose:
                    Ret = Statuses.Disposed;
                    break;
                case FulfillMenu.Dispense:
                    Ret = Statuses.Dispensed;
                    break;
                case FulfillMenu.Order:
                    Ret = Statuses.Ordered;
                    break;
                case FulfillMenu.Receive:
                    Ret = Statuses.Received;
                    break;
            }
            if( FulfillMenu.Options.IndexOf( Status.Value ) >= FulfillMenu.Options.IndexOf( ButtonText ) )
            {
                Ret = Status.Value;
            }
            return Ret;
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
                                if( null != NodeAsContainer )
                                {
                                    if( null != NodeAsContainer.Dispense.NodeTypeProp )
                                    {
                                        NbtButtonData DispenseData = new NbtButtonData( NodeAsContainer.Dispense.NodeTypeProp );
                                        NodeAsContainer.onButtonClick( DispenseData );
                                        ButtonData.clone( DispenseData );
                                    }
                                }
                                ButtonData.Action = NbtButtonAction.dispense;
                                break;
                            case FulfillMenu.Dispose:
                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != NodeAsContainer )
                                {
                                    NodeAsContainer.Disposed.Checked = Tristate.True;
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
                                        ButtonData.Data["nodeid"] = Container.RelatedNodeId.ToString();
                                        CswPropIdAttr LocIdAttr = new CswPropIdAttr( NodeAsContainer.Node, NodeAsContainer.Location.NodeTypeProp );
                                        ButtonData.Data["propidattr"] = LocIdAttr.ToString();
                                        Status.Value = Statuses.Moved;
                                        ButtonData.Action = NbtButtonAction.editprop;
                                        ButtonData.Data["title"] = "Set " + NodeAsContainer.Node.NodeName + " Container's Location";
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
                                    }
                                }
                                break;
                        } //switch( ButtonData.SelectedText )

                        Status.Value = _getNextStatus( ButtonData.SelectedText );
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
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Request
        {
            get { return _CswNbtNode.Properties[PropertyName.Request]; }
        }
        private void OnRequestPropChange()
        {
            Request.setReadOnly( value: true, SaveToDb: true );
            Request.setHidden( value: true, SaveToDb: false );
        }

        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }
        private void OnTypePropChange()
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
        private void OnRequestByPropChange()
        {
            /* Spec W1010: Size and Count apply only to Request */
            Size.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Count.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Size.setReadOnly( value: ( Status.Value != Statuses.Pending || RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
            Count.setReadOnly( value: ( Status.Value != Statuses.Pending || RequestBy.Value != RequestsBy.Size ), SaveToDb: true );

            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            Quantity.setHidden( value: ( RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
            Quantity.setReadOnly( value: ( Status.Value != Statuses.Pending || RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
        }

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }
        private void OnQuantityPropChange()
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
        private void OnMaterialPropChange()
        {
            if( null != Material.RelatedNodeId )
            {
                Material.setReadOnly( value: true, SaveToDb: true );
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                CswNbtView UnitView = Vb.getQuantityUnitOfMeasureView( Material.RelatedNodeId );
                if( null != UnitView )
                {
                    Quantity.View = UnitView;
                }

                TotalDispensed.View = UnitView;
                TotalDispensed.Quantity = 0;
            }
        }
        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container]; }
        }
        private void OnContainerPropChange()
        {
            if( null != Container.RelatedNodeId )
            {
                Container.setReadOnly( value: true, SaveToDb: true );
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

        private void OnStatusPropChange()
        {
            AssignedTo.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ),
                SaveToDb: true );
            Fulfill.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ),
                SaveToDb: true );
            TotalDispensed.setHidden( value: ( Status.Value == Statuses.Pending || ( Type.Value != Types.Dispense && Type.Value != Types.Request ) ),
                SaveToDb: true );

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
                    Node.setReadOnly( true, true );
                    break;
            }

            if( Status.Value != Statuses.Pending )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );
                /* Email notification logic */
                if( null != NodeAsRequest &&
                    null != NodeAsRequest.Requestor.RelatedNodeId )
                {
                    CswNbtObjClassUser RequestorAsUser =
                        _CswNbtResources.Nodes.GetNode( NodeAsRequest.Requestor.RelatedNodeId );
                    if( null != RequestorAsUser )
                    {
                        string Subject = Node.NodeName + "'s Request Item Status has Changed to " + Status.Value;
                        string Message = _makeNotificationMessage();
                        string Recipient = RequestorAsUser.Email;
                        Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message,
                                                                                                    Recipient );
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

        public CswNbtNodePropPropertyReference InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }

        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
        private void OnTotalDispensedPropChange()
        {
            if( TotalDispensed.Quantity >= Quantity.Quantity )
            {
                Fulfill.State = FulfillMenu.Complete;
            }
        }

        #endregion
    }//CswNbtObjClassRequestItem

}//namespace ChemSW.Nbt.ObjClasses
