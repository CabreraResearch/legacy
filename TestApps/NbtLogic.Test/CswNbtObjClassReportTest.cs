using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChemSW.Nbt.Test
{
    [TestClass]
    public class CswNbtObjClassReportTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            //TestData.DeleteTestNodes();
        }

        #endregion

        #region ExtractReportParams

        /// <summary>
        /// Given a parameterized SQL string and a UserId,
        /// assert that the returned SQL string contains the UserId
        /// </summary>
        [TestMethod]
        public void setReportParams()
        {
            string Sql = "select u.* from ocuserclass u where u.nodeid = {userid}";
            CswPrimaryKey UserId = new CswPrimaryKey("nodes", 2);
            CswNbtObjClassUser UserNode = TestData.CswNbtResources.Nodes.GetNode(UserId);
            CswNbtMetaDataNodeType ReportNT = TestData.CswNbtResources.MetaData.getNodeType( "Report" );
            CswNbtObjClassReport ReportNode = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            ReportNode.SQL.Text = Sql;
            string ReportSql = CswNbtObjClassReport.ReplaceReportParams( Sql, ReportNode.ExtractReportParams( UserNode ) );
            Assert.AreEqual( "select u.* from ocuserclass u where u.nodeid = 2", ReportSql );
        }

        #endregion

    }
}
