using System;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSw.Nbt.Test
{
    [TestClass]
    public class CswNbtBatchOpContainerReconciliationActionsTest
    {
        #region Setup and Teardown

        private TestData TestData;

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

        #region CswNbtBatchOpContainerReconciliationActions Tests

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with an out of date ContainerLocation
        /// (i.e. - a newer ContainerLocation exists for the related Container),
        /// assert that the ContainerLocation is marked ActionApplied and the related Container is unchanged.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestMoreRecentExists()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation FirstContainerLocationNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString(), DateTime.Now.AddSeconds( -1 ) );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            _runReconciliationBatchOp( FirstContainerLocationNode.NodeId );
            Assert.AreEqual( Tristate.True, FirstContainerLocationNode.ActionApplied.Checked );
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
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );

            _runReconciliationBatchOp( ContainerLocationNode.NodeId );
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
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.WrongLocation, ContainerLocationNode.Status.Value );

            _runReconciliationBatchOp( ContainerLocationNode.NodeId );
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
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );

            _runReconciliationBatchOp( ContainerLocationNode.NodeId );
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
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );

            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.UndisposeAndMove.ToString(),
                LocationId: ContainerLocationLocId );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );

            _runReconciliationBatchOp( ContainerLocationNode.NodeId );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( Tristate.False, ContainerNode.Missing.Checked );
            Assert.AreEqual( Tristate.False, ContainerNode.Disposed.Checked );
        }

        /// <summary>
        /// Given a ContainerReconciliationActions BatchOp with a ContainerLocation with Action "MarkMissing"
        /// (given a Container with no ContainerLocation in the given timeframe),
        /// assert that the Contianer has Missing set to true, 
        /// and that the ContainerLocation is marked ActionApplied.
        /// </summary>
        [TestMethod]
        public void runBatchOpTestMarkMissing()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode( 
                ContainerNode.Node, 
                CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString(), 
                Type: CswNbtObjClassContainerLocation.TypeOptions.Missing.ToString() );
            _runReconciliationBatchOp( ContainerLocationNode.NodeId );
            Assert.AreEqual( Tristate.True, ContainerLocationNode.ActionApplied.Checked );
            Assert.AreEqual( Tristate.True, ContainerNode.Missing.Checked );
        }

        private void _runReconciliationBatchOp( CswPrimaryKey ContainerLocationNodeId )
        {
            CswCommaDelimitedString ContainerLocationIds = new CswCommaDelimitedString();
            ContainerLocationIds.Add( ContainerLocationNodeId.ToString() );
            CswNbtBatchOpContainerReconciliationActions BatchOp = new CswNbtBatchOpContainerReconciliationActions( TestData.CswNbtResources );
            CswNbtObjClassBatchOp BatchOpNode = BatchOp.makeBatchOp( ContainerLocationIds, 10 );
            BatchOp.runBatchOp( BatchOpNode );
        }

        #endregion

    }
}
