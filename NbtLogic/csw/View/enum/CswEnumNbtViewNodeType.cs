using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Type of ViewNode
    /// </summary>
    public sealed class CswEnumNbtViewNodeType : CswEnum<CswEnumNbtViewNodeType>
    {
        private CswEnumNbtViewNodeType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewNodeType> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewNodeType( string str )
        {
            CswEnumNbtViewNodeType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewNodeType Unknown = new CswEnumNbtViewNodeType( "Unknown" );

        /// <summary>
        /// Property
        /// </summary>
        public static readonly CswEnumNbtViewNodeType CswNbtViewProperty = new CswEnumNbtViewNodeType( "CswNbtViewProperty" );
        /// <summary>
        /// Property Filter
        /// </summary>
        public static readonly CswEnumNbtViewNodeType CswNbtViewPropertyFilter = new CswEnumNbtViewNodeType( "CswNbtViewPropertyFilter" );
        /// <summary>
        /// Relationship
        /// </summary>
        public static readonly CswEnumNbtViewNodeType CswNbtViewRelationship = new CswEnumNbtViewNodeType( "CswNbtViewRelationship" );
        /// <summary>
        /// Root
        /// </summary>
        public static readonly CswEnumNbtViewNodeType CswNbtViewRoot = new CswEnumNbtViewNodeType( "CswNbtViewRoot" );
    }

} // namespace ChemSW.Nbt
