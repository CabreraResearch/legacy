using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// Defines nodetypes that can be the target of a Generator.
    /// </summary>
    public interface ICswNbtPropertySetRequest
    {
        string RequestButtonPropertyName { get; }
        CswNbtNodePropButton Request { get; }

    } // ICswNbtPropertySetGeneratorTarget
}
