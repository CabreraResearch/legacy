using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
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

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources, string Prefix )
        {
			_CswNbtResources = CswNbtResources;
            _ViewBuilder = new wsViewBuilder( _CswNbtResources, _ProhibittedFieldTypes, Prefix );  
            //wsViewBuilder.CswViewBuilderProp 

		}//ctor

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ViewBuilder = new wsViewBuilder( _CswNbtResources, _ProhibittedFieldTypes );
            //wsViewBuilder.CswViewBuilderProp 
        }//ctor

        #region Generic Search Form XML

        /// <summary>
        /// Generates the XML for a NodeTypeSelect pick list
        /// </summary>
        private XElement _getNodeTypeBasedSearch( CswNbtMetaDataNodeType SelectedNodeType )
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
                else if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.FE ) )
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
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
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                }
                else
                {
                    SearchOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
                }

                if( null != SearchOC.NodeTypes )
                {
                    SelectedNodeType = SearchOC.NodeTypes.OfType<CswNbtMetaDataNodeType>().First().LatestVersionNodeType;
                }
            }

            XElement NodeTypeSelect = new XElement( "optgroup", new XAttribute( "label", "Specific Types" ) );
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes.Cast<CswNbtMetaDataNodeType>()
                                                        .Where( NodeType => ( NodeType.NodeTypeProps.Count > 0 ) ) )
                                                                  
            {
                XElement ThisOption = new XElement( "option",
                                            new XAttribute( "title", "nodetypeid" ),
                                            new XAttribute( "label", NodeType.NodeTypeName ), // for Chrome
                                            new XAttribute( "value", NodeType.FirstVersionNodeTypeId ), 
                                            new XAttribute( "id", _NodeTypePrefix + NodeType.FirstVersionNodeTypeId ) );
                if( SelectedNodeType == NodeType )
                {
                    ThisOption.Add( new XAttribute( "selected", "selected" ) );
                }
                if( NodeType.NodeTypeName.Length > SelectWidth )
                {
                    SelectWidth = NodeType.NodeTypeName.Length;
                }
                ThisOption.Value = NodeType.NodeTypeName;
                NodeTypeSelect.Add( ThisOption );
            }
            //SelectOptions.Add( NodeTypeSelect );

            XElement ObjectClassSelect = new XElement( "optgroup", new XAttribute( "label", "Generic Types" ) );
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses.Cast<CswNbtMetaDataObjectClass>()
                                                              .Where( ObjectClass => CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass != ObjectClass.ObjectClass &&
                                                                      ( ObjectClass.ObjectClassProps.Count > 0 &&
                                                                        ObjectClass.NodeTypes.Count > 0 ) ) )
            {
                XElement ThisOption = new XElement( "option",
                                            new XAttribute( "title", "objectclassid" ),
                                            new XAttribute( "label", "All " + ObjectClass.ObjectClass ), // for Chrome
                                            new XAttribute( "value", ObjectClass.ObjectClassId ),
                                            new XAttribute( "id", _ObjectClassPrefix + ObjectClass.ObjectClassId ) );
                if( null == SelectedNodeType && SearchOC == ObjectClass)
                {
                    ThisOption.Add( new XAttribute( "selected", "selected" ) );
                }
                if( ObjectClass.ObjectClass.ToString().Length > SelectWidth )
                {
                    SelectWidth = ObjectClass.ObjectClass.ToString().Length;
                }
                ThisOption.Value = "All " + ObjectClass.ObjectClass;
                ObjectClassSelect.Add( ThisOption );
            }
            //SelectOptions.Add( ObjectClassSelect );

            
            //SelectOptions.Add( new XAttribute( "style", "width: " + (SelectWidth*7) + "px;" ) );
            //NodeTypeSearch.Add( SelectOptions );

            XElement NodeTypeProps;
            if( null != SelectedNodeType )
            {
                NodeTypeProps = _ViewBuilder.getNodeTypeProps( SelectedNodeType );
            }
            else
            {
                NodeTypeProps = _ViewBuilder.getNodeTypeProps( SearchOC );
            }

            XElement NodeTypeSearch = new XElement( "search", 
                                            new XAttribute( "searchtype", "nodetypesearch" ), 
                                            new XElement( "nodetypes", 
                                                new XElement( "select",
                                                        NodeTypeSelect,
                                                        ObjectClassSelect)),
                                            NodeTypeProps);
            return NodeTypeSearch;
        } // getNodeTypeBasedSearch()

        #endregion

        #region Get Search XML

        /// <summary>
        /// Returns the XML for filtered (searchable) View properties, if the View is searchable.
        /// Else, returns XML for a NodeTypeSelect.
        /// </summary>
        public XElement getSearchXml( CswNbtView View, string SelectedNodeTypeIdNum, string NodeKey )
        {
            XElement SearchNode = new XElement( "search", 
                                        new XAttribute( "searchtype", "viewsearch" ) );
            
            XElement PropNode = new XElement( "properties" );

            if( null == View || !View.IsSearchable() )
            {
                CswNbtMetaDataNodeType SelectedNodeType = null;
                if( string.IsNullOrEmpty(SelectedNodeTypeIdNum) && !string.IsNullOrEmpty( NodeKey ) )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( NodeKey );
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                    CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];
                    SelectedNodeType = Node.NodeType;
                }
                else if( !string.IsNullOrEmpty( SelectedNodeTypeIdNum ) )
                {
                    Int32 SelectedNodeTypeId = CswConvert.ToInt32( SelectedNodeTypeIdNum );
                    SelectedNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                }
                SearchNode = _getNodeTypeBasedSearch( SelectedNodeType );
            }
            else
            {
                foreach( CswViewBuilderProp SearchProp in View.getOrderedViewProps(false)
                                                     .Where( Prop => Prop.Filters.Count > 0 &&
                                                        !_ProhibittedFieldTypes.Contains( Prop.FieldType ) )
                                                     .Select( Prop => new CswViewBuilderProp( Prop ) ) )
                {

                    ArrayList ViewPropFilters = new ArrayList();
                    foreach( CswNbtViewPropertyFilter Filt in SearchProp.Filters )
                    {
                        ViewPropFilters.Add( Filt );
                    }
                    _ViewBuilder.getViewBuilderPropSubfields( ref PropNode, SearchProp, ViewPropFilters );
                }
                SearchNode.Add( PropNode );
            }

            return SearchNode;
        } // getViewBasedSearch()

        public XElement getSearchProps( string RelatedIdType, string NodeTypeOrObjectClassId, string NodeKey )
        {
            XElement SearchProps = _ViewBuilder.getViewBuilderProps( RelatedIdType, NodeTypeOrObjectClassId, NodeKey );
            return SearchProps;
        }

        #endregion Get Search XML

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

                string ParentViewId = (string) ViewSearch.Property( "parentviewid" ).Value;
                string SearchViewId = (string) ViewSearch.Property( "searchviewid" ).Value;
                SearchPair = new CswNbtViewSearchPair( _CswNbtResources, ParentViewId, SearchViewId );
                if( null != ViewSearch.Property( "viewprops" ) && null != SearchPair.SearchView )
                {
                    JArray Props = (JArray) ViewSearch.Property( "viewprops" ).Value;

                    foreach( JObject FilterProp in Props.Children()
                        .Cast<JObject>()
                        .Where( FilterProp => FilterProp.HasValues ) )
                    {
                        _ViewBuilder.makeViewPropFilter( SearchPair.SearchView, FilterProp );
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
                //NodesSearch = XElement.Parse( SearchJson );
                CswNbtView SearchView = new CswNbtView( _CswNbtResources ) {ViewMode = NbtViewRenderingMode.Tree};

                var ViewNtRelationships = new Dictionary<CswNbtMetaDataNodeType, CswNbtViewRelationship>();
                var ViewOcRelationships = new Dictionary<CswNbtMetaDataObjectClass, CswNbtViewRelationship>();
                
                string ParentViewId = (string) NodesSearch.Property( "parentviewid" ).Value;
                
                if( null != NodesSearch.Property( "viewbuilderprops") )
                {
                    JArray Props = (JArray) NodesSearch.Property( "viewbuilderprops" ).Value;

                    foreach( JObject FilterProp in Props.Children()
                                                        .Cast<JObject>()
                                                        .Where( FilterProp => FilterProp.HasValues ) )
                    {
                        var PropType = CswNbtViewRelationship.RelatedIdType.Unknown;
                        CswNbtViewRelationship.RelatedIdType.TryParse( (string) FilterProp["relatedidtype"], true, out PropType );
                        Int32 NodeTypeOrObjectClassId = CswConvert.ToInt32( (string) FilterProp["nodetypeorobjectclassid"] );
                        Int32 PropId = CswConvert.ToInt32( (string) FilterProp["viewbuilderpropid"] );
                        CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId );
                        if( PropType == CswNbtViewRelationship.RelatedIdType.ObjectClassId &&
                            Int32.MinValue != NodeTypeProp.ObjectClassPropId )
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( NodeTypeOrObjectClassId );
                            if( string.IsNullOrEmpty( ViewName ) ) ViewName = ObjectClass.ObjectClass + " Search";
                            if( NodeTypeProp.NodeType.ObjectClass == ObjectClass )
                            {

                                CswNbtViewRelationship OcRelationship;
                                if( !ViewOcRelationships.ContainsKey( ObjectClass ) )
                                {
                                    OcRelationship = SearchView.AddViewRelationship( ObjectClass, false );
                                    ViewOcRelationships.Add( ObjectClass, OcRelationship );
                                }
                                else
                                {
                                    ViewOcRelationships.TryGetValue( ObjectClass, out OcRelationship );
                                }

                                CswNbtMetaDataObjectClassProp ObjectClassProp = NodeTypeProp.ObjectClassProp;
                                CswNbtViewProperty ViewOcProperty = SearchView.AddViewProperty( OcRelationship, ObjectClassProp );
                                CswNbtViewPropertyFilter ViewOcPropFilt = SearchView.AddViewPropertyFilter( ViewOcProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Undefined, string.Empty, false );
                                _ViewBuilder.makeViewPropFilter( ViewOcPropFilt, FilterProp );
                            }
                        }
                        else if( PropType == CswNbtViewRelationship.RelatedIdType.NodeTypeId &&
                            Int32.MinValue != NodeTypeProp.PropId )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeOrObjectClassId );
                            if( string.IsNullOrEmpty( ViewName ) ) ViewName = NodeType.NodeTypeName + " Search";
                            if( NodeTypeProp.NodeType == NodeType )
                            {
                                CswNbtViewRelationship NtRelationship;
                                if( !ViewNtRelationships.ContainsKey( NodeType ) )
                                {
                                    NtRelationship = SearchView.AddViewRelationship( NodeType, false );
                                    ViewNtRelationships.Add( NodeType, NtRelationship );
                                }
                                else
                                {
                                    ViewNtRelationships.TryGetValue( NodeType, out NtRelationship );
                                }

                                CswNbtViewProperty ViewNtProperty = SearchView.AddViewProperty( NtRelationship, NodeTypeProp );
                                CswNbtViewPropertyFilter ViewNtPropFilt = SearchView.AddViewPropertyFilter( ViewNtProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Undefined, string.Empty, false );
                                _ViewBuilder.makeViewPropFilter( ViewNtPropFilt, FilterProp );
                            }
                        }
                    }
                }
                if( string.IsNullOrEmpty( ViewName ) ) ViewName = "No Results for Search";
                SearchView.ViewName = ViewName;
                SearchView.SaveToCache( true );
                string SearchViewId = SearchView.SessionViewId.ToString();
                GenericSearch = new CswNbtViewSearchPair(_CswNbtResources, ParentViewId, SearchViewId );
            }

            return GenericSearch;
        }

        #endregion

        
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
                if( CswNbtViewId.isViewIdString( _ParentViewKey ))
                {
                    CswNbtViewId _ParentVid = new CswNbtViewId( _ParentViewKey );
                    _ParentView = _CswNbtResources.ViewSelect.restoreView( _ParentVid );
                    if( null == _ParentView.SessionViewId )
                    {
                        _ParentView.SaveToCache( true );
                    }
                }
                else if( CswNbtSessionDataId.isSessionDataIdString( _ParentViewKey ))
                {
                    CswNbtSessionDataId _ParentSessionId = new CswNbtSessionDataId( _ParentViewKey );
                    _ParentView = _CswNbtResources.ViewSelect.getSessionView( _ParentSessionId );
                }
            }
            
            // If this is the 2nd search, try to recycle the SessionView
            CswNbtView _SearcharbleView = null;
            if( _ParentViewKey != _SearchViewKey && // true if we're coming from a non-view, like Welcome
                !string.IsNullOrEmpty( _SearchViewKey ) )
            {
                CswNbtSessionDataId _SessionViewId = new CswNbtSessionDataId( _SearchViewKey );
                _SearcharbleView = _CswNbtResources.ViewSelect.getSessionView( _SessionViewId );
            }
            // This is the 1st search, spin off ParenView into a SessionView
            else if( null != _ParentView )
            {
                _SearcharbleView = new CswNbtView( _CswNbtResources );
                _SearcharbleView.LoadXml( _ParentView.ToXml() );
                _SearcharbleView.ViewName = _makeSearchViewName( _SearcharbleView.ViewName );
                //Must depart the nest immediately
                _SearcharbleView.ViewId = new CswNbtViewId( Int32.MinValue );
                _SearcharbleView.clearSessionViewId();
                _SearcharbleView.SaveToCache( true );
            }

            // Sanity check: we have a SearchView
            if( null != _SearcharbleView )
            {
                SearchView = _SearcharbleView;
                SearchViewId = _SearcharbleView.SessionViewId.ToString();
                if( ViewMode == NbtViewRenderingMode.Unknown ) ViewMode = _SearcharbleView.ViewMode;
                // If we're coming from the Welcome page, this will be true
                if( null == _ParentView ) _ParentView = _SearcharbleView;
            }

            // In case we have neither Search nor Parent views 
            if( null != _ParentView )
            {
                ParentViewId = _ParentView.SessionViewId.ToString();
                ViewMode = _ParentView.ViewMode;
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
