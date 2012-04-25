using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25991
    /// </summary>
    public class CswUpdateSchemaCase25991 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp InventoryGroupOcp = LocationOC.getObjectClassProp( CswNbtObjClassLocation.InventoryGroupPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InventoryGroupOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, false );

        }//Update()

    }//class CswUpdateSchemaCase25991

}//namespace ChemSW.Nbt.Schema