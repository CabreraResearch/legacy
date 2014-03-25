
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyUpdate : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to update pre-existing temp and real nodes.  Will not update unmodified nodes unless forced.
        /// </summary>
        public CswNbtNodePersistStrategyUpdate(){}

        /// <summary>
        /// Strategy used to update pre-existing temp and real nodes.  Will not update unmodified nodes unless forced.
        /// </summary>
        public CswNbtNodePersistStrategyUpdate( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            ForceUpdate = false;
            IsCopy = false;
            OverrideUniqueValidation = false;
            SkipEvents = false;
            AllowAuditing = true;
            Creating = false;
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
            if( CswEnumNbtNodeModificationState.Modified == Node.ModificationState || ForceUpdate )
            {
                if( null != Node.ObjClass && false == SkipEvents)
                {
                    Node.ObjClass.beforeWriteNode( IsCopy, Creating );
                }

                Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing && ( false == Node.IsTemp ), SkipEvents );

                if( null != Node.ObjClass && false == SkipEvents )
                {
                    Node.ObjClass.afterWriteNode();
                }

                Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
            }
        }
    }
}
