using System;
using System.Data;
using ChemSW.Audit;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_021_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_021.Purpose, "verify contents of audit" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_021 _CswTstCaseRsrc_021 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_021_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_021 = (CswTstCaseRsrc_021) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_021.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			string MisMatchMessage = string.Empty;
            if( false == _CswTstCaseRsrc_021.compareTargetAndAuditedData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed: " + MisMatchMessage ) );
            }

            CswAuditMetaData CswAuditMetaData = new Audit.CswAuditMetaData();
            CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_021.ArbitraryTableName_01 ) );
            DataTable DataTable = CswTableSelect.getTable();
            if( DataTable.Rows.Count <= 0 )
            {
                throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            }

            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                if( AuditEventType.Insert != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
                {
                    throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
                }
            }

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
