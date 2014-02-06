using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeMaterialBySize : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeMaterialBySize( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

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
                case CswNbtObjClassRequestItem.PropertyName.Quantity:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                    Prop.setHidden( true, SaveToDb: false );
                    IsVisible = false;
                    break;
                case CswNbtObjClassRequestItem.PropertyName.Material:
                    Prop.setReadOnly( true, SaveToDb: false );
                    break;
                case CswNbtObjClassRequestItem.PropertyName.Size:
                    //Case 31302 - Filter Size options to requested Material
                    CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                    CswNbtView SizeView = _CswNbtResources.ViewSelect.restoreView( _RequestItem.Size.View.ViewId );
                    SizeView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship SizeVr = SizeView.AddViewRelationship( SizeOc, false );
                    SizeView.AddViewPropertyAndFilter( SizeVr, SizeMaterialOcp, _RequestItem.Material.RelatedNodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                    SizeView.AddViewPropertyAndFilter( SizeVr, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ), "false", FilterMode: CswEnumNbtFilterMode.NotEquals );
                    SizeView.AddViewPropertyAndFilter( SizeVr, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity ), FilterMode: CswEnumNbtFilterMode.NotNull, SubFieldName: CswEnumNbtSubFieldName.Value );
                    SizeView.AddViewPropertyAndFilter( SizeVr, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount ), FilterMode: CswEnumNbtFilterMode.NotNull );
                    SizeView.save();
                    _RequestItem.Size.OverrideView( SizeView );
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
            Description += "Dispense " + _RequestItem.SizeCount.Gestalt + " x " + _RequestItem.Size.Gestalt + " of " + _RequestItem.Material.Gestalt;
            _RequestItem.Description.StaticText = Description;
        }

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.Order, 
                    CswNbtObjClassRequestItem.FulfillMenu.Receive,
                    CswNbtObjClassRequestItem.FulfillMenu.DispenseMaterial,
                    CswNbtObjClassRequestItem.FulfillMenu.MoveContainers, 
                    CswNbtObjClassRequestItem.FulfillMenu.Complete,
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.MoveContainers;
        }
    }
}
