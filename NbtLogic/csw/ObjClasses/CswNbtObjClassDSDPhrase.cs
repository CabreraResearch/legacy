using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDSDPhrase: CswNbtPropertySetPhrase
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtPropertySetPhrase.PropertyName
        {
            public const string Category = "Category";
        }

        #endregion Enums

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDSDPhrase( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DSDPhraseClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDSDPhrase
        /// </summary>
        public static implicit operator CswNbtObjClassDSDPhrase( CswNbtNode Node )
        {
            CswNbtObjClassDSDPhrase ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DSDPhraseClass ) )
            {
                ret = (CswNbtObjClassDSDPhrase) Node.ObjClass;
            }
            return ret;
        }


        #region Inherited Events

        public override void beforePromoteNode()
        {
            _CswNbtObjClassDefault.beforePromoteNode();
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( Creating );
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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Property Set Methods

        public override void beforePropertySetWriteNode() { }

        public override void afterPropertySetWriteNode() { }

        public override void beforePropertySetDeleteNode() { }

        public override void afterPropertySetDeleteNode() { }

        public override void afterPropertySetPopulateProps() { }

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }

        #endregion

    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
