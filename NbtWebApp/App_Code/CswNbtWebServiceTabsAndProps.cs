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

        private const char PropIdDelim = '_';

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
                            ret += "<prop id=\"" + Node.NodeId.ToString() + PropIdDelim.ToString() + Prop.PropId.ToString() + "\"";
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


        public string saveProps( string NodePkString, string NewPropsXml )
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml( NewPropsXml );
            
            CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkString );
            CswNbtNode Node = _CswNbtResources.Nodes[NodePk];

            foreach( XmlNode PropNode in XmlDoc.DocumentElement.ChildNodes )
            {
                string NodePropId = PropNode.Attributes["id"].Value;
                string[] SplitNodePropId = NodePropId.Split( PropIdDelim );
                Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[2] );

                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );
            }
            Node.postChanges( false );

            return "{ \"result\": \"Succeeded\" }";
        } // saveProp()
            

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
