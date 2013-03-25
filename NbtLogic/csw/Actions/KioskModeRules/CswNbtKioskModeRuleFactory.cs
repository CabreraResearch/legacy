
namespace ChemSW.Nbt.Actions.KioskMode
{
    public static class CswNbtKioskModeRuleFactory
    {
        public static CswNbtKioskModeRule Make( CswNbtResources CswNbtResources, CswNbtKioskModeRuleName KioskModeRuleName )
        {
            return Make( CswNbtResources, KioskModeRuleName._Name );
        }

        public static CswNbtKioskModeRule Make( CswNbtResources CswNbtResources, string KioskModeRuleName )
        {
            CswNbtKioskModeRule ret = null;

            string loweredName = KioskModeRuleName.ToLower();

            if( loweredName.Equals( CswNbtKioskModeRuleName.Move._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleMove( CswNbtResources );
            }
            else if( loweredName.Equals( CswNbtKioskModeRuleName.Owner._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleOwner( CswNbtResources );
            }
            else if( loweredName.Equals( CswNbtKioskModeRuleName.Transfer._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleTransfer( CswNbtResources );
            }
            else if( loweredName.Equals( CswNbtKioskModeRuleName.Dispense._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleDispense( CswNbtResources );
            }
            else if( loweredName.Equals( CswNbtKioskModeRuleName.Dispose._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleDispose( CswNbtResources );
            }
            else if( loweredName.Equals( CswNbtKioskModeRuleName.Status._Name.ToLower() ) )
            {
                ret = new CswNbtKioskModeRuleStatus( CswNbtResources );
            }
            return ret;
        }
    }
}
