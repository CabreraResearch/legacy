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
			if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( "jct_nodes_props_audit_audit" ) )
			{
				_CswNbtSchemaModTrnsctn.dropTable( "jct_nodes_props_audit_audit" );
			}

			if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( "jct_modules_objectclass_audit" ) )
			{
				_CswNbtSchemaModTrnsctn.dropTable( "jct_modules_objectclass_audit" );
			}

			if( _CswNbtSchemaModTrnsctn.isTableDefinedInMetaData( "object_class_props_audit" ) )
			{
				_CswNbtSchemaModTrnsctn.dropTable( "object_class_props_audit" );
			}

		} // update()

	}//class CswUpdateSchemaTo01H42

}//namespace ChemSW.Nbt.Schema

