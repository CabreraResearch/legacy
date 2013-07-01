using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleScientific : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleScientific( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            BaseSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, CswEnumNbtSubFieldName.Base, true );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( BaseSubField );

            ExponentSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Numeric, CswEnumNbtSubFieldName.Exponent, true );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ExponentSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ExponentSubField );

        }//ctor

        public CswNbtSubField BaseSubField;
        public CswNbtSubField ExponentSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }



        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, true ) );
        }

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute()
            {
                OwnerFieldType = CswEnumNbtFieldType.Scientific,
                Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.Scientific
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
