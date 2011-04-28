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
    public class CswTstCaseRsrc_001
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;


        public CswTstCaseRsrc_001( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }//ctor


        public string Purpose = "Add Columns";

        public string TestColumnNameOne = "test_column_one";
        public string TestColumnNameTwo = "test_column_two";
        public string TestTableName = "modules";
        public string TestValStem = "Test val ";

        public void testAddColumnValues( string ColumnName )
        {
            Int32 TotalUpdated = 0;
            CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "testAddColumnValues_update", TestTableName );
            DataTable TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                CurrentRow[ColumnName] = "Test val " + TestTable.Rows.IndexOf( CurrentRow ).ToString();
                TotalUpdated++;
            }

            TestTableUpdate.update( TestTable );

            Int32 TotalUpdatedInfact = 0;
            TestTable = TestTableUpdate.getTable();
            foreach( DataRow CurrentRow in TestTable.Rows )
            {
                if( ( TestValStem + TestTable.Rows.IndexOf( CurrentRow ) ) == CurrentRow[ColumnName].ToString() )
                    TotalUpdatedInfact++;
            }

            if( TotalUpdatedInfact != TotalUpdated )
                throw ( new CswDniException( "Error adding column " + ColumnName + ": updated " + TotalUpdated.ToString() + " rows but retrieved " + TotalUpdatedInfact.ToString() + " with that value" ) );


        }//_testAddColumnValues

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
