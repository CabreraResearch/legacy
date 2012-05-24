using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

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

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTargetGroup
        /// </summary>
        public static explicit operator CswNbtObjClassInspectionTargetGroup( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTargetGroup ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass ) )
            {
                ret = (CswNbtObjClassInspectionTargetGroup) Node.ObjClass;
            }
            return ret;
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

            foreach( CswNbtMetaDataNodeType NodeType in GeneratorOC.getNodeTypes() )
            {
                OwnerNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName );
                if( NbtViewRelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType )
                {
                    OwnerNT = _CswNbtResources.MetaData.getNodeType( OwnerNTP.FKValue );
                    if( null != OwnerNT && OwnerNT == Node.getNodeType() )
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
                } //RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType
                //else if( RelatedIdType.ObjectClassId.ToString() == OwnerNTP.FKType )
            }

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
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

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
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
