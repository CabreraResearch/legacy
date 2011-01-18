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
    public class CswNbtWebServiceJQuery
    {
        private CswNbtWebServiceResources _CswNbtWebServiceResources;

        public CswNbtWebServiceJQuery( CswNbtWebServiceResources CswNbtWebServiceResources )
        {
            _CswNbtWebServiceResources = CswNbtWebServiceResources;
        }

        public string getViews()
        {
            string ret = string.Empty;
            DataTable ViewDT = _CswNbtWebServiceResources.CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtWebServiceResources.CswNbtResources.CurrentNbtUser, false, false );
            foreach( DataRow ViewRow in ViewDT.Rows )
            {
                ret += "<view id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
                ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
                ret += "/>";
            }
            return "<views>" + ret + "</views>";
        }

        //private static string ViewIdPrefix = "viewid_";
        //private static string PropIdPrefix = "prop_";
        //private static string TabIdPrefix = "tab_";
        //private static string NodeIdPrefix = "nodeid_";
        //private static string NodeKeyPrefix = "nodekey_";

        public string getTree( Int32 ViewId )
        {
            string ret = string.Empty;
            ret += @"<item id=""-1""><content><name>No results</name></content></item>";

            CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtWebServiceResources.CswNbtResources, ViewId );

            ICswNbtTree Tree = _CswNbtWebServiceResources.CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );

            if( Tree.getChildNodeCount() > 0 )
                ret = "<root>" + _runTreeNodesRecursive( Tree ) + "</root>";

            return ret;
        } // getTree()


        public string getTabs( string NodePkString )
        {
            string ret = string.Empty;

            CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkString );

            CswNbtNode Node = _CswNbtWebServiceResources.CswNbtResources.Nodes[NodePk];
            if( Node != null )
            {
                ret = "<tabs>";
                foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
                {
                    ret += "<tab id=\"" + Tab.TabId + "\" name=\"" + Tab.TabName + "\" />";
                }
                ret += "</tabs>";
            }
            return ret;
        } // getTabs()

        public string getProps( string NodePkString, string TabId )
        {
            string ret = string.Empty;
                       CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkString );

            CswNbtNode Node = _CswNbtWebServiceResources.CswNbtResources.Nodes[NodePk];
            if( Node != null )
            {
                foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
                {
                    if( Tab.TabId.ToString() == TabId )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                        {
                            CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];
                            ret += "<prop id=\"" + Node.NodeId.ToString() + "_" + Prop.PropId.ToString() + "\"";
                            ret += " name=\"" + Prop.PropNameWithQuestionNo + "\"";
                            ret += " fieldtype=\"" + Prop.FieldType.FieldType.ToString() + "\"";
                            ret += " gestalt=\"" + PropWrapper.Gestalt.Replace( "\"", "&quot;" ) + "\"";
                            ret += " ocpname=\"" + PropWrapper.ObjectClassPropName + "\"";
                            ret += " displayrow=\"" + Prop.DisplayRow.ToString() + "\"";
                            ret += " displaycol=\"" + Prop.DisplayColumn.ToString() + "\"";
                            ret += ">";
                            XmlDocument XmlDoc = new XmlDocument();
                            CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
                            PropWrapper.ToXml( XmlDoc.DocumentElement );
                            ret += XmlDoc.DocumentElement.InnerXml;
                            ret += "</prop>";
                        }
                    }
                }
            }
            return "<props>" + ret + "</props>";
        } // getTab()

        private string _runTreeNodesRecursive( ICswNbtTree Tree )
        {
            string ret = string.Empty;
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                ret += "<item id=\"" + ThisNodeId + "\"><content><name>" + ThisNodeName + "</name></content>";
                ret += _runTreeNodesRecursive( Tree );
                ret += "</item>";

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
                        //ret += "<prop id=\"" + PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId.ToString() + "\"";
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


    } // class CswNbtWebServiceJQuery

} // namespace ChemSW.Nbt.WebServices
