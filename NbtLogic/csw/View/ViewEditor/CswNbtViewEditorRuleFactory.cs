using ChemSW.Exceptions;

namespace ChemSW.Nbt.ViewEditor
{
    public static class CswNbtViewEditorRuleFactory
    {
        public static CswNbtViewEditorRule Make( CswNbtResources CswNbtResources, CswEnumNbtViewEditorRuleName RuleName, CswNbtViewEditorData Request )
        {
            return Make( CswNbtResources, RuleName._Name, Request );
        }

        public static CswNbtViewEditorRule Make( CswNbtResources CswNbtResources, string RuleName, CswNbtViewEditorData Request )
        {
            CswNbtViewEditorRule Ret = null;

            if( RuleName.Equals( CswEnumNbtViewEditorRuleName.FirstViewLevel._Name ) )
            {
                Ret = new CswNbtViewEditorRuleFirstViewLevel( CswNbtResources, Request );
            }
            else if( RuleName.Equals( CswEnumNbtViewEditorRuleName.AddViewLevels._Name ) )
            {
                Ret = new CswNbtViewEditorRuleAddViewLevels( CswNbtResources, Request );
            }
            else if( RuleName.Equals( CswEnumNbtViewEditorRuleName.SetFilters._Name ) )
            {
                Ret = new CswNbtViewEditorRuleSetFilters( CswNbtResources, Request );
            }
            else if( RuleName.Equals( CswEnumNbtViewEditorRuleName.ViewAttributes._Name ) )
            {
                Ret = new CswNbtViewEditorRuleViewAttributes( CswNbtResources, Request );
            }
            else if( RuleName.Equals( CswEnumNbtViewEditorRuleName.FineTuning._Name ) )
            {
                Ret = new CswNbtViewEditorRuleFineTuning( CswNbtResources, Request );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot interpret the supplied View Editor rule name.", "An invalid rule name: " + RuleName + " was passed to CswNbtViewRuleFactory" );
            }

            return Ret;
        }
    }
}
