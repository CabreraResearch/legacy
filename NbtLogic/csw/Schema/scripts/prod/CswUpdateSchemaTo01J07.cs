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
            CswNbtNode CswAdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            foreach( CswNbtView RauView in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                if( RauView.ViewMode == NbtViewRenderingMode.Tree && RauView.Visibility != NbtViewVisibility.Property )
                {
                    CswNbtView NewRauView = _CswNbtSchemaModTrnsctn.makeView();
                    NewRauView.makeNew( "Roles and Users", NbtViewVisibility.Role, CswAdminNode.NodeId, null, RauView.ViewId.get() );
                    NewRauView.save();
                    break;
                }
            }

            foreach( CswNbtView UlView in _CswNbtSchemaModTrnsctn.restoreViews( "User List" ) )
            {
                if( UlView.ViewMode == NbtViewRenderingMode.Grid && UlView.Visibility != NbtViewVisibility.Property )
                {
                    CswNbtView NewUlView = _CswNbtSchemaModTrnsctn.makeView();
                    NewUlView.makeNew( "User List", NbtViewVisibility.Role, CswAdminNode.NodeId, null, UlView.ViewId.get() );
                    NewUlView.save();
                    break;
                }
            }
            #endregion Case 23666
        }//Update()

    }//class CswUpdateSchemaTo01J07

}//namespace ChemSW.Nbt.Schema


