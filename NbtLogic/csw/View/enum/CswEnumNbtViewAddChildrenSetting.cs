using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// View permissions concerning adding new nodes
    /// </summary>
    public sealed class CswEnumNbtViewAddChildrenSetting : CswEnum<CswEnumNbtViewAddChildrenSetting>
    {
        private CswEnumNbtViewAddChildrenSetting( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewAddChildrenSetting> _All { get { return CswEnum<CswEnumNbtViewAddChildrenSetting>.All; } }
        public static implicit operator CswEnumNbtViewAddChildrenSetting( string str )
        {
            CswEnumNbtViewAddChildrenSetting ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewAddChildrenSetting Unknown = new CswEnumNbtViewAddChildrenSetting( "Unknown" );

        /// <summary>
        /// No children can be added
        /// </summary>
        public static readonly CswEnumNbtViewAddChildrenSetting None = new CswEnumNbtViewAddChildrenSetting( "None" );
        /// <summary>
        /// Only nodetypes defined in this view can be added as children
        /// </summary>
        public static readonly CswEnumNbtViewAddChildrenSetting InView = new CswEnumNbtViewAddChildrenSetting( "InView" );
        /// <summary>
        /// DEPRECATED Any child nodetype can be added to any nodetype
        /// </summary>
        public static readonly CswEnumNbtViewAddChildrenSetting True = new CswEnumNbtViewAddChildrenSetting( "True" );
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means InView
        /// </summary>
        public new static readonly CswEnumNbtViewAddChildrenSetting All = new CswEnumNbtViewAddChildrenSetting( "All" );
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means None
        /// </summary>
        public static readonly CswEnumNbtViewAddChildrenSetting False = new CswEnumNbtViewAddChildrenSetting( "False" );
    }

} // namespace ChemSW.Nbt
