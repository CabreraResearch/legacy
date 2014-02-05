﻿
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

            if( null != Node.ObjClass )
            {
                Node.ObjClass.beforePromoteNode( OverrideUniqueValidation: OverrideUniqueValidation );
                Node.ObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
            }

            Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );

            if( null != Node.ObjClass )
            {
                Node.ObjClass.afterPromoteNode();
                Node.ObjClass.afterWriteNode( OverrideMailReportEvents );
            }

            Node.setModificationState( CswEnumNbtNodeModificationState.Posted );

            Node.Audit();
        }
    }
}
