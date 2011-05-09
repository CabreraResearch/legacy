using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_006
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_006( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Rollback drop of fk constraint";

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

        //public string RealTestTableName { get { return ( _CswTestCaseRsrc.getRealTestTableName( TestTableNamesReal.Nodes ) ); } }
        //public string RealTestColumnName { get { return ( _CswTestCaseRsrc.getRealTestColumnName( TestColumnNamesReal.NodeName ) ); } }

        //public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        //public string FakeTestColumnName { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }

        //public void testAddColumnValues( TestColumnNamesFake TestColumnName ) { _CswTestCaseRsrc.testAddColumnValues( TestTableNamesReal.DataDictionary, TestColumnName ); }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
