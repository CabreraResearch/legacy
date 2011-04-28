using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData
{

    public class CswNbtPropFilterSql
    {
        /// <summary>
        /// Indicates the type of operator to be used for a property filter. 
        /// The numbers assigned to the enum values is absolutely required in 
        /// order for bitwise operators to work. 
        /// </summary>
        [Flags]
        public enum PropertyFilterMode
        {
            Equals = 1,
            GreaterThan = 2,
            GreaterThanOrEquals = 4,
            LessThan = 8,
            LessThanOrEquals = 16,
            NotEquals = 32,
            //Range = 64,
            Begins = 64,
            Ends = 128,
            Contains = 256,
            //            In = 1024,
            Null = 512,
            NotNull = 1024,
            Undefined = 2048
        };
        public enum PropertyFilterConjunction { And, AndNot };


        private string _FilterTableAlias = "jnp.";
        public CswNbtPropFilterSql()
        {
        }


        // UseNumericHack: SEE BZ 6661

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, CswNbtSubField CswNbtSubField, bool UseNumericHack )
        {
            if( CswNbtSubField == null )
                throw ( new CswDniException( "CswNbtPropFilterSql.renderViewPropFilter() got a null CswNbtSubField for view: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            if ( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
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
            if ( CswNbtPropFilterSql.PropertyFilterMode.Null != CswNbtViewPropertyFilterIn.FilterMode &&
                 CswNbtPropFilterSql.PropertyFilterMode.NotNull != CswNbtViewPropertyFilterIn.FilterMode )
            {
                if ( UseNumericHack )
                {
                    if (
                        CswNbtPropFilterSql.PropertyFilterMode.Begins == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswNbtPropFilterSql.PropertyFilterMode.Contains == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswNbtPropFilterSql.PropertyFilterMode.Ends == CswNbtViewPropertyFilterIn.FilterMode
                        )
                    {
                        throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for numeric values" ) );
                    }


                    string NumericValueColumn = " nvl(" + FilterTableAlias + Column + ", 0) ";

                    switch ( CswNbtViewPropertyFilterIn.FilterMode )
                    {
                        case CswNbtPropFilterSql.PropertyFilterMode.Equals:
                            ReturnVal = NumericValueColumn + " = " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
                            ReturnVal = NumericValueColumn + " > " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
                            ReturnVal = NumericValueColumn + " >= " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
                            ReturnVal = NumericValueColumn + " < " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
                            ReturnVal = NumericValueColumn + " <= " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
                            ReturnVal = NumericValueColumn + " <> " + CswNbtViewPropertyFilterIn.Value;
                            break;
                        default:
                            throw new CswDniException( "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }  //switch
                }
                else
                {
                    string CasePrepend = "";
                    string CaseAppend = "";
                    if ( !CswNbtViewPropertyFilterIn.CaseSensitive )
                    {
                        CasePrepend = "lower(";
                        CaseAppend = ")";
                    }

                    string NonNumericValueColumn = CasePrepend + FilterTableAlias + Column + CaseAppend;
					string SafeValue = CswNbtViewPropertyFilterIn.Value.Replace( "'", "''" );   // case 21455

                    switch ( CswNbtViewPropertyFilterIn.FilterMode )
                    {
                        case CswNbtPropFilterSql.PropertyFilterMode.Begins:
							ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "%'" + CaseAppend;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.Contains:
							ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "%'" + CaseAppend;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.Ends:
							ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "'" + CaseAppend;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.Equals: //covers the case of clobs
							ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.GreaterThan:
							ReturnVal = NonNumericValueColumn + " > '" + SafeValue + "'";
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals:
							ReturnVal = NonNumericValueColumn + " >= '" + SafeValue + "'";
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.LessThan:
							ReturnVal = NonNumericValueColumn + " < '" + SafeValue + "'";
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals:
							ReturnVal = NonNumericValueColumn + " <= '" + SafeValue + "'";
                            break;
                        case CswNbtPropFilterSql.PropertyFilterMode.NotEquals:
							ReturnVal = NonNumericValueColumn + " not like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                            break;
                        default:
                            throw new CswDniException( "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }  //switch

                }//if-else UserNumericHack
            }
            else
            {
                string NullValueColumn = FilterTableAlias + Column;

                switch ( CswNbtViewPropertyFilterIn.FilterMode )
                {
                    case CswNbtPropFilterSql.PropertyFilterMode.NotNull:
                        ReturnVal = NullValueColumn + " is not null";
                        break;
                    case CswNbtPropFilterSql.PropertyFilterMode.Null:
                        ReturnVal = NullValueColumn + " is null";
                        break;
                    default:
                        throw new CswDniException( "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                }  //switch

            }//if-else filter mode is not null or not-null

            return ( ReturnVal );

        }//renderViewPropFilter()



    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
