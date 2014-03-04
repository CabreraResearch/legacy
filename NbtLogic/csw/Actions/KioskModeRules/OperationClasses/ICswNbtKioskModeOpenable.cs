
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions.KioskModeRules.OperationClasses
{
    public interface ICswNbtKioskModeOpenable
    {
        bool CanOpen();
        void OpenItem();

        CswNbtNodePropButton Open { get; }
        CswNbtNodePropDateTime OpenedDate { get; }
        CswNbtNodePropDateTime ExpirationDate { get; }
        CswNbtNodePropRelationship Material { get; }
    }
}
