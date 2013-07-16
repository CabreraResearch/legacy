using ChemSW.RscAdo;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Ado
{
    [TestFixture]
    public class CswSqlAnalysis_UnitTests
    {
        [Test]
        public void DoesSqlContainDmlOrDdlTest()
        {
            string[] ValidSql =
                {
                    "select * from nodes", 
                    "select count(*) from jct_nodes_props", 
                    "select * from (select nodetypeid from nodetypes)",
                    "select * from nodes where pendingupdate = '1'"
                };
            string[] InvalidSql =
                {
                    "truncate table nodes", 
                    "alter user nbt identified by nbt account unlock", 
                    "delete from object_class_props where objectclassid > 1",
                    "update table nodes set nodename = 'node'",
                    "lock(nodes)"
                };

            foreach( string StringToValidate in ValidSql )
            {
                Assert.IsFalse( CswSqlAnalysis.doesSqlContainDmlOrDdl( StringToValidate ), "The following SQL was incorrectly considered invalid:\n\n" + StringToValidate + "\n" );
            }
            foreach( string StringToValidate in InvalidSql )
            {
                Assert.IsTrue( CswSqlAnalysis.doesSqlContainDmlOrDdl( StringToValidate ), "The following SQL was incorrectly considered valid:\n\n" + StringToValidate + "\n");
            }
        }
    }
}
