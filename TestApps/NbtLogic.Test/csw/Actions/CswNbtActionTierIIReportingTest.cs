using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.RscAdo;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Actions
{
    [TestFixture]
    public class CswNbtActionTierIIReportingTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private Int32 _TierIIHWM;
        private CswNbtActTierIIReporting TierIIAction;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData { FinalizeNodes = true };
            TierIIAction = new CswNbtActTierIIReporting( TestData.CswNbtResources );
            DataTable MaxTierIITable = TestData.CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "getHWM", "select max(tier2id) as hwm from tier2" );
            _TierIIHWM = CswConvert.ToInt32( MaxTierIITable.Rows[0]["hwm"] );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
            TestData.CswNbtResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( "delete from tier2 where tier2id > " + _TierIIHWM );
        }

        #endregion

        #region getTierIIData

        /// <summary>
        /// Given a location and timeframe that has no Materials,
        /// assert that the returned TierII data is empty
        /// </summary>
        [Test]
        public void getTierIIDataTestNoData()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddDays( -10 ).ToString(),
                EndDate = DateTime.Now.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a location and timeframe that has one Material,
        /// assert that the returned TierII data contains one Material
        /// </summary>
        [Test]
        public void getTierIIDataTestOneMaterial()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode();
            CswNbtNode KilogramsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1, 0, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, KilogramsUnit, ChemicalNode, LocationId ) ;
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddDays( -10 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );//Material exists
            Assert.IsNotNullOrEmpty( Data.Materials[0].TradeName );//Material name exists
            Assert.AreEqual( "12-34-0", Data.Materials[0].CASNo );//Material data exists
            Assert.AreEqual( "Storage", Data.Materials[0].Storage[0].UseType );//Container data exists
            Assert.AreEqual( "New Room", Data.Materials[0].Locations[0].Location );//Location data exists
        }

        /// <summary>
        /// Given a location and timeframe that has one Material for two days,
        /// given that the quantity of that Material is different for each day,
        /// assert that the returned TierII data contains an average quantity of the total quantity between Dat 1 and Day 2.
        /// Prior to resolving Case 31508, this test failed.
        /// </summary>
        [Test]
        public void getTierIIDataTestTwoDaysAverage()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode();
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today.AddDays( -1 ) );
            TestData.CswNbtResources.execArbitraryPlatformNeutralSql( "update tier2 set dateadded = dateadded - 1 where tier2id > " + _TierIIHWM );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddDays( -10 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 2, Data.Materials[0].DaysOnSite );//Material existed for two days
            Assert.AreEqual( 1.5, Data.Materials[0].AverageQty );//AverageQty is the average between Day 1 and Day 2
        }

        #endregion getTierIIData

        #region Acceptance Criteria

        //for a given day, accurately display the following:
        //all containers of all TierII materials with CASNos in a given location (or sublocation of the given location)
        //for each material: total qty in lbs (max and average should be the same), all locations and all container use types
        //TierII materials that are components of other materials should be displayed in their proper percentages

        /// <summary>
        /// Given a material with IsTierII set to false,
        /// Given a location that allows inventory,
        /// Given a container of the given material in the given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material is not listed
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialNotPresentNotTierII()
        {
            Assert.Inconclusive("Write Me!");
    }

        /// <summary>
        /// Given a material with an invalid (or missing) CASNo and IsTierII set to true,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material is not listed
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialNotPresentBadCASNo()
        {
            Assert.Inconclusive( "Write Me!" );
}

        /// <summary>
        /// Given a material with a valid CASNo, IsTierII set to true, and a physical state of solid,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material is listed with MaxQty and AvgQty set to the given container's quantity (in pounds)
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialPresent()
        {
            Assert.Inconclusive("Write Me!");
        }

        /// <summary>
        /// Given a material with a physical state of liquid, and specific gravity defined,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material is listed with MaxQty and AvgQty set to the given container's quantity (in pounds)
        /// after accounting for volume-to-weight conversion
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialPresentSpecificGravity()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a material that has a component with a percentage of 50%,
        /// Given that the given component points to a constituent with a valid CASNo and IsTierII set to true,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material's component constituent is listed 
        /// with MaxQty and AvgQty set to 50% of the given container's quantity
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialComponentPresent()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a parent location A with two child locations B and C,
        /// Given two containers of a given TierII material, one in location B and one in location C,
        /// When the TierII report is run for one day on the parent location,
        /// Assert that the given material is listed with MaxQty and AvgQty set to the sum of both container quantities
        /// and that both location B and location C are listed under Storage Locations
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialPresentInChildLocations()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a parent location A with child location B, and location C as a child of B,
        /// Given a container of a given TierII material in location C,
        /// When the TierII report is run for one day on location A,
        /// Assert that the given material is listed with MaxQty and AvgQty set to the given container's quantity
        /// and that location C is listed under Storage Locations
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialPresentInChildOfChildLocation()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given two containers of a given TierII material in a given location, 
        /// one with a UseType of Storage and one with a UseType of Closed,
        /// When the TierII report is run for one day on the parent location,
        /// Assert that the given material is listed with MaxQty and AvgQty set to the sum of both container quantities
        /// and that both Storage and Closed are listed under Type of Storage
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialPresentWithMultipleStorageTypes()
        {
            //the same should hold true for storage conditions (pressure/temperature)
            Assert.Inconclusive( "Write Me!" );
        }

        //for more than one day, account for the following actions:
        //moving containers to different location (ensure both locations show up)
        //receiving containers (ensure max/avg qty are affected correctly)
        //dispensing contianers (for use and into child containers - ensure max/avg qty are affected correctly)
        //disposing/undisposing containers (ensure daysonsite includes all days excluding disposed days)
        //changing a container's use type (ensure both use types show up)

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container has its UseType changed from Closed to Open on the second day,
        /// When the TierII report is run for two days on the given location,
        /// Assert that the given material is listed with both Closed and Open listed under Type of Storage
        /// </summary>
        [Test]
        public void TierII_2Days_ChangeContainerStorageType()
        {
            //the same should hold true for storage conditions (pressure/temperature)
            Assert.Inconclusive( "Write Me!" );
        }
        
        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that another container of the same material is added to the given location on the second day,
        /// When the TierII report is run for two days on the given location,
        /// Assert that the given material is listed with MaxQty set to the total of both container quantities,
        /// AvgQty set to ( MaxQty + Quantity of 1st Container ) / 2,
        /// and DaysOnSite set to 2
        /// </summary>
        [Test]
        public void TierII_2Days_MaterialReceive()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container has half of its quantity dispensed on the second day,
        /// When the TierII report is run for two days on the given location,
        /// Assert that the given material is listed with MaxQty set to the container's quantity on Day 1,
        /// and AvgQty set to ( Container Quantity on Day 1 + Container Quantity on Day 2 ) / 2
        /// </summary>
        [Test]
        public void TierII_2Days_MaterialDispenseForUse()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container has half of its quantity dispensed into a second container (in the same location) on the second day,
        /// When the TierII report is run for two days on the given location,
        /// Assert that the given material is listed with both MaxQty and AvgQty set to the total of both container quantities,
        /// </summary>
        [Test]
        public void TierII_2Days_MaterialDispenseIntoChildContainer()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container is moved to a second, unrelated location on the second day,
        /// When the TierII report is run for two days on the first location,
        /// Assert that the given material is listed with only the first location listed under Storage Locations
        /// and that DaysOnSite is set to 1
        /// </summary>
        [Test]
        public void TierII_2Days_ContainerMoveToUnrelatedLocation()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a parent location A with two child locations B and C,
        /// Given a container of a given TierII material in Location B on the first day,
        /// Given that the container is moved to a Location C on the second day,
        /// When the TierII report is run for two days on Location A,
        /// Assert that the given material is listed with both locations B and C listed under Storage Locations
        /// and that DaysOnSite is set to 2
        /// </summary>
        [Test]
        public void TierII_2Days_ContainerMoveToOtherChildLocation()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container is disposed on the second day,
        /// When the TierII report is run for two days on the given location,
        /// Assert that the given material is listed with MaxQty set to the container's quantity on Day 1,
        /// AvgQty set to MaxQty / 2,
        /// and DaysOnSite set to 1
        /// </summary>
        [Test]
        public void TierII_2Days_ContainerDispose()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container is disposed on the second day,
        /// Given that the container is undisposed on the third day,
        /// When the TierII report is run for three days on the given location,
        /// Assert that the given material is listed with MaxQty set to the container's quantity on Day 1,
        /// AvgQty set to ( MaxQty * 2 ) / 3,
        /// and DaysOnSite set to 2
        /// </summary>
        [Test]
        public void TierII_3Days_ContainerDisposeAndUndispose()
        {
            Assert.Inconclusive( "Write Me!" );
        }

        #endregion Acceptance Criteria
    }
}
