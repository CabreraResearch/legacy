using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassAliquot : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Quantity = "Quantity";
            //public string IncrementPropertyName { get { return "Increment"; } }
            public const string Barcode = "Barcode";
            public const string Location = "Location";
            public const string Sample = "Sample";
            public const string ParentAliquot = "Parent Aliquot";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassAliquot( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.AliquotClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassAliquot
        /// </summary>
        public static implicit operator CswNbtObjClassAliquot( CswNbtNode Node )
        {
            CswNbtObjClassAliquot ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.AliquotClass ) )
            {
                ret = (CswNbtObjClassAliquot) Node.ObjClass;
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

        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[PropertyName.Quantity] ); } }
        //public CswNbtNodePropText Increment { get { return ( _CswNbtNode.Properties[IncrementPropertyName] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropRelationship Sample { get { return ( _CswNbtNode.Properties[PropertyName.Sample] ); } }
        public CswNbtNodePropRelationship ParentAliquot { get { return ( _CswNbtNode.Properties[PropertyName.ParentAliquot] ); } }

        #endregion

    }//CswNbtObjClassAliquot

}//namespace ChemSW.Nbt.ObjClasses
