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

    public enum TestTableNamesFake { TestTable01, TestTable02 }
    public enum TestTableNamesReal { Modules, DataDictionary, Nodes }
    public enum TestColumnNamesFake { TestColumn01, TestColumn02 }
    public enum TestColumnNamesReal { NodeName }
    public enum TestNameStem { PrimeKeyTable, ForeignKeyTable, TestVal }


    public struct PkFkPair
    {
        public string PkTableName;
        public string PkTablePkColumnName;
        public string FkTableName;
        public string FkTablePkColumnName;
    }//


    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTestCaseRsrc
    {
        private Dictionary<TestTableNamesReal, string> _TestTableNamesReal = new Dictionary<TestTableNamesReal, string>();
        private Dictionary<TestTableNamesFake, string> _TestTableNamesFake = new Dictionary<TestTableNamesFake, string>();
        private Dictionary<TestColumnNamesFake, string> _TestColumnNamesFake = new Dictionary<TestColumnNamesFake, string>();
        private Dictionary<TestColumnNamesReal, string> _TestColumnNamesReal = new Dictionary<TestColumnNamesReal, string>();
        private Dictionary<TestNameStem, string> _TestNameStems = new Dictionary<TestNameStem, string>();


        private string _ForeignKeyTableStem = "fk_Table_";
        private string _PrimeKeyTableStem = "pk_Table_";

        private string _ArbitraryValCol = "arbitraryvalue";
        private string _TestValStem = "Test val ";

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        public CswTestCaseRsrc( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;


            _TestTableNamesReal.Add( TestTableNamesReal.Modules, "modules" );
            _TestTableNamesReal.Add( TestTableNamesReal.DataDictionary, "data_dictionary" );
            _TestTableNamesReal.Add( TestTableNamesReal.Nodes, "nodes" );


            _TestColumnNamesFake.Add( TestColumnNamesFake.TestColumn01, "test_column_one" );
            _TestColumnNamesFake.Add( TestColumnNamesFake.TestColumn02, "test_column_two" );

            _TestColumnNamesReal.Add( TestColumnNamesReal.NodeName, "nodename" );


            _TestTableNamesFake.Add( TestTableNamesFake.TestTable01, "test_table_01" );
            _TestTableNamesFake.Add( TestTableNamesFake.TestTable02, "test_table_02" );

            _TestNameStems.Add( TestNameStem.ForeignKeyTable, "fk_Table_" );
            _TestNameStems.Add( TestNameStem.PrimeKeyTable, "pk_Table_" );
            _TestNameStems.Add( TestNameStem.TestVal, "Test val " );

        }//ctor

        public string getFakeTestTableName( TestTableNamesFake TestTableName ) { return ( _TestTableNamesFake[TestTableName] ); }
        public string getRealTestTableName( TestTableNamesReal TestTableName ) { return ( _TestTableNamesReal[TestTableName] ); }
        public string getFakeTestColumnName( TestColumnNamesFake TestColumnName ) { return ( _TestColumnNamesFake[TestColumnName] ); }
        public string getRealTestColumnName( TestColumnNamesReal TestColumnName ) { return ( _TestColumnNamesReal[TestColumnName] ); }
        public string getTestNameStem( TestNameStem TestNameStem ) { return ( _TestNameStems[TestNameStem] ); }


        public void testAddColumnValues( TestTableNamesReal TestTableName, TestColumnNamesFake TestColumnName )
        {
            Int32 TotalUpdated = 0;
            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "testAddColumnValues_update", _TestTableNamesReal[TestTableName] );
            DataTable TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[_TestColumnNamesFake[TestColumnName]] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                if( ( getTestNameStem( TestNameStem.TestVal ) + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[_TestColumnNamesFake[TestColumnName]].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + _TestColumnNamesFake[TestColumnName] + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );


        }//_testAddColumnValues() 

        public string makeTestCaseDescription( string RawClassName, string TestCasePurpose, string SubPurpose )
        {
            return ( "Test Case " + RawClassName.Substring( 12 ) + ": " + TestCasePurpose + "--" + SubPurpose );
        }


        public List<PkFkPair> getPkFkPairs( Int32 Size )
        {

            List<PkFkPair> ReturnVal = new List<PkFkPair>();

            for( Int32 idx = 0; idx < Size; idx++ )
            {
                PkFkPair CurrentPair = new PkFkPair();

                CurrentPair.PkTableName = getTestNameStem( TestNameStem.PrimeKeyTable ) + idx.ToString();
                CurrentPair.PkTablePkColumnName = CurrentPair.PkTableName + "id";
                CurrentPair.FkTableName = getTestNameStem( TestNameStem.ForeignKeyTable ) + idx.ToString();
                CurrentPair.FkTablePkColumnName = CurrentPair.FkTableName + "id";

                ReturnVal.Add( CurrentPair );
            }

            return ( ReturnVal );

        }//

        public void fillTableWithArbitraryData( string TableName, string ColumnName, Int32 TotalRows )
        {
            Int32 ArbitraryValue = 0;
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fillTableWithArbitraryData_update", TableName );
            DataTable PkTableTable = CswTableUpdate.getTable();
            for( Int32 idx = 0; idx < TotalRows; idx++ )
            {
                DataRow NewRow = PkTableTable.NewRow();
                NewRow[ColumnName] = getTestNameStem( TestNameStem.TestVal ) + ":" + ( +ArbitraryValue ).ToString();
                PkTableTable.Rows.Add( NewRow );
            }
            CswTableUpdate.update( PkTableTable );

        }//fillTableWithArbitraryData()

        public void addArbitraryForeignKeyRecords( string PkTable, string FkTable, string ReferenceColumnName, string FkTableArbitraryValueColumnName, string FkTableValueStem )
        {

            CswTableSelect PkTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "addArbitraryForeignKeyRecords_pktable_select", PkTable );
            DataTable PkTableTable = PkTableSelect.getTable();


            CswTableUpdate FkTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "addArbitraryForeignKeyRecords_fktable_update", FkTable );
            DataTable FkTableTable = FkTableUpdate.getTable();
            Int32 ArbitraryValue = 0;
            foreach( DataRow CurrentRow in PkTableTable.Rows )
            {
                Int32 PkTablePk = CswConvert.ToInt32( CurrentRow[ReferenceColumnName] );

                DataRow NewFkTableRow = FkTableTable.NewRow();
                NewFkTableRow[ReferenceColumnName] = PkTablePk;
                NewFkTableRow[FkTableArbitraryValueColumnName] = FkTableValueStem + ": " + ( ++ArbitraryValue ).ToString();
                FkTableTable.Rows.Add( NewFkTableRow );
            }

            FkTableUpdate.update( FkTableTable );

        }//addArbitraryForeignKeyRecords()

        public bool isExceptionRecordDeletionConstraintViolation( Exception Exception )
        {
            //This is fine for oracle but will need to be pymorphed when we support sql server
            return (
                Exception.Message.Contains( "integrity constraint" ) &&
                 Exception.Message.Contains( "child record found" )
             );
        }//isRecordDeletionConstraintViolation()

        public bool isExceptionTableDropConstraintViolation( Exception Exception )
        {
            //This is fine for oracle but will need to be pymorphed when we support sql server
            return ( Exception.Message.Contains( "keys in table referenced by foreign keys" ) );
        }//isRecordDeletionConstraintViolation()

        public void assertColumnIsAbsent( string TableName, string ColumnName )
        {
            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( TableName, ColumnName) )
                throw ( new CswDniException( "Column " + ColumnName+ " was not removed from data base" ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( TableName, ColumnName) )
                throw ( new CswDniException( "Column " + ColumnName+ " was not removed from the data base" ) );

        }//assertColumnIsAbsent() 

        public void assertTableIsAbsent( string TableName )
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TableName ) )
                throw ( new CswDniException( "Table " + TableName + " was not dropped from the database" ) );

        }//assertTableIsAbsent() 


    }

}//ChemSW.Nbt.Schema
