using System;
using System.Collections.ObjectModel;
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

		public string getTabs( NodeEditMode EditMode, string NodePkString, Int32 NodeTypeId )
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
				CswPrimaryKey NodePk = new CswPrimaryKey();
				NodePk.FromString( NodePkString );
				CswNbtNode Node = _CswNbtResources.Nodes[NodePk];

				foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
				{
					TabsNode.Add( new XElement("tab", 
										new XAttribute( "id", Tab.TabId ),
										new XAttribute( "name", Tab.TabName ) ) );
				}
			}
			string ret = TabsNode.ToString();
			return ret;
		} // getTabs()

		private const char PropIdDelim = '_';

		public string getProps( NodeEditMode EditMode, string NodePkString, string TabId, Int32 NodeTypeId )
		{
			//string ret = string.Empty;
			XElement PropsNode = new XElement( "props" );
	
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
						PropsNode.Add( _makePropXml( Node, Prop, Prop.DisplayRowAdd, Prop.DisplayColAdd ) );
					}
				}
			}
			else
			{
				CswPrimaryKey NodePk = new CswPrimaryKey();
				NodePk.FromString( NodePkString );
				CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
				CswNbtMetaDataNodeTypeTab Tab = Node.NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );

				foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
				{
					if( !Prop.hasFilter() )
					{
						PropsNode.Add( _makePropXml( Node, Prop, Prop.DisplayRow, Prop.DisplayColumn ) );
					}
				}
			}
			string ret = PropsNode.ToString();
			return ret;
		} // getProps()


		private XElement _makePropXml( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 Row, Int32 Column )
		{
			var ThisProp = new XElement( "prop" );
			CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

			string PropId = string.Empty;
			if( Node.NodeId != null )
			{
				PropId = Node.NodeId.ToString() + PropIdDelim + Prop.PropId;
			}
			else
			{
				PropId = "new" + PropIdDelim + Prop.PropId;
			}
			ThisProp.SetAttributeValue( "id", PropId );
			ThisProp.SetAttributeValue( "name", Prop.PropNameWithQuestionNo );
			ThisProp.SetAttributeValue( "fieldtype", Prop.FieldType.FieldType );			
			
			if( Prop.ObjectClassProp != null )
			{
				ThisProp.SetAttributeValue( "ocpname", Prop.ObjectClassProp.PropName );
			}
			ThisProp.SetAttributeValue( "displayrow", Row );
			ThisProp.SetAttributeValue( "displaycol", Column );
			ThisProp.SetAttributeValue( "required", Prop.IsRequired );
			ThisProp.SetAttributeValue( "gestalt", PropWrapper.Gestalt.Replace( "\"", "&quot;" ) );

			XmlDocument XmlDoc = new XmlDocument();
			CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
			PropWrapper.ToXml( XmlDoc.DocumentElement );
			string ret = XmlDoc.DocumentElement.InnerXml;
			// ThisProp += ret ?
			
			return ThisProp;
		}

		public string saveProps( NodeEditMode EditMode, string NodePkString, string NewPropsXml, Int32 NodeTypeId )
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
				CswPrimaryKey NodePk = new CswPrimaryKey();
				NodePk.FromString( NodePkString );
				Node = _CswNbtResources.Nodes[NodePk];
			}

			foreach( XmlNode PropNode in XmlDoc.DocumentElement.ChildNodes )
			{
				string NodePropId = PropNode.Attributes["id"].Value;
				string[] SplitNodePropId = NodePropId.Split( PropIdDelim );
				Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[SplitNodePropId.Length - 1] );

				CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
				Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );
			}
			Node.postChanges( false );

			return "{ \"result\": \"Succeeded\", \"nodeid\": \"" + Node.NodeId.ToString() + "\" }";
		} // saveProp()


	} // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
