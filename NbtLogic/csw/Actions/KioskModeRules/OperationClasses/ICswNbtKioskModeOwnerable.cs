
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    public interface ICswNbtKioskModeOwnerable
    {
        void UpdateOwner(CswNbtObjClassUser NewUser);
        bool CanUpdateOwner(out string Error);

        CswNbtNodePropRelationship User { get; }
    }
}
