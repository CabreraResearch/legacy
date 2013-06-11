using ChemSW.Core;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleFineTuning: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleFineTuning( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.FineTuning;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();
            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );
            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            CswNbtViewNode foundNode = Request.CurrentView.FindViewNodeByArbitraryId( Request.ArbitraryId );
            if( null != foundNode )
            {
                if( foundNode is CswNbtViewPropertyFilter )
                {
                    Return.Step6.FilterNode = (CswNbtViewPropertyFilter) foundNode;
                }
                else if( foundNode is CswNbtViewRelationship )
                {
                    Return.Step6.RelationshipNode = (CswNbtViewRelationship) foundNode;
                }
            }

            base.Finalize( Return );
            return Return;
        }

        
    }
}
