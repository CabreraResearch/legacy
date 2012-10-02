using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_013_05 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_013.Purpose, "Verify that data are as they should be" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_013 _CswTstCaseRsrc_013 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_013_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_013 = (CswTstCaseRsrc_013) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_013.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_013.FakeTestTableName );
            CswTableUpdate.StorageMode = StorageMode.Cached; // causes the rolback behavior we want
            DataTable DataTable = CswTableUpdate.getTable();
            
            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName02].ToString() != _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName02 + " does not have the committed value " + _CswTstCaseRsrc_013.LocalAribtiraryValue02Delta ) );

            if( DataTable.Rows[0][_CswTstCaseRsrc_013.FakeValColumnName01].ToString() == _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta )
                throw ( new CswDniException( "Column  " + _CswTstCaseRsrc_013.FakeValColumnName01 + " has the rolled back value (with another value modication): " + _CswTstCaseRsrc_013.LocalAribtiraryValue01Delta ) );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
