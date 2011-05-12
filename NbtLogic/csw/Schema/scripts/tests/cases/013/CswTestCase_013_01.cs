using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_013_01 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_013.Purpose, "Set up test tables" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_01( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsrc;
        }//ctor

        public void update()
        {

            _CswNbtSchemaModTrnsctn.addTable( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakePkColumnName );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakeValColumnName01, "test", false, false, 240 );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakeValColumnName02, "test", false, false, 240 );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
