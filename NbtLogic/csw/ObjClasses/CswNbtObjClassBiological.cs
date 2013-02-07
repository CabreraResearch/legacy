using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassBiological : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            
        }

        public CswNbtObjClassBiological( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BiologicalClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassBiological
        /// </summary>
        public static implicit operator CswNbtObjClassBiological( CswNbtNode Node )
        {
            CswNbtObjClassBiological ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.BiologicalClass ) )
            {
                ret = (CswNbtObjClassBiological) Node.ObjClass;
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

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            
            
            
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        // None

        #endregion

    }//CswNbtObjClassBiological

}//namespace ChemSW.Nbt.ObjClasses
