using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28714
    /// </summary>
    public class CswUpdateSchema_01W_Case28714 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28714; }
        }

        public override void update()
        {

            CswNbtObjClassRole AdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != AdminRoleNode )
            {

                bool CanCreateMaterial = _CswNbtSchemaModTrnsctn.Permit.can( CswNbtActionName.Create_Material, AdminRoleNode );
                if( CanCreateMaterial )
                {
                    bool HasOneMaterialCreate = false;
                    CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                    foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
                    {
                        string NodeTypePermission = _makeNodeTypePermissionValue( MaterialNt.FirstVersionNodeTypeId, CswNbtPermit.NodeTypePermission.Create );
                        HasOneMaterialCreate = AdminRoleNode.NodeTypePermissions.CheckValue( NodeTypePermission );
                    }

                    if( false == HasOneMaterialCreate )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Create_Material, AdminRoleNode, false );
                    }
                }
            }

        } //Update()

        private static string _makeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswNbtPermit.NodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }

    }//class CswUpdateSchema_01V_Case28714

}//namespace ChemSW.Nbt.Schema