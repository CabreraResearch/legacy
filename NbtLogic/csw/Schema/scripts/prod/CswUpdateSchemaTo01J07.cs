using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-07
    /// </summary>
    public class CswUpdateSchemaTo01J07 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 07 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 23666
            CswNbtNode CswAdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            CswNbtNode CswAdminUserNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            CswNbtView RoleUserView = null;
            bool RuViewExists = false;
            foreach( CswNbtView RauView in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                if( null == RoleUserView && RauView.ViewMode == NbtViewRenderingMode.Tree && RauView.Visibility != NbtViewVisibility.Property )
                {
                    RoleUserView = RauView;
                }
                RuViewExists = ( RuViewExists || RauView.VisibilityRoleId == CswAdminRoleNode.NodeId || RauView.VisibilityUserId == CswAdminUserNode.NodeId );
            }

            if( false == RuViewExists && null != RoleUserView )
            {
                CswNbtView NewRauView = _CswNbtSchemaModTrnsctn.makeView();
                NewRauView.makeNew( "Roles and Users", NbtViewVisibility.Role, CswAdminRoleNode.NodeId, null, RoleUserView.ViewId.get() );
                NewRauView.save();
            }

            CswNbtView UserView = null;
            bool UlViewExists = false;
            foreach( CswNbtView UlView in _CswNbtSchemaModTrnsctn.restoreViews( "User List" ) )
            {
                if( null == UserView && UlView.ViewMode == NbtViewRenderingMode.Grid && UlView.Visibility != NbtViewVisibility.Property )
                {
                    UserView = UlView;
                }
                UlViewExists = ( UlViewExists || UlView.VisibilityRoleId == CswAdminRoleNode.NodeId || UlView.VisibilityUserId == CswAdminUserNode.NodeId );
            }

            if( false == UlViewExists && null != UserView )
            {
                CswNbtView NewUlView = _CswNbtSchemaModTrnsctn.makeView();
                NewUlView.makeNew( "User List", NbtViewVisibility.Role, CswAdminRoleNode.NodeId, null, UserView.ViewId.get() );
                NewUlView.save();
            }
            #endregion Case 23666
        }//Update()

    }//class CswUpdateSchemaTo01J07

}//namespace ChemSW.Nbt.Schema


