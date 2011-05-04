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
    public class CswTestCase_001_04_004 : ICswUpdateSchemaTo
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 004 ); } }


        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name,_CswTstCaseRsrc_001.Purpose, "Verify tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;
        public CswTestCase_001_04_004( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_001 = new CswTstCaseRsrc_001( _CswNbtSchemaModTrnsctn );
        }//ctor



        public void update()
        {

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not removed from data base" ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not removed from the data base" ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameOne ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameOne + " was not removed from the data base" ) );

            if( _CswNbtSchemaModTrnsctn.isColumnDefinedInMetaData( _CswTstCaseRsrc_001.TestTableName, _CswTstCaseRsrc_001.TestColumnNameTwo ) )
                throw ( new CswDniException( "Column " + _CswTstCaseRsrc_001.TestColumnNameTwo + " was not removed fromme the data base" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
