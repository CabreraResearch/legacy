using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26665
    /// </summary>
    public class CswUpdateSchemaCase26665 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Create new Dispense and Dispose buttons for direct action (instead of Request)
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                CswNbtObjClassContainer.DispensePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Button );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                CswNbtObjClassContainer.DisposePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Button );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                CswNbtObjClassContainer.UndisposePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Button );

            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.UndisposeContainer, false, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.UndisposeContainer );

        }//Update()

    }//class CswUpdateSchemaCase26665

}//namespace ChemSW.Nbt.Schema