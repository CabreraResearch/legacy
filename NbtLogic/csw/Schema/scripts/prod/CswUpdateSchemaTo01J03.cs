using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-03
    /// </summary>
    public class CswUpdateSchemaTo01J03 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 03 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 20970 - Grant permissions to new action to administrator roles
			CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass);
			foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, true ) )
			{
				CswNbtObjClassRole RoleNodeAsRole = (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode );
				if( RoleNodeAsRole.Administrator.Checked == Tristate.True || RoleNodeAsRole.Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName )
				{
					_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Quotas, RoleNodeAsRole, true );
				}
			}

		}//Update()

    }//class CswUpdateSchemaTo01J03

}//namespace ChemSW.Nbt.Schema


