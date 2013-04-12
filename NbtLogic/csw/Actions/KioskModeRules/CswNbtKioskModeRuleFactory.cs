
namespace ChemSW.Nbt.Actions.KioskMode
{
    public static class CswNbtKioskModeRuleFactory
    {
        public static CswNbtKioskModeRule Make( CswNbtResources CswNbtResources, CswEnumNbtKioskModeRuleName KioskModeRuleName )
        {
            return Make( CswNbtResources, KioskModeRuleName._Name );
        }

        public static CswNbtKioskModeRule Make( CswNbtResources CswNbtResources, string KioskModeRuleName )
        {
            CswNbtKioskModeRule ret = null;

            string loweredName = KioskModeRuleName.ToLower();

            if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Move._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleMove( CswNbtResources );
            }
            else if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Owner._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleOwner( CswNbtResources );
            }
            else if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Transfer._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleTransfer( CswNbtResources );
            }
            else if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Dispense._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleDispense( CswNbtResources );
            }
            else if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Dispose._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleDispose( CswNbtResources );
            }
            else if( loweredName.Equals( CswEnumNbtKioskModeRuleName.Status._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleStatus( CswNbtResources );
            }
            return ret;
        }
    }
}
