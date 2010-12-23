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
    class CswNbtFieldTypeRuleDateImpl
    {
        public static string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtFieldResources CswNbtFieldResources, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, CswNbtSubField.PropColumn Column )
        {
            string ValueColumn = "jnp." + Column.ToString();
            string ReturnVal = string.Empty;

            DateTime FilterValue = DateTime.MinValue;
            if( CswNbtViewPropertyFilterIn.Value.Substring( 0, "today".Length ) == "today" )
            {
                Int32 PlusDays = CswConvert.ToInt32( CswNbtViewPropertyFilterIn.Value.Substring( "today+".Length ) );
                FilterValue = DateTime.Now.AddDays( PlusDays );
            }
            else
            {
                FilterValue = Convert.ToDateTime( CswNbtViewPropertyFilterIn.Value ).Date;
            }

            if( FilterValue != DateTime.MinValue )
            {
                string FilterValueString = CswNbtFieldResources.CswNbtResources.getDbNativeDate( FilterValue ); //FilterValue.ToShortDateString();
                switch( CswNbtViewPropertyFilterIn.FilterMode )
                {
                    case CswNbtPropFilterSql.PropertyFilterMode.Equals:
                        ReturnVal = ValueColumn + " = '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                        ReturnVal = ValueColumn + " > '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
                        ReturnVal = ValueColumn + " >= '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                        ReturnVal = ValueColumn + " < '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
                        ReturnVal = ValueColumn + " <= '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
                        ReturnVal = ValueColumn + " <> '" + FilterValueString + "'";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.NotNull:
                        ReturnVal = ValueColumn + " is not null";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.Null:
                        ReturnVal = ValueColumn + " is null";
                        break;
                    default:
                        throw new CswDniException( "Invalid filter", "An invalid FilterMode was encountered in CswNbtNodeProp.GetFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );

                }// switch( CswNbtViewPropertyFilterIn.FilterMode )
            }// if( FilterValue != DateTime.MinValue )

            return ( ReturnVal );

        }//makeWhereClause()


        public static string FilterModeToString( CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            string ret = FilterMode.ToString();
            switch( FilterMode )
            {
                case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                    ret = "After";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
                    ret = "After Or On";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                    ret = "Before";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
                    ret = "Before Or On";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.Equals:
                    ret = "On";
                    break;
                case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
                    ret = "Not On";
                    break;
            }
            return ret;
        }


    }
}
