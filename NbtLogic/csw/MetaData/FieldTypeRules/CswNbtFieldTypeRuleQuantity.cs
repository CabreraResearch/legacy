using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleQuantity: ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleQuantity( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            QuantitySubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1_Numeric, CswNbtSubField.SubFieldName.Value );
            QuantitySubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( QuantitySubField, true );

            UnitsSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Units );
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
            if(CswNbtViewPropertyFilterIn.SubfieldName == CswNbtSubField.SubFieldName.Units)
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

        public void afterCreateNodeTypeProp(  CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
