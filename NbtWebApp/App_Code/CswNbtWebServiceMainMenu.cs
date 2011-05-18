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
				View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
			}

			string NodeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );
		    string NodeIdNum = string.Empty;
			CswNbtNode Node = null;
		    Int32 NodeTypeId = Int32.MinValue;
		    Int32 NodeId = Int32.MinValue;
            if( !string.IsNullOrEmpty(NodeKey) )
			{
				CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
				Node = _CswNbtResources.Nodes[NbtNodeKey];
			    NodeId = Node.NodeId.PrimaryKey;
			    NodeTypeId = Node.NodeTypeId;
			    NodeIdNum = NodeId.ToString();
			}

            // SEARCH
            XElement SearchNode = new XElement( "item",
                                    new XAttribute( "text", "Search" ) ); 
                
            XElement SearchGenericNode =  new XElement( "item",
                                                new XAttribute( "text", "Generic Search" ),
                                                new XAttribute( "action", "GenericSearch" ) );

			if( View != null )
			{
			    // Generic search
			    if( View.Visibility != NbtViewVisibility.Property )
			    {
			        SearchGenericNode = new XElement( "item",
			                                          new XAttribute( "text", "Generic Search" ),
			                                          new XAttribute( "nodeid", NodeId ),
			                                          new XAttribute( "nodetypeid", NodeTypeId ),
			                                          new XAttribute( "action", "GenericSearch" ) );


			    }
			    // View based search
			    if( View.IsSearchable() )
			    {
			        SearchNode.AddFirst( new XElement( "item",
			                                           new XAttribute( "text", "This View" ),
			                                           new XAttribute( "nodeid", NodeId ),
			                                           new XAttribute( "nodetypeid", NodeTypeId ),
			                                           new XAttribute( "viewid", ViewId ),
			                                           new XAttribute( "action", "ViewSearch" ) ) );
			    }

			    // ADD
			    XElement AddNode = new XElement( "item",
			                                     new XAttribute( "text", "Add" ) );

                // case 21672
                CswNbtViewNode ParentNode = View.Root;
			    bool LimitToFirstGeneration = ( View.ViewMode == NbtViewRenderingMode.Grid );
                if( LimitToFirstGeneration && View.Visibility == NbtViewVisibility.Property )
                {
                    ParentNode = View.Root.ChildRelationships[0];
                }
                foreach( XElement AddNodeType in ParentNode.AllowedChildNodeTypes( LimitToFirstGeneration )
			        .Select( Entry => new XElement( "item",
			                                        new XAttribute( "text", Entry.NodeType.NodeTypeName ),
			                                        new XAttribute( "nodetypeid", Entry.NodeType.NodeTypeId ),
			                                        new XAttribute( "relatednodeid", NodeIdNum ), //for Grid Props
			                                        new XAttribute( "action", "AddNode" ) ) ) )
			    {
			        AddNode.Add( AddNodeType );
			    }

			    MenuNode.Add( AddNode );

			    // COPY
			    if( null != Node && Node.NodeSpecies == NodeSpecies.Plain &&
			        View.ViewMode != NbtViewRenderingMode.Grid &&
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
			        View.ViewMode != NbtViewRenderingMode.Grid &&
			        Node.NodeSpecies == NodeSpecies.Plain &&
			        _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.Delete, Node.NodeTypeId, Node, null ) )
			    {

			        MenuNode.Add( new XElement( "item",
			                                    new XAttribute( "text", "Delete" ),
			                                    new XAttribute( "nodeid", Node.NodeId.ToString() ),
			                                    new XAttribute( "nodename", Node.NodeName ),
			                                    new XAttribute( "action", "DeleteNode" ) ) );
			    }

			    // SAVE VIEW AS
			    if( View.ViewId <= 0 )
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
			            MenuNode.Add( new XElement( "item",
			                                        new XAttribute( "text", "Print Label" ),
													new XAttribute( "nodeid", Node.NodeId.ToString() ),
													new XAttribute( "propid", new CswPropIdAttr(Node, BarcodeProperty).ToString() ),
													new XAttribute( "action", "PrintLabel" ) ) );
			        }
			    }

			    // PRINT
			    if( View.ViewMode == NbtViewRenderingMode.Grid )
			    {
			        MenuNode.Add( new XElement( "item",
			                                    new XAttribute( "text", "Print" ),
			                                    new XAttribute( "popup", "PrintGrid.aspx?sessionviewid=" + View.SessionViewId ) ) );
			    }

			    // EXPORT
			    XElement ExportNode = new XElement( "item",
			                                        new XAttribute( "text", "Export" ) );

			    if( NbtViewRenderingMode.Grid == View.ViewMode )
			    {

			        foreach( XElement ExportType in from ExportOutputFormat FormatType
			                                            in Enum.GetValues( typeof (ExportOutputFormat) )
			                                        where ExportOutputFormat.MobileXML != FormatType || _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.Mobile )
			                                        select new XElement( "item",
			                                                             new XAttribute( "text", FormatType ),
			                                                             new XAttribute( "popup", "Popup_Export.aspx?sessionviewid=" + View.SessionViewId + "&format=" + FormatType.ToString().ToLower() + "&renderingmode=" + View.ViewMode ) ) )
			        {
			            ExportNode.Add( ExportType );
			        }

			    }
			    else // tree or list
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
            } // if( null != View )
		    
            // EDIT VIEW
			if( ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( CswNbtActionName.Edit_View ) )
			{
				//string EditViewHref = "EditView.aspx?viewid=" + ViewId;
				//if( View != null && View.Visibility == NbtViewVisibility.Property )
				//{
				//    EditViewHref += "&step=2";
				//}
				//MenuNode.Add( new XElement( "item",
				//                            new XAttribute( "text", "Edit View" ),
				//                            new XAttribute( "href", EditViewHref ) ) );
				MenuNode.Add( new XElement( "item",
											new XAttribute( "text", "Edit View" ),
											new XAttribute( "action", "editview" ) ) );
			}

            if( null != View )
            {
                // MULTI-EDIT
				if( View.ViewMode == NbtViewRenderingMode.Tree || View.ViewMode == NbtViewRenderingMode.List )
				{
					MenuNode.Add( new XElement( "item",
												new XAttribute( "text", "Multi-Edit" ),
												new XAttribute( "action", "multiedit" ) ) );
				}

            } // if( null != View )

            //Case 21680
            if( null == View || View.Visibility != NbtViewVisibility.Property )
            {
                SearchNode.Add( SearchGenericNode );
            }
            if( SearchNode.HasElements )
            {
                MenuNode.AddFirst( SearchNode );
            }

		    return MenuNode;
		} // getMenu()

	} // class CswNbtWebServiceMainMenu

} // namespace ChemSW.Nbt.WebServices
