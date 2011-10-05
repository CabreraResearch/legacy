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

    public class CswTestCase_016_03 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_016.Purpose, "Verify that column name did not change" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_016 _CswTstCaseRsrc_016 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_016_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_016 = (CswTstCaseRsrc_016) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_016.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc_016.FakeTestTableName, _CswTstCaseRsrc_016.FakeValColumnName02, "destination of rename operation still exists after rollback" );
            _CswTstCaseRsrc.assertColumnIsPresent( _CswTstCaseRsrc_016.FakeTestTableName, _CswTstCaseRsrc_016.FakeValColumnName01, "target of rename operation does not exist after rollback" );
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
