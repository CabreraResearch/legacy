using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the table of components on the Welcome page
    /// </summary>
    public class CswNbtWebServiceQuickLaunch : CompositeControl
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceQuickLaunch( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

        }

        /// <summary>
        /// Types of Quick Launch Links
        /// </summary>
        public enum QuickLaunchType
        {
            /// <summary>
            /// Link to a View
            /// </summary>
            View,
            /// <summary>
            /// Link to an Action
            /// </summary>
            Action,
            /// <summary>
            /// Undefined
            /// </summary>
            Unknown
        }


        private DataTable _getWelcomeTable( Int32 RoleId )
        {
            CswTableSelect WelcomeSelect = _CswNbtResources.makeCswTableSelect( "WelcomeSelect", "welcome" );
            string WhereClause = "where roleid = '" + RoleId.ToString() + "'";
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "welcomeid", OrderByType.Ascending ) );
            return WelcomeSelect.getTable( WhereClause, OrderBy );
        } // _getWelcomeTable()


        public string GetWelcomeItems( string strUserId )
        {
            string ret = string.Empty;

            CswPrimaryKey UserPk = new CswPrimaryKey();
            UserPk.FromString( strUserId );

            //CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserPk );
            CswNbtObjClassUser UserOC = CswNbtNodeCaster.AsUser( UserNode );
            CswCommaDelimitedString QuickLaunchViews = UserOC.QuickLaunchViews.SelectedViewIds;
            //CswCommaDelimitedString QuickLaunchActions = UserOC.QuickLaunchActions.


                return String.Empty;

            /*  DataTable WelcomeTable = _getWelcomeTable( RoleId );

            // see BZ 10234
            if( WelcomeTable.Rows.Count == 0 )
            {
                ResetWelcomeItems( strRoleId );
                WelcomeTable = _getWelcomeTable( RoleId );
            }

            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                ret += "<item";
                string LinkText = string.Empty;
                if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                {
                    CswNbtView ThisView = CswNbtViewFactory.restoreView( _CswNbtResources, CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) );
                    if( ThisView.IsFullyEnabled() )
                    {
                        if( WelcomeRow["displaytext"].ToString() != string.Empty )
                            LinkText = WelcomeRow["displaytext"].ToString();
                        else
                            LinkText = ThisView.ViewName;
                        ret += " viewid=\"" + WelcomeRow["nodeviewid"].ToString() + "\"";
                    }
                }
                if( CswConvert.ToInt32( WelcomeRow["actionid"] ) != Int32.MinValue )
                {
                    CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( WelcomeRow["actionid"] )];
                    if( _CswNbtResources.CurrentNbtUser.CheckActionPermission( ThisAction.Name ) )
                    {
                        if( WelcomeRow["displaytext"].ToString() != string.Empty )
                            LinkText = WelcomeRow["displaytext"].ToString();
                        else
                            LinkText = ThisAction.Name.ToString();
                    }
                    ret += " actionid=\"" + WelcomeRow["actionid"].ToString() + "\"";
                }
                if( CswConvert.ToInt32( WelcomeRow["reportid"] ) != Int32.MinValue )
                {
                    CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( WelcomeRow["reportid"] ) )];
                    if( WelcomeRow["displaytext"].ToString() != string.Empty )
                        LinkText = WelcomeRow["displaytext"].ToString();
                    else
                        LinkText = ThisReportNode.NodeName;
                    ret += " reportid=\"" + WelcomeRow["reportid"].ToString() + "\"";
                }
                if( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) != Int32.MinValue )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) );
                    if( WelcomeRow["displaytext"].ToString() != string.Empty )
                        LinkText = WelcomeRow["displaytext"].ToString();
                    else
                        LinkText = "Add New " + NodeType.NodeTypeName;
                    ret += " nodetypeid=\"" + WelcomeRow["nodetypeid"].ToString() + "\"";
                }

                if( LinkText != string.Empty )
                {
                    ret += "      type=\"" + WelcomeRow["componenttype"].ToString() + "\"";
                    ret += "      buttonicon=\"" + IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString() + "\"";
                    ret += "      text=\"" + LinkText + "\"";
                    ret += "      displayrow=\"" + WelcomeRow["display_row"].ToString() + "\"";
                    ret += "      displaycol=\"" + WelcomeRow["display_col"].ToString() + "\"";
                    ret += "/>";
                }

            } // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

            return "<welcome>" + ret + "</welcome>";

        } // GetWelcomeItems()


        public void ResetWelcomeItems( string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // Reset the contents for this role to factory default
            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateReset", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "roleid", RoleId );
            for( Int32 i = 0; i < WelcomeTable.Rows.Count; i++ )
            {
                WelcomeTable.Rows[i].Delete();
            }

            Int32 EquipmentByTypeViewId = Int32.MinValue;
            Int32 TasksOpenViewId = Int32.MinValue;
            Int32 ProblemsOpenViewId = Int32.MinValue;
            Int32 FindEquipmentViewId = Int32.MinValue;

            DataTable ViewsTable = _CswNbtResources.ViewSelect.getVisibleViews( false );
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                if( ViewRow["viewname"].ToString() == "All Equipment" )
                    EquipmentByTypeViewId = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                if( ViewRow["viewname"].ToString() == "Tasks: Open" )
                    TasksOpenViewId = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                if( ViewRow["viewname"].ToString() == "Problems: Open" )
                    ProblemsOpenViewId = CswConvert.ToInt32( ViewRow["nodeviewid"] );
                if( ViewRow["viewname"].ToString() == "Find Equipment" )
                    FindEquipmentViewId = CswConvert.ToInt32( ViewRow["nodeviewid"] );
            }

            Int32 ProblemNodeTypeId = Int32.MinValue;
            Int32 TaskNodeTypeId = Int32.MinValue;
            Int32 ScheduleNodeTypeId = Int32.MinValue;
            Int32 EquipmentNodeTypeId = Int32.MinValue;
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( NodeType.NodeTypeName == "Equipment Problem" )
                    ProblemNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment Task" )
                    TaskNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment Schedule" )
                    ScheduleNodeTypeId = NodeType.FirstVersionNodeTypeId;
                if( NodeType.NodeTypeName == "Equipment" )
                    EquipmentNodeTypeId = NodeType.FirstVersionNodeTypeId;
            }

            // Equipment
            if( FindEquipmentViewId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Search, CswViewListTree.ViewType.View, FindEquipmentViewId, Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
            if( EquipmentNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
            if( EquipmentByTypeViewId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, EquipmentByTypeViewId, Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

            // Problems
            if( ProblemsOpenViewId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, ProblemsOpenViewId, Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
            if( ProblemNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

            // Schedules and Tasks
            if( TasksOpenViewId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, TasksOpenViewId, Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
            if( TaskNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
            if( ScheduleNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // ResetWelcomeItems()

        /// <summary>
        /// Adds a welcome component to the welcome page
        /// </summary>
        public void AddWelcomeItem( WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, Int32 ViewValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getEmptyTable();

            _AddWelcomeItem( WelcomeTable, ComponentType, ViewType, ViewValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // AddWelcomeItem()

        private void _AddWelcomeItem( DataTable WelcomeTable, WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, Int32 ViewValue,
                                      Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, Int32 RoleId )
        {
            if( Row == Int32.MinValue )
            {
                string SqlText = @"select max(display_row) maxcol
                                     from welcome
                                    where display_col = 1
                                      and (roleid = " + RoleId.ToString() + @")";
                CswArbitrarySelect WelcomeSelect = _CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_WelcomeSelect", SqlText );
                DataTable WelcomeSelectTable = WelcomeSelect.getTable();
                Int32 MaxRow = 0;
                if( WelcomeSelectTable.Rows.Count > 0 )
                {
                    MaxRow = CswConvert.ToInt32( WelcomeSelectTable.Rows[0]["maxcol"] );
                    if( MaxRow < 0 ) MaxRow = 0;
                }
                Row = MaxRow + 1;
                Column = 1;
            }

            if( ButtonIcon == "blank.gif" )
                ButtonIcon = string.Empty;

            DataRow NewWelcomeRow = WelcomeTable.NewRow();
            NewWelcomeRow["roleid"] = RoleId;
            NewWelcomeRow["componenttype"] = ComponentType.ToString();
            NewWelcomeRow["display_col"] = Column;
            NewWelcomeRow["display_row"] = Row;

            switch( ComponentType )
            {
                case WelcomeComponentType.Add:
                    NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case WelcomeComponentType.Link:
                    switch( ViewType )
                    {
                        case CswViewListTree.ViewType.View:
                            NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        case CswViewListTree.ViewType.Action:
                            NewWelcomeRow["actionid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        case CswViewListTree.ViewType.Report:
                            NewWelcomeRow["reportid"] = CswConvert.ToDbVal( ViewValue );
                            break;
                        default:
                            throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
                    }
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case WelcomeComponentType.Search:
                    if( ViewType == CswViewListTree.ViewType.View )
                    {
                        NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( ViewValue );
                        NewWelcomeRow["buttonicon"] = ButtonIcon;
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
                    break;
                case WelcomeComponentType.Text:
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
            }
            WelcomeTable.Rows.Add( NewWelcomeRow );

            
           */
        } // _AddWelcomeItem()

    } // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

