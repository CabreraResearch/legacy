using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    #region Enums

    ///// <summary>
    ///// Type of View
    ///// </summary>
    //public enum NbtViewType
    //{
    //    /// <summary>
    //    /// "Typical" view built on relationships and properties
    //    /// </summary>
    //    RelationshipView,
    //    /// <summary>
    //    /// Quick Search
    //    /// </summary>
    //    QuickSearch,
    //    /// <summary>
    //    /// Action Placeholder
    //    /// </summary>
    //    Action
    //};

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
        Unknown
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

    ///// <summary>
    ///// Represents an NBT View (RelationshipView, QuickSearch, or Action)
    ///// </summary>
    //public abstract class CswNbtViewBase
    //{
    //    /// <summary>
    //    /// Character delimiter used for saving the view as a string
    //    /// </summary>
    //    public static char delimiter = '|';

    //    /// <summary>
    //    /// Type of View
    //    /// </summary>
    //    public abstract NbtViewType ViewType { get; }
    //    /// <summary>
    //    /// Name of View
    //    /// </summary>
    //    public abstract string ViewName { get; set; }
    //    /// <summary>
    //    /// Category for View
    //    /// </summary>
    //    public abstract string Category { get; }
    //    /// <summary>
    //    /// Rendering Mode for View
    //    /// </summary>
    //    public abstract NbtViewRenderingMode ViewMode { get; }

    //    private Int32 _SessionViewId = Int32.MinValue;
    //    /// <summary>
    //    /// Key for retrieving the view from the Session's View Cache
    //    /// </summary>
    //    public Int32 SessionViewId
    //    {
    //        get { return _SessionViewId; }
    //        //set { _SessionViewId = value; }
    //    }

    //    /// <summary>
    //    /// Save this view as a string
    //    /// </summary>
    //    public abstract override string ToString();
    //    /// <summary>
    //    /// Clear all settings from View and restore to default
    //    /// </summary>
    //    public abstract void Clear();

    //    /// <summary>
    //    /// CswNbtResources reference
    //    /// </summary>
    //    protected CswNbtResources _CswNbtResources;

    //    /// <summary>
    //    /// Creator
    //    /// </summary>
    //    public CswNbtViewBase(CswNbtResources CswNbtResources, NbtViewType ViewType)
    //    {
    //        _CswNbtResources = CswNbtResources;
    //        //SessionViewId = _CswNbtResources.ViewCache.putView(this);
    //    }

    //    /// <summary>
    //    /// Save this View to Session's ViewCache
    //    /// </summary>
    //    public void SaveToCache()
    //    {
    //        _SessionViewId = _CswNbtResources.ViewCache.putView(this);
    //    }
    //    //public void UpdateCache()
    //    //{
    //    //    _CswNbtResources.ViewCache.updateCachedView(_SessionViewId, this);
    //    //}

    //    /// <summary>
    //    /// Clears the view from the cache.  Useful for saving changes to views.
    //    /// </summary>
    //    public void ClearFromCache()
    //    {
    //        _CswNbtResources.ViewCache.clearFromCache(this);
    //    }

    //} // interface ICswNbtView
} // namespace ChemSW.Nbt
