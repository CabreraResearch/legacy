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
	/// Updates the schema to version 01H-53
	/// </summary>
	public class CswUpdateSchemaTo01H53 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 53 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H53( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{

			// case 22176

			//New Inspection Status action
			_CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Inspection_Status, true, "", "" );

			// Add permissions
			CswNbtObjClassRole AdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" ) );
			CswNbtObjClassRole ChemSWAdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "chemsw_admin_role" ) );
			_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Inspection_Status, AdministratorRoleNode, true );
			_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Inspection_Status, ChemSWAdministratorRoleNode, true );

		} // update()

	}//class CswUpdateSchemaTo01H53

}//namespace ChemSW.Nbt.Schema

