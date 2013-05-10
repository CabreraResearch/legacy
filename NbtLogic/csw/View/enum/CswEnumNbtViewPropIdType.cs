using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: NodeTypePropId, ObjectClassPropId
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewPropIdType : CswEnum<CswEnumNbtViewPropIdType>
    {
        private CswEnumNbtViewPropIdType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropIdType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropIdType( string str )
        {
            CswEnumNbtViewPropIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        [DataMember]
        public static readonly CswEnumNbtViewPropIdType Unknown = new CswEnumNbtViewPropIdType( "Unknown" );

        [DataMember]
        public static readonly CswEnumNbtViewPropIdType NodeTypePropId = new CswEnumNbtViewPropIdType( "NodeTypePropId" );
        [DataMember]
        public static readonly CswEnumNbtViewPropIdType ObjectClassPropId = new CswEnumNbtViewPropIdType( "ObjectClassPropId" );
    }
} // namespace ChemSW.Nbt
