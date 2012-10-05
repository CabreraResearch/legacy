
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_023_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_023.Purpose, "set up test tables" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_023 _CswTstCaseRsrc_023 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_023_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_023 = (CswTstCaseRsrc_023) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_023.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswTstCaseRsrc_023.setAuditingOn(); 
            _CswTstCaseRsrc_023.makeArbitraryTable();
            _CswNbtSchemaModTrnsctn.makeTableAuditable( _CswTstCaseRsrc_023.ArbitraryTableName_01 );

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
