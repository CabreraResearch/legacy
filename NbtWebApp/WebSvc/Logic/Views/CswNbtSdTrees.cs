using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Grid.ExtJs;
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
                public bool ExpandAll = false;

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
                [DataMember( EmitDefaultValue = false, IsRequired = false )]
                public bool UseCheckboxes = false;

                [DataMember]
                public List<string> PropsToShow = new List<string>();

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
                        Columns = new Collection<CswExtJsGridColumn>();
                        Fields = new Collection<CswExtJsGridField>();
                        ViewMode = CswEnumNbtViewRenderingMode.Tree;
                        PageSize = 50;
                    }

                    [DataMember]
                    public string Name;

                    [DataMember]
                    public Collection<CswExtTree.TreeNode> Tree;

                    public Int32 treecount()
                    {
                        return _treecount( Tree );
                    }

                    private Int32 _treecount( Collection<CswExtTree.TreeNode> Coll )
                    {
                        Int32 ret = 0;
                        foreach( CswExtTree.TreeNode t in Coll )
                        {
                            ret++;
                            if( t.Children != null && t.Children.Count > 0 )
                            {
                                ret += _treecount( t.Children );
                            }
                        }
                        return ret;
                    }

                    [DataMember]
                    public Collection<CswExtJsGridColumn> Columns;

                    [DataMember]
                    public Collection<CswExtJsGridField> Fields;

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
                    public CswEnumNbtViewRenderingMode ViewMode { get; set; }

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
        private void _runTreeNodesRecursive( ICswNbtTree Tree, Collection<CswExtTree.TreeNode> TreeNodes, CswExtTree.TreeNode ParentNode, CswNbtSdTrees.Contract.Request Request, Int32 MaxNodes, ref Int32 Count )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c += 1 )
            {
                Tree.goToNthChild( c );
                if( Count < MaxNodes || MaxNodes == Int32.MinValue )
                {
                    CswExtTree.TreeNode TreeNode = _getTreeNode( Tree, ParentNode, Request );
                    TreeNodes.Add( TreeNode );
                    Count++;

                    if( null != TreeNode.Children )
                    {
                        _runTreeNodesRecursive( Tree, TreeNode.Children, TreeNode, Request, MaxNodes, ref Count );
                    }
                }
                Tree.goToParentNode();
            }
            if( Tree.getCurrentNodeChildrenTruncated() )
            {
                CswExtTree.TreeNode TruncatedTreeNode = _getTreeNode( Tree, ParentNode, null );
                TruncatedTreeNode.Name = "Results Truncated";
                TruncatedTreeNode.IsLeaf = true;
                TruncatedTreeNode.Icon = "Images/icons/truncated.gif";
                TruncatedTreeNode.Id = TruncatedTreeNode.Id + "_truncated";
                TruncatedTreeNode.NodeId = "";
                TreeNodes.Insert( 0, TruncatedTreeNode );
            }
        } // _runTreeNodesRecursive()


        /// <summary>
        /// Generate a JObject for the tree's current node
        /// </summary>
        private CswExtTree.TreeNode _getTreeNode( ICswNbtTree Tree, CswExtTree.TreeNode Parent, CswNbtSdTrees.Contract.Request Request )
        {
            CswExtTree.TreeNode Ret;
            if( null != Request && Request.UseCheckboxes )
            {
                Ret = new CswExtTree.TreeNodeWithCheckbox();
            }
            else
            {
                Ret = new CswExtTree.TreeNode();
            }

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeIcon = "";
            string ThisNodeKeyString = ThisNodeKey.ToString();
            string ThisNodeId = "";
            int ThisNodeTypeId = int.MinValue;
            int ThisObjectClassId = int.MinValue;

            bool ThisNodeLocked = false;
            bool ThisNodeDisabled = false;
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            switch( ThisNodeKey.NodeSpecies )
            {
                case CswEnumNbtNodeSpecies.Plain:
                    ThisNodeId = ThisNodeKey.NodeId.ToString();
                    ThisNodeName = Tree.getNodeNameForCurrentPosition();
                    ThisNodeTypeId = ThisNodeType.FirstVersionNodeTypeId;
                    ThisObjectClassId = ThisNodeType.ObjectClassId;
                    ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();
                    ThisNodeDisabled = ( false == Tree.getNodeIncludedForCurrentPosition() );
                    //if( false == string.IsNullOrEmpty( ThisNodeType.IconFileName ) )
                    //{
                    //    ThisNodeIcon = CswNbtMetaDataObjectClass.IconPrefix16 + ThisNodeType.IconFileName;
                    //}
                    ThisNodeIcon = CswNbtMetaDataObjectClass.IconPrefix16 + Tree.getNodeIconForCurrentPosition();
                    break;
                case CswEnumNbtNodeSpecies.Group:
                    Ret.CssClass = "folder";
                    break;
            }

            CswNbtViewNode ThisNodeViewNode = _View.FindViewNodeByUniqueId( ThisNodeKey.ViewNodeUniqueId );

            string ThisNodeState = "closed";
            if( ThisNodeKey.NodeSpecies == CswEnumNbtNodeSpecies.More ||
                _View.ViewMode == CswEnumNbtViewRenderingMode.List ||
                ( Tree.IsFullyPopulated && Tree.getChildNodeCount() == 0 ) ||
                ( ThisNodeViewNode != null && ThisNodeViewNode.GetChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ).Count == 0 ) )
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

            if( null != Request )
            {
                Ret.Expanded = Request.ExpandAll;
            }

            if( int.MinValue != ThisNodeTypeId )
            {
                Ret.NodeTypeId = ThisNodeTypeId.ToString();
            }
            if( int.MinValue != ThisObjectClassId )
            {
                Ret.ObjectClassId = ThisObjectClassId.ToString();
            }

            Ret.NodeSpecies = ThisNodeKey.NodeSpecies.ToString();
            Ret.NodeId = ThisNodeId;
            Ret.IsLocked = ThisNodeLocked;
            if( ThisNodeDisabled )
            {
                Ret.IsDisabled = true;
                Ret.CssClass = "disabled";
            }

            if( null != Parent && false == string.IsNullOrEmpty( Parent.Path ) )
            {
                Ret.Path = Parent.Path;
            }
            else
            {
                Ret.ParentId = "root";
                Ret.Path = "|root";
            }
            if( false == Tree.isCurrentPositionRoot() )
            {
                CswNbtNodeKey ParentKey = Tree.getNodeKeyForParentOfCurrentPosition();
                if( ParentKey.NodeSpecies != CswEnumNbtNodeSpecies.Root )
                {
                    Ret.ParentId = ParentKey.ToString();
                }
            }

            Ret.Path += "|" + Ret.Id;

            if( Tree.getChildNodeCount() > 0 )
            {
                Ret.Children = new Collection<CswExtTree.TreeNode>();
            }
            else
            {
                Ret.Children = null;
            }

            Collection<CswNbtTreeNodeProp> ThisNodeProps = Tree.getChildNodePropsOfNode();
            CswNbtViewRoot.forEachProperty EachNodeProp = ( ViewProp ) =>
                                                 {
                                                     foreach( CswNbtTreeNodeProp NodeProp in ThisNodeProps )
                                                     {
                                                         if( NodeProp.PropName.ToLower().Trim() == ViewProp.Name.ToLower().Trim() )
                                                         {
                                                             Ret.data[new CswExtJsGridDataIndex( _View.ViewName, ViewProp.Name.ToLower().Trim() )] = NodeProp.Gestalt;
                                                         }
                                                     }
                                                 };

            _View.Root.eachRelationship( relationshipCallBack : null, propertyCallBack : EachNodeProp );

            //ThisNodeObj["childcnt"] = Tree.getChildNodeCount().ToString();


            return Ret;

        } // _treeNodeJObject()

        public void runTree( Contract.Response.ResponseData ResponseData, Contract.Request Request, Int32 ResultsLimit = Int32.MinValue, Int32 MaxNodes = Int32.MinValue )
        {
            ResponseData.Tree = ResponseData.Tree ?? new Collection<CswExtTree.TreeNode>();
            ICswNbtTree Tree = null;
            string RootName = string.Empty;
            if( null != _View )
            {
                Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false, ResultsLimit );
                _View.SaveToCache( Request.IncludeInQuickLaunch );
                RootName = _View.ViewName;
            }

            CswPrimaryKey IncludeNodeId = null;
            CswNbtNodeKey SelectKey = null;
            Int32 IncludeNodeTypeId = Int32.MinValue;
            if( null != Request.IncludeNodeKey )
            {
                IncludeNodeId = Request.IncludeNodeKey.NodeId;
                IncludeNodeTypeId = Request.IncludeNodeKey.NodeTypeId;
                if( null != Tree )
                {
                    Tree.makeNodeCurrent( Request.IncludeNodeKey );
                    if( Tree.isCurrentNodeDefined() )
                    {
                        SelectKey = Request.IncludeNodeKey;
                    }
                }
            }
            else if( CswTools.IsPrimaryKey( Request.IncludeNodeId ) )
            {
                IncludeNodeId = Request.IncludeNodeId;
                CswNbtNode IncludeNode = _CswNbtResources.Nodes[IncludeNodeId];
                if( null != IncludeNode )
                {
                    IncludeNodeTypeId = IncludeNode.NodeTypeId;
                }
                if( null != Tree )
                {
                    SelectKey = Tree.getNodeKeyByNodeId( IncludeNodeId );
                }
            }

            if( ( CswTools.IsPrimaryKey( IncludeNodeId ) && IncludeNodeTypeId != Int32.MinValue ) &&
               ( Tree == null ||
                ( Request.IncludeNodeRequired && SelectKey == null ) ) )
            {
                CswNbtMetaDataNodeType IncludeNodeType = _CswNbtResources.MetaData.getNodeType( IncludeNodeTypeId );
                if( null != IncludeNodeType )
                {
                    _View = IncludeNodeType.CreateDefaultView( false );
                    _View.ViewName = IncludeNodeType.NodeTypeName;
                    _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeId );
                    _View.SaveToCache( Request.IncludeInQuickLaunch ); // case 22713
                    RootName = _View.ViewName;

                    Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                }
            }

            bool HasResults = false;
            if( null != Tree )
            {
                Tree.goToRoot();
                HasResults = ( Tree.getChildNodeCount() > 0 );
                //ReturnObj["result"] = HasResults.ToString().ToLower();
                //ReturnObj["types"] = getTypes();
                ResponseData.PageSize = _CswNbtResources.CurrentNbtUser.PageSize;

                ResponseData.SelectedNodeKey = null;
                if( HasResults )
                {
                    // Determine the default selected node:
                    // If the requested node to select is on the tree, return it.
                    // If the requested node to select is not on the tree, return the first child of the root.
                    if( SelectKey != null )
                    {
                        Tree.makeNodeCurrent( SelectKey );
                        if( Tree.isCurrentNodeDefined() )
                        {
                            ResponseData.SelectedNodeKey = SelectKey;
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
                                       CurrentKey.NodeSpecies != CswEnumNbtNodeSpecies.Plain &&
                                       Tree.getChildNodeCount() > 0 )
                                {
                                    Tree.goToNthChild( 0 );
                                    CurrentKey = Tree.getNodeKeyForCurrentPosition();
                                }
                                if( CurrentKey != null && CurrentKey.NodeSpecies == CswEnumNbtNodeSpecies.Plain )
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
            }
            //Build the Response:

            ResponseData.Name = RootName;

            //#1: the Root node
            CswExtTree.TreeNode RootNode = new CswExtTree.TreeNode();
            ResponseData.Tree.Add( RootNode );

            RootNode.Name = RootName;
            RootNode.IsRoot = true;
            RootNode.Expanded = true;
            RootNode.Path = "|root";
            RootNode.Id = "root";
            RootNode.Icon = "Images/view/viewtree.gif";

            //#2: the columns for the Tree Grid
            ResponseData.Columns.Add( new CswExtJsGridColumn
                {
                    dataIndex = new CswExtJsGridDataIndex( _View.ViewName, "text" ),
                    xtype = CswEnumExtJsXType.treecolumn,
                    MenuDisabled = true,
                    width = 269,
                    header = "Tree",
                    resizable = false,
                } );
            ResponseData.Columns.Add( new CswExtJsGridColumn
                {
                    dataIndex = new CswExtJsGridDataIndex( _View.ViewName, "nodetypeid" ),
                    header = "NodeTypeId",
                    hidden = true,
                    resizable = false,
                    width = 0,
                    xtype = CswEnumExtJsXType.gridcolumn,
                    MenuDisabled = false
                } );
            ResponseData.Columns.Add( new CswExtJsGridColumn
                {
                    dataIndex = new CswExtJsGridDataIndex( _View.ViewName, "objectclassid" ),
                    header = "ObjectClassId",
                    hidden = true,
                    resizable = false,
                    width = 0,
                    xtype = CswEnumExtJsXType.gridcolumn,
                    MenuDisabled = false
                } );
            ResponseData.Columns.Add( new CswExtJsGridColumn
            {
                dataIndex = new CswExtJsGridDataIndex( _View.ViewName, "nodeid" ),
                header = "NodeId",
                hidden = true,
                resizable = false,
                width = 0,
                xtype = CswEnumExtJsXType.gridcolumn,
                MenuDisabled = false
            } );
            ResponseData.Columns.Add( new CswExtJsGridColumn
                {
                    dataIndex = new CswExtJsGridDataIndex( _View.ViewName, "disabled" ),
                    header = "Disabled",
                    hidden = true,
                    resizable = false,
                    width = 0,
                    xtype = CswEnumExtJsXType.booleancolumn,
                    MenuDisabled = false
                } );


            //#3: The fields to map the columns to the data store
            ResponseData.Fields.Add( new CswExtJsGridField { name = "text", type = "string" } );
            ResponseData.Fields.Add( new CswExtJsGridField { name = "nodetypeid", type = "string" } );
            ResponseData.Fields.Add( new CswExtJsGridField { name = "objectclassid", type = "string" } );
            ResponseData.Fields.Add( new CswExtJsGridField { name = "nodeid", type = "string" } );
            ResponseData.Fields.Add( new CswExtJsGridField { name = "disabled", type = "bool" } );

            //#4: View Properties are columns now too
            Collection<string> UniqueColumnNames = new Collection<string>()
                                                       {
                                                           "text", "nodetypeid", "objectclassid", "nodeid", "disabled"
                                                       };
            CswNbtViewRoot.forEachProperty AddProp = ( ViewProperty ) =>
                                                         {
                                                             string PropName = ViewProperty.Name.ToLower().Trim();
                                                             bool HideProp = ( null != Request.PropsToShow && false == Request.PropsToShow.Contains( PropName ) );
                                                             if( false == UniqueColumnNames.Contains( PropName ) )
                                                             {
                                                                 UniqueColumnNames.Add( PropName );

                                                                 CswExtJsGridColumn Col = new CswExtJsGridColumn
                                                                                              {
                                                                                                  dataIndex = new CswExtJsGridDataIndex( _View.ViewName, PropName ),
                                                                                                  header = ViewProperty.Name,
                                                                                                  hidden = HideProp,
                                                                                                  resizable = false,
                                                                                                  width = ViewProperty.Width * 7,
                                                                                                  xtype = CswEnumExtJsXType.gridcolumn,
                                                                                                  MenuDisabled = false
                                                                                              };
                                                                 CswExtJsGridField Fld = new CswExtJsGridField { name = PropName, type = "string" };
                                                                 Fld.dataIndex = Col.dataIndex;

                                                                 ResponseData.Columns.Add( Col );
                                                                 ResponseData.Fields.Add( Fld );
                                                             }
                                                         };
            _View.Root.eachRelationship( relationshipCallBack : null, propertyCallBack : AddProp );


            //#5: the tree
            RootNode.Children = new Collection<CswExtTree.TreeNode>();
            if( HasResults )
            {
                Tree.goToRoot();
                int count = 0;
                _runTreeNodesRecursive( Tree, RootNode.Children, RootNode, Request, MaxNodes, ref count );

                if( Int32.MinValue != MaxNodes && count >= MaxNodes && RootNode.Children[0].Name != "Results Truncated" )
                {
                    CswExtTree.TreeNode TruncatedTreeNode = _getTreeNode( Tree, RootNode, null );
                    TruncatedTreeNode.Name = "Results Truncated";
                    TruncatedTreeNode.IsLeaf = true;
                    TruncatedTreeNode.Icon = "Images/icons/truncated.gif";
                    TruncatedTreeNode.Id = TruncatedTreeNode.Id + "_truncated";
                    TruncatedTreeNode.NodeId = "";
                    RootNode.Children.Insert( 0, TruncatedTreeNode );
                }
            }
            else
            {
                CswExtTree.TreeNode EmptyNode = new CswExtTree.TreeNode();
                EmptyNode.Name = "No Results";
                EmptyNode.IsLeaf = true;
                EmptyNode.Selected = true;
                EmptyNode.Id = "empty";
                EmptyNode.ParentId = RootNode.Id;
                EmptyNode.Path = RootNode.Path + "|empty";
                RootNode.Children.Add( EmptyNode );
            }
            //}
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
                //if( null != View )
                //{
                CswNbtSdTrees SdTrees = new CswNbtSdTrees( Resources, View );
                SdTrees.runTree( Response.Data, Request );
                //}
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


        public static void getLocationTree( ICswResources CswResources, Contract.Response Response, object Request )
        {
            CswNbtResources Resources = (CswNbtResources) CswResources;

            CswNbtView View = new CswNbtView( Resources );
            CswNbtObjClassLocation.makeLocationsTreeView( ref View, Resources );
            View.SaveToCache( false, false );
            Response.Data.ViewId = View.SessionViewId;


        }//getLocationTree()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
