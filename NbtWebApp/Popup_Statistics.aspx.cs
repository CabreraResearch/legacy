using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_Statistics : System.Web.UI.Page
    {
        private bool _NbtMgrEnabled = false;
        private CswPrimaryKey UserId;
        private string ShowMode;
        private string AccessId;
        private DateTime StartDate;
        private DateTime EndDate;

        protected override void OnInit( EventArgs e )
        {
            _NbtMgrEnabled = Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager );

            AccessId = Request.QueryString["accessid"];
            if( !_NbtMgrEnabled )
                AccessId = Master.AccessID;

            UserId = null;
            if( Request.QueryString["userid"] != null && Request.QueryString["userid"] != string.Empty )
                UserId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Request.QueryString["userid"] ) );

            ShowMode = Request.QueryString["show"];

            StartDate = Convert.ToDateTime( Request.QueryString["startdate"] );
            EndDate = Convert.ToDateTime( Request.QueryString["enddate"] );

            base.OnInit( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            // Set the connection to the target AccessId
            Master.CswNbtResources.AccessId = AccessId;

            CswNbtNode UserNode = null;
            if( UserId != null )
                UserNode = Master.CswNbtResources.Nodes[UserId];

            CswAutoTable UserDataTable = new CswAutoTable();
            UserDataTable.ID = "UserDataTable";
            UserDataTable.CssClass = "StatisticsTable";
            UserDataTable.EvenRowCssClass = "EvenRow";
            UserDataTable.OddRowCssClass = "OddRow";

            Int32 Row = 0;
            Int32 Col = 0;

            string Title = string.Empty;
            string Action = string.Empty;
            CswStaticSelect CswStaticSelect = null;
            switch( ShowMode )
            {
                case "actionloads":
                    Title = "Actions Loaded";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup1", "getStatisticsActions" );
                    Action = "load";
                    break;
                case "nodessaved":
                    Title = "Node Types of Nodes Saved";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup2", "getStatisticsNodeTypes" );
                    Action = "save";
                    break;
                case "nodesadded":
                    Title = "Node Types of Nodes Added";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup3", "getStatisticsNodeTypes" );
                    Action = "add";
                    break;
                case "nodescopied":
                    Title = "Node Types of Nodes Copied";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup4", "getStatisticsNodeTypes" );
                    Action = "copy";
                    break;
                case "nodesdeleted":
                    Title = "Node Types of Nodes Deleted";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup5", "getStatisticsNodeTypes" );
                    Action = "delete";
                    break;
                case "reportsrun":
                    Title = "Reports Run";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup6", "getStatisticsReports" );
                    Action = "load";
                    break;
                case "viewsloaded":
                    Title = "Views Loaded";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup7", "getStatisticsViews" );
                    Action = "load";
                    break;
                case "viewsedited":
                    Title = "Views Edited";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup8", "getStatisticsViews" );
                    Action = "edit";
                    break;
                case "viewsmultiedited":
                    Title = "Views Multi-Edited";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup9", "getStatisticsViews" );
                    Action = "multiedit";
                    break;
                case "searchesloaded":
                    Title = "Searches Loaded";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup10", "getStatisticsSearches" );
                    Action = "load";
                    break;
                case "viewfiltermods":
                    Title = "View Filters Modified";
                    CswStaticSelect = Master.CswNbtResources.makeCswStaticSelect( "statisticspopup11", "getStatisticsSearches" );
                    Action = "modify";
                    break;
                default:
                    throw new CswDniException( ErrorType.Error, "Invalid Mode: " + ShowMode, "Popup_Statistics encountered an unhandled ShowMode: " + ShowMode );
            }
            if( UserId != null )
                Title += " for User " + UserNode.NodeName;
            else
                Title += " for Access ID " + AccessId;

            Title += " from " + StartDate.ToShortDateString() + " to ";
            if( EndDate > DateTime.Now )
                Title += DateTime.Now.ToString();
            else
                Title += EndDate.ToShortDateString();
            Title += ":";

            Literal TitleLiteral = new Literal();
            TitleLiteral.Text = Title;
            ph.Controls.Add( TitleLiteral );
            Row++;
            Col = 0;

            CswStaticSelect.S4Parameters.Add( "getaction", new CswStaticParam( "getaction", Action ) );
            if( UserId != null )
            { CswStaticSelect.S4Parameters.Add( "getuserid", new CswStaticParam( "getuserid", UserId.PrimaryKey.ToString() ) ); }
            else
            { CswStaticSelect.S4Parameters.Add( "getuserid", new CswStaticParam( "getuserid", "%" ) ); }
            CswStaticSelect.S4Parameters.Add( "getbeforedate", new CswStaticParam( "getbeforedate", Master.CswNbtResources.getDbNativeDate( EndDate ), true ) );
            CswStaticSelect.S4Parameters.Add( "getafterdate", new CswStaticParam( "getafterdate", Master.CswNbtResources.getDbNativeDate( StartDate ), true ) );
            DataTable QueryData = CswStaticSelect.getTable();

            Hashtable DataHash = new Hashtable();
            foreach( DataRow DataRow in QueryData.Rows )
            {
                string Label = string.Empty;
                if( DataRow["label"] != null && DataRow["label"].ToString() != string.Empty )
                    Label = DataRow["label"].ToString();
                else if( DataRow["cachedlabel"] != null && DataRow["cachedlabel"].ToString() != string.Empty )
                    Label = DataRow["cachedlabel"].ToString();
                if( Label != string.Empty )
                {
                    if( DataHash[Label] != null )
                        DataHash[Label] = CswConvert.ToInt32( DataHash[Label] ) + CswConvert.ToInt32( DataRow["hitcount"].ToString() );
                    else
                        DataHash.Add( Label, CswConvert.ToInt32( DataRow["hitcount"].ToString() ) );
                }
            }

            foreach( string Label in DataHash.Keys )
            {
                Literal LabelLiteral = new Literal();
                LabelLiteral.Text = Label;
                UserDataTable.addControl( Row, Col, LabelLiteral );
                Col++;

                Literal ResultLiteral = new Literal();
                ResultLiteral.Text = DataHash[Label].ToString();
                UserDataTable.addControl( Row, Col, ResultLiteral );
                Col++;

                Row++;
                Col = 0;
            }

            ph.Controls.Add( UserDataTable );

            // Set the connection back to the original
            Master.CswNbtResources.AccessId = Master.AccessID;

            base.OnLoad( e );
        }
    }
}