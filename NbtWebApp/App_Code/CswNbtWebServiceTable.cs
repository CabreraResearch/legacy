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

            // Add 'default' Table layout elements for the nodetype to the view for efficiency
            Int32 Order = 10;
            foreach( CswNbtViewRelationship ViewRel in View.Root.ChildRelationships )
            {
                if( ViewRel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    foreach( CswNbtMetaDataNodeTypeProp NTProp in _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( ViewRel.SecondId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table ) )
                    {
                        bool AlreadyExists = false;
                        foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                        {
                            if( ViewProp.NodeTypePropId == NTProp.PropId )
                            {
                                AlreadyExists = true;
                            }
                        }

                        if( false == AlreadyExists )
                        {
                            CswNbtViewProperty NewViewProp = View.AddViewProperty( ViewRel, NTProp );
                            NewViewProp.Order = Order;
                            Order++;
                        }
                    } // foreach( CswNbtMetaDataNodeTypeProp NTProp in _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( ViewRel.SecondId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table ) )
                } // if( ViewRel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
            } // foreach( CswNbtViewRelationship ViewRel in View.Root.ChildRelationships )

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

            JArray PropsArray = new JArray();
            if( NodeType != null )
            {
                // default image, overridden below
                ret["thumbnailurl"] = "Images/icons/" + NodeType.IconFileName;
            }

            //// Props in the Table Layout of the node
            //// This is gonna be expensive.
            //foreach( CswNbtMetaDataNodeTypeProp Prop in  )
            //{
            //    CswPropIdAttr PropId = new CswPropIdAttr( NodeId, Prop.PropId );
            //    CswNbtNode CurrentNode = Tree.getNodeForCurrentPosition();
            //    CswNbtNodePropWrapper PropWrapper = CurrentNode.Properties[Prop];
            //    _handleProp( ret,
            //                 PropId,
            //                 Prop.FieldType.FieldType.ToString(),
            //                 Prop.PropName,
            //                 PropWrapper.Gestalt,
            //                 PropWrapper.JctNodePropId );
            //}

            // Props in the View
            foreach( XElement PropElm in Tree.getChildNodePropsOfNode() )
            {
                Int32 NodeTypePropId = CswConvert.ToInt32( PropElm.Attribute( "nodetypepropid" ).Value );
                CswPropIdAttr PropId = new CswPropIdAttr( NodeId, NodeTypePropId );
                string FieldType = PropElm.Attribute( "fieldtype" ).Value;
                string PropName = PropElm.Attribute( "name" ).Value;
                string Gestalt = PropElm.Attribute( "gestalt" ).Value;
                Int32 JctNodePropId = CswConvert.ToInt32( PropElm.Attribute( "jctnodepropid" ).Value );

                // Special case: Image becomes thumbnail
                if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image.ToString() )
                {
                    ret["thumbnailurl"] = CswNbtNodePropImage.makeImageUrl( JctNodePropId, NodeId, NodeTypePropId );
                }
                else
                {
                    JObject ThisProp = new JObject();
                    ThisProp["propid"] = PropId.ToString();
                    ThisProp["propname"] = PropName;
                    ThisProp["gestalt"] = Gestalt;
                    PropsArray.Add( ThisProp );
                }
            } // foreach( XElement PropElm in NodeElm.Elements() )
            ret["props"] = PropsArray;

            return ret;
        } // _makeNodeObj()


    } // class CswNbtWebServiceTable
} // namespace ChemSW.Nbt.WebServices