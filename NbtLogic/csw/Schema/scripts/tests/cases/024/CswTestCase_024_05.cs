using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_024_05 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_024.Purpose, "clean up test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_024 _CswTstCaseRsrc_024 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_024_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_024 = (CswTstCaseRsrc_024) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_024.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_024.ArbitraryTableName_01 );
            _CswNbtSchemaModTrnsctn.dropTable( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_024.ArbitraryTableName_01 ) );
            _CswTstCaseRsrc_024.restoreAuditSetting(); 
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
