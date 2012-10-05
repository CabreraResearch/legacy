using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_003_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_003.Purpose, "drop column" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_003 _CswTstCaseRsrc_003 = null;
 
        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_003_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_003 = (CswTstCaseRsrc_003) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_003.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;


            _CswNbtSchemaModTrnsctn.dropColumn( _CswTstCaseRsrc_003.RealTestTableName, _CswTstCaseRsrc_003.RealTestColumnName );
            throw ( new CswDniExceptionIgnoreDeliberately() );//this will cause the rollback dropping the column as a result of which this test case has as its purpose


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
