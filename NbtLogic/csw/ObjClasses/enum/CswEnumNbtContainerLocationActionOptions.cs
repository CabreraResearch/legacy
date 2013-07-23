using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerLocationActionOptions : CswEnum<CswEnumNbtContainerLocationActionOptions>
    {
        private CswEnumNbtContainerLocationActionOptions( string Name ) : base( Name )
        {
        }

        public static IEnumerable<CswEnumNbtContainerLocationActionOptions> _All
        {
            get { return All; }
        }

        public static implicit operator CswEnumNbtContainerLocationActionOptions( string str )
        {
            CswEnumNbtContainerLocationActionOptions ret = Parse( str );
            return ret ?? Ignore;
        }

        public static readonly CswEnumNbtContainerLocationActionOptions Ignore = new CswEnumNbtContainerLocationActionOptions( "Ignore" );
        public static readonly CswEnumNbtContainerLocationActionOptions Undispose = new CswEnumNbtContainerLocationActionOptions( "Undispose" );
        public static readonly CswEnumNbtContainerLocationActionOptions MoveToLocation = new CswEnumNbtContainerLocationActionOptions( "Move To Location" );
        public static readonly CswEnumNbtContainerLocationActionOptions UndisposeAndMove = new CswEnumNbtContainerLocationActionOptions( "Undispose And Move" );
        public static readonly CswEnumNbtContainerLocationActionOptions MarkMissing = new CswEnumNbtContainerLocationActionOptions( "Mark Missing" );
    }
} // namespace