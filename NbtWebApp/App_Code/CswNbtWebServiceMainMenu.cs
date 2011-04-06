using System;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceMainMenu
	{

		/// <summary>
		/// Supported Export formats
		/// </summary>
		public enum ExportOutputFormat
		{
			CSV,
			Excel,
			PDF,
			Word,
			MobileXML,
			ReportXML
		}

		private CswNbtResources _CswNbtResources;
		public CswNbtWebServiceMainMenu( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		public XElement getMenu( Int32 ViewId, string SafeNodeKey )
		{
			XElement MenuNode = new XElement( "menu" );

			CswNbtView View = null;
			if( ViewId != Int32.MinValue )
			{
				View = _CswNbtResources.ViewCache.getView( ViewId );
			}

			string NodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
			CswNbtNode Node = null;
		    Int32 NodeTypeId = Int32.MinValue;
		    Int32 NodeId = Int32.MinValue;
            if( !string.IsNullOrEmpty(NodeKey) )
			{
				CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				Node = _CswNbtResources.Nodes[NbtNodeKey];
			    NodeId = Node.NodeId.PrimaryKey;
			    NodeTypeId = Node.NodeTypeId;
			}

			// SEARCH

			MenuNode.Add( new XElement( "item",
										new XAttribute( "text", "Search" ),
                                        new XAttribute( "nodeid", NodeId ),
                                        new XAttribute( "nodetypeid", NodeTypeId ),
                                        new XAttribute( "viewid", ViewId ),
										new XAttribute( "action", "Search" ) ) );

			// ADD
			XElement AddNode = new XElement( "item",
											new XAttribute( "text", "Add" ) );
			if( View != null )
			{
				foreach( XElement AddNodeType in View.Root.AllowedChildNodeTypes()
												 .Select( Entry => new XElement( "item",
																				new XAttribute( "text", Entry.NodeType.NodeTypeName ),
																				new XAttribute( "nodetypeid", Entry.NodeType.NodeTypeId ),
																				new XAttribute( "action", "AddNode" ) ) ) )
				{
					AddNode.Add( AddNodeType );
				}
				MenuNode.Add( AddNode );
			}

			// COPY
			if( null != Node && 
                Node.NodeSpecies == NodeSpecies.Plain &&
				_CswNbtResources.CurrentNbtUser.CheckCreatePermission( Node.NodeTypeId ) )
			{
				string BadPropertyName = string.Empty;
				if( !Node.NodeType.IsUniqueAndRequired( ref BadPropertyName ) )
				{
					MenuNode.Add( new XElement( "item",
												new XAttribute( "text", "Copy" ),
												new XAttribute( "nodeid", Node.NodeId.ToString() ),
												new XAttribute( "nodename", Node.NodeName ),
												new XAttribute( "action", "CopyNode" ) ) );
				}
			}

			// DELETE
			if( !string.IsNullOrEmpty( NodeKey ) &&
				null != Node &&
				Node.NodeSpecies == NodeSpecies.Plain &&
				_CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Delete, Node.NodeTypeId, Node, null ) )
			{

				string PopUp = "Popup_DeleteNode.aspx?nodekey=" + Node.NodeId.PrimaryKey + "&checkednodeids=";
				MenuNode.Add( new XElement( "item",
											new XAttribute( "text", "Delete" ),
											new XAttribute( "nodeid", Node.NodeId.ToString() ),
											new XAttribute( "nodename", Node.NodeName ),
											new XAttribute( "action", "DeleteNode" ) ) );
			}

			// SAVE VIEW AS
			if( View != null && View.ViewId <= 0 )
			{
				MenuNode.Add( new XElement( "item",
											new XAttribute( "text", "SaveViewAs" ),
											new XAttribute( "popup", "Popup_NewView.aspx?sessionviewid=" + View.SessionViewId ) ) );
			}

			// PRINT LABEL
			if( !string.IsNullOrEmpty( NodeKey ) && null != Node && Node.NodeType != null )
			{
				CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.NodeType.BarcodeProperty;
				if( BarcodeProperty != null )
				{
					string PopUp = "Popup_PrintLabel.aspx?nodeid=" + Node.NodeId.PrimaryKey + "&propid=" +
								   BarcodeProperty.PropId + "&checkednodeids=";
					MenuNode.Add( new XElement( "item",
												new XAttribute( "text", "Print Label" ),
												new XAttribute( "popup", PopUp ) ) );
				}
			}

			// PRINT
			if( View != null && View.ViewMode == NbtViewRenderingMode.Grid )
			{
				MenuNode.Add( new XElement( "item",
											 new XAttribute( "text", "Print" ),
											 new XAttribute( "popup", "PrintGrid.aspx?sessionviewid=" + View.SessionViewId ) ) );
			}
			// EXPORT
			XElement ExportNode = new XElement( "item",
												new XAttribute( "text", "Export" ) );
			if( View != null )
			{
				if( NbtViewRenderingMode.Grid == View.ViewMode )
				{

					foreach( XElement ExportType in from ExportOutputFormat FormatType
														in Enum.GetValues( typeof( ExportOutputFormat ) )
													where ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile )
													select new XElement( "item",
														new XAttribute( "text", FormatType ),
														new XAttribute( "popup", "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode ) ) )
					{
						ExportNode.Add( ExportType );
					}

				}
				else  // tree or list
				{
					ExportNode.Add( new XElement( "item",
													new XAttribute( "text", "Report XML" ) ) );
					if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
					{
						string PopUp = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" +
										ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
						ExportNode.Add( new XElement( "item",
														new XAttribute( "text", "Mobile XML" ),
														new XAttribute( "popup", PopUp ) ) );
					}
				}
				MenuNode.Add( ExportNode );
			}

			// MOBILE
			if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
			{
				string PopUp = "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" +
								ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode;
				XElement MobileNode = new XElement( "item",
													new XAttribute( "text", "Mobile" ) );
				MobileNode.Add( new XElement( "item",
												new XAttribute( "text", "Export Mobile XML" ),
												new XAttribute( "popup", PopUp ) ) );
				MobileNode.Add( new XElement( "item",
												new XAttribute( "text", "Import Mobile XML" ),
												new XAttribute( "href", _CswNbtResources.Actions[CswNbtActionName.Load_Mobile_Data].Url ) ) );
				MenuNode.Add( MobileNode );
			}

			//// SWITCH VIEW
			//ret += "<item text=\"Switch View\" popup=\"Popup_ChangeView.aspx\"/>";

			// EDIT VIEW
			if( ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Edit_View ) )
			{
				string EditViewHref = "EditView.aspx?viewid=" + ViewId;
				if( View != null && View.Visibility == NbtViewVisibility.Property )
				{
					EditViewHref += "&step=2";
				}
				MenuNode.Add( new XElement( "item",
											new XAttribute( "text", "Edit View" ),
											new XAttribute( "href", EditViewHref ) ) );
			}

			// MULTI-EDIT
			MenuNode.Add( new XElement( "item",
										new XAttribute( "text", "Multi-Edit" ),
										new XAttribute( "action", "multiedit" ) ) );
			return MenuNode;
		}

	} // class CswNbtWebServiceMainMenu

} // namespace ChemSW.Nbt.WebServices
