
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase25875 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createObjectClassProp( MetaData.CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass, "Vendor Name", MetaData.CswNbtMetaDataFieldType.NbtFieldType.Text );
        }//Update()

    }//class CswUpdateSchemaCase25875

}//namespace ChemSW.Nbt.Schema