
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyCreateTemp : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to create and copy new temp nodes.  To create new real nodes, use CswNbtNodePersistStrategyPromoteReal.
        /// </summary>
        public CswNbtNodePersistStrategyCreateTemp( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }

        public void postChanges( CswNbtNode Node )
        {
            Node.checkWriter();

            if( null != Node.ObjClass )
            {
                Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, true );
            }

            Node.requestWrite( true, IsCopy, OverrideUniqueValidation, true, ( false == Node.IsTemp ) );

            if( null != Node.ObjClass )
            {
                Node.ObjClass.afterWriteNode( true );
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
        }
    }
}
