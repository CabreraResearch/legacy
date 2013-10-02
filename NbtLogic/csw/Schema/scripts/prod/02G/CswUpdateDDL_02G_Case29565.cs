using System;
using System.Data;
using ChemSW.Audit;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for MetaData changes
    /// </summary>
    public class CswUpdateDDL_02G_Case29565 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Add any missing columns to audit tables"; } }

        public override string ScriptName
        {
            get { return "Case_29565"; }
        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29565; }
        }

        public override void update()
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            // part 4, add audittransaction.datetime
            _CswNbtSchemaModTrnsctn.addDateColumn( CswAuditMetaData.AuditTransactionTableName, "auditdate", "DateTime of audit transaction", false, true );


            // Inspect shadow tables for missing columns
            Int32 n = 1;
            foreach( string TableName in _CswNbtSchemaModTrnsctn.CswDataDictionary.getTableNames( IncludeAudit: false ) )
            {
                if( _CswNbtSchemaModTrnsctn.isTableAuditable( TableName ) )
                {
                    // this function will handle finding missing columns and creating them
                    _CswNbtSchemaModTrnsctn.makeTableAuditable( TableName );

                    // case 30839
                    _CswNbtSchemaModTrnsctn.indexColumn( CswAuditMetaData.makeAuditTableName( TableName ), "audittransactionid, recordcreated", "audit" + n );
                    n++;
                }
            }
        } // update()

    }
}


