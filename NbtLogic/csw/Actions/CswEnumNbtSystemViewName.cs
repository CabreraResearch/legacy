using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    public sealed class CswEnumNbtSystemViewName : CswEnum<CswEnumNbtSystemViewName>
    {
        private CswEnumNbtSystemViewName( String Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtSystemViewName> all { get { return All; } }
        public static explicit operator CswEnumNbtSystemViewName( string Str )
        {
            CswEnumNbtSystemViewName Ret = Parse( Str );
            return Ret ?? Unknown;
        }
        public static readonly CswEnumNbtSystemViewName SILocationsList = new CswEnumNbtSystemViewName( "SI Locations List" );
        public static readonly CswEnumNbtSystemViewName SILocationsTree = new CswEnumNbtSystemViewName( "SI Locations Tree" );
        public static readonly CswEnumNbtSystemViewName SIInspectionsbyDate = new CswEnumNbtSystemViewName( "SI Inspections by Date" );
        public static readonly CswEnumNbtSystemViewName SIInspectionsbyBarcode = new CswEnumNbtSystemViewName( "SI Inspections by Barcode" );
        public static readonly CswEnumNbtSystemViewName SIInspectionsbyLocation = new CswEnumNbtSystemViewName( "SI Inspections by Location" );
        public static readonly CswEnumNbtSystemViewName SIInspectionsbyUser = new CswEnumNbtSystemViewName( "SI Inspections by User" );
        public static readonly CswEnumNbtSystemViewName Unknown = new CswEnumNbtSystemViewName( "Unknown" );
    }
}
