using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTargetGroup : CswNbtObjClass
    {
        public static string NamePropertyName { get { return "Name"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionTargetGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public CswNbtObjClassInspectionTargetGroup( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass ); }
        }

        #region Inherited Events

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp OwnerOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName );
            CswNbtMetaDataNodeTypeProp OwnerNTP;
            CswNbtMetaDataNodeType OwnerNT;
            //CswNbtMetaDataObjectClass OwnerOC;
            CswNbtNode GeneratorNode;
            CswNbtObjClassGenerator NewGenerator;

            foreach( CswNbtMetaDataNodeType NodeType in GeneratorOC.NodeTypes )
            {
                OwnerNTP = NodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                if( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType )
                {
                    OwnerNT = _CswNbtResources.MetaData.getNodeType( OwnerNTP.FKValue );
                    if( null != OwnerNT && OwnerNT == Node.NodeType )
                    {
                        GeneratorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                        if( null != GeneratorNode )
                        {
                            NewGenerator = CswNbtNodeCaster.AsGenerator( GeneratorNode );
                            NewGenerator.Owner.RelatedNodeId = this.NodeId;
                            NewGenerator.Owner.RefreshNodeName(); // 20959
                            GeneratorNode.postChanges( true );
                        }
                    }
                } //CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType
                //else if( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() == OwnerNTP.FKType )
            }

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
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

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
