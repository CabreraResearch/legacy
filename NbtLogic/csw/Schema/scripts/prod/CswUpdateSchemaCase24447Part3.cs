using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24447Part3
    /// </summary>
    public class CswUpdateSchemaCase24447Part3 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            CswNbtMetaDataObjectClassProp CapacityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

        }//Update()

    }//class CswUpdateSchemaCase24447Part3

}//namespace ChemSW.Nbt.Schema