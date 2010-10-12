using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;


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
            CswNbtViewProperty OwnerProperty = GeneratorView.AddViewProperty( GeneratorRelationship, GeneratorObjectClass.getObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName ) );
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
                CswNbtNodeCaster.AsGenerator( CopiedGeneratorNode ).Owner.RelatedNodeId = CopiedEquipmentNode.NodeId;
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
                CswNbtNodeCaster.AsEquipment( CopiedEquipmentNode ).Assembly.RelatedNodeId = CopiedAssemblyNode.NodeId;
                CopiedEquipmentNode.postChanges( true, true );
                EquipmentTree.goToParentNode();
                c++;
            }
            //}
            return CopiedAssemblyNode;
        }

        public CswNbtNode CopyNode( CswNbtNode OriginalNode )
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedNode.copyPropertyValues( OriginalNode );
            CopiedNode.postChanges( true, true );  // sets the PK
            return CopiedNode;
        }
    }
}
