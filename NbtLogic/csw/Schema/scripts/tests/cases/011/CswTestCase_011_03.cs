using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_011_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_011.Purpose, "Do an insert and cause rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_011 _CswTstCaseRsrc_011 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_011_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_011 = (CswTstCaseRsrc_011) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_011.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswTableUpdate CswUpdateTestTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_011.FakeTestTableName );

            CswUpdateTestTable.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswUpdateTestTable.getEmptyTable(); 

            DataRow DataRow = DataTable.NewRow();
            DataRow[_CswTstCaseRsrc_011.FakeTestColumnName] = "FOO  ";
            DataTable.Rows.Add( DataRow );

            CswUpdateTestTable.update( DataTable );

            new CswDniExceptionIgnoreDeliberately(); //causes rollback

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
