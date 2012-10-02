using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_030_02 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_030.Purpose, "Add row to test table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_030 _CswTstCaseRsrc_030 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_030_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_030 = (CswTstCaseRsrc_030) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_030.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "Testscript_030_02", _CswTstCaseRsrc_030.ArbitraryTableName_01 );

            DataTable DataTable = CswTableUpdate.getTable();

            DataRow NewRow = DataTable.NewRow();

            NewRow[_CswTstCaseRsrc_030.ArbitraryColumnName_02] = "snot";

            DataTable.Rows.Add( NewRow );

            CswTableUpdate.update( DataTable );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
