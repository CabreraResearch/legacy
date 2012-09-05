
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27647
    /// </summary>
    public class CswUpdateSchemaCase27647 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp UnitCountOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SizeOc )
            {
                PropName = CswNbtObjClassSize.PropertyName.UnitCount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                IsRequired = true,
                SetValOnAdd = true,
                NumberMinValue = 1,
                NumberPrecision = 0
            } );
        }//Update()

    }//class CswUpdateSchemaCase27647

}//namespace ChemSW.Nbt.Schema