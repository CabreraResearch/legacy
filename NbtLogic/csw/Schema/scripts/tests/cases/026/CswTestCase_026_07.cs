﻿
namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_026_07 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_026.Purpose, "verify test data was cleaned up" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_07( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_026 = (CswTstCaseRsrc_026) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			//CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            //_CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_026.ArbitraryTableName_01 );
            //_CswTstCaseRsrc.assertTableIsAbsent( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_026.ArbitraryTableName_01 ) );
            //_CswTstCaseRsrc_026.assertAuditSettingIsRestored(); 
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
