using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobileView
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly bool _ForMobile;
        private readonly Int32 MobilePageSize = 30;

        public CswNbtWebServiceMobileView( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
            string PageSize = _CswNbtResources.getConfigVariableValue( CswNbtResources.ConfigurationVariables.mobileview_resultlimit.ToString() );
            if( CswTools.IsInteger( PageSize ) )
            {
                MobilePageSize = CswConvert.ToInt32( PageSize );
            }
        }

        private const string PropIdPrefix = "prop_";
        private const string NodeIdPrefix = "nodeid_";


        public XElement getViewsList( string ParentId, ICswNbtUser CurrentUser )
        {
            // All Views
            XElement RetXml = new XElement( "views" );
            Collection<CswNbtView> MobileViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, CurrentUser, false, _ForMobile, false, NbtViewRenderingMode.Any );
            foreach( CswNbtView MobileView in MobileViews )
            {
                RetXml.Add( new XElement( "view",
                        new XAttribute( "id", MobileView.ViewId ),
                        new XAttribute( "name", MobileView.ViewName ) ) );
            }

            return RetXml;
        } // Run()

        public XElement getView( string ViewId, ICswNbtUser CurrentUser )
        {
            XElement RetXml = new XElement( "node" );

            // Get the full XML for the entire view
            CswNbtViewId NbtViewId = new CswNbtViewId( ViewId );
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( NbtViewId );

            // case 20083
            if( _ForMobile )
            {
                RetXml = _getSearchNodes( View );
            }

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false, MobilePageSize );

            if( Tree.getChildNodeCount() > 0 )
            {
                _runTreeNodesRecursive( Tree, ref RetXml );
            }
            else
            {
                RetXml = new XElement( "node",
                            new XAttribute( "id", NodeIdPrefix + Int32.MinValue ),
                            new XAttribute( "name", "No Results" )
                        );
            }

            return RetXml;
        } // Run()

        // case 20083 - search options
        private XElement _getSearchNodes( CswNbtView View )
        {
            XElement Searches = new XElement( "searches" );
            foreach( XElement Search in from CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes
                                        where View.ContainsNodeType( NodeType )
                                        from CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.NodeTypeProps
                                        where MetaDataProp.MobileSearch
                                        let PropId = ( MetaDataProp.ObjectClassProp != null ) ? "search_ocp_" + MetaDataProp.ObjectClassPropId : "search_ntp_" + MetaDataProp.PropId
                                        select new XElement( "search", new XAttribute( "name", CswTools.SafeJavascriptParam( MetaDataProp.PropNameWithQuestionNo ) ), new XAttribute( "id", PropId ) ) )
            {
                Searches.Add( Search );
            }
            return Searches;
        } // _getSearchNodes


        private void _runTreeNodesRecursive( ICswNbtTree Tree, ref XElement ParentXmlNode )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                XElement ThisXmlNode = new XElement( "node" );
                XElement ThisSubItems = new XElement( "subitems" );
                ThisXmlNode.Add( ThisSubItems );

                _runTreeNodesRecursive( Tree, ref ThisSubItems );

                if( Tree.getNodeShowInTreeForCurrentPosition() )
                {
                    if( _ForMobile && Tree.getChildNodeCount() == 0 && NodeSpecies.More != ThisNodeKey.NodeSpecies )   // is a leaf
                    {
                        _runProperties( ThisNode, ref ThisSubItems );
                    }

                    if( _ForMobile )
                    {
                        if( NodeSpecies.More == ThisNodeKey.NodeSpecies )
                        {
                            ThisNodeName = "Results Truncated at " + MobilePageSize;
                        }
                        ThisXmlNode.SetAttributeValue( "id", NodeIdPrefix + ThisNodeId );
                        ThisXmlNode.SetAttributeValue( "name", CswTools.SafeJavascriptParam( ThisNodeName ) );
                        ThisXmlNode.SetAttributeValue( "nodetype", CswTools.SafeJavascriptParam( ThisNode.NodeType.NodeTypeName ) );
                        ThisXmlNode.SetAttributeValue( "objectclass", CswTools.SafeJavascriptParam( ThisNode.ObjectClass.ObjectClass.ToString() ) );
                        ThisXmlNode.SetAttributeValue( "iconfilename", CswTools.SafeJavascriptParam( ThisNode.NodeType.IconFileName ) );
                        ThisXmlNode.SetAttributeValue( "nodespecies", CswTools.SafeJavascriptParam( ThisNodeKey.NodeSpecies.ToString() ) );
                    }

                    // case 20083 - search values
                    foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps.Cast<CswNbtMetaDataNodeTypeProp>().Where( MetaDataProp => MetaDataProp.MobileSearch ) )
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


        private static void _runProperties( CswNbtNode Node, ref XElement SubItemsXmlNode )
        {
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
                        string ReadOnly = ( Node.ReadOnly || Prop.ReadOnly ) ? "true" : "false";
                        XElement PropXElement = new XElement( "prop",
                                            new XAttribute( "id", PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() ),
                                            new XAttribute( "name", CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) ),
                                            new XAttribute( "tab", CswTools.SafeJavascriptParam( Tab.TabName ) ),
                                            new XAttribute( "isreadonly", ReadOnly ),
                                            new XAttribute( "fieldtype", Prop.FieldType.FieldType.ToString() ),
                                            new XAttribute( "gestalt", CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) ),
                                            new XAttribute( "ocpname", CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) ) );
                        SubItemsXmlNode.Add( PropXElement );
                        if( null != XmlDoc.DocumentElement )
                            foreach( XElement ChildXElement in from XmlNode PropValueChildNode
                                                                   in XmlDoc.DocumentElement.ChildNodes
                                                               select XElement.Parse( PropValueChildNode.OuterXml ) )
                            {
                                PropXElement.Add( ChildXElement );
                            }
                    }
                } //foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
            } // foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )

        } // _runProperties()


    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices
