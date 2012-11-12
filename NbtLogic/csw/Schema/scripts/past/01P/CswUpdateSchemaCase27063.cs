using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27063
    /// </summary>
    public class CswUpdateSchemaCase27063 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContDispTransOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass );

            foreach( CswNbtMetaDataNodeType ContDispTransNt in ContDispTransOc.getNodeTypes() )
            {
                CswNbtMetaDataObjectClass RoleOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );

                foreach( CswNbtNode RoleNodeInstance in RoleOc.getNodes( false, false ) )
                {
                    CswNbtObjClassRole RoleNode = RoleNodeInstance;
                    if( RoleNode.Name.Text != CswNbtObjClassRole.ChemSWAdminRoleName )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Delete, ContDispTransNt, RoleNode, false );
                    }
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27063

}//namespace ChemSW.Nbt.Schema