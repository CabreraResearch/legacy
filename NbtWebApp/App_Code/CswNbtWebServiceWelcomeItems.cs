using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.NbtWebControls;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the table of components on the Welcome page
    /// </summary>
    public class CswNbtWebServiceWelcomeItems
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceWelcomeItems( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

        }

        /// <summary>
        /// Folder Path for Button Images
        /// </summary>
        public static string IconImageRoot = "Images/biggerbuttons";

        /// <summary>
        /// Types of Welcome Page Components
        /// </summary>
        public enum WelcomeComponentType
        {
            /// <summary>
            /// Link to a View, Report, or Action
            /// </summary>
            Link,
            /// <summary>
            /// Search on a View
            /// </summary>
            Search,
            /// <summary>
            /// Static text
            /// </summary>
            Text,
            /// <summary>
            /// Add link for new node
            /// </summary>
            Add
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


        public JObject GetWelcomeItems( string strRoleId )
        {
            JObject Ret = new JObject();
            //string ret = string.Empty;
            //var ReturnXML = new XmlDocument();
            //XmlNode WelcomeNode = CswXmlDocument.SetDocumentElement( ReturnXML, "welcome" );

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            // Welcome components from database
            DataTable WelcomeTable = _getWelcomeTable( RoleId );

            // see BZ 10234
            if( WelcomeTable.Rows.Count == 0 )
            {
                ResetWelcomeItems( strRoleId );
                WelcomeTable = _getWelcomeTable( RoleId );
            }
            Collection<CswNbtView> VisibleViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false, false, NbtViewRenderingMode.Any );
            
            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                string WelcomeId = WelcomeRow["welcomeid"].ToString();
                Ret[WelcomeId] = new JObject();

                WelcomeComponentType ThisType = (WelcomeComponentType) Enum.Parse( typeof( WelcomeComponentType ), WelcomeRow["componenttype"].ToString(), true );
                string LinkText = string.Empty;

                switch( ThisType )
                {
                    case WelcomeComponentType.Add:
                        if( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) != Int32.MinValue )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) );
                            bool CanAdd = _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, NodeType );
                            if( CanAdd )
                            {
                                if( WelcomeRow["displaytext"].ToString() != string.Empty )
                                    LinkText = WelcomeRow["displaytext"].ToString();
                                else
                                    LinkText = "Add New " + NodeType.NodeTypeName;
                                Ret[WelcomeId]["nodetypeid"] = WelcomeRow["nodetypeid"].ToString();
                                Ret[WelcomeId]["type"] = "add_new_nodetype";
                            }
                        }
                        break;

                    case WelcomeComponentType.Link:
                        if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
                            if( null != ThisView && ThisView.IsFullyEnabled() && VisibleViews.Contains(ThisView) )
                            {
                                // FogBugz case 9552, Keith Baldwin 7/27/2011
                                LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisView.ViewName;
                                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( ThisView, false, true, false, false );
                                if( null != CswNbtTree )
                                {
                                    LinkText += " (" + CswNbtTree.getChildNodeCount().ToString() + ")";
                                }
                                Ret[WelcomeId]["viewid"] = new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString();
                                Ret[WelcomeId]["viewmode"] = ThisView.ViewMode.ToString().ToLower();
                                Ret[WelcomeId]["type"] = "view";

                            }
                        }
                        if( CswConvert.ToInt32( WelcomeRow["actionid"] ) != Int32.MinValue )
                        {
                            CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( WelcomeRow["actionid"] )];
                            if( _CswNbtResources.Permit.can( ThisAction.Name ) )
                            {
                                LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : CswNbtAction.ActionNameEnumToString( ThisAction.Name );
                            }
                            Ret[WelcomeId]["actionid"] = WelcomeRow["actionid"].ToString();
                            Ret[WelcomeId]["actionname"] = ThisAction.Name.ToString();      // not using CswNbtAction.ActionNameEnumToString here
                            Ret[WelcomeId]["actionurl"] = ThisAction.Url;
                            Ret[WelcomeId]["type"] = "action";
                        }
                        if( CswConvert.ToInt32( WelcomeRow["reportid"] ) != Int32.MinValue )
                        {
                            CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( WelcomeRow["reportid"] ) )];
                            LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisReportNode.NodeName;
                            Ret[WelcomeId]["reportid"] = WelcomeRow["reportid"].ToString();
                            Ret[WelcomeId]["type"] = "report";
                        }
                        break;
                    case WelcomeComponentType.Search:
                        if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
                        {
                            CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
                            if( null != ThisView && ThisView.IsSearchable() )
                            {
                                LinkText = WelcomeRow["displaytext"].ToString() != string.Empty ? WelcomeRow["displaytext"].ToString() : ThisView.ViewName;
                                Ret[WelcomeId]["viewid"] = new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString();
                                Ret[WelcomeId]["viewmode"] = ThisView.ViewMode.ToString().ToLower();
                                Ret[WelcomeId]["type"] = "view";
                            }
                        }
                        break;

                    case WelcomeComponentType.Text:
                        LinkText = WelcomeRow["displaytext"].ToString();
                        break;

                } // switch( ThisType )

                if( LinkText != string.Empty )
                {
                    Ret[WelcomeId]["linktype"] = WelcomeRow["componenttype"].ToString();
                    if( WelcomeRow["buttonicon"].ToString() != string.Empty )
                    {
                        Ret[WelcomeId]["buttonicon"] = IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString();
                    }
                    Ret[WelcomeId]["text"] = LinkText;
                    Ret[WelcomeId]["displayrow"] = WelcomeRow["display_row"].ToString();
                    Ret[WelcomeId]["displaycol"] = WelcomeRow["display_col"].ToString();
                }

            } // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

            return Ret;

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

            CswNbtViewId EquipmentByTypeViewId = new CswNbtViewId();
            CswNbtViewId TasksOpenViewId = new CswNbtViewId();
            CswNbtViewId ProblemsOpenViewId = new CswNbtViewId();
            CswNbtViewId FindEquipmentViewId = new CswNbtViewId();

            Collection<CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( false );
            foreach( CswNbtView View in Views )
            {
                if( View.ViewName == "All Equipment" )
                    EquipmentByTypeViewId = View.ViewId;
                if( View.ViewName == "Tasks: Open" )
                    TasksOpenViewId = View.ViewId;
                if( View.ViewName == "Problems: Open" )
                    ProblemsOpenViewId = View.ViewId;
                if( View.ViewName == "Find Equipment" )
                    FindEquipmentViewId = View.ViewId;
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
            if( FindEquipmentViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Search, CswViewListTree.ViewType.View, FindEquipmentViewId.ToString(), Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
            if( EquipmentNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, string.Empty, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
            if( EquipmentByTypeViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, EquipmentByTypeViewId.ToString(), Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

            // Problems
            if( ProblemsOpenViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, ProblemsOpenViewId.ToString(), Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
            if( ProblemNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, string.Empty, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

            // Schedules and Tasks
            if( TasksOpenViewId.isSet() )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, TasksOpenViewId.ToString(), Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
            if( TaskNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, string.Empty, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
            if( ScheduleNodeTypeId != Int32.MinValue )
                _AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, string.Empty, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // ResetWelcomeItems()

        /// <summary>
        /// Adds a welcome component to the welcome page
        /// </summary>
        public void AddWelcomeItem( WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, string PkValue,
                                    Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
        {
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getEmptyTable();

            _AddWelcomeItem( WelcomeTable, ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, RoleId );

            WelcomeUpdate.update( WelcomeTable );
        } // AddWelcomeItem()

        private void _AddWelcomeItem( DataTable WelcomeTable, WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, string PkValue,
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
                    if( NodeTypeId != Int32.MinValue )
                    {
                        NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
                        NewWelcomeRow["buttonicon"] = ButtonIcon;
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select something to add", "No nodetype selected for new Add Welcome Page Component" );
                    break;
                case WelcomeComponentType.Link:
                    switch( ViewType )
                    {
                        case CswViewListTree.ViewType.View:
                            NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( PkValue ).get() );
                            break;
                        case CswViewListTree.ViewType.Action:
                            NewWelcomeRow["actionid"] = CswConvert.ToDbVal( CswConvert.ToInt32( PkValue ) );
                            break;
                        case CswViewListTree.ViewType.Report:
                            NewWelcomeRow["reportid"] = CswConvert.ToDbVal( CswConvert.ToInt32( PkValue ) );
                            break;
                        default:
                            throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Link Welcome Page Component" );
                    }
                    NewWelcomeRow["buttonicon"] = ButtonIcon;
                    NewWelcomeRow["displaytext"] = DisplayText;
                    break;
                case WelcomeComponentType.Search:
                    if( ViewType == CswViewListTree.ViewType.View )
                    {
                        NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( new CswNbtViewId( PkValue ).get() );
                        NewWelcomeRow["buttonicon"] = ButtonIcon;
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must select a view", "No view was selected for new Search Welcome Page Component" );
                    break;
                case WelcomeComponentType.Text:
                    if( DisplayText != string.Empty )
                    {
                        NewWelcomeRow["displaytext"] = DisplayText;
                    }
                    else
                        throw new CswDniException( ErrorType.Warning, "You must enter text to display", "No text entered for new Text Welcome Page Component" );
                    break;
            }
            WelcomeTable.Rows.Add( NewWelcomeRow );

        } // _AddWelcomeItem()


        public bool MoveWelcomeItems( string strRoleId, Int32 WelcomeId, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            if( WelcomeId != Int32.MinValue )
            {
                CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
                DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", WelcomeId );
                if( WelcomeTable.Rows.Count > 0 )
                {
                    DataRow WelcomeRow = WelcomeTable.Rows[0];
                    WelcomeRow["display_row"] = CswConvert.ToDbVal( NewRow );
                    WelcomeRow["display_col"] = CswConvert.ToDbVal( NewColumn );
                    WelcomeUpdate.update( WelcomeTable );
                    ret = true;
                }
            } // if( WelcomeId != Int32.MinValue ) 

            return ret;
        } // MoveWelcomeItems

        public bool DeleteWelcomeItem( string strRoleId, Int32 WelcomeId )
        {
            bool ret = false;

            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( strRoleId );
            Int32 RoleId = RolePk.PrimaryKey;

            if( WelcomeId != Int32.MinValue )
            {
                CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
                DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", WelcomeId );
                if( WelcomeTable.Rows.Count > 0 )
                {
                    DataRow WelcomeRow = WelcomeTable.Rows[0];
                    WelcomeRow.Delete();
                    WelcomeUpdate.update( WelcomeTable );
                    ret = true;
                }
            } // if( WelcomeId != Int32.MinValue ) 

            return ret;
        } // MoveWelcomeItems

        public JObject getButtonIconList()
        {
            JObject Ret = new JObject();

            //ret.Add( new XElement( "icon", new XAttribute( "filename", "blank.gif" ) ) );

            DirectoryInfo d = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.PhysicalApplicationPath + CswWelcomeTable.IconImageRoot );
            FileInfo[] IconFiles = d.GetFiles();
            foreach( FileInfo IconFile in IconFiles
                .Where( IconFile => ( IconFile.Name.EndsWith( ".gif" ) || IconFile.Name.EndsWith( ".jpg" ) || IconFile.Name.EndsWith( ".png" ) ) ) )
            {
                Ret.Add( new JProperty( IconFile.Name ) );
            }
            return Ret;
        } // _initButtonIconList()


    } // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

