using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt;
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
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
            TestData.Nodes.createContainerLocationNode( LocationId: Location1.NodeId );
            TestData.Nodes.createContainerLocationNode( LocationId: Location2.NodeId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = Location1.NodeId.ToString(),
                IncludeChildLocations = true,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 2, Data.ContainerStatuses.Count );
            foreach( ContainerData.ReconciliationStatuses Stat in Data.ContainerStatuses )
            {
                Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
        }

        /// <summary>
        /// Given a location that has one Container with two ContainerLocations in the given timeframe,
        /// given that the first ContainerLocation is of Type Scan and the second is not,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of ScannedCorrect
        /// </summary>
        [TestMethod]
        public void getContainerStatusesTestScanTrumpsTouch()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Dispense.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            Thread.Sleep( 1000 );//Running into a race condition
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.ScannedCorrect.ToString(), Data.ContainerStatuses[0].ContainerStatus );
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
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's PercentScanned value = 0
        /// and that ScannedCorrect ContainerStatus row's PercentScanned value = 100
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestPercentScanned()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, 
                LocationId: LocationId, 
                ContainerScan: ContainerNode.Barcode.Barcode, 
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 0, Stat.PercentScanned );
                }
                else if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.ScannedCorrect.ToString() )
                {
                    Assert.AreEqual( 100, Stat.PercentScanned );
                }
            }
        }

        /// <summary>
        /// Given a location that has one Container with two ContainerLocations in the given timeframe,
        /// given that the first ContainerLocation is of Type Scan and the second is not,
        /// assert that both the returned data's NotScanned and Received/Dispensed/Disposed/etc 
        /// ContainerStatus row's ContainerCount value = 0
        /// </summary>
        [TestMethod]
        public void getContainerStatisticsTestScanTrumpsTouch()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, 
                LocationId: LocationId, 
                Type: CswNbtObjClassContainerLocation.TypeOptions.Dispense.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString() ||
                    Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 0, Stat.ContainerCount );
                }
                else if( Stat.Status == CswNbtObjClassContainerLocation.StatusOptions.ScannedCorrect.ToString())
                {
                    Assert.AreEqual( 1, Stat.ContainerCount );
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
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
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

        private Collection<ContainerData.ReconciliationTypes> _getTypes()
        {
            Collection<ContainerData.ReconciliationTypes> Types = new Collection<ContainerData.ReconciliationTypes>();
            for( int i = 0; i < CswNbtObjClassContainerLocation.TypeOptions._All.Count(); i++ )
            {
                Types.Add( new ContainerData.ReconciliationTypes { Enabled = true, Type = CswNbtObjClassContainerLocation.TypeOptions._All.ToArray()[i].ToString() } );
            }
            return Types;
        }

        #endregion

        #region saveContainerActions

        /// <summary>
        /// Given that no ContainerLocation actions have changed,
        /// assert that no exception is thrown
        /// </summary>
        [TestMethod]
        public void saveContainerActionsTestNoResults()
        {
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = TestData.Nodes.createLocationNode().NodeId.ToString(),
                IncludeChildLocations = false,
                ContainerActions = null
            };
            try
            {
                ReconciliationAction.saveContainerActions( Request );
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected exception thrown: " + ex.Message);
            }
        }

        /// <summary>
        /// Given a ContainerLocation whose action has been set to MarkMissing,
        /// assert that a new ContainerLocation node has been created with an action of MarkMissing
        /// </summary>
        [TestMethod]
        public void saveContainerActionsTestMarkMissing()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            Collection<ContainerData.ReconciliationActions> Actions = new Collection<ContainerData.ReconciliationActions>();
            ContainerData.ReconciliationActions Action = new ContainerData.ReconciliationActions
            {
                Action = CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString(),
                ContainerId = ContainerNode.NodeId.ToString(),
                LocationId = LocationId.ToString()
            };
            Actions.Add( Action );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerActions = Actions
            };
            Thread.Sleep( 1000 );//Running into two race conditions:
            //The Missing ContainerLocation is created before the Container onCreate's Move ContainerLocation
            //The new ContainerLocation doesn't exist when trying to retreive it
            ReconciliationAction.saveContainerActions( Request );
            CswNbtObjClassContainerLocation NewContLocNode = _getNewContianerLocation( ContainerNode.NodeId );
            Assert.AreEqual( CswNbtObjClassContainerLocation.TypeOptions.Missing.ToString(), NewContLocNode.Type.Value );
            Assert.AreEqual( CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString(), NewContLocNode.Action.Value );
            Assert.AreEqual( CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString(), NewContLocNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation whose action has been set to NoAction,
        /// assert that the selected ContainerLocation has its action set to NoAction
        /// </summary>
        [TestMethod]
        public void saveContainerActionsTestNoAction()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            CswNbtObjClassContainerLocation ContLocNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, LocationId: LocationId, ContainerScan: ContainerNode.Barcode.Barcode );
            Collection<ContainerData.ReconciliationActions> Actions = new Collection<ContainerData.ReconciliationActions>();
            ContainerData.ReconciliationActions Action = new ContainerData.ReconciliationActions
            {
                Action = CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(),
                ContainerId = ContainerNode.NodeId.ToString(),
                ContainerLocationId = ContLocNode.NodeId.ToString(),
                LocationId = LocationId.ToString()
            };
            Actions.Add( Action );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerActions = Actions
            };
            Assert.AreNotEqual( CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(), ContLocNode.Action.Value );
            ReconciliationAction.saveContainerActions( Request );
            Assert.AreEqual( CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString(), ContLocNode.Action.Value );
        }

        private CswNbtObjClassContainerLocation _getNewContianerLocation( CswPrimaryKey ContainerId )
        {
            CswNbtMetaDataObjectClass ContainerOC = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass ContainerLocationOC = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );
            CswNbtView ContainersView = new CswNbtView( TestData.CswNbtResources );

            CswNbtViewRelationship ContainerVR = ContainersView.AddViewRelationship( ContainerOC, false );
            ContainerVR.NodeIdsToFilterIn = new Collection<CswPrimaryKey>{ ContainerId };
            CswNbtViewRelationship ContainerLocationVR = ContainersView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.Second, ContainerOCP, false );
            CswNbtViewProperty ScanDateVP = ContainersView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            ContainersView.setSortProperty( ScanDateVP, NbtViewPropertySortMethod.Descending );
            ICswNbtTree ContainersTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainersView, false, true, false );
            ContainersTree.goToNthChild( 0 );
            ContainersTree.goToNthChild( 0 );
            CswNbtObjClassContainerLocation ContLocNode = TestData.CswNbtResources.Nodes.GetNode( ContainersTree.getNodeIdForCurrentPosition() );
            return ContLocNode;
        }

        #endregion
    }
}
