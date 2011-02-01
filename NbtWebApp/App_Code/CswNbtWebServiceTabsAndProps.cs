using System;
using System.Collections.Generic;
using System.Collections;
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
    public class CswNbtWebServiceTabsAndProps
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public string getTabs( string NodePkString )
        {
            string ret = string.Empty;

            CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkString );

            CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
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

            CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
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
                            ret += " required=\"" + Prop.IsRequired.ToString().ToLower() + "\"";
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

        public void saveProps(string NodePkString, string PropXml)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml( UpdatedViewXml );

            XmlNodeList PropNodes = XmlDoc.SelectNodes( "//prop[@wasmodified='1']" );
            foreach( XmlNode PropNode in PropNodes )
            {
                string NodePropId = PropNode.Attributes["id"].Value;
                string[] SplitNodePropId = NodePropId.Split( '_' );
                Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                CswNbtNode Node = _CswNbtWebServiceResources.CswNbtResources.Nodes[NodePk];
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtWebServiceResources.CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );
                Node.postChanges( false );
            }

            // return the refreshed view
            CswNbtWebServiceView ViewService = new CswNbtWebServiceView( _CswNbtWebServiceResources, _ForMobile );
            return ViewService.Run( ParentId );
        }

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
