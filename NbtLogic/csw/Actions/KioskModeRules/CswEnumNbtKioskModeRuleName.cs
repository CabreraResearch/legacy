using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public sealed class CswEnumNbtKioskModeRuleName : CswEnum<CswEnumNbtKioskModeRuleName>
    {
        private CswEnumNbtKioskModeRuleName( string Name ) : base( Name )
        {
        }

        public static IEnumerable<CswEnumNbtKioskModeRuleName> _All
        {
            get { return All; }
        }

        public static implicit operator CswEnumNbtKioskModeRuleName( string str )
        {
            CswEnumNbtKioskModeRuleName ret = Parse( str );
            return ret ?? Unknown;
        }

        public static readonly CswEnumNbtKioskModeRuleName Unknown = new CswEnumNbtKioskModeRuleName( "Unknown" );
        public static readonly CswEnumNbtKioskModeRuleName Status = new CswEnumNbtKioskModeRuleName( "Status" );
        public static readonly CswEnumNbtKioskModeRuleName Move = new CswEnumNbtKioskModeRuleName( "Move" );
        public static readonly CswEnumNbtKioskModeRuleName Owner = new CswEnumNbtKioskModeRuleName( "Owner" );
        public static readonly CswEnumNbtKioskModeRuleName Transfer = new CswEnumNbtKioskModeRuleName( "Transfer" );
        public static readonly CswEnumNbtKioskModeRuleName Dispense = new CswEnumNbtKioskModeRuleName( "Dispense" );
        public static readonly CswEnumNbtKioskModeRuleName Dispose = new CswEnumNbtKioskModeRuleName( "Dispose" );
    }
}
