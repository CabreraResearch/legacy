using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeContainerDispose : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeContainerDispose( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

        public override CswNbtNodePropRelationship Target
        {
            get { return _RequestItem.Container; }
        }

        public override void setPropUIVisibility( CswNbtNodeProp Prop )
        {
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.InventoryGroup:
                case CswNbtObjClassRequestItem.PropertyName.Location:
                case CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber:
                case CswNbtObjClassRequestItem.PropertyName.EnterprisePart:
                case CswNbtObjClassRequestItem.PropertyName.Material:
                case CswNbtObjClassRequestItem.PropertyName.Quantity:
                case CswNbtObjClassRequestItem.PropertyName.TotalDispensed:
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
            }
        }

        public override void setDescription()
        {
            string Description = "Dispose " + _RequestItem.Container.Gestalt + " of " + _RequestItem.Material.Gestalt;
            _RequestItem.Description.StaticText = Description;
        }

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.Dispose, 
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.Dispose;
        }
    }
}
