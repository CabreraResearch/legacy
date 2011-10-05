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

    public class CswTestCase_012_01 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_012.Purpose, "Set up test tables and data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_012 = (CswTstCaseRsrc_012) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_012.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswNbtSchemaModTrnsctn.addTable( _CswTstCaseRsrc_012.FakeTestTableName, _CswTstCaseRsrc_012.FakePkColumnName );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_012.FakeTestTableName, _CswTstCaseRsrc_012.FakeValColumnName, "test", false, false, 240 );
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
