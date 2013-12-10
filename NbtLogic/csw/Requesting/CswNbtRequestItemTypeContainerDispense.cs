using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeContainerDispense : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeContainerDispense( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

        public override CswNbtNodePropRelationship Target
        {
            get { return _RequestItem.Container; }
        }

        public override void setPropUIVisibility( CswNbtNodeProp Prop )
        {
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber:
                case CswNbtObjClassRequestItem.PropertyName.EnterprisePart:
                case CswNbtObjClassRequestItem.PropertyName.Material:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                case CswNbtObjClassRequestItem.PropertyName.GoodsReceived:
                case CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense:
                case CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived:
                    Prop.setHidden( true, SaveToDb: false );
                    break;
                case CswNbtObjClassRequestItem.PropertyName.InventoryGroup:
                    Prop.setReadOnly( true, SaveToDb: false );
                    break;
            }
        }

        public override void setDescription()
        {
            string Description = "Dispense " + _RequestItem.Quantity.Gestalt + 
                " from " + _RequestItem.Container.Gestalt + 
                " of " + _RequestItem.Material.Gestalt;
            _RequestItem.Description.StaticText = Description;
        }

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.DispenseContainer,
                    //CswNbtObjClassRequestItem.FulfillMenu.MoveContainers, //TODO - uncomment when fulfill Move container(s) works for parent and child containers
                    CswNbtObjClassRequestItem.FulfillMenu.Complete,
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.DispenseContainer;
        }

        public override void setQuantityOptions()
        {
            CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.GetNode( _RequestItem.Container.RelatedNodeId );
            if( null != ContainerNode )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( ContainerNode.Material.RelatedNodeId );
                if( MaterialNode != null )
                {
                    CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                    Vb.setQuantityUnitOfMeasureView( MaterialNode, _RequestItem.Quantity );
                }
            }
        }
    }
}
