using System;
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
    public class CswScheduleLogicNbtContainerReconciliationActionsTest
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
            Sched.makeReconciliationActionBatchProcess(TestData.CswNbtResources);
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
            TestData.Nodes.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            Sched.makeReconciliationActionBatchProcess( TestData.CswNbtResources );
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
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
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
            TestData.Nodes.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.IsTrue( ContainerLocationsTree.getChildNodeCount() > 0, "Expected results; view is empty." );
        }

        /// <summary>
        /// Given one ContainerLocation node with an action of No Action in the data,
        /// assert that the returned view contains 0 nodes.
        /// </summary>
        [TestMethod]
        public void getOutstandingContainerLocationsTestNoActionFilteredOut()
        {
            TestData.setAllContainerLocationNodeActions( String.Empty );
            TestData.Nodes.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            CswScheduleLogicNbtContainerReconciliationActions Sched = _getReconciliationActionSched();
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            ICswNbtTree ContainerLocationsTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainerLocationsView, false, false, false );
            Assert.AreEqual( 0, ContainerLocationsTree.getChildNodeCount(), "Unexpected results found in view." );
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
            CswNbtView ContainerLocationsView = Sched.getOutstandingContainerLocations( TestData.CswNbtResources );
            CswCommaDelimitedString ContainerLocationIds = Sched.getContainerLocationIds( TestData.CswNbtResources, ContainerLocationsView );
            Assert.AreEqual( 0, ContainerLocationIds.Count, "Unexpected results found in CommaDelimitedString." );
        }

        /// <summary>
        /// Given a view that contains at least one ContainerLocation node,
        /// assert that the returned CommaDelimitedString contains all of the prmary keys of the nodes in the view
        /// </summary>
        [TestMethod]
        public void getContainerLocationIdsTestHasNodes()
        {
            TestData.Nodes.createContainerLocationNode( Action: CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
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
            return Sched;
        }

        #endregion
    }
}
