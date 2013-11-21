using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31222_LocationsFix : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31222; }
        }

        public override string AppendToScriptName()
        {
            return "Locations_v2.0";
        }

        public override string Title
        {
            get
            {
                return "Rewrite locations bindings with separate sheets";
            }
        }

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units

            CswNbtSchemaUpdateImportMgr LocationsMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            // Bindings

            LocationsMgr.removeImportOrder( "CAF", "Site", 1, true );
            LocationsMgr.removeImportOrder( "CAF", "Building", 2, true );
            LocationsMgr.removeImportOrder( "CAF", "Room", 3, true );
            LocationsMgr.removeImportOrder( "CAF", "Cabinet", 4, true );
            LocationsMgr.removeImportOrder( "CAF", "Shelf", 5, true );
            LocationsMgr.removeImportOrder( "CAF", "Box", 6, true );

            //the instance on the relationships is different for Locations, so they must be deleted manually
            LocationsMgr.removeImportRelationship( "CAF", "Building", "Location", 1 );
            LocationsMgr.removeImportRelationship( "CAF", "Room", "Location", 2 );
            LocationsMgr.removeImportRelationship( "CAF", "Cabinet", "Location", 3 );
            LocationsMgr.removeImportRelationship( "CAF", "Shelf", "Location", 4 );
            LocationsMgr.removeImportRelationship( "CAF", "Box", "Location", 5 );


            #region Site

            LocationsMgr.CAFimportOrder( "Site", "sites", "site_view", "siteid" );
            LocationsMgr.importBinding( "sitename", CswNbtObjClassLocation.PropertyName.Name, "" );
            LocationsMgr.importBinding( "sitecode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            #endregion

            #region Building
            LocationsMgr.CAFimportOrder( "Building", "locations", "building_view", "locationid" );

            LocationsMgr.importBinding( "locationlevel1name", CswNbtObjClassLocation.PropertyName.Name, "");
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "");
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString());
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            LocationsMgr.importBinding( "siteid", CswNbtObjClassLocation.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );

            #endregion

            #region Room
            LocationsMgr.CAFimportOrder( "Room", "locations", "room_view", "locationid" );

            LocationsMgr.importBinding( "locationlevel2name", CswNbtObjClassLocation.PropertyName.Name, "" );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            LocationsMgr.importBinding( "buildingid", CswNbtObjClassLocation.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );

            #endregion

            #region Cabinet
            LocationsMgr.CAFimportOrder( "Cabinet", "locations", "cabinet_view", "locationid" );

            LocationsMgr.importBinding( "locationlevel3name", CswNbtObjClassLocation.PropertyName.Name, "" );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            LocationsMgr.importBinding( "roomid", CswNbtObjClassLocation.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );

            #endregion

            #region Shelf
            LocationsMgr.CAFimportOrder( "Shelf", "locations", "shelf_view", "locationid" );

            LocationsMgr.importBinding( "locationlevel4name", CswNbtObjClassLocation.PropertyName.Name, "" );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            LocationsMgr.importBinding( "cabinetid", CswNbtObjClassLocation.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );

            #endregion

            #region Box
            LocationsMgr.CAFimportOrder( "Box", "locations", "box_view", "locationid" );

            LocationsMgr.importBinding( "locationlevel5name", CswNbtObjClassLocation.PropertyName.Name, "" );
            LocationsMgr.importBinding( "locationcode", CswNbtObjClassLocation.PropertyName.LocationCode, "" );
            LocationsMgr.importBinding( "inventorygroupid", CswNbtObjClassLocation.PropertyName.InventoryGroup, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "controlzoneid", CswNbtObjClassLocation.PropertyName.ControlZone, CswEnumNbtSubFieldName.NodeID.ToString() );
            LocationsMgr.importBinding( "allowinventory", CswNbtObjClassLocation.PropertyName.AllowInventory, "" );
            LocationsMgr.importBinding( "barcode", CswNbtObjClassLocation.PropertyName.Barcode, "" );
            LocationsMgr.importBinding( "shelfid", CswNbtObjClassLocation.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );

            #endregion


                                             
                                             
                                             

            LocationsMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30043_Locations

}//namespace ChemSW.Nbt.Schema