//using System;
//using System.Collections.ObjectModel;
//using ChemSW.Core;
//using ChemSW.Exceptions;
//using ChemSW.Mail;
//using ChemSW.Nbt.Actions;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Nbt.ServiceDrivers;
//using ChemSW.Nbt.UnitsOfMeasure;
//using Newtonsoft.Json.Linq;

//namespace ChemSW.Nbt.ObjClasses
//{
//    /// <summary>
//    /// Material Dispense Request Item
//    /// </summary>
//    public class CswNbtObjClassRequestMaterialDispense : CswNbtPropertySetRequestItem
//    {
//        /// <summary>
//        /// Property Names
//        /// </summary>
//        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
//        {
//            /// <summary>
//            /// For "Request By Size" items, the number(<see cref="CswNbtNodePropNumber"/>) of sizes(<see cref="Size"/>) to request. 
//            /// </summary>
//            public const string Count = "Count";

//            /// <summary>
//            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material (<see cref="CswNbtObjClassMaterial"/>) from which the Request Item will be Fulfilled.
//            /// </summary>
//            public const string Material = "Material";

//            /// <summary>
//            /// For "Request By Bulk" items, the Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
//            /// </summary>
//            public const string Quantity = "Quantity";

//            /// <summary>
//            /// For "Request By Size" items, a relationship(<see cref="CswNbtNodePropRelationship"/>) to the Size(<see cref="CswNbtObjClassSize"/>) to request. 
//            /// </summary>
//            public const string Size = "Size";

//            /// <summary>
//            /// For Dispense requests, the total amount(<see cref="CswNbtNodePropQuantity"/>) dispensed.
//            /// <para>ServerManaged</para>
//            /// </summary>
//            public const string TotalDispensed = "Total Dispensed";

//            /// <summary>
//            /// The request's type(<see cref="CswNbtNodePropList"/>)
//            /// <para>ServerManaged</para>
//            /// </summary>
//            public const string Type = "Type";
//        }

//        public sealed class Types
//        {
//            public const string Bulk = "Request By Bulk";
//            public const string Size = "Request By Size";
//            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Bulk, Size };
//        }

//        public sealed class Statuses
//        {
//            public const string Pending = "Pending";
//            public const string Submitted = "Submitted";
//            public const string Ordered = "Ordered";
//            public const string Received = "Received";
//            public const string Dispensed = "Dispensed";
//            public const string Completed = "Completed";
//            public const string Cancelled = "Cancelled";

//            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
//                {
//                    Pending, Submitted, Ordered, Received, Dispensed, Completed, Cancelled
//                };
//        }

//        public sealed class FulfillMenu
//        {
//            public const string CreateMaterial = "Create Material";
//            public const string Order = "Order";
//            public const string Receive = "Receive";
//            public const string Dispense = "Dispense this Container";
//            public const string Complete = "Complete Request";
//            public const string Cancel = "Cancel Request";

//            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
//                {
//                    Order, Receive, Dispense, Complete, Cancel
//                };
//        }

//        private CswNbtPropertySetRequestItem _CswNbtPropertySetRequestItem = null;

//        public static implicit operator CswNbtObjClassRequestMaterialDispense( CswNbtNode Node )
//        {
//            CswNbtObjClassRequestMaterialDispense ret = null;
//            if( null != Node && _Validate( Node, NbtObjectClass.RequestMaterialDispenseClass ) )
//            {
//                ret = (CswNbtObjClassRequestMaterialDispense) Node.ObjClass;
//            }
//            return ret;
//        }

//        public CswNbtObjClassRequestMaterialDispense( CswNbtResources CswNbtResources, CswNbtNode Node )
//            : base( CswNbtResources, Node )
//        {
//            _CswNbtPropertySetRequestItem = new CswNbtPropertySetRequestItem( CswNbtResources, Node );

//        }//ctor()

