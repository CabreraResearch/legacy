using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCofAMethodTemplate : CswNbtObjClass
    {
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string CertMethodId = "C of A Method ID";
            public const string Description = "Description";
            public const string MethodNo = "Method No";
            public const string Conditions = "Conditions";
            public const string Lower = "Lower";
            public const string Upper = "Upper";
            public const string Units = "Units";
            public const string Qualified = "Qualified";
            public const string CertDefConditionSet = "CertDef Condition Set";
            public const string Obsolete = "Obsolete";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassCofAMethodTemplate( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.CofAMethodTemplateClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGeneric
        /// </summary>
        public static implicit operator CswNbtObjClassCofAMethodTemplate( CswNbtNode Node )
        {
            CswNbtObjClassCofAMethodTemplate ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.CofAMethodTemplateClass ) )
            {
                ret = (CswNbtObjClassCofAMethodTemplate) Node.ObjClass;
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

        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        //TODO: Implement this property when the PropRefSequence FieldType is implemented
        //public CswNbtNodePropPropRefSequence Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropText Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropText MethodNo { get { return ( _CswNbtNode.Properties[PropertyName.MethodNo] ); } }
        public CswNbtNodePropText Conditions { get { return ( _CswNbtNode.Properties[PropertyName.Conditions] ); } }
        public CswNbtNodePropNumber Lower { get { return ( _CswNbtNode.Properties[PropertyName.Lower] ); } }
        public CswNbtNodePropNumber Upper { get { return ( _CswNbtNode.Properties[PropertyName.Upper] ); } }
        public CswNbtNodePropText Units { get { return ( _CswNbtNode.Properties[PropertyName.Units] ); } }
        public CswNbtNodePropLogical Qualified { get { return ( _CswNbtNode.Properties[PropertyName.Qualified] ); } }
        public CswNbtNodePropRelationship CertDefContionSet { get { return ( _CswNbtNode.Properties[PropertyName.CertDefConditionSet] ); } }
        public CswNbtNodePropLogical Obsolete { get { return ( _CswNbtNode.Properties[PropertyName.Obsolete] ); } }

        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
