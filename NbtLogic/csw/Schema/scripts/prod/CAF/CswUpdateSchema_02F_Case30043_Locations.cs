using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_Locations : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30043; }
        }

        public override string AppendToScriptName()
        {
            return "Locations";
        }

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units
            const string SiteNTName = "Site";
            const string BuildingNTName = "Building";
            const string RoomNTName = "Room";
            const string CabinetNTName = "Cabinet";
            const string ShelfNTName = "Shelf";
            const string BoxNTName = "Box";
            const string LocationSheetName = "CAF";


            CswNbtSchemaUpdateImportMgr LocationsMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            // Bindings

            #region Site
            LocationsMgr.CAFimportOrder( SiteNTName, "locations", "locations_view", "locationid", false, 1 );
            LocationsMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, SiteNTName, 1 );
            LocationsMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, SiteNTName, 1 );
            LocationsMgr.importBinding( "siteid", "Legacy Id", "", LocationSheetName, SiteNTName, 1 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, SiteNTName, 1 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, SiteNTName, 1 );
            #endregion

            #region Building
            LocationsMgr.CAFimportOrder( BuildingNTName, "locations", "locations_view", "locationid", false, 2 );

            LocationsMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BuildingNTName, 2 );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BuildingNTName, 2 );
            LocationsMgr.importBinding( "buildingid", "Legacy Id", "", LocationSheetName, BuildingNTName, 2 );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName, 2 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName, 2 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, BuildingNTName, 2 );
            #endregion

            #region Room
            LocationsMgr.CAFimportOrder( RoomNTName, "locations", "locations_view", "locationid", false, 3 );

            LocationsMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, RoomNTName, 3 );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, RoomNTName, 3 );
            LocationsMgr.importBinding( "roomid", "Legacy Id", "", LocationSheetName, RoomNTName, 3 );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName, 3 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName, 3 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, RoomNTName, 3 );
            #endregion

            #region Cabinet
            LocationsMgr.CAFimportOrder( CabinetNTName, "locations", "locations_view", "locationid", false, 4 );

            LocationsMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, CabinetNTName, 4 );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, CabinetNTName, 4 );
            LocationsMgr.importBinding( "cabinetid", "Legacy Id", "", LocationSheetName, CabinetNTName, 4 );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName, 4 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName, 4 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, CabinetNTName, 4 );
            #endregion

            #region Shelf
            LocationsMgr.CAFimportOrder( ShelfNTName, "locations", "locations_view", "locationid", false, 5 );

            LocationsMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, ShelfNTName, 5 );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, ShelfNTName, 5 );
            LocationsMgr.importBinding( "shelfid", "Legacy Id", "", LocationSheetName, ShelfNTName, 5 );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName, 5 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName, 5 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, ShelfNTName, 5 );
            #endregion

            #region Box
            LocationsMgr.CAFimportOrder( BoxNTName, "locations", "locations_view", "locationid", false, 6 );

            LocationsMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BoxNTName, 6 );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BoxNTName, 6 );
            LocationsMgr.importBinding( "boxid", "Legacy Id", "", LocationSheetName, BoxNTName, 6 );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName, 6 );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName, 6 );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "", LocationSheetName, BoxNTName, 6 );
            #endregion

            //Relationships
            LocationsMgr.importRelationship( LocationSheetName, BuildingNTName, CswNbtObjClassLocation.PropertyName.Location, 1, "siteid" );

            LocationsMgr.importRelationship( LocationSheetName, RoomNTName, CswNbtObjClassLocation.PropertyName.Location, 2, "buildingid" );

            LocationsMgr.importRelationship( LocationSheetName, CabinetNTName, CswNbtObjClassLocation.PropertyName.Location, 3, "roomid" );

            LocationsMgr.importRelationship( LocationSheetName, ShelfNTName, CswNbtObjClassLocation.PropertyName.Location, 4, "cabinetid" );

            LocationsMgr.importRelationship( LocationSheetName, BoxNTName, CswNbtObjClassLocation.PropertyName.Location, 5, "shelfid" );

            LocationsMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30043_Locations

}//namespace ChemSW.Nbt.Schema