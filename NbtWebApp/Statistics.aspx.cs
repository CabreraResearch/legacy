using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebPages
{
    public partial class Statistics : System.Web.UI.Page
    {
        public enum StatisticsDisplayMode
        {
            Schema,
            User
        };

        private StatisticsDisplayMode _DisplayMode;

        private bool _NbtMgrEnabled = false;
        private string LimitToAccessId = string.Empty;

        private Hashtable RowHash;
        private static string NameRowName = "Name";
        private static string UserIdRowName = "User ID";
        private static string AccessIdRowName = "Access ID";
        private static string LastLogoutRowName = "Last Logout";
        private static string TotalUsersRowName = "Total Users";
        private static string PeakUsersRowName = "Peak Simultaneous Users";
        private static string PeakUsersCountRowName = "Peak Simultaneous Users Count";
        private static string AverageServerTimeRowName = "Average Server Time";
        private static string TotalSessionsRowName = "Total Sessions";
        private static string TotalNodesRowName = "Total Nodes";
        private static string ErrorsRowName = "Errors";
        private static string AverageSessionLengthRowName = "Average Session Length";
        private static string MostSessionsPerUserRowName = "Most Sessions Per User";
        private static string ActionLoadsCountRowName = "Action Usage Count";
        private static string MultiEditCountRowName = "Multi-Edit Usage Count";
        private static string ReportRunsCountRowName = "Reports Run Count";
        private static string ViewsLoadCountRowName = "Views Loaded Count";
        private static string SearchesLoadCountRowName = "Searches Loaded Count";
        private static string ViewFilterModsRowName = "View Filters Modified";
        private static string NodesSavedCountRowName = "Nodes Saved";
        private static string NodesAddedCountRowName = "Nodes Added";
        private static string NodesCopiedCountRowName = "Nodes Copied";
        private static string NodesDeletedCountRowName = "Nodes Deleted";
        private static string ViewsEditedCountRowName = "View Edits";
        private static string SwitchModeRowName = "More Statistics";

        private ListBox AccessIdListBox;
        private ListBox UserIdListBox;
        private DropDownList TimeList;
        private CheckBox AverageOnlyCheckBox;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                _NbtMgrEnabled = Master.CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.NBTManager );

                _DisplayMode = StatisticsDisplayMode.Schema;
                if( Request.QueryString["mode"] == "user" )
                    _DisplayMode = StatisticsDisplayMode.User;

                if( _DisplayMode == StatisticsDisplayMode.User )
                {
                    if( Request.QueryString["accessid"] != null && Request.QueryString["accessid"] != string.Empty )
                        LimitToAccessId = Request.QueryString["accessid"];
                    else
                        LimitToAccessId = Master.AccessID;
                }

                if( !_NbtMgrEnabled )
                    LimitToAccessId = Master.AccessID;

                if( _DisplayMode == StatisticsDisplayMode.User )
                {
                    LinkButton BackLink = new LinkButton();
                    BackLink.ID = "BackLink";
                    BackLink.Text = "Back to Schema Statistics<br><br>";
                    BackLink.PostBackUrl = "Statistics.aspx?accessid=" + LimitToAccessId;
                    leftph.Controls.Add( BackLink );
                }

                if( _NbtMgrEnabled && _DisplayMode == StatisticsDisplayMode.Schema )
                {
                    Literal AccessIdListBoxLiteral = new Literal();
                    AccessIdListBoxLiteral.ID = "AccessIdListBoxLiteral";
                    AccessIdListBoxLiteral.Text = "Customers to Display:<br>";
                    leftph.Controls.Add( AccessIdListBoxLiteral );

                    AccessIdListBox = new ListBox();
                    AccessIdListBox.ID = "AccessIdListBox";
                    AccessIdListBox.SelectionMode = ListSelectionMode.Multiple;
                    AccessIdListBox.Height = Unit.Parse( "300px" );

                    CswNbtMetaDataObjectClass CustomerOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
                    //CswNbtView CustomerView = Master.CswNbtResources.Trees.getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
                    //ICswNbtTree CustomerTree = Master.CswNbtResources.Trees.getTreeFromView( CustomerView, true, true, false, false );
                    Collection<CswNbtNode> CustomerNodes = CustomerOC.getNodes( false, false );
                    // Sort by Customer Name
                    SortedList CustomerList = new SortedList();
                    ArrayList AccessIds = new ArrayList( Master.CswNbtResources.CswDbCfgInfo.AccessIds );
                    foreach( string AccessId in AccessIds )
                    {
                        CswNbtNode CustomerNode = null;
                        //CustomerTree.goToRoot();
                        //for( int i = 0; i < CustomerTree.getChildNodeCount(); i++ )
                        //{
                        //    CustomerTree.goToNthChild( i );
                        foreach( CswNbtNode ThisCustomerNode in CustomerNodes )
                        {
                            //CswNbtNode ThisCustomerNode = CustomerTree.getNodeForCurrentPosition();
                            if( ( (CswNbtObjClassCustomer) ThisCustomerNode ).CompanyID.Text == AccessId )
                            {
                                CustomerNode = ThisCustomerNode;
                                break;
                            }
                            //CustomerTree.goToParentNode();
                        }

                        if( CustomerNode != null )
                            CustomerList.Add( CustomerNode.NodeName + " (" + AccessId + ")", AccessId );
                        else
                            CustomerList.Add( "Unassigned (" + AccessId + ")", AccessId );
                    }

                    foreach( string Name in CustomerList.Keys )
                    {
                        ListItem CustomerItem = new ListItem( Name, CustomerList[Name].ToString() );
                        AccessIdListBox.Items.Add( CustomerItem );
                    }
                    leftph.Controls.Add( AccessIdListBox );
                }

                if( _DisplayMode == StatisticsDisplayMode.User )
                {
                    // Set the connection to the target AccessId
                    Master.CswNbtResources.AccessId = LimitToAccessId;

                    Literal UserIdListBoxLiteral = new Literal();
                    UserIdListBoxLiteral.ID = "UserIdListBoxLiteral";
                    UserIdListBoxLiteral.Text = "Users to Display:<br>";
                    leftph.Controls.Add( UserIdListBoxLiteral );

                    UserIdListBox = new ListBox();
                    UserIdListBox.ID = "UserIdListBox";
                    UserIdListBox.SelectionMode = ListSelectionMode.Multiple;
                    UserIdListBox.Height = Unit.Parse( "300px" );

                    //CswNbtView UserView = Master.CswNbtResources.Trees.getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                    //ICswNbtTree UserTree = Master.CswNbtResources.Trees.getTreeFromView( UserView, true, true, false, false );

                    //UserTree.goToRoot();
                    //for( int i = 0; i < UserTree.getChildNodeCount(); i++ )
                    //{
                    //    UserTree.goToNthChild( i );
                    //    CswNbtNode ThisUserNode = UserTree.getNodeForCurrentPosition();
                    CswNbtMetaDataObjectClass UserOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                    foreach( CswNbtNode ThisUserNode in UserOC.getNodes( false, false ) )
                    {
                        UserIdListBox.Items.Add( new ListItem( ThisUserNode.NodeName, ThisUserNode.NodeId.PrimaryKey.ToString() ) );
                        //UserTree.goToParentNode();
                    }
                    leftph.Controls.Add( UserIdListBox );

                    // Set the connection back to the original
                    Master.CswNbtResources.AccessId = Master.AccessID;
                }

                leftph.Controls.Add( new CswLiteralBr() );

                AverageOnlyCheckBox = new CheckBox();
                AverageOnlyCheckBox.ID = "AverageOnlyCheckBox";
                AverageOnlyCheckBox.AutoPostBack = false;
                AverageOnlyCheckBox.Text = "Display Average Only";
                leftph.Controls.Add( AverageOnlyCheckBox );

                leftph.Controls.Add( new CswLiteralBr() );

                TimeList = new DropDownList();
                TimeList.ID = "TimeList";
                TimeList.CssClass = "selectinput";
                TimeList.Items.Add( new ListItem( "1 month", "1" ) );
                TimeList.Items.Add( new ListItem( "3 months", "3" ) );
                TimeList.Items.Add( new ListItem( "6 months", "6" ) );
                TimeList.Items.Add( new ListItem( "12 months", "12" ) );
                leftph.Controls.Add( TimeList );

                Button SubmitButton = new Button();
                SubmitButton.ID = "SubmitButton";
                SubmitButton.CssClass = "Button";
                SubmitButton.Text = "Display";
                leftph.Controls.Add( SubmitButton );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );

        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                if( AccessIdListBox != null && AccessIdListBox.SelectedItem == null )
                {
                    if( Request.QueryString["accessid"] != null && Request.QueryString["accessid"] != string.Empty )
                        AccessIdListBox.SelectedValue = Request.QueryString["accessid"];
                    else
                        AccessIdListBox.SelectedIndex = 0;
                }
                if( UserIdListBox != null && UserIdListBox.SelectedItem == null )
                    UserIdListBox.SelectedIndex = 0;

                CswAutoTable CustomerDataTable = new CswAutoTable();
                CustomerDataTable.ID = "StatisticsTable";
                CustomerDataTable.CssClass = "StatisticsTable";
                CustomerDataTable.EvenRowCssClass = "EvenRow";
                CustomerDataTable.OddRowCssClass = "OddRow";

                // If you want to change the display order of rows, here's where you do it.
                Int32 i = 0;
                RowHash = new Hashtable();
                if( _NbtMgrEnabled || _DisplayMode == StatisticsDisplayMode.User )
                    RowHash.Add( NameRowName, i++ );
                if( _DisplayMode == StatisticsDisplayMode.User )
                    RowHash.Add( UserIdRowName, i++ );
                RowHash.Add( AccessIdRowName, i++ );
                RowHash.Add( LastLogoutRowName, i++ );
                if( _DisplayMode == StatisticsDisplayMode.Schema )
                {
                    RowHash.Add( PeakUsersRowName, i++ );
                    RowHash.Add( PeakUsersCountRowName, i++ );
                    RowHash.Add( TotalUsersRowName, i++ );
                }
                RowHash.Add( TotalSessionsRowName, i++ );
                RowHash.Add( AverageServerTimeRowName, i++ );
                RowHash.Add( AverageSessionLengthRowName, i++ );
                if( _DisplayMode == StatisticsDisplayMode.Schema )
                    RowHash.Add( MostSessionsPerUserRowName, i++ );
                if( _DisplayMode == StatisticsDisplayMode.Schema )
                    RowHash.Add( TotalNodesRowName, i++ );
                RowHash.Add( ActionLoadsCountRowName, i++ );
                RowHash.Add( MultiEditCountRowName, i++ );
                RowHash.Add( ReportRunsCountRowName, i++ );
                RowHash.Add( ViewsLoadCountRowName, i++ );
                RowHash.Add( ViewsEditedCountRowName, i++ );
                RowHash.Add( SearchesLoadCountRowName, i++ );
                RowHash.Add( ViewFilterModsRowName, i++ );
                RowHash.Add( NodesSavedCountRowName, i++ );
                RowHash.Add( NodesAddedCountRowName, i++ );
                RowHash.Add( NodesCopiedCountRowName, i++ );
                RowHash.Add( NodesDeletedCountRowName, i++ );
                RowHash.Add( ErrorsRowName, i++ );
                if( _DisplayMode == StatisticsDisplayMode.Schema )
                    RowHash.Add( SwitchModeRowName, i++ );

                // First Column
                foreach( string RowName in RowHash.Keys )
                {
                    Label RowNameLiteral = new Label();
                    RowNameLiteral.Text = RowName;
                    RowNameLiteral.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
                    CustomerDataTable.addControl( CswConvert.ToInt32( RowHash[RowName] ), 0, RowNameLiteral );
                }

                DateTime StartDate = DateTime.Now.AddMonths( -1 * CswConvert.ToInt32( TimeList.SelectedValue ) );
                DateTime EndDate = DateTime.Now.AddDays( 1 );   // EndDate = tomorrow will include today's data

                // Data Columns
                Int32 aggregatecol = 1;
                Int32 col = 2;
                if( _DisplayMode == StatisticsDisplayMode.Schema )
                {
                    if( _NbtMgrEnabled )
                    {

                        ArrayList AccessIds = new ArrayList( Master.CswNbtResources.CswDbCfgInfo.AccessIds );
                        foreach( string AccessId in AccessIds )
                        {
                            ListItem AccessIdItem = AccessIdListBox.Items.FindByValue( AccessId );
                            if( AccessIdItem != null && AccessIdItem.Selected )
                            {
                                _makeDataColumn( AccessId, null, CustomerDataTable, col, StartDate, EndDate, !AverageOnlyCheckBox.Checked );
                                col++;
                            }
                        }
                    }
                    else
                    {
                        _makeDataColumn( LimitToAccessId, null, CustomerDataTable, 1, StartDate, EndDate, !AverageOnlyCheckBox.Checked );
                    }
                }
                else
                {
                    foreach( ListItem UserItem in UserIdListBox.Items )
                    {
                        if( UserItem.Selected )
                        {
                            _makeDataColumn( LimitToAccessId, new CswPrimaryKey( "nodes", CswConvert.ToInt32( UserItem.Value ) ), CustomerDataTable, col, StartDate, EndDate, !AverageOnlyCheckBox.Checked );
                            col++;
                        }
                    }
                }

                if( Aggregate_Count > 1 || AverageOnlyCheckBox.Checked )
                    _displayAggregateColumn( CustomerDataTable, aggregatecol );

                rightph.Controls.Add( CustomerDataTable );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnLoad( e );
        }


        private Int32 Aggregate_PeakUsers = 0;
        private Int32 Aggregate_PeakUsersCount = 0;
        private Int32 Aggregate_TotalNodes = 0;
        private Int32 Aggregate_UserCount = 0;
        private Int32 Aggregate_TotalSessions = 0;
        private double Aggregate_PageTimeTotal = 0;
        private Int32 Aggregate_PageCount = 0;
        private TimeSpan Aggregate_SessionTotalTime = new TimeSpan();
        private Int32 Aggregate_SessionCount = 0;
        private Int32 Aggregate_ActionLoadsCount = 0;
        private Int32 Aggregate_MultiEditCount = 0;
        private Int32 Aggregate_ReportRunsCount = 0;
        private Int32 Aggregate_ViewsLoadCount = 0;
        private Int32 Aggregate_NodesSavedCount = 0;
        private Int32 Aggregate_NodesAddedCount = 0;
        private Int32 Aggregate_NodesCopiedCount = 0;
        private Int32 Aggregate_NodesDeletedCount = 0;
        private Int32 Aggregate_ViewsEditedCount = 0;
        private Int32 Aggregate_SearchesLoadCount = 0;
        private Int32 Aggregate_ViewFilterMods = 0;
        private Int32 Aggregate_Errors = 0;
        private Int32 Aggregate_Count = 0;


        private void _makeDataColumn( string AccessId, CswPrimaryKey LimitToUserId, CswAutoTable AutoTable, Int32 ColumnNo, DateTime StartDate, DateTime EndDate, bool Display )
        {
            // ------------------------------------------------------------------------------
            // Setup data retrieval 

            string CustomerName = string.Empty;
            CswPrimaryKey CustomerNodeId = null;
            Int32 UserLimit = Int32.MinValue;
            if( _NbtMgrEnabled )
            {
                CswNbtNode CustomerNode = _getCustomerNode( AccessId );
                if( CustomerNode != null )
                {
                    CustomerNodeId = CustomerNode.NodeId;
                    CustomerName = CustomerNode.NodeName;
                    UserLimit = CswConvert.ToInt32( ( (CswNbtObjClassCustomer) CustomerNode ).UserCount.Value );
                }
                CustomerNode = null;
            }

            // Set the connection to the target AccessId
            string PreviousAccessId = Master.CswNbtResources.AccessId;

            //bz #: 8576 commit is no longer necessary. 
            //Master.CswNbtResources.CswDbResources.commitTransaction(); //KLUDGE: bz # 8416( cf. 8576)
            Master.AccessID = AccessId;
            //Master.CswNbtResources.AccessId = AccessId;

            CswTableSelect StatisticsTableSelect = Master.CswNbtResources.makeCswTableSelect( "statistics_makeDataColumn_select", "Statistics" );
            //StatisticsTableCaddy.addOrderByColumn( "statisticsid" );

            string WhereClause = string.Empty;
            if( LimitToUserId != null )
            {
                if( WhereClause != string.Empty ) WhereClause += " and ";
                WhereClause += "userid = " + LimitToUserId.PrimaryKey.ToString();
            }
            if( StartDate > DateTime.MinValue )
            {
                if( WhereClause != string.Empty ) WhereClause += " and ";
                WhereClause += "logoutdate >= " + Master.CswNbtResources.getDbNativeDate( StartDate );
            }
            if( EndDate > DateTime.MinValue )
            {
                if( WhereClause != string.Empty ) WhereClause += " and ";
                WhereClause += "logoutdate <= " + Master.CswNbtResources.getDbNativeDate( EndDate );
            }
            if( WhereClause != string.Empty )
                WhereClause = " where " + WhereClause;

            DataTable StatisticsTable = StatisticsTableSelect.getTable( WhereClause );

            CswNbtNode UserNode = null;
            CswTableSelect NodesSelect = null;
            CswNbtView UserView = null;
            ICswNbtTree UserTree = null;
            CswStaticSelect SimultaneousUsageSelect = Master.CswNbtResources.makeCswStaticSelect( "SimultaneousUsageSelect", "getSimultaneousUsage" );
            if( LimitToUserId != null )
            {
                UserNode = Master.CswNbtResources.Nodes[LimitToUserId];
            }
            else
            {
                NodesSelect = Master.CswNbtResources.makeCswTableSelect( "totalnodes_select", "nodes" );
                CswNbtMetaDataObjectClass UserOC = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                UserView = UserOC.CreateDefaultView(); // Master.CswNbtResources.Trees.getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                UserTree = Master.CswNbtResources.Trees.getTreeFromView( Master.CswNbtResources.CurrentNbtUser, UserView, true, false, false );
            }

            // ------------------------------------------------------------------------------
            // Get data

            Int32 TotalNodes = 0;
            Int32 UserCount = 0;
            Int32 TotalSessions = 0;
            double PageTimeTotal = 0;
            Int32 PageCount = 0;
            TimeSpan SessionTotalTime = new TimeSpan();
            Int32 SessionCount = 0;
            Hashtable SessionsPerUserHash = new Hashtable();
            Int32 ActionLoadsCount = 0;
            Int32 MultiEditCount = 0;
            Int32 ReportRunsCount = 0;
            Int32 ViewsLoadCount = 0;
            Int32 NodesSavedCount = 0;
            Int32 NodesAddedCount = 0;
            Int32 NodesCopiedCount = 0;
            Int32 NodesDeletedCount = 0;
            Int32 ViewsEditedCount = 0;
            Int32 SearchesLoadCount = 0;
            Int32 ViewFilterMods = 0;
            Int32 Errors = 0;
            DateTime LastLogout = DateTime.MinValue;
            Int32 PeakUsers = 0;
            Int32 PeakUsersCount = 0;

            if( LimitToUserId == null )
            {
                TotalNodes = NodesSelect.getRecordCount();
                UserCount = UserTree.getChildNodeCount();

                SimultaneousUsageSelect.S4Parameters.Add( "getbeforedate", new CswStaticParam( "getbeforedate", Master.CswNbtResources.getDbNativeDate( EndDate ), true ) );
                SimultaneousUsageSelect.S4Parameters.Add( "getafterdate", new CswStaticParam( "getafterdate", Master.CswNbtResources.getDbNativeDate( StartDate ), true ) );
                DataTable SimultaneousUsageTable = SimultaneousUsageSelect.getTable();
                //PeakUsers = CswConvert.ToInt32( SimultaneousUsageTable.Rows[0]["maxcnt"] );
                _getPeakSimultaneousUsers( SimultaneousUsageTable, out PeakUsers, out PeakUsersCount );
            }

            TotalSessions = StatisticsTable.Rows.Count;

            foreach( DataRow StatRow in StatisticsTable.Rows )
            {
                double ThisPageTimeTotal = Convert.ToDouble( StatRow["average_servertime"].ToString() );
                Int32 ThisPageCount = CswConvert.ToInt32( StatRow["count_lifecycles"].ToString() );
                PageTimeTotal += ThisPageTimeTotal * ThisPageCount;
                PageCount += ThisPageCount;

                DateTime LogoutDate = Convert.ToDateTime( StatRow["logoutdate"] );
                DateTime LoginDate = Convert.ToDateTime( StatRow["logindate"] );
                if( LogoutDate > LastLogout )
                    LastLogout = LogoutDate;

                TimeSpan ThisSessionTime = LogoutDate.Subtract( LoginDate );
                SessionTotalTime += ThisSessionTime;
                SessionCount++;

                if( LimitToUserId == null )
                {
                    CswPrimaryKey ThisUserID = new CswPrimaryKey( "nodes", CswConvert.ToInt32( StatRow["userid"] ) );
                    if( SessionsPerUserHash[ThisUserID] != null )
                        SessionsPerUserHash[ThisUserID] = CswConvert.ToInt32( SessionsPerUserHash[ThisUserID] ) + 1;
                    else
                        SessionsPerUserHash[ThisUserID] = 1;
                }

                if( StatRow["count_actionloads"] != null && StatRow["count_actionloads"].ToString() != string.Empty )
                    ActionLoadsCount += CswConvert.ToInt32( StatRow["count_actionloads"] );
                if( StatRow["count_multiedit"] != null && StatRow["count_multiedit"].ToString() != string.Empty )
                    MultiEditCount += CswConvert.ToInt32( StatRow["count_multiedit"] );
                if( StatRow["count_reportruns"] != null && StatRow["count_reportruns"].ToString() != string.Empty )
                    ReportRunsCount += CswConvert.ToInt32( StatRow["count_reportruns"] );
                if( StatRow["count_viewloads"] != null && StatRow["count_viewloads"].ToString() != string.Empty )
                    ViewsLoadCount += CswConvert.ToInt32( StatRow["count_viewloads"] );
                if( StatRow["count_nodessaved"] != null && StatRow["count_nodessaved"].ToString() != string.Empty )
                    NodesSavedCount += CswConvert.ToInt32( StatRow["count_nodessaved"] );
                if( StatRow["count_nodesadded"] != null && StatRow["count_nodesadded"].ToString() != string.Empty )
                    NodesAddedCount += CswConvert.ToInt32( StatRow["count_nodesadded"] );
                if( StatRow["count_nodescopied"] != null && StatRow["count_nodescopied"].ToString() != string.Empty )
                    NodesCopiedCount += CswConvert.ToInt32( StatRow["count_nodescopied"] );
                if( StatRow["count_nodesdeleted"] != null && StatRow["count_nodesdeleted"].ToString() != string.Empty )
                    NodesDeletedCount += CswConvert.ToInt32( StatRow["count_nodesdeleted"] );
                if( StatRow["count_viewsedited"] != null && StatRow["count_viewsedited"].ToString() != string.Empty )
                    ViewsEditedCount += CswConvert.ToInt32( StatRow["count_viewsedited"] );
                if( StatRow["count_errors"] != null && StatRow["count_errors"].ToString() != string.Empty )
                    Errors += CswConvert.ToInt32( StatRow["count_errors"] );
                if( StatRow["count_searches"] != null && StatRow["count_searches"].ToString() != string.Empty )
                    SearchesLoadCount += CswConvert.ToInt32( StatRow["count_searches"] );
                if( StatRow["count_viewfiltermods"] != null && StatRow["count_viewfiltermods"].ToString() != string.Empty )
                    ViewFilterMods += CswConvert.ToInt32( StatRow["count_viewfiltermods"] );
            }

            double AverageServerTime = 0;
            if( PageCount != 0 )
                AverageServerTime = Math.Round( ( PageTimeTotal / PageCount ), 0, MidpointRounding.AwayFromZero );

            TimeSpan AverageSessionTime = new TimeSpan();
            if( SessionCount != 0 )
                AverageSessionTime = new TimeSpan( SessionTotalTime.Ticks / SessionCount );

            Int32 MostSessions = 0;
            CswPrimaryKey MostSessionsUserId = null;
            string MostSessionsUserName = string.Empty;
            if( LimitToUserId == null )
            {
                foreach( CswPrimaryKey UserId in SessionsPerUserHash.Keys )
                {
                    if( CswConvert.ToInt32( SessionsPerUserHash[UserId] ) > MostSessions )
                    {
                        MostSessions = CswConvert.ToInt32( SessionsPerUserHash[UserId] );
                        MostSessionsUserId = UserId;
                    }
                }
                if( MostSessionsUserId != null )
                {
                    CswNbtNode MostSessionsUserNode = UserTree.getNode( UserTree.getNodeKeyByNodeId( MostSessionsUserId ) );
                    if( MostSessionsUserNode != null )
                        MostSessionsUserName = MostSessionsUserNode.NodeName;
                }
            }

            // ------------------------------------------------------------------------------
            // Increment Aggregates
            Aggregate_PeakUsers += PeakUsers;
            Aggregate_PeakUsersCount += PeakUsersCount;
            Aggregate_TotalNodes += TotalNodes;
            Aggregate_UserCount += UserCount;
            Aggregate_TotalSessions += TotalSessions;
            Aggregate_PageTimeTotal += PageTimeTotal;
            Aggregate_PageCount += PageCount;
            Aggregate_SessionTotalTime += SessionTotalTime;
            Aggregate_SessionCount += SessionCount;
            Aggregate_ActionLoadsCount += ActionLoadsCount;
            Aggregate_MultiEditCount += MultiEditCount;
            Aggregate_ReportRunsCount += ReportRunsCount;
            Aggregate_ViewsLoadCount += ViewsLoadCount;
            Aggregate_NodesSavedCount += NodesSavedCount;
            Aggregate_NodesAddedCount += NodesAddedCount;
            Aggregate_NodesCopiedCount += NodesCopiedCount;
            Aggregate_NodesDeletedCount += NodesDeletedCount;
            Aggregate_ViewsEditedCount += ViewsEditedCount;
            Aggregate_SearchesLoadCount += SearchesLoadCount;
            Aggregate_ViewFilterMods += ViewFilterMods;
            Aggregate_Errors += Errors;
            Aggregate_Count++;


            // ------------------------------------------------------------------------------
            // Add data to table
            string UserName = string.Empty;
            if( UserNode != null )
                UserName = UserNode.NodeName;

            if( Display )
            {
                _displayDataColumn( AutoTable, ColumnNo, true, ( LimitToUserId == null ), StartDate, EndDate,
                                   LimitToUserId, UserName, CustomerNodeId, CustomerName, AccessId, LastLogout,
                                   TotalNodes, UserCount, MostSessions, MostSessionsUserName, TotalSessions,
                                   PeakUsers, PeakUsersCount, UserLimit,
                                   AverageServerTime, AverageSessionTime, ActionLoadsCount, MultiEditCount,
                                   ReportRunsCount, ViewsLoadCount, ViewsEditedCount, SearchesLoadCount,
                                   ViewFilterMods,
                                   NodesSavedCount, NodesAddedCount,
                                   NodesCopiedCount, NodesDeletedCount, Errors );
            }

            // ------------------------------------------------------------------------------
            // Set the connection back to the original
            //Master.CswNbtResources.AccessId = Master.AccessID;
            //Master.CswNbtResources.CswDbResources.commitTransaction();//KLUDGE: bz # 8416 (cf. 8576)
            Master.CswNbtResources.finalize();
            Master.AccessID = PreviousAccessId;
        }


        private void _displayAggregateColumn( CswAutoTable AutoTable, Int32 ColumnNo )
        {
            double Aggregate_AverageServerTime = 0;
            if( Aggregate_PageCount != 0 )
                Aggregate_AverageServerTime = Math.Round( ( Aggregate_PageTimeTotal / Aggregate_PageCount ), 0, MidpointRounding.AwayFromZero );

            TimeSpan Aggregate_AverageSessionTime = new TimeSpan();
            if( Aggregate_SessionCount != 0 )
                Aggregate_AverageSessionTime = new TimeSpan( Aggregate_SessionTotalTime.Ticks / Aggregate_SessionCount );


            _displayDataColumn( AutoTable, ColumnNo, false, false, DateTime.MinValue, DateTime.MinValue,
                                null, "[Average]", null, "[Average]", string.Empty, DateTime.MinValue,
                                Aggregate_TotalNodes / Aggregate_Count,
                                Aggregate_UserCount / Aggregate_Count,
                                Int32.MinValue,
                                string.Empty,
                                Aggregate_TotalSessions / Aggregate_Count,
                                Aggregate_PeakUsers / Aggregate_Count,
                                Aggregate_PeakUsersCount / Aggregate_Count,
                                Int32.MinValue,
                                Aggregate_AverageServerTime,
                                Aggregate_AverageSessionTime,
                                Aggregate_ActionLoadsCount / Aggregate_Count,
                                Aggregate_MultiEditCount / Aggregate_Count,
                                Aggregate_ReportRunsCount / Aggregate_Count,
                                Aggregate_ViewsLoadCount / Aggregate_Count,
                                Aggregate_ViewsEditedCount / Aggregate_Count,
                                Aggregate_SearchesLoadCount / Aggregate_Count,
                                Aggregate_ViewFilterMods / Aggregate_Count,
                                Aggregate_NodesSavedCount / Aggregate_Count,
                                Aggregate_NodesAddedCount / Aggregate_Count,
                                Aggregate_NodesCopiedCount / Aggregate_Count,
                                Aggregate_NodesDeletedCount / Aggregate_Count,
                                Aggregate_Errors / Aggregate_Count );
        }


        private void _displayDataColumn( CswAutoTable AutoTable, Int32 ColumnNo, bool ShowPopupLinks, bool ShowModeLink, DateTime StartDate, DateTime EndDate,
                                        CswPrimaryKey LimitToUserId, string UserName, CswPrimaryKey CustomerNodeId, string CustomerName, string AccessId, DateTime LastLogout,
                                        Int32 TotalNodes, Int32 UserCount, Int32 MostSessions, string MostSessionsUserName, Int32 TotalSessions,
                                        Int32 PeakUsers, Int32 PeakUsersCount, Int32 UserLimit,
                                        double AverageServerTime, TimeSpan AverageSessionTime, Int32 ActionLoadsCount,
                                        Int32 MultiEditCount, Int32 ReportRunsCount, Int32 ViewsLoadCount, Int32 ViewsEditedCount, Int32 SearchesLoadCount,
                                        Int32 ViewFilterMods, Int32 NodesSavedCount, Int32 NodesAddedCount,
                                        Int32 NodesCopiedCount, Int32 NodesDeletedCount, Int32 Errors )
        {
            if( _DisplayMode == StatisticsDisplayMode.User )
            {
                if( LimitToUserId != null && UserName != string.Empty )
                {
                    LinkButton UserNameLink = new LinkButton();
                    UserNameLink.ID = "UserNameLink";
                    UserNameLink.Text = UserName;
                    UserNameLink.OnClientClick = "openEditNodePopupFromNodeId('" + LimitToUserId.ToString() + "'); return false;";
                    AutoTable.addControl( CswConvert.ToInt32( RowHash[NameRowName] ), ColumnNo, UserNameLink );
                }
                else
                {
                    Literal UserNameLiteral = new Literal();
                    if( UserName != string.Empty )
                        UserNameLiteral.Text = UserName;
                    else
                        UserNameLiteral.Text = "&nbsp;";
                    AutoTable.addControl( CswConvert.ToInt32( RowHash[NameRowName] ), ColumnNo, UserNameLiteral );
                }

                Literal UserIdLiteral = new Literal();
                if( LimitToUserId != null )
                    UserIdLiteral.Text = LimitToUserId.PrimaryKey.ToString();
                else
                    UserIdLiteral.Text = "&nbsp;";
                AutoTable.addControl( CswConvert.ToInt32( RowHash[UserIdRowName] ), ColumnNo, UserIdLiteral );
            }
            else if( _NbtMgrEnabled )
            {
                if( CustomerNodeId != null )
                {
                    LinkButton CustomerNameLink = new LinkButton();
                    CustomerNameLink.ID = "CustomerNameLink";
                    CustomerNameLink.Text = CustomerName;
                    CustomerNameLink.OnClientClick = "openEditNodePopupFromNodeId('" + CustomerNodeId.ToString() + "'); return false;";
                    AutoTable.addControl( CswConvert.ToInt32( RowHash[NameRowName] ), ColumnNo, CustomerNameLink );
                }
                else
                {
                    Literal CustomerNameLiteral = new Literal();
                    CustomerNameLiteral.Text = CustomerName;
                    AutoTable.addControl( CswConvert.ToInt32( RowHash[NameRowName] ), ColumnNo, CustomerNameLiteral );
                }
            }

            Literal AccessIDLiteral = new Literal();
            AccessIDLiteral.Text = AccessId;
            AutoTable.addControl( CswConvert.ToInt32( RowHash[AccessIdRowName] ), ColumnNo, AccessIDLiteral );

            Literal LastLogoutLiteral = new Literal();
            if( LastLogout > DateTime.MinValue )
                LastLogoutLiteral.Text = LastLogout.ToString();
            else
                LastLogoutLiteral.Text = "&nbsp;";
            AutoTable.addControl( CswConvert.ToInt32( RowHash[LastLogoutRowName] ), ColumnNo, LastLogoutLiteral );

            if( _DisplayMode == StatisticsDisplayMode.Schema )
            {
                Literal TotalNodesLiteral = new Literal();
                TotalNodesLiteral.Text = TotalNodes.ToString();
                AutoTable.addControl( CswConvert.ToInt32( RowHash[TotalNodesRowName] ), ColumnNo, TotalNodesLiteral );

                Literal TotalUsersLiteral = new Literal();
                TotalUsersLiteral.Text = UserCount.ToString();
                AutoTable.addControl( CswConvert.ToInt32( RowHash[TotalUsersRowName] ), ColumnNo, TotalUsersLiteral );

                Literal MostSessionsPerUserLiteral = new Literal();
                if( MostSessions > 0 && MostSessionsUserName != string.Empty )
                    MostSessionsPerUserLiteral.Text = MostSessions.ToString() + " (user: " + MostSessionsUserName + ")";
                else
                    MostSessionsPerUserLiteral.Text = "0";
                AutoTable.addControl( CswConvert.ToInt32( RowHash[MostSessionsPerUserRowName] ), ColumnNo, MostSessionsPerUserLiteral );

                Literal PeakUsersLiteral = new Literal();
                if( PeakUsers > 0 )
                    PeakUsersLiteral.Text = PeakUsers.ToString();
                else
                    PeakUsersLiteral.Text = "0";
                if( UserLimit > 0 )
                    PeakUsersLiteral.Text += " (limit: " + UserLimit.ToString() + ")";
                AutoTable.addControl( CswConvert.ToInt32( RowHash[PeakUsersRowName] ), ColumnNo, PeakUsersLiteral );

                Literal PeakUsersCountLiteral = new Literal();
                if( PeakUsersCount > 0 )
                    PeakUsersCountLiteral.Text = PeakUsersCount.ToString();
                else
                    PeakUsersCountLiteral.Text = "0";
                AutoTable.addControl( CswConvert.ToInt32( RowHash[PeakUsersCountRowName] ), ColumnNo, PeakUsersCountLiteral );
            }

            Literal TotalSessionsLiteral = new Literal();
            TotalSessionsLiteral.Text = TotalSessions.ToString();
            AutoTable.addControl( CswConvert.ToInt32( RowHash[TotalSessionsRowName] ), ColumnNo, TotalSessionsLiteral );

            Literal AverageServerTimeLiteral = new Literal();
            AverageServerTimeLiteral.Text = AverageServerTime.ToString() + "ms";
            AutoTable.addControl( CswConvert.ToInt32( RowHash[AverageServerTimeRowName] ), ColumnNo, AverageServerTimeLiteral );

            Literal AverageSessionLengthLiteral = new Literal();
            AverageSessionLengthLiteral.Text = AverageSessionTime.ToString();
            AutoTable.addControl( CswConvert.ToInt32( RowHash[AverageSessionLengthRowName] ), ColumnNo, AverageSessionLengthLiteral );

            Literal ErrorsLiteral = new Literal();
            ErrorsLiteral.Text = Errors.ToString();
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ErrorsRowName] ), ColumnNo, ErrorsLiteral );

            WebControl MultiEditCountLink = _makeStatisticsPopupLink( MultiEditCount, AccessId, LimitToUserId, "viewsmultiedited", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[MultiEditCountRowName] ), ColumnNo, MultiEditCountLink );

            WebControl ActionLoadsCountLink = _makeStatisticsPopupLink( ActionLoadsCount, AccessId, LimitToUserId, "actionloads", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ActionLoadsCountRowName] ), ColumnNo, ActionLoadsCountLink );

            WebControl ReportRunsCountLink = _makeStatisticsPopupLink( ReportRunsCount, AccessId, LimitToUserId, "reportsrun", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ReportRunsCountRowName] ), ColumnNo, ReportRunsCountLink );

            WebControl ViewsLoadCountLink = _makeStatisticsPopupLink( ViewsLoadCount, AccessId, LimitToUserId, "viewsloaded", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ViewsLoadCountRowName] ), ColumnNo, ViewsLoadCountLink );

            WebControl ViewsEditedCountLink = _makeStatisticsPopupLink( ViewsEditedCount, AccessId, LimitToUserId, "viewsedited", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ViewsEditedCountRowName] ), ColumnNo, ViewsEditedCountLink );

            WebControl SearchesLoadCountLink = _makeStatisticsPopupLink( SearchesLoadCount, AccessId, LimitToUserId, "searchesloaded", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[SearchesLoadCountRowName] ), ColumnNo, SearchesLoadCountLink );

            WebControl ViewFilterModsLink = _makeStatisticsPopupLink( ViewFilterMods, AccessId, LimitToUserId, "viewfiltermods", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[ViewFilterModsRowName] ), ColumnNo, ViewFilterModsLink );

            WebControl NodesSavedCountLink = _makeStatisticsPopupLink( NodesSavedCount, AccessId, LimitToUserId, "nodessaved", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[NodesSavedCountRowName] ), ColumnNo, NodesSavedCountLink );

            WebControl NodesAddedCountLink = _makeStatisticsPopupLink( NodesAddedCount, AccessId, LimitToUserId, "nodesadded", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[NodesAddedCountRowName] ), ColumnNo, NodesAddedCountLink );

            WebControl NodesCopiedCountLink = _makeStatisticsPopupLink( NodesCopiedCount, AccessId, LimitToUserId, "nodescopied", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[NodesCopiedCountRowName] ), ColumnNo, NodesCopiedCountLink );

            WebControl NodesDeletedCountLink = _makeStatisticsPopupLink( NodesDeletedCount, AccessId, LimitToUserId, "nodesdeleted", StartDate, EndDate, ShowPopupLinks );
            AutoTable.addControl( CswConvert.ToInt32( RowHash[NodesDeletedCountRowName] ), ColumnNo, NodesDeletedCountLink );

            if( ShowModeLink )
            {
                LinkButton SwitchModeLink = new LinkButton();
                SwitchModeLink.Text = "See Users";
                SwitchModeLink.PostBackUrl = "Statistics.aspx?mode=user&accessid=" + AccessId;
                AutoTable.addControl( CswConvert.ToInt32( RowHash[SwitchModeRowName] ), ColumnNo, SwitchModeLink );
            }
            else
            {
                AutoTable.addControl( CswConvert.ToInt32( RowHash[SwitchModeRowName] ), ColumnNo, new CswLiteralNbsp() );
            }
        }

        private WebControl _makeStatisticsPopupLink( Int32 Count, string AccessId, CswPrimaryKey LimitToUserId, string ActionString, DateTime StartDate, DateTime EndDate, bool ShowPopupLinks )
        {
            WebControl Link = null;
            if( ShowPopupLinks )  // yeah, this is a little kludgey
            {
                Link = new LinkButton();
                Link.ID = ActionString + "Link";
                ( (LinkButton) Link ).Text = Count.ToString();
                string UserIdString = string.Empty;
                if( LimitToUserId != null )
                    UserIdString = LimitToUserId.PrimaryKey.ToString();
                ( (LinkButton) Link ).OnClientClick = "OpenStatisticsPopup('" + AccessId + "','" + UserIdString + "','" + ActionString + "','" + StartDate.ToShortDateString() + "','" + EndDate.ToShortDateString() + "' ); return false;";
            }
            else
            {
                Link = new Label();
                ( (Label) Link ).Text = Count.ToString();
            }
            return Link;
        }

        private CswNbtNode _getCustomerNode( string AccessId )
        {
            CswNbtMetaDataObjectClass CustomerObjectClass = Master.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            CswNbtView CustomerView = new CswNbtView( Master.CswNbtResources );
            CswNbtViewRelationship CustomerRelationship = CustomerView.AddViewRelationship( CustomerObjectClass, true );
            CswNbtViewProperty AccessIdProperty = CustomerView.AddViewProperty( CustomerRelationship, CustomerObjectClass.getObjectClassProp( CswNbtObjClassCustomer.PropertyName.CompanyID ) );
            CswNbtViewPropertyFilter AccessIdFilter = CustomerView.AddViewPropertyFilter( AccessIdProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, AccessId, false );

            CswNbtNode CustomerNode = null;
            ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( Master.CswNbtResources.CurrentNbtUser, CustomerView, true, false, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                Tree.goToNthChild( 0 );
                CustomerNode = Tree.getNodeForCurrentPosition();
            }
            return CustomerNode;
        }

        /// <summary>
        /// BZ 8326
        /// This function uses a 'clustering' algorithm to determine how many sessions actually occurred simultaneously.
        /// The query returns a set of pairs of sessions that occurred at the same time.  If we have results like this:
        ///           1   2
        ///           1   3
        ///           2   1
        ///           2   3
        ///           3   1
        ///           3   2
        /// we know that sessions 1, 2, and 3 all occurred simultaneously, and so peak usage is 3.  The clustering algorithm
        /// builds one cluster: (1,2,3).  If we have results like this:
        ///           1   3
        ///           2   3
        ///           3   1
        ///           3   2
        /// then, though sessions 1 and 2 both occurred during session 3, they didn't occur at the same time,
        /// so the peak usage in this case is 2, not 3.  The clustering algorithm builds two clusters: (1,3) and (2,3)
        /// This algorithm relies on the SimultaneousUsageTable to be ordered.
        /// </summary>

        private void _getPeakSimultaneousUsers( DataTable SimultaneousUsageTable, out Int32 PeakUsers, out Int32 PeakUsersCount )
        {
            PeakUsers = 0;
            PeakUsersCount = 0;

            Collection<Cluster> Clusters = new Collection<Cluster>();
            foreach( DataRow Row in SimultaneousUsageTable.Rows )
            {
                Int32 ThisPk = CswConvert.ToInt32( Row["statisticsid"] );
                Int32 ThisSimultaneousId = CswConvert.ToInt32( Row["simultaneousid"] );

                // Is there already a cluster with these two as members?  If so, skip
                bool skip = false;
                foreach( Cluster otherCluster in Clusters )
                {
                    if( otherCluster.Contains( ThisPk ) &&
                        otherCluster.Contains( ThisSimultaneousId ) )
                    {
                        skip = true;
                    }
                }

                if( !skip )
                {
                    Cluster thisCluster = new Cluster();
                    thisCluster.Add( ThisPk );
                    thisCluster.Add( ThisSimultaneousId );
                    Clusters.Add( thisCluster );

                    // Greedily find other entries that might be in this cluster
                    foreach( DataRow InnerRow in SimultaneousUsageTable.Rows )
                    {
                        Int32 InnerPk = CswConvert.ToInt32( InnerRow["statisticsid"] );
                        Int32 InnerSimultaneousId = CswConvert.ToInt32( InnerRow["simultaneousid"] );
                        if( InnerPk == ThisPk )
                        {
                            if( !thisCluster.Contains( InnerSimultaneousId ) )
                            {
                                // InnerSimultaneousId must have everything in the cluster for it to be a member
                                bool AddIt = true;
                                foreach( Int32 MemberId in thisCluster )
                                {
                                    if( !_matchExists( SimultaneousUsageTable, InnerSimultaneousId, MemberId ) )
                                        AddIt = false;
                                }
                                if( AddIt )
                                    thisCluster.Add( InnerSimultaneousId );
                            }
                        }
                    }
                }
            }

            // Now determine peak usage:
            foreach( Cluster thisCluster in Clusters )
            {
                if( thisCluster.Count > PeakUsers )
                {
                    PeakUsers = thisCluster.Count;
                    PeakUsersCount = 1;
                }
                else if( thisCluster.Count == PeakUsers )
                    PeakUsersCount++;
            }
        }

        private bool _matchExists( DataTable SimultaneousUsageTable, Int32 SessionId, Int32 SimultaneousId )
        {
            bool ret = false;
            foreach( DataRow Row in SimultaneousUsageTable.Rows )
            {
                Int32 ThisPk = CswConvert.ToInt32( Row["statisticsid"] );
                Int32 ThisSimultaneousId = CswConvert.ToInt32( Row["simultaneousid"] );
                if( ThisPk == SessionId && ThisSimultaneousId == SimultaneousId )
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private class Cluster : Collection<Int32> { }

    }
}
