using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-56
	/// </summary>
	public class CswUpdateSchemaTo01H56 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 56 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H56( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{

			// case 22525
			if( !_CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "readonly" ) )
			{
				_CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "readonly", "Whether the node and all of its properties are read only", false, false );
			}
			if( !_CswNbtSchemaModTrnsctn.isColumnDefined( "nodes_audit", "readonly" ) )
			{
				_CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes_audit", "readonly", "Whether the node and all of its properties are read only", false, false );
			}

		} // update()


	}//class CswUpdateSchemaTo01H56

}//namespace ChemSW.Nbt.Schema

