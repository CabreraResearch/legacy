using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{
    class CswNbtFieldTypeRuleDateImpl
    {
        public static string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtFieldResources CswNbtFieldResources, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, CswEnumNbtPropColumn Column )
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

            if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotNull )
            {
                ReturnVal = ValueColumn + " is not null";
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Null )
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

                if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
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
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThan )
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
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThanOrEquals )
                {
                    ReturnVal = ValueColumn + " >= " + ThisDayString;
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThan )
                {
                    ReturnVal = ValueColumn + " < " + ThisDayString;
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThanOrEquals )
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
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
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
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtNodeProp.GetFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                }// switch( CswNbtViewPropertyFilterIn.FilterMode )

            }// if( FilterValue != DateTime.MinValue )

            return ( ReturnVal );

        }//renderViewPropFilter()


        public static string FilterModeToString( CswEnumNbtFilterMode FilterMode )
        {
            string ret = FilterMode.ToString();
            if( FilterMode == CswEnumNbtFilterMode.GreaterThan )
            {
                ret = "After";
            }
            else if( FilterMode == CswEnumNbtFilterMode.GreaterThanOrEquals )
            {
                ret = "After Or On";
            }
            else if( FilterMode == CswEnumNbtFilterMode.LessThan )
            {
                ret = "Before";
            }
            else if( FilterMode == CswEnumNbtFilterMode.LessThanOrEquals )
            {
                ret = "Before Or On";
            }
            else if( FilterMode == CswEnumNbtFilterMode.Equals )
            {
                ret = "On";
            }
            else if( FilterMode == CswEnumNbtFilterMode.NotEquals )
            {
                ret = "Not On";
            }
            return ret;
        }


    }
}
