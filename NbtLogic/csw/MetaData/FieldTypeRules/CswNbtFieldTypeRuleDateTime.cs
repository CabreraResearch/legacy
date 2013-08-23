using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleDateTime : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleDateTime( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            DateValueSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Date, CswEnumNbtSubFieldName.Value, true );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            DateValueSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( DateValueSubField );
        }//ctor

        public CswNbtSubField DateValueSubField;

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
            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            return CswNbtFieldTypeRuleDateImpl.renderViewPropFilter( RunAsUser, _CswNbtFieldResources, CswNbtViewPropertyFilterIn, CswNbtSubField.Column );

        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return CswNbtFieldTypeRuleDateImpl.FilterModeToString( FilterMode );
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
            return "Click \"today\" to use the current date.</br></br>Enter \"today+N\" to use a date N days in the future, where N is any number.</br></br>Enter \"today-N\" to use a date N days in the past, where N is any number.";
        }

    }//CswNbtFieldTypeRuleDateTime

}//namespace ChemSW.Nbt.MetaData
