using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30299: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30299; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
            if( null != SiteNT )
            {
                foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, true, false, true ) )
                {
                    string SiteViewPermissionVal = CswNbtObjClassRole.MakeNodeTypePermissionValue( SiteNT.NodeTypeId, CswEnumNbtNodeTypePermission.View );
                    if( false == RoleNode.NodeTypePermissions.CheckValue( SiteViewPermissionVal ) )
                    {
                        RoleNode.NodeTypePermissions.AddValue( SiteViewPermissionVal );
                        RoleNode.postChanges( false );
                    }
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema