using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25148
    /// </summary>
    public class CswUpdateSchemaCase25148 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass VendorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );
            CswNbtMetaDataNodeType VendorNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
            if( null != VendorNt )
            {
                _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( VendorNt, VendorOc );
            }
            else
            {
                VendorNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( VendorOc.ObjectClassId, "Supplier", "Materials" );
            }

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SupplierProp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );
                SupplierProp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), VendorOc.ObjectClassId );
            }
        }//Update()

    }//class CswUpdateSchemaCase25148

}//namespace ChemSW.Nbt.Schema