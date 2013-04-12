using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: First, Second
    /// </summary>
    public sealed class CswEnumNbtViewPropOwnerType : CswEnum<CswEnumNbtViewPropOwnerType>
    {
        private CswEnumNbtViewPropOwnerType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropOwnerType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropOwnerType( string str )
        {
            CswEnumNbtViewPropOwnerType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewPropOwnerType Unknown = new CswEnumNbtViewPropOwnerType( "Unknown" );

        public static readonly CswEnumNbtViewPropOwnerType First = new CswEnumNbtViewPropOwnerType( "First" );
        public static readonly CswEnumNbtViewPropOwnerType Second = new CswEnumNbtViewPropOwnerType( "Second" );
    }

} // namespace ChemSW.Nbt
