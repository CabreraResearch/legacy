using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;
using NbtWebApp.WebSvc.Logic.CISPro;

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
        public JObject getProps( string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, CswDateTime Date, string filterToPropId, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            return _TabsPropsSd.getProps( NodeId, NodeKey, TabId, NodeTypeId, Date, filterToPropId, RelatedNodeId, RelatedNodeTypeId, RelatedObjectClassId );
        } // getProps()

        /// <summary>
        /// Returns JObject for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            return _TabsPropsSd.getSingleProp( NodeId, NodeKey, PropIdFromJson, NodeTypeId, NewPropJson );
        } // getProp()


        public JProperty makePropJson( CswPrimaryKey NodeId, Int32 TabId, CswNbtMetaDataNodeTypeProp Prop, CswNbtNodePropWrapper PropWrapper, Int32 Row, Int32 Column, string TabGroup )
        {
            return _TabsPropsSd.makePropJson( NodeId, TabId, Prop, PropWrapper, Row, Column, TabGroup );
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

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _TabsPropsSd.addNode( NodeType, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
        }

        public JObject saveProps( CswPrimaryKey NodePk, Int32 TabId, JObject NewPropsJson, Int32 NodeTypeId, CswNbtView View, bool setIsTempToFalse, bool IsIdentityTab )
        {
            return _TabsPropsSd.saveProps( NodePk, TabId, NewPropsJson, NodeTypeId, View, IsIdentityTab, setIsTempToFalse );
        } // saveProps()

        public JObject copyPropValues( string SourceNodeId, string CopyNodeIds, string PropIds )
        {
            return _TabsPropsSd.copyPropValues( SourceNodeId, CopyNodeIds, PropIds );
        } // copyPropValues()

        public JArray getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            return _TabsPropsSd.getPropertiesForLayoutAdd( NodeId, NodeKey, NodeTypeId, TabId, LayoutType );
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            return _TabsPropsSd.addPropertyToLayout( PropId, TabId, LayoutType );
        } // addPropertyToLayout()

        public bool ClearPropValue( string PropIdAttr, bool IncludeBlob )
        {
            return _TabsPropsSd.ClearPropValue( PropIdAttr, IncludeBlob );
        } // ClearPropValue()

        public bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, string PropIdAttr, string Column, out string Href )
        {
            return _TabsPropsSd.SetPropBlobValue( Data, FileName, ContentType, PropIdAttr, Column, out Href );
        } // SetPropBlobValue()

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
                    ParentObj["entries"][Entry.NodeType.NodeTypeName] = CswNbtWebServiceMainMenu.makeAddMenuItem( Entry.NodeType, new CswPrimaryKey(), string.Empty, string.Empty, string.Empty );
                }
            }
            JObject ChildObj = new JObject();
            ParentObj["children"] = ChildObj;

            // recurse
            foreach( CswNbtViewRelationship ChildViewRel in ViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                _getDefaultContentRecursive( ChildObj, ChildViewRel );
            }
        } // _getDefaultContentRecursive()

        public JObject getObjectClassButtons( string ObjectClassId )
        {
            return _TabsPropsSd.getObjectClassButtons( ObjectClassId );
        }

        public JObject getLocationTree( string NodeId )
        {
            return _TabsPropsSd.getLocationTree( NodeId );
        }

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
