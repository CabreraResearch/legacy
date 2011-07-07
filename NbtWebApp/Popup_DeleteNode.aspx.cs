using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_DeleteNode : System.Web.UI.Page
    {
        private CswNbtNodeKey _NodeKey = null;
        private CswNbtNode _Node = null;
        public string CheckedNodeIds = string.Empty;
        private CswNbtView _RelatedNodesView = null;

        protected override void OnInit( EventArgs e )
        {
            if( Request.QueryString["nodekey"] != null )
            {
                _NodeKey = new CswNbtNodeKey( Master.CswNbtResources, Request.QueryString["nodekey"].ToString() );

                if( _NodeKey.NodeSpecies != NodeSpecies.Plain )
					throw new CswDniException( ErrorType.Error, "Invalid Node", "Cannot delete node of species: " + _NodeKey.NodeSpecies );

                _Node = Master.CswNbtResources.Nodes[_NodeKey];
                bool canDelete = true;
                string deleteError1 = "";
                string deleteError2 = "";

                if( _Node != null )
                {
                    if( !Master.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, _NodeKey.NodeTypeId, _Node, null ) )
                    {
                        deleteError1 = "You do not have permission to delete:";
                        canDelete = false;
                    }
                    else
                    {
                        //Look for required relationship targets. BZ 8435

                        foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.NodeTypes )
                        {
                            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
                            {
                                if( canDelete )
                                {
                                    if( Prop.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                                    {
                                        if( ( Prop.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                                              Prop.FKValue == _Node.NodeType.FirstVersionNodeTypeId ) ||
                                            ( Prop.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
                                              Prop.FKValue == _Node.ObjectClassId ) )
                                        {
                                            CswNbtView RelatedNodesView = new CswNbtView( Master.CswNbtResources );
                                            RelatedNodesView.ViewName = "Related " + NodeType.NodeTypeName + " View";
                                            CswNbtViewRelationship ParentRelationship = RelatedNodesView.AddViewRelationship( _Node.NodeType, false );
                                            CswNbtViewRelationship ChildRelationship = RelatedNodesView.AddViewRelationship( ParentRelationship,
                                                                                                                             CswNbtViewRelationship.PropOwnerType.Second,
                                                                                                                             Prop,
                                                                                                                             false );
                                            ParentRelationship.NodeIdsToFilterIn.Add( _Node.NodeId );
                                            ICswNbtTree RelatedNodesTree = Master.CswNbtResources.Trees.getTreeFromView( RelatedNodesView, true, true, false, false );
                                            RelatedNodesTree.goToNthChild( 0 );
                                            Int32 NodeCount = RelatedNodesTree.getChildNodeCount();

                                            if( Prop.IsRequired )
                                            {
                                                if( NodeCount > 0 )
                                                {
                                                    canDelete = false;
                                                    deleteError1 = "You cannot delete:";
                                                    deleteError2 = "because it is the target of a required relationship.";
                                                    ShowButton.Visible = true;
                                                    _RelatedNodesView = RelatedNodesView;
                                                    //ShowButton.OnClientClick = "Popup_OK_Clicked(" + RelatedNodesView.SessionViewId.ToString() + ");";
                                                    ShowButton.Click += new EventHandler( ShowButton_Click );
                                                }
                                            }
                                            else
                                            {
                                                CswNbtNode ChildNode = Master.CswNbtResources.Nodes.GetNode( _Node.NodeId );

                                                for( Int32 NodeNumber = 0; NodeNumber < NodeCount; NodeNumber++ )
                                                {
                                                    RelatedNodesTree.goToNthChild( NodeNumber );
                                                    ChildNode = RelatedNodesTree.getNodeForCurrentPosition();
                                                    ChildNode.Properties[Prop].AsRelationship.RelatedNodeId = null;
                                                    ChildNode.Properties[Prop].AsRelationship.CachedNodeName = string.Empty;
                                                    ChildNode.postChanges( true );
                                                    RelatedNodesTree.goToParentNode();
                                                }
                                            }
                                        }
                                    }
                                }
                            } //foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
                        } //foreach( CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.NodeTypes )
                    }
                    if( !canDelete )
                    {
                        Literal1.Text = deleteError1;
                        Literal2.Text = deleteError2;
                        DeleteButton.Visible = false;
                    }

                    //NodeNameHolder.Text = _Node.NodeName;
                    DeleteNodeNameLiteral.Text = _Node.NodeName;

                    if( Request.QueryString["checkednodeids"] != null )
                        CheckedNodeIds = Request.QueryString["checkednodeids"].ToString();
                    if( CheckedNodeIds != string.Empty )
                    {
                        foreach( string NodeIdToDeleteString in CheckedNodeIds.Split( ',' ) )
                        {
                            CswPrimaryKey NodePKToDelete = new CswPrimaryKey();
                            NodePKToDelete.FromString( NodeIdToDeleteString );
                            if( NodePKToDelete != _Node.NodeId )
                            {
                                CswNbtNode NodeToDelete = Master.CswNbtResources.Nodes.GetNode( NodePKToDelete );
                                DeleteNodeNameLiteral.Text += ", " + NodeToDelete.NodeName;
                            }
                        }
                    }
                }
            }

            base.OnInit( e );
        }

        void ShowButton_Click( object sender, EventArgs e )
        {
            string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            Master.setViewXml( _RelatedNodesView.ToString() );
        }

        protected void DeleteButton_Click( object sender, EventArgs e )
        {
            try
            {
				if( Master.CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Delete, _NodeKey.NodeTypeId, _Node, null ) )
                {
                    _Node.delete();

                    if( CheckedNodeIds != string.Empty )
                    {
                        foreach( string NodeIdToDeleteString in CheckedNodeIds.Split( ',' ) )
                        {
                            CswPrimaryKey NodePKToDelete = new CswPrimaryKey();
                            NodePKToDelete.FromString( NodeIdToDeleteString );
                            if( NodePKToDelete != _Node.NodeId )
                            {
                                CswNbtNode NodeToDelete = Master.CswNbtResources.Nodes.GetNode( NodePKToDelete );
                                NodeToDelete.delete();
                            }
                        }
                    }
                }

                // this isn't appropriate for deleting from a grid, and is unnecessary
                //Session["Main_SelectedNodeKey"] = string.Empty;

                string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

    } // Popup_DeleteNode
} // namespace ChemSW.Nbt.WebPages
