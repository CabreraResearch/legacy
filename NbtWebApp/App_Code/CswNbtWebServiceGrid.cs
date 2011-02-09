using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using System.Xml;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceGrid
	{
		private CswNbtResources _CswNbtResources;
		private const string QuickLaunchViews = "QuickLaunchViews";
	   
		public CswNbtWebServiceGrid( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		public string getGridJSON( CswNbtView View, HttpSessionState Session )
		{
			string GridJSON = string.Empty;
			string GridJSONData = string.Empty;
			string GridJSONColumns = string.Empty;
		    
			XmlDocument GridXml = getGridXml( View, Session );

			var UnsortedXmlDataSet = new DataSet();
			UnsortedXmlDataSet.ReadXml( new System.IO.StringReader( GridXml.ToString() ), XmlReadMode.InferTypedSchema );

			if( UnsortedXmlDataSet.Tables.Count > 0 && UnsortedXmlDataSet.Tables[0].Rows.Count > 0 )
			{
				
			}

			GridJSON = @"{
							""columns"": [
											" + GridJSONColumns +
										 @"],
							""grid"": {                
								" + GridJSONData +
							@"}
						}";

			return GridJSON;
		} // getGridJSON

		public XmlDocument getGridXml( CswNbtView View, HttpSessionState Session )
		{
			//Append to QuickLaunch
			Stack<KeyValuePair<Int32, string>> ViewHistory = null;
			if( null == Session[QuickLaunchViews] )
			{
				ViewHistory = new Stack<KeyValuePair<Int32, string>>();
			}
			else
			{
				ViewHistory = (Stack<KeyValuePair<Int32, string>>) Session[QuickLaunchViews];
			}
			var ThisView = new KeyValuePair<int, string>( View.ViewId, View.ViewName );

			if( !ViewHistory.Contains( ThisView ) )
			{
				ViewHistory.Push( ThisView );
			}
			Session[QuickLaunchViews] = ViewHistory;

			ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );

			//if( ParentKey != null )
			//    CswNbtTree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.TelerikRadGridProperty;
			//else
			Tree.XmlTreeDestinationFormat = XmlTreeDestinationFormat.TelerikRadGrid;

			string GridXmlString = Tree.getTreeAsXml();
			var GridXml = new XmlDocument();
			GridXml.LoadXml( GridXmlString );

			return GridXml;
		} // getGridXml()




		/// <summary>
		/// Generates JSON content for all properties selected in the View
		/// </summary>
		/// <param name="ParentJSON"></param>
		/// <param name="Props"></param>
		/// <param name="NbtNode"></param>
		/// <returns>Returns an array of property names for use as column names</returns>
		private CswDelimitedString _getNodePropertiesJSON( ref string ParentJSON, CswDelimitedString ColumnNames, Stack<CswNbtMetaDataNodeTypeProp> Props, CswNbtNode NbtNode )
		{
			string ColumnDefinition = string.Empty;
			foreach( var NodeTypeProp in Props )
			{
				if( NodeTypeProp.NodeType != NbtNode.NodeType ) continue;
				ColumnDefinition = @"{""id"":""" + NodeTypeProp.PropName.Replace( " ", "" ).ToLower() + @"";
				ColumnDefinition += @", ""name"":" + NodeTypeProp.PropName + @"";
				ColumnDefinition += @", ""field"":" + NodeTypeProp.PropName.Replace( " ", "" ).ToLower() + @"";
				if( !NbtNode.Properties[NodeTypeProp].ReadOnly )
				{
					ColumnDefinition += ", editor:TextCellEditor";
				}
				if( NbtNode.Properties[NodeTypeProp].Required )
				{
					ColumnDefinition += ", validator:requiredFieldValidator ";
				}
				if( 0 < NodeTypeProp.Length )
				{
					ColumnDefinition += ", width:" + NodeTypeProp.Length.ToString();
				}
				ColumnDefinition += "}";
				ColumnNames.Add( ColumnDefinition );

				ParentJSON += @"" + NodeTypeProp.PropName + @""":" + @"" + NbtNode.Properties[NodeTypeProp].Gestalt + @"";
			}
			return ColumnNames;
		} // _getNodePropertiesJSON


		//public string getViews()
		//{
		//    string ret = string.Empty;
		//    DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( NbtViewRenderingMode.Grid );
		//    foreach( DataRow ViewRow in ViewDT.Rows )
		//    {
		//        ret += "<view id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
		//        ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
		//        ret += "/>";
		//    }
		//    return "<views>" + ret + "</views>";
		//}

		//public XmlDocument getGridXml( CswNbtView View, HttpSessionState Session )
		//{
		//    var ThisNodeAttr = new Dictionary<string, string>();

		//    _GridXML = new XmlDocument();
		//    _XmlDec = _GridXML.CreateXmlDeclaration( "1.0", null, null );
		//    XmlNode DefinitionNode = _GridXML.AppendChild( _XmlDec );
		//    _RootNode = CswXmlDocument.SetDocumentElement( _GridXML, "gridview" );
		//    XmlNode ResultNode = CswXmlDocument.AppendXmlNode( _RootNode, XmlElements.result.ToString() );
		//    XmlNode TreeNode = CswXmlDocument.AppendXmlNode( ResultNode, XmlElements.tree.ToString() );
		//    XmlNode RootNode = CswXmlDocument.AppendXmlNode( TreeNode, XmlElements.root.ToString() );

		//    _GridXML = getGridTree( View, Session );

		//    //if( Tree.getChildNodeCount() > 0 )
		//    //{
		//    //    ThisNodeAttr.Add( XmlAttributes.rel.ToString(), XmlElements.root.ToString() );
		//    //    ThisNodeAttr.Add( XmlAttributes.id.ToString(), XmlElements.root.ToString() );
		//    //    _makeItemNodeXML( RootNode, ThisNodeAttr, View.ViewName );
		//    //    _runTreeNodesRecursiveXML( RootNode, Tree, View );
		//    //    _getTypesXML( ResultNode, View );
		//    //}

		//    return _GridXML;
		//} // getGridXml
		
		
		//private void _getTypesXML( XmlNode ParentNode, CswNbtView View )
		//{
		//    var NodeTypes = new Collection<CswNbtMetaDataNodeType>();
		//    ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
			
		//    foreach( CswNbtViewRelationship Rel in Relationships )
		//    {
		//        if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
		//        {
		//            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
		//            NodeTypes.Add( NodeType );
		//        }
		//        else
		//        {
		//            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
		//            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
		//            {
		//                NodeTypes.Add( NodeType );
		//            }
		//        }
		//    } // foreach( CswNbtViewRelationship Rel in Relationships )

		//    XmlNode TypesNode = CswXmlDocument.AppendXmlNode( ParentNode, XmlElements.types.ToString() );
		//    XmlNode RootNode = CswXmlDocument.AppendXmlNode( TypesNode, XmlElements.root.ToString() );
		//    CswXmlDocument.AppendXmlAttribute( RootNode, XmlAttributes.icon.ToString(), string.Empty );
		//    CswXmlDocument.AppendXmlAttribute( RootNode, XmlAttributes.image.ToString(), "Images/view/viewgrid.gif" );
		//    XmlNode DefaultNode = CswXmlDocument.AppendXmlNode( TypesNode, "default" );

		//    foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
		//    {
		//        XmlNode NodeTypeNode = CswXmlDocument.AppendXmlNode( DefaultNode, XmlElements.nodetype.ToString() );
		//        CswXmlDocument.AppendXmlAttribute( NodeTypeNode, XmlAttributes.icon.ToString(), string.Empty );
		//        CswXmlDocument.AppendXmlAttribute( NodeTypeNode, XmlAttributes.image.ToString(), "Images/icons/" + NodeType.IconFileName );
		//        CswXmlDocument.AppendXmlAttribute( NodeTypeNode, XmlElements.nodetype.ToString(), "nt_" + NodeType.FirstVersionNodeTypeId.ToString() );
		//    }
		//} // _getTypesXML

		//private void _runTreeNodesRecursiveXML( XmlNode ParentNode, ICswNbtTree Tree, CswNbtView View )
		//{
		//    for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
		//    {
		//        Tree.goToNthChild( c );

		//        CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
		//        var ThisNodeAttr = new Dictionary<string, string>();
		//        string ThisNodeName = Tree.getNodeNameForCurrentPosition();

		//        ThisNodeAttr.Add( XmlAttributes.id.ToString(), ThisNode.NodeId.ToString() );
		//        ThisNodeAttr.Add( XmlAttributes.rel.ToString(), "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId.ToString() );
		//        var TheseXmlNodes = _makeItemNodeXML( ParentNode, ThisNodeAttr, ThisNodeName );
		//        var ItemNode = TheseXmlNodes[0]; 
		//        var NameNode = TheseXmlNodes[1];
		//        Stack<CswNbtMetaDataNodeTypeProp> Props = View.getOrderedViewProps();
		//        _getNodePropertiesXML( NameNode, Props, ThisNode );
				
		//        _runTreeNodesRecursiveXML( ItemNode, Tree, View );

		//        Tree.goToParentNode();
		//    }
		//} // _runTreeNodesRecursiveXML

		//private void _getNodePropertiesXML( XmlNode ParentNode, Stack<CswNbtMetaDataNodeTypeProp> Props, CswNbtNode NbtNode )
		//{
		//    var PropertiesNode = CswXmlDocument.AppendXmlNode( ParentNode, XmlElements.properties.ToString() );
		//    foreach( var NodeTypeProp in Props )
		//    {
		//        if( NodeTypeProp.NodeType != NbtNode.NodeType ) continue;
		//        var PropNode = CswXmlDocument.AppendXmlNode( PropertiesNode, XmlElements.prop.ToString() );
		//        PropNode.InnerText = NodeTypeProp.PropName;
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.propid.ToString(), NodeTypeProp.PropId.ToString() );
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.gestalt.ToString(), NbtNode.Properties[NodeTypeProp].Gestalt );
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.fieldtype.ToString(), NbtNode.Properties[NodeTypeProp].FieldType.FieldType.ToString() );
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.ocpropname.ToString(), NbtNode.Properties[NodeTypeProp].ObjectClassPropName );
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.ntpropname.ToString(), NbtNode.Properties[NodeTypeProp].PropName );
		//        CswXmlDocument.AppendXmlAttribute( PropNode, XmlAttributes.read_only.ToString(), NbtNode.Properties[NodeTypeProp].ReadOnly.ToString() );
		//    }
		//} // _getNodePropertiesXML

		///// <summary>
		///// Appends XML Nodes to the ParentNode
		///// </summary>
		///// <param name="ParentNode">"root" node to append sub-elements</param>
		///// <param name="NodesText">node element name, node inner text</param>
		///// <param name="Attributes">attribute name, attribute value applied to first node</param>
		///// <returns>Collection with 2 members; 1=="item",2=="name"</returns>
		//private Collection<XmlNode> _makeItemNodeXML( XmlNode ParentNode, Dictionary<string, string> Attributes, string Text )
		//{
		//    var XmlNodes = new Collection<XmlNode>();
		//    XmlNode ItemNode = CswXmlDocument.AppendXmlNode( ParentNode, XmlElements.item.ToString() );
		//    XmlNode ContentNode = CswXmlDocument.AppendXmlNode( ItemNode, XmlElements.content.ToString() );
		//    XmlNode NameNode = CswXmlDocument.AppendXmlNode( ContentNode, XmlElements.name.ToString() );
		//    NameNode.InnerText = Text;

		//    XmlNodes.Insert( 0, ItemNode );
		//    XmlNodes.Insert( 1, NameNode );

		//    foreach( var attr in Attributes )
		//    {
		//        CswXmlDocument.AppendXmlAttribute( ItemNode, attr.Key, attr.Value );
		//    }
		//    return XmlNodes;
		//} // _makeItemNodeXML

		//private void _runTreeNodesRecursiveJSON( string ParentJSON, ICswNbtTree Tree, CswNbtView View )
		//{
		//    for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
		//    {
		//        Tree.goToNthChild( c );

		//        CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
		//        var ThisNodeAttr = new Dictionary<string, string>();
		//        string ThisNodeName = Tree.getNodeNameForCurrentPosition();

		//        Stack<CswNbtMetaDataNodeTypeProp> Props = View.getOrderedViewProps();
		//        if( Props.Count > 0 )
		//        {
		//           // _getNodePropertiesJSON( ParentJSON, Props, ThisNode );
		//        }

		//        _runTreeNodesRecursiveJSON( ParentJSON, Tree, View );

		//        Tree.goToParentNode();
		//    }
		//} // _runTreeNodesRecursiveJSON
	} // class CswNbtWebServiceGrid

} // namespace ChemSW.Nbt.WebServices
