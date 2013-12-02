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

        public override string AppendToScriptName()
        {
            return "B_V2";
        }

        public override string Title
        {
            get { return "02H_Case28518B"; }
        }

        public override void update()
        {
            Int32 AdminRolePk = Int32.MinValue;
            CswCommaDelimitedString AdminRoles = new CswCommaDelimitedString();

            // Change the visibilily of the 'Roles and Users' view to global
            CswNbtView RolesAndUsersView = _CswNbtSchemaModTrnsctn.restoreView( "Roles and Users" );
            if( null != RolesAndUsersView )
            {
                RolesAndUsersView.SetVisibility( CswEnumNbtViewVisibility.Global, null, null );
                RolesAndUsersView.save();

                // For any roles that aren't Administrator roles, remove any role permissions
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
                foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false ) )
                {
                    if( CswEnumTristate.True != RoleNode.Administrator.Checked )
                    {
                        foreach( CswEnumNbtNodeTypePermission Permission in CswEnumNbtNodeTypePermission.Members )
                        {
                            RoleNode.NodeTypePermissions.RemoveValue( CswNbtObjClassRole.MakeNodeTypePermissionValue( RoleOC.FirstNodeType.NodeTypeId, Permission ) );
                        }
                        RoleNode.NodeTypePermissions.SyncGestalt();
                        RoleNode.postChanges( false );
                    }
                    else
                    {
                        AdminRoles.Add( CswConvert.ToString( RoleNode.NodeId.PrimaryKey ) );
                    }

                    // We need this because setting the landing page for this Role is a special case
                    if( RoleNode.Name.Text == "Administrator" )
                    {
                        AdminRolePk = RoleNode.NodeId.PrimaryKey;
                    }
                }

                // Redirect Welcome Landingpage items
                CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateLandingPageItems_Case28518", "landingpage" );
                DataTable LandingPageDt = TableUpdate.getTable( "where for_roleid in (" + AdminRoles.ToString() + ")" );
                foreach( DataRow CurrentRow in LandingPageDt.Rows )
                {
                    if( CswConvert.ToInt32( CurrentRow["for_roleid"] ) == AdminRolePk )
                    {
                        if( CswConvert.ToString( CurrentRow["to_nodeviewid"] ) == "19" )
                        {
                            CurrentRow["displaytext"] = RolesAndUsersView.ViewName;
                        }
                    }

                    if( CswConvert.ToString( CurrentRow["displaytext"] ) == "Roles and Users" )
                    {
                        CurrentRow["to_nodeviewid"] = RolesAndUsersView.ViewId.get();
                    }

                }

                TableUpdate.update( LandingPageDt );
            }//if( null != RolesAndUsersView )

        }// update()
    }

}//namespace ChemSW.Nbt.Schema