using System.Collections;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActAssignTests
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtActAssignTests( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public delegate void NewNodeHandler( object CswNbtNode );
        public NewNodeHandler onBeforeInsertNode = null;


        /// <summary>
        /// Creates a result for each parameter of each test, for each Aliquot
        /// </summary>
        /// <param name="AliquotNodeKeys">ArrayList of Aliquot NodeKeys</param>
        /// <param name="TestNodeKeys">ArrayList of Test NodeKeys</param>
        public Collection<CswPrimaryKey> AssignTest( Collection<CswNbtNodeKey> AliquotNodeKeys, Collection<CswNbtNodeKey> TestNodeKeys )
        {
            Collection<CswPrimaryKey> ResultNodeIds = new Collection<CswPrimaryKey>();

            // First, get all parameters for all tests
            CswNbtMetaDataObjectClass TestObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.TestClass );
            CswNbtMetaDataObjectClass ParameterObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ParameterClass );
            CswNbtMetaDataObjectClassProp ParameterTestObjectClassProp = ParameterObjectClass.getObjectClassProp( CswNbtObjClassParameter.PropertyName.Test );
            
            CswNbtView ParameterView = new CswNbtView( _CswNbtResources );
            ParameterView.ViewName = "AssignTest().Parameters";

            CswNbtViewRelationship TestRel = ParameterView.AddViewRelationship( TestObjectClass, false );
            foreach( CswNbtNodeKey TestNodeKey in TestNodeKeys )
                TestRel.NodeIdsToFilterIn.Add( TestNodeKey.NodeId );

            CswNbtViewRelationship ParamRel = ParameterView.AddViewRelationship( TestRel, NbtViewPropOwnerType.Second, ParameterTestObjectClassProp, false );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( ParameterView, true, true, false, false );

            ArrayList ParamNodes = new ArrayList();
            for( int j = 0; j < Tree.getChildNodeCount(); j++ )
            {
                Tree.goToNthChild( j );  // Test
                for( int i = 0; i < Tree.getChildNodeCount(); i++ )
                {
                    Tree.goToNthChild( i );  // Parameter
                    CswNbtNode ParamNode = Tree.getNodeForCurrentPosition();
                    ParamNodes.Add( ParamNode );
                    Tree.goToRoot();
                }
            }

            // Next, create results matching each Parameter for each Aliquot

            foreach( CswNbtNodeKey AliquotNodeKey in AliquotNodeKeys )
            {
                CswNbtNode AliquotNode = _CswNbtResources.Nodes[AliquotNodeKey];
                foreach( CswNbtNode ParamNode in ParamNodes )
                {
                    CswNbtObjClassParameter ParameterObjClass = (CswNbtObjClassParameter) ParamNode;
                    CswNbtMetaDataNodeType ResultNodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( ParameterObjClass.ResultType.SelectedNodeTypeIds ) );

                    if( ResultNodeType != null )
                    {
                        // Make a new result as a child of current Aliquot
                        CswNbtNode NewResultNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ResultNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                        NewResultNode.copyPropertyValues( ParamNode );
                        CswNbtObjClassResult NewResultObjClass = (CswNbtObjClassResult) NewResultNode;
                        NewResultObjClass.Aliquot.RelatedNodeId = AliquotNode.NodeId;
                        NewResultObjClass.Aliquot.CachedNodeName = AliquotNode.NodeName;
                        NewResultObjClass.Parameter.RelatedNodeId = ParamNode.NodeId;
                        NewResultObjClass.Parameter.CachedNodeName = ParamNode.NodeName;

                        if( null != onBeforeInsertNode )
                        {
                            onBeforeInsertNode( NewResultNode );
                        }
                        NewResultNode.postChanges( true );

                        ResultNodeIds.Add( NewResultNode.NodeId );
                    }
                }
            }
            return ResultNodeIds;
        } // AssignTest()

    } // class CswNbtActAssignTests
}// namespace ChemSW.Nbt.Actions