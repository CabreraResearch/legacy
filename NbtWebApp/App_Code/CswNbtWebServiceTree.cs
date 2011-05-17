using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
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


		public XElement getTree( CswNbtView View, string IDPrefix, bool IsFirstLoad, CswNbtNodeKey ParentNodeKey, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging, bool ShowEmpty, bool ForSearch )
		{
			var ReturnNode = new XElement( "result" );
            string EmptyOrInvalid = "No Results";
		    // Case 21699: Show empty tree for search
		    bool ValidView = ( null != View && ( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List ) );
		    string ViewName = string.Empty;
			CswNbtSessionDataId SessionViewId = new CswNbtSessionDataId();
			//bool IsFirstLoad = true;
			//if( ParentNodeKey != null || IncludeNodeKey != null )
			//    IsFirstLoad = false;
            
            if( !ValidView )
            {
                ShowEmpty = true;
                EmptyOrInvalid = "Not a Tree or List view.";
            }
            else if ( !ShowEmpty )
			{
				Int32 PageSize = Int32.MinValue;
				if( UsePaging )
					PageSize = _CswNbtResources.CurrentNbtUser.PageSize;

				CswNbtViewRelationship ChildRelationshipToStartWith = null;
				//if( IncludeNodeKey != null )
				//    ChildRelationshipToStartWith = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( IncludeNodeKey.ViewNodeUniqueId );

				ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, ChildRelationshipToStartWith, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );

				// case 21262
				if( IncludeNodeKey != null && IncludeNodeRequired && ( IncludeNodeKey.TreeKey != Tree.Key || Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null ) )
				{
					View = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId ).CreateDefaultView();
					View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
					Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, null, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );
				}

                if( ( Tree.getChildNodeCount() > 0 ) )
                {

                    var RootNode = new XElement( "root" );
                    if( IsFirstLoad && ( View.ViewMode == NbtViewRenderingMode.Tree || ForSearch ) )
                    {
                        var RootItemNode = new XElement( "item",
                                            new XAttribute( "id", IDPrefix + "root" ),
                                            new XAttribute( "rel", "root" ),
                                            new XElement( "content",
                                                        new XElement( "name", View.ViewName ) ) );
                        RootNode.Add( RootItemNode );
                        _runTreeNodesRecursive( Tree, IDPrefix, RootItemNode );
                    }
                    else // List, or non-top level of Tree
                    {
                        _runTreeNodesRecursive( Tree, IDPrefix, RootNode );
                    }

                    if( IsFirstLoad )
                    {
                        ReturnNode.Add( new XElement( "tree", RootNode ),
                                        new XElement( "viewid", View.SessionViewId.get() ),
                                        new XElement( "types", getTypes( View ).ToString() ) );
                    }
                    else
                    {
                        ReturnNode = RootNode;
                    }
                } // if( Tree.getChildNodeCount() > 0 )
                else
                {
                    ShowEmpty = ( IsFirstLoad ); // else return an empty <result/> for junior most node on tree
                    //    if( IsFirstLoad )
                    //    {
                    //        EmptyOrInvalid = "No Results";
                    //    }
                }
			
            } // else if( !ShowEmpty )

            XElement Types = new XElement( "types" );
            if( ValidView )
            {
                ViewName = View.ViewName;
                SessionViewId = View.SessionViewId;
                Types.Add( getTypes( View ).ToString() );
            }

			if( ShowEmpty )
			{
                ReturnNode.Add( new XElement( "tree",
                    new XElement( "root",
                                    new XElement( "item",
                                        new XAttribute( "id", "-1" ),
                                        new XAttribute( "rel", "root" ),
                                            new XElement( "content",
                                                new XElement( "name", ViewName ) ),

                                            new XElement( "item",
                                            new XAttribute( "id", "-1" ),
                                            new XAttribute( "rel", "child" ),
                                                new XElement( "content",
                                                    new XElement( "name", EmptyOrInvalid ) ) ) ) )
                                        ),
                                new XElement( "viewid", SessionViewId.get() ),
                                Types );
			}
			return ReturnNode;
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
		private void _runTreeNodesRecursive( ICswNbtTree Tree, string IDPrefix, XElement GrandParentNode )
		{
			for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
			{
				Tree.goToNthChild( c );

				CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
				string ThisNodeName = Tree.getNodeNameForCurrentPosition();
				string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
				string ThisNodeId = "";
				string ThisNodeRel = "";

				if( ThisNodeKey.NodeSpecies == NodeSpecies.Plain || ThisNodeKey.NodeSpecies == NodeSpecies.More )
				{
					CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
					ThisNodeId = IDPrefix + ThisNode.NodeId.ToString();
					ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;
				}
				else if( ThisNodeKey.NodeSpecies == NodeSpecies.Group )
				{
					ThisNodeId = "";
					ThisNodeRel = "group";
				}

				string ThisNodeState = "closed";
				if( ThisNodeKey.NodeSpecies == NodeSpecies.More )
					ThisNodeState = "leaf";

				var ParentNode = ( new XElement( "item",
										new XAttribute( "id", ThisNodeId ),
										new XAttribute( "rel", ThisNodeRel ),
										new XAttribute( "state", ThisNodeState ),
										new XAttribute( "species", ThisNodeKey.NodeSpecies.ToString() ),
										new XAttribute( "cswnbtnodekey", ThisNodeKeyString ),
											new XElement( "content",
												new XElement( "name", ThisNodeName )
												)
											)
									);
				if( Tree.getChildNodeCount() > 0 )
				{
					// XElement ChildNode = _runTreeNodesRecursive()
					_runTreeNodesRecursive( Tree, IDPrefix, ParentNode );
				}

				GrandParentNode.Add( ParentNode );
				Tree.goToParentNode();
			} // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

		} // _runTreeNodesRecursive()

	} // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
