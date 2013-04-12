using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Search
{

    public sealed class CswEnumNbtSearchFilterType : CswEnum<CswEnumNbtSearchFilterType>
    {
        private CswEnumNbtSearchFilterType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtSearchFilterType> _All { get { return CswEnum<CswEnumNbtSearchFilterType>.All; } }
        public static explicit operator CswEnumNbtSearchFilterType( string str )
        {
            CswEnumNbtSearchFilterType ret = Parse( str );
            return ( ret != null ) ? ret : CswEnumNbtSearchFilterType.Unknown;
        }
        public static readonly CswEnumNbtSearchFilterType Unknown = new CswEnumNbtSearchFilterType( "Unknown" );

        public static readonly CswEnumNbtSearchFilterType nodetype = new CswEnumNbtSearchFilterType( "nodetype" );
        public static readonly CswEnumNbtSearchFilterType objectclass = new CswEnumNbtSearchFilterType( "objectclass" );
        public static readonly CswEnumNbtSearchFilterType propval = new CswEnumNbtSearchFilterType( "propval" );
    }

} // namespace ChemSW.Nbt.Search

