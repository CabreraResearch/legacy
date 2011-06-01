using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
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
    public enum TestColumnNamesFake { TestColumn01, TestColumn02, TestColumn03 }
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
            _TestColumnNamesFake.Add( TestColumnNamesFake.TestColumn03, "test_column_three" );

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

        public void fillTableWithArbitraryData( string TableName, string ColumnName, Int32 TotalRows, string Value = "" )
        {
            Int32 ArbitraryValue = 0;
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fillTableWithArbitraryData_update", TableName );
            DataTable DataTable = CswTableUpdate.getTable();
            for( Int32 idx = 0; idx < TotalRows; idx++ )
            {
                DataRow NewRow = DataTable.NewRow();

                if( "" != Value )
                {
                    NewRow[ColumnName] = Value;
                }
                else
                {
                    NewRow[ColumnName] = getTestNameStem( TestNameStem.TestVal ) + ":" + ( +ArbitraryValue ).ToString();
                }

                DataTable.Rows.Add( NewRow );
            }

            CswTableUpdate.update( DataTable );

        }//fillTableWithArbitraryData()



        public Dictionary<string, List<string>> makeArbitraryTestValues( Int32 TotalRows )
        {

            Dictionary<string, List<string>> ReturnVal = new Dictionary<string, List<string>>();

            foreach( TestColumnNamesFake CurrentFakeColumnId in Enum.GetValues( typeof( TestColumnNamesFake ) ) )
            {
                List<string> CurrentValueList = new List<string>();
                string CurrentFakeColumnName = _TestColumnNamesFake[CurrentFakeColumnId];
                ReturnVal.Add( CurrentFakeColumnName, CurrentValueList );
                for( int idx = 0; idx < TotalRows; idx++ )
                {
                    CurrentValueList.Add( idx.ToString() + "_valsnot_" + CurrentFakeColumnName );

                }//iterate test values

            }//iterate fake column name enumeration


            return ( ReturnVal );

        }//makeArbitraryTestValues() 


        public bool doTableValuesMatch( DataTable DataTable_1, DataTable DataTable_2, IEnumerable CompareColumns, ref string MisMatchReason )
        {
            bool ReturnVal = true;

            if( DataTable_1.Rows.Count == DataTable_2.Rows.Count )
            {

                for( Int32 rowidx = 0; ( rowidx < DataTable_1.Rows.Count ) && ( true == ReturnVal ); rowidx++ )
                {
                    DataRow CurrentRowInTable_1 = DataTable_1.Rows[rowidx];
                    //for( Int32 columnidx = 0; ( columnidx < CompareColumns.Count ) && ( true == ReturnVal ); columnidx++ )
                    foreach( string DataTable_1_ColumnName in CompareColumns )
                    {
                        //string DataTable_1_ColumnName = CompareColumns[columnidx];
                        if( DataTable_2.Columns.Contains( DataTable_1_ColumnName ) )
                        {
                            if( ( false == CurrentRowInTable_1.IsNull( DataTable_1_ColumnName ) && ( false == DataTable_2.Rows[rowidx].IsNull( DataTable_1_ColumnName ) ) ) )
                            {
                                if( CurrentRowInTable_1[DataTable_1_ColumnName].ToString() != DataTable_2.Rows[rowidx][DataTable_1_ColumnName].ToString() )
                                {
                                    ReturnVal = false;
                                    MisMatchReason = "The ToString()'ed (sic.) value of column " + DataTable_1_ColumnName + " at row index " + rowidx.ToString() + " differs between DataTable_1 and DataTable_1: " + CurrentRowInTable_1[DataTable_1_ColumnName].ToString() + " and " + DataTable_2.Rows[rowidx][DataTable_1_ColumnName].ToString() + " (respectively)";
                                }
                            }
                            else if( CurrentRowInTable_1.IsNull( DataTable_1_ColumnName ) != DataTable_2.Rows[rowidx].IsNull( DataTable_1_ColumnName ) )
                            {
                                ReturnVal = false;
                                MisMatchReason = "The value of column " + DataTable_1_ColumnName + " at row index " + rowidx.ToString() + " is null in one table but not in the other";
                            }//else they are _both_ null which is means they match and ReturnVal is still true :-) 
                        }
                        else
                        {
                            ReturnVal = false;
                            MisMatchReason = "DataTable_1 has column " + DataTable_1_ColumnName + " but DataTable_2 does not";
                        }
                    }//iterate columns in table 1

                }//iterate rows in table 1 

            }
            else
            {
                ReturnVal = false;
                MisMatchReason = "The number of rows do not match: DatTable_1 has " + DataTable_1.Rows.Count.ToString() + " rows whilst DataTable_2 has " + DataTable_2.Rows.Count.ToString() + "rows";
            }//if-else row count matches


            return ( ReturnVal );

        }//doTableValuesMatch() 


        public void fillTableWithArbitraryData( string TableName, Dictionary<string, List<string>> FillData )
        {
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "fillTableWtithArbitraryData_update", TableName );
            DataTable DataTable = CswTableUpdate.getTable();

            Int32 TotalRows = 0;
            foreach( List<string> CurrentList in FillData.Values )
            {
                if( CurrentList.Count > TotalRows )
                {
                    TotalRows = CurrentList.Count;
                }
            }


            foreach( string CurrentColumnName in FillData.Keys )
            {
                if( false == DataTable.Columns.Contains( CurrentColumnName ) )
                {
                    throw ( new CswDniException( "Value-fill column " + CurrentColumnName + " does not exist in table " + TableName ) );
                }
            }//iterate lists to get max row count

            for( int idx = 0; idx < TotalRows; idx++ )
            {
                DataRow DataRow = DataTable.NewRow();
                DataTable.Rows.Add( DataRow );
            }//iterate max rows to prime the tables

            foreach( string CurrentColumn in FillData.Keys )
            {
                List<string> CurrentValueList = FillData[CurrentColumn];
                for( int idx = 0; idx < CurrentValueList.Count; idx++ )
                {
                    DataTable.Rows[idx][CurrentColumn] = CurrentValueList[idx];
                }//iterate value list for current column

            }//iterate fill data to to populate table

            CswTableUpdate.update( DataTable );

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

        public void assertColumnIsAbsent( string TableName, string ColumnName, string ThrowMessageIn = "" )
        {
            string ThrowMessage = string.Empty;
            if( string.Empty == ThrowMessageIn )
            {
                ThrowMessage = " exists in ";
            }
            else
            {
                ThrowMessage = " " + ThrowMessageIn;
            }

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( TableName, ColumnName ) )
                throw ( new CswDniException( "Column " + ColumnName + ThrowMessage + " the database " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( TableName, ColumnName ) )
                throw ( new CswDniException( "Column " + ColumnName + ThrowMessage + " the meta data " ) );

        }//assertColumnIsAbsent() 

        public void assertColumnIsPresent( string TableName, string ColumnName, string ThrowMessageIn = "" )
        {
            string ThrowMessage = string.Empty;
            if( string.Empty == ThrowMessageIn )
            {
                ThrowMessage = " does not exist in ";
            }
            else
            {
                ThrowMessage = " " + ThrowMessageIn;
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( TableName, ColumnName ) )
                throw ( new CswDniException( "Column " + ColumnName + ThrowMessage + " the data base" ) );

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( TableName, ColumnName ) )
                throw ( new CswDniException( "Column " + ColumnName + ThrowMessage + " the meta data" ) );

        }//assertColumnIsAbsent() 

        public void assertTableIsAbsent( string TableName )
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TableName ) )
                throw ( new CswDniException( "Table " + TableName + " was not dropped from the database" ) );

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TableName ) )
                throw ( new CswDniException( "Table " + TableName + " was not dropped from meta data" ) );

        }//assertTableIsAbsent() 

        public void assertTableIsPresent( string TableName )
        {
            if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( TableName ) )
                throw ( new CswDniException( "Table " + TableName + " was not in the database" ) );

            if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( TableName ) )
                throw ( new CswDniException( "Table " + TableName + " in not in the meta data" ) );

        }//assertTableIsAbsent() 




    }//CswTestCaseRsrc

}//ChemSW.Nbt.Schema
