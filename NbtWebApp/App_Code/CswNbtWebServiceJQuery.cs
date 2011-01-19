using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private class DashIcon
        {
            public string Text;
            public string Id;
            public string Href;
            public CswNbtResources.CswNbtModule Module;

            public DashIcon( string inText, string inId, string inHref, CswNbtResources.CswNbtModule inModule )
            {
                Text = inText;
                Id = inId;
                Href = inHref;
                Module = inModule;
            }
        }

        public string getDashboard()
        {
            string ret = string.Empty;
            Collection<DashIcon> DashIcons = new Collection<DashIcon>();
            DashIcons.Add( new DashIcon( "IMCS - Instrument Maintenance and Calibration",
                                         "dash_imcs",
                                         "http://www.chemswlive.com/19013.htm",
                                         CswNbtResources.CswNbtModule.IMCS ) );
            DashIcons.Add( new DashIcon( "FE - Fire Extinguisher Inspection",
                                         "dash_fe",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.FE ) );
            DashIcons.Add( new DashIcon( "SI - Site Inspection",
                                         "dash_si",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.SI ) );
            DashIcons.Add( new DashIcon( "STIS - Sample Tracking and Inventory System",
                                         "dash_stis",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.STIS ) );
            DashIcons.Add( new DashIcon( "CISPro - Chemical Inventory System",
                                         "dash_cispro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CISPro ) );
            DashIcons.Add( new DashIcon( "CCPro - Control Charts",
                                         "dash_ccpro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CCPro ) );
            DashIcons.Add( new DashIcon( "BioSafety",
                                         "dash_biosafety",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.BioSafety ) );
            DashIcons.Add( new DashIcon( "Mobile",
                                         "dash_hh",
                                         "http://www.chemswlive.com/cis-pro-mobile.htm",
                                         CswNbtResources.CswNbtModule.Mobile ) );
            DashIcons.Add( new DashIcon( "NBTManager",
                                         "dash_nbtmgr",
                                         "",
                                         CswNbtResources.CswNbtModule.NBTManager ) );

            foreach( DashIcon DashIcon in DashIcons )
            {
                if( _CswNbtWebServiceResources.CswNbtResources.IsModuleEnabled( DashIcon.Module ) )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
                else if( DashIcon.Module != CswNbtResources.CswNbtModule.NBTManager )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "_off\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
            }

            return "<dashboard>" + ret + "</dashboard>";
        } // getDashboard()

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
