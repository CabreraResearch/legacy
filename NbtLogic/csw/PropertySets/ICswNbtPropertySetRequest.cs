using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// Defines nodetypes that participate in Ordering.
    /// </summary>
    public interface ICswNbtPropertySetRequest
    {
        string RequestButtonPropertyName { get; }
        CswNbtNodePropButton Request { get; }

    } // ICswNbtPropertySetGeneratorTarget
}
