using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Search
{
    /// <summary>
    /// Enum: Source for the order of a property in a search
    /// </summary>
    public sealed class CswEnumNbtSearchPropOrderSourceType : CswEnum<CswEnumNbtSearchPropOrderSourceType>
    {
        private CswEnumNbtSearchPropOrderSourceType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtSearchPropOrderSourceType> _All { get { return All; } }

        public static explicit operator CswEnumNbtSearchPropOrderSourceType( string str )
        {
            CswEnumNbtSearchPropOrderSourceType ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswEnumNbtSearchPropOrderSourceType Unknown = new CswEnumNbtSearchPropOrderSourceType( "Unknown" );
        public static readonly CswEnumNbtSearchPropOrderSourceType View = new CswEnumNbtSearchPropOrderSourceType( "View" );
        public static readonly CswEnumNbtSearchPropOrderSourceType Table = new CswEnumNbtSearchPropOrderSourceType( "Table" );
        public static readonly CswEnumNbtSearchPropOrderSourceType Results = new CswEnumNbtSearchPropOrderSourceType( "Results" );
    }

} // namespace ChemSW.Nbt



