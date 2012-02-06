using System.Collections.Generic;
//using ChemSW.RscAdo;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_005
    {

		private CswTestCaseRsrc _CswTestCaseRsrc;
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
		{
			set
			{
				_CswNbtSchemaModTrnsctn = value;
				_CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			}
		}


        public static string Purpose = "Rollback Add of Constrained Columns";

        public List<PkFkPair> getPkFkPairs() { return ( _CswTestCaseRsrc.getPkFkPairs( 6 ) ); }


        public void makePkFkTables()
        {
            List<PkFkPair> PkFkPairs = getPkFkPairs();
            foreach( PkFkPair CurrentPair in PkFkPairs )
            {
                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.PkTableName, CurrentPair.PkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, CurrentPair.PkTableName, DataDictionaryUniqueType.None, false, string.Empty );

                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, CurrentPair.FkTableName, DataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( CurrentPair.PkTablePkColumnName, DataDictionaryColumnType.Fk, 20, 0, "foo", "test column", CurrentPair.PkTablePkColumnName, CurrentPair.PkTableName, true, false, false, string.Empty, false, DataDictionaryPortableDataType.Long, false, false, CurrentPair.FkTableName, DataDictionaryUniqueType.None, false, string.Empty );

            }
        }

        //public string RealTestTableName { get { return ( _CswTestCaseRsrc.getRealTestTableName( TestTableNamesReal.Nodes ) ); } }
        //public string RealTestColumnName { get { return ( _CswTestCaseRsrc.getRealTestColumnName( TestColumnNamesReal.NodeName ) ); } }

        //public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        //public string FakeTestColumnName { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }

        //public void testAddColumnValues( TestColumnNamesFake TestColumnName ) { _CswTestCaseRsrc.testAddColumnValues( TestTableNamesReal.DataDictionary, TestColumnName ); }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
