
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

            if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Move._Name ) )
            {
                ret = new CswNbtKioskModeRuleMove( CswNbtResources );
            }
            else if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Owner._Name ) )
            {
                ret = new CswNbtKioskModeRuleOwner( CswNbtResources );
            }
            else if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Transfer._Name ) )
            {
                ret = new CswNbtKioskModeRuleTransfer( CswNbtResources );
            }
            else if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Dispense._Name ) )
            {
                ret = new CswNbtKioskModeRuleDispense( CswNbtResources );
            }
            else if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Dispose._Name ) )
            {
                ret = new CswNbtKioskModeRuleDispose( CswNbtResources );
            }
            else if( KioskModeRuleName.Equals( CswNbtKioskModeRuleName.Status._Name ) )
            {
                ret = new CswNbtKioskModeRuleStatus( CswNbtResources );
            }
            return ret;
        }
    }
}
