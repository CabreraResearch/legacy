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
        public static IEnumerable<NbtViewRenderingMode> _All { get { return CswEnum<NbtViewRenderingMode>.All; } }
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
        public static IEnumerable<NbtViewAddChildrenSetting> _All { get { return CswEnum<NbtViewAddChildrenSetting>.All; } }
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
        public new static readonly NbtViewAddChildrenSetting All = new NbtViewAddChildrenSetting( "All" );
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means None
        /// </summary>
        public static readonly NbtViewAddChildrenSetting False = new NbtViewAddChildrenSetting( "False" );
    }

    /// <summary>
    /// Type of XML node used in Views
    /// </summary>
    public sealed class CswNbtViewXmlNodeName : CswEnum<CswNbtViewXmlNodeName>
    {
        private CswNbtViewXmlNodeName( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtViewXmlNodeName> _All { get { return CswEnum<CswNbtViewXmlNodeName>.All; } }
        public static explicit operator CswNbtViewXmlNodeName( string str )
        {
            CswNbtViewXmlNodeName ret = Parse( str );
            return ( ret != null ) ? ret : CswNbtViewXmlNodeName.Unknown;
        }
        public static readonly CswNbtViewXmlNodeName Unknown = new CswNbtViewXmlNodeName( "Unknown" );

        /// <summary>
        /// The real Root node of a View
        /// </summary>
        public static readonly CswNbtViewXmlNodeName TreeView = new CswNbtViewXmlNodeName( "TreeView" );
        /// <summary>
        /// A Relationship in the View
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Relationship = new CswNbtViewXmlNodeName( "Relationship" );
        /// <summary>
        /// Group-By
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Group = new CswNbtViewXmlNodeName( "Group" );
        /// <summary>
        /// A Property
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Property = new CswNbtViewXmlNodeName( "Property" );
        /// <summary>
        /// A Property Filter
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Filter = new CswNbtViewXmlNodeName( "Filter" );
        /// <summary>
        /// The FilterMode of a Filter
        /// </summary>
        public static readonly CswNbtViewXmlNodeName FilterMode = new CswNbtViewXmlNodeName( "FilterMode" );
        /// <summary>
        /// And, Or, or And Not for a filter
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Conjunction = new CswNbtViewXmlNodeName( "Conjunction" );
        //public static readonly CswNbtViewXmlNodeName RetrievalType= new CswNbtViewXmlNodeName( "RetrievalType" );
        /// <summary>
        /// The Value of a filter
        /// </summary>
        public static readonly CswNbtViewXmlNodeName Value = new CswNbtViewXmlNodeName( "Value" );
        /// <summary>
        /// Whether the Filter is CaseSensitive
        /// </summary>
        public static readonly CswNbtViewXmlNodeName CaseSensitive = new CswNbtViewXmlNodeName( "CaseSensitive" );
    }

    /// <summary>
    /// Visibility permission setting on a View
    /// </summary>
    public sealed class NbtViewVisibility : CswEnum<NbtViewVisibility>
    {
        private NbtViewVisibility( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewVisibility> _All { get { return CswEnum<NbtViewVisibility>.All; } }
        public static explicit operator NbtViewVisibility( string str )
        {
            NbtViewVisibility ret = Parse( str );
            return ( ret != null ) ? ret : NbtViewVisibility.Unknown;
        }
        public static readonly NbtViewVisibility Unknown = new NbtViewVisibility( "Unknown" );

        /// <summary>
        /// Only one User can use this View
        /// </summary>
        public static readonly NbtViewVisibility User = new NbtViewVisibility( "User" );
        /// <summary>
        /// All Users of a given Role can use this View
        /// </summary>
        public static readonly NbtViewVisibility Role = new NbtViewVisibility( "Role" );
        /// <summary>
        /// All Users can use this View
        /// </summary>
        public static readonly NbtViewVisibility Global = new NbtViewVisibility( "Global" );
        /// <summary>
        /// The View is used by a Property (relationship or grid)
        /// </summary>
        public static readonly NbtViewVisibility Property = new NbtViewVisibility( "Property" );
    }

    /// <summary>
    /// Type of ViewNode
    /// </summary>
    public sealed class NbtViewNodeType : CswEnum<NbtViewNodeType>
    {
        private NbtViewNodeType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewNodeType> _All { get { return CswEnum<NbtViewNodeType>.All; } }
        public static explicit operator NbtViewNodeType( string str )
        {
            NbtViewNodeType ret = Parse( str );
            return ( ret != null ) ? ret : NbtViewNodeType.Unknown;
        }
        public static readonly NbtViewNodeType Unknown = new NbtViewNodeType( "Unknown" );

        /// <summary>
        /// Property
        /// </summary>
        public static readonly NbtViewNodeType CswNbtViewProperty = new NbtViewNodeType( "CswNbtViewProperty" );
        /// <summary>
        /// Property Filter
        /// </summary>
        public static readonly NbtViewNodeType CswNbtViewPropertyFilter = new NbtViewNodeType( "CswNbtViewPropertyFilter" );
        /// <summary>
        /// Relationship
        /// </summary>
        public static readonly NbtViewNodeType CswNbtViewRelationship = new NbtViewNodeType( "CswNbtViewRelationship" );
        /// <summary>
        /// Root
        /// </summary>
        public static readonly NbtViewNodeType CswNbtViewRoot = new NbtViewNodeType( "CswNbtViewRoot" );
    }

    public enum RelatedIdType { Unknown, NodeTypeId, ObjectClassId };
    public enum PropIdType { NodeTypePropId, ObjectClassPropId, Unknown };
    public enum PropOwnerType { First, Second, Unknown };

    public sealed class CswNbtPropType : CswEnum<CswNbtPropType>
    {
        private CswNbtPropType( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtPropType> _All { get { return CswEnum<CswNbtPropType>.All; } }
        public static explicit operator CswNbtPropType( string str )
        {
            CswNbtPropType ret = Parse( str );
            return ( ret != null ) ? ret : CswNbtPropType.Unknown;
        }
        public static readonly CswNbtPropType Unknown = new CswNbtPropType( "Unknown" );

        public static readonly CswNbtPropType NodeTypePropId = new CswNbtPropType( "NodeTypePropId" );
        public static readonly CswNbtPropType ObjectClassPropId = new CswNbtPropType( "ObjectClassPropId" );
    }

    public sealed class PropertySortMethod : CswEnum<PropertySortMethod>
    {
        private PropertySortMethod( string Name ) : base( Name ) { }
        public static IEnumerable<PropertySortMethod> _All { get { return CswEnum<PropertySortMethod>.All; } }
        public static explicit operator PropertySortMethod( string str )
        {
            PropertySortMethod ret = Parse( str );
            return ( ret != null ) ? ret : PropertySortMethod.Unknown;
        }
        public static readonly PropertySortMethod Unknown = new PropertySortMethod( "Unknown" );

        public static readonly PropertySortMethod Ascending = new PropertySortMethod( "Ascending" );
        public static readonly PropertySortMethod Descending = new PropertySortMethod( "Descending" );
    }
    
    #endregion Enums

} // namespace ChemSW.Nbt
