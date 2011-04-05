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


		public XElement getTree( CswNbtView View, string IDPrefix, bool IsFirstLoad, CswNbtNodeKey ParentNodeKey, CswNbtNodeKey IncludeNodeKey, bool IncludeNodeRequired, bool UsePaging )
		{
			var ReturnNode = new XElement( "root" );
			string EmptyOrInvalid = "";

			//bool IsFirstLoad = true;
			//if( ParentNodeKey != null || IncludeNodeKey != null )
			//    IsFirstLoad = false;

			if( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List )
			{
				Int32 PageSize = Int32.MinValue;
				if( UsePaging )
					PageSize = _CswNbtResources.CurrentNbtUser.PageSize;

				ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, null, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );

				// case 21262
				if( IncludeNodeKey != null && IncludeNodeRequired && ( IncludeNodeKey.TreeKey != Tree.Key || Tree.getNodeKeyByNodeId( IncludeNodeKey.NodeId ) == null ) )
				{
					View = _CswNbtResources.MetaData.getNodeType( IncludeNodeKey.NodeTypeId ).CreateDefaultView();
					View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( IncludeNodeKey.NodeId );
					Tree = _CswNbtResources.Trees.getTreeFromView( View, true, ref ParentNodeKey, null, PageSize, IsFirstLoad, UsePaging, IncludeNodeKey, false );
				}

				if( Tree.getChildNodeCount() > 0 )
				{

					var RootNode = new XElement( "root" );
					if( IsFirstLoad && View.ViewMode == NbtViewRenderingMode.Tree )
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
						ReturnNode = new XElement( "result",
										new XElement( "tree", RootNode ),
										new XElement( "viewid", View.SessionViewId ),
										new XElement( "types", getTypes( View ).ToString() ) );
					}
					else
					{
						ReturnNode = RootNode;
					}
				} // if( Tree.getChildNodeCount() > 0 )
				else
				{
					if( IsFirstLoad )
					{
						EmptyOrInvalid = "No Results";
					}
				}
			} // if( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List )
			else
			{
				EmptyOrInvalid = "Not a Tree or List view.";
			}

			if( !string.IsNullOrEmpty( EmptyOrInvalid ) )
			{
				ReturnNode = new XElement( "root",
									new XElement( "item",
										new XAttribute( "id", "-1" ),
										new XAttribute( "rel", "root" ),
											new XElement( "content",
												new XElement( "name", EmptyOrInvalid ) ) ) );
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
								)
							) { new JProperty( "default", "" ) };

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

				CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
				CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

				string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
				string ThisNodeName = Tree.getNodeNameForCurrentPosition();
				string ThisNodeId = IDPrefix + ThisNode.NodeId.ToString();
				string ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;

				string ThisNodeState = "closed";
				if( ThisNodeKey.NodeSpecies == NodeSpecies.More )
					ThisNodeState = "leaf";

				var ParentNode = ( new XElement( "item",
										new XAttribute( "id", ThisNodeId ),
										new XAttribute( "rel", ThisNodeRel ),
										new XAttribute( "state", ThisNodeState ),
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
