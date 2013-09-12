using System;
using System.Collections.Generic;
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

        public override string ScriptName
        {
            get { return "02F_Case30043_Locations"; }
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
            const string LocationSheetName = "locations";

            // Import order is based on these
            List<Tuple<string, Int32>> DestNodeTypesAndInstances = new List<Tuple<string, int>>();
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( SiteNTName, 1 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( BuildingNTName, 2 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( RoomNTName, 3 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( CabinetNTName, 4 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( ShelfNTName, 5 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( BoxNTName, 6 ) );

            CswNbtSchemaUpdateImportMgr LocationsMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, LocationSheetName, DestNodeTypesAndInstances, "locations_view" );

            // Bindings
            LocationsMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "siteid", "Legacy ID", "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, SiteNTName );

            LocationsMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "inventorygroup", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName );

            LocationsMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "inventorygroup", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName );

            LocationsMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "inventorygroup", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName );

            LocationsMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "inventorygroup", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName );

            LocationsMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "inventorygroup", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "controlzone", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName );

            //Relationships
            LocationsMgr.importRelationship( LocationSheetName, BuildingNTName, CswNbtObjClassLocation.PropertyName.Location, 1, "siteid" );

            LocationsMgr.importRelationship( LocationSheetName, RoomNTName, CswNbtObjClassLocation.PropertyName.Location, 2, "buildingid" );

            LocationsMgr.importRelationship( LocationSheetName, CabinetNTName, CswNbtObjClassLocation.PropertyName.Location, 3, "roomid" );

            LocationsMgr.importRelationship( LocationSheetName, ShelfNTName, CswNbtObjClassLocation.PropertyName.Location, 4, "cabinetid" );

            LocationsMgr.importRelationship( LocationSheetName, BoxNTName, CswNbtObjClassLocation.PropertyName.Location, 5, "shelfid" );

            LocationsMgr.finalize( null, null, true );

            #region Old code

            //{
            //    //select l.*, l1.locationlevel1name, l1.locationlevel1id, s.siteid
            //    //from locations l
            //    //join locations_level1 l1 on (l1.locationlevel1id = l.locationlevel1id)
            //    //join sites s on (s.siteid = l1.siteid)

            //    CswNbtSchemaUpdateImportMgr BuildingImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, LocationSheetName, "Building", "locationslevel1_view" );

            //    BuildingImpMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "" );
            //    BuildingImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

            //    // Relationships
            //    BuildingImpMgr.importBinding( "siteid", CswNbtObjClassLocation.PropertyName.Location, "" );
            //    BuildingImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
            //    BuildingImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

            //    // Legacy Id
            //    BuildingImpMgr.importBinding( "locationid", "Legacy Id", "" );

            //    BuildingImpMgr.finalize( null, null, true );
            //}

            //{
            //    //select l.*, l2.locationlevel2name
            //    //from locations l
            //    //join locations_level2 l2 on (l2.locationlevel2id = l.locationlevel2id)

            //    CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level2", "Room", "locationslevel2_view" );

            //    RoomImpMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "" );
            //    RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

            //    // Relationships
            //    RoomImpMgr.importBinding( "locationlevel1id", CswNbtObjClassLocation.PropertyName.Location, "" );
            //    RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
            //    RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

            //    RoomImpMgr.finalize();
            //}

            //{
            //    //select l.*, l3.locationlevel3name
            //    //from locations l
            //    //join locations_level3 l3 on (l3.locationlevel3id = l.locationlevel3id)

            //    CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level3", "Cabinet", "locationslevel3_view" );

            //    RoomImpMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "" );
            //    RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

            //    // Relationships
            //    RoomImpMgr.importBinding( "locationlevel2id", CswNbtObjClassLocation.PropertyName.Location, "" );
            //    RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
            //    RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

            //    RoomImpMgr.finalize();
            //}

            //{
            //    //select l.*, l4.locationlevel3name
            //    //from locations l
            //    //join locations_level4 l4 on (l4.locationlevel4id = l.locationlevel4id)

            //    CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level4", "Shelf", "locationslevel4_view" );

            //    RoomImpMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "" );
            //    RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

            //    // Relationships
            //    RoomImpMgr.importBinding( "locationlevel3id", CswNbtObjClassLocation.PropertyName.Location, "" );
            //    RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
            //    RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

            //    RoomImpMgr.finalize();
            //}

            //{
            //    //select l.*, l5.locationlevel3name
            //    //from locations l
            //    //join locations_level5 l5 on (l5.locationlevel5id = l.locationlevel5id)

            //    CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level5", "Box", "locationslevel5_view" );

            //    RoomImpMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "" );
            //    RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

            //    // Relationships
            //    RoomImpMgr.importBinding( "locationlevel4id", CswNbtObjClassLocation.PropertyName.Location, "" );
            //    RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
            //    RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

            //    RoomImpMgr.finalize();
            //}

            #endregion

        } // update()

    } // class CswUpdateSchema_02F_Case30043_Locations

}//namespace ChemSW.Nbt.Schema