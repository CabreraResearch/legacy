using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using Telerik.Web.UI;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswRelationship : CswFieldTypeWebControl, INamingContainer
    {
        //private bool AllowEditValue = false;

        //private CswNbtNodeKey _SelectedNodeKey;

        public CswRelationship( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            //AllowEditValue = ( EditMode != NodeEditMode.Edit && EditMode != NodeEditMode.Demo && EditMode != NodeEditMode.PrintReport && EditMode != NodeEditMode.LowRes );
            this.DataBinding += new EventHandler( CswRelationship_DataBinding );
        }

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnInit( e );
        }

        private void CswRelationship_DataBinding( object sender, EventArgs e )
        {
            try
            {
                EnsureChildControls();
                //CswNbtNode Node = _CswNbtResources.Nodes[this.NodeKey];
                //Prop = Node.Properties[_CswNbtMetaDataNodeTypeProp];
                if( Prop != null )
                {
                    _ValueLabel.Text = Prop.AsRelationship.CachedNodeName + "&nbsp;";

                    // BZ 10216
                    if( Prop.AsRelationship.TargetType == CswNbtViewRelationship.RelatedIdType.NodeTypeId && !ReadOnly )
                        ReadOnly = !(_CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, Prop.AsRelationship.TargetId, null, null ));

                    //if( AllowEditValue && !ReadOnly )
                    //{
                    //    CswNbtView View = Prop.AsRelationship.View;
                    //    if( View != null )
                    //    {
                    //        // Filter out my node
                    //        if( Prop.NodeId != null )
                    //        {
                    //            foreach( CswNbtViewRelationship R in View.Root.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
                    //                R.NodeIdsToFilterOut.Add( Prop.NodeId );
                    //        }
                    //        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );
                    //        string XmlStr = Tree.getTreeAsXml();
                    //        _TreeView.LoadXml( XmlStr );

                    //        _SelectedNodeKey = Tree.getNodeKeyByNodeId( Prop.AsRelationship.RelatedNodeId );
                    //    }
                    //    _TreeView.DataBound += new EventHandler( _TreeView_DataBound );
                    //}
                    CswNbtView View = Prop.AsRelationship.View;
                    if( View != null )
                    {
                        _ValueList.Items.Clear();
                        if( !Required )
                            _ValueList.Items.Add( new ListItem( "" ) );
                        ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );
                        for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
                        {
                            CswNbtTree.goToNthChild( c );
                            _ValueList.Items.Add( new ListItem( CswNbtTree.getNodeNameForCurrentPosition(), CswNbtTree.getNodeIdForCurrentPosition().ToString() ) );
                            if( Prop.AsRelationship.RelatedNodeId == CswNbtTree.getNodeIdForCurrentPosition() )
                                _ValueList.SelectedValue = Prop.AsRelationship.RelatedNodeId.ToString();
                            CswNbtTree.goToParentNode();
                        } // for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )

                    } // if( View != null )
                } // if( Prop != null )
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        } // CswRelationship_DataBinding()

        //void _TreeView_DataBound( object sender, EventArgs e )
        //{
        //    try
        //    {
        //        if( _SelectedNodeKey != null )
        //        {
        //            RadTreeNode SelectedNode = _TreeView.FindNodeByValue( _SelectedNodeKey.ToString() );
        //            if( SelectedNode != null )
        //                SelectedNode.Selected = true;
        //        }
        //    }
        //    catch( Exception ex )
        //    {
        //        HandleError( ex );
        //    }
        //}

        public override void Save()
        {
            try
            {
                if( !ReadOnly )
                {
                    //if( _hiddenClear.Value == "1" )
                    //{
                    //    Prop.ClearValue();
                    //}
                    //else
                    //{
                    //if( AllowEditValue && !ReadOnly )
                    //if( !ReadOnly )
                    //{
                    //bool saved = false;
                    //if( _TreeView.SelectedNode != null )
                    //{
                    //    CswNbtNodeKey SelectedNodeKey = new CswNbtNodeKey( _CswNbtResources, _TreeView.SelectedNode.Value );
                    //    if( SelectedNodeKey.NodeId != null )
                    //    {
                    //        Prop.AsRelationship.RelatedNodeId = SelectedNodeKey.NodeId;
                    //        Prop.AsRelationship.CachedNodeName = _TreeView.SelectedNode.Text;
                    //        saved = true;
                    //    }
                    //}
                    //if( !saved )
                    //{
                    //    Prop.AsRelationship.RelatedNodeId = null;
                    //    Prop.AsRelationship.CachedNodeName = String.Empty;
                    //}
                    if( _ValueList.SelectedValue != string.Empty )
                    {
                        Prop.AsRelationship.RelatedNodeId = SelectedNodeId;
                        Prop.AsRelationship.CachedNodeName = SelectedNodeName;
                    }
                    else
                    {
                        Prop.AsRelationship.RelatedNodeId = null;
                        Prop.AsRelationship.CachedNodeName = String.Empty;
                    }
                    //}
                    //}
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            try
            {
                _ValueLabel.Text = string.Empty;
                _ValueList.SelectedValue = null;
                _ValueList.Text = string.Empty;
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public CswPrimaryKey SelectedNodeId
        {
            get
            {
                CswPrimaryKey ret = new CswPrimaryKey();
                ret.FromString( _ValueList.SelectedValue );
                return ret;
            }
        }
        public string SelectedNodeName
        {
            get
            {
                return _ValueList.SelectedItem.Text;
            }
        }

        //private HtmlGenericControl _Div;
        //private CswAutoTable _OuterTable;
        //private RadTreeView _TreeView;
        private Label _ValueLabel;
        //private CswImageButton _EditButton;
        //private CswImageButton _ClearButton;
        //private HiddenField _hiddenClear;
        private DropDownList _ValueList;
        private CswImageButton _AddNewButton;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _ValueList = new DropDownList();
            _ValueList.ID = "relval";
            _ValueList.CssClass = DropDownCssClass;
            //this.Controls.Add( _ValueList );
            Table.addControl( 0, 0, _ValueList );

            _ValueLabel = new Label();
            _ValueLabel.ID = "rellabel";
            _ValueLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            Table.addControl( 0, 0, _ValueLabel );

            Table.addControl( 0, 1, new CswLiteralNbsp() );

            _AddNewButton = new CswImageButton( CswImageButton.ButtonType.Add );
            _AddNewButton.ID = "newrel";
            //this.Controls.Add( _AddNewButton );
            Table.addControl( 0, 2, _AddNewButton );

            //_EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
            //_EditButton.ID = "reledit";
            //Table.addControl( 0, 1, _EditButton );

            //_ClearButton = new CswImageButton( CswImageButton.ButtonType.Clear );
            //_ClearButton.ID = "relclearbutton";
            //Table.addControl( 0, 2, _ClearButton );

            //_hiddenClear = new HiddenField();
            //_hiddenClear.ID = "hiddenclear";
            //Table.addControl( 0, 2, _hiddenClear );

            //_OuterTable = new CswAutoTable();
            //_OuterTable.ID = "relationship_outertable";
            //_OuterTable.Attributes.Add( "class", "cbarray_outertable" );
            //Table.getCell( 1, 0 ).ColumnSpan = 3;
            //Table.addControl( 1, 0, _OuterTable );

            //_Div = new HtmlGenericControl( "div" );
            //_Div.ID = "relationshipdiv";
            //_Div.Attributes.Add( "class", "cbarraydiv" );
            //_OuterTable.addControl( 0, 0, _Div );

            //_TreeView = new RadTreeView();
            //_TreeView.ID = "relationshiptree";
            //_TreeView.EnableEmbeddedSkins = false;
            //_TreeView.Skin = "ChemSW";
            //_Div.Controls.Add( _TreeView );

            base.CreateChildControls();

            //if( !( AllowEditValue && !ReadOnly ) )
            //    _RequiredValidator.ControlToValidate = _ValueLabel.ID;
            if( !ReadOnly && Required )
                _RequiredValidator.ControlToValidate = _ValueLabel.ID;
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                _ValueList.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange()" );

                if( _EditMode != NodeEditMode.AddInPopup &&
                    _EditMode != NodeEditMode.EditInPopup &&
                    _EditMode != NodeEditMode.Demo &&
                    Prop.AsRelationship.TargetType == CswNbtViewRelationship.RelatedIdType.NodeTypeId &&
                    !ReadOnly &&
                    _CswNbtResources.CurrentNbtUser.CheckCreatePermission( Prop.AsRelationship.TargetId ) )
                {
                    _AddNewButton.OnClientClick = "return RelationshipAddNodeDialog_openPopup('" + Prop.AsRelationship.TargetId.ToString() + "');";
                    _AddNewButton.Visible = true;
                }
                else
                {
                    _AddNewButton.Visible = false;
                }

                if( ReadOnly )
                {
                    _ValueLabel.Visible = true;
                    _ValueList.Visible = false;
                    _ValueList.Enabled = false;
                }
                else
                {
                    _ValueLabel.Visible = false;
                    _ValueList.Visible = true;
                }

                //Int32 Height = 4;
                //if(Prop != null)
                //    Height = Prop.AsRelationship.Rows * 25;
                //_Div.Style.Add( HtmlTextWriterStyle.Height, Height.ToString() + "px" );

                //_ValueLabel.Visible = true;
                //_EditButton.Visible = true;
                //_OuterTable.Visible = false;
                //_ClearButton.Visible = true;

                //if( AllowEditValue && !ReadOnly )
                //{
                //    _OuterTable.Visible = true;
                //    _TreeView.ExpandAllNodes();   // BZ 7745

                //    _EditButton.Visible = false;
                //    _ClearButton.Visible = false;
                //    _ValueLabel.Visible = false;
                //}
                //if( ReadOnly )
                //{
                //    _EditButton.Visible = false;
                //    _ClearButton.Visible = false;
                //}
                //if( Required )
                //{
                //    _ClearButton.Visible = false;
                //}

                //if( _EditButton.Visible && Prop != null && Prop.NodeId != null )
                //    _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                //if( _ClearButton.Visible )
                //    _ClearButton.OnClientClick = "return CswRelationship_clear('" + _ValueLabel.ClientID + "', '" + _hiddenClear.ClientID + "');";

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }
    }
}
