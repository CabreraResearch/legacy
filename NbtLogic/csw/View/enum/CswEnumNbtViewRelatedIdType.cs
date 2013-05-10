using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    
    /// <summary>
    /// Options: ObjectClassId, NodeTypeId
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewRelatedIdType : CswEnum<CswEnumNbtViewRelatedIdType>
    {
        private CswEnumNbtViewRelatedIdType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewRelatedIdType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewRelatedIdType( string str )
        {
            CswEnumNbtViewRelatedIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        [DataMember]
        public static readonly CswEnumNbtViewRelatedIdType Unknown = new CswEnumNbtViewRelatedIdType( "Unknown" );

        [DataMember]
        public static readonly CswEnumNbtViewRelatedIdType NodeTypeId = new CswEnumNbtViewRelatedIdType( "NodeTypeId" );
        [DataMember]
        public static readonly CswEnumNbtViewRelatedIdType ObjectClassId = new CswEnumNbtViewRelatedIdType( "ObjectClassId" );
        [DataMember]
        public static readonly CswEnumNbtViewRelatedIdType PropertySetId = new CswEnumNbtViewRelatedIdType( "PropertySetId" );
    }

} // namespace ChemSW.Nbt
