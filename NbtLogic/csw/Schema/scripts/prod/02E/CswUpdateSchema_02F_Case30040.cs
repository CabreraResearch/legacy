using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30040 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30040; }
        }

        private CswTableUpdate _importDefinitionUpdate;
        private CswTableUpdate _importOrderUpdate;
        private CswTableUpdate _importBindingsUpdate;
        private CswTableUpdate _importRelationshipsUpdate;
        private DataTable _importDefinitionTable;
        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;
        private Int32 _importDefId;

        public override void update()
        {
            // IMCS bindings definition
            _importDefinitionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30040_import_def_update", "import_definitions" );
            _importDefinitionTable = _importDefinitionUpdate.getEmptyTable();
            DataRow defrow = _importDefinitionTable.NewRow();
            defrow["definitionname"] = "IMCS";
            _importDefinitionTable.Rows.Add( defrow );
            _importDefId = CswConvert.ToInt32( defrow["importdefinitionid"] );

            // Set up other import tables
            _importOrderUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30040_import_order_update", "import_order" );
            _importBindingsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30040_import_bindings_update", "import_bindings" );
            _importRelationshipsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30040_import_relationships_update", "import_relationships" );

            _importOrderTable = _importOrderUpdate.getEmptyTable();
            _importBindingsTable = _importBindingsUpdate.getEmptyTable();
            _importRelationshipsTable = _importRelationshipsUpdate.getEmptyTable();

            // Fill import definition tables with IMCS definitions
            #region Order
            _importOrder( 1, "assembly", "Role" );
            _importOrder( 2, "assembly", "User" );
            _importOrder( 3, "assembly", "Department" );
            _importOrder( 4, "assembly", "Building", 1 );
            _importOrder( 5, "assembly", "Room", 2 );
            _importOrder( 6, "assembly", "Vendor", 1 );
            _importOrder( 7, "assembly", "Vendor", 2 );
            _importOrder( 8, "assembly", "Equipment Type" );
            _importOrder( 9, "assembly", "Assembly" );
            _importOrder( 10, "asm_problem", "Role" );
            _importOrder( 11, "asm_problem", "User" );
            _importOrder( 12, "asm_problem", "Department" );
            _importOrder( 13, "asm_problem", "Assembly" );
            _importOrder( 14, "asm_problem", "Assembly Problem" );
            _importOrder( 15, "asm_schedules", "Role" );
            _importOrder( 16, "asm_schedules", "User" );
            _importOrder( 17, "asm_schedules", "Department" );
            _importOrder( 18, "asm_schedules", "Assembly" );
            _importOrder( 19, "asm_schedules", "Assembly Schedule" );
            _importOrder( 20, "asm_tasks", "Role" );
            _importOrder( 21, "asm_tasks", "User" );
            _importOrder( 22, "asm_tasks", "Department" );
            _importOrder( 23, "asm_tasks", "Assembly" );
            _importOrder( 24, "asm_tasks", "Assembly Task" );
            _importOrder( 25, "equipment", "Role" );
            _importOrder( 26, "equipment", "User" );
            _importOrder( 27, "equipment", "Department" );
            _importOrder( 28, "equipment", "Building", 1 );
            _importOrder( 29, "equipment", "Room", 2 );
            _importOrder( 30, "equipment", "Vendor", 1 );
            _importOrder( 31, "equipment", "Vendor", 2 );
            _importOrder( 32, "equipment", "Equipment Type" );
            _importOrder( 33, "equipment", "Assembly" );
            _importOrder( 34, "equipment", "Equipment" );
            _importOrder( 35, "eq_problem", "Role" );
            _importOrder( 36, "eq_problem", "User" );
            _importOrder( 37, "eq_problem", "Department" );
            _importOrder( 38, "eq_problem", "Equipment" );
            _importOrder( 39, "eq_problem", "Equipment Problem" );
            _importOrder( 40, "eq_schedules", "Role" );
            _importOrder( 41, "eq_schedules", "User" );
            _importOrder( 42, "eq_schedules", "Department" );
            _importOrder( 43, "eq_schedules", "Equipment" );
            _importOrder( 44, "eq_schedules", "Equipment Schedule" );
            _importOrder( 45, "eq_tasks", "Role" );
            _importOrder( 46, "eq_tasks", "User" );
            _importOrder( 47, "eq_tasks", "Department" );
            _importOrder( 48, "eq_tasks", "Equipment" );
            _importOrder( 49, "eq_tasks", "Equipment Task" );
            #endregion Order

            #region Binding
            _importBinding( "assembly", "nodetype", "", "", "" );
            _importBinding( "assembly", "departmentname", "Department", "Department Name", "" );
            _importBinding( "assembly", "location", "Room", "Name", "", 2 );
            _importBinding( "assembly", "building", "Building", "Name", "", 1 );
            _importBinding( "assembly", "equiptypename", "Equipment Type", "Type Name", "" );
            _importBinding( "assembly", "assemblyid", "Assembly", "Assembly ID", "" );
            _importBinding( "assembly", "propertyno", "Assembly", "Assembly Property No", "" );
            _importBinding( "assembly", "serialno", "Assembly", "Assembly Serial No", "" );
            _importBinding( "assembly", "model", "Assembly", "Assembly Model", "" );
            _importBinding( "assembly", "mfr_vendorname", "Assembly", "Assembly Manufacturer", "" );
            _importBinding( "assembly", "availability", "Assembly", "Status", "" );
            _importBinding( "assembly", "datepurchased", "Assembly", "Assembly Purchased", "" );
            _importBinding( "assembly", "datereceived", "Assembly", "Assembly MTBF", "StartDateTime" );
            _importBinding( "assembly", "datereceived", "Assembly", "Assembly Received", "" );
            _importBinding( "assembly", "DateSignedOut", "Assembly", "Assembly Out On", "" );
            _importBinding( "assembly", "description", "Assembly", "Assembly Description", "" );
            _importBinding( "assembly", "condition", "Assembly", "Assembly Condition", "" );
            _importBinding( "assembly", "startingcost", "Assembly", "Assembly Starting Cost", "" );
            _importBinding( "assembly", "haswarranty", "Assembly", "Assembly Has Service Contract", "" );
            _importBinding( "assembly", "datewarrantyends", "Assembly", "Assembly Service Ends On", "" );
            _importBinding( "assembly", "warrantycost", "Assembly", "Assembly Service Cost", "" );
            _importBinding( "assembly", "warrantyphoneno", "Assembly", "Assembly Service Phone", "" );
            _importBinding( "assembly", "warrantycontractno", "Assembly", "Assembly Contract No", "" );
            _importBinding( "assembly", "manualstoredat", "Assembly", "Assembly Manual Stored At", "" );
            _importBinding( "assembly", "notes", "Assembly", "Assembly Notes", "" );
            _importBinding( "assembly", "OriginalUsrName", "User", "Last Name", "" );
            _importBinding( "assembly", "username", "User", "Username", "" );
            _importBinding( "assembly", "role", "Role", "Name", "" );
            _importBinding( "assembly", "pwd", "User", "Password", "" );
            _importBinding( "assembly", "user_disabled", "User", "AccountLocked", "" );
            _importBinding( "assembly", "supp_vendorname", "Vendor", "Vendor Name", "", 1 );
            _importBinding( "assembly", "supp_streetone", "Vendor", "Street1", "", 1 );
            _importBinding( "assembly", "supp_streettwo", "Vendor", "Street2", "", 1 );
            _importBinding( "assembly", "supp_city", "Vendor", "City", "", 1 );
            _importBinding( "assembly", "supp_state", "Vendor", "State", "", 1 );
            _importBinding( "assembly", "supp_zip", "Vendor", "Zip", "", 1 );
            _importBinding( "assembly", "supp_phone", "Vendor", "Phone", "", 1 );
            _importBinding( "assembly", "supp_fax", "Vendor", "Fax", "", 1 );
            _importBinding( "assembly", "supp_conttactname", "Vendor", "Contact Name", "", 1 );
            _importBinding( "assembly", "supp_accountno", "Vendor", "Account No", "", 1 );
            _importBinding( "assembly", "supp_deptbillcode", "Vendor", "Dept Bill Code", "", 1 );
            _importBinding( "assembly", "warr_vendorname", "Vendor", "Vendor Name", "", 2 );
            _importBinding( "assembly", "warr_streetone", "Vendor", "Street1", "", 2 );
            _importBinding( "assembly", "warr_streettwo", "Vendor", "Street2", "", 2 );
            _importBinding( "assembly", "warr_city", "Vendor", "City", "", 2 );
            _importBinding( "assembly", "warr_state", "Vendor", "State", "", 2 );
            _importBinding( "assembly", "warr_zip", "Vendor", "Zip", "", 2 );
            _importBinding( "assembly", "warr_phone", "Vendor", "Phone", "", 2 );
            _importBinding( "assembly", "warr_fax", "Vendor", "Fax", "", 2 );
            _importBinding( "assembly", "warr_conttactname", "Vendor", "Contact Name", "", 2 );
            _importBinding( "assembly", "warr_accountno", "Vendor", "Account No", "", 2 );
            _importBinding( "assembly", "warr_deptbillcode", "Vendor", "Dept Bill Code", "", 2 );
            _importBinding( "assembly", "responsible", "Assembly", "Responsible", "" );

            _importBinding( "asm_problem", "assemblyid", "Assembly", "Assembly ID", "" );
            _importBinding( "asm_problem", "PID", "Assembly Problem", "Summary", "" );
            _importBinding( "asm_problem", "equipid", "", "", "" );
            _importBinding( "asm_problem", "Technician", "Assembly Problem", "Technician", "" );
            _importBinding( "asm_problem", "TechPhone", "Assembly Problem", "Technician Phone", "" );
            _importBinding( "asm_problem", "ReportedBy", "User", "Username", "" );
            _importBinding( "asm_problem", "UserName", "", "", "" );
            _importBinding( "asm_problem", "UserDeptName", "Department", "Department Name", "" );
            _importBinding( "asm_problem", "DateStartedOn", "Assembly Problem", "Start Date", "" );
            _importBinding( "asm_problem", "DateStartedOn", "Assembly Problem", "Date Opened", "" );
            _importBinding( "asm_problem", "DateEndedOn", "Assembly Problem", "Date Closed", "" );
            _importBinding( "asm_problem", "ProblemDescript", "Assembly Problem", "Problem", "" );
            _importBinding( "asm_problem", "ResolutionDescript", "Assembly Problem", "Resolution", "" );
            _importBinding( "asm_problem", "IsUnderWarranty", "Assembly Problem", "Under Warranty", "" );
            _importBinding( "asm_problem", "LaborCost", "Assembly Problem", "Labor Cost", "" );
            _importBinding( "asm_problem", "TravelCost", "Assembly Problem", "Travel Cost", "" );
            _importBinding( "asm_problem", "PartsCost", "Assembly Problem", "Parts Cost", "" );
            _importBinding( "asm_problem", "OtherCost", "Assembly Problem", "Other Cost", "" );
            _importBinding( "asm_problem", "OtherCostName", "Assembly Problem", "Other Cost Name", "" );
            _importBinding( "asm_problem", "ProblemType", "", "", "" );
            _importBinding( "asm_problem", "ProblemStatus", "Assembly Problem", "Closed", "" );
            _importBinding( "asm_problem", "UserOption1", "", "", "" );
            _importBinding( "asm_problem", "UserNumber1", "", "", "" );
            _importBinding( "asm_problem", "WorkOrderPrinted", "Assembly Problem", "Work Order Printed", "" );
            _importBinding( "asm_problem", "role", "Role", "Name", "" );
            _importBinding( "asm_problem", "user_disabled", "User", "AccountLocked", "" );

            _importBinding( "asm_schedules", "assemblyid", "Assembly", "Assembly ID", "" );
            _importBinding( "asm_schedules", "sid", "Assembly Schedule", "Summary", "" );
            _importBinding( "asm_schedules", "equipid", "", "", "" );
            _importBinding( "asm_schedules", "FreqUnits", "", "", "" );
            _importBinding( "asm_schedules", "FreqIncrement", "", "", "" );
            _importBinding( "asm_schedules", "EventType", "Assembly Schedule", "Event Type", "" );
            _importBinding( "asm_schedules", "GenCount", "", "", "" );
            _importBinding( "asm_schedules", "GenLimit", "", "", "" );
            _importBinding( "asm_schedules", "WarnAhead", "Assembly Schedule", "Warning Days", "" );
            _importBinding( "asm_schedules", "IsSchedule", "", "", "" );
            _importBinding( "asm_schedules", "Enabled", "Assembly Schedule", "Enabled", "" );
            _importBinding( "asm_schedules", "TechName", "User", "Username", "" );
            _importBinding( "asm_schedules", "TechPhone", "", "", "" );
            _importBinding( "asm_schedules", "ReportedBy", "", "", "" );
            _importBinding( "asm_schedules", "UserName", "", "", "" );
            _importBinding( "asm_schedules", "DeptName", "Department", "Department Name", "" );
            _importBinding( "asm_schedules", "StartedOn", "", "", "" );
            _importBinding( "asm_schedules", "EndedOn", "", "", "" );
            _importBinding( "asm_schedules", "Description", "Assembly Schedule", "Description", "" );
            _importBinding( "asm_schedules", "Completion", "", "", "" );
            _importBinding( "asm_schedules", "Status", "", "", "" );
            _importBinding( "asm_schedules", "LinkToFile", "", "", "" );
            _importBinding( "asm_schedules", "CommandLine", "Assembly Schedule", "File Link", "Text" );
            _importBinding( "asm_schedules", "Labor", "", "", "" );
            _importBinding( "asm_schedules", "Parts", "", "", "" );
            _importBinding( "asm_schedules", "Travel", "", "", "" );
            _importBinding( "asm_schedules", "OtherCost", "", "", "" );
            _importBinding( "asm_schedules", "OtherText", "", "", "" );
            _importBinding( "asm_schedules", "evext01id", "", "", "" );
            _importBinding( "asm_schedules", "EventID", "", "", "" );
            _importBinding( "asm_schedules", "LowLimit", "Assembly Schedule", "Lower Limit", "" );
            _importBinding( "asm_schedules", "HighLimit", "Assembly Schedule", "Upper Limit", "" );
            _importBinding( "asm_schedules", "CalValue", "", "", "" );
            _importBinding( "asm_schedules", "IsCriticalTest", "Assembly Schedule", "Is Critical Test", "" );
            _importBinding( "asm_schedules", "CalDate", "", "", "" );
            _importBinding( "asm_schedules", "CalSOP", "Assembly Schedule", "SOP/Ref#", "" );
            _importBinding( "asm_schedules", "CalIgnore", "", "", "" );
            _importBinding( "asm_schedules", "Cal_CCPRO", "", "", "" );
            _importBinding( "asm_schedules", "Extra_int1", "", "", "" );
            _importBinding( "asm_schedules", "Extra_int2", "", "", "" );
            _importBinding( "asm_schedules", "Exta_num1", "", "", "" );
            _importBinding( "asm_schedules", "Extra_num2", "", "", "" );
            _importBinding( "asm_schedules", "Exta_date1", "", "", "" );
            _importBinding( "asm_schedules", "Extra_date2", "", "", "" );
            _importBinding( "asm_schedules", "Extra_text1", "", "", "" );
            _importBinding( "asm_schedules", "Extra_text2", "", "", "" );
            _importBinding( "asm_schedules", "Passes", "", "", "" );
            _importBinding( "asm_schedules", "TestCompleted", "", "", "" );
            _importBinding( "asm_schedules", "role", "Role", "Name", "" );
            _importBinding( "asm_schedules", "user_disabled", "User", "AccountLocked", "" );
            _importBinding( "asm_schedules", "SchedIntervalData", "Assembly Schedule", "Due Date Interval", "" );
            _importBinding( "asm_schedules", "FinalDueDate", "Assembly Schedule", "Final Due Date", "" );

            _importBinding( "asm_tasks", "eid", "", "", "" );
            _importBinding( "asm_tasks", "assemblyid", "Assembly", "Assembly ID", "" );
            _importBinding( "asm_tasks", "sid", "Assembly Task", "Summary", "" );
            _importBinding( "asm_tasks", "equipid", "", "", "" );
            _importBinding( "asm_tasks", "FreqUnits", "", "", "" );
            _importBinding( "asm_tasks", "FreqIncrement", "", "", "" );
            _importBinding( "asm_tasks", "EventType", "Assembly Task", "Event Type", "" );
            _importBinding( "asm_tasks", "GenCount", "", "", "" );
            _importBinding( "asm_tasks", "GenLimit", "", "", "" );
            _importBinding( "asm_tasks", "WarnAhead", "", "", "" );
            _importBinding( "asm_tasks", "IsSchedule", "", "", "" );
            _importBinding( "asm_tasks", "Enabled", "", "", "" );
            _importBinding( "asm_tasks", "TechName", "User", "Username", "" );
            _importBinding( "asm_tasks", "TechPhone", "", "", "" );
            _importBinding( "asm_tasks", "ReportedBy", "", "", "" );
            _importBinding( "asm_tasks", "UserName", "", "", "" );
            _importBinding( "asm_tasks", "DeptName", "Department", "Department Name", "" );
            _importBinding( "asm_tasks", "StartedOn", "Assembly Task", "Due Date", "" );
            _importBinding( "asm_tasks", "EndedOn", "Assembly Task", "Done On", "" );
            _importBinding( "asm_tasks", "Description", "Assembly Task", "Description", "" );
            _importBinding( "asm_tasks", "Completion", "Assembly Task", "Completion Description", "" );
            _importBinding( "asm_tasks", "Status", "Assembly Task", "Completed", "" );
            _importBinding( "asm_tasks", "LinkToFile", "", "", "" );
            _importBinding( "asm_tasks", "CommandLine", "Assembly Task", "File Link", "Text" );
            _importBinding( "asm_tasks", "Labor", "Assembly Task", "Labor Cost", "" );
            _importBinding( "asm_tasks", "Parts", "Assembly Task", "Parts Cost", "" );
            _importBinding( "asm_tasks", "Travel", "Assembly Task", "Travel Cost", "" );
            _importBinding( "asm_tasks", "OtherCost", "Assembly Task", "Other Cost", "" );
            _importBinding( "asm_tasks", "OtherText", "Assembly Task", "Other Cost Name", "" );
            _importBinding( "asm_tasks", "evext01id", "", "", "" );
            _importBinding( "asm_tasks", "EventID", "", "", "" );
            _importBinding( "asm_tasks", "LowLimit", "Assembly Task", "Lower Limit", "" );
            _importBinding( "asm_tasks", "HighLimit", "Assembly Task", "Upper Limit", "" );
            _importBinding( "asm_tasks", "CalValue", "", "", "" );
            _importBinding( "asm_tasks", "IsCriticalTest", "Assembly Task", "Is Critical Test", "" );
            _importBinding( "asm_tasks", "CalDate", "Assembly Task", "Calibration Date", "" );
            _importBinding( "asm_tasks", "CalSOP", "Assembly Task", "SOP/Ref#", "" );
            _importBinding( "asm_tasks", "CalIgnore", "Assembly Task", "Ignore Calibration Result", "" );
            _importBinding( "asm_tasks", "Cal_CCPRO", "", "", "" );
            _importBinding( "asm_tasks", "Extra_int1", "", "", "" );
            _importBinding( "asm_tasks", "Extra_int2", "", "", "" );
            _importBinding( "asm_tasks", "Exta_num1", "", "", "" );
            _importBinding( "asm_tasks", "Extra_num2", "", "", "" );
            _importBinding( "asm_tasks", "Exta_date1", "", "", "" );
            _importBinding( "asm_tasks", "Extra_date2", "", "", "" );
            _importBinding( "asm_tasks", "Extra_text1", "", "", "" );
            _importBinding( "asm_tasks", "Extra_text2", "", "", "" );
            _importBinding( "asm_tasks", "Passes", "", "", "" );
            _importBinding( "asm_tasks", "TestCompleted", "", "", "" );
            _importBinding( "asm_tasks", "role", "Role", "Name", "" );
            _importBinding( "asm_tasks", "user_disabled", "User", "AccountLocked", "" );

            _importBinding( "equipment", "nodetype", "", "", "" );
            _importBinding( "equipment", "departmentname", "Department", "Department Name", "" );
            _importBinding( "equipment", "building", "Building", "Name", "", 1 );
            _importBinding( "equipment", "location", "Room", "Name", "", 2 );
            _importBinding( "equipment", "equiptypename", "Equipment Type", "Type Name", "" );
            _importBinding( "equipment", "eid", "Equipment", "Equipment ID", "" );
            _importBinding( "equipment", "assemblyid", "Assembly", "Assembly ID", "" );
            _importBinding( "equipment", "propertyno", "Equipment", "Property No", "" );
            _importBinding( "equipment", "serialno", "Equipment", "Serial No", "" );
            _importBinding( "equipment", "model", "Equipment", "Model", "" );
            _importBinding( "equipment", "mfr_vendorname", "Equipment", "Manufacturer", "" );
            _importBinding( "equipment", "availability", "Equipment", "Status", "" );
            _importBinding( "equipment", "datepurchased", "Equipment", "Purchased", "" );
            _importBinding( "equipment", "datereceived", "Equipment", "MTBF", "StartDateTime" );
            _importBinding( "equipment", "datereceived", "Equipment", "Received", "" );
            _importBinding( "equipment", "description", "Equipment", "Description", "" );
            _importBinding( "equipment", "condition", "Equipment", "Condition", "" );
            _importBinding( "equipment", "startingcost", "Equipment", "Starting Cost", "" );
            _importBinding( "equipment", "OrigUsrName", "User", "Last Name", "" );
            _importBinding( "equipment", "username", "User", "Username", "" );
            _importBinding( "equipment", "role", "Role", "Name", "" );
            _importBinding( "equipment", "pwd", "User", "Password", "" );
            _importBinding( "equipment", "user_disabled", "User", "AccountLocked", "" );
            _importBinding( "equipment", "datesignedout", "Equipment", "Out On", "" );
            _importBinding( "equipment", "haswarranty", "Equipment", "Has Service Contract", "" );
            _importBinding( "equipment", "datewarrantyends", "Equipment", "Service Ends On", "" );
            _importBinding( "equipment", "warrantycost", "Equipment", "Service Cost", "" );
            _importBinding( "equipment", "warrantyphoneno", "Equipment", "Service Phone", "" );
            _importBinding( "equipment", "warrantycontractno", "Equipment", "Contract No", "" );
            _importBinding( "equipment", "manualstoredat", "Equipment", "Manual Stored At", "" );
            _importBinding( "equipment", "notes", "Equipment", "Notes", "" );
            _importBinding( "equipment", "supp_vendorname", "Vendor", "Vendor Name", "", 1 );
            _importBinding( "equipment", "supp_streetone", "Vendor", "Street1", "", 1 );
            _importBinding( "equipment", "supp_streettwo", "Vendor", "Street2", "", 1 );
            _importBinding( "equipment", "supp_city", "Vendor", "City", "", 1 );
            _importBinding( "equipment", "supp_state", "Vendor", "State", "", 1 );
            _importBinding( "equipment", "supp_zip", "Vendor", "Zip", "", 1 );
            _importBinding( "equipment", "supp_phone", "Vendor", "Phone", "", 1 );
            _importBinding( "equipment", "supp_fax", "Vendor", "Fax", "", 1 );
            _importBinding( "equipment", "supp_conttactname", "Vendor", "Contact Name", "", 1 );
            _importBinding( "equipment", "supp_accountno", "Vendor", "Account No", "", 1 );
            _importBinding( "equipment", "supp_deptbillcode", "Vendor", "Dept Bill Code", "", 1 );
            _importBinding( "equipment", "warr_vendorname", "Vendor", "Vendor Name", "", 2 );
            _importBinding( "equipment", "warr_streetone", "Vendor", "Street1", "", 2 );
            _importBinding( "equipment", "warr_streettwo", "Vendor", "Street2", "", 2 );
            _importBinding( "equipment", "warr_city", "Vendor", "City", "", 2 );
            _importBinding( "equipment", "warr_state", "Vendor", "State", "", 2 );
            _importBinding( "equipment", "warr_zip", "Vendor", "Zip", "", 2 );
            _importBinding( "equipment", "warr_phone", "Vendor", "Phone", "", 2 );
            _importBinding( "equipment", "warr_fax", "Vendor", "Fax", "", 2 );
            _importBinding( "equipment", "warr_conttactname", "Vendor", "Contact Name", "", 2 );
            _importBinding( "equipment", "warr_accountno", "Vendor", "Account No", "", 2 );
            _importBinding( "equipment", "warr_deptbillcode", "Vendor", "Dept Bill Code", "", 2 );
            _importBinding( "equipment", "responsible", "Equipment", "Responsible", "" );

            _importBinding( "eq_problem", "eid", "Equipment", "Equipment ID", "" );
            _importBinding( "eq_problem", "PID", "Equipment Problem", "Summary", "" );
            _importBinding( "eq_problem", "equipid", "", "", "" );
            _importBinding( "eq_problem", "Technician", "Equipment Problem", "Technician", "" );
            _importBinding( "eq_problem", "TechPhone", "Equipment Problem", "Technician Phone", "" );
            _importBinding( "eq_problem", "ReportedBy", "User", "Username", "" );
            _importBinding( "eq_problem", "UserName", "", "", "" );
            _importBinding( "eq_problem", "UserDeptName", "Department", "Department Name", "" );
            _importBinding( "eq_problem", "DateStartedOn", "Equipment Problem", "Start Date", "" );
            _importBinding( "eq_problem", "DateStartedOn", "Equipment Problem", "Date Opened", "" );
            _importBinding( "eq_problem", "DateEndedOn", "Equipment Problem", "Date Closed", "" );
            _importBinding( "eq_problem", "ProblemDescript", "Equipment Problem", "Problem", "" );
            _importBinding( "eq_problem", "ResolutionDescript", "Equipment Problem", "Resolution", "" );
            _importBinding( "eq_problem", "IsUnderWarranty", "Equipment Problem", "Under Warranty", "" );
            _importBinding( "eq_problem", "LaborCost", "Equipment Problem", "Labor Cost", "" );
            _importBinding( "eq_problem", "TravelCost", "Equipment Problem", "Travel Cost", "" );
            _importBinding( "eq_problem", "PartsCost", "Equipment Problem", "Parts Cost", "" );
            _importBinding( "eq_problem", "OtherCost", "Equipment Problem", "Other Cost", "" );
            _importBinding( "eq_problem", "OtherCostName", "Equipment Problem", "Other Cost Name", "" );
            _importBinding( "eq_problem", "ProblemType", "", "", "" );
            _importBinding( "eq_problem", "ProblemStatus", "Equipment Problem", "Closed", "" );
            _importBinding( "eq_problem", "UserOption1", "", "", "" );
            _importBinding( "eq_problem", "UserNumber1", "", "", "" );
            _importBinding( "eq_problem", "WorkOrderPrinted", "Equipment Problem", "Work Order Printed", "" );
            _importBinding( "eq_problem", "role", "Role", "Name", "" );
            _importBinding( "eq_problem", "user_disabled", "User", "AccountLocked", "" );

            _importBinding( "eq_schedules", "eid", "Equipment", "Equipment ID", "" );
            _importBinding( "eq_schedules", "sid", "Equipment Schedule", "Summary", "" );
            _importBinding( "eq_schedules", "equipid", "", "", "" );
            _importBinding( "eq_schedules", "FreqUnits", "", "", "" );
            _importBinding( "eq_schedules", "FreqIncrement", "", "", "" );
            _importBinding( "eq_schedules", "EventType", "Equipment Schedule", "Event Type", "" );
            _importBinding( "eq_schedules", "GenCount", "", "", "" );
            _importBinding( "eq_schedules", "GenLimit", "", "", "" );
            _importBinding( "eq_schedules", "WarnAhead", "Equipment Schedule", "Warning Days", "" );
            _importBinding( "eq_schedules", "IsSchedule", "", "", "" );
            _importBinding( "eq_schedules", "Enabled", "Equipment Schedule", "Enabled", "" );
            _importBinding( "eq_schedules", "TechName", "User", "Username", "" );
            _importBinding( "eq_schedules", "TechPhone", "", "", "" );
            _importBinding( "eq_schedules", "ReportedBy", "", "", "" );
            _importBinding( "eq_schedules", "UserName", "", "", "" );
            _importBinding( "eq_schedules", "DeptName", "Department", "Department Name", "" );
            _importBinding( "eq_schedules", "StartedOn", "", "", "" );
            _importBinding( "eq_schedules", "EndedOn", "", "", "" );
            _importBinding( "eq_schedules", "Description", "Equipment Schedule", "Description", "" );
            _importBinding( "eq_schedules", "Completion", "", "", "" );
            _importBinding( "eq_schedules", "Status", "", "", "" );
            _importBinding( "eq_schedules", "LinkToFile", "", "", "" );
            _importBinding( "eq_schedules", "CommandLine", "Equipment Schedule", "File Link", "Text" );
            _importBinding( "eq_schedules", "Labor", "", "", "" );
            _importBinding( "eq_schedules", "Parts", "", "", "" );
            _importBinding( "eq_schedules", "Travel", "", "", "" );
            _importBinding( "eq_schedules", "OtherCost", "", "", "" );
            _importBinding( "eq_schedules", "OtherText", "", "", "" );
            _importBinding( "eq_schedules", "evext01id", "", "", "" );
            _importBinding( "eq_schedules", "EventID", "", "", "" );
            _importBinding( "eq_schedules", "LowLimit", "Equipment Schedule", "Lower Limit", "" );
            _importBinding( "eq_schedules", "HighLimit", "Equipment Schedule", "Upper Limit", "" );
            _importBinding( "eq_schedules", "CalValue", "", "", "" );
            _importBinding( "eq_schedules", "IsCriticalTest", "Equipment Schedule", "Is Critical Test", "" );
            _importBinding( "eq_schedules", "CalDate", "", "", "" );
            _importBinding( "eq_schedules", "CalSOP", "Equipment Schedule", "SOP/Ref#", "" );
            _importBinding( "eq_schedules", "CalIgnore", "", "", "" );
            _importBinding( "eq_schedules", "Cal_CCPRO", "", "", "" );
            _importBinding( "eq_schedules", "Extra_int1", "", "", "" );
            _importBinding( "eq_schedules", "Extra_int2", "", "", "" );
            _importBinding( "eq_schedules", "Exta_num1", "", "", "" );
            _importBinding( "eq_schedules", "Extra_num2", "", "", "" );
            _importBinding( "eq_schedules", "Exta_date1", "", "", "" );
            _importBinding( "eq_schedules", "Extra_date2", "", "", "" );
            _importBinding( "eq_schedules", "Extra_text1", "", "", "" );
            _importBinding( "eq_schedules", "Extra_text2", "", "", "" );
            _importBinding( "eq_schedules", "Passes", "", "", "" );
            _importBinding( "eq_schedules", "TestCompleted", "", "", "" );
            _importBinding( "eq_schedules", "role", "Role", "Name", "" );
            _importBinding( "eq_schedules", "user_disabled", "User", "AccountLocked", "" );
            _importBinding( "eq_schedules", "SchedIntervalData", "Equipment Schedule", "Due Date Interval", "" );
            _importBinding( "eq_schedules", "FinalDueDate", "Equipment Schedule", "Final Due Date", "" );

            _importBinding( "eq_tasks", "eid", "Equipment", "Equipment ID", "" );
            _importBinding( "eq_tasks", "assemblyid", "", "", "" );
            _importBinding( "eq_tasks", "sid", "Equipment Task", "Summary", "" );
            _importBinding( "eq_tasks", "equipid", "", "", "" );
            _importBinding( "eq_tasks", "FreqUnits", "", "", "" );
            _importBinding( "eq_tasks", "FreqIncrement", "", "", "" );
            _importBinding( "eq_tasks", "EventType", "Equipment Task", "Event Type", "" );
            _importBinding( "eq_tasks", "GenCount", "", "", "" );
            _importBinding( "eq_tasks", "GenLimit", "", "", "" );
            _importBinding( "eq_tasks", "WarnAhead", "", "", "" );
            _importBinding( "eq_tasks", "IsSchedule", "", "", "" );
            _importBinding( "eq_tasks", "Enabled", "", "", "" );
            _importBinding( "eq_tasks", "TechName", "User", "Username", "" );
            _importBinding( "eq_tasks", "TechPhone", "", "", "" );
            _importBinding( "eq_tasks", "ReportedBy", "", "", "" );
            _importBinding( "eq_tasks", "UserName", "", "", "" );
            _importBinding( "eq_tasks", "DeptName", "Department", "Department Name", "" );
            _importBinding( "eq_tasks", "StartedOn", "Equipment Task", "Due Date", "" );
            _importBinding( "eq_tasks", "EndedOn", "Equipment Task", "Done On", "" );
            _importBinding( "eq_tasks", "Description", "Equipment Task", "Description", "" );
            _importBinding( "eq_tasks", "Completion", "Equipment Task", "Completion Description", "" );
            _importBinding( "eq_tasks", "Status", "Equipment Task", "Completed", "" );
            _importBinding( "eq_tasks", "LinkToFile", "", "", "" );
            _importBinding( "eq_tasks", "CommandLine", "Equipment Task", "File Link", "Text" );
            _importBinding( "eq_tasks", "Labor", "Equipment Task", "Labor Cost", "" );
            _importBinding( "eq_tasks", "Parts", "Equipment Task", "Parts Cost", "" );
            _importBinding( "eq_tasks", "Travel", "Equipment Task", "Travel Cost", "" );
            _importBinding( "eq_tasks", "OtherCost", "Equipment Task", "Other Cost", "" );
            _importBinding( "eq_tasks", "OtherText", "Equipment Task", "Other Cost Name", "" );
            _importBinding( "eq_tasks", "evext01id", "", "", "" );
            _importBinding( "eq_tasks", "EventID", "", "", "" );
            _importBinding( "eq_tasks", "LowLimit", "Equipment Task", "Lower Limit", "" );
            _importBinding( "eq_tasks", "HighLimit", "Equipment Task", "Upper Limit", "" );
            _importBinding( "eq_tasks", "CalValue", "", "", "" );
            _importBinding( "eq_tasks", "IsCriticalTest", "Equipment Task", "Is Critical Test", "" );
            _importBinding( "eq_tasks", "CalDate", "Equipment Task", "Calibration Date", "" );
            _importBinding( "eq_tasks", "CalSOP", "Equipment Task", "SOP/Ref#", "" );
            _importBinding( "eq_tasks", "CalIgnore", "Equipment Task", "Ignore Calibration Result", "" );
            _importBinding( "eq_tasks", "Cal_CCPRO", "", "", "" );
            _importBinding( "eq_tasks", "Extra_int1", "", "", "" );
            _importBinding( "eq_tasks", "Extra_int2", "", "", "" );
            _importBinding( "eq_tasks", "Exta_num1", "", "", "" );
            _importBinding( "eq_tasks", "Extra_num2", "", "", "" );
            _importBinding( "eq_tasks", "Exta_date1", "", "", "" );
            _importBinding( "eq_tasks", "Extra_date2", "", "", "" );
            _importBinding( "eq_tasks", "Extra_text1", "", "", "" );
            _importBinding( "eq_tasks", "Extra_text2", "", "", "" );
            _importBinding( "eq_tasks", "Passes", "", "", "" );
            _importBinding( "eq_tasks", "TestCompleted", "", "", "" );
            _importBinding( "eq_tasks", "role", "Role", "Name", "" );
            _importBinding( "eq_tasks", "user_disabled", "User", "AccountLocked", "" );
            #endregion Binding

            #region Relationship
            _importRelationship( "assembly", "User", "Role" );
            _importRelationship( "assembly", "Assembly", "Department" );
            _importRelationship( "assembly", "Assembly", "User" );
            _importRelationship( "assembly", "Assembly", "Assembly Type" );
            _importRelationship( "assembly", "Room", "Location", 1 );
            _importRelationship( "assembly", "Assembly", "Location", 2 );
            _importRelationship( "assembly", "Assembly", "Assembly Vendor", 1 );
            _importRelationship( "assembly", "Assembly", "Assembly Service Vendor", 2 );
            _importRelationship( "asm_problem", "User", "Role" );
            _importRelationship( "asm_problem", "Assembly Problem", "Department" );
            _importRelationship( "asm_problem", "Assembly Problem", "Assembly" );
            _importRelationship( "asm_problem", "Assembly Problem", "Reported By" );
            _importRelationship( "asm_schedules", "User", "Role" );
            _importRelationship( "asm_schedules", "Assembly Schedule", "Technician" );
            _importRelationship( "asm_schedules", "Assembly Schedule", "Assembly" );
            _importRelationship( "asm_schedules", "Assembly Schedule", "Department" );
            _importRelationship( "asm_tasks", "User", "Role" );
            _importRelationship( "asm_tasks", "Assembly Task", "Technician" );
            _importRelationship( "asm_tasks", "Assembly Task", "Assembly" );
            _importRelationship( "asm_tasks", "Assembly Task", "Department" );
            _importRelationship( "equipment", "User", "Role" );
            _importRelationship( "equipment", "Assembly", "Department" );
            _importRelationship( "equipment", "Assembly", "User" );
            _importRelationship( "equipment", "Assembly", "Assembly Type" );
            _importRelationship( "equipment", "Assembly", "Assembly Vendor", 1 );
            _importRelationship( "equipment", "Assembly", "Assembly Service Vendor", 2 );
            _importRelationship( "equipment", "Equipment", "Department" );
            _importRelationship( "equipment", "Equipment", "Vendor", 1 );
            _importRelationship( "equipment", "Equipment", "Service Vendor", 2 );
            _importRelationship( "equipment", "Equipment", "User" );
            _importRelationship( "equipment", "Equipment", "Type" );
            _importRelationship( "equipment", "Equipment", "Assembly" );
            _importRelationship( "equipment", "Equipment", "Location", 2 );
            _importRelationship( "equipment", "Room", "Location", 1 );
            _importRelationship( "eq_problem", "User", "Role" );
            _importRelationship( "eq_problem", "Equipment Problem", "Department" );
            _importRelationship( "eq_problem", "Equipment Problem", "Equipment" );
            _importRelationship( "eq_problem", "Equipment Problem", "Reported By" );
            _importRelationship( "eq_schedules", "User", "Role" );
            _importRelationship( "eq_schedules", "Equipment Schedule", "Technician" );
            _importRelationship( "eq_schedules", "Equipment Schedule", "Equipment" );
            _importRelationship( "eq_schedules", "Equipment Schedule", "Department" );
            _importRelationship( "eq_tasks", "User", "Role" );
            _importRelationship( "eq_tasks", "Equipment Task", "Technician" );
            _importRelationship( "eq_tasks", "Equipment Task", "Equipment" );
            _importRelationship( "eq_tasks", "Equipment Task", "Department" );
            #endregion Relationship

            // Post changes
            _importDefinitionUpdate.update( _importDefinitionTable );
            _importOrderUpdate.update( _importOrderTable );
            _importBindingsUpdate.update( _importBindingsTable );
            _importRelationshipsUpdate.update( _importRelationshipsTable );
        } // update()

        private void _importOrder( Int32 Order, string SheetName, string NodeTypeName, Int32 Instance = Int32.MinValue )
        {
            if( false == string.IsNullOrEmpty( NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["importdefinitionid"] = _importDefId;
                row["importorder"] = Order;
                row["sourcesheetname"] = SheetName;
                row["nodetypename"] = NodeTypeName;
                row["instance"] = CswConvert.ToDbVal( Instance );
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        private void _importBinding( string SheetName, string SourceColumnName, string DestNodeTypeName, string DestPropertyName, string DestSubFieldName, Int32 Instance = Int32.MinValue )
        {
            if( false == string.IsNullOrEmpty( DestNodeTypeName ) )
            {
                DataRow row = _importBindingsTable.NewRow();
                row["importdefinitionid"] = _importDefId;
                row["sourcesheetname"] = SheetName;
                row["sourcecolumnname"] = SourceColumnName;
                row["destnodetypename"] = DestNodeTypeName;
                row["destpropname"] = DestPropertyName;
                row["destsubfield"] = DestSubFieldName;
                row["instance"] = CswConvert.ToDbVal( Instance );
                _importBindingsTable.Rows.Add( row );
            }
        } // _importBinding()
        private void _importRelationship( string SheetName, string NodetypeName, string RelationshipPropName, Int32 Instance = Int32.MinValue )
        {
            DataRow row = _importRelationshipsTable.NewRow();
            row["importdefinitionid"] = _importDefId;
            row["sourcesheetname"] = SheetName;
            row["nodetypename"] = NodetypeName;
            row["relationship"] = RelationshipPropName;
            row["instance"] = CswConvert.ToDbVal( Instance );
            _importRelationshipsTable.Rows.Add( row );
        } // _importRelationship()

    } // class CswUpdateSchema_02F_Case30040

}//namespace ChemSW.Nbt.Schema