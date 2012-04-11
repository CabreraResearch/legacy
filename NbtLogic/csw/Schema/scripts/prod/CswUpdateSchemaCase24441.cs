using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24441
    /// </summary>
    public class CswUpdateSchemaCase24441 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
   
            // Location - Inventory Group object class property
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass,
                CswNbtObjClassInventoryGroupPermission.InventoryGroupPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                false,
                false,
                true,
                NbtViewRelatedIdType.ObjectClassId.ToString(),
                InventoryGroupOC.ObjectClassId,
                true );

        }//Update()

    }//class CswUpdateSchemaCase24441

}//namespace ChemSW.Nbt.Schema