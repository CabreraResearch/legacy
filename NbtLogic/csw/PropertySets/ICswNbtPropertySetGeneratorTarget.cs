using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// Defines nodetypes that can be the target of a Generator.
    /// </summary>
    public interface ICswNbtPropertySetGeneratorTarget
    {
        string GeneratorTargetGeneratedDatePropertyName { get; }
        string GeneratorTargetIsFuturePropertyName { get; }
        string GeneratorTargetGeneratorPropertyName { get; }
        string GeneratorTargetParentPropertyName { get; }

        CswNbtNodePropDateTime GeneratedDate { get; }
        CswNbtNodePropLogical IsFuture { get; }
        CswNbtNodePropRelationship Generator { get; }
        CswNbtNodePropRelationship Parent { get; }

    } // ICswNbtPropertySetGeneratorTarget
}
