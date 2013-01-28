using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.RscAdo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSw.Nbt.Test
{
    [TestClass]
    public class CswNbtActionTierIIReportingTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private CswNbtActTierIIReporting TierIIAction;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            TierIIAction = new CswNbtActTierIIReporting( TestData.CswNbtResources );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
        }

        #endregion

        #region getTierIIData

        /// <summary>
        /// Given a location and timeframe that has no Materials,
        /// assert that the returned TierII data is empty
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public void getTierIIDataTestOneMaterial()
        {
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode().NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode();
            CswNbtNode KilogramsUnit = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1, 1, Tristate.True );
            TestData.Nodes.createContainerNode("Container", 1, KilogramsUnit, ChemicalNode, CswConvert.ToPrimaryKey("nodes_23407"));//LocationId) );
            TestData.CswNbtResources.execStoredProc( "TIER_II_DATA_MANAGER.SET_TIER_II_DATA", new List<CswStoredProcParam>() );
            TierIIData.TierIIDataRequest Request = new TierIIData.TierIIDataRequest
            {
                LocationId = "nodes_23407",//LocationId.ToString(),
                StartDate = DateTime.Now.AddYears( -1 ).ToString(),
                EndDate = DateTime.Now.AddDays( 1 ).ToString()
            };
            TierIIData Data = TierIIAction.getTierIIData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.IsTrue( Data.Materials[0].Storage.Count > 0 );
            Assert.IsTrue( Data.Materials[0].Locations.Count > 0 );
        }

        #endregion getTierIIData
    }
}
