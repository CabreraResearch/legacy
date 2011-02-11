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
using System.Web.UI.WebControls;

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
			var GridJSONData = new CswCommaDelimitedString();
			string GridJSONColumns = "{}";
			
			XmlDocument GridXml = getGridXml( View, Session );

			var UnsortedXmlDataSet = new DataSet();
			UnsortedXmlDataSet.ReadXml( new System.IO.StringReader( GridXml.ToString() ), XmlReadMode.InferTypedSchema );

			if( UnsortedXmlDataSet.Tables.Count > 0 && UnsortedXmlDataSet.Tables[0].Rows.Count > 0 )
			{
				GridJSONColumns = _getGridColumnsJSON( UnsortedXmlDataSet, View, ref GridJSONData ).ToString();
			}
			

			GridJSON = @"{
							""columns"": [
											" + GridJSONColumns +
										 @"],
							""grid"": {                
								" + GridJSONData.ToString() +
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



		private const string PropColumnPrefix = "Prop_";
		/// <summary>
		/// Generates JSON content for all properties selected in the View
		/// </summary>
		/// <param name="ParentJSON"></param>
		/// <param name="Props"></param>
		/// <param name="NbtNode"></param>
		/// <returns>Returns an array of property names for use as column names</returns>
		private CswCommaDelimitedString _getGridColumnsJSON( DataSet Grid, CswNbtView View, ref CswCommaDelimitedString GridData )
		{
			string ColumnDefinition = string.Empty;
			var ColumnNames = new CswCommaDelimitedString();
			string RowDefinition = string.Empty;
			
			foreach( DataTable Table in Grid.Tables )
			{
				foreach( DataColumn Column in Table.Columns )
				{
					string ColumnName = Column.ColumnName;
					if( Column.ColumnName.Length > PropColumnPrefix.Length && Column.ColumnName.Substring( 0, PropColumnPrefix.Length ) == PropColumnPrefix )
					{
						string NoPrefixColumnName = ColumnName.Substring( PropColumnPrefix.Length );
						string RealColumnName = CswTools.XmlRealAttributeName( NoPrefixColumnName );
						
						CswNbtViewProperty CurrentViewProp = View.FindPropertyByName( RealColumnName );
						
						var ColFieldType = CswNbtMetaDataFieldType.NbtFieldType.Unknown;
						CswNbtMetaDataNodeTypeProp CurrentNTP = null;
						if( CurrentViewProp != null )
						{
							if( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
							{
								CswNbtMetaDataNodeType CurrentNT = _CswNbtResources.MetaData.getNodeType( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondId );
								CurrentNTP = CurrentNT.getNodeTypeProp( RealColumnName );
								if( CurrentNTP != null )
									ColFieldType = CurrentNTP.FieldType.FieldType;
							}
							else if( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
							{
								CswNbtMetaDataObjectClass CurrentOC = _CswNbtResources.MetaData.getObjectClass( ( (CswNbtViewRelationship) CurrentViewProp.Parent ).SecondId );
								foreach( CswNbtMetaDataNodeType CurrentNT in CurrentOC.NodeTypes )
								{
									CurrentNTP = CurrentNT.getNodeTypeProp( RealColumnName );
									if( CurrentNTP != null )
										ColFieldType = CurrentNTP.FieldType.FieldType;
								}
							}
						}

						if( null != CurrentNTP )
						{
							ColumnDefinition = @"{""id"":""" + ColumnName.Replace( " ", "" ).ToLower() + @"";
							ColumnDefinition += @", ""name"":" + ColumnName + @"";
							ColumnDefinition += @", ""field"":" + ColumnName.Replace( " ", "" ).ToLower() + @"";

							if( !CurrentNTP.ReadOnly )
							{
								ColumnDefinition += ", editor:TextCellEditor";
							}
							if( CurrentNTP.IsRequired )
							{
								ColumnDefinition += ", validator:requiredFieldValidator ";
							}
							if( CurrentViewProp.Width != Int32.MinValue )
							{
								string Width = Unit.Parse( ( CswConvert.ToInt32( CurrentViewProp.Width*7 ) ).ToString() + "px" ).ToString();
								ColumnDefinition += ", width:" + Width;
							}

							switch( ColFieldType )
							{
								case CswNbtMetaDataFieldType.NbtFieldType.Date:
									break;
								case CswNbtMetaDataFieldType.NbtFieldType.Time:
									break;
								default:
									break;
							}

							ColumnDefinition += "}";
							ColumnNames.Add( ColumnDefinition );
						}

					}
				}
				foreach( DataRow Row in Table.Rows )
				{
					foreach( DataColumn Column in Table.Columns )
					{
						RowDefinition += @"{""" + Column.ColumnName + @""": " + @"""" + Row[Column.ColumnName].ToString() + @"""}";
						GridData.Add( RowDefinition );
					}
				}
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
