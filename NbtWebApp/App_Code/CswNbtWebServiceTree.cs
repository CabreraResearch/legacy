using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.Caching;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTree
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceTree( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Recursively iterate the tree and add child nodes according to parent hierarchy
        /// </summary>
        private void _runTreeNodesRecursive( CswNbtView View, ICswNbtTree Tree, string IdPrefix, JArray GrandParentNode, bool Recurse )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                JObject ThisNodeObj = _treeNodeJObject( View, Tree, IdPrefix );
                GrandParentNode.Add( ThisNodeObj );

                if( Recurse )
                {
                    _runTreeNodesRecursive( View, Tree, IdPrefix, (JArray) ThisNodeObj["children"], Recurse );
                }
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
        } // _runTreeNodesRecursive()


        /// <summary>
        /// Generate a JObject for the tree's current node
        /// </summary>
        private JObject _treeNodeJObject( CswNbtView View, ICswNbtTree Tree, string IdPrefix )
        {
            JObject ThisNodeObj = new JObject();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeIcon = "";
            string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
            string ThisNodeId = "";
            string ThisNodeRel = "";
            bool ThisNodeLocked = false;
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            switch( ThisNodeKey.NodeSpecies )
            {
                case NodeSpecies.More:
                    ThisNodeId = IdPrefix + ThisNodeKey.NodeId.ToString();
                    ThisNodeName = NodeSpecies.More.ToString() + "...";
                    ThisNodeIcon = "triangle_blueS.gif";
                    ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                    break;
                case NodeSpecies.Plain:
                    ThisNodeId = IdPrefix + ThisNodeKey.NodeId.ToString();
                    ThisNodeName = Tree.getNodeNameForCurrentPosition();
                    ThisNodeIcon = ThisNodeType.IconFileName;
                    ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                    ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();

                    break;
                case NodeSpecies.Group:
                    ThisNodeRel = "group";
                    break;
            }

            CswNbtViewNode ThisNodeViewNode = View.FindViewNodeByUniqueId( ThisNodeKey.ViewNodeUniqueId );

            string ThisNodeState = "closed";
            if( ThisNodeKey.NodeSpecies == NodeSpecies.More ||
                View.ViewMode == NbtViewRenderingMode.List ||
                ( Tree.IsFullyPopulated && Tree.getChildNodeCount() == 0 ) ||
                ( ThisNodeViewNode != null && ThisNodeViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ).Count == 0 ) )
            {
                ThisNodeState = "leaf";
            }

            ThisNodeObj["data"] = ThisNodeName;
            ThisNodeObj["icon"] = "Images/icons/" + ThisNodeIcon;
            ThisNodeObj["attr"] = new JObject();
            ThisNodeObj["attr"]["id"] = ThisNodeId;
            ThisNodeObj["attr"]["rel"] = ThisNodeRel;
            ThisNodeObj["attr"]["state"] = ThisNodeState;
            ThisNodeObj["attr"]["species"] = ThisNodeKey.NodeSpecies.ToString();
            ThisNodeObj["attr"]["cswnbtnodekey"] = ThisNodeKeyString;
            ThisNodeObj["attr"]["locked"] = ThisNodeLocked.ToString().ToLower();
            CswNbtNodeKey ParentKey = Tree.getNodeKeyForParentOfCurrentPosition();
            if( ParentKey.NodeSpecies != NodeSpecies.Root )
            {
                ThisNodeObj["attr"]["parentkey"] = wsTools.ToSafeJavaScriptParam( ParentKey.ToString() );
            }

            if( "leaf" != ThisNodeState && Tree.getChildNodeCount() > 0 )
            {
                ThisNodeObj["state"] = ThisNodeState;
                ThisNodeObj["children"] = new JArray();
                ThisNodeObj["childcnt"] = Tree.getChildNodeCount().ToString();
            }
            return ThisNodeObj;
        } // _treeNodeJObject()

        public JObject runTree( CswNbtView View, string IdPrefix, CswPrimaryKey IncludeNodeId, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool IncludeInQuickLaunch )
        {
            JObject ReturnObj = new JObject();

            //string CacheTreeName = _CacheTreeName( View, IdPrefix );

            //_CswNbtResources.CswSuperCycleCache.delete( CacheTreeName );

            if( null != View && ( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List ) )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false );
                View.SaveToCache( IncludeInQuickLaunch );

                if( IncludeNodeId != null && IncludeNodeId.PrimaryKey != Int32.MinValue && IncludeNodeKey == null )
                {
                    IncludeNodeKey = Tree.getNodeKeyByNodeId( IncludeNodeId );
                    if( IncludeNodeRequired && IncludeNodeKey == null )
                    {
                        CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.Nodes[IncludeNodeId].NodeType;
                        View = IncludeKeyNodeType.CreateDefaultView();
                        View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                        View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeId );
                        View.SaveToCache( IncludeInQuickLaunch ); // case 22713
                        ReturnObj["newviewid"] = View.SessionViewId.ToString();
                        Tree = _CswNbtResources.Trees.getTreeFromView( View, false );
                    }
                }
                if( IncludeNodeRequired && IncludeNodeKey != null && Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null )
                {
                    CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId );
                    View = IncludeKeyNodeType.CreateDefaultView();
                    View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                    View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
                    View.SaveToCache( IncludeInQuickLaunch ); // case 22713
                    ReturnObj["newviewid"] = View.SessionViewId.ToString();
                    Tree = _CswNbtResources.Trees.getTreeFromView( View, false );
                }

                Tree.goToRoot();
                bool HasResults = (Tree.getChildNodeCount() > 0 );
                ReturnObj["result"] = HasResults.ToString().ToLower();
                ReturnObj["types"] = getTypes( View );
                //ReturnObj["pagesize"] = _CswNbtResources.CurrentNbtUser.PageSize.ToString();

                if( HasResults )
                {
                    // Determine the default selected node:
                    // If the requested node to select is on the tree, return it.
                    // If the requested node to select is not on the tree, return the first child of the root.
                    if( IncludeNodeKey != null )
                    {
                        Tree.makeNodeCurrent( IncludeNodeKey );
                        if( Tree.isCurrentNodeDefined() )
                        {
                            ReturnObj["selectid"] = IdPrefix + IncludeNodeKey.NodeId.ToString();
                        }
                    }
                    if( ReturnObj["selectid"] == null )
                    {
                        Tree.goToRoot();
                        Tree.goToNthChild( 0 );
                        ReturnObj["selectid"] = IdPrefix + Tree.getNodeIdForCurrentPosition().ToString();
                    }
                }

                ReturnObj["root"] = new JObject();
                ReturnObj["root"]["data"] = View.ViewName;
                ReturnObj["root"]["attr"] = new JObject();
                ReturnObj["root"]["attr"]["id"] = IdPrefix + "root";
                ReturnObj["root"]["attr"]["rel"] = "root";
                //Tree.goToRoot();
                //ReturnObj["attr"]["cswnbtnodekey"] = Tree.getNodeKeyForCurrentPosition().ToString();
                ReturnObj["root"]["state"] = "open";
                ReturnObj["root"]["children"] = new JArray();

                Tree.goToRoot();
                _runTreeNodesRecursive( View, Tree, IdPrefix, (JArray) ReturnObj["root"]["children"], true );

                //_CswNbtResources.CswSuperCycleCache.put( CacheTreeName, Tree );
                //View.SaveToCache( IncludeInQuickLaunch );
            }
            return ReturnObj;
        } // runTree()


        ///// <summary>
        ///// Fetch a page of nodes out of a tree
        ///// </summary>
        ///// <param name="View">View from which the Tree was created</param>
        ///// <param name="Cache">Storage mechanism for Tree</param>
        ///// <param name="IdPrefix">Prefix for Tree ID</param>
        ///// <param name="Level">Level of tree to populate</param>
        ///// <param name="ParentRangeStart">Parent number on previous level to start (inclusive)</param>
        ///// <param name="ParentRangeEnd">Parent number on previous level to end (inclusive)</param>
        ///// <param name="PageNo">Page of nodes on this level, if number of nodes exceeds pagesize</param>
        ///// <param name="PageSize">Size of pages</param>
        ///// <param name="ForSearch">True if view is from a search</param>
        //public JObject fetchTreeFirstLevel( CswNbtView View, string IdPrefix, Int32 PageSize, Int32 PageNo, bool ForSearch )
        //{
        //    JObject ReturnObj = new JObject();

        //    ICswNbtTree Tree = _getCachedTree( View, IdPrefix );

        //    if( Tree != null )
        //    {
        //        JArray RootArray = new JArray();
        //        ReturnObj["tree"] = RootArray;

        //        Int32 NodeCountStart = Int32.MinValue;
        //        Int32 NodeCountEnd = Int32.MinValue;
        //        if( Tree.getChildNodeCount() > 0 )
        //        {
        //            Tree.goToRoot();
        //            for( Int32 c = PageSize * PageNo; c < PageSize * ( PageNo + 1 ) && c < Tree.getChildNodeCount(); c++ )
        //            {
        //                Tree.goToNthChild( c );

        //                if( NodeCountStart == Int32.MinValue )
        //                {
        //                    NodeCountStart = Tree.getNodeKeyForCurrentPosition().NodeCount;
        //                }
        //                NodeCountEnd = Tree.getNodeKeyForCurrentPosition().NodeCount;

        //                JObject ThisNodeObj = _treeNodeJObject( View, Tree, IdPrefix );
        //                RootArray.Add( ThisNodeObj );

        //                Tree.goToParentNode();
        //            }
        //        } // if( Tree.getChildNodeCount() > 0 )

        //        ReturnObj["nodecountstart"] = NodeCountStart.ToString();
        //        ReturnObj["nodecountend"] = NodeCountEnd.ToString();
        //        ReturnObj["more"] = ( PageSize * ( PageNo + 1 ) <= Tree.getChildNodeCount() ).ToString().ToLower();
        //    }
        //    else
        //    {
        //        ReturnObj["tree"] = new JArray( new JObject() );
        //        //ReturnObj["tree"][0] = new JObject();
        //        //ReturnObj["tree"][0]["data"] = ViewName;
        //        ReturnObj["tree"][0]["attr"] = new JObject();
        //        //ReturnObj["tree"][0]["attr"]["viewid"] = ViewId;
        //        ReturnObj["tree"][0]["state"] = "leaf";
        //        ReturnObj["tree"][0]["children"] = new JArray( new JObject() );
        //        ReturnObj["tree"][0]["children"][0] = "No Results";
        //    }
        //    return ReturnObj;
        //} // fetchTreeFirstLevel

        //public JObject fetchTreeChildren( CswNbtView View, string IdPrefix, Int32 Level, Int32 ParentRangeStart, Int32 ParentRangeEnd, Int32 PageSize, Int32 PageNo, bool ForSearch )
        //{
        //    JObject ReturnObj = new JObject();

        //    ICswNbtTree Tree = _getCachedTree( View, IdPrefix );

        //    if( Tree != null )
        //    {
        //        JArray RootArray = new JArray();
        //        ReturnObj["tree"] = RootArray;

        //        Int32 NodeCountStart = PageSize * PageNo;
        //        Int32 NodeCountEnd = PageSize * PageNo;
        //        bool More = false;
        //        if( Tree.getChildNodeCount() > 0 )
        //        {
        //            Collection<CswNbtNodeKey> NodeKeys = _getNextPageOfNodes( Tree, Level, ParentRangeStart, ParentRangeEnd, PageSize, PageNo, ref More );

        //            foreach( CswNbtNodeKey NodeKey in NodeKeys )
        //            {
        //                Tree.makeNodeCurrent( NodeKey );
        //                NodeCountEnd++;
        //                JObject ThisNodeObj = _treeNodeJObject( View, Tree, IdPrefix );
        //                RootArray.Add( ThisNodeObj );
        //            } // foreach( CswNbtNodeKey NodeKey in NodeKeys )
        //        } // if( Tree.getChildNodeCount() > 0 )

        //        ReturnObj["nodecountstart"] = NodeCountStart.ToString();
        //        ReturnObj["nodecountend"] = NodeCountEnd.ToString();
        //        ReturnObj["more"] = More.ToString().ToLower();
        //        // ReturnObj["types"] = getTypes( View );
        //    }
        //    else
        //    {
        //        ReturnObj["tree"] = new JArray( new JObject() );
        //        //ReturnObj["tree"][0] = new JObject();
        //        //ReturnObj["tree"][0]["data"] = ViewName;
        //        ReturnObj["tree"][0]["attr"] = new JObject();
        //        //ReturnObj["tree"][0]["attr"]["viewid"] = ViewId;
        //        ReturnObj["tree"][0]["state"] = "leaf";
        //        ReturnObj["tree"][0]["children"] = new JArray( new JObject() );
        //        ReturnObj["tree"][0]["children"][0] = "No Results";
        //    }
        //    return ReturnObj;
        //} // fetchTree()

        /// <summary>
        /// Deprecated
        /// </summary>
        public JObject getTree( CswNbtView View, string IdPrefix, bool IsFirstLoad, CswNbtNodeKey ParentNodeKey, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging, bool ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
        {
            JObject ReturnObj = new JObject();
            JArray RootArray = new JArray();
            ReturnObj["viewmode"] = View.ViewMode.ToString();
            ReturnObj["tree"] = RootArray;

            string EmptyOrInvalid = "No Results";
            // Case 21699: Show empty tree for search
            bool ValidView = ( null != View && ( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List ) );
            //bool IsFirstLoad = true;
            //if( ParentNodeKey != null || IncludeNodeKey != null )
            //    IsFirstLoad = false;

            if( !ValidView )
            {
                ShowEmpty = true;
                EmptyOrInvalid = "Not a Tree or List view.";
            }
            else if( !ShowEmpty )
            {
                Int32 PageSize = Int32.MinValue;
                if( UsePaging )
                    PageSize = _CswNbtResources.CurrentNbtUser.PageSize;

                //CswNbtViewRelationship ChildRelationshipToStartWith = null;
                //if( IncludeNodeKey != null )
                //    ChildRelationshipToStartWith = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( IncludeNodeKey.ViewNodeUniqueId );

                //ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false );

                // case 21262
                if( IncludeNodeKey != null && IncludeNodeRequired && ( //IncludeNodeKey.TreeKey != Tree.Key || 
                                                                        Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null ) )
                {
                    CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId );
                    View = IncludeKeyNodeType.CreateDefaultView();
                    View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                    View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
                    View.SaveToCache( true ); // case 22713
                    Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, null, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );
                }

                if( ( Tree.getChildNodeCount() > 0 ) )
                {
                    if( IsFirstLoad && ( View.ViewMode == NbtViewRenderingMode.Tree || ForSearch ) )
                    {
                        JArray ChildArray = new JArray();
                        JObject FirstObj = new JObject();
                        RootArray.Add( FirstObj );
                        FirstObj["data"] = View.ViewName;
                        FirstObj["attr"] = new JObject();
                        FirstObj["attr"]["id"] = IdPrefix + "root";
                        FirstObj["attr"]["rel"] = "root";
                        FirstObj["attr"]["cswnbtnodekey"] = wsTools.ToSafeJavaScriptParam( Tree.getNodeKeyForCurrentPosition().ToString() );
                        FirstObj["state"] = "open";
                        FirstObj["children"] = ChildArray;

                        _runTreeNodesRecursive( View, Tree, IdPrefix, ChildArray, true );
                    }
                    else // List, or non-top level of Tree
                    {
                        _runTreeNodesRecursive( View, Tree, IdPrefix, RootArray, true );
                    }
                } // if( Tree.getChildNodeCount() > 0 )
                else
                {
                    ShowEmpty = ( IsFirstLoad ); // else return an empty <result/> for junior most node on tree
                }

                if( IsFirstLoad )
                {
                    View.SaveToCache( IncludeInQuickLaunch );
                    ReturnObj["viewid"] = View.SessionViewId.ToString();
                }
            } // else if( !ShowEmpty )

            string ViewName = string.Empty;
            string ViewId = string.Empty;
            ReturnObj["types"] = new JObject();
            if( ValidView )
            {
                ViewName = View.ViewName;
                ViewId = View.ViewId.ToString();
                ReturnObj["types"] = getTypes( View );
            }

            if( ShowEmpty )
            {
                ReturnObj["tree"] = new JArray( new JObject() );
                //ReturnObj["tree"][0] = new JObject();
                ReturnObj["tree"][0]["data"] = ViewName;
                ReturnObj["tree"][0]["attr"] = new JObject();
                ReturnObj["tree"][0]["attr"]["viewid"] = ViewId;
                ReturnObj["tree"][0]["state"] = "open";
                ReturnObj["tree"][0]["children"] = new JArray( new JObject() );
                ReturnObj["tree"][0]["children"][0] = EmptyOrInvalid;
            }

            return ReturnObj;
        } // getTree()

        public JObject getTypes( CswNbtView View )
        {
            JObject TypesJson = new JObject();
            TypesJson["root"] = new JObject();
            TypesJson["root"]["icon"] = new JObject();
            TypesJson["root"]["icon"]["image"] = "Images/view/viewtree.gif";
            TypesJson["group"] = new JObject();
            TypesJson["group"]["icon"] = new JObject();
            TypesJson["group"]["icon"]["image"] = "Images/icons/group.gif";
            TypesJson["default"] = "";

            var NodeTypes = new Dictionary<Int32, string>();
            ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
            foreach( CswNbtViewRelationship Rel in Relationships )
            {
                if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
                    if( !NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                    {
                        NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
                    }
                } // if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                else
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
                    foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
                    {
                        if( !NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                        {
                            NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
                        }
                    }
                } // else
            } // foreach( CswNbtViewRelationship Rel in Relationships )

            foreach( KeyValuePair<Int32, string> NodeType in NodeTypes )
            {
                TypesJson["nt_" + NodeType.Key] = new JObject();
                TypesJson["nt_" + NodeType.Key]["icon"] = new JObject();
                TypesJson["nt_" + NodeType.Key]["icon"]["image"] = "Images/icons/" + NodeType.Value;
            }
            return TypesJson;
        } // getTypes()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
