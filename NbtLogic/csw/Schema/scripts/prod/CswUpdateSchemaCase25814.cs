using ChemSW.Nbt.Actions;
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

            CswNbtMetaDataObjectClassProp UndisposeProp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp(
                    InvGrpPermObjClass,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassInventoryGroupPermission.UndisposePropertyName,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical
                    }
                );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp DisposedProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( ContainerOC.ObjectClassId, CswNbtObjClassContainer.DisposedPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisposedProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            CswNbtMetaDataObjectClassProp ContainerDispenseProp = ContainerOC.getObjectClassProp( "Dispense" );
            if( ContainerDispenseProp != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                    ContainerDispenseProp,
                    CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname,
                    CswNbtObjClassContainer.RequestPropertyName );
            }

            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.DisposeContainer, false, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.DisposeContainer );

        }//Update()

    }//class CswUpdateSchemaCase25814

}//namespace ChemSW.Nbt.Schema