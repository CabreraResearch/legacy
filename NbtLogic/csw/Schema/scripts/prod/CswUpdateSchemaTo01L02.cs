using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-02
    /// </summary>
    public class CswUpdateSchemaTo01L02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24087
            OrderByClause OrderBy = new OrderByClause( "name", OrderByType.Ascending );
            CswTableUpdate ModulesTs = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_module_select", "modules" );
            DataTable ModuleTable = ModulesTs.getTable( new CswCommaDelimitedString() { "moduleid", "name", "enabled" },
                                                        string.Empty,
                                                        Int32.MinValue, "where name='FE' or name='SI'",
                                                        false,
                                                        new Collection<OrderByClause>() { OrderBy } );
            Int32 FeModuleId = Int32.MinValue;
            Int32 SiModuleId = Int32.MinValue;
            bool EnableSi = false;
            foreach( DataRow ModuleRow in ModuleTable.Rows )
            {
                if( "FE" == CswConvert.ToString( ModuleRow["name"] ) )
                {
                    FeModuleId = CswConvert.ToInt32( ModuleRow["moduleid"] );
                    EnableSi = CswConvert.ToBoolean( ModuleRow["enabled"] );
                }

                if( "SI" == CswConvert.ToString( ModuleRow["name"] ) )
                {
                    SiModuleId = CswConvert.ToInt32( ModuleRow["moduleid"] );
                    EnableSi = EnableSi || CswConvert.ToBoolean( ModuleRow["enabled"] );
                    ModuleRow["enabled"] = CswConvert.ToDbVal( EnableSi );
                }
            }
            ModulesTs.update( ModuleTable );

            if( Int32.MinValue != FeModuleId && Int32.MinValue != SiModuleId )
            {
                //jct_modules_actions
                CswTableUpdate JctModActUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_modules_actions_update", "jct_modules_actions" );
                DataTable JctModActTable = JctModActUpdate.getTable( "moduleid", FeModuleId );
                foreach( DataRow JctRow in JctModActTable.Rows )
                {
                    JctRow["moduleid"] = CswConvert.ToDbVal( SiModuleId );
                }
                JctModActUpdate.update( JctModActTable );

                //jct_modules_nodetypes
                CswTableUpdate JctModNtUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_modules_nodetypes_update", "jct_modules_nodetypes" );
                DataTable JctModNtTable = JctModNtUpdate.getTable( "moduleid", FeModuleId );
                foreach( DataRow JctRow in JctModNtTable.Rows )
                {
                    JctRow["moduleid"] = CswConvert.ToDbVal( SiModuleId );
                }
                JctModNtUpdate.update( JctModNtTable );

                //jct_modules_objectclass
                CswTableUpdate JctModOcUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_modules_objectclass_update", "jct_modules_objectclass" );
                DataTable JctModOcTable = JctModOcUpdate.getTable( "moduleid", FeModuleId );
                foreach( DataRow JctRow in JctModOcTable.Rows )
                {
                    JctRow["moduleid"] = CswConvert.ToDbVal( SiModuleId );
                }
                JctModOcUpdate.update( JctModOcTable );

                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from modules where name='FE'" );
            }
            #endregion Case 24087


        }//Update()

    }//class CswUpdateSchemaTo01L02

}//namespace ChemSW.Nbt.Schema


