using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: NodeTypePropId, ObjectClassPropId
    /// </summary>
    public sealed class CswEnumNbtViewPropIdType : CswEnum<CswEnumNbtViewPropIdType>
    {
        private CswEnumNbtViewPropIdType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropIdType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropIdType( string str )
        {
            CswEnumNbtViewPropIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewPropIdType Unknown = new CswEnumNbtViewPropIdType( "Unknown" );

        public static readonly CswEnumNbtViewPropIdType NodeTypePropId = new CswEnumNbtViewPropIdType( "NodeTypePropId" );
        public static readonly CswEnumNbtViewPropIdType ObjectClassPropId = new CswEnumNbtViewPropIdType( "ObjectClassPropId" );
    }
} // namespace ChemSW.Nbt
