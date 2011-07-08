using System;
using ChemSW.Core;
using ChemSW.Exceptions;
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
            //Case 22715 and 22716
            string TodayString = CswNbtViewPropertyFilterIn.Value.ToLower().Trim();
            if( TodayString == "today" )
            {
                FilterValue = DateTime.Now.Date;
            }
            else if( TodayString.Substring( 0, "today+".Length ) == "today+" )
            {
                string Days = TodayString.Substring( "today+".Length );
                Int32 PlusDays = CswTools.IsInteger( Days ) ? CswConvert.ToInt32( Days ) : 0;
                FilterValue = DateTime.Now.AddDays( PlusDays ).Date;
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
                        ReturnVal = ValueColumn + " = " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                        ReturnVal = ValueColumn + " > " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
                        ReturnVal = ValueColumn + " >= " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                        ReturnVal = ValueColumn + " < " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
                        ReturnVal = ValueColumn + " <= " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
                        ReturnVal = ValueColumn + " <> " + FilterValueString;
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.NotNull:
                        ReturnVal = ValueColumn + " is not null";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.Null:
                        ReturnVal = ValueColumn + " is null";
                        break;
                    default:
                        throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtNodeProp.GetFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );

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
