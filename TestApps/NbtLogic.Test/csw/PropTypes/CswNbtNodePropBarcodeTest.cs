using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.Actions.Receiving;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.PropTypes
{
    [TestFixture]
    public class CswNbtNodePropBarcodeTest
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
            TestData.DeleteTestNodes();
            TestData.RevertNodeTypePropAttributes();
            TestData.Release();
        }

        #endregion

        [Test]
        public void testNonTempNodeCreate()
        {
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );

            string Before = BarcodeNTP.Sequence.getCurrent();

            CswNbtObjClassContainer Container = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainer AsContainer = NewNode;
                    AsContainer.Barcode.setBarcodeValueOverride( "TestVal", false );
                } );

            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.AreEqual( Before, After, "Creating a non-temp Container consumed a barcode sequence value when it shouldn't have" );
        }

        [Test]
        public void testTempNodeCreate()
        {
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );

            string Before = BarcodeNTP.Sequence.getCurrent();

            CswNbtObjClassContainer Container = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId,
                OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainer AsContainer = NewNode;
                    AsContainer.Barcode.setBarcodeValueOverride( "TestVal", false );
                },
                IsTemp : true );

            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.AreEqual( Before, After, "Creating a temp Container consumed a barcode sequence value when it shouldn't have" );
        }

        [Test]
        public void testNonTempNodeCreateNoBarcode()
        {
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );

            string Before = BarcodeNTP.Sequence.getCurrent();

            CswNbtObjClassContainer Container = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId );

            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.IsNotNull( Container.Barcode.Barcode, "Creating a container and not assigning it a custom barcode did not create a barcode when it should have" );
            Assert.AreNotEqual( Before, After, "Creating a container with no custom barcode did not consume a sequence value when it should have" );
        }

        [Test]
        public void testTempNodeCreateNoBarcode()
        {
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );

            string Before = BarcodeNTP.Sequence.getCurrent();

            CswNbtObjClassContainer Container = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContainerNT.NodeTypeId,
                OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassContainer AsContainer = NewNode;
                },
                IsTemp : true );

            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.AreEqual( Before, After, "Creating a temp container with no custom barcode consumed a barcode sequence value when it shouldn't have" );
        }

        [Test]
        public void testReceivingBarcodes()
        {
            CswNbtMetaDataNodeType ChemicalNT = TestData.CswNbtResources.MetaData.getNodeType( "Chemical" );
            CswNbtObjClassChemical ChemicalNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId );

            CswNbtObjClassUnitOfMeasure GramNode = TestData.CswNbtResources.Nodes.GetNode( new CswPrimaryKey( "nodes", 26745 ) );

            CswNbtMetaDataNodeType SizeNT = TestData.CswNbtResources.MetaData.getNodeType( "Size" );
            CswNbtObjClassSize Size = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassSize AsSize = NewNode;
                AsSize.Material.RelatedNodeId = ChemicalNode.NodeId;
                AsSize.UnitCount.Value = 5;
                AsSize.InitialQuantity.Quantity = 10;
                AsSize.InitialQuantity.UnitId = GramNode.NodeId;
            }, OverrideUniqueValidation : true );

            CswNbtActReceiving ReceivingAction = new CswNbtActReceiving( TestData.CswNbtResources, ChemicalNode.NodeId );

            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtObjClassContainer InitialContainer = ReceivingAction.makeContainer( null );
            JObject ContainerAddProps = ReceivingAction.getContainerAddProps( InitialContainer );

            CswCommaDelimitedString Barcodes = new CswCommaDelimitedString();
            Barcodes.Add( "testbarcode1" );
            Barcodes.Add( "testbarcode2" );

            int NumContainers = 2;
            CswNbtAmountsGridQuantity Quantity = new CswNbtAmountsGridQuantity()
            {
                NumContainers = NumContainers,
                Quantity = 10,
                SizeName = Size.NodeName,
                SizeNodeId = Size.NodeId,
                UnitNodeId = GramNode.NodeId,
                ContainerIds = new Collection<string>() { InitialContainer.NodeId.ToString() },
            };
            foreach( string Barcode in Barcodes )
            {
                Quantity.AddBarcode( Barcode );
            }

            CswNbtReceivingDefinition ReceiptDef = new CswNbtReceivingDefinition()
            {
                ContainerNodeId = InitialContainer.NodeId,
                ContainerNodeTypeId = ContainerNT.NodeTypeId,
                ContainerProps = ContainerAddProps,
                MaterialNodeId = ChemicalNode.NodeId,
                Quantities = new Collection<CswNbtAmountsGridQuantity>()
                        {
                            Quantity
                        }
            };

            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );
            string Before = BarcodeNTP.Sequence.getCurrent();
            ReceivingAction.receiveMaterial( ReceiptDef );
            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.AreEqual( Before, After, "Creating containers in Receiving with custom barcodes consumed barcode sequence values when it shouldn't have" );
        }

        [Test]
        public void testReceivingBarcodesNoBarcode()
        {
            CswNbtMetaDataNodeType ChemicalNT = TestData.CswNbtResources.MetaData.getNodeType( "Chemical" );
            CswNbtObjClassChemical ChemicalNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId );

            CswNbtObjClassUnitOfMeasure GramNode = TestData.CswNbtResources.Nodes.GetNode( new CswPrimaryKey( "nodes", 26745 ) );

            CswNbtMetaDataNodeType SizeNT = TestData.CswNbtResources.MetaData.getNodeType( "Size" );
            CswNbtObjClassSize Size = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassSize AsSize = NewNode;
                AsSize.Material.RelatedNodeId = ChemicalNode.NodeId;
                AsSize.UnitCount.Value = 5;
                AsSize.InitialQuantity.Quantity = 10;
                AsSize.InitialQuantity.UnitId = GramNode.NodeId;
            }, OverrideUniqueValidation : true );

            CswNbtActReceiving ReceivingAction = new CswNbtActReceiving( TestData.CswNbtResources, ChemicalNode.NodeId );

            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtObjClassContainer InitialContainer = ReceivingAction.makeContainer( null );
            JObject ContainerAddProps = ReceivingAction.getContainerAddProps( InitialContainer );

            int NumContainers = 2;
            CswNbtReceivingDefinition ReceiptDef = new CswNbtReceivingDefinition()
            {
                ContainerNodeId = InitialContainer.NodeId,
                ContainerNodeTypeId = ContainerNT.NodeTypeId,
                ContainerProps = ContainerAddProps,
                MaterialNodeId = ChemicalNode.NodeId,
                Quantities = new Collection<CswNbtAmountsGridQuantity>()
                        {
                            new CswNbtAmountsGridQuantity()
                                {
                                    NumContainers = NumContainers,
                                    Quantity = 10,
                                    SizeName = Size.NodeName,
                                    SizeNodeId = Size.NodeId,
                                    UnitNodeId = GramNode.NodeId,
                                    ContainerIds = new Collection<string>() { InitialContainer.NodeId.ToString() }
                                }
                        }
            };

            CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );
            string Before = BarcodeNTP.Sequence.getCurrent();
            ReceivingAction.receiveMaterial( ReceiptDef );
            string After = BarcodeNTP.Sequence.getCurrent();

            Assert.AreNotEqual( Before, After, "Creating containers in receiving with no custom barcodes specified did not consume sequence values when it should have" );
        }

    }
}
