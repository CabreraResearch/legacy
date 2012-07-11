using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
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
            ItemInstance.Request.ReadOnly = IsReadOnly;
            ItemInstance.RequestBy.ReadOnly = IsReadOnly;
            ItemInstance.Type.ReadOnly = IsReadOnly;
            ItemInstance.Quantity.ReadOnly = IsReadOnly;
            ItemInstance.Size.ReadOnly = IsReadOnly;
            ItemInstance.Container.ReadOnly = IsReadOnly;
            ItemInstance.Count.ReadOnly = IsReadOnly;
            ItemInstance.Material.ReadOnly = IsReadOnly;
            ItemInstance.Location.ReadOnly = IsReadOnly;
            ItemInstance.Number.ReadOnly = IsReadOnly;
            ItemInstance.ExternalOrderNumber.ReadOnly = IsReadOnly;
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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
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
        private void OnTypePropChange()
        {
            /* Spec W1010: Location applies to all but Dispose */
            Location.Hidden = ( Types.Dispose == Type.Value );
            Location.ReadOnly = ( Types.Dispose == Type.Value );

            /* Spec W1010: Container applies only to Dispense, Dispose and Move */
            RequestBy.ReadOnly = ( Types.Request != Type.Value );
            RequestBy.Hidden = ( Types.Request != Type.Value );
            Container.Hidden = ( Types.Request == Type.Value );

            /* Spec W1010: Material applies only to Request and Dispense */
            Material.Hidden = ( Types.Request != Type.Value && Types.Dispense != Type.Value );
            AssignedTo.Hidden = ( Types.Dispense != Type.Value );

            switch( Type.Value )
            {
                case Types.Dispense:
                    Fulfill.MenuOptions = FulfillMenu.DispenseOptions.ToString();
                    Fulfill.SelectedMenuOption = FulfillMenu.Dispense;
                    break;
                case Types.Dispose:
                    Fulfill.MenuOptions = FulfillMenu.DisposeOptions.ToString();
                    Fulfill.SelectedMenuOption = FulfillMenu.Dispose;
                    break;
                case Types.Move:
                    Fulfill.MenuOptions = FulfillMenu.MoveOptions.ToString();
                    Fulfill.SelectedMenuOption = FulfillMenu.Move;
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
            Size.Hidden = ( RequestBy.Value != RequestsBy.Size );
            Count.Hidden = ( RequestBy.Value != RequestsBy.Size );
            Size.ReadOnly = ( RequestBy.Value != RequestsBy.Size );
            Count.ReadOnly = ( RequestBy.Value != RequestsBy.Size );

            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            Quantity.Hidden = ( RequestBy.Value == RequestsBy.Size );
            Quantity.ReadOnly = ( RequestBy.Value == RequestsBy.Size );
        }

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
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
                Material.ReadOnly = true;
                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                CswNbtView UnitView = Vb.getQuantityUnitOfMeasureView( Material.RelatedNodeId );
                if( null != UnitView )
                {
                    Quantity.View = UnitView;
                }
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
                Container.ReadOnly = true;
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
            CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );
            /* Email notification logic */
            if( Status.Value != Statuses.Pending )
            {
                if( Status.Value == Statuses.Submitted )
                {
                    _toggleReadOnlyProps( true, this );
                }

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
        #endregion
    }//CswNbtObjClassRequestItem

}//namespace ChemSW.Nbt.ObjClasses
