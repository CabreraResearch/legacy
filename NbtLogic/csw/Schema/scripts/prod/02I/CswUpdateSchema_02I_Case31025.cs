using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31025 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31025; }
        }

        public override string Title
        {
            get { return "CISPro and SI bindings"; }
        }

        public override void update()
        {
            // CISPro
            {
                CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
                
                CswNbtSchemaUpdateImportMgr ImpMgrCIS = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CISPro" );

                ImpMgrCIS.importDef( 1, "chemicals" );

                ImpMgrCIS.importOrder( 1, "Inventory Group", "chemicals" );
                ImpMgrCIS.importOrder( 2, "Department", "chemicals" );
                if( null != SiteNT ) // case 31306
                {
                    ImpMgrCIS.importOrder( 3, "Site", "chemicals", 1 );
                }
                ImpMgrCIS.importOrder( 4, "Building", "chemicals", 2 );
                ImpMgrCIS.importOrder( 5, "Room", "chemicals", 3 );
                ImpMgrCIS.importOrder( 6, "Cabinet", "chemicals", 4 );
                ImpMgrCIS.importOrder( 7, "Vendor", "chemicals" );
                ImpMgrCIS.importOrder( 8, "Chemical", "chemicals" );
                ImpMgrCIS.importOrder( 9, "Unit_Weight", "chemicals" );
                ImpMgrCIS.importOrder( 10, "Unit_Volume", "chemicals" );
                ImpMgrCIS.importOrder( 11, "Unit_Each", "chemicals" );
                ImpMgrCIS.importOrder( 12, "Role", "chemicals" );
                ImpMgrCIS.importOrder( 13, "User", "chemicals" );
                ImpMgrCIS.importOrder( 14, "Size", "chemicals" );
                ImpMgrCIS.importOrder( 15, "Container", "chemicals" );

                ImpMgrCIS.importBinding( "inventorygroupname", "Name", "", "chemicals", "inventory group" );
                ImpMgrCIS.importBinding( "department", "Department Name", "", "chemicals", "department" );
                if( null != SiteNT ) // case 31306
                {
                    ImpMgrCIS.importBinding( "site", "Name", "", "chemicals", "site", 1 );
                }
                ImpMgrCIS.importBinding( "building", "Name", "", "chemicals", "building", 2 );
                ImpMgrCIS.importBinding( "room", "Name", "", "chemicals", "room", 3 );
                ImpMgrCIS.importBinding( "cabinet", "Name", "", "chemicals", "cabinet", 4 );
                ImpMgrCIS.importBinding( "vendorname", "Vendor Name", "", "chemicals", "vendor" );
                ImpMgrCIS.importBinding( "materialname", "Tradename", "", "chemicals", "chemical" );
                ImpMgrCIS.importBinding( "catalogno", "Part Number", "", "chemicals", "chemical" );
                ImpMgrCIS.importBinding( "casno", "CAS No", "", "chemicals", "chemical" );
                ImpMgrCIS.importBinding( "Unit_Weight", "Name", "", "chemicals", "Unit_Weight" );
                ImpMgrCIS.importBinding( "Unit_Volume", "Name", "", "chemicals", "Unit_Volume" );
                ImpMgrCIS.importBinding( "Unit_Each", "Name", "", "chemicals", "Unit_Each" );
                ImpMgrCIS.importBinding( "ConvFactExp", "Conversion Factor", "exponent", "chemicals", "Unit_Weight" );
                ImpMgrCIS.importBinding( "ConvFactExp", "Conversion Factor", "exponent", "chemicals", "Unit_Volume" );
                ImpMgrCIS.importBinding( "ConvFactExp", "Conversion Factor", "exponent", "chemicals", "Unit_Each" );
                ImpMgrCIS.importBinding( "ConvFactBase", "Conversion Factor", "base", "chemicals", "Unit_Weight" );
                ImpMgrCIS.importBinding( "ConvFactBase", "Conversion Factor", "base", "chemicals", "Unit_Volume" );
                ImpMgrCIS.importBinding( "ConvFactBase", "Conversion Factor", "base", "chemicals", "Unit_Each" );
                ImpMgrCIS.importBinding( "UnitOfMeasureName", "Quantity", "name", "chemicals", "container" );
                ImpMgrCIS.importBinding( "netquantity", "Quantity", "value", "chemicals", "container" );
                ImpMgrCIS.importBinding( "UnitOfMeasureName", "Initial Quantity", "name", "chemicals", "size" );
                ImpMgrCIS.importBinding( "barcodeid", "Barcode", "", "chemicals", "container" );
                ImpMgrCIS.importBinding( "responsible", "Username", "", "chemicals", "user" );
                ImpMgrCIS.importBinding( "expirationdate", "expiration date", "", "chemicals", "container" );
                ImpMgrCIS.importBinding( "rolename", "Name", "", "chemicals", "role" );

                ImpMgrCIS.importRelationship( "chemicals", "Building", "Location", 1 );
                ImpMgrCIS.importRelationship( "chemicals", "Room", "Location", 2 );
                ImpMgrCIS.importRelationship( "chemicals", "Room", "Department" );
                ImpMgrCIS.importRelationship( "chemicals", "Cabinet", "Location", 3 );
                ImpMgrCIS.importRelationship( "chemicals", "Chemical", "Supplier" );
                ImpMgrCIS.importRelationship( "chemicals", "Size", "Material" );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Size" );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Material" );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Location", 1 );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Location", 2 );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Location", 3 );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Location", 4 );
                ImpMgrCIS.importRelationship( "chemicals", "Container", "Owner" );
                if( null != SiteNT ) // case 31306
                {
                    ImpMgrCIS.importRelationship( "chemicals", "Site", "Inventory Group" );
                }
                ImpMgrCIS.importRelationship( "chemicals", "Room", "Inventory Group" );
                ImpMgrCIS.importRelationship( "chemicals", "Building", "Inventory Group" );
                ImpMgrCIS.importRelationship( "chemicals", "Cabinet", "Inventory Group" );
                ImpMgrCIS.importRelationship( "chemicals", "User", "Role" );

                ImpMgrCIS.finalize();
            }

            //// SI
            //{
            //    CswNbtSchemaUpdateImportMgr ImpMgrSI = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "SI" );

            //    ImpMgrSI.importDef( 1, "InspectionTarget" );

            //    ImpMgrSI.importOrder( 1, "Site", "InspectionTarget", 1 );
            //    ImpMgrSI.importOrder( 2, "Building", "InspectionTarget", 2 );
            //    ImpMgrSI.importOrder( 3, "Room", "InspectionTarget", 3 );
            //    ImpMgrSI.importOrder( 4, "InspectionTarget Group", "InspectionTarget" );
            //    ImpMgrSI.importOrder( 5, "InspectionTarget", "InspectionTarget" );
            //    ImpMgrSI.importOrder( 6, "Inspection Schedule", "InspectionTarget" );

            //    ImpMgrSI.importBinding( "site", "Name", "", "InspectionTarget", "site", 1 );
            //    ImpMgrSI.importBinding( "building", "Name", "", "InspectionTarget", "building", 2 );
            //    ImpMgrSI.importBinding( "room", "Name", "", "InspectionTarget", "room", 3 );
            //    ImpMgrSI.importBinding( "inspectiongroup", "Name", "", "InspectionTarget", "inspectiontarget group" );
            //    ImpMgrSI.importBinding( "UniqueTargetName", "Description", "", "InspectionTarget", "inspectiontarget" );
            //    ImpMgrSI.importBinding( "schedule_summary", "Summary", "", "InspectionTarget", "Inspection Schedule" );
            //    ImpMgrSI.importBinding( "checklist name", "Inspection Type", "", "InspectionTarget", "Inspection Schedule" );
            //    ImpMgrSI.importBinding( "Next Due Date", "Next Due Date", "", "InspectionTarget", "Inspection Schedule" );
            //    ImpMgrSI.importBinding( "DueDateInterval", "Due Date Interval", "", "InspectionTarget", "Inspection Schedule" );
            //    ImpMgrSI.importBinding( "WarningDays", "Warning Days", "", "InspectionTarget", "Inspection Schedule" );

            //    ImpMgrSI.importRelationship( "InspectionTarget", "InspectionTarget", "Location", 3 );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "InspectionTarget", "Location", 2 );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "InspectionTarget", "Location", 1 );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "InspectionTarget", "InspectionTarget Group" );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "Inspection Schedule", "Inspection Group" );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "Building", "Location", 1 );
            //    ImpMgrSI.importRelationship( "InspectionTarget", "Room", "Location", 2 );

            //    ImpMgrSI.finalize();
            //}
        } // update()

    } // class

}//namespace ChemSW.Nbt.Schema