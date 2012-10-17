using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.NbtWebControls
{

    public class CswLocationImageGrid : CswLocationImage
    {
        public Int32 TotalRows = 0;
        public Int32 TotalColumns = 0;

        private CswPrimaryKey _ParentNodeId
        {
            get
            {
                if( _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ) != null )
                    return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText ) );
                else
                    return null;
            }
        }

        public CswLocationImageGrid( CswNbtResources CswNbtResources, XmlNode ParentNode, CswPrimaryKey SelectedNodeId )
            : base( CswNbtResources, ParentNode, SelectedNodeId )
        {
            XmlNodeList Rows = _ParentNodeSet.SelectNodes( CswNbtLocationTreeDeprecated.XmlNodeName_Row );
            TotalRows = Rows.Count;
            foreach( XmlNode Row in Rows )
            {
                Int32 currentCellCount = Row.SelectNodes( CswNbtLocationTreeDeprecated.XmlNodeName_Cell ).Count;
                if( currentCellCount > TotalColumns )
                    TotalColumns = currentCellCount;
            }
        }

        protected override void OnLoad( EventArgs e )
        {
            EnsureChildControls();
            base.OnLoad( e );
        }

        private CswAutoTable _Table;
        protected override void CreateChildControls()
        {
            _Table = new CswAutoTable();
            _Table.CellSpacing = 0;
            _Table.CellPadding = 0;
            _Table.CssClass = "LocationGridTable";
            this.Controls.Add( _Table );

            Int32 currentRow = 0;
            Int32 currentCol = 0;

            TableCell TitleCell = _Table.getCell( currentRow, currentCol );
            TitleCell.ColumnSpan = TotalColumns + 1;
            TitleCell.Style.Add( HtmlTextWriterStyle.TextAlign, "center" );

            CswImageOverlay Title = new CswImageOverlay();
            Title.ID = "title";
            Title.ButtonText = KeyPrefix + _ParentNodeId;
            Title.ImageUrl = "";
            Title.IsButton = true;
            Title.Click += new EventHandler( Image_Click );
            TitleCell.Controls.Add( Title );

            Label TitleLabel = new Label();
            TitleLabel.ID = this.ID + "_label";
            TitleLabel.Text = _ParentNode.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Display ).InnerText;
            if( _SelectedNodeId == _ParentNodeId )
                TitleLabel.CssClass = "LocationTitleTextSelected";
            else
                TitleLabel.CssClass = "LocationTitleText";
            Title.Content.Add( TitleLabel );

            currentRow++;
            currentCol = 0;



            // Top Label Row
            currentCol++;  // blank cell
            while( currentCol < TotalColumns + 1 )
            {
                TableCell LabelCell = _Table.getCell( currentRow, currentCol );
                //LabelCell.HorizontalAlign = HorizontalAlign.Center;
                LabelCell.CssClass = "LocationColumnLabelCell";

                Label ColLabel = new Label();
                ColLabel.ID = "ColLabel_" + currentCol.ToString();
                ColLabel.Text = currentCol.ToString();
                ColLabel.CssClass = "LocationColumnLabel";
                LabelCell.Controls.Add( ColLabel );
                currentCol++;
            }
            currentRow++;
            currentCol = 0;


            // Top graphic row
            currentCol++;    // blank cell

            Image TopLeft = new Image();
            TopLeft.ImageUrl = ImageUrlBase + "g_topleft.gif";
            _Table.addControl( currentRow, currentCol, TopLeft );
            currentCol++;

            while( currentCol < TotalColumns + 1 )
            {
                Image Top = new Image();
                Top.ImageUrl = ImageUrlBase + "g_top.gif";
                _Table.addControl( currentRow, currentCol, Top );
                currentCol++;
            }

            Image TopRight = new Image();
            TopRight.ImageUrl = ImageUrlBase + "g_topright.gif";
            TopRight.Width = 17;
            TopRight.Height = 17;
            _Table.addControl( currentRow, currentCol, TopRight );
            currentCol++;

            currentRow++;
            currentCol = 0;

            ICswNbtTree CswNbtTree = null;
            if( View != null )
            {
                // BZ 8106 - Show properties in cells
                // In terms of view mechanics, this is similar to what we do in Grids
                if( _ParentNodeId != null && View.Root.ChildRelationships.Count > 0 )
                {
                    ( (CswNbtViewRelationship) View.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( _ParentNodeId );
                }
                CswNbtTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
            }


            // Main Rows
            while( currentRow < TotalRows + 3 )
            {
                currentCol = 0;

                // Row Label
                TableCell RowLabelCell = _Table.getCell( currentRow, currentCol );
                Label RowLabel = new Label();
                RowLabel.ID = "RowLabel_" + currentRow.ToString();
                RowLabel.Text = CswTools.NumberToLetter( currentRow - 2 ).ToString().ToUpper();
                RowLabel.CssClass = "LocationRowLabel";
                //_Table.addControl(currentRow, currentCol, RowLabel);
                RowLabelCell.Controls.Add( RowLabel );
                RowLabelCell.CssClass = "LocationRowLabelCell";
                currentCol++;

                while( currentCol < TotalColumns + 1 )
                {
                    // Content Cells
                    XmlNode CellNode = _ParentNodeSet.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Row + "[" + ( currentRow - 2 ) + "]/" + CswNbtLocationTreeDeprecated.XmlNodeName_Cell + "[" + ( currentCol ) + "]" );
                    CswNbtLocationTreeDeprecated.GridLocationTemplate Template = (CswNbtLocationTreeDeprecated.GridLocationTemplate) Enum.Parse( typeof( CswNbtLocationTreeDeprecated.GridLocationTemplate ), CellNode.Attributes[CswNbtLocationTreeDeprecated.XmlAttrName_LocationTemplate].Value, true );

                    CswImageOverlay CellImage = new CswImageOverlay();
                    CellImage.ID = GridKeyPrefix + ( currentRow - 3 ).ToString() + "_" + ( currentCol - 1 ).ToString();
                    if( Template != CswNbtLocationTreeDeprecated.GridLocationTemplate.Empty )
                    {
                        CellImage.ID = GridKeyPrefix + ( currentRow - 3 ).ToString() + "_" + ( currentCol - 1 ).ToString();
                        CellImage.ButtonText = GridKeyPrefix + ( currentRow - 3 ).ToString() + "_" + ( currentCol - 1 ).ToString();
                    }
                    CellImage.ImageWidth = 101;
                    CellImage.ImageHeight = 101;
                    CellImage.IsButton = ( _MoveMode );

                    switch( Template )
                    {
                        case CswNbtLocationTreeDeprecated.GridLocationTemplate.Grid:
                            CellImage.ImageUrl = "g_grid";
                            break;
                        case CswNbtLocationTreeDeprecated.GridLocationTemplate.Empty:
                            CellImage.ImageUrl = "g_empty";
                            break;
                    }

                    if( Template != CswNbtLocationTreeDeprecated.GridLocationTemplate.Empty )
                    {
                        //CellImage.HoverImageUrl = CellImage.ImageUrl + "_hover";
                        if( _SelectedNodeId == _ParentNodeId && SelectedRow == ( currentRow - 3 ) && SelectedColumn == currentCol - 1 )
                        {
                            CellImage.ImageUrl += "_selected";
                            //CellImage.HoverImageUrl += "_selected";
                        }
                    }

                    CellImage.ImageUrl = ImageUrlBase + CellImage.ImageUrl + ".gif";
                    if( CellImage.HoverImageUrl != String.Empty )
                        CellImage.HoverImageUrl = ImageUrlBase + CellImage.HoverImageUrl + ".gif";

                    CellImage.Click += new EventHandler( Image_Click );
                    _Table.addControl( currentRow, currentCol, CellImage );
                    currentCol++;

                    // Non-Location Children
                    XmlNodeList ChildNodes = CellNode.SelectNodes( CswNbtLocationTreeDeprecated.XmlNodeName_ChildSet );
                    foreach( XmlNode Child in ChildNodes )
                    {
                        CswPrimaryKey ChildNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Child.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Key ).InnerText ) );

                        string ThisPropString = string.Empty;
                        if( CswNbtTree != null )
                        {
                            CswNbtNodeKey ChildNodeKey = CswNbtTree.getNodeKeyByNodeId( ChildNodeId );
                            if( ChildNodeKey != null )
                            {
                                CswNbtNode ChildNode = CswNbtTree.getNode( ChildNodeKey );
                                CswNbtMetaDataNodeType ChildNodeType = _CswNbtResources.MetaData.getNodeType( ChildNode.NodeTypeId );
                                CswNbtViewRelationship ChildNodeViewRelationship = View.FindViewNodeByUniqueId( ChildNodeKey.ViewNodeUniqueId ) as CswNbtViewRelationship;

                                foreach( CswNbtViewProperty ChildNodeViewProp in ChildNodeViewRelationship.Properties )
                                {
                                    CswNbtMetaDataNodeTypeProp ThisNTProp = null;
                                    if( ChildNodeViewProp.Type == NbtViewPropType.NodeTypePropId )
                                    {
                                        ThisNTProp = ChildNodeViewProp.NodeTypeProp;
                                    }
                                    else
                                    {
                                        CswNbtMetaDataObjectClassProp ThisOCProp = _CswNbtResources.MetaData.getObjectClassProp( ChildNodeViewProp.ObjectClassPropId );
                                        ThisNTProp = ChildNodeType.getNodeTypePropByObjectClassProp( ThisOCProp.PropName );
                                    }
                                    CswNbtNodePropWrapper ThisProp = ChildNode.Properties[ThisNTProp];
                                    ThisPropString += ThisProp.PropName + ": " + ThisProp.Gestalt + "<BR>";
                                }
                            }
                        }

                        CswImageOverlay NodeButton = new CswImageOverlay();
                        NodeButton.ID = NodePrefix + ChildNodeId.PrimaryKey.ToString();
                        NodeButton.ButtonText = NodePrefix + ChildNodeId.PrimaryKey.ToString();
                        NodeButton.Click += new EventHandler( Image_Click );
                        if( _MoveMode )
                            NodeButton.IsButton = false;
                        if( OnClientSideLocationImageClick != string.Empty )
                            NodeButton.OnClientClick = OnClientSideLocationImageClick + "(" + ChildNodeId.ToString() + ");";
                        CellImage.Content.Add( NodeButton );

                        Image Icon = new Image();
                        Icon.ImageUrl = "Images/icons/" + Child.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_IconFileName ).InnerText;
                        NodeButton.Content.Add( Icon );

                        Label NodeLabel = new Label();
                        NodeLabel.Text = Child.SelectSingleNode( CswNbtLocationTreeDeprecated.XmlNodeName_Display ).InnerText + "<BR>";
                        if( ChildNodeId == _SelectedNodeId )
                            NodeLabel.CssClass = "LocationTextSelected";
                        else
                            NodeLabel.CssClass = "LocationText";
                        NodeButton.Content.Add( NodeLabel );

                        if( ThisPropString != string.Empty )
                        {
                            Literal PropsLiteral = new Literal();
                            PropsLiteral.Text = ThisPropString;
                            NodeButton.Content.Add( PropsLiteral );
                        }
                    }
                }

                // End Cell
                Image Right = new Image();
                if( currentRow == TotalRows + 2 )
                    Right.ImageUrl = ImageUrlBase + "g_bottomright.gif";
                else
                    Right.ImageUrl = ImageUrlBase + "g_right.gif";
                _Table.addControl( currentRow, currentCol, Right );
                currentRow++;
            }

            base.CreateChildControls();
        }
    }
}