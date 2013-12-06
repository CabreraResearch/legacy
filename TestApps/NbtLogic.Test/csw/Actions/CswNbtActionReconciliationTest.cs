﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Actions
{
    [TestFixture]
    public class CswNbtActionReconciliationTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private CswNbtActReconciliation ReconciliationAction;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            ReconciliationAction = new CswNbtActReconciliation( TestData.CswNbtResources );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        #region getContainerStatuses

        /// <summary>
        /// Given a location that has no Containers,
        /// assert that the returned ContainerStatus data is empty
        /// </summary>
        [Test]
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
        [Test]
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
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one Container and one ContainerLocation in the given timeframe,
        /// given that the Type of the given ContainerLocation is not enabled,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of NotScanned
        /// </summary>
        [Test]
        public void getContainerStatusesTestTypeDisabled()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = new Collection<ContainerData.ReconciliationTypes>
                {
                    new ContainerData.ReconciliationTypes
                    {
                        Enabled=true, 
                        Type= CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString()
                    }
                }
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one disposed Container in the given timeframe, given that the container has not been scanned,
        /// when grabbing Containers for Reconcile Scans only,
        /// assert that no Container Status data is returned for the disposed Container.
        /// Prior to resolving Case 31304, this test failed.
        /// </summary>
        [Test]
        public void getContainerStatusesTestDisposedExcludedFromReconcileScans()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerNode.DisposeContainer( true );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = new Collection<ContainerData.ReconciliationTypes>
                {
                    new ContainerData.ReconciliationTypes
                    {
                        Enabled=true, 
                        Type= CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString()
                    }
                }
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 0, Data.ContainerStatuses.Count );
        }

        /// <summary>
        /// Given a location that has one Container that did not exist in the given timeframe,
        /// assert that the no ContainerStatus data is returned
        /// </summary>
        [Test]
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
        [Test]
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
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one Container and a ContainerLocation for that Container in a different location,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of WrongLocation
        /// </summary>
        [Test]
        public void getContainerStatusesTestWrongLocation()
        {
            CswNbtObjClassLocation Location1 = TestData.Nodes.createLocationNode();
            CswNbtObjClassLocation Location2 = TestData.Nodes.createLocationNode( Name: "NewRoom2" );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: Location1.NodeId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: Location2.NodeId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = Location1.NodeId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one Container and a ContainerLocation for that Container in a different location,
        /// given that the scanned location does not allow inventory
        /// assert that the returned ContainerStatus data does not have the action option "Move To Location"
        /// </summary>
        [Test]
        public void getContainerStatusesTestWrongLocationNoInventory()
        {
            CswNbtObjClassLocation Location1 = TestData.Nodes.createLocationNode();
            CswNbtObjClassLocation Location2 = TestData.Nodes.createLocationNode( Name: "NewRoom2", AllowInventory: false );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: Location1.NodeId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: Location2.NodeId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = Location1.NodeId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatuses( Request );
            Assert.AreEqual( 1, Data.ContainerStatuses.Count );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(), Data.ContainerStatuses[0].ContainerStatus );
            foreach( string ActionOption in Data.ContainerStatuses[0].ActionOptions )
            {
                Assert.AreNotEqual( CswEnumNbtContainerLocationActionOptions.MoveToLocation.ToString(), ActionOption );
            }
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data contains more than one row.
        /// </summary>
        [Test]
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
                Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
        }

        /// <summary>
        /// Given a location that has more than one child locations with a Container that has a Correct ContainerLocation in the given timeframe,
        /// assert that the returned ContainerStatus data includes these Containers.
        /// </summary>
        [Test]
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
                Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
        }

        /// <summary>
        /// Given a location that has one Container with two ContainerLocations in the given timeframe,
        /// given that the first ContainerLocation is of Type ReconcileScans and the second is not,
        /// assert that the returned ContainerStatus data has a ContainerStatus value of ScannedCorrect
        /// </summary>
        [Test]
        public void getContainerStatusesTestScanTrumpsTouch()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                Type: CswEnumNbtContainerLocationTypeOptions.Dispense.ToString() );
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
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.ScannedCorrect.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one disposed Container and a ContainerLocation in the given timeframe with a "Moved, Dispensed, or Disposed/Undisposed"  status,
        /// assert that the returned ContainerStatus data has a container  with a ContainerStatus value of "Moved, Dispensed, or Disposed/Undisposed"
        /// </summary>
        [Test]
        public void getContainerStatusesTestDisposed()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerNode.DisposeContainer( true );
            ContainerNode.postChanges( true );
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
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        /// <summary>
        /// Given a location that has one disposed Container and a ContainerLocation in the given timeframe with a "Scanned, but already marked Disposed"  status,
        /// assert that the returned ContainerStatus data has a container with a ContainerStatus value of "Scanned, but already marked Disposed"
        /// </summary>
        [Test]
        public void getContainerStatusesTestScannedDisposed()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerNode.DisposeContainer( true );
            ContainerNode.postChanges( true );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, LocationId: LocationId, Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
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
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Disposed.ToString(), Data.ContainerStatuses[0].ContainerStatus );
        }

        #endregion

        #region getContainerStatistics

        /// <summary>
        /// Given a location that has no Containers,
        /// assert that each row of the returned ContainerStatistics data has their ContainerAmount value set to 0
        /// </summary>
        [Test]
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
        [Test]
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
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() )
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
        /// Given a location that has one disposed Container in the given timeframe, given that the container has not been scanned,
        /// when grabbing Containers for Reconcile Scans only,
        /// assert that no ContainerStatistics data is returned.
        /// Prior to resolving Case 31304, this test failed.
        /// </summary>
        [Test]
        public void getContainerStatisticsTestDisposedExcludedFromReconcileScans()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            ContainerNode.DisposeContainer( true );
            ContainerNode.postChanges( true );
            Assert.AreEqual( CswEnumTristate.True, ContainerNode.Disposed.Checked.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = new Collection<ContainerData.ReconciliationTypes>
                {
                    new ContainerData.ReconciliationTypes
                    {
                        Enabled=true, 
                        Type= CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString()
                    }
                }
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                Assert.AreEqual( 0, Stat.ContainerCount );
            }
        }

        /// <summary>
        /// Given a location that has one Container and one ContainerLocation in the given timeframe,
        /// given that the Type of the given ContainerLocation is not enabled,
        /// assert that the returned ContainerStatistics data has a ContainerStatus value of NotScanned
        /// </summary>
        [Test]
        public void getContainerStatisticsTestTypeDisabled()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddDays( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = new Collection<ContainerData.ReconciliationTypes>
                {
                    new ContainerData.ReconciliationTypes
                    {
                        Enabled=true, 
                        Type= CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString()
                    }
                }
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() )
                {
                    Assert.AreEqual( 1, Stat.ContainerCount, "There should have been one NotScanned container." );
                }
                else
                {
                    Assert.AreEqual( 0, Stat.ContainerCount, "Status " + Stat.Status + " should have been empty." );
                }
            }
        }

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that the returned ContainerStatus data's Correct ContainerStatus row's ContainerCount value > 1.
        /// </summary>
        [Test]
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
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.Correct.ToString() )
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
        [Test]
        public void getContainerStatisticsTestPercentScanned()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, 
                LocationId: LocationId, 
                ContainerScan: ContainerNode.Barcode.Barcode, 
                Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
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
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 0, Stat.PercentScanned );
                }
                else if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.ScannedCorrect.ToString() )
                {
                    Assert.AreEqual( 100, Stat.PercentScanned );
                }
            }
        }

        /// <summary>
        /// Given a location that has one Container with two ContainerLocations in the given timeframe,
        /// given that the first ContainerLocation is of Type ReconcileScans and the second is not,
        /// assert that both the returned data's NotScanned and Received/Dispensed/Disposed/etc 
        /// ContainerStatus row's ContainerCount value = 0
        /// </summary>
        [Test]
        public void getContainerStatisticsTestScanTrumpsTouch()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
                LocationId: LocationId,
                ContainerScan: ContainerNode.Barcode.Barcode,
                Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, 
                LocationId: LocationId, 
                Type: CswEnumNbtContainerLocationTypeOptions.Dispense.ToString() );
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
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() ||
                    Stat.Status == CswEnumNbtContainerLocationStatusOptions.Correct.ToString() )
                {
                    Assert.AreEqual( 0, Stat.ContainerCount );
                }
                else if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.ScannedCorrect.ToString())
                {
                    Assert.AreEqual( 1, Stat.ContainerCount );
                }
            }
        }

        /// <summary>
        /// Given a Container that was scanned in the wrong location, 
        /// given that no other containers were scanned in the right location,
        /// when container statistics are gathered for both locations,
        /// assert that the scanned location does not show up in the list of unscanned locations.
        /// Prior to resolving Case 31153, this test failed.
        /// </summary>
        [Test]
        public void getContainerStatisticsTestWrongScannedLocation()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswPrimaryKey LocationId2 = TestData.Nodes.createLocationNode( Name: "Room2", ParentLocationId: LocationId ).NodeId;
            CswPrimaryKey LocationId3 = TestData.Nodes.createLocationNode( Name: "Room3", ParentLocationId: LocationId ).NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId2 );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node,
               LocationId: LocationId3,
               ContainerScan: ContainerNode.Barcode.Barcode,
               Type: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = true,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getContainerStatistics( Request );
            foreach( ContainerData.UnscannedLocation UnscannedLocation in Data.UnscannedLocations )
            {
                Assert.AreNotEqual( UnscannedLocation.LocationId, LocationId3.ToString() );
            }
        }

        #endregion

        #region getReconciliationData

        /// <summary>
        /// Given a location that has more than one Container with ContainerLocations in the given timeframe with a Correct status,
        /// assert that both expected ContainerStatus and ContainerStatistics data is returned.
        /// </summary>
        [Test]
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
                Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.Correct.ToString(), Stat.ContainerStatus );
            }
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.Status == CswEnumNbtContainerLocationStatusOptions.Correct.ToString() )
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
            for( int i = 0; i < CswEnumNbtContainerLocationTypeOptions._All.Count(); i++ )
            {
                Types.Add( new ContainerData.ReconciliationTypes { Enabled = true, Type = CswEnumNbtContainerLocationTypeOptions._All.ToArray()[i].ToString() } );
            }
            return Types;
        }

        #endregion

        #region saveContainerActions

        /// <summary>
        /// Given that no ContainerLocation actions have changed,
        /// assert that no exception is thrown
        /// </summary>
        [Test]
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
        [Test]
        public void saveContainerActionsTestMarkMissing()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            Collection<ContainerData.ReconciliationActions> Actions = new Collection<ContainerData.ReconciliationActions>();
            ContainerData.ReconciliationActions Action = new ContainerData.ReconciliationActions
            {
                Action = CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString(),
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
            ReconciliationAction.saveContainerActions( Request );
            CswNbtObjClassContainerLocation NewContLocNode = _getNewContianerLocation( ContainerNode.NodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationTypeOptions.Missing.ToString(), NewContLocNode.Type.Value );
            Assert.AreEqual( CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString(), NewContLocNode.Action.Value );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString(), NewContLocNode.Status.Value );
        }

        /// <summary>
        /// Given an unscanned ContainerLocation whose action has been set to Ignore,
        /// assert that a new ContainerLocation node has been created with an action of Ignore
        /// </summary>
        [Test]
        public void saveContainerActionsTestNotScannedNoAction()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            Collection<ContainerData.ReconciliationActions> Actions = new Collection<ContainerData.ReconciliationActions>();
            ContainerData.ReconciliationActions Action = new ContainerData.ReconciliationActions
            {
                Action = CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
                ContainerId = ContainerNode.NodeId.ToString(),
                LocationId = LocationId.ToString()
            };
            Actions.Add( Action );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( 1 ).ToString(),
                EndDate = DateTime.Now.AddHours( 2 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false,
                ContainerActions = Actions
            };
            ReconciliationAction.saveContainerActions( Request );
            CswNbtObjClassContainerLocation NewContLocNode = _getNewContianerLocation( ContainerNode.NodeId );
            Assert.AreEqual( CswEnumNbtContainerLocationTypeOptions.Ignore.ToString(), NewContLocNode.Type.Value );
            Assert.AreEqual( CswEnumNbtContainerLocationActionOptions.Ignore.ToString(), NewContLocNode.Action.Value );
            Assert.AreEqual( CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString(), NewContLocNode.Status.Value );
        }

        /// <summary>
        /// Given a ContainerLocation whose action has been set to Ignore,
        /// assert that the selected ContainerLocation has its action set to Ignore
        /// </summary>
        [Test]
        public void saveContainerActionsTestNoAction()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            CswNbtObjClassContainerLocation ContLocNode = TestData.Nodes.createContainerLocationNode( ContainerNode.Node, LocationId: LocationId, ContainerScan: ContainerNode.Barcode.Barcode );
            Collection<ContainerData.ReconciliationActions> Actions = new Collection<ContainerData.ReconciliationActions>();
            ContainerData.ReconciliationActions Action = new ContainerData.ReconciliationActions
            {
                Action = CswEnumNbtContainerLocationActionOptions.Ignore.ToString(),
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
            Assert.AreNotEqual( CswEnumNbtContainerLocationActionOptions.Ignore.ToString(), ContLocNode.Action.Value );
            ReconciliationAction.saveContainerActions( Request );
            Assert.AreEqual( CswEnumNbtContainerLocationActionOptions.Ignore.ToString(), ContLocNode.Action.Value );
        }

        private CswNbtObjClassContainerLocation _getNewContianerLocation( CswPrimaryKey ContainerId )
        {
            CswNbtMetaDataObjectClass ContainerOC = TestData.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass ContainerLocationOC = TestData.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );
            CswNbtView ContainersView = new CswNbtView( TestData.CswNbtResources );

            CswNbtViewRelationship ContainerVR = ContainersView.AddViewRelationship( ContainerOC, false );
            ContainerVR.NodeIdsToFilterIn = new Collection<CswPrimaryKey>{ ContainerId };
            CswNbtViewRelationship ContainerLocationVR = ContainersView.AddViewRelationship( ContainerVR, CswEnumNbtViewPropOwnerType.Second, ContainerOCP, false );
            CswNbtViewProperty ScanDateVP = ContainersView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            ContainersView.setSortProperty( ScanDateVP, CswEnumNbtViewPropertySortMethod.Descending );
            ICswNbtTree ContainersTree = TestData.CswNbtResources.Trees.getTreeFromView( ContainersView, false, true, false );
            ContainersTree.goToNthChild( 0 );
            ContainersTree.goToNthChild( 0 );
            CswNbtObjClassContainerLocation ContLocNode = TestData.CswNbtResources.Nodes.GetNode( ContainersTree.getNodeIdForCurrentPosition() );
            return ContLocNode;
        }

        #endregion

        #region getOutstandingChangesCount

        /// <summary>
        /// Given a location that has no Containers,
        /// assert that the returned getOutstandingChangesCount is 0
        /// </summary>
        [Test]
        public void getOutstandingChangesCountTestNoContainers()
        {
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = TestData.Nodes.createLocationNode().NodeId.ToString(),
                IncludeChildLocations = false,
                ContainerLocationTypes = _getTypes()
            };
            ContainerData Data = ReconciliationAction.getOutstandingActionsCount( Request );
            Assert.AreEqual( 0, Data.OutstandingActionsCount );
    }

        /// <summary>
        /// Given a location that has one Container and a ContainerLocation with no changes to Action
        /// assert that the returned getOutstandingChangesCount is 0
        /// </summary>
        [Test]
        public void getOutstandingChangesCountTestNoChanges()
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
            ContainerData Data = ReconciliationAction.getOutstandingActionsCount( Request );
            Assert.AreEqual( 0, Data.OutstandingActionsCount );
}

        /// <summary>
        /// Given a ContainerLocation whose action has been set to Ignore and saved,
        /// assert that the returned getOutstandingChangesCount is 1
        /// </summary>
        [Test]
        public void getOutstandingChangesCountTestOneChange()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerNode( LocationId: LocationId );
            TestData.Nodes.createContainerLocationNode( ContainerNode.Node, CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString(), LocationId: LocationId, ContainerScan: ContainerNode.Barcode.Barcode );
            ContainerData.ReconciliationRequest Request = new ContainerData.ReconciliationRequest
            {
                StartDate = DateTime.Now.AddHours( -1 ).ToString(),
                EndDate = DateTime.Now.AddSeconds( 1 ).ToString(),
                LocationId = LocationId.ToString(),
                IncludeChildLocations = false
            };
            ContainerData Data = ReconciliationAction.getOutstandingActionsCount( Request );
            Assert.AreEqual( 1, Data.OutstandingActionsCount );
        }

        #endregion
    }
}
