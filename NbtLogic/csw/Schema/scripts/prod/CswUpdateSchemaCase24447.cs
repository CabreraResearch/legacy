using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24447
    /// </summary>
    public class CswUpdateSchemaCase24447 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Create_Material, true, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.Create_Material );

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            foreach( CswNbtNode UserNode in UserOc.getNodes( true, false ) )
            {
                bool HasOneMaterialCreate = false;
                CswNbtObjClassUser UserNodeAsOc = CswNbtNodeCaster.AsUser( UserNode );
                foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
                {
                    HasOneMaterialCreate = HasOneMaterialCreate ||
                                           _CswNbtSchemaModTrnsctn.Permit.can( CswNbtPermit.NodeTypePermission.Create,
                                                                               MaterialNt, false, null, UserNodeAsOc );
                }
                if( HasOneMaterialCreate )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Create_Material, UserNodeAsOc, true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase24447

}//namespace ChemSW.Nbt.Schema