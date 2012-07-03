using System;
using System.Collections;
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

namespace ChemSw.Nbt.Test
{
    internal class TestData
    {
        private CswNbtResources _CswNbtResources = null;
        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private string UserName = "TestUser";
        private CswPrimaryKey _HighWaterMark = null;

        internal TestData()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswNbtResources.InitCurrentUser = _InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;
            _setHighWaterMark();
        }

        internal CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
        }

        #region Setup and Teardown

        private ICswUser _InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, UserName );
        }

        private void _setHighWaterMark()
        {
            CswNbtNode PlaceHolderNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Report" ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            _HighWaterMark = PlaceHolderNode.NodeId;
        }

        private List<Int32> _getNodesAboveHighWaterMark()
        {
            List<Int32> TestNodeIds = new List<Int32>();
            TestNodeIds.Add( _HighWaterMark.PrimaryKey );

            IEnumerator CurrentNodes = _CswNbtResources.Nodes.GetEnumerator();
            while( CurrentNodes.MoveNext() )
            {
                DictionaryEntry dentry = (DictionaryEntry) CurrentNodes.Current;
                CswNbtNode CurrentNode = (CswNbtNode) dentry.Value;
                if( CurrentNode.NodeId.PrimaryKey > _HighWaterMark.PrimaryKey )
                {
                    TestNodeIds.Add( CurrentNode.NodeId.PrimaryKey );
                }
            }

            return TestNodeIds;
        }

        internal void DeleteTestNodes()
        {
            List<Int32> TestNodePKs = _getNodesAboveHighWaterMark();
            TestNodePKs.Sort();
            TestNodePKs.Reverse();
            foreach( Int32 NodePK in TestNodePKs )
            {
                CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", NodePK );
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId );
                Node.delete();
            }
        }

        #endregion

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

            return UnitOfMeasureNode;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName, string State, double SpecificGravity )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassMaterial NodeAsMaterial = MaterialNode;
            if( CswTools.IsDouble( SpecificGravity ) )
                NodeAsMaterial.SpecificGravity.Value = SpecificGravity;
            NodeAsMaterial.PhysicalState.Value = State;
            NodeAsMaterial.postChanges( true );

            return MaterialNode;
        }

        #endregion

        #region Private Helper Functions

        private int _getNodeTypeId( string NodeTypeName )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            return NodeType.NodeTypeId;
        }

        #endregion

    }
}
