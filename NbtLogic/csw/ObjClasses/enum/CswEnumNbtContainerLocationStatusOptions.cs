using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerLocationStatusOptions : CswEnum<CswEnumNbtContainerLocationStatusOptions>
    {
        private CswEnumNbtContainerLocationStatusOptions( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtContainerLocationStatusOptions> _All { get { return All; } }
        public static implicit operator CswEnumNbtContainerLocationStatusOptions( string str )
        {
            CswEnumNbtContainerLocationStatusOptions ret = Parse( str );
            return ret ?? NotScanned;
        }

        public static readonly CswEnumNbtContainerLocationStatusOptions Correct = new CswEnumNbtContainerLocationStatusOptions( "Moved, Dispensed, or Disposed/Undisposed" );
        public static readonly CswEnumNbtContainerLocationStatusOptions ScannedCorrect = new CswEnumNbtContainerLocationStatusOptions( "Scanned Correct" );
        public static readonly CswEnumNbtContainerLocationStatusOptions WrongLocation = new CswEnumNbtContainerLocationStatusOptions( "Scanned at Wrong Location" );
        public static readonly CswEnumNbtContainerLocationStatusOptions Disposed = new CswEnumNbtContainerLocationStatusOptions( "Scanned, but already marked Disposed" );
        public static readonly CswEnumNbtContainerLocationStatusOptions DisposedAtWrongLocation = new CswEnumNbtContainerLocationStatusOptions( "Scanned, but Disposed at Wrong Location" );
        public static readonly CswEnumNbtContainerLocationStatusOptions Missing = new CswEnumNbtContainerLocationStatusOptions( "Scanned, but already marked Missing" );
        //ContainerLocation nodes only have a status of NotScanned when used as a placeholder to Mark Missing
        public static readonly CswEnumNbtContainerLocationStatusOptions NotScanned = new CswEnumNbtContainerLocationStatusOptions( "Not Scanned" );
    }

}//namespace ChemSW.Nbt.ObjClasses

