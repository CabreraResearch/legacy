using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Conversion
{

    /// <summary>
    /// Enum: Used to identify the UnitType relationship between two UnitOfMeasure Nodes in order to apply the correct conversion logic
    /// </summary>
    public sealed class CswEnumNbtUnitTypeRelationship : CswEnum<CswEnumNbtUnitTypeRelationship>
    {
        private CswEnumNbtUnitTypeRelationship( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtUnitTypeRelationship> _All { get { return CswEnum<CswEnumNbtUnitTypeRelationship>.All; } }

        public static explicit operator CswEnumNbtUnitTypeRelationship( string str )
        {
            CswEnumNbtUnitTypeRelationship ret = Parse( str );
            return ret ?? CswEnumNbtUnitTypeRelationship.Unknown;
        }

        public static readonly CswEnumNbtUnitTypeRelationship Unknown = new CswEnumNbtUnitTypeRelationship( "Unknown" );
        public static readonly CswEnumNbtUnitTypeRelationship Same = new CswEnumNbtUnitTypeRelationship( "Same" );
        public static readonly CswEnumNbtUnitTypeRelationship NotSupported = new CswEnumNbtUnitTypeRelationship( "NotSupported" );
        public static readonly CswEnumNbtUnitTypeRelationship WeightToVolume = new CswEnumNbtUnitTypeRelationship( "WeightToVolume" );
        public static readonly CswEnumNbtUnitTypeRelationship VolumeToWeight = new CswEnumNbtUnitTypeRelationship( "VolumeToWeight" );
    }
}
