using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_014_05 : CswUpdateSchemaTo
    {

        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_014.Purpose, "Verify data clean up" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_014 _CswTstCaseRsrc_014 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_014_05( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
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
            if( DataTableMaterials.Rows.Count != 0 )
                throw ( new CswDniException( "Test data were not cleaned up (materials record with materialid = " + _CswTstCaseRsrc_014.InsertedMaterialsRecordPk.ToString() + ")" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
