using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    public interface ICswNbtKioskModeStatusable
    {
        void ChangeStatus(string Status);
        bool CanChangeStatus(out string Error);

        CswNbtNodePropList Status { get; }
    }
}
