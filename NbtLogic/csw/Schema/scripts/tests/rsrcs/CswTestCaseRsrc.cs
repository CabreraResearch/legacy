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

    public enum TestTableNames { TestTable01, TestTable02 }
    public enum TestColumnNames { TestColumn01, TestColumn02 }
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTestCaseRsrc
    {
        private Dictionary<TestTableNames, string> _TestTableNames = new Dictionary<TestTableNames, string>();
        private Dictionary<TestColumnNames, string> _TestColumnNames = new Dictionary<TestColumnNames, string>();


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        public CswTestCaseRsrc( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;


            _TestTableNames.Add( TestTableNames.TestTable01, "modules" );
            _TestTableNames.Add( TestTableNames.TestTable02, "data_dictionary" );


            _TestColumnNames.Add( TestColumnNames.TestColumn01, "test_column_one" );
            _TestColumnNames.Add( TestColumnNames.TestColumn02, "test_column_two" );



        }//ctor

        public string getTestTableName( TestTableNames TestTableName ) { return ( _TestTableNames[TestTableName] ); }
        public string getTestColumnName( TestColumnNames TestColumnName ) { return ( _TestColumnNames[TestColumnName] ); }


        public void testAddColumnValues( TestTableNames TestTableName, TestColumnNames TestColumnName )
        {
            Int32 TotalUpdated = 0;
            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "testAddColumnValues_update", _TestTableNames[TestTableName] );
            DataTable TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[_TestColumnNames[TestColumnName]] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            string TestValStem = "Test val ";
            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                if( ( TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[_TestColumnNames[TestColumnName]].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + _TestColumnNames[TestColumnName] + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );


        }//_testAddColumnValues() 

        public string makeTestCaseDescription( string RawClassName, string TestCasePurpose, string SubPurpose )
        {
            return ( "Test Case " + RawClassName.Substring( 12 ) + ": " + TestCasePurpose + "--" + SubPurpose );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
