using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Logic
{
    #region CswNbtViewBuilder
    public class CswNbtViewBuilder //: System.Web.Services.WebService
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ArrayList _ProhibittedFieldTypes;

        public CswNbtViewBuilder( CswNbtResources CswNbtResources, ArrayList ProhibittedFieldTypes )
        {
            _CswNbtResources = CswNbtResources;
            _ProhibittedFieldTypes = ProhibittedFieldTypes;
        } //ctor

        public CswNbtViewBuilder( CswNbtResources CswNbtResources )
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
                CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
                while( ThisVersionNodeType != null )
                {
                    foreach( CswViewBuilderProp ViewBuilderProp in ThisVersionNodeType.getNodeTypeProps().Cast<CswNbtMetaDataNodeTypeProp>()
                                                         .Where( ThisProp => !Props.ContainsValue( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                                                                 !Props.ContainsKey( ThisProp.FirstPropVersionId ) )
                                                         .Select( ThisProp => new CswViewBuilderProp( ThisProp ) ) )
                    {
                        Props.Add( ViewBuilderProp.MetaDataPropId, ViewBuilderProp.MetaDataPropNameWithQuestionNo.ToLower() );
                        NtProps.Add( ViewBuilderProp );
                    }
                    ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
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
                                                                            in ObjectClass.getNodeTypes()
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
        private JObject _getVbProperties( IEnumerable<CswViewBuilderProp> ViewBuilderProperties, NbtViewRelatedIdType RelatedIdType )
        {
            JObject PropObj = new JObject();

            PropObj["properties"] = new JObject();
            PropObj["properties"]["Specific Properties"] = new JObject();
            PropObj["properties"]["Generic Properties"] = new JObject();

            bool Selected = true;
            if( null != ViewBuilderProperties )
            {
                foreach( CswViewBuilderProp Prop in from ViewBuilderProp in ViewBuilderProperties
                                                    where !_ProhibittedFieldTypes.Contains( ViewBuilderProp.FieldType )
                                                    orderby ViewBuilderProp.MetaDataPropName
                                                    select ViewBuilderProp )
                {
                    JObject PropNodeObj = new JObject();
                    PropNodeObj["type"] = RelatedIdType.ToString();
                    PropNodeObj["name"] = Prop.MetaDataPropName;
                    PropNodeObj["id"] = Prop.MetaDataPropId.ToString();

                    if( Selected )
                    {
                        PropNodeObj["selected"] = true;
                        PropObj["defaultprop"] = Prop.MetaDataPropName;
                        Selected = false;
                    }

                    if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                    {
                        PropObj["properties"]["Specific Properties"][Prop.MetaDataPropName] = PropNodeObj;
                    }
                    else if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                    {
                        PropObj["properties"]["Generic Properties"][Prop.MetaDataPropName] = PropNodeObj;
                    }

                    _getVbPropData( PropNodeObj, Prop );
                }

            }
            return PropObj;
        } // _getVbProperties()

        /// <summary>
        /// Fetches all props and all prop filters for a NodeType
        /// </summary>
        private JObject _getVbProperties( NbtViewRelatedIdType Relationship, Int32 NodeTypeOrObjectClassId )
        {
            JObject ViewBuilderProps = new JObject();

            if( Int32.MinValue != NodeTypeOrObjectClassId )
            {
                IEnumerable<CswViewBuilderProp> ViewBuilderProperties = null;
                if( Relationship == NbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeOrObjectClassId );
                    Dictionary<Int32, string> UniqueProps = new Dictionary<int, string>();
                    ViewBuilderProperties = _getNodeTypeProps( NodeType, ref UniqueProps );
                }
                else if( Relationship == NbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( NodeTypeOrObjectClassId );
                    ViewBuilderProperties = _getObjectClassProps( ObjectClass );
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
            if( null != ViewBuilderProp && ViewBuilderProp.FieldTypeRule.SearchAllowed )
            {
                CswNbtSubFieldColl SubFields = ViewBuilderProp.FieldTypeRule.SubFields;

                ParentObj["propname"] = ViewBuilderProp.MetaDataPropName;
                ParentObj["viewbuilderpropid"] = ViewBuilderProp.MetaDataPropId.ToString();
                ParentObj["relatedidtype"] = ViewBuilderProp.RelatedIdType.ToString();
                ParentObj["proptype"] = ViewBuilderProp.Type.ToString();
                ParentObj["metadatatypename"] = ViewBuilderProp.MetaDataTypeName;
                ParentObj["fieldtype"] = ViewBuilderProp.FieldType.ToString();
                if( ViewBuilderProp.ViewProp != null )
                {
                    ParentObj["proparbitraryid"] = ViewBuilderProp.ViewProp.ArbitraryId;
                }
                string FiltArbitraryId = string.Empty;
                if( ViewBuilderProp.Filters.Count > 0 )
                {
                    CswNbtViewPropertyFilter Filt = (CswNbtViewPropertyFilter) ViewBuilderProp.Filters[0];
                    FiltArbitraryId = Filt.ArbitraryId;
                }
                ParentObj["filtarbitraryid"] = FiltArbitraryId;
                ParentObj["defaultconjunction"] = CswNbtPropFilterSql.PropertyFilterConjunction.And.ToString();
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

                if( ViewBuilderProp.FieldType == CswNbtMetaDataFieldType.NbtFieldType.List )
                {
                    ParentObj["filtersoptions"] = new JObject();
                    ParentObj["filtersoptions"]["name"] = ViewBuilderProp.MetaDataPropName;
                    if( ViewBuilderProp.ListOptions.Count() > 0 )
                    {
                        ParentObj["filtersoptions"]["selected"] = ViewBuilderProp.ListOptions.First();
                        ParentObj["filtersoptions"]["options"] = _getListPropFilterOptions( ViewBuilderProp );
                    }
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
            foreach( string Value in ViewBuilderProp.ListOptions )
            {
                FilterOptions[Value] = Value;
            }
            return FilterOptions;

        } // _getListPropFilterOptions()

        private void _addVbPropFilter( JObject ParentObj, CswNbtViewPropertyFilter Filter )
        {
            string FiltId = Filter.ArbitraryId;
            ParentObj[FiltId] = new JObject();
            ParentObj[FiltId]["arbitraryid"] = Filter.ArbitraryId;
            ParentObj[FiltId]["nodename"] = NbtViewXmlNodeName.Filter.ToString().ToLower();
            ParentObj[FiltId]["subfieldname"] = Filter.SubfieldName.ToString();
            ParentObj[FiltId]["value"] = Filter.Value;
            ParentObj[FiltId]["filtermode"] = Filter.FilterMode.ToString();
            ParentObj[FiltId]["casesensitive"] = Filter.CaseSensitive;
            ParentObj[FiltId]["conjunction"] = Filter.Conjunction.ToString();
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
                    Ret = getVbProp( View, VbProp );
                }
            }
            return Ret;
        }

        public JObject getVbProp( CswNbtView View, CswViewBuilderProp VbProp )
        {
            JObject Ret = new JObject();
            if( null != VbProp )
            {
                NbtViewRelatedIdType Relationship = VbProp.RelatedIdType;
                Int32 NodeTypeOrObjectClassId = VbProp.MetaDataPropId;
                if( Int32.MinValue != NodeTypeOrObjectClassId && NbtViewRelatedIdType.Unknown != Relationship )
                {
                    _getVbPropData( Ret, VbProp );
                }
            }
            return Ret;
        }

        ///// <summary>
        ///// Returns all props and prop filters for a NodeType or ObjectClass
        ///// </summary>
        //public JObject getVbProperties( string RelatedIdTypeStr, string NodeTypeOrObjectClassId, string NodeKey )
        //{
        //    JObject ViewBuilderProps = new JObject();
        //    if( ( !string.IsNullOrEmpty( RelatedIdTypeStr ) && !string.IsNullOrEmpty( NodeTypeOrObjectClassId ) ) || !string.IsNullOrEmpty( NodeKey ) )
        //    {
        //        Int32 TypeOrObjectClassId = Int32.MinValue;
        //        NbtViewRelatedIdType Relationship = NbtViewRelatedIdType.Unknown;
        //        if( string.IsNullOrEmpty( NodeTypeOrObjectClassId ) && !string.IsNullOrEmpty( NodeKey ) )
        //        {
        //            CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
        //            CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];
        //            if( null != Node.getNodeType() )
        //            {
        //                TypeOrObjectClassId = Node.NodeTypeId;
        //                Relationship = NbtViewRelatedIdType.NodeTypeId;
        //            }
        //            else if( null != Node.getObjectClass() )
        //            {
        //                TypeOrObjectClassId = Node.getObjectClassId();
        //                Relationship = NbtViewRelatedIdType.ObjectClassId;
        //            }
        //        }
        //        else if( false == string.IsNullOrEmpty( NodeTypeOrObjectClassId ) )
        //        {
        //            TypeOrObjectClassId = CswConvert.ToInt32( NodeTypeOrObjectClassId );
        //            Relationship = (NbtViewRelatedIdType) RelatedIdTypeStr;
        //            //NbtViewRelatedIdType.TryParse( RelatedIdTypeStr, true, out Relationship );
        //        }
        //        ViewBuilderProps = _getVbProperties( Relationship, TypeOrObjectClassId );
        //    }
        //    return ViewBuilderProps;
        //}

        private void _getVbPropertiesRecursive( IEnumerable<CswNbtViewRelationship> Relationships, JObject PropObject )
        {
            foreach( CswNbtViewRelationship Relationship in Relationships )
            {
                foreach( CswNbtViewProperty ViewProperty in Relationship.Properties )
                {
                    CswViewBuilderProp VbProp = new CswViewBuilderProp( ViewProperty );
                    JObject ThisProp = new JObject();
                    PropObject[VbProp.ViewProp.ArbitraryId] = ThisProp;
                    _getVbPropData( ThisProp, VbProp );
                }
                if( Relationship.ChildRelationships.Count > 0 )
                {
                    _getVbPropertiesRecursive( Relationship.ChildRelationships, PropObject );
                }
            }
        }

        /// <summary>
        /// Returns new, proposed View Props and Filters for a View in construction
        /// </summary>
        public void getVbProperties( JObject ParentObj, string ViewPropArbitraryIds, string ViewJson )
        {
            if( false == string.IsNullOrEmpty( ViewJson ) && false == string.IsNullOrEmpty( ViewPropArbitraryIds ) )
            {
                CswNbtView ThisView = new CswNbtView( _CswNbtResources );
                ThisView.LoadJson( ViewJson );
                CswCommaDelimitedString ArbIds = new CswCommaDelimitedString();
                ArbIds.FromString( ViewPropArbitraryIds );
                foreach( string ArbId in ArbIds )
                {
                    ParentObj[ArbId] = getVbProp( ThisView, ArbId );
                }
            }
        }

        /// <summary>
        /// Returns all props and prop filters for a CswNbtView
        /// </summary>
        public JObject getVbProperties( CswNbtView View )
        {
            JObject ViewBuilderProps = new JObject();
            if( null != View && View.Root.ChildRelationships.Count > 0 )
            {
                _getVbPropertiesRecursive( View.Root.ChildRelationships, ViewBuilderProps );
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
        public JObject makeViewPropFilter( CswNbtView View, JObject FilterProp, bool ClearFilters = false )
        {
            JObject Ret = new JObject();

            string FiltArbitraryId = CswConvert.ToString( FilterProp["filtarbitraryid"] );
            string PropArbitraryId = CswConvert.ToString( FilterProp["proparbitraryid"] );
            if( FiltArbitraryId == "undefined" ) FiltArbitraryId = string.Empty;
            if( PropArbitraryId == "undefined" ) PropArbitraryId = string.Empty;

            CswNbtViewPropertyFilter ViewPropFilt = null;
            if( false == string.IsNullOrEmpty( PropArbitraryId ) )
            {
                CswNbtViewProperty ViewProp = (CswNbtViewProperty) View.FindViewNodeByArbitraryId( PropArbitraryId );

                if( false == string.IsNullOrEmpty( FiltArbitraryId ) )
                {
                    ViewPropFilt = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FiltArbitraryId );
                }
                else
                {
                    ViewPropFilt = View.AddViewPropertyFilter( ViewProp, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Unknown, string.Empty, false );
                }

                //Case 23779, 23937, 24064
                if( ClearFilters && null != ViewPropFilt )
                {
                    ViewProp.Filters.Clear();
                    ViewProp.Filters.Add( ViewPropFilt );
                }
            }

            if( ViewPropFilt != null )
            {
                CswNbtPropFilterSql.PropertyFilterConjunction Conjunction = (CswNbtPropFilterSql.PropertyFilterConjunction) CswConvert.ToString( FilterProp["conjunction"] );
                CswNbtSubField.SubFieldName FieldName = (CswNbtSubField.SubFieldName) CswConvert.ToString( FilterProp["subfieldname"] );
                CswNbtPropFilterSql.PropertyFilterMode FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) CswConvert.ToString( FilterProp["filter"] );
                string FilterValue = CswConvert.ToString( FilterProp["filtervalue"] );

                if( FieldName != CswNbtSubField.SubFieldName.Unknown &&
                    FilterMode != CswNbtPropFilterSql.PropertyFilterMode.Unknown )
                {
                    ViewPropFilt.FilterMode = FilterMode;
                    ViewPropFilt.Conjunction = Conjunction;
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
                NodeTypeProps = _getVbProperties( ViewBuilderProps, NbtViewRelatedIdType.NodeTypeId );
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
                NodeTypeProps = _getVbProperties( ViewBuilderProps, NbtViewRelatedIdType.ObjectClassId );
            }
            return NodeTypeProps;
        }


        #endregion Public Methods
    }
    #endregion CswNbtViewBuilder

    #region CswViewBuilderProp Class
    public class CswViewBuilderProp
    {
        //private Int32 _MetaDataPropId = Int32.MinValue;
        public readonly Int32 MetaDataPropId = Int32.MinValue;
        public readonly CswNbtViewProperty ViewProp = null;
        public readonly string MetaDataPropName = string.Empty;
        public readonly string MetaDataPropNameWithQuestionNo = string.Empty;
        public readonly string MetaDataTypeName = string.Empty;
        public readonly CswNbtMetaDataFieldType.NbtFieldType FieldType;
        public readonly ICswNbtFieldTypeRule FieldTypeRule = null;
        public readonly CswCommaDelimitedString ListOptions = new CswCommaDelimitedString();
        public readonly NbtViewPropType Type = NbtViewPropType.Unknown;
        public readonly NbtViewRelatedIdType RelatedIdType = NbtViewRelatedIdType.Unknown;
        public readonly ArrayList Filters = new ArrayList();
        public readonly bool SortBy = false;
        public readonly NbtViewPropertySortMethod SortMethod = NbtViewPropertySortMethod.Ascending;
        public readonly Int32 Width = 40;
        public readonly string PropName = string.Empty;
        public CswCommaDelimitedString AssociatedPropIds = new CswCommaDelimitedString();
        private string _PropNameUnique = string.Empty;
        public string PropNameUnique
        {
            get
            {
                if( PropName != string.Empty && _PropNameUnique == string.Empty )
                {
                    _PropNameUnique = PropName.Trim().Replace( " ", "_" ).ToLower();
                }
                return _PropNameUnique;
            }
        }

        public CswViewBuilderProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            FieldType = NodeTypeProp.getFieldTypeValue();
            ListOptions.FromString( NodeTypeProp.ListOptions );
            RelatedIdType = NbtViewRelatedIdType.NodeTypeId;
            MetaDataPropNameWithQuestionNo = NodeTypeProp.PropNameWithQuestionNo;
            MetaDataPropId = NodeTypeProp.FirstPropVersionId;
            MetaDataPropName = NodeTypeProp.PropName;
            MetaDataTypeName = NodeTypeProp.getNodeType().NodeTypeName;
            FieldTypeRule = NodeTypeProp.getFieldTypeRule();
            Type = NbtViewPropType.NodeTypePropId;
            PropName = MetaDataPropName;
            AssociatedPropIds.Add( MetaDataPropId.ToString() );
            if( NodeTypeProp.ObjectClassPropId != Int32.MinValue )
            {
                AssociatedPropIds.Add( NodeTypeProp.ObjectClassPropId.ToString() );
            }
        } //ctor Ntp

        public CswViewBuilderProp( CswNbtMetaDataObjectClassProp ObjectClassProp )
        {
            FieldType = ObjectClassProp.getFieldTypeValue();
            setObjectClassPropListOptions( ObjectClassProp );
            RelatedIdType = NbtViewRelatedIdType.NodeTypeId;
            MetaDataPropNameWithQuestionNo = ObjectClassProp.PropNameWithQuestionNo;
            MetaDataPropId = ObjectClassProp.ObjectClassPropId;
            MetaDataPropName = ObjectClassProp.PropName;
            MetaDataTypeName = ObjectClassProp.getObjectClass().ObjectClass.ToString();
            FieldTypeRule = ObjectClassProp.getFieldTypeRule();
            Type = NbtViewPropType.ObjectClassPropId;
            PropName = MetaDataPropName;
            AssociatedPropIds.Add( MetaDataPropId.ToString() );
        } //ctor Ntp

        public CswViewBuilderProp( CswNbtViewProperty ViewProperty )
        {
            if( ViewProperty.Type == NbtViewPropType.NodeTypePropId &&
                null != ViewProperty.NodeTypeProp )
            {
                FieldType = ViewProperty.NodeTypeProp.getFieldTypeValue();
                ListOptions.FromString( ViewProperty.NodeTypeProp.ListOptions );
                RelatedIdType = NbtViewRelatedIdType.NodeTypeId;
                MetaDataPropNameWithQuestionNo = ViewProperty.NodeTypeProp.PropNameWithQuestionNo;
                MetaDataPropId = ViewProperty.NodeTypeProp.FirstPropVersionId;
                MetaDataPropName = ViewProperty.NodeTypeProp.PropName;
                MetaDataTypeName = ViewProperty.NodeTypeProp.getNodeType().NodeTypeName;
                FieldTypeRule = ViewProperty.NodeTypeProp.getFieldTypeRule();
                AssociatedPropIds.Add( MetaDataPropId.ToString() );
                if( ViewProperty.NodeTypeProp.ObjectClassPropId != Int32.MinValue )
                {
                    AssociatedPropIds.Add( ViewProperty.NodeTypeProp.ObjectClassPropId.ToString() );
                }
            }
            else if( ViewProperty.Type == NbtViewPropType.ObjectClassPropId &&
                null != ViewProperty.ObjectClassProp )
            {
                FieldType = ViewProperty.ObjectClassProp.getFieldTypeValue();
                setObjectClassPropListOptions( ViewProperty.ObjectClassProp );
                RelatedIdType = NbtViewRelatedIdType.ObjectClassId;
                MetaDataPropNameWithQuestionNo = ViewProperty.ObjectClassProp.PropNameWithQuestionNo;
                MetaDataPropId = ViewProperty.ObjectClassProp.ObjectClassPropId;
                MetaDataPropName = ViewProperty.ObjectClassProp.PropName;
                MetaDataTypeName = ViewProperty.ObjectClassProp.getObjectClass().ObjectClass.ToString().Replace( "Class", "" );
                FieldTypeRule = ViewProperty.ObjectClassProp.getFieldTypeRule();
                AssociatedPropIds.Add( MetaDataPropId.ToString() );
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

        private void setObjectClassPropListOptions( CswNbtMetaDataObjectClassProp ObjectClassProp )
        {
            if( ObjectClassProp.ListOptions != string.Empty )
            {
                ListOptions.FromString( ObjectClassProp.ListOptions );
            }
            else
            {
                // Get all options from all nodetypes
                foreach( CswNbtMetaDataNodeTypeProp VPNodeTypeProp in ObjectClassProp.getNodeTypeProps() )
                {
                    CswCommaDelimitedString NTPListOptions = new CswCommaDelimitedString();
                    NTPListOptions.FromString( VPNodeTypeProp.ListOptions );

                    foreach( string Option in NTPListOptions )
                    {
                        ListOptions.Add( Option, false, true );
                    }
                }
            }
        } // setObjectClassPropListOptions()

    }// CswViewBuilderProp
    #endregion CswViewBuilderProp Class
} // namespace ChemSW.WebServices
