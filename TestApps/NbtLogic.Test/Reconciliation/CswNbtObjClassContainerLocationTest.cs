using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSW.Nbt.Test
{
    [TestClass]
    public class CswNbtObjClassContainerLocationTest
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

        #region ContainerLocation Status

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation with the same location,
        /// assert that the ContainerLocation's Status has been set to Correct
        /// </summary>
        [TestMethod]
        public void setStatusTestCorrect()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocId );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation of type Scan with the same location,
        /// assert that the ContainerLocation's Status has been set to ScannedCorrect
        /// </summary>
        [TestMethod]
        public void setStatusTestScannedCorrect()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocId,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.ScannedCorrect, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation with Missing = true,
        /// assert that the ContainerLocation's Status has been set to Missing
        /// </summary>
        [TestMethod]
        public void setStatusTestMissing()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            ContainerNode.Missing.Checked = Tristate.True;
            ContainerNode.postChanges( false );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(), 
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Missing, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation with no related Container,
        /// assert that the ContainerLocation's Status has been set to NotScanned
        /// </summary>
        [TestMethod]
        public void setStatusTestNotScanned()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                Type: CswNbtObjClassContainerLocation.TypeOptions.Missing.ToString() );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.NotScanned, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation of type Scan with the same location,
        /// assert that the ContainerLocation's Status has been set to Disposed
        /// </summary>
        [TestMethod]
        public void setStatusTestDisposed()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocId, 
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Disposed, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation of type Scan with different locations,
        /// assert that the ContainerLocation's Status has been set to WrongLocation
        /// </summary>
        [TestMethod]
        public void setStatusTestWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.WrongLocation, ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation of type Scan with different locations,
        /// assert that the ContainerLocation's Status has been set to DisposedAtWrongLocation
        /// </summary>
        [TestMethod]
        public void setStatusTestDisposedAtWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( Tristate.True, ContainerNode.Disposed.Checked );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation, ContainerLocationNode.Status.Value );
        }

        #endregion

    }
}
