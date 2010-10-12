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

    public class CswScmUpdt_TstCse_Table_Drop : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Table_Drop( )
            : base( "Drop table" )
        {
        }//ctor

        private string _TestTablename = "TestTable";
        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.addTable( _TestTablename, ( _TestTablename + "id" ).ToLower() );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.dropTable( _TestTablename );
            _CswNbtSchemaModTrnsctn.commitTransaction();

            if ( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Dropped table " + _TestTablename + " was not dropped from the database server" ) );

            if ( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _TestTablename ) )
                throw ( new CswScmUpdt_Exception( "Dropped table " + _TestTablename + " was not removed from the meta data collection" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropTable

}//ChemSW.Nbt.Schema
