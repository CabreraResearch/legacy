
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514RequestButton : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ContainerRequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassContainer.RequestPropertyName
                } );
            foreach( CswNbtMetaDataNodeType Container in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestNtp = Container.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.RequestPropertyName );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true );
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp MaterialRequestOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                MaterialOc, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    PropName = CswNbtObjClassMaterial.RequestPropertyName
                } );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.RequestPropertyName );
                RequestNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table, true );
            }

        }//Update()

    }//class CswUpdateSchemaCase24514RequestButton

}//namespace ChemSW.Nbt.Schema