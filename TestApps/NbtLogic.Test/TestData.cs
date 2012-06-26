using System;
using System.Collections.Generic;
using ChemSW;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace NbtLogic.Test
{
    internal class TestData
    {
        private CswNbtResources _CswNbtResources = null;
        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private string UserName = "TestUser";
        private List<CswPrimaryKey> TestNodeIds = new List<CswPrimaryKey>();

        public TestData()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswNbtResources.InitCurrentUser = InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, UserName );
        }

        public CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
        }

        #region Nodes

        internal CswNbtNode createContainerNode( string NodeTypeName, double Quantity, CswNbtNode UnitOfMeasure, CswNbtNode Material = null )
        {
            CswNbtNode ContainerNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassContainer NodeAsContianer = ContainerNode;
            NodeAsContianer.Quantity.Quantity = Quantity;
            NodeAsContianer.Quantity.UnitId = UnitOfMeasure.NodeId;
            if( Material != null )
            {
                NodeAsContianer.Material.RelatedNodeId = Material.NodeId;
            }
            NodeAsContianer.postChanges( true );

            TestNodeIds.Add( NodeAsContianer.NodeId );

            return ContainerNode;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName + " Unit" ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = UnitOfMeasureNode;
            NodeAsUnitOfMeasure.Name.Text = Name + "Test";
            if( ConversionFactorBase != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Base = ConversionFactorBase;
            if( ConversionFactorExponent != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Exponent = ConversionFactorExponent;
            NodeAsUnitOfMeasure.Fractional.Checked = Fractional;
            NodeAsUnitOfMeasure.UnitType.Value = NodeTypeName;
            NodeAsUnitOfMeasure.postChanges( true );

            TestNodeIds.Add( NodeAsUnitOfMeasure.NodeId );

            return UnitOfMeasureNode;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName, string State, double SpecificGravityBase, int SpecificGravityExponent )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassMaterial NodeAsMaterial = MaterialNode;
            if( SpecificGravityBase != Int32.MinValue )
                NodeAsMaterial.SpecificGravity.Base = SpecificGravityBase;
            if( SpecificGravityExponent != Int32.MinValue )
                NodeAsMaterial.SpecificGravity.Exponent = SpecificGravityExponent;
            NodeAsMaterial.PhysicalState.Value = State;
            NodeAsMaterial.postChanges( true );

            TestNodeIds.Add( NodeAsMaterial.NodeId );

            return MaterialNode;
        }

        #endregion

        #region Private Helper Functions

        private int _getNodeTypeId( string NodeTypeName )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            return NodeType.NodeTypeId;
        }

        private void _addContDispTransNodesToTestNodeList()
        {
            //TODO - is this easier than setting a high watermark?
            //iterate TestNodeIds - for each Container node, get all related transaction nodes and add them to delete list
        }

        #endregion

        #region Cleanup

        internal void DeleteTestNodes()
        {
            _addContDispTransNodesToTestNodeList();
            TestNodeIds.Reverse();//Act like a stack
            foreach( CswPrimaryKey NodeId in TestNodeIds )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId );
                Node.delete();
            }
        }

        #endregion

    }
}
