using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeEnterprisePart : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeEnterprisePart( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

        public override CswNbtNodePropRelationship Target
        {
            get { return _RequestItem.EnterprisePart; }
        }

        public override void setPropUIVisibility( CswNbtNodeProp Prop )
        {
            bool IsVisible = true;
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.Container:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                    Prop.setHidden( true, SaveToDb: false );
                    IsVisible = false;
                    break;
            }
            if( IsVisible && _RequestItem.IsRecurring.Checked == CswEnumTristate.True )
            {
                setRecurringPropVisibility( Prop );
            }
        }

        public override void setDescription()
        {
            string Description = "";
            if( CswEnumTristate.True == _RequestItem.IsRecurring.Checked && false == _RequestItem.RecurringFrequency.Empty )
            {
                Description = "Recurring " + _RequestItem.RecurringFrequency.RateInterval.RateType + ": ";
            }
            Description += "Dispense " + _RequestItem.Quantity.Gestalt + " of " + _RequestItem.EnterprisePart.Gestalt;
            _RequestItem.Description.StaticText = Description;
        }

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.Order,
                    CswNbtObjClassRequestItem.FulfillMenu.Receive, 
                    CswNbtObjClassRequestItem.FulfillMenu.MoveContainers,
                    CswNbtObjClassRequestItem.FulfillMenu.DispenseMaterial, 
                    CswNbtObjClassRequestItem.FulfillMenu.Complete,
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.DispenseMaterial;
        }
    }
}
