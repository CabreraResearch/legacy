
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_006_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_006.Purpose, "add constrained tables and fk constraint" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_006 _CswTstCaseRsrc_006 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_006_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_006 = (CswTstCaseRsrc_006) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_006.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswTstCaseRsrc_006.makePkFkTables();
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
