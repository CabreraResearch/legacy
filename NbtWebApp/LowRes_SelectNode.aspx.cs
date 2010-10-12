using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.DB;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowRes_SelectNode : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
                InitViewTree(null);
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnInit( e );
        }

        private Label _PageLabel;
        private Button _BackButton;
        private PlaceHolder _LinksPH;

        protected override void CreateChildControls()
        {
            Literal ViewLiteral = new Literal();
            ViewLiteral.Text = "View: ";
            ph.Controls.Add( ViewLiteral );

            Literal ViewNameLiteral = new Literal();
            ViewNameLiteral.Text = Master.SelectedView.ViewName;
            ph.Controls.Add( ViewNameLiteral );

            ph.Controls.Add( new CswLiteralBr() );
            ph.Controls.Add( new CswLiteralBr() );

            _PageLabel = new Label();
            _PageLabel.Text = "Select a Node:";
            _PageLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            ph.Controls.Add( _PageLabel );

            ph.Controls.Add( new CswLiteralBr() );

            _LinksPH = new PlaceHolder();
            _LinksPH.ID = "LinksPH";
            ph.Controls.Add( _LinksPH );

            ph.Controls.Add( new CswLiteralBr() );

            _BackButton = new Button();
            _BackButton.ID = "BackButton";
            _BackButton.CssClass = "Button";
            _BackButton.Text = "Back";
            _BackButton.Click += new EventHandler( _BackButton_Click );
            ph.Controls.Add( _BackButton );

            base.CreateChildControls();
        }

        void _BackButton_Click( object sender, EventArgs e )
        {
            Master.Redirect( Master.MakeLink( "LowRes_SelectView.aspx" ) );
        }

        void MoreLink_Click( object sender, EventArgs e )
        {
            LinkButton MoreLink = ( (LinkButton) sender );
            CswNbtNodeKey MoreKey = new CswNbtNodeKey( Master.CswNbtResources, MoreLink.Attributes["nodekey"].ToString() );
            InitViewTree( MoreKey );
            //MoreLink.Parent.Controls.Remove( MoreLink );
        }


        private void InitViewTree( CswNbtNodeKey StartingKey )
        {
            _LinksPH.Controls.Clear();  // BEWARE!

            if( Master.SelectedViewId != Int32.MinValue )
            {
                CswNbtNodeKey ParentKey = null;
                CswNbtViewRelationship FirstChildRelationship  = null;
                if( StartingKey == null )
                    StartingKey = Master.SelectedNodeKey;
                if( StartingKey != null )
                    FirstChildRelationship = (CswNbtViewRelationship) Master.SelectedView.FindViewNodeByUniqueId( StartingKey.ViewNodeUniqueId );

                ICswNbtTree NodeTree = Master.CswNbtResources.Trees.getTreeFromView( Master.SelectedView, true, ref ParentKey, FirstChildRelationship, 20, true, false, StartingKey, false );

                DataTable NodeTable = new CswDataTable( "selectnodetable", "" );
                NodeTable.Columns.Add( "nodename" );
                NodeTable.Columns.Add( "nodekey" );
                NodeTable.Columns.Add( "depth" );
                NodeTable.Columns.Add( "iconfilename" );
                NodeTable.Columns.Add( "nodespecies" );
                NodeTable.Columns.Add( "nodeid" );
                NodeTree.goToRoot();
                _makeDataTableFromTreeRecursive( ref NodeTable, NodeTree, 0 );

                foreach( DataRow NodeRow in NodeTable.Rows )
                {
                    Literal Indent = new Literal();
                    Indent.Text = string.Empty;
                    for( int i = 0; i < CswConvert.ToInt32( NodeRow["depth"] ); i++ )
                        Indent.Text += "&nbsp;&nbsp;&nbsp;";
                    _LinksPH.Controls.Add( Indent );

                    Image Icon = new Image();
                    Icon.ImageUrl = "Images/icons/" + NodeRow["iconfilename"].ToString();
                    _LinksPH.Controls.Add( Icon );

                    _LinksPH.Controls.Add( new CswLiteralNbsp() );

                    if( (NodeSpecies) Enum.Parse( typeof( NodeSpecies ), NodeRow["nodespecies"].ToString() ) == NodeSpecies.More )
                    {
                        //Button MoreLink = new Button();
                        LinkButton MoreLink = new LinkButton();
                        MoreLink.Attributes["nodekey"] = NodeRow["nodekey"].ToString();
                        MoreLink.ID = "more_" + NodeRow["nodeid"].ToString();
                        MoreLink.Text = NodeRow["nodename"].ToString();
                        MoreLink.Click += new EventHandler( MoreLink_Click );
                        _LinksPH.Controls.Add( MoreLink );
                    }
                    else
                    {
                        HyperLink NodeLink = new HyperLink();
                        NodeLink.NavigateUrl = Master.MakeLink( "LowRes_SelectTab.aspx", Int32.MinValue, NodeRow["nodekey"].ToString() );
                        NodeLink.Text = NodeRow["nodename"].ToString();
                        if( Master.SelectedNodeKey != null && NodeRow["nodekey"].ToString() == Master.SelectedNodeKey.ToString() )
                            NodeLink.Style.Add( HtmlTextWriterStyle.BackgroundColor, "#ffff00" );
                        _LinksPH.Controls.Add( NodeLink );
                    }
                    _LinksPH.Controls.Add( new CswLiteralBr() );
                }
            }
            else
            {
                Master.Redirect( Master.MakeLink( "LowRes_SelectView.aspx" ) );
            }
        }

        private void _makeDataTableFromTreeRecursive( ref DataTable NodeTable, ICswNbtTree Tree, Int32 Depth )
        {
            for( int c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                // Don't do this.
                //CswNbtNode CurrentNode = Tree.getNodeForCurrentPosition();
                
                DataRow ThisRow = NodeTable.NewRow();
                CswNbtNodeKey CurrentKey = Tree.getNodeKeyForCurrentPosition();
                ThisRow["nodename"] = Tree.getNodeNameForCurrentPosition();
                ThisRow["nodekey"] = CurrentKey.ToString();
                ThisRow["depth"] = Depth;
                ThisRow["nodespecies"] = CurrentKey.NodeSpecies.ToString();
                ThisRow["nodeid"] = CurrentKey.NodeId.ToString();
                ThisRow["iconfilename"] = Master.CswNbtResources.MetaData.getNodeType( CurrentKey.NodeTypeId ).IconFileName;
                NodeTable.Rows.Add( ThisRow );

                if( Tree.getChildNodeCount() > 0 )
                    _makeDataTableFromTreeRecursive( ref NodeTable, Tree, Depth+1 );

                Tree.goToParentNode();
            }
        }
    }
}