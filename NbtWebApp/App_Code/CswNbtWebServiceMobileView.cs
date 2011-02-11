using System;
using System.Xml;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
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

        public string Run( string ParentId )
        {
            string ret = string.Empty;

            if( ParentId.StartsWith( ViewIdPrefix ) )
            {
                // Get the full XML for the entire view
                Int32 ViewId = CswConvert.ToInt32( ParentId.Substring( ViewIdPrefix.Length ) );
                CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtWebServiceResources.CswNbtResources, ViewId );
                //View.SaveToCache();
                //Session["SessionViewId"] = View.SessionViewId;

                // case 20083
                if( _ForMobile )
                    ret += "<searches>" + _getSearchNodes( View ) + "</searches>";

                ICswNbtTree Tree = _CswNbtWebServiceResources.CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );

                if( Tree.getChildNodeCount() > 0 )
                    ret += _runTreeNodesRecursive( Tree );
                else
                {
                    ret += @"<node id="""" name=""No results""></node>";
                }

            }// if( ParentId.StartsWith( ViewIdPrefix ) )
            else
            {
                // All Views
                DataTable ViewDT = _CswNbtWebServiceResources.CswNbtResources.ViewSelect.getVisibleViews( false );
                if( ViewDT.Rows.Count > 0 )
                {
                    foreach( DataRow ViewRow in ViewDT.Rows )
                    {
                        ret += "<view id=\"" + ViewIdPrefix + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
                        ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
                        ret += "/>";
                    }
                }
                else
                {
                    ret += @"<node id="""" name=""No results""></node>";
                }
            }

            return ret;
        } // Run()

        // case 20083 - search options
        private string _getSearchNodes( CswNbtView View )
        {
            string ret = string.Empty;
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtWebServiceResources.CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( View.ContainsNodeType( NodeType ) )
                {
                    foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.NodeTypeProps )
                    {
                        if( MetaDataProp.MobileSearch )
                        {
                            ret += "<search name=\"" + CswTools.SafeJavascriptParam(MetaDataProp.PropNameWithQuestionNo )+ "\" id=\"";
                            if( MetaDataProp.ObjectClassProp != null )
                                ret += "search_ocp_" + MetaDataProp.ObjectClassPropId.ToString();
                            else
                                ret += "search_ntp_" + MetaDataProp.PropId.ToString();
                            ret += "\"/>";
                        }
                    }
                }
            }
            return ret;
        } // _getSearchNodes

        private string _runTreeNodesRecursive( ICswNbtTree Tree )
        {
            string ret = string.Empty;
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                string ThisSubItems = _runTreeNodesRecursive( Tree );
                if( _ForMobile && ThisSubItems == string.Empty )
                {
                    ThisSubItems = _runProperties( ThisNode );
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
                            ret += " search_ocp_" + MetaDataProp.ObjectClassPropId.ToString() + "=\"" + CswTools.SafeJavascriptParam(ThisNode.Properties[MetaDataProp].Gestalt) + "\"";
                        else
                            ret += " search_ntp_" + MetaDataProp.PropId.ToString() + "=\"" + CswTools.SafeJavascriptParam(ThisNode.Properties[MetaDataProp].Gestalt) + "\"";
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
                Tree.goToParentNode();
            }
            return ret;
        }


        private string _runProperties( CswNbtNode Node )
        {
            string ret = string.Empty;
            foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
            {
                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                {
                    if( !Prop.HideInMobile &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid ) // Case 20772
                    {
                        CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                        ret += "<prop id=\"" + PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() + "\"";
                        ret += " name=\"" + CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) + "\"";
                        ret += " tab=\"" + CswTools.SafeJavascriptParam( Tab.TabName ) + "\"";
                        ret += " readonly=\"" + Prop.ReadOnly.ToString().ToLower() + "\"";
                        ret += " fieldtype=\"" + Prop.FieldType.FieldType.ToString() + "\"";
                        ret += " gestalt=\"" + CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) + "\"";
                        ret += " ocpname=\"" + CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) + "\"";
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
