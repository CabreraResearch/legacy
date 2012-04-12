using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24510
    /// </summary>
    public class CswUpdateSchemaCase24510 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // case 24510
            // Inventory Group Permission object class

            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtSchemaModTrnsctn.createObjectClass(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                "tag.gif",
                true,
                false );

        }//Update()

    }//class CswUpdateSchemaCase24510

}//namespace ChemSW.Nbt.Schema