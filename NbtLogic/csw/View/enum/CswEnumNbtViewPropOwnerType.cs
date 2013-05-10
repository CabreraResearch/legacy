using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Options: First, Second
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewPropOwnerType : CswEnum<CswEnumNbtViewPropOwnerType>
    {
        private CswEnumNbtViewPropOwnerType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewPropOwnerType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewPropOwnerType( string str )
        {
            CswEnumNbtViewPropOwnerType ret = Parse( str );
            return ret ?? Unknown;
        }
        [DataMember]
        public static readonly CswEnumNbtViewPropOwnerType Unknown = new CswEnumNbtViewPropOwnerType( "Unknown" );

        [DataMember]
        public static readonly CswEnumNbtViewPropOwnerType First = new CswEnumNbtViewPropOwnerType( "First" );
        [DataMember]
        public static readonly CswEnumNbtViewPropOwnerType Second = new CswEnumNbtViewPropOwnerType( "Second" );
    }

} // namespace ChemSW.Nbt
