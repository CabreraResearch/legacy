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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass ); }
        }

        public sealed class PropertyName
        {
            public const string Material = "Material";
            public const string Name = "Name";
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialSynonym
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialSynonym( CswNbtNode Node )
        {
            CswNbtObjClassMaterialSynonym ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass ) )
            {
                ret = (CswNbtObjClassMaterialSynonym) Node.ObjClass;
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

        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

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
