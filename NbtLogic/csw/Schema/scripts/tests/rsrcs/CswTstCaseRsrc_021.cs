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
    public class CswTstCaseRsrc_021
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        public CswTstCaseRsrc_021( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor


        public string Purpose = "Basic audit mechanism";

        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string ArbitraryColumnName_03 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn03 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 10 ); } }



        public void makeArbitraryTable()
        {
            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, ( ArbitraryTableName_01 + "id" ).Replace( "_", "" ) );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_02, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_03, ArbitraryColumnName_03, false, false, 20 );

       }


        private Dictionary<string, List<string>> __ArbitraryTestValues;
        private Dictionary<string, List<string>> _ArbitraryTestValues
        {
            get
            {
                if( null == __ArbitraryTestValues )
                {
                    __ArbitraryTestValues = _CswTestCaseRsrc.makeArbitraryTestValues( 20 );
                }

                return ( __ArbitraryTestValues );

            }//get

        }//_ArbitraryTestValues


        public void makeArbitraryTableData()
        {
            _CswTestCaseRsrc.fillTableWithArbitraryData( ArbitraryTableName_01, _ArbitraryTestValues );
        }

        public void dropArbitraryTables()
        {
            _CswNbtSchemaModTrnsctn.dropTable( ArbitraryTableName_01 );
        }

        public void verifyTablesDropped()
        {
            _CswTestCaseRsrc.assertTableIsAbsent( ArbitraryTableName_01 );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
