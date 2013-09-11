using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtPropColumn : CswEnum<CswEnumNbtPropColumn>
    {
        private CswEnumNbtPropColumn( String Name ) : base( Name )
        {
        }

        public static IEnumerable<CswEnumNbtPropColumn> _All
        {
            get { return CswEnum<CswEnumNbtPropColumn>.All; }
        }

        public static explicit operator CswEnumNbtPropColumn( string str )
        {
            CswEnumNbtPropColumn ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswEnumNbtPropColumn Unknown = new CswEnumNbtPropColumn( "Unknown" );

        public static readonly CswEnumNbtPropColumn Field1 = new CswEnumNbtPropColumn( "Field1" );
        public static readonly CswEnumNbtPropColumn Field1_FK = new CswEnumNbtPropColumn( "Field1_FK" );
        public static readonly CswEnumNbtPropColumn Field1_Date = new CswEnumNbtPropColumn( "Field1_Date" );
        public static readonly CswEnumNbtPropColumn Field1_Numeric = new CswEnumNbtPropColumn( "Field1_Numeric" );
        public static readonly CswEnumNbtPropColumn Field2_Numeric = new CswEnumNbtPropColumn( "Field2_Numeric" );
        public static readonly CswEnumNbtPropColumn Field3_Numeric = new CswEnumNbtPropColumn( "Field3_Numeric" );
        public static readonly CswEnumNbtPropColumn Field2 = new CswEnumNbtPropColumn( "Field2" );
        public static readonly CswEnumNbtPropColumn Field2_Date = new CswEnumNbtPropColumn( "Field2_Date" );
        public static readonly CswEnumNbtPropColumn Field3 = new CswEnumNbtPropColumn( "Field3" );
        public static readonly CswEnumNbtPropColumn Field4 = new CswEnumNbtPropColumn( "Field4" );
        public static readonly CswEnumNbtPropColumn Field5 = new CswEnumNbtPropColumn( "Field5" );
        public static readonly CswEnumNbtPropColumn Gestalt = new CswEnumNbtPropColumn( "Gestalt" ); //bz # 6628
        public static readonly CswEnumNbtPropColumn GestaltSearch = new CswEnumNbtPropColumn( "GestaltSearch" ); // case 25780
        public static readonly CswEnumNbtPropColumn ClobData = new CswEnumNbtPropColumn( "ClobData" );
        public static readonly CswEnumNbtPropColumn ReadOnly = new CswEnumNbtPropColumn( "ReadOnly" );
        public static readonly CswEnumNbtPropColumn Hidden = new CswEnumNbtPropColumn( "Hidden" );
        public static readonly CswEnumNbtPropColumn PendingUpdate = new CswEnumNbtPropColumn( "PendingUpdate" );
    }

}//namespace ChemSW.Nbt.MetaData
