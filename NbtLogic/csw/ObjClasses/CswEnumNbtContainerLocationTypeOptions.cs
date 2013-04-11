using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerLocationTypeOptions : CswEnum<CswEnumNbtContainerLocationTypeOptions>
    {
        private CswEnumNbtContainerLocationTypeOptions( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtContainerLocationTypeOptions> _All { get { return All; } }
        public static implicit operator CswEnumNbtContainerLocationTypeOptions( string str )
        {
            CswEnumNbtContainerLocationTypeOptions ret = Parse( str );
            return ret ?? Missing;
        }
        public static readonly CswEnumNbtContainerLocationTypeOptions Scan = new CswEnumNbtContainerLocationTypeOptions( "Scan" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Receipt = new CswEnumNbtContainerLocationTypeOptions( "Receipt" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Move = new CswEnumNbtContainerLocationTypeOptions( "Move" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Dispense = new CswEnumNbtContainerLocationTypeOptions( "Dispense" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Dispose = new CswEnumNbtContainerLocationTypeOptions( "Dispose" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Undispose = new CswEnumNbtContainerLocationTypeOptions( "Undispose" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Missing = new CswEnumNbtContainerLocationTypeOptions( "Missing" );
    }
}//namespace ChemSW.Nbt.ObjClasses

