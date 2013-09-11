using System;
using System.Collections.Generic;
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


            List<Tuple<string, Int32>> DestNodeTypesAndInstances = new List<Tuple<string, int>>();
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( SiteNTName, 1 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( BuildingNTName, 2 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( RoomNTName, 3 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( CabinetNTName, 4 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( ShelfNTName, 5 ) );
            DestNodeTypesAndInstances.Add( new Tuple<string, int>( BoxNTName, 6 ) );

            CswNbtSchemaUpdateImportMgr LocationsMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations", DestNodeTypesAndInstances, "locations_view" );

            // Bindings
            LocationsMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "", "locations", SiteNTName );
            LocationsMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", SiteNTName );
            LocationsMgr.importBinding( "siteid", "Legacy ID", "", "locations", SiteNTName );

            LocationsMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "", "locations", BuildingNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", BuildingNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", "locations", BuildingNTName );

            LocationsMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "", "locations", RoomNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", RoomNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", "locations", RoomNTName );

            LocationsMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "", "locations", CabinetNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", CabinetNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", "locations", CabinetNTName );

            LocationsMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "", "locations", ShelfNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", ShelfNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", "locations", ShelfNTName );

            LocationsMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "", "locations", BoxNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", "locations", BoxNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", "locations", BoxNTName );

            //Relationships
            LocationsMgr.importRelationship( "locations", SiteNTName, CswNbtObjClassLocation.PropertyName.ControlZone );

            LocationsMgr.importRelationship( "locations", BuildingNTName, CswNbtObjClassLocation.PropertyName.Location, 1 );
            LocationsMgr.importRelationship( "locations", BuildingNTName, CswNbtObjClassLocation.PropertyName.ControlZone );
            LocationsMgr.importRelationship( "locations", BuildingNTName, CswNbtObjClassLocation.PropertyName.InventoryGroup );


            LocationsMgr.importRelationship( "locations", RoomNTName, CswNbtObjClassLocation.PropertyName.Location, 2 );
            LocationsMgr.importRelationship( "locations", RoomNTName, CswNbtObjClassLocation.PropertyName.ControlZone );
            LocationsMgr.importRelationship( "locations", RoomNTName, CswNbtObjClassLocation.PropertyName.InventoryGroup );

            LocationsMgr.importRelationship( "locations", CabinetNTName, CswNbtObjClassLocation.PropertyName.Location, 3 );
            LocationsMgr.importRelationship( "locations", CabinetNTName, CswNbtObjClassLocation.PropertyName.ControlZone );
            LocationsMgr.importRelationship( "locations", CabinetNTName, CswNbtObjClassLocation.PropertyName.InventoryGroup );

            LocationsMgr.importRelationship( "locations", ShelfNTName, CswNbtObjClassLocation.PropertyName.Location, 4 );
            LocationsMgr.importRelationship( "locations", ShelfNTName, CswNbtObjClassLocation.PropertyName.ControlZone );
            LocationsMgr.importRelationship( "locations", ShelfNTName, CswNbtObjClassLocation.PropertyName.InventoryGroup );

            LocationsMgr.importRelationship( "locations", BoxNTName, CswNbtObjClassLocation.PropertyName.Location, 5 );
            LocationsMgr.importRelationship( "locations", BoxNTName, CswNbtObjClassLocation.PropertyName.ControlZone );
            LocationsMgr.importRelationship( "locations", BoxNTName, CswNbtObjClassLocation.PropertyName.InventoryGroup );

            LocationsMgr.finalize( null, null, true );

            #region Old code

            //{
            //    //select l.*, l1.locationlevel1name, l1.locationlevel1id, s.siteid
            //    //from locations l
            //    //join locations_level1 l1 on (l1.locationlevel1id = l.locationlevel1id)
            //    //join sites s on (s.siteid = l1.siteid)

            //    CswNbtSchemaUpdateImportMgr BuildingImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "locations", "Building", "locationslevel1_view" );

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