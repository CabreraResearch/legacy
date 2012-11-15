using System;
using System.Collections.ObjectModel;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSw.Nbt.Test
{
    [TestClass]
    public class CswNbtActionReconciliationTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private CswNbtActReconciliation ReconciliationAction;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            ReconciliationAction = new CswNbtActReconciliation( TestData.CswNbtResources );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
            TestData.RevertNodeProps();
        }

        #endregion

        #region getContainerStatuses

        /// <summary>
        /// Given a location that has no Containers,
        /// assert that the returned ContainerStatus data is empty
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestNoResults()
        {
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = TestData.Nodes.createLocationNode().NodeId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 0, Data.ContainerStatuses.Count );
        }

        /// <summary>
        /// Given a location that has one Container and no ContainerLocations in the given timeframe,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of NotScanned
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestNotScanned()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 2 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one Container that did not exist in the given timeframe,
        /// assert that the no ContainerStatus data is returned
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestContainerDidNotExist()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -2 ).ToString(),
                EndDate = DateTime.Now.AddHours( -1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 0, Data.ContainerStatuses.Count );
        }

        /// <summary>
        /// Given a location that has one Container and a ContainerLocation in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of Correct
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestCorrect()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data contains more than one row.
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestMultipleContainers()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 2, Data.ContainerStatuses.Count );
            foreach( ContainerData.ReconciliationStatuses Stat in Data.ContainerStatuses )
            {
                Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
        }

        /// <summary>
        /// Given a location that has more than one child locations with a Container that has a Correct ContainerLocation in the given timeframe,
        /// assert that the returned ContainerStatus data includes these Containers.
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestIncludeChildLocations()
        {
            CswNbtObjClassLocation Location1 = TestData.Nodes.createLocationNode();
            CswNbtObjClassLocation Location2 = TestData.Nodes.createLocationNode( ParentLocationId: Location1.NodeId );
            TestData.Nodes.createContainerNode( LocationId: Location1.NodeId );
            TestData.Nodes.createContainerNode( LocationId: Location2.NodeId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = Location1.NodeId.ToString(),
                IncludeChildLocations = true
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 2, Data.ContainerStatuses.Count );
            foreach( ContainerData.ReconciliationStatuses Stat in Data.ContainerStatuses )
            {
                Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
        }

        #endregion

        #region getContainerStatistics

        /// <summary>
        /// Given a location that has no Containers,
        /// assert that each row of the returned ContainerStatistics data has their ContainerAmount value set to 0
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestNoResults()
        {
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = TestData.Nodes.createLocationNode().NodeId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                Assert.AreEqual( 0, Stat.ContainerCount );
            }
        }

        /// <summary>
        /// Given a location that has one Container and no ContainerLocations in the given timeframe,
        /// assert that the returned ContainerStatistics data's NotScanned ContainerStatus row's ContainerCount value > 1.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestNotScanned()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 2 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString() )
                {
                    Assert.AreEqual( 1, Stat.ContainerCount );
                }
                else
                {
                    Assert.AreEqual( 0, Stat.ContainerCount );
                }
            }
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's ContainerCount value > 1.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestMultipleContainers()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 2, Stat.ContainerCount, "There should have been two Correct containers." );
                }
                else
                {
                    Assert.AreEqual( 0, Stat.ContainerCount, "Status " + Stat.Status + " should have been empty." );
                }
            }
        }

        /// <summary>
        /// Given a location that has two Containers with ContainerLocations in the given timeframe with a Correct status,
        /// given one of these ContainerLocations has a ContainerScan value,
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's PercentScanned value = 50.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestPercentScanned()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, LocationId: LocationId, ContainerScan: ContainerNode.Barcode.Barcode );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 50, Stat.PercentScanned );
                }
            }
        }

        #endregion

        #region getReconciliationData

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that both expected ContainerStatus and ContainerStatistics data is returned.
        /// </summary>
        [TestMethod]
        public void getReconciliationDataTest()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getReconciliationData( Request );
            Assert.AreEqual( 2, Data.ContainerStatuses.Count );
            foreach( ContainerData.ReconciliationStatuses Stat in Data.ContainerStatuses )
            {
                Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 2, Stat.ContainerCount, "There should have been two Correct containers." );
                }
                else
                {
                    Assert.AreEqual( 0, Stat.ContainerCount, "Status " + Stat.Status + " should have been empty." );
                }
            }
        }

        #endregion

    }
}
