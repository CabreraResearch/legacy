using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassAliquot : CswNbtObjClass
    {
        public static string QuantityPropertyName { get { return "Quantity"; } }
        //public string IncrementPropertyName { get { return "Increment"; } }
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string SamplePropertyName { get { return "Sample"; } }
        public static string ParentAliquotPropertyName { get { return "Parent Aliquot"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassAliquot( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassAliquot( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass ); }
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

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ActionObj )
        {
            if( null != NodeTypeProp ) { /*Do Something*/ }
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName].AsQuantity ); } }
        //public CswNbtNodePropText Increment { get { return ( _CswNbtNode.Properties[IncrementPropertyName].AsText ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[BarcodePropertyName].AsBarcode ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation ); } }
        public CswNbtNodePropRelationship Sample { get { return ( _CswNbtNode.Properties[SamplePropertyName].AsRelationship ); } }
        public CswNbtNodePropRelationship ParentAliquot { get { return ( _CswNbtNode.Properties[ParentAliquotPropertyName].AsRelationship ); } }

        #endregion

    }//CswNbtObjClassAliquot

}//namespace ChemSW.Nbt.ObjClasses
