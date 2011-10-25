using System;
using System.Collections;
using System.Collections.Generic;
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


        public JObject getTree( CswNbtView View, string IdPrefix, bool IsFirstLoad, CswNbtNodeKey ParentNodeKey, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging, bool ShowEmpty, bool ForSearch, bool IncludeInQuickLaunch )
        {
            JObject ReturnObj = new JObject();
            JArray RootArray = new JArray();
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

                CswNbtViewRelationship ChildRelationshipToStartWith = null;
                //if( IncludeNodeKey != null )
                //    ChildRelationshipToStartWith = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( IncludeNodeKey.ViewNodeUniqueId );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );

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
                        FirstObj["state"] = "open";
                        FirstObj["children"] = ChildArray;

                        _runTreeNodesRecursive( View, Tree, IdPrefix, ChildArray );
                    }
                    else // List, or non-top level of Tree
                    {
                        _runTreeNodesRecursive( View, Tree, IdPrefix, RootArray );
                    }

                    if( IsFirstLoad )
                    {
                        View.SaveToCache( IncludeInQuickLaunch );
                        ReturnObj["viewid"] = View.SessionViewId.ToString();
                    }
                } // if( Tree.getChildNodeCount() > 0 )
                else
                {
                    ShowEmpty = ( IsFirstLoad ); // else return an empty <result/> for junior most node on tree
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

        /// <summary>
        /// Recursively iterate the tree and add child nodes according to parent hierarchy
        /// </summary>
        private void _runTreeNodesRecursive( CswNbtView View, ICswNbtTree Tree, string IdPrefix, JArray GrandParentNode )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                JObject ThisNodeObj = new JObject();
                GrandParentNode.Add( ThisNodeObj );

                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeIcon = "";
                string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
                string ThisNodeId = "";
                string ThisNodeRel = "";
                CswNbtNode ThisNode;
				bool ThisNodeLocked = false;
                switch( ThisNodeKey.NodeSpecies )
                {
					case NodeSpecies.More:
                        ThisNode = Tree.getNodeForCurrentPosition();
                        ThisNodeId = IdPrefix + ThisNode.NodeId.ToString();
                        ThisNodeName = NodeSpecies.More.ToString() + "...";
                        ThisNodeIcon = "triangle_blueS.gif";
                        ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;
                        break;
					case NodeSpecies.Plain:
                        ThisNode = Tree.getNodeForCurrentPosition();
                        ThisNodeId = IdPrefix + ThisNode.NodeId.ToString();
                        ThisNodeName = ThisNode.NodeName;
                        ThisNodeIcon = ThisNode.IconFileName;
                        ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;
							ThisNodeLocked = ThisNode.Locked;
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

                if( "leaf" != ThisNodeState && Tree.getChildNodeCount() > 0 )
                {
                    JArray ThisNodeChildren = new JArray();
                    ThisNodeObj["children"] = ThisNodeChildren;
                    ThisNodeObj["state"] = ThisNodeState;
                    _runTreeNodesRecursive( View, Tree, IdPrefix, ThisNodeChildren );
                }
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

        } // _runTreeNodesRecursive()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
