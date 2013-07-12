using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29861
    /// </summary>
    public class CswUpdateSchema_02C_Case29861: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29861; }
        }

        public override void update()
        {
            //Remove the Modules action permission from every role except ChemSW_Admin
            string ModuleActionValue = CswNbtObjClassRole.MakeActionPermissionValue( _CswNbtSchemaModTrnsctn.Actions[CswEnumNbtActionName.Modules] );
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false, true ) )
            {
                if( RoleNode.Name.Text != CswNbtObjClassRole.ChemSWAdminRoleName )
                {
                    if( RoleNode.ActionPermissions.CheckValue( ModuleActionValue ) )
                    {
                        RoleNode.ActionPermissions.RemoveValue( ModuleActionValue );
                        RoleNode.postChanges( false );
                    }
                }
                else
                {
                    if( false == RoleNode.ActionPermissions.CheckValue( ModuleActionValue ) )
                    {
                        RoleNode.ActionPermissions.AddValue( ModuleActionValue );
                        RoleNode.postChanges( false );
                    }
                }
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29861

}//namespace ChemSW.Nbt.Schema