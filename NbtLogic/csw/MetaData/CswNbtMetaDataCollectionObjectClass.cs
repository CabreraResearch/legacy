using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionObjectClass : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionObjectClass( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "objectclassid",
                                                          "objectclass",
                                                          _CswNbtMetaDataResources.ObjectClassTableSelect,
                                                          _CswNbtMetaDataResources.ObjectClassTableUpdate,
                                                          makeObjectClass );
        }

        public void AddToCache( CswNbtMetaDataObjectClass NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataObjectClass makeObjectClass( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataObjectClass( Resources, Row );
        }

        public Dictionary<CswNbtMetaDataObjectClass.NbtObjectClass,Int32> getObjectClassIds()
        {
            Dictionary<CswNbtMetaDataObjectClass.NbtObjectClass, Int32> ret = new Dictionary<CswNbtMetaDataObjectClass.NbtObjectClass, Int32>();
            Dictionary<string, Int32> OCDict = _CollImpl.getPkDict();
            CswNbtMetaDataObjectClass.NbtObjectClass OCKey = CswNbtMetaDataObjectClass.NbtObjectClass.Unknown;
            foreach( string Key in OCDict.Keys )
            {
                Enum.TryParse<CswNbtMetaDataObjectClass.NbtObjectClass>( Key, out OCKey );
                if( false == ret.ContainsKey( OCKey ) )
                {
                    ret.Add( OCKey, OCDict[Key] );
                }
            }
            return ret;
        }
        public IEnumerable<CswNbtMetaDataObjectClass> getObjectClasses()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataObjectClass>();
        }

        public CswNbtMetaDataObjectClass getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getWhereFirst( "where objectclass = '" + ObjectClass.ToString() + "'" );
        }
        public CswNbtMetaDataObjectClass getObjectClass( Int32 ObjectClassId )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getByPk( ObjectClassId );
        }
        public CswNbtMetaDataObjectClass getObjectClassByNodeTypeId( Int32 NodeTypeId )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getWhereFirst( "where objectclassid in (select objectclassid from nodetypes where nodetypeid = " + NodeTypeId.ToString() + ")" );
        }

        //public void ClearKeys()
        //{
        //    _ByName.Clear();
        //    _ById.Clear();
        //}

        //public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        //{
        //    return RegisterNew( Row, Int32.MinValue );
        //}
        //public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        //{
        //    CswNbtMetaDataObjectClass ObjectClass = null;
        //    if( PkToOverride != Int32.MinValue )
        //    {
        //        // This allows existing objects to always point to the latest version of a node type prop in the collection
        //        ObjectClass = getObjectClass( PkToOverride );
        //        Deregister( ObjectClass );

        //        CswNbtMetaDataObjectClass OldObjectClass = new CswNbtMetaDataObjectClass( _CswNbtMetaDataResources, ObjectClass._DataRow );
        //        _AllObjectClasses.Add( OldObjectClass );

        //        ObjectClass.Reassign( Row );
                
        //        RegisterExisting( OldObjectClass );
        //        RegisterExisting( ObjectClass );
        //    }
        //    else
        //    {
        //        ObjectClass = new CswNbtMetaDataObjectClass( _CswNbtMetaDataResources, Row );
        //        _AllObjectClasses.Add( ObjectClass );

        //        RegisterExisting( ObjectClass );
        //    }
        //    return ObjectClass;
        //}

        //public void RegisterExisting( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClass ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

        //    _ByName.Add( ObjectClass.ObjectClass, ObjectClass );
        //    _ById.Add( ObjectClass.ObjectClassId, ObjectClass );
        //}

        //public void Deregister( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClass ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

        //    _ByName.Remove( ObjectClass.ObjectClass );
        //    _ById.Remove( ObjectClass.ObjectClassId );
        //}

        //public void Remove( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClass ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClass.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClass ObjectClass = Object as CswNbtMetaDataObjectClass;

        //    _AllObjectClasses.Remove( ObjectClass );
        //}
    } // class CswNbtMetaDataCollectionObjectClass
} // namespace ChemSW.Nbt.MetaData
