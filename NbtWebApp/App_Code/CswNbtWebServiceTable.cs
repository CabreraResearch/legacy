using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceTable
	{
		private readonly CswNbtResources _CswNbtResources;
		public CswNbtWebServiceTable( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		} //ctor

		public JObject getTable( CswNbtView View, CswNbtNode SelectedNode )
		{
			JObject ret = new JObject();
			ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

			XDocument XDoc = XDocument.Parse( Tree.getRawTreeXml() );
			foreach( XElement TreeElm in XDoc.Elements() )                        // NbtTree
			{
				foreach( XElement RootElm in TreeElm.Elements() )                 // NbtNode (root)
				{
					foreach( XElement NodeElm in RootElm.Elements() )             // NbtNode
					{
						CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeElm.Attribute( "nodeid" ).Value ) );
						ret[NodeId.ToString()] = new JObject();
						ret[NodeId.ToString()]["nodename"] = NodeElm.Attribute( "nodename" ).Value;
						ret[NodeId.ToString()]["nodeid"] = NodeId.ToString();
						ret[NodeId.ToString()]["nodekey"] = NodeElm.Attribute( "key" ).Value;
						ret[NodeId.ToString()]["locked"] = NodeElm.Attribute( "locked" ).Value;
						ret[NodeId.ToString()]["props"] = new JObject();
						CswNbtMetaDataNodeType NodeType= _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32(NodeElm.Attribute("nodetypeid").Value));
						if( NodeType != null )
						{
							// default image, overridden below
							ret[NodeId.ToString()]["thumbnailurl"] = "Images/icons/" + NodeType.IconFileName;
						}

						foreach( XElement PropElm in NodeElm.Descendants( "NbtNodeProp" ) )
						{
							Int32 NodeTypePropId = CswConvert.ToInt32( PropElm.Attribute( "nodetypepropid" ).Value );
							Int32 JctNodePropId = CswConvert.ToInt32( PropElm.Attribute( "jctnodepropid" ).Value );
							CswPropIdAttr PropId = new CswPropIdAttr( NodeId, NodeTypePropId );
							string FieldType = PropElm.Attribute( "fieldtype" ).Value;

							// Special case: Image becomes thumbnail
							if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image.ToString() )
							{
								ret[NodeId.ToString()]["thumbnailurl"] = CswNbtNodePropImage.makeImageUrl( JctNodePropId, NodeId, NodeTypePropId );
							}
							else
							{
								ret[NodeId.ToString()]["props"][PropId.ToString()] = new JObject();
								ret[NodeId.ToString()]["props"][PropId.ToString()]["propname"] = PropElm.Attribute( "name" ).Value;
								ret[NodeId.ToString()]["props"][PropId.ToString()]["gestalt"] = PropElm.Attribute( "gestalt" ).Value;
							}
						} // foreach( XElement PropElm in NodeElm.Elements() )
					} // foreach( XElement NodeElm in RootElm.Elements() ) 
				} // foreach( XElement RootElm in TreeElm.Elements() )
			} // foreach( XElement TreeElm in XDoc.Elements() )  

			return ret;

		} // class CswNbtWebServiceTable

	} // namespace ChemSW.Nbt.WebServices
}