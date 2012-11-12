using System;
using System.Collections.ObjectModel;
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

        #region CswNbtActionReconciliation Tests

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
                LocationId = _getArbitraryLocationId().ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 0, Data.ContainerStatuses.Count );
        }

        /// <summary>
        /// Given a location that has one Container and no ContainerLocations in the given timeframe,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of Unknown
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestUnknown()
        {
            Assert.Inconclusive( "" );
        }

        /// <summary>
        /// Given a location that has one Container and a ContainerLocation in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of Correct
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestCorrect()
        {
            Assert.Inconclusive( "" );
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data contains more than one row.
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestMultipleContainers()
        {
            Assert.Inconclusive( "" );
        }

        /// <summary>
        /// Given a location that has more than one child locations with a Container that has a Correct ContainerLocation in the given timeframe,
        /// assert that the returned ContainerStatus data includes these Containers.
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestIncludeChildLocations()
        {
            Assert.Inconclusive( "" );
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
                LocationId = _getArbitraryLocationId().ToString(),
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
        /// assert that the returned ContainerStatistics data's Unknown ContainerStatus row's ContainerCount value > 1.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestUnknown()
        {
            Assert.Inconclusive( "" );
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's ContainerCount value > 1.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestMultipleContainers()
        {
            Assert.Inconclusive( "" );
        }

        /// <summary>
        /// Given a location that has two Containers with ContainerLocations in the given timeframe with a Correct status,
        /// given one of these ContainerLocations has a ContainerScan value,
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's PercentScanned value = 50.
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestPercentScanned()
        {
            Assert.Inconclusive( "" );
        }

        #endregion

        private CswPrimaryKey _getArbitraryLocationId()
        {
            CswNbtMetaDataObjectClass LocationOc = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            Collection<CswNbtNode> Locations = LocationOc.getNodes( false, false );
            return Locations[0].NodeId;
        }

        #endregion

    }
}
