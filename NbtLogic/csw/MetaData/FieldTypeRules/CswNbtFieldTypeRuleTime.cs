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

    public class CswNbtFieldTypeRuleTime : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleTime( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            TimeValueSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1_Date, CswNbtSubField.SubFieldName.Value );
            TimeValueSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                            CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( TimeValueSubField );
        }//ctor

        public CswNbtSubField TimeValueSubField;

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

            string ValueColumn = "jnp." + CswNbtSubField.Column.ToString();
            string ReturnVal = "";
            
            switch( CswNbtViewPropertyFilterIn.FilterMode )
            {
                case CswNbtPropFilterSql.PropertyFilterMode.Equals:
                    ReturnVal = ValueColumn + " like " + "'" + CswNbtViewPropertyFilterIn.Value + "'";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                    // this is the right way to do it, but it won't work until bz 6682 is done.
                    //ReturnVal = ValueColumn + " > '" + _CswNbtResources.CswDbResources.getCswDbNativeDate().getNativeDate( Convert.ToDateTime( CswNbtViewPropertyFilterIn.Value ) ).ToString() + "'";
                    ReturnVal = ValueColumn + " > '" + Convert.ToDateTime( CswNbtViewPropertyFilterIn.Value ).Date.ToString() + "'";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                    // this is the right way to do it, but it won't work until bz 6682 is done.
                    //ReturnVal = ValueColumn + " < '" + _CswNbtResources.CswDbResources.getCswDbNativeDate().getNativeDate( Convert.ToDateTime( CswNbtViewPropertyFilterIn.Value ) ).ToString() + "'";
                    ReturnVal = ValueColumn + " < '" + Convert.ToDateTime( CswNbtViewPropertyFilterIn.Value ).Date.ToString() + "'";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
					ReturnVal = "(" + ValueColumn + " not like " + CswNbtViewPropertyFilterIn.Value + "'" +
								" or " + ValueColumn + " is null )";   // case 21623
					break;
                case CswNbtPropFilterSql.PropertyFilterMode.NotNull:
                    ReturnVal = ValueColumn + " is not null";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.Null:
                    ReturnVal = ValueColumn + " is null";
                    break;
                default:
					throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtNodeProp.GetFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
            }

            return ( ReturnVal );

        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            string ret = FilterMode.ToString();
            switch( FilterMode )
            {
                case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                    ret = "After";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
                    ret = "After Or At";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                    ret = "Before";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
                    ret = "Before Or At";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.Equals:
                    ret = "At";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
                    ret = "Not At";
                    break;
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

    }//CswNbtFieldTypeRuleTime

}//namespace ChemSW.Nbt.MetaData
