using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterialSynonym : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterialSynonym( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialSynonymClass ); }
        }

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string Name = "Name";
            public const string Type = "Type";
            public const string Language = "Language";
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialSynonym
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialSynonym( CswNbtNode Node )
        {
            CswNbtObjClassMaterialSynonym ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MaterialSynonymClass ) )
            {
                ret = (CswNbtObjClassMaterialSynonym) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforePromoteNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );

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

        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropList Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropList Language { get { return ( _CswNbtNode.Properties[PropertyName.Language] ); } }

        //public CswNbtNodePropRelationship Assembly
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[_CswNbtObjClassRuleEquipment.AssemblyPropertyName].AsRelationship );
        //    }
        //}
        //public CswNbtNodePropRelationship Type
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[_CswNbtObjClassRuleEquipment.TypePropertyName].AsRelationship );
        //    }
        //}
        //public CswNbtNodePropLogicalSet Parts
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[_CswNbtObjClassRuleEquipment.PartsPropertyName].AsLogicalSet );
        //    }
        //}


        #endregion

    }//CswNbtObjClassMaterialSynonym

}//namespace ChemSW.Nbt.ObjClasses
