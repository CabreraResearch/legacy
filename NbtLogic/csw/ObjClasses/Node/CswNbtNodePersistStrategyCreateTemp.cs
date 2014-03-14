
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyCreateTemp : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to create and copy new temp nodes.  To create new real nodes, use CswNbtNodePersistStrategyPromoteReal.
        /// </summary>
        public CswNbtNodePersistStrategyCreateTemp(){}

        /// <summary>
        /// Strategy used to create and copy new temp nodes.  To create new real nodes, use CswNbtNodePersistStrategyPromoteReal.
        /// </summary>
        public CswNbtNodePersistStrategyCreateTemp( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            
            ForceUpdate = true;
            IsCopy = false;
            OverrideUniqueValidation = true;
            SkipEvents = false;
            AllowAuditing = false;
            Creating = true;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }
        public bool SkipEvents { get; set; }
        public bool AllowAuditing { get; set; }
        public bool Creating { get; set; }
        
        public void postChanges( CswNbtNode Node )
        {
            Node.OverrideValidation = OverrideUniqueValidation;
            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.beforeWriteNode( IsCopy, Creating );
            }

            Node.SessionId = _CswNbtResources.Session.SessionId;

            Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );

            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.afterWriteNode();
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
        }
    }
}
