using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_012_05 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_012.Purpose, "Commit row three value" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_012 = (CswTstCaseRsrc_012) CswTstCaseRsc;
        }//ctor

        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_012.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_012.FakeTestTableName );
            CswTableUpdate.StorageMode = CswEnumStorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswTableUpdate.getTable();

            DataTable.Rows[2][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.Val_Row_3;
            CswTableUpdate.update( DataTable );

        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override string ScriptName
        {
            get { throw new System.NotImplementedException(); }
        }

        public override bool AlwaysRun
        {
            get { throw new System.NotImplementedException(); }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
