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
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_EditNode : System.Web.UI.Page
    {
        private CswNbtNodeKey _NodeKey;
        private CswNbtNodeKey NodeKey
        {
            get { return _NodeKey; }
            set { _NodeKey = value; }
        }
        private bool _AddMode = false;
        private bool _DontChangeView = false;
        private bool _DontChangeSelectedNode = false;

        private CswNbtNode _Node = null;
        private CswPropertyTable PropTable;

        private string SelectedTabId
        {
            get
            {
                string ret = string.Empty;
                EnsureChildControls();
                if (PropTable != null && PropTable.TabStrip.SelectedTab != null)
                    ret = PropTable.TabStrip.SelectedTab.Value;
                else if (Session["EditNode_SelectedTabId"] != null && Session["EditNode_SelectedTabId"].ToString() != string.Empty)
                    ret = Session["EditNode_SelectedTabId"].ToString();
                return ret;
            }
            set
            {
                EnsureChildControls();
                foreach (RadTab Tab in PropTable.TabStrip.Tabs)
                {
                    if (Tab.Value == value)
                    {
                        Tab.Selected = true;
                        break;
                    }
                }
            }
        }

        private CswNbtNodeKey ParentNodeKey = null;
        public string CheckedNodeIds = string.Empty;
        //public string _ViewRelationshipUniqueId = string.Empty;
        public Int32 _SessionViewId = Int32.MinValue;
        //public CswNbtViewRelationship _ViewRelationship
        //{
        //    get
        //    {
        //        CswNbtViewRelationship ret = null;
        //        if(_SessionViewId != Int32.MinValue)
        //            ret = (CswNbtViewRelationship) Master.CswNbtResources.ViewCache.getView(_SessionViewId).FindViewNodeByUniqueId( _ViewRelationshipUniqueId );
        //        return ret;
        //    }
        //}

        protected void Page_Init( object sender, EventArgs e )
        {
            try
            {
                String TitleBar = "Edit ";
                PropTable = new CswPropertyTable( Master.CswNbtResources, Master.AjaxManager );
                PropTable.EnableViewState = false;
                PropTable.OnError += new CswErrorHandler( Master.HandleError );

                if( Request.QueryString["dcv"] != null && Request.QueryString["dcv"] != string.Empty )
                    _DontChangeView = ( Request.QueryString["dcv"] == "1" );
                if( Request.QueryString["dcsn"] != null && Request.QueryString["dcsn"] != string.Empty )
                    _DontChangeSelectedNode = ( Request.QueryString["dcsn"] == "1" );
                if( Request.QueryString["svid"] != null && Request.QueryString["svid"] != string.Empty )
                    _SessionViewId = CswConvert.ToInt32( Request.QueryString["svid"] );
                //if( Request.QueryString["vrui"] != null && Request.QueryString["vrui"] != string.Empty )
                //    _ViewRelationshipUniqueId = Request.QueryString["vrui"];

                if( Request.QueryString["nodekey"] != null && Request.QueryString["nodekey"] != string.Empty )
                {
                    // Edit from nodekey
                    NodeKey = new CswNbtNodeKey( Master.CswNbtResources, Request.QueryString["nodekey"].ToString() );
                    _Node = Master.CswNbtResources.Nodes[NodeKey.NodeId];
                    PropTable.EditMode = NodeEditMode.EditInPopup;
                    PropTable.SelectedNode = _Node;
                    PropTable.TabStrip.TabClick += new RadTabStripEventHandler( TabStrip_TabClick );
                    TitleBar += _Node.NodeType.NodeTypeName.ToString();
                }
                else if( Request.QueryString["nodeid"] != null && Request.QueryString["nodeid"] != string.Empty )
                {
                    // Edit from nodeid
                    //NodeKey = null;
                    CswPrimaryKey NodeId = new CswPrimaryKey();
                    NodeId.FromString( Request.QueryString["nodeid"] );
                    _Node = Master.CswNbtResources.Nodes[NodeId];
                    NodeKey = new CswNbtNodeKey( Master.CswNbtResources, null, string.Empty, _Node.NodeId, _Node.NodeSpecies, _Node.NodeTypeId, _Node.ObjectClassId, string.Empty, string.Empty );
                    PropTable.EditMode = NodeEditMode.EditInPopup;
                    PropTable.SelectedNode = _Node;
                    PropTable.TabStrip.TabClick += new RadTabStripEventHandler( TabStrip_TabClick );
                    TitleBar += _Node.NodeType.NodeTypeName.ToString();
                }
                else if( Request.QueryString["nodetypeid"] != null && Request.QueryString["nodetypeid"] != string.Empty )
                {
                    // Add new node from nodetype

                    // grab the selected node so we have the parent node fetched
                    CswNbtNode ParentNode = null;
                    ParentNodeKey = null;

                    if( Request.QueryString["parentnodekey"] != null && Request.QueryString["parentnodekey"] != string.Empty )
                        ParentNodeKey = new CswNbtNodeKey( Master.CswNbtResources, Request.QueryString["parentnodekey"].ToString() );
                    else if(_SessionViewId != Int32.MinValue)
                        ParentNodeKey = SelectedNodeKey;
                    
                    if( ParentNodeKey != null && ParentNodeKey.NodeSpecies == NodeSpecies.Plain )
                        ParentNode = Master.CswNbtResources.Nodes[ParentNodeKey];

                    CheckedNodeIds = Request.QueryString["checkednodeids"];

                    _AddMode = true;
      
                    string NodeTypeIdString = Request.QueryString["nodetypeid"].ToString();
                    Int32 NodeTypeId = CswConvert.ToInt32( NodeTypeIdString );
                    CswNbtMetaDataNodeType MetaDataNodeType = Master.CswNbtResources.MetaData.getNodeType( NodeTypeId );

                    //BZ 10181
                    TitleBar = "Add " + MetaDataNodeType.NodeTypeName.ToString();
                    
                    string MultiEditErrorPropName = string.Empty;
                    if( CheckedNodeIds == string.Empty || !MetaDataNodeType.IsUniqueAndRequired( ref MultiEditErrorPropName ) )
                    {
                        _Node = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                        PropTable.SelectedNode = _Node;

                        if( null != ParentNode && ParentNode.NodeSpecies == NodeSpecies.Plain )
                        {
                            //Case 20544 - Use view from querystring in case we're adding from a grid/view prop
                            if( null != Request.QueryString["sourceviewid"] && String.Empty != Request.QueryString["sourceviewid"] )
                            {
								CswNbtViewId SourceViewId = new CswNbtViewId( CswConvert.ToInt32( Request.QueryString["sourceviewid"] ) );
								CswNbtView SourceView = Master.CswNbtResources.ViewSelect.restoreView( SourceViewId );
                                if( null != SourceView )
                                    _Node.RelateToNode( ParentNode, SourceView );
                                else
                                    _Node.RelateToNode( ParentNode, Master.CswNbtView );
                            }
                            else
                                _Node.RelateToNode( ParentNode, Master.CswNbtView );
                        }

                        // BZ 10372 - We can't do this anymore since we can add new nodes anywhere in the tree
                        //// BZ 8339 - Make relationship read only
                        //if( _ViewRelationship != null && _ViewRelationship.PropOwner == CswNbtViewRelationship.PropOwnerType.Second )
                        //{
                        //    CswNbtMetaDataNodeTypeProp RelationshipProp = null;
                        //    if( _ViewRelationship.PropType == CswNbtViewRelationship.PropIdType.NodeTypePropId )
                        //        RelationshipProp = MetaDataNodeType.getNodeTypePropByFirstVersionId( _ViewRelationship.PropId );
                        //    else if( _ViewRelationship.PropType == CswNbtViewRelationship.PropIdType.ObjectClassPropId )
                        //        RelationshipProp = MetaDataNodeType.getNodeTypePropByObjectClassPropName( Master.CswNbtResources.MetaData.getObjectClassProp( _ViewRelationship.PropId ).PropName );

                        //    // BZ 10025 - Instead of making it read only, just make it required
                        //    //_Node.Properties[RelationshipProp].ReadOnly = true;
                        //    _Node.Properties[RelationshipProp].TemporarilyRequired = true;
                        //}

                        PropTable.EditMode = NodeEditMode.AddInPopup;
                    }
                    else
                    {
                        string Errormsg = MetaDataNodeType.NodeTypeName + " has a unique and required property (" + MultiEditErrorPropName + "), and cannot be multi-added.";
						throw new CswDniException( ErrorType.Warning, Errormsg, Errormsg );
                    }

                    // BZ 8338
                    if( CheckedNodeIds != string.Empty )
                    {
                        ParentNodeNameLiteral.Text = "Add New " + MetaDataNodeType.NodeTypeName + " Below: " + ParentNode.NodeName;
                        foreach( string AnotherParentNodeIdString in CheckedNodeIds.Split( ',' ) )
                        {
                            CswPrimaryKey AnotherParentNodeId = new CswPrimaryKey();
                            AnotherParentNodeId.FromString( AnotherParentNodeIdString );
                            if( AnotherParentNodeId != ParentNode.NodeId )
                            {
                                CswNbtNode AnotherParentNode = Master.CswNbtResources.Nodes.GetNode( AnotherParentNodeId );
                                ParentNodeNameLiteral.Text += ", " + AnotherParentNode.NodeName;
                            }
                        }
                    }

                }
                else
                {
					throw new CswDniException( ErrorType.Error, "NodeId, NodeKey, or NodeTypeId is required", "Popup_EditNode.aspx requires a valid NodeKey, NodeId, or NodeTypeId" );
                }
                
                TitleContentLiteral.Text = TitleBar;
                PropGridPlaceHolder.Controls.Add( PropTable );

                PropTable.ShowCancelButton = true;
                PropTable.CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";
                PropTable.CancelButton.UseSubmitBehavior = false;
                if( _AddMode )
                    PropTable.SaveButton.Text = "Create";
                else
                    PropTable.SaveButton.Text = "Save Tab";

                PropTable.SelectedTabId = SelectedTabId;
                PropTable.initTabStrip();
                SelectedTabId = PropTable.initTabContents();
            }

            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void TabStrip_TabClick(object sender, RadTabStripEventArgs e)
        {
            PropTable.SelectedTabId = e.Tab.Value;
            SelectedTabId = PropTable.initTabContents();
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                PropTable.OnSave += new CswPropertyTable.SaveHandler(PropTable_OnSave);
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }

            base.OnLoad(e);
        }

        private CswNbtNodeKey SelectedNodeKey
        {
            get
            {
                CswNbtNodeKey ret = null;
                if( Session[ "Main_SelectedNodeKey" ] != null )
                {
                    ret = new CswNbtNodeKey( Master.CswNbtResources, Session[ "Main_SelectedNodeKey" ].ToString() );
                }
                return ret;
            }
            set
            {
                if( value != null )
                    Session[ "Main_SelectedNodeKey" ] = value.ToString();
                else
                    Session.Remove( "Main_SelectedNodeKey" );
            }
        }
        private CswPrimaryKey NewNodeId
        {
            get { return (CswPrimaryKey) Session["NewNodeId"]; }
            set { Session["NewNodeId"] = value; }
        }

        public void HandleAddNode( CswNbtNode Node )
        {
            Master.HandleAddNode( Node );
        }


        void PropTable_OnSave(object sender, CswPropertyTable.SaveHandlerEventArgs e)
        {
            try
            {
                if( _AddMode )
                {
                    CswNbtView View = Master.CswNbtView;

                    // The actual property saves are handled by the CswPropertyTable before this event fires.
                    CswNbtNode SourceNode = e.Node;

                    // BZ 8517 - this sets sequences that have setvalonadd = 0
                    Master.CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( SourceNode );
                    SourceNode.postChanges( false );

                    HandleAddNode( SourceNode );

                    // Batch Add - Copy the new node for the other parents
                    if( CheckedNodeIds != string.Empty )
                    {
                        foreach( string ParentNodeIdString in CheckedNodeIds.Split( ',' ) )
                        {
                            CswPrimaryKey ParentNodeId = new CswPrimaryKey();
                            ParentNodeId.FromString( ParentNodeIdString );
                            if( ParentNodeId != ParentNodeKey.NodeId )
                            {
                                CswNbtNode ParentNode = Master.CswNbtResources.Nodes.GetNode( ParentNodeId );
                                CswNbtNode AnotherNode = Master.CswNbtResources.Nodes.makeNodeFromNodeTypeId( SourceNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                                AnotherNode.copyPropertyValues( SourceNode );
                                AnotherNode.RelateToNode( ParentNode, View );
                                AnotherNode.postChanges( true );

                                HandleAddNode( AnotherNode );
                            }
                        }
                    }

                    Master.CswNbtResources.finalize();

                    //// BZ 9609 - We must force reinit since we just added node(s)
                    //// actually, let's reinit everything, since any number of trees may be affected
                    // Moved this to CswNbtObjClassDefault
                    //Master.CswNbtResources.Trees.clear();
                    ICswNbtTree Tree = Master.CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

                    if( !_DontChangeView && View.ViewMode == NbtViewRenderingMode.Tree)
                    {
                        // If the view doesn't include the nodetype of the new node, adjust it
                        //CswNbtTreeKey _TreeKey = Tree.Key;
                        Tree.makeNodeCurrent( ParentNodeKey );


                        Int32 NodeTypeId = SourceNode.NodeTypeId;
                        CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( NodeTypeId );

                        //CswNbtNode Node = Tree.getNodeForCurrentPosition();
                        if( Tree.getNodeKeyByNodeId( SourceNode.NodeId ) == null &&
                            !View.FindViewNodeByUniqueId( ParentNodeKey.ViewNodeUniqueId ).ContainsNodeType( NodeType.FirstVersionNodeTypeId ) )
                        {
                            // Add the nodetype to the root of the current view.
                            View.AddViewRelationship( NodeType, true );

                            Master.setViewXml( View.ToString() );

                            // Reset the tree so that we can find the selected node key 
                            if( !_DontChangeSelectedNode )
                                Tree = Master.CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

                        }
                    }

                    if( !_DontChangeSelectedNode )
                    {
                        SelectedNodeKey = Tree.getNodeKeyByNodeId( SourceNode.NodeId );
                    }

                    NewNodeId = SourceNode.NodeId;

                    string JS = @"<script language=""Javascript"">Popup_OK_Clicked();</script>";
                    ScriptManager.RegisterClientScriptBlock( this, this.GetType(), this.UniqueID + "_JS", JS, false );
                }



            }//try
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // PropTable_OnSave


        protected void DeleteButton_OnClickHandler( object sender, EventArgs e )
        {
            try
            {
                //Master.CswNbtResources.Nodes.Delete( NodeKey.NodeId );
                Master.CswNbtResources.Nodes[ NodeKey ].delete();


                // Commit any transactions
                //Master.CswNbtResources.finalize();  // already being done by Master.ReleaseAll()

                // Reload the tree and content panes
                PropTable.Visible = false;
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }

        protected override void OnUnload(EventArgs e)
        {
            Session["EditNode_SelectedTabId"] = SelectedTabId;
            //if ( _Node != null )
            //    _Node.cancelChanges();  // BZ 8342

            //Master.ReleaseAll();
            base.OnUnload( e );
        }
    }
}
