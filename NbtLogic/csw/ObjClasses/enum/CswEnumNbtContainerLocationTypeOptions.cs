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
        //This Type is set to ContainerLocations created from physical Container scans for Reconciliation
        public static readonly CswEnumNbtContainerLocationTypeOptions ReconcileScans = new CswEnumNbtContainerLocationTypeOptions( "Reconcile Scans" );
        //These Types are set to ContainerLocations that are created whenever their respective actions have been performed on the related Container
        public static readonly CswEnumNbtContainerLocationTypeOptions Receipt = new CswEnumNbtContainerLocationTypeOptions( "Receipt" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Move = new CswEnumNbtContainerLocationTypeOptions( "Move" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Dispense = new CswEnumNbtContainerLocationTypeOptions( "Dispense" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Dispose = new CswEnumNbtContainerLocationTypeOptions( "Dispose" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Undispose = new CswEnumNbtContainerLocationTypeOptions( "Undispose" );
        //These Types are only used in placeholder ContainerLocations created explicitly by the user by assigning Actions in the Reconcilaition wizard.
        public static readonly CswEnumNbtContainerLocationTypeOptions Missing = new CswEnumNbtContainerLocationTypeOptions( "Missing" );
        public static readonly CswEnumNbtContainerLocationTypeOptions Ignore = new CswEnumNbtContainerLocationTypeOptions( "Ignore" );
        //This Type is used for comparison in the Reconciliation wizard and should not be set on any ContainerLocations
        public static readonly CswEnumNbtContainerLocationTypeOptions Touch = new CswEnumNbtContainerLocationTypeOptions( "Moves, Dispenses, Disposals/Undisposals" );
    }
}//namespace ChemSW.Nbt.ObjClasses

