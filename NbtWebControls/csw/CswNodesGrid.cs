using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    public class CswNodesGrid : CompositeControl, INamingContainer
    {
        private static Int32 PageSize = 10;
        public Int32 ResultsLimit = Int32.MinValue;
        private static string PropColumnPrefix = "Prop_";

        private CswNbtResources _CswNbtResources;

        private RadGrid _Grid;
        public RadGrid Grid { get { return _Grid; } }

        private CswNbtView _View;
        public CswNbtView View { get { return _View; } set { _View = value; } }

        private CswNbtNodeKey _ParentNodeKey;
        public CswNbtNodeKey ParentNodeKey { get { return _ParentNodeKey; } set { _ParentNodeKey = value; } }

        //BZ 9232
        private string _ViewName;
        public string ViewName
        {
            get
            {
                _ViewName = string.Empty;
                if( View != null )
                    _ViewName = View.ViewName;
                return _ViewName;
            }
            set
            {
                _ViewName = value;
            }
        }

        public bool DisplayViewName = true;

        private DataSet _DataSet;
        public DataSet DataSet { get { return _DataSet; } }

        public bool DisplayMenu = false;
        public bool ShowActionColumns = false;
        public bool ShowAsHtmlTable = false;

        public CswNodesGrid( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            this.DataBinding += new EventHandler( CswNodesGrid_DataBinding );
            EnsureChildControls();
        }

        private CswMainMenu _MainMenu;
        public CswMainMenu Menu { get { EnsureChildControls(); return _MainMenu; } }

        private Literal _GridHeaderLiteral;
        private Image _GridHeaderIcon;
        private Literal _NoResultsLiteral;
        private CswAutoTable _Table;
        protected override void CreateChildControls()
        {
            try
            {
                CswAutoTable GridTable = new CswAutoTable();
                this.Controls.Add( GridTable );

                _MainMenu = new CswMainMenu( _CswNbtResources );
                _MainMenu.ID = "gridmenu";
                GridTable.addControl( 0, 0, _MainMenu );

                _GridHeaderIcon = new Image();
                GridTable.addControl( 1, 0, _GridHeaderIcon );

                _GridHeaderLiteral = new Literal();
                GridTable.addControl( 1, 0, _GridHeaderLiteral );

                _Grid = new RadGrid();
                _Grid.ID = "nodesgrid";
                _Grid.EnableEmbeddedSkins = false;
                _Grid.Skin = "ChemSW";
                string[] GridDataKeyNames = new string[1];
                GridDataKeyNames[0] = "NodeKey";
                _Grid.MasterTableView.DataKeyNames = GridDataKeyNames;
                _Grid.MasterTableView.ClientDataKeyNames = GridDataKeyNames;
                _Grid.AllowPaging = true;
                _Grid.ShowFooter = false;
                _Grid.AllowSorting = true;
                _Grid.AllowMultiRowEdit = true;
                _Grid.ClientSettings.Resizing.AllowColumnResize = true;
                _Grid.ClientSettings.Resizing.EnableRealTimeResize = true;
                //BZ 10097 -- Waiting on Telerik subscription update
                //_Grid.PagerStyle.Mode = GridPagerMode.NextPrevNumericAndAdvanced;
                _Grid.PageSize = PageSize;
                //_Grid.PageSizeChanged += new GridPageSizeChangedEventHandler( _Grid_PageSizeChanged );
                //_Grid.PageIndexChanged += new GridPageChangedEventHandler( _Grid_PageIndexChanged );
                GridTable.addControl( 2, 0, _Grid );

                _NoResultsLiteral = new Literal();
                _NoResultsLiteral.Text = "No Results.";
                GridTable.addControl( 2, 0, _NoResultsLiteral );

                _Table = new CswAutoTable();
                _Table.ID = "nodestable";
                _Table.CellPadding = 2;
                _Table.CssClass = "NodesGridTable";
                _Table.CellCssClass = "NodesGridTableCell";
                GridTable.addControl( 2, 0, _Table );

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.CreateChildControls();
        }

        /* void _Grid_PageIndexChanged( object source, GridPageChangedEventArgs e )
        {
            //throw new NotImplementedException();
        }

        void _Grid_PageSizeChanged( object source, GridPageSizeChangedEventArgs e )
        {
            //throw new NotImplementedException();
        } */

        public ICswNbtTree CswNbtTree = null;

        public bool ReadOnly = false;

        void CswNodesGrid_DataBinding( object sender, EventArgs e )
        {
            try
            {
                //if( ReadOnly )
                //{
                //    DisplayMenu = false;
                //    ShowActionColumn = false;
                //}

                if( View != null )
                {
                    _GridHeaderIcon.ImageUrl = _View.IconFileName;

                    // View width is in characters, not pixels
                    _Grid.Width = Unit.Parse( ( CswConvert.ToInt32( View.Width * 7 ) ).ToString() + "px" );  // average pixel width per character

                    CswNbtNodeKey ParentKey = null;
                    if( ParentNodeKey != null && _View.Root.ChildRelationships.Count > 0 )
                    {
                        // This is a Grid Property
                        ( (CswNbtViewRelationship) _View.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( ParentNodeKey.NodeId );
                        //_View.Root.FilterInNodesRecursively = false;
                        ParentKey = ParentNodeKey;
                    }

                    // Note - Second parameter true for BZ 9200
                    CswNbtTree = _CswNbtResources.Trees.getTreeFromView(
                        RunAsUser: _CswNbtResources.CurrentNbtUser,
                        View: View,
                        RequireViewPermissions: true,
                        IncludeSystemNodes: false,
                        IncludeHiddenNodes: false );
                    //_MainFilterEditor.LoadView(Master.CswNbtView);

                    // BROKEN BY case 24709
                    //if( ParentKey != null )
                    //    CswNbtTree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.TelerikRadGridProperty;
                    //else
                    //    CswNbtTree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.TelerikRadGrid;

                    //string XmlStrForGrid = CswNbtTree.getTreeAsXml();
                    DataSet UnsortedXmlDataSet = new DataSet();
                    //UnsortedXmlDataSet.ReadXml( new System.IO.StringReader( XmlStrForGrid ), XmlReadMode.InferTypedSchema );

                    if( UnsortedXmlDataSet.Tables.Count > 0 && UnsortedXmlDataSet.Tables[0].Rows.Count > 0 )
                    {
                        _NoResultsLiteral.Visible = false;
                        setupGridColumns( UnsortedXmlDataSet, View );

                        //CswNbtViewProperty SortProp = View.getSortProperty();
                        //if( SortProp != null )
                        //{
                        //    string SortMethod = "";
                        //    if( SortProp != null && SortProp.SortMethod == PropertySortMethod.Descending )
                        //        SortMethod = " DESC";

                        //    DataView Sortedview = UnsortedXmlDataSet.Tables[0].DefaultView;
                        //    Sortedview.Sort = "Prop_" + SortProp.NodeTypePropId + "_" + CswTools.XmlSafeAttributeName( SortProp.Name ) + SortMethod;
                        //    _Grid.DataSource = Sortedview;

                        //    if( ShowAsHtmlTable )
                        //        _setupHtmlTable( Sortedview.Table );
                        //}
                        //else
                        //{
                        _Grid.DataSource = UnsortedXmlDataSet;

                        if( ShowAsHtmlTable )
                            _setupHtmlTable( UnsortedXmlDataSet.Tables[0] );
                        //}

                        if( !ShowAsHtmlTable )
                        {
                            _Grid.AutoGenerateColumns = false;
                            _Grid.DataBind();
                        }
                    }
                    else
                    {
                        _NoResultsLiteral.Visible = true;
                    }
                    _DataSet = UnsortedXmlDataSet;


                    if( DisplayMenu )
                    {
                        _MainMenu.View = View;
                        _MainMenu.ParentNodeKey = CswNbtTree.getNodeKeyByNodeId( ParentNodeKey.NodeId );
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        private void _setupHtmlTable( DataTable Data )
        {
            // Header
            Int32 row = 0;
            Int32 col = 0;
            foreach( DataColumn Column in Data.Columns )
            {
                GridColumn GC = _Grid.Columns.FindByDataFieldSafe( Column.ColumnName );
                if( GC != null && GC.Display )
                {
                    Label HeaderCellValue = new Label();
                    HeaderCellValue.Text = GC.HeaderText;
                    HeaderCellValue.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
                    _Table.addControl( row, col, HeaderCellValue );
                    col++;
                }
            }
            row++;
            col = 0;

            // Content
            foreach( DataRow Row in Data.Rows )
            {
                foreach( DataColumn Column in Data.Columns )
                {
                    GridColumn GC = _Grid.Columns.FindByDataFieldSafe( Column.ColumnName );
                    if( GC != null && GC.Display )
                    {
                        Literal CellValue = new Literal();
                        CellValue.Text = Row[Column.ColumnName].ToString();
                        _Table.addControl( row, col, CellValue );
                        col++;
                    }
                }
                row++;
                col = 0;
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                if( !DisplayMenu )
                    _MainMenu.Visible = false;
                if( ShowAsHtmlTable )
                {
                    _Grid.Visible = false;
                    _Table.Visible = true;
                }
                else
                {
                    _Grid.Visible = true;
                    _Table.Visible = false;
                }
                if( DisplayViewName )
                    _GridHeaderLiteral.Text = ViewName;
                else
                {
                    _GridHeaderLiteral.Visible = false;
                    _GridHeaderIcon.Visible = false;
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }

        private void setupGridColumns( DataSet DS, CswNbtView View )
        {
            _Grid.Columns.Clear();

            // Edit column
            GridBoundColumn EditColumn = new GridBoundColumn();
            EditColumn.UniqueName = "EditColumn";
            EditColumn.HeaderText = "Edit";
            EditColumn.DataField = "Edit";
            EditColumn.Display = ShowActionColumns;
            _Grid.Columns.Add( EditColumn );

            // Node column
            GridBoundColumn NodeNameColumn = new GridBoundColumn();
            NodeNameColumn.UniqueName = "NodeName";
            NodeNameColumn.HeaderText = "Node Name";
            NodeNameColumn.DataField = "NodeName";
            // BZ 6704
            NodeNameColumn.Visible = false;
            NodeNameColumn.Display = false;
            _Grid.Columns.Add( NodeNameColumn );

            // NodeKey column
            GridBoundColumn NodeKeyColumn = new GridBoundColumn();
            NodeKeyColumn.UniqueName = "NodeKey";
            NodeKeyColumn.HeaderText = "NodeKey";
            NodeKeyColumn.DataField = "NodeKey";
            NodeKeyColumn.Display = false;
            _Grid.Columns.Add( NodeKeyColumn );

            foreach( DataTable Table in DS.Tables )
            {
                foreach( DataColumn Column in Table.Columns )
                {
                    string ColumnName = Column.ColumnName;
                    if( ColumnName.Length > PropColumnPrefix.Length && ColumnName.Substring( 0, PropColumnPrefix.Length ) == PropColumnPrefix )
                    {
                        string NoPrefixColumnName = ColumnName.Substring( PropColumnPrefix.Length );
                        //Int32 CurrentNodeTypePropId = CswConvert.ToInt32( NoPrefixColumnName.Substring( 0, NoPrefixColumnName.IndexOf( '_' ) ) );
                        string RealColumnName = CswTools.XmlRealAttributeName( NoPrefixColumnName ); //.Substring( NoPrefixColumnName.IndexOf( '_' ) + 1 ) );
                        //CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( CurrentNodeTypePropId );
                        //CswNbtViewProperty CurrentViewProp = View.FindPropertyById( CswNbtPropType.NodeTypePropId, CurrentNodeTypePropId );
                        CswNbtViewProperty CurrentViewProp = View.findPropertyByName( RealColumnName );
                        //if( CurrentViewProp == null )
                        //    CurrentViewProp = View.FindPropertyByName( CurrentNodeTypeProp.PropName );
                        CswNbtMetaDataFieldType.NbtFieldType ColFieldType = CswNbtResources.UnknownEnum;
                        if( CurrentViewProp != null )
                        {
                            if( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                CswNbtMetaDataNodeType CurrentNT = _CswNbtResources.MetaData.getNodeType( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondId );
                                CswNbtMetaDataNodeTypeProp CurrentNTP = CurrentNT.getNodeTypeProp( RealColumnName );
                                if( CurrentNTP != null )
                                    ColFieldType = CurrentNTP.getFieldTypeValue();
                            }
                            else if( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondType == NbtViewRelatedIdType.ObjectClassId )
                            {
                                CswNbtMetaDataObjectClass CurrentOC = _CswNbtResources.MetaData.getObjectClass( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondId );
                                foreach( CswNbtMetaDataNodeType CurrentNT in CurrentOC.getNodeTypes() )
                                {
                                    CswNbtMetaDataNodeTypeProp CurrentNTP = CurrentNT.getNodeTypeProp( RealColumnName );
                                    if( CurrentNTP != null )
                                        ColFieldType = CurrentNTP.getFieldTypeValue();
                                }
                            }
                        }

                        GridBoundColumn thisColumn = null;
                        //switch( CurrentNodeTypeProp.FieldType.FieldType )
                        switch( ColFieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                                thisColumn = new GridDateTimeColumn();
                                thisColumn.DataFormatString = "{0:M/d/yyyy}";
                                thisColumn.DataType = typeof( DateTime );
                                break;
                            //case CswNbtMetaDataFieldType.NbtFieldType.Time:
                            //    thisColumn = new GridDateTimeColumn();
                            //    thisColumn.DataFormatString = "{0:H:mm:ss}";
                            //    thisColumn.DataType = typeof( DateTime );
                            //    break;
                            default:
                                thisColumn = new GridBoundColumn();
                                break;
                        }
                        thisColumn.UniqueName = ColumnName;
                        thisColumn.HeaderText = RealColumnName;
                        thisColumn.DataField = ColumnName;
                        if( CurrentViewProp != null && CurrentViewProp.Width != Int32.MinValue )
                            thisColumn.HeaderStyle.Width = Unit.Parse( ( CswConvert.ToInt32( CurrentViewProp.Width * 7 ) ).ToString() + "px" );  // average pixel width per character

                        //thisColumn.OrderIndex = CurrentViewProp.Order;
                        //Telerik.Web.UI.GridTableView GTV = new GridTableView( _Grid );
                        if( CurrentViewProp != null && CurrentViewProp.Order > 0 && _Grid.Columns.Count >= CurrentViewProp.Order )
                        {
                            _Grid.Columns.AddAt( CurrentViewProp.Order, thisColumn );
                        }
                        else
                            _Grid.Columns.Add( thisColumn );
                    }
                }
            }

            // Delete column
            GridBoundColumn DeleteColumn = new GridBoundColumn();
            DeleteColumn.UniqueName = "DeleteColumn";
            DeleteColumn.HeaderText = "Delete";
            DeleteColumn.DataField = "Delete";
            DeleteColumn.Display = ShowActionColumns;
            _Grid.Columns.Add( DeleteColumn );

        } // setupGridColumns()

        public event CswErrorHandler OnError;

        void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }

    } // class CswNodesGrid 
} // namespace ChemSW.NbtWebControls
