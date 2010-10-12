using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Nbt.PropTypes;
using ChemSW.TblDn;


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

        CswNbtNodePropDate GeneratedDate { get; }
        CswNbtNodePropLogical IsFuture { get; }
        CswNbtNodePropRelationship Generator { get; }
        CswNbtNodePropRelationship Parent { get; }

    } // ICswNbtPropertySetGeneratorTarget
}
