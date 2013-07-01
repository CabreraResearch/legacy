using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This interface defines Object Classes to which PermissionSet Object Classes may point
    /// </summary>
    public interface ICswNbtPermissionGroup
    {
        CswEnumNbtObjectClass PermissionClass { get; }
        CswEnumNbtObjectClass TargetClass { get; }
    }
}
