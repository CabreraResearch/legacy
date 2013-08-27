using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleTimeInterval : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            IntervalSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Interval, true );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            IntervalSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( IntervalSubField );

            StartDateSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Date, CswEnumNbtSubFieldName.StartDateTime, true );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            StartDateSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( StartDateSubField );



        }//ctor

        public CswNbtSubField IntervalSubField;
        public CswNbtSubField StartDateSubField;

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
                ReturnVal = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn );
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

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
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
