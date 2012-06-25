using System;
using System.Collections.Generic;
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

namespace CswNbtActions.Test
{
    [TestClass]
    public class CswNbtActDispenseContainerTest
    {
        #region Setup and Teardown

        private CswNbtResources _CswNbtResources = null;
        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private List<CswPrimaryKey> TestNodeIds = new List<CswPrimaryKey>();
        private string UserName = "TestUser";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswNbtResources.InitCurrentUser = InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;

            //TODO - get latest nodePK and set it as HWM
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, UserName );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            //TODO - get all nodes > HWM
            foreach( CswPrimaryKey NodeId in TestNodeIds )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId );
                Node.delete();
            }
        }

        #endregion

        [TestMethod]
        public void updateDispensedContainerTestInvalidDispenseType()
        {
            string InvalidDispenseType = "Receive";
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( _CswNbtResources );
            try
            {
                JObject obj = wiz.updateDispensedContainer( "nodeid_99999", InvalidDispenseType, "5 gal" );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( InvalidDispenseType, "Add" );
                Assert.AreNotEqual( InvalidDispenseType, "Waste" );
            }
        }
    }
}
