using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Test;
using ChemSW.Nbt.WebServices;
using NUnit.Framework;

namespace NbtWebApp.Test.WebServices
{
    [TestFixture]
    public class CswNbtWebServiceNodeTest
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

        /// <summary>
        /// Given a NodeId and a UserId, given that the nodeid is not favorited by the user,
        /// when toggleFavorite is called with the nodeid and userid,
        /// assert that a new row has been added to the favorites table.
        /// </summary>
        [Test]
        public void toggleFavoriteTestAdd()
        {
            Int32 NodeId = 32767;
            Int32 UserId = 1;

            CswNbtWebServiceNode.toggleFavorite( TestData.CswNbtResources, NodeId, UserId );
            DataTable FavoritesTable = TestData.CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "getFavorite", "select count(*) as favoriteExists from favorites where itemid = " + NodeId + " and UserId = " + UserId );
            Assert.AreEqual( 1, CswConvert.ToInt32( FavoritesTable.Rows[0]["favoriteExists"] ) );
        }

        /// <summary>
        /// Given a NodeId and a UserId, given that the nodeid is favorited by the user,
        /// when toggleFavorite is called with the nodeid and userid,
        /// assert that the existing row has been deleted from the favorites table.
        /// </summary>
        [Test]
        public void toggleFavoriteTestDelete()
        {
            Int32 NodeId = 32767;
            Int32 UserId = 1;
            CswNbtWebServiceNode.toggleFavorite( TestData.CswNbtResources, NodeId, UserId );

            CswNbtWebServiceNode.toggleFavorite( TestData.CswNbtResources, NodeId, UserId );
            DataTable FavoritesTable = TestData.CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "getFavorite", "select count(*) as favoriteExists from favorites where itemid = " + NodeId + " and UserId = " + UserId );
            Assert.AreEqual( 0, CswConvert.ToInt32( FavoritesTable.Rows[0]["favoriteExists"] ) );
        }
    }
}
