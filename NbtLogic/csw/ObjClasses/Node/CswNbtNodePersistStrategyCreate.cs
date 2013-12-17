
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyCreate : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtNodePersistStrategyCreate( CswNbtResources CswNbtResources )
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
