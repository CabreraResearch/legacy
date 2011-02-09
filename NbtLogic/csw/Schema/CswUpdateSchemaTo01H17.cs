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
            Int32 ImcsModuleID = CswConvert.ToInt32( ImcsTable.Rows[0]["moduleid"] );

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
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( "is_demo", "If 1, Schema is in Demo mode", "0", true );
            
            // Case 20828
            _CswNbtSchemaModTrnsctn.setConfigVariableValue( "failedloginlimit", "5" );
            
        } // update()

    }//class CswUpdateSchemaTo01H17

}//namespace ChemSW.Nbt.Schema


