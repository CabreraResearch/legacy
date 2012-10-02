using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_023_05 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_023.Purpose, "verify test data was cleaned up" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_023 _CswTstCaseRsrc_023 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_023_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_023 = (CswTstCaseRsrc_023) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_023.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswTstCaseRsrc.assertTableIsAbsent( _CswTstCaseRsrc_023.ArbitraryTableName_01 );
            _CswTstCaseRsrc.assertTableIsAbsent( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_023.ArbitraryTableName_01 ) );
            _CswTstCaseRsrc_023.assertAuditSettingIsRestored(); 
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
