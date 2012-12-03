using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24446Action
    /// </summary>
    public class CswUpdateSchemaCase24446Action : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Receiving, false, "", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.Receiving );

            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.CatalogNoPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassMaterial.ReceivePropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button
            } );

            CswNbtMetaDataObjectClassProp CapacityProp = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( SizeOc.ObjectClassId, CswNbtObjClassSize.InitialQuantityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            
        }//Update()

    }//class CswUpdateSchemaCase24446Action

}//namespace ChemSW.Nbt.Schema