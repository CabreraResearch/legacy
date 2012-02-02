using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTable
    {
        Int32 MaxLength = 35;
        
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        } //ctor

        public JObject getTable( CswNbtView View, CswNbtNode SelectedNode )
        {
            JObject ret = new JObject();

            // Add 'default' Table layout elements for the nodetype to the view for efficiency
            Int32 Order = -1000;
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

        private string _Truncate( string InStr )
        {
            string OutStr = InStr;
            if( OutStr.Length > MaxLength )
            {
                OutStr = OutStr.Substring( 0, MaxLength ) + "...";
            }
            return OutStr;
        } // _Truncate()

        private JObject _makeNodeObj( CswNbtView View, ICswNbtTree Tree )
        {
            JObject ret = new JObject();
            CswPrimaryKey NodeId = Tree.getNodeIdForCurrentPosition();
            CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );

            ret["nodename"] = _Truncate( Tree.getNodeNameForCurrentPosition() );
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

            if( NodeType != null )
            {
                // default image, overridden below
                ret["thumbnailurl"] = "Images/icons/" + NodeType.IconFileName;
            }

            // Map property order to insert position
            Dictionary<Int32, Int32> OrderMap = new Dictionary<Int32, Int32>();
            foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
            {
                Int32 ThisOrder = 0;
                foreach( CswNbtViewProperty OtherViewProp in ViewRel.Properties )
                {
                    if( OtherViewProp.Order < ViewProp.Order || ViewProp.Order == Int32.MinValue )
                    {
                        ThisOrder++;
                    }
                }
                while( OrderMap.ContainsValue( ThisOrder ) )
                {
                    ThisOrder++;
                }
                OrderMap.Add( ViewProp.NodeTypePropId, ThisOrder );
            } // foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )

            // Props in the View
            SortedList<Int32, JObject> PropObjs = new SortedList<Int32, JObject>();
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
                    ThisProp["gestalt"] = _Truncate( Gestalt );
                    
                    PropObjs.Add(OrderMap[NodeTypePropId], ThisProp);
                }
            } // foreach( XElement PropElm in NodeElm.Elements() )

            // insert in order
            JArray PropsArray = new JArray();
            foreach( JObject PropObj in PropObjs.Values )
            {
                PropsArray.Add( PropObj );
            }
            ret["props"] = PropsArray;

            return ret;
        } // _makeNodeObj()


    } // class CswNbtWebServiceTable
} // namespace ChemSW.Nbt.WebServices