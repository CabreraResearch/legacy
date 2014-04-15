using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleNumericRange : ICswNbtFieldTypeRule
    {

        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Lower = CswEnumNbtSubFieldName.Lower;
            public static CswEnumNbtSubFieldName Target = CswEnumNbtSubFieldName.Target;
            public static CswEnumNbtSubFieldName Upper = CswEnumNbtSubFieldName.Upper;
            public static CswEnumNbtSubFieldName LowerInclusive = CswEnumNbtSubFieldName.LowerInclusive;
            public static CswEnumNbtSubFieldName UpperInclusive = CswEnumNbtSubFieldName.UpperInclusive;
            public static CswEnumNbtSubFieldName Units = CswEnumNbtSubFieldName.Units;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleNumericRange( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            LowerSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, SubFieldName.Lower, true );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            LowerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( LowerSubField );

            TargetSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Numeric, SubFieldName.Target, true );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            TargetSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( TargetSubField );

            UpperSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3_Numeric, SubFieldName.Upper, true );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UpperSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( UpperSubField );

            LowerInclusiveSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.LowerInclusive, true );
            LowerInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            LowerInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            LowerInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            LowerInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( LowerInclusiveSubField );

            UpperInclusiveSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, SubFieldName.UpperInclusive, true );
            UpperInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UpperInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UpperInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UpperInclusiveSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( UpperInclusiveSubField );

            UnitsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Units, true );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( UnitsSubField );

        } //ctor

        public CswNbtSubField LowerSubField;
        public CswNbtSubField TargetSubField;
        public CswNbtSubField UpperSubField;
        public CswNbtSubField LowerInclusiveSubField;
        public CswNbtSubField UpperInclusiveSubField;
        public CswNbtSubField UnitsSubField;

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
            public const string MaximumValue = CswEnumNbtPropertyAttributeName.MaximumValue;
            public const string ExcludeRangeLimits = CswEnumNbtPropertyAttributeName.ExcludeRangeLimits;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Number );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NumericRange,
                    Name = AttributeName.Precision,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberprecision
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NumericRange,
                    Name = AttributeName.MinimumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberminvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NumericRange,
                    Name = AttributeName.MaximumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NumericRange,
                    Name = AttributeName.ExcludeRangeLimits,
                    Column = CswEnumNbtPropertyAttributeColumn.Attribute1,
                    AttributeFieldType = CswEnumNbtFieldType.Logical
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.NumericRange,
                    Name = AttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.NumericRange
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
