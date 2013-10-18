using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28518B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28518; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "B"; }
        }

        public override string Title
        {
            get { return "02H_Case28518B"; }
        }

        public override void update()
        {
            Int32 CISProAdminRolePK = Int32.MinValue;
            Int32 AdminRolePk = Int32.MinValue;
            Int32 ChemSWAdminRolePk = Int32.MinValue;

            // Change the visibilily of the 'Roles and Users' view to global
            CswNbtView RolesAndUsersView = _CswNbtSchemaModTrnsctn.restoreView( "Roles and Users" );
            RolesAndUsersView.SetVisibility( CswEnumNbtViewVisibility.Global, null, null );
            RolesAndUsersView.save();

            // For any roles that aren't Administrator OR "Inspection Manager" OR "Equipment Manager" roles, remove any role permissions
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false ) )
            {
                if( CswEnumTristate.True != RoleNode.Administrator.Checked && RoleNode.Name.Text != "Inspection Manager" && RoleNode.Name.Text != "Equipment Manager" )
                {
                    string RoleViewPerm = CswNbtObjClassRole.MakeNodeTypePermissionValue( RoleOC.FirstNodeType.NodeTypeId, CswEnumNbtNodeTypePermission.View );
                    RoleNode.NodeTypePermissions.RemoveValue( RoleViewPerm );
                    RoleNode.NodeTypePermissions.SyncGestalt();
                    RoleNode.postChanges( false );
                }

                if( RoleNode.Name.Text == "CISPro_Admin" )
                {
                    CISProAdminRolePK = RoleNode.NodeId.PrimaryKey;
                }
                if( RoleNode.Name.Text == "Administrator" )
                {
                    AdminRolePk = RoleNode.NodeId.PrimaryKey;
                }
                if( RoleNode.Name.Text == "chemsw_admin_role" )
                {
                    ChemSWAdminRolePk = RoleNode.NodeId.PrimaryKey;
                }
            }

            // Redirect Welcome Landingpage items
            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateLandingPageItems_Case28518", "landingpage" );
            DataTable LandingPageDt = TableUpdate.getTable( "where for_roleid in (" + CISProAdminRolePK + "," + AdminRolePk + "," + ChemSWAdminRolePk + ")" );
            foreach( DataRow CurrentRow in LandingPageDt.Rows )
            {
                if( CswConvert.ToInt32( CurrentRow["for_roleid"] ) == CISProAdminRolePK )
                {
                    if( CswConvert.ToString( CurrentRow["displaytext"] ) == "Roles and Users" )
                    {
                        CurrentRow["to_nodeviewid"] = RolesAndUsersView.ViewId.get();
                    }
                }
                else if( CswConvert.ToInt32( CurrentRow["for_roleid"] ) == AdminRolePk )
                {
                    if( CswConvert.ToString( CurrentRow["to_nodeviewid"] ) == "19" )
                    {
                        CurrentRow["to_nodeviewid"] = RolesAndUsersView.ViewId.get();
                        CurrentRow["displaytext"] = RolesAndUsersView.ViewName;
                    }
                }
                else if( CswConvert.ToInt32( CurrentRow["for_roleid"] ) == ChemSWAdminRolePk )
                {
                    if( CswConvert.ToString( CurrentRow["displaytext"] ) == "Roles and Users" )
                    {
                        CurrentRow["to_nodeviewid"] = RolesAndUsersView.ViewId.get();
                    }
                }
            }

            TableUpdate.update( LandingPageDt );

        }// update()
    }

}//namespace ChemSW.Nbt.Schema