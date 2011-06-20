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

        /// <summary>
        /// Returns enumarable collection of all properties on a node type
        /// </summary>
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

        /// <summary>
        /// Returns enumarable collection of all properties on all node types of the object class
        /// </summary>
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

        /// <summary>
        /// Fetches all props and all prop fiters for a ViewProp collection
        ///     <viewbuilderprops>
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
        ///     </viewbuilderprops>
        /// </summary>
        private XElement _getViewBuilderProps( IEnumerable<CswViewBuilderProp> ViewBuilderProperties, CswNbtViewRelationship.RelatedIdType RelatedIdType, Int32 NodeTypeOrObjectClassId )
        {
            XElement ViewBuilderPropsNode = new XElement( "viewbuilderprops" );
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
                string ElementId = wsTools.makeId( _Prefix, "properties_select_nodetypeid", NodeTypeOrObjectClassId.ToString() );
                ViewBuilderPropsNode.Add( new XElement( "properties",
                                                     new XAttribute( "defaultprop", DefaultPropName ),
                                                     new XElement( "select",
                                                                   ( NodeTypePropsGroup.HasElements ) ? NodeTypePropsGroup : null,
                                                                   ( ObjectClassPropsGroup.HasElements ) ? ObjectClassPropsGroup : null ) ),
                                       FiltersNode );
            }
            return ViewBuilderPropsNode;
        }

        /// <summary>
        /// Fetches all props and all prop fiters for a NodeType
        ///     <viewbuilderprops>
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
        ///     </viewbuilderprops>
        /// </summary>
        private XElement _getViewBuilderProps( CswNbtViewRelationship.RelatedIdType Relationship, Int32 NodeTypeOrObjectClassId )
        {
            XElement ViewBuilderProps = new XElement( "viewbuilderprops" );

            if( Int32.MinValue != NodeTypeOrObjectClassId )
            {
                IEnumerable<CswViewBuilderProp> ViewBuilderProperties = null;
                switch( Relationship )
                {
                    case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeOrObjectClassId );
                            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                            ViewBuilderProperties = _getNodeTypeProps( NodeType, ref UniqueProps );
                            break;
                        }

                    case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                        {
                            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( NodeTypeOrObjectClassId );
                            ViewBuilderProperties = _getObjectClassProps( ObjectClass );
                            break;
                        }
                }
                ViewBuilderProps = _getViewBuilderProps( ViewBuilderProperties, Relationship, NodeTypeOrObjectClassId );
            }
            return ViewBuilderProps;
        } // getViewBuilderProps()

        /// <summary>
        /// Returns the Subfields XML for a NodeTypeProp/ObjectClassProp's SubFields collection as:
        ///     <propertyfilters>
        ///         <property propname="Barcode" fieldtype="Barcode" relatedidtype="nodetypeprop" viewbuilderpropid="1">
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
                XElement SubfieldSelect = new XElement( "select" );

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
                    _getSubFieldFilters( ref FiltersNode, Field, ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode.Undefined );
                    SubfieldSelect.Add( FieldNode );
                }

                XElement FiltersOptionsNode = new XElement( "filtersoptions" );
                if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    FiltersOptionsNode.Value = ViewBuilderProp.MetaDataPropName;
                    FiltersOptionsNode.Add( new XElement( "select", 
                                                          _getFilterOptions( ViewBuilderProp, string.Empty ) ) );
                }

                ParentNode.Add( new XElement( "property", ViewBuilderProp.MetaDataPropName,
                                              new XAttribute( "propname", ViewBuilderProp.MetaDataPropName ),
                                              new XAttribute( "viewbuilderpropid", ViewBuilderProp.MetaDataPropId ),
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
        ///         <property propname="Barcode" fieldtype="Barcode" relatedidtype="nodetypeprop" viewbuilderpropid="1">
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
                    XElement SubfieldSelect = new XElement( "select" );

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
                        _getSubFieldFilters( ref FiltersNode, Field, ViewBuilderProp, DefaultFilterMode );
                        SubfieldSelect.Add( FieldNode );
                    }


                    XElement FiltersOptionsNode = new XElement( "filtersoptions" );
                    if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                    {
                        FiltersOptionsNode.Value = ViewBuilderProp.MetaDataPropName;
                        FiltersOptionsNode.Add( new XElement( "select",
                                                              _getFilterOptions( ViewBuilderProp, ValueSubfieldVal ) ) );
                    }

                    ParentNode.Add( new XElement( "property", ViewBuilderProp.MetaDataPropName,
                                               new XAttribute( "propname", ViewBuilderProp.MetaDataPropName ),
                                               new XAttribute( "viewbuilderpropid", ViewBuilderProp.MetaDataPropId ),
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
        private void _getSubFieldFilters( ref XElement FiltersNode, CswNbtSubField SubField, CswViewBuilderProp ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode )
        {
            if( DefaultFilterMode == CswNbtPropFilterSql.PropertyFilterMode.Undefined )
            {
                DefaultFilterMode = SubField.DefaultFilterMode;
            }
            XElement SubFieldNode = new XElement( "subfield", new XAttribute( "column", SubField.Column ), new XAttribute( "name", SubField.Name ) );
            XElement FiltersSelect = new XElement( "select" );
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
        
        public XElement getViewBuilderProps( string ViewXml, string ViewPropArbitraryId )
        {
            XElement ViewBuilderProps = new XElement( "viewbuilderprops" );
            if( !string.IsNullOrEmpty( ViewXml ) && !string.IsNullOrEmpty( ViewPropArbitraryId ) )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadXml( ViewXml );
                CswNbtViewProperty ThisProp = (CswNbtViewProperty) ThisView.FindViewNodeByArbitraryId( ViewPropArbitraryId );
                if( null != ThisProp )
                {
                    CswViewBuilderProp VbProp = new CswViewBuilderProp( ThisProp );
                    CswNbtViewRelationship.RelatedIdType Relationship = VbProp.RelatedIdType;
                    Int32 NodeTypeOrObjectClassId = VbProp.MetaDataPropId;
                    if( Int32.MinValue != NodeTypeOrObjectClassId && CswNbtViewRelationship.RelatedIdType.Unknown != Relationship )
                    {
                        List<CswViewBuilderProp> ViewBuilderProp = new List<CswViewBuilderProp>() {VbProp};
                        
                        ViewBuilderProps = _getViewBuilderProps( ViewBuilderProp, Relationship, NodeTypeOrObjectClassId );
                    }
                }
            }
            return ViewBuilderProps;
        }

        /// <summary>
        /// Returns all props and prop filters for a NodeType or ObjectClass
        /// </summary>
        public XElement getViewBuilderProps( string RelatedIdType, string NodeTypeOrObjectClassId, string NodeKey )
        {
            XElement ViewBuilderProps = new XElement( "viewbuilderprops" );
            if( (!string.IsNullOrEmpty( RelatedIdType ) && !string.IsNullOrEmpty( NodeTypeOrObjectClassId )  ) || !string.IsNullOrEmpty( NodeKey ) )
            {
                Int32 TypeOrObjectClassId = Int32.MinValue;
                CswNbtViewRelationship.RelatedIdType Relationship = CswNbtViewRelationship.RelatedIdType.Unknown;
                if( string.IsNullOrEmpty( NodeTypeOrObjectClassId ) && !string.IsNullOrEmpty( NodeKey ) )
                {
                    string ParsedNodeKey = wsTools.FromSafeJavaScriptParam( NodeKey );
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNodeKey );
                    CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];
                    if( null != Node.NodeType )
                    {
                        TypeOrObjectClassId = Node.NodeTypeId;
                        Relationship = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                    }
                    else if( null != Node.ObjectClass )
                    {
                        TypeOrObjectClassId = Node.ObjectClassId;
                        Relationship = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
                    }
                }
                else if( !string.IsNullOrEmpty( NodeTypeOrObjectClassId ) )
                {
                    TypeOrObjectClassId = CswConvert.ToInt32( NodeTypeOrObjectClassId );
                    CswNbtViewRelationship.RelatedIdType.TryParse( RelatedIdType, true, out Relationship );
                }
                ViewBuilderProps = _getViewBuilderProps( Relationship, TypeOrObjectClassId );
            }
            return ViewBuilderProps;

        }

        /// <summary>
        /// Returns all props and prop filters for a NodeType
        /// </summary>
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

        /// <summary>
        /// Returns all props and prop filters for all NodeTypes of an ObjectClass
        /// </summary>
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

        /// <summary>
        /// Returns all prop filters for a CswNbtViewProperty
        /// </summary>
        public void getViewBuilderPropSubfields(ref XElement ParentNode, CswViewBuilderProp ViewBuilderProp, ArrayList PropFilters)
        {
            _getViewBuilderPropSubFields( ref ParentNode, ViewBuilderProp, PropFilters );
        }

        /// <summary>
        /// Uses View XML to construct a view and create a CswNbtViewPropertyFilter. and r
        /// Returns filter's XML
        /// </summary>
        public XElement getViewPropFilter(string ViewXml, string PropFilterJson )
        {
            XElement PropFilterXml = null;
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadXml( ViewXml );
			JObject PropFilter = JObject.Parse( PropFilterJson );
			XElement ThisPropFilter = makeViewPropFilter( View, PropFilter );
            if( null != ThisPropFilter )
            {
                PropFilterXml = ThisPropFilter;
            }
            return PropFilterXml;
        }
        
        /// <summary>
        /// Creates a CswNbtViewPropertyFilter and returns its XML
        /// </summary>
        public XElement makeViewPropFilter( CswNbtView View, JObject FilterProp )
        {
            XElement PropFilterXml = null;
            
            var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
            CswNbtViewProperty.CswNbtPropType.TryParse( (string) FilterProp["proptype"], true, out PropType );

			string FiltArbitraryId = (string) FilterProp["filtarbitraryid"];
			string PropArbitraryId = (string) FilterProp["proparbitraryid"];
            if ( FiltArbitraryId == "undefined" ) FiltArbitraryId = string.Empty;
            if ( PropArbitraryId == "undefined" ) PropArbitraryId = string.Empty;

			CswNbtViewPropertyFilter ViewPropFilt = null;
			if( PropType != CswNbtViewProperty.CswNbtPropType.Unknown )
			{
				if( !string.IsNullOrEmpty( FiltArbitraryId ) )
				{
					ViewPropFilt = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FiltArbitraryId );
				}
				else if( !string.IsNullOrEmpty( PropArbitraryId ) )
				{
					CswNbtViewProperty ViewProp = (CswNbtViewProperty) View.FindViewNodeByArbitraryId( PropArbitraryId );
					ViewPropFilt = View.AddViewPropertyFilter( ViewProp, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Undefined, string.Empty, false );
				}
			}

            if( ViewPropFilt != null )
            {
                XElement ThisPropFilter = makeViewPropFilter( ViewPropFilt, FilterProp );
                if( null != ThisPropFilter )
                {
                    PropFilterXml = ThisPropFilter;
                }
            }
            return PropFilterXml;
        }

        /// <summary>
        /// Modifies an existing CswNbtViewPropertyFilter and returns its XML 
        /// </summary>
        public XElement makeViewPropFilter( CswNbtViewPropertyFilter ViewPropFilt, JObject FilterProp )
        {
            XElement PropFilterXml = null;
            if( null != ViewPropFilt )
            {
                var FieldName = CswNbtSubField.SubFieldName.Unknown;
                CswNbtSubField.SubFieldName.TryParse( (string) FilterProp["subfield"], true, out FieldName );
                var FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Undefined;
                CswNbtPropFilterSql.PropertyFilterMode.TryParse( (string) FilterProp["filter"], true, out FilterMode );
                string SearchTerm = (string) FilterProp["filtervalue"];

                if( FieldName != CswNbtSubField.SubFieldName.Unknown &&
                    FilterMode != CswNbtPropFilterSql.PropertyFilterMode.Undefined )
                {
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
        //private Int32 _MetaDataPropId = Int32.MinValue;
        public readonly Int32 MetaDataPropId = Int32.MinValue;
        public readonly CswNbtViewProperty ViewProp = null;
        public readonly string MetaDataPropName = string.Empty;
        public readonly string MetaDataPropNameWithQuestionNo = string.Empty;
        public readonly string MetaDataTypeName = string.Empty;
        public readonly CswNbtMetaDataFieldType FieldType;
        public readonly ICswNbtFieldTypeRule FieldTypeRule = null;
        public readonly CswCommaDelimitedString ListOptions = new CswCommaDelimitedString();
        public readonly CswNbtViewProperty.CswNbtPropType Type = CswNbtViewProperty.CswNbtPropType.Unknown;
        public readonly CswNbtViewRelationship.RelatedIdType RelatedIdType = CswNbtViewRelationship.RelatedIdType.Unknown;
        public readonly ArrayList Filters = new ArrayList();
        public readonly bool SortBy = false;
        public readonly CswNbtViewProperty.PropertySortMethod SortMethod = CswNbtViewProperty.PropertySortMethod.Ascending;
        public readonly Int32 Width = Int32.MinValue;
        public readonly string PropName = string.Empty;

        public CswViewBuilderProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            FieldType = NodeTypeProp.FieldType;
            ListOptions.FromString( NodeTypeProp.ListOptions );
            RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
            MetaDataPropNameWithQuestionNo = NodeTypeProp.PropNameWithQuestionNo;
            MetaDataPropId = NodeTypeProp.FirstPropVersionId;
            MetaDataPropName = NodeTypeProp.PropName;
            MetaDataTypeName = NodeTypeProp.NodeType.NodeTypeName;
            FieldTypeRule = NodeTypeProp.FieldTypeRule;
            Type = CswNbtViewProperty.CswNbtPropType.NodeTypePropId;
            PropName = MetaDataPropName;
        } //ctor Ntp

        public CswViewBuilderProp( CswNbtMetaDataObjectClassProp ObjectClassProp )
        {
            FieldType = ObjectClassProp.FieldType;
            ListOptions.FromString( ObjectClassProp.ListOptions );
            RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
            MetaDataPropNameWithQuestionNo = ObjectClassProp.PropNameWithQuestionNo;
            MetaDataPropId = ObjectClassProp.ObjectClassPropId;
            MetaDataPropName = ObjectClassProp.PropName;
            MetaDataTypeName = ObjectClassProp.ObjectClass.ObjectClass.ToString();
            FieldTypeRule = ObjectClassProp.FieldTypeRule;
            Type = CswNbtViewProperty.CswNbtPropType.ObjectClassPropId;
            PropName = MetaDataPropName;
        } //ctor Ntp

        public CswViewBuilderProp( CswNbtViewProperty ViewProperty )
        {
            if( ViewProperty.Type == CswNbtViewProperty.CswNbtPropType.NodeTypePropId && 
                null != ViewProperty.NodeTypeProp)
            {
                FieldType = ViewProperty.NodeTypeProp.FieldType;
                ListOptions.FromString( ViewProperty.NodeTypeProp.ListOptions );
                RelatedIdType = CswNbtViewRelationship.RelatedIdType.NodeTypeId;
                MetaDataPropNameWithQuestionNo = ViewProperty.NodeTypeProp.PropNameWithQuestionNo;
                MetaDataPropId = ViewProperty.NodeTypeProp.FirstPropVersionId;
                MetaDataPropName = ViewProperty.NodeTypeProp.PropName;
                MetaDataTypeName = ViewProperty.NodeTypeProp.NodeType.NodeTypeName;
                FieldTypeRule = ViewProperty.NodeTypeProp.FieldTypeRule;
            }
            else if( ViewProperty.Type == CswNbtViewProperty.CswNbtPropType.ObjectClassPropId &&
                null != ViewProperty.ObjectClassProp )
            {
                FieldType = ViewProperty.ObjectClassProp.FieldType;
                ListOptions.FromString( ViewProperty.ObjectClassProp.ListOptions );
                RelatedIdType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
                MetaDataPropNameWithQuestionNo = ViewProperty.ObjectClassProp.PropNameWithQuestionNo;
                MetaDataPropId = ViewProperty.ObjectClassProp.ObjectClassPropId;
                MetaDataPropName = ViewProperty.ObjectClassProp.PropName;
                MetaDataTypeName = ViewProperty.ObjectClassProp.ObjectClass.ObjectClass.ToString().Replace( "Class", "" );
                FieldTypeRule = ViewProperty.ObjectClassProp.FieldTypeRule;
            }
            ViewProp = ViewProperty;
            FieldType = ViewProperty.FieldType;
            Filters = ViewProperty.Filters;
            Type = ViewProperty.Type;
            Width = ViewProperty.Width;
            SortBy = ViewProperty.SortBy;
            SortMethod = ViewProperty.SortMethod;
            PropName = ViewProperty.Name ?? MetaDataPropName;
        } //ctor Vp

    }// CswViewBuilderProp
    #endregion CswViewBuilderProp Class
} // namespace ChemSW.WebServices
