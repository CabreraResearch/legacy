using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleScientific : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Base = CswEnumNbtSubFieldName.Base;
            public static CswEnumNbtSubFieldName Exponent = CswEnumNbtSubFieldName.Exponent;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleScientific( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            BaseSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, SubFieldName.Base, true );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            BaseSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( BaseSubField );

            ExponentSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Numeric, SubFieldName.Exponent, true );
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

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string Precision = CswEnumNbtPropertyAttributeName.Precision;
            public const string MinimumValue = CswEnumNbtPropertyAttributeName.MinimumValue;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Scientific );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Scientific,
                Name = AttributeName.Precision,
                Column = CswEnumNbtPropertyAttributeColumn.Numberprecision,
                AttributeFieldType = CswEnumNbtFieldType.Number
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Scientific,
                Name = AttributeName.MinimumValue,
                Column = CswEnumNbtPropertyAttributeColumn.Numberminvalue,
                AttributeFieldType = CswEnumNbtFieldType.Number
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Scientific,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.Scientific
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
