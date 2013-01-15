using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Grid.ExtJs;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Tree;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtSdTrees
    {
        public class Contract
        {
            [DataContract]
            public class Request
            {
                public Request()
                {
                    IncludeNodeId = new CswPrimaryKey();
                    IncludeNodeKey = null;
                    ViewId = new CswNbtViewId();
                }

                public CswPrimaryKey IncludeNodeId;

                [DataMember]
                public string NodeId
                {
                    get
                    {
                        string ret = string.Empty;
                        if( CswTools.IsPrimaryKey( IncludeNodeId ) )
                        {
                            ret = IncludeNodeId.ToString();
                        }
                        return ret;
                    }
                    set { IncludeNodeId = CswConvert.ToPrimaryKey( value ); }
                }

                public CswNbtNodeKey IncludeNodeKey;

                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public string NodeKey
                {
                    get
                    {
                        string ret = string.Empty;
                        if( null != IncludeNodeKey )
                        {
                            ret = IncludeNodeKey.ToString();
                        }
                        return ret;
                    } 
                    set
                    {
                        string Key = value;
                        if( false == string.IsNullOrEmpty( Key ) )
                        {
                            IncludeNodeKey = new CswNbtNodeKey( Key );
                        }
                    }
                }

                public CswNbtViewId ViewId = null;
                public CswNbtSessionDataId SessionViewId = null;

                [DataMember]
                public string NbtViewId
                {
                    get
                    {
                        string ret = string.Empty;
                        if( null != ViewId && ViewId.isSet() )
                        {
                            ret = ViewId.ToString();
                        }
                        return ret;
                    }
                    set
                    {
                        string Id = value;
                        if( false == string.IsNullOrEmpty( Id ) )
                        {
                            if( CswNbtViewId.isViewIdString( Id ) )
                            {
                                ViewId = new CswNbtViewId( Id );
                            }
                            else if( CswNbtSessionDataId.isSessionDataIdString( Id ) )
                            {
                                SessionViewId = new CswNbtSessionDataId( Id );
                            }
                        }
                    }
                }

                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public bool IncludeNodeRequired = false;
                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public bool IncludeInQuickLaunch = false;
                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public string DefaultSelect;
                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public string AccessedByObjClassId = "";
            }

            public class Response: CswWebSvcReturn
            {
                public Response()
                {
                    Data = new ResponseData();
                }

                [DataMember]
                public ResponseData Data;

                [DataContract]
                public class ResponseData
                {
                    public ResponseData()
                    {
                        Tree = new Collection<CswExtTree.TreeNode>();
                        Columns = new Collection<CswNbtGridExtJsColumn>();
                        Fields = new Collection<CswNbtGridExtJsField>();
                        ViewMode = NbtViewRenderingMode.Tree;
                        PageSize = 50;
                    }

                    [DataMember]
                    public Collection<CswExtTree.TreeNode> Tree;

                    [DataMember]
                    public Collection<CswNbtGridExtJsColumn> Columns;

                    [DataMember]
                    public Collection<CswNbtGridExtJsField> Fields; 

                    public CswNbtSessionDataId ViewId { get; set; }

                    [DataMember( EmitDefaultValue = false, IsRequired = false )]
                    public string NewViewId
                    {
                        get
                        {
                            string ret = string.Empty;
                            if( null != ViewId && ViewId.isSet() )
                            {
                                ret = ViewId.ToString();
                            }
                            return ret;
                        }
                        set { ViewId = new CswNbtSessionDataId( value ); }
                    }

                    [IgnoreDataMember]
                    public NbtViewRenderingMode ViewMode { get; set; }

                    [DataMember( EmitDefaultValue = false, IsRequired = false )]
                    public string NewViewMode
                    {
                        get { return ViewMode.ToString(); }
                        set { ViewMode = value; }
                    }

                    [DataMember]
                    public int PageSize = 50;

                    public CswNbtNodeKey SelectedNodeKey;

                    [DataMember( EmitDefaultValue = false, IsRequired = false )]
                    public string SelectedId
                    {
                        get
                        {
                            string ret = string.Empty;
                            if( null != SelectedNodeKey )
                            {
                                ret = SelectedNodeKey.ToString();
                            }
                            return ret;
                        }
                        set { SelectedNodeKey = new CswNbtNodeKey( value ); }
                    }
                }
            }
        }


        private readonly CswNbtResources _CswNbtResources;
        private CswNbtView _View;

        public CswNbtSdTrees( CswNbtResources CswNbtResources, CswNbtView View )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;
        }

        /// <summary>
        /// Recursively iterate the tree and add child nodes according to parent hierarchy
        /// </summary>
        private void _runTreeNodesRecursive( ICswNbtTree Tree, Collection<CswExtTree.TreeNode> TreeNodes, CswExtTree.TreeNode ParentNode )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c += 1 )
            {
                Tree.goToNthChild( c );

                CswExtTree.TreeNode TreeNode = _getTreeNode( Tree, ParentNode );
                TreeNodes.Add( TreeNode );

                if( null != TreeNode.Children )
                {
                    _runTreeNodesRecursive( Tree, TreeNode.Children, TreeNode );
                }
                Tree.goToParentNode();
                // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

                if( Tree.getCurrentNodeChildrenTruncated() )
                {
                    CswExtTree.TreeNode TruncatedTreeNode = _getTreeNode( Tree, TreeNode );
                    TruncatedTreeNode.Name = "Results Truncated";
                    TruncatedTreeNode.IsLeaf = true;
                    TruncatedTreeNode.Icon = "Images/icons/truncated.gif";
                    TreeNodes.Add( TruncatedTreeNode );
                }
            }
        } // _runTreeNodesRecursive()


        /// <summary>
        /// Generate a JObject for the tree's current node
        /// </summary>
        private CswExtTree.TreeNode _getTreeNode( ICswNbtTree Tree, CswExtTree.TreeNode Parent )
        {
            CswExtTree.TreeNode Ret = new CswExtTree.TreeNode();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeIcon = "";
            string ThisNodeKeyString = ThisNodeKey.ToString();
            string ThisNodeId = "";
            int ThisNodeTypeId = int.MinValue;
            int ThisObjectClassId = int.MinValue;
            string ThisNodeRel = "";
            bool ThisNodeLocked = false;
            bool ThisNodeDisabled = false;
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            switch( ThisNodeKey.NodeSpecies )
            {
                case NodeSpecies.Plain:
                    ThisNodeId = ThisNodeKey.NodeId.ToString();
                    ThisNodeName = Tree.getNodeNameForCurrentPosition();
                    ThisNodeTypeId = ThisNodeType.FirstVersionNodeTypeId;
                    ThisObjectClassId = ThisNodeType.ObjectClassId;
                    ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();
                    ThisNodeDisabled = ( false == Tree.getNodeIncludedForCurrentPosition() );
                    if( false == string.IsNullOrEmpty( ThisNodeType.IconFileName ) )
                    {
                        ThisNodeIcon = CswNbtMetaDataObjectClass.IconPrefix16 + ThisNodeType.IconFileName;
                    }
                    break;
                case NodeSpecies.Group:
                    ThisNodeRel = "group";
                    Ret.CssClass = "folder";
                    break;
            }

            CswNbtViewNode ThisNodeViewNode = _View.FindViewNodeByUniqueId( ThisNodeKey.ViewNodeUniqueId );

            string ThisNodeState = "closed";
            if( ThisNodeKey.NodeSpecies == NodeSpecies.More ||
                _View.ViewMode == NbtViewRenderingMode.List ||
                ( Tree.IsFullyPopulated && Tree.getChildNodeCount() == 0 ) ||
                ( ThisNodeViewNode != null && ThisNodeViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count == 0 ) )
            {
                ThisNodeState = "leaf";
            }

            Ret.Name = ThisNodeName;
            Ret.Icon = ThisNodeIcon;
            Ret.Id = ThisNodeKeyString;
            switch( ThisNodeState )
            {
                case "closed":
                    Ret.Expanded = false;
                    break;
                case "leaf":
                    Ret.IsLeaf = true;
                    break;
            }
            if( int.MinValue != ThisNodeTypeId )
            {
                Ret.NodeTypeId = ThisNodeTypeId;
            }
            if( int.MinValue != ThisObjectClassId )
            {
                Ret.ObjectClassId = ThisObjectClassId;
            }
            if( null != Parent )
            {
                Ret.Path = "|" + Parent.Path;
            }
            Ret.Path += "|" + Ret.Id;
            Ret.NodeSpecies = ThisNodeKey.NodeSpecies.ToString();
            Ret.NodeId = ThisNodeId;
            Ret.IsLocked = ThisNodeLocked;
            if( ThisNodeDisabled )
            {
                Ret.IsDisabled = true;
            }

            CswNbtNodeKey ParentKey = Tree.getNodeKeyForParentOfCurrentPosition();
            if( ParentKey.NodeSpecies != NodeSpecies.Root )
            {
                Ret.ParentId = ParentKey.ToString();
            }
            else
            {
                Ret.ParentId = "root";
            }
            if( Tree.getChildNodeCount() > 0 )
            {
                Ret.Children = new Collection<CswExtTree.TreeNode>();
            }
            else
            {
                Ret.Children = null;
            }
            //ThisNodeObj["childcnt"] = Tree.getChildNodeCount().ToString();

            return Ret;
        } // _treeNodeJObject()

        public void runTree( Contract.Response.ResponseData ResponseData, Contract.Request Request )
        {
            ResponseData.Tree = ResponseData.Tree ?? new Collection<CswExtTree.TreeNode>();

            if( null != _View ) //&& ( _View.ViewMode == NbtViewRenderingMode.Tree || _View.ViewMode == NbtViewRenderingMode.List ) )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                _View.SaveToCache( Request.IncludeInQuickLaunch );

                if( CswTools.IsPrimaryKey( Request.IncludeNodeId ) && null == Request.IncludeNodeKey )
                {
                    Request.IncludeNodeKey = Tree.getNodeKeyByNodeId( Request.IncludeNodeId );
                    if( Request.IncludeNodeRequired && null == Request.IncludeNodeKey )
                    {
                        CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.Nodes[Request.IncludeNodeId].getNodeType();
                        _View = IncludeKeyNodeType.CreateDefaultView();
                        _View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                        _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Request.IncludeNodeId );
                        _View.SaveToCache( Request.IncludeInQuickLaunch ); // case 22713
                        ResponseData.ViewId = _View.SessionViewId;
                        ResponseData.ViewMode = _View.ViewMode;
                        Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                    }
                }
                if( Request.IncludeNodeRequired && Request.IncludeNodeKey != null && Tree.getNodeKeyByNodeId( Request.IncludeNodeKey.NodeId ) == null )
                {
                    CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.MetaData.getNodeType( Request.IncludeNodeKey.NodeTypeId );
                    _View = IncludeKeyNodeType.CreateDefaultView();
                    _View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                    _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Request.IncludeNodeKey.NodeId );
                    _View.SaveToCache( Request.IncludeInQuickLaunch ); // case 22713
                    ResponseData.ViewId = _View.SessionViewId;
                    ResponseData.ViewMode = _View.ViewMode;
                    Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                }

                Tree.goToRoot();
                bool HasResults = ( Tree.getChildNodeCount() > 0 );
                //ReturnObj["result"] = HasResults.ToString().ToLower();
                //ReturnObj["types"] = getTypes();
                ResponseData.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;

                ResponseData.SelectedNodeKey = null;
                if( HasResults )
                {
                    // Determine the default selected node:
                    // If the requested node to select is on the tree, return it.
                    // If the requested node to select is not on the tree, return the first child of the root.
                    if( Request.IncludeNodeKey != null )
                    {
                        Tree.makeNodeCurrent( Request.IncludeNodeKey );
                        if( Tree.isCurrentNodeDefined() )
                        {
                            ResponseData.SelectedNodeKey = Request.IncludeNodeKey;
                        }
                    }
                    if( ResponseData.SelectedNodeKey == null )
                    {
                        switch( Request.DefaultSelect )
                        {
                            case "none":
                                break;
                            case "root":
                                break;
                            case "firstchild":
                                Tree.goToRoot();
                                CswNbtNodeKey CurrentKey = Tree.getNodeKeyForCurrentPosition();
                                while( CurrentKey != null &&
                                       CurrentKey.NodeSpecies != NodeSpecies.Plain &&
                                       Tree.getChildNodeCount() > 0 )
                                {
                                    Tree.goToNthChild( 0 );
                                    CurrentKey = Tree.getNodeKeyForCurrentPosition();
                                }
                                if( CurrentKey != null && CurrentKey.NodeSpecies == NodeSpecies.Plain )
                                {
                                    ResponseData.SelectedNodeKey = CurrentKey;
                                }
                                break;
                        } // switch( DefaultSelect )
                    } // if( ReturnObj["selectid"] == null )
                } // if( HasResults )
                else
                {
                    Request.DefaultSelect = "root";
                }

                Tree.goToRoot();

                //Build the Respose:

                //#1: the Root node
                CswExtTree.TreeNode RootNode = new CswExtTree.TreeNode();
                ResponseData.Tree.Add( RootNode );

                RootNode.Name = _View.ViewName;
                RootNode.IsRoot = true;
                RootNode.Expanded = true;
                RootNode.Id = "root";

                //#2: the columns for the Tree Grid
                ResponseData.Columns.Add(new CswNbtGridExtJsColumn
                    {
                        ExtDataIndex = "text",
                        xtype = extJsXType.treecolumn,
                        MenuDisabled = true,
                        width = 268,
                        resizable = false,
                    });
                ResponseData.Columns.Add(new CswNbtGridExtJsColumn
                    {
                        ExtDataIndex = "nodetypeid",
                        hidden = true,
                        resizable = false,
                        xtype = extJsXType.numbercolumn,
                        MenuDisabled = true
                    });
                ResponseData.Columns.Add(new CswNbtGridExtJsColumn
                    {
                        ExtDataIndex = "objectclassid",
                        hidden = true,
                        resizable = false,
                        xtype = extJsXType.numbercolumn,
                        MenuDisabled = true
                    });

                //#3: The fields to map the columns to the data store
                ResponseData.Fields.Add(new CswNbtGridExtJsField { name = "text", type = "string" });
                ResponseData.Fields.Add(new CswNbtGridExtJsField { name = "nodetypeid", type = "number" });
                ResponseData.Fields.Add(new CswNbtGridExtJsField { name = "objectclassid", type = "number" });

                //#4: the tree
                RootNode.Children = new Collection<CswExtTree.TreeNode>();
                if( HasResults )
                {
                    Tree.goToRoot();
                    _runTreeNodesRecursive( Tree, RootNode.Children, RootNode );
                }
                else
                {
                    CswExtTree.TreeNode EmptyNode = new CswExtTree.TreeNode();
                    EmptyNode.Name = "No Results";
                    EmptyNode.IsLeaf = true;
                    EmptyNode.Id = "empty";
                    RootNode.Children.Add( EmptyNode );
                }
            }
        } // runTree()

        public static void runTree( ICswResources CswResources, Contract.Response Response, Contract.Request Request )
        {
            CswNbtResources Resources = (CswNbtResources) CswResources;
            if( null != Resources )
            {
                CswNbtView View = null;
                if( null != Request.ViewId && Request.ViewId.isSet() )
                {
                    View = Resources.ViewSelect.restoreView( Request.ViewId );
                }
                else if( null != Request.SessionViewId && Request.SessionViewId.isSet() )
                {
                    View = Resources.ViewSelect.getSessionView( Request.SessionViewId );
                }
                if( null != View )
                {
                    CswNbtSdTrees SdTrees = new CswNbtSdTrees( Resources, View );
                    SdTrees.runTree( Response.Data, Request );
                }
            }
        }

        public static void runTree( ICswResources CswResources, Contract.Response Response, string ViewName )
        {
            CswNbtResources Resources = (CswNbtResources) CswResources;
            if( null != Resources )
            {
                CswNbtView View = null;
                foreach( KeyValuePair<CswNbtViewId, CswNbtView> EnabledView in Resources.ViewSelect.getVisibleViews( false ) )
                {
                    if( EnabledView.Value.ViewName.ToLower().Trim() == ViewName.ToLower().Trim() )
                    {
                        View = EnabledView.Value;
                        break;
                    }
                }

                if( null != View )
                {
                    CswNbtSdTrees SdTrees = new CswNbtSdTrees( Resources, View );
                    SdTrees.runTree( Response.Data, new Contract.Request() );
                }
            }
        }

        //public JObject getTypes()
        //{
        //    JObject TypesJson = new JObject();
        //    TypesJson["root"] = new JObject();
        //    TypesJson["root"]["icon"] = new JObject();
        //    TypesJson["root"]["icon"]["image"] = "Images/view/viewtree.gif";
        //    TypesJson["group"] = new JObject();
        //    TypesJson["group"]["icon"] = new JObject();
        //    TypesJson["group"]["icon"]["image"] = CswNbtMetaDataObjectClass.IconPrefix16 + "folder.gif";
        //    TypesJson["default"] = "";

        //    var NodeTypes = new Dictionary<Int32, string>();
        //    ArrayList Relationships = _View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
        //    foreach( CswNbtViewRelationship Rel in Relationships )
        //    {
        //        if( Rel.SecondType == NbtViewRelatedIdType.NodeTypeId )
        //        {
        //            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
        //            if( null != NodeType && false == NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
        //            {
        //                NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
        //            }
        //        } // if( Rel.SecondType == RelatedIdType.NodeTypeId )
        //        else
        //        {
        //            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
        //            if( null != ObjectClass )
        //            {
        //                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
        //                {
        //                    if( !NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
        //                    {
        //                        NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
        //                    }
        //                }
        //            }
        //        } // else
        //    } // foreach( CswNbtViewRelationship Rel in Relationships )

        //    foreach( KeyValuePair<Int32, string> NodeType in NodeTypes )
        //    {
        //        TypesJson["nt_" + NodeType.Key] = new JObject();
        //        TypesJson["nt_" + NodeType.Key]["icon"] = new JObject();
        //        if( false == string.IsNullOrEmpty( NodeType.Value ) )
        //        {
        //            TypesJson["nt_" + NodeType.Key]["icon"]["image"] = CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.Value;
        //        }
        //    }
        //    return TypesJson;
        //} // getTypes()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
