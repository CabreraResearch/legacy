using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
                    _ModuleRules.Add( ModuleName.ToString().ToLower(), CswNbtModuleRuleFactory.makeModuleRule( _CswNbtResources, ModuleName ) );
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
                        CswNbtModuleName ModuleName = CswConvert.ToString( ModuleRow["name"] );
                        if( ModuleName != CswNbtModuleName.Unknown )
                        {
                            CswNbtModuleRule ModuleRule = _ModuleRules[ModuleName];
                            if( null != ModuleRule )
                            {
                                ModuleRule.Enabled = CswConvert.ToBoolean( ModuleRow["enabled"].ToString() );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( ErrorType.Error,
                                                   "Invalid Module: " + CswConvert.ToString( ModuleRow["name"] ),
                                                   "An invalid module was detected in the Modules table: " + CswConvert.ToString( ModuleRow["name"] ), ex );
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
        /// Enable a Module and trigger its enable event
        /// </summary>
        public bool EnableModule( CswNbtModuleName ModuleToEnable )
        {
            return UpdateModules( new Collection<CswNbtModuleName> { ModuleToEnable }, null );
        }

        /// <summary>
        /// Disable a Module and trigger its disable event
        /// </summary>
        public bool DisableModule( CswNbtModuleName ModuleToDisable )
        {
            return UpdateModules( null, new Collection<CswNbtModuleName> { ModuleToDisable } );
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
                ModulesToEnable = ModulesToEnable ?? new Collection<CswNbtModuleName>();
                bool Enabled = CswConvert.ToBoolean( ModuleRow["enabled"] );
                if( ModulesToEnable.Contains( Module ) )
                {
                    if( false == Enabled )
                    {
                        ModuleRow["enabled"] = CswConvert.ToDbVal( true );
                        _ModuleRules[Module].OnEnable();
                    }
                }
                ModulesToDisable = ModulesToDisable ?? new Collection<CswNbtModuleName>();
                if( ModulesToDisable.Contains( Module ) )
                {
                    if( Enabled )
                    {
                        ModuleRow["enabled"] = CswConvert.ToDbVal( false );
                        _ModuleRules[Module].OnDisable();
                    }
                }
            }
            ret = ModulesUpdate.update( ModulesTable );

            initModules();

            // case 26029
            _CswNbtResources.MetaData.ResetEnabledNodeTypes();

            return ret;
        }

        /// <summary>
        /// Trigger the event appropriate to whether the module is currently enabled or disabled
        /// Use this to sync new or edited events with existing schemata
        /// </summary>
        public void triggerEvent( CswNbtModuleName ModuleName )
        {
            if( IsModuleEnabled( ModuleName ) )
            {
                _ModuleRules[ModuleName].OnEnable();
            }
            else
            {
                _ModuleRules[ModuleName].OnDisable();
            }
        }

        /// <summary>
        /// Refresh the Modules Collection
        /// </summary>
        public void ClearModulesCache()
        {
            initModules();
        }

        /// <summary>
        /// Convenience function for hiding User nodes. Entering 'cispro' for the modulename param will hide all users with 'cispro' in the username.
        /// </summary>
        /// <param name="hidden">True if the nodes should be hidden</param>
        /// <param name="modulename">The module of the nodes to hide</param>
        public void ToggleUserNodes( bool hidden, string modulename )
        {
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp usernameOCP = userOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );
            CswNbtView usersView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = usersView.AddViewRelationship( userOC, false );
            usersView.AddViewPropertyAndFilter( parent,
                MetaDataProp: usernameOCP,
                Value: modulename,
                SubFieldName: CswNbtSubField.SubFieldName.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

            ICswNbtTree cisproUsersTree = _CswNbtResources.Trees.getTreeFromView( usersView, false, true, true );
            int count = cisproUsersTree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                cisproUsersTree.goToNthChild( i );
                CswNbtNode userNode = cisproUsersTree.getNodeForCurrentPosition();
                userNode.Hidden = hidden;
                userNode.postChanges( false );
                cisproUsersTree.goToParentNode();
            }
        }

        /// <summary>
        /// Convenience function for hiding Role nodes. Entering 'cispro' for the modulename param will hide all Roels with 'cispro' in the name.
        /// </summary>
        /// <param name="hidden">True if the nodes should be hidden</param>
        /// <param name="modulename">The module of the nodes to hide</param>
        public void ToggleRoleNodes( bool hidden, string modulename )
        {
            CswNbtMetaDataObjectClass roleOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp nameOCP = roleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );
            CswNbtView rolesView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = rolesView.AddViewRelationship( roleOC, false );
            rolesView.AddViewPropertyAndFilter( parent,
                MetaDataProp: nameOCP,
                Value: modulename,
                SubFieldName: CswNbtSubField.SubFieldName.Text,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

            ICswNbtTree cisproUsersTree = _CswNbtResources.Trees.getTreeFromView( rolesView, false, true, true );
            int count = cisproUsersTree.getChildNodeCount();
            for( int i = 0; i < count; i++ )
            {
                cisproUsersTree.goToNthChild( i );
                CswNbtNode userNode = cisproUsersTree.getNodeForCurrentPosition();
                userNode.Hidden = hidden;
                userNode.postChanges( false );
                cisproUsersTree.goToParentNode();
            }
        }

        /// <summary>
        /// Convenience function for hiding views (null safe)
        /// </summary>
        /// <param name="hidden">True if the view should be hidden</param>
        /// <param name="viewName">The name of the view to hide/unhide</param>
        /// /// <param name="visibility">the original visibility of the view when not hidden</param>
        public void ToggleView( bool hidden, string viewName, NbtViewVisibility Visibility )
        {
            NbtViewVisibility FindVisibility = hidden ? Visibility : NbtViewVisibility.Hidden;
            NbtViewVisibility SetVisibility = hidden ? NbtViewVisibility.Hidden : Visibility;

            DataTable viewDT = _CswNbtResources.ViewSelect.getView( viewName, FindVisibility, null, null );
            if( viewDT.Rows.Count == 1 )
            {
                CswNbtView view = _CswNbtResources.ViewSelect.restoreView( viewDT.Rows[0]["viewxml"].ToString() );
                if( null != view )
                {
                    view.SetVisibility( SetVisibility, null, null );
                    view.save();
                }
            }
        }        

        /// <summary>
        /// Convenience function for hiding all views in a category
        /// </summary>
        /// <param name="hidden">true if the views should be hidden</param>
        /// <param name="category">the category to get all views in</param>
        /// <param name="visibility">the original visibility of the view when not hidden</param>
        public void ToggleViewsInCategory( bool hidden, string category, NbtViewVisibility visibility )
        {
            CswTableSelect tu = _CswNbtResources.makeCswTableSelect( "toggleViewsInCategory_26717", "node_views" );
            DataTable nodeviews = tu.getTable( "where category = '" + category + "'" );
            foreach( DataRow row in nodeviews.Rows )
            {
                _CswNbtResources.Modules.ToggleView( hidden, row["viewname"].ToString(), visibility );
            }
        }

    } // class CswNbtModuleManager

}// namespace ChemSW.Nbt
