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
        private readonly ArrayList _ProhibittedFieldTypes;

        public wsViewBuilder( CswNbtResources CswNbtResources, ArrayList ProhibittedFieldTypes )
        {
            _CswNbtResources = CswNbtResources;
            _ProhibittedFieldTypes = ProhibittedFieldTypes;
        } //ctor

        public wsViewBuilder( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ProhibittedFieldTypes = new ArrayList();
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
        /// Fetches all props and all prop filters for a ViewProp collection
        /// </summary>
        private JObject _getVbProperties( IEnumerable<CswViewBuilderProp> ViewBuilderProperties, CswNbtViewRelationship.RelatedIdType RelatedIdType )
        {
            JObject PropObj = new JObject();

            JObject PropertiesObj = new JObject();
            PropObj["properties"] = PropertiesObj;

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
                        PropNodeObj["selected"] = true;
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

                    _getVbPropData( PropertiesObj, Prop );
                }

            }
            return PropObj;
        } // _getVbProperties()

        /// <summary>
        /// Fetches all props and all prop filters for a NodeType
        /// </summary>
        private JObject _getVbProperties( CswNbtViewRelationship.RelatedIdType Relationship, Int32 NodeTypeOrObjectClassId )
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
                ViewBuilderProps = _getVbProperties( ViewBuilderProperties, Relationship );
            }
            return ViewBuilderProps;
        } // _getVbProperties()

        /// <summary>
        /// Returns the JSON for a Vb prop
        /// </summary>
        private void _getVbPropData( JObject ParentObj, CswViewBuilderProp ViewBuilderProp )
        {
            if( null != ViewBuilderProp )
            {
                CswNbtSubFieldColl SubFields = ViewBuilderProp.FieldTypeRule.SubFields;

                ParentObj["propname"] = ViewBuilderProp.MetaDataPropName;
                ParentObj["viewbuilderpropid"] = ViewBuilderProp.MetaDataPropId;
                ParentObj["relatedidtype"] = ViewBuilderProp.RelatedIdType.ToString();
                ParentObj["proptype"] = ViewBuilderProp.Type.ToString();
                ParentObj["metadatatypename"] = ViewBuilderProp.MetaDataTypeName;
                ParentObj["fieldtype"] = ViewBuilderProp.FieldType.FieldType.ToString();
                ParentObj["proparbitraryid"] = ViewBuilderProp.ViewProp.ArbitraryId;
                ParentObj["filtarbitraryid"] = string.Empty;
                ParentObj["defaultsubfield"] = ViewBuilderProp.FieldTypeRule.SubFields.Default.Name.ToString();
                ParentObj["defaultfiltermode"] = ViewBuilderProp.FieldTypeRule.SubFields.Default.DefaultFilterMode.ToString();

                ParentObj["subfields"] = new JObject();

                foreach( CswNbtSubField Field in SubFields )
                {
                    string OptName = Field.Name.ToString();
                    ParentObj["subfields"][OptName] = new JObject();
                    ParentObj["subfields"][OptName]["column"] = Field.Column.ToString();
                    ParentObj["subfields"][OptName]["name"] = OptName;
                    JObject FiltersObj = new JObject();
                    ParentObj["subfields"][OptName]["filtermodes"] = FiltersObj;

                    _addSubFieldFilterModes( FiltersObj, Field );
                }

                addVbPropFilters( ParentObj, ViewBuilderProp );

                if( ViewBuilderProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    ParentObj["filtersoptions"] = new JObject();
                    ParentObj["filtersoptions"]["name"] = ViewBuilderProp.MetaDataPropName;
                    ParentObj["filtersoptions"]["selected"] = ViewBuilderProp.ListOptions.First();
                    ParentObj["filtersoptions"]["options"] = _getListPropFilterOptions( ViewBuilderProp );
                }
            }
        } // _getVbPropData()

        /// <summary>
        /// Returns the JSON for SubFields Filters
        /// </summary>
        private void _addSubFieldFilterModes( JObject FiltersObj, CswNbtSubField SubField )
        {
            foreach( CswNbtPropFilterSql.PropertyFilterMode FilterModeOpt in SubField.SupportedFilterModes )
            {
                FiltersObj[FilterModeOpt.ToString()] = FilterModeOpt.ToString();
            }
        } // _addSubFieldFilterModes()

        /// <summary>
        /// Returns the List Options as JSON
        /// </summary>
        private JObject _getListPropFilterOptions( CswViewBuilderProp ViewBuilderProp )
        {
            JObject FilterOptions = new JObject();
            FilterOptions["name"] = ViewBuilderProp.MetaDataPropName.ToLower();
            FilterOptions["options"] = new JObject();
            foreach( string Value in ViewBuilderProp.ListOptions )
            {
                FilterOptions["options"][Value] = Value;
            }
            return FilterOptions;

        } // _getListPropFilterOptions()

        private void _addVbPropFilter( JObject ParentObj, CswNbtViewPropertyFilter Filter )
        {
            string FiltId = Filter.ArbitraryId;
            ParentObj[FiltId] = new JObject();
            ParentObj[FiltId]["arbitraryid"] = Filter.ArbitraryId;
            ParentObj[FiltId]["subfield"] = Filter.SubfieldName.ToString();
            ParentObj[FiltId]["value"] = Filter.Value;
            ParentObj[FiltId]["filtermode"] = Filter.FilterMode.ToString();
        }

        #endregion Private Assembly Methods

        #region Public Methods

        public JObject getVbProp( string ViewJson, string ViewPropArbitraryId )
        {
            JObject Ret = new JObject();
            if( !string.IsNullOrEmpty( ViewJson ) )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadJson( ViewJson );
                Ret = getVbProp( ThisView, ViewPropArbitraryId );
            }
            return Ret;
        }

        public JObject getVbProp( CswNbtView View, string ViewPropArbitraryId )
        {
            JObject Ret = new JObject();
            if( false == string.IsNullOrEmpty( ViewPropArbitraryId ) )
            {
                CswNbtViewProperty ThisProp = (CswNbtViewProperty) View.FindViewNodeByArbitraryId( ViewPropArbitraryId );
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
                ViewBuilderProps = _getVbProperties( Relationship, TypeOrObjectClassId );
            }
            return ViewBuilderProps;
        }

        /// <summary>
        /// Returns all prop filters for a CswNbtViewProperty
        /// </summary>
        public void addVbPropFilters( JObject ParentObj, CswViewBuilderProp ViewBuilderProp )
        {
            if( null != ViewBuilderProp )
            {
                JObject PropFilters = new JObject();
                ParentObj["propfilters"] = PropFilters;

                ArrayList Filters = ViewBuilderProp.Filters;
                foreach( CswNbtViewPropertyFilter Filter in Filters )
                {
                    _addVbPropFilter( PropFilters, Filter );
                }
            }
        }

        /// <summary>
        /// Uses View JSON to construct a view and create a CswNbtViewPropertyFilter
        /// Returns filter's JSON
        /// </summary>
        public JObject makeViewPropFilter( string ViewJson, string PropFilterJson )
        {
            JObject Ret = new JObject();
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadJson( ViewJson );
            JObject PropFilter = JObject.Parse( PropFilterJson );
            Ret = makeViewPropFilter( View, PropFilter );
            return Ret;
        }

        /// <summary>
        /// Creates a CswNbtViewPropertyFilter and returns its Json
        /// </summary>
        public JObject makeViewPropFilter( CswNbtView View, JObject FilterProp )
        {
            JObject Ret = new JObject();

            var PropType = CswNbtViewProperty.CswNbtPropType.Unknown;
            CswNbtViewProperty.CswNbtPropType.TryParse( (string) FilterProp["proptype"], true, out PropType );

            string FiltArbitraryId = CswConvert.ToString( FilterProp["filtarbitraryid"] );
            string PropArbitraryId = CswConvert.ToString( FilterProp["proparbitraryid"] );
            if( FiltArbitraryId == "undefined" ) FiltArbitraryId = string.Empty;
            if( PropArbitraryId == "undefined" ) PropArbitraryId = string.Empty;

            CswNbtViewPropertyFilter ViewPropFilt = null;
            if( PropType != CswNbtViewProperty.CswNbtPropType.Unknown )
            {
                if( false == string.IsNullOrEmpty( FiltArbitraryId ) )
                {
                    ViewPropFilt = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FiltArbitraryId );
                }
                else if( false == string.IsNullOrEmpty( PropArbitraryId ) )
                {
                    CswNbtViewProperty ViewProp = (CswNbtViewProperty) View.FindViewNodeByArbitraryId( PropArbitraryId );
                    ViewPropFilt = View.AddViewPropertyFilter( ViewProp, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Undefined, string.Empty, false );
                }
            }

            if( ViewPropFilt != null )
            {
                CswNbtSubField.SubFieldName FieldName;
                Enum.TryParse( CswConvert.ToString( FilterProp["subfield"] ), true, out FieldName );

                CswNbtPropFilterSql.PropertyFilterMode FilterMode;
                Enum.TryParse( CswConvert.ToString( FilterProp["filter"] ), true, out FilterMode );

                string FilterValue = CswConvert.ToString( FilterProp["filtervalue"] );

                if( FieldName != CswNbtSubField.SubFieldName.Unknown &&
                    FilterMode != CswNbtPropFilterSql.PropertyFilterMode.Undefined )
                {
                    ViewPropFilt.FilterMode = FilterMode;
                    ViewPropFilt.SubfieldName = FieldName;
                    ViewPropFilt.Value = FilterValue;
                    _addVbPropFilter( Ret, ViewPropFilt );
                }
            }
            return Ret;
        }

        /// <summary>
        /// Returns all props and prop filters for a NodeType
        /// </summary>
        public JObject getNodeTypeProps( CswNbtMetaDataNodeType NodeType )
        {
            JObject NodeTypeProps = new JObject();
            Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();

            IEnumerable<CswViewBuilderProp> ViewBuilderProps = _getNodeTypeProps( NodeType, ref UniqueProps );
            if( ViewBuilderProps != null && ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getVbProperties( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.NodeTypeId );
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
            if( ViewBuilderProps != null && ViewBuilderProps.Count() > 0 )
            {
                NodeTypeProps = _getVbProperties( ViewBuilderProps, CswNbtViewRelationship.RelatedIdType.ObjectClassId );
            }
            return NodeTypeProps;
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
