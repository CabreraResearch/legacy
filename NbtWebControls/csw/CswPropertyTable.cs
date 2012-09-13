using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.NbtWebControls.FieldTypes;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls
{
    //public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue, AuditHistoryInPopup, Preview };

    public class CswPropertyTable : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        //private CswFieldTypeWebControlFactory _CswFieldTypeWebControlFactory;
        private Dictionary<Int32, PropertyControlSet> _PropertyControlSetHash;

        public bool BatchMode = false;
        public ArrayList CheckedKeys = null;
        //private static string MultiCheckBoxPrefix = "propcheck_";
        private static string PropLabelPrefix = "pl_";

        private RadAjaxManager _AjaxManager;

        //private CswNbtNodeKey _SelectedNodeKey;
        //public CswNbtNodeKey SelectedNodeKey
        //{
        //    get { return _SelectedNodeKey; }
        //    set { _SelectedNodeKey = value; }
        //}

        private string _SelectedNodeText;
        public string SelectedNodeText
        {
            get { return _SelectedNodeText; }
            set { _SelectedNodeText = value; }
        }

        private CswNbtNode _SelectedNode;
        public CswNbtNode SelectedNode
        {
            get { return _SelectedNode; }
            set
            {
                _SelectedNode = value;
                SelectedNodeTypeId = _SelectedNode.NodeTypeId;
                SelectedNodeSpecies = _SelectedNode.NodeSpecies;
            }
        }

        private NodeSpecies _SelectedNodeSpecies;
        public NodeSpecies SelectedNodeSpecies
        {
            get { return _SelectedNodeSpecies; }
            set { _SelectedNodeSpecies = value; }
        }

        private CswNbtView _View;
        public CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
        }

        private string _SelectedTabId;
        public string SelectedTabId
        {
            get { return _SelectedTabId; }
            set
            {
                EnsureChildControls();
                _SelectedTabId = value;

                //RadTab SelectedTab = TabStrip.FindTabByValue(value);
                //if (SelectedTab != null)
                //    SelectedTab.Selected = true;
            }
        }
        private Int32 _SelectedNodeTypeId;
        public Int32 SelectedNodeTypeId
        {
            get { return _SelectedNodeTypeId; }
            set { _SelectedNodeTypeId = value; }
        }

        public NodeEditMode EditMode = NodeEditMode.Edit;


        public CswPropertyTable( CswNbtResources Rsc, RadAjaxManager AjaxManager )
        {
            finishConstructor( Rsc, AjaxManager );
        }
        public CswPropertyTable( CswNbtResources Rsc, RadAjaxManager AjaxManager, CswNbtNodeKey SelectedNodeKey, string SelectedTabId )
        {
            finishConstructor( Rsc, AjaxManager );
            _SelectedTabId = SelectedTabId;
        }

        private void finishConstructor( CswNbtResources Rsc, RadAjaxManager AjaxManager )
        {
            _CswNbtResources = Rsc;
            _AjaxManager = AjaxManager;
            _PropertyControlSetHash = new Dictionary<int, PropertyControlSet>();
        }


        private RadTabStrip _TabStrip;
        public RadTabStrip TabStrip
        {
            get
            {
                EnsureChildControls();
                return _TabStrip;
            }
        }

        //private CswAutoTable _TabTable;
        private CswLayoutTable _LayoutTable;

        private Button _SaveButton;
        public Button SaveButton
        {
            get { EnsureChildControls(); return _SaveButton; }
        }
        private Button _CancelButton;
        public Button CancelButton
        {
            get { EnsureChildControls(); return _CancelButton; }
        }
        public bool ShowCancelButton = false;

        private CswAutoTable _ButtonTable;
        private CswTabOuterTable _TabOuterTable;
        private HyperLink _NodeReportLink;
        private CswImageButton _ConfigButton;
        private CswImageButton _AddButton;

        protected override void CreateChildControls()
        {
            try
            {
                _TabStrip = new RadTabStrip();
                _TabStrip.ID = "TabStrip";
                _TabStrip.CssClass = "TabStrip";
                _TabStrip.EnableEmbeddedSkins = false;
                _TabStrip.Skin = "ChemSW";
                //_TabStrip.Width = Unit.Percentage( 97 );
                _TabStrip.OnClientTabSelecting = "CswPropertyTable_TabStrip_OnClientTabSelecting";
                this.Controls.Add( _TabStrip );

                _TabOuterTable = new CswTabOuterTable();
                _TabOuterTable.ID = "TabOuterTable";
                this.Controls.Add( _TabOuterTable );

                //_TabTable = new CswAutoTable();
                //_TabTable.ID = "TabTable";
                //_TabTable.CssClass = "TabTable";
                //_TabTable.OddCellRightAlign = true;
                //_TabOuterTable.ContentCell.Controls.Add( _TabTable );

                CswAutoTable LOTbl = new CswAutoTable();
                LOTbl.Width = Unit.Parse( "100%" );
                _TabOuterTable.ContentCell.Controls.Add( LOTbl );

                _LayoutTable = new CswLayoutTable( _CswNbtResources, _AjaxManager );
                _LayoutTable.OnError += new CswErrorHandler( HandleError );
                _LayoutTable.OnMoveComponent += new CswLayoutTable.MoveComponentEventHandler( _LayoutTable_OnMoveComponent );
                _LayoutTable.OnDeleteComponent += new CswLayoutTable.DeleteComponentEventHandler( _LayoutTable_OnDeleteComponent );
                _LayoutTable.CssClassLabelCell = "propertylabel";
                _LayoutTable.CssClassValueCell = "propertyvaluecell";
                _LayoutTable.CssClassValueCellEditMode = "ComponentValueCellEditMode";
                _LayoutTable.TableTextAlign = "left";
                //_LayoutTable.EditMode = true;
                //_TabOuterTable.ContentCell.Controls.Add( _LayoutTable );
                LOTbl.addControl( 0, 0, _LayoutTable );

                CswAutoTable ConfigTable = new CswAutoTable();
                LOTbl.addControl( 0, 1, ConfigTable );
                LOTbl.getCell( 0, 0 ).Style.Add( HtmlTextWriterStyle.TextAlign, "right" );

                _ConfigButton = new CswImageButton( CswImageButton.ButtonType.Configure );
                _ConfigButton.Click += new EventHandler( _ConfigButton_Click );
                ConfigTable.addControl( 0, 0, _ConfigButton );

                _AddButton = new CswImageButton( CswImageButton.ButtonType.Add );
                ConfigTable.addControl( 0, 1, _AddButton );

                _TabOuterTable.ContentCell.Controls.Add( new CswLiteralBr() );

                _ButtonTable = new CswAutoTable();
                _ButtonTable.ID = "ButtonTable";
                _TabOuterTable.ContentCell.Controls.Add( _ButtonTable );

                _SaveButton = new Button();
                _SaveButton.ID = "savebutton";
                _SaveButton.Text = "Save Changes";
                _SaveButton.CssClass = "Button";
                //_SaveButton.Enabled = false;
                _SaveButton.UseSubmitBehavior = true;
                _SaveButton.Click += new EventHandler( SaveButton_Click );
                _SaveButton.OnClientClick = "if(!CswPropertyTable_SaveButton_PreClick()) return false;";
                _SaveButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                _ButtonTable.addControl( 0, 0, _SaveButton );

                _CancelButton = new Button();
                _CancelButton.ID = "cancelbutton";
                _CancelButton.Text = "Cancel";
                _CancelButton.CssClass = "Button";
                _CancelButton.UseSubmitBehavior = true;
                _CancelButton.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                _ButtonTable.addControl( 0, 1, _CancelButton );


                HtmlGenericControl NodeReportLinkDiv = new HtmlGenericControl( "div" );
                NodeReportLinkDiv.Attributes.Add( "class", "NodeReportLinkDiv" );
                this.Controls.Add( NodeReportLinkDiv );

                _NodeReportLink = new HyperLink();
                _NodeReportLink.ID = "NodeReportLink";
                _NodeReportLink.CssClass = "NodeReportLink";
                _NodeReportLink.Text = "As Report";
                _NodeReportLink.Visible = false;
                NodeReportLinkDiv.Controls.Add( _NodeReportLink );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.CreateChildControls();
        }

        protected void _LayoutTable_OnDeleteComponent( Int32 LayoutComponentId )
        {
            try
            {
                if( !_CswNbtResources.Permit.can( CswNbtActionName.Design ) )
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to edit the tab layout", "User (" + _CswNbtResources.CurrentNbtUser.Username + ") does not have Design Action permissions" );

                // LayoutComponentId == PropId (set in addPropertyToTable below)
                CswNbtMetaDataNodeTypeProp DoomedProp = _CswNbtResources.MetaData.getNodeTypeProp( LayoutComponentId );
                _CswNbtResources.MetaData.DeleteNodeTypeProp( DoomedProp );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }


        }

        protected void _LayoutTable_OnMoveComponent( Int32 LayoutComponentId, Int32 NewDisplayRow, Int32 NewDisplayColumn )
        {
            try
            {
                if( !_CswNbtResources.Permit.can( CswNbtActionName.Design ) )
                    throw new CswDniException( ErrorType.Warning, "You do not have permission to edit the tab layout", "User (" + _CswNbtResources.CurrentNbtUser.Username + ") does not have Design Action permissions" );

                // LayoutComponentId == PropId (set in addPropertyToTable below)
                CswNbtMetaDataNodeTypeProp MovedProp = _CswNbtResources.MetaData.getNodeTypeProp( LayoutComponentId );
                if( EditMode == NodeEditMode.Add )
                {
                    //MovedProp.DisplayRowAdd = NewDisplayRow;
                    //MovedProp.DisplayColAdd = NewDisplayColumn;
                    MovedProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false, Int32.MinValue, NewDisplayRow, NewDisplayColumn );
                }
                else
                {
                    //MovedProp.DisplayRow = NewDisplayRow;
                    //MovedProp.DisplayColumn = NewDisplayColumn;
                    MovedProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, Int32.MinValue, NewDisplayRow, NewDisplayColumn );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {

                // Hide controls whose properties have a conditional filter
                //if( _SelectedNodeKey != null )
                //{
                //CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( _SelectedNodeKey.NodeTypeId );
                if( EditMode != NodeEditMode.Demo )
                {
                    CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                    if( MetaDataNodeType != null )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in MetaDataNodeType.getNodeTypeProps() )
                        {
                            if( MetaDataProp.hasFilter() )
                            {
                                if( !_CheckFilter( MetaDataProp ) )
                                {
                                    PropertyControlSet PropCS = null;
                                    if( _PropertyControlSetHash.ContainsKey( MetaDataProp.FirstPropVersionId ) )
                                        PropCS = _PropertyControlSetHash[MetaDataProp.FirstPropVersionId];
                                    if( PropCS != null )
                                    {
                                        if( !_LayoutTable.EditMode && PropCS.Label != null )
                                            PropCS.Label.ShowLabel = false;
                                        PropCS.Control.Style.Add( HtmlTextWriterStyle.Display, "none" );
                                        PropCS.Control.Clear();
                                    }
                                } // if( !_CheckFilter( MetaDataProp ) )
                            } // if( MetaDataProp.hasFilter() )
                        } // foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in MetaDataNodeType.NodeTypeProps )
                    } // if( MetaDataNodeType != null )
                } // if( !( EditMode == NodeEditMode.Demo || _LayoutTable.EditMode ) )

                if( ShowCancelButton )
                    _CancelButton.Visible = true;
                else
                    _CancelButton.Visible = false;

                if( _CswNbtResources.Permit.can( CswNbtActionName.Design ) )
                {
                    _ConfigButton.Visible = true;
                    _AddButton.Visible = true;
                }
                else
                {
                    _ConfigButton.Visible = false;
                    _AddButton.Visible = false;
                }

                RadTab SelectedTab = TabStrip.FindTabByValue( SelectedTabId );
                if( SelectedTab != null )
                {
                    SelectedTab.Selected = true;
                    _AddButton.Visible = true;
                    _AddButton.OnClientClick = "openDesignAddPopup('" + CswNodeTypeTree.NodeTypeTreeSelectedType.Property + "','" + CswNodeTypeTree.NodeTypeTreeSelectedType.Tab + "','" + SelectedTabId + "', '');";
                }
                else
                {
                    _AddButton.Visible = false;
                }

                if( SelectedNode != null && _NodeReportLink != null &&
                    SelectedNode.NodeSpecies == NodeSpecies.Plain &&
                    SelectedNode.NodeId != null && SelectedNode.NodeId.PrimaryKey != Int32.MinValue )
                {
                    _NodeReportLink.NavigateUrl = "NodeReport.aspx?nodeid=" + SelectedNode.NodeId.ToString();
                    _NodeReportLink.Target = "_blank";
                    _NodeReportLink.Visible = true;
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        } //OnPreRender()


        protected void _ConfigButton_Click( object sender, EventArgs e )
        {
            _setEditMode( _LayoutTable, !_LayoutTable.EditMode );

            foreach( CswLayoutTable SubLayoutTable in PropTables.Values )
            {
                _setEditMode( SubLayoutTable, _LayoutTable.EditMode );
            }
        } // _ConfigButton_Click

        private void _setEditMode( CswLayoutTable LTbl, bool NewEditMode )
        {
            LTbl.EditMode = NewEditMode;
            foreach( CswLayoutTable.LayoutComponent Comp in LTbl.Components.Values )
            {
                ( (CswPropertyTableLabel) Comp.LabelControl ).EditMode = NewEditMode;
            }
            LTbl.ReinitComponents();
        }



        // This is similar, but NOT redundant with CswNbtMetaDataNodeTypeProp.CheckFilter()
        private bool _CheckFilter( CswNbtMetaDataNodeTypeProp MetaDataProp )
        {
            bool FilterMatches = false;
            CswNbtMetaDataNodeTypeProp FilterMetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( MetaDataProp.FilterNodeTypePropId );

            CswNbtSubField SubField = FilterMetaDataProp.getFieldTypeRule().SubFields.Default;
            CswNbtPropFilterSql.PropertyFilterMode FilterMode = SubField.DefaultFilterMode;
            string FilterValue = null;
            MetaDataProp.getFilter( ref SubField, ref FilterMode, ref FilterValue );

            if( _PropertyControlSetHash.ContainsKey( FilterMetaDataProp.FirstPropVersionId ) )
            {
                CswFieldTypeWebControl FilterControl = _PropertyControlSetHash[FilterMetaDataProp.FirstPropVersionId].Control;
                CswNbtNode Node = _CswNbtResources.Nodes[FilterControl.Prop.NodeId];
                if( Node != null )
                {
                    CswNbtNodePropWrapper FilterProp = Node.Properties[FilterMetaDataProp];

                    // We compare to FilterControl rather than FilterProp so that 
                    // we are using the unsaved form contents to decide, rather than the DB value.

                    // Logical needs a special case
                    if( FilterMetaDataProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical )
                    {
                        if( SubField.Name == CswNbtSubField.SubFieldName.Checked && FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                        {
                            //if( FilterValue == "1" || FilterValue.ToLower() == "true" )
                            //    FilterMatches = ( ( (CswLogical) FilterControl ).Checked == Tristate.True );
                            //else if( FilterValue == "0" || FilterValue.ToLower() == "false" )
                            //    FilterMatches = ( ( (CswLogical) FilterControl ).Checked == Tristate.False );
                            //else
                            //    FilterMatches = ( ( (CswLogical) FilterControl ).Checked == Tristate.Null );
                            FilterMatches = ( CswConvert.ToTristate( FilterValue ) == ( (CswLogical) FilterControl ).Checked );
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswPropertyTable only supports 'Checked Equals' filters on Logical properties" );
                        }
                    }
                    else
                    {
                        string ValueToCompare = string.Empty;

                        switch( FilterMetaDataProp.getFieldType().FieldType )
                        {
                            case CswNbtMetaDataFieldType.NbtFieldType.List:
                                ValueToCompare = ( (CswList) FilterControl ).SelectedValue;
                                break;
                            case CswNbtMetaDataFieldType.NbtFieldType.Text:
                                ValueToCompare = ( (CswText) FilterControl ).Text;
                                break;
                            default:
                                throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswPropertyTable does not support field type: " + FilterMetaDataProp.getFieldType().FieldType.ToString() );
                        } // switch( FilterMetaDataProp.FieldType.FieldType )

                        if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                        {
                            FilterMatches = ( ValueToCompare.ToLower() == FilterValue.ToLower() );
                        }
                        else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                        {
                            FilterMatches = ( ValueToCompare.ToLower() != FilterValue.ToLower() );
                        }
                        else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Null )
                        {
                            FilterMatches = ( ValueToCompare == string.Empty );
                        }
                        else if( FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotNull )
                        {
                            FilterMatches = ( ValueToCompare != string.Empty );
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "Invalid filter condition", "CswPropertyTable does not support filter mode){ " + FilterMode.ToString() );
                        }

                    } // if-else( FilterMetaDataProp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical )
                } // if( Node != null )
            }  // if( _PropertyControlSetHash[FilterMetaDataProp.FirstPropVersionId] != null )
            return FilterMatches;

        } // _CheckFilter()


        /// <summary>
        /// Initializes tab strip for current selected node
        /// </summary>
        public void initTabStrip()
        {
            EnsureChildControls();
            TabStrip.Tabs.Clear();

            if( EditMode == NodeEditMode.Add && SelectedNodeTypeId > 0 )
            {
                // Adding new node in popup - Single Placeholder Tab
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                RadTab Tab = new RadTab();
                Tab.Text = "Add New " + NodeType.NodeTypeName;
                Tab.Value = "0";
                Tab.CssClass = "Tab";
                TabStrip.Tabs.Add( Tab );
                Tab.Selected = true;
            }
            else if( //( SelectedNodeKey != null && SelectedNodeKey.NodeSpecies == NodeSpecies.Plain ) ||
                     ( SelectedNodeSpecies == NodeSpecies.Plain ) ||
                     ( SelectedNodeTypeId > 0 && EditMode == NodeEditMode.Demo ) )
            {
                CswNbtMetaDataNodeType NodeType = null;
                //if( SelectedNodeKey != null )
                //    NodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeKey.NodeTypeId );
                //else
                NodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );

                if( NodeType != null )
                {
                    foreach( CswNbtMetaDataNodeTypeTab TabDef in NodeType.getNodeTypeTabs() )
                    {
                        RadTab Tab = new RadTab();
                        Tab.Text = TabDef.TabName;
                        Tab.Value = TabDef.TabId.ToString();
                        Tab.CssClass = "Tab";
                        TabStrip.Tabs.Add( Tab );
                    }

                    if( SelectedTabId != string.Empty )
                    {
                        RadTab STab = TabStrip.FindTabByValue( SelectedTabId );
                        if( STab != null )
                            STab.Selected = true;
                    }

                    if( TabStrip.SelectedTab == null )
                    {
                        //first tab
                        TabStrip.Tabs[0].Selected = true;
                    }
                    SelectedTabId = TabStrip.SelectedTab.Value;
                }
            }
            else if//( SelectedNodeKey != null && SelectedNodeKey.NodeSpecies == NodeSpecies.Root )
                   ( SelectedNodeSpecies == NodeSpecies.Root )
            {
                // Root - Single Placeholder Tab
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                RadTab Tab = new RadTab();
                Tab.Text = SelectedNodeText;
                Tab.Value = "0";
                Tab.CssClass = "Tab";
                TabStrip.Tabs.Add( Tab );
                Tab.Selected = true;
            }

            //if(_TabStrip.Tabs.Count > 6)
            //    _TabStrip.Width = Unit.Parse( ( _TabStrip.Tabs.Count * 100 ) + "px" );

        } // initTabStrip()


        //private TableCell SearchCell;
        //private TableCell AddCell;
        private Dictionary<CswNbtMetaDataNodeTypeProp, CswLayoutTable> PropTables = null;
        /// <summary>
        /// Initializes tab contents for the current selected tab
        /// </summary>
        /// <returns>A modified SelectedTabId (if the current selected one was invalid)</returns>
        public string initTabContents()
        {
            EnsureChildControls();
            //_TabTable.Controls.Clear();
            _LayoutTable.Clear();
            PropTables = new Dictionary<CswNbtMetaDataNodeTypeProp, CswLayoutTable>();

            // case 20692
            if( SelectedNode != null && SelectedNode.NodeSpecies == NodeSpecies.Plain && SelectedNode.NodeId != null )
            {
                CswNbtActUpdatePropertyValue ActUPV = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                ActUPV.UpdateNode( SelectedNode, true );
                SelectedNode.postChanges( false );
            }

            if( ( EditMode != NodeEditMode.Add && SelectedNodeSpecies == NodeSpecies.Plain ) ||
                ( EditMode == NodeEditMode.Demo && SelectedNodeTypeId > 0 ) )
            {
                CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );

                // If any property uses numbering, align left
                bool OddCellRightAlign = true;
                foreach( CswNbtMetaDataNodeTypeProp Prop in MetaDataNodeType.getNodeTypeProps() )
                {
                    if( Prop.UseNumbering )
                    {
                        OddCellRightAlign = false;
                        break;
                    }
                }
                //_TabTable.OddCellRightAlign = OddCellRightAlign;
                _LayoutTable.LabelCellRightAlign = OddCellRightAlign;

                if( SelectedTabId == null || SelectedTabId == string.Empty )
                {
                    // get first tab for node
                    SelectedTabId = MetaDataNodeType.getFirstNodeTypeTab().TabId.ToString();
                }

                // Non-conditionals first
                foreach( CswNbtMetaDataNodeTypeProp Prop in MetaDataNodeType.getNodeTypeProps() )
                {
                    if( Prop.FirstEditLayout.TabId != Int32.MinValue && Prop.FirstEditLayout.TabId.ToString() == SelectedTabId.ToString() )
                    {
                        if( !Prop.hasFilter() )
                        {
                            PropertyControlSet PCS = addPropertyToTable( _CswNbtResources, _LayoutTable, Prop, SelectedNode, false, BatchMode, EditMode, HandleError );
                            if( _PropertyControlSetHash.ContainsKey( Prop.FirstPropVersionId ) )
                                _PropertyControlSetHash[Prop.FirstPropVersionId] = PCS;
                            else
                                _PropertyControlSetHash.Add( Prop.FirstPropVersionId, PCS );
                        }
                    }
                }

                // Conditionals second
                foreach( CswNbtMetaDataNodeTypeProp Prop in MetaDataNodeType.getNodeTypeProps() )
                {
                    if( Prop.FirstEditLayout.TabId != Int32.MinValue && Prop.FirstEditLayout.TabId.ToString() == SelectedTabId.ToString() )
                    {
                        // Conditional Filter on Properties
                        if( Prop.hasFilter() )
                        {
                            // BZ 7939 - FilterNodeTypePropId is a property FirstVersionId, so we have to fetch it
                            CswNbtMetaDataNodeTypeProp ParentProp = _CswNbtResources.MetaData.getNodeTypeProp( Prop.FilterNodeTypePropId );

                            if( _PropertyControlSetHash.ContainsKey( ParentProp.FirstPropVersionId ) )
                            {// The parent needs to use postback
                                switch( ParentProp.getFieldType().FieldType )
                                {
                                    case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                                        ( (CswLogical) _PropertyControlSetHash[ParentProp.FirstPropVersionId].Control ).AutoPostBack = true;
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.List:
                                        ( (CswList) _PropertyControlSetHash[ParentProp.FirstPropVersionId].Control ).AutoPostBack = true;
                                        break;
                                    case CswNbtMetaDataFieldType.NbtFieldType.Text:
                                        ( (CswText) _PropertyControlSetHash[ParentProp.FirstPropVersionId].Control ).AutoPostBack = true;
                                        break;
                                }
                            }

                            if( !PropTables.ContainsKey( ParentProp ) )
                            {
                                CswLayoutTable SubLayoutTable = new CswLayoutTable( _CswNbtResources, _AjaxManager );
                                SubLayoutTable.ID = "proptable_" + ParentProp.PropId;
                                SubLayoutTable.OnError += new CswErrorHandler( HandleError );
                                SubLayoutTable.OnMoveComponent += new CswLayoutTable.MoveComponentEventHandler( _LayoutTable_OnMoveComponent );
                                SubLayoutTable.OnDeleteComponent += new CswLayoutTable.DeleteComponentEventHandler( _LayoutTable_OnDeleteComponent );
                                SubLayoutTable.LabelCellRightAlign = OddCellRightAlign;
                                SubLayoutTable.EditMode = _LayoutTable.EditMode;
                                if( _PropertyControlSetHash.ContainsKey( ParentProp.FirstPropVersionId ) )
                                {
                                    PropertyControlSet ParentPCS = _PropertyControlSetHash[ParentProp.FirstPropVersionId];
                                    ParentPCS.Control.Controls.Add( SubLayoutTable );
                                }
                                PropTables.Add( ParentProp, SubLayoutTable );
                            }

                            PropertyControlSet SubPCS = addPropertyToTable( _CswNbtResources, (CswLayoutTable) PropTables[ParentProp], Prop, SelectedNode, false, BatchMode, EditMode, HandleError );
                            if( _PropertyControlSetHash.ContainsKey( Prop.FirstPropVersionId ) )
                                _PropertyControlSetHash[Prop.FirstPropVersionId] = SubPCS;
                            else
                                _PropertyControlSetHash.Add( Prop.FirstPropVersionId, SubPCS );

                        } // if( Prop.hasFilter() )
                    } // if( Prop.NodeTypeTab != null && Prop.NodeTypeTab.TabId.ToString() == SelectedTabId.ToString() )
                } // foreach( CswNbtMetaDataNodeTypeProp Prop in MetaDataNodeType.NodeTypeProps )

                if( !_CswNbtResources.Permit.canNode( CswNbtPermit.NodeTypePermission.Edit, _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId ), SelectedNode.NodeId ) )
                {
                    SaveButton.Visible = false;
                }



            }
            else if( EditMode == NodeEditMode.Add && _SelectedNodeTypeId > 0 )
            {
                CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( SelectedNodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp Prop in MetaDataNodeType.getNodeTypeProps() )
                {
                    if( ( ( Prop.IsRequired && Prop.DefaultValue.Empty ) || SelectedNode.Properties[Prop].TemporarilyRequired || Prop.AddLayout != null ) && Prop.FilterNodeTypePropId == Int32.MinValue )
                    {
                        PropertyControlSet PCS = addPropertyToTable( _CswNbtResources, _LayoutTable, Prop, SelectedNode, true, BatchMode, EditMode, HandleError );
                        if( _PropertyControlSetHash.ContainsKey( Prop.FirstPropVersionId ) )
                            _PropertyControlSetHash[Prop.FirstPropVersionId] = PCS;
                        else
                            _PropertyControlSetHash.Add( Prop.FirstPropVersionId, PCS );
                    }
                }
                if( !_CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, MetaDataNodeType ) )
                {
                    SaveButton.Visible = false;
                }
            }
            else if( SelectedNodeSpecies == NodeSpecies.Root )
            {
                if( _View.ViewMode == NbtViewRenderingMode.Tree )
                {
                    _TabOuterTable.ContentCell.Controls.Clear();
                    _TabOuterTable.ContentCell.Controls.Add( new CswLiteralText( "Please select what you want to edit from the tree." ) );
                }
                else if( _View.ViewMode == NbtViewRenderingMode.List )
                {
                    _TabOuterTable.ContentCell.Controls.Clear();
                    _TabOuterTable.ContentCell.Controls.Add( new CswLiteralText( "Please select what you want to edit from the list." ) );
                }
                SaveButton.Visible = false;
            }
            else
            {
                SaveButton.Visible = false;
            }

            _LayoutTable.ReinitComponents();
            foreach( CswLayoutTable SubLayoutTable in PropTables.Values )
                SubLayoutTable.ReinitComponents();

            return SelectedTabId;
        }

        /// <summary>
        /// Adds a property to a CswAutoTable in the correct location.  
        /// </summary>
        /// <remarks>This function is public static for visibility to NodeReport.aspx</remarks>
        public static PropertyControlSet addPropertyToTable( CswNbtResources CswNbtResources,
                                                             CswLayoutTable LayoutTable,
                                                             CswNbtMetaDataNodeTypeProp MetaDataProp,
                                                             CswNbtNodePropWrapper PropWrapper,
                                                             bool IsAddForm,
                                                             bool BatchMode,
                                                             NodeEditMode EditMode,
                                                             CswErrorHandler HandleError )
        {
            return _addPropertyToTable( CswNbtResources, LayoutTable, MetaDataProp, null, PropWrapper, IsAddForm, BatchMode, EditMode, HandleError );
        }

        /// <summary>
        /// Adds a property to a CswAutoTable in the correct location.  
        /// </summary>
        /// <remarks>This function is public static for visibility to NodeReport.aspx</remarks>
        public static PropertyControlSet addPropertyToTable( CswNbtResources CswNbtResources,
                                                             CswLayoutTable LayoutTable,
                                                             CswNbtMetaDataNodeTypeProp MetaDataProp,
                                                             CswNbtNode Node,
                                                             bool IsAddForm,
                                                             bool BatchMode,
                                                             NodeEditMode EditMode,
                                                             CswErrorHandler HandleError )
        {
            CswNbtNodePropWrapper CswPropWrapper = null;
            if( Node != null )
                CswPropWrapper = Node.Properties[MetaDataProp];

            return _addPropertyToTable( CswNbtResources, LayoutTable, MetaDataProp, Node, CswPropWrapper, IsAddForm, BatchMode, EditMode, HandleError );
        }


        private static PropertyControlSet _addPropertyToTable( CswNbtResources CswNbtResources,
                                                               CswLayoutTable LayoutTable,
                                                               CswNbtMetaDataNodeTypeProp MetaDataProp,
                                                               CswNbtNode Node,
                                                               CswNbtNodePropWrapper PropWrapper,
                                                               bool IsAddForm,
                                                               bool BatchMode,
                                                               NodeEditMode EditMode,
                                                               CswErrorHandler HandleError )
        {
            PropertyControlSet ret = null;
            if( PropWrapper == null || !PropWrapper.Hidden || EditMode == NodeEditMode.Demo )
            {
                CswPropertyTableLabel PropLabel = null;
                //if( MetaDataProp.ShowLabel )
                //{
                string PropName = string.Empty;
                if( MetaDataProp.UseNumbering )
                    PropName = MetaDataProp.PropNameWithQuestionNo;
                else
                    PropName = MetaDataProp.PropName;

                PropLabel = new CswPropertyTableLabel();
                PropLabel.ShowLabel = MetaDataProp.ShowLabel;
                PropLabel.LabelText = PropName;
                PropLabel.ToolTipText = MetaDataProp.HelpText;
                PropLabel.EnableCheckbox = ( BatchMode && MetaDataProp.IsCopyable() );
                PropLabel.ID = PropLabelPrefix + MetaDataProp.PropId.ToString();
                PropLabel.CheckboxOnClientClick = "PropTable_MultiEditCheck_Click();";
                //}

                CswFieldTypeWebControl PropControl = null;
                if( Node != null )
                {
                    PropControl = CswFieldTypeWebControlFactory.makeControl( CswNbtResources, LayoutTable.Controls, string.Empty, MetaDataProp, Node, EditMode, HandleError );
                }
                else if( PropWrapper != null )
                {
                    PropControl = CswFieldTypeWebControlFactory.makeControl( CswNbtResources, LayoutTable.Controls, string.Empty, PropWrapper, EditMode, HandleError );
                }
                else
                    throw new CswDniException( ErrorType.Error, "Invalid Property", "CswPropertyTable.addPropertyToTable requires either a valid NodeKey or a valid PropWrapper" );

                CswLayoutTable.LayoutComponent ThisComponent = null;
                if( EditMode == NodeEditMode.Add )
                    ThisComponent = new CswLayoutTable.LayoutComponent( MetaDataProp.PropId, MetaDataProp.AddLayout.DisplayRow, MetaDataProp.AddLayout.DisplayColumn, PropLabel, PropControl, false ); //MetaDataProp.IsDeletable() );
                else
                    ThisComponent = new CswLayoutTable.LayoutComponent( MetaDataProp.PropId, MetaDataProp.FirstEditLayout.DisplayRow, MetaDataProp.FirstEditLayout.DisplayColumn, PropLabel, PropControl, false ); //MetaDataProp.IsDeletable() );
                LayoutTable.Components.Add( MetaDataProp.PropId, ThisComponent );

                if( PropControl != null )
                    ( (System.Web.UI.WebControls.WebControl) PropControl ).DataBind();

                ret = new PropertyControlSet( PropLabel, PropControl );
            }
            return ret;
        }

        private static Int32 _getRealRowIndex( Int32 FakeRowNum )
        {
            Int32 ret = FakeRowNum;
            if( ret < 1 ) ret = 1;
            return ret - 1;

        }
        private static Int32 _getRealColumnIndex( Int32 FakeColumnNum )
        {
            Int32 ret = FakeColumnNum;
            if( ret < 1 ) ret = 1;
            ret = ( ret * 2 ) - 1;
            return ret;
        }

        public class SaveHandlerEventArgs : EventArgs
        {
            public CswNbtNode Node = null;
            public SaveHandlerEventArgs( CswNbtNode NodeSaved )
            {
                Node = NodeSaved;
            }
        }
        public delegate void SaveHandler( object sender, SaveHandlerEventArgs e );
        public event SaveHandler OnSave;

        void SaveButton_Click( object sender, EventArgs e )
        {
            try
            {
                //CswNbtNode SelectedNode = null;

                //if( _SelectedNodeKey == null )
                //{
                //    // New node?
                //}

                if( SelectedNode != null )
                {
                    // It's important that this happens before the SaveButtonClick event.
                    if( Page.IsValid && this.HasControls() )
                    {
                        SaveFieldTypeWebControls( this.Controls );

                        //Master.CswNbtResources.Nodes.Write(Master.CswNbtResources.Nodes[SelectedNodeKey]);
                        //SelectedNode = _CswNbtResources.Nodes[_SelectedNodeKey];
                        SelectedNode.postChanges( true );

                        if( BatchMode && CheckedKeys != null && CheckedKeys.Count > 0 )
                        {
                            CopyCheckedProperties( SelectedNode, this.Controls );
                            foreach( CswNbtNodeKey CheckedNodeKey in CheckedKeys )
                            {
                                CswNbtNode ThisNode = _CswNbtResources.Nodes.GetNode( CheckedNodeKey.NodeId );
                                ThisNode.postChanges( true );
                            }
                        }

                        AfterSaveFieldTypeWebControls( this.Controls );
                    }
                }

                // Commit any transactions
                _CswNbtResources.finalize();

                if( OnSave != null )
                    OnSave( sender, new SaveHandlerEventArgs( SelectedNode ) );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }//SaveButton_Click()

        public static void SaveFieldTypeWebControls( ControlCollection Controls )
        {
            foreach( System.Web.UI.Control thisControl in Controls )
            {
                if( thisControl is CswFieldTypeWebControl )
                    ( (CswFieldTypeWebControl) thisControl ).Save();

                if( thisControl.Controls.Count > 0 )
                    SaveFieldTypeWebControls( thisControl.Controls );
            }


        }
        public static void AfterSaveFieldTypeWebControls( ControlCollection Controls )
        {
            foreach( System.Web.UI.Control thisControl in Controls )
            {
                if( thisControl is CswFieldTypeWebControl )
                {
                    // We need to update readonly controls here too!  BZ 6122
                    //if (!((ICswFieldTypeWebControl)thisControl).ReadOnly)
                    ( (CswFieldTypeWebControl) thisControl ).AfterSave();
                }

                if( thisControl.Controls.Count > 0 )
                    AfterSaveFieldTypeWebControls( thisControl.Controls );
            }
        }

        public void CopyCheckedProperties( CswNbtNode SourceNode, ControlCollection Controls )
        {
            foreach( System.Web.UI.Control thisControl in Controls )
            {
                //if( thisControl is CheckBox &&
                //    thisControl.ID.Length > MultiCheckBoxPrefix.Length &&
                //    thisControl.ID.Substring( 0, MultiCheckBoxPrefix.Length ) == MultiCheckBoxPrefix &&
                //    ( (CheckBox) thisControl ).Checked )
                if( thisControl is CswPropertyTableLabel && ( (CswPropertyTableLabel) thisControl ).Checked )
                {
                    // Apply checked property value to checked nodes
                    Int32 ThisPropId = CswConvert.ToInt32( thisControl.ID.Substring( PropLabelPrefix.Length ) );
                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( ThisPropId );
                    foreach( CswNbtNodeKey CheckedNodeKey in CheckedKeys )
                    {
                        CswNbtNode ThisNode = _CswNbtResources.Nodes.GetNode( CheckedNodeKey.NodeId );
                        if( ThisNode != SourceNode && ThisNode.NodeTypeId == SourceNode.NodeTypeId )
                            ThisNode.Properties[ThisProp].copy( SourceNode.Properties[ThisProp] );
                    }
                }

                if( thisControl.Controls.Count > 0 )
                    CopyCheckedProperties( SourceNode, thisControl.Controls );
            }
        }

        // Error handling
        //public delegate void ErrorHandler( Exception ex );
        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }

        public class PropertyControlSet
        {
            public CswPropertyTableLabel Label;
            public CswFieldTypeWebControl Control;
            public PropertyControlSet( CswPropertyTableLabel InLabel, CswFieldTypeWebControl InControl )
            {
                Label = InLabel;
                Control = InControl;
            }
        }

        public CswFieldTypeWebControl GetFieldTypeWebControl( CswNbtMetaDataNodeTypeProp MetaDataNodeTypeProp )
        {
            return ( (PropertyControlSet) _PropertyControlSetHash[MetaDataNodeTypeProp.FirstPropVersionId] ).Control;
        }
    } // class CswPropertyTable
}
