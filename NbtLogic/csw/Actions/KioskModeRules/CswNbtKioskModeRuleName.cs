using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public sealed class CswNbtKioskModeRuleName : CswEnum<CswNbtKioskModeRuleName>
    {
        private CswNbtKioskModeRuleName( string Name ) : base( Name )
        {
        }

        public static IEnumerable<CswNbtKioskModeRuleName> _All
        {
            get { return All; }
        }

        public static implicit operator CswNbtKioskModeRuleName( string str )
        {
            CswNbtKioskModeRuleName ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswNbtKioskModeRuleName Unknown = new CswNbtKioskModeRuleName( "Unknown" );
        public static readonly CswNbtKioskModeRuleName Status = new CswNbtKioskModeRuleName( "Status" );
        public static readonly CswNbtKioskModeRuleName Move = new CswNbtKioskModeRuleName( "Move" );
        public static readonly CswNbtKioskModeRuleName Owner = new CswNbtKioskModeRuleName( "Owner" );
        public static readonly CswNbtKioskModeRuleName Transfer = new CswNbtKioskModeRuleName( "Transfer" );
        public static readonly CswNbtKioskModeRuleName Dispense = new CswNbtKioskModeRuleName( "Dispense" );
        public static readonly CswNbtKioskModeRuleName Dispose = new CswNbtKioskModeRuleName( "Dispose" );
    }
}
