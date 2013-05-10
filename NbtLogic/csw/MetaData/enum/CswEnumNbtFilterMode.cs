using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Indicates the type of operator to be used for a property filter. 
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtFilterMode : CswEnum<CswEnumNbtFilterMode>
    {
        private CswEnumNbtFilterMode( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtFilterMode> _All { get { return CswEnum<CswEnumNbtFilterMode>.All; } }
        public static explicit operator CswEnumNbtFilterMode( string str )
        {
            CswEnumNbtFilterMode ret = Parse( str );
            return ret ?? CswEnumNbtFilterMode.Unknown;
        }
        public static readonly CswEnumNbtFilterMode Unknown = new CswEnumNbtFilterMode( "Unknown" );

        public new static readonly CswEnumNbtFilterMode Equals = new CswEnumNbtFilterMode( "Equals" );
        public static readonly CswEnumNbtFilterMode GreaterThan = new CswEnumNbtFilterMode( "GreaterThan" );
        public static readonly CswEnumNbtFilterMode GreaterThanOrEquals = new CswEnumNbtFilterMode( "GreaterThanOrEquals" );
        public static readonly CswEnumNbtFilterMode LessThan = new CswEnumNbtFilterMode( "LessThan" );
        public static readonly CswEnumNbtFilterMode LessThanOrEquals = new CswEnumNbtFilterMode( "LessThanOrEquals" );
        public static readonly CswEnumNbtFilterMode NotEquals = new CswEnumNbtFilterMode( "NotEquals" );
        //public static readonly PropertyFilterMode Range = new PropertyFilterMode("Range");
        public static readonly CswEnumNbtFilterMode Begins = new CswEnumNbtFilterMode( "Begins" );
        public static readonly CswEnumNbtFilterMode Ends = new CswEnumNbtFilterMode( "Ends" );
        public static readonly CswEnumNbtFilterMode Contains = new CswEnumNbtFilterMode( "Contains" );
        public static readonly CswEnumNbtFilterMode NotContains = new CswEnumNbtFilterMode( "NotContains" );
        public static readonly CswEnumNbtFilterMode In = new CswEnumNbtFilterMode( "In" );
        public static readonly CswEnumNbtFilterMode Null = new CswEnumNbtFilterMode( "Null" );
        public static readonly CswEnumNbtFilterMode NotNull = new CswEnumNbtFilterMode( "NotNull" );
        //public static readonly PropertyFilterMode Undefined = new PropertyFilterMode("Undefined");
    }

}//namespace ChemSW.Nbt.MetaData
