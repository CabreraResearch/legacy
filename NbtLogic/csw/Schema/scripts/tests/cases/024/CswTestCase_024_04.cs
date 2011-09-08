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

    public class CswTestCase_024_04 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_024.Purpose, "verify contents of audit" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_024 _CswTstCaseRsrc_024 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_024_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_024 = (CswTstCaseRsrc_024) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_024.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            //Records from Update ********************************************************************
            string MisMatchMessage = string.Empty;
            if( false == _CswTstCaseRsrc_024.compareUpdateAuditData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed on update datea: " + MisMatchMessage ) );
            }

            DataTable DataTableFromUpdate = _CswTstCaseRsrc_024.TestRecordsFromUpdate;
            if( DataTableFromUpdate.Rows.Count <= 0 )
            {
                throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            }

            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData(); 

            foreach( DataRow CurrentRow in DataTableFromUpdate.Rows )
            {
                if( AuditEventType.Update != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
                {
                    throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
                }
            }


            //Records from insert **************************************************************
            if( false == _CswTstCaseRsrc_024.compareInsertAuditData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed on insert data: " + MisMatchMessage ) );
            }

            DataTable DataTableFromInsert = _CswTstCaseRsrc_024.TestRecordsFromInsert;
            if( DataTableFromInsert.Rows.Count <= 0 )
            {
                throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            }

            foreach( DataRow CurrentRow in DataTableFromInsert.Rows )
            {
                if( AuditEventType.Insert != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
                {
                    throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
                }
            }


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
