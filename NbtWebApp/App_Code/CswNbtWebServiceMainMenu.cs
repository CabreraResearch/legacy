using System;
using System.Xml.Linq;
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
			XElement MenuNode = new XElement("menu");
            
            CswNbtView View = null;
			if( ViewId != Int32.MinValue )
			{
				View = _CswNbtResources.ViewCache.getView( ViewId );
			}

		    string NodeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
			CswNbtNode Node = null;
            if( NodeKey != string.Empty )
			{
                CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				Node = _CswNbtResources.Nodes[NbtNodeKey];				
			}

			// SEARCH
			string SearchHref = "Search.aspx?viewid=" + ViewId;
            if( null != Node )
            {
                SearchHref += "&nodeid=" + Node.NodeId.PrimaryKey;
            }
            MenuNode.Add( new XElement("item", 
                            new XAttribute("text","Search"),
                            new XAttribute("href",SearchHref)));

			// ADD
			if( View != null && View.ViewId <= 0 )
			{
                XElement AddNode = new XElement( "item",
                                    new XAttribute( "text", "Add" ) );
                foreach( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry in View.Root.AllowedChildNodeTypes() )
				{
					AddNode.Add(new XElement("item"),
                                    new XAttribute("text",Entry.NodeType.NodeTypeName),
                                    new XAttribute( "nodetypeid", Entry.NodeType.NodeTypeId ),
                                    new XAttribute("action","AddNode"));
				}
                MenuNode.Add(AddNode);
			}

			// COPY
            MenuNode.Add( new XElement( "item",
                            new XAttribute( "text", "Copy" ),
                            new XAttribute( "href", "Popup_CopyNode.aspx?nodekey=" + Node.NodeId.PrimaryKey ) ) );

			// DELETE
			if( !string.IsNullOrEmpty( NodeKey ) && null != Node && Node.NodeSpecies == NodeSpecies.Plain )
			{
				//if( SelectedNodeKeyViewNode is CswNbtViewRelationship &&
				//    ( (CswNbtViewRelationship) SelectedNodeKeyViewNode ).AllowDelete &&
				if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Delete, Node.NodeTypeId, Node, null ) )
				{
                    MenuNode.Add( new XElement( "item" ,
                                    new XAttribute( "text", "Delete" ),
                                    new XAttribute( "popup", "Popup_DeleteNode.aspx?nodekey=" + Node.NodeId.PrimaryKey + "&checkednodeids=" )));
				}
			}

			// SAVE VIEW AS
			if( View != null && View.ViewId <= 0 )
			{
                MenuNode.Add( new XElement( "item",
                                    new XAttribute( "text", "SaveViewAs" ),
                                    new XAttribute( "popup", "Popup_NewView.aspx?sessionviewid=" + View.SessionViewId )));
			}

			// PRINT LABEL
            if( !string.IsNullOrEmpty( NodeKey ) && null != Node && Node.NodeType != null )
			{
				CswNbtMetaDataNodeTypeProp BarcodeProperty = Node.NodeType.BarcodeProperty;
				if( BarcodeProperty != null )
				{
                    MenuNode.Add( new XElement( "item",
                                    new XAttribute( "text", "Print Label" ),
                                    new XAttribute( "popup", "Popup_PrintLabel.aspx?nodeid=" + Node.NodeId.PrimaryKey + "&propid=" + BarcodeProperty.PropId + "&checkednodeids=" )));
				}
			}

			// PRINT
            if( View != null && View.ViewMode == NbtViewRenderingMode.Grid )
            {
                MenuNode.Add(new XElement("item",
                             new XAttribute("text", "Print"),
                             new XAttribute("popup", "PrintGrid.aspx?sessionviewid=" + View.SessionViewId)));
            }
		    // EXPORT
			if( NbtViewRenderingMode.Grid == View.ViewMode )
			{
                XElement ExportNode = new XElement( "item",
                                    new XAttribute( "text", "Export" ) );
                foreach( ExportOutputFormat FormatType in Enum.GetValues( typeof( ExportOutputFormat ) ) )
				{
					if( ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
					{
						ExportNode.Add(new XElement( "item",
                                    new XAttribute( "text", FormatType ),
                                    new XAttribute( "popup", "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode )));
					}
				}
			    MenuNode.Add(ExportNode);
            }
			else  // tree or list
			{
                XElement ExportNode = new XElement( "item",
                                    new XAttribute( "text", "Report XML" ) );
                if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
                {
                    ExportNode.Add(new XElement( "item",
                                    new XAttribute( "text", "Mobile XML" ),
                                    new XAttribute( "popup", "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode )));
                }
			}

			// MOBILE
			if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile ) )
			{
                XElement MobileNode = new XElement( "item",
                                    new XAttribute( "text", "Mobile" ) );
                MobileNode.Add( new XElement( "item",
                                    new XAttribute( "text", "Export Mobile XML" ),
                                    new XAttribute( "popup", "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" + ExportOutputFormat.MobileXML.ToString().ToLower() + "&renderingmode=" + View.ViewMode )));
                MobileNode.Add( new XElement( "item",
                                    new XAttribute( "text", "Import Mobile XML" ),
                                    new XAttribute( "href", _CswNbtResources.Actions[CswNbtActionName.Load_Mobile_Data].Url )));
                MenuNode.Add(MobileNode);
			}

			//// SWITCH VIEW
			//ret += "<item text=\"Switch View\" popup=\"Popup_ChangeView.aspx\"/>";

			// EDIT VIEW
			if( ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Edit_View ) )
			{
                string EditViewHref = "EditView.aspx?viewid=" + ViewId;
                if( View.Visibility == NbtViewVisibility.Property )
                {
                    EditViewHref += "&step=2";
                }
                MenuNode.Add( new XElement( "item",
                                    new XAttribute( "text", "Edit View" ),
                                    new XAttribute( "href", EditViewHref )));
			}

			// MULTI-EDIT
            MenuNode.Add( new XElement( "item",
                                    new XAttribute( "text", "Multi-Edit" ),
                                    new XAttribute( "action", "multiedit" )));
			return MenuNode;
		}


	} // class CswNbtWebServiceMainMenu

} // namespace ChemSW.Nbt.WebServices
