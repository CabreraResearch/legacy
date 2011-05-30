using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-39
    /// </summary>
    public class CswUpdateSchemaTo01H39 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 39 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H39( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswNbtSchemaModTrnsctn.addTable( CswAuditMetaData.AuditTransactionTableName, CswAuditMetaData.AuditTransactionIdColName );
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, CswAuditMetaData.AuditLevelColName, CswAuditMetaData.AuditLevelColDescription, false, CswAuditMetaData.AuditLevelColIsRequired, CswAuditMetaData.AuditLevelColLength );
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, "auditeventname", "Describes the event tying together all associated audit records", false, false, 50 );
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, "esigreason", "Description of ESIG event", false, false, 50 );
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, "esigusername", "Name of user who signed the ESIG event", false, false, 50 );
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, "esiguserpassword", "Password of user who signed the ESIG event", false, false, 50 );

            _CswNbtSchemaModTrnsctn.makeTableAuditable( "jct_modules_objectclass" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "users" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "packdetail" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "object_class_props" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "nodetypes" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "license_accept" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "containers" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "materials_subclass" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "materials_synonyms" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "vendors" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "materials" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "inventory_groups" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "units_of_measure" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "locations" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "node_views" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "packages" );
            _CswNbtSchemaModTrnsctn.makeTableAuditable( "jct_nodes_props" );

        } // update()

    }//class CswUpdateSchemaTo01H39

}//namespace ChemSW.Nbt.Schema

