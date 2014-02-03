
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
            OverrideMailReportEvents = false;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }
        public bool SkipEvents { get; set; }
        public bool AllowAuditing { get; set; }
        public bool Creating { get; set; }
        public bool OverrideMailReportEvents { get; set; }
        
        public void postChanges( CswNbtNode Node )
        {
            Node.checkWriter();

            if( null != Node.ObjClass )
            {
                Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
            }

            Node.SessionId = _CswNbtResources.Session.SessionId;

            Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );

            if( null != Node.ObjClass )
            {
                Node.ObjClass.afterWriteNode( OverrideMailReportEvents );
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
        }
    }
}
