using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01J-06
	/// </summary>
	public class CswUpdateSchemaTo01J06 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 06 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{

			// case 23920 - Grant permissions to new action to administrator roles
			CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
			foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, true ) )
			{
				CswNbtObjClassRole RoleNodeAsRole = (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode );
				if( RoleNodeAsRole.Administrator.Checked == Tristate.True || RoleNodeAsRole.Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName )
				{
					_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Sessions, RoleNodeAsRole, true );
				}
			}

		}//Update()

	}//class CswUpdateSchemaTo01J06

}//namespace ChemSW.Nbt.Schema


