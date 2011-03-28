using System;
using System.Xml;
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
        private CswNbtWebServiceResources _CswNbtWebServiceResources;
        private bool _ForMobile;

        public CswNbtWebServiceMobileView( CswNbtWebServiceResources CswNbtWebServiceResources, bool ForMobile )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
            _ForMobile = ForMobile;
        }

        private static string ViewIdPrefix = "viewid_";
        private static string PropIdPrefix = "prop_";
        private static string TabIdPrefix = "tab_";
        private static string NodeIdPrefix = "nodeid_";
        private static string NodeKeyPrefix = "nodekey_";

        public XElement Run( string ParentId, ICswNbtUser CurrentUser )
        {
            XElement ret = new XElement( "node",
                                new XAttribute("id", Int32.MinValue),
                                new XAttribute( "name", "No Results" ) 
                            );

            if( ParentId.StartsWith( ViewIdPrefix ) )
            {
                // Get the full XML for the entire view
                Int32 ViewId = CswConvert.ToInt32( ParentId.Substring( ViewIdPrefix.Length ) );
                CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtWebServiceResources.CswNbtResources, ViewId );
                //View.SaveToCache();
                //Session["SessionViewId"] = View.SessionViewId;

                // case 20083
                if( _ForMobile )
                {
                    ret = _getSearchNodes( View );
                }

                ICswNbtTree Tree = _CswNbtWebServiceResources.CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );

                if( Tree.getChildNodeCount() > 0 )
                {
                    ret = _runTreeNodesRecursive( Tree );
                }

            }// if( ParentId.StartsWith( ViewIdPrefix ) )
            else
            {
                // All Views
                XElement MobileViews = _CswNbtWebServiceResources.CswNbtResources.ViewSelect.getVisibleViewsXml( CurrentUser, true, false, string.Empty );
                ret = MobileViews;
            }

            return ret;
        } // Run()

        // case 20083 - search options
        private XElement _getSearchNodes( CswNbtView View )
        {
            XElement Searches = new XElement( "searches" );
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtWebServiceResources.CswNbtResources.MetaData.LatestVersionNodeTypes )
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

        private void _runTreeNodesRecursive1( ICswNbtTree Tree, string IDPrefix, XElement GrandParentNode )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

                string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToString() );
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = IDPrefix + ThisNode.NodeId.ToString();
                string ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;

                string ThisNodeState = "closed";
                if( ThisNodeKey.NodeSpecies == NodeSpecies.More )
                    ThisNodeState = "leaf";

                var ParentNode = ( new XElement( "item",
                                        new XAttribute( "id", ThisNodeId ),
                                        new XAttribute( "rel", ThisNodeRel ),
                                        new XAttribute( "state", ThisNodeState ),
                                        new XAttribute( "species", ThisNodeKey.NodeSpecies.ToString() ),
                                        new XAttribute( "cswnbtnodekey", ThisNodeKeyString ),
                                            new XElement( "content",
                                                new XElement( "name", ThisNodeName )
                                                )
                                            )
                                    );
                if( Tree.getChildNodeCount() > 0 )
                {
                    // XElement ChildNode = _runTreeNodesRecursive()
                    _runTreeNodesRecursive( Tree, IDPrefix, ParentNode );
                }

                GrandParentNode.Add( ParentNode );
                Tree.goToParentNode();
            } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )

        } // _runTreeNodesRecursive()

        private void _runTreeNodesRecursive( ICswNbtTree Tree, XElement GrandParentNode, XElement PropertiesNode )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                _runTreeNodesRecursive( Tree, GrandParentNode, PropertiesNode );

                if( Tree.getNodeShowInTreeForCurrentPosition() )
                {
                    if( _ForMobile && PropertiesNode == null )
                    {
                        PropertiesNode = _runProperties( ThisNode );
                    }

                    if( _ForMobile )
                    {
                        ret += "<node id=\"" + NodeIdPrefix + ThisNodeId + "\"";
                        ret += " name=\"" + CswTools.SafeJavascriptParam( ThisNodeName ) + "\"";
                        ret += " nodetype=\"" + CswTools.SafeJavascriptParam( ThisNode.NodeType.NodeTypeName ) + "\"";
                        ret += " objectclass=\"" + CswTools.SafeJavascriptParam( ThisNode.ObjectClass.ObjectClass.ToString() ) + "\"";
                        ret += " iconfilename=\"" + CswTools.SafeJavascriptParam( ThisNode.NodeType.IconFileName ) + "\"";
                    }
                    else
                    {
                        ret += "<item id=\"" + NodeIdPrefix + ThisNodeId + "\"><content><name>" + ThisNodeName + "</name></content>";
                    }

                    // case 20083 - search values
                    foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps )
                    {
                        if( MetaDataProp.MobileSearch )
                        {
                            if( MetaDataProp.ObjectClassProp != null )
                                ret += " search_ocp_" + MetaDataProp.ObjectClassPropId.ToString() + "=\"" + CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) + "\"";
                            else
                                ret += " search_ntp_" + MetaDataProp.PropId.ToString() + "=\"" + CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt ) + "\"";
                        }
                    }

                    if( _ForMobile )
                    {
                        ret += "><subitems>" + ThisSubItems + "</subitems>";
                        ret += "</node>";
                    }
                    else
                    {
                        ret += ThisSubItems;
                        ret += "</item>";
                    }
                } // if( Tree.getNodeShowInTreeForCurrentPosition() )
                else
                {
                    ret += ThisSubItems;
                }
                Tree.goToParentNode();
            }
            return ret;
        }


        private XElement _runProperties( CswNbtNode Node )
        {
            XElement Props = new XElement( "No Results" );
            foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                {
                    if( !Prop.HideInMobile &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid ) // Case 20772
                    {
                        CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                        Props.Add( new XElement("prop",
                                        new XAttribute( "id", PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() ),
                                        new XAttribute( "name", CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) ),
                                        new XAttribute( "tab", CswTools.SafeJavascriptParam( Tab.TabName ) ),
                                        new XAttribute( "readonly", Prop.ReadOnly.ToString().ToLower() ),
                                        new XAttribute( "fieldtype", Prop.FieldType.FieldType.ToString() ),
                                        new XAttribute( "gestalt", CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) ),
                                        new XAttribute( "ocpname", CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) )
                            
                            ) );
                        ret += "<prop id=\"" + + "\"";
                        ret += " =\"" +  + "\"";
                        ret += " =\"" +  + "\"";
                        ret += " =\"" +  + "\"";
                        ret += " =\"" +  + "\"";
                        ret += " =\"" + + "\"";
                        ret += " =\"" +  + "\"";
                        ret += ">";
                        XmlDocument XmlDoc = new XmlDocument();
                        CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
                        PropWrapper.ToXml( XmlDoc.DocumentElement );
                        ret += CswTools.SafeJavascriptParam( XmlDoc.DocumentElement.InnerXml );
                        ret += "<subitems></subitems>";
                        ret += "</prop>";
                    }
                }
            }

            return ret;
        }


    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices
