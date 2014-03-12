
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    public interface ICswNbtKioskModeTransferable
    {
        void Transfer(CswNbtObjClassUser NewUser);
        bool CanTransfer(out string Error);

        CswNbtNodePropRelationship User { get; }
        CswNbtNodePropLocation Location { get; }
    }
}
