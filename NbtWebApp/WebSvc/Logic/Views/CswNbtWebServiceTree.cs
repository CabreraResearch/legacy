using System;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTree
    {
        private readonly CswNbtResources _CswNbtResources;
        private CswNbtView _View;
        private string _IdPrefix;


        public CswNbtWebServiceTree( CswNbtResources CswNbtResources, CswNbtView View, string IdPrefix = "" )
        {
            _CswNbtResources = CswNbtResources;
            _IdPrefix = IdPrefix;
            _View = View;
        }

        /// <summary>
        /// Recursively iterate the tree and add child nodes according to parent hierarchy
        /// </summary>
        private void _runTreeNodesRecursive( ICswNbtTree Tree, JArray GrandParentNode, bool Recurse )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                JObject ThisNodeObj = _treeNodeJObject( Tree );
                GrandParentNode.Add( ThisNodeObj );

                if( Recurse )
                {
                    _runTreeNodesRecursive( Tree, (JArray) ThisNodeObj["children"], Recurse );
                }
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

            if( Tree.getCurrentNodeChildrenTruncated() )
            {
                JObject ThisTruncated = new JObject();
                ThisTruncated["data"] = "Results Truncated";
                ThisTruncated["icon"] = "Images/icons/truncated.gif";
                ThisTruncated["state"] = "leaf";
                GrandParentNode.Add( ThisTruncated );
            }

        } // _runTreeNodesRecursive()


        /// <summary>
        /// Generate a JObject for the tree's current node
        /// </summary>
        private JObject _treeNodeJObject( ICswNbtTree Tree )
        {
            JObject ThisNodeObj = new JObject();

            CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
            string ThisNodeName = Tree.getNodeNameForCurrentPosition();
            string ThisNodeIcon = "";
            string ThisNodeKeyString = ThisNodeKey.ToString();
            string ThisNodeId = "";

            string ThisNodeRel = "";
            bool ThisNodeLocked = false;
            bool ThisNodeDisabled = false;
            CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisNodeKey.NodeTypeId );
            switch( ThisNodeKey.NodeSpecies )
            {
                //case NodeSpecies.More:
                //    ThisNodeId = ThisNodeKey.NodeId.ToString();
                //    ThisNodeName = NodeSpecies.More.ToString() + "...";
                //    ThisNodeIcon = "Images/icons/triangle_blueS.gif";
                //    ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                //    break;
                case NodeSpecies.Plain:
                    ThisNodeId = ThisNodeKey.NodeId.ToString();
                    ThisNodeName = Tree.getNodeNameForCurrentPosition();
                    ThisNodeRel = "nt_" + ThisNodeType.FirstVersionNodeTypeId;
                    ThisNodeLocked = Tree.getNodeLockedForCurrentPosition();
                    ThisNodeDisabled = ( false == Tree.getNodeIncludedForCurrentPosition() );
                    if( false == string.IsNullOrEmpty( ThisNodeType.IconFileName ) )
                    {
                        ThisNodeIcon = CswNbtMetaDataObjectClass.IconPrefix16 + ThisNodeType.IconFileName;
                    }
                    break;
                case NodeSpecies.Group:
                    ThisNodeRel = "group";
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

            ThisNodeObj["data"] = ThisNodeName;
            ThisNodeObj["icon"] = ThisNodeIcon;
            ThisNodeObj["attr"] = new JObject();
            ThisNodeObj["attr"]["id"] = _IdPrefix + ThisNodeKeyString;   // This is the only unique string for this node in this tree
            //ThisNodeObj["attr"]["id"] = _IdPrefix + ThisNodeId;
            ThisNodeObj["attr"]["rel"] = ThisNodeRel;
            ThisNodeObj["attr"]["state"] = ThisNodeState;
            ThisNodeObj["attr"]["species"] = ThisNodeKey.NodeSpecies.ToString();
            ThisNodeObj["attr"]["nodeid"] = ThisNodeId;
            ThisNodeObj["attr"]["nodekey"] = ThisNodeKeyString;
            ThisNodeObj["attr"]["locked"] = ThisNodeLocked.ToString().ToLower();
            if( ThisNodeDisabled )
            {
                ThisNodeObj["attr"]["disabled"] = ThisNodeDisabled.ToString().ToLower();
            }
            CswNbtNodeKey ParentKey = Tree.getNodeKeyForParentOfCurrentPosition();
            if( ParentKey.NodeSpecies != NodeSpecies.Root )
            {
                ThisNodeObj["attr"]["parentkey"] = ParentKey.ToString();
            }

            //if( "leaf" != ThisNodeState && Tree.getChildNodeCount() > 0 )
            //{
            ThisNodeObj["state"] = ThisNodeState;
            ThisNodeObj["children"] = new JArray();
            ThisNodeObj["childcnt"] = Tree.getChildNodeCount().ToString();
            //}
            return ThisNodeObj;
        } // _treeNodeJObject()

        public JObject runTree( CswPrimaryKey IncludeNodeId, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool IncludeInQuickLaunch, string DefaultSelect, string AccessedByObjClassId = "" )
        {
            JObject ReturnObj = new JObject();

            if( null != _View ) //&& ( _View.ViewMode == NbtViewRenderingMode.Tree || _View.ViewMode == NbtViewRenderingMode.List ) )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                _View.SaveToCache( IncludeInQuickLaunch );

                if( IncludeNodeId != null && IncludeNodeId.PrimaryKey != Int32.MinValue && IncludeNodeKey == null )
                {
                    IncludeNodeKey = Tree.getNodeKeyByNodeId( IncludeNodeId );
                    if( IncludeNodeRequired && IncludeNodeKey == null )
                    {
                        CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.Nodes[IncludeNodeId].getNodeType();
                        _View = IncludeKeyNodeType.CreateDefaultView();
                        _View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                        _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeId );
                        _View.SaveToCache( IncludeInQuickLaunch ); // case 22713
                        ReturnObj["newviewid"] = _View.SessionViewId.ToString();
                        ReturnObj["newviewmode"] = _View.ViewMode.ToString();
                        Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                    }
                }
                if( IncludeNodeRequired && IncludeNodeKey != null && Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null )
                {
                    CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId );
                    _View = IncludeKeyNodeType.CreateDefaultView();
                    _View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                    _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
                    _View.SaveToCache( IncludeInQuickLaunch ); // case 22713
                    ReturnObj["newviewid"] = _View.SessionViewId.ToString();
                    ReturnObj["newviewmode"] = _View.ViewMode.ToString();
                    Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );
                }

                Tree.goToRoot();
                bool HasResults = ( Tree.getChildNodeCount() > 0 );
                ReturnObj["result"] = HasResults.ToString().ToLower();
                ReturnObj["types"] = getTypes();
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
                            //ReturnObj["selectid"] = _IdPrefix + IncludeNodeKey.NodeId.ToString();
                            ReturnObj["selectid"] = _IdPrefix + IncludeNodeKey.ToString();
                        }
                    }
                    if( ReturnObj["selectid"] == null )
                    {
                        switch( DefaultSelect )
                        {
                            case "none":
                                break;
                            case "root":
                                ReturnObj["selectid"] = _IdPrefix + "root";
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
                                    // ReturnObj["selectid"] = _IdPrefix + Tree.getNodeIdForCurrentPosition().ToString();
                                    ReturnObj["selectid"] = _IdPrefix + CurrentKey.ToString();
                                }
                                break;
                        } // switch( DefaultSelect )
                    } // if( ReturnObj["selectid"] == null )
                } // if( HasResults )
                else
                {
                    ReturnObj["selectid"] = _IdPrefix + "root";
                }

                Tree.goToRoot();
                ReturnObj["root"] = new JObject();
                ReturnObj["root"]["data"] = _View.ViewName;
                ReturnObj["root"]["attr"] = new JObject();
                ReturnObj["root"]["attr"]["id"] = _IdPrefix + "root";
                ReturnObj["root"]["attr"]["rel"] = "root";
                ReturnObj["root"]["attr"]["disabled"] = false == Tree.getNodeIncludedForCurrentPosition();
                //Tree.goToRoot();
                //ReturnObj["attr"]["nodekey"] = Tree.getNodeKeyForCurrentPosition().ToString();
                ReturnObj["root"]["state"] = "open";
                ReturnObj["root"]["children"] = new JArray();

                if( HasResults )
                {
                    Tree.goToRoot();
                    _runTreeNodesRecursive( Tree, (JArray) ReturnObj["root"]["children"], true );
                }
                else
                {
                    JObject NoResultsObj = new JObject();
                    NoResultsObj["state"] = "leaf";
                    NoResultsObj["data"] = "No Results";
                    ( (JArray) ReturnObj["root"]["children"] ).Add( NoResultsObj );
                }
                //_wsTreeOfView.saveTreeToCache( Tree );
                //_View.SaveToCache( IncludeInQuickLaunch );

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
        //public JObject fetchTreeFirstLevel( Int32 PageSize, Int32 PageNo, bool ForSearch )
        //{
        //    JObject ReturnObj = new JObject();

        //    ICswNbtTree Tree = _wsTreeOfView.getTreeFromCache();

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

        //                JObject ThisNodeObj = _treeNodeJObject( Tree );
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

        //public JObject fetchTreeChildren( Int32 Level, Int32 ParentRangeStart, Int32 ParentRangeEnd, bool ForSearch )
        //{
        //    JObject ReturnObj = new JObject();

        //    ICswNbtTree Tree = _wsTreeOfView.getTreeFromCache();

        //    if( Tree != null )
        //    {
        //        JArray RootArray = new JArray();
        //        ReturnObj["tree"] = RootArray;

        //        Int32 NodeCountStart = PageSize * PageNo;
        //        Int32 NodeCountEnd = PageSize * PageNo;
        //        bool More = false;
        //        if( Tree.getChildNodeCount() > 0 )
        //        {
        //            Collection<CswNbtNodeKey> NodeKeys = _wsTreeOfView.getNextPageOfNodes( Tree, Level, ParentRangeStart, ParentRangeEnd );
        //                JObject ThisNodeObj = _treeNodeJObject( Tree );

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
        public JObject getTree( bool IsFirstLoad, CswNbtNodeKey ParentNodeKey, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging, bool ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
        {
            JObject ReturnObj = new JObject();
            JArray RootArray = new JArray();
            ReturnObj["viewmode"] = _View.ViewMode.ToString();
            ReturnObj["tree"] = RootArray;

            string EmptyOrInvalid = "No Results";
            // Case 21699: Show empty tree for search
            bool ValidView = ( null != _View && ( _View.ViewMode == NbtViewRenderingMode.Tree || _View.ViewMode == NbtViewRenderingMode.List ) );
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
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( _View, false, false, false );

                // case 21262
                if( IncludeNodeKey != null && IncludeNodeRequired && ( //IncludeNodeKey.TreeKey != Tree.Key || 
                                                                        Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null ) )
                {
                    CswNbtMetaDataNodeType IncludeKeyNodeType = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId );
                    _View = IncludeKeyNodeType.CreateDefaultView();
                    _View.ViewName = "New " + IncludeKeyNodeType.NodeTypeName;
                    _View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
                    _View.SaveToCache( true ); // case 22713
                    Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, _View, true, false, false );
                }

                if( ( Tree.getChildNodeCount() > 0 ) )
                {
                    if( IsFirstLoad && ( _View.ViewMode == NbtViewRenderingMode.Tree || ForSearch ) )
                    {
                        JArray ChildArray = new JArray();
                        JObject FirstObj = new JObject();
                        RootArray.Add( FirstObj );
                        FirstObj["data"] = _View.ViewName;
                        FirstObj["attr"] = new JObject();
                        FirstObj["attr"]["id"] = _IdPrefix + "root";
                        FirstObj["attr"]["rel"] = "root";
                        FirstObj["attr"]["nodekey"] = Tree.getNodeKeyForCurrentPosition().ToString();
                        FirstObj["state"] = "open";
                        FirstObj["children"] = ChildArray;

                        _runTreeNodesRecursive( Tree, ChildArray, true );
                    }
                    else // List, or non-top level of Tree
                    {
                        _runTreeNodesRecursive( Tree, RootArray, true );
                    }
                } // if( Tree.getChildNodeCount() > 0 )
                else
                {
                    ShowEmpty = ( IsFirstLoad ); // else return an empty <result/> for junior most node on tree
                }

                if( IsFirstLoad )
                {
                    _View.SaveToCache( IncludeInQuickLaunch );
                    ReturnObj["viewid"] = _View.SessionViewId.ToString();
                }
            } // else if( !ShowEmpty )

            string ViewName = string.Empty;
            string ViewId = string.Empty;
            ReturnObj["types"] = new JObject();
            if( ValidView )
            {
                ViewName = _View.ViewName;
                ViewId = _View.ViewId.ToString();
                ReturnObj["types"] = getTypes();
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

        public JObject getTypes()
        {
            JObject TypesJson = new JObject();
            TypesJson["root"] = new JObject();
            TypesJson["root"]["icon"] = new JObject();
            TypesJson["root"]["icon"]["image"] = "Images/view/viewtree.gif";
            TypesJson["group"] = new JObject();
            TypesJson["group"]["icon"] = new JObject();
            TypesJson["group"]["icon"]["image"] = CswNbtMetaDataObjectClass.IconPrefix16 + "folder.gif";
            TypesJson["default"] = "";

            var NodeTypes = new Dictionary<Int32, string>();
            ArrayList Relationships = _View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
            foreach( CswNbtViewRelationship Rel in Relationships )
            {
                if( Rel.SecondType == NbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
                    if( null != NodeType && false == NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                    {
                        NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
                    }
                } // if( Rel.SecondType == RelatedIdType.NodeTypeId )
                else
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
                    if( null != ObjectClass )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
                        {
                            if( !NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
                            {
                                NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
                            }
                        }
                    }
                } // else
            } // foreach( CswNbtViewRelationship Rel in Relationships )

            foreach( KeyValuePair<Int32, string> NodeType in NodeTypes )
            {
                TypesJson["nt_" + NodeType.Key] = new JObject();
                TypesJson["nt_" + NodeType.Key]["icon"] = new JObject();
                if( false == string.IsNullOrEmpty( NodeType.Value ) )
                {
                    TypesJson["nt_" + NodeType.Key]["icon"]["image"] = CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.Value;
                }
            }
            return TypesJson;
        } // getTypes()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
