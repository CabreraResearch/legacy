using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using ChemSW.Audit;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case29565B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29565; }
        }

        public override string ScriptName
        {
            get { return "02G_Case29565B"; }
        }

        public override void update()
        {
            // Populate audit_transaction.auditdate
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            // Since we don't know what table goes with what audit transaction, we'll need to go through every auditable table.  Fun fun.
            string Sql = @"update audit_transactions x
                              set auditdate = (select min(recordcreated) auditdate 
                                                 from (    
                                                             select audittransactionid, recordcreated from blob_data_audit
                                                       union select audittransactionid, recordcreated from jct_nodes_props_audit
                                                       union select audittransactionid, recordcreated from license_accept_audit
                                                       union select audittransactionid, recordcreated from nodes_audit
                                                       union select audittransactionid, recordcreated from nodetypes_audit
                                                       union select audittransactionid, recordcreated from nodetype_props_audit
                                                       union select audittransactionid, recordcreated from nodetype_tabset_audit
                                                       union select audittransactionid, recordcreated from node_views_audit
                                                       union select audittransactionid, recordcreated from object_class_audit
                                                       union select audittransactionid, recordcreated from object_class_props_audit
                                                      ) m
                                                where m.audittransactionid = x.audittransactionid)
                            where auditdate is null";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Sql );
        } // update()

    } // class CswUpdateSchema_02G_Case29565B

}//namespace ChemSW.Nbt.Schema