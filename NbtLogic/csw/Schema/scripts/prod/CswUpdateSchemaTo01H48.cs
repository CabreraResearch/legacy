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
    /// Updates the schema to version 01H-42
    /// </summary>
    public class CswUpdateSchemaTo01H48 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 48 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H48( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {



            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "jct_nodes_props_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodes_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetype_props_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "nodetypes_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "object_class_props_audit" );



            CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Description, "select distinct tablename from data_dictionary where lower(tablename) like '%_audit'" );
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();

            DataTable DataTable = CswArbitrarySelect.getTable();
            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                string CurrentAuditTableName = CurrentRow["tablename"].ToString();

                if( _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentAuditTableName, "deletedphysically" ) )
                {
                    _CswNbtSchemaModTrnsctn.dropColumn( CurrentAuditTableName, "deletedphysically" );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentAuditTableName, CswAuditMetaData.AuditEventTypeColName ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( CurrentAuditTableName, CswAuditMetaData.AuditEventTypeColName, CswAuditMetaData.AuditEventTypeColDescription, false, true, CswAuditMetaData.AuditEventTypeColLength );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentAuditTableName, CswAuditMetaData.AuditRecordCreatedColName ) )
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn( CurrentAuditTableName, CswAuditMetaData.AuditRecordCreatedColName, CswAuditMetaData.AuditRecordCreatedColDescription, false, true );
                }


                string AuditedTableName = CswAuditMetaData.makeNameOfAuditedTable( CurrentAuditTableName );
                string AuditedTablePkColName = _CswNbtSchemaModTrnsctn.CswDataDictionary.getPrimeKeyColumn( AuditedTableName );
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentAuditTableName, AuditedTablePkColName ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( CurrentAuditTableName, AuditedTablePkColName, "prime key of audited record", false, true );
                }

            }//iterate audit tables





        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

