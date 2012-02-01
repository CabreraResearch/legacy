using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTable
    {
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        } //ctor

        public JObject getTable( CswNbtView View, CswNbtNode SelectedNode )
        {
            JObject ret = new JObject();
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );
                ret[Tree.getNodeIdForCurrentPosition().ToString()] = _makeNodeObj( View, Tree );
                Tree.goToParentNode();
            }

            if( Tree.getCurrentNodeChildrenTruncated() )
            {
                ret["truncated"] = new JObject( new JProperty( "nodename", "Results Truncated" ) );
            }

            return ret;

        } // getTable()

        private JObject _makeNodeObj( CswNbtView View, ICswNbtTree Tree )
        {
            JObject ret = new JObject();
            CswPrimaryKey NodeId = Tree.getNodeIdForCurrentPosition();
            CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );

            ret = new JObject();
            ret["nodename"] = Tree.getNodeNameForCurrentPosition();
            ret["nodeid"] = NodeId.ToString();
            ret["nodekey"] = NodeKey.ToString();
            ret["locked"] = Tree.getNodeLockedForCurrentPosition().ToString().ToLower();

            CswNbtViewRelationship ViewRel = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( NodeKey.ViewNodeUniqueId );
            bool CanView = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.View, NodeType );
            bool CanEdit = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType );
            bool CanDelete = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Delete, NodeType );
            ret["allowview"] = ( ViewRel.AllowView && CanView ).ToString().ToLower();
            ret["allowedit"] = ( ViewRel.AllowEdit && CanEdit ).ToString().ToLower();
            ret["allowdelete"] = ( ViewRel.AllowDelete && CanDelete ).ToString().ToLower();

            ret["props"] = new JObject();
            if( NodeType != null )
            {
                // default image, overridden below
                ret["thumbnailurl"] = "Images/icons/" + NodeType.IconFileName;
            }

            foreach( XElement PropElm in Tree.getChildNodePropsOfNode() )
            {
                Int32 NodeTypePropId = CswConvert.ToInt32( PropElm.Attribute( "nodetypepropid" ).Value );
                Int32 JctNodePropId = CswConvert.ToInt32( PropElm.Attribute( "jctnodepropid" ).Value );
                CswPropIdAttr PropId = new CswPropIdAttr( NodeId, NodeTypePropId );
                string FieldType = PropElm.Attribute( "fieldtype" ).Value;

                // Special case: Image becomes thumbnail
                if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image.ToString() )
                {
                    ret["thumbnailurl"] = CswNbtNodePropImage.makeImageUrl( JctNodePropId, NodeId, NodeTypePropId );
                }
                else
                {
                    ret["props"][PropId.ToString()] = new JObject();
                    ret["props"][PropId.ToString()]["propname"] = PropElm.Attribute( "name" ).Value;
                    ret["props"][PropId.ToString()]["gestalt"] = PropElm.Attribute( "gestalt" ).Value;
                }
            } // foreach( XElement PropElm in NodeElm.Elements() )
            return ret;
        } // _makeNodeObj()

    } // class CswNbtWebServiceTable
} // namespace ChemSW.Nbt.WebServices