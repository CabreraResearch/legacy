using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24510 part C
    /// </summary>
    public class CswUpdateSchemaCase24510C : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataObjectClass InventoryGroupPermOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass );

            // default Inventory Group Permission nodetype
            CswNbtMetaDataNodeType InvGrpPermNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InventoryGroupPermOC.ObjectClassId, "Inventory Group Permission", "System" );
            InvGrpPermNT.IconFileName = "tag.gif";
            InvGrpPermNT.setNameTemplateText(
                CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInventoryGroupPermission.InventoryGroupPropertyName ) + "-" +
                CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInventoryGroupPermission.RolePropertyName ) + "-" +
                CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassInventoryGroupPermission.WorkUnitPropertyName ) );


            // default Work Unit nodetype
            CswNbtMetaDataNodeType WorkUnitNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( WorkUnitOC.ObjectClassId, "Work Unit", "System" );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( WorkUnitNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", WorkUnitNT.getFirstNodeTypeTab().TabId );
            WorkUnitNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Name" ) );

            // default Work Unit view
            CswNbtView WorkUnitView = _CswNbtSchemaModTrnsctn.makeView();
            WorkUnitView.makeNew( "Work Units", NbtViewVisibility.Global, null, null, null );
            WorkUnitView.Category = "System";
            WorkUnitView.AddViewRelationship( WorkUnitOC, true );
            WorkUnitView.save();

            // Add inventory group object classes to CISPro
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, InventoryGroupOC.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, InventoryGroupPermOC.ObjectClassId );

        }//Update()

    }//class CswUpdateSchemaCase24510C

}//namespace ChemSW.Nbt.Schema