using System;
using System.Data;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-49
    /// </summary>
    public class CswUpdateSchemaTo01H49 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 49 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H49( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {

            //Nuke the old audit data -- who knows where it came from? 
            CswTableUpdate JctNodePropsAuditUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, "jct_nodes_props_audit" );
            DataTable JctNodePropsAuditOldRecords = JctNodePropsAuditUpdate.getTable();
            foreach( DataRow CurrentRow in JctNodePropsAuditOldRecords.Rows )
            {
                CurrentRow.Delete();
            }
            JctNodePropsAuditUpdate.update( JctNodePropsAuditOldRecords );


            ////Make all current props auditable
            //CswAuditMetaData CswAuditMetaData = new Audit.CswAuditMetaData(); 
            //CswTableUpdate JctNodesPropsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, "jct_nodes_props" );
            //DataTable JctNodesPropsRecords = JctNodesPropsUpdate.getTable();
            //foreach( DataRow CurrentRow in JctNodesPropsRecords.Rows )
            //{
            //    CurrentRow[CswAuditMetaData.AuditLevelColName] = AuditLevel.PlainAudit; 
            //}
            //JctNodesPropsUpdate.update( JctNodesPropsRecords );


            //To deal with tables to which we added the audit level column, but previously neglected to set the default aduit level value
            CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Description, "select distinct tablename from data_dictionary where lower(tablename) like '%_audit'" );
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            DataTable DataTable = CswArbitrarySelect.getTable();
            foreach( DataRow CurrentAuditTableRow in DataTable.Rows )
            {

                string AuditedTableName = CswAuditMetaData.makeNameOfAuditedTable( CurrentAuditTableRow["tablename"].ToString() );
                CswTableUpdate CurrentCswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description + "_" + AuditedTableName, AuditedTableName );
                DataTable CurrentTargetOfAuditTable = CurrentCswTableUpdate.getTable();
                foreach( DataRow CurrentRowOfTargetOfAuditTable in CurrentTargetOfAuditTable.Rows )
                {
                    CurrentRowOfTargetOfAuditTable[CswAuditMetaData.AuditLevelColName] = CswAuditMetaData.DefaultAuditLevel;
                }

                CurrentCswTableUpdate.update( CurrentTargetOfAuditTable );

            }//iterate audit tables

            //Turn on auditing for one and all!!!
            CswTableUpdate CswTableUpdateConfigVals = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description + "__configuration_variables", "configuration_variables" );
            DataTable DataTableConfigVals = CswTableUpdateConfigVals.getTable( " where lower(variablename)='auditing'" );
            DataTableConfigVals.Rows[0]["variablevalue"] = "1";
            CswTableUpdateConfigVals.update( DataTableConfigVals ); 




        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

