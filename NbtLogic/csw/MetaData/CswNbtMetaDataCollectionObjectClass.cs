using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionObjectClass : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllObjectClasses;
        private SortedList _ByName;
        private Hashtable _ById;

        public CswNbtMetaDataCollectionObjectClass( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            _AllObjectClasses = new Collection<ICswNbtMetaDataObject>();
            _ByName = new SortedList();
            _ById = new Hashtable();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllObjectClasses; } }
        public ICollection getObjectClassIds()
        {
            return _ById.Keys;
        }
        public ICollection getObjectClasses()
        {
            return _ByName.Values;
        }

        public CswNbtMetaDataObjectClass getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            CswNbtMetaDataObjectClass ret = null;
            if( _ByName.ContainsKey( ObjectClass ) )
                ret = _ByName[ObjectClass] as CswNbtMetaDataObjectClass;
            return ret;
        }
        public CswNbtMetaDataObjectClass getObjectClass( Int32 ObjectClassId )
        {
            CswNbtMetaDataObjectClass ret = null;
            if( _ById.ContainsKey( ObjectClassId ) )
                ret = _ById[ObjectClassId] as CswNbtMetaDataObjectClass;
            return ret;
        }

        public void ClearKeys()
        {
            _ByName.Clear();
            _ById.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataObjectClass ObjectClass = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a node type prop in the collection
                ObjectClass = getObjectClass( PkToOverride );
                Deregister( ObjectClass );

                CswNbtMetaDataObjectClass OldObjectClass = new CswNbtMetaDataObjectClass( _CswNbtMetaDataResources, ObjectClass._DataRow );
                _AllObjectClasses.Add( OldObjectClass );

                ObjectClass.Reassign( Row );
                
                RegisterExisting( OldObjectClass );
                RegisterExisting( ObjectClass );
            }
            else
            {
                ObjectClass = new CswNbtMetaDataObjectClass( _CswNbtMetaDataResources, Row );
                _AllObjectClasses.Add( ObjectClass );

                RegisterExisting( ObjectClass );
            }
            return ObjectClass;
        }

        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClass ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

            _ByName.Add( ObjectClass.ObjectClass, ObjectClass );
            _ById.Add( ObjectClass.ObjectClassId, ObjectClass );
        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClass ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

            _ByName.Remove( ObjectClass.ObjectClass );
            _ById.Remove( ObjectClass.ObjectClassId );
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataObjectClass ) )
                throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
            CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

            _AllObjectClasses.Remove( ObjectClass );
        }
    }
}
