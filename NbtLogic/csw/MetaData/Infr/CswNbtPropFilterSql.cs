using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{

    public class CswNbtPropFilterSql
    {
        private string _FilterTableAlias = "jnp.";

        private readonly Dictionary<CswEnumNbtFilterMode, string> _numericHackFilterModes = new Dictionary<CswEnumNbtFilterMode, string>
            {
                {CswEnumNbtFilterMode.Equals, " = "},
                {CswEnumNbtFilterMode.NotEquals, " <> " },
                {CswEnumNbtFilterMode.GreaterThan, " > " },
                {CswEnumNbtFilterMode.LessThan, " < " },
                {CswEnumNbtFilterMode.GreaterThanOrEquals, " >= " },
                {CswEnumNbtFilterMode.LessThanOrEquals, " <= " },
                {CswEnumNbtFilterMode.In, " in(" }
            };

        public CswNbtPropFilterSql()
        {   
        }

        // UseNumericHack: SEE BZ 6661

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, CswNbtSubField CswNbtSubField, Dictionary<string, string> ParameterCollection, int FilterNumber, bool UseNumericHack )
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

            string ParameterName = "filt" + FilterNumber + "filtval";
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
                    ReturnVal = NumericValueColumn;
                    if( _numericHackFilterModes.ContainsKey( CswNbtViewPropertyFilterIn.FilterMode ) )
                    {
                        ReturnVal += _numericHackFilterModes[CswNbtViewPropertyFilterIn.FilterMode];
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }

                    ReturnVal += ":" + ParameterName;
                    ParameterCollection.Add( ParameterName, CswNbtViewPropertyFilterIn.Value );

                    if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.In )
                    {
                        ReturnVal += ")";
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
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName,  SafeValue + "%" );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Contains )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName,  "%" + SafeValue + "%" );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotContains )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + ":" + ParameterName + CaseAppend
                            + " or " + NonNumericValueColumn + " is null" + ")";
                        ParameterCollection.Add( ParameterName,  "%" + SafeValue + "%" );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Ends )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName, "%" + SafeValue  );

                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
                    {
                        //covers the case of clobs
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName, SafeValue  );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThan )
                    {
                        ReturnVal = NonNumericValueColumn + " > " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName, SafeValue  );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.GreaterThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " >= " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName, SafeValue );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThan )
                    {
                        ReturnVal = NonNumericValueColumn + " < " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName,  SafeValue );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.LessThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " <= " + CasePrepend + ":" + ParameterName + CaseAppend;
                        ParameterCollection.Add( ParameterName, SafeValue );
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + ":" + ParameterName + CaseAppend +
                                    " or " + NonNumericValueColumn + " is null )";   //case 21623
                        ParameterCollection.Add( ParameterName, SafeValue );

                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.In )
                    {
                        //ReturnVal = NonNumericValueColumn + " in( " + CasePrepend + "'" + SafeValue + "'" + CaseAppend + " ) ";
                        // see case 30165
                        ReturnVal = NonNumericValueColumn + " in(:" + ParameterName + ") ";
                        ParameterCollection.Add( ParameterName, CswNbtViewPropertyFilterIn.Value );
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
