
namespace ChemSW.Nbt
{
    #region Enums

    /// <summary>
    /// View Rendering Mode
    /// </summary>
    public enum NbtViewRenderingMode
    {
        /// <summary>
        /// The View should be rendered as a Tree
        /// </summary>
        Tree,
        /// <summary>
        /// The View should be rendered as a Grid
        /// </summary>
        Grid,
        /// <summary>
        /// The View should be rendered as a List
        /// </summary>
        List,
        /// <summary>
        /// Unknown rendering mode
        /// </summary>
        Unknown,
        /// <summary>
        /// Any View render mode
        /// </summary>
        Any
    };

    /// <summary>
    /// View permissions concerning adding new nodes
    /// </summary>
    public enum NbtViewAddChildrenSetting
    {
        /// <summary>
        /// No children can be added
        /// </summary>
        None,
        /// <summary>
        /// Only nodetypes defined in this view can be added as children
        /// </summary>
        InView,
        /// <summary>
        /// DEPRECATED Any child nodetype can be added to any nodetype
        /// </summary>
        All,
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means InView
        /// </summary>
        True,
        /// <summary>
        /// DEPRECATED For backwards-compatibility, this means None
        /// </summary>
        False
    };

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
