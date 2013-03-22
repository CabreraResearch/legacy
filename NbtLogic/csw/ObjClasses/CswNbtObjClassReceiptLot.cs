using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReceiptLot : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            //public const string ReceiptLotNo = "Receipt Lot No"; //waiting on 27877
            public const string Material = "Material";
            //public const string MaterialID = "Material ID"; //waiting on 27864
            public const string ExpirationDate = "Expiration Date";
            //public const string Certificates = "Certificates"; //waiting for Certificate ObjClass to be implemented (allegedly in William)
            public const string UnderInvestigation = "Under Investigation";
            public const string InvestigationNotes = "Investigation Notes";
            public const string Manufacturer = "Manufacturer";
            public const string RequestItem = "Request Item";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReceiptLot( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReceiptLot
        /// </summary>
        public static implicit operator CswNbtObjClassReceiptLot( CswNbtNode Node )
        {
            CswNbtObjClassReceiptLot ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.ReceiptLotClass ) )
            {
                ret = (CswNbtObjClassReceiptLot) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

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

        //public CswNbtNodePropPropRefSequence ReceiptLotNo { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotNo]; } } //waiting on 27877
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        //public CswNbtNodePropPropertyReference MaterialID { get { return _CswNbtNode.Properties[PropertyName.MaterialID]; } } //waiting on 27864
        public CswNbtNodePropDateTime ExpirationDate { get { return _CswNbtNode.Properties[PropertyName.ExpirationDate]; } }
        //public CswNbtNodePropGrid Certificates { get { return _CswNbtNode.Properties[PropertyName.Certificates]; } } //waiting for Certificate ObjClass to be implemented (allegedly in William)
        public CswNbtNodePropLogical UnderInvestigation { get { return _CswNbtNode.Properties[PropertyName.UnderInvestigation]; } }
        public CswNbtNodePropComments InvestigationNotes { get { return _CswNbtNode.Properties[PropertyName.InvestigationNotes]; } }
        public CswNbtNodePropRelationship Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }
        public CswNbtNodePropRelationship RequestItem { get { return _CswNbtNode.Properties[PropertyName.RequestItem]; } }

        #endregion

    }//CswNbtObjClassReceiptLot

}//namespace ChemSW.Nbt.ObjClasses