//        public override CswNbtMetaDataObjectClass ObjectClass
//        {
//            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass ); }
//        }

//        #region Inherited Events

//        private void _toggleReadOnlyProps( bool IsReadOnly, CswNbtObjClassRequestMaterialDispense ItemInstance )
//        {
//            ItemInstance.Request.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Type.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Quantity.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Size.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Count.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Material.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Location.setReadOnly( value: IsReadOnly, SaveToDb: true );
//            ItemInstance.Number.setReadOnly( value: IsReadOnly, SaveToDb: true );
//        }

//        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
//        {
//            //case 2753 - naming logic
//            if( false == IsTemp )
//            {
//                if( Type.Value.Equals( Types.Size ) && CswTools.IsPrimaryKey( Size.RelatedNodeId ) ) //request material by size
//                {
//                    CswNbtObjClassSize sizeNode = _CswNbtResources.Nodes.GetNode( Size.RelatedNodeId );
//                    if( null != sizeNode )
//                    {
//                        Name.Text = "Request " + Count.Value + " x " + sizeNode.Node.NodeName;
//                    }
//                }
//                else //request material by bulk
//                {
//                    Name.Text = "Request " + Quantity.Quantity + Quantity.CachedUnitName;
//                }
//            }

//            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
//        }//beforeWriteNode()

//        public override void afterWriteNode()
//        {
//            _CswNbtObjClassDefault.afterWriteNode();
//        }//afterWriteNode()

//        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
//        {
//            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

//        }//beforeDeleteNode()

//        public override void afterDeleteNode()
//        {
//            _CswNbtObjClassDefault.afterDeleteNode();
//        }//afterDeleteNode()        

//        public new void afterPopulateProps()
//        {
//            Quantity.SetOnPropChange( OnQuantityPropChange );
//            TotalDispensed.SetOnPropChange( OnTotalDispensedPropChange );
//            RequestBy.SetOnPropChange( OnRequestByPropChange );
//            Type.SetOnPropChange( OnTypePropChange );
//            Material.SetOnPropChange( OnMaterialPropChange );
//            Container.SetOnPropChange( OnContainerPropChange );
//            Status.SetOnPropChange( OnStatusPropChange );

//            _CswNbtObjClassDefault.afterPopulateProps();
//        }//afterPopulateProps()

//        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
//        {

//            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
//        }

