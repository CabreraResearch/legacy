using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeTab : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionNodeTypeTab( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "nodetypetabsetid",
                                                          "tabname",
                                                          _CswNbtMetaDataResources.NodeTypeTabTableSelect,
                                                          _CswNbtMetaDataResources.NodeTypeTabTableUpdate,
                                                          makeNodeTypeTab,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataNodeTypeTab NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataNodeTypeTab makeNodeTypeTab( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataNodeTypeTab( Resources, Row );
        }

        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeTabId )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getByPk( NodeTypeTabId );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getByPk( NodeTypeTabId );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTab( Int32 NodeTypeId, string NodeTypeTabName )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and lower(tabname) = '" + CswTools.SafeSqlParam( NodeTypeTabName.ToLower() ) + "'" );
        }
        public CswNbtMetaDataNodeTypeTab getNodeTypeTabVersion( Int32 NodeTypeId, Int32 NodeTypeTabId )
        {
            return (CswNbtMetaDataNodeTypeTab) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and firsttabversionid = (select firsttabversionid from nodetype_tabset where nodetypetabsetid = " + NodeTypeTabId.ToString() + ")" );
        }
        public Collection<Int32> getNodeTypeTabIds( Int32 NodeTypeId )
        {
            return _CollImpl.getPks( "where nodetypeid = " + NodeTypeId.ToString() );
        }
        public IEnumerable<CswNbtMetaDataNodeTypeTab> getNodeTypeTabs( Int32 NodeTypeId )
        {
            return _CollImpl.getWhere( "where nodetypeid = " + NodeTypeId.ToString() ).Cast<CswNbtMetaDataNodeTypeTab>();
        }

        private string _makeModuleWhereClause()
        {
//            return @" ( ( exists (select j.jctmoduleobjectclassid
//                                    from jct_modules_objectclass j
//                                    join modules m on j.moduleid = m.moduleid
//                                   where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)
//                                     and m.enabled = '1')
//                          or not exists (select j.jctmoduleobjectclassid
//                                           from jct_modules_objectclass j
//                                           join modules m on j.moduleid = m.moduleid
//                                          where j.objectclassid = (select t.objectclassid from nodetypes t where t.nodetypeid = nodetype_tabset.nodetypeid)) )
//                    and ( exists (select j.jctmodulenodetypeid
//                                    from jct_modules_nodetypes j
//                                    join modules m on j.moduleid = m.moduleid
//                                   where j.nodetypeid = nodetype_tabset.nodetypeid
//                                     and m.enabled = '1')
//                          or not exists (select j.jctmodulenodetypeid
//                                           from jct_modules_nodetypes j
//                                           join modules m on j.moduleid = m.moduleid
//                                          where j.nodetypeid = nodetype_tabset.nodetypeid) ) )";
            return " nodetype_tabset.nodetypeid in (select nodetypeid from nodetypes where enabled = '1') ";
        }

        //public void ClearKeys()
        //{
        //    _ById.Clear();
        //    _ByNodeType.Clear();
        //}

        //public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        //{
        //    return RegisterNew( Row, Int32.MinValue );
        //}
        //public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        //{
        //    CswNbtMetaDataNodeTypeTab NodeTypeTab = null;
        //    if( PkToOverride != Int32.MinValue )
        //    {
        //        // This allows existing objects to always point to the latest version of a node type prop in the collection
        //        NodeTypeTab = getNodeTypeTab( PkToOverride );
        //        Deregister( NodeTypeTab );

        //        CswNbtMetaDataNodeTypeTab OldNodeTypeTab = new CswNbtMetaDataNodeTypeTab( _CswNbtMetaDataResources, NodeTypeTab._DataRow );
        //        _AllNodeTypeTabs.Add( OldNodeTypeTab );

        //        NodeTypeTab.Reassign( Row );

        //        RegisterExisting( OldNodeTypeTab );
        //        RegisterExisting( NodeTypeTab );
        //    }
        //    else
        //    {
        //        NodeTypeTab = new CswNbtMetaDataNodeTypeTab( _CswNbtMetaDataResources, Row );
        //        _AllNodeTypeTabs.Add( NodeTypeTab );

        //        RegisterExisting( NodeTypeTab );
        //    }
        //    return NodeTypeTab;
        //}

        //public void RegisterExisting( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeTab ) )
        //    {
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
        //    }
        //    CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

        //    _CswNbtMetaDataResources.tryAddToMetaDataCollection( NodeTypeTab.TabId, NodeTypeTab, _ById, "NodeTypeTab", NodeTypeTab.TabId, NodeTypeTab.TabName );

        //    if( !_ByNodeType.ContainsKey( NodeTypeTab.NodeType.NodeTypeId ) )
        //    {
        //        _ByNodeType.Add( NodeTypeTab.NodeType.NodeTypeId, new NodeTypeHashEntry() );
        //    }
        //    NodeTypeHashEntry Entry = _ByNodeType[NodeTypeTab.NodeType.NodeTypeId] as NodeTypeHashEntry;
        //    Entry.ByTabId.Add( NodeTypeTab.TabId, NodeTypeTab );
        //    Entry.ByTabName.Add( NodeTypeTab.TabName.ToLower(), NodeTypeTab );
        //    Entry.ByTabOrder.Add( CswTools.PadInt( NodeTypeTab.TabOrder, 3 ) + "_" + NodeTypeTab.TabId, NodeTypeTab );
        //}

        //public void Deregister( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeTab ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

        //    _ById.Remove( NodeTypeTab.TabId );
        //    if( _ByNodeType.ContainsKey( NodeTypeTab.NodeType.NodeTypeId ) )
        //    {
        //        NodeTypeHashEntry Entry = _ByNodeType[NodeTypeTab.NodeType.NodeTypeId] as NodeTypeHashEntry;
        //        Entry.ByTabId.Remove( NodeTypeTab.TabId );
        //        Entry.ByTabName.Remove( NodeTypeTab.TabName.ToLower() );
        //        Entry.ByTabOrder.Remove( CswTools.PadInt( NodeTypeTab.TabOrder, 3 ) + "_" + NodeTypeTab.TabId );
        //    }
        //}

        //public void Remove( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeTab ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeTab.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeTypeTab NodeTypeTab = Object as CswNbtMetaDataNodeTypeTab;

        //    _AllNodeTypeTabs.Remove( NodeTypeTab );
        //}

        //private class NodeTypeHashEntry
        //{
        //    public Hashtable ByTabId = new Hashtable();
        //    public SortedList ByTabName = new SortedList();
        //    public SortedList ByTabOrder = new SortedList();
        //}

    } // class CswNbtMetaDataCollectionNodeTypeTab
} // namespace ChemSW.Nbt.MetaData