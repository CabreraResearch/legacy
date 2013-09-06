using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
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

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units

            {
                //select l.*, l1.locationlevel1name, l1.locationlevel1id, s.siteid
                //from locations l
                //join locations_level1 l1 on (l1.locationlevel1id = l.locationlevel1id)
                //join sites s on (s.siteid = l1.siteid)

                CswNbtSchemaUpdateImportMgr BuildingImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level1", "Building", "locationslevel1_view" );

                BuildingImpMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "" );
                BuildingImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

                // Relationships
                BuildingImpMgr.importBinding( "siteid", CswNbtObjClassLocation.PropertyName.Location, "" );
                BuildingImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
                BuildingImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

                // Column names
                //deleted
                //inventorygroupid
                //istransferlocaiton
                //locationid
                //locationlevel1id
                //locationlevel2id
                //locationlevel3id
                //locationlevel4id
                //locationlevel5id
                //reqdeliverylocation
                //selfserveocde
                //ishomelocation
                //controlzoneid
                //descript
                //pathname
                //locationisinactive
                //sglncode
                //rfid
                //locationcode
                //locationlevel1name
                //siteid   

                BuildingImpMgr.finalize();
            }

            {
                //select l.*, l2.locationlevel2name
                //from locations l
                //join locations_level2 l2 on (l2.locationlevel2id = l.locationlevel2id)

                CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level2", "Room", "locationslevel2_view" );

                RoomImpMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "" );
                RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

                // Relationships
                RoomImpMgr.importBinding( "locationlevel1id", CswNbtObjClassLocation.PropertyName.Location, "" );
                RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
                RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

                RoomImpMgr.finalize();
            }

            {
                //select l.*, l3.locationlevel3name
                //from locations l
                //join locations_level3 l3 on (l3.locationlevel3id = l.locationlevel3id)

                CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level3", "Cabinet", "locationslevel3_view" );

                RoomImpMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "" );
                RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

                // Relationships
                RoomImpMgr.importBinding( "locationlevel2id", CswNbtObjClassLocation.PropertyName.Location, "" );
                RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
                RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

                RoomImpMgr.finalize();
            }

            {
                //select l.*, l4.locationlevel3name
                //from locations l
                //join locations_level4 l4 on (l4.locationlevel4id = l.locationlevel4id)

                CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level4", "Shelf", "locationslevel4_view" );

                RoomImpMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "" );
                RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

                // Relationships
                RoomImpMgr.importBinding( "locationlevel3id", CswNbtObjClassLocation.PropertyName.Location, "" );
                RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
                RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

                RoomImpMgr.finalize();
            }

            {
                //select l.*, l5.locationlevel3name
                //from locations l
                //join locations_level5 l5 on (l5.locationlevel5id = l.locationlevel5id)

                CswNbtSchemaUpdateImportMgr RoomImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations_level5", "Box", "locationslevel5_view" );

                RoomImpMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "" );
                RoomImpMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );

                // Relationships
                RoomImpMgr.importBinding( "locationlevel4id", CswNbtObjClassLocation.PropertyName.Location, "" );
                RoomImpMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, "" );
                RoomImpMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, "" );

                RoomImpMgr.finalize();
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30043_Locations

}//namespace ChemSW.Nbt.Schema