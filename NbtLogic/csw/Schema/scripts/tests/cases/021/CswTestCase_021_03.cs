using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_021_03 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_021.Purpose, "verify contents of audit" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_021 _CswTstCaseRsrc_021 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_021_03( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_021 = new CswTstCaseRsrc_021( _CswNbtSchemaModTrnsctn );
			
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

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
