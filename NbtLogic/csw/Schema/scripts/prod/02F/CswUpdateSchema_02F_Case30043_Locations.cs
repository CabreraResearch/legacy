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

            #region Site
            LocationsMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "siteid", "Legacy ID", "", LocationSheetName, SiteNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, SiteNTName );
            #endregion

            #region Building
            LocationsMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BuildingNTName );
            #endregion

            #region Room
            LocationsMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, RoomNTName );
            #endregion

            #region Cabinet
            LocationsMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, CabinetNTName );
            #endregion

            #region Shelf
            LocationsMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, ShelfNTName );
            #endregion

            #region Box
            LocationsMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "locationid", "Legacy ID", "", LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString(), LocationSheetName, BoxNTName );
            #endregion

            //Relationships
            LocationsMgr.importRelationship( LocationSheetName, BuildingNTName, CswNbtObjClassLocation.PropertyName.Location, 1, "siteid" );

            LocationsMgr.importRelationship( LocationSheetName, RoomNTName, CswNbtObjClassLocation.PropertyName.Location, 2, "buildingid" );

            LocationsMgr.importRelationship( LocationSheetName, CabinetNTName, CswNbtObjClassLocation.PropertyName.Location, 3, "roomid" );

            LocationsMgr.importRelationship( LocationSheetName, ShelfNTName, CswNbtObjClassLocation.PropertyName.Location, 4, "cabinetid" );

            LocationsMgr.importRelationship( LocationSheetName, BoxNTName, CswNbtObjClassLocation.PropertyName.Location, 5, "shelfid" );

            LocationsMgr.finalize( null, null, true );

        } // update()

    } // class CswUpdateSchema_02F_Case30043_Locations

}//namespace ChemSW.Nbt.Schema