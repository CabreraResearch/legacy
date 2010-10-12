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
    public class CswNbtMetaDataCollectionFieldType : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllFieldTypes;
        private Hashtable _ById;
        private SortedList _ByType;

        public CswNbtMetaDataCollectionFieldType( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            
            _AllFieldTypes = new Collection<ICswNbtMetaDataObject>();
            _ById = new Hashtable();
            _ByType = new SortedList();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllFieldTypes; } }

        public ICollection getFieldTypeIds() { return _ById.Keys; } 
        public ICollection getFieldTypes() { return _ByType.Values; }

        public CswNbtMetaDataFieldType getFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            CswNbtMetaDataFieldType ret = null;
            if( _ByType.Contains( FieldType ) )
                ret = _ByType[FieldType] as CswNbtMetaDataFieldType;
            return ret;
        }

        public CswNbtMetaDataFieldType getFieldType( Int32 FieldTypeId )
        {
            CswNbtMetaDataFieldType ret = null;
            if( _ById.Contains( FieldTypeId ) )
                ret = _ById[FieldTypeId] as CswNbtMetaDataFieldType;
            return ret;
        }


        public void ClearKeys()
        {
            _ById.Clear();
            _ByType.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataFieldType FieldType = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a field type in the collection
                FieldType = getFieldType( PkToOverride );
                Deregister( FieldType );

                CswNbtMetaDataFieldType OldFieldType = new CswNbtMetaDataFieldType( _CswNbtMetaDataResources, FieldType._DataRow );
                _AllFieldTypes.Add( OldFieldType );

                FieldType.Reassign( Row );
                
                RegisterExisting( OldFieldType );
                RegisterExisting( FieldType );
            }
            else
            {
                FieldType = new CswNbtMetaDataFieldType( _CswNbtMetaDataResources, Row );
                _AllFieldTypes.Add( FieldType );
                
                RegisterExisting( FieldType );
            }
            return FieldType;
        }

        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataFieldType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionFieldType.Register got an invalid Object as a parameter" );
            CswNbtMetaDataFieldType FieldType = Object as CswNbtMetaDataFieldType;

            _ByType.Add( FieldType.FieldType, FieldType );
            _ById.Add( FieldType.FieldTypeId, FieldType );
        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataFieldType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionFieldType.Register got an invalid Object as a parameter" );
            CswNbtMetaDataFieldType FieldType = Object as CswNbtMetaDataFieldType;

            _AllFieldTypes.Remove( FieldType );
            _ByType.Remove( FieldType.FieldType );
            _ById.Remove( FieldType.FieldTypeId );
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataFieldType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionFieldType.Register got an invalid Object as a parameter" );
            CswNbtMetaDataFieldType FieldType = Object as CswNbtMetaDataFieldType;

            _AllFieldTypes.Remove( FieldType );
        }
    }
}
