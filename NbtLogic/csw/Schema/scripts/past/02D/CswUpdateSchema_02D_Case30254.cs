using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30254 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30254; }
        }

        public override void update()
        {
            // CISPro_Admin needs to be able to Edit, Delete, View, and Create Regulatory List List Code nodes
            CswEnumNbtNodeTypePermission[] NTPermissions = 
            { 
                CswEnumNbtNodeTypePermission.View, 
                CswEnumNbtNodeTypePermission.Create, 
                CswEnumNbtNodeTypePermission.Edit, 
                CswEnumNbtNodeTypePermission.Delete
            };

            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != RoleNode )
            {
                CswNbtMetaDataNodeType RegulatoryListListCodeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List List Code" );
                if( null != RegulatoryListListCodeNT )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( NTPermissions, RegulatoryListListCodeNT, RoleNode, true );
                }
            }

            // This is a placeholder script that does nothing.
        } // update()

    }//CswUpdateSchema_02D_Case30254

}//namespace ChemSW.Nbt.Schema