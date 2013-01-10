using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSw.Nbt.Test
{
    [TestClass]
    public class CswNbtActionHMISReportingTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private CswNbtActHMISReporting HMISAction;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            HMISAction = new CswNbtActHMISReporting( TestData.CswNbtResources );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
        }

        #endregion

        #region getHMISData

        /// <summary>
        /// Given a Control Zone that has no Locations,
        /// assert that the returned HMIS data is empty
        /// </summary>
        [TestMethod]
        public void getHMISDataTestNoLocations()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a Control Zone that has a Location with no Containers,
        /// assert that the returned HMIS data is empty
        /// </summary>
        [TestMethod]
        public void getHMISDataTestNoContainers()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a Control Zone that has a Location with a Container whose Material is Not Reportable,
        /// assert that the returned HMIS data is empty
        /// </summary>
        [TestMethod]
        public void getHMISDataTestNotReportable()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( Hazards: "Exp", SpecialFlags: "Not Reportable" );
            TestData.Nodes.createContainerNode( "Container", 1, null, ChemicalNode, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a Control Zone that has a Location with a Container whose Material has no hazards,
        /// assert that the returned HMIS data is empty
        /// </summary>
        [TestMethod]
        public void getHMISDataTestNotHazardous()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( );
            TestData.Nodes.createContainerNode( "Container", 1, null, ChemicalNode, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 0, Data.Materials.Count );
        }

        /// <summary>
        /// Given a Control Zone that has a Location with a Container whose Material has 1 hazard,
        /// assert that the returned HMIS data contains 1 row with the expected hazard class info
        /// </summary>
        [TestMethod]
        public void getHMISDataTestOneHazard()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( Hazards: "Exp" );
            CswNbtNode GallonsUnit = TestData.Nodes.createUnitOfMeasureNode( "Volume", "gal_Test", 2.641721, -1, Tristate.True );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 1, Data.Materials[0].Storage.Solid.MAQ );
            Assert.AreEqual( -1, Data.Materials[0].Storage.Liquid.MAQ );
            Assert.AreEqual( 0.25, Data.Materials[0].Closed.Solid.MAQ );
            Assert.AreEqual( -0.25, Data.Materials[0].Closed.Liquid.MAQ );
            Assert.AreEqual( 0.25, Data.Materials[0].Open.Solid.MAQ );
            Assert.AreEqual( -0.25, Data.Materials[0].Open.Liquid.MAQ );
        }

        /// <summary>
        /// Given a Container whose Material has 2 hazards,
        /// assert that the returned HMIS data contains 2 rows, each with the given material and container qty
        /// </summary>
        [TestMethod]
        public void getHMISDataTestTwoHazards()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( Hazards: "Carc,Exp" );
            CswNbtNode GallonsUnit = TestData.Nodes.createUnitOfMeasureNode( "Volume", "gal_Test", 2.641721, -1, Tristate.True );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 2, Data.Materials.Count );
            Assert.AreNotEqual( Data.Materials[0].FireClass, Data.Materials[1].FireClass );
            foreach( HMISData.HMISMaterial Material in Data.Materials )
            {
                Assert.AreEqual( ChemicalNode.NodeName, Material.Material );
                Assert.AreEqual( 1, Material.Storage.Liquid.Qty );
            }
        }

        /// <summary>
        /// Given 2 Containers, each with the same 1 hazard Material,
        /// assert that the returned HMIS data contains 1 row with the combined qty of both containers
        /// </summary>
        [TestMethod]
        public void getHMISDataTestTwoContainersSameMaterial()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( Hazards: "Exp" );
            CswNbtNode GallonsUnit = TestData.Nodes.createUnitOfMeasureNode( "Volume", "gal_Test", 2.641721, -1, Tristate.True );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode, LocationId );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 1, Data.Materials.Count );
            Assert.AreEqual( 2, Data.Materials[0].Storage.Liquid.Qty );
        }

        /// <summary>
        /// Given 2 Containers, each with different Materials (but with the same hazard),
        /// assert that the returned HMIS data contains 2 rows with the combined qty of both containers
        /// </summary>
        [TestMethod]
        public void getHMISDataTestTwoContainersDifferentMaterial()
        {
            CswPrimaryKey ControlZoneId = TestData.Nodes.createControlZoneNode().NodeId;
            CswPrimaryKey LocationId = TestData.Nodes.createLocationNode( ControlZoneId: ControlZoneId ).NodeId;
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( Hazards: "Exp" );
            CswNbtNode ChemicalNode2 = TestData.Nodes.createMaterialNode( Hazards: "Exp" );
            CswNbtNode GallonsUnit = TestData.Nodes.createUnitOfMeasureNode( "Volume", "gal_Test", 2.641721, -1, Tristate.True );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode, LocationId );
            TestData.Nodes.createContainerNode( "Container", 1, GallonsUnit, ChemicalNode2, LocationId );
            HMISData.HMISDataRequest Request = new HMISData.HMISDataRequest
            {
                ControlZoneId = ControlZoneId.ToString()
            };
            HMISData Data = HMISAction.getHMISData( Request );
            Assert.AreEqual( 2, Data.Materials.Count );
            foreach( HMISData.HMISMaterial Material in Data.Materials )
            {
                Assert.AreEqual( "Exp", Material.FireClass );
                Assert.AreEqual( 1, Material.Storage.Liquid.Qty );
            }
        }

        #endregion getHMISData
    }
}
