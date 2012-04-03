using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24463
    /// </summary>
    public class CswUpdateSchemaCase24463 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass, "contact.gif", true, false );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                           CswNbtObjClassSize.MaterialPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false,
                                                           false,
                                                           true,
                                                           NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                           MaterialOc.ObjectClassId );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                           CswNbtObjClassSize.CapacityPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                           CswNbtObjClassSize.QuantityEditablePropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                           CswNbtObjClassSize.DispensablePropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Logical );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass.ToString(), "Size", "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, SizeOc.ObjectClassId );

        }//Update()

    }//class CswUpdateSchemaCase24463

}//namespace ChemSW.Nbt.Schema