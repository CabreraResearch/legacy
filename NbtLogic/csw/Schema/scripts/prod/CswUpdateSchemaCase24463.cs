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
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass, "", true, false );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            CswNbtMetaDataObjectClassProp MaterialOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                               CswNbtObjClassSize.MaterialPropertyName,
                                                               CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                               false,
                                                               false,
                                                               true,
                                                               NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                               MaterialOc.ObjectClassId );

            CswNbtMetaDataObjectClassProp CapacityOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                               CswNbtObjClassSize.CapacityPropertyName,
                                                               CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            CswNbtMetaDataObjectClassProp QuantityEditableOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                               CswNbtObjClassSize.QuantityEditablePropertyName,
                                                               CswNbtMetaDataFieldType.NbtFieldType.Logical );

            CswNbtMetaDataObjectClassProp DispensableOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass,
                                                               CswNbtObjClassSize.DispensablePropertyName,
                                                               CswNbtMetaDataFieldType.NbtFieldType.Logical );


        }//Update()

    }//class CswUpdateSchemaCase24463

}//namespace ChemSW.Nbt.Schema