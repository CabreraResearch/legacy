using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTargetGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
        }

        public CswNbtObjClassInspectionTargetGroup( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTargetGroup
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTargetGroup( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTargetGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InspectionTargetGroupClass ) )
            {
                ret = (CswNbtObjClassInspectionTargetGroup) Node.ObjClass;
            }
            return ret;
        }

        private void _setDefaultValues()
        {
            if( string.IsNullOrEmpty( Name.Text ) )
            {
                CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );

                foreach( CswNbtMetaDataNodeType NodeType in GeneratorOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp OwnerNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
                    if( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType )
                    {
                        CswNbtMetaDataNodeType OwnerNT = _CswNbtResources.MetaData.getNodeType( OwnerNTP.FKValue );
                        if( null != OwnerNT && OwnerNT == Node.getNodeType() )
                        {
                            _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, delegate( CswNbtNode NewGenerator )
                                {
                                    if( null != NewGenerator )
                                    {
                                        //NewGenerator = (CswNbtObjClassGenerator) GeneratorNode;
                                        ( (CswNbtObjClassGenerator) NewGenerator ).Owner.RelatedNodeId = this.NodeId;
                                        ( (CswNbtObjClassGenerator) NewGenerator ).Owner.RefreshNodeName(); // 20959
                                        //GeneratorNode.postChanges( true );
                                    }
                                } );
                        }
                    } //RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType
                }
            }
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            _setDefaultValues();
        }//beforeWriteNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
