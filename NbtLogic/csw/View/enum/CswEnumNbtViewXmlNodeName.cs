using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Type of XML node used in Views
    /// </summary>
    public sealed class CswEnumNbtViewXmlNodeName : CswEnum<CswEnumNbtViewXmlNodeName>
    {
        private CswEnumNbtViewXmlNodeName( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewXmlNodeName> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewXmlNodeName( string str )
        {
            CswEnumNbtViewXmlNodeName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewXmlNodeName Unknown = new CswEnumNbtViewXmlNodeName( "Unknown" );

        /// <summary>
        /// The real Root node of a View
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName TreeView = new CswEnumNbtViewXmlNodeName( "TreeView" );
        /// <summary>
        /// A Relationship in the View
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Relationship = new CswEnumNbtViewXmlNodeName( "Relationship" );
        /// <summary>
        /// Group-By
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Group = new CswEnumNbtViewXmlNodeName( "Group" );
        /// <summary>
        /// A Property
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Property = new CswEnumNbtViewXmlNodeName( "Property" );
        /// <summary>
        /// A Property Filter
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Filter = new CswEnumNbtViewXmlNodeName( "Filter" );
        /// <summary>
        /// The FilterMode of a Filter
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName FilterMode = new CswEnumNbtViewXmlNodeName( "FilterMode" );
        /// <summary>
        /// And, Or, or And Not for a filter
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Conjunction = new CswEnumNbtViewXmlNodeName( "Conjunction" );
        //public static readonly CswNbtViewXmlNodeName RetrievalType= new CswNbtViewXmlNodeName( "RetrievalType" );
        /// <summary>
        /// The Value of a filter
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName Value = new CswEnumNbtViewXmlNodeName( "Value" );
        /// <summary>
        /// Whether the Filter is CaseSensitive
        /// </summary>
        public static readonly CswEnumNbtViewXmlNodeName CaseSensitive = new CswEnumNbtViewXmlNodeName( "CaseSensitive" );
    }

} // namespace ChemSW.Nbt
