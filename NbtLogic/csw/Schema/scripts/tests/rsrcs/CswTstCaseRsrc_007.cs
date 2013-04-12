using System.Collections.Generic;
//using ChemSW.RscAdo;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_007
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


        public static string Purpose = "Data Dictionary, Add two columns";

        public List<PkFkPair> getPkFkPairs() { return ( _CswTestCaseRsrc.getPkFkPairs( 6 ) ); }


        public void makePkFkTables()
        {
            List<PkFkPair> PkFkPairs = getPkFkPairs();
            foreach( PkFkPair CurrentPair in PkFkPairs )
            {
                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.PkTableName, CurrentPair.PkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), CswEnumDataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.String, false, false, CurrentPair.PkTableName, CswEnumDataDictionaryUniqueType.None, false, string.Empty );

                _CswNbtSchemaModTrnsctn.addTable( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName );
                _CswNbtSchemaModTrnsctn.addColumn( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ), CswEnumDataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.String, false, false, CurrentPair.FkTableName, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
                _CswNbtSchemaModTrnsctn.addColumn( CurrentPair.PkTablePkColumnName, CswEnumDataDictionaryColumnType.Fk, 20, 0, "foo", "test column", CurrentPair.PkTablePkColumnName, CurrentPair.PkTableName, true, false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Long, false, false, CurrentPair.FkTableName, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
                

            }
        }


        public void dropPkFkTables()
        {
            //Clean up after ourselves (will verify in next script)
            List<PkFkPair> PkFkPairs = getPkFkPairs();
            foreach( PkFkPair CurrentPair in PkFkPairs )
            {
                _CswNbtSchemaModTrnsctn.dropTable( CurrentPair.PkTableName );
                _CswNbtSchemaModTrnsctn.dropTable( CurrentPair.FkTableName );
            }
        }


    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
