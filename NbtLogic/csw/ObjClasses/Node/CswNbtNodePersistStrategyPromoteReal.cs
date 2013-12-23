
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyPromoteReal : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to promote pre-existing temp nodes to real nodes.  Also used to create and copy new real nodes.
        /// </summary>
        public CswNbtNodePersistStrategyPromoteReal( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }

        public void postChanges( CswNbtNode Node )
        {
            if( Node.IsTemp )
            {
                Node.removeTemp();

                Node.checkWriter();

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.beforeCreateNode( IsCopy, OverrideUniqueValidation );
                    Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, true );
                }

                Node.requestWrite( false, IsCopy, OverrideUniqueValidation, true, false );

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.afterCreateNode();
                    Node.ObjClass.afterWriteNode( true );
                }

                Node.setModificationState( CswEnumNbtNodeModificationState.Posted );

                Node.Audit();
            }
        }
    }
}
