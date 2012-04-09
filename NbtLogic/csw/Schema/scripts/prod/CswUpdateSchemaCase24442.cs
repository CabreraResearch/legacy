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

            CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( InventoryGroupNT == null )
            {
                InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( InventoryGroupOC.ObjectClassId, "Inventory Group", "System" );
            }

            // Inventory Group - Description property (nodetype)
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                InventoryGroupNT,
                CswNbtMetaDataFieldType.NbtFieldType.Memo,
                "Description",
                InventoryGroupNT.getFirstNodeTypeTab().TabId );

            // Inventory Group - Locations grid property (nodetype)
            CswNbtMetaDataNodeTypeTab LocationsTab = InventoryGroupNT.getNodeTypeTab( "Locations" );
            if( LocationsTab == null )
            {
                LocationsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( InventoryGroupNT, "Locations", 2 );
            }
            CswNbtMetaDataNodeTypeProp LocationsGridProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                InventoryGroupNT,
                CswNbtMetaDataFieldType.NbtFieldType.Grid,
                "Locations",
                LocationsTab.TabId );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocationInvGrpOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.InventoryGroupPropertyName );
            CswNbtMetaDataObjectClassProp LocationBarcodeOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.BarcodePropertyName );
            CswNbtMetaDataObjectClassProp LocationNameOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
            CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

            CswNbtView LocationsGridView = _CswNbtSchemaModTrnsctn.restoreView( LocationsGridProp.ViewId );
            LocationsGridView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtViewRelationship InvGrpVR = LocationsGridView.AddViewRelationship( InventoryGroupNT, false );
            CswNbtViewRelationship LocationVR = LocationsGridView.AddViewRelationship( InvGrpVR, NbtViewPropOwnerType.Second, LocationInvGrpOCP, true );

            CswNbtViewProperty LocationBarcodeVP = LocationsGridView.AddViewProperty( LocationVR, LocationBarcodeOCP );
            CswNbtViewProperty LocationNameVP = LocationsGridView.AddViewProperty( LocationVR, LocationNameOCP );
            CswNbtViewProperty LocationLocationVP = LocationsGridView.AddViewProperty( LocationVR, LocationLocationOCP );

            LocationBarcodeVP.Order = 2; 
            LocationNameVP.Order = 4;
            LocationLocationVP.Order = 6;

            LocationsGridView.save();


            // Default view of Inventory Groups
            CswNbtView InvGrpView = _CswNbtSchemaModTrnsctn.makeView();
            InvGrpView.makeNew( "Inventory Groups", NbtViewVisibility.Global, null, null, null );
            InvGrpView.Category = "System";
            InvGrpView.ViewMode = NbtViewRenderingMode.Tree;
            InvGrpView.AddViewRelationship( InventoryGroupOC, true );
            InvGrpView.save();

        }//Update()

    }//class CswUpdateSchemaCase24442

}//namespace ChemSW.Nbt.Schema