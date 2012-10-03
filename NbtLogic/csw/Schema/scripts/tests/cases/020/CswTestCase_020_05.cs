using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_020_05 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_020.Purpose, "verify tear down test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_020 _CswTstCaseRsrc_020 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_020_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_020 = (CswTstCaseRsrc_020) CswTstCaseRsc;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_020.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            CswAuditMetaData CswAuditMetaData = new Audit.CswAuditMetaData();
            string AuditTableName01 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_01 );
            string AuditTableName02 = CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_020.ArbitraryTableName_02 );

            _CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_020.ArbitraryTableName_01 );
            _CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_020.ArbitraryTableName_02 );
            _CswTstCaseRsrc.assertTableIsAbsent( AuditTableName01 );
            _CswTstCaseRsrc.assertTableIsAbsent( AuditTableName02 );

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
