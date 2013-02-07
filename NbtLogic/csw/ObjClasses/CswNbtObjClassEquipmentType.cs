using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipmentType : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Parts = "Parts";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEquipmentType( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentTypeClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipmentType
        /// </summary>
        public static implicit operator CswNbtObjClassEquipmentType( CswNbtNode Node )
        {
            CswNbtObjClassEquipmentType ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.EquipmentTypeClass ) )
            {
                ret = (CswNbtObjClassEquipmentType) Node.ObjClass;
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

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties


        public CswNbtNodePropMemo Parts
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Parts] );
            }
        }

        #endregion


    }//CswNbtObjClassEquipmentType

}//namespace ChemSW.Nbt.ObjClasses
