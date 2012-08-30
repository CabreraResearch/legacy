using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


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

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSample
        /// </summary>
        public static implicit operator CswNbtObjClassSample( CswNbtNode Node )
        {
            CswNbtObjClassSample ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass ) )
            {
                ret = (CswNbtObjClassSample) Node.ObjClass;
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

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);

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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            
            
            
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
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
