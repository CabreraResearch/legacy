using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Welcome;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-11
    /// </summary>
    public class CswUpdateSchemaTo01L11 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 11 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 21214

            CswNbtNode ChemSwAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );

            CswNbtView RoleUserView = null;
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                if( View.VisibilityRoleId == ChemSwAdminRole.NodeId )
                {
                    RoleUserView = View;
                }
            }
            if( null == RoleUserView )
            {
                RoleUserView = new CswNbtView( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
                RoleUserView.makeNew( "Roles and Users", NbtViewVisibility.Role, ChemSwAdminRole.NodeId, null, Int32.MinValue );
                RoleUserView.Category = "System";

                CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClassProp RoleOcp = UserOc.getObjectClassProp( CswNbtObjClassUser.RolePropertyName );

                CswNbtViewRelationship RoleVr = RoleUserView.AddViewRelationship( RoleOc, false );
                RoleUserView.AddViewRelationship( RoleVr, CswNbtViewRelationship.PropOwnerType.Second, RoleOcp, false );
                RoleUserView.save();
            }

            CswNbtWelcomeTable WelcomeTable = new CswNbtWelcomeTable( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
            WelcomeTable.AddWelcomeItem( CswNbtWelcomeTable.WelcomeComponentType.Link,
                                            CswNbtView.ViewType.View,
                                            RoleUserView.ViewId.ToString(),
                                            Int32.MinValue,
                                            RoleUserView.ViewName,
                                            Int32.MinValue,
                                            Int32.MinValue,
                                            "silhouette.gif",
                                            ChemSwAdminRole.NodeId.ToString() );

            #endregion Case 21214

        }//Update()

    }//class CswUpdateSchemaTo01L11

}//namespace ChemSW.Nbt.Schema


