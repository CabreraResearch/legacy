using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_DesignDelete : System.Web.UI.Page
    {
        #region Selected

        private CswNodeTypeTree.NodeTypeTreeSelectedType _SelectedType
        {
            get
            {
                if( Request.QueryString["type"] != null )
                    return (CswNodeTypeTree.NodeTypeTreeSelectedType) Enum.Parse( typeof( CswNodeTypeTree.NodeTypeTreeSelectedType ), Request.QueryString["type"].ToString() );
                else
                    return CswNodeTypeTree.NodeTypeTreeSelectedType.Root;
            }
        }
        private string _SelectedValue
        {
            get
            {
                if( Request.QueryString["value"] != null )
                    return Request.QueryString["value"].ToString();
                else
                    return string.Empty;
            }
        }


        private CswNbtMetaDataNodeType _SelectedNodeType = null;
        private CswNbtMetaDataNodeTypeTab _SelectedNodeTypeTab = null;
        private CswNbtMetaDataNodeTypeProp _SelectedNodeTypeProp = null;
        private void InitSelectedMetaDataObjects()
        {
            EnsureChildControls();
            switch( _SelectedType )
            {
                case CswNodeTypeTree.NodeTypeTreeSelectedType.Category:
                    _SelectedNodeType = null;
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType:
                    if( Convert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeType = Master.CswNbtResources.MetaData.getNodeType( Convert.ToInt32( _SelectedValue ) );
                    }
                    else
                    {
                        _SelectedNodeType = null;
                    }
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Property:
                    if( Convert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeTypeProp = Master.CswNbtResources.MetaData.getNodeTypeProp( Convert.ToInt32( _SelectedValue ) );
                        _SelectedNodeType = _SelectedNodeTypeProp.getNodeType();
                        _SelectedNodeTypeTab = Master.CswNbtResources.MetaData.getNodeTypeTab( _SelectedNodeTypeProp.FirstEditLayout.TabId );
                    }
                    else
                    {
                        _SelectedNodeType = null;
                        _SelectedNodeTypeTab = null;
                        _SelectedNodeTypeProp = null;
                    }
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Root:
                    _SelectedNodeType = null;
                    _SelectedNodeTypeProp = null;
                    _SelectedNodeTypeTab = null;
                    break;

                case CswNodeTypeTree.NodeTypeTreeSelectedType.Tab:
                    if( Convert.ToInt32( _SelectedValue ) > 0 )
                    {
                        _SelectedNodeTypeTab = Master.CswNbtResources.MetaData.getNodeTypeTab( Convert.ToInt32( _SelectedValue ) );
                        _SelectedNodeType = _SelectedNodeTypeTab.getNodeType();
                    }
                    else
                    {
                        _SelectedNodeType = null;
                        _SelectedNodeTypeTab = null;
                    }
                    _SelectedNodeTypeProp = null;
                    break;
            }
        }

        private CswNbtMetaDataNodeType SelectedNodeType
        {
            get
            {
                if( _SelectedNodeType == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeType;
            }
        }
        private CswNbtMetaDataNodeTypeTab SelectedNodeTypeTab
        {
            get
            {
                if( _SelectedNodeTypeTab == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeTab;
            }
        }
        private CswNbtMetaDataNodeTypeProp SelectedNodeTypeProp
        {
            get
            {
                if( _SelectedNodeTypeProp == null )
                    InitSelectedMetaDataObjects();
                return _SelectedNodeTypeProp;
            }
        }

        #endregion Selected


        private Button OKButton;
        private Button CancelButton;
        private CswAutoTable DeleteTable;

        protected void Page_Load( object sender, EventArgs e )
        {
            try
            {
                DeleteTable = new CswAutoTable();
                DeleteTable.ID = "DeleteTable";
                DeleteTable.FirstCellRightAlign = true;
                ph.Controls.Add( DeleteTable );

                OKButton = new Button();
                OKButton.Text = "Delete";
                OKButton.CssClass = "Button";
                OKButton.OnClientClick = "Popup_OK_Clicked(); return false;";

                CancelButton = new Button();
                CancelButton.Text = "Cancel";
                CancelButton.CssClass = "Button";
                CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";

                Int32 LastRow = 0;
                if( Request.QueryString["type"] != null && Request.QueryString["value"] != null )
                {
                    if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property )
                    {
                        create_DeletePropertyPage();
                        DeleteTable.addControl( 0, 0, Label5 );
                        DeleteTable.addControl( 0, 1, DeletePropName );
                        DeleteTable.addControl( 1, 0, new CswLiteralNbsp() );
                        DeleteTable.addControl( 2, 0, Label3 );
                        DeleteTable.addControl( 2, 1, ViewsOfDeletedNodeTypeProp );
                        DeleteTable.addControl( 2, 1, EditSelectedViewsButtonProp );
                        LastRow = 2;
                        init_DeletePropertyPage();
                    }
                    else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab )
                    {
                        create_DeleteTabPage();
                        DeleteTable.addControl( 0, 0, DeleteTabNameLabel );
                        DeleteTable.addControl( 0, 1, DeleteTabName );
                        DeleteTable.addControl( 1, 0, new CswLiteralNbsp() );
                        DeleteTable.addControl( 2, 1, DeleteTabNoteLabel );
                        LastRow = 2;
                        init_DeleteTabPage();
                    }
                    else if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType )
                    {
                        create_DeleteNodeTypePage();
                        DeleteTable.addControl( 0, 0, Label4 );
                        DeleteTable.addControl( 0, 1, DeleteNodeTypeName );
                        DeleteTable.addControl( 1, 0, new CswLiteralNbsp() );
                        DeleteTable.addControl( 2, 0, DeleteNodeTypeGenericLabel );
                        DeleteTable.addControl( 2, 1, TreeOfDeletedNodeType );
                        DeleteTable.addControl( 3, 1, ViewsOfDeletedTypeLabel );
                        DeleteTable.addControl( 4, 1, ViewsOfDeletedNodeType );
                        DeleteTable.addControl( 4, 1, EditSelectedViewsButton );
                        LastRow = 4;
                        init_DeleteNodeTypePage();
                    }
                }
                else
                {
                    OKButton.Visible = false;
                }
                DeleteTable.addControl( LastRow + 1, 0, new CswLiteralNbsp() );

                DeleteTable.addControl( LastRow + 2, 1, OKButton );
                DeleteTable.addControl( LastRow + 2, 1, CancelButton );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // Page_Load()


        private Label Label4;
        private Label DeleteNodeTypeName;
        private Label DeleteNodeTypeGenericLabel;
        private Label ViewsOfDeletedTypeLabel;
        private ListBox ViewsOfDeletedNodeType;
        private Button EditSelectedViewsButton;
        private RadTreeView TreeOfDeletedNodeType;

        private void create_DeleteNodeTypePage()
        {
            Label4 = new Label();
            Label4.ID = "Label4";
            Label4.Text = "Node Type To Delete:";

            DeleteNodeTypeName = new Label();
            DeleteNodeTypeName.ID = "DeleteNodeTypeName";

            DeleteNodeTypeGenericLabel = new Label();
            DeleteNodeTypeGenericLabel.ID = "DeleteNodeTypeGenericLabel";
            DeleteNodeTypeGenericLabel.Text = "Additional items to be deleted:";

            TreeOfDeletedNodeType = new RadTreeView();
            TreeOfDeletedNodeType.ID = "TreeOfDeletedNodeType";
            TreeOfDeletedNodeType.Width = 200;

            ViewsOfDeletedTypeLabel = new Label();
            ViewsOfDeletedTypeLabel.ID = "ViewsOfDeletedTypeLabel";
            ViewsOfDeletedTypeLabel.Text = "Views:";

            ViewsOfDeletedNodeType = new ListBox();
            ViewsOfDeletedNodeType.ID = "ViewsOfDeletedNodeType";
            ViewsOfDeletedNodeType.CssClass = "selectinput";

            EditSelectedViewsButton = new Button();
            EditSelectedViewsButton.ID = "EditSelectedViewsButton";
            EditSelectedViewsButton.Text = "Edit";
            EditSelectedViewsButton.CssClass = "Button";
        }

        private Label Label5;
        private Label DeletePropName;
        private Label Label3;
        private ListBox ViewsOfDeletedNodeTypeProp;
        private Button EditSelectedViewsButtonProp;

        private void create_DeletePropertyPage()
        {
            Label5 = new Label();
            Label5.ID = "Label5";
            Label5.Text = "Property To Delete:";

            DeletePropName = new Label();
            DeletePropName.ID = "DeleteNodeTypeName";


            Label3 = new Label();
            Label3.ID = "Label3";
            Label3.Text = "Views to be deleted:";

            ViewsOfDeletedNodeTypeProp = new ListBox();
            ViewsOfDeletedNodeTypeProp.ID = "ViewsOfDeletedNodeTypeProp";
            ViewsOfDeletedNodeTypeProp.CssClass = "selectinput";

            EditSelectedViewsButtonProp = new Button();
            EditSelectedViewsButtonProp.ID = "EditSelectedViewsButtonProp";
            EditSelectedViewsButtonProp.Text = "Edit";
            EditSelectedViewsButtonProp.CssClass = "Button";
        }

        private Label DeleteTabNameLabel;
        private Label DeleteTabName;
        private Label DeleteTabNoteLabel;

        private void create_DeleteTabPage()
        {
            DeleteTabNameLabel = new Label();
            DeleteTabNameLabel.ID = "DeleteTabNameLabel";
            DeleteTabNameLabel.Text = "Tab to Delete:";

            DeleteTabName = new Label();
            DeleteTabName.ID = "DeleteTabName";

            DeleteTabNoteLabel = new Label();
            DeleteTabNoteLabel.ID = "DeleteTabNoteLabel";
            DeleteTabNoteLabel.Text = "All Properties will be reassigned to the first tab";
        }

        private void init_DeleteTabPage()
        {
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Tab && _SelectedValue != string.Empty )
            {
                DeleteTabName.Text = SelectedNodeTypeTab.TabName;
            }
        }


        private void init_DeleteNodeTypePage()
        {
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType && _SelectedValue != string.Empty )
            {
                // NodeTypeName Delete Label
                DeleteNodeTypeName.Text = SelectedNodeType.NodeTypeName;

                // Delete Nodetype
                CswNbtMetaDataNodeType SelectedNT = Master.CswNbtResources.MetaData.getNodeType( Convert.ToInt32( _SelectedValue ) );
                CswNbtView DeleteNodeTypeView = SelectedNT.CreateDefaultView();
                DeleteNodeTypeView.ViewName = "Nodes to Delete";
                ICswNbtTree CswNbtTree = Master.CswNbtResources.Trees.getTreeFromView( Master.CswNbtResources.CurrentNbtUser, DeleteNodeTypeView, true, true, false );
                // BROKEN BY case 24709
                //string XmlStr = CswNbtTree.getTreeAsXml();
                //TreeOfDeletedNodeType.LoadXml(XmlStr);

                //if (TreeOfDeletedNodeType.Nodes[0].Nodes.Count > 0)
                //{
                //    TreeOfDeletedNodeType.Visible = true;
                //    TreeOfDeletedNodeType.ExpandAllNodes();
                //    DeleteNodeTypeGenericLabel.Visible = true;
                //}
                //else
                //{
                TreeOfDeletedNodeType.Visible = false;
                //}

                DataTable ViewsOfNodeType = getViewsUsingNodeType( Convert.ToInt32( _SelectedValue ) );

                ViewsOfDeletedNodeType.Items.Clear();
                ViewsOfDeletedNodeType.Visible = false;
                ViewsOfDeletedTypeLabel.Visible = false;
                EditSelectedViewsButton.Visible = false;
                //if (TreeOfDeletedNodeType.Nodes[0].Nodes.Count > 0)
                //{
                //    DeleteNodeTypeGenericLabel.Visible = false;
                //}

                if( ViewsOfNodeType.Rows.Count > 0 )
                {
                    ViewsOfDeletedNodeType.DataTextField = "viewname";
                    ViewsOfDeletedNodeType.DataValueField = "nodeviewid";
                    ViewsOfDeletedNodeType.DataSource = ViewsOfNodeType;
                    ViewsOfDeletedNodeType.DataBind();

                    ViewsOfDeletedNodeType.Visible = true;
                    ViewsOfDeletedTypeLabel.Visible = true;
                    EditSelectedViewsButton.Visible = true;
                    ViewsOfDeletedNodeType.SelectedIndex = 0;
                    DeleteNodeTypeGenericLabel.Visible = true;
                }
            } // if (SelectedNodeTypeId > 0)
        } // init_DeleteNodeTypePage()


        private void init_DeletePropertyPage()
        {
            if( _SelectedType == CswNodeTypeTree.NodeTypeTreeSelectedType.Property && _SelectedValue != string.Empty )
            {
                DeletePropName.Text = SelectedNodeTypeProp.PropName;

                // Delete Prop
                ViewsOfDeletedNodeTypeProp.Visible = false;
                EditSelectedViewsButtonProp.Visible = false;
                Label3.Visible = false;

                DataTable ViewsOfNodeTypeProp = getViewsUsingNodeTypeProp( Convert.ToInt32( _SelectedValue ) );
                if( ViewsOfNodeTypeProp.Rows.Count > 0 )
                {
                    ViewsOfDeletedNodeTypeProp.DataTextField = "viewname";
                    ViewsOfDeletedNodeTypeProp.DataValueField = "nodeviewid";
                    ViewsOfDeletedNodeTypeProp.DataSource = ViewsOfNodeTypeProp;
                    ViewsOfDeletedNodeTypeProp.DataBind();

                    Label3.Visible = true;
                    ViewsOfDeletedNodeTypeProp.Visible = true;
                    EditSelectedViewsButtonProp.Visible = true;
                    ViewsOfDeletedNodeTypeProp.SelectedIndex = 0;
                }
            }
        } // init_DeletePropertyPage()

        public DataTable getViewsUsingNodeType( Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswTableSelect ViewsSelect = Master.CswNbtResources.makeCswTableSelect( "getViewsUsingNodeType_select", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            SelectCols.Add( "viewname" );
            DataTable ViewsTable = ViewsSelect.getTable( SelectCols );
            ArrayList RowsToRemove = new ArrayList();
            foreach( DataRow CurrentRow in ViewsTable.Rows )
            {
                CswNbtView CurrentView = Master.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) );
                if( CurrentView != null && !CurrentView.ContainsNodeType( NodeType ) )
                    RowsToRemove.Add( CurrentRow );
            }

            foreach( DataRow CurrentRow in RowsToRemove )
                ViewsTable.Rows.Remove( CurrentRow );

            return ViewsTable;

        }//getViewsUsingNodeType()



        public DataTable getViewsUsingNodeTypeProp( Int32 NodeTypePropId )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = Master.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
            CswTableSelect ViewsSelect = Master.CswNbtResources.makeCswTableSelect( "getViewsUsingNodeTypeProp_select", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            SelectCols.Add( "viewname" );
            DataTable ViewsTable = ViewsSelect.getTable( SelectCols );
            ArrayList RowsToRemove = new ArrayList();
            foreach( DataRow CurrentRow in ViewsTable.Rows )
            {
                CswNbtView CurrentView = Master.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) );
                if( CurrentView != null && !CurrentView.ContainsNodeTypeProp( NodeTypeProp ) )
                    RowsToRemove.Add( CurrentRow );
            }

            foreach( DataRow CurrentRow in RowsToRemove )
                ViewsTable.Rows.Remove( CurrentRow );

            return ViewsTable;
        }//getViewsUsingNodeTypeProp()

    }
}