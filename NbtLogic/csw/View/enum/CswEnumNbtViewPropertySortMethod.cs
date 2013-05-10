using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: Ascending, Descending
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewPropertySortMethod : CswEnum<CswEnumNbtViewPropertySortMethod>
    {
        private CswEnumNbtViewPropertySortMethod( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropertySortMethod> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropertySortMethod( string str )
        {
            CswEnumNbtViewPropertySortMethod ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewPropertySortMethod Unknown = new CswEnumNbtViewPropertySortMethod( "Unknown" );

        public static readonly CswEnumNbtViewPropertySortMethod Ascending = new CswEnumNbtViewPropertySortMethod( "Ascending" );
        public static readonly CswEnumNbtViewPropertySortMethod Descending = new CswEnumNbtViewPropertySortMethod( "Descending" );
    }

} // namespace ChemSW.Nbt
