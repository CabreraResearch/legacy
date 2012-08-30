using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTargetGroup : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Name = "Name";
        }


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
        public static implicit operator CswNbtObjClassInspectionTargetGroup( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTargetGroup ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass ) )
            {
                ret = (CswNbtObjClassInspectionTargetGroup) Node.ObjClass;
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
            Name.SetOnPropChange( OnNameChange );
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

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        private void OnNameChange( CswNbtNodeProp NodeProp )
        {
            if( string.IsNullOrEmpty( Name.Text ) )
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
                                NewGenerator = (CswNbtObjClassGenerator) GeneratorNode;
                                NewGenerator.Owner.RelatedNodeId = this.NodeId;
                                NewGenerator.Owner.RefreshNodeName(); // 20959
                                GeneratorNode.postChanges( true );
                            }
                        }
                    } //RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType
                    //else if( RelatedIdType.ObjectClassId.ToString() == OwnerNTP.FKType )
                }
            }
        }
        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
