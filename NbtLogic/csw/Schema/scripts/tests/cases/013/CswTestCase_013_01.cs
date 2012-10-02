
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_013_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_013.Purpose, "Set up test tables" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_013.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswNbtSchemaModTrnsctn.addTable( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakePkColumnName );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakeValColumnName01, "test", false, false, 240 );
            _CswNbtSchemaModTrnsctn.addStringColumn( _CswTstCaseRsrc_013.FakeTestTableName, _CswTstCaseRsrc_013.FakeValColumnName02, "test", false, false, 240 );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
