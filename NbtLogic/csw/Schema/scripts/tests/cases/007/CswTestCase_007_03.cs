
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_007_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_007.Purpose, "verify tear-down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_007 _CswTstCaseRsrc_007 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_007_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_007 = (CswTstCaseRsrc_007) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_007.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) );
            _CswTstCaseRsrc.assertColumnIsAbsent( _CswTstCaseRsrc.getRealTestTableName( TestTableNamesReal.DataDictionary ), _CswTstCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) );

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
