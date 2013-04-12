using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    
    /// <summary>
    /// Options: ObjectClassId, NodeTypeId
    /// </summary>
    public sealed class CswEnumNbtViewRelatedIdType : CswEnum<CswEnumNbtViewRelatedIdType>
    {
        private CswEnumNbtViewRelatedIdType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewRelatedIdType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewRelatedIdType( string str )
        {
            CswEnumNbtViewRelatedIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewRelatedIdType Unknown = new CswEnumNbtViewRelatedIdType( "Unknown" );

        public static readonly CswEnumNbtViewRelatedIdType NodeTypeId = new CswEnumNbtViewRelatedIdType( "NodeTypeId" );
        public static readonly CswEnumNbtViewRelatedIdType ObjectClassId = new CswEnumNbtViewRelatedIdType( "ObjectClassId" );
        public static readonly CswEnumNbtViewRelatedIdType PropertySetId = new CswEnumNbtViewRelatedIdType( "PropertySetId" );
    }

} // namespace ChemSW.Nbt
