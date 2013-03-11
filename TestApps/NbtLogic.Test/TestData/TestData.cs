using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;

namespace ChemSW.Nbt.Test
{
    public class TestData: IDisposable
    {
        private CswNbtResources _CswNbtResources;
        private ICswDbCfgInfo _CswDbCfgInfoNbt;
        private Int32 _NodeIdHighWaterMark;
        private Int32 _NodeTypeHighWaterMark;

        //TODO - refactor the way we're handling NTP states
        private Dictionary<int, string> _ChangedNodeTypePropListOptions = new Dictionary<int, string>();
        private Dictionary<int, string> _ChangedNodeTypePropExtended = new Dictionary<int, string>();
        private Dictionary<int, int> _ChangedNodeTypePropMaxValue = new Dictionary<int, int>();


        private Dictionary<CswPrimaryKey, String> _ContainerLocationNodeActions = new Dictionary<CswPrimaryKey, String>();

        internal TestDataNodes Nodes;

        internal TestData()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.TestProject, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile : false );
            _CswNbtResources.InitCurrentUser = _InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;
            Nodes = new TestDataNodes( _CswNbtResources );
            _setHighWaterMark();
        }

        internal CswNbtResources CswNbtResources
        {
            get { return _CswNbtResources; }
        }

        #region Setup and Teardown

        private ICswUser _InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswSystemUserNames.SysUsr_Test );
        }

        private void _setHighWaterMark()
        {
            DataTable MaxNodeTable = CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "getHWM", "select max(nodeid) as hwm from nodes" );
            _NodeIdHighWaterMark = CswConvert.ToInt32(MaxNodeTable.Rows[0]["hwm"]);
            _NodeTypeHighWaterMark = _CswNbtResources.MetaData.getNodeTypeIds().Max();
        }

        internal bool isTestNode( CswPrimaryKey NodeId )
        {
            return NodeId.PrimaryKey > _NodeIdHighWaterMark;
        }

        private List<Int32> _getNodesAboveHighWaterMark()
        {
            List<Int32> TestNodeIds = new List<Int32>();

            CswTableSelect  NodesSelect = _CswNbtResources.makeCswTableSelect( "NodesAboveHWM", "nodes" );
            DataTable TestNodesTable = NodesSelect.getTable( "where nodeid > " + _NodeIdHighWaterMark );
            TestNodeIds.AddRange( from DataRow Row in TestNodesTable.Rows select CswConvert.ToInt32( Row["nodeid"] ) );

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
                CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
                if( null != Node )
                {
                    Node.delete( DeleteAllRequiredRelatedNodes: true, OverridePermissions: true );
                }
            }
        }

        internal void RevertNodeTypePropAttributes()
        {
            foreach( KeyValuePair<int, string> OriginalNodeTypePropId in _ChangedNodeTypePropListOptions )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                if( null != OriginalNodeTypeProp )
                {
                    OriginalNodeTypeProp.ListOptions = OriginalNodeTypePropId.Value;
                }
            }
            foreach( KeyValuePair<int, string> OriginalNodeTypePropId in _ChangedNodeTypePropExtended )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                if( null != OriginalNodeTypeProp )
                {
                    OriginalNodeTypeProp.Extended = OriginalNodeTypePropId.Value;
                }
            }
            foreach( KeyValuePair<int, int> OriginalNodeTypePropId in _ChangedNodeTypePropMaxValue )
            {
                CswNbtMetaDataNodeTypeProp OriginalNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( OriginalNodeTypePropId.Key );
                if( null != OriginalNodeTypeProp )
                {
                    OriginalNodeTypeProp.MaxValue = OriginalNodeTypePropId.Value;
                }
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

        internal void DeleteTestNodeTypes()
        {
            if( null != _CswNbtResources )
            {
                foreach( CswNbtMetaDataNodeType NodeType in from NodeTypeId
                                                                in _CswNbtResources.MetaData.getNodeTypeIds()
                                                            where NodeTypeId > _NodeTypeHighWaterMark
                                                            select _CswNbtResources.MetaData.getNodeType( NodeTypeId )
                                                                into NodeType
                                                                where null != NodeType
                                                                select NodeType )
                {
                    _CswNbtResources.MetaData.DeleteNodeType( NodeType );
                }
            }
        }

        internal void Destroy()
        {
            if( null != _CswNbtResources )
            {
                DeleteTestNodeTypes();
                DeleteTestNodes();
                RevertNodeProps();
                RevertNodeTypePropAttributes();
                RevertNodeProps();
            }
        }

        #endregion Setup and Teardown

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

        #region IDisposable
        
        public void Dispose()
        {
            Destroy();
        }

        #endregion IDisposable
    }
}
