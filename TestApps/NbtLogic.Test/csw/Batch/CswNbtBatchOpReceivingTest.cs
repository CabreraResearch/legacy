using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Test;
using NbtWebApp.Actions.Receiving;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NbtLogic.Test.csw.Batch
{
    [TestFixture]
    public class CswNbtBatchOpReceivingTest
    {

        private TestData TestData;
        private CswNbtActReceiving _ReceivingAction;

        private CswNbtObjClassSize _SizeOne;
        private CswNbtObjClassSize _SizeTwo;
        private CswNbtObjClassChemical _FirstChemicalNode;
        
        #region Setup and Teardown

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();

            CswNbtMetaDataNodeType ChemicalNT = TestData.CswNbtResources.MetaData.getNodeType( "Chemical" );
            _FirstChemicalNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId );
            _ReceivingAction = new CswNbtActReceiving( TestData.CswNbtResources, _FirstChemicalNode.NodeId );

            CswNbtObjClassUnitOfMeasure GramNode = TestData.CswNbtResources.Nodes.GetNode( new CswPrimaryKey( "nodes", 26745 ) );

            CswNbtMetaDataNodeType SizeNT = TestData.CswNbtResources.MetaData.getNodeType( "Size" );
            _SizeOne = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassSize AsSize = NewNode;
                    AsSize.Material.RelatedNodeId = _FirstChemicalNode.NodeId;
                    AsSize.UnitCount.Value = 5;
                    AsSize.InitialQuantity.Quantity = 10;
                    AsSize.InitialQuantity.UnitId = GramNode.NodeId;
                } );

            _SizeTwo = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassSize AsSize = NewNode;
                    AsSize.Material.RelatedNodeId = _FirstChemicalNode.NodeId;
                    AsSize.UnitCount.Value = 10;
                    AsSize.InitialQuantity.Quantity = 100;
                    AsSize.InitialQuantity.UnitId = GramNode.NodeId;
                } );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        /// <summary>
        /// Simulates clicking "Finish" on the Receiving Wizard
        /// </summary>
        private CswNbtReceivingDefiniton FinishWizard()
        {
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            CswNbtObjClassContainer InitialContainer = _ReceivingAction.makeContainer( null );
            JObject ContainerAddProps = _ReceivingAction.getContainerAddProps( InitialContainer );

            CswNbtObjClassUnitOfMeasure GramNode = TestData.CswNbtResources.Nodes.GetNode( new CswPrimaryKey( "nodes", 26745 ) ); //TODO: don't hardcode this Gram

            //CREATES 16 CONTAINERS
            CswNbtReceivingDefiniton ReceiptDef = new CswNbtReceivingDefiniton()
            {
                ContainerNodeId = InitialContainer.NodeId,
                ContainerNodeTypeId = ContainerNT.NodeTypeId,
                ContainerProps = ContainerAddProps,
                MaterialNodeId = _FirstChemicalNode.NodeId,
                Quantities = new Collection<CswNbtAmountsGridQuantity>()
                        {
                            new CswNbtAmountsGridQuantity()
                                {
                                    NumContainers = 11,
                                    Quantity = 10,
                                    SizeName = _SizeOne.NodeName,
                                    SizeNodeId = _SizeOne.NodeId,
                                    UnitNodeId = GramNode.NodeId
                                },
                            new CswNbtAmountsGridQuantity()
                                {
                                    NumContainers = 5,
                                    Quantity = 100,
                                    SizeName = _SizeTwo.NodeName,
                                    SizeNodeId = _SizeTwo.NodeId,
                                    UnitNodeId = GramNode.NodeId
                                }
                        }
            };

            //This should create out receipt lot and finalize the initial container
            _ReceivingAction.HandleInitialContainer( InitialContainer, ReceiptDef );

            return ReceiptDef;
        }

        [Test]
        public void TestInitialContainerCreate()
        {
            CswNbtReceivingDefiniton ReceiptDef = FinishWizard();
            CswNbtObjClassContainer InitialContainer = TestData.CswNbtResources.Nodes.GetNode( ReceiptDef.ContainerNodeId );
            Assert.IsFalse( InitialContainer.IsTemp );

            Assert.AreEqual( InitialContainer.Quantity.Quantity, 10 ); //Did we get the right quantity?
            Assert.AreEqual( InitialContainer.Quantity.CachedNodeName, _SizeOne.InitialQuantity.CachedNodeName );
        }

        [Test]
        public void TestContainerCreation()
        {
            CswNbtReceivingDefiniton ReceiptDef = FinishWizard(); //This method is testing by the "TestInitialContainerCreate" test

            CswNbtBatchOpReceiving ReceivingBatchOp = new CswNbtBatchOpReceiving( TestData.CswNbtResources );
            ReceivingBatchOp.OverrideMaxProcessed( 10 );
            CswNbtObjClassBatchOp Op = ReceivingBatchOp.makeBatchOp( ReceiptDef );

            double PercentDone = 0;
            int NumInterations = 0; //Just in case
            while( PercentDone != 100.0 ) //there should never be more than 2 iteration if we're doing 10 containers at a time and we have 16 containers to do
            {
                ReceivingBatchOp.runBatchOp( Op );
                PercentDone = ReceivingBatchOp.getPercentDone( Op );

                NumInterations++; //If we get to three, we exit and report a failure
                if( 3 == NumInterations )
                {
                    break; //Prevent infinate loops
                }
            }

            ReceiptDef = ReceivingBatchOp.GetBatchData( Op );
            Assert.AreEqual( 2, NumInterations, "runBatchOp() ran more than two times when it shouldn have only run twice" );

            int NumContainerIds = 0;
            foreach( CswNbtAmountsGridQuantity Quant in ReceiptDef.Quantities )
            {
                NumContainerIds += Quant.ContainerIds.Count;
            }

            int NumContainers = 0;
            foreach( CswNbtAmountsGridQuantity Quant in ReceiptDef.Quantities )
            {
                NumContainers += Quant.NumContainers;
            }
            Assert.AreEqual( NumContainers, NumContainerIds, "The batch op created " + NumContainerIds + " containers when it expected " + NumContainers );
        }
    }
}
