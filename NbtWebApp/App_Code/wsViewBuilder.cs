using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    #region wsViewBuilder
    public class wsViewBuilder : System.Web.Services.WebService
    {
        private readonly CswNbtResources _CswNbtResources;
        //private CswNbtView _View;
        private readonly ArrayList _ProhibittedFieldTypes;
        private readonly string _Prefix = "csw";

        public wsViewBuilder( CswNbtResources CswNbtResources, ArrayList ProhibittedFieldTypes, string Prefix = null)
        {
            _CswNbtResources = CswNbtResources;
            //_View = View;
            _ProhibittedFieldTypes = ProhibittedFieldTypes;
            if( !string.IsNullOrEmpty( Prefix ) )
            {
                _Prefix = Prefix;
            }
        } //ctor

        public wsViewBuilder( CswNbtResources CswNbtResources, string Prefix = null)
        {
            _CswNbtResources = CswNbtResources;
            //_View = View;
            _ProhibittedFieldTypes = new ArrayList();
            if( !string.IsNullOrEmpty( Prefix ) )
            {
                _Prefix = Prefix;
            }
        } //ctor

        #region Private Assembly Methods

        private IEnumerable<CswViewBuilderProp> _getNodeTypeProps( CswNbtMetaDataNodeType NodeType, ref Dictionary<Int32, string> UniqueProps )
        {
            var NtProps = new List<CswViewBuilderProp>();
            // "??" == if(null == UniqueProps)
            var Props = UniqueProps ?? new Dictionary<int, string>();
            if( null != NodeType )
            {
                CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
                while( ThisVersionNodeType != null )
                {
                    foreach( CswViewBuilderProp ViewBuilderProp in ThisVersionNodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>()
                                                         .Where( ThisProp => !Props.ContainsValue( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                                                                 !Props.ContainsKey( ThisProp.FirstPropVersionId ) )
                                                         .Select( ThisProp => new CswViewBuilderProp( ThisProp ) ) )
                    {
                        Props.Add( ViewBuilderProp.MetaDataPropId, ViewBuilderProp.MetaDataPropNameWithQuestionNo.ToLower() );
                        NtProps.Add( ViewBuilderProp );
                    }
                    ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
                }
            }
            UniqueProps = Props;
            return NtProps;
        } // _getNodeTypeProps()

        private IEnumerable<CswViewBuilderProp> _getObjectClassProps( CswNbtMetaDataObjectClass ObjectClass )
        {
            var OcProps = new List<CswViewBuilderProp>();
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
            if( null != ObjectClass )
            {
                //Iterate all NodeTypes and all versions of the NodeTypes to build complete list of NTPs
                foreach( IEnumerable<CswViewBuilderProp> ViewBuilderProps in from CswNbtMetaDataNodeType NodeType
                                                                            in ObjectClass.NodeTypes
                                                                        select _getNodeTypeProps( NodeType, ref UniqueProps ) )
                {
                    OcProps.AddRange( ViewBuilderProps );
                }
            }
            var ReturnProps = from Prop in OcProps
                              group Prop by Prop into PropGroup
                              where PropGroup.Count() == 1
                              select PropGroup.Key;
            return ReturnProps;
        } // _getObjectClassProps()

        private XElement _getViewBuilderProps( IEnumerable<CswViewBuilderProp> ViewBuilderProperties, CswNbtViewRelationship.RelatedIdType RelatedIdType, Int32 ObjectPk )
        {
            XElement NodeTypePropsNode = new XElement( "nodetypeprops" );
            string DefaultPropName = string.Empty;
            XElement FiltersNode = new XElement( "propertyfilters" );
            XElement NodeTypePropsGroup = new XElement( "optgroup", new XAttribute( "label", "Specific Properties" ) );
            XElement ObjectClassPropsGroup = new XElement( "optgroup", new XAttribute( "label", "Generic Properties" ) );

            if( null != ViewBuilderProperties )
            {
                foreach( CswViewBuilderProp Prop in from ViewBuilderProp in ViewBuilderProperties
                                                    where !_ProhibittedFieldTypes.Contains( ViewBuilderProp.FieldType )
                                                    orderby ViewBuilderProp.MetaDataPropName
                                                    select ViewBuilderProp )
                {
                    var PropNode = new XElement( "option", Prop.MetaDataPropName,
                                                 new XAttribute( "title", RelatedIdType ),
                                                 new XAttribute( "label", Prop.MetaDataPropName ), //for Chrome
                                                 new XAttribute( "id", Prop.MetaDataPropId ) );
                    if( Prop == ViewBuilderProperties.First() )
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

                    _getViewBuilderPropSubFields( ref FiltersNode, Prop );
                }
                string ElementId = wsTools.makeId( _Prefix, "properties_select_nodetypeid", ObjectPk.ToString() );
                NodeTypePropsNode.Add( new XElement( "properties",
                                                     new XAttribute( "defaultprop", DefaultPropName ),
                                                     new XElement( "select",
                                                                   new XAttribute( "id", ElementId ),
                                                                   new XAttribute( "name", ElementId ),
                                                                   new XAttribute( "class", "csw_viewbuilder_properties_select" ),
                                                                   ( NodeTypePropsGroup.HasElements ) ? NodeTypePropsGroup : null,
                                                                   ( ObjectClassPropsGroup.HasElements ) ? ObjectClassPropsGroup : null ) ),
                                       FiltersNode );
            }
            return NodeTypePropsNode;
        }

        /// <summary>
        /// Returns the Subfields XML for a NodeTypeProp/ObjectClassProp's SubFields collection as:
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
        private void _getViewBuilderPropSubFields( ref XElement ParentNode, CswViewBuilderProp ViewBuilderProp )
        {
            if( null != ViewBuilderProp )
            {
                CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = ViewBuilderProp.FieldType.FieldType;
                CswNbtSubFieldColl SubFields = ViewBuilderProp.FieldTypeRule.SubFields;
                string SubFieldsElementId = wsTools.makeId( _Prefix, "subfield_select_viewbuilderpropid", ViewBuilderProp.MetaDataPropId.ToString() );
                XElement SubfieldSelect = new XElement( "select",
                                                        new XAttribute( "id", SubFieldsElementId ),
                                                        new XAttribute( "name", SubFieldsElementId ),
                                                        new XAttribute( "class", "csw_viewbuilder_subfield_select" ) );

                string DefaultFilter = string.Empty;
                string DefaultSubfield = string.Empty;
                if( null != ViewBuilderProp.FieldTypeRule.SubFields.Default )
                {
                    DefaultFilter = ViewBuilderProp.FieldTypeRule.SubFields.Default.Name.ToString();
                    DefaultSubfield = ViewBuilderProp.FieldTypeRule.SubFields.Default.Column.ToString();
                }
                XElement FiltersNode = new XElement( "propertyfilters", ViewBuilderProp.MetaDataPropName );

                foreach( CswNbtSubField Field in SubFields )
                {
                    XElement FieldNode = new XElement( "option",
                                                       new XAttribute( "title", SelectedFieldType ),
                                                       new XAttribute( "label", Field.Name ), // for Chrome
                                                       new XAttribute( "value", Field.Column ),
                                                       new XAttribute( "id", Field.Column ) );
                    FieldNode.Value = Field.Name.ToString();
                    if( Field.Name == ViewBuilderProp.FieldTypeRule.SubFields.Default.Name )
                    {
                        FieldNode.Add( new XAttribute( "selected", "selected" ) );
                    }

                    if( !string.IsNullOrEmpty( DefaultFilter ) )
                    {
                        DefaultFilter = Field.DefaultFilterMode.ToString();
                    }
                    string UniqueId = "filter_select_viewbuilderpropid_" + ViewBuilderProp.MetaDataPropId;
                    _getSubFieldFilters( ref FiltersNode, Field, ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode.Undefined, UniqueId );
                    SubfieldSelect.Add( FieldNode );
                }

                XElement FiltersOptionsNode = new XElement( "filtersoptions" );
                if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    string FiltOptElementId = wsTools.makeId( _Prefix, "filtersoptions_select_viewbuilderpropid", ViewBuilderProp.MetaDataPropId.ToString() );
                    FiltersOptionsNode.Value = ViewBuilderProp.MetaDataPropName;
                    FiltersOptionsNode.Add( new XElement( "select",
                                                          new XAttribute( "id", FiltOptElementId ),
                                                          new XAttribute( "name", FiltOptElementId ),
                                                          new XAttribute( "class", "csw_viewbuilder_filtersoptions_select" ),
                                                          _getFilterOptions( ViewBuilderProp, string.Empty ) ) );
                }

                ParentNode.Add( new XElement( "property", ViewBuilderProp.MetaDataPropName,
                                              new XAttribute( "propname", ViewBuilderProp.MetaDataPropName ),
                                              new XAttribute( "propid", ViewBuilderProp.MetaDataPropId ),
                                              new XAttribute( "relatedidtype", ViewBuilderProp.RelatedIdType ),
                                              new XAttribute( "proptype", ViewBuilderProp.Type ),
                                              new XAttribute( "metadatatypename", ViewBuilderProp.MetaDataTypeName ),
                                              new XAttribute( "fieldtype", ViewBuilderProp.FieldType.FieldType ),
                                              new XElement( "defaultsubfield",
                                                            new XAttribute( "filter", DefaultFilter ),
                                                            new XAttribute( "subfield", DefaultSubfield ) ),
                                              new XElement( "subfields",
                                                            SubfieldSelect ),
                                              FiltersNode,
                                              FiltersOptionsNode )
                    );
            }
        } // _getViewBuilderPropSubFields()

        /// <summary>
        /// Returns the Subfields XML for a CswNbtViewProp's SubFields collection as:
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
        private void _getViewBuilderPropSubFields( ref XElement ParentNode, CswViewBuilderProp ViewBuilderProp, ArrayList PropFilters )
        {
            if( null != ViewBuilderProp )
            { 
                CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = ViewBuilderProp.FieldType.FieldType;
                foreach( CswNbtViewPropertyFilter Filter in PropFilters )
                {
                    string SubFieldElementId = wsTools.makeId( _Prefix, "subfield_select_filtarbitraryid", Filter.ArbitraryId );

                    XElement SubfieldSelect = new XElement( "select",
                                                            new XAttribute( "id", SubFieldElementId ),
                                                            new XAttribute( "name", SubFieldElementId ),
                                                            new XAttribute( "class", "csw_viewbuilder_subfield_select" ) );

                    CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode = Filter.FilterMode;
                    string DefaultSubfield = Filter.SubfieldName.ToString();
                    string ValueSubfieldVal = Filter.Value;

                    XElement FiltersNode = new XElement( "propertyfilters", ViewBuilderProp.MetaDataPropName );


                    foreach( CswNbtSubField Field in ViewBuilderProp.FieldTypeRule.SubFields )
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
                        string UniqueId = "filter_select_filtarbitraryid_" + Filter.ArbitraryId;
                        _getSubFieldFilters( ref FiltersNode, Field, ViewBuilderProp, DefaultFilterMode, UniqueId );
                        SubfieldSelect.Add( FieldNode );
                    }


                    XElement FiltersOptionsNode = new XElement( "filtersoptions" );
                    if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                    {
                        string FiltOptElementId = wsTools.makeId( _Prefix, "filtersoptions_select_filtarbitraryid", Filter.ArbitraryId );
                        FiltersOptionsNode.Value = ViewBuilderProp.MetaDataPropName;
                        FiltersOptionsNode.Add( new XElement( "select",
                                                              new XAttribute( "id", FiltOptElementId ),
                                                              new XAttribute( "name", FiltOptElementId ),
                                                              new XAttribute( "class", "csw_viewbuilder_filtersoptions_select" ),
                                                              _getFilterOptions( ViewBuilderProp, ValueSubfieldVal ) ) );
                    }

                    ParentNode.Add( new XElement( "property", ViewBuilderProp.MetaDataPropName,
                                               new XAttribute( "propname", ViewBuilderProp.MetaDataPropName ),
                                               new XAttribute( "propid", ViewBuilderProp.MetaDataPropId ),
                                               new XAttribute( "relatedidtype", ViewBuilderProp.RelatedIdType ),
                                               new XAttribute( "proptype", ViewBuilderProp.Type ),
                                               new XAttribute( "metadatatypename", ViewBuilderProp.MetaDataTypeName ),
                                               new XAttribute( "fieldtype", ViewBuilderProp.FieldType.FieldType ),
                                               new XAttribute( "proparbitraryid", ViewBuilderProp.ViewProp.ArbitraryId ),
                                               new XAttribute( "filtarbitraryid", Filter.ArbitraryId ),
                                               new XElement( "defaultsubfield",
                                                             new XAttribute( "filter", DefaultFilterMode.ToString() ),
                                                             new XAttribute( "subfield", DefaultSubfield ) ),
                                               new XElement( "subfields",
                                                             SubfieldSelect ),
                                               FiltersNode,
                                               FiltersOptionsNode ) );
                }
            }
        } // _getViewBuilderPropSubFields()

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
        private void _getSubFieldFilters( ref XElement FiltersNode, CswNbtSubField SubField, CswViewBuilderProp ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode, string UniqueId )
        {
            if( DefaultFilterMode == CswNbtPropFilterSql.PropertyFilterMode.Undefined )
            {
                DefaultFilterMode = SubField.DefaultFilterMode;
            }
            string SubFieldElementId = wsTools.makeId( _Prefix, UniqueId, string.Empty );
            XElement SubFieldNode = new XElement( "subfield", new XAttribute( "column", SubField.Column ), new XAttribute( "name", SubField.Name ) );
            XElement FiltersSelect = new XElement( "select",
                                        new XAttribute( "id", SubFieldElementId ),
                                        new XAttribute( "name", SubFieldElementId ),
                                        new XAttribute( "class", "csw_viewbuilder_filter_select" ) );
            foreach( CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt in SubField.SupportedFilterModes )
            {
                XElement ThisFilter = new XElement( "option", FilterModeOpt.ToString(),
                                                    new XAttribute( "value", FilterModeOpt ),
                                                    new XAttribute( "title", ViewBuilderProp.MetaDataPropName ),
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
        private XElement _getFilterOptions( CswViewBuilderProp ViewBuilderProp, string FilterSelected )
        {
            XElement FilterOptions = new XElement( "options", new XAttribute( "propname", ViewBuilderProp.MetaDataPropName.ToLower() ) );

            foreach( string Option in ViewBuilderProp.ListOptions )
            {
                FilterOptions.Add( new XElement( "option", Option,
                                    new XAttribute( "value", Option ),
                                    new XAttribute( "id", wsTools.makeId( ViewBuilderProp.MetaDataPropName, Option, string.Empty ) ),
                                    ( Option == FilterSelected ) ? new XAttribute( "selected", "selected" ) : null )
                                  );
            }
            return FilterOptions;

        } // _getFilterInitValue()

        #endregion Private Assembly Methods

        #region Public Methods
        
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
        private XElement _getViewBuilderProps( CswNbtViewRelationship.RelatedIdType Relationship, Int32 ObjectId )
        {
            XElement Props = new XElement( "nodetypeprops" );
            

            if( Int32.MinValue != ObjectId )
            {
                IEnumerable<CswViewBuilderProp> ViewBuilderProperties = null;
                switch( Relationship )
                {
                    case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ObjectId );
                            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                            ViewBuilderProperties = _getNodeTypeProps( NodeType, ref UniqueProps );
                            break;
                        }

                    case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectId );
                            ViewBuilderProperties = _getObjectClassProps( ObjectClass );
                            break;
                        }
                }
                Props = _getViewBuilderProps( ViewBuilderProperties, Relationship, ObjectId );
            }
            return Props; 
        } // getViewBuilderProps()
        
        public XElement getViewBuilderProps( string ViewXml, string ViewPropArbitraryId )
        {
            XElement ViewBuilderProps = new XElement( "viewbuilderprops" );
            if( !string.IsNullOrEmpty( ViewXml ) && !string.IsNullOrEmpty( ViewPropArbitraryId ) )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadXml( ViewXml );
                CswNbtViewProperty ThisProp = (CswNbtViewProperty)ThisView.FindViewNodeByArbitraryId( ViewPropArbitraryId );
                if( null != ThisProp )
                {
                    CswViewBuilderProp VbProp = new CswViewBuilderProp( ThisProp );
                    CswNbtViewRelationship.RelatedIdType Relationship = VbProp.RelatedIdType;
                    Int32 ObjectId = VbProp.MetaDataPropId;
                    if( Int32.MinValue != ObjectId && CswNbtViewRelationship.RelatedIdType.Unknown != Relationship )
                    {
                        ViewBuilderProps = _getViewBuilderProps( Relationship, ObjectId );
                    }
                }
            }
            return ViewBuilderProps;
        }

        public XElement getViewBuilderProps( string RelatedIdType, string ObjectPk, string NodeKey )
        {
            XElement ViewBuilderProps = new XElement( "nodetypeprops" );
            if( (!string.IsNullOrEmpty( RelatedIdType ) && !string.IsNullOrEmpty( ObjectPk )  ) || !string.IsNullOrEmpty( NodeKey ) )
            {
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
                ViewBuilderProps = _getViewBuilderProps( Relationship, ObjectId );
            }
            return ViewBuilderProps;

        }
        
        public XElement getNodeTypeProps(CswNbtMetaDataNodeType NodeType)
        {
            XElement NodeTypeProps = new XElement( "properties", "none" );
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();

            IEnumerable<CswViewBuilderProp> ViewBuilderProps = _getNodeTypeProps( NodeType, ref UniqueProps );
            if( ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getViewBuilderProps( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.NodeTypeId, NodeType.NodeTypeId );
            }
            return NodeTypeProps;
        }

        public XElement getNodeTypeProps(CswNbtMetaDataObjectClass ObjectClass )
        {
            XElement NodeTypeProps = new XElement( "properties", "none" );

            IEnumerable<CswViewBuilderProp> ViewBuilderProps = _getObjectClassProps( ObjectClass );
            if( ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getViewBuilderProps( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.ObjectClassId, ObjectClass.ObjectClassId );
            }
            return NodeTypeProps;
        }

        public void getViewBuilderPropSubfields(ref XElement ParentNode, CswViewBuilderProp ViewBuilderProp, ArrayList PropFilters)
        {
            _getViewBuilderPropSubFields( ref ParentNode, ViewBuilderProp, PropFilters );
        }

        public XElement makeViewPropFilter(string ViewXml, JToken FilterProp )
        {
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadXml( ViewXml );
            return makeViewPropFilter( View, FilterProp );
        }
        public XElement makeViewPropFilter( CswNbtView View, JToken FilterProp )
        {
            XElement PropFilterXml = new XElement( "propfilter" );
            var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
            CswNbtViewProperty.CswNbtPropType.TryParse( (string) FilterProp.First["proptype"], true, out PropType );
            string PropArbitraryId = (string) FilterProp.First["proparbitraryid"];
            string FiltArbitraryId = (string) FilterProp.First["filtarbitraryid"];

            if( PropType != CswNbtViewProperty.CswNbtPropType.Unknown &&
                !string.IsNullOrEmpty( PropArbitraryId ) &&
                !string.IsNullOrEmpty( FiltArbitraryId ) )
            {
                CswNbtViewProperty ViewProp = (CswNbtViewProperty) View.FindViewNodeByArbitraryId( PropArbitraryId );
                CswNbtViewPropertyFilter ViewPropFilt = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FiltArbitraryId );
                if( null != ViewPropFilt )
                {
                    var FieldName = CswNbtSubField.SubFieldName.Unknown;
                    CswNbtSubField.SubFieldName.TryParse( (string) FilterProp.First["subfield"], true, out FieldName );
                    var FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
                    CswNbtPropFilterSql.PropertyFilterMode.TryParse( (string) FilterProp.First["filter"], true, out FilterMode );
                    string SearchTerm = (string) FilterProp.First["searchtext"];

                    ViewPropFilt.FilterMode = FilterMode;
                    ViewPropFilt.SubfieldName = FieldName;
                    ViewPropFilt.Value = SearchTerm;

                    PropFilterXml = ViewPropFilt.ToXElement();
                }
            }

            return PropFilterXml;
        }

        #endregion Public Methods
    }
    #endregion wsViewBuilder

    #region CswViewBuilderProp Class
    public class CswViewBuilderProp
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

        public CswViewBuilderProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
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

        public CswViewBuilderProp( CswNbtMetaDataObjectClassProp ObjectClassProp )
        {
            _FieldType = ObjectClassProp.FieldType;
            _ListOptions.FromString( ObjectClassProp.ListOptions );
            _RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
            _MetaDataPropNameWithQuestionNo = ObjectClassProp.PropNameWithQuestionNo;
            _MetaDataPropId = ObjectClassProp.ObjectClassPropId;
            _MetaDataPropName = ObjectClassProp.PropName;
            _MetaDataTypeName = ObjectClassProp.ObjectClass.ObjectClass.ToString();
            _FieldTypeRule = ObjectClassProp.FieldTypeRule;
            _Type = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId;
        } //ctor Ntp

        public CswViewBuilderProp( CswNbtViewProperty ViewProp )
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


    }// CswViewBuilderProp
    #endregion CswViewBuilderProp Class
} // namespace ChemSW.WebServices
