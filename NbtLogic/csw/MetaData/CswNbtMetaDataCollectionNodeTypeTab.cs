using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeTab : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllNodeTypeTabs;
        private Hashtable _ById;
        private Hashtable _ByNodeType;

        public CswNbtMetaDataCollectionNodeTypeTab( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            _AllNodeTypeTabs = new Collection<ICswNbtMetaDataObject>();
            _ById = new Hashtable();
            _ByNodeType = new Hashtable();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllNodeTypeTabs; } }

        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeTabId )
        {
            CswNbtMetaDataNodeTypeTab ret = null;
            if( _ById.Contains( NodeTypeTabId ) )
                ret = _ById[NodeTypeTabId] as CswNbtMetaDataNodeTypeTab;
            return ret;
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            CswNbtMetaDataNodeTypeTab ret = null;
            if( _ByNodeType.Contains( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByTabId[NodeTypeTabId] as CswNbtMetaDataNodeTypeTab;
            return ret;
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, string NodeTypeTabName )
        {
            CswNbtMetaDataNodeTypeTab ret = null;
            if( _ByNodeType.Contains( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByTabName[NodeTypeTabName.ToLower()] as CswNbtMetaDataNodeTypeTab;
            return ret;
        }
        public ICollection getNodeTypeTabIds( Int32 NodeTypeId )
        {
            ICollection ret;
            if( _ByNodeType.Contains( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByTabId.Keys;
            else
                ret = new ArrayList();
            return ret;
        }
        public ICollection getNodeTypeTabs( Int32 NodeTypeId )
        {
            ICollection ret;
            if( _ByNodeType.Contains( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByTabOrder.Values;
            else
                ret = new ArrayList();
            return ret;
        }

        public void ClearKeys()
        {
            _ById.Clear();
            _ByNodeType.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataNodeTypeTab NodeTypeTab = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a node type prop in the collection
                NodeTypeTab = getNodeTypeTab( PkToOverride );
                Deregister( NodeTypeTab );

                CswNbtMetaDataNodeTypeTab OldNodeTypeTab = new CswNbtMetaDataNodeTypeTab( _CswNbtMetaDataResources, NodeTypeTab._DataRow );
                _AllNodeTypeTabs.Add( OldNodeTypeTab );

                NodeTypeTab.Reassign( Row );
                
                RegisterExisting( OldNodeTypeTab );
                RegisterExisting( NodeTypeTab );
            }
            else
            {
                NodeTypeTab = new CswNbtMetaDataNodeTypeTab( _CswNbtMetaDataResources, Row );
                _AllNodeTypeTabs.Add( NodeTypeTab ); 
                
                RegisterExisting( NodeTypeTab );
            }
            return NodeTypeTab;
        }


        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeTab ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

            _ById.Add( NodeTypeTab.TabId, NodeTypeTab );
            if( !_ByNodeType.ContainsKey( NodeTypeTab.NodeType.NodeTypeId ) )
                _ByNodeType.Add( NodeTypeTab.NodeType.NodeTypeId, new NodeTypeHashEntry() );
            NodeTypeHashEntry Entry = _ByNodeType[NodeTypeTab.NodeType.NodeTypeId] as NodeTypeHashEntry;
            Entry.ByTabId.Add( NodeTypeTab.TabId, NodeTypeTab );
            Entry.ByTabName.Add( NodeTypeTab.TabName.ToLower(), NodeTypeTab );
            Entry.ByTabOrder.Add( CswTools.PadInt(NodeTypeTab.TabOrder, 3) + "_" + NodeTypeTab.TabId, NodeTypeTab );
        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeTab ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

            _ById.Remove( NodeTypeTab.TabId );
            if( _ByNodeType.ContainsKey( NodeTypeTab.NodeType.NodeTypeId ) )
            {
                NodeTypeHashEntry Entry = _ByNodeType[NodeTypeTab.NodeType.NodeTypeId] as NodeTypeHashEntry;
                Entry.ByTabId.Remove( NodeTypeTab.TabId );
                Entry.ByTabName.Remove( NodeTypeTab.TabName.ToLower() );
                Entry.ByTabOrder.Remove( CswTools.PadInt( NodeTypeTab.TabOrder, 3 ) + "_" + NodeTypeTab.TabId );
            }
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeTab ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

            _AllNodeTypeTabs.Remove( NodeTypeTab );
        }
        
        private class NodeTypeHashEntry
        {
            public Hashtable ByTabId = new Hashtable();
            public SortedList ByTabName = new SortedList();
            public SortedList ByTabOrder = new SortedList();
        }
    }
}
