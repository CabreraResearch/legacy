using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int _UniqueSequence;
        public int Sequence
        {
            get
            {
                _UniqueSequence++;
                return _UniqueSequence;
            }
        }

        private Dictionary<CswPrimaryKey, String> _ContainerLocationNodeActions = new Dictionary<CswPrimaryKey, String>();

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
            CswNbtNode PlaceHolderNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Dispense Transaction" ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            _NodeIdHighWaterMark = PlaceHolderNode.NodeId;
        }

        internal bool isTestNode( CswPrimaryKey NodeId )
        {
            return NodeId.PrimaryKey > _NodeIdHighWaterMark.PrimaryKey;
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

        internal void RevertNodeProps()
        {
            foreach( KeyValuePair<CswPrimaryKey, String> ContainerLocationNodeId in _ContainerLocationNodeActions )
            {
                CswNbtObjClassContainerLocation ContainerLocationNode = CswNbtResources.Nodes[ContainerLocationNodeId.Key];
                ContainerLocationNode.Action.Value = ContainerLocationNodeId.Value;
                ContainerLocationNode.postChanges( false );
            }
        }

        #endregion Setup and Teardown

        #region Nodes

        internal CswNbtNode createLocationNode( String LocationType = "Room", String Name = "New Room", CswPrimaryKey ParentLocationId = null )
        {
            CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( LocationType ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            LocationNode.Name.Text = Name;
            if( ParentLocationId != null )
            {
                LocationNode.Location.SelectedNodeId = ParentLocationId;
                LocationNode.Location.RefreshNodeName();
            }
            LocationNode.postChanges( false );
            return LocationNode.Node;
        }

        internal CswNbtNode createContainerLocationNode( CswNbtNode ContainerNode = null, String Action = "", DateTime? NullableScanDate = null, CswPrimaryKey LocationId = null, String ContainerScan = "" )
        {
            CswNbtObjClassContainerLocation ContainerLocationNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Container Location" ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            if( null == ContainerNode )
            {
                ContainerNode = createContainerNode();
            }
            ContainerLocationNode.Container.RelatedNodeId = ContainerNode.NodeId;
            ContainerLocationNode.Action.Value = Action;
            DateTime ScanDate = NullableScanDate ?? DateTime.Now;
            ContainerLocationNode.ScanDate.DateTimeValue = ScanDate;
            if( LocationId != null )
            {
                ContainerLocationNode.Location.SelectedNodeId = LocationId;
                ContainerLocationNode.Location.RefreshNodeName();
            }
            ContainerLocationNode.ContainerScan.Text = ContainerScan;
            ContainerLocationNode.postChanges( false );
            return ContainerLocationNode.Node;
        }

        internal CswNbtNode createContainerNode( string NodeTypeName = "Container", double Quantity = 1.0, CswNbtNode UnitOfMeasure = null, CswNbtNode Material = null, CswPrimaryKey LocationId = null )
        {
            CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            ContainerNode.Quantity.Quantity = Quantity;
            if( null == UnitOfMeasure )
            {
                UnitOfMeasure = createUnitOfMeasureNode( "Volume", "Liters" + Sequence, 1.0, 0, Tristate.True );
            }
            ContainerNode.Quantity.UnitId = UnitOfMeasure.NodeId;
            if( Material != null )
            {
                ContainerNode.Material.RelatedNodeId = Material.NodeId;
            }
            if( LocationId != null )
            {
                ContainerNode.Location.SelectedNodeId = LocationId;
                ContainerNode.Location.RefreshNodeName();
            }
            ContainerNode.postChanges( true );

            return ContainerNode.Node;
        }

        internal CswNbtNode createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( "Unit (" + NodeTypeName + ")" ), CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            UnitOfMeasureNode.Name.Text = Name + "Test";
            if( CswTools.IsDouble( ConversionFactorBase ) )
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

        #region Node Props

        internal void setAllContainerLocationNodeActions( String Action )
        {
            CswNbtMetaDataObjectClass ContainerLocationOc = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            foreach( CswNbtObjClassContainerLocation ContainerLocationNode in ContainerLocationOc.getNodes( false, false ) )
            {
                _ContainerLocationNodeActions.Add( ContainerLocationNode.NodeId, ContainerLocationNode.Action.Value );
                ContainerLocationNode.Action.Value = Action;
                ContainerLocationNode.postChanges( false );
            }
        }

        internal void getTwoDifferentLocationIds( out CswPrimaryKey LocationId1, out CswPrimaryKey LocationId2 )
        {
            LocationId1 = null;
            LocationId2 = null;
            CswNbtMetaDataObjectClass LocationOc = CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtObjClassLocation LocationNode in LocationOc.getNodes( false, false ) )
            {
                if( LocationId1 != null )
                {
                    LocationId2 = LocationNode.NodeId;
                    break;
                }
                if( LocationId1 == null )
                {
                    LocationId1 = LocationNode.NodeId;
                }
            }
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
