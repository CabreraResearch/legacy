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

    public class CswNbtFieldTypeRuleTimeInterval: ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;

        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            IntervalSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Interval );
            IntervalSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                           CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                           CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( IntervalSubField );

            StartDateSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1_Date, CswNbtSubField.SubFieldName.StartDateTime );
            StartDateSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                            CswNbtPropFilterSql.PropertyFilterMode.Null;
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


        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
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
