using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                StartDate = DateTime.Today.ToString(),
                EndDate = DateTime.Now.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( IsTierII: CswEnumTristate.False );
            CswNbtNode KilogramsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1, 0, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, KilogramsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a material with a missing CASNo and IsTierII set to true,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material is not listed
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialNotPresentNoCASNo()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( CASNo: "" );
            CswNbtNode KilogramsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1, 0, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, KilogramsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1, Data.Materials[0].AverageQty );
            Assert.IsNotNullOrEmpty( Data.Materials[0].TradeName );//Material name exists
            Assert.AreEqual( "12-34-0", Data.Materials[0].CASNo );//Material data exists
            Assert.AreEqual( "Storage", Data.Materials[0].Storage[0].UseType );//Container data exists
            Assert.AreEqual( "New Room", Data.Materials[0].Locations[0].Location );//Location data exists
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( SpecificGravity: 0.5 );//1 L = .5 kg = 1.102 lb
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, LiterNode, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1.102, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1.102, Data.Materials[0].AverageQty );
        }

        /// <summary>
        /// Given a material that is not Tier II, that has a component with a percentage of 10%,
        /// Given that the component points to a constituent with a valid CASNo and IsTierII set to true,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the given material's component constituent is listed 
        /// with MaxQty and AvgQty set to 10% of the given container's quantity
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialComponentPresent()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode Constituent = TestData.Nodes.createConstituentNode( IsTierII: CswEnumTristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( CASNo: "", IsTierII: CswEnumTristate.False, State: "Solid", Constituents: new Collection<CswNbtNode>{ Constituent } );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 0.1, Data.Materials[0].MaxQty );
        }

        /// <summary>
        /// Given a material with a valid CASNo, IsTierII set to true, and a component with a percentage of 10%,
        /// Given that the component points to a constituent with a valid CASNo and IsTierII set to true,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that both the given material and its component constituent is listed 
        /// with MaxQty and AvgQty set to 100% and 10% of the given container's quantity, respectively
        /// </summary>
        [Test]
        public void TierII_1Day_MaterialComponentAndBasePresent()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode Constituent = TestData.Nodes.createConstituentNode( IsTierII: CswEnumTristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( CASNo: "50-00-0", IsTierII: CswEnumTristate.True, State: "Solid", Constituents: new Collection<CswNbtNode> { Constituent } );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 2, Data.Materials.Count );
            foreach( TierIIData.TierIIMaterial TierIIMat in Data.Materials )
            {
                Assert.AreEqual( TierIIMat.CASNo == "12-34-0" ? .1 : 1, TierIIMat.MaxQty );
            }
        }

        /// <summary>
        /// Given a Tier II material with two components, each with a percentage of 10%,
        /// Given that one component points to a constituent with a valid CASNo and IsTierII set to true,
        /// Given that the other component's constituent does not,
        /// Given a container of the given material in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that only the base material and constituent with IsTierII = True is listed 
        /// </summary>
        [Test]
        public void TierII_1Day_OneOfTwoMaterialComponentsPresent()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode Constituent1 = TestData.Nodes.createConstituentNode( IsTierII: CswEnumTristate.True );
            CswNbtNode Constituent2 = TestData.Nodes.createConstituentNode( IsTierII: CswEnumTristate.False, CASNo: "67-64-1" );
            Collection<CswNbtNode> Constituents = new Collection<CswNbtNode>{ Constituent1, Constituent2 };
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( CASNo: "50-00-0", IsTierII: CswEnumTristate.True, State: "Solid", Constituents: Constituents );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 2, Data.Materials.Count );
            foreach( TierIIData.TierIIMaterial TierIIMat in Data.Materials )
            {
                Assert.AreEqual( TierIIMat.CASNo == "12-34-0" ? .1 : 1, TierIIMat.MaxQty );
            }
        }

        /// <summary>
        /// Given two materials with the same CASNo and IsTierII set to true,
        /// Given two containers, one of each material, in a given location,
        /// When the TierII report is run for one day on the given location,
        /// Assert that the CASNo is only listed once under one of the given material's Tradenames,
        /// and is listed with MaxQty and AvgQty set to the total of both containers' quantities
        /// </summary>
        [Test]
        public void TierII_1Day_TwoMaterialsSameCASNoOnePresent()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode ChemicalNode2 = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode2, LocationId );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 2, Data.Materials[0].MaxQty );
            Assert.AreEqual( 2, Data.Materials[0].AverageQty );
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
            CswPrimaryKey LocationIdA = TestData.Nodes.createLocationNode().NodeId;
            CswPrimaryKey LocationIdB = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdA ).NodeId;
            CswPrimaryKey LocationIdC = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdA ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationIdB );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationIdC );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationIdA.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 2, Data.Materials[0].MaxQty );
            Assert.AreEqual( 2, Data.Materials[0].AverageQty );
            Assert.AreEqual( 2, Data.Materials[0].Locations.Count );
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
            CswPrimaryKey LocationIdA = TestData.Nodes.createLocationNode().NodeId;
            CswPrimaryKey LocationIdB = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdA ).NodeId;
            CswPrimaryKey LocationIdC = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdB ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationIdC );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationIdA.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1, Data.Materials[0].AverageQty );
            Assert.AreEqual( 1, Data.Materials[0].Locations.Count );
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, UseType: CswEnumNbtContainerUseTypes.Closed );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 2, Data.Materials[0].MaxQty );
            Assert.AreEqual( 2, Data.Materials[0].AverageQty );
            Assert.AreEqual( 2, Data.Materials[0].Storage.Count );
        }

        //for more than one day, account for the following actions:
        //moving containers to different location (ensure both locations show up)
        //receiving containers (ensure max/avg qty are affected correctly)
        //dispensing contianers (for use and into child containers - ensure max/avg qty are affected correctly)
        //disposing/undisposing containers (ensure daysonsite includes all days excluding disposed days)

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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, UseType: CswEnumNbtContainerUseTypes.Open );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 2, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1.5, Data.Materials[0].AverageQty );
            Assert.AreEqual( 2, Data.Materials[0].DaysOnSite );
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            TestData.Nodes.createContainerDispenseTransactionNode( ContainerNode, Quantity: .5, Type: CswEnumNbtContainerDispenseType.Dispense.ToString() );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1, Data.Materials[0].MaxQty );
            Assert.AreEqual( .75, Data.Materials[0].AverageQty );
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
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            CswNbtObjClassContainer SourceContainer = TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            CswNbtNode ChildContainer = TestData.Nodes.createContainerNode( "Container", 0, PoundsUnit, ChemicalNode, LocationId );
            SourceContainer.DispenseOut( CswEnumNbtContainerDispenseType.Dispense, 0.5, PoundsUnit.NodeId, DestinationContainer: ChildContainer );
            ChildContainer.postChanges( false );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1, Data.Materials[0].AverageQty );
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
            CswPrimaryKey LocationIdA = TestData.Nodes.createLocationNode().NodeId;
            CswPrimaryKey LocationIdB = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationIdA, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            TestData.Nodes.createContainerLocationNode( ContainerNode, Type: CswEnumNbtContainerLocationTypeOptions.Move.ToString(), LocationId: LocationIdB );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationIdA.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials[0].Locations.Count );
            Assert.AreEqual( 1, Data.Materials[0].DaysOnSite );
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
            CswPrimaryKey LocationIdA = TestData.Nodes.createLocationNode().NodeId;
            CswPrimaryKey LocationIdB = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdA ).NodeId;
            CswPrimaryKey LocationIdC = TestData.Nodes.createLocationNode( ParentLocationId: LocationIdA ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationIdB, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            TestData.Nodes.createContainerLocationNode( ContainerNode, Type: CswEnumNbtContainerLocationTypeOptions.Move.ToString(), LocationId: LocationIdC );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationIdA.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 2, Data.Materials[0].DaysOnSite );
            Assert.AreEqual( 2, Data.Materials[0].Locations.Count );
        }

        /// <summary>
        /// Given a container of a given TierII material in a given location on the first day,
        /// Given that the container is disposed on the second day, and nothing happens on the third day,
        /// When the TierII report is run for three days on the given location,
        /// Assert that the given material is listed with MaxQty set to the container's quantity on Day 1,
        /// AvgQty set to MaxQty,
        /// and DaysOnSite set to 2
        /// </summary>
        [Test]
        public void TierII_3Days_ContainerDispose()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( State: "Solid" );
            CswNbtNode PoundsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "lb", 4.53592, -1, CswEnumTristate.True );
            CswNbtObjClassContainer ContainerNode = TestData.Nodes.createContainerWithRecords( "Container", 1, PoundsUnit, ChemicalNode, LocationId, DateTime.Today.AddDays( -1 ), CswEnumNbtContainerUseTypes.Closed );
            ContainerNode.DisposeContainer( true );
            ContainerNode.postChanges( false );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Today.AddDays( -1 ).ToString(),
                EndDate = DateTime.Today.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials[0].MaxQty );
            Assert.AreEqual( 1, Data.Materials[0].AverageQty );
            Assert.AreEqual( 2, Data.Materials[0].DaysOnSite );
        }

        #endregion Acceptance Criteria
    }
}
