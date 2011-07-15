using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Text;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_027
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        public Process Process = null;
        public CswTstCaseRsrc_027( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {

            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            Process = System.Diagnostics.Process.GetCurrentProcess();
        }//ctor



        public string Purpose = "Memory consumption";


        public long ProcessMemoryAtStart = Int32.MinValue;
        public long ProcessMemoryAtEnd = Int32.MinValue;
        public string ProcessMemoryDelta { get { return ( ( ProcessMemoryAtEnd - ProcessMemoryAtStart ).ToString( "N" ) ); } }
        public string TotalProcessMemory { get { return ( Process.PrivateMemorySize64.ToString( "N" ) ); } }

        public long GCMemoryAtStart = Int32.MinValue;
        public long GCMemoryAtEnd = Int32.MinValue;
        public string GCMemoryDelta { get { return ( ( GCMemoryAtEnd - GCMemoryAtStart ).ToString( "N" ) ); } }
        public string TotalGCMemorySansCollection { get { return ( GC.GetTotalMemory( false ).ToString( "N" ) ); } }



        public void memoryTestBegin()
        {
            GCMemoryAtStart = System.GC.GetTotalMemory( true );//forcing finalization and colleciton
            ProcessMemoryAtStart = Process.PrivateMemorySize64;
        }//memoryTestBegin()

        public void memoryTestEnd()
        {
            GCMemoryAtEnd = System.GC.GetTotalMemory( true );//forcing finalization and colleciton
            ProcessMemoryAtEnd = Process.PrivateMemorySize64;
        }//memoryTestEnd()


        public string ArbitraryTableName_01 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string ArbitraryTableName_02 { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable02 ) ); } }

        public string ArbitraryColumnName_01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string ArbitraryColumnName_02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }

        public string ArbitraryColumnValue { get { return ( "snot" ); } }

        public Int32 TotalTestRows { get { return ( 100 ); } }



        public void makeArbitraryTables()
        {

            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_01, CswTools.makePkColNameFromTableName( ArbitraryTableName_01 + "id" ) );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_01, ArbitraryColumnName_01, false, false, 20 );
            _CswNbtSchemaModTrnsctn.addStringColumn( ArbitraryTableName_01, ArbitraryColumnName_02, ArbitraryColumnName_01, false, false, 20 );

            _CswNbtSchemaModTrnsctn.addTable( ArbitraryTableName_02, CswTools.makePkColNameFromTableName( ArbitraryTableName_01 + "id" ) );
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
