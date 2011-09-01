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
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_026_07 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_026.Purpose, "verify test data was cleaned up" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_07( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026 = new CswTstCaseRsrc_026( _CswNbtSchemaModTrnsctn );
			
			//CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            //_CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_026.ArbitraryTableName_01 );
            //_CswTstCaseRsrc.assertTableIsAbsent( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_026.ArbitraryTableName_01 ) );
            //_CswTstCaseRsrc_026.assertAuditSettingIsRestored(); 
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
