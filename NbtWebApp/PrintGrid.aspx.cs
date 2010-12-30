using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebPages
{
    public partial class PrintGrid : System.Web.UI.Page
    {
        public CswNbtView View
        {
            get { return Master.CswNbtView; }
        }

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Label TitleLabel = new Label();
            TitleLabel.ID = "TitleLabel";
            TitleLabel.CssClass = "Title";
            ph.Controls.Add(TitleLabel);

            CswNodesGrid _NodesGrid = new CswNodesGrid(Master.CswNbtResources); //, CswNodesGrid.GridSelectMode.NoAction );
            //_NodesGrid.Printable = true;
            _NodesGrid.ID = "grid";
            //_NodesGrid.ItemLooks = Master.ItemLooks;
            //_NodesGrid.ShowMenu = true;
            _NodesGrid.OnError += new CswErrorHandler(_NodesGrid_OnError);
            ph.Controls.Add(_NodesGrid);

            if (Request.QueryString["nodeid"] != null &&
                Request.QueryString["propid"] != null &&
                //CswTools.IsInteger(Request.QueryString["nodeid"].ToString()) &&
                CswTools.IsInteger(Request.QueryString["propid"].ToString()))
            {
                CswNbtMetaDataNodeTypeProp MetaDataProp = Master.CswNbtResources.MetaData.getNodeTypeProp(CswConvert.ToInt32(Request.QueryString["propid"].ToString()));
                CswPrimaryKey NodeId = new CswPrimaryKey();
                NodeId.FromString( Request.QueryString["nodeid"] );
                CswNbtNode Node = Master.CswNbtResources.Nodes[NodeId];
                //CswNbtNodePropWrapper Prop = Node.Properties[CswConvert.ToInt32(Request.QueryString["propid"].ToString())];
                CswNbtNodePropWrapper Prop = Node.Properties[MetaDataProp];
                CswNbtView GridView = Prop.AsGrid.View; //new CswNbtView(Master.CswNbtResources);
                //GridView.LoadXml(Prop.AsGrid.ViewXml);
                //_NodesGrid.RootNodeId = Node.NodeId;
                _NodesGrid.View = GridView;
                //_NodesGrid.PropNodeId = Node.NodeId;
                //_NodesGrid.PropId = Prop.NodeTypePropId;
            }
            else if (Request.QueryString["viewid"] != null)
            {
                //CswNbtView AView = new CswNbtView(Master.CswNbtResources);
                //AView.LoadXml(CswConvert.ToInt32(Request.QueryString["viewid"].ToString()));
                CswNbtView AView = (CswNbtView)CswNbtViewFactory.restoreView(Master.CswNbtResources, CswConvert.ToInt32(Request.QueryString["viewid"].ToString()));
                _NodesGrid.View = AView;
            }
            else if (Request.QueryString["sessionviewid"] != null)
            {
                //CswNbtView AView = new CswNbtView(Master.CswNbtResources);
                //AView.LoadXml(CswConvert.ToInt32(Request.QueryString["viewid"].ToString()));
                CswNbtView AView = (CswNbtView)Master.CswNbtResources.ViewCache.getView(CswConvert.ToInt32(Request.QueryString["sessionviewid"].ToString()));
                _NodesGrid.View = AView;
            }
            else
            {
                _NodesGrid.View = View;
            }
            _NodesGrid.Grid.AllowPaging = false;
            _NodesGrid.Grid.Skin = "ChemSWPrint";

            _NodesGrid.DataBind();

            TitleLabel.Text = _NodesGrid.ViewName;
        }

        void _NodesGrid_OnError( Exception ex )
        {
            Master.HandleError( ex );
        }
    }
}