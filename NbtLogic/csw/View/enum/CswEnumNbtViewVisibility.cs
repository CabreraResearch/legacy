using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Visibility permission setting on a View
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewVisibility : CswEnum<CswEnumNbtViewVisibility>
    {
        private CswEnumNbtViewVisibility( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewVisibility> _All { get { return All; } }
        public static explicit operator CswEnumNbtViewVisibility( string str )
        {
            CswEnumNbtViewVisibility ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewVisibility Unknown = new CswEnumNbtViewVisibility( "Unknown" );

        /// <summary>
        /// Only one User can use this View
        /// </summary>
        public static readonly CswEnumNbtViewVisibility User = new CswEnumNbtViewVisibility( "User" );
        /// <summary>
        /// All Users of a given Role can use this View
        /// </summary>
        public static readonly CswEnumNbtViewVisibility Role = new CswEnumNbtViewVisibility( "Role" );
        /// <summary>
        /// All Users can use this View
        /// </summary>
        public static readonly CswEnumNbtViewVisibility Global = new CswEnumNbtViewVisibility( "Global" );
        /// <summary>
        /// The View is used by a Property (relationship or grid)
        /// </summary>
        public static readonly CswEnumNbtViewVisibility Property = new CswEnumNbtViewVisibility( "Property" );
        /// <summary>
        /// The View is used internally
        /// </summary>
        public static readonly CswEnumNbtViewVisibility Hidden = new CswEnumNbtViewVisibility( "Hidden" );
    }

} // namespace ChemSW.Nbt
