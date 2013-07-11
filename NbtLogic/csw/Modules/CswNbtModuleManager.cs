using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Manages NBT Modules
    /// </summary>
    public class CswNbtModuleManager
    {
        private CswNbtResources _CswNbtResources;
        private Dictionary<CswEnumNbtModuleName, CswNbtModuleRule> _ModuleRules;

        public CswNbtModuleManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ModuleRules = new Dictionary<CswEnumNbtModuleName, CswNbtModuleRule>();

            foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName._All )
            {
                if( CswEnumNbtModuleName.Unknown != ModuleName )
                {
                    _ModuleRules.Add( ModuleName.ToString().ToLower(), CswNbtModuleRuleFactory.makeModuleRule( _CswNbtResources, ModuleName ) );
                }
            }
        }

        private void initModules()
        {
            // Fetch modules from database
            if( _CswNbtResources.IsInitializedForDbAccess )
            {
                CswTableSelect ModulesTableSelect = _CswNbtResources.makeCswTableSelect( "modules_select", "modules" );
                DataTable ModulesTable = ModulesTableSelect.getTable();
                foreach( DataRow ModuleRow in ModulesTable.Rows )
                {
                    CswEnumNbtModuleName ModuleName = CswConvert.ToString( ModuleRow["name"] );
                    if( ModuleName != CswEnumNbtModuleName.Unknown )
                    {
                        CswNbtModuleRule ModuleRule = _ModuleRules[ModuleName];
                        ModuleRule.Enabled = CswConvert.ToBoolean( ModuleRow["enabled"] );
                    }
                }
            } // if( _CswResources.IsInitializedForDbAccess )
        } // initModules()

        /// <summary>
        /// Returns whether a module is enabled
        /// </summary>
        public bool IsModuleEnabled( CswEnumNbtModuleName Module )
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

        public Int32 GetModuleId( CswEnumNbtModuleName Module )
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
        public Collection<CswEnumNbtModuleName> ModulesEnabled()
        {
            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }

            Collection<CswEnumNbtModuleName> EnabledModules = new Collection<CswEnumNbtModuleName>();
            foreach( CswEnumNbtModuleName Module in _ModuleRules.Keys )
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
            List<CswEnumNbtModuleName> Rules = _ModuleRules.Keys.ToList();
            foreach( CswEnumNbtModuleName Module in Rules )
            {
                if( _ModuleRules[Module].Enabled )
                {
                    _ModuleRules[Module].Enable();
                }
                else
                {
                    _ModuleRules[Module].Disable();
                }
            }
        }

        /// <summary>
        /// Enable a Module and trigger its enable event
        /// </summary>
        public void EnableModule( CswEnumNbtModuleName ModuleToEnable )
        {
            _updateModule( ModuleToEnable, true );
        }

        /// <summary>
        /// Disable a Module and trigger its disable event
        /// </summary>
        public void DisableModule( CswEnumNbtModuleName ModuleToDisable )
        {
            _updateModule( ModuleToDisable, false );
        }

        /// <summary>
        /// Enable/disable a module in an ORNy safe fashion
        /// </summary>
        private void _updateModule( CswEnumNbtModuleName Module, bool Enable )
        {
            int moduleid = GetModuleId( Module );

            if( _ModuleRules.Count == 0 )
            {
                initModules();
            }

            CswTableUpdate ModuleUpdate = _CswNbtResources.makeCswTableUpdate( "ModuleManager.UpdateModule", "modules" );
            DataTable ModulesTbl = ModuleUpdate.getTable( "moduleid", moduleid );
            foreach( DataRow row in ModulesTbl.Rows ) //should only ever get one row
            {
                bool ModuleEnabled = CswConvert.ToBoolean( row["enabled"] );
                if( Enable && false == ModuleEnabled )
                {
                    row["enabled"] = CswConvert.ToDbVal( true );
                    _ModuleRules[Module].Enabled = true;
                    _ModuleRules[Module].Enable();
                }
                else if( false == Enable && ModuleEnabled )
                {
                    row["enabled"] = CswConvert.ToDbVal( false );
                    _ModuleRules[Module].Enabled = false;
                    _ModuleRules[Module].Disable();
                }
            }
            ModuleUpdate.update( ModulesTbl );

            _CswNbtResources.MetaData.ResetEnabledNodeTypes();
            _CswNbtResources.finalize();
            _CswNbtResources.MetaData.refreshAll();

            //We have to clear Session data or the view selects recent views will have non-accesible views and break
            _CswNbtResources.SessionDataMgr.removeAllSessionData( _CswNbtResources.Session.SessionId );
        }

        /// <summary>
        /// Trigger the event appropriate to whether the module is currently enabled or disabled
        /// Use this to sync new or edited events with existing schemata
        /// </summary>
        public void triggerEvent( CswEnumNbtModuleName ModuleName )
        {
            if( IsModuleEnabled( ModuleName ) )
            {
                _ModuleRules[ModuleName].Enable();
            }
            else
            {
                _ModuleRules[ModuleName].Disable();
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
            CswNbtMetaDataObjectClass userOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp usernameOCP = userOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );
            CswNbtView usersView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = usersView.AddViewRelationship( userOC, false );
            usersView.AddViewPropertyAndFilter( parent,
                MetaDataProp : usernameOCP,
                Value : modulename,
                SubFieldName : CswEnumNbtSubFieldName.Text,
                FilterMode : CswEnumNbtFilterMode.Contains );

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
            CswNbtMetaDataObjectClass roleOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp nameOCP = roleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );
            CswNbtView rolesView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = rolesView.AddViewRelationship( roleOC, false );
            rolesView.AddViewPropertyAndFilter( parent,
                MetaDataProp : nameOCP,
                Value : modulename,
                SubFieldName : CswEnumNbtSubFieldName.Text,
                FilterMode : CswEnumNbtFilterMode.Contains );

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
        /// Convenience function for hiding/showing nodes by name. Fragile, so only use it in the case where it's okay if it doesn't execute.
        /// </summary>
        /// <param name="Hidden">True if the node should be hidden</param>
        /// <param name="NodeName">The name of the node to hide</param>
        /// <param name="ObjectClassName">The name of the node's objectclass</param>
        public void ToggleNode( bool Hidden, string NodeName, String ObjectClassName )
        {
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassName );
            foreach( CswNbtNode Node in ObjectClass.getNodes( false, false, IncludeHiddenNodes : true ) )
            {
                if( Node.NodeName == NodeName )
                {
                    Node.Hidden = Hidden;
                    Node.postChanges( false );
                }
            }
        }

        /// <summary>
        /// Convenience function for hiding views (null safe)
        /// </summary>
        /// <param name="hidden">True if the view should be hidden</param>
        /// <param name="viewName">The name of the view to hide/unhide</param>
        /// /// <param name="visibility">the original visibility of the view when not hidden</param>
        public void ToggleView( bool hidden, string viewName, CswEnumNbtViewVisibility Visibility )
        {
            CswEnumNbtViewVisibility FindVisibility = hidden ? Visibility : CswEnumNbtViewVisibility.Hidden;
            CswEnumNbtViewVisibility SetVisibility = hidden ? CswEnumNbtViewVisibility.Hidden : Visibility;

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
        public void ToggleViewsInCategory( bool hidden, string category, CswEnumNbtViewVisibility visibility )
        {
            CswTableSelect tu = _CswNbtResources.makeCswTableSelect( "toggleViewsInCategory_26717", "node_views" );
            DataTable nodeviews = tu.getTable( "where category = '" + category + "'" );
            foreach( DataRow row in nodeviews.Rows )
            {
                ToggleView( hidden, row["viewname"].ToString(), visibility );
            }
        }

        public void ToggleAction( bool showInList, CswEnumNbtActionName actionName )
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

        public void ToggleScheduledRule( CswEnumNbtScheduleRuleNames RuleName, bool Disabled )
        {
            CswTableUpdate RuleUpdate = _CswNbtResources.makeCswTableUpdate( "toggleScheduledRule", "scheduledrules" );
            DataTable RuleDt = RuleUpdate.getTable( "where lower(rulename) = '" + RuleName.ToString().ToLower() + "'" );
            foreach( DataRow row in RuleDt.Rows )
            {
                row["disabled"] = CswConvert.ToDbVal( Disabled );
            }
            RuleUpdate.update( RuleDt );
        }

        public void TogglePrintLabels( bool Hidden, CswEnumNbtModuleName Module )
        {
            CswNbtMetaDataObjectClass printLabelOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
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
                            Value : NodeType.NodeTypeName,
                            FilterMode : CswEnumNbtFilterMode.Contains );

                        first = false;
                    }
                    else
                    {
                        printLabelsView.AddViewPropertyAndFilter( parent, nodetypesOCP,
                            Value : NodeType.NodeTypeName,
                            FilterMode : CswEnumNbtFilterMode.Contains,
                            Conjunction : CswEnumNbtFilterConjunction.Or );
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

        public void AddPropToTab( int NodeTypeId, string PropName, CswNbtMetaDataNodeTypeTab Tab, int Row = Int32.MinValue, int Col = Int32.MinValue, string TabGroup = "" )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, PropName );
            if( null != NodeTypeProp )
            {
                CswNbtMetaDataNodeType locationNT = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( Int32.MinValue != Row && Int32.MinValue != Col )
                {
                    NodeTypeProp.updateLayout( CswEnumNbtLayoutType.Edit, true, Tab.TabId, DisplayRow : Row, DisplayColumn : Col, TabGroup : TabGroup );
                }
                else
                {
                    NodeTypeProp.updateLayout( CswEnumNbtLayoutType.Edit, true, Tab.TabId );
                }
            }
        }

        public void AddPropToTab( int NodeTypeId, string PropName, string TabName, int TabOrder = 99 )
        {
            CswNbtMetaDataNodeTypeTab tab = _CswNbtResources.MetaData.getNodeTypeTab( NodeTypeId, TabName );
            if( null == tab )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                tab = _CswNbtResources.MetaData.makeNewTabNew( NodeType, TabName, TabOrder );
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
            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp categoryOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Category );

            CswNbtView reportsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = reportsView.AddViewRelationship( reportOC, false );
            reportsView.AddViewPropertyAndFilter( parent, categoryOCP,
                Value : Category,
                FilterMode : CswEnumNbtFilterMode.Equals );

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

        public void CreateModuleDependency( CswEnumNbtModuleName ParentModule, CswEnumNbtModuleName ChildModule )
        {
            int parentId = GetModuleId( ParentModule );
            int childId = GetModuleId( ChildModule );

            CswTableUpdate modulesTU = _CswNbtResources.makeCswTableUpdate( "createChildModule", "modules" );
            DataTable modulesDT = modulesTU.getTable( "where moduleid = " + childId );
            foreach( DataRow row in modulesDT.Rows )
            {
                row["prereq"] = parentId;
            }
            modulesTU.update( modulesDT );
        }

        public CswEnumNbtModuleName GetModulePrereq( CswEnumNbtModuleName Module )
        {
            int moduleId = _CswNbtResources.Modules.GetModuleId( Module );
            string sql = @"select m2.name from modules m1
                                join modules m2 on m2.moduleid = m1.prereq
                           where m1.moduleid = :moduleid ";
            
            CswArbitrarySelect modulesAS = _CswNbtResources.makeCswArbitrarySelect( "getPrereq", sql );
            modulesAS.addParameter( "moduleid", moduleId.ToString() );
            DataTable modulesDT = modulesAS.getTable();

            string PrereqName = "";
            foreach( DataRow row in modulesDT.Rows )
            {
                PrereqName = row["name"].ToString();
            }

            return PrereqName;
        }

        public bool ModuleHasPrereq( CswEnumNbtModuleName Module )
        {
            int moduleId = _CswNbtResources.Modules.GetModuleId( Module );
            CswTableSelect modulesTS = _CswNbtResources.makeCswTableSelect( "modulehasparent", "modules" );
            DataTable modulesDT = modulesTS.getTable( "where prereq is not null and moduleid = " + moduleId );
            return modulesDT.Rows.Count > 0;
        }

        public Collection<CswEnumNbtModuleName> GetChildModules( CswEnumNbtModuleName Module )
        {
            Collection<CswEnumNbtModuleName> ret = new Collection<CswEnumNbtModuleName>();

            int moduleId = _CswNbtResources.Modules.GetModuleId( Module );
            string sql = @"select m1.name from modules m1
                               join modules m2 on m2.moduleid = m1.prereq
                           where m1.prereq = :moduleid ";

            CswArbitrarySelect arbSelect = _CswNbtResources.makeCswArbitrarySelect( "ModuleManage.GetChildModules", sql );
            arbSelect.addParameter("moduleid", moduleId.ToString());

            DataTable tbl = arbSelect.getTable();
            foreach( DataRow row in tbl.Rows )
            {
                ret.Add( row["name"].ToString() );
            }

            return ret;
        }

    } // class CswNbtModuleManager

}// namespace ChemSW.Nbt
