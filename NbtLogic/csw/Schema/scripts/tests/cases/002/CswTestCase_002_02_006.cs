using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_002_02_006 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 006 ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_002.Purpose, "verify rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_002 = null;
        public CswTestCase_002_02_006( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_002 = new CswTstCaseRsrc_001( _CswNbtSchemaModTrnsctn );
        }//ctor


        private string _TestColumnName = "test_column";
        private string _TestTableName = "DATA_DICTIONARY";
        private string _TestValStem = "Test val ";

        public void update()
        {

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the database " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _TestTableName, _TestColumnName ) )
                throw ( new CswDniException( "Added column " + _TestColumnName + " was not rolled back from the meta data " ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
