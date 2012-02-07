using System;
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

            BaseSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field1_Numeric, CswNbtSubField.SubFieldName.Base );
            BaseSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                        CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( BaseSubField );

            ExponentSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field2_Numeric, CswNbtSubField.SubFieldName.Exponent );
            ExponentSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                        CswNbtPropFilterSql.PropertyFilterMode.Null;
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
