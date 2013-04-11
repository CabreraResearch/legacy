using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Event Options
    /// </summary>
    public sealed class CswEnumNbtMailReportEventOption : CswEnum<CswEnumNbtMailReportEventOption>
    {
        private CswEnumNbtMailReportEventOption( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtMailReportEventOption> _All { get { return All; } }
        public static implicit operator CswEnumNbtMailReportEventOption( string str )
        {
            CswEnumNbtMailReportEventOption ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtMailReportEventOption Unknown = new CswEnumNbtMailReportEventOption( "Unknown" );

        public static readonly CswEnumNbtMailReportEventOption Exists = new CswEnumNbtMailReportEventOption( "Exists" );
        //public static readonly EventOption Create = new EventOption( "Create" );
        public static readonly CswEnumNbtMailReportEventOption Edit = new CswEnumNbtMailReportEventOption( "Edit" );
        //public static readonly EventOption Delete = new EventOption( "Delete" );
    }

}//namespace ChemSW.Nbt.ObjClasses
