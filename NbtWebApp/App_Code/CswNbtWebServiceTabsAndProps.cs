using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

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

		private const char PropIdDelim = '_';

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
						Prop.FilterNodeTypePropId == Int32.MinValue )
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
                            if (!Prop.hasFilter())
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
                CswXmlDocument.AppendXmlAttribute(PropXmlNode, "id", Node.NodeId.ToString() + PropIdDelim + Prop.PropId);
            }
            else
            {
                CswXmlDocument.AppendXmlAttribute( PropXmlNode, "id", "new" + PropIdDelim + Prop.PropId );
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
			CswXmlDocument.AppendXmlAttribute( PropXmlNode, "gestalt", PropWrapper.Gestalt.Replace( "\"", "&quot;" ) );

			PropWrapper.ToXml( PropXmlNode );

			return PropXmlNode;
        } // _makePropXml()

        public bool moveProp( string PropIdAttr, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;
            Int32 NodeTypePropId = _getPropIdFromAttribute( PropIdAttr );
            if( NodeTypePropId != Int32.MinValue && NewRow > 0 && NewColumn > 0 )
            {
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Prop.DisplayColumn = NewColumn;
                Prop.DisplayRow = NewRow;
                ret = true;
            }
            return ret;
        } // moveProp()

		public string saveProps( NodeEditMode EditMode, string NodeKey, string NewPropsXml, Int32 NodeTypeId )
		{
			XmlDocument XmlDoc = new XmlDocument();
			XmlDoc.LoadXml( NewPropsXml );

			CswNbtNode Node = null;
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
				    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey(_CswNbtResources, NodeKey);
				    if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
				    {
				        Node = _CswNbtResources.Nodes[NbtNodeKey];
				    }
				}
			}

			foreach( XmlNode PropNode in XmlDoc.DocumentElement.ChildNodes )
			{
				_applyPropXml( Node, PropNode );
			}

			Node.postChanges( false );

			return "{ \"result\": \"Succeeded\", \"nodeid\": \"" + Node.NodeId.ToString() + "\" }";
		} // saveProps()

        private Int32 _getPropIdFromAttribute( string PropIdAttr )
		{
            string[] SplitNodePropId = PropIdAttr.Split( PropIdDelim );
            Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[SplitNodePropId.Length - 1] );
            return NodeTypePropId;
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


	} // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
