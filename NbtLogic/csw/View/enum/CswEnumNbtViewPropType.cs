using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: NodeTypePropId, ObjectClassPropId
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewPropType : CswEnum<CswEnumNbtViewPropType>
    {
        private CswEnumNbtViewPropType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropType( string str )
        {
            CswEnumNbtViewPropType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewPropType Unknown = new CswEnumNbtViewPropType( "Unknown" );

        public static readonly CswEnumNbtViewPropType NodeTypePropId = new CswEnumNbtViewPropType( "NodeTypePropId" );
        public static readonly CswEnumNbtViewPropType ObjectClassPropId = new CswEnumNbtViewPropType( "ObjectClassPropId" );
    }

} // namespace ChemSW.Nbt
