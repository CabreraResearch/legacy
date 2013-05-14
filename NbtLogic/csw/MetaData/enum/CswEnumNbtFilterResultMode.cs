using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Indicates how to treat results that are filtered out
    /// </summary>
    public sealed class CswEnumNbtFilterResultMode : CswEnum<CswEnumNbtFilterResultMode>
    {
        private CswEnumNbtFilterResultMode( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtFilterResultMode> _All { get { return All; } }
        public static explicit operator CswEnumNbtFilterResultMode( string str )
        {
            CswEnumNbtFilterResultMode ret = Parse( str );
            return ( ret != null ) ? ret : CswEnumNbtFilterResultMode.Unknown;
        }
        public static readonly CswEnumNbtFilterResultMode Unknown = new CswEnumNbtFilterResultMode( "Unknown" );

        public static readonly CswEnumNbtFilterResultMode Hide = new CswEnumNbtFilterResultMode( "Hide" );
        public static readonly CswEnumNbtFilterResultMode Disabled = new CswEnumNbtFilterResultMode( "Disabled" );
    }

}//namespace ChemSW.Nbt.MetaData
