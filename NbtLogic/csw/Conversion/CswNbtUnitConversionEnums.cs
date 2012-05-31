using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;

namespace ChemSW.Nbt.csw.Conversion
{
    public class CswNbtUnitConversionEnums
    {
        /// <summary>
        /// Enum: Used to identify the UnitType relationship between two UnitOfMeasure Nodes in order to apply the correct conversion logic
        /// </summary>
        public sealed class UnitTypeRelationship : CswEnum<UnitTypeRelationship>
        {
            private UnitTypeRelationship( string Name ) : base( Name ) { }
            public static IEnumerable<UnitTypeRelationship> _All { get { return CswEnum<UnitTypeRelationship>.All; } }

            public static explicit operator UnitTypeRelationship( string str )
            {
                UnitTypeRelationship ret = Parse( str );
                return ( ret != null ) ? ret : UnitTypeRelationship.Unknown;
            }

            public static readonly UnitTypeRelationship Unknown = new UnitTypeRelationship( "Unknown" );
            public static readonly UnitTypeRelationship Same = new UnitTypeRelationship( "Same" );
            public static readonly UnitTypeRelationship NotSupported = new UnitTypeRelationship( "NotSupported" );
            public static readonly UnitTypeRelationship WeightToVolume = new UnitTypeRelationship( "WeightToVolume" );
            public static readonly UnitTypeRelationship VolumeToWeight = new UnitTypeRelationship( "VolumeToWeight" );
        }
    }
}
