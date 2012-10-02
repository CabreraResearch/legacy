using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_012_02 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_012.Purpose, "Add empty test values" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_012 _CswTstCaseRsrc_012 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_012_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_012 = (CswTstCaseRsrc_012) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_012.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswTableUpdate CswArbitraryTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_012.FakeTestTableName );
            CswArbitraryTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTableArbitrary = CswArbitraryTableUpdate.getTable();

            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );
            DataTableArbitrary.Rows.Add( DataTableArbitrary.NewRow() );


            DataTableArbitrary.Rows[0][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;
            DataTableArbitrary.Rows[1][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;
            DataTableArbitrary.Rows[2][_CswTstCaseRsrc_012.FakeValColumnName] = _CswTstCaseRsrc_012.LocalAribtiraryValue;

            CswArbitraryTableUpdate.update( DataTableArbitrary );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
