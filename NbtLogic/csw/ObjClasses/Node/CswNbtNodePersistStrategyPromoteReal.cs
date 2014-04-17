
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyPromoteReal : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to promote pre-existing temp nodes to real nodes.  Also used to create and copy new real nodes.
        /// </summary>
        public CswNbtNodePersistStrategyPromoteReal(){}

        /// <summary>
        /// Strategy used to promote pre-existing temp nodes to real nodes.  Also used to create and copy new real nodes.
        /// </summary>
        public CswNbtNodePersistStrategyPromoteReal( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            ForceUpdate = false;
            IsCopy = false;
            OverrideUniqueValidation = false;
            SkipEvents = false;
            AllowAuditing = false;
            Creating = false;
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
            Node.removeTemp();

            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.beforePromoteNode( OverrideUniqueValidation: OverrideUniqueValidation );
                Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
            }

            // AllowAuditing should be false here, so that we record the audit event as an INSERT instead of an UPDATE
            Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );

            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.afterPromoteNode();
                Node.ObjClass.afterWriteNode( OverrideMailReportEvents );
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );

            // This audit will record as an INSERT
            Node.Audit();
        }
    }
}
