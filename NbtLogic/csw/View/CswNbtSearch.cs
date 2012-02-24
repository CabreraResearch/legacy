using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents a Universal Search
    /// </summary>
    [Serializable()]
    public class CswNbtSearch : IEquatable<CswNbtSearch>
    {
        /// <summary>
        /// CswNbtResources reference
        /// </summary>
        protected CswNbtResources _CswNbtResources;

        /// <summary>
        /// Constructor - new search
        /// </summary>
        public CswNbtSearch( CswNbtResources CswNbtResources, string SearchTerm )
        {
            _CswNbtResources = CswNbtResources;

            _SearchObj = new JObject();
            _SearchObj["searchterm"] = SearchTerm;
            _SearchObj["filters"] = new JArray();
        }

        /// <summary>
        /// Constructor - from session data
        /// </summary>
        public CswNbtSearch( CswNbtResources CswNbtResources, DataRow SessionDataRow )
        {
            _CswNbtResources = CswNbtResources;
            _SearchObj = JObject.Parse( SessionDataRow["viewxml"].ToString() );
            SessionDataId = new CswNbtSessionDataId( CswConvert.ToInt32( SessionDataRow["sessiondataid"] ) );
        }

        private JObject _SearchObj;

        public JArray FiltersApplied { get { return (JArray) _SearchObj["filters"]; } }
        private string _SearchTerm { get { return _SearchObj["searchterm"].ToString(); } }

        /// <summary>
        /// A display name for the search
        /// </summary>
        public string Name
        {
            get
            {
                return "Search for: " + _SearchTerm;
            }
        }

        public override string ToString()
        {
            return _SearchObj.ToString();
        }

        public bool IsSingleNodeType()
        {
            bool ret = false;
            foreach( JObject FilterObj in FiltersApplied )
            {
                CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterObj );
                if( Filter.Type == CswNbtSearchFilterType.nodetype )
                {
                    ret = true;
                }
            }
            return ret;
        } // IsSingleNodeType()

        private Collection<Int32> _FilteredPropIds = null;
        public Collection<Int32> getFilteredPropIds()
        {
            if( _FilteredPropIds == null )
            {
                _FilteredPropIds = new Collection<Int32>();
                foreach( JObject FilterObj in FiltersApplied )
                {
                    CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterObj );
                    if( Filter.Type == CswNbtSearchFilterType.propval )
                    {
                        _FilteredPropIds.Add( Filter.FirstPropVersionId );
                    }
                } // foreach(JObject FilterObj in FiltersApplied)
            }
            return _FilteredPropIds;
        } // getFilteredPropIds()

        #region Search Functions

        public void addFilter( JObject Filter )
        {
            FiltersApplied.Add( Filter );
            _FilteredPropIds = null;
        }

        public void removeFilter( JObject Filter )
        {
            FiltersApplied.Remove( Filter );
            _FilteredPropIds = null;
        }

        public ICswNbtTree Results()
        {
            // Filters to apply
            string WhereClause = string.Empty;
            //bool SingleNodeType = false;
            //Collection<Int32> FilteredPropIds = new Collection<Int32>();
            foreach( JObject FilterObj in FiltersApplied )
            {
                CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterObj );

                if( Filter.Type == CswNbtSearchFilterType.nodetype )
                {
                    // NodeType filter
                    Int32 NodeTypeFirstVersionId = Filter.FirstVersionId;
                    if( NodeTypeFirstVersionId != Int32.MinValue )
                    {
                        WhereClause += " and t.nodetypeid in (select nodetypeid from nodetypes where firstversionid = " + NodeTypeFirstVersionId.ToString() + @") ";
                        //SingleNodeType = true;
                    }
                }
                else if( Filter.Type == CswNbtSearchFilterType.propval )
                {
                    // Property Filter
                    // Someday we may need to do this in a view instead
                    Int32 NodeTypePropFirstVersionId = Filter.FirstPropVersionId;
                    string FilterStr = Filter.FilterValue;
                    if( FilterStr == CswNbtSearchFilterWrapper.BlankValue )
                    {
                        FilterStr = "gestalt is null";
                    }
                    else
                    {
                        FilterStr = "gestalt like '" + FilterStr + "%'";
                    }
                    if( NodeTypePropFirstVersionId != Int32.MinValue )
                    {
                        WhereClause += @" and n.nodeid in (select nodeid 
                                                             from jct_nodes_props 
                                                            where nodetypepropid in (select nodetypepropid 
                                                                                       from nodetype_props 
                                                                                      where firstpropversionid = (select firstpropversionid 
                                                                                                                    from nodetype_props 
                                                                                                                   where nodetypepropid = " + NodeTypePropFirstVersionId.ToString() + @" ))
                                                              and " + FilterStr + @") ";
                        //FilteredPropIds.Add( NodeTypePropFirstVersionId );
                        //SingleNodeType = true;
                    }
                }
            } // foreach(JObject FilterObj in FiltersApplied)

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromSearch( _SearchTerm, WhereClause, false );
            return Tree;
        }

        /// <summary>
        /// New Filters to offer, based on Results
        /// </summary>
        public JObject FilterOptions( ICswNbtTree Tree )
        {
            JObject FiltersObj = new JObject();
            Tree.goToRoot();
            bool SingleNodeType = IsSingleNodeType();
            if( false == SingleNodeType )
            {
                // Filter on NodeTypes only
                Dictionary<Int32, Int32> NodeTypeIds = new Dictionary<Int32, Int32>();
                Int32 ChildCnt = Tree.getChildNodeCount();
                for( Int32 n = 0; n < ChildCnt; n++ )
                {
                    Tree.goToNthChild( n );
                    CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
                    if( NodeKey != null )
                    {
                        if( false == NodeTypeIds.ContainsKey( NodeKey.NodeTypeId ) )
                        {
                            NodeTypeIds[NodeKey.NodeTypeId] = 0;
                        }
                        NodeTypeIds[NodeKey.NodeTypeId] += 1;
                    }
                    Tree.goToParentNode();
                } // for( Int32 n = 0; n < ChildCnt; n++ )

                if( NodeTypeIds.Keys.Count == 1 )
                {
                    SingleNodeType = true;
                }
                else
                {
                    string FilterName = "Filter To";

                    JArray FilterSet = new JArray();
                    FiltersObj[FilterName] = FilterSet;

                    // Sort by count descending, then (unfortunately) by nodetypeid
                    Dictionary<Int32, Int32> sortedDict = ( from entry
                                                              in NodeTypeIds
                                                            orderby entry.Value descending, entry.Key ascending
                                                            select entry
                                                           ).ToDictionary( pair => pair.Key, pair => pair.Value );
                    foreach( Int32 NodeTypeId in sortedDict.Keys )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                        Int32 Count = sortedDict[NodeTypeId];

                        CswNbtSearchFilterWrapper NodeTypeFilter = new CswNbtSearchFilterWrapper( FilterName, CswNbtSearchFilterType.nodetype, "NT_" + NodeType.NodeTypeId.ToString(), NodeType.NodeTypeName, Count, NodeType.IconFileName );
                        NodeTypeFilter.FirstVersionId = NodeType.FirstVersionNodeTypeId;

                        FilterSet.Add( NodeTypeFilter.ToJObject() );
                    }
                }
            } // if( false == SingleNodeType )

            if( SingleNodeType )
            {
                // Filter on property values in the results
                Collection<Int32> FilteredPropIds = getFilteredPropIds();
                Dictionary<Int32, Dictionary<string, Int32>> PropCounts = new Dictionary<Int32, Dictionary<string, Int32>>();
                Int32 ChildCnt = Tree.getChildNodeCount();
                for( Int32 n = 0; n < ChildCnt; n++ )
                {
                    Tree.goToNthChild( n );
                    JArray Props = Tree.getChildNodePropsOfNode();
                    foreach( JObject Prop in Props )
                    {
                        Int32 NodeTypePropId = CswConvert.ToInt32( Prop["nodetypepropid"] );
                        CswNbtMetaDataFieldType FieldType = _CswNbtResources.MetaData.getFieldType( (CswNbtMetaDataFieldType.NbtFieldType) Enum.Parse( typeof( CswNbtMetaDataFieldType.NbtFieldType ), Prop["fieldtype"].ToString() ) );
                        if( false == FilteredPropIds.Contains( NodeTypePropId ) && FieldType.Searchable )
                        {
                            string Gestalt = Prop["gestalt"].ToString();
                            if( Gestalt.Length > 50 )
                            {
                                Gestalt = Gestalt.Substring( 0, 50 );
                            }

                            if( false == PropCounts.ContainsKey( NodeTypePropId ) )
                            {
                                PropCounts[NodeTypePropId] = new Dictionary<string, Int32>();
                            }
                            if( false == PropCounts[NodeTypePropId].ContainsKey( Gestalt ) )
                            {
                                PropCounts[NodeTypePropId][Gestalt] = 0;
                            }
                            PropCounts[NodeTypePropId][Gestalt] += 1;
                        }
                    }
                    Tree.goToParentNode();
                } // for( Int32 n = 0; n < ChildCnt; n++ )

                foreach( Int32 NodeTypePropId in PropCounts.Keys )
                {
                    CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypePropLatestVersion( NodeTypePropId );
                    string FilterName = NodeTypeProp.PropName;

                    JArray FilterSet = new JArray();
                    FiltersObj[FilterName] = FilterSet;


                    // Sort by count descending, then alphabetically by gestalt
                    Dictionary<string, Int32> sortedDict = ( from entry
                                                               in PropCounts[NodeTypePropId]
                                                             orderby entry.Value descending, entry.Key ascending
                                                             select entry
                                                           ).ToDictionary( pair => pair.Key, pair => pair.Value );
                    foreach( string Value in sortedDict.Keys )
                    {
                        Int32 Count = sortedDict[Value];

                        CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterName, CswNbtSearchFilterType.propval, NodeTypePropId.ToString() + "_" + Value, Value, Count, string.Empty );
                        Filter.FirstPropVersionId = NodeTypeProp.FirstPropVersionId;

                        FilterSet.Add( Filter.ToJObject() );
                    }
                }
            } // if( SingleNodeType )

            return FiltersObj;
        } // FilterOptions()


        public bool saveSearchAsView( CswNbtView View )
        {
            bool ret = false;
            View.Root.ChildRelationships.Clear();

            // NodeType filter becomes Relationship
            Int32 NodeTypeId = Int32.MinValue;
            CswNbtMetaDataNodeType NodeType = null;
            CswNbtViewRelationship ViewRel = null;
            foreach( JObject FilterObj in FiltersApplied )
            {
                CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterObj );
                if( Filter.Type == CswNbtSearchFilterType.nodetype )
                {
                    NodeTypeId = Filter.FirstVersionId;
                    NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                    ViewRel = View.AddViewRelationship( NodeType, false );
                    break;
                }
            } // foreach( JObject FilterObj in FiltersApplied )

            if( ViewRel != null )
            {
                // Add property filters
                foreach( JObject FilterObj in FiltersApplied )
                {
                    CswNbtSearchFilterWrapper Filter = new CswNbtSearchFilterWrapper( FilterObj );
                    if( Filter.Type == CswNbtSearchFilterType.propval )
                    {
                        CswNbtMetaDataNodeTypeProp NodeTypeProp = NodeType.getNodeTypePropByFirstVersionId( Filter.FirstPropVersionId );
                        CswNbtViewProperty ViewProp = View.AddViewProperty( ViewRel, NodeTypeProp );

                        CswNbtSubField DefaultSubField = NodeTypeProp.getFieldTypeRule().SubFields.Default;
                        if( Filter.FilterValue == CswNbtSearchFilterWrapper.BlankValue )
                        {
                            if( DefaultSubField.SupportedFilterModes.Contains( CswNbtPropFilterSql.PropertyFilterMode.Null ) )
                            {
                                View.AddViewPropertyFilter( ViewProp,
                                                            DefaultSubField.Name,
                                                            CswNbtPropFilterSql.PropertyFilterMode.Null,
                                                            string.Empty,
                                                            false );
                            }
                        }
                        else
                        {
                            if( DefaultSubField.SupportedFilterModes.Contains( CswNbtPropFilterSql.PropertyFilterMode.Equals ) )
                            {
                                View.AddViewPropertyFilter( ViewProp,
                                                            DefaultSubField.Name,
                                                            CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                            Filter.FilterValue,
                                                            false );
                            }
                        }
                    }
                } // foreach( JProperty FilterProp in Filters.Properties() )

                View.save();
                View.SaveToCache( true );
                ret = true;

            } // if(ViewRel != null)
            return ret;
        } // saveSearchAsView()

        #endregion Search Functions

        #region Filter Wrapper

        private sealed class CswNbtSearchFilterType : CswEnum<CswNbtSearchFilterType>
        {
            private CswNbtSearchFilterType( string Name ) : base( Name ) { }
            public static IEnumerable<CswNbtSearchFilterType> _All { get { return CswEnum<CswNbtSearchFilterType>.All; } }
            public static explicit operator CswNbtSearchFilterType( string str )
            {
                CswNbtSearchFilterType ret = Parse( str );
                return ( ret != null ) ? ret : CswNbtSearchFilterType.Unknown;
            }
            public static readonly CswNbtSearchFilterType Unknown = new CswNbtSearchFilterType( "Unknown" );

            public static readonly CswNbtSearchFilterType nodetype = new CswNbtSearchFilterType( "nodetype" );
            public static readonly CswNbtSearchFilterType propval = new CswNbtSearchFilterType( "propval" );
        }

        private class CswNbtSearchFilterWrapper
        {
            private JObject _FilterObj;
            public const string BlankValue = "[blank]";

            public CswNbtSearchFilterWrapper( JObject FilterObj )
            {
                _FilterObj = FilterObj;
            }
            public CswNbtSearchFilterWrapper( string inFilterName, CswNbtSearchFilterType inType, string inFilterId, string inFilterValue, Int32 inCount, string inIcon )
            {
                _FilterObj = new JObject();

                FilterName = inFilterName;
                Type = inType;
                FilterId = inFilterId;
                FilterValue = inFilterValue;
                Count = inCount;
                Icon = inIcon;
            }

            public JObject ToJObject()
            {
                return _FilterObj;
            }

            public string FilterName
            {
                get { return _FilterObj["filtername"].ToString(); }
                set { _FilterObj["filtername"] = value; }
            }
            public CswNbtSearchFilterType Type
            {
                get { return (CswNbtSearchFilterType) _FilterObj["filtertype"].ToString(); }
                set { _FilterObj["filtertype"] = value.ToString(); }
            }
            public string FilterId
            {
                get { return _FilterObj["filterid"].ToString(); }
                set { _FilterObj["filterid"] = value; }
            }
            public Int32 Count
            {
                get { return CswConvert.ToInt32( _FilterObj["count"] ); }
                set { _FilterObj["count"] = value.ToString(); }
            }
            public string Icon
            {
                get { return _FilterObj["icon"].ToString(); }
                set { _FilterObj["icon"] = value; }
            }
            public string FilterValue
            {
                get { return _FilterObj["filtervalue"].ToString(); }
                set
                {
                    if( value != string.Empty )
                    {
                        _FilterObj["filtervalue"] = value;
                    }
                    else
                    {
                        _FilterObj["filtervalue"] = BlankValue;
                    }
                }
            } // FilterValue

            #region for NodeType Filters

            public Int32 FirstVersionId
            {
                get { return CswConvert.ToInt32( _FilterObj["firstversionid"] ); }
                set { _FilterObj["firstversionid"] = value.ToString(); }
            }

            #endregion for NodeType Filters

            #region for PropVal Filters

            public Int32 FirstPropVersionId
            {
                get { return CswConvert.ToInt32( _FilterObj["firstpropversionid"] ); }
                set { _FilterObj["firstpropversionid"] = value.ToString(); }
            }

            #endregion for PropVal Filters

        } // CswNbtSearchFilterWrapper

        #endregion Filter Wrapper


        #region Session Cache functions

        /// <summary>
        /// Save this View to Session's data cache
        /// </summary>
        public void SaveToCache( bool IncludeInQuickLaunch, bool ForceCache = false, bool KeepInQuickLaunch = false )
        {
            // don't cache twice
            if( SessionDataId == null || ForceCache || IncludeInQuickLaunch )  // case 23999
            {
                SessionDataId = _CswNbtResources.SessionDataMgr.saveSessionData( this, IncludeInQuickLaunch, KeepInQuickLaunch );
            }
        } // SaveToCache()

        public void clearSessionDataId()
        {
            SessionDataId = null;
        }

        /// <summary>
        /// Key for retrieving the view from the Session's data cache
        /// </summary>
        public CswNbtSessionDataId SessionDataId
        {
            get
            {
                CswNbtSessionDataId ret = null;
                if( _SearchObj["sessiondataid"] != null )
                {
                    ret = new CswNbtSessionDataId( _SearchObj["sessiondataid"].ToString() );
                }
                return ret;
            }
            set
            {
                if( value != null )
                {
                    _SearchObj["sessiondataid"] = value.ToString();
                }
            }
        }

        #endregion Session Cache functions

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtSearch view1, CswNbtSearch view2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( view1, view2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) view1 == null ) || ( (object) view2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( view1.ToString() == view2.ToString() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtSearch view1, CswNbtSearch view2 )
        {
            return !( view1 == view2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtSearch ) )
                return false;
            return this == (CswNbtSearch) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtSearch obj )
        {
            return this == (CswNbtSearch) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 0;
            //if( this.ViewId.isSet() )
            //{
            //    // Positive hashes are for saved views
            //    hashcode = this.ViewId.get();
            //}
            //else
            //{
            // Negative hashes are for dynamic views or searches
            hashcode = Math.Abs( this.ToString().GetHashCode() ) * -1;
            //}
            return hashcode;
        }
        #endregion IEquatable

    } // class CswNbtSearch


} // namespace ChemSW.Nbt



