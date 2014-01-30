using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleMTBF : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName StartDateTime = CswEnumNbtSubFieldName.StartDateTime;
            public static CswEnumNbtSubFieldName Units = CswEnumNbtSubFieldName.Units;
            public static CswEnumNbtSubFieldName Value = CswEnumNbtSubFieldName.Value;
        }


        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleMTBF( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            StartDateTimeSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.StartDateTime, true );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            StartDateTimeSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( StartDateTimeSubField );

            UnitsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, SubFieldName.Units, true );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            UnitsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            SubFields.add( UnitsSubField );

            ValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, SubFieldName.Value, true );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            ValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SubFields.add( ValueSubField, true );
        }//ctor

        public CswNbtSubField StartDateTimeSubField;
        public CswNbtSubField UnitsSubField;
        public CswNbtSubField ValueSubField;

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
            if( CswNbtSubField.Name == StartDateTimeSubField.Name )
            {
                return CswNbtFieldTypeRuleDateImpl.renderViewPropFilter( RunAsUser, _CswNbtFieldResources, CswNbtViewPropertyFilterIn, CswNbtSubField.Column );
            }
            else
            {
                ReturnVal = ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber ) );
            }

            return ( ReturnVal );

        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            string ret = string.Empty;

            // Are we using a Date filter?
            if( SubField.Name == StartDateTimeSubField.Name )
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
            public const string DefaultToToday = CswEnumNbtPropertyAttributeName.DefaultToToday;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.MTBF );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.MTBF,
                    Name = AttributeName.DefaultToToday,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Datetoday
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

    }//CswNbtFieldTypeRuleMTBF

}//namespace ChemSW.Nbt.MetaData
