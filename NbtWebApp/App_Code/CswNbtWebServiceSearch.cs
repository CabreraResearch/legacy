using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceSearch
	{
		private readonly CswNbtResources _CswNbtResources;
	    private readonly Int32 _ConstrainToObjectClassId = Int32.MinValue;
	    private const string _NodeTypePrefix = "nt_";
	    private const string _ObjectClassPrefix = "oc_";

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources, Int32 ConstrainToObjectClassId )
		{
			_CswNbtResources = CswNbtResources;
		    _ConstrainToObjectClassId = ConstrainToObjectClassId;
		}//ctor

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        #region Generic Search Form XML

        /// <summary>
        /// Generates the XML for a NodeTypeSelect pick list
        /// </summary>
        public XElement getNodeTypeSelect( Int32 SelectedNodeTypeId )
        {
            var NodeTypeSelect = new XElement( "nodetypeselect" );
            var SelectedNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
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

                var NodeTypes = (IEnumerable<CswNbtMetaDataNodeType>) SearchOC.NodeTypes;
                SelectedNodeType = NodeTypes.First().LatestVersionNodeType;
            }

            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes.Cast<CswNbtMetaDataNodeType>()
                                                        .Where( NodeType => _ConstrainToObjectClassId == Int32.MinValue || 
                                                                NodeType.ObjectClass.ObjectClassId == _ConstrainToObjectClassId ) )
            {
                NodeTypeSelect.Add( new XElement( "nodetype",
                                                  new XAttribute( "relatedidtype", "nodetype" ), 
                                                  new XAttribute( "isselected", (SelectedNodeType == NodeType) ), 
                                                  new XAttribute( "name", NodeType.NodeTypeName ), 
                                                  new XAttribute( "id", NodeType.FirstVersionNodeTypeId ), 
                                                  new XAttribute( "elementid", _NodeTypePrefix + NodeType.FirstVersionNodeTypeId ) ) );
            }

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses.Cast<CswNbtMetaDataObjectClass>()
                                                              .Where( ObjectClass => _ConstrainToObjectClassId == Int32.MinValue || 
                                                                      ObjectClass.ObjectClassId == _ConstrainToObjectClassId ) )
            {
                NodeTypeSelect.Add( new XElement( "nodetype",
                                                  new XAttribute( "relatedidtype", "objectclass" ),
                                                  new XAttribute( "isselected", ( null == SelectedNodeType && SearchOC == ObjectClass) ),
                                                  new XAttribute( "name", "All " + ObjectClass.ObjectClass ),
                                                  new XAttribute( "id", ObjectClass.ObjectClassId ),
                                                  new XAttribute( "elementid", _ObjectClassPrefix + ObjectClass.ObjectClassId ) ) );
            }
            if( null != SelectedNodeType )
            {
                XElement NodeTypeProps = getNodeTypeProps( "nodetype", SelectedNodeType.NodeTypeId.ToString(), string.Empty, string.Empty, string.Empty );
                NodeTypeSelect.AddAfterSelf( NodeTypeProps );
            }
            else
            {
                XElement NodeTypeProps = getNodeTypeProps( "objectclass", SearchOC.ObjectClassId.ToString(), string.Empty, string.Empty, string.Empty );
                NodeTypeSelect.AddAfterSelf( NodeTypeProps );
            }
            return NodeTypeSelect;
        }

        private IEnumerable _getNodeTypeProps( CswNbtMetaDataNodeType NodeType )
        {
            var NtProps = new LinkedList<CswNbtMetaDataNodeTypeProp>();
            if( null != NodeType)
            {
                var PropsByName = new SortedList();
                var PropsById = new SortedList();

                CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
                while( ThisVersionNodeType != null )
                {
                    foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>()
                                                                    .Where( ThisProp => !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) 
                                                                            && !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                        NtProps.AddLast( ThisProp );
                    }
                    ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
                }
            }
            return NtProps;
        }
        
        private static IEnumerable _getObjectClassProps( CswNbtMetaDataObjectClass ObjectClass )
        {
            var OcProps = new LinkedList<CswNbtMetaDataObjectClassProp>();
            if( null != ObjectClass )
            {
                foreach( CswNbtMetaDataObjectClassProp OcProp in ObjectClass.ObjectClassProps )
                {
                     OcProps.AddLast( OcProp );
                }
            }
            return OcProps;
        }

        public XElement getNodeTypeProps( string RelatedIdType, string ObjectPk, string SelectedSubField, string SelectedPropName, string FilterValue )
        {
            var PropsNode = new XElement( "searchprops" );
            Int32 ObjectId = CswConvert.ToInt32( ObjectPk );
            if( Int32.MinValue != ObjectId )
            {
                if( RelatedIdType.ToLower() == "nodetype" )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ObjectId );
                    var NodeTypeProps = _getNodeTypeProps( NodeType );
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeProps )
                    {
                        var PropNode =  new XElement( "prop",
                                            new XAttribute( "relatedidtype", CswNbtViewRelationship.RelatedIdType.NodeTypeId ),
                                            new XAttribute( "nodetypepropid", Prop.FirstPropVersionId ),
                                            new XAttribute( "nodetypeid", Prop.NodeType.NodeTypeId ),
                                            new XAttribute( "name", Prop.PropName ),
                                            new XAttribute( "selected", (Prop.PropName.ToLower() == SelectedPropName.ToLower() )));
                        var PropSubFields = _getPropSubFields( Prop, SelectedSubField, FilterValue, null );
                        PropNode.Add( PropSubFields );
                        PropsNode.Add( PropNode );
                    }
                }
                else if( RelatedIdType.ToLower() == "objectclass")
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectId );
                    var ObjectClassProps = _getObjectClassProps( ObjectClass );
                    foreach( CswNbtMetaDataObjectClassProp Prop in ObjectClassProps )
                    {
                        var PropNode = new XElement( "prop",
                                            new XAttribute( "relatedidtype", CswNbtViewRelationship.RelatedIdType.ObjectClassId ),
                                            new XAttribute( "objectclasspropid", Prop.PropId ),
                                            new XAttribute( "objectclassid", Prop.ObjectClass.ObjectClassId ),
                                            new XAttribute( "name", Prop.PropName ),
                                            new XAttribute( "selected", ( Prop.PropName.ToLower() == SelectedPropName.ToLower() ) ) );
                        var PropSubFields = _getPropSubFields( Prop, SelectedSubField, FilterValue, null );
                        PropNode.Add( PropSubFields );
                        PropsNode.Add( PropNode );
                    }
                }
            }
            return PropsNode;
        }

        /// <summary>
        /// Returns the Subfields XML for a NodeTypeProp
        /// </summary>
        private XElement _getPropSubFields( CswNbtMetaDataNodeTypeProp Prop, string SelectedSubField, string FilterValue,
                                            ArrayList ViewFilters )
        {
            string SelectedValue = string.IsNullOrEmpty( SelectedSubField ) ? 
                                            Prop.FieldTypeRule.SubFields.Default.Column.ToString() : SelectedSubField;
            return _getPropSubFields( Prop.FieldTypeRule.SubFields, SelectedValue, Prop.FieldType.FieldType, Prop.PropId, FilterValue, ViewFilters );
        }

        /// <summary>
        /// Returns the Subfields XML for an ObjectClassProp
        /// </summary>
        private XElement _getPropSubFields( CswNbtMetaDataObjectClassProp Prop, string SelectedSubField, string FilterValue,
                                            ArrayList ViewFilters )
        {
            string SelectedValue = string.IsNullOrEmpty( SelectedSubField ) ? 
                                            Prop.FieldTypeRule.SubFields.Default.Column.ToString() : SelectedSubField;
            return _getPropSubFields( Prop.FieldTypeRule.SubFields, SelectedValue, Prop.FieldType.FieldType, Prop.PropId, FilterValue, ViewFilters );
        }

        /// <summary>
        /// Returns the Subfields XML for a SubFields collection
        /// </summary>
        private XElement _getPropSubFields( CswNbtSubFieldColl SubFields, string SelectedSubField, CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType,
                                            Int32 SelectedPropId, string FilterValue, ArrayList ViewFilters )
        {
            if( null == ViewFilters )
            {
                ViewFilters = new ArrayList();
            }
            var SubFieldsNode = new XElement( "subfields" );
            foreach( CswNbtSubField Field in SubFields )
            {
                var FieldNode = new XElement( "subfield",
                                                 new XAttribute( "name", Field.Name ),
                                                 new XAttribute( "column", Field.Column ),
                                                 new XAttribute( "defaultfiltermode", Field.DefaultFilterMode ),
                                                 new XAttribute( "isviewdefault", (ViewFilters.Contains(Field.Name) ) ),
                                                 new XAttribute( "selected", ( Field.Name.ToString() == SelectedSubField ) ) );
                FieldNode.Add( _getSubFieldFilters( FilterValue, SelectedFieldType, Field, SelectedPropId ) );
                SubFieldsNode.Add( FieldNode );
            }
            return SubFieldsNode;
        }

        /// <summary>
        /// Returns the XML for For SubFields Filters
        /// </summary>
        private XElement _getSubFieldFilters( string FilterValue, CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType, CswNbtSubField SubField, 
                                              Int32 SelectedPropId, CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect = CswNbtPropFilterSql.PropertyFilterMode.Undefined )
        {
            var FiltersNode = new XElement( "filters" );
            if( FilterModeToSelect == CswNbtPropFilterSql.PropertyFilterMode.Undefined )
            {
                FilterModeToSelect = SubField.DefaultFilterMode;
            }
            foreach( CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt in SubField.SupportedFilterModes )
            {
                var FilterModeNode = ( new XElement( "filter",
                                    new XAttribute( "name", FilterModeOpt ),
                                    new XAttribute( "selected", ( FilterModeOpt == FilterModeToSelect ) ) ) );

                _getFilterInitValue( ref FilterModeNode, FilterValue, SelectedFieldType, SelectedPropId, FilterModeOpt );
                
                FiltersNode.Add( FilterModeNode );
            }

            return FiltersNode;
        }

        private void _getFilterInitValue(ref XElement FilterModeNode, string FilterValue, CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType, 
                                         Int32 SelectedPropId, CswNbtPropFilterSql.PropertyFilterMode SelectedFilterMode)
        {

            if( SelectedFilterMode != CswNbtPropFilterSql.PropertyFilterMode.Null &&
                SelectedFilterMode != CswNbtPropFilterSql.PropertyFilterMode.NotNull )
            {
                switch( SelectedFieldType )
                {
                    case CswNbtMetaDataFieldType.NbtFieldType.Date:
                        FilterModeNode.SetAttributeValue( "fieldtype", "date" );

                        if( !string.IsNullOrEmpty(FilterValue) )
                            if( FilterValue.Substring( 0, "today".Length ) == "today" )
                            {
                                FilterModeNode.SetAttributeValue( "today", true );
                                FilterModeNode.SetAttributeValue( "selecteddate", DateTime.Today.Date );
                                FilterModeNode.SetAttributeValue( "todayplusdays", FilterValue.Substring( "today+".Length ) );
                            }
                            else
                            {
                                FilterModeNode.SetAttributeValue( "today", false );
                                FilterModeNode.SetAttributeValue( "selecteddate", FilterValue );
                                FilterModeNode.SetAttributeValue( "todayplusdays", "" );
                            }
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.List:
                        FilterModeNode.SetAttributeValue( "fieldtype", "list" );
                        var PickList = new XElement( "options" );
                        var CswNbtNodeTypePropListOptions = new PropTypes.CswNbtNodeTypePropListOptions( _CswNbtResources, SelectedPropId );
                        foreach( var Option in CswNbtNodeTypePropListOptions.Options )
                        {
                            PickList.Add( new XElement( "option",
                                                new XAttribute( "value", Option.Value ),
                                                new XAttribute( "text", Option.Text ), 
                                                new XAttribute("selected", (Option.Value == FilterValue))));
                        }
                        FilterModeNode.Add( PickList );
                        break;
                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        FilterModeNode.SetAttributeValue( "fieldtype", "logical" );
                        if( !string.IsNullOrEmpty( FilterValue ) )
                        {
                            Tristate Checked = CswConvert.ToTristate( FilterValue );
                            FilterModeNode.SetAttributeValue( "checked", Checked );
                        }
                        break;
                    default:
                        FilterModeNode.SetAttributeValue( "fieldtype", "text" );
                        FilterModeNode.SetAttributeValue( "text", FilterValue );
                        break;
                    }
                }
            }

        #endregion

        #region View-based Search XML

        /// <summary>
        /// Returns the XML for filtered (searchable) View properties, if the View is searchable.
        /// Else, returns XML for a NodeTypeSelect.
        /// </summary>
        public XElement getSearchProps( CswNbtView View, string SelectedSubField, string FilterValue )
        {
            var SearchProps = new XElement( "searchprops" );
            if( !View.IsSearchable() )
            {
                SearchProps = getNodeTypeSelect( Int32.MinValue );
            }
            else
            {
                foreach( CswNbtViewProperty Prop in View.getOrderedViewProps().Where(Prop => Prop.Filters.Count > 0 ) )
                {
                    var PropNode = new XElement( "viewprop",
                                                 new XAttribute( "propidtype", Prop.Type.ToString().ToLower() ),
                                                 new XAttribute( "nodetypepropid", Prop.FirstVersionNodeTypeProp.PropId ),
                                                 new XAttribute( "objectclasspropid", Prop.ObjectClassPropId ),
                                                 new XAttribute( "name", Prop.Name ) );
                    
                    var Filters = (IEnumerable<CswNbtViewPropertyFilter>) Prop.Filters;
                    ArrayList ViewSubFieldNames = new ArrayList();
                    foreach( CswNbtViewPropertyFilter var in Filters )
                    {
                        ViewSubFieldNames.Add( var.SubfieldName );
                    }
                    var SubFields = new XElement("subfields");
                    if( Int32.MinValue != Prop.NodeTypePropId )
                    {
                        SubFields = _getPropSubFields( Prop.NodeTypeProp, SelectedSubField, FilterValue, ViewSubFieldNames );
                    }
                    else if( Int32.MinValue != Prop.ObjectClassPropId)
                    {
                        SubFields = _getPropSubFields( Prop.ObjectClassProp, SelectedSubField, FilterValue, ViewSubFieldNames );
                    }
                    PropNode.Add( SubFields );
                    SearchProps.Add( PropNode );
                }
            }
            return SearchProps;
        }

        public XElement getSearchViews(ICswNbtUser Userid, bool ForMobile, string OrderBy)
        {
            var ReturnNode = new XElement( "search" );
            if( null != Userid )
            {
                ReturnNode = _CswNbtResources.ViewSelect.getSearchableViews( Userid, ForMobile, OrderBy);
            }
            return ReturnNode;
        }

        #endregion

        #region Execute Search

        /// <summary>
        /// Takes a View and applies search parameters as ViewPropertyFilters.
        /// Returns the modified View for processing as Tree/Grid/List.
        /// </summary>
        /// <param name="SearchXml"></param>
        /// <param name="SearchView"></param>
        /// <returns></returns>
        public CswNbtView doViewBasedSearch( string SearchXml, string ViewIdNum)
        {
            CswNbtView SearchView = null;
            if( !string.IsNullOrEmpty( SearchXml ) && null != ViewIdNum )
            {
                Int32 ViewId = CswConvert.ToInt32( ViewIdNum );
                CswNbtView InitialView = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                SearchView = new CswNbtView( _CswNbtResources );
                SearchView.makeNew( "Search " + InitialView.ViewName, InitialView.Visibility, InitialView.VisibilityRoleId, InitialView.VisibilityUserId, InitialView );
                var ViewSearch = XElement.Parse( SearchXml );
                foreach( XElement Prop in ViewSearch.Elements("viewprop") )
                {
                    var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
                    CswNbtViewProperty.CswNbtPropType.TryParse( Prop.Attribute( "propidtype" ).Value, true, out PropType );
                    Int32 PropId = Int32.MinValue;
                    switch( PropType )
                    {
                        case CswNbtViewProperty.CswNbtPropType.NodeTypePropId:
                            PropId = CswConvert.ToInt32(Prop.Attribute( "nodetypepropid" ).Value );
                            break;
                        case CswNbtViewProperty.CswNbtPropType.ObjectClassPropId:
                            PropId = CswConvert.ToInt32(Prop.Attribute( "objectclasspropid" ).Value );
                            break;
                    }
                    if( PropType != CswNbtViewProperty.CswNbtPropType.Unknown &&
                        PropId != Int32.MinValue )
                    {
                        CswNbtViewProperty ViewProp = SearchView.findPropertyById( PropType, PropId );
                        if(null != ViewProp)
                        {
                            _addViewPropFilter( Prop, ref SearchView, ViewProp );
                        }
                    }
                }
            }
            return SearchView;
        }

        private void _addViewPropFilter( XElement SearchNode, ref CswNbtView SearchView, CswNbtViewProperty ViewProp )
        {
            var FieldName = CswNbtSubField.SubFieldName.Unknown;
            CswNbtSubField.SubFieldName.TryParse( SearchNode.Attribute( "subfieldname" ).Value, true, out FieldName );
            var FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
            CswNbtPropFilterSql.PropertyFilterMode.TryParse( SearchNode.Attribute( "filtermode" ).Value, true, out FilterMode );
            string SearchTerm = SearchNode.Attribute( "value" ).Value;

            SearchView.AddViewPropertyFilter( ViewProp, FieldName, FilterMode, SearchTerm, false );
        }

        /// <summary>
        /// If the search is based on NodeType/ObjectClass, construct a View with the included search terms as Property Filters.
        /// Return the View for processing as a Tree
        /// </summary>
        public CswNbtView doNodesSearch( string SearchXml )
        {
            var NodesSearch = new XElement( "search" );
            CswNbtView SearchView = null;
            if( !string.IsNullOrEmpty( SearchXml ) )
            {
                NodesSearch = XElement.Parse( SearchXml );
                SearchView = new CswNbtView( _CswNbtResources );
                SearchView.makeNew( "Search Results", NbtViewVisibility.Global, null, null, Int32.MinValue );
                SearchView.ViewMode = NbtViewRenderingMode.Tree;

                var ViewNtRelationships = new Dictionary<CswNbtMetaDataNodeType, CswNbtViewRelationship>();
                var ViewOcRelationships = new Dictionary<CswNbtMetaDataObjectClass, CswNbtViewRelationship>();
                
                foreach( XElement NT in NodesSearch.Elements("prop") )
                {
                    var PropType = CswNbtViewRelationship.RelatedIdType.Unknown;
                    CswNbtViewRelationship.RelatedIdType.TryParse( NT.Attribute( "relatedidtype" ).Value, true, out PropType );
                    switch( PropType )
                    {
                        case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                            Int32 NodeTypeId = CswConvert.ToInt32( NT.Attribute( "nodetypeid" ).Value );
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                            
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

                            Int32 NodeTypePropId = CswConvert.ToInt32( NT.Attribute( "nodetypepropid" ).Value );
                            CswNbtMetaDataNodeTypeProp NodeTypeProp = NodeType.getNodeTypePropByFirstVersionId( NodeTypePropId );
                            
                            CswNbtViewProperty ViewNtProperty = SearchView.AddViewProperty(NtRelationship, NodeTypeProp);
                            _addViewPropFilter( NT, ref SearchView, ViewNtProperty );
                            
                            break;
                        case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                            Int32 ObjectClassId = CswConvert.ToInt32( NT.Attribute( "nodetypeid" ).Value );
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );

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

                            Int32 ObjectClassPropId = CswConvert.ToInt32( NT.Attribute( "objectclasspropid" ).Value );
                            CswNbtMetaDataObjectClassProp ObjectClassProp = ObjectClass.getObjectClassProp( ObjectClassPropId );

                            CswNbtViewProperty ViewOcProperty = SearchView.AddViewProperty( OcRelationship, ObjectClassProp );
                            _addViewPropFilter( NT, ref SearchView, ViewOcProperty );

                            break;
                    }

                }

            }
            return SearchView;
        }

        #endregion

    } // class CswNbtWebServiceSearch

} // namespace ChemSW.Nbt.WebServices
