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

            //STEP I: Nuke the old audit data -- who knows where it came from? 
            /*
            CswTableUpdate JctNodePropsAuditUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, "jct_nodes_props_audit" );
            DataTable JctNodePropsAuditOldRecords = JctNodePropsAuditUpdate.getTable();
            foreach( DataRow CurrentRow in JctNodePropsAuditOldRecords.Rows )
            {
                CurrentRow.Delete();
            }
            JctNodePropsAuditUpdate.update( JctNodePropsAuditOldRecords );
             */

            //STEP I: With raw SQL
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from jct_nodes_props_audit" );



            //STEP II: Set the default audit level for auditable tables
            //To deal with tables to which we added the audit level column, but previously neglected to set the default aduit level value
            CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Description, "select distinct tablename from data_dictionary where lower(tablename) like '%_audit'" );
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            DataTable DataTable = CswArbitrarySelect.getTable();
            foreach( DataRow CurrentAuditTableRow in DataTable.Rows )
            {

                string AuditedTableName = CswAuditMetaData.makeNameOfAuditedTable( CurrentAuditTableRow["tablename"].ToString() );

                //Core of STEP II code with raw SQL:
                string FrickinUpdateCommand = "update " + AuditedTableName + " set " + CswAuditMetaData.AuditLevelColName + "='" + CswAuditMetaData.DefaultAuditLevel.ToString() + "'";
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( FrickinUpdateCommand);

                /*
                CswTableUpdate CurrentCswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description + "_" + AuditedTableName, AuditedTableName );
                DataTable CurrentTargetOfAuditTable = CurrentCswTableUpdate.getTable();
                foreach( DataRow CurrentRowOfTargetOfAuditTable in CurrentTargetOfAuditTable.Rows )
                {
                    CurrentRowOfTargetOfAuditTable[CswAuditMetaData.AuditLevelColName] = CswAuditMetaData.DefaultAuditLevel;
                }

                CurrentCswTableUpdate.update( CurrentTargetOfAuditTable );
                 */

            }//iterate audit tables

            //Turn on auditing for one and all!!!
            CswTableUpdate CswTableUpdateConfigVals = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description + "__configuration_variables", "configuration_variables" );
            DataTable DataTableConfigVals = CswTableUpdateConfigVals.getTable( " where lower(variablename)='auditing'" );
            DataTableConfigVals.Rows[0]["variablevalue"] = "1";
            CswTableUpdateConfigVals.update( DataTableConfigVals );




        } // update()

    }//class CswUpdateSchemaTo01H49

}//namespace ChemSW.Nbt.Schema

