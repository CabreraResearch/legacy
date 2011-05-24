
using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-38
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

            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();



        } // update()

    }//class CswUpdateSchemaTo01H39

}//namespace ChemSW.Nbt.Schema

