using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceTabsAndProps
	{
		public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue };

		private readonly CswNbtResources _CswNbtResources;
	    private readonly ICswNbtUser _ThisUser;

        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
            _ThisUser = _CswNbtResources.CurrentNbtUser;
		}

		private CswNbtNode _getNode( string NodeId, string NodeKey )
		{
			CswNbtNode Node = null;
			if( !string.IsNullOrEmpty( NodeKey ) )
			{
				CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				Node = _CswNbtResources.Nodes[RealNodeKey];
			}
			else if( !string.IsNullOrEmpty( NodeId ) )
			{
				CswPrimaryKey RealNodeId = new CswPrimaryKey();
				RealNodeId.FromString( NodeId );
				Node = _CswNbtResources.Nodes[RealNodeId];
			}
			return Node;
		} // _getNode()

		public XElement getTabs( NodeEditMode EditMode, string NodeId, string NodeKey, Int32 NodeTypeId )
		{
			XElement TabsNode = new XElement( "tabs" );
			if( EditMode == NodeEditMode.AddInPopup && NodeTypeId != Int32.MinValue )
			{
				CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
				TabsNode.Add( new XElement( "tab",
											new XAttribute( "id", "newtab" ),
											new XAttribute( "name", "Add New " + NodeType.NodeTypeName ) ) );
			}
			else
			{
				CswNbtNode Node = _getNode( NodeId, NodeKey );
				if( Node != null )
				{
					foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
					{
						TabsNode.Add( new XElement( "tab",
											new XAttribute( "id", Tab.TabId ),
											new XAttribute( "name", Tab.TabName ) ) );
					}
				} // if( Node != null )
			} // if-else( EditMode == NodeEditMode.AddInPopup )
			return TabsNode;
		} // getTabs()


		/// <summary>
		/// Returns XML for all properties in a given tab
		/// </summary>
		public XmlDocument getProps( NodeEditMode EditMode, string NodeId, string NodeKey, string TabId, Int32 NodeTypeId )
		{
			XmlDocument PropXmlDoc = new XmlDocument();
			XElement PropsElement = new XElement( "props" );
			CswXmlDocument.SetDocumentElement( PropXmlDoc, "props" );

			if( EditMode == NodeEditMode.AddInPopup )
			{
			    CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );

			    foreach( CswNbtMetaDataNodeTypeProp Prop in Node.NodeType.NodeTypeProps
                                                                         .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                         .Where( Prop => Prop.EditProp( Node, _ThisUser, true ) ) )
			    {
			        _addProp( PropXmlDoc, EditMode, Node, Prop );
			    }
			}
			else
			{
				//CswPrimaryKey NodePk = new CswPrimaryKey();
				//NodePk.FromString( NodePkString );
				//if( !string.IsNullOrEmpty( NodeKey ) )
				//{
				//    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				//    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
				//    {
				//        CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];

				CswNbtNode Node = _getNode( NodeId, NodeKey );
				if( Node != null )
				{
					// case 21209
					if( Node.NodeSpecies == NodeSpecies.Plain )
					{
						CswNbtActUpdatePropertyValue PropUpdater = new CswNbtActUpdatePropertyValue( _CswNbtResources );
						PropUpdater.UpdateNode( Node, true );
						Node.postChanges( false );
					}

					CswNbtMetaDataNodeTypeTab Tab = Node.NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
					foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder
                                                                   .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                   .Where( Prop => Prop.ShowProp( Node, _ThisUser ) ) )
					{
					    _addProp( PropXmlDoc, EditMode, Node, Prop );
					}
				}
				//}
			} // if-else( EditMode == NodeEditMode.AddInPopup )
			return PropXmlDoc;
		} // getProps()


		/// <summary>
		/// Returns XML for a single property and its conditional properties
		/// </summary>
		public XmlDocument getSingleProp( NodeEditMode EditMode, string NodeId, string NodeKey, string PropIdFromXml, Int32 NodeTypeId, string NewPropXml )
		{
			XmlDocument PropXmlDoc = new XmlDocument();
			CswXmlDocument.SetDocumentElement( PropXmlDoc, "props" );
			CswNbtNode Node = null;
			if( EditMode == NodeEditMode.AddInPopup )
			{
				Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
			}
			else
			{
				//CswPrimaryKey NodePk = new CswPrimaryKey();
				//NodePk.FromString( NodePkString );
				//if( !string.IsNullOrEmpty( NodeKey ) )
				//{
				//    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				//    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
				//    {
				//        Node = _CswNbtResources.Nodes[NbtNodeKey];
				//    }
				//}
				Node = _getNode( NodeId, NodeKey );
			}

			if( Node != null )
			{
				// case 21209
				if( Node.NodeSpecies == NodeSpecies.Plain )
				{
					CswNbtActUpdatePropertyValue PropUpdater = new Actions.CswNbtActUpdatePropertyValue( _CswNbtResources );
					PropUpdater.UpdateNode( Node, true );
					Node.postChanges( false );
				}

				if( NewPropXml != string.Empty )
				{
					// for prop filters, update node prop value but don't save the change
					XmlDocument XmlDoc = new XmlDocument();
					XmlDoc.LoadXml( NewPropXml );
					_applyPropXml( Node, XmlDoc.DocumentElement );
				}

				CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdFromXml );
				CswNbtMetaDataNodeTypeProp Prop = Node.NodeType.getNodeTypeProp( PropIdAttr.NodeTypePropId );
				_addProp( PropXmlDoc, EditMode, Node, Prop );

				if( NewPropXml != string.Empty )
				{
					//Node.Rollback();
				}
			}

			return PropXmlDoc;
		} // getProp()


		private void _addProp( XmlDocument XmlDoc, NodeEditMode EditMode, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			if( EditMode == NodeEditMode.AddInPopup )
			{
				_makePropXml( XmlDoc.DocumentElement, Node, Prop, Prop.DisplayRowAdd, Prop.DisplayColAdd );
			}
			else
			{
				XmlNode PropXmlNode = _makePropXml( XmlDoc.DocumentElement, Node, Prop, Prop.DisplayRow, Prop.DisplayColumn );

				// Handle conditional properties
				XmlNode SubPropsXmlNode = null;
				foreach( CswNbtMetaDataNodeTypeProp FilterProp in Prop.NodeTypeTab.NodeTypePropsByDisplayOrder )
				{
					if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
					{
						CswXmlDocument.AppendXmlAttribute( PropXmlNode, "hassubprops", "true" );
						if( SubPropsXmlNode == null )
						{
							SubPropsXmlNode = CswXmlDocument.AppendXmlNode( PropXmlNode, "subprops" );
						}
						XmlNode FilterPropXml = _makePropXml( SubPropsXmlNode, Node, FilterProp, FilterProp.DisplayRow, FilterProp.DisplayColumn );

						// Hide those for whom the filter doesn't match
						// (but we need the XML node to be there to store the value, for client-side changes)
						CswXmlDocument.AppendXmlAttribute( FilterPropXml, "display", FilterProp.CheckFilter( Node ).ToString().ToLower() );

					} // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
				} // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
			} // if-else( EditMode == NodeEditMode.AddInPopup )
		} // addProp()


        private XmlNode _makePropXml( XmlNode ParentXmlNode, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 Row, Int32 Column )
		{
			XmlNode PropXmlNode = CswXmlDocument.AppendXmlNode( ParentXmlNode, "prop" );

			CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

			CswPropIdAttr PropIdAttr = null;
			if( Node.NodeId != null )
			{
				PropIdAttr = new CswPropIdAttr( Node, Prop );
			}
			else
			{
				PropIdAttr = new CswPropIdAttr( null, Prop );
			}
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "id", PropIdAttr.ToString() );

			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "name", Prop.PropNameWithQuestionNo );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "helptext", Prop.HelpText );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "fieldtype", Prop.FieldType.FieldType.ToString() );
			if( Prop.ObjectClassProp != null )
			{
				CswXmlDocument.AppendXmlAttribute( PropXmlNode, "ocpname", Prop.ObjectClassProp.PropName );
			}
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "displayrow", Row.ToString() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "displaycol", Column.ToString() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "required", Prop.IsRequired.ToString().ToLower() );
            bool IsReadOnly = ( Prop.ReadOnly || PropWrapper.ReadOnly ||
				!_CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, Prop.NodeType.NodeTypeId, Node, Prop ) );

            CswXmlDocument.AppendXmlAttribute( PropXmlNode, "readonly", IsReadOnly.ToString().ToLower() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "gestalt", PropWrapper.Gestalt.Replace( "\"", "&quot;" ) );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "copyable", Prop.IsCopyable().ToString().ToLower() );

			PropWrapper.ToXml( PropXmlNode );

			return PropXmlNode;
		} // _makePropXml()

		public bool moveProp( string PropIdAttr, Int32 NewRow, Int32 NewColumn, NodeEditMode EditMode )
		{
			bool ret = false;
			CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
			Int32 NodeTypePropId = PropId.NodeTypePropId;
			if( NodeTypePropId != Int32.MinValue && NewRow > 0 && NewColumn > 0 )
			{
				CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
				if( EditMode == NodeEditMode.AddInPopup )
				{
					Prop.DisplayColAdd = NewColumn;
					Prop.DisplayRowAdd = NewRow;
				}
				else
				{
					Prop.DisplayColumn = NewColumn;
					Prop.DisplayRow = NewRow;
				}
				ret = true;
			}
			return ret;
		} // moveProp()

		public JObject saveProps( NodeEditMode EditMode, string NodeId, string NodeKey, string NewPropsXml, Int32 NodeTypeId, CswNbtView View )
		{
			JObject ret = null;
			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.LoadXml( NewPropsXml );

			CswNbtNode Node = null;
			CswNbtNodeKey NbtNodeKey = null;
			if( EditMode == NodeEditMode.AddInPopup )
			{
				Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
			}
			else
			{
				//CswPrimaryKey NodePk = new CswPrimaryKey();
				//NodePk.FromString( NodePkString );
				//if( !string.IsNullOrEmpty( NodeKey ) )
				//{
				//    NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				//    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
				//    {
				//        Node = _CswNbtResources.Nodes[NbtNodeKey];
				//    }
				//}

				Node = _getNode( NodeId, NodeKey );
			}

			if( Node != null )
			{
                CswNbtNodeWriter Writer = new CswNbtNodeWriter( _CswNbtResources );
                Writer.setDefaultPropertyValues( Node );
                foreach( XmlNode PropNode in XmlDoc.DocumentElement.ChildNodes )
				{
                    _applyPropXml( Node, PropNode );
				}

				// BZ 8517 - this sets sequences that have setvalonadd = 0
				_CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( Node );

				Node.postChanges( false );

				// case 21267 
				if( Node.NodeId == _CswNbtResources.CurrentNbtUser.UserNode.NodeId )
				{
					_CswNbtResources.CurrentUser = CswNbtNodeCaster.AsUser( Node );
				}

				if( NbtNodeKey == null && View != null)
				{
					// Get the nodekey of this node in the current view
					ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
					NbtNodeKey = Tree.getNodeKeyByNodeId( Node.NodeId );
					if( NbtNodeKey == null )
					{
						// Make a nodekey from the default view
						View = Node.NodeType.CreateDefaultView();
						View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Node.NodeId );
						Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
						NbtNodeKey = Tree.getNodeKeyByNodeId( Node.NodeId );
					}
				}
				string NodeKeyString = string.Empty;
				if( NbtNodeKey != null )
					NodeKeyString = wsTools.ToSafeJavaScriptParam( NbtNodeKey.ToString() );

				ret = new JObject( new JProperty( "result", "Succeeded" ),
									new JProperty( "nodeid", Node.NodeId.ToString() ),
									new JProperty( "cswnbtnodekey", NodeKeyString ) );
			}
			else
			{
				ret = new JObject( new JProperty( "result", "Failed" ) );
			}
			return ret;
		} // saveProps()


		public bool copyPropValues( string SourceNodeKeyStr, string[] CopyNodeIds, string[] PropIds )
		{
			CswNbtNodeKey SourceNodeKey = new CswNbtNodeKey( _CswNbtResources, SourceNodeKeyStr );
			if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
			{
				CswNbtNode SourceNode = _CswNbtResources.Nodes[SourceNodeKey];
				if( SourceNode != null )
				{
					foreach( string NodeIdStr in CopyNodeIds )
					{
						CswPrimaryKey CopyToNodePk = new CswPrimaryKey();
						CopyToNodePk.FromString( NodeIdStr );
						CswNbtNode CopyToNode = _CswNbtResources.Nodes[CopyToNodePk];
						if( CopyToNode != null )
						{
						    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in PropIds.Select( PropIdAttr => new CswPropIdAttr( PropIdAttr ) )
                                                                                       .Select( PropId => _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId ) ) )
						    {
						        CopyToNode.Properties[NodeTypeProp].copy( SourceNode.Properties[NodeTypeProp] );
						    }

						    CopyToNode.postChanges( false );
						} // if( CopyToNode != null )
					} // foreach( string NodeIdStr in CopyNodeIds )
				} // if(SourceNode != null)
			} // if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
			return true;
		} // copyPropValues()

        private void _applyPropXml( CswNbtNode Node, XmlNode PropNode )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropNode.Attributes["id"].Value );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
            Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );

            // Recurse on sub-props
            XmlNode SubPropsNode = CswXmlDocument.ChildXmlNode( PropNode, "subprops" );
            if( SubPropsNode != null )
            {
                foreach( XmlNode ChildPropNode in SubPropsNode.ChildNodes )
                {
                    _applyPropXml( Node, ChildPropNode );
                }
            }

        } // _applyPropXml

		public bool ClearPropValue( string PropIdAttr, bool IncludeBlob )
		{
			bool ret = false;
			CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
			if( Int32.MinValue != PropId.NodeId.PrimaryKey )
			{
				CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
				CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];
				PropWrapper.ClearValue();
				if( IncludeBlob )
				{
					PropWrapper.ClearBlob();
				}
				Node.postChanges( false );
				ret = true;
			}
			return ret;
		} // ClearPropValue()

		public bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, string PropIdAttr )
		{
			bool ret = false;
			CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
			if( Int32.MinValue != PropId.NodeId.PrimaryKey )
			{
				CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
				CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];

				// Do the update directly
				CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Blobber_save_update", "jct_nodes_props" );
				JctUpdate.AllowBlobColumns = true;
				if( PropWrapper.JctNodePropId > 0 )
				{
					DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropWrapper.JctNodePropId );
					JctTable.Rows[0]["blobdata"] = Data;
					JctTable.Rows[0]["field1"] = FileName;
					JctTable.Rows[0]["field2"] = ContentType;
					JctUpdate.update( JctTable );
				}
				else
				{
					DataTable JctTable = JctUpdate.getEmptyTable();
					DataRow JRow = JctTable.NewRow();
					JRow["nodetypepropid"] = CswConvert.ToDbVal( PropId.NodeTypePropId );
					JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
					JRow["nodeidtablename"] = Node.NodeId.TableName;
					JRow["blobdata"] = Data;
					JRow["field1"] = FileName;
					JRow["field2"] = ContentType;
					JctTable.Rows.Add( JRow );
					JctUpdate.update( JctTable );
				}
				ret = true;
			} // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
			return ret;
		} // SetPropBlobValue()

	} // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
