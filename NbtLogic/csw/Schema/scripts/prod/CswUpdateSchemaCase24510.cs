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
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass );
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );

            // case 24510
            // Inventory Group Permission object class

            CswNbtMetaDataObjectClass InvGrpPermOC = _CswNbtSchemaModTrnsctn.createObjectClass(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                "tag.gif",
                true,
                false );

            CswNbtMetaDataObjectClassProp PermInvGrpOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.InventoryGroupPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                false,
                false,
                true,
                NbtViewRelatedIdType.ObjectClassId.ToString(),
                InventoryGroupOC.ObjectClassId,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.WorkUnitPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                false,
                false,
                true,
                NbtViewRelatedIdType.ObjectClassId.ToString(),
                WorkUnitOC.ObjectClassId,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.RolePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                false,
                false,
                true,
                NbtViewRelatedIdType.ObjectClassId.ToString(),
                RoleOC.ObjectClassId,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.ViewPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Logical,
                false,
                false,
                false,
                string.Empty,
                Int32.MinValue,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.EditPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Logical,
                false,
                false,
                false,
                string.Empty,
                Int32.MinValue,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.DispensePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Logical,
                false,
                false,
                false,
                string.Empty,
                Int32.MinValue,
                true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass,
                CswNbtObjClassInventoryGroupPermission.RequestPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Logical,
                false,
                false,
                false,
                string.Empty,
                Int32.MinValue,
                true );

            // case 24442
            // Inventory Group - Permissions Grid (nodetype)
            foreach( CswNbtMetaDataNodeType InventoryGroupNT in InventoryGroupOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PermissionsGripProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( InventoryGroupNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Permissions", InventoryGroupNT.getFirstNodeTypeTab().TabId );

                CswNbtView PermissionsGridView = _CswNbtSchemaModTrnsctn.restoreView(PermissionsGripProp.ViewId);
                PermissionsGridView.ViewMode = NbtViewRenderingMode.Grid;
                CswNbtViewRelationship InvGrpVR = PermissionsGridView.AddViewRelationship( InventoryGroupNT, false );
                CswNbtViewRelationship PermVR = PermissionsGridView.AddViewRelationship( InvGrpVR, NbtViewPropOwnerType.Second, PermInvGrpOCP, true );
                PermissionsGridView.save();
            }

        }//Update()

    }//class CswUpdateSchemaCase24510

}//namespace ChemSW.Nbt.Schema