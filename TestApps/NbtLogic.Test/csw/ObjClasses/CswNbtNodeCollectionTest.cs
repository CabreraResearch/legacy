using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.ObjClasses
{
    [TestFixture]
    public class CswNbtNodeCollectionTest
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

        #region makeNodeFromNodeTypeId

        /// <summary>
        /// Given a NodeTypeId,
        /// when making a real node,
        /// assert that the node is temp when first created, but not temp afterward
        /// </summary>
        [Test]
        public void makeNodeFromNodeTypeIdTestTempStateRemoved()
        {
            CswNbtMetaDataObjectClass ControlZoneOC = TestData.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtNode ControlZoneNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ControlZoneOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                Assert.IsTrue( NewNode.IsTemp );
            } );
            Assert.IsFalse( ControlZoneNode.IsTemp );
        }

        /// <summary>
        /// Given a NodeTypeId,
        /// when making a temp node,
        /// assert that the node is still temp afterward
        /// </summary>
        [Test]
        public void makeNodeFromNodeTypeIdTestTempStateRemains()
        {
            CswNbtMetaDataObjectClass ControlZoneOC = TestData.CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            CswNbtNode ControlZoneNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ControlZoneOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                Assert.IsTrue( NewNode.IsTemp );
            }, true );
            Assert.IsTrue( ControlZoneNode.IsTemp );
        }

        #endregion makeNodeFromNodeTypeId
    }
}
