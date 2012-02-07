using System;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleQuantity : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleQuantity( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            QuantitySubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field1_Numeric, CswNbtSubField.SubFieldName.Value,true );
            QuantitySubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( QuantitySubField, true );

            UnitsSubField = new CswNbtSubField( _CswNbtFieldResources,  CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Units,true );
            UnitsSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                        CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                        CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                        CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                        CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                        CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                        CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( UnitsSubField );

        }//ctor

        public CswNbtSubField QuantitySubField;
        public CswNbtSubField UnitsSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            // BZ 7941
            bool UseNumericHack = true;
            if( CswNbtViewPropertyFilterIn.SubfieldName == CswNbtSubField.SubFieldName.Units )
                UseNumericHack = false;
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, UseNumericHack ) );
        }//makeWhereClause()


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
