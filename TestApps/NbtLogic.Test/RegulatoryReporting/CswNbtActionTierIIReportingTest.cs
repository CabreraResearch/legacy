using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.RscAdo;
using NUnit.Framework;

namespace ChemSW.Nbt.Test
{
    [TestFixture]
    public class CswNbtActionTierIIReportingTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private CswNbtActTierIIReporting TierIIAction;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            TierIIAction = new CswNbtActTierIIReporting( TestData.CswNbtResources );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
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

        /* TODO - figure out why calling a stored procedure is ignoring to newly-written nodes
         * perhaps also try 
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
            //TestData.CswNbtResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( "begin TIER_II_DATA_MANAGER.SET_TIER_II_DATA(); end;" );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = LocationId.ToString(),
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.IsTrue( Data.Materials[0].Storage.Count > 0 );
            Assert.IsTrue( Data.Materials[0].Locations.Count > 0 );
        }
         */

        #endregion getTierIIData
    }
}
