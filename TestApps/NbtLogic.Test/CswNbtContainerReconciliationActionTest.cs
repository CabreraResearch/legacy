using System;
using System.Threading;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSw.Nbt.Test
{
    [TestClass]
    public class CswNbtContainerReconciliationActionTest
    {
        #region Setup and Teardown

        private TestData TestData = null;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
            TestData.RevertNodeProps();
        }

        #endregion

        #region CswScheduleLogicNbtContainerReconciliationActions Tests

        /// <summary>
        /// Given that no ContainerLocation nodes with a not-null action exist in the data,
        /// assert that no BatchOp nodes are created.
        /// </summary>
        [TestMethod]
        public void makeReconciliationActionBatchProcessTestNoNodes()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.makeReconciliationActionBatchProcess();
            CswNbtMetaDataObjectClass BatchOpOc = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
            foreach( CswNbtObjClassBatchOp BatchOpNode in BatchOpOc.getNodes( false, false ) )
            {
                if( BatchOpNode.OpName.Value == NbtBatchOpName.ContainerReconciliationActions.ToString() && TestData.isTestNode( BatchOpNode.NodeId ) )
                {
                    Assert.Fail( "Unexpected BatchOp created." );
                }
            }
        }

        /// <summary>
        /// Given that at least one ContainerLocation node with a not-null action exists in the data,
        /// assert that a BatchOp node of type ContainerReconciliationActions is created.
        /// </summary>
        [TestMethod]
        public void makeReconciliationActionBatchProcessTestHasNodes()
        {
            TestData.setAllContainerLocationNodeActions( CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            TestData.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.makeReconciliationActionBatchProcess();
            CswNbtMetaDataObjectClass BatchOpOc = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.BatchOpClass );
            bool BatchOpFound = false;
            foreach( CswNbtObjClassBatchOp BatchOpNode in BatchOpOc.getNodes( false, false ) )
            {
                if( BatchOpNode.OpName.Value == NbtBatchOpName.ContainerReconciliationActions.ToString() && TestData.isTestNode( BatchOpNode.NodeId ) )
                {
                    BatchOpFound = true;
                }
            }
            Assert.IsTrue( BatchOpFound, "BatchOp not found!" );
        }

        /// <summary>
        /// Given that no ContainerLocation nodes with a not-null action exist in the data,
        /// assert that the returned view contains 0 nodes.
        /// </summary>
        [TestMethod]
        public void getOutstandingContainerLocationsTestNoNodes()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations();
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.AreEqual( 0, ContainerLocationsTree.getChildNodeCount(), "Unexpected results found in view." );
        }

        /// <summary>
        /// Given that at least one ContainerLocation node with a not-null action exists in the data,
        /// assert that the returned view contains at least 1 node.
        /// </summary>
        [TestMethod]
        public void getOutstandingContainerLocationsTestHasNodes()
        {
            TestData.setAllContainerLocationNodeActions( CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            TestData.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations();
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.IsTrue( ContainerLocationsTree.getChildNodeCount() > 0, "Expected results; view is empty." );
        }

        /// <summary>
        /// Given a view that contains no ContainerLocation nodes,
        /// assert that the returned string is empty
        /// </summary>
        [TestMethod]
        public void getContainerLocationIdsTestNoNodes()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations();
            CswCommaDelimitedString ContainerLocationIds = Sched.getContainerLocationIds( ContainerLocationsView );
            Assert.AreEqual( 0, ContainerLocationIds.Count, "Unexpected results found in CommaDelimitedString." );
        }

        /// <summary>
        /// Given a view that contains at least one ContainerLocation node,
        /// assert that the returned CommaDelimitedString contains all of the prmary keys of the nodes in the view
        /// </summary>
        [TestMethod]
        public void getContainerLocationIdsTestHasNodes()
        {
            TestData.setAllContainerLocationNodeActions( CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            TestData.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations();
            CswCommaDelimitedString ContainerLocationIds = Sched.getContainerLocationIds( ContainerLocationsView );
            Assert.IsTrue( ContainerLocationIds.Count > 0, "Expected results; CommaDelimitedString is empty." );
        }

        private CswScheduleLogicNbtContainerReconciliationActions _getReconciliationActionSched()
        {
            CswScheduleLogicNbtContainerReconciliationActions Sched = new CswScheduleLogicNbtContainerReconciliationActions();
            CswScheduleLogicDetail CswScheduleLogicDetail = new CswScheduleLogicDetail();
            Sched.init( TestData.CswNbtResources, CswScheduleLogicDetail );
            return Sched;
        }

        #endregion

        #region CswNbtBatchOpContainerReconciliationActions Tests

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with an out of date ContainerLocation
        /// (i.e. - a newer ContainerLocation exists for the related Container),
        /// assert that the ContainerLocation is marked ActionApplied and the related Container is unchanged.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestMoreRecentExists()
        {
            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode();
            CswNbtObjClassContainerLocation FirstContainerLocationNode = TestData.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString(), DateTime.Today.AddDays( -1 ) );
            ContainerLocationIds.Add( FirstContainerLocationNode.NodeId.ToString() );
            TestData.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreEqual( Tristate.True, FirstContainerLocationNode.ActionApplied.Checked );
            Assert.AreNotEqual( Tristate.False, ContainerNode.Missing.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with Action "No Action"
        /// (given a Container and a related ContainerLocation),
        /// assert that the ContainerLocation and related Container remain unchanged.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestNoAction()
        {
            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            ContainerLocationIds.Add( ContainerLocationNode.NodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreNotEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreNotEqual( Tristate.False, ContainerNode.Missing.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with Action "Undispose"
        /// (given a disposed Container and a related ContainerLocation),
        /// assert that the Contianer has Disposed and Missing set to false, 
        /// and that the ContainerLocation is marked ActionApplied.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestUndispose()
        {
            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode();
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            ContainerLocationIds.Add( ContainerLocationNode.NodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( Tristate.False, ContainerNode.Disposed.Checked );
            Assert.AreEqual( Tristate.False, ContainerNode.Missing.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with status "WrongLocation" and Action "MoveToLocation"
        /// (given an undisposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Missing set to false, 
        /// assert that the ContainerLocation is marked ActionApplied,
        /// assert that a new ContainerLocation of Type Move is created with a status of Correct.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestWrongLocationMoveToLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.WrongLocation, ContainerLocationNode.Status.Value );

            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            ContainerLocationIds.Add( ContainerLocationNode.NodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( Tristate.False, ContainerNode.Missing.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with status "DisposedAtWrongLocation" and Action "MoveToLocation"
        /// (given a disposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Missing set to false,
        /// assert that the ContainerLocation is marked ActionApplied,
        /// assert that a new ContainerLocation of Type Move is created with a status of Disposed.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestDisposedAtWrongLocationMoveToLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );

            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            ContainerLocationIds.Add( ContainerLocationNode.NodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( Tristate.False, ContainerNode.Missing.Checked );
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with Action "UndisposeAndMove"
        /// (given a disposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Disposed and Missing set to false,
        /// assert that the ContainerLocation is marked ActionApplied,
        /// assert that a new ContainerLocation of Type Move is created with a status of Correct.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestUndisposeAndMove()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.UndisposeAndMove.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );

            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            ContainerLocationIds.Add( ContainerLocationNode.NodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( Tristate.False, ContainerNode.Missing.Checked );
            Assert.AreEqual( Tristate.False, ContainerNode.Disposed.Checked );
        }

        private void _getTwoDifferentLocationIds( out CswPrimaryKey LocationId1, out CswPrimaryKey LocationId2 )
        {
            LocationId1 = null;
            LocationId2 = null;
            CswNbtMetaDataObjectClass LocationOc = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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

        #region ContainerLocation Status

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation with the same location,
        /// assert that the ContainerLocation's Status has been set to Correct
        /// </summary>
        [TestMethod]
        public void setStatusTestCorrect()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocId );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation with no related Container,
        /// assert that the ContainerLocation's Status has been set to Missing
        /// </summary>
        [TestMethod]
        public void setStatusTestMissing()
        {
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            ContainerLocationNode.Container.RelatedNodeId = null;
            ContainerLocationNode.postChanges( false );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Missing, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation with the same location,
        /// assert that the ContainerLocation's Status has been set to Disposed
        /// </summary>
        [TestMethod]
        public void setStatusTestDisposed()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocId );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Disposed, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation with different locations,
        /// assert that the ContainerLocation's Status has been set to WrongLocation
        /// </summary>
        [TestMethod]
        public void setStatusTestWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.WrongLocation, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation with different locations,
        /// assert that the ContainerLocation's Status has been set to DisposedAtWrongLocation
        /// </summary>
        [TestMethod]
        public void setStatusTestDisposedAtWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            _getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );
        }

        #endregion

    }
}
