//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTestCase_001_01 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_001.Purpose, "Add column" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_001 _CswTstCaseRsrc_001 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_001_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_001 = (CswTstCaseRsrc_001) CswTstCaseRsc;
		}//ctor

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_001.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			_CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc_001.TestColumnNameOne, CswEnumDataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc_001.TestTableName, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
            _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc_001.TestColumnNameTwo, CswEnumDataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc_001.TestTableName, CswEnumDataDictionaryUniqueType.None, false, string.Empty );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
