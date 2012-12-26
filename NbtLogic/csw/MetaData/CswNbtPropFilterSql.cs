using System;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{

    public class CswNbtPropFilterSql
    {
        ///// <summary>
        ///// Indicates the type of operator to be used for a property filter. 
        ///// The numbers assigned to the enum values is absolutely required in 
        ///// order for bitwise operators to work. 
        ///// </summary>
        //[Flags]
        //public enum PropertyFilterMode
        //{
        //    Equals = 1,
        //    GreaterThan = 2,
        //    GreaterThanOrEquals = 4,
        //    LessThan = 8,
        //    LessThanOrEquals = 16,
        //    NotEquals = 32,
        //    //Range = 64,
        //    Begins = 64,
        //    Ends = 128,
        //    Contains = 256,
        //    //            In = 1024,
        //    Null = 512,
        //    NotNull = 1024,
        //    Undefined = 2048
        //};

        /// <summary>
        /// Indicates the type of operator to be used for a property filter. 
        /// </summary>
        public sealed class PropertyFilterMode : CswEnum<PropertyFilterMode>
        {
            private PropertyFilterMode( String Name ) : base( Name ) { }
            public static IEnumerable<PropertyFilterMode> _All { get { return CswEnum<PropertyFilterMode>.All; } }
            public static explicit operator PropertyFilterMode( string str )
            {
                PropertyFilterMode ret = Parse( str );
                return ( ret != null ) ? ret : PropertyFilterMode.Unknown;
            }
            public static readonly PropertyFilterMode Unknown = new PropertyFilterMode( "Unknown" );

            public new static readonly PropertyFilterMode Equals = new PropertyFilterMode( "Equals" );
            public static readonly PropertyFilterMode GreaterThan = new PropertyFilterMode( "GreaterThan" );
            public static readonly PropertyFilterMode GreaterThanOrEquals = new PropertyFilterMode( "GreaterThanOrEquals" );
            public static readonly PropertyFilterMode LessThan = new PropertyFilterMode( "LessThan" );
            public static readonly PropertyFilterMode LessThanOrEquals = new PropertyFilterMode( "LessThanOrEquals" );
            public static readonly PropertyFilterMode NotEquals = new PropertyFilterMode( "NotEquals" );
            //public static readonly PropertyFilterMode Range = new PropertyFilterMode("Range");
            public static readonly PropertyFilterMode Begins = new PropertyFilterMode( "Begins" );
            public static readonly PropertyFilterMode Ends = new PropertyFilterMode( "Ends" );
            public static readonly PropertyFilterMode Contains = new PropertyFilterMode( "Contains" );
            public static readonly PropertyFilterMode NotContains = new PropertyFilterMode( "NotContains" );
            //public static readonly PropertyFilterMode In = new PropertyFilterMode("In");
            public static readonly PropertyFilterMode Null = new PropertyFilterMode( "Null" );
            public static readonly PropertyFilterMode NotNull = new PropertyFilterMode( "NotNull" );
            //public static readonly PropertyFilterMode Undefined = new PropertyFilterMode("Undefined");
        }

        /// <summary>
        /// Indicates how to treat results that are filtered out
        /// </summary>
        public sealed class FilterResultMode : CswEnum<FilterResultMode>
        {
            private FilterResultMode( String Name ) : base( Name ) { }
            public static IEnumerable<FilterResultMode> _All { get { return All; } }
            public static explicit operator FilterResultMode( string str )
            {
                FilterResultMode ret = Parse( str );
                return ( ret != null ) ? ret : FilterResultMode.Unknown;
            }
            public static readonly FilterResultMode Unknown = new FilterResultMode( "Unknown" );

            public static readonly FilterResultMode Hide = new FilterResultMode( "Hide" );
            public static readonly FilterResultMode Disabled = new FilterResultMode( "Disabled" );
        }
        

        //public enum PropertyFilterConjunction { And, AndNot };

        /// <summary>
        /// Filter Conjunction
        /// </summary>
        public sealed class PropertyFilterConjunction : CswEnum<PropertyFilterConjunction>
        {
            private PropertyFilterConjunction( string Name ) : base( Name ) { }
            public static IEnumerable<PropertyFilterConjunction> _All { get { return All; } }
            public static implicit operator PropertyFilterConjunction( string str )
            {
                PropertyFilterConjunction ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly PropertyFilterConjunction Unknown = new PropertyFilterConjunction( "Unknown" );

            public static readonly PropertyFilterConjunction And = new PropertyFilterConjunction( "And" );
            public static readonly PropertyFilterConjunction Or = new PropertyFilterConjunction( "Or" );
        }

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
            if( CswNbtPropFilterSql.PropertyFilterMode.Null != CswNbtViewPropertyFilterIn.FilterMode &&
                 CswNbtPropFilterSql.PropertyFilterMode.NotNull != CswNbtViewPropertyFilterIn.FilterMode )
            {
                if( UseNumericHack )
                {
                    if(
                        CswNbtPropFilterSql.PropertyFilterMode.Begins == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswNbtPropFilterSql.PropertyFilterMode.Contains == CswNbtViewPropertyFilterIn.FilterMode ||
                        CswNbtPropFilterSql.PropertyFilterMode.Ends == CswNbtViewPropertyFilterIn.FilterMode
                        )
                    {
                        throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for numeric values" ) );
                    }


                    string NumericValueColumn = " nvl(" + FilterTableAlias + Column + ", 0) ";

                    if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                    {
                        ReturnVal = NumericValueColumn + " = " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThan )
                    {
                        ReturnVal = NumericValueColumn + " > " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals )
                    {
                        ReturnVal = NumericValueColumn + " >= " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThan )
                    {
                        ReturnVal = NumericValueColumn + " < " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals )
                    {
                        ReturnVal = NumericValueColumn + " <= " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                    {
                        ReturnVal = NumericValueColumn + " <> " + CswNbtViewPropertyFilterIn.Value;
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
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


                    if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Begins )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "%'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Contains )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "%'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotContains )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + "'%" + SafeValue + "%'" + CaseAppend
                            + " or " + NonNumericValueColumn + " is null" + ")";
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Ends )
                    {
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'%" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                    {
                        //covers the case of clobs
                        ReturnVal = NonNumericValueColumn + " like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThan )
                    {
                        ReturnVal = NonNumericValueColumn + " > " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " >= " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThan )
                    {
                        ReturnVal = NonNumericValueColumn + " < " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals )
                    {
                        ReturnVal = NonNumericValueColumn + " <= " + CasePrepend + "'" + SafeValue + "'" + CaseAppend;
                    }
                    else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                    {
                        ReturnVal = "(" + NonNumericValueColumn + " not like " + CasePrepend + "'" + SafeValue + "'" + CaseAppend +
                                    " or " + NonNumericValueColumn + " is null )";   //case 21623
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter()) { " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                    }


                }//if-else UserNumericHack
            }
            else
            {
                string NullValueColumn = FilterTableAlias + Column;

                if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
                {
                    ReturnVal = NullValueColumn + " is not null";
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                {
                    ReturnVal = NullValueColumn + " is null";
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtPropFilterSql.renderViewPropFilter(): " + CswNbtViewPropertyFilterIn.FilterMode.ToString() );
                }
            }//if-else filter mode is not null or not-null

            return ( ReturnVal );

        }//renderViewPropFilter()



    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
