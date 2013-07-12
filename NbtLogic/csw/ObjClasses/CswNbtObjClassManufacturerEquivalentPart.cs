using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassManufacturerEquivalentPart : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string EnterprisePart = "Enterprise Part";
            public const string Manufacturer = "Manufacturer";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassManufacturerEquivalentPart( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerEquivalentPartClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassManufacturerEquivalentPart
        /// </summary>
        public static implicit operator CswNbtObjClassManufacturerEquivalentPart( CswNbtNode Node )
        {
            CswNbtObjClassManufacturerEquivalentPart ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ManufacturerEquivalentPartClass ) )
            {
                ret = (CswNbtObjClassManufacturerEquivalentPart) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

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

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropRelationship EnterprisePart { get { return _CswNbtNode.Properties[PropertyName.EnterprisePart]; } }
        public CswNbtNodePropRelationship Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }

        #endregion

    }//CswNbtObjClassManufacturerEquivalentPart

}//namespace ChemSW.Nbt.ObjClasses
