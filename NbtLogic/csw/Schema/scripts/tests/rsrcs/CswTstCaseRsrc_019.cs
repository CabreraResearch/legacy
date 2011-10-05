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
    public class CswTstCaseRsrc_019
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


        public static string Purpose = "DML an object that was DDLed in the same script";

        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string ArbitraryTableName_02 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable02 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 10 ); } }



        public void makeArbitraryTables()
        {
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, ArbitraryTableName_01 + "id" );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_01, false, false, 20 );

            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_02, ArbitraryTableName_02 + "id" );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_02, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_02, ArbitraryColumnName_02, ArbitraryColumnName_01, false, false, 20 );

        }

        public void makeArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, ArbitraryColumnName_01, TotalTestRows, ArbitraryColumnValue );
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_02, ArbitraryColumnName_01, TotalTestRows, ArbitraryColumnValue );
        }

        public void dropArbitraryTables()
        {
            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName_01 );
            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName_02 );
        }

        public void verifyTablesDropped()
        {
            _CswTestCaseRsrc.assertTableIsAbsent( ArbitraryTableName_01 );
            _CswTestCaseRsrc.assertTableIsAbsent( ArbitraryTableName_02 );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
