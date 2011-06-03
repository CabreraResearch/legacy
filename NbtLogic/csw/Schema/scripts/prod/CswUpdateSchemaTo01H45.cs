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
    public class CswUpdateSchemaTo01H45 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 45 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H45( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {

            CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Description, "select distinct tablename from data_dictionary where lower(tablename) like '%_audit'" );
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData(); 
                 
            DataTable DataTable = CswArbitrarySelect.getTable();
            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                string CurrentTableName = CurrentRow["tablename"].ToString(); 

                if( _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentTableName, "deletedphysically" ) )
                {
                    _CswNbtSchemaModTrnsctn.dropColumn( CurrentTableName, "deletedphysically" );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( CurrentTableName, CswAuditMetaData.AuditEventTypeColName ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( CurrentTableName, CswAuditMetaData.AuditEventTypeColName, CswAuditMetaData.AuditEventTypeColDescription, false, true, CswAuditMetaData.AuditEventTypeColLength );
                }


            }//iterate audit tables

        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

