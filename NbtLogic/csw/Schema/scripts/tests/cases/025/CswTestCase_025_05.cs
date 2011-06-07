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

    public class CswTestCase_025_05 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_025.Purpose, "verify contents of audit" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_025 _CswTstCaseRsrc_025 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_025_05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_025 = (CswTstCaseRsrc_025) CswTstCaseRsrc;

        }//ctor


        public void update()
        {

            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            //Records from insert **************************************************************
            string MisMatchMessage = string.Empty;
            if( false == _CswTstCaseRsrc_025.compareInsertAuditData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed on insert data: " + MisMatchMessage ) );
            }

            DataTable DataTableFromInsert = _CswTstCaseRsrc_025.TestRecordsFromInsert;
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



            //Records from Update ********************************************************************
            if( false == _CswTstCaseRsrc_025.compareUpdateAuditData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed on update datea: " + MisMatchMessage ) );
            }

            DataTable DataTableFromUpdate = _CswTstCaseRsrc_025.TestRecordsFromUpdate;
            if( DataTableFromUpdate.Rows.Count <= 0 )
            {
                throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            }


            foreach( DataRow CurrentRow in DataTableFromUpdate.Rows )
            {
                if( AuditEventType.Update != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
                {
                    throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
                }
            }


            //Records from delete **************************************************************
            if( false == _CswTstCaseRsrc_025.compareDeleteAuditData( ref MisMatchMessage ) )
            {
                throw ( new CswDniException( "Auditing test failed on Delete data: " + MisMatchMessage ) );
            }

            DataTable DataTableFromDelete = _CswTstCaseRsrc_025.TestRecordsFromDelete;
            if( DataTableFromDelete.Rows.Count <= 0 )
            {
                throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            }

            foreach( DataRow CurrentRow in DataTableFromDelete.Rows )
            {
                if( AuditEventType.PhysicalDelete != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
                {
                    throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
                }
            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
