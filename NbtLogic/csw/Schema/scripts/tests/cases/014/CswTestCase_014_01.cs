using System;
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_014_01 : CswUpdateSchemaTo
    {

        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_014.Purpose, "Add test row to materials table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_014 _CswTstCaseRsrc_014 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_014_01( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_014 = (CswTstCaseRsrc_014) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_014.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;


            CswTableUpdate CswTableUpdateSetup = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( CswTstCaseRsrc_014.Purpose + "_setupdata", "materials" );
            DataTable MaterialsTable = CswTableUpdateSetup.getEmptyTable();
            DataRow NewMaterialsRow = MaterialsTable.NewRow();
            MaterialsTable.Rows.Add( NewMaterialsRow );
            _CswTstCaseRsrc_014.InsertedMaterialsRecordPk = Convert.ToInt32( MaterialsTable.Rows[0]["materialid"] );
            MaterialsTable.Rows[0]["materialname"] = CswTstCaseRsrc_014.Purpose + "--test row";
            CswTableUpdateSetup.update( MaterialsTable );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