//        public override bool onButtonClick( NbtButtonData ButtonData )
//        {
//            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
//            if( null != ButtonData.NodeTypeProp && null != OCP )
//            {
//                switch( OCP.PropName )
//                {
//                    case PropertyName.Fulfill:
//                        CswNbtObjClassContainer NodeAsContainer = null;
//                        switch( ButtonData.SelectedText )
//                        {
//                            case FulfillMenu.Cancel:
//                                ButtonData.Action = NbtButtonAction.refresh;
//                                break;
//                            case FulfillMenu.Complete:
//                                ButtonData.Action = NbtButtonAction.refresh;
//                                break;
//                            case FulfillMenu.Dispense:
//                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
//                                if( null != NodeAsContainer && null != NodeAsContainer.Dispense.NodeTypeProp )
//                                {
//                                    NbtButtonData DispenseData = new NbtButtonData( NodeAsContainer.Dispense.NodeTypeProp );
//                                    NodeAsContainer.onButtonClick( DispenseData );
//                                    ButtonData.clone( DispenseData );
//                                }
//                                else
//                                {
//                                    ButtonData.Data["containernodetypeid"] = Container.TargetId;
//                                    ButtonData.Data["containerobjectclassid"] = Container.TargetId;
//                                    JObject InitialQuantity = null;
//                                    if( null != Size.RelatedNodeId && Int32.MinValue != Size.RelatedNodeId.PrimaryKey )
//                                    {
//                                        CswNbtObjClassSize NodeAsSize = _CswNbtResources.Nodes[Size.RelatedNodeId];
//                                        if( null != NodeAsSize )
//                                        {
//                                            InitialQuantity = new JObject();
//                                            NodeAsSize.InitialQuantity.ToJSON( InitialQuantity );
//                                            ButtonData.Data["initialQuantity"] = InitialQuantity;
//                                        }
//                                    }
//                                    else if( false == Quantity.Empty )
//                                    {
//                                        InitialQuantity = new JObject();
//                                        Quantity.ToJSON( InitialQuantity );
//                                    }
//                                    if( null != InitialQuantity )
//                                    {
//                                        ButtonData.Data["initialQuantity"] = InitialQuantity;
//                                    }
//                                    ButtonData.Action = NbtButtonAction.dispense;
//                                }
//                                break;
//                            case FulfillMenu.Dispose:
//                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
//                                if( null != NodeAsContainer )
//                                {
//                                    NodeAsContainer.Disposed.Checked = Tristate.True;
//                                    NodeAsContainer.postChanges( true );
//                                }
//                                ButtonData.Action = NbtButtonAction.refresh;
//                                break;
//                            case FulfillMenu.Move:
//                                if( null != Container.RelatedNodeId )
//                                {
//                                    NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
//                                    if( null != NodeAsContainer )
//                                    {
//                                        NodeAsContainer.Location.SelectedNodeId = Location.SelectedNodeId;
//                                        NodeAsContainer.Location.CachedNodeName = Location.CachedNodeName;
//                                        NodeAsContainer.Location.CachedPath = Location.CachedPath;
//                                        NodeAsContainer.postChanges( false );
//                                        Status.Value = Statuses.Completed;
//                                        ButtonData.Action = NbtButtonAction.refresh;
//                                        //ButtonData.Data["nodeid"] = Container.RelatedNodeId.ToString();
//                                        //CswPropIdAttr LocIdAttr = new CswPropIdAttr( NodeAsContainer.Node, NodeAsContainer.Location.NodeTypeProp );
//                                        //ButtonData.Data["propidattr"] = LocIdAttr.ToString();
//                                        //Status.Value = Statuses.Moved;
//                                        //ButtonData.Action = NbtButtonAction.editprop;
//                                        //ButtonData.Data["title"] = "Set " + NodeAsContainer.Node.NodeName + " Container's Location";
//                                    }
//                                }
//                                break;
//                            case FulfillMenu.Order:
//                                ButtonData.Action = NbtButtonAction.editprop;
//                                ButtonData.Data["nodeid"] = NodeId.ToString();
//                                CswPropIdAttr OrdIdAttr = new CswPropIdAttr( Node, ExternalOrderNumber.NodeTypeProp );
//                                ButtonData.Data["propidattr"] = OrdIdAttr.ToString();
//                                ButtonData.Data["title"] = "Enter the External Order Number";
//                                break;
//                            case FulfillMenu.Receive:
//                                CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
//                                if( null != NodeAsMaterial )
//                                {
//                                    if( null != NodeAsMaterial.Receive.NodeTypeProp )
//                                    {
//                                        NbtButtonData ReceiveData = new NbtButtonData( NodeAsMaterial.Receive.NodeTypeProp );
//                                        NodeAsMaterial.onButtonClick( ReceiveData );
//                                        ButtonData.clone( ReceiveData );
//                                        Int32 DocumentNodeTypeId = CswNbtActReceiving.getMaterialDocumentNodeTypeId( _CswNbtResources, NodeAsMaterial );
//                                        if( Int32.MinValue != DocumentNodeTypeId )
//                                        {
//                                            ButtonData.Data["documenttypeid"] = DocumentNodeTypeId;
//                                        }
//                                    }
//                                }
//                                break;
//                        } //switch( ButtonData.SelectedText )

