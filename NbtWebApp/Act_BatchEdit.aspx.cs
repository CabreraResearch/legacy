using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using Telerik.Web.UI;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.ImportExport;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_BatchEdit : System.Web.UI.Page
    {
        //private DataSet _DS;
        private CswNbtView _View;
        private CswNbtImportExport _ImportExport;
        private CswAutoTable _Table;
        //private CswFieldTypeWebControlFactory _Factory;
        private Button _SaveButton;
        private Hashtable _ColumnMap;
        private ICswNbtTree _Tree;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                _ImportExport = new CswNbtImportExport( Master.CswNbtResources );
				_View = Master.CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( 696 ) );
                _Tree = Master.CswNbtResources.Trees.getTreeFromView( _View, true, true, false, false );
                //_Factory = new CswFieldTypeWebControlFactory( Master.CswNbtResources );

                _Table = new CswAutoTable();
                _Table.ID = "batchedittable";
                Int32 CurrentRow = 0;
                Int32 CurrentCol = 0;
                _ColumnMap = new Hashtable();

                // Header
                _Tree.goToRoot();
                Literal NodeNameHeaderLiteral = new Literal();
                NodeNameHeaderLiteral.Text = "Name";
                _Table.addControl( CurrentRow, CurrentCol, NodeNameHeaderLiteral );
                _ColumnMap.Add( "NodeName", CurrentCol );
                CurrentCol++;

                _SetupHeaderColumns( _View.Root, CurrentRow, ref CurrentCol );

                CurrentRow++;
                CurrentCol = 0;

                // Data
                _Tree.goToRoot();
                _SetupData( ref CurrentRow );

                _SaveButton = new Button();
                _SaveButton.ID = "savebutton";
                _SaveButton.Text = "Save";
                _SaveButton.CssClass = "Button";
                _SaveButton.UseSubmitBehavior = true;
                _SaveButton.Click += new EventHandler( SaveButton_Click );
                _SaveButton.OnClientClick = "if(!CswPropertyTable_SaveButton_PreClick()) return false;";
                _SaveButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                ph.Controls.Add( _SaveButton );

                ph.Controls.Add( _Table );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }


        private void _SetupHeaderColumns( CswNbtViewNode ViewNode, Int32 CurrentRow, ref Int32 CurrentCol )
        {
            foreach( CswNbtViewRelationship ViewRelationship in ViewNode.GetChildrenOfType(NbtViewNodeType.CswNbtViewRelationship ))
            {
                foreach( CswNbtViewProperty ViewProp in ViewRelationship.Properties )
                {
                    CswNbtMetaDataNodeTypeProp NodeTypeProp = ViewProp.NodeTypeProp;
                    Literal PropHeaderLiteral = new Literal();
                    PropHeaderLiteral.Text = NodeTypeProp.PropName;
                    _Table.addControl( CurrentRow, CurrentCol, PropHeaderLiteral );
                    _ColumnMap.Add( NodeTypeProp, CurrentCol );
                    CurrentCol++;
                }

                _SetupHeaderColumns( ViewRelationship, CurrentRow, ref CurrentCol ); 
            }
        }

        private void _SetupData(ref Int32 CurrentRow)
        {
            for( int i = 0; i < _Tree.getChildNodeCount(); i++ )
            {
                _Tree.goToNthChild( i );

                //CswNbtNodeKey Key = _Tree.getNodeKeyForCurrentPosition();
                CswNbtNode Node = _Tree.getNodeForCurrentPosition();
                CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );

                Literal NodeNameLiteral = new Literal();
                NodeNameLiteral.Text = Node.NodeName;
                _Table.addControl( CurrentRow, Convert.ToInt32( _ColumnMap["NodeName"].ToString() ), NodeNameLiteral );

                foreach(CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeType.NodeTypeProps)
                {
                    if(_ColumnMap.ContainsKey(NodeTypeProp))
                    {
                        TableCell ThisCell = _Table.getCell( CurrentRow, Convert.ToInt32( _ColumnMap[NodeTypeProp].ToString() ) );
                        CswFieldTypeWebControl Control = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources, ThisCell.Controls, string.Empty, NodeTypeProp, Node, NodeEditMode.Edit, new CswErrorHandler( Master.HandleError ) );
                        //Control.ReadOnly = !( NodeTypeProp.FieldType.IsSimpleType() );
                        Control.DataBind();
                    }
                }
                CurrentRow++;

                if( _Tree.getChildNodeCount() > 0 )
                    _SetupData( ref CurrentRow );

                _Tree.goToParentNode();
            }
        }


        void SaveButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswPropertyTable.SaveFieldTypeWebControls( ph.Controls );

                _Tree.goToRoot(); 
                _postNodes();

                // Commit any transactions
                Master.CswNbtResources.finalize();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//SaveButton_Click()

        private void _postNodes()
        {
            for( int i = 0; i < _Tree.getChildNodeCount(); i++ )
            {
                _Tree.goToNthChild( i );
                
                CswNbtNode Node = _Tree.getNodeForCurrentPosition();
                Node.postChanges( false );

                if( _Tree.getChildNodeCount() > 0 )
                    _postNodes();
                
                _Tree.goToParentNode();
            }
        }



        //private DataSet _DS;
        //private RadGrid _Grid;

        //protected override void OnInit( EventArgs e )
        //{
        //    CswNbtView View = (CswNbtView) CswNbtViewFactory.restoreView( Master.CswNbtResources, 361 );
        //    //ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );

        //    CswNbtImportExport Exporter = new CswNbtImportExport( Master.CswNbtResources );
        //    XmlDocument XmlDoc = Exporter.ExportXml( View, false );
        //    string XmlString = XmlDoc.InnerXml;

        //    _DS = new DataSet();
        //    _DS.ReadXml( new System.IO.StringReader( XmlString ) );

        //    _Grid = new RadGrid();
        //    _Grid.ID = "Batchgrid";
        //    _Grid.AllowMultiRowEdit = true;
        //    _Grid.ItemDataBound += new GridItemEventHandler( _Grid_ItemDataBound );

        //    _Grid.AllowPaging = true;
        //    _Grid.PageSize = 10;

        //    _Grid.DataSource = _DS;
        //    _Grid.MasterTableView.DataSource = _DS;
        //    _Grid.MasterTableView.DataMember = CswNbtImportExport._Element_Node;
        //    _Grid.MasterTableView.DataKeyNames = new string[1] { CswNbtImportExport._Attribute_NodeId };
        //    _Grid.MasterTableView.AutoGenerateColumns = false;
        //    //_Grid.MasterTableView.EditMode = Telerik.Web.UI.GridEditMode.EditForms;
        //    //_Grid.MasterTableView.EditFormSettings.EditFormType = GridEditFormType.Template;
        //    //_Grid.MasterTableView.EditFormSettings.FormTemplate = new GridEditFieldTypeTemplate( Master.CswNbtResources );
        //    _Grid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.TopAndBottom;
        //    _Grid.MasterTableView.CommandItemTemplate = new GridCommandItemTemplate();
        //    _Grid.AllowAutomaticUpdates = true;

        //    GridBoundColumn NodeNameColumn = new GridBoundColumn();
        //    NodeNameColumn.DataField = "nodename";
        //    NodeNameColumn.DataType = typeof( string );
        //    NodeNameColumn.HeaderText = "Name";
        //    NodeNameColumn.ReadOnly = true;
        //    _Grid.MasterTableView.Columns.Add( NodeNameColumn );

        //    foreach( DataRow Row in _DS.Tables[CswNbtMetaDataNodeTypeProp._Element_MetaDataNodeTypeProp].Rows )
        //    {
        //        Int32 ThisNodeTypePropId = CswConvert.ToInt32( Row[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId].ToString() );
        //        if( View.ContainsNodeTypeProp( ThisNodeTypePropId ) )
        //        {
        //            GridTemplateColumn PropColumn = new GridTemplateColumn();
        //            PropColumn.HeaderText = Row[CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName].ToString();
        //            PropColumn.ItemTemplate = new GridFieldTypeTemplate( Master.CswNbtResources, _DS, ThisNodeTypePropId, false );
        //            //PropColumn.EditItemTemplate = new GridFieldTypeTemplate( Master.CswNbtResources, _DS, ThisNodeTypePropId, false );
        //            _Grid.MasterTableView.Columns.Add( PropColumn );
        //        }
        //    }

        //    //GridButtonColumn EditColumn = new GridButtonColumn();
        //    //EditColumn.CommandName = "Edit";
        //    //EditColumn.ButtonType = GridButtonColumnType.PushButton;
        //    //EditColumn.Text = "Edit";
        //    //EditColumn.UniqueName = "EditColumn";
        //    //_Grid.MasterTableView.Columns.Add( EditColumn );

        //    // This MUST be last
        //    ph.Controls.Add( _Grid );

        //    _Grid.DataBind();

        //    base.OnInit( e );

        //}

        //protected override void OnLoad( EventArgs e )
        //{
        //    Master.AjaxManager.AjaxSettings.AddAjaxSetting( _Grid, _Grid );
        //    base.OnLoad( e );
        //}

        //void _Grid_ItemDataBound( object sender, GridItemEventArgs e )
        //{
        //    if( e.Item is GridDataItem && e.Item.IsInEditMode )
        //    {
        //        GridDataItem dataItem = e.Item as GridDataItem;
        //        //Hides the Update button for each edit form
        //        dataItem["EditCommandColumn"].Controls[0].Visible = false;
        //        dataItem.Edit = true;
        //    }
        //}





        //private class GridCommandItemTemplate : ITemplate
        //{
        //    public void InstantiateIn( Control parent )
        //    {
        //        Button UpdateAllButton = new Button();
        //        UpdateAllButton.ID = "UpdateAllButton";
        //        UpdateAllButton.CssClass = "Button";
        //        UpdateAllButton.Text = "Update All";
        //        UpdateAllButton.CommandName = "UpdateAll";
        //        parent.Controls.Add( UpdateAllButton );
        //    }

        //} // GridCommandItemTemplate

        //private class GridFieldTypeTemplate : ITemplate
        //{
        //    private CswNbtResources _CswNbtResources;
        //    private DataSet _DS;
        //    private Int32 _NodeTypePropId;
        //    private bool _ReadOnly;
        //    public GridFieldTypeTemplate( CswNbtResources R, DataSet DS, Int32 NodeTypePropId, bool ReadOnly )
        //    {
        //        _CswNbtResources = R;
        //        _NodeTypePropId = NodeTypePropId;
        //        _DS = DS;
        //        _ReadOnly = ReadOnly;
        //    }

        //    #region ITemplate Members

        //    public void InstantiateIn( Control parent )
        //    {
        //        GridItem Item = (GridItem) parent.NamingContainer;
        //        CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( _NodeTypePropId );
        //        if( NodeTypeProp.FieldType.IsSimpleType() )
        //        {
        //            DataRowView NodeRow = (DataRowView) Item.DataItem;
        //            Int32 NodeId = CswConvert.ToInt32( NodeRow[CswNbtImportExport._Attribute_NodeId].ToString() );
        //            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId );
        //            CswNbtNodeKey Key = new CswNbtNodeKey( _CswNbtResources, null, string.Empty, Node.NodeId, Node.NodeSpecies, Node.NodeTypeId, Node.ObjectClassId, string.Empty, string.Empty, null );
        //            CswFieldTypeWebControlFactory Factory = new CswFieldTypeWebControlFactory( _CswNbtResources );
        //            CswFieldTypeWebControl Control = Factory.makeControl( parent.Controls, NodeTypeProp, Key, NodeEditMode.Edit );
        //            Control.ReadOnly = _ReadOnly;
        //        }
        //    }
    
        //    #endregion
            
        //} // GridFieldTypeTemplate
    
    }
}