using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.ObjClasses
{
    [TestFixture]
    public class CswNbtObjClassContainerLocationTest
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

        #region ContainerLocation Status

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation with the same location,
        /// assert that the ContainerLocation's Status has been set to Correct
        /// </summary>
        [Test]
        public void setStatusTestCorrect()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                LocationId: ContainerLocId );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation of type Scan with the same location,
        /// assert that the ContainerLocation's Status has been set to ScannedCorrect
        /// </summary>
        [Test]
        public void setStatusTestScannedCorrect()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                LocationId: ContainerLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.ScannedCorrect.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation with Missing = true,
        /// assert that the ContainerLocation's Status has been set to Missing
        /// </summary>
        [Test]
        public void setStatusTestMissing()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            ContainerNode.Missing.Checked = CswEnumTristate.True;
            ContainerNode.postChanges( false );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(), 
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Missing.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation with no related Container,
        /// assert that the ContainerLocation's Status has been set to NotScanned
        /// </summary>
        [Test]
        public void setStatusTestNotScanned()
        {
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode();
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                Type: CswEnumNbtContainerLocationTypeOptions.Missing.ToString() );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation of type Scan with the same location,
        /// assert that the ContainerLocation's Status has been set to Disposed
        /// </summary>
        [Test]
        public void setStatusTestDisposed()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                LocationId: ContainerLocId, 
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Disposed.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given an undisposed Container and a related ContainerLocation of type Scan with different locations,
        /// assert that the ContainerLocation's Status has been set to WrongLocation
        /// </summary>
        [Test]
        public void setStatusTestWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(), ContainerLocationNode.Status.Value );
        }

        /// <summary>
        /// Given a disposed Container and a related ContainerLocation of type Scan with different locations,
        /// assert that the ContainerLocation's Status has been set to DisposedAtWrongLocation
        /// </summary>
        [Test]
        public void setStatusTestDisposedAtWrongLocation()
        {
            CswPrimaryKey ContainerLocId, ContainerLocationLocId;
            TestData.getTwoDifferentLocationIds( out ContainerLocId, out ContainerLocationLocId );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: ContainerLocId );
            ContainerNode.DisposeContainer();
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            CswNbtObjClassContainerLocation ContainerLocationNode = TestData.Nodes.createContainerLocationNode(
                ContainerNode.Node,
                CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                LocationId: ContainerLocationLocId,
                Type: CswEnumNbtContainerLocationTypeOptions.Scan.ToString() );
            Assert.AreNotEqual( ContainerLocationNode.Location.SelectedNodeId, ContainerNode.Location.SelectedNodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString(), ContainerLocationNode.Status.Value );
        }

        #endregion

    }
}
