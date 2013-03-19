using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.RscAdo;
using NUnit.Framework;

namespace ChemSW.Nbt.Test
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
            TestData = new TestData();
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
            CswNbtNode KilogramsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1, 1, Tristate.True );
            TestData.Nodes.createContainerNode("Container", 1, KilogramsUnit, ChemicalNode, LocationId) ;
            TestData.CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
        }

        #endregion getTierIIData
    }
}
