using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public class CswNbtRequestItemTypeMaterialCreate : CswNbtRequestItemType
    {
        public CswNbtRequestItemTypeMaterialCreate( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem ) : base( CswNbtResources, RequestItem ) { }

        public override CswNbtNodePropRelationship Target
        {
            get { return _RequestItem.Material; }
        }

        public override void setPropUIVisibility( CswNbtNodeProp Prop )
        {
            bool IsVisible = true;
            switch ( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.Container:
                case CswNbtObjClassRequestItem.PropertyName.EnterprisePart:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                    IsVisible = false;
                    break;
            }
            Prop.setHidden( false == IsVisible, SaveToDb: false );
        }

        public override void setDescription()
        {
            string Description = "Create new " + _RequestItem.NewMaterialType.SelectedNodeTypeNames() + ": " +
                _RequestItem.NewMaterialTradename.Text + " " +
                _RequestItem.NewMaterialSupplier.Gestalt;
            if( false == string.IsNullOrEmpty( _RequestItem.NewMaterialPartNo.Text ) )
            {
                Description += " " + _RequestItem.NewMaterialPartNo.Text;
            }
            _RequestItem.Description.StaticText = Description;
        }
    }
}
