using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Table_Add : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Table_Add( )
            : base( "Add Table" )
        {
        }//ctor

        private string _TestTablename = "TestTable";
        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.addTable( _TestTablename, ( _TestTablename + "id" ).ToLower() );


            //Check that database knows about the table
            if ( !_CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Add Table for table " + _TestTablename + " could not be verified by the database server" ) );


            //Check that the meta data knows about the table
            if ( !_CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Add Table for table " + _TestTablename + " could not be verified by the meta data collection" ) );


            //Check that we can add a row to the new table
            try
            {
                CswTableUpdate TestTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_Table_Add_update", "TestTable" );
                DataTable TestTable = TestTableUpdate.getTable();

                DataRow NewRow = TestTable.NewRow();
                TestTable.Rows.Add( NewRow );
                TestTableUpdate.update( TestTable );
            }
            catch ( Exception Exception )
            {
                throw ( new CswScmUpdt_Exception( "Row could not be inserted in added table " + _TestTablename + ": " + Exception.Message ) );
            }

            _CswNbtSchemaModTrnsctn.rollbackTransaction();

        }//runTest()

    }//CswSchemaUpdaterTestCaseAddTable

}//ChemSW.Nbt.Schema
