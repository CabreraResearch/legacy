using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeProp : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllNodeTypeProps;
        private Hashtable _ById;
        private Hashtable _ByNodeType;
        private Hashtable _ByNodeTypeTab;
        private Hashtable _ByObjectClassProp;

        public CswNbtMetaDataCollectionNodeTypeProp( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            _AllNodeTypeProps = new Collection<ICswNbtMetaDataObject>();
            _ById = new Hashtable();
            _ByNodeType = new Hashtable();
            _ByNodeTypeTab = new Hashtable();
            _ByObjectClassProp = new Hashtable();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllNodeTypeProps; } }

        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypePropId )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            if( _ById.Contains( NodeTypePropId ) )
                ret = _ById[NodeTypePropId] as CswNbtMetaDataNodeTypeProp;
            return ret;
        }
        public ICollection getNodeTypePropIds()
        {
            return _ById.Keys;
        }
        public ICollection getNodeTypePropIds( Int32 NodeTypeId )
        {
            ICollection ret;
            if( _ByNodeType.ContainsKey( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByPropId.Keys;
            else
                ret = new ArrayList();
            return ret;
        }
        public ICollection getNodeTypePropIdsByTab( Int32 TabId )
        {
            ICollection ret;
            if( _ByNodeTypeTab.ContainsKey( TabId ) )
                ret = ( (NodeTypeTabHashEntry) _ByNodeTypeTab[TabId] ).ByPropId.Keys;
            else
                ret = new ArrayList();
            return ret;
        }
        public ICollection getNodeTypePropsByObjectClassProp( Int32 ObjectClassPropId )
        {
            ICollection ret;
            if( _ByObjectClassProp.ContainsKey( ObjectClassPropId ) )
                ret = ( (ObjectClassPropHashEntry) _ByObjectClassProp[ObjectClassPropId] ).ByPropId.Values;
            else
                ret = new ArrayList();
            return ret;
        }
        public ICollection getNodeTypeProps( Int32 NodeTypeId )
        {
            ICollection ret;
            if( _ByNodeType.ContainsKey( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByPropName.Values;
            else
                ret = new ArrayList();
            return ret;
        }
		public ICollection getNodeTypePropsByTab( Int32 TabId )
		{
			ICollection ret;
			if( _ByNodeTypeTab.ContainsKey( TabId ) )
				ret = ( (NodeTypeTabHashEntry) _ByNodeTypeTab[TabId] ).ByPropName.Values;
			else
				ret = new ArrayList();
			return ret;
		}
        public ICollection getNodeTypePropsByDisplayOrder( Int32 TabId )
        {
            ICollection ret;
            if( _ByNodeTypeTab.ContainsKey( TabId ) )
                ret = ( (NodeTypeTabHashEntry) _ByNodeTypeTab[TabId] ).ByDisplayOrder.Values;
            else
                ret = new ArrayList();
            return ret;
        }
        public Int32 getNodeTypePropDisplayOrder( Int32 TabId, CswNbtMetaDataNodeTypeProp Prop )
        {
            Int32 ret = Int32.MinValue;
            if( _ByNodeTypeTab.ContainsKey( TabId ) )
                ret = ( (NodeTypeTabHashEntry) _ByNodeTypeTab[TabId] ).ByDisplayOrder.IndexOfKey( Prop );
            return ret;
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, Int32 NodeTypePropId )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            if( _ByNodeType.ContainsKey( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByPropId[NodeTypePropId] as CswNbtMetaDataNodeTypeProp;
            return ret;
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, string NodeTypePropName )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            if( _ByNodeType.ContainsKey( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByPropName[NodeTypePropName.ToLower()] as CswNbtMetaDataNodeTypeProp;
            return ret;
        }
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassPropName( Int32 NodeTypeId, string ObjectClassPropName )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            if( _ByNodeType.ContainsKey( NodeTypeId ) )
                ret = ( (NodeTypeHashEntry) _ByNodeType[NodeTypeId] ).ByObjectClassPropName[ObjectClassPropName.ToLower()] as CswNbtMetaDataNodeTypeProp;
            return ret;
        }

        public void ClearKeys()
        {
            _ById.Clear();
            _ByNodeType.Clear();
            _ByNodeTypeTab.Clear();
            _ByObjectClassProp.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a node type prop in the collection
                NodeTypeProp = getNodeTypeProp( PkToOverride );
                Deregister( NodeTypeProp );

                CswNbtMetaDataNodeTypeProp OldNodeTypeProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, NodeTypeProp._DataRow );
                _AllNodeTypeProps.Add( OldNodeTypeProp );

                NodeTypeProp.Reassign( Row );

                RegisterExisting( OldNodeTypeProp );
                RegisterExisting( NodeTypeProp );
            }
            else
            {
                NodeTypeProp = new CswNbtMetaDataNodeTypeProp( _CswNbtMetaDataResources, Row );
                _AllNodeTypeProps.Add( NodeTypeProp );
                
                RegisterExisting( NodeTypeProp );
            }
            return NodeTypeProp;
        }

        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

            _ById.Add( NodeTypeProp.PropId, NodeTypeProp );

            // By NodeType
            if( !_ByNodeType.ContainsKey( NodeTypeProp.NodeType.NodeTypeId) )
            {
                _ByNodeType.Add( NodeTypeProp.NodeType.NodeTypeId, new NodeTypeHashEntry() );
            }
            NodeTypeHashEntry Entry = _ByNodeType[NodeTypeProp.NodeType.NodeTypeId] as NodeTypeHashEntry;
            Entry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
            Entry.ByPropName.Add( NodeTypeProp.PropName.ToLower(), NodeTypeProp );
			if( NodeTypeProp.EditLayout != null && NodeTypeProp.EditLayout.Tab != null )
			{
				Entry.ByQuestionNo.Add( NodeTypeProp.EditLayout.Tab.SectionNo + "." + NodeTypeProp.QuestionNo + "_" + NodeTypeProp.PropId, NodeTypeProp );
				if( !_ByNodeTypeTab.ContainsKey( NodeTypeProp.EditLayout.Tab.TabId ) )
				{
					_ByNodeTypeTab.Add( NodeTypeProp.EditLayout.Tab.TabId, new NodeTypeTabHashEntry() );
				}

				// By Tab
				NodeTypeTabHashEntry TabEntry = _ByNodeTypeTab[NodeTypeProp.EditLayout.Tab.TabId] as NodeTypeTabHashEntry;
				TabEntry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
				TabEntry.ByPropName.Add( NodeTypeProp.PropName.ToLower(), NodeTypeProp );
				TabEntry.ByDisplayOrder.Add( NodeTypeProp, NodeTypeProp );
			}
			if( NodeTypeProp.ObjectClassProp != null )
                Entry.ByObjectClassPropName.Add( NodeTypeProp.ObjectClassProp.PropName.ToLower(), NodeTypeProp );

            // By Object Class Prop
            if( NodeTypeProp.ObjectClassProp != null )
            {
                if( !_ByObjectClassProp.ContainsKey( NodeTypeProp.ObjectClassProp.PropId ) )
                {
                    _ByObjectClassProp.Add( NodeTypeProp.ObjectClassProp.PropId, new ObjectClassPropHashEntry() );
                }
                ObjectClassPropHashEntry OCPEntry = _ByObjectClassProp[NodeTypeProp.ObjectClassProp.PropId] as ObjectClassPropHashEntry;
                OCPEntry.ByPropId.Add( NodeTypeProp.PropId, NodeTypeProp );
            }
        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

            _ById.Remove( NodeTypeProp.PropId );

            // By NodeType
            if( _ByNodeType.ContainsKey( NodeTypeProp.NodeType.NodeTypeId ) )
            {
                NodeTypeHashEntry Entry = _ByNodeType[NodeTypeProp.NodeType.NodeTypeId] as NodeTypeHashEntry;
                Entry.ByPropId.Remove( NodeTypeProp.PropId );
                Entry.ByPropName.Remove( NodeTypeProp.PropName.ToLower() );
				Entry.ByQuestionNo.Remove( NodeTypeProp.EditLayout.Tab.SectionNo + "." + NodeTypeProp.QuestionNo + "_" + NodeTypeProp.PropId );
                if( NodeTypeProp.ObjectClassProp != null )
                    Entry.ByObjectClassPropName.Remove( NodeTypeProp.ObjectClassProp.PropName.ToLower() );
            }
            // By Tab
			if( _ByNodeTypeTab.ContainsKey( NodeTypeProp.EditLayout.Tab.TabId ) )
            {
				NodeTypeTabHashEntry TabEntry = _ByNodeTypeTab[NodeTypeProp.EditLayout.Tab.TabId] as NodeTypeTabHashEntry;
                TabEntry.ByPropId.Remove( NodeTypeProp.PropId );
                TabEntry.ByPropName.Remove( NodeTypeProp.PropName.ToLower() );
                TabEntry.ByDisplayOrder.Remove( NodeTypeProp );
            }
            // By Object Class Prop
            if( NodeTypeProp.ObjectClassProp != null )
            {
                if( _ByObjectClassProp.ContainsKey( NodeTypeProp.ObjectClassProp.PropId ) )
                {
                    ObjectClassPropHashEntry OCPEntry = _ByObjectClassProp[NodeTypeProp.ObjectClassProp.PropId] as ObjectClassPropHashEntry;
                    OCPEntry.ByPropId.Remove( NodeTypeProp.PropId );
                }
            }
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeTypeProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeTypeProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeTypeProp NodeTypeProp = Object as CswNbtMetaDataNodeTypeProp;

            _AllNodeTypeProps.Remove( NodeTypeProp );
        }


        private class NodeTypeHashEntry
        {
            public SortedList ByObjectClassPropName = new SortedList();
            public Hashtable ByPropId = new Hashtable();
            public SortedList ByPropName = new SortedList();
            public SortedList ByQuestionNo = new SortedList();
        }
        private class NodeTypeTabHashEntry
        {
            public Hashtable ByPropId = new Hashtable();
            public SortedList ByPropName = new SortedList();
            public SortedList ByDisplayOrder = new SortedList();
        }
        private class ObjectClassPropHashEntry
        {
            public Hashtable ByPropId = new Hashtable();
        }
    
    }
}