//                        _getNextStatus( ButtonData.SelectedText );
//                        postChanges( true );
//                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
//                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
//                        ButtonData.Data["requestitem"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
//                        ButtonData.Data["requestitem"]["containerid"] = ( Container.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
//                        ButtonData.Data["requestitem"]["locationid"] = ( Location.SelectedNodeId ?? new CswPrimaryKey() ).ToString();
//                        break; //case PropertyName.Fulfill:
//                }
//            }
//            return true;
//        }

//        private void _getNextStatus( string ButtonText )
//        {
//            switch( ButtonText )
//            {
//                case FulfillMenu.Cancel:
//                    setNextStatus( Statuses.Cancelled );
//                    break;
//                case FulfillMenu.Complete:
//                    setNextStatus( Statuses.Completed );
//                    break;
//                case FulfillMenu.Dispose:
//                    setNextStatus( Statuses.Disposed );
//                    break;
//                case FulfillMenu.Move:
//                    setNextStatus( Statuses.Moved );
//                    break;
//                case FulfillMenu.Order:
//                    setNextStatus( Statuses.Ordered );
//                    break;
//                case FulfillMenu.Receive:
//                    setNextStatus( Statuses.Received );
//                    break;
//            }
//        }

//        public void setNextStatus( string StatusVal )
//        {
//            switch( Status.Value )
//            {
//                case Statuses.Submitted:
//                    if( StatusVal == Statuses.Dispensed || StatusVal == Statuses.Disposed || StatusVal == Statuses.Moved || StatusVal == Statuses.Received || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
//                    {
//                        Status.Value = StatusVal;
//                    }
//                    break;
//                case Statuses.Received:
//                    if( StatusVal == Statuses.Dispensed || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
//                    {
//                        Status.Value = StatusVal;
//                    }
//                    break;
//                case Statuses.Dispensed:
//                case Statuses.Moved:
//                case Statuses.Disposed:
//                    if( StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
//                    {
//                        Status.Value = StatusVal;
//                    }
//                    break;
//            }
//        }

//        #endregion

//        #region CswNbtPropertySetRequestItem Members

//        public override CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
//        public override CswNbtNodePropPropertyReference Requestor { get { return _CswNbtNode.Properties[PropertyName.Requestor]; } }
//        public override CswNbtNodePropRelationship Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
//        public override CswNbtNodePropRelationship RequestedFor { get { return _CswNbtNode.Properties[PropertyName.RequestedFor]; } }
//        public override CswNbtNodePropLocation Location
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Location]; }
//        }

//        public override CswNbtNodePropComments Comments
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Comments]; }
//        }

//        public override CswNbtNodePropDateTime NeededBy
//        {
//            get { return _CswNbtNode.Properties[PropertyName.NeededBy]; }
//        }

//        public override CswNbtNodePropList Status
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Status]; }
//        }

//        private void OnStatusPropChange( CswNbtNodeProp Prop )
//        {
//            AssignedTo.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );
//            Fulfill.setHidden( value: ( Status.Value == Statuses.Pending || Status.Value == Statuses.Completed || Status.Value == Statuses.Cancelled ), SaveToDb: true );
//            TotalDispensed.setHidden( value: ( Status.Value == Statuses.Pending || ( Type.Value != Types.Dispense && Type.Value != Types.Request ) ), SaveToDb: true );

//            //27800 - don't show redundant props when status is pending
//            Request.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
//            RequestBy.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
//            Name.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
//            Requestor.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
//            Status.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
//            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

