using System;
using ChemSW.Core;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Sched
{
    [TestFixture]
    public class CswScheduleLogicNbtContainerReconciliationActionsTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        #region CswScheduleLogicNbtContainerReconciliationActions Tests

        /// <summary>
        /// Given that no ContainerLocation nodes with a not-null action exist in the data,
        /// assert that the returned view contains 0 nodes.
        /// </summary>
        [Test]
        public void getOutstandingContainerLocationsTestNoNodes()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.AreEqual( 0, ContainerLocationsTree.getChildNodeCount(), "Unexpected results found in view." );
        }

        /// <summary>
        /// Given that at least one ContainerLocation node with a not-null action exists in the data,
        /// assert that the returned view contains at least 1 node.
        /// </summary>
        [Test]
        public void getOutstandingContainerLocationsTestHasNodes()
        {
            TestData.Nodes.createContainerLocationNode( Action: CswEnumNbtContainerLocationActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.IsTrue( ContainerLocationsTree.getChildNodeCount() > 0, "Expected results; view is empty." );
        }

        /// <summary>
        /// Given one ContainerLocation node with an action of No Action in the data,
        /// assert that the returned view contains 0 nodes.
        /// </summary>
        [Test]
        public void getOutstandingContainerLocationsTestNoActionFilteredOut()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            TestData.Nodes.createContainerLocationNode( Action: CswEnumNbtContainerLocationActionOptions.Ignore.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.AreEqual( 0, ContainerLocationsTree.getChildNodeCount(), "Unexpected results found in view." );
        }

        /// <summary>
        /// Given a view that contains no ContainerLocation nodes,
        /// assert that the returned string is empty
        /// </summary>
        [Test]
        public void getContainerLocationIdsTestNoNodes()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            CswCommaDelimitedString ContainerLocationIds = Sched.getContainerLocationIds( TestData.CswNbtResources, ContainerLocationsView );
            Assert.AreEqual( 0, ContainerLocationIds.Count, "Unexpected results found in CommaDelimitedString." );
        }

        /// <summary>
        /// Given a view that contains at least one ContainerLocation node,
        /// assert that the returned CommaDelimitedString contains all of the prmary keys of the nodes in the view
        /// </summary>
        [Test]
        public void getContainerLocationIdsTestHasNodes()
        {
            TestData.Nodes.createContainerLocationNode( Action: CswEnumNbtContainerLocationActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            CswCommaDelimitedString ContainerLocationIds = Sched.getContainerLocationIds( TestData.CswNbtResources, ContainerLocationsView );
            Assert.IsTrue( ContainerLocationIds.Count > 0, "Expected results; CommaDelimitedString is empty." );
        }

        private CswScheduleLogicNbtContainerReconciliationActions _getReconciliationActionSched()
        {
            CswScheduleLogicNbtContainerReconciliationActions Sched = new CswScheduleLogicNbtContainerReconciliationActions();
            CswScheduleLogicDetail CswScheduleLogicDetail = new CswScheduleLogicDetail();
            Sched.initScheduleLogicDetail(CswScheduleLogicDetail);
            Sched.getLoadCount( TestData.CswNbtResources );
            return Sched;
        }

        #endregion CswScheduleLogicNbtContainerReconciliationActions Tests

        #region Old BatchOp Tests

        /// <summary>
        /// Given an out of date ContainerLocation (i.e. - a newer ContainerLocation exists for the related Container),
        /// assert that the ContainerLocation is marked ActionApplied and the related Container is unchanged.
        /// </summary>
        [Test]
        public void processReconciliationActionsTestMoreRecentExists()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation FirstContainerLocationNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswEnumNbtContainerLocationActionOptions.Undispose.ToString(), DateTime.Now.AddSeconds( -1 ) );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswEnumNbtContainerLocationActionOptions.Ignore.ToString() );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, FirstContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Missing.Checked.ToString() );
        }

        /// <summary>
        /// Given a ContainerLocation with Action "Undispose"
        /// (given a disposed Container and a related ContainerLocation),
        /// assert that the Contianer has Disposed and Missing set to false, 
        /// and that the ContainerLocation is marked ActionApplied.
        /// </summary>
        [Test]
        public void processReconciliationActionsTestUndispose()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            ContainerNode.DisposeContainer();
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswEnumNbtContainerLocationActionOptions.Undispose.ToString() );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, ContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Disposed.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Missing.Checked.ToString() );
        }

        /// <summary>
        /// Given a ContainerLocation with status "WrongLocation" and Action "MoveToLocation"
        /// (given an undisposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Missing set to false, 
        /// assert that the ContainerLocation is marked ActionApplied,
        /// </summary>
        [Test]
        public void processReconciliationActionsTestWrongLocationMoveToLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(), ContainerLocationNode.Status.Value );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, ContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Missing.Checked.ToString() );
        }

        /// <summary>
        /// Given a ContainerLocation with status "DisposedAtWrongLocation" and Action "MoveToLocation"
        /// (given a disposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Missing set to false,
        /// assert that the ContainerLocation is marked ActionApplied,
        /// </summary>
        [Test]
        public void processReconciliationActionsTestDisposedAtWrongLocationMoveToLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString(), ContainerLocationNode.Status.Value );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, ContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Missing.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
        }

        /// <summary>
        /// Given a ContainerLocation with Action "UndisposeAndMove"
        /// (given a disposed Container and a related ContainerLocation with different locations),
        /// assert that the Contianer's location matches the ContainerLocation's Location,
        /// assert that the Container has Disposed and Missing set to false,
        /// assert that the ContainerLocation is marked ActionApplied,
        /// </summary>
        [Test]
        public void processReconciliationActionsTestUndisposeAndMove()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.UndisposeAndMove.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString(), ContainerLocationNode.Status.Value );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, ContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Missing.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.False, ContainerNode.Disposed.Checked.ToString() );
        }

        /// <summary>
        /// Given a ContainerLocation with Action "MarkMissing"
        /// (given a Container with no ContainerLocation in the given timeframe),
        /// assert that the Contianer has Missing set to true, 
        /// and that the ContainerLocation is marked ActionApplied.
        /// </summary>
        [Test]
        public void processReconciliationActionsTestMarkMissing()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString(),
                Type: CswEnumNbtContainerLocationTypeOptions.Missing.ToString() );

            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.processReconciliationActions( TestData.CswNbtResources );
            Assert.AreEqual( CswEnumTristate.True, ContainerLocationNode.ActionApplied.Checked.ToString() );
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Missing.Checked.ToString() );
        }

        #endregion Old BatchOp Tests
    }
}
