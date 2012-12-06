using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFireClassExemptAmountSet : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string SetName = "Set Name";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;

        public CswNbtObjClassFireClassExemptAmountSet( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFireClassExemptAmountSet
        /// </summary>
        public static implicit operator CswNbtObjClassFireClassExemptAmountSet( CswNbtNode Node )
        {
            CswNbtObjClassFireClassExemptAmountSet ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.FireClassExemptAmountSetClass ) )
            {
                ret = (CswNbtObjClassFireClassExemptAmountSet) Node.ObjClass;
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

        public CswNbtNodePropText SetName { get { return _CswNbtNode.Properties[PropertyName.SetName]; } }

        #endregion

    }//CswNbtObjClassFireClassExemptAmountSet

}//namespace ChemSW.Nbt.ObjClasses
