using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for RequestingCase27470
    /// </summary>
    public class CswUpdateSchema_Requesting_Case27470 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClassProp DoomedRequestInventoryGroupOcp = RequestOc.getObjectClassProp( "Inventory Group" );
            if( null != DoomedRequestInventoryGroupOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( DoomedRequestInventoryGroupOcp, DeleteNodeTypeProps: true );
            }

            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClassProp DoomedItemInventoryGroupOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( DoomedItemInventoryGroupOcp, DeleteNodeTypeProps: true );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOc )
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = InventoryGroupOc.ObjectClassId,
                    IsRequired = true,
                    SetValOnAdd = true
                } );

        }//Update()

    }//class CswUpdateSchema_Requesting_Case27470

}//namespace ChemSW.Nbt.Schema