using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_009_03 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_009.Purpose, "verify tear down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_009 _CswTstCaseRsrc_009 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_009_03( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_009 = new CswTstCaseRsrc_009( _CswNbtSchemaModTrnsctn );
        }//ctor

        public void update()
        {
            if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( _CswTstCaseRsrc_009.FakeTestTableName ) )
                throw ( new CswDniException( "Table " + _CswTstCaseRsrc_009.FakeTestTableName + " was not dropped deleted from the metadata" ) );

            if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( _CswTstCaseRsrc_009.FakeTestTableName ) )
                throw ( new CswDniException( "Table " + _CswTstCaseRsrc_009.FakeTestTableName + " was not dropped from the database" ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
