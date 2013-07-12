using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{

    public class CswNbtPropFilterSql
    {
        private string _FilterTableAlias = "jnp.";
        public CswNbtPropFilterSql()
        {
        }

        // UseNumericHack: SEE BZ 6661

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, CswNbtSubField CswNbtSubField, bool UseNumericHack )
        {
            if( CswNbtSubField == null )
                throw ( new CswDniException( "CswNbtPropFilterSql.renderViewPropFilter() got a null CswNbtSubField for view: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string Column = CswNbtSubField.Column.ToString();
            string FilterTableAlias = _FilterTableAlias;
            if( CswNbtSubField.RelationalColumn != string.Empty )
            {
                Column = CswNbtSubField.RelationalColumn;
                FilterTableAlias = "n.";
            }

            string ReturnVal = "";

            //This is sort of a hacky way of dealing with the bz # 6936 issue. But since it's all going to need to be
            //revisited for bz #6682 anyway, this seems like the clearest and simplest way of handling the problem.
            //Also, see 7095
            if( CswEnumNbtFilterMode.Null != CswNbtViewPropertyFilterIn.FilterMode &&
                 CswEnumNbtFilterMode.NotNull != CswNbtViewPropertyFilterIn.FilterMode )
            {
                if( UseNumericHack )
                {
                    if(
                        CswEnumNbtFilterMode.Begins == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswEnumNbtFilterMode.Contains == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswEnumNbtFilterMode.Ends == CswNbtViewPropertyFilterIn.FilterMode
                        )
                    {
                        throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for numeric values" ) );
                    }


                    string NumericValueColumn = " nvl(" + FilterTableAlias + Column + ", 0) ";

                    if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
                    {
                        ReturnVal = NumericValueColumn + " = " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThan )
                    {
                        ReturnVal = NumericValueColumn + " > " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThanOrEquals )
                    {
                        ReturnVal = NumericValueColumn + " >= " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThan )
                    {
                        ReturnVal = NumericValueColumn + " < " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThanOrEquals )
                    {
                        ReturnVal = NumericValueColumn + " <= " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
                    {
                        ReturnVal = NumericValueColumn + " <> " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.In )
                    {
                        ReturnVal = NumericValueColumn + " in(" + CswNbtViewPropertyFilterIn.Value + ") ";
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }
                }
                else
                {
                    string CasePrepend = "";
                    string CaseAppend = "";
                    if( !CswNbtViewPropertyFilterIn.CaseSensitive )
                    {
                        CasePrepend = "lower(";
                        CaseAppend = ")";
                    }

                    string NonNumericValueColumn = CasePrepend + FilterTableAlias + Column + CaseAppend;
                    string SafeValue = CswNbtViewPropertyFilterIn.Value.Replace( "'", "''" );   // case 21455


                    if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Begins )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "%'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Contains )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "%'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotContains )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + "'%" + SafeValue + "%'" + CaseAppend
                            + " or " + NonNumericValueColumn + " is null" + ")";
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Ends )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
                    {
                        //covers the case of clobs
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThan )
                    {
                        ReturnVal = NonNumericValueColumn + " > " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " >= " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThan )
                    {
                        ReturnVal = NonNumericValueColumn + " < " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " <= " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend +
                                    " or " + NonNumericValueColumn + " is null )";   //case 21623
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.In )
                    {
                        //ReturnVal = NonNumericValueColumn + " in( " + CasePrepend + "'" + SafeValue + "'" + CaseAppend + " ) ";
                        // see case 30165
                        ReturnVal = NonNumericValueColumn + " in(" + CswNbtViewPropertyFilterIn.Value + ") ";
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }


                }//if-else UserNumericHack
            }
            else
            {
                string NullValueColumn = FilterTableAlias + Column;

                if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotNull )
                {
                    ReturnVal = NullValueColumn + " is not null";
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Null )
                {
                    ReturnVal = NullValueColumn + " is null";
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                }
            }//if-else filter mode is not null or not-null

            return ( ReturnVal );

        }//renderViewPropFilter()



    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
