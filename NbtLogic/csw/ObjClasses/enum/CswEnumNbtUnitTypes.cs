using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{

    /// <summary>
    /// Enum: Used to identify the UnitType of a UnitOfMeasure Node/NodeType in order to apply correct unit conversion logic
    /// </summary>
    public sealed class CswEnumNbtUnitTypes : CswEnum<CswEnumNbtUnitTypes>
    {
        private CswEnumNbtUnitTypes( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtUnitTypes> _All { get { return All; } }

        public static explicit operator CswEnumNbtUnitTypes( string str )
        {
            CswEnumNbtUnitTypes ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswEnumNbtUnitTypes Unknown = new CswEnumNbtUnitTypes( "Unknown" );
        public static readonly CswEnumNbtUnitTypes Weight = new CswEnumNbtUnitTypes( "Weight" );
        public static readonly CswEnumNbtUnitTypes Volume = new CswEnumNbtUnitTypes( "Volume" );
        public static readonly CswEnumNbtUnitTypes Time = new CswEnumNbtUnitTypes( "Time" );
        public static readonly CswEnumNbtUnitTypes Each = new CswEnumNbtUnitTypes( "Each" );
        public static readonly CswEnumNbtUnitTypes Radiation = new CswEnumNbtUnitTypes( "Radiation" );
    }

}//namespace ChemSW.Nbt.ObjClasses
