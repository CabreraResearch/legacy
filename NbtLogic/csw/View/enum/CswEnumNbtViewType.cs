using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    public sealed class CswEnumNbtViewType : CswEnum<CswEnumNbtViewType>
    {
        private CswEnumNbtViewType( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewType> _All { get { return CswEnum<CswEnumNbtViewType>.All; } }
        public static explicit operator CswEnumNbtViewType( string str )
        {
            CswEnumNbtViewType ret = Parse( str );
            return ( ret != null ) ? ret : CswEnumNbtViewType.Unknown;
        }
        public static readonly CswEnumNbtViewType Unknown = new CswEnumNbtViewType( "Unknown" );

        public static readonly CswEnumNbtViewType Root = new CswEnumNbtViewType( "Root" );
        public static readonly CswEnumNbtViewType View = new CswEnumNbtViewType( "View" );
        //public static readonly CswEnumNbtViewType ViewCategory  = new CswEnumNbtViewType( "ViewCategory " );
        public static readonly CswEnumNbtViewType Category = new CswEnumNbtViewType( "Category" );
        public static readonly CswEnumNbtViewType Action = new CswEnumNbtViewType( "Action" );
        public static readonly CswEnumNbtViewType Report = new CswEnumNbtViewType( "Report" );
        //public static readonly CswEnumNbtViewType ReportCategory = new CswEnumNbtViewType( "ReportCategory" );
        public static readonly CswEnumNbtViewType Search = new CswEnumNbtViewType( "Search" );
        public static readonly CswEnumNbtViewType RecentView = new CswEnumNbtViewType( "RecentView" );
    }
} // namespace ChemSW.Nbt
