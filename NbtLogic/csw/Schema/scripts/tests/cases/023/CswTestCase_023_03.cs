using System.Data;
using ChemSW.Audit;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_023_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_023.Purpose, "verify audit data were rolled back" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_023 _CswTstCaseRsrc_023 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_023_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_023 = (CswTstCaseRsrc_023) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_023.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			CswAuditMetaData CswAuditMetaData = new Audit.CswAuditMetaData();
            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_023.ArbitraryTableName_01 ) );
            DataTable DataTable = CswTableSelect.getTable();
            if( DataTable.Rows.Count > 0 )
            {
                throw ( new CswDniException( "Audit records were not rolled back" ) );
            }


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