//            switch( Status.Value )
//            {
//                case Statuses.Submitted:
//                    _toggleReadOnlyProps( true, this );
//                    break;
//                case Statuses.Received:
//                    Fulfill.State = FulfillMenu.Dispense;
//                    break;
//                case Statuses.Dispensed:
//                    if( TotalDispensed.Quantity >= Quantity.Quantity )
//                    {
//                        Fulfill.State = FulfillMenu.Complete;
//                    }
//                    break;
//                case Statuses.Moved: //This fallthrough is intentional
//                case Statuses.Disposed:
//                    Fulfill.State = FulfillMenu.Complete;
//                    break;
//                case Statuses.Ordered:
//                    Fulfill.State = FulfillMenu.Receive;
//                    break;
//                case Statuses.Cancelled: //This fallthrough is intentional
//                case Statuses.Completed:
//                    CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes[Request.RelatedNodeId];
//                    if( null != NodeAsRequest )
//                    {
//                        NodeAsRequest.setCompletedDate();
//                    }
//                    Node.setReadOnly( true, true );
//                    break;
//            }

//            if( Status.Value != Statuses.Pending &&
//                Status.Value != Status.GetOriginalPropRowValue() )
//            {
//                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );
//                /* Email notification logic */
//                if( null != NodeAsRequest &&
//                    null != NodeAsRequest.Requestor.RelatedNodeId )
//                {
//                    CswNbtObjClassUser RequestorAsUser = _CswNbtResources.Nodes.GetNode( NodeAsRequest.Requestor.RelatedNodeId );
//                    if( null != RequestorAsUser )
//                    {
//                        string Subject = Node.NodeName + "'s Request Item Status has Changed to " + Status.Value;
//                        string Message = _makeNotificationMessage();
//                        string Recipient = RequestorAsUser.Email;
//                        Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message, Recipient );
//                        _CswNbtResources.sendEmailNotification( EmailMessage );
//                    }
//                }
//            }
//        }

//        public override CswNbtNodePropSequence Number
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Number]; }
//        }

//        public override CswNbtNodePropText ExternalOrderNumber
//        {
//            get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber]; }
//        }

//        public override CswNbtNodePropRelationship AssignedTo { get { return _CswNbtNode.Properties[PropertyName.AssignedTo]; } }

//        public override CswNbtNodePropButton Fulfill { get { return _CswNbtNode.Properties[PropertyName.Fulfill]; } }
//        #endregion

//        #region Object class specific properties

//        public CswNbtNodePropList Type
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Type]; }
//        }
//        private void OnTypePropChange( CswNbtNodeProp Prop )
//        {
//            /* Spec W1010: Location applies to all but Dispose */
//            Location.setHidden( value: ( Types.Dispose == Type.Value ), SaveToDb: true );
//            Location.setReadOnly( value: ( Status.Value != Statuses.Pending || Types.Dispose == Type.Value ), SaveToDb: true );

//            /* Spec W1010: Container applies only to Dispense, Dispose and Move */
//            RequestBy.setReadOnly( value: ( Status.Value != Statuses.Pending || ( Types.Request != Type.Value && Types.Dispense != Type.Value ) ), SaveToDb: true );
//            RequestBy.setHidden( value: ( Types.Request != Type.Value && Types.Dispense != Type.Value ), SaveToDb: true );
//            Container.setHidden( value: ( Types.Request == Type.Value ), SaveToDb: true );

//            /* Spec W1010: Material applies only to Request and Dispense */
//            Material.setHidden( value: ( Types.Request != Type.Value && Types.Dispense != Type.Value ), SaveToDb: true );
//            //AssignedTo.setHidden( value: ( AssignedTo.Hidden && Types.Dispense != Type.Value ), SaveToDb: true );

//            switch( Type.Value )
//            {
//                case Types.Request: //This fall through is intentional
//                case Types.Dispense:
//                    Fulfill.MenuOptions = FulfillMenu.DispenseOptions.ToString();
//                    Fulfill.State = FulfillMenu.Dispense;
//                    break;
//                case Types.Dispose:
//                    Fulfill.MenuOptions = FulfillMenu.DisposeOptions.ToString();
//                    Fulfill.State = FulfillMenu.Dispose;
//                    break;
//                case Types.Move:
//                    Fulfill.MenuOptions = FulfillMenu.MoveOptions.ToString();
//                    Fulfill.State = FulfillMenu.Move;
//                    break;
//            }
//        }

