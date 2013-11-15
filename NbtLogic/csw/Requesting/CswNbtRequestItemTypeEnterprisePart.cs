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
    }
}
