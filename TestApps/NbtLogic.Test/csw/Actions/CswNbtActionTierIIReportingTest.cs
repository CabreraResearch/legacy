﻿using System;
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
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
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
            TestData.Nodes.createContainerNode("Container", 1, KilogramsUnit, ChemicalNode, LocationId) ;
            TestData.CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
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
            TestData.Nodes.createContainerNode( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TestData.CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
            TestData.CswNbtResources.execArbitraryPlatformNeutralSql( "update tier2 set dateadded = dateadded - 1 where tier2id > " + _TierIIHWM );
            TestData.Nodes.createContainerNode( "Container", 1, PoundsUnit, ChemicalNode, LocationId );
            TestData.CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 2, Data.Materials[0].DaysOnSite );//Material existed for two days
            Assert.AreEqual( 1.5, Data.Materials[0].AverageQty );//AverageQty is the average between Day 1 and Day 2
        }

        #endregion getTierIIData
    }
}
