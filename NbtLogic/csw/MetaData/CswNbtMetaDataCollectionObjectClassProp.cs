using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionObjectClassProp : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllObjectClassProps;
        private Hashtable _ById;
        private Hashtable _ByObjectClass;

        public CswNbtMetaDataCollectionObjectClassProp( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            _AllObjectClassProps = new Collection<ICswNbtMetaDataObject>();
            _ById = new Hashtable();
            _ByObjectClass = new Hashtable();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllObjectClassProps; } }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            CswNbtMetaDataObjectClassProp ret = null;
            if(_ById.ContainsKey(ObjectClassPropId))
                ret = _ById[ObjectClassPropId] as CswNbtMetaDataObjectClassProp;
            return ret;
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, string ObjectClassPropName )
        {
            CswNbtMetaDataObjectClassProp ret = null;
            if( _ByObjectClass.ContainsKey( ObjectClassId ) )
            {
                ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassId] as ObjectClassHashEntry;
                ret = Entry.ByName[ObjectClassPropName.ToLower()] as CswNbtMetaDataObjectClassProp;
            }
            return ret;
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, Int32 ObjectClassPropId)
        {
            CswNbtMetaDataObjectClassProp ret = null;
            if( _ByObjectClass.ContainsKey( ObjectClassId ) )
            {
                ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassId] as ObjectClassHashEntry;
                ret = Entry.ById[ObjectClassPropId] as CswNbtMetaDataObjectClassProp;
            }
            return ret;
        }

        public ICollection getObjectClassPropIds()
        {
            return _ById.Keys;
        }

        public ICollection getObjectClassProps()
        {
            return _ById.Values;
        }

        public ICollection getObjectClassPropIdsByObjectClass( Int32 ObjectClassId )
        {
            ICollection ret;
            if( _ByObjectClass.ContainsKey( ObjectClassId ) )
            {
                ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassId] as ObjectClassHashEntry;
                ret = Entry.ById.Keys;
            }
            else
            {
                ret = new ArrayList();
            }
            return ret;
        }

        public ICollection getObjectClassPropsByObjectClass( Int32 ObjectClassId )
        {
            ICollection ret;
            if( _ByObjectClass.ContainsKey( ObjectClassId ) )
            {
                ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassId] as ObjectClassHashEntry;
                ret = Entry.ByName.Values;
            }
            else
            {
                ret = new ArrayList();
            }
            return ret;
        }

        public void ClearKeys()
        {
            _ById.Clear();
            _ByObjectClass.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataObjectClassProp ObjectClassProp = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a node type prop in the collection
                ObjectClassProp = getObjectClassProp( PkToOverride );
                Deregister( ObjectClassProp );

                CswNbtMetaDataObjectClassProp OldObjectClassProp = new CswNbtMetaDataObjectClassProp( _CswNbtMetaDataResources, ObjectClassProp._DataRow );
                _AllObjectClassProps.Add( OldObjectClassProp );

                ObjectClassProp.Reassign( Row );
                
                RegisterExisting( OldObjectClassProp );
                RegisterExisting( ObjectClassProp );
            }
            else
            {
                ObjectClassProp = new CswNbtMetaDataObjectClassProp( _CswNbtMetaDataResources, Row );
                _AllObjectClassProps.Add( ObjectClassProp );

                RegisterExisting( ObjectClassProp );
            }
            return ObjectClassProp;
        }

        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClassProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

            _ById.Add( ObjectClassProp.PropId, ObjectClassProp );
            if( !_ByObjectClass.ContainsKey( ObjectClassProp.ObjectClass.ObjectClassId ) )
            {
                _ByObjectClass.Add( ObjectClassProp.ObjectClass.ObjectClassId, new ObjectClassHashEntry() );
            }
            ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassProp.ObjectClass.ObjectClassId] as ObjectClassHashEntry;
            Entry.ById.Add( ObjectClassProp.PropId, ObjectClassProp );
            Entry.ByName.Add( ObjectClassProp.PropName.ToLower(), ObjectClassProp );
        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClassProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

            _ById.Remove( ObjectClassProp.PropId );
            if( _ByObjectClass.ContainsKey( ObjectClassProp.ObjectClass.ObjectClassId ) )
            {
                ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassProp.ObjectClass.ObjectClassId] as ObjectClassHashEntry;
                Entry.ById.Remove( ObjectClassProp.PropId );
                Entry.ByName.Remove( ObjectClassProp.PropName.ToLower() );
            }
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClassProp ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

            _AllObjectClassProps.Remove( ObjectClassProp );
        }

        private class ObjectClassHashEntry
        {
            public Hashtable ById = new Hashtable();
            public Hashtable ByName = new Hashtable();
        }
    }
}
