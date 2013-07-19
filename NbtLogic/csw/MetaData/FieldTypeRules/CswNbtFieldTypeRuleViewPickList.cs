using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleViewPickList : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
            public static CswEnumNbtSubFieldName ViewID = CswEnumNbtSubFieldName.ViewID;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        //private CswNbtPropFilterSql _CswNbtPropFilterSql = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;

            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CachedViewNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Gestalt, SubFieldName.Name );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            CachedViewNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SubFields.add( CachedViewNameSubField );

            SelectedViewIdsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.ViewID );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            SelectedViewIdsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( SelectedViewIdsSubField );

        }//ctor

        public CswNbtSubField SelectedViewIdsSubField;
        public CswNbtSubField CachedViewNameSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return false; } } // return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( MetaDataProp, DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
            public const string SelectMode = CswEnumNbtPropertyAttributeName.SelectMode;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.ViewPickList );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ViewPickList,
                    Name = AttributeName.SelectMode,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Multi
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ViewPickList,
                    Name = AttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.ViewPickList
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//CswNbtFieldTypeRuleViewPickList

}//namespace ChemSW.Nbt.MetaData
