using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTable
    {
        Int32 MaxLength = 35;

        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtView _View;
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        public CswNbtWebServiceTable( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, CswNbtView View )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _View = View;
            _CswNbtResources.EditMode = NodeEditMode.Table;
        }

        public JObject getTable()
        {
            JObject ret = new JObject();

            // Add 'default' Table layout elements for the nodetype to the view for efficiency
            ICswNbtTree Tree = null;
            if( _View != null )
            {
                Int32 Order = -1000;
                foreach( CswNbtViewRelationship ViewRel in _View.Root.ChildRelationships )
                {
                    if( ViewRel.SecondType == NbtViewRelatedIdType.NodeTypeId )
                    {
                        IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( ViewRel.SecondId, Int32.MinValue, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table );
                        foreach( CswNbtMetaDataNodeTypeProp NTProp in Props )
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
                                CswNbtViewProperty NewViewProp = _View.AddViewProperty( ViewRel, NTProp );
                                NewViewProp.Order = Order;
                                Order++;
                            }
                        } // foreach( CswNbtMetaDataNodeTypeProp NTProp in Props )
                    } // if( ViewRel.SecondType == RelatedIdType.NodeTypeId )
                } // foreach( CswNbtViewRelationship ViewRel in View.Root.ChildRelationships )

                Tree = _CswNbtResources.Trees.getTreeFromView( _View, false );
                ret = makeTableFromTree( Tree, null );
            } // if( _View != null )
            return ret;
        } // getTable()

        public JObject makeTableFromTree( ICswNbtTree Tree, Collection<Int32> PropsToHide )
        {
            JObject ret = new JObject();
            if( Tree != null )
            {
                ret["results"] = Tree.getChildNodeCount().ToString();
                JArray NodesArray = new JArray();
                for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                {
                    Tree.goToNthChild( c );
                    NodesArray.Add( _makeNodeObj( Tree, PropsToHide ) );
                    Tree.goToParentNode();
                }

                if( Tree.getCurrentNodeChildrenTruncated() )
                {
                    NodesArray.Add( new JObject( new JProperty( "nodename", "Results Truncated" ) ) );
                }
                ret["nodes"] = NodesArray;
            }
            return ret;

        } // makeTableFromTree()

        private string _Truncate( string InStr )
        {
            string OutStr = InStr;
            if( OutStr.Length > MaxLength )
            {
                OutStr = OutStr.Substring( 0, MaxLength ) + "...";
            }
            return OutStr;
        } // _Truncate()

        private JObject _makeNodeObj( ICswNbtTree Tree, Collection<Int32> PropsToHide )
        {
            CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
            CswNbtViewRelationship ViewRel = null;
            if( _View != null )
            {
                ViewRel = (CswNbtViewRelationship) _View.FindViewNodeByUniqueId( NodeKey.ViewNodeUniqueId );
            }

            return makeNodeObj( Tree.getNodeIdForCurrentPosition(),
                                NodeKey,
                                Tree.getNodeNameForCurrentPosition(),
                                Tree.getNodeLockedForCurrentPosition(),
                                Tree.getNodeIncludedForCurrentPosition(),
                                _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId ),
                                Tree.getChildNodePropsOfNode(),
                                PropsToHide,
                                ViewRel );
        }

        public JObject makeNodeObj( CswPrimaryKey NodeId,
                                    CswNbtNodeKey NodeKey,
                                    string NodeName,
                                    bool Locked,
                                    bool Included,
                                    CswNbtMetaDataNodeType NodeType,
                                    JArray TreeProps,
                                    Collection<Int32> PropsToHide,
                                    CswNbtViewRelationship ViewRel = null )
        {
            JObject ret = new JObject();
            //CswPrimaryKey NodeId = Tree.getNodeIdForCurrentPosition();
            //CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();
            //CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeKey.NodeTypeId );

            ret["nodename"] = _Truncate( NodeName ); // Tree.getNodeNameForCurrentPosition() );
            ret["nodeid"] = NodeId.ToString();
            if( NodeKey != null )
            {
                ret["nodekey"] = NodeKey.ToString();
            }
            ret["locked"] = Locked.ToString().ToLower();
            ret["disabled"] = ( false == Included ).ToString().ToLower();

            //CswNbtViewRelationship ViewRel = (CswNbtViewRelationship) View.FindViewNodeByUniqueId( NodeKey.ViewNodeUniqueId );
            bool CanView = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.View, NodeType );
            bool CanEdit = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType );
            bool CanDelete = _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Delete, NodeType );
            if( ViewRel != null )
            {
                CanView = CanView && ViewRel.AllowView;
                CanEdit = CanEdit && ViewRel.AllowEdit;
                CanDelete = CanDelete && ViewRel.AllowDelete;
            }
            ret["allowview"] = CanView;
            ret["allowedit"] = CanEdit;
            ret["allowdelete"] = CanDelete;

            if( NodeType != null )
            {
                // default image, overridden below
                if( NodeType.IconFileName != string.Empty )
                {
                    ret["thumbnailurl"] = CswNbtMetaDataObjectClass.IconPrefix100 + NodeType.IconFileName;
                }
                else
                {
                    ret["thumbnailurl"] = "Images/icons/300/_placeholder.gif";
                }
            }

            // Map property order to insert position
            Dictionary<Int32, Int32> OrderMap = new Dictionary<Int32, Int32>();
            if( ViewRel != null )
            {
                foreach( CswNbtViewProperty ViewProp in ViewRel.Properties )
                {
                    Int32 ThisOrder = 0;
                    foreach( CswNbtViewProperty OtherViewProp in ViewRel.Properties )
                    {
                        if( ( OtherViewProp.Order != Int32.MinValue && OtherViewProp.Order < ViewProp.Order ) ||
                            ViewProp.Order == Int32.MinValue )
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
            }

            // Props in the View
            SortedList<Int32, JObject> PropObjs = new SortedList<Int32, JObject>();
            //foreach( JObject PropElm in Tree.getChildNodePropsOfNode() )
            Int32 OrderCnt = 100;
            foreach( JObject PropElm in TreeProps )
            {
                if( false == CswConvert.ToBoolean( PropElm["hidden"] ) )
                {
                    Int32 NodeTypePropId = CswConvert.ToInt32( PropElm["nodetypepropid"].ToString() );
                    if( PropsToHide == null || false == PropsToHide.Contains( NodeTypePropId ) )
                    {
                        CswPropIdAttr PropId = new CswPropIdAttr( NodeId, NodeTypePropId );
                        string FieldType = PropElm["fieldtype"].ToString();
                        string PropName = PropElm["propname"].ToString();
                        string Gestalt = PropElm["gestalt"].ToString();
                        Int32 JctNodePropId = CswConvert.ToInt32( PropElm["jctnodepropid"].ToString() );

                        // Special case: Image becomes thumbnail
                        if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image.ToString() )
                        {
                            ret["thumbnailurl"] = CswNbtNodePropImage.getLink( JctNodePropId, NodeId, NodeTypePropId );
                        }

                        if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.MOL.ToString() )
                        {
                            ret["thumbnailurl"] = CswNbtNodePropMol.getLink( JctNodePropId, NodeId, NodeTypePropId );
                        }
                        else
                        {
                            JObject ThisProp = new JObject();
                            ThisProp["propid"] = PropId.ToString();
                            ThisProp["propname"] = PropName;
                            ThisProp["gestalt"] = _Truncate( Gestalt );
                            ThisProp["fieldtype"] = FieldType;

                            if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button.ToString() )
                            {
                                // Include full info for rendering the button
                                // This was done in such a way as to prevent instancing the CswNbtNode object, 
                                // which we don't need for Buttons.
                                CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

                                CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources, _CswNbtStatisticsEvents );
                                JProperty JpPropData = ws.makePropJson( NodeId, NodeTypeProp.FirstEditLayout.TabId, NodeTypeProp, null, Int32.MinValue, Int32.MinValue, string.Empty );
                                JObject PropData = (JObject) JpPropData.Value;

                                JObject PropValues = new JObject();
                                CswNbtNodePropButton.AsJSON( NodeTypeProp, PropValues, CswConvert.ToString( PropElm["field2"] ), CswConvert.ToString( PropElm["field1"] ) );
                                PropData["values"] = PropValues;

                                ThisProp["propData"] = PropData;
                            }
                            //if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Link.ToString() )
                            if( OrderMap.ContainsKey( NodeTypePropId ) )
                            {
                                PropObjs.Add( OrderMap[NodeTypePropId], ThisProp );
                            }
                            else
                            {
                                PropObjs.Add( OrderCnt, ThisProp );
                                OrderCnt++;
                            }
                        }
                    } // if( false == PropsToHide.Contains( NodeTypePropId ) )
                } //if (false == CswConvert.ToBoolean(PropElm["hidden"]))
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