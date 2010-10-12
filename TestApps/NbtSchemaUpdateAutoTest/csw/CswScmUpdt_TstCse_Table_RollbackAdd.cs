using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_Table_RollbackAdd : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Table_RollbackAdd( )
            : base( "Rollback add table" )
        {
        }//ctor

        private string _TestTablename = "TestTable";
        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            //Since we're not testing add table here, we assume that add table does work. 
            _CswNbtSchemaModTrnsctn.addTable( _TestTablename, ( _TestTablename + "id" ).ToLower() );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            if ( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Rolled-back table " + _TestTablename + " was not dropped from the database server" ) );

            if ( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Rolled-back table " + _TestTablename + " was not removed from the meta data collection" ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropTable

}//ChemSW.Nbt.Schema
