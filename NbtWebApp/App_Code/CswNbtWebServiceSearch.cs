﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSearch
    {
        private readonly CswNbtResources _CswNbtResources;
        //private readonly Int32 _ConstrainToObjectClassId = Int32.MinValue;
        private const string _NodeTypePrefix = "nt_";
        private const string _ObjectClassPrefix = "oc_";
        private wsViewBuilder _ViewBuilder;

        /// <summary>
        /// Searching against these field types is not yet supported
        /// </summary>
        private ArrayList _ProhibittedFieldTypes
        {
            get
            {
                ArrayList InvalidFieldTypes = new ArrayList();
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Button ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.LogicalSet ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewPickList ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.ViewReference ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MOL ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MTBF ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ) );
                InvalidFieldTypes.Add( _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Password ) );
                return InvalidFieldTypes;
            }
        }

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ViewBuilder = new wsViewBuilder( _CswNbtResources, _ProhibittedFieldTypes );
            //wsViewBuilder.CswViewBuilderProp 
        }//ctor

        #region Generic Search Form JSON

        /// <summary>
        /// Generates the JSON for a NodeTypeSelect pick list
        /// </summary>
        private JObject _getNodeTypeBasedSearch( CswNbtMetaDataNodeType SelectedNodeType )
        {
            Int32 SelectWidth = 0;
            //var SelectedNodeType = Node.NodeType; //_CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
            CswNbtMetaDataObjectClass SearchOC = null;

            if( null == SelectedNodeType )
            {

                if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.IMCS ) )
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
                }
                else if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                }
                else if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.BioSafety ) )
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.BiologicalClass );
                }
                else if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.SI ) )
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                }
                else
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
                }

                if( null != SearchOC.getNodeTypes() )
                {
                    SelectedNodeType = SearchOC.getNodeTypes().OfType<CswNbtMetaDataNodeType>().First().getNodeTypeLatestVersion();
                }
            }

            JObject NodeTypeSelObj = new JObject();
            NodeTypeSelObj["label"] = "Specific Types";

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion()
                                                        .Where( NodeType => ( NodeType.getNodeTypeProps().Count() > 0 ) ) )
            {
                string OptionId = "option_" + NodeType.FirstVersionNodeTypeId;
                NodeTypeSelObj[OptionId] = new JObject();
                NodeTypeSelObj[OptionId]["type"] = "nodetypeid";
                NodeTypeSelObj[OptionId]["name"] = NodeType.NodeTypeName;
                NodeTypeSelObj[OptionId]["value"] = NodeType.FirstVersionNodeTypeId.ToString();
                NodeTypeSelObj[OptionId]["id"] = _NodeTypePrefix + NodeType.FirstVersionNodeTypeId;

                if( SelectedNodeType == NodeType )
                {
                    NodeTypeSelObj[OptionId]["selected"] = true;
                }
                if( NodeType.NodeTypeName.Length > SelectWidth )
                {
                    SelectWidth = NodeType.NodeTypeName.Length;
                }
            } //SelectOptions.Add( NodeTypeSelect );

            JObject ObjectClassSelObj = new JObject();
            ObjectClassSelObj["label"] = "Generic Types";

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClasses().Cast<CswNbtMetaDataObjectClass>()
                                                              .Where( ObjectClass => CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass != ObjectClass.ObjectClass &&
                                                                      ( ObjectClass.ObjectClassProps.Count() > 0 &&
                                                                        ObjectClass.getNodeTypes().Count() > 0 ) ) )
            {
                string OptionId = "option_" + ObjectClass.ObjectClassId;
                ObjectClassSelObj[OptionId] = new JObject();
                ObjectClassSelObj[OptionId]["type"] = "objectclassid";
                ObjectClassSelObj[OptionId]["name"] = "All " + ObjectClass.ObjectClass;
                ObjectClassSelObj[OptionId]["value"] = ObjectClass.ObjectClassId.ToString();
                ObjectClassSelObj[OptionId]["id"] = _ObjectClassPrefix + ObjectClass.ObjectClassId;

                if( null == SelectedNodeType && SearchOC == ObjectClass )
                {
                    ObjectClassSelObj[OptionId]["selected"] = true;
                }
                if( ObjectClass.ObjectClass.ToString().Length > SelectWidth )
                {
                    SelectWidth = ObjectClass.ObjectClass.ToString().Length;
                }
            }
            //SelectOptions.Add( ObjectClassSelect );


            //SelectOptions.Add( new XAttribute( "style", "width: " + (SelectWidth*7) + "px;" ) );
            //NodeTypeSearch.Add( SelectOptions );

            JObject NodeTypeProps = null;
            if( null != SelectedNodeType )
            {
                NodeTypeProps = _ViewBuilder.getNodeTypeProps( SelectedNodeType );
            }
            else
            {
                NodeTypeProps = _ViewBuilder.getNodeTypeProps( SearchOC );
            }

            JObject NodeTypeSearch = new JObject();
            NodeTypeSearch["searchtype"] = "nodetypesearch";
            NodeTypeSearch["nodetypes"] = new JObject();
            NodeTypeSearch["nodetypes"]["nodetypeselect"] = NodeTypeSelObj;
            NodeTypeSearch["nodetypes"]["objectclassselect"] = ObjectClassSelObj;
            NodeTypeSearch["props"] = NodeTypeProps;

            return NodeTypeSearch;
        } // getNodeTypeBasedSearch()

        #endregion

        #region Get Search JSON

        /// <summary>
        /// Returns the JSON for filtered (searchable) View properties, if the View is searchable.
        /// Else, returns JSON for a NodeTypeSelect.
        /// </summary>
        public JObject getSearchJson( CswNbtView View, string SelectedNodeTypeIdNum, CswNbtNodeKey NbtNodeKey = null )
        {
            JObject RetObj = new JObject();

            if( null == View || false == View.IsSearchable() )
            {
                CswNbtMetaDataNodeType SelectedNodeType = null;
                if( null != NbtNodeKey )
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NbtNodeKey.NodeId );
                    if( null != Node )
                    {
                        SelectedNodeType = Node.getNodeType();
                    }
                }

                if( null == SelectedNodeType && false == string.IsNullOrEmpty( SelectedNodeTypeIdNum ) )
                {
                    Int32 SelectedNodeTypeId = CswConvert.ToInt32( SelectedNodeTypeIdNum );
                    SelectedNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                }

                RetObj = _getNodeTypeBasedSearch( SelectedNodeType );
            }
            else
            {
                RetObj["searchtype"] = "viewsearch";
                RetObj["properties"] = new JObject();
                foreach( CswViewBuilderProp SearchProp in View.getOrderedViewProps( false )
                                                     .Where( Prop => Prop.Filters.Count > 0 &&
                                                        false == _ProhibittedFieldTypes.Contains( Prop.FieldType ) )
                                                     .Select( Prop => new CswViewBuilderProp( Prop ) ) )
                {

                    ArrayList ViewPropFilters = new ArrayList();
                    foreach( CswNbtViewPropertyFilter Filt in SearchProp.Filters )
                    {
                        ViewPropFilters.Add( Filt );
                    }
                    RetObj["properties"][SearchProp.ViewProp.ArbitraryId] = _ViewBuilder.getVbProp( View, SearchProp );
                }
            }

            return RetObj;
        } // getViewBasedSearch()

        public JObject getSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string NodeKey )
        {
            JObject SearchProps = _ViewBuilder.getVbProperties( RelatedIdType, NodeTypeOrObjectClassId, NodeKey );
            return SearchProps;
        }

        #endregion Get Search JSON

        #region Execute Search

        /// <summary>
        /// Takes a View and applies search parameters as ViewPropertyFilters.
        /// Returns the modified View for processing as Tree/Grid/List.
        /// </summary>
        public CswNbtViewSearchPair doViewBasedSearch( object SearchJson )
        {
            CswNbtViewSearchPair SearchPair = null;
            if( null != SearchJson )
            {
                JObject ViewSearch = JObject.FromObject( SearchJson );

                string ParentViewId = (string) ViewSearch["parentviewid"];
                string SearchViewId = (string) ViewSearch["searchviewid"];
                SearchPair = new CswNbtViewSearchPair( _CswNbtResources, ParentViewId, SearchViewId );
                if( null != ViewSearch["viewprops"] &&
                    null != SearchPair.SearchView &&
                    JTokenType.Array == ViewSearch["viewprops"].Type )
                {
                    JArray Props = (JArray) ViewSearch["viewprops"];

                    foreach( JObject FilterProp in Props.Children()
                        .Cast<JObject>()
                        .Where( FilterProp => FilterProp.HasValues ) )
                    {
                        _ViewBuilder.makeViewPropFilter( SearchPair.SearchView, FilterProp, true );
                    }
                    SearchPair.finalize();
                }
            }
            return SearchPair;
        }

        /// <summary>
        /// If the search is based on NodeType/ObjectClass, construct a View with the included search terms as Property Filters.
        /// Return the View for processing as a Tree
        /// </summary>
        public CswNbtViewSearchPair doNodesSearch( object SearchJson )
        {
            JObject NodesSearch = new JObject();

            CswNbtViewSearchPair GenericSearch = null;
            //CswNbtView SearchView = null;
            string ViewName = string.Empty;
            if( null != SearchJson )
            {
                NodesSearch = JObject.FromObject( SearchJson );

                CswNbtView SearchView = new CswNbtView( _CswNbtResources )
                                            {
                                                ViewMode = NbtViewRenderingMode.Tree,
                                                VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId,
                                                Category = "Search",
                                                Visibility = NbtViewVisibility.User
                                            };

                var ViewNtRelationships = new Dictionary<CswNbtMetaDataNodeType, CswNbtViewRelationship>();
                var ViewOcRelationships = new Dictionary<CswNbtMetaDataObjectClass, CswNbtViewRelationship>();

                string ParentViewId = (string) NodesSearch["parentviewid"];

                if( null != NodesSearch["viewbuilderprops"] &&
                    JTokenType.Array == NodesSearch["viewbuilderprops"].Type )
                {
                    JArray Props = (JArray) NodesSearch["viewbuilderprops"];

                    foreach( JObject FilterProp in Props.Children()
                                                        .Cast<JObject>()
                                                        .Where( FilterProp => FilterProp.HasValues ) )
                    {
                        CswNbtViewRelationship.RelatedIdType PropType;
                        Enum.TryParse( (string) FilterProp["relatedidtype"], true, out PropType );

                        Int32 NodeTypeOrObjectClassId = CswConvert.ToInt32( (string) FilterProp["nodetypeorobjectclassid"] );
                        Int32 PropId = CswConvert.ToInt32( (string) FilterProp["viewbuilderpropid"] );
                        CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId );

                        CswNbtSubField.SubFieldName SubField;
                        Enum.TryParse( (string) FilterProp["subfield"], true, out SubField );

                        CswNbtPropFilterSql.PropertyFilterMode FilterMode;
                        Enum.TryParse( (string) FilterProp["filter"], true, out FilterMode );

                        string FilterValue = CswConvert.ToString( FilterProp["filtervalue"] );

                        if( PropType == CswNbtViewRelationship.RelatedIdType.ObjectClassId &&
                            Int32.MinValue != NodeTypeProp.ObjectClassPropId )
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( NodeTypeOrObjectClassId );
                            if( string.IsNullOrEmpty( ViewName ) ) ViewName = ObjectClass.ObjectClass + " Search";
                            if( _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeProp.NodeTypeId ) == ObjectClass )
                            {

                                CswNbtViewRelationship OcRelationship;
                                if( false == ViewOcRelationships.ContainsKey( ObjectClass ) )
                                {
                                    OcRelationship = SearchView.AddViewRelationship( ObjectClass, false );
                                    ViewOcRelationships.Add( ObjectClass, OcRelationship );
                                }
                                else
                                {
                                    ViewOcRelationships.TryGetValue( ObjectClass, out OcRelationship );
                                }

                                CswNbtMetaDataObjectClassProp ObjectClassProp = NodeTypeProp.getObjectClassProp();
                                CswNbtViewProperty ViewOcProperty = SearchView.AddViewProperty( OcRelationship, ObjectClassProp );
                                SearchView.AddViewPropertyFilter( ViewOcProperty, SubField, FilterMode, FilterValue, false );
                                _ViewBuilder.makeViewPropFilter( SearchView, FilterProp );
                            }
                        }
                        else if( PropType == CswNbtViewRelationship.RelatedIdType.NodeTypeId &&
                            Int32.MinValue != NodeTypeProp.PropId )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeOrObjectClassId );
                            if( string.IsNullOrEmpty( ViewName ) )
                            {
                                ViewName = NodeType.NodeTypeName + " Search";
                            }
                            if( NodeTypeProp.getNodeType() == NodeType )
                            {
                                CswNbtViewRelationship NtRelationship;
                                if( false == ViewNtRelationships.ContainsKey( NodeType ) )
                                {
                                    NtRelationship = SearchView.AddViewRelationship( NodeType, false );
                                    ViewNtRelationships.Add( NodeType, NtRelationship );
                                }
                                else
                                {
                                    ViewNtRelationships.TryGetValue( NodeType, out NtRelationship );
                                }

                                CswNbtViewProperty ViewNtProperty = SearchView.AddViewProperty( NtRelationship, NodeTypeProp );
                                SearchView.AddViewPropertyFilter( ViewNtProperty, SubField, FilterMode, FilterValue, false );
                                _ViewBuilder.makeViewPropFilter( SearchView, FilterProp );
                            }
                        }
                    }
                }
                if( string.IsNullOrEmpty( ViewName ) )
                {
                    ViewName = "No Results for Search";
                }
                SearchView.ViewName = ViewName;
                SearchView.SaveToCache( true );
                string SearchViewId = SearchView.SessionViewId.ToString();
                GenericSearch = new CswNbtViewSearchPair( _CswNbtResources, ParentViewId, SearchViewId );
            }

            return GenericSearch;
        }

        #endregion


        #region Universal Search

        public JObject doUniversalSearch( string SearchTerm, JObject Filters )
        {
            JObject ret = new JObject();
            // Filters Applied
            string WhereClause = string.Empty;
            foreach( JProperty FilterProp in Filters.Properties() )
            {
                JObject Filter = (JObject) FilterProp.Value;
                if( Filter["type"].ToString() == "nodetype" )
                {
                    Int32 NodeTypeId = CswConvert.ToInt32( Filter["id"] );
                    if( NodeTypeId != Int32.MinValue )
                    {
                        WhereClause += " and t.nodetypeid = " + NodeTypeId.ToString();
                    }
                }
                else
                {
                }
            } // foreach(JObject Filter in Filters.Properties)

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromSearch( SearchTerm, WhereClause, false );

            // Results Table
            CswNbtWebServiceTable wsTable = new CswNbtWebServiceTable( _CswNbtResources, null );
            ret["table"] = wsTable.makeTableFromTree( Tree );

            // New Filters
            Tree.goToRoot();
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
            }

            JObject FiltersObj = new JObject();
            foreach( Int32 NodeTypeId in NodeTypeIds.Keys )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                string ThisNTName = NodeType.NodeTypeName.ToLower();
                FiltersObj[ThisNTName] = new JObject();
                FiltersObj[ThisNTName]["type"] = "nodetype";
                FiltersObj[ThisNTName]["id"] = NodeType.NodeTypeId.ToString();
                FiltersObj[ThisNTName]["name"] = NodeType.NodeTypeName.ToString();
                FiltersObj[ThisNTName]["count"] = NodeTypeIds[NodeTypeId].ToString();
                FiltersObj[ThisNTName]["icon"] = NodeType.IconFileName.ToString();
            }
            ret["filters"] = FiltersObj;
            return ret;
        } // doUniversalSearch()

        #endregion Universal Search

    } // class CswNbtWebServiceSearch

    /// <summary>
    /// Represents a relationship between two views: a view of orgin (Parent) and a clone (Search)
    /// The Search view is a temporary, session-only view
    /// ParentViewId is maintained in order to restore the original view
    /// </summary>
    public class CswNbtViewSearchPair
    {
        public readonly string ParentViewId = string.Empty;
        public readonly string SearchViewId = string.Empty;
        public readonly CswNbtView SearchView;
        public readonly NbtViewRenderingMode ViewMode = NbtViewRenderingMode.Unknown;
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtViewSearchPair( CswNbtView _ParentView, CswNbtView _SearchableView )
        {
            ViewMode = _ParentView.ViewMode;
            if( null == _ParentView.SessionViewId || !_ParentView.SessionViewId.isSet() ) _ParentView.SaveToCache( true );
            ParentViewId = _ParentView.SessionViewId.ToString();

            if( null == _SearchableView.SessionViewId || !_SearchableView.SessionViewId.isSet() ) _SearchableView.SaveToCache( true );
            SearchViewId = _SearchableView.SessionViewId.ToString();

            SearchView = _SearchableView;
        }

        public CswNbtViewSearchPair( CswNbtResources CswNbtResources, string _ParentViewKey, string _SearchViewKey )
        {
            _CswNbtResources = CswNbtResources;
            CswNbtView _ParentView = null;
            // Try to fetch ParentView for reload on Clear()
            if( !string.IsNullOrEmpty( _ParentViewKey ) )
            {
                if( CswNbtViewId.isViewIdString( _ParentViewKey ) )
                {
                    CswNbtViewId _ParentVid = new CswNbtViewId( _ParentViewKey );
                    _ParentView = _CswNbtResources.ViewSelect.restoreView( _ParentVid );
                    if( null == _ParentView.SessionViewId )
                    {
                        _ParentView.SaveToCache( true );
                    }
                }
                else if( CswNbtSessionDataId.isSessionDataIdString( _ParentViewKey ) )
                {
                    CswNbtSessionDataId _ParentSessionId = new CswNbtSessionDataId( _ParentViewKey );
                    _ParentView = _CswNbtResources.ViewSelect.getSessionView( _ParentSessionId );
                }
            }

            // If this is the 2nd search, try to recycle the SessionView
            CswNbtView _SearchableView = null;
            if( _ParentViewKey != _SearchViewKey && // true if we're coming from a non-view, like Welcome
                !string.IsNullOrEmpty( _SearchViewKey ) )
            {
                CswNbtSessionDataId _SessionViewId = new CswNbtSessionDataId( _SearchViewKey );
                _SearchableView = _CswNbtResources.ViewSelect.getSessionView( _SessionViewId );
            }
            // This is the 1st search, spin off ParenView into a SessionView
            else if( null != _ParentView )
            {
                _SearchableView = new CswNbtView( _CswNbtResources );
                _SearchableView.LoadXml( _ParentView.ToXml() );
                _SearchableView.ViewName = _makeSearchViewName( _SearchableView.ViewName );
                //Must depart the nest immediately
                _SearchableView.ViewId = new CswNbtViewId( Int32.MinValue );
                _SearchableView.clearSessionViewId();
            }

            // Sanity check: we have a SearchView
            if( null != _SearchableView )
            {
                _SearchableView.SaveToCache( true, true );
                SearchView = _SearchableView;
                SearchViewId = _SearchableView.SessionViewId.ToString();
                if( ViewMode == NbtViewRenderingMode.Unknown )
                {
                    ViewMode = _SearchableView.ViewMode;
                }
                // If we're coming from the Welcome page, this will be true
                if( null == _ParentView )
                {
                    _ParentView = _SearchableView;
                    _ParentView.SaveToCache( false );
                }
            }

            // In case we have neither Search nor Parent views 
            if( null != _ParentView )
            {
                ParentViewId = _ParentView.SessionViewId.ToString();
                if( ViewMode == NbtViewRenderingMode.Unknown )
                {
                    ViewMode = _ParentView.ViewMode;
                }
            }
        }

        private static string _makeSearchViewName( string _ViewName )
        {
            string _SearchViewName = _ViewName;

            if( !_SearchViewName.StartsWith( "Search " ) && !_SearchViewName.EndsWith( " Search" ) )
            {
                _SearchViewName = "Search '" + _SearchViewName + "'";
            }
            if( !_SearchViewName.EndsWith( " Results" ) )
            {
                _SearchViewName += " Results";
            }

            return _SearchViewName;
        }

        public void finalize()
        {
            if( null != SearchView )
            {
                SearchView.SaveToCache( true, true );
            }
        }
    }
} // namespace ChemSW.Nbt.WebServices
