//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_002_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_002.Purpose, "add and throw" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_002 _CswTstCaseRsrc_002 = null;
        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_002_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_002 = (CswTstCaseRsrc_002) CswTstCaseRsc;

        }//ctor

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_002.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc_002.TestColumnNameOne, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc_002.TestTableName, DataDictionaryUniqueType.None, false, string.Empty );

            throw ( new ChemSW.Exceptions.CswDniExceptionIgnoreDeliberately() );//this will cause a rollback, so we can test that the column add was rolled back in the next script in the 002 series


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
