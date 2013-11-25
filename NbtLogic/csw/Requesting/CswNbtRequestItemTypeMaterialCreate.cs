using ChemSW.Core;
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
            switch ( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.Container:
                case CswNbtObjClassRequestItem.PropertyName.EnterprisePart:
                case CswNbtObjClassRequestItem.PropertyName.Size:
                case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                    Prop.setHidden( true, SaveToDb: false );
                    break;
                case CswNbtObjClassRequestItem.PropertyName.Material:
                    Prop.setHidden( _RequestItem.Status.Value == CswNbtObjClassRequestItem.Statuses.Pending, SaveToDb: false );
                    break;
            }
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

        public override void setFulfillOptions()
        {
            _RequestItem.Fulfill.MenuOptions = new CswCommaDelimitedString
                {
                    CswNbtObjClassRequestItem.FulfillMenu.Create, 
                    CswNbtObjClassRequestItem.FulfillMenu.Cancel
                }.ToString();
            _RequestItem.Fulfill.State = CswNbtObjClassRequestItem.FulfillMenu.Create;
        }
    }
}
