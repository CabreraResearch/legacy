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

        private Dictionary<CswNbtModuleName, CswNbtModuleRule> _ModuleRules = new Dictionary<CswNbtModuleName, CswNbtModuleRule>();

        private void initModules()
        {
            _ModuleRules.Clear();
            foreach( CswNbtModuleName ModuleName in CswNbtModuleName._All )
            {
                if( CswNbtModuleName.Unknown != ModuleName )
                {
                    _ModuleRules.Add( ModuleName, CswNbtModuleRuleFactory.makeModuleRule( _CswNbtResources, ModuleName ) );
                }
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
                        CswNbtModuleRule ModuleRule = _ModuleRules[ModuleRow["name"].ToString()];
                        if( null != ModuleRule )
                        {
                            ModuleRule.Enabled = CswConvert.ToBoolean( ModuleRow["enabled"].ToString() );
                        }
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( ErrorType.Error,
                                                   "Invalid Module",
                                                   "An invalid module was detected in the Modules table: " + ModuleRow["name"].ToString(), ex );
                    }
                }
            } // if( _CswResources.IsInitializedForDbAccess )
        } // initModules()

        /// <summary>
        /// Returns whether a module is enabled
        /// </summary>
        public bool IsModuleEnabled( CswNbtModuleName Module )
        {
            bool ret = false;     // Assume modules are disabled if we have no db connection (for login page)
            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }

            if( _ModuleRules.Count > 0 )
            {
                ret = _ModuleRules[Module].Enabled;
            }
            return ret;
        } // IsModuleEnabled()
        

        /// <summary>
        /// Collection of all enabled modules
        /// </summary>
        public Collection<CswNbtModuleName> ModulesEnabled()
        {
            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }

            Collection<CswNbtModuleName> EnabledModules = new Collection<CswNbtModuleName>();
            foreach( CswNbtModuleName Module in _ModuleRules.Keys )
            {
                if( _ModuleRules[Module].Enabled )
                {
                    EnabledModules.Add( Module );
                }
            }
            return EnabledModules;
        }

        /// <summary>
        /// This will explicitly enable or disable a set of modules.  
        /// Any modules not listed in either list will not be altered.
        /// </summary>
        public bool UpdateModules( Collection<CswNbtModuleName> ModulesToEnable, Collection<CswNbtModuleName> ModulesToDisable )
        {
            bool ret = false;

            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }

            CswTableUpdate ModulesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtResources.UpdateModules_update", "modules" );
            DataTable ModulesTable = ModulesUpdate.getTable();
            foreach( DataRow ModuleRow in ModulesTable.Rows )
            {
                CswNbtModuleName Module = ModuleRow["name"].ToString();
                //Enum.TryParse( ModuleRow["name"].ToString(), true, out Module );
                if( ModulesToEnable.Contains( Module ) )
                {
                    ModuleRow["enabled"] = CswConvert.ToDbVal( true );
                    _ModuleRules[Module].OnEnable();
                }
                if( ModulesToDisable.Contains( Module ) )
                {
                    ModuleRow["enabled"] = CswConvert.ToDbVal( false );
                    _ModuleRules[Module].OnDisable();
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
