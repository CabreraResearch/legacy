using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSample : CswNbtObjClass
    {
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string QuantityPropertyName { get { return "Quantity"; } }
        public static string ParentSamplePropertyName { get { return "Parent Sample"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassSample( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassSample( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            if( null != NodeTypeProp ) { /*Do Something*/ }
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[QuantityPropertyName].AsQuantity; }
        }
        public CswNbtNodePropBarcode Barcode
        {
            get { return _CswNbtNode.Properties[BarcodePropertyName].AsBarcode; }
        }
        public CswNbtNodePropRelationship ParentSample
        {
            get { return _CswNbtNode.Properties[ParentSamplePropertyName].AsRelationship; }
        }

        #endregion


    }//CswNbtObjClassSample

}//namespace ChemSW.Nbt.ObjClasses
