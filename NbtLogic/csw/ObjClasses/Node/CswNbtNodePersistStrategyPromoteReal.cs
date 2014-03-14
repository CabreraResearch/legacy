
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
            Node.removeTemp();

            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.beforePromoteNode();
                Node.ObjClass.beforeWriteNode( IsCopy, Creating );
            }

            if( CswEnumNbtNodeSpecies.Plain == Node.NodeSpecies )
            {
                Node.Properties.update( Node, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );
                Node.syncNodeName();
                Node.write( ForceUpdate );
            }

            if( null != Node.ObjClass && false == SkipEvents )
            {
                Node.ObjClass.afterPromoteNode();
                Node.ObjClass.afterWriteNode();
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );

            Node.Audit();
        }
    }
}
