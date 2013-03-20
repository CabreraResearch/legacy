﻿using System;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Test.Actions
{
    [TestFixture]
    public class CswNbtActionDispenseContainerTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        #region dispenseSourceContainer Test

        [Test]
        public void dispenseSourceContainerTestInvalidDispenseType()
        {
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "gal", 2.64172052, -1, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 0.5, LiterNode );
            string InvalidDispenseType = "Receive";
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            try
            {
                JObject obj = wiz.dispenseSourceContainer( InvalidDispenseType, "5", LiterNode.NodeId.ToString(), String.Empty );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( InvalidDispenseType, "Add", e.Message );
                Assert.AreNotEqual( InvalidDispenseType, "Waste", e.Message );
            }
        }

        [Test]
        public void dispenseSourceContainerTestAdd()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add Material to Container";
            JObject obj = wiz.dispenseSourceContainer( DispenseType, ".5", LiterNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseSourceContainerTestAddBasicConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add Material to Container";
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, -3, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "500", MilliliterNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseSourceContainerTestAddWeightToVolumeConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( "Chemical", "Liquid", .1 );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 0.5, LiterNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add Material to Container";
            CswNbtNode GramNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "g", 1.0, -3, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "50", GramNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseSourceContainerTestAddVolumeToWeightConversion()
        {
            double Expected = 1.0;
            CswNbtNode KilogramNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( "Chemical", "Liquid", .1 );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 0.5, KilogramNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add Material to Container";
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "5", LiterNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseSourceContainerTestWaste()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Waste Material";
            JObject obj = wiz.dispenseSourceContainer( DispenseType, ".5", LiterNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseSourceContainerTestDispenseForUse()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Dispense for Use";
            JObject obj = wiz.dispenseSourceContainer( DispenseType, ".5", LiterNode.NodeId.ToString(), String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 0, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        #endregion

        #region dispenseIntoChildContainers Tests

        [Test]
        public void dispenseIntoChildContainersTestWasteOne()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"0\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseIntoChildContainersTestWasteTwo()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, -3, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 2.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"0\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseRow2 = "{ \"containerNo\":\"0\", \"quantity\":\"500\", \"unitid\":\"" + MilliliterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "," + DispenseRow2 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseIntoChildContainersTestDispenseOne()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"1\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 1, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseIntoChildContainersTestDispenseTwo()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, -3, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 2.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"1\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseRow2 = "{ \"containerNo\":\"1\", \"quantity\":\"500\", \"unitid\":\"" + MilliliterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "," + DispenseRow2 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            List<CswNbtObjClassContainer> NewContainers = _getNewContainers( ContainerNode.NodeId );
            foreach( CswNbtObjClassContainer NewContainer in NewContainers )
            {
                Assert.IsTrue( ( NewContainer.Quantity.Quantity == 500 && NewContainer.Quantity.UnitId == MilliliterNode.NodeId ) ||
                   ( NewContainer.Quantity.Quantity == 0.5 && NewContainer.Quantity.UnitId == LiterNode.NodeId ) );
            }
            Assert.AreEqual( 2, NewContainers.Count );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseIntoChildContainersTestDispenseOneWasteOne()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, -3, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 2.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"1\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseRow2 = "{ \"containerNo\":\"0\", \"quantity\":\"500\", \"unitid\":\"" + MilliliterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "," + DispenseRow2 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 1, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        [Test]
        public void dispenseIntoChildContainersTestDispenseMany()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.Nodes.createContainerNode( "Container", 2.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"containerNo\":\"3\", \"quantity\":\"0.5\", \"unitid\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid, String.Empty );
            Assert.IsNotNull( obj );
            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 3, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        #endregion

        #region Private Helper Methods

        private double _getNewSourceContainerQuantity( CswPrimaryKey SourceContainerId )
        {
            CswNbtObjClassContainer UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( SourceContainerId );
            return UpdatedContainerNode.Quantity.Quantity;
        }

        private int _getNewContainerCount( CswPrimaryKey SourceContainerId )
        {
            List<CswNbtObjClassContainer> NewContainers = _getNewContainers( SourceContainerId );
            return NewContainers.Count;
        }

        private List<CswNbtObjClassContainer> _getNewContainers( CswPrimaryKey SourceContainerId )
        {
            List<CswNbtObjClassContainer> NewContainers = new List<CswNbtObjClassContainer>();
            CswNbtMetaDataObjectClass ContainerOc = TestData.CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            IEnumerator CurrentNodes = TestData.CswNbtResources.Nodes.GetEnumerator();
            while( CurrentNodes.MoveNext() )
            {
                DictionaryEntry dentry = (DictionaryEntry) CurrentNodes.Current;
                CswNbtNode CurrentNode = (CswNbtNode) dentry.Value;
                if( CurrentNode.ObjClass.ObjectClass == ContainerOc )
                {
                    CswNbtObjClassContainer NewContainer = CurrentNode;
                    if( NewContainer.SourceContainer.RelatedNodeId == SourceContainerId )
                    {
                        NewContainers.Add( NewContainer );
                    }
                }
            }
            return NewContainers;
        }

        #endregion

    }
}
