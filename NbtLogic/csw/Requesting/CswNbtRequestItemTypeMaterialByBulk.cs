﻿using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeMaterialByBulk : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeMaterialByBulk( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

        public override CswNbtNodePropRelationship Target
        {
            get { return _RequestItem.Material; }
        }

        public override void setPropUIVisibility( CswNbtNodeProp Prop )
        {
            bool IsVisible = true;
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.Container:
                case CswNbtObjClassRequestItem.PropertyName.EnterprisePart:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                    IsVisible = false;
                    break;
                case CswNbtObjClassRequestItem.PropertyName.Material:
                    Prop.setReadOnly( true, SaveToDb: false );
                    break;
            }
            Prop.setHidden( false == IsVisible, SaveToDb: false );
        }

        public override void setDescription()
        {
            string Description = "";
            if( CswEnumTristate.True == _RequestItem.IsRecurring.Checked && false == _RequestItem.RecurringFrequency.Empty )
            {
                Description = "Recurring " + _RequestItem.RecurringFrequency.RateInterval.RateType + ": ";
            }
            Description += "Dispense " + _RequestItem.Quantity.Gestalt + " of " + _RequestItem.Material.Gestalt;
            _RequestItem.Description.StaticText = Description;
        }

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.Order, 
                    CswNbtObjClassRequestItem.FulfillMenu.Receive,
                    CswNbtObjClassRequestItem.FulfillMenu.DispenseMaterial, 
                    CswNbtObjClassRequestItem.FulfillMenu.MoveMaterial, 
                    CswNbtObjClassRequestItem.FulfillMenu.Complete,
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.DispenseMaterial;
        }
    }
}
