using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Request Item Property Set
    /// </summary>
    public abstract class CswNbtPropertySetRequestItem : CswNbtObjClass
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public class PropertyName
        {
            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Inventory Group (<see cref="CswNbtObjClassInventoryGroup"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string InventoryGroup = "Inventory Group";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the User (<see cref="CswNbtObjClassUser"/>) to whom the Request Item is assigned.
            /// </summary>
            public const string AssignedTo = "Assigned To";

            /// <summary>
            /// Comments(<see cref="CswNbtNodePropComments"/>) on this Item
            /// </summary>
            public const string Comments = "Comments";

            /// <summary>
            /// External Order Number(<see cref="CswNbtNodePropText"/>)
            /// </summary>
            public const string ExternalOrderNumber = "External Order Number";

            /// <summary>
            /// Menu button(<see cref="CswNbtNodePropButton"/>) to fulfill request.
            /// </summary>
            public const string Fulfill = "Fulfill";

            /// <summary>
            /// Location(<see cref="CswNbtNodePropLocation"/> ) to which the request should be delivered
            /// </summary>
            public const string Location = "Location";

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
            /// Numeric priority of this Item, Sequence(<see cref="CswNbtNodePropNumber"/>)
            /// </summary>
            public const string Priority = "Priority";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Request(<see cref="CswNbtObjClassRequest"/>) to which this Item belongs. 
            /// <para>ServerManaged</para>
            /// </summary>
            public const string Request = "Request";

            /// <summary>
            /// The User (<see cref="CswNbtObjClassUser"/>) who initiated the Request(<see cref="CswNbtObjClassRequest"/>) as a Property Ref(<see cref="CswNbtNodePropPropertyReference"/>).
            /// </summary>
            public const string Requestor = "Requestor";

            /// <summary>
            /// A relationship(<see cref="CswNbtNodePropRelationship"/>) to the User (<see cref="CswNbtObjClassUser"/>) for whom the Request(<see cref="CswNbtObjClassRequest"/>) is intended.
            /// </summary>
            public const string RequestedFor = "RequestedFor";

            /// <summary>
            /// The status(<see cref="CswNbtNodePropList"/>) of the item.
            /// </summary>
            public const string Status = "Status";

            /// <summary>
            /// The Type(<see cref="CswNbtNodePropList"/>) of the item.
            /// </summary>
            public const string Type = "Type";
        }

        /// <summary>
        /// Property Set base class for Statuses
        /// </summary>
        public class Statuses
        {
            public const string Pending = "Pending";
            public const string Submitted = "Submitted";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
        }

        /// <summary>
        /// Property Set base class for Fulfill Menu Options
        /// </summary>
        public class FulfillMenu
        {
            public const string Complete = "Complete Request";
            public const string Cancel = "Cancel Request";
        }

        public class Types
        {

        }

        #endregion Enums

        public static Collection<NbtObjectClass> Members()
        {
            Collection<NbtObjectClass> Ret = new Collection<NbtObjectClass>();
            Ret.Add( NbtObjectClass.RequestContainerDispenseClass );
            Ret.Add( NbtObjectClass.RequestContainerUpdateClass );
            Ret.Add( NbtObjectClass.RequestMaterialCreateClass );
            Ret.Add( NbtObjectClass.RequestMaterialDispenseClass );
            return Ret;
        }

        private void _toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance )
        {
            ItemInstance.Request.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Location.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.Number.setReadOnly( value: IsReadOnly, SaveToDb: true );
            ItemInstance.toggleReadOnlyProps( IsReadOnly, ItemInstance );
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance );

        /// <summary>
        /// Copy the Request Item
        /// </summary>
        public CswNbtPropertySetRequestItem copyNode()
        {
            CswNbtPropertySetRequestItem RetCopy = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            RetCopy.Node.copyPropertyValues( Node );
            RetCopy.Status.Value = Statuses.Pending;
            RetCopy.Request.RelatedNodeId = null;
            _toggleReadOnlyProps( false, RetCopy );
            RetCopy.postChanges( true );
            return RetCopy;
        }

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        public CswNbtPropertySetRequestItem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetRequestItem
        /// </summary>
        public static implicit operator CswNbtPropertySetRequestItem( CswNbtNode Node )
        {
            CswNbtPropertySetRequestItem ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetRequestItem) Node.ObjClass;
            }
            return ret;
        }


        #region Inherited Events

        private void _setDefaultValues()
        {
            if( false == CswTools.IsPrimaryKey( Request.RelatedNodeId ) )
            {
                CswNbtActSubmitRequest RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true );
                Request.RelatedNodeId = RequestAct.CurrentRequestNode().NodeId;
                Request.setReadOnly( value: true, SaveToDb: true );
                Request.setHidden( value: true, SaveToDb: false );
            }
            CswNbtObjClassRequest ThisRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
            if( null != ThisRequest )
            {
                Requestor.RelatedNodeId = ThisRequest.Requestor.RelatedNodeId;
            }

        }

        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );
            _setDefaultValues();
            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public abstract void afterPropertySetWriteNode();

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()


        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public void setFulfillVisibility()
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

        public abstract void afterPropertySetPopulateProps();

        public override void afterPopulateProps()
        {
            //TODO: Create Mail Report for Status Changes

            afterPropertySetPopulateProps();
            setFulfillVisibility();
            Status.SetOnPropChange( _onStatusPropChange );
            Type.SetOnPropChange( _onTypePropChange );
            CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public abstract bool onPropertySetButtonClick( CswNbtMetaDataObjectClassProp OCP, NbtButtonData ButtonData );

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            bool Ret = false;
            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                switch( OCP.PropName )
                {
                    case PropertyName.Fulfill:
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Cancel:
                                Status.Value = Statuses.Cancelled;
                                Fulfill.State = FulfillMenu.Cancel;
                                Fulfill.MenuOptions = "";
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Complete:
                                Status.Value = Statuses.Completed;
                                Fulfill.State = FulfillMenu.Complete;
                                Fulfill.MenuOptions = "";
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
                        }
                        break;
                }
                Ret = onPropertySetButtonClick( OCP, ButtonData );
                postChanges( ForceUpdate: false );
            }

            return Ret;
        }
        #endregion

        #region Property Set specific properties

        public CswNbtNodePropButton Fulfill { get { return _CswNbtNode.Properties[PropertyName.Fulfill]; } }
        public CswNbtNodePropList Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        private void _onStatusPropChange( CswNbtNodeProp Prop )
        {
            AssignedTo.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );
            Fulfill.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );

            //27800 - don't show redundant props when status is pending
            Request.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Name.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Requestor.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Status.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Submitted:
                    toggleReadOnlyProps( true, this );
                    break;
                case Statuses.Cancelled: //This fallthrough is intentional
                case Statuses.Completed:
                    CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
                    if( null != NodeAsRequest )
                    {
                        NodeAsRequest.setCompletedDate();
                    }
                    _toggleReadOnlyProps( IsReadOnly: true, ItemInstance: this );
                    Node.setReadOnly( value: true, SaveToDb: true );
                    break;
            }

            onStatusPropChange( Prop );

        }

        /// <summary>
        /// Status change event for derived classes to implement
        /// </summary>
        public abstract void onStatusPropChange( CswNbtNodeProp Prop );

        public CswNbtNodePropComments Comments { get { return _CswNbtNode.Properties[PropertyName.Comments]; } }
        public CswNbtNodePropDateTime NeededBy { get { return _CswNbtNode.Properties[PropertyName.NeededBy]; } }

        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        private void _onTypePropChange( CswNbtNodeProp Prop )
        {
            onTypePropChange( Prop );
        }

        public abstract void onTypePropChange( CswNbtNodeProp Prop );

        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        public CswNbtNodePropNumber Priority { get { return _CswNbtNode.Properties[PropertyName.Priority]; } }
        public CswNbtNodePropRelationship AssignedTo { get { return _CswNbtNode.Properties[PropertyName.AssignedTo]; } }
        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }
        public CswNbtNodePropRelationship Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
        public CswNbtNodePropRelationship RequestedFor { get { return _CswNbtNode.Properties[PropertyName.RequestedFor]; } }
        public CswNbtNodePropRelationship Requestor { get { return _CswNbtNode.Properties[PropertyName.Requestor]; } }
        public CswNbtNodePropSequence Number { get { return _CswNbtNode.Properties[PropertyName.Number]; } }
        public CswNbtNodePropText ExternalOrderNumber { get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber]; } }
        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }

        #endregion


    }//CswNbtPropertySetRequestItem

}//namespace ChemSW.Nbt.ObjClasses
