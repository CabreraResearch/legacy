using System;
using ChemSW;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.csw.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NbtLogic.Test
{
    [TestClass]
    public class CswNbtActDispenseContainerTest
    {
        #region Setup and Teardown

        private CswNbtResources _CswNbtResources = null;
        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private TestData TestData = null;
        private string UserName = "TestUser";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswNbtResources.InitCurrentUser = InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;
            TestData = new TestData( _CswNbtResources );
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, UserName );
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
            string SourceContainerId = "nodeid_99999";
            string InvalidDispenseType = "Receive";
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources, SourceContainerId );
            try
            {
                JObject obj = wiz.updateDispensedContainer( InvalidDispenseType, "5 gal" );
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
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            JObject obj = wiz.updateDispensedContainer( DispenseType, ".5 Liters" );
            CswNbtNode UpdatedContainerNode = _CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }

        [TestMethod]
        public void updateDispensedContainerTestWaste()
        {
            double Expected = 0.5;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 1.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Waste";
            JObject obj = wiz.updateDispensedContainer( DispenseType, ".5 Liters" );
            CswNbtNode UpdatedContainerNode = _CswNbtResources.Nodes.GetNode( ContainerNode.NodeId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            double Actual = NodeAsContianer.Quantity.Quantity;
            Assert.AreEqual( Expected, Actual );
        }
    }
}
