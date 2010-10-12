using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowResLayout : System.Web.UI.MasterPage
    {
        public Button LogoutButton { get { return _LogoutButton; } }


        public CswNbtResources CswNbtResources
        {
            get { return Master.CswNbtResources; }
        }

        protected void Page_Load( object sender, EventArgs e )
        {
            _LogoutButton.Click += new EventHandler( LogoutButton_Click );
        }

        protected void LogoutButton_Click( object sender, EventArgs e )
        {
            Master.LogoutPath = "LowRes_Login.aspx";
            Master.Logout();
        }

        public void Redirect( string url )
        {
            Master.Redirect( url );
        }

        public void HandleError( Exception ex )
        {
            Master.HandleError( ex );
        }

        public string AccessId
        {
            get { return Master.AccessId; }
            set { Master.AccessId = value; }
        }
        public string LogoutPath
        {
            get { return Master.LogoutPath; }
            set { Master.LogoutPath = value; }
        }
        public ChemSW.Security.AuthenticationStatus Authenticate( string username, string password )
        {
            return Master.Authenticate( username, password );
        }



        #region Selected

        public Int32 SelectedViewId
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( CswTools.IsInteger( Request.QueryString["view"] ) )
                    ret = Convert.ToInt32( Request.QueryString["view"] );
                return ret;
            }
        }

        private CswNbtView _SelectedView = null;
        public CswNbtView SelectedView
        {
            get
            {
                if( _SelectedView == null && SelectedViewId != Int32.MinValue )
                    _SelectedView = CswNbtViewFactory.restoreView( Master.CswNbtResources, SelectedViewId );
                return _SelectedView;
            }
        }

        private CswNbtNodeKey _SelectedNodeKey = null;
        public CswNbtNodeKey SelectedNodeKey
        {
            get
            {
                if( _SelectedNodeKey == null )
                {
                    if( Request.QueryString["nk"] != null && Request.QueryString["nk"] != string.Empty )
                        _SelectedNodeKey = new CswNbtNodeKey( Master.CswNbtResources, Request.QueryString["nk"] );
                }
                return _SelectedNodeKey;
            }
        }
        private CswNbtNode _SelectedNode = null;
        public CswNbtNode SelectedNode
        {
            get
            {
                if( _SelectedNode == null && SelectedNodeKey != null )
                    _SelectedNode = Master.CswNbtResources.Nodes[SelectedNodeKey];
                return _SelectedNode;
            }
        }

        private CswNbtMetaDataNodeType _SelectedNodeType = null;
        public CswNbtMetaDataNodeType SelectedNodeType
        {
            get
            {
                if( _SelectedNodeType == null && SelectedNode != null )
                    _SelectedNodeType = Master.CswNbtResources.MetaData.getNodeType( SelectedNode.NodeTypeId );
                return _SelectedNodeType;
            }
        }

        public Int32 SelectedTabId
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( CswTools.IsInteger( Request.QueryString["tab"] ) )
                    ret = Convert.ToInt32( Request.QueryString["tab"] );
                return ret;
            }
        }

        private CswNbtMetaDataNodeTypeTab _SelectedTab = null;
        public CswNbtMetaDataNodeTypeTab SelectedTab
        {
            get
            {
                if( _SelectedTab == null && SelectedNodeType != null && SelectedTabId != Int32.MinValue )
                    _SelectedTab = SelectedNodeType.getNodeTypeTab( SelectedTabId );
                return _SelectedTab;
            }
        }

        public Int32 SelectedPropId
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( CswTools.IsInteger( Request.QueryString["prop"] ) )
                    ret = Convert.ToInt32( Request.QueryString["prop"] );
                return ret;
            }
        }

        private CswNbtMetaDataNodeTypeProp _SelectedProp =null;
        public CswNbtMetaDataNodeTypeProp SelectedProp
        {
            get
            {
                if( _SelectedProp == null && SelectedNodeType != null && SelectedPropId != Int32.MinValue )
                    _SelectedProp = SelectedNodeType.getNodeTypeProp( SelectedPropId );
                return _SelectedProp;
            }
        }



        public string MakeLink( string PageName )
        {
            return MakeLink( PageName, Int32.MinValue, string.Empty, Int32.MinValue, Int32.MinValue );
        }

        public string MakeLink( string PageName, Int32 OverrideViewId )
        {
            return MakeLink( PageName, OverrideViewId, string.Empty, Int32.MinValue, Int32.MinValue );
        }

        public string MakeLink( string PageName, Int32 OverrideViewId, string OverrideNodeKeyString )
        {
            return MakeLink( PageName, OverrideViewId, OverrideNodeKeyString, Int32.MinValue, Int32.MinValue );
        }

        public string MakeLink( string PageName, Int32 OverrideViewId, string OverrideNodeKeyString, Int32 OverrideTabId )
        {
            return MakeLink( PageName, OverrideViewId, OverrideNodeKeyString, OverrideTabId, Int32.MinValue );
        }

        public string MakeLink( string PageName, Int32 OverrideViewId, string OverrideNodeKeyString, Int32 OverrideTabId, Int32 OverridePropId )
        {
            string Params = PageName;
            if( !Params.Contains( "?" ) )
                Params += "?";

            if( OverrideViewId != Int32.MinValue )
                Params += "&view=" + OverrideViewId.ToString();
            else if( SelectedViewId != Int32.MinValue )
                Params += "&view=" + SelectedViewId.ToString();

            if( OverrideNodeKeyString != string.Empty )
                Params += "&nk=" + OverrideNodeKeyString;
            else if( SelectedNodeKey != null )
                Params += "&nk=" + SelectedNodeKey.ToString();

            if( OverrideTabId != Int32.MinValue )
                Params += "&tab=" + OverrideTabId.ToString();
            else if( SelectedTabId != Int32.MinValue )
                Params += "&tab=" + SelectedTabId.ToString();

            if( OverridePropId != Int32.MinValue )
                Params += "&prop=" + OverridePropId.ToString();
            else if( SelectedPropId != Int32.MinValue )
                Params += "&prop=" + SelectedPropId.ToString();

            return Params;
        }

        #endregion Selected

    }
}