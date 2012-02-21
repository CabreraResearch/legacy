using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    #region Enums

    /// <summary>
    /// View Rendering Mode
    /// </summary>
    public sealed class NbtViewRenderingMode : CswEnum<NbtViewRenderingMode>
    {
        private NbtViewRenderingMode( string Name ) : base( Name ) { }
        public new static IEnumerable<NbtViewRenderingMode> _All { get { return CswEnum<NbtViewRenderingMode>.All; } }
        public static explicit operator NbtViewRenderingMode( string str )
        {
            NbtViewRenderingMode ret = Parse( str );
            return ( ret != null ) ? ret : NbtViewRenderingMode.Unknown;
        }
        public static readonly NbtViewRenderingMode Unknown = new NbtViewRenderingMode( "Unknown" );

        public static readonly NbtViewRenderingMode Tree = new NbtViewRenderingMode( "Tree" );
        public static readonly NbtViewRenderingMode Grid = new NbtViewRenderingMode( "Grid" );
        public static readonly NbtViewRenderingMode List = new NbtViewRenderingMode( "List" );
        public static readonly NbtViewRenderingMode Table = new NbtViewRenderingMode( "Table" );
        public static readonly NbtViewRenderingMode Any = new NbtViewRenderingMode( "Any" );
    }

    /// <summary>
    /// View permissions concerning adding new nodes
    /// </summary>
    public sealed class NbtViewAddChildrenSetting : CswEnum<NbtViewAddChildrenSetting>
    {
        private NbtViewAddChildrenSetting( string Name ) : base( Name ) { }
        public new static IEnumerable<NbtViewAddChildrenSetting> _All { get { return CswEnum<NbtViewAddChildrenSetting>.All; } }
        public static explicit operator NbtViewAddChildrenSetting( string str )
        {
            NbtViewAddChildrenSetting ret = Parse( str );
            return ( ret != null ) ? ret : NbtViewAddChildrenSetting.Unknown;
        }
        public static readonly NbtViewAddChildrenSetting Unknown = new NbtViewAddChildrenSetting( "Unknown" );

        /// <summary>
        /// No children can be added
        /// </summary>
        public static readonly NbtViewAddChildrenSetting None = new NbtViewAddChildrenSetting( "None" );
        /// <summary>
        /// Only nodetypes defined in this view can be added as children
        /// </summary>
        public static readonly NbtViewAddChildrenSetting InView = new NbtViewAddChildrenSetting( "InView" );
        /// <summary>
        /// DEPRECATED Any child nodetype can be added to any nodetype
        /// </summary>
        public static readonly NbtViewAddChildrenSetting True = new NbtViewAddChildrenSetting( "True" );
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means InView
        /// </summary>
        public static readonly NbtViewAddChildrenSetting All = new NbtViewAddChildrenSetting( "All" );
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means None
        /// </summary>
        public static readonly NbtViewAddChildrenSetting False = new NbtViewAddChildrenSetting( "False" );
    }

    /// <summary>
    /// Type of XML node used in Views
    /// </summary>
    public enum CswNbtViewXmlNodeName
    {
        /// <summary>
        /// The real Root node of a View
        /// </summary>
        TreeView,
        /// <summary>
        /// A Relationship in the View
        /// </summary>
        Relationship,
        /// <summary>
        /// Group-By
        /// </summary>
        Group,
        /// <summary>
        /// A Property
        /// </summary>
        Property,
        /// <summary>
        /// A Property Filter
        /// </summary>
        Filter,
        /// <summary>
        /// The FilterMode of a Filter
        /// </summary>
        FilterMode,
        /// <summary>
        /// And, Or, or And Not for a filter
        /// </summary>
        Conjunction,
        //RetrievalType,
        /// <summary>
        /// The Value of a filter
        /// </summary>
        Value,
        /// <summary>
        /// Whether the Filter is CaseSensitive
        /// </summary>
        CaseSensitive,
        /// <summary>
        /// Unknown ViewNode 
        /// </summary>
        Unknown
    };
    /// <summary>
    /// Visibility permission setting on a View
    /// </summary>
    public enum NbtViewVisibility
    {
        /// <summary>
        /// Only one User can use this View
        /// </summary>
        User,
        /// <summary>
        /// All Users of a given Role can use this View
        /// </summary>
        Role,
        /// <summary>
        /// All Users can use this View
        /// </summary>
        Global,
        /// <summary>
        /// The View is used by a Property (relationship or grid)
        /// </summary>
        Property,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    };


    /// <summary>
    /// Type of ViewNode
    /// </summary>
    public enum NbtViewNodeType
    {
        /// <summary>
        /// Property
        /// </summary>
        CswNbtViewProperty,
        /// <summary>
        /// Property Filter
        /// </summary>
        CswNbtViewPropertyFilter,
        /// <summary>
        /// Relationship
        /// </summary>
        CswNbtViewRelationship,
        /// <summary>
        /// Root
        /// </summary>
        CswNbtViewRoot
    }

    #endregion Enums

} // namespace ChemSW.Nbt
