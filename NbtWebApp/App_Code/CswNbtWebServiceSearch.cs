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
        private XElement _getNodeTypeBasedSearch( Int32 SelectedNodeTypeId )
        {
            Int32 SelectWidth = 0;
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

            XElement NodeTypeSelect = new XElement( "optgroup", new XAttribute( "label", "Specific Types" ) );
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes.Cast<CswNbtMetaDataNodeType>()
                                                        .Where( NodeType => _ConstrainToObjectClassId == Int32.MinValue || 
                                                                NodeType.ObjectClass.ObjectClassId == _ConstrainToObjectClassId ) )
            {
                XElement ThisOption = new XElement( "option",
                                            new XAttribute( "label", "nodetype" ), 
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
                                                              .Where( ObjectClass => ( _ConstrainToObjectClassId == Int32.MinValue || 
                                                                      ObjectClass.ObjectClassId == _ConstrainToObjectClassId ) &&
                                                                      CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass != ObjectClass.ObjectClass) )
            {
                XElement ThisOption = new XElement( "option",
                                            new XAttribute( "label", "objectclass" ),
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
                NodeTypeProps = getNodeTypeProps( "nodetype", SelectedNodeType.NodeTypeId.ToString() );
            }
            else
            {
                NodeTypeProps = getNodeTypeProps( "objectclass", SearchOC.ObjectClassId.ToString() );
            }

            XElement NodeTypeSearch = new XElement( "search", 
                                            new XAttribute( "searchtype", "nodetypesearch" ), 
                                            new XElement( "nodetypes", 
                                                new XElement( "select", 
                                                    new XAttribute( "id", "node_type_select" ), 
                                                    new XAttribute( "name", "node_type_select" ),
                                                        NodeTypeSelect,
                                                        ObjectClassSelect)),
                                            NodeTypeProps);
            return NodeTypeSearch;
        } // getNodeTypeBasedSearch()

        private IEnumerable<CswNbtMetaDataNodeTypeProp> _getNodeTypeProps( CswNbtMetaDataNodeType NodeType, ref Dictionary<Int32, string> UniqueProps )
        {
            var NtProps = new LinkedList<CswNbtMetaDataNodeTypeProp>();
            // "??" == if(null == UniqueProps)
            var Props = UniqueProps ?? new Dictionary<int, string>();
            if( null != NodeType)
            {
                CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
                while( ThisVersionNodeType != null )
                {
                    foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>()
                                                                    .Where( ThisProp => !Props.ContainsValue( ThisProp.PropNameWithQuestionNo.ToLower() ) 
                                                                            && !Props.ContainsKey( ThisProp.FirstPropVersionId ) ) )
                    {
                        Props.Add( ThisProp.FirstPropVersionId, ThisProp.PropNameWithQuestionNo.ToLower() );
                        NtProps.AddLast( ThisProp );
                    }
                    ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
                }
            }
            UniqueProps = Props;
            return NtProps;
        } // _getNodeTypeProps()

        private IEnumerable<CswNbtMetaDataNodeTypeProp> _getObjectClassProps( CswNbtMetaDataObjectClass ObjectClass )
        {
            var OcProps = new LinkedList<CswNbtMetaDataNodeTypeProp>();
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
            if( null != ObjectClass )
            {
                //Iterate all NodeTypes and all versions of the NodeTypes to build complete list of NTPs
                foreach( IEnumerable<CswNbtMetaDataNodeTypeProp> NtProps in from CswNbtMetaDataNodeType NodeType 
                                                                            in ObjectClass.NodeTypes 
                                                                            select _getNodeTypeProps( NodeType, ref UniqueProps ) )
                {
                    OcProps.Concat( NtProps );
                }
            }
            
            return OcProps;
        } // _getObjectClassProps()

        /// <summary>
        /// In the context of a NodeType based, retuns as:
        ///     <nodetypeprops>
        ///         <properties>
        ///             <select>
        ///                 <optgroup label="Specific Properties">
        ///                     <option value="1">Barcode</option>
        ///                 </optgroup>
        ///             </select>
        ///         </properties>
        ///         <propertyfilters>
        ///             <property propname="Barcode" fieldtype="Barcode">
        ///                 <defaultsubfield propname="Barcode>Equals</defaultsubfield>
        ///                 <subfields>
        ///                     <select id="filter_select">
        ///                         <option value="Equals">Equals</option>
        ///                     </select>
        ///                 </subfields>
        ///                 <filters>
        ///                     <subfield propname="Barcode">Field1
        ///                         <select id="filter_select">
        ///                             <option value="Equals">Equals</option>
        ///                         </select>
        ///                     </subfield>
        ///                 </filters>
        ///             </property>   
        ///         </propertyfilters>
        ///     </nodetypeprops>
        /// </summary>
        public XElement getNodeTypeProps( string RelatedIdType, string ObjectPk )
        {
            XElement NodeTypePropsNode = new XElement( "nodetypeprops" );
            Int32 ObjectId = CswConvert.ToInt32( ObjectPk );
            if( Int32.MinValue != ObjectId )
            {
                IEnumerable<CswNbtMetaDataNodeTypeProp> NodeTypeProps = null;
                if( RelatedIdType.ToLower() == "nodetype" )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ObjectId );
                    Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                    NodeTypeProps = _getNodeTypeProps( NodeType, ref UniqueProps );
                }
                else if( RelatedIdType.ToLower() == "objectclass")
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectId );
                    NodeTypeProps = _getObjectClassProps( ObjectClass );
                }

                string DefaultPropName = string.Empty;
                XElement FiltersNode = new XElement( "propertyfilters" );
                XElement NodeTypePropsGroup = new XElement( "optgroup", new XAttribute( "label", "Specific Properties" ) );
                XElement ObjectClassPropsGroup = new XElement( "optgroup", new XAttribute( "label", "Generic Properties" ) );

                if( null != NodeTypeProps )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeProps )
                    {
                        var PropNode = new XElement( "option",
                                                     new XAttribute( "label", RelatedIdType ),
                                                     new XAttribute( "id", Prop.FirstPropVersionId ));
                        PropNode.Value = Prop.PropName;
                        if( Prop == NodeTypeProps.First() )
                        {
                            PropNode.Add( new XAttribute( "selected", "selected" ) );
                            DefaultPropName = Prop.PropName;
                        }
                        if( Int32.MinValue != Prop.FirstPropVersionId && "nodetype" == RelatedIdType )
                        {
                            PropNode.Add( new XAttribute( "value", Prop.FirstPropVersionId ) );
                            NodeTypePropsGroup.Add( PropNode );
                        }
                        else if( Int32.MinValue != Prop.ObjectClassPropId && "objectclass" == RelatedIdType )
                        {
                            PropNode.Add( new XAttribute( "value", Prop.ObjectClassPropId ) );
                            ObjectClassPropsGroup.Add( PropNode );
                        }

                        _getPropSubFields( ref FiltersNode, Prop );
                    }
                    
                    NodeTypePropsNode.Add( new XElement("properties",
                                                new XAttribute( "defaultprop", DefaultPropName),
                                                new XElement( "select",
                                                    new XAttribute( "id", "properties_select" ),
                                                    new XAttribute( "name", "properties_select" ),
                                                    ( NodeTypePropsGroup.HasElements ) ? NodeTypePropsGroup : new XElement( "optgroup" ),
                                                    ( ObjectClassPropsGroup.HasElements ) ? ObjectClassPropsGroup : new XElement( "optgroup" ) ) ), 
                                                FiltersNode );
                }
            }
            return NodeTypePropsNode;
        } // getNodeTypeProps()

        /// <summary>
        /// Returns the Subfields XML for a SubFields collection as:
        ///     <propertyfilters>
        ///         <property propname="Barcode" fieldtype="Barcode">
        ///             <defaultsubfield propname="Barcode>Equals</defaultsubfield>
        ///             <subfields>
        ///                 <select id="filter_select">
        ///                     <option value="Equals">Equals</option>
        ///                 </select>
        ///             </subfields>
        ///             <filters>
        ///                 <subfield propname="Barcode">Field1
        ///                     <select id="filter_select">
        ///                         <option value="Equals">Equals</option>
        ///                     </select>
        ///                 </subfield>
        ///             </filters>
        ///          </property>   
        ///      </propertyfilters>
        /// </summary>
        private void _getPropSubFields( ref XElement ParentNode, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            CswNbtSubFieldColl SubFields = NodeTypeProp.FieldTypeRule.SubFields;
            CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = NodeTypeProp.FieldType.FieldType;

            XElement SubfieldSelect = new XElement( "select", new XAttribute( "id", "subfield_select" ), new XAttribute( "name", "subfield_select" ) );
            
            string DefaultFilter = string.Empty;  
            string DefaultSubfield = string.Empty; 
            if(null != NodeTypeProp.FieldTypeRule.SubFields.Default)
            {
                DefaultFilter = NodeTypeProp.FieldTypeRule.SubFields.Default.Name.ToString();
                DefaultSubfield = NodeTypeProp.FieldTypeRule.SubFields.Default.Column.ToString();
            }
            XElement FiltersNode = new XElement( "propertyfilters" ) { Value = NodeTypeProp.PropName };

            foreach( CswNbtSubField Field in SubFields )
            {
                XElement FieldNode = new XElement( "option",
                                                 new XAttribute( "label", SelectedFieldType ),
                                                 new XAttribute( "value", Field.Column ),
                                                 new XAttribute( "id", Field.Column ) );
                FieldNode.Value = Field.Name.ToString();
                if( Field.Name == NodeTypeProp.FieldTypeRule.SubFields.Default.Name )
                {
                    FieldNode.Add( new XAttribute( "selected", "selected" ) );
                }

                if( !string.IsNullOrEmpty( DefaultFilter ) )
                {
                    DefaultFilter = Field.DefaultFilterMode.ToString();
                }
                
                _getSubFieldFilters( ref FiltersNode, Field, NodeTypeProp );
                SubfieldSelect.Add( FieldNode );
            }

            XElement FiltersOptionsNode = new XElement( "filtersoptions" );
            if( NodeTypeProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
            {
                FiltersOptionsNode.Value = NodeTypeProp.PropName;
                FiltersOptionsNode.Add( new XElement( "select",
                                                      new XAttribute( "id", "filtersoptions_select" ),
                                                      new XAttribute( "name", "filtersoptions_select" ),
                                                      _getFilterOptions( NodeTypeProp ) ) );
            }



            XElement ThisProp = new XElement( "property" ) { Value = NodeTypeProp.PropName };
            ThisProp.Add( new XAttribute( "propname", NodeTypeProp.PropName ),
                          new XAttribute( "propid", NodeTypeProp.PropId ),
                          new XAttribute( "fieldtype", NodeTypeProp.FieldType.FieldType ),
                            new XElement( "defaultsubfield",
                                        new XAttribute( "filter", DefaultFilter ),
                                        new XAttribute( "subfield", DefaultSubfield ) ),
                            new XElement( "subfields",
                                        SubfieldSelect ),
                            FiltersNode,
                            FiltersOptionsNode
                          );
            ParentNode.Add( ThisProp );
            

        } // _getPropSubFields()

        /// <summary>
        /// Returns the XML for For SubFields Filters as:
        ///     <filters>
        ///         <subfield propname="Barcode">Field1
        ///             <select id="filter_select">
        ///                 <option value="Equals">Equals</option>
        ///             </select>
        ///         </subfield>
        ///      </filters>
        /// </summary>
        private void _getSubFieldFilters( ref XElement FiltersNode, CswNbtSubField SubField, CswNbtMetaDataNodeTypeProp SelectedProp )
        {
            CswNbtPropFilterSql.PropertyFilterMode FilterModeToSelect = SubField.DefaultFilterMode;
            XElement SubFieldNode = new XElement( "subfield", new XAttribute( "column", SubField.Column ), new XAttribute( "name", SubField.Name ) );
            XElement FiltersSelect = new XElement( "select", new XAttribute( "id", "filter_select" ), new XAttribute( "name", "filter_select" ) );
            foreach(  CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt  in SubField.SupportedFilterModes )
            {
                XElement ThisFilter = new XElement( "option",
                                                    new XAttribute( "value", FilterModeOpt ),
                                                    new XAttribute( "label", SelectedProp.PropName ) );
                ThisFilter.Value = FilterModeOpt.ToString();                                                    
                if( FilterModeOpt == FilterModeToSelect )
                {
                    ThisFilter.Add( new XAttribute( "selected", "selected" ) );
                }
                FiltersSelect.Add( ThisFilter );
            }
            SubFieldNode.Add( FiltersSelect );
            FiltersNode.Add( SubFieldNode );
        } // _getSubFieldFilters()

        /// <summary>
        /// Sets fieldtype attributes on the Filter node and appends an XElement containing extended attributes, such as an options picklist
        /// </summary>
        private XElement _getFilterOptions( CswNbtMetaDataNodeTypeProp SelectedProp )
        {
            XElement FilterOptions = new XElement( "options", new XAttribute("propname", SelectedProp.PropName.ToLower() ) );

            var CswNbtNodeTypePropListOptions = new PropTypes.CswNbtNodeTypePropListOptions( _CswNbtResources, SelectedProp.PropId );
            foreach( var Option in CswNbtNodeTypePropListOptions.Options )
            {
                XElement OptionNode = new XElement( "option",
                                    new XAttribute( "value", Option.Value ),
                                    new XAttribute( "id", Option.Text ));
                OptionNode.Value = Option.Text;
                if( SelectedProp.DefaultValue.AsList.Value == Option.Value )
                {
                    OptionNode.Add( new XAttribute( "selected", "selected" ) );
                }
            }
            return FilterOptions;

        } // _getFilterInitValue()

        #endregion

        #region Get Search XML

        /// <summary>
        /// Returns the XML for filtered (searchable) View properties, if the View is searchable.
        /// Else, returns XML for a NodeTypeSelect.
        /// </summary>
        public XElement getSearchXml( string ViewIdNum, string SelectedNodeTypeIdNum )
        {
            var SearchNode = new XElement( "search", new XAttribute( "searchtype", "viewsearch" ) );
            
            Int32 ViewId = CswConvert.ToInt32( ViewIdNum );
            Int32 SelectedNodeTypeId = CswConvert.ToInt32( SelectedNodeTypeIdNum );
            CswNbtView View = null;
            if( Int32.MinValue != ViewId )
            {
                View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
            }

            if( null == View || !View.IsSearchable() )
            {
                SearchNode = _getNodeTypeBasedSearch( SelectedNodeTypeId );
            }
            else
            {
                foreach( CswNbtViewProperty Prop in View.getOrderedViewProps().Where(Prop => Prop.Filters.Count > 0 ) )
                {
                    XElement PropNode = new XElement( "viewprop",
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
                    
                    XElement SubFields = new XElement( "subfields" );
                    _getPropSubFields( ref SubFields, Prop.NodeTypeProp );
                    
                    PropNode.Add( SubFields );
                    SearchNode.Add( PropNode );
                }
            }
            return SearchNode;
        } // getViewBasedSearch()
        
        #endregion Get Search XML

        #region Execute Search

        /// <summary>
        /// Takes a View and applies search parameters as ViewPropertyFilters.
        /// Returns the modified View for processing as Tree/Grid/List.
        /// </summary>
        public CswNbtView doViewBasedSearch( string SearchXml, string ViewIdNum)
        {
            CswNbtView SearchView = null;
            if( !string.IsNullOrEmpty( SearchXml ) && null != ViewIdNum )
            {
                Int32 ViewId = CswConvert.ToInt32( ViewIdNum );
                CswNbtView InitialView = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                SearchView = new CswNbtView( _CswNbtResources );
                SearchView.LoadXml( InitialView.ToXml() );
                SearchView.ViewName = "Search " + InitialView.ViewName;

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
                SearchView = new CswNbtView( _CswNbtResources ) {ViewMode = NbtViewRenderingMode.Tree};

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
