using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_EnterResults : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
                _initViewList();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnInit( e );
        }

        private Label _LoadViewLabel;
        private DropDownList _LoadViewList;
        private Button _PickViewButton;
        private FarPoint.Web.Spread.FpSpread _Spread;
        private HiddenField ColumnHashHiddenField;
        protected override void CreateChildControls()
        {
            CswAutoTable OuterTable = new CswAutoTable();
            OuterTable.ID = "OuterTable";
            ph.Controls.Add( OuterTable );

            _LoadViewLabel = new Label();
            _LoadViewLabel.ID = "LoadViewLabel";
            _LoadViewLabel.Text = "Select a View of Results:";

            _LoadViewList = new DropDownList();
            _LoadViewList.ID = "viewlist";
            _LoadViewList.CssClass = "selectinput";

            _PickViewButton = new Button();
            _PickViewButton.ID = "PickViewButton";
            _PickViewButton.Text = "Load";
            _PickViewButton.CssClass = "Button";
            _PickViewButton.UseSubmitBehavior = true;
            _PickViewButton.Click += new EventHandler( _PickViewButton_Click );

            _Spread = new FarPoint.Web.Spread.FpSpread();
            _Spread.ID = "ResultSpread";
            _Spread.Height = 340;
            _Spread.Width = 600;
            _Spread.EditModeReplace = true;
            _Spread.ActiveSheetViewIndex = 0;
            _Spread.EnableAjaxCall = true;
            _Spread.UpdateCommand += new FarPoint.Web.Spread.SpreadCommandEventHandler( _Spread_UpdateCommand );
            _Spread.Visible = false;

            FarPoint.Web.Spread.SheetView Sheet1 = new FarPoint.Web.Spread.SheetView();
            Sheet1.SheetName = "Sheet1";
            Sheet1.AllowInsert = false;
            Sheet1.AllowDelete = false;
            Sheet1.AutoGenerateColumns = false;
            _Spread.Sheets.Add( Sheet1 );

            OuterTable.addControl( 0, 0, _LoadViewLabel );
            OuterTable.addControl( 0, 0, _LoadViewList );
            OuterTable.addControl( 0, 0, _PickViewButton );
            OuterTable.addControl( 1, 0, _Spread );

            ColumnHashHiddenField = new HiddenField();
            ColumnHashHiddenField.ID = "ColumnHashHiddenField";
            ph.Controls.Add( ColumnHashHiddenField );


            base.CreateChildControls();
        }

        private void _initViewList()
        {
            _LoadViewList.Items.Clear();
            Collection<CswNbtView> Views = Master.CswNbtResources.ViewSelect.getVisibleViews( false );
            //else
                //Views = CswNbtView.getUserViews( Master.CswNbtResources );

            if( Views.Count > 0 )
            {
                foreach( CswNbtView View in Views )
                {
                    _LoadViewList.Items.Add( new ListItem( View.ViewName, View.ViewId.ToString() ) );
                }
            }
            else
            {
                _LoadViewList.Visible = false;
                _LoadViewLabel.Visible = false;
            }
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                Master.AjaxManager.AjaxSettings.AddAjaxSetting( _Spread, Master.ErrorBox );

                if(ColumnHashHiddenField.Value != string.Empty)
                    ColumnHash = CswTools.DeserializeHash( ColumnHashHiddenField.Value );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                ColumnHashHiddenField.Value = CswTools.SerializeHash( ColumnHash );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnPreRender( e );
        }


        void _PickViewButton_Click( object sender, EventArgs e )
        {
            initSpread();
        }

        private Int32 AliquotNameColumn = 0;
        private Int32 ResultNameColumn = 1;
        private Int32 ResultNodeIdColumn = 2;
        private Hashtable ColumnHash = new Hashtable();
        private Int32 CurrentMaxColumn = 2;

        private void initSpread()
        {
            if( CswTools.IsInteger( _LoadViewList.SelectedValue ) )
            {
                // Run the selected View
				CswNbtView SelectedView = Master.CswNbtResources.ViewSelect.restoreView( CswConvert.ToInt32( _LoadViewList.SelectedValue ) ) as CswNbtView;
                ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( SelectedView, true, true, false, false );

                // Harvest the Result IDs
                Collection<CswNbtNode> ResultNodes = new Collection<CswNbtNode>();
                _harvestResultNodes( Tree, ref ResultNodes );

                // Convert the nodes to FarPoint's Spread XML format
                FarPoint.Web.Spread.SheetView Sheet = _Spread.Sheets[0];

                // clear the sheet
                ColumnHash = new Hashtable();
                CurrentMaxColumn = 2;
                Sheet.RemoveRows( 0, Sheet.Rows.Count );
                Sheet.RemoveColumns( 0, Sheet.Columns.Count );
                //Sheet.ColumnCount = CurrentMaxColumn + 1;

                _Spread.Visible = false;
                if( ResultNodes.Count > 0 )
                {
                    _Spread.Visible = true;
                    Sheet.Rows.Count = ResultNodes.Count;
                    // Build the column hash first
                    foreach( CswNbtNode ResultNode in ResultNodes )
                    {
                        CswNbtMetaDataNodeType ResultNodeType = Master.CswNbtResources.MetaData.getNodeType( ResultNode.NodeTypeId );
                        CswNbtObjClassResult ResultObjClass = CswNbtNodeCaster.AsResult( ResultNode );
                        foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ResultNodeType.NodeTypeProps )
                        {
                            if( NodeTypeProp.FieldType.IsSimpleType() )
                            {
                                if( !ColumnHash.ContainsKey( NodeTypeProp.PropName.ToLower() ) )
                                {
                                    CurrentMaxColumn++;
                                    ColumnHash.Add( NodeTypeProp.PropName.ToLower(), CurrentMaxColumn );
                                }
                            }
                        }
                    }
                    Sheet.ColumnCount = CurrentMaxColumn + 1;

                    Int32 Row = 0;
                    foreach( CswNbtNode ResultNode in ResultNodes )
                    {
                        //Sheet.Rows.Count = Row + 1;
                        CswNbtMetaDataNodeType ResultNodeType = Master.CswNbtResources.MetaData.getNodeType( ResultNode.NodeTypeId );

                        CswNbtObjClassResult ResultObjClass = CswNbtNodeCaster.AsResult( ResultNode );

                        Sheet.Cells[Row, ResultNameColumn].Text = ResultNode.NodeName;
                        Sheet.Cells[Row, ResultNodeIdColumn].Text = ResultNode.NodeId.PrimaryKey.ToString();
                        Sheet.Cells[Row, AliquotNameColumn].Text = ResultObjClass.Aliquot.CachedNodeName;
                        
                        // Lock all property cells here, unlock below
                        for( int c = 0; c < Sheet.ColumnCount; c++ )
                        {
                            Sheet.Cells[Row, c].Locked = true;
                        }

                        // Properties
                        foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in ResultNodeType.NodeTypeProps )
                        {
                            if( NodeTypeProp.FieldType.IsSimpleType() )
                            {
                                //if( !ColumnHash.ContainsKey( NodeTypeProp.PropName ) )
                                //{
                                //    CurrentMaxColumn++;
                                //    ColumnHash.Add( NodeTypeProp.PropName, CurrentMaxColumn );
                                //    Sheet.ColumnCount = CurrentMaxColumn + 1;
                                //}

                                FarPoint.Web.Spread.Cell ThisCell = Sheet.Cells[Row, CswConvert.ToInt32( ColumnHash[NodeTypeProp.PropName.ToLower()] )];
                                ThisCell.Locked = false;
                                switch( NodeTypeProp.FieldType.FieldType )
                                {
                                    case CswNbtMetaDataFieldType.NbtFieldType.Date:
                                        FarPoint.Web.Spread.DateTimeCellType datecell = new FarPoint.Web.Spread.DateTimeCellType();
                                        datecell.ErrorMessage = "Invalid Date";
                                        datecell.FormatString = "M/d/yyyy";
                                        ThisCell.CellType = datecell;
                                        if(ResultNode.Properties[NodeTypeProp].AsDate.DateValue > DateTime.MinValue)
                                            ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsDate.DateValue.ToString();
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Time:
                                        FarPoint.Web.Spread.DateTimeCellType timecell = new FarPoint.Web.Spread.DateTimeCellType();
                                        timecell.ErrorMessage = "Invalid Time";
                                        timecell.FormatString = "h:m:s";
                                        ThisCell.CellType = timecell;
                                        if( ResultNode.Properties[NodeTypeProp].AsDate.DateValue > DateTime.MinValue ) 
                                            ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsTime.TimeValue.ToString();
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.List:
                                        FarPoint.Web.Spread.ComboBoxCellType listcell = new FarPoint.Web.Spread.ComboBoxCellType();
                                        listcell.Items = CswTools.SplitAndTrim( NodeTypeProp.ListOptions, ',' );
                                        listcell.Values = CswTools.SplitAndTrim( NodeTypeProp.ListOptions, ',' );
                                        ThisCell.CellType = listcell;
                                        ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsList.Value; 
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                                        FarPoint.Web.Spread.ComboBoxCellType logicalcell = new FarPoint.Web.Spread.ComboBoxCellType();
                                        logicalcell.Items = new string[3] { "True", "False", "Null" };
                                        ThisCell.CellType = logicalcell;
                                        ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsLogical.Checked.ToString();
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Number:
                                        FarPoint.Web.Spread.DoubleCellType numbercell = new FarPoint.Web.Spread.DoubleCellType();
                                        numbercell.FixedPoint = true;
                                        if( NodeTypeProp.MaxValue != Int32.MinValue )
                                            numbercell.MaximumValue = NodeTypeProp.MaxValue;
                                        if( NodeTypeProp.MinValue != Int32.MinValue )
                                            numbercell.MinimumValue = NodeTypeProp.MinValue;
                                        if(NodeTypeProp.NumberPrecision != Int32.MinValue)
                                            numbercell.DecimalDigits = NodeTypeProp.NumberPrecision;
                                        ThisCell.CellType = numbercell;
                                        if( // ResultNode.Properties[NodeTypeProp].AsNumber.Value != Double.MinValue &&
                                            !Double.IsNaN( ResultNode.Properties[NodeTypeProp].AsNumber.Value ) )
                                        {
                                            ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsNumber.Value.ToString();
                                        }
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                                        FarPoint.Web.Spread.TextCellType memocell = new FarPoint.Web.Spread.TextCellType();
                                        memocell.Multiline = true;
                                        ThisCell.CellType = memocell;
                                        ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsMemo.Text;
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Text:
                                        ThisCell.Text = ResultNode.Properties[NodeTypeProp].AsText.Text;
                                        break;
                                }
                            }
                        }
                        Row++;
                    }

                    Sheet.Columns[AliquotNameColumn].Label = "Aliquot";
                    Sheet.Columns[AliquotNameColumn].Locked = true;
                    Sheet.Columns[ResultNameColumn].Label = "Test Point";
                    Sheet.Columns[ResultNameColumn].Locked = true;
                    Sheet.Columns[ResultNodeIdColumn].Visible = false;
                    Sheet.Columns[ResultNodeIdColumn].Locked = true;
                    foreach( string PropName in ColumnHash.Keys )
                    {
                        Sheet.Columns[CswConvert.ToInt32( ColumnHash[PropName.ToLower()] )].Label = PropName.ToUpper();
                    }
                    Sheet.PageSize = 200;
                    Sheet.SelectionBackColor = System.Drawing.Color.FromArgb( 255, 255, 0 );
                    Sheet.LockBackColor = System.Drawing.Color.FromArgb( 200, 200, 200 );
                }
            }
        }

        private void _harvestResultNodes( ICswNbtTree Tree, ref Collection<CswNbtNode> ResultNodes)
        {
            for( int i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );
                
                // Get result node
                CswNbtNode CurrentNode = Tree.getNodeForCurrentPosition();
                if( CurrentNode.NodeSpecies == NodeSpecies.Plain &&
                    CurrentNode.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.ResultClass )
                {
                    ResultNodes.Add( CurrentNode );
                }
                //Recurse
                if( Tree.getChildNodeCount() > 0 )
                    _harvestResultNodes( Tree, ref ResultNodes );

                Tree.goToParentNode();
            }
        }

        void _Spread_UpdateCommand( object sender, FarPoint.Web.Spread.SpreadCommandEventArgs e )
        {
            try
            {

                // Save changes
                CswPrimaryKey ResultNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( _Spread.Sheets[0].GetValue( CswConvert.ToInt32( e.CommandArgument ), ResultNodeIdColumn ) ) );
                CswNbtNode ResultNode = Master.CswNbtResources.Nodes[ResultNodeId];
                CswNbtMetaDataNodeType ResultNodeType = Master.CswNbtResources.MetaData.getNodeType( ResultNode.NodeTypeId );

                foreach( string PropName in ColumnHash.Keys )
                {
                    CswNbtMetaDataNodeTypeProp Prop = ResultNodeType.getNodeTypeProp( PropName );
                    // If the field is an "object", it is unchanged.  If it's a string, we have a value to store.
                    if( e.EditValues[CswConvert.ToInt32( ColumnHash[PropName.ToLower()] )] is string )
                    {
                        string NewValue = e.EditValues[CswConvert.ToInt32( ColumnHash[PropName.ToLower()] )].ToString();
                        switch( Prop.FieldType.FieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.Date:
                                ResultNode.Properties[Prop].AsDate.DateValue = Convert.ToDateTime( NewValue );
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Text:
                                ResultNode.Properties[Prop].AsText.Text = NewValue;
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Time:
                                ResultNode.Properties[Prop].AsTime.TimeValue = Convert.ToDateTime( NewValue );
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.List:
                                ResultNode.Properties[Prop].AsList.Value = NewValue;
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                                ResultNode.Properties[Prop].AsLogical.Checked = CswConvert.ToTristate( NewValue );
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                                ResultNode.Properties[Prop].AsMemo.Text = NewValue;
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Number:
                                ResultNode.Properties[Prop].AsNumber.Value = Convert.ToDouble( NewValue );
                                break;
                        }
                    }
                }
                ResultNode.postChanges( true );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // _Spread_UpdateCommand()



        //private class FieldTypeWebControlSpreadEditor : FarPoint.Web.Spread.Editor.IEditor
        //{
        //    #region IEditor Members

        //    private CswNbtResources _CswNbtResources;
        //    private CswFieldTypeWebControlFactory _FTWCFactory;
        //    public FieldTypeWebControlSpreadEditor( CswNbtResources Resources )
        //    {
        //        _CswNbtResources = Resources;
        //        _FTWCFactory = new CswFieldTypeWebControlFactory( Resources );
        //    }


        //    string FarPoint.Web.Spread.Editor.IEditor.EditorClientScriptUrl
        //    {
        //        get { return string.Empty; }
        //    }

        //    Control FarPoint.Web.Spread.Editor.IEditor.GetEditorControl( string id, TableCell parent, FarPoint.Web.Spread.Appearance style, FarPoint.Web.Spread.Inset margin, object value, bool upperLevel )
        //    {
        //        CswFieldTypeWebControl Control = _FTWCFactory.makeControl( parent.Controls, null, null, NodeEditMode.Edit );
        //        return Control;
        //    }

        //    object FarPoint.Web.Spread.Editor.IEditor.GetEditorValue( Control owner, string id )
        //    {
        //        throw new NotImplementedException();
        //    }

        //    BaseValidator FarPoint.Web.Spread.Editor.IEditor.GetValidator()
        //    {
        //        //throw new NotImplementedException();
        //        return null;
        //    }

        //    string FarPoint.Web.Spread.Editor.IEditor.ValidateEditorValue( object value )
        //    {
        //        //throw new NotImplementedException();
        //        return string.Empty;
        //    }

        //    #endregion
        //}
    
    } // class Act_EnterResults
} // namespace ChemSW.Nbt.WebPages
    
