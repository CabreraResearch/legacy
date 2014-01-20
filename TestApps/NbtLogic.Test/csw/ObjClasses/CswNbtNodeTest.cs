using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.ObjClasses
{
    [TestFixture]
    public class CswNbtNodeTest
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

        #region isFavorite

        /// <summary>
        /// Given a newly created node and a userid,
        /// assert that the node is not listed as a favorite for that userid
        /// </summary>
        [Test]
        public void isFavoriteTestReturnsFalse()
        {
            CswNbtNode TestNode = TestData.Nodes.createControlZoneNode();
            Int32 UserId = 1;
            Assert.IsFalse( TestNode.isFavorite( UserId ) );
        }

        /// <summary>
        /// Given a newly created node and a userid with a favorites record linking them,
        /// assert that the node is listed as a favorite for that userid
        /// </summary>
        [Test]
        public void isFavoriteTestReturnsTrue()
        {
            CswNbtNode TestNode = TestData.Nodes.createControlZoneNode();
            Int32 UserId = 1;
            CswTableUpdate FavoritesUpdate = TestData.CswNbtResources.makeCswTableUpdate( "favoritesUpdate", "favorites" );
            DataTable FavoritesTable = FavoritesUpdate.getEmptyTable();
            DataRow FavoritesRow = FavoritesTable.NewRow();
            FavoritesRow["userid"] = UserId;
            FavoritesRow["itemid"] = TestNode.NodeId.PrimaryKey;
            FavoritesTable.Rows.Add( FavoritesRow );
            FavoritesUpdate.update( FavoritesTable );
            Assert.IsTrue( TestNode.isFavorite( UserId ) );
        }

        #endregion isFavorite

        #region postChanges

        /// <summary>
        /// Given a newly created node,
        /// assert that the node's state is unchanged
        /// </summary>
        [Test]
        public void postChangesTestStateModified()
        {
            CswNbtNode TestNode = TestData.Nodes.createControlZoneNode();
            //TestNode.postChanges( false );
            Assert.AreEqual( CswEnumNbtNodeModificationState.Modified, TestNode.ModificationState.ToString() );
        }

        #endregion postChanges

        #region promoteTempToReal

        /// <summary>
        /// Given a newly created temp node,
        /// when the temp node is promoted,
        /// assert that the node is no longer temp
        /// </summary>
        [Test]
        public void promoteTempToRealTestTempStateRemoved()
        {
            CswNbtNode TestNode = TestData.Nodes.createTempNode();
            Assert.IsTrue( TestNode.IsTemp );
            TestNode.PromoteTempToReal();
            Assert.IsFalse( TestNode.IsTemp );
        }

        /// <summary>
        /// Given a newly created temp node with compoundunique properties
        /// given that a pre-existing node has the same compoundunique values,
        /// when the temp node is promoted to real,
        /// assert that a counpoununique violation error is thrown.
        /// Prior to resolving Case 31214, this test failed.
        /// </summary>
        [Test]
        public void promoteTempToRealTestCompoundUniqueViolation()
        {
            TestData.Nodes.createVendorNode();
            CswNbtNode InvalidVendor = TestData.Nodes.createVendorNode( true );
            Assert.IsTrue( InvalidVendor.IsTemp );
            bool posted = false;
            try
            {
                InvalidVendor.PromoteTempToReal();
                posted = true;
            }
            catch( Exception e )
            {
                Assert.IsTrue( e.Message.Contains( "The following properties must have unique values" ) );
            }

            Assert.IsFalse( posted, "No Compound-Unique validation occurred." );
        }

        #endregion promoteTempToReal
    }
}
