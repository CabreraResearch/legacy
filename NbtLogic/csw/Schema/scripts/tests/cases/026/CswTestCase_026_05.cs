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

    public class CswTestCase_026_05 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_026.Purpose, "verify contents of audit" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_026 = (CswTstCaseRsrc_026) CswTstCaseRsrc;

        }//ctor


        public void update()
        {

            //CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            ////Records from insert **************************************************************
            //string MisMatchMessage = string.Empty;
            //if( false == _CswTstCaseRsrc_026.compareInsertAuditData( ref MisMatchMessage ) )
            //{
            //    throw ( new CswDniException( "Auditing test failed on insert data: " + MisMatchMessage ) );
            //}

            //DataTable DataTableFromInsert = _CswTstCaseRsrc_026.AuditRecordsFromInsert;
            //if( DataTableFromInsert.Rows.Count <= 0 )
            //{
            //    throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            //}

            //string TargetTablePkColName = _CswNbtSchemaModTrnsctn.getPrimeKeyColName( _CswTstCaseRsrc_026.ArbitraryTableName_01 );

            //for( int idx = 0; idx < DataTableFromInsert.Rows.Count; idx++ )
            //{
            //    DataRow CurrentRow = DataTableFromInsert.Rows[idx];
            //    if( AuditEventType.Insert != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
            //    {
            //        throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Insert.ToString() ) );
            //    }

            //    if( Convert.ToInt32( CurrentRow[TargetTablePkColName] ) != _CswTstCaseRsrc_026.PksOfAuditedRecords[idx] )
            //    {
            //        throw ( new CswDniException( "The " + CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() + " audit record at row " + idx.ToString() + " has a mismatched prime key" ) );
            //    }

            //}



            ////Records from Update ********************************************************************
            //if( false == _CswTstCaseRsrc_026.compareUpdateAuditData( ref MisMatchMessage ) )
            //{
            //    throw ( new CswDniException( "Auditing test failed on update datea: " + MisMatchMessage ) );
            //}

            //DataTable DataTableFromUpdate = _CswTstCaseRsrc_026.AuditRecordsFromUpdate;
            //if( DataTableFromUpdate.Rows.Count <= 0 )
            //{
            //    throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            //}


            //for( int idx = 0; idx < DataTableFromUpdate.Rows.Count; idx++ )
            //{
            //    DataRow CurrentRow = DataTableFromUpdate.Rows[idx];

            //    if( AuditEventType.Update != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
            //    {
            //        throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.Update.ToString() ) );
            //    }


            //    if( Convert.ToInt32( CurrentRow[TargetTablePkColName] ) != _CswTstCaseRsrc_026.PksOfAuditedRecords[idx] )
            //    {
            //        throw ( new CswDniException( "The " + CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() + " audit record at row " + idx.ToString() + " has a mismatched prime key" ) );
            //    }

            //}


            ////Records from delete **************************************************************
            //if( false == _CswTstCaseRsrc_026.compareDeleteAuditData( ref MisMatchMessage ) )
            //{
            //    throw ( new CswDniException( "Auditing test failed on Delete data: " + MisMatchMessage ) );
            //}

            //DataTable DataTableFromDelete = _CswTstCaseRsrc_026.AuditRecordsFromDelete;
            //if( DataTableFromDelete.Rows.Count <= 0 )
            //{
            //    throw ( new CswDniException( "Unable to evalutate state of audit table: there are no rows :-( " ) );
            //}

            //for( int idx = 0; idx < DataTableFromDelete.Rows.Count; idx++ )
            //{
            //    DataRow CurrentRow = DataTableFromDelete.Rows[idx];

            //    if( AuditEventType.PhysicalDelete != (AuditEventType) Enum.Parse( typeof( AuditEventType ), CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() ) )
            //    {
            //        throw ( new CswDniException( "A row in the audit table does not have AuditEventType == " + AuditEventType.PhysicalDelete.ToString() ) );
            //    }

            //    if( Convert.ToInt32( CurrentRow[TargetTablePkColName] ) != _CswTstCaseRsrc_026.PksOfAuditedRecords[idx] )
            //    {
            //        throw ( new CswDniException( "The " + CurrentRow[CswAuditMetaData.AuditEventTypeColName].ToString() + " audit record at row " + idx.ToString() + " has a mismatched prime key" ) );
            //    }
            //}

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
