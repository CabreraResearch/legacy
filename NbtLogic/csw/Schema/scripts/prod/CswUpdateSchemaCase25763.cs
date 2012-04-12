using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase25763 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );

            CswNbtMetaDataObjectClassProp CompatabilityProp = MaterialOc.getObjectClassProp( "Storage Capacity" );//sic
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CompatabilityProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, ChemSW.Nbt.ObjClasses.CswNbtObjClassMaterial.StorageCompatibilityPropName );//sic.


        }//Update()

    }//class CswUpdateSchemaCase25763

}//namespace ChemSW.Nbt.Schema