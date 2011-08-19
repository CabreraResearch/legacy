using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public wsViewBuilder( CswNbtResources CswNbtResources, ArrayList ProhibittedFieldTypes, string Prefix = null )
        {
            _CswNbtResources = CswNbtResources;
            //_View = View;
            _ProhibittedFieldTypes = ProhibittedFieldTypes;
            if( !string.IsNullOrEmpty( Prefix ) )
            {
                _Prefix = Prefix;
            }
        } //ctor

        public wsViewBuilder( CswNbtResources CswNbtResources, string Prefix = null )
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
        /// </summary>
        private JObject _getViewBuilderProps( IEnumerable<CswViewBuilderProp> ViewBuilderProperties, CswNbtViewRelationship.RelatedIdType RelatedIdType )
        {
            JObject ViewBuilderPropsNode = new JObject();
            JObject PropObj = new JObject();
            ViewBuilderPropsNode["properties"] = PropObj;

            JObject FiltersNodeObj = new JObject();
            PropObj["propertyfilters"] = FiltersNodeObj;

            JObject PropGroups = new JObject();
            JObject NodeTypePropsGrpObj = new JObject();
            PropGroups["Specific Properties"] = NodeTypePropsGrpObj;

            JObject ObjectClassPropsGrpObj = new JObject();
            PropGroups["Generic Properties"] = ObjectClassPropsGrpObj;
            PropObj["select"] = PropGroups;

            bool Selected = false;
            if( null != ViewBuilderProperties )
            {
                foreach( CswViewBuilderProp Prop in from ViewBuilderProp in ViewBuilderProperties
                                                    where !_ProhibittedFieldTypes.Contains( ViewBuilderProp.FieldType )
                                                    orderby ViewBuilderProp.MetaDataPropName
                                                    select ViewBuilderProp )
                {
                    JObject PropNodeObj = new JObject();
                    PropNodeObj["title"] = RelatedIdType.ToString();
                    PropNodeObj["name"] = Prop.MetaDataPropName;
                    PropNodeObj["id"] = Prop.MetaDataPropId;

                    if( !Selected )
                    {
                        PropNodeObj["selected"] = "selected";
                        PropObj["defaultprop"] = Prop.MetaDataPropName;
                        Selected = true;
                    }

                    switch( RelatedIdType )
                    {
                        case CswNbtViewRelationship.RelatedIdType.NodeTypeId:
                            NodeTypePropsGrpObj[Prop.MetaDataPropName] = PropNodeObj;
                            break;
                        case CswNbtViewRelationship.RelatedIdType.ObjectClassId:
                            ObjectClassPropsGrpObj[Prop.MetaDataPropName] = PropNodeObj;
                            break;
                    }

                    _getVbPropData( FiltersNodeObj, Prop );
                }

            }
            return ViewBuilderPropsNode;
        }

        /// <summary>
        /// Fetches all props and all prop fiters for a NodeType
        /// </summary>
        private JObject _getViewBuilderProps( CswNbtViewRelationship.RelatedIdType Relationship, Int32 NodeTypeOrObjectClassId )
        {
            JObject ViewBuilderProps = new JObject();

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
                ViewBuilderProps = _getViewBuilderProps( ViewBuilderProperties, Relationship );
            }
            return ViewBuilderProps;
        } // getVbProp()

        /// <summary>
        /// Returns the JSON for a Vb prop
        /// </summary>
        private void _getVbPropData( JObject ParentObj, CswViewBuilderProp ViewBuilderProp )
        {
            if( null != ViewBuilderProp )
            {
                CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = ViewBuilderProp.FieldType.FieldType;
                CswNbtSubFieldColl SubFields = ViewBuilderProp.FieldTypeRule.SubFields;

                JObject ChildObj = new JObject();
                ParentObj[ViewBuilderProp.MetaDataPropName] = ChildObj;

                ChildObj["propname"] = ViewBuilderProp.MetaDataPropName;
                ChildObj["viewbuilderpropid"] = ViewBuilderProp.MetaDataPropId;
                ChildObj["relatedidtype"] = ViewBuilderProp.RelatedIdType.ToString();
                ChildObj["proptype"] = ViewBuilderProp.Type.ToString();
                ChildObj["metadatatypename"] = ViewBuilderProp.MetaDataTypeName;
                ChildObj["fieldtype"] = ViewBuilderProp.FieldType.FieldType.ToString();

                JObject SubfieldObj = new JObject();
                ChildObj["select"] = SubfieldObj;

                if( null != ViewBuilderProp.FieldTypeRule.SubFields.Default )
                {
                    ChildObj["filter"] = ViewBuilderProp.FieldTypeRule.SubFields.Default.Name.ToString();
                    ChildObj["subfield"] = ViewBuilderProp.FieldTypeRule.SubFields.Default.Column.ToString();
                }

                JObject FiltersObj = new JObject();
                FiltersObj["name"] = ViewBuilderProp.MetaDataPropName;
                ChildObj["propertyfilters"] = FiltersObj;

                foreach( CswNbtSubField Field in SubFields )
                {
                    JObject FieldObj = new JObject();
                    SubfieldObj["option_" + Field.Column] = FiltersObj;

                    FieldObj["selectedfieldtype"] = SelectedFieldType.ToString();
                    FieldObj["column"] = Field.Column.ToString();
                    FieldObj["name"] = Field.Name.ToString();

                    if( Field.Name == ViewBuilderProp.FieldTypeRule.SubFields.Default.Name )
                    {
                        FieldObj["selected"] = true;
                    }

                    if( null == ViewBuilderProp.FieldTypeRule.SubFields.Default )
                    {
                        ChildObj["filter"] = Field.DefaultFilterMode.ToString();
                    }
                    _getSubFieldFilters( FiltersObj, Field, ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode.Undefined );

                }

                JObject FiltersOptsObj = new JObject();
                ChildObj["filtersoptions"] = FiltersOptsObj;

                if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    FiltersOptsObj["name"] = ViewBuilderProp.MetaDataPropName;
                    FiltersOptsObj["select"] = _getFilterOptions( ViewBuilderProp, string.Empty );
                }
            }
        } // _getViewBuilderPropData()

        /// <summary>
        /// Returns the JSON for a Vb prop collection
        /// </summary>
        private void _getVbPropertiesData( JObject ParentObj, CswViewBuilderProp ViewBuilderProp, ArrayList PropFilters )
        {
            if( null != ViewBuilderProp )
            {
                CswNbtMetaDataFieldType.NbtFieldType SelectedFieldType = ViewBuilderProp.FieldType.FieldType;
                foreach( CswNbtViewPropertyFilter Filter in PropFilters )
                {
                    JObject PropObj = new JObject();
                    ParentObj["property_" + ViewBuilderProp.ViewProp.ArbitraryId] = PropObj;

                    PropObj["propname"] = ViewBuilderProp.MetaDataPropName;
                    PropObj["viewbuilderpropid"] = ViewBuilderProp.MetaDataPropId;
                    PropObj["relatedidtype"] = ViewBuilderProp.RelatedIdType.ToString();
                    PropObj["proptype"] = ViewBuilderProp.Type.ToString();
                    PropObj["metadatatypename"] = ViewBuilderProp.MetaDataTypeName;
                    PropObj["fieldtype"] = ViewBuilderProp.FieldType.FieldType.ToString();
                    PropObj["proparbitraryid"] = ViewBuilderProp.ViewProp.ArbitraryId;
                    PropObj["filtarbitraryid"] = Filter.ArbitraryId;
                    PropObj["defaultsubfield"] = Filter.SubfieldName.ToString();
                    PropObj["defaultfilter"] = Filter.Value;

                    JObject SubfieldObj = new JObject();
                    PropObj["subfields"] = SubfieldObj;

                    CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode = Filter.FilterMode;

                    JObject FiltersObj = new JObject( new JProperty( "name", ViewBuilderProp.MetaDataPropName ) );
                    PropObj["propertyfilters"] = FiltersObj;
                    foreach( CswNbtSubField Field in ViewBuilderProp.FieldTypeRule.SubFields )
                    {

                        JObject FieldObj = new JObject();
                        SubfieldObj["option_" + Field.Name.ToString()] = FieldObj;

                        FieldObj["title"] = SelectedFieldType.ToString();
                        FieldObj["name"] = Field.Name.ToString();
                        FieldObj["column"] = Field.Column.ToString();
                        FieldObj["defaultvalue"] = Filter.Value;
                        FieldObj["arbitraryid"] = Filter.ArbitraryId;

                        if( Field.Name == Filter.SubfieldName )
                        {
                            FieldObj["selected"] = true;
                        }
                        _getSubFieldFilters( FiltersObj, Field, ViewBuilderProp, DefaultFilterMode );
                    }

                    JObject FiltersOptObj = new JObject();
                    PropObj["filtersoptions"] = FiltersOptObj;
                    if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                    {
                        FiltersOptObj["name"] = ViewBuilderProp.MetaDataPropName;
                        FiltersOptObj["select"] = _getFilterOptions( ViewBuilderProp, Filter.Value );
                    }
                }
            }
        } // _getViewBuilderPropData()

        /// <summary>
        /// Returns the JSON for SubFields Filters
        /// </summary>
        private void _getSubFieldFilters( JObject FiltersObj, CswNbtSubField SubField, CswViewBuilderProp ViewBuilderProp, CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode )
        {
            if( DefaultFilterMode == CswNbtPropFilterSql.PropertyFilterMode.Undefined )
            {
                DefaultFilterMode = SubField.DefaultFilterMode;
            }
            FiltersObj["subfield_" + SubField.Column] = new JObject( new JProperty( "column", SubField.Column ), new JProperty( "name", SubField.Name ) );

            JObject Filters = new JObject();
            FiltersObj["select"] = Filters;
            foreach( CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt in SubField.SupportedFilterModes )
            {
                Filters[FilterModeOpt.ToString()] = ViewBuilderProp.MetaDataPropName;
                if( FilterModeOpt == DefaultFilterMode )
                {
                    FiltersObj["selected"] = FilterModeOpt.ToString();
                }
            }
        } // _getSubFieldFilters()

        /// <summary>
        /// Sets fieldtype attributes on the Filter node and appends an XElement containing extended attributes, such as an options picklist
        /// </summary>
        private JObject _getFilterOptions( CswViewBuilderProp ViewBuilderProp, string FilterSelected )
        {
            JObject FilterOptions = new JObject();
            FilterOptions["name"] = ViewBuilderProp.MetaDataPropName.ToLower();

            JObject FilterPropObj = new JObject();
            FilterOptions["options"] = FilterPropObj;
            foreach( string Value in ViewBuilderProp.ListOptions )
            {
                string Id = wsTools.makeId( ViewBuilderProp.MetaDataPropName, Value, string.Empty );
                FilterPropObj[Id] = Value;
                if( Value == FilterSelected )
                {
                    FilterOptions["selected"] = Id;
                }
            }
            return FilterOptions;

        } // _getFilterInitValue()

        #endregion Private Assembly Methods

        #region Public Methods

        public JObject getVbProp( string ViewJson, string ViewPropArbitraryId )
        {
            JObject Ret = new JObject();
            if( !string.IsNullOrEmpty( ViewJson ) && !string.IsNullOrEmpty( ViewPropArbitraryId ) )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadJson( ViewJson );
                CswNbtViewProperty ThisProp = (CswNbtViewProperty) ThisView.FindViewNodeByArbitraryId( ViewPropArbitraryId );
                if( null != ThisProp )
                {
                    CswViewBuilderProp VbProp = new CswViewBuilderProp( ThisProp );
                    CswNbtViewRelationship.RelatedIdType Relationship = VbProp.RelatedIdType;
                    Int32 NodeTypeOrObjectClassId = VbProp.MetaDataPropId;
                    if( Int32.MinValue != NodeTypeOrObjectClassId && CswNbtViewRelationship.RelatedIdType.Unknown != Relationship )
                    {
                        _getVbPropData( Ret, VbProp );
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// Returns all props and prop filters for a NodeType or ObjectClass
        /// </summary>
        public JObject getVbProperties( string RelatedIdType, string NodeTypeOrObjectClassId, string NodeKey )
        {
            JObject ViewBuilderProps = new JObject();
            if( ( !string.IsNullOrEmpty( RelatedIdType ) && !string.IsNullOrEmpty( NodeTypeOrObjectClassId ) ) || !string.IsNullOrEmpty( NodeKey ) )
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
        public JObject getNodeTypeProps( CswNbtMetaDataNodeType NodeType )
        {
            JObject NodeTypeProps = new JObject();
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();

            IEnumerable<CswViewBuilderProp> ViewBuilderProps = _getNodeTypeProps( NodeType, ref UniqueProps );
            if( ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getViewBuilderProps( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.NodeTypeId );
            }
            return NodeTypeProps;
        }

        /// <summary>
        /// Returns all props and prop filters for all NodeTypes of an ObjectClass
        /// </summary>
        public JObject getNodeTypeProps( CswNbtMetaDataObjectClass ObjectClass )
        {
            JObject NodeTypeProps = new JObject();

            IEnumerable<CswViewBuilderProp> ViewBuilderProps = _getObjectClassProps( ObjectClass );
            if( ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getViewBuilderProps( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.ObjectClassId );
            }
            return NodeTypeProps;
        }

        /// <summary>
        /// Returns all prop filters for a CswNbtViewProperty
        /// </summary>
        public void getViewBuilderPropSubfields( JObject ParentNode, CswViewBuilderProp ViewBuilderProp, ArrayList PropFilters )
        {
            _getVbPropertiesData( ParentNode, ViewBuilderProp, PropFilters );
        }

        /// <summary>
        /// Uses View XML to construct a view and create a CswNbtViewPropertyFilter. and r
        /// Returns filter's XML
        /// </summary>
        public JObject getViewPropFilter( string ViewJson, string PropFilterJson )
        {
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadJson( ViewJson );
            JObject PropFilter = JObject.Parse( PropFilterJson );
            makeViewPropFilter( View, PropFilter );
            return PropFilter;
        }

        /// <summary>
        /// Creates a CswNbtViewPropertyFilter and returns its Json
        /// </summary>
        public JProperty makeViewPropFilter( CswNbtView View, JObject FilterProp )
        {
            JProperty PropFilterXml = null;

            var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
            CswNbtViewProperty.CswNbtPropType.TryParse( (string) FilterProp["proptype"], true, out PropType );

            string FiltArbitraryId = (string) FilterProp["filtarbitraryid"];
            string PropArbitraryId = (string) FilterProp["proparbitraryid"];
            if( FiltArbitraryId == "undefined" ) FiltArbitraryId = string.Empty;
            if( PropArbitraryId == "undefined" ) PropArbitraryId = string.Empty;

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
                JProperty ThisPropFilter = makeViewPropFilter( ViewPropFilt, FilterProp );
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
        public JProperty makeViewPropFilter( CswNbtViewPropertyFilter ViewPropFilt, JObject FilterProp )
        {
            JProperty PropFilterJProp = null;
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

                    PropFilterJProp = ViewPropFilt.ToJson();
                }
            }
            return PropFilterJProp;
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
                null != ViewProperty.NodeTypeProp )
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
