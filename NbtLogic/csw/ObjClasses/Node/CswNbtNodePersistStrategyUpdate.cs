
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyUpdate : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to update pre-existing temp and real nodes.
        /// </summary>
        public CswNbtNodePersistStrategyUpdate( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }

        public void postChanges( CswNbtNode Node )
        {
            if( CswEnumNbtNodeModificationState.Modified == Node.ModificationState || ForceUpdate )
            {
                Node.checkWriter();

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, false );
                }

                Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, false, ( false == Node.IsTemp ) );

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.afterWriteNode( false );
                }

                Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
            }
        }
    }
}
