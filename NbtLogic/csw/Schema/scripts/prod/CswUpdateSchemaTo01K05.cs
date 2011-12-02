using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-05
    /// </summary>
    public class CswUpdateSchemaTo01K05 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 05 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            // Case 24294
            // Remove deprecated Inspection Actions

            CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K05_actions_update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "where actionname = 'Assign Inspection' or actionname = 'Inspection Design' " );
            foreach( DataRow ActionRow in ActionTable.Rows )
            {
                string ActionId = CswConvert.ToString( ActionRow["actionid"] );
                CswTableUpdate JctModulesActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01K05_jctmodulesactions_update", "jct_modules_actions" );
                DataTable JmaTable = JctModulesActionUpdate.getTable( "where actionid = ' " + ActionId + "' " );
                foreach( DataRow JctRow in JmaTable.Rows )
                {
                    JctRow.Delete();
                }
                JctModulesActionUpdate.update( JmaTable );
                ActionRow.Delete();
            }
            ActionUpdate.update( ActionTable );

            // Case 24288: Limit Wizard to ChemSW_Admin
            CswNbtAction CreateInspection = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Create_Inspection];
            CreateInspection.Url = ""; //Clear this while we're here
            CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            foreach( CswNbtNode RoleNode in RoleOc.getNodes( true, false ) )
            {
                CswNbtObjClassRole Role = CswNbtNodeCaster.AsRole( RoleNode );
                bool CanEdit = Role.Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName;
                _CswNbtSchemaModTrnsctn.Permit.set( CreateInspection, Role, CanEdit );
            }

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtNode UserNode in UserOc.getNodes( true, false ) )
            {
                CswNbtObjClassUser User = CswNbtNodeCaster.AsUser( UserNode );
                bool CanEdit = User.Username == CswNbtObjClassUser.ChemSWAdminUsername;
                _CswNbtSchemaModTrnsctn.Permit.set( CreateInspection, User, CanEdit );
            }

        }//Update()

    }//class CswUpdateSchemaTo01K05

}//namespace ChemSW.Nbt.Schema


