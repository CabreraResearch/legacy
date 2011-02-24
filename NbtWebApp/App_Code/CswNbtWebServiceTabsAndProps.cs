using System;
using System.Collections.ObjectModel;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTabsAndProps
    {
        public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue };

        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public string getTabs( NodeEditMode EditMode, string NodePkString, Int32 NodeTypeId )
        {
            string ret = string.Empty;
            if( EditMode == NodeEditMode.AddInPopup )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                ret = "<tabs>";
                ret += "  <tab id=\"newtab\" name=\"Add New " + NodeType.NodeTypeName + "\" />";
                ret += "</tabs>";
            }
            else
            {
                CswPrimaryKey NodePk = new CswPrimaryKey();
                NodePk.FromString( NodePkString );
                CswNbtNode Node = _CswNbtResources.Nodes[NodePk];

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

        public string getProps( NodeEditMode EditMode, string NodePkString, string TabId, Int32 NodeTypeId )
        {
            string ret = string.Empty;
            if( EditMode == NodeEditMode.AddInPopup )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );

                foreach( CswNbtMetaDataNodeTypeProp Prop in Node.NodeType.NodeTypeProps )
                {
                    if( ( ( Prop.IsRequired && Prop.DefaultValue.Empty ) ||
                          Node.Properties[Prop].TemporarilyRequired ||
                          Prop.SetValueOnAdd ) &&
                        Prop.FilterNodeTypePropId == Int32.MinValue )
                    {
                        ret += _makePropXml( Node, Prop, Prop.DisplayRowAdd, Prop.DisplayColAdd );
                    }
                }
            }
            else
            {
                CswPrimaryKey NodePk = new CswPrimaryKey();
                NodePk.FromString( NodePkString );
                CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
                CswNbtMetaDataNodeTypeTab Tab = Node.NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );

                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                {
                    if( !Prop.hasFilter() )
                    {
                        ret += _makePropXml( Node, Prop, Prop.DisplayRow, Prop.DisplayColumn );
                    }
                }
            }
            return "<props>" + ret + "</props>";
        } // getProps()


        private string _makePropXml( CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 Row, Int32 Column )
        {
            string ret = string.Empty;
            CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

            if( Node.NodeId != null )
                ret += "<prop id=\"" + Node.NodeId.ToString() + PropIdDelim.ToString() + Prop.PropId.ToString() + "\"";
            else
                ret += "<prop id=\"new" + PropIdDelim.ToString() + Prop.PropId.ToString() + "\"";
            ret += " name=\"" + Prop.PropNameWithQuestionNo + "\"";
            ret += " fieldtype=\"" + Prop.FieldType.FieldType.ToString() + "\"";
            if( Prop.ObjectClassProp != null )
            {
                ret += " ocpname=\"" + Prop.ObjectClassProp.PropName + "\"";
            }
            ret += " displayrow=\"" + Row.ToString() + "\"";
            ret += " displaycol=\"" + Column.ToString() + "\"";
            ret += " required=\"" + Prop.IsRequired.ToString().ToLower() + "\"";
            ret += " gestalt=\"" + PropWrapper.Gestalt.Replace( "\"", "&quot;" ) + "\"";
            ret += ">";

            XmlDocument XmlDoc = new XmlDocument();
            CswXmlDocument.SetDocumentElement( XmlDoc, "root" );
            PropWrapper.ToXml( XmlDoc.DocumentElement );
            ret += XmlDoc.DocumentElement.InnerXml;

            ret += "</prop>";
            return ret;
        }

        public string saveProps( NodeEditMode EditMode, string NodePkString, string NewPropsXml, Int32 NodeTypeId )
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml( NewPropsXml );

            CswNbtNode Node = null;
            if( EditMode == NodeEditMode.AddInPopup )
            {
                Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            }
            else
            {
                CswPrimaryKey NodePk = new CswPrimaryKey();
                NodePk.FromString( NodePkString );
                Node = _CswNbtResources.Nodes[NodePk];
            }

            foreach( XmlNode PropNode in XmlDoc.DocumentElement.ChildNodes )
            {
                string NodePropId = PropNode.Attributes["id"].Value;
                string[] SplitNodePropId = NodePropId.Split( PropIdDelim );
                Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[SplitNodePropId.Length - 1] );

                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                Node.Properties[MetaDataProp].ReadXml( PropNode, null, null );
            }
            Node.postChanges( false );

            return "{ \"result\": \"Succeeded\", \"nodeid\": \"" + Node.NodeId.ToString() + "\" }";
        } // saveProp()


    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
