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
	    private readonly string _IdPrefix = string.Empty;
	    private readonly string _Delimiter = "_";
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
	            return InvalidFieldTypes;
	        }

	    }

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources )
        {
			_CswNbtResources = CswNbtResources;
		}//ctor

        public CswNbtWebServiceSearch( CswNbtResources CswNbtResources, string IdPrefix)
        {
            _IdPrefix = IdPrefix;
            _CswNbtResources = CswNbtResources;
        }//ctor

        #region Generic Search Form XML

	    private string _NodeKey;
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

                var NodeTypes = (IEnumerable<CswNbtMetaDataNodeType>) SearchOC.NodeTypes;
                SelectedNodeType = NodeTypes.First().LatestVersionNodeType;
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
                Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                IEnumerable<CswSearchProp> SearchProps = _getNodeTypeProps( SelectedNodeType, ref UniqueProps );
                NodeTypeProps = _getSearchProps( SearchProps, CswNbtViewRelationship.RelatedIdType.NodeTypeId, SelectedNodeType.NodeTypeId );
            }
            else
            {
                IEnumerable<CswSearchProp> SearchProps = _getObjectClassProps( SearchOC );
                NodeTypeProps = _getSearchProps( SearchProps, CswNbtViewRelationship.RelatedIdType.ObjectClassId, SearchOC.ObjectClassId );
            }

            XElement NodeTypeSearch = new XElement( "search", 
                                            new XAttribute( "searchtype", "nodetypesearch" ), 
                                            new XElement( "nodetypes", 
                                                new XElement( "select", 
                                                    new XAttribute( "id", "node_type_select" ), 
                                                    new XAttribute( "name", "node_type_select" ),
                                                    new XAttribute( "class", "csw_search_node_type_select" ),
                                                        NodeTypeSelect,
                                                        ObjectClassSelect)),
                                            NodeTypeProps);
            return NodeTypeSearch;
        } // getNodeTypeBasedSearch()

        private IEnumerable<CswSearchProp> _getNodeTypeProps( CswNbtMetaDataNodeType NodeType, ref Dictionary<Int32, string> UniqueProps )
        {
            var NtProps = new List<CswSearchProp>();
            // "??" == if(null == UniqueProps)
            var Props = UniqueProps ?? new Dictionary<int, string>();
            if( null != NodeType)
            {
                CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
                while( ThisVersionNodeType != null )
                {
                    foreach( CswSearchProp SearchProp in ThisVersionNodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>()
                                                         .Where( ThisProp => !Props.ContainsValue( ThisProp.PropNameWithQuestionNo.ToLower() ) && 
                                                                 !Props.ContainsKey( ThisProp.FirstPropVersionId ) )
                                                         .Select( ThisProp => new CswSearchProp( ThisProp ) ) )
                    {
                        Props.Add( SearchProp.MetaDataPropId, SearchProp.MetaDataPropNameWithQuestionNo.ToLower() );
                        NtProps.Add( SearchProp );
                    }
                    ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
                }
            }
            UniqueProps = Props;
            return NtProps;
        } // _getNodeTypeProps()

        private IEnumerable<CswSearchProp> _getObjectClassProps( CswNbtMetaDataObjectClass ObjectClass )
        {
            var OcProps = new List<CswSearchProp>();
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
            if( null != ObjectClass )
            {
                //Iterate all NodeTypes and all versions of the NodeTypes to build complete list of NTPs
                foreach( IEnumerable<CswSearchProp> SearchProps in from CswNbtMetaDataNodeType NodeType 
                                                                            in ObjectClass.NodeTypes 
                                                                            select _getNodeTypeProps( NodeType, ref UniqueProps ) )
                {
                    OcProps.AddRange( SearchProps );
                }
            }
            var ReturnProps = from Prop in OcProps 
                              group Prop by Prop into PropGroup 
                              where PropGroup.Count() == 1 
                              select PropGroup.Key;
            return ReturnProps;
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
        public XElement getSearchProps( string RelatedIdType, string ObjectPk, string NodeKey )
        {
            XElement Props = new XElement( "nodetypeprops" );
            Int32 ObjectId = Int32.MinValue;
            CswNbtViewRelationship.RelatedIdType Relationship = CswNbtViewRelationship.RelatedIdType.Unknown;
            
            if( string.IsNullOrEmpty( ObjectPk ) && !string.IsNullOrEmpty( NodeKey ) )
            {
                string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( NodeKey );
                CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];
                if( null != Node.NodeType )
                {
                    ObjectId = Node.NodeTypeId;
                    Relationship = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                }
                else if( null != Node.ObjectClass )
                {
                    ObjectId = Node.ObjectClassId;
                    Relationship = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
                }
            }
            else if( !string.IsNullOrEmpty( ObjectPk ) )
            {
                ObjectId = CswConvert.ToInt32( ObjectPk );
                CswNbtViewRelationship.RelatedIdType.TryParse( RelatedIdType, out Relationship );
            }
            
            if( Int32.MinValue != ObjectId )
            {
                IEnumerable<CswSearchProp> SearchProperties = null;
                switch(Relationship)
                {
                    case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ObjectId );
                            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                            SearchProperties = _getNodeTypeProps( NodeType, ref UniqueProps );
                            break;
                        }

                    case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectId );
                            SearchProperties = _getObjectClassProps( ObjectClass );
                            break;
                        }
                }

                Props = _getSearchProps( SearchProperties, Relationship, ObjectId );
            }
            return Props; //NodeTypePropsNode;
        } // getNodeTypeProps()

        private XElement _getSearchProps( IEnumerable<CswSearchProp> SearchProperties, CswNbtViewRelationship.RelatedIdType RelatedIdType, Int32 ObjectPk )
        {
            XElement NodeTypePropsNode = new XElement( "nodetypeprops" );
            string DefaultPropName = string.Empty;
            XElement FiltersNode = new XElement( "propertyfilters" );
            XElement NodeTypePropsGroup = new XElement( "optgroup", new XAttribute( "label", "Specific Properties" ) );
            XElement ObjectClassPropsGroup = new XElement( "optgroup", new XAttribute( "label", "Generic Properties" ) );

            if( null != SearchProperties )
            {
                foreach( CswSearchProp Prop in from SearchProp in SearchProperties
                                               where !_ProhibittedFieldTypes.Contains( SearchProp.FieldType )
                                               orderby SearchProp.MetaDataPropName
                                               select SearchProp )
                {
                    var PropNode = new XElement( "option", Prop.MetaDataPropName,
                                                 new XAttribute( "title", RelatedIdType ),
                                                 new XAttribute( "label", Prop.MetaDataPropName ), //for Chrome
                                                 new XAttribute( "id", Prop.MetaDataPropId ) );
                    if( Prop == SearchProperties.First() )
                    {
                        PropNode.Add( new XAttribute( "selected", "selected" ) );
                        DefaultPropName = Prop.MetaDataPropName;
                    }
                    PropNode.Add( new XAttribute( "value", Prop.MetaDataPropId ) );
                    switch( RelatedIdType )
                    {
                        case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                            NodeTypePropsGroup.Add( PropNode );
                            break;
                        case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                            ObjectClassPropsGroup.Add( PropNode );
                            break;
                    }

                    _getSearchPropSubFieldsForNtp( ref FiltersNode, Prop );
                }
                string ElementId = _IdPrefix + _Delimiter + "properties_select_nodetypeid_" + ObjectPk;
                NodeTypePropsNode.Add( new XElement( "properties",
                                                     new XAttribute( "defaultprop", DefaultPropName ),
                                                     new XElement( "select",
                                                                   new XAttribute( "id", ElementId ),
                                                                   new XAttribute( "name", ElementId ),
                                                                   new XAttribute( "class", "csw_search_properties_select" ),
                                                                   ( NodeTypePropsGroup.HasElements ) ? NodeTypePropsGroup : null,
                                                                   ( ObjectClassPropsGroup.HasElements ) ? ObjectClassPropsGroup : null ) ),
                                       FiltersNode );
            }
            return NodeTypePropsNode;
        }

	    /// <summary>
        /// Returns the Subfields XML for a SubFields collection as:
        ///     <propertyfilters>
        ///         <property propname="Barcode" fieldtype="Barcode" relatedidtype="nodetypeprop" propid="1">
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
        private void _getSearchPropSubFieldsForNtp( ref XElement ParentNode, CswSearchProp SearchProp )
        {
            CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = SearchProp.FieldType.FieldType;
            CswNbtSubFieldColl SubFields = SearchProp.FieldTypeRule.SubFields;
            string SubFieldsElementId = _IdPrefix + _Delimiter + "subfield_select_searchpropid_" + SearchProp.MetaDataPropId;
            XElement SubfieldSelect = new XElement( "select",
                                                    new XAttribute( "id", SubFieldsElementId ),
                                                    new XAttribute( "name", SubFieldsElementId ),
                                                    new XAttribute( "class", "csw_search_subfield_select" ) );

            string DefaultFilter = string.Empty;
            string DefaultSubfield = string.Empty;
            if( null != SearchProp.FieldTypeRule.SubFields.Default )
            {
                DefaultFilter = SearchProp.FieldTypeRule.SubFields.Default.Name.ToString();
                DefaultSubfield = SearchProp.FieldTypeRule.SubFields.Default.Column.ToString();
            }
            XElement FiltersNode = new XElement( "propertyfilters", SearchProp.MetaDataPropName );

            foreach( CswNbtSubField Field in SubFields )
            {
                XElement FieldNode = new XElement( "option",
                                                   new XAttribute( "title", SelectedFieldType ),
                                                   new XAttribute( "label", Field.Name ), // for Chrome
                                                   new XAttribute( "value", Field.Column ),
                                                   new XAttribute( "id", Field.Column ) );
                FieldNode.Value = Field.Name.ToString();
                if( Field.Name == SearchProp.FieldTypeRule.SubFields.Default.Name )
                {
                    FieldNode.Add( new XAttribute( "selected", "selected" ) );
                }

                if( !string.IsNullOrEmpty( DefaultFilter ) )
                {
                    DefaultFilter = Field.DefaultFilterMode.ToString();
                }

                _getSubFieldFilters( ref FiltersNode, Field, SearchProp, CswNbtPropFilterSql.PropertyFilterMode.Undefined );
                SubfieldSelect.Add( FieldNode );
            }

            XElement FiltersOptionsNode = new XElement( "filtersoptions" );
            if( SearchProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
            {
                string FiltOptElementId = _IdPrefix + _Delimiter + "filtersoptions_select_searchpropid_" + SearchProp.MetaDataPropId;
                FiltersOptionsNode.Value = SearchProp.MetaDataPropName;
                FiltersOptionsNode.Add( new XElement( "select",
                                                      new XAttribute( "id", FiltOptElementId ),
                                                      new XAttribute( "name", FiltOptElementId ),
                                                      new XAttribute( "class", "csw_search_filtersoptions_select" ),
                                                      _getFilterOptions( SearchProp, string.Empty ) ) );
            }

            ParentNode.Add( new XElement( "property", SearchProp.MetaDataPropName,
                                          new XAttribute( "propname", SearchProp.MetaDataPropName ),
                                          new XAttribute( "propid", SearchProp.MetaDataPropId ),
                                          new XAttribute( "relatedidtype", SearchProp.RelatedIdType ),
                                          new XAttribute( "proptype", SearchProp.Type ),
                                          new XAttribute( "metadatatypename", SearchProp.MetaDataTypeName ),
                                          new XAttribute( "fieldtype", SearchProp.FieldType.FieldType ),
                                          new XElement( "defaultsubfield",
                                                        new XAttribute( "filter", DefaultFilter ),
                                                        new XAttribute( "subfield", DefaultSubfield ) ),
                                          new XElement( "subfields",
                                                        SubfieldSelect ),
                                          FiltersNode,
                                          FiltersOptionsNode )
                );
        } // _getSearchPropSubFieldsForNtp()

        private void _getSearchPropSubFieldsForVp( ref XElement ParentNode, CswSearchProp SearchProp, ArrayList PropFilters )
        {
            CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = SearchProp.FieldType.FieldType;
            foreach( CswNbtViewPropertyFilter Filter in PropFilters )
            {
                string SubFieldElementId = _IdPrefix + _Delimiter + "subfield_select_searchpropid_" + SearchProp.MetaDataPropId;

                XElement SubfieldSelect = new XElement( "select",
                                                        new XAttribute( "id", SubFieldElementId ),
                                                        new XAttribute( "name", SubFieldElementId ),
                                                        new XAttribute( "class", "csw_search_subfield_select" ) );

                CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode = Filter.FilterMode;
                string DefaultSubfield = Filter.SubfieldName.ToString();
                string ValueSubfieldVal = Filter.Value;

                XElement FiltersNode = new XElement( "propertyfilters", SearchProp.MetaDataPropName );


                foreach( CswNbtSubField Field in SearchProp.FieldTypeRule.SubFields )
                {
                    XElement FieldNode = new XElement( "option", Field.Name.ToString(),
                                                       new XAttribute( "title", SelectedFieldType ),
                                                       new XAttribute( "label", Field.Name ), // for Chrome
                                                       new XAttribute( "value", Field.Column ),
                                                       new XAttribute( "defaultvalue", Filter.Value ),
                                                       new XAttribute( "arbitraryid", Filter.ArbitraryId ),
                                                       new XAttribute( "id", Field.Column ) );
                    if( Field.Name == Filter.SubfieldName )
                    {
                        FieldNode.Add( new XAttribute( "selected", "selected" ) );
                    }

                    _getSubFieldFilters( ref FiltersNode, Field, SearchProp, DefaultFilterMode );
                    SubfieldSelect.Add( FieldNode );
                }


                XElement FiltersOptionsNode = new XElement( "filtersoptions" );
                if( SearchProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    string FiltOptElementId = _IdPrefix + _Delimiter + "filtersoptions_select_searchpropid_" + SearchProp.MetaDataPropId;
                    FiltersOptionsNode.Value = SearchProp.MetaDataPropName;
                    FiltersOptionsNode.Add( new XElement( "select",
                                                          new XAttribute( "id", FiltOptElementId ),
                                                          new XAttribute( "name", FiltOptElementId ),
                                                          new XAttribute( "class", "csw_search_filtersoptions_select" ),
                                                          _getFilterOptions( SearchProp, ValueSubfieldVal ) ) );
                }

                ParentNode.Add( new XElement( "property", SearchProp.MetaDataPropName,
                                              new XAttribute( "propname", SearchProp.MetaDataPropName ),
                                              new XAttribute( "propid", SearchProp.MetaDataPropId ),
                                              new XAttribute( "relatedidtype", SearchProp.RelatedIdType ),
                                              new XAttribute( "proptype", SearchProp.Type ),
                                              new XAttribute( "metadatatypename", SearchProp.MetaDataTypeName ),
                                              new XAttribute( "fieldtype", SearchProp.FieldType.FieldType ),
                                              new XAttribute( "proparbitraryid", SearchProp.ViewProp.ArbitraryId ),
                                              new XAttribute( "filtarbitraryid", Filter.ArbitraryId ),
                                              new XElement( "defaultsubfield",
                                                            new XAttribute( "filter", DefaultFilterMode.ToString() ),
                                                            new XAttribute( "subfield", DefaultSubfield ) ),
                                              new XElement( "subfields",
                                                            SubfieldSelect ),
                                              FiltersNode,
                                              FiltersOptionsNode )
                    );
            }
        }

	    // _getSearchPropSubFieldsForVp()

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
        private void _getSubFieldFilters( ref XElement FiltersNode, CswNbtSubField SubField, CswSearchProp SearchProp, CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode )
        {
            if( DefaultFilterMode == CswNbtPropFilterSql.PropertyFilterMode.Undefined )
            {
                DefaultFilterMode = SubField.DefaultFilterMode;
            }
            string SubFieldElementId = _IdPrefix + _Delimiter + "filter_select_searchpropid_" + SearchProp.MetaDataPropId;
            XElement SubFieldNode = new XElement( "subfield", new XAttribute( "column", SubField.Column ), new XAttribute( "name", SubField.Name ) );
            XElement FiltersSelect = new XElement( "select",
                                        new XAttribute( "id", SubFieldElementId ),
                                        new XAttribute( "name", SubFieldElementId ),
                                        new XAttribute( "class", "csw_search_filter_select" ) );
            foreach(  CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt  in SubField.SupportedFilterModes )
            {
                XElement ThisFilter = new XElement( "option", FilterModeOpt.ToString(),                                                   
                                                    new XAttribute( "value", FilterModeOpt ),
                                                    new XAttribute( "title", SearchProp.MetaDataPropName ),
                                                    new XAttribute( "label", FilterModeOpt ) ); // for Chrome

                if( FilterModeOpt == DefaultFilterMode )
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
        private XElement _getFilterOptions( CswSearchProp SearchProp, string FilterSelected )
        {
            XElement FilterOptions = new XElement( "options", new XAttribute( "propname", SearchProp.MetaDataPropName.ToLower() ) );

            foreach( string Option in SearchProp.ListOptions )
            {
                FilterOptions.Add( new XElement( "option", Option,
                                    new XAttribute( "value", Option ),
                                    new XAttribute( "id", SearchProp.MetaDataPropName + _Delimiter + Option ),
                                    (Option == FilterSelected) ? new XAttribute( "selected","selected" ) : null ) 
                                  );
            }
            return FilterOptions;

        } // _getFilterInitValue()

        #endregion

        #region Get Search XML

        /// <summary>
        /// Returns the XML for filtered (searchable) View properties, if the View is searchable.
        /// Else, returns XML for a NodeTypeSelect.
        /// </summary>
        public XElement getSearchXml( string ViewIdNum, string SelectedNodeTypeIdNum, string NodeKey )
        {
            XElement SearchNode = new XElement( "search", 
                                        new XAttribute( "searchtype", "viewsearch" ) );
            
            XElement PropNode = new XElement( "properties" );
            Int32 ViewId = CswConvert.ToInt32( ViewIdNum );
            
            CswNbtView View = null;
            if( Int32.MinValue != ViewId )
            {
                View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
            }

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
                foreach( CswSearchProp SearchProp in View.getOrderedViewProps()
                                                     .Where( Prop => Prop.Filters.Count > 0 &&
                                                        !_ProhibittedFieldTypes.Contains( Prop.FieldType ) )
                                                     .Select( Prop => new CswSearchProp( Prop ) ) )
                {

                    ArrayList ViewPropFilters = new ArrayList();
                    foreach( CswNbtViewPropertyFilter Filt in SearchProp.Filters )
                    {
                        ViewPropFilters.Add( Filt );
                    }
                    _getSearchPropSubFieldsForVp( ref PropNode, SearchProp, ViewPropFilters );
                }
                SearchNode.Add( PropNode );
            }

            return SearchNode;
        } // getViewBasedSearch()
        
        #endregion Get Search XML

        #region Execute Search

        /// <summary>
        /// Takes a View and applies search parameters as ViewPropertyFilters.
        /// Returns the modified View for processing as Tree/Grid/List.
        /// </summary>
        public CswNbtView doViewBasedSearch( string SearchJson )
        {
            CswNbtView SearchView = null;
            if( !string.IsNullOrEmpty( SearchJson ) )
            {
                JObject ViewSearch = JObject.Parse( SearchJson );
                string ViewIdNum = (string)ViewSearch.Property( "viewid" ).Value;
                Int32 ViewId = CswConvert.ToInt32( ViewIdNum );
                CswNbtView InitialView = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                SearchView = new CswNbtView( _CswNbtResources );
                SearchView.LoadXml( InitialView.ToXml() );
                if( InitialView.SessionViewId != Int32.MinValue &&
                    !InitialView.ViewName.StartsWith( "Search " ) )
                {
                    SearchView.ViewName = "Search " + InitialView.ViewName;
                }

                if( null != ViewSearch.Property( "viewprops" ) )
                {
                    foreach( var Prop in ViewSearch["viewprops"].Children() )
                    {
                        var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
                        CswNbtViewProperty.CswNbtPropType.TryParse( (string) Prop.First["proptype"], true, out PropType );
                        Int32 PropId = Int32.MinValue;

                        PropId = CswConvert.ToInt32( (string) Prop.First["propid"] );

                        if( PropType != CswNbtViewProperty.CswNbtPropType.Unknown &&
                            PropId != Int32.MinValue )
                        {
                            CswNbtViewProperty ViewProp = SearchView.findPropertyById( PropType, PropId );
                            CswNbtViewPropertyFilter ViewPropFilt = (CswNbtViewPropertyFilter) SearchView.FindViewNodeByArbitraryId( (string) Prop.First["arbitraryid"] );
                            if( null != ViewProp )
                            {
                                _addViewPropFilter( Prop, ref ViewPropFilt );
                            }
                        }
                    }
                }
            }
            return SearchView;
        }

        private void _addViewPropFilter( JToken JProp, ref CswNbtViewPropertyFilter ViewPropFilt )
        {
            var FieldName = CswNbtSubField.SubFieldName.Unknown;
            CswNbtSubField.SubFieldName.TryParse( (string)JProp.First["subfield"], true, out FieldName );
            var FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
            CswNbtPropFilterSql.PropertyFilterMode.TryParse( (string) JProp.First["filter"], true, out FilterMode );
            string SearchTerm = (string) JProp.First["searchtext"];
            
            ViewPropFilt.FilterMode = FilterMode;
            ViewPropFilt.SubfieldName = FieldName;
            ViewPropFilt.Value = SearchTerm;
        }

        /// <summary>
        /// If the search is based on NodeType/ObjectClass, construct a View with the included search terms as Property Filters.
        /// Return the View for processing as a Tree
        /// </summary>
        public CswNbtView doNodesSearch( string SearchJson )
        {
            JObject NodesSearch = new JObject();
            CswNbtView SearchView = null;
            string ViewName = string.Empty;
            if( !string.IsNullOrEmpty( SearchJson ) )
            {
                NodesSearch = JObject.Parse( SearchJson );
                //NodesSearch = XElement.Parse( SearchJson );
                SearchView = new CswNbtView( _CswNbtResources ) {ViewMode = NbtViewRenderingMode.Tree};

                var ViewNtRelationships = new Dictionary<CswNbtMetaDataNodeType, CswNbtViewRelationship>();
                var ViewOcRelationships = new Dictionary<CswNbtMetaDataObjectClass, CswNbtViewRelationship>();

                foreach( JToken Ntp in NodesSearch["nodetypeprops"].Children() )
                {
                    var PropType = CswNbtViewRelationship.RelatedIdType.Unknown;
                    CswNbtViewRelationship.RelatedIdType.TryParse( (string)Ntp.First["relatedidtype"], true, out PropType );
                    Int32 ObjectPk = CswConvert.ToInt32( (string) Ntp.First["objectpk"] );
                    Int32 PropId = CswConvert.ToInt32( (string) Ntp.First["propid"] );
                    CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId );
                    if( PropType == CswNbtViewRelationship.RelatedIdType.ObjectClassId &&
                        Int32.MinValue != NodeTypeProp.ObjectClassPropId )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectPk );
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
                            _addViewPropFilter( Ntp, ref ViewOcPropFilt );
                        }
                    }
                    else
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ObjectPk );
                        if( string.IsNullOrEmpty( ViewName ) ) ViewName = NodeType.NodeTypeName + " Search";
                        if(NodeTypeProp.NodeType == NodeType)
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
                            _addViewPropFilter( Ntp, ref ViewNtPropFilt );
                        }
                    }
                }

            }
            if( string.IsNullOrEmpty( ViewName ) ) ViewName = "No Results for Search";
            SearchView.ViewName = ViewName;
            return SearchView;
        }

        #endregion

        private class CswSearchProp
        {
            private Int32 _MetaDataPropId = Int32.MinValue;
            public Int32 MetaDataPropId
            {
                get { return _MetaDataPropId; }
            }

            private CswNbtViewProperty _ViewProp = null;
            public CswNbtViewProperty ViewProp
            {
                get { return _ViewProp; }
            }

            private string _MetaDataPropName = string.Empty;
            public string MetaDataPropName
            {
                get { return _MetaDataPropName; }
            }

            private string _MetaDataPropNameWithQuestionNo = string.Empty;
            public string MetaDataPropNameWithQuestionNo
            {
                get { return _MetaDataPropNameWithQuestionNo; }
            }

            private string _MetaDataTypeName = string.Empty;
            public string MetaDataTypeName
            {
                get { return _MetaDataTypeName; }
            }

            private CswNbtMetaDataFieldType _FieldType;
            public CswNbtMetaDataFieldType FieldType
            {
                get { return _FieldType; }
            }

            private ICswNbtFieldTypeRule _FieldTypeRule = null;
            public ICswNbtFieldTypeRule FieldTypeRule
            {
                get { return _FieldTypeRule; }
            }
           
            private CswCommaDelimitedString _ListOptions = new CswCommaDelimitedString();
            public CswCommaDelimitedString ListOptions
            {
                get { return _ListOptions; }
            }

            private CswNbtViewProperty.CswNbtPropType _Type = CswNbtViewProperty.CswNbtPropType.Unknown;
            public CswNbtViewProperty.CswNbtPropType Type
            {
                get { return _Type; }
            }

            private CswNbtViewRelationship.RelatedIdType _RelatedIdType = CswNbtViewRelationship.RelatedIdType.Unknown;
            public CswNbtViewRelationship.RelatedIdType RelatedIdType
            {
                get { return _RelatedIdType; }
            }
            private ArrayList _Filters = new ArrayList();
            public ArrayList Filters
            {
                get { return _Filters; }
            }

            public CswSearchProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
            {
                _FieldType = NodeTypeProp.FieldType;
                _ListOptions.FromString( NodeTypeProp.ListOptions );
                _RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                _MetaDataPropNameWithQuestionNo = NodeTypeProp.PropNameWithQuestionNo;
                _MetaDataPropId = NodeTypeProp.FirstPropVersionId;
                _MetaDataPropName = NodeTypeProp.PropName;
                _MetaDataTypeName = NodeTypeProp.NodeType.NodeTypeName;
                _FieldTypeRule = NodeTypeProp.FieldTypeRule;
                _Type = CswNbtViewProperty.CswNbtPropType.NodeTypePropId;
            } //ctor Ntp

            public CswSearchProp( CswNbtViewProperty ViewProp )
            {
                if( ViewProp.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId && 
                    null != ViewProp.NodeTypeProp)
                {
                    _FieldType = ViewProp.NodeTypeProp.FieldType;
                    _ListOptions.FromString( ViewProp.NodeTypeProp.ListOptions );
                    _RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                    _MetaDataPropNameWithQuestionNo = ViewProp.NodeTypeProp.PropNameWithQuestionNo;
                    _MetaDataPropId = ViewProp.NodeTypeProp.FirstPropVersionId;
                    _MetaDataPropName = ViewProp.NodeTypeProp.PropName;
                    _MetaDataTypeName = ViewProp.NodeTypeProp.NodeType.NodeTypeName;
                    _FieldTypeRule = ViewProp.NodeTypeProp.FieldTypeRule;
                }
                else if( ViewProp.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId &&
                    null != ViewProp.ObjectClassProp )
                {
                    _FieldType = ViewProp.ObjectClassProp.FieldType;
                    _ListOptions.FromString( ViewProp.ObjectClassProp.ListOptions );
                    _RelatedIdType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
                    _MetaDataPropNameWithQuestionNo = ViewProp.ObjectClassProp.PropNameWithQuestionNo;
                    _MetaDataPropId = ViewProp.ObjectClassProp.ObjectClassPropId;
                    _MetaDataPropName = ViewProp.ObjectClassProp.PropName;
                    _MetaDataTypeName = ViewProp.ObjectClassProp.ObjectClass.ObjectClass.ToString().Replace( "Class", "" );
                    _FieldTypeRule = ViewProp.ObjectClassProp.FieldTypeRule;
                }
                _ViewProp = ViewProp;
                _FieldType = ViewProp.FieldType;
                _Filters = ViewProp.Filters;
                _Type = ViewProp.Type;                    
            } //ctor Vp

        }

    } // class CswNbtWebServiceSearch

    

} // namespace ChemSW.Nbt.WebServices