//        public CswNbtNodePropList RequestBy
//        {
//            get { return _CswNbtNode.Properties[PropertyName.RequestBy]; }
//        }
//        private void OnRequestByPropChange( CswNbtNodeProp Prop )
//        {
//            switch( RequestBy.Value )
//            {
//                case RequestsBy.Size:
//                    Quantity.CachedUnitName = "";
//                    Quantity.UnitId = null;
//                    Quantity.Quantity = Double.NaN;
//                    break;
//                case RequestsBy.Bulk:
//                    Size.CachedNodeName = "";
//                    Size.RelatedNodeId = null;
//                    Count.Value = Double.NaN;
//                    break;
//            }
//            /* Spec W1010: Size and Count apply only to Request */
//            Size.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
//            Count.setHidden( value: ( RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
//            Size.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value != RequestsBy.Size ), SaveToDb: true );
//            Count.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value != RequestsBy.Size ), SaveToDb: true );

//            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
//            Quantity.setHidden( value: ( RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
//            Quantity.setReadOnly( value: ( Status.Value != Statuses.Pending && RequestBy.Value == RequestsBy.Size ), SaveToDb: true );
//        }

//        public CswNbtNodePropQuantity Quantity
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
//        }
//        private void OnQuantityPropChange( CswNbtNodeProp Prop )
//        {
//            TotalDispensed.UnitId = Quantity.UnitId;
//        }

//        public CswNbtNodePropRelationship Size
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Size]; }
//        }

//        public CswNbtNodePropNumber Count
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Count]; }
//        }

//        public CswNbtNodePropRelationship Material
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Material]; }
//        }
//        private void OnMaterialPropChange( CswNbtNodeProp Prop )
//        {
//            if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
//            {
//                Material.setReadOnly( value: true, SaveToDb: true );
//                CswNbtNode MaterialNode = _CswNbtResources.Nodes[Material.RelatedNodeId];
//                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
//                Vb.setQuantityUnitOfMeasureView( MaterialNode, Quantity );
//                Vb.setQuantityUnitOfMeasureView( MaterialNode, TotalDispensed );
//                TotalDispensed.Quantity = 0;
//            }
//        }
//        public CswNbtNodePropRelationship Container
//        {
//            get { return _CswNbtNode.Properties[PropertyName.Container]; }
//        }
//        private void OnContainerPropChange( CswNbtNodeProp Prop )
//        {
//            if( null != Container.RelatedNodeId )
//            {
//                Container.setReadOnly( value: true, SaveToDb: true );
//                if( null == Material.RelatedNodeId )
//                {
//                    CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes[Container.RelatedNodeId];
//                    if( null != NodeAsContainer )
//                    {
//                        Material.RelatedNodeId = NodeAsContainer.Material.RelatedNodeId;
//                    }
//                }
//            }
//        }


//        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup]; } }

//        public CswNbtNodePropQuantity TotalDispensed { get { return _CswNbtNode.Properties[PropertyName.TotalDispensed]; } }
//        private void OnTotalDispensedPropChange( CswNbtNodeProp Prop )
//        {
//            if( Status.Value != Statuses.Pending &&
//                Status.Value != Statuses.Cancelled &&
//                Status.Value != Statuses.Completed )
//            {
//                if( TotalDispensed.Quantity >= Quantity.Quantity )
//                {
//                    Fulfill.State = FulfillMenu.Complete;
//                }
//                else if( TotalDispensed.Quantity > 0 && ( Type.Value == Types.Request || Type.Value == Types.Dispense ) )
//                {
//                    Fulfill.State = FulfillMenu.Dispense;
//                    Status.Value = Statuses.Dispensed;
//                }
//            }
//        }



//        #endregion
//    }//CswNbtObjClassRequestMaterialDispense

//}//namespace ChemSW.Nbt.ObjClasses
