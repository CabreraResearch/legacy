using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

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

        public Int32 GetModuleId( CswNbtModuleName Module )
        {
            return GetModuleId( Module.ToString() );
        }

        public Int32 GetModuleId( string ModuleName )
        {
            Int32 RetModuleId = Int32.MinValue;
            CswTableSelect ModulesTable = _CswNbtResources.makeCswTableSelect( "SchemaModTrnsctn_ModuleUpdate", "modules" );
            string WhereClause = " where lower(name)='" + ModuleName.ToLower() + "'";
            DataTable ModulesDataTable = ModulesTable.getTable( WhereClause, true );
            if( ModulesDataTable.Rows.Count == 1 )
            {
                DataRow ModuleRow = ModulesDataTable.Rows[0];
                RetModuleId = CswConvert.ToInt32( ModuleRow["moduleid"] );
            }
            return RetModuleId;
        }

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
        /// Triggers all OnEnable and OnDisable logic for enabled and disabled modules, respectively
        /// </summary>
        public void TriggerModuleEventHandlers()
        {
            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }
            foreach( CswNbtModuleName Module in _ModuleRules.Keys )
            {
                if( _ModuleRules[Module].Enabled )
                {
                    _ModuleRules[Module].OnEnable();
                }
                else
                {
                    _ModuleRules[Module].OnDisable();
                }
            }
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
                        _ModuleRules[Module].Enabled = true;
                        _ModuleRules[Module].OnEnable();
                    }
                }
                ModulesToDisable = ModulesToDisable ?? new Collection<CswNbtModuleName>();
                if( ModulesToDisable.Contains( Module ) )
                {
                    if( Enabled )
                    {
                        ModuleRow["enabled"] = CswConvert.ToDbVal( false );
                        _ModuleRules[Module].Enabled = false;
                        _ModuleRules[Module].OnDisable();
                    }
                }
            }
            ret = ModulesUpdate.update( ModulesTable );

            initModules();

            // case 26029
            _CswNbtResources.MetaData.ResetEnabledNodeTypes();

            //We have to clear Session data or the view selects recent views will have non-accesible views and break
            _CswNbtResources.SessionDataMgr.removeAllSessionData( _CswNbtResources.Session.SessionId );

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
                    // case 28964 - check for redundant existing view
                    DataTable redundantViewDT = _CswNbtResources.ViewSelect.getView( viewName, SetVisibility, null, null );
                    if( redundantViewDT.Rows.Count == 0 )
                    {
                        view.SetVisibility( SetVisibility, null, null );
                        view.save();
                    }
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
                ToggleView( hidden, row["viewname"].ToString(), visibility );
            }
        }

        public void ToggleAction( bool showInList, CswNbtActionName actionName )
        {
            string databaseActionName = CswNbtAction.ActionNameEnumToString( actionName );
            CswTableUpdate actionTU = _CswNbtResources.makeCswTableUpdate( "toggleActionVisibility", "actions" );
            DataTable actionsDT = actionTU.getTable( "where actionname = '" + databaseActionName + "'" );
            foreach( DataRow row in actionsDT.Rows )
            {
                row["showinlist"] = CswConvert.ToDbVal( showInList );
            }
            actionTU.update( actionsDT );
        }

        public void ToggleScheduledRule( NbtScheduleRuleNames RuleName, bool Disabled )
        {
            CswTableUpdate RuleUpdate = _CswNbtResources.makeCswTableUpdate( "toggleScheduledRule", "scheduledrules" );
            DataTable RuleDt = RuleUpdate.getTable( "where lower(rulename) = '" + RuleName.ToString().ToLower() + "'" );
            foreach( DataRow row in RuleDt.Rows )
            {
                row["disabled"] = CswConvert.ToDbVal( Disabled );
            }
            RuleUpdate.update( RuleDt );
        }

        public void TogglePrintLabels( bool Hidden, CswNbtModuleName Module )
        {
            CswNbtMetaDataObjectClass printLabelOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp nodetypesOCP = printLabelOC.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );

            CswNbtView printLabelsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = printLabelsView.AddViewRelationship( printLabelOC, false );

            CswTableSelect childObjectClasses_TS = _CswNbtResources.makeCswTableSelect( "getModuleChildren", "jct_modules_objectclass" );

            int moduleId = _CswNbtResources.Modules.GetModuleId( Module );
            DataTable childObjClasses_DT = childObjectClasses_TS.getTable( "where moduleid = " + moduleId );
            bool first = true;
            foreach( DataRow Row in childObjClasses_DT.Rows )
            {
                int ObjClassId = CswConvert.ToInt32( Row["objectclassid"] );
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes( ObjClassId ) )
                {
                    if( first )
                    {
                        printLabelsView.AddViewPropertyAndFilter( parent, nodetypesOCP,
                            Value: NodeType.NodeTypeName,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

                        first = false;
                    }
                    else
                    {
                        printLabelsView.AddViewPropertyAndFilter( parent, nodetypesOCP,
                            Value: NodeType.NodeTypeName,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains,
                            Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.Or );
                    }
                }
            }


            ICswNbtTree printLabelsTree = _CswNbtResources.Trees.getTreeFromView( printLabelsView, false, true, true );
            int childCount = printLabelsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                printLabelsTree.goToNthChild( i );
                CswNbtNode printLabelNode = printLabelsTree.getNodeForCurrentPosition();
                printLabelNode.Hidden = Hidden;
                printLabelNode.postChanges( false );
                printLabelsTree.goToParentNode();
            }
        }

        public void AddPropToFirstTab( int NodeTypeId, string PropName, int Row = Int32.MinValue, int Col = Int32.MinValue )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            if( null != NodeType )
            {
                CswNbtMetaDataNodeTypeTab firstNTT = NodeType.getFirstNodeTypeTab();
                AddPropToTab( NodeTypeId, PropName, firstNTT, Row, Col );
            }
        }

        public void AddPropToTab( int NodeTypeId, string PropName, CswNbtMetaDataNodeTypeTab Tab, int Row = Int32.MinValue, int Col = Int32.MinValue )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, PropName );
            if( null != NodeTypeProp )
            {
                CswNbtMetaDataNodeType locationNT = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( Int32.MinValue != Row && Int32.MinValue != Col )
                {
                    NodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId, DisplayRow: Row, DisplayColumn: Col );
                }
                else
                {
                    NodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId );
                }
            }
        }

        public void AddPropToTab( int NodeTypeId, string PropName, string TabName, int TabOrder = 99 )
        {
            CswNbtMetaDataNodeTypeTab tab = _CswNbtResources.MetaData.getNodeTypeTab( NodeTypeId, TabName );
            if( null == tab )
            {
                CswNbtMetaDataNodeType locationNT = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                tab = _CswNbtResources.MetaData.makeNewTab( locationNT, TabName, TabOrder );
            }
            AddPropToTab( NodeTypeId, PropName, tab );
        }

        public void HideProp( int NodeTypeId, string PropName )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, PropName );
            if( null != NodeTypeProp )
            {
                NodeTypeProp.removeFromAllLayouts();
            }
        }


        public void ToggleReportNodes( string Category, bool Hidden )
        {
            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp categoryOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Category );

            CswNbtView reportsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = reportsView.AddViewRelationship( reportOC, false );
            reportsView.AddViewPropertyAndFilter( parent, categoryOCP,
                Value: Category,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree reportsTree = _CswNbtResources.Trees.getTreeFromView( reportsView, false, true, true );
            int childCount = reportsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                reportsTree.goToNthChild( i );
                CswNbtNode reportNode = reportsTree.getNodeForCurrentPosition();
                reportNode.Hidden = Hidden;
                reportNode.postChanges( false );
                reportsTree.goToParentNode();
            }
        }

    } // class CswNbtModuleManager

}// namespace ChemSW.Nbt
