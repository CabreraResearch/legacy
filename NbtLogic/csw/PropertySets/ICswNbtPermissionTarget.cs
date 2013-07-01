using ChemSW.Core;

namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This interface defines Object Classes to which PermissionSet Object Classes apply
    /// </summary>
    public interface ICswNbtPermissionTarget
    {
        CswPrimaryKey getPermissionGroupId();
    }
}
