using ChemSW.Core;
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
            // Change the visibilily of the 'Roles and Users' view to global
            CswNbtView RolesAndUsersView = _CswNbtSchemaModTrnsctn.restoreView( "Roles and Users" );
            RolesAndUsersView.SetVisibility( CswEnumNbtViewVisibility.Global, null, null );
            RolesAndUsersView.save();

            // For any roles that aren't Administrator OR "Inspection Manager" OR "Equipment Manager" roles, remove any role permissions
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false ) )
            {
                if( CswEnumTristate.True != RoleNode.Administrator.Checked && RoleNode.NodeName != "Inspection Manager" && RoleNode.NodeName != "Equipment Manager" )
                {
                    string RoleViewPerm = CswNbtObjClassRole.MakeNodeTypePermissionValue( RoleOC.FirstNodeType.NodeTypeId, CswEnumNbtNodeTypePermission.View );
                    RoleNode.NodeTypePermissions.RemoveValue( RoleViewPerm );
                    RoleNode.NodeTypePermissions.SyncGestalt();
                    RoleNode.postChanges( false );
                }
            }

        }// update()
    }

}//namespace ChemSW.Nbt.Schema