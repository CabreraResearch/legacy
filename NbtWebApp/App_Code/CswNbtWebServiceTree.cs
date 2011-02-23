using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using System.Xml;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTree
    {
        private CswNbtResources _CswNbtResources;
        
        public CswNbtWebServiceTree( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public string getTree( CswNbtView View, string IDPrefix )
        {
            string ret = string.Empty;
            ret += @"<item id=""-1""><content><name>No results</name></content></item>";

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );
            string TreeXml = "<root>" +
                             "  <item id=\""+ IDPrefix + "root\" rel=\"root\">" +
                             "    <content>" +
                             "      <name>" + View.ViewName + "</name>" +
                             "    </content>" +
                                  _runTreeNodesRecursive( Tree, IDPrefix ) +
                             "  </item>" +
                             "</root>";

            if( Tree.getChildNodeCount() > 0 )
                ret = "<result>" +
                      "  <tree>" + TreeXml + "</tree>" +
                      "  <types>{ " + _getTypes( View ) + " }</types>" +
                      "</result>";

            return ret;
        } // getTree()

        public string _getTypes( CswNbtView View )
        {
            string ret = string.Empty;
            Collection<CswNbtMetaDataNodeType> NodeTypes = new Collection<CswNbtMetaDataNodeType>();
            ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
            foreach( CswNbtViewRelationship Rel in Relationships )
            {
                if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
                    NodeTypes.Add( NodeType );
                }
                else
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
                    foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
                    {
                        NodeTypes.Add( NodeType );
                    }
                }
            }

            ret += "\"root\": { " +
                    "    \"icon\": { " +
                    "      \"image\": \"Images/view/viewtree.gif\" " +
                    "    } " +
                    "  }";
            ret += ",\"default\": \"\" ";

            foreach( CswNbtMetaDataNodeType NodeType in NodeTypes )
            {
                ret += ",\"nt_" + NodeType.FirstVersionNodeTypeId.ToString() + "\": { " +
                        "    \"icon\": { " +
                        "      \"image\": \"Images/icons/" + NodeType.IconFileName + "\" " +
                        "    } " +
                        "  }";
            }
            return ret;
        }


        private string _runTreeNodesRecursive( ICswNbtTree Tree, string IDPrefix )
        {
            string ret = string.Empty;
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                string ThisNodeName = Tree.getNodeNameForCurrentPosition();
                string ThisNodeId = ThisNode.NodeId.ToString();

                ret += "<item id=\"" + IDPrefix + ThisNodeId + "\" rel=\"nt_" + ThisNode.NodeType.FirstVersionNodeTypeId.ToString() + "\">";
                ret += "  <content>";
                ret += "    <name>" + ThisNodeName + "</name>";
                ret += "  </content>";
                ret += _runTreeNodesRecursive( Tree, IDPrefix );
                ret += "</item>";

                Tree.goToParentNode();
            }
            return ret;
        }



    } // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
