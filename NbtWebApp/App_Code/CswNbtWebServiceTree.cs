using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.SessionState;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceTree
	{
		private CswNbtResources _CswNbtResources;
		
		public CswNbtWebServiceTree( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		public string getTree( CswNbtView View, string IDPrefix )
		{
			string ret = @"<item id=""-1""><content><name>No results</name></content></item>";

			ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );
			XElement TreeXml = _runTreeNodesRecursive( Tree, IDPrefix, View.ViewName );

			if( Tree.getChildNodeCount() > 0 )
			{
				var OuterXML = new XElement( "result",
				                                  new XElement( "tree", TreeXml ),
				                                  new XElement( "types", _getTypes( View ) ) );
				ret = OuterXML.ToString();
			}

			return ret;
		} // getTree()

		public string _getTypes( CswNbtView View )
		{
			JObject TypesJson = new JObject( 
				                    new JProperty( "root",
			                            new JObject(
			                                new JProperty( "icon",
			                                    new JObject(
			                                     	new JProperty( "image", "Images/view/viewtree.gif" )
			                                    )
			                                )
			                            )
								    )
								);
			TypesJson.Add( new JProperty( "default", "" ) );

			//var NodeTypes = new Collection<CswNbtMetaDataNodeType>();
			var NodeTypes = new Dictionary<Int32, string>();
			ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
			foreach( CswNbtViewRelationship Rel in Relationships )
			{
				if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
				{
					CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
					if( !NodeTypes.ContainsKey(NodeType.FirstVersionNodeTypeId) )
					{
						NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
					}
				}
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
				}
			}

			foreach( KeyValuePair<Int32,string > NodeType in NodeTypes )
			{
				JProperty NodeTypeJProp = new JProperty( "nt_" + NodeType.Key.ToString(),
				                                         new JObject(
				                                         	new JProperty( "icon",
				                                         	               new JObject(
				                                         	               	new JProperty( "image", "Images/icons/" + NodeType.Value )
				                                         	               	)
				                                         		)
				                                         	)
												);
				if( null != TypesJson.Property( NodeTypeJProp.Name ) )
				{
					TypesJson.Add( NodeTypeJProp );
				}
			}
			string ret = TypesJson.ToString();
			return ret;
		}


		private XElement _runTreeNodesRecursive( ICswNbtTree Tree, string IDPrefix, string ViewName )
		{
			XElement RootElement = new XElement( "root",
										new XElement("item",
											new XAttribute( "id", IDPrefix + "root" ),
											new XAttribute("rel","root"),
												new XElement("content"),
													new XElement("name", ViewName)
												) 
											);
			//string ret = string.Empty;
			for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
			{
				Tree.goToNthChild( c );

				CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
				string ThisNodeName = Tree.getNodeNameForCurrentPosition();
				string ThisNodeId = IDPrefix + ThisNode.NodeId.ToString();
				CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

				RootElement.Add( new XElement( "item",
												  new XAttribute( "id", ThisNodeId ),
												  new XAttribute( "rel", ThisNode.NodeType.FirstVersionNodeTypeId.ToString() ),
												  new XAttribute( "cswnodekey", ThisNodeKey.ToString() ), 
														new XElement( "content" ,
															new XElement( "name" , ThisNodeName )
															)
														)
									);

				Tree.goToParentNode();
			}
			return RootElement;
		}



	} // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
