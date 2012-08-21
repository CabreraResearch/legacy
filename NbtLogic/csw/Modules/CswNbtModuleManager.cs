using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.DB;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Manages NBT Modules
    /// </summary>
    public class CswNbtModuleManager
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtModuleManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private SortedList ModulesHt = new SortedList();
        private void initModules()
        {
            ModulesHt.Clear();
            foreach( CswNbtModule Module in Enum.GetValues( typeof( CswNbtModule ) ) )
            {
                ModulesHt.Add( Module, false );
            }

            // Fetch modules from database
            if( _CswNbtResources.IsInitializedForDbAccess )
            {
                CswTableSelect ModulesTableSelect = _CswNbtResources.makeCswTableSelect( "modules_select", "modules" );
                DataTable ModulesTable = ModulesTableSelect.getTable();
                foreach( DataRow ModuleRow in ModulesTable.Rows )
                {
                    try
                    {
                        CswNbtModule Module = ModuleRow["name"].ToString();
                        //Enum.TryParse( ModuleRow["name"].ToString(), true, out Module );
                        ModulesHt[Module] = ( ModuleRow["enabled"].ToString() == "1" );
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid Module", "An invalid module was detected in the Modules table: " + ModuleRow["name"].ToString(), ex );
                    }
                }
            } // if( _CswResources.IsInitializedForDbAccess )
        } // initModules()

        /// <summary>
        /// Returns whether a module is enabled
        /// </summary>
        public bool IsModuleEnabled( CswNbtModule Module )
        {
            if( ModulesHt.Count == 0 )
            {
                initModules();
            }

            if( ModulesHt.Count > 0 )
                return (bool) ModulesHt[Module];
            else
                return false;   // Assume modules are disabled if we have no db connection (for login page)
        }

        /// <summary>
        /// Collection of all enabled modules
        /// </summary>
        public Collection<CswNbtModule> ModulesEnabled()
        {
            if( ModulesHt.Count == 0 )
            {
                initModules();
            }

            Collection<CswNbtModule> EnabledModules = new Collection<CswNbtModule>();
            foreach( CswNbtModule Module in ModulesHt.Keys )
            {
                if( (bool) ModulesHt[Module] )
                    EnabledModules.Add( Module );
            }
            return EnabledModules;
        }

        /// <summary>
        /// This will explicitly enable or disable a set of modules.  
        /// Any modules not listed in either list will not be altered.
        /// </summary>
        public bool UpdateModules( Collection<CswNbtModule> ModulesToEnable, Collection<CswNbtModule> ModulesToDisable )
        {
            bool ret = false;

            CswTableUpdate ModulesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtResources.UpdateModules_update", "modules" );
            DataTable ModulesTable = ModulesUpdate.getTable();
            foreach( DataRow ModuleRow in ModulesTable.Rows )
            {
                CswNbtModule Module = ModuleRow["name"].ToString();
                //Enum.TryParse( ModuleRow["name"].ToString(), true, out Module );
                if( ModulesToEnable.Contains( Module ) )
                {
                    ModuleRow["enabled"] = CswConvert.ToDbVal( true );
                }
                if( ModulesToDisable.Contains( Module ) )
                {
                    ModuleRow["enabled"] = CswConvert.ToDbVal( false );
                }
            }
            ret = ModulesUpdate.update( ModulesTable );

            initModules();

            // case 26029
            _CswNbtResources.MetaData.ResetEnabledNodeTypes();

            return ret;
        }

        /// <summary>
        /// Refresh the Modules Collection
        /// </summary>
        public void ClearModulesCache()
        {
            initModules();
        }


    } // class CswNbtModuleManager

}// namespace ChemSW.Nbt
