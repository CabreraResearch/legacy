using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-42
	/// </summary>
	public class CswUpdateSchemaTo01H42 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 42 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H42( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{
			// fix audit tables from script 39

			// audit
			_CswNbtSchemaModTrnsctn.makeTableAuditable( "nodetype_tabset" );

			// don't audit
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "jct_nodes_props_audit" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "jct_modules_objectclass" );
            _CswNbtSchemaModTrnsctn.makeTableNotAuditable( "object_class_props" ); 

            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswNbtSchemaModTrnsctn.addStringColumn( CswAuditMetaData.AuditTransactionTableName, "transactionusername", "the user as which the transaction was comitted", false, false, 50 );
		} // update()

	}//class CswUpdateSchemaTo01H42

}//namespace ChemSW.Nbt.Schema

