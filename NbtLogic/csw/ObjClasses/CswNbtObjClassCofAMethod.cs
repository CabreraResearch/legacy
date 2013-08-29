using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCofAMethod : CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string CertMethodTemplate = "C of A Method Template";
            public const string ReceiptLot = "Receipt Lot";
            public const string Description = "Description";
            public const string MethodNo = "Method No";
            public const string Conditions = "Conditions";
            public const string Lower = "Lower";
            public const string Upper = "Upper";
            public const string Value = "Value";
            public const string Units = "Units";
            public const string Qualified = "Qualified";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassCofAMethod( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofAMethodClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGeneric
        /// </summary>
        public static implicit operator CswNbtObjClassCofAMethod( CswNbtNode Node )
        {
            CswNbtObjClassCofAMethod ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CofAMethodClass ) )
            {
                ret = (CswNbtObjClassCofAMethod) Node.ObjClass;
            }
            return ret;
        }


        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()

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

        public CswNbtNodePropRelationship CertMethodTemplate { get { return ( _CswNbtNode.Properties[PropertyName.CertMethodTemplate] ); } }
        //TODO: Implement this property when the Receipt Lot ObjectClass is implemented
        //public CswNbtNodePropRelationship ReceiptLot { get { return ( _CswNbtNode.Properties[PropertyName.ReceiptLot] ); } }
        public CswNbtNodePropText Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropText MethodNo { get { return ( _CswNbtNode.Properties[PropertyName.MethodNo] ); } }
        public CswNbtNodePropText Conditions { get { return ( _CswNbtNode.Properties[PropertyName.Conditions] ); } }
        public CswNbtNodePropNumber Lower { get { return ( _CswNbtNode.Properties[PropertyName.Lower] ); } }
        public CswNbtNodePropNumber Upper { get { return ( _CswNbtNode.Properties[PropertyName.Upper] ); } }
        public CswNbtNodePropNumber Value { get { return ( _CswNbtNode.Properties[PropertyName.Value] ); } }
        public CswNbtNodePropText Units { get { return ( _CswNbtNode.Properties[PropertyName.Units] ); } }
        public CswNbtNodePropLogical Qualified { get { return ( _CswNbtNode.Properties[PropertyName.Qualified] ); } }

        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
