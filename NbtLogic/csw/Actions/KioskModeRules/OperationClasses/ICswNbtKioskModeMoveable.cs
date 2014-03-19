
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    public interface ICswNbtKioskModeMoveable
    {
        void Move(CswNbtObjClassLocation LocationToMoveTo);
        bool CanMove(out string Error);

        CswNbtNodePropLocation Location { get; }
    }
}
