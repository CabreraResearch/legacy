using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25814
    /// </summary>
    public class CswUpdateSchemaCase25814 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InvGrpPermObjClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass );

            CswNbtMetaDataObjectClassProp DisposeProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    InvGrpPermObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassInventoryGroupPermission.DisposePropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical
                    }
                );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp DisposedProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( ContainerOC.ObjectClassId, CswNbtObjClassContainer.DisposedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisposedProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

        }//Update()

    }//class CswUpdateSchemaCase25814

}//namespace ChemSW.Nbt.Schema