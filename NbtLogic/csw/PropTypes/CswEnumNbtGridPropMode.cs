using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Indicates the mode of grid to render
    /// </summary>
    public sealed class CswEnumNbtGridPropMode : CswEnum<CswEnumNbtGridPropMode>
    {
        private CswEnumNbtGridPropMode( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtGridPropMode> all { get { return All; } }
        public static explicit operator CswEnumNbtGridPropMode( string Str )
        {
            CswEnumNbtGridPropMode Ret = Parse( Str );
            return Ret ?? Unknown;
        }
        public static readonly CswEnumNbtGridPropMode Unknown = new CswEnumNbtGridPropMode( "Unknown" );
        public static readonly CswEnumNbtGridPropMode Full = new CswEnumNbtGridPropMode( "Full" );
        public static readonly CswEnumNbtGridPropMode Small = new CswEnumNbtGridPropMode( "Small" );
        public static readonly CswEnumNbtGridPropMode Link = new CswEnumNbtGridPropMode( "Link" );
    }

}//namespace ChemSW.Nbt.PropTypes
