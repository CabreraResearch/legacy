using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobileView
    {
        private CswNbtResources _CswNbtResources;
        private bool _ForMobile;

        public CswNbtWebServiceMobileView( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
        }

        private static string ViewIdPrefix = "viewid_";
        private static string PropIdPrefix = "prop_";
        private static string TabIdPrefix = "tab_";
        private static string NodeIdPrefix = "nodeid_";
        private static string NodeKeyPrefix = "nodekey_";

        public XElement Run( string ParentId, ICswNbtUser CurrentUser )
        {
            XElement ret = new XElement( "node" );

            if( ParentId.StartsWith( ViewIdPrefix ) )
            {
                // Get the full XML for the entire view
                Int32 ViewId = CswConvert.ToInt32( ParentId.Substring( ViewIdPrefix.Length ) );
                CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                //View.SaveToCache();
                //Session["SessionViewId"] = View.SessionViewId;

                // case 20083
                if( _ForMobile )
                {
                    ret = _getSearchNodes( View );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );

                if( Tree.getChildNodeCount() > 0 )
                {
                    _runTreeNodesRecursive( Tree, ref ret );
                }
                else
                {
                    ret = new XElement( "node",
								new XAttribute( "id", NodeIdPrefix + Int32.MinValue ),
                                new XAttribute( "name", "No Results" ) 
                            );
                }

            }// if( ParentId.StartsWith( ViewIdPrefix ) )
            else
            {
                // All Views
				ret = new XElement("views");
				Collection<CswNbtView> MobileViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, CurrentUser, false, true, false, NbtViewRenderingMode.Any );
				foreach( CswNbtView MobileView in MobileViews )
				{
					ret.Add( new XElement( "view",
							new XAttribute( "id", ViewIdPrefix + MobileView.ViewId ),
							new XAttribute( "name", MobileView.ViewName ) ) );
				}
            }

            return ret;
        } // Run()

        // case 20083 - search options
        private XElement _getSearchNodes( CswNbtView View )
        {
            XElement Searches = new XElement( "searches" );
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( View.ContainsNodeType( NodeType ) )
                {
                    foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.NodeTypeProps )
                    {
                        if( MetaDataProp.MobileSearch )
                        {
                            string PropId = ( MetaDataProp.ObjectClassProp != null ) ? "search_ocp_" + MetaDataProp.ObjectClassPropId : "search_ntp_" + MetaDataProp.PropId;
                            XElement Search = new XElement( "search",
                                                new XAttribute( "name", CswTools.SafeJavascriptParam( MetaDataProp.PropNameWithQuestionNo ) ),
                                                new XAttribute( "id", PropId ) 
                                                );
                            Searches.Add( Search );
                        }
                    }
                }
            }
            return Searches;
        } // _getSearchNodes


		private void _runTreeNodesRecursive( ICswNbtTree Tree, ref XElement ParentXmlNode )
		{
			string ret = string.Empty;
			for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
			{
				Tree.goToNthChild( c );

				CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
				string ThisNodeName = Tree.getNodeNameForCurrentPosition();
				string ThisNodeId = ThisNode.NodeId.ToString();

				XElement ThisXmlNode = new XElement( "node" );
				XElement ThisSubItems = new XElement( "subitems" );
				ThisXmlNode.Add( ThisSubItems );

				_runTreeNodesRecursive( Tree, ref ThisSubItems );

				if( Tree.getNodeShowInTreeForCurrentPosition() )
				{
					if( _ForMobile && Tree.getChildNodeCount() == 0 )   // is a leaf
					{
						_runProperties( ThisNode, ref ThisSubItems );
					}

					if( _ForMobile )
					{
						ThisXmlNode.SetAttributeValue( "id", NodeIdPrefix + ThisNodeId );
						ThisXmlNode.SetAttributeValue( "name", CswTools.SafeJavascriptParam( ThisNodeName ) );
						ThisXmlNode.SetAttributeValue( "nodetype", CswTools.SafeJavascriptParam( ThisNode.NodeType.NodeTypeName ) );
						ThisXmlNode.SetAttributeValue( "objectclass", CswTools.SafeJavascriptParam( ThisNode.ObjectClass.ObjectClass.ToString() ) );
						ThisXmlNode.SetAttributeValue( "iconfilename", CswTools.SafeJavascriptParam( ThisNode.NodeType.IconFileName ) );
					}
					else
					{
						//ret += "<item id=\"" + NodeIdPrefix + ThisNodeId + "\"><content><name>" + ThisNodeName + "</name></content>";
					}

					// case 20083 - search values
					foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps )
					{
						if( MetaDataProp.MobileSearch )
						{
							if( MetaDataProp.ObjectClassProp != null )
							{
								//ret += " search_ocp_" + MetaDataProp.ObjectClassPropId.ToString() + "=\"" + CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) + "\"";
								ThisXmlNode.SetAttributeValue( "search_ocp_" + MetaDataProp.ObjectClassPropId.ToString(), CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) );
							}
							else
							{
								//ret += " search_ntp_" + MetaDataProp.PropId.ToString() + "=\"" + CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) + "\"";
								ThisXmlNode.SetAttributeValue( "search_ntp_" + MetaDataProp.PropId.ToString(), CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) );
							}
						}
					}

					//if( _ForMobile )
					//{
					//    ret += "><subitems>" + ThisSubItems + "</subitems>";
					//    ret += "</node>";
					//}
					//else
					//{
					//    ret += ThisSubItems;
					//    ret += "</item>";
					//}
					ParentXmlNode.Add( ThisXmlNode );
				} // if( Tree.getNodeShowInTreeForCurrentPosition() )
				else
				{
					//ret += ThisSubItems;
					ParentXmlNode.Add( ThisSubItems );
				}
				Tree.goToParentNode();
			}
			//return ret;
		} // _runTreeNodesRecursive()


        private void _runProperties( CswNbtNode Node, ref XElement SubItemsXmlNode )
        {
            //XElement Props = new XElement( "subitems" );
            foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                {
                    if( !Prop.HideInMobile &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid ) // Case 20772
                    {
                        CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                        XmlDocument XmlDoc = new XmlDocument();
                        CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
                        PropWrapper.ToXml( XmlDoc.DocumentElement );

						SubItemsXmlNode.Add( new XElement( "prop",
                                        new XAttribute( "id", PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() ),
                                        new XAttribute( "name", CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) ),
                                        new XAttribute( "tab", CswTools.SafeJavascriptParam( Tab.TabName ) ),
                                        new XAttribute( "readonly", Prop.ReadOnly.ToString().ToLower() ),
                                        new XAttribute( "fieldtype", Prop.FieldType.FieldType.ToString() ),
                                        new XAttribute( "gestalt", CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) ),
                                        new XAttribute( "ocpname", CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) ),
                                        XElement.Parse( XmlDoc.InnerXml )
                            ) );
                    }
                }
            }

            //return Props;
		} // _runProperties()


    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices
