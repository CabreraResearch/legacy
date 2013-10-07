using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTabsAndProps
    {
        private CswNbtSdTabsAndProps _TabsPropsSd;

        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, bool Multi = false, bool ConfigMode = false )
        {
            _TabsPropsSd = new CswNbtSdTabsAndProps( CswNbtResources, CswNbtStatisticsEvents, Multi, ConfigMode );

        }

        public JObject getTabs( string NodeId, string NodeKey, CswDateTime Date, string filterToPropId )
        {
            return _TabsPropsSd.getTabs( NodeId, NodeKey, Date, filterToPropId );
        } // getTabs()

        public void _makeTab( JObject ParentObj, Int32 TabOrder, string Id, string Name, bool CanEditLayout )
        {
            _TabsPropsSd._makeTab( ParentObj, TabOrder, Id, Name, CanEditLayout );
        }

        /// <summary>
        /// Returns JObject for all properties in a given tab
        /// </summary>
        public JObject getProps( string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, string filterToPropId, string RelatedNodeId, bool ForceReadOnly, CswDateTime Date = null )
        {
            return _TabsPropsSd.getProps( NodeId, NodeKey, TabId, NodeTypeId, filterToPropId, RelatedNodeId, ForceReadOnly, Date );
        } // getProps()

        public JObject getIdentityTabProps( CswPrimaryKey NodeId, string filterToPropId, string RelatedNodeId, CswDateTime Date = null )
        {
            return _TabsPropsSd.getIdentityTabProps( NodeId, filterToPropId, RelatedNodeId, Date );
        } // getProps()

        /// <summary>
        /// Returns JObject for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            return _TabsPropsSd.getSingleProp( NodeId, NodeKey, PropIdFromJson, NodeTypeId, NewPropJson );
        } // getProp()


        public JProperty makePropJson( CswPrimaryKey NodeId, Int32 TabId, CswNbtMetaDataNodeTypeProp Prop, CswNbtNodePropWrapper PropWrapper, Int32 Row, Int32 Column, string TabGroup, bool NodeLocked )
        {
            return _TabsPropsSd.makePropJson( NodeId, Prop, PropWrapper, new CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout( Prop.NodeTypeId, Prop.PropId, Row, Column, TabId, null, TabGroup ), NodeLocked: NodeLocked );
        } // makePropJson()


        public void _getAuditHistoryGridProp( JObject ParentObj, CswNbtNode Node )
        {
            _TabsPropsSd._getAuditHistoryGridProp( ParentObj, Node );

        } // _getAuditHistoryGridProp()

        public bool moveProp( string PropIdAttr, Int32 TabId, Int32 NewRow, Int32 NewColumn )
        {
            return _TabsPropsSd.moveProp( PropIdAttr, TabId, NewRow, NewColumn );
        } // moveProp()

        public bool removeProp( string PropIdAttr, Int32 TabId )
        {
            return _TabsPropsSd.removeProp( PropIdAttr, TabId );
        } // removeProp()

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, CswNbtNodeCollection.AfterMakeNode After, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _TabsPropsSd.addNode( NodeType, PropsObj, out RetNbtNodeKey, After, View, NodeTypeTab );
        }

        public JObject saveProps( CswPrimaryKey NodePk, Int32 TabId, JObject NewPropsJson, Int32 NodeTypeId, CswNbtView View, bool IsIdentityTab, bool RemoveTempStatus )
        {
            return _TabsPropsSd.saveProps( NodePk, TabId, NewPropsJson, NodeTypeId, View, IsIdentityTab, RemoveTempStatus );
        } // saveProps()

        public JArray getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, CswEnumNbtLayoutType LayoutType )
        {
            return _TabsPropsSd.getPropertiesForLayoutAdd( NodeId, NodeKey, NodeTypeId, TabId, LayoutType );
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswEnumNbtLayoutType LayoutType )
        {
            return _TabsPropsSd.addPropertyToLayout( PropId, TabId, LayoutType );
        } // addPropertyToLayout()

        public bool ClearPropValue( string PropIdAttr, bool IncludeBlob )
        {
            return _TabsPropsSd.ClearPropValue( PropIdAttr, IncludeBlob );
        } // ClearPropValue()

        /// <summary>
        /// Default content to display when no node is selected, or the tree is empty
        /// </summary>
        public JObject getDefaultContent( CswNbtView View )
        {
            JObject ret = new JObject();
            _getDefaultContentRecursive( ret, View.Root );
            return ret;
        }
        private void _getDefaultContentRecursive( JObject ParentObj, CswNbtViewNode ViewNode )
        {
            Collection<CswNbtViewNode.CswNbtViewAddNodeTypeEntry> Entries = ViewNode.AllowedChildNodeTypes( true );
            if( Entries.Count > 0 )
            {
                ParentObj["entries"] = new JObject();
                foreach( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry in Entries )
                {
                    ParentObj["entries"][Entry.NodeType.NodeTypeName] = CswNbtWebServiceMainMenu.makeAddMenuItem( Entry.NodeType, new CswPrimaryKey(), string.Empty );
                }
            }
            JObject ChildObj = new JObject();
            ParentObj["children"] = ChildObj;

            // recurse
            foreach( CswNbtViewRelationship ChildViewRel in ViewNode.GetChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ) )
            {
                _getDefaultContentRecursive( ChildObj, ChildViewRel );
            }
        } // _getDefaultContentRecursive()

        public JObject getObjectClassButtons( string ObjectClassId )
        {
            return _TabsPropsSd.getObjectClassButtons( ObjectClassId );
        }

        public JObject getLocationView( string NodeId, bool RequireAllowInventory )
        {
            return _TabsPropsSd.getLocationView( NodeId, RequireAllowInventory: RequireAllowInventory );
        }

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
