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
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTestCaseRsrc
    {
        private Dictionary<TestTableNamesReal, string> _TestTableNamesReal = new Dictionary<TestTableNamesReal, string>();
        private Dictionary<TestTableNamesFake, string> _TestTableNamesFake = new Dictionary<TestTableNamesFake, string>();
        private Dictionary<TestColumnNamesFake, string> _TestColumnNamesFake = new Dictionary<TestColumnNamesFake, string>();
        private Dictionary<TestColumnNamesReal, string> _TestColumnNamesReal = new Dictionary<TestColumnNamesReal, string>();


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

        }//ctor

        public string getFakeTestTableName( TestTableNamesFake TestTableName ) { return ( _TestTableNamesFake[TestTableName] ); }
        public string getRealTestTableName( TestTableNamesReal TestTableName ) { return ( _TestTableNamesReal[TestTableName] ); }
        public string getFakeTestColumnName( TestColumnNamesFake TestColumnName ) { return ( _TestColumnNamesFake[TestColumnName] ); }
        public string getRealTestColumnName( TestColumnNamesReal TestColumnName ) { return ( _TestColumnNamesReal[TestColumnName] ); }


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

            string TestValStem = "Test val ";
            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                if( ( TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[_TestColumnNamesFake[TestColumnName]].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + _TestColumnNamesFake[TestColumnName] + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );


        }//_testAddColumnValues() 

        public string makeTestCaseDescription( string RawClassName, string TestCasePurpose, string SubPurpose )
        {
            return ( "Test Case " + RawClassName.Substring( 12 ) + ": " + TestCasePurpose + "--" + SubPurpose );
        }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
