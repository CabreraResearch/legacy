using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-33
	/// </summary>
	public class CswUpdateSchemaTo01H33 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 33 ); } }
		public CswUpdateSchemaTo01H33( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21210 - 'chemsw_admin' user account

			CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
			CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

			CswNbtMetaDataNodeType RoleNT = RoleOC.FirstNodeType;
			CswNbtMetaDataNodeType UserNT = UserOC.FirstNodeType;

			// add 'chemsw_admin' role
			CswNbtNode CswAdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( RoleNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
			CswNbtObjClassRole CswAdminRole = CswNbtNodeCaster.AsRole( CswAdminRoleNode );
	
			// grant Administrator to 'chemsw_admin' role
			CswAdminRole.Administrator.Checked = Tristate.True;
			CswAdminRole.Name.Text = "chemsw_admin_role";
			CswAdminRole.Timeout.Value = 30;

			// add all permissions to 'chemsw_admin' role
			foreach( CswNbtMetaDataNodeType OtherNodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
			{
				CswNbtNodeTypePermissions Perms = _CswNbtSchemaModTrnsctn.getNodeTypePermissions( CswAdminRole, OtherNodeType );
				Perms.Create = true;
				Perms.View = true;
				Perms.Edit = true;
				Perms.Delete = true;
				Perms.Save();
			} // foreach( CswNbtMetaDataNodeType OtherNodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )

			// add all actions to 'chemsw_admin' role
			foreach( CswNbtActionName ActionName in Enum.GetValues( typeof( Actions.CswNbtActionName ) ) )
			{
				_CswNbtSchemaModTrnsctn.SetActionPermission( CswAdminRoleNode, ActionName, true );
			}
						
			// add 'chemsw_admin' user
			CswNbtNode CswAdminUserNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UserNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
			CswNbtObjClassUser CswAdminUser = CswNbtNodeCaster.AsUser( CswAdminUserNode );
			CswAdminUser.Role.RelatedNodeId = CswAdminRoleNode.NodeId;
			CswAdminUser.FirstNameProperty.Text = "ChemSW";
			CswAdminUser.LastNameProperty.Text = "Admin";
			CswAdminUser.UsernameProperty.Text = "chemsw_admin";
			CswAdminUser.PasswordProperty.Password = "chemsw123";
			// set new user password to be expired
			CswAdminUser.PasswordProperty.ChangedDate = new DateTime( 2000, 1, 1 );

			// remove Design from existing 'admin' role
			CswNbtNode OldAdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
			_CswNbtSchemaModTrnsctn.SetActionPermission( OldAdminRoleNode, CswNbtActionName.Design, false );

			// post changes
			CswAdminRoleNode.postChanges( false );
			CswAdminUserNode.postChanges( false );
			OldAdminRoleNode.postChanges( false );

		} // update()

	}//class CswUpdateSchemaTo01H33

}//namespace ChemSW.Nbt.Schema

