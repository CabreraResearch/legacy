using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-17
    /// </summary>
    public class CswUpdateSchemaTo01H17 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 17 ); } }
        public CswUpdateSchemaTo01H17( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // Case 20706
            CswNbtMetaDataNodeType AssemblyScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Schedule" );
            CswNbtMetaDataNodeType EquipmentScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Schedule" );
            CswNbtMetaDataNodeType PhysicalInspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection_Schedule ) );
            
            CswTableSelect ModulesTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "modules_select", "modules" );
            DataTable FETable = ModulesTableSelect.getTable( "where name = 'FE'" );
            Int32 FEModuleId = CswConvert.ToInt32( FETable.Rows[0]["moduleid"] );

            DataTable ImcsTable = ModulesTableSelect.getTable( "where name = 'IMCS'" );
            Int32 ImcsModuleID = CswConvert.ToInt32( FETable.Rows[0]["moduleid"] );

            if( null != AssemblyScheduleNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( ImcsModuleID, AssemblyScheduleNT.NodeTypeId );
            }
            if( null != EquipmentScheduleNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( ImcsModuleID, EquipmentScheduleNT.NodeTypeId );
            }
            if( null != PhysicalInspectionScheduleNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( FEModuleId, PhysicalInspectionScheduleNT.NodeTypeId );
            }
            
            // Case 20689
            String ConfigVarSQL = "update configuration_variables set variablename=lower(variablename)";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( ConfigVarSQL );
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswResources.NbtConfigurationVariables.Is_Demo, "If 1, Schema is in Demo mode", "0", true );
            
            // isdemo columns
            const String DemoColumnDescription = "1 if the row is demo data";
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.containers.ToString(), CswSchemaVersion.ContainersColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.inventory_groups.ToString(), CswSchemaVersion.InventoryGroupsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.jct_modules_actions.ToString(), CswSchemaVersion.JctModulesActionsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.jct_modules_nodetypes.ToString(), CswSchemaVersion.JctModulesNodeTypesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.jct_nodes_props.ToString(), CswSchemaVersion.JctNodesPropsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.jct_nodes_props_audit.ToString(), CswSchemaVersion.JctNodesPropsAuditColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.license_accept.ToString(), CswSchemaVersion.LicenseAcceptColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.locations.ToString(), CswSchemaVersion.LocationsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.materials.ToString(), CswSchemaVersion.MaterialsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.materials_subclass.ToString(), CswSchemaVersion.MaterialsSubclassColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.materials_synonyms.ToString(), CswSchemaVersion.MaterialsSynonymsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodes.ToString(), CswSchemaVersion.NodesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodes_audit.ToString(), CswSchemaVersion.NodesAuditColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodetypes.ToString(), CswSchemaVersion.NodeTypesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodetypes_audit.ToString(), CswSchemaVersion.NodeTypesAuditColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodetype_props.ToString(), CswSchemaVersion.NodeTypePropsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodetype_props_audit.ToString(), CswSchemaVersion.NodeTypePropsAuditColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.nodetype_tabset.ToString(), CswSchemaVersion.NodeTypeTabsetColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.node_views.ToString(), CswSchemaVersion.NodeViewsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.packages.ToString(), CswSchemaVersion.PackagesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.packdetail.ToString(), CswSchemaVersion.PackdetailColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.schedule_items.ToString(), CswSchemaVersion.ScheduleItemsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.sequences.ToString(), CswSchemaVersion.SequencesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.sessionlist.ToString(), CswSchemaVersion.SessionListColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics.ToString(), CswSchemaVersion.StatisticsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics_actions.ToString(), CswSchemaVersion.StatisticsActionsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics_nodetypes.ToString(), CswSchemaVersion.StatisticsNodeTypesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics_reports.ToString(), CswSchemaVersion.StatisticsReportsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics_searches.ToString(), CswSchemaVersion.StatisticsSearchesColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.statistics_views.ToString(), CswSchemaVersion.StatisticsViewsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.units_of_measure.ToString(), CswSchemaVersion.UnitsOfMeasureColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.update_history.ToString(), CswSchemaVersion.UpdateHistoryColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.users.ToString(), CswSchemaVersion.UsersColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.vendors.ToString(), CswSchemaVersion.VendorsColumns.isdemo.ToString(), DemoColumnDescription, true, false );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( CswSchemaVersion.NbtTables.welcome.ToString(), CswSchemaVersion.WelcomeColumns.isdemo.ToString(), DemoColumnDescription, true, false );

        } // update()

    }//class CswUpdateSchemaTo01H17

}//namespace ChemSW.Nbt.Schema


