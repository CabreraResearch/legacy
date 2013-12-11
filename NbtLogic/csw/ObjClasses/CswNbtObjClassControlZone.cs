using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassControlZone : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string MAQOffset = "MAQ Offset %";
            public const string FireClassSetName = "Fire Class Set Name";
            public const string Locations = "Locations";
            public const string HMISTotals = "HMIS Totals";
            public const string HMISMaterials = "HMIS Materials";
        }

        #region ctor

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassControlZone( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassControlZone
        /// </summary>
        public static implicit operator CswNbtObjClassControlZone( CswNbtNode Node )
        {
            CswNbtObjClassControlZone ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ControlZoneClass ) )
            {
                ret = (CswNbtObjClassControlZone) Node.ObjClass;
            }
            return ret;
        }

        #endregion ctor

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropNumber MAQOffset { get { return ( _CswNbtNode.Properties[PropertyName.MAQOffset] ); } }
        public CswNbtNodePropRelationship FireClassSetName { get { return ( _CswNbtNode.Properties[PropertyName.FireClassSetName] ); } }
        public CswNbtNodePropGrid Locations { get { return ( _CswNbtNode.Properties[PropertyName.Locations] ); } }
        public CswNbtNodePropReportLink HMISMaterials { get { return ( _CswNbtNode.Properties[PropertyName.HMISMaterials] ); } }
        public CswNbtNodePropReportLink HMISTotals { get { return ( _CswNbtNode.Properties[PropertyName.HMISTotals] ); } }

        #endregion

    }//CswNbtObjClassControlZone

}//namespace ChemSW.Nbt.ObjClasses