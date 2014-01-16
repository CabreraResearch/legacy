using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleTimeInterval : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Interval = CswEnumNbtSubFieldName.Interval;
            public static CswEnumNbtSubFieldName StartDateTime = CswEnumNbtSubFieldName.StartDateTime;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            IntervalSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Interval, true );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( IntervalSubField );

            StartDateSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Date, SubFieldName.StartDateTime, true );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( StartDateSubField );

            ClobDataSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, CswEnumNbtSubFieldName.Value );
            ClobDataSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            ClobDataSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( ClobDataSubField );

        }//ctor

        public CswNbtSubField IntervalSubField;
        public CswNbtSubField StartDateSubField;
        public CswNbtSubField ClobDataSubField;

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
            string ReturnVal = string.Empty;

            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            // Are we using a Date filter?
            if( CswNbtSubField.Name == StartDateSubField.Name )
            {
                return CswNbtFieldTypeRuleDateImpl.renderViewPropFilter( RunAsUser, _CswNbtFieldResources, CswNbtViewPropertyFilterIn, CswNbtSubField.Column );
            }
            else
            {
                ReturnVal = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber );
            }

            return ( ReturnVal );

        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            string ret = string.Empty;

            // Are we using a Date filter?
            if( SubField.Name == StartDateSubField.Name )
            {
                return CswNbtFieldTypeRuleDateImpl.FilterModeToString( FilterMode );
            }
            else
            {
                ret = _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
            }

            return ret;
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
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
            public const string AllowHourly = CswEnumNbtPropertyAttributeName.AllowHourly;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.TimeInterval );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.TimeInterval,
                Name = AttributeName.AllowHourly,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute1,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.TimeInterval,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.TimeInterval
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

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
