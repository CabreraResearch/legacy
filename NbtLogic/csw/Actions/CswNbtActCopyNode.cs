using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActCopyNode
    {
        CswNbtResources _CswNbtResources = null;

        public CswNbtActCopyNode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        public CswNbtNode CopyEquipmentNode( CswNbtNode OriginalEquipmentNode )
        {
            // Copy this Equipment
            CswNbtNode CopiedEquipmentNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalEquipmentNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedEquipmentNode.copyPropertyValues( OriginalEquipmentNode );
            CopiedEquipmentNode.postChanges( true, true );  // sets the PK

            // Copy all Generators
            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtView GeneratorView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship GeneratorRelationship = GeneratorView.AddViewRelationship( GeneratorObjectClass, false );
            CswNbtViewProperty OwnerProperty = GeneratorView.AddViewProperty( GeneratorRelationship, GeneratorObjectClass.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner ) );
            CswNbtViewPropertyFilter OwnerIsEquipmentFilter = GeneratorView.AddViewPropertyFilter( OwnerProperty, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, OriginalEquipmentNode.NodeId.PrimaryKey.ToString(), false );

            ICswNbtTree GeneratorTree = _CswNbtResources.Trees.getTreeFromView( GeneratorView, true, true, false, false );
            GeneratorTree.goToRoot();
            //if (GeneratorTree.getChildNodeCount() > 0)
            //{
            //    GeneratorTree.goToNthChild(0);
            Int32 c = 0;
            while( c < GeneratorTree.getChildNodeCount() )
            {
                GeneratorTree.goToNthChild( c );
                CswNbtNode OriginalGeneratorNode = GeneratorTree.getNodeForCurrentPosition();
                CswNbtNode CopiedGeneratorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalGeneratorNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                CopiedGeneratorNode.copyPropertyValues( OriginalGeneratorNode );
                ( (CswNbtObjClassGenerator) CopiedGeneratorNode ).Owner.RelatedNodeId = CopiedEquipmentNode.NodeId;
                CopiedGeneratorNode.postChanges( true, true );
                GeneratorTree.goToParentNode();
                c++;
            }
            //}

            return CopiedEquipmentNode;
        }

        public CswNbtNode CopyEquipmentAssemblyNode( CswNbtNode OriginalAssemblyNode )
        {
            // Copy this Assembly
            CswNbtNode CopiedAssemblyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalAssemblyNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedAssemblyNode.copyPropertyValues( OriginalAssemblyNode );
            CopiedAssemblyNode.postChanges( true, true );  // sets the PK

            // Copy all Equipment
            CswNbtMetaDataObjectClass EquipmentObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtView EquipmentView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship EquipmentRelationship = EquipmentView.AddViewRelationship( EquipmentObjectClass, false );
            CswNbtViewProperty AssemblyProperty = EquipmentView.AddViewProperty( EquipmentRelationship, EquipmentObjectClass.getObjectClassProp( CswNbtObjClassEquipment.AssemblyPropertyName ) );
            CswNbtViewPropertyFilter AssemblyIsOriginalFilter = EquipmentView.AddViewPropertyFilter( AssemblyProperty, CswNbtSubField.SubFieldName.NodeID, CswNbtPropFilterSql.PropertyFilterMode.Equals, OriginalAssemblyNode.NodeId.PrimaryKey.ToString(), false );

            ICswNbtTree EquipmentTree = _CswNbtResources.Trees.getTreeFromView( EquipmentView, true, true, false, false );
            EquipmentTree.goToRoot();
            //if (EquipmentTree.getChildNodeCount() > 0)
            //{
            //    EquipmentTree.goToNthChild(0);
            Int32 c = 0;
            while( c < EquipmentTree.getChildNodeCount() )
            {
                EquipmentTree.goToNthChild( c );
                CswNbtNode OriginalEquipmentNode = EquipmentTree.getNodeForCurrentPosition();
                CswNbtNode CopiedEquipmentNode = CopyEquipmentNode( OriginalEquipmentNode );
                CopiedEquipmentNode.copyPropertyValues( OriginalEquipmentNode );
                ( (CswNbtObjClassEquipment) CopiedEquipmentNode ).Assembly.RelatedNodeId = CopiedAssemblyNode.NodeId;
                CopiedEquipmentNode.postChanges( true, true );
                EquipmentTree.goToParentNode();
                c++;
            }
            //}
            return CopiedAssemblyNode;
        }

        public CswNbtNode CopyGeneratorNode( CswNbtNode OriginalIDNode )//Case 26281
        {
            CswNbtNode CopiedIDNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalIDNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedIDNode.copyPropertyValues( OriginalIDNode );

            CswNbtObjClassGenerator CopiedIDNodeAsID = (CswNbtObjClassGenerator) CopiedIDNode;
            CopiedIDNodeAsID.RunStatus.CommentsJson = new Newtonsoft.Json.Linq.JArray();

            CopiedIDNode.postChanges( true, true );
            return CopiedIDNode;
        } // CopyCopyGeneratorNodeNode()

        public CswNbtNode CopyInspectionTargetNode( CswNbtNode OriginalInspectionTargetNode )
        {
            // Copy this Inspection Target
            CswNbtNode CopiedInspectionTargetNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalInspectionTargetNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedInspectionTargetNode.copyPropertyValues( OriginalInspectionTargetNode );

            CswNbtObjClassInspectionTarget NodeAsInspectionTarget = (CswNbtObjClassInspectionTarget) CopiedInspectionTargetNode;
            NodeAsInspectionTarget.Status.Value = CswNbtObjClassInspectionDesign.TargetStatusAsString( CswNbtObjClassInspectionDesign.TargetStatus.Not_Inspected );

            CopiedInspectionTargetNode.postChanges( true, true );  // sets the PK

            return CopiedInspectionTargetNode;
        }

        public CswNbtNode CopyInspectionDesignNode( CswNbtNode OriginalIDNode )
        {
            CswNbtNode CopiedIDNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalIDNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedIDNode.copyPropertyValues( OriginalIDNode );

            // case 24454
            CswNbtObjClassInspectionDesign CopiedIDNodeAsID = (CswNbtObjClassInspectionDesign) CopiedIDNode;
            CopiedIDNodeAsID.Generator.RelatedNodeId = null;
            CopiedIDNodeAsID.Generator.RefreshNodeName();

            CopiedIDNode.postChanges( true, true );  // sets the PK
            return CopiedIDNode;
        } // CopyInspectionDesignNode()

        public CswNbtNode CopyNode( CswNbtNode OriginalNode )
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedNode.copyPropertyValues( OriginalNode );
            CopiedNode.postChanges( true, true );  // sets the PK
            return CopiedNode;
        }
    }
}
