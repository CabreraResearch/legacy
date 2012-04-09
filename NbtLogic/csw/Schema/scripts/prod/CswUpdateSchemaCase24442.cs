using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24442
    /// </summary>
    public class CswUpdateSchemaCase24442 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );

            // Inventory Group - Name property (object class)
            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass,
                CswNbtObjClassInventoryGroup.NamePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Text,
                false,
                false,
                false,
                string.Empty,
                Int32.MinValue,
                true,
                true );

            // Inventory Group - Description property (nodetype)
            CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( InventoryGroupNT == null )
            {
                InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InventoryGroupOC.ObjectClassId, "Inventory Group", "System" );
            }
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                InventoryGroupNT,
                CswNbtMetaDataFieldType.NbtFieldType.Memo,
                "Description",
                InventoryGroupNT.getFirstNodeTypeTab().TabId );




        }//Update()

    }//class CswUpdateSchemaCase24442

}//namespace ChemSW.Nbt.Schema