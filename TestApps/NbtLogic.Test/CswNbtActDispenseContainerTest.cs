using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Actions;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NbtLogic.Test
{
    [TestClass]
    public class CswNbtActDispenseContainerTest
    {
        #region Setup and Teardown

        private TestData TestData = null;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
        }

        #endregion

        [TestMethod]
        public void updateDispensedContainerTestInvalidDispenseType()
        {
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "gal", 2.64172052, -1, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            string InvalidDispenseType = "Receive";
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            try
            {
                JObject obj = wiz.updateDispensedContainer( InvalidDispenseType, "5", LiterNode.NodeId.ToString() );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( InvalidDispenseType, "Add", e.Message );
                Assert.AreNotEqual( InvalidDispenseType, "Waste", e.Message );
            }
        }

        [TestMethod]
        public void updateDispensedContainerTestAdd()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            JObject obj = wiz.updateDispensedContainer( DispenseType, ".5", LiterNode.NodeId.ToString() );
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }

        [TestMethod]
        public void updateDispensedContainerTestAddBasicConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode MilliliterNode = TestData.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, 3, Tristate.True );
            JObject obj = wiz.updateDispensedContainer( DispenseType, "500", MilliliterNode.NodeId.ToString() );
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }

        [TestMethod]
        public void updateDispensedContainerTestAddWeightToVolumeConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode GramNode = TestData.createUnitOfMeasureNode( "Weight", "g", 1.0, 3, Tristate.True );
            JObject obj = wiz.updateDispensedContainer( DispenseType, "50", GramNode.NodeId.ToString() );
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }

        [TestMethod]
        public void updateDispensedContainerTestAddVolumeToWeightConversion()
        {
            double Expected = 1.0;
            CswNbtNode KilogramNode = TestData.createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, KilogramNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            JObject obj = wiz.updateDispensedContainer( DispenseType, "5", LiterNode.NodeId.ToString() );
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }

        [TestMethod]
        public void updateDispensedContainerTestWaste()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Waste";
            JObject obj = wiz.updateDispensedContainer( DispenseType, ".5", LiterNode.NodeId.ToString() );
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }
    }
}
