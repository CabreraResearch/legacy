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
        public static IEnumerable<NbtViewRenderingMode> _All { get { return All; } }
        public static implicit operator NbtViewRenderingMode( string str )
        {
            NbtViewRenderingMode ret = Parse( str );
            return ret ?? Unknown;
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
        public static implicit operator NbtViewAddChildrenSetting( string str )
        {
            NbtViewAddChildrenSetting ret = Parse( str );
            return ret ?? Unknown;
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
    public sealed class NbtViewXmlNodeName : CswEnum<NbtViewXmlNodeName>
    {
        private NbtViewXmlNodeName( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewXmlNodeName> _All { get { return All; } }
        public static implicit operator NbtViewXmlNodeName( string str )
        {
            NbtViewXmlNodeName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewXmlNodeName Unknown = new NbtViewXmlNodeName( "Unknown" );

        /// <summary>
        /// The real Root node of a View
        /// </summary>
        public static readonly NbtViewXmlNodeName TreeView = new NbtViewXmlNodeName( "TreeView" );
        /// <summary>
        /// A Relationship in the View
        /// </summary>
        public static readonly NbtViewXmlNodeName Relationship = new NbtViewXmlNodeName( "Relationship" );
        /// <summary>
        /// Group-By
        /// </summary>
        public static readonly NbtViewXmlNodeName Group = new NbtViewXmlNodeName( "Group" );
        /// <summary>
        /// A Property
        /// </summary>
        public static readonly NbtViewXmlNodeName Property = new NbtViewXmlNodeName( "Property" );
        /// <summary>
        /// A Property Filter
        /// </summary>
        public static readonly NbtViewXmlNodeName Filter = new NbtViewXmlNodeName( "Filter" );
        /// <summary>
        /// The FilterMode of a Filter
        /// </summary>
        public static readonly NbtViewXmlNodeName FilterMode = new NbtViewXmlNodeName( "FilterMode" );
        /// <summary>
        /// And, Or, or And Not for a filter
        /// </summary>
        public static readonly NbtViewXmlNodeName Conjunction = new NbtViewXmlNodeName( "Conjunction" );
        //public static readonly CswNbtViewXmlNodeName RetrievalType= new CswNbtViewXmlNodeName( "RetrievalType" );
        /// <summary>
        /// The Value of a filter
        /// </summary>
        public static readonly NbtViewXmlNodeName Value = new NbtViewXmlNodeName( "Value" );
        /// <summary>
        /// Whether the Filter is CaseSensitive
        /// </summary>
        public static readonly NbtViewXmlNodeName CaseSensitive = new NbtViewXmlNodeName( "CaseSensitive" );
    }

    /// <summary>
    /// Visibility permission setting on a View
    /// </summary>
    public sealed class NbtViewVisibility : CswEnum<NbtViewVisibility>
    {
        private NbtViewVisibility( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewVisibility> _All { get { return All; } }
        public static explicit operator NbtViewVisibility( string str )
        {
            NbtViewVisibility ret = Parse( str );
            return ret ?? Unknown;
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
        /// <summary>
        /// The View is used internally
        /// </summary>
        public static readonly NbtViewVisibility Hidden = new NbtViewVisibility( "Hidden" );
    }

    /// <summary>
    /// Type of ViewNode
    /// </summary>
    public sealed class NbtViewNodeType : CswEnum<NbtViewNodeType>
    {
        private NbtViewNodeType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewNodeType> _All { get { return All; } }
        public static implicit operator NbtViewNodeType( string str )
        {
            NbtViewNodeType ret = Parse( str );
            return ret ?? Unknown;
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

    /// <summary>
    /// Options: ObjectClassId, NodeTypeId
    /// </summary>
    public sealed class NbtViewRelatedIdType : CswEnum<NbtViewRelatedIdType>
    {
        private NbtViewRelatedIdType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewRelatedIdType> _All { get { return All; } }
        public static implicit operator NbtViewRelatedIdType( string str )
        {
            NbtViewRelatedIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewRelatedIdType Unknown = new NbtViewRelatedIdType( "Unknown" );

        public static readonly NbtViewRelatedIdType NodeTypeId = new NbtViewRelatedIdType( "NodeTypeId" );
        public static readonly NbtViewRelatedIdType ObjectClassId = new NbtViewRelatedIdType( "ObjectClassId" );
    }

    /// <summary>
    /// Options: NodeTypePropId, ObjectClassPropId
    /// </summary>
    public sealed class NbtViewPropIdType : CswEnum<NbtViewPropIdType>
    {
        private NbtViewPropIdType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewPropIdType> _All { get { return All; } }
        public static implicit operator NbtViewPropIdType( string str )
        {
            NbtViewPropIdType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewPropIdType Unknown = new NbtViewPropIdType( "Unknown" );

        public static readonly NbtViewPropIdType NodeTypePropId = new NbtViewPropIdType( "NodeTypePropId" );
        public static readonly NbtViewPropIdType ObjectClassPropId = new NbtViewPropIdType( "ObjectClassPropId" );
    }

    /// <summary>
    /// Options: First, Second
    /// </summary>
    public sealed class NbtViewPropOwnerType : CswEnum<NbtViewPropOwnerType>
    {
        private NbtViewPropOwnerType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewPropOwnerType> _All { get { return All; } }
        public static implicit operator NbtViewPropOwnerType( string str )
        {
            NbtViewPropOwnerType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewPropOwnerType Unknown = new NbtViewPropOwnerType( "Unknown" );

        public static readonly NbtViewPropOwnerType First = new NbtViewPropOwnerType( "First" );
        public static readonly NbtViewPropOwnerType Second = new NbtViewPropOwnerType( "Second" );
    }

    /// <summary>
    /// Options: NodeTypePropId, ObjectClassPropId
    /// </summary>
    public sealed class NbtViewPropType : CswEnum<NbtViewPropType>
    {
        private NbtViewPropType( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewPropType> _All { get { return All; } }
        public static implicit operator NbtViewPropType( string str )
        {
            NbtViewPropType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewPropType Unknown = new NbtViewPropType( "Unknown" );

        public static readonly NbtViewPropType NodeTypePropId = new NbtViewPropType( "NodeTypePropId" );
        public static readonly NbtViewPropType ObjectClassPropId = new NbtViewPropType( "ObjectClassPropId" );
    }

    /// <summary>
    /// Options: Ascending, Descending
    /// </summary>
    public sealed class NbtViewPropertySortMethod : CswEnum<NbtViewPropertySortMethod>
    {
        private NbtViewPropertySortMethod( string Name ) : base( Name ) { }
        public static IEnumerable<NbtViewPropertySortMethod> _All { get { return All; } }
        public static implicit operator NbtViewPropertySortMethod( string str )
        {
            NbtViewPropertySortMethod ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtViewPropertySortMethod Unknown = new NbtViewPropertySortMethod( "Unknown" );

        public static readonly NbtViewPropertySortMethod Ascending = new NbtViewPropertySortMethod( "Ascending" );
        public static readonly NbtViewPropertySortMethod Descending = new NbtViewPropertySortMethod( "Descending" );
    }

    #endregion Enums

} // namespace ChemSW.Nbt
