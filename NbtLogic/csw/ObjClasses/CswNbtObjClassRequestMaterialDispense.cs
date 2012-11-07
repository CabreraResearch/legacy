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
        /// <summary>
        /// Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
        {
            /// <summary>
            /// For "Request By Size" items, the number(<see cref="CswNbtNodePropNumber"/>) of sizes(<see cref="Size"/>) to request. 
            /// </summary>
            public const string Count = "Count";

            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material (<see cref="CswNbtObjClassMaterial"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Material = "Material";

            /// <summary>
            /// For "Request By Bulk" items, the Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
            /// </summary>
            public const string Quantity = "Quantity";

            /// <summary>
            /// For "Request By Size" items, a relationship(<see cref="CswNbtNodePropRelationship"/>) to the Size(<see cref="CswNbtObjClassSize"/>) to request. 
            /// </summary>
            public const string Size = "Size";

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

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Ordered, Received, Dispensed, Completed, Cancelled
                };
        }

        /// <summary>
        /// Fulfill menu options
        /// </summary>
        public new sealed class FulfillMenu : CswNbtPropertySetRequestItem.FulfillMenu
        {
            public const string Order = "Order";
            public const string Receive = "Receive";
            public const string Dispense = "Dispense this Container";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Order, Receive, Dispense, Complete, Cancel
                };
        }

        public static implicit operator CswNbtObjClassRequestMaterialDispense( CswNbtNode Node )
        {
            CswNbtObjClassRequestMaterialDispense ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestMaterialDispenseClass ) )
            {
                ret = (CswNbtObjClassRequestMaterialDispense) Node.ObjClass;
            }
            return ret;
        }

        public CswNbtObjClassRequestMaterialDispense( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass ); }
        }

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
            Type.SetOnPropChange( onTypePropChange );
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
                                    //ButtonData.Data["containernodetypeid"] = Container.TargetId;
                                    //ButtonData.Data["containerobjectclassid"] = Container.TargetId;
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
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
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
            }
        }

        #endregion

        #region CswNbtPropertySetRequestItem Members

        /// <summary>
        /// 
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop )
        {
            TotalDispensed.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );
            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

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

                case Statuses.Ordered:
                    Fulfill.State = FulfillMenu.Receive;
                    break;
            }
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }

        public override void onTypePropChange( CswNbtNodeProp Prop )
        {
            switch( Type.Value )
            {
                case Types.Size:
                    Quantity.CachedUnitName = "";
                    Quantity.UnitId = null;
                    Quantity.Quantity = Double.NaN;
                    break;
                case Types.Bulk:
                    Size.CachedNodeName = "";
                    Size.RelatedNodeId = null;
                    Count.Value = Double.NaN;
                    break;
            }


            /* Spec W1010: Quantity applies only to Request by Bulk and Dispense */
            bool QuantityVisible = Type.Value != Types.Size;
            bool AmountsReadOnly = Status.Value != Statuses.Pending;

            Quantity.setHidden( value: QuantityVisible, SaveToDb: true );
            Size.setHidden( value: false == QuantityVisible, SaveToDb: true );
            Count.setHidden( value: false == QuantityVisible, SaveToDb: true );

            Quantity.setReadOnly( value: ( QuantityVisible && AmountsReadOnly ), SaveToDb: true );
            Size.setReadOnly( value: AmountsReadOnly, SaveToDb: true );
            Count.setReadOnly( value: AmountsReadOnly, SaveToDb: true );
        }

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }
        private void onQuantityPropChange( CswNbtNodeProp Prop )
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



        #endregion
    }//CswNbtObjClassRequestMaterialDispense

}//namespace ChemSW.Nbt.ObjClasses
