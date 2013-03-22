using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Central = "Central";
            public const string AutomaticCertificateApproval = "Automatic Certificate Approval";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInventoryGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInventoryGroup
        /// </summary>
        public static implicit operator CswNbtObjClassInventoryGroup( CswNbtNode Node )
        {
            CswNbtObjClassInventoryGroup ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.InventoryGroupClass ) )
            {
                ret = (CswNbtObjClassInventoryGroup) Node.ObjClass;
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
            AutomaticCertificateApproval.setHidden( Central.Checked != Tristate.True, false );
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

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropLogical Central { get { return _CswNbtNode.Properties[PropertyName.Central]; } }
        public CswNbtNodePropLogical AutomaticCertificateApproval { get { return _CswNbtNode.Properties[PropertyName.AutomaticCertificateApproval]; } }

        #endregion

    }//CswNbtObjClassInventoryGroup

}//namespace ChemSW.Nbt.ObjClasses
