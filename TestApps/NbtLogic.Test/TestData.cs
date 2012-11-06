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
        private CswPrimaryKey _NodeIdHighWaterMark = null;
        //TODO - refactor the way we're handling NTP states
        private Dictionary<int, string> _ChangedNodeTypePropListOptions = new Dictionary<int, string>();
        private Dictionary<int, string> _ChangedNodeTypePropExtended = new Dictionary<int, string>();
        private Dictionary<int, int> _ChangedNodeTypePropMaxValue = new Dictionary<int, int>();

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
            return new CswNbtSystemUser( Resources, SystemUserNames.SysUsr_Test );
        }

        private void _setHighWaterMark()
        {
            CswNbtNode PlaceHolderNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Location" ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            _NodeIdHighWaterMark = PlaceHolderNode.NodeId;
        }

        private List<Int32> _getNodesAboveHighWaterMark()
        {
            List<Int32> TestNodeIds = new List<Int32>();
            TestNodeIds.Add( _NodeIdHighWaterMark.PrimaryKey );

            IEnumerator CurrentNodes = _CswNbtResources.Nodes.GetEnumerator();
            while( CurrentNodes.MoveNext() )
            {
                DictionaryEntry dentry = (DictionaryEntry) CurrentNodes.Current;
                CswNbtNode CurrentNode = (CswNbtNode) dentry.Value;
                if( CurrentNode.NodeId.PrimaryKey > _NodeIdHighWaterMark.PrimaryKey )
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

        internal void RevertNodeTypePropAttributes()
        {
            foreach( KeyValuePair<int, string> OriginalNodeTypePropId in _ChangedNodeTypePropListOptions )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                OriginalNodeTypeProp.ListOptions = OriginalNodeTypePropId.Value;
            }
            foreach( KeyValuePair<int, string> OriginalNodeTypePropId in _ChangedNodeTypePropExtended )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                OriginalNodeTypeProp.Extended = OriginalNodeTypePropId.Value;
            }
            foreach( KeyValuePair<int, int> OriginalNodeTypePropId in _ChangedNodeTypePropMaxValue )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                OriginalNodeTypeProp.MaxValue = OriginalNodeTypePropId.Value;
            }
        }

        #endregion Setup and Teardown

        #region Nodes

        internal CswNbtNode createContainerNode( string NodeTypeName, double Quantity, CswNbtNode UnitOfMeasure, CswNbtNode Material = null )
        {
            CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            ContainerNode.Quantity.Quantity = Quantity;
            ContainerNode.Quantity.UnitId = UnitOfMeasure.NodeId;
            if( Material != null )
            {
                ContainerNode.Material.RelatedNodeId = Material.NodeId;
            }
            ContainerNode.postChanges( true );

            return ContainerNode.Node;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Unit (" + NodeTypeName + ")" ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            UnitOfMeasureNode.Name.Text = Name + "Test";
            if( ConversionFactorBase != Int32.MinValue )
                UnitOfMeasureNode.ConversionFactor.Base = ConversionFactorBase;
            if( ConversionFactorExponent != Int32.MinValue )
                UnitOfMeasureNode.ConversionFactor.Exponent = ConversionFactorExponent;
            UnitOfMeasureNode.Fractional.Checked = Fractional;
            UnitOfMeasureNode.UnitType.Value = NodeTypeName;
            UnitOfMeasureNode.postChanges( true );

            return UnitOfMeasureNode.Node;
        }

        internal CswNbtNode createMaterialNode( string NodeTypeName, string State, double SpecificGravity )
        {
            CswNbtObjClassMaterial MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            if( CswTools.IsDouble( SpecificGravity ) )
                MaterialNode.SpecificGravity.Value = SpecificGravity;
            MaterialNode.PhysicalState.Value = State;
            MaterialNode.postChanges( true );

            return MaterialNode.Node;
        }

        internal CswNbtNode createChemicalNodeWithPPE( string PPE )
        {
            CswCommaDelimitedString PPEString = new CswCommaDelimitedString();
            PPEString.FromString( PPE );
            CswNbtNode ChemicalNode = createMaterialNode( "Chemical", "Liquid", 1 );
            CswNbtMetaDataNodeTypeProp PPENTP = _CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            ChemicalNode.Properties[PPENTP].AsMultiList.Value = PPEString;
            ChemicalNode.postChanges( true );

            return ChemicalNode;
        }

        #endregion

        #region Node Type Props

        internal void SetPPENodeTypeProp( string ListOptions, string Delimiter = ",", int HideThreshold = 5 )
        {
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
            if( ChemicalNT != null )
            {
                CswNbtMetaDataNodeTypeProp PPENTP = _CswNbtResources.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "PPE" );
                if( PPENTP != null )
                {
                    _ChangedNodeTypePropListOptions.Add( PPENTP.PropId, ListOptions );
                    PPENTP.ListOptions = ListOptions;
                    _ChangedNodeTypePropExtended.Add( PPENTP.PropId, Delimiter );
                    PPENTP.Extended = Delimiter;
                    _ChangedNodeTypePropMaxValue.Add( PPENTP.PropId, HideThreshold );
                    PPENTP.MaxValue = HideThreshold;
                }
            }
        }

        #endregion

        #region Private Helper Functions

        private int _getNodeTypeId( string NodeTypeName )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            if( NodeType == null )
            {
                throw new Exception( "Expected NodeType not found: " + NodeTypeName );
            }
            return NodeType.NodeTypeId;
        }

        #endregion

    }
}
