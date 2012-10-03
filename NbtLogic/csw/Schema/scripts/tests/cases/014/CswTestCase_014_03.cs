using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_014_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_014.Purpose, "Verify record was not deleted" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_014 _CswTstCaseRsrc_014 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_014_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_014 = (CswTstCaseRsrc_014) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_014.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswTableSelect CswTableSelectMaterials = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, "materials" );
            DataTable DataTableMaterials = CswTableSelectMaterials.getTable( " where materialid=" + _CswTstCaseRsrc_014.InsertedMaterialsRecordPk.ToString() );
            if( DataTableMaterials.Rows.Count != 1 )
                throw ( new CswDniException( "Update of a record from a CswTableUpdate with specified columns resulted in deletion of materials record " + _CswTstCaseRsrc_014.InsertedMaterialsRecordPk.ToString() ) );
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
