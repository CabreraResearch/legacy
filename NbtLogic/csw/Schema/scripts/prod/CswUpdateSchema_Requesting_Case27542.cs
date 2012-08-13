using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for Requesting_Case27542
    /// </summary>
    public class CswUpdateSchema_Requesting_Case27542 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );

            CswNbtMetaDataObjectClassProp RequestOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
            CswNbtMetaDataObjectClassProp MaterialOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
            CswNbtMetaDataObjectClassProp ContainerOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Container );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );


        }//Update()

    }//class Requesting_Case27542

}//namespace ChemSW.Nbt.Schema