using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

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
                    if( Prop.AsRelationship.TargetType == NbtViewRelatedIdType.NodeTypeId && !ReadOnly )
                        ReadOnly = !( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( Prop.AsRelationship.TargetId ) ) );

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
                        /* Required has no meaning for a default value (the only way this control will be used until it retires). 
                         * if( !Required  )
                         */
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

        public override void Save()
        {
            try
            {
                if( !ReadOnly )
                {
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

        private Label _ValueLabel;
        private DropDownList _ValueList;
        private CswImageButton _AddNewButton;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _ValueList = new DropDownList();
            _ValueList.ID = "relval";
            _ValueList.CssClass = DropDownCssClass;
            Table.addControl( 0, 0, _ValueList );

            _ValueLabel = new Label();
            _ValueLabel.ID = "rellabel";
            _ValueLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            Table.addControl( 0, 0, _ValueLabel );

            Table.addControl( 0, 1, new CswLiteralNbsp() );

            _AddNewButton = new CswImageButton( CswImageButton.ButtonType.Add );
            _AddNewButton.ID = "newrel";
            Table.addControl( 0, 2, _AddNewButton );

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

                if( _EditMode != NodeEditMode.Add &&
                    _EditMode != NodeEditMode.EditInPopup &&
                    _EditMode != NodeEditMode.Demo &&
                    !ReadOnly &&
                    Prop.AsRelationship.TargetType == NbtViewRelatedIdType.NodeTypeId &&
                    _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType( Prop.AsRelationship.TargetId ) ) )
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

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }
    }
}
