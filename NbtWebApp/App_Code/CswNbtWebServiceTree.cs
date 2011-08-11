using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            JArray RootArray = new JArray();
            JProperty DataProp = new JProperty( "tree", RootArray );
            JObject ReturnObj = new JObject( DataProp );

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
                        RootArray.Add( new JObject(
                                           new JProperty( "data", View.ViewName ),
                                           new JProperty( "attr",
                                                          new JObject(
                                                              new JProperty( "id", IdPrefix + "root" ),
                                                              new JProperty( "rel", "root" )
                                                              )
                                               ),
                                           new JProperty( "children", ChildArray )
                                           ) );
                        _runTreeNodesRecursive( View, Tree, IdPrefix, ChildArray );
                    }
                    else // List, or non-top level of Tree
                    {
                        _runTreeNodesRecursive( View, Tree, IdPrefix, RootArray );
                    }

                    if( IsFirstLoad )
                    {
                        View.SaveToCache( IncludeInQuickLaunch );
                        //ReturnObj.Add( new JProperty( "tree", RootProp ) );
                        ReturnObj.Add( new JProperty( "viewid", View.SessionViewId.ToString() ) );
                        ReturnObj.Add( new JProperty( "types", getTypes( View ).ToString() ) );
                    }
                } // if( Tree.getChildNodeCount() > 0 )
                else
                {
                    ShowEmpty = ( IsFirstLoad ); // else return an empty <result/> for junior most node on tree
                }

            } // else if( !ShowEmpty )

            JProperty Types = new JProperty( "types" );
            string ViewName = string.Empty;
            string ViewId = string.Empty;
            if( ValidView )
            {
                ViewName = View.ViewName;
                ViewId = View.ViewId.ToString();
                Types.Value = getTypes( View );
            }

            if( ShowEmpty )
            {
                ReturnObj.Add( new JProperty( "tree",
                                               new JArray(
                                                   new JObject(
                                                       new JProperty( "data", ViewName ),
                                                       new JProperty( "attr",
                                                                      new JObject(
                                                                          new JProperty( "viewid", ViewId ) ) ),
                                                       new JProperty( "children",
                                                                      new JArray(
                                                                          EmptyOrInvalid
                                                                          )
                                                           ) )

                                                   ) ) );
                ReturnObj.Add( Types );
            }
            return ReturnObj;
        } // getTree()

        public JObject getTypes( CswNbtView View )
        {
            var TypesJson = new JObject(
                                new JProperty( "root",
                                    new JObject(
                                        new JProperty( "icon",
                                            new JObject(
                                                new JProperty( "image", "Images/view/viewtree.gif" )
                                            )
                                        )
                                    )
                                ),
                                new JProperty( "group",
                                    new JObject(
                                        new JProperty( "icon",
                                            new JObject(
                                                new JProperty( "image", "Images/icons/group.gif" )
                                            )
                                        )
                                    )
                                ),
                                new JProperty( "default", "" )
                            );

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

            foreach( var JProperty in NodeTypes
                                .Select( NodeType => new JProperty( "nt_" + NodeType.Key,
                                                        new JObject( new JProperty( "icon",
                                                            new JObject( new JProperty( "image", "Images/icons/" + NodeType.Value ) ) ) ) ) )
                                .Where( NodeTypeJProp => null == TypesJson.Property( NodeTypeJProp.Name ) ) )
            {
                TypesJson.Add( JProperty );
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

                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
                string ThisNodeId = "";
                string ThisNodeRel = "";

                switch( ThisNodeKey.NodeSpecies )
                {
                    case NodeSpecies.More:
                    case NodeSpecies.Plain:
                        {
                            CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                            ThisNodeId = IdPrefix + ThisNode.NodeId.ToString();
                            ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;

                        }
                        break;
                    case NodeSpecies.Group:
                        ThisNodeId = "";
                        ThisNodeRel = "group";
                        break;
                }

                string ThisNodeState = "closed";
                if( ThisNodeKey.NodeSpecies == NodeSpecies.More ||
                    View.ViewMode == NbtViewRenderingMode.List ||
                    ( Tree.IsFullyPopulated && Tree.getChildNodeCount() == 0 ) )
                {
                    ThisNodeState = "leaf";
                }

                JArray ThisNodeChildren = new JArray();
                GrandParentNode.Add( new JObject(
                                          new JProperty( "data", ThisNodeName ),
                                          new JProperty( "attr",
                                              new JObject(
                                                    new JProperty( "id", ThisNodeId ),
                                                    new JProperty( "rel", ThisNodeRel ),
                                                    new JProperty( "state", ThisNodeState ),
                                                    new JProperty( "species", ThisNodeKey.NodeSpecies.ToString() ),
                                                    new JProperty( "cswnbtnodekey", ThisNodeKeyString )
                                                  )
                                              ),
                                          new JProperty( "children", ThisNodeChildren )
                                    ) );
                if( Tree.getChildNodeCount() > 0 )
                {
                    // XElement ChildNode = _runTreeNodesRecursive()
                    _runTreeNodesRecursive( View, Tree, IdPrefix, ThisNodeChildren );
                }
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

        } // _runTreeNodesRecursive()

    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
