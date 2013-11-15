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
                case CswNbtObjClassRequestItem.PropertyName.Material:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                    IsVisible = false;
                    break;
            }
            Prop.setHidden( false == IsVisible, SaveToDb: false );
        }
    }
}
