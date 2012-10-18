using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using Telerik.Web.UI;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLocation : CswFieldTypeWebControl
    {
        private bool _RelationshipMode = false;
        private CswNbtNodeKey _SelectedNodeKey;

        public CswLocation( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswLocation_DataBinding );

            AllowEditValue = ( EditMode != NodeEditMode.Edit && EditMode != NodeEditMode.Demo && EditMode != NodeEditMode.PrintReport );
            _RelationshipMode = ( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) == "0" );

            EnsureChildControls();
        }

        private bool AllowEditValue = false;

        protected void CswLocation_DataBinding( object sender, EventArgs e )
        {
            try
            {
                if( Prop != null )
                {
                    if( AllowEditValue )
                    {
                        if( _RelationshipMode )
                        {
                            CswNbtView View = Prop.AsLocation.View;
                            if( View != null )
                            {
                                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );

                                // BROKEN BY case 24709
                                //string XmlStr = Tree.getTreeAsXml();
                                //_TreeView.LoadXml( XmlStr );

                                _SelectedNodeKey = Tree.getNodeKeyByNodeId( Prop.AsLocation.SelectedNodeId );
                            }
                            _TreeView.DataBound += new EventHandler( _TreeView_DataBound );
                        }
                        else
                        {
                            if( _LocationNavigator.ParentNodeId == null && _LocationNavigator.SelectedNodeId == null )
                            {
                                _LocationNavigator.PropOwnerNodeId = Prop.NodeId;
                                _LocationNavigator.ParentNodeId = Prop.AsLocation.SelectedNodeId;
                                _LocationNavigator.ParentNodeName = Prop.AsLocation.CachedNodeName;
                                _LocationNavigator.SelectedNodeId = Prop.AsLocation.SelectedNodeId;
                                _LocationNavigator.SelectedRow = Prop.AsLocation.SelectedRow;
                                _LocationNavigator.SelectedColumn = Prop.AsLocation.SelectedColumn;
                            }
                        }
                    } // if (AllowEditValue)
                    else
                    {
                        if( Prop.AsLocation.CachedNodeName != String.Empty )
                        {
                            _LocationLabel.Text = Prop.AsLocation.CachedPath;
                            if( Prop.AsLocation.SelectedColumn != Int32.MinValue && Prop.AsLocation.SelectedRow != Int32.MinValue )
                            {
                                _LocationLabel.Text += "&nbsp;(";
                                _LocationLabel.Text += CswTools.NumberToLetter( Prop.AsLocation.SelectedRow + 1 ).ToString().ToUpper();
                                _LocationLabel.Text += ( Prop.AsLocation.SelectedColumn + 1 ).ToString();
                                _LocationLabel.Text += ")";
                            }
                        }
                        else
                            _LocationLabel.Text = "Top";
                    } // if-else (AllowEditValue)
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        } // CswLocation_DataBinding()


        void _TreeView_DataBound( object sender, EventArgs e )
        {
            if( _SelectedNodeKey != null )
            {
                RadTreeNode SelectedNode = _TreeView.FindNodeByValue( _SelectedNodeKey.ToString() );
                if( SelectedNode != null )
                    SelectedNode.Selected = true;
            }
        }


        public override void Save()
        {
            if( AllowEditValue && !ReadOnly )
            {
                if( _RelationshipMode )
                {
                    bool saved = false;
                    if( _TreeView.SelectedNode != null )
                    {
                        CswNbtNodeKey SelectedNodeKey = new CswNbtNodeKey( _CswNbtResources, _TreeView.SelectedNode.Value );
                        if( SelectedNodeKey.NodeId != null && SelectedNodeKey.NodeId.PrimaryKey > 0 )
                        {
                            Prop.AsLocation.SelectedNodeId = SelectedNodeKey.NodeId;
                            //Prop.AsLocation.CachedNodeName = _TreeView.SelectedNode.Text;
                            Prop.AsLocation.RefreshNodeName();
                            saved = true;
                        }
                    }
                    if( !saved )
                    {
                        Prop.AsLocation.SelectedNodeId = null;
                        //Prop.AsLocation.CachedNodeName = String.Empty;
                        Prop.AsLocation.RefreshNodeName();
                    }
                }
                else
                {
                    Prop.AsLocation.SelectedNodeId = _LocationNavigator.SelectedNodeId;
                    //Prop.AsLocation.CachedNodeName = _LocationNavigator.SelectedNodeName;
                    Prop.AsLocation.SelectedRow = _LocationNavigator.SelectedRow;
                    Prop.AsLocation.SelectedColumn = _LocationNavigator.SelectedColumn;
                    Prop.AsLocation.RefreshNodeName();
                }
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            if( _RelationshipMode )
            {

            }
            else
            {
                _LocationNavigator.ClearSelected();
            }
        }

        private CswLocationNavigator _LocationNavigator;
        private Label _LocationLabel;
        private CswImageButton _ClearButton;
        private CswImageButton _EditButton;
        private RadTreeView _TreeView;
        private HtmlGenericControl _Div;

        protected override void CreateChildControls()
        {
            try
            {
                CswAutoTable Table = new CswAutoTable();
                this.Controls.Add( Table );

                _LocationLabel = new Label();
                _LocationLabel.ID = "loclabel";
                _LocationLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
                Table.addControl( 0, 0, _LocationLabel );

                _ClearButton = new CswImageButton( CswImageButton.ButtonType.Clear );
                _ClearButton.ID = "clear";
                //_ClearButton.Click += new ImageClickEventHandler(_ClearButton_Click);
                _ClearButton.Click += new EventHandler( _ClearButton_Click );
                Table.addControl( 0, 1, _ClearButton );

                _EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
                _EditButton.ID = "edit";
                Table.addControl( 0, 2, _EditButton );

                if( AllowEditValue )
                {
                    if( _RelationshipMode )
                    {
                        _Div = new HtmlGenericControl( "div" );
                        _Div.ID = "loctreediv";
                        _Div.Attributes.Add( "class", "loctreediv" );
                        this.Controls.Add( _Div );

                        _TreeView = new RadTreeView();
                        _TreeView.ID = "relationshiptree";
                        _TreeView.EnableEmbeddedSkins = false;
                        _TreeView.Skin = "ChemSW";
                        _Div.Controls.Add( _TreeView );
                    }
                    else
                    {
                        _LocationNavigator = new CswLocationNavigator( _CswNbtResources );//, false);
                        _LocationNavigator.OnError += new CswErrorHandler( HandleError );
                        _LocationNavigator.ID = "locnav";
                        _LocationNavigator.MoveMode = true;
                        Table.getCell( 1, 0 ).ColumnSpan = 3;
                        Table.addControl( 1, 0, _LocationNavigator );
                    }
                }
                base.CreateChildControls();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        protected void _ClearButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( AllowEditValue )
                {
                    Clear();
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
                if( Prop != null && Prop.NodeId != null )
                    _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";

                _LocationLabel.Visible = true;
                _EditButton.Visible = true;
                _ClearButton.Visible = false;
                if( AllowEditValue && !ReadOnly )
                {
                    _LocationLabel.Visible = false;
                    _EditButton.Visible = false;
                    if( _RelationshipMode )
                    {
                        _TreeView.Visible = true;
                        _Div.Visible = true;
                        _TreeView.ExpandAllNodes();   // BZ 7745
                    }
                    else
                    {
                        _LocationNavigator.Visible = true;
                        _ClearButton.Visible = true;

                        if( _LocationNavigator.SelectedNodeId != null )
                            _LocationLabel.Text = _LocationNavigator.SelectedNodeName;
                        else
                            _LocationLabel.Text = "Top";
                    }
                }
                if( ReadOnly )
                    _EditButton.Visible = false;

                _LocationLabel.Text += "&nbsp;";

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }
    }
}
