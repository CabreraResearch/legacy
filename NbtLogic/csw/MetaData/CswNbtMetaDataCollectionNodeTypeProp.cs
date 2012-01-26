using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeProp : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionNodeTypeProp( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "nodetypepropid",
                                                          _CswNbtMetaDataResources.NodeTypePropTableUpdate,
                                                          makeNodeTypeProp );
        }

        public CswNbtMetaDataNodeTypeProp makeNodeTypeProp( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataNodeTypeProp( Resources, Row );
        }

        public Collection<ICswNbtMetaDataObject> All
        {
            get
            {
                return _CollImpl.getAll();
            }
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getByPk( NodeTypePropId );
        }

        public Collection<Int32> getNodeTypePropIds()
        {
            return _CollImpl.getPks();
        }
        public Collection<Int32> getNodeTypePropIds( Int32 NodeTypeId )
        {
            return _CollImpl.getPks( "where nodetypeid = " + NodeTypeId.ToString() );
        }
        public Collection<Int32> getNodeTypePropIdsByTab( Int32 TabId )
        {
            return _CollImpl.getPks( "where nodetypetabsetid = " + TabId.ToString() );
        }
        
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CollImpl.getWhere( "where objectclasspropid = " + ObjectClassPropId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( Int32 NodeTypeId )
        {
            return _CollImpl.getWhere( "where nodetypeid = " + NodeTypeId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByTab( Int32 TabId )
        {
            return _CollImpl.getWhere( "where nodetypetabsetid = " + TabId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByDisplayOrder( Int32 NodeTypeId, Int32 TabId )
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getPropsInLayout( NodeTypeId, TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
        }
        //public Int32 getNodeTypePropDisplayOrder( Int32 TabId, CswNbtMetaDataNodeTypeProp Prop )
        //{
        //    Int32 ret = Int32.MinValue;
        //    if( _ByNodeTypeTab.ContainsKey( TabId ) )
        //        ret = ( (NodeTypeTabHashEntry) _ByNodeTypeTab[TabId] ).ByDisplayOrder.IndexOfKey( Prop );
        //    return ret;
        //}
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getByPk( NodeTypePropId );
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, string NodeTypePropName )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and lower(propname) = '" + NodeTypePropName.ToLower() + "'" );
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassPropName( Int32 NodeTypeId, string ObjectClassPropName )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and objectclasspropid in (select objectclasspropid from object_class_prop where lower(propname) = '" + ObjectClassPropName.ToLower() + "')" );
        }

        //public void ClearKeys()
        //{
        //    _ById.Clear();
        //    _ByNodeType.Clear();
        //    _ByNodeTypeTab.Clear();
        //    _ByObjectClassProp.Clear();
        //}

        //public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        //{
        //    return RegisterNew( Row, Int32.MinValue );
        //}
        //public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        //{
        //    CswNbtMetaDataNodeTypeProp NodeTypeProp = null;
        //    if( PkToOverride != Int32.MinValue )
        //    {
        //        // This allows existing objects to always point to the latest version of a node type prop in the collection
        //        NodeTypeProp = getNodeTypeProp( PkToOverride );
        //        Deregister( NodeTypeProp );

        //        CswNbtMetaDataNodeTypeProp OldNodeTypeProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, NodeTypeProp._DataRow );
        //        _AllNodeTypeProps.Add( OldNodeTypeProp );

        //        OldNodeTypeProp.clearCachedLayouts();
        //        NodeTypeProp.clearCachedLayouts();
        //        NodeTypeProp.Reassign( Row );

        //        RegisterExisting( OldNodeTypeProp );
        //        RegisterExisting( NodeTypeProp );
        //    }
        //    else
        //    {
        //        NodeTypeProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, Row );
        //        _AllNodeTypeProps.Add( NodeTypeProp );

        //        RegisterExisting( NodeTypeProp );
        //    }
        //    return NodeTypeProp;
        //}

        //public void RegisterExisting( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeProp ) )
        //    {
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
        //    }
        //    CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

        //    _CswNbtMetaDataResources.tryAddToMetaDataCollection( NodeTypeProp.PropId, NodeTypeProp, _ById, "NodeTypeProp", NodeTypeProp.PropId, NodeTypeProp.PropName );

        //    // By NodeType
        //    if( !_ByNodeType.ContainsKey( NodeTypeProp.NodeType.NodeTypeId ) )
        //    {
        //        _ByNodeType.Add( NodeTypeProp.NodeType.NodeTypeId, new NodeTypeHashEntry() );
        //    }
        //    NodeTypeHashEntry Entry = _ByNodeType[NodeTypeProp.NodeType.NodeTypeId] as NodeTypeHashEntry;
        //    Entry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
        //    Entry.ByPropName.Add( NodeTypeProp.PropName.ToLower(), NodeTypeProp );
        //    if( NodeTypeProp.EditLayout != null && NodeTypeProp.EditLayout.TabId != Int32.MinValue )
        //    {
        //        CswNbtMetaDataNodeTypeTab Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( NodeTypeProp.EditLayout.TabId );
        //        if( Tab != null )
        //        {
        //            Entry.ByQuestionNo.Add( Tab.SectionNo + "." + NodeTypeProp.QuestionNo + "_" + NodeTypeProp.PropId, NodeTypeProp );
        //            if( !_ByNodeTypeTab.ContainsKey( NodeTypeProp.EditLayout.TabId ) )
        //            {
        //                _ByNodeTypeTab.Add( NodeTypeProp.EditLayout.TabId, new NodeTypeTabHashEntry() );
        //            }

        //            // By Tab
        //            NodeTypeTabHashEntry TabEntry = _ByNodeTypeTab[NodeTypeProp.EditLayout.TabId] as NodeTypeTabHashEntry;
        //            TabEntry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
        //            TabEntry.ByPropName.Add( NodeTypeProp.PropName.ToLower(), NodeTypeProp );
        //            TabEntry.ByDisplayOrder.Add( NodeTypeProp, NodeTypeProp );
        //        }
        //    }
        //    if( NodeTypeProp.ObjectClassProp != null )
        //        Entry.ByObjectClassPropName.Add( NodeTypeProp.ObjectClassProp.PropName.ToLower(), NodeTypeProp );

        //    // By Object Class Prop
        //    if( NodeTypeProp.ObjectClassProp != null )
        //    {
        //        if( !_ByObjectClassProp.ContainsKey( NodeTypeProp.ObjectClassProp.PropId ) )
        //        {
        //            _ByObjectClassProp.Add( NodeTypeProp.ObjectClassProp.PropId, new ObjectClassPropHashEntry() );
        //        }
        //        ObjectClassPropHashEntry OCPEntry = _ByObjectClassProp[NodeTypeProp.ObjectClassProp.PropId] as ObjectClassPropHashEntry;
        //        OCPEntry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
        //    }
        //}

        //public void Deregister( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeProp ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

        //    _ById.Remove( NodeTypeProp.PropId );

        //    // By NodeType
        //    if( _ByNodeType.ContainsKey( NodeTypeProp.NodeType.NodeTypeId ) )
        //    {
        //        NodeTypeHashEntry Entry = _ByNodeType[NodeTypeProp.NodeType.NodeTypeId] as NodeTypeHashEntry;
        //        Entry.ByPropId.Remove( NodeTypeProp.PropId );
        //        Entry.ByPropName.Remove( NodeTypeProp.PropName.ToLower() );
        //        CswNbtMetaDataNodeTypeTab Tab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTab( NodeTypeProp.EditLayout.TabId );
        //        Entry.ByQuestionNo.Remove( Tab.SectionNo + "." + NodeTypeProp.QuestionNo + "_" + NodeTypeProp.PropId );
        //        if( NodeTypeProp.ObjectClassProp != null )
        //            Entry.ByObjectClassPropName.Remove( NodeTypeProp.ObjectClassProp.PropName.ToLower() );
        //    }
        //    // By Tab
        //    if( _ByNodeTypeTab.ContainsKey( NodeTypeProp.EditLayout.TabId ) )
        //    {
        //        NodeTypeTabHashEntry TabEntry = _ByNodeTypeTab[NodeTypeProp.EditLayout.TabId] as NodeTypeTabHashEntry;
        //        TabEntry.ByPropId.Remove( NodeTypeProp.PropId );
        //        TabEntry.ByPropName.Remove( NodeTypeProp.PropName.ToLower() );
        //        TabEntry.ByDisplayOrder.Remove( NodeTypeProp );
        //    }
        //    // By Object Class Prop
        //    if( NodeTypeProp.ObjectClassProp != null )
        //    {
        //        if( _ByObjectClassProp.ContainsKey( NodeTypeProp.ObjectClassProp.PropId ) )
        //        {
        //            ObjectClassPropHashEntry OCPEntry = _ByObjectClassProp[NodeTypeProp.ObjectClassProp.PropId] as ObjectClassPropHashEntry;
        //            OCPEntry.ByPropId.Remove( NodeTypeProp.PropId );
        //        }
        //    }
        //}

        //public void Remove( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeTypeProp ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

        //    _AllNodeTypeProps.Remove( NodeTypeProp );
        //}


        //private class NodeTypeHashEntry
        //{
        //    public SortedList ByObjectClassPropName = new SortedList();
        //    public Hashtable ByPropId = new Hashtable();
        //    public SortedList ByPropName = new SortedList();
        //    public SortedList ByQuestionNo = new SortedList();
        //}
        //private class NodeTypeTabHashEntry
        //{
        //    public Hashtable ByPropId = new Hashtable();
        //    public SortedList ByPropName = new SortedList();
        //    public SortedList ByDisplayOrder = new SortedList();
        //}
        //private class ObjectClassPropHashEntry
        //{
        //    public Hashtable ByPropId = new Hashtable();
        //}

    } // class CswNbtMetaDataCollectionNodeTypeProp
} // namespace ChemSW.Nbt.MetaData