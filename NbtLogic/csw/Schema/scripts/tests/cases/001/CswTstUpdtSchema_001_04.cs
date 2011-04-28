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
    /// Test Case: 001, part 04
    /// </summary>
    public class CswTstUpdtSchema_001_04 : ICswUpdateSchemaTo
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 01, _CswTstCaseRsrc.makeTestCaseDescription( _CswTstCaseRsrc_001.Purpose, "Verify test data removed" ) ); } }

        private CswTstCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;
        public CswTstUpdtSchema_001_04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTstCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_001 = new CswTstCaseRsrc_001( _CswNbtSchemaModTrnsctn );
        }//ctor



        public void update()
        {

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not remove fromdata base " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not remove fromdata base " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not remove frommeta data " ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not remove frommeta data " ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
