using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30370 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30370; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false ) )
            {
                _setNTPermissionsForOC( CswEnumNbtObjectClass.ReportGroupClass, RoleNode, RoleNode.Administrator.Checked == CswEnumTristate.True );
                _setNTPermissionsForOC( CswEnumNbtObjectClass.ReportGroupPermissionClass, RoleNode, RoleNode.Administrator.Checked == CswEnumTristate.True );
                _setNTPermissionsForOC( CswEnumNbtObjectClass.MailReportGroupClass, RoleNode, RoleNode.Administrator.Checked == CswEnumTristate.True );
                _setNTPermissionsForOC( CswEnumNbtObjectClass.MailReportGroupPermissionClass, RoleNode, RoleNode.Administrator.Checked == CswEnumTristate.True );
            }
        }

        private void _setNTPermissionsForOC( CswEnumNbtObjectClass OCName, CswNbtObjClassRole Role, bool AllowEdit )
        {
            CswNbtMetaDataObjectClass OC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( OCName );
            foreach( CswNbtMetaDataNodeType NT in OC.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, NT, Role, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, NT, Role, AllowEdit );
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, NT, Role, AllowEdit );
                _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, NT, Role, AllowEdit );
            }
        }
    }

}//namespace ChemSW.Nbt.Schema