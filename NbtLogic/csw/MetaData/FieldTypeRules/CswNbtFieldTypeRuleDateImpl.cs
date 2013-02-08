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
            bool IncludesTime = false;

            DateTime FilterValue = DateTime.MinValue;
            string Value = CswNbtViewPropertyFilterIn.Value.ToLower().Trim();
            if( Value.StartsWith( "today" ) )
            {
                Int32 PlusDays = 0;
                if( Value.Length > "today".Length )
                {
                    string Operator = Value.Substring( "today".Length, 1 );
                    string Operand = Value.Substring( "today".Length + 1 );
                    if( CswTools.IsInteger( Operand ) )
                    {
                        PlusDays = CswConvert.ToInt32( Operand );
                        if( Operator == "-" )
                            PlusDays = PlusDays * -1;
                    }
                }
                FilterValue = DateTime.Now.AddDays( PlusDays ).Date;
            }
            else
            {
                FilterValue = CswConvert.ToDateTime( CswNbtViewPropertyFilterIn.Value );
                if( FilterValue.TimeOfDay != TimeSpan.Zero ) // midnight
                {
                    IncludesTime = true;
                }
            }

            if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
            {
                ReturnVal = ValueColumn + " is not null";
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
            {
                ReturnVal = ValueColumn + " is null";
            }
            else if( FilterValue != DateTime.MinValue )
            {
                // case 26844
                // If no time was specified in our filter value, then 
                // we need to ignore the time part of values in our comparisons

                string ThisDayString = CswNbtFieldResources.CswNbtResources.getDbNativeDate( FilterValue );
                string NextDayString = CswNbtFieldResources.CswNbtResources.getDbNativeDate( FilterValue.AddDays( 1 ) );

                if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                {
                    if( IncludesTime )
                    {
                        ReturnVal = ValueColumn + " = " + ThisDayString;
                    }
                    else
                    {
                        ReturnVal = ValueColumn + " >= " + ThisDayString;
                        ReturnVal += " and " + ValueColumn + " < " + NextDayString;
                    }
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThan )
                {
                    if( IncludesTime )
                    {
                        ReturnVal = ValueColumn + " > " + ThisDayString;
                    }
                    else
                    {
                        ReturnVal = ValueColumn + " >= " + NextDayString;
                    }
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals )
                {
                    ReturnVal = ValueColumn + " >= " + ThisDayString;
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThan )
                {
                    ReturnVal = ValueColumn + " < " + ThisDayString;
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals )
                {
                    if( IncludesTime )
                    {
                        ReturnVal = ValueColumn + " <= " + ThisDayString;
                    }
                    else
                    {
                        ReturnVal = ValueColumn + " < " + NextDayString;   // not <=, see case 28620
                    }
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                {
                    if( IncludesTime )
                    {
                        ReturnVal = ValueColumn + " <> " + ThisDayString;
                    }
                    else
                    {
                        ReturnVal = "(" + ValueColumn + " < " + ThisDayString;
                        ReturnVal += " or " + ValueColumn + " >= " + NextDayString + ")";
                    }
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtNodeProp.GetFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                }// switch( CswNbtViewPropertyFilterIn.FilterMode )

            }// if( FilterValue != DateTime.MinValue )

            return ( ReturnVal );

        }//renderViewPropFilter()


        public static string FilterModeToString( CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            string ret = FilterMode.ToString();
            if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThan )
            {
                ret = "After";
            }
            else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals )
            {
                ret = "After Or On";
            }
            else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThan )
            {
                ret = "Before";
            }
            else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals )
            {
                ret = "Before Or On";
            }
            else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
            {
                ret = "On";
            }
            else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
            {
                ret = "Not On";
            }
            return ret;
        }


    }
}
