using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26572
    /// </summary>
    public class CswUpdateSchemaCase26572 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // container size relationship
            CswNbtMetaDataObjectClass ocSize = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp sizeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass,
                "Size", CswNbtMetaDataFieldType.NbtFieldType.Relationship, IsRequired: true, IsFk: true,
                FkType: NbtViewRelatedIdType.ObjectClassId.ToString(), FkValue: ocSize.ObjectClassId );

        }//Update()

    }//class CswUpdateSchemaCase26572

}//namespace ChemSW.Nbt.Schema