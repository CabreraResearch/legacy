using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Web.Services;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceView
    {
        private CswNbtWebServiceResources _CswNbtWebServiceResources;
        public CswNbtWebServiceView( CswNbtWebServiceResources CswNbtWebServiceResources )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
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
                DataTable ViewDT = _CswNbtWebServiceResources.CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtWebServiceResources.CswNbtResources.CurrentNbtUser, false, true ); 
                foreach( DataRow ViewRow in ViewDT.Rows )
                {
                    ret += "<view id=\"" + ViewIdPrefix + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
                    ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
                    ret += "/>";
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
                            ret += "<search name=\"" + MetaDataProp.PropNameWithQuestionNo + "\" id=\"";
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
                if( ThisSubItems == string.Empty )
                {
                    ThisSubItems = _runProperties( ThisNode );
                }

                ret += "<node id=\"" + NodeIdPrefix + ThisNodeId + "\"";
                ret += " name=\"" + ThisNodeName + "\"";
                ret += " nodetype=\"" + ThisNode.NodeType.NodeTypeName + "\"";
                ret += " objectclass=\"" + ThisNode.ObjectClass.ObjectClass.ToString() + "\"";
                ret += " iconfilename=\"" + ThisNode.NodeType.IconFileName + "\"";

                // case 20083 - search values
                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps )
                {
                    if( MetaDataProp.MobileSearch )
                    {
                        if( MetaDataProp.ObjectClassProp != null )
                            ret += " search_ocp_" + MetaDataProp.ObjectClassPropId.ToString() + "=\"" + ThisNode.Properties[MetaDataProp].Gestalt + "\"";
                        else
                            ret += " search_ntp_" + MetaDataProp.PropId.ToString() + "=\"" + ThisNode.Properties[MetaDataProp].Gestalt + "\"";
                    }
                }

                ret += "><subitems>" + ThisSubItems + "</subitems>";
                ret += "</node>";

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
                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password )
                    {
                        CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                        ret += "<prop id=\"" + PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() + "\"";
                        ret += " name=\"" + Prop.PropNameWithQuestionNo + "\"";
                        ret += " tab=\"" + Tab.TabName + "\"";
                        ret += " fieldtype=\"" + Prop.FieldType.FieldType.ToString() + "\"";
                        ret += " gestalt=\"" + PropWrapper.Gestalt.Replace( "\"", "&quot;" ) + "\"";
                        ret += " ocpname=\"" + PropWrapper.ObjectClassPropName + "\"";
                        ret += ">";
                        XmlDocument XmlDoc = new XmlDocument();
                        CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
                        PropWrapper.ToXml( XmlDoc.DocumentElement );
                        ret += XmlDoc.DocumentElement.InnerXml;
                        ret += "<subitems></subitems>";
                        ret += "</prop>";
                    }
                }
            }

            return ret;
        }


    } // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
