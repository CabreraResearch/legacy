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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceTabsAndProps
	{
		public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue };

		private readonly CswNbtResources _CswNbtResources;
		public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		public XElement getTabs( NodeEditMode EditMode, string NodeKey, Int32 NodeTypeId )
		{
			XElement TabsNode = new XElement( "tabs" );
			if( EditMode == NodeEditMode.AddInPopup )
			{
				CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
				TabsNode.Add( new XElement( "tab",
											new XAttribute( "id", "newtab" ),
											new XAttribute( "name", "Add New " + NodeType.NodeTypeName ) ) );
			}
			else
			{
				//CswPrimaryKey NodePk = new CswPrimaryKey();
				CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey(_CswNbtResources,NodeKey);
			    CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];

				foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
				{
					TabsNode.Add( new XElement("tab", 
										new XAttribute( "id", Tab.TabId ),
										new XAttribute( "name", Tab.TabName ) ) );
				}
			}
            return TabsNode;
		} // getTabs()


		/// <summary>
		/// Returns XML for all properties in a given tab
		/// </summary>
		public XmlDocument getProps( NodeEditMode EditMode, string NodeKey, string TabId, Int32 NodeTypeId )
		{
			XmlDocument PropXmlDoc = new XmlDocument();
			XElement PropsElement = new XElement("props");
			CswXmlDocument.SetDocumentElement( PropXmlDoc, "props" );

			if( EditMode == NodeEditMode.AddInPopup )
			{
				CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );

				foreach( CswNbtMetaDataNodeTypeProp Prop in Node.NodeType.NodeTypeProps )
				{
					if( ( ( Prop.IsRequired && Prop.DefaultValue.Empty ) ||
						  Node.Properties[Prop].TemporarilyRequired ||
						  Prop.SetValueOnAdd ) &&
						Prop.FilterNodeTypePropId == Int32.MinValue &&
						! ( Node.Properties[Prop].Hidden ) )
					{
						_addProp( PropXmlDoc, EditMode, Node, Prop );
					}
				}
			}
			else
			{
                //CswPrimaryKey NodePk = new CswPrimaryKey();
                //NodePk.FromString( NodePkString );
                if( !string.IsNullOrEmpty( NodeKey ) )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey(_CswNbtResources, NodeKey);
                    if (Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey)
                    {
                        CswNbtNode Node = _CswNbtResources.Nodes[NbtNodeKey];

                        CswNbtMetaDataNodeTypeTab Tab = Node.NodeType.getNodeTypeTab(CswConvert.ToInt32(TabId));

                        foreach (CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder)
                        {
                            if (!Prop.hasFilter() && !Node.Properties[Prop].Hidden)
                            {
                                _addProp(PropXmlDoc, EditMode, Node, Prop);
                            }
                        }
                    }
                }
			} // if-else( EditMode == NodeEditMode.AddInPopup )
			return PropXmlDoc;
		} // getProps()


		/// <summary>
		/// Returns XML for a single property and its conditional properties
		/// </summary>
        public XmlDocument getSingleProp( NodeEditMode EditMode, string NodeKey, string PropIdFromXml, Int32 NodeTypeId, string NewPropXml )
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
                if( !string.IsNullOrEmpty( NodeKey ) )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
                    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
                    {
                        Node = _CswNbtResources.Nodes[NbtNodeKey];
                    }
                }
			}

			if( NewPropXml != string.Empty )
			{
				// for prop filters, update node prop value but don't save the change
				XmlDocument XmlDoc = new XmlDocument();
				XmlDoc.LoadXml( NewPropXml );
				_applyPropXml( Node, XmlDoc.DocumentElement );
			}

            Int32 NodeTypePropId = _getPropIdFromAttribute( PropIdFromXml );

			CswNbtMetaDataNodeTypeProp Prop = Node.NodeType.getNodeTypeProp( NodeTypePropId );
			_addProp( PropXmlDoc, EditMode, Node, Prop );

			if( NewPropXml != string.Empty )
			{
				//Node.Rollback();
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

            if( Node.NodeId != null )
            {
				CswXmlDocument.AppendXmlAttribute( PropXmlNode, "id", makePropIdAttribute( Node, Prop ) );
            }
            else
            {
				CswXmlDocument.AppendXmlAttribute( PropXmlNode, "id", makePropIdAttribute( null, Prop ) );
            }
		    CswXmlDocument.AppendXmlAttribute( PropXmlNode, "name", Prop.PropNameWithQuestionNo );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "fieldtype", Prop.FieldType.FieldType.ToString() );
			if( Prop.ObjectClassProp != null )
			{
				CswXmlDocument.AppendXmlAttribute( PropXmlNode, "ocpname", Prop.ObjectClassProp.PropName );
			}
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "displayrow", Row.ToString() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "displaycol", Column.ToString() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "required", Prop.IsRequired.ToString().ToLower() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "readonly", Prop.ReadOnly.ToString().ToLower() );
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "gestalt", PropWrapper.Gestalt.Replace( "\"", "&quot;" ) );

			PropWrapper.ToXml( PropXmlNode );

			return PropXmlNode;
        } // _makePropXml()

        public bool moveProp( string PropIdAttr, Int32 NewRow, Int32 NewColumn, NodeEditMode EditMode )
        {
            bool ret = false;
            Int32 NodeTypePropId = _getPropIdFromAttribute( PropIdAttr );
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

		public JObject saveProps( NodeEditMode EditMode, string NodeKey, string NewPropsXml, Int32 NodeTypeId, Int32 ViewId )
		{
			JObject ret = null;
			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.LoadXml( NewPropsXml );

			CswNbtNode Node = null;
			CswNbtNodeKey NbtNodeKey = null;
			if( EditMode == NodeEditMode.AddInPopup )
			{
				Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
			}
			else
			{
				//CswPrimaryKey NodePk = new CswPrimaryKey();
				//NodePk.FromString( NodePkString );
				if(!string.IsNullOrEmpty(NodeKey))
				{
				    NbtNodeKey = new CswNbtNodeKey(_CswNbtResources, NodeKey);
				    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
				    {
				        Node = _CswNbtResources.Nodes[NbtNodeKey];
				    }
				}
			}

			if( Node != null )
			{
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
					_CswNbtResources.CurrentUser = CswNbtNodeCaster.AsUser(Node);
				}

				if( NbtNodeKey == null )
				{
					// Get the nodekey of this node in the current view
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
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

		private const char PropIdDelim = '_';

		public static string makePropIdAttribute( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
		{
			string ret = string.Empty;
			if( Node != null )
				ret = Node.NodeId.ToString();
			else
				ret = "new";
			ret += PropIdDelim + Prop.PropId.ToString();
			return ret;
		} // _makePropId()


		private Int32 _getPropIdFromAttribute( string PropIdAttr )
		{
			CswDelimitedString ds = new CswDelimitedString( PropIdDelim );
			ds.FromString( PropIdAttr );
			Int32 NodeTypePropId = CswConvert.ToInt32( ds[ds.Count - 1] );
			return NodeTypePropId;
		}
		private CswPrimaryKey _getNodePkFromAttribute( string PropIdAttr )
		{
			CswDelimitedString ds = new CswDelimitedString( PropIdDelim );
			ds.FromString( PropIdAttr );
			ds.RemoveAt( ds.Count - 1 );
			string NodePkStr = ds.ToString();
			CswPrimaryKey NodePk = new CswPrimaryKey();
			NodePk.FromString( NodePkStr );
			return NodePk;
		}

        private void _applyPropXml( CswNbtNode Node, XmlNode PropNode )
        {
            Int32 NodeTypePropId = _getPropIdFromAttribute(PropNode.Attributes["id"].Value);

			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
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
			CswPrimaryKey NodePk = _getNodePkFromAttribute( PropIdAttr );
			Int32 NodeTypePropId = _getPropIdFromAttribute( PropIdAttr );
			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
			if( Int32.MinValue != NodePk.PrimaryKey )
			{
				CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
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
			CswPrimaryKey NodePk = _getNodePkFromAttribute( PropIdAttr );
			Int32 NodeTypePropId = _getPropIdFromAttribute( PropIdAttr );
			CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
			if( Int32.MinValue != NodePk.PrimaryKey )
			{
				CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
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
					JRow["nodetypepropid"] = CswConvert.ToDbVal( NodeTypePropId );
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
