using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionObjectClassProp : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionObjectClassProp( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "objectclasspropid",
                                                          "propname",
                                                          _CswNbtMetaDataResources.ObjectClassPropTableSelect,
                                                          _CswNbtMetaDataResources.ObjectClassPropTableUpdate,
                                                          makeObjectClassProp,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataObjectClassProp NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataObjectClassProp makeObjectClassProp( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataObjectClassProp( Resources, Row );
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getByPk( ObjectClassPropId );
        }
        public string getObjectClassPropName( Int32 ObjectClassPropId )
        {
            // This fetches all of them at once.  This was done on purpose.
            // This will actually perform better in any case where you need more than one.
            return getObjectClassPropNames()[ObjectClassPropId];
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, string ObjectClassPropName )
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getWhereFirst( "where objectclassid = " + ObjectClassId.ToString() + " and lower(propname) = '" + CswTools.SafeSqlParam(ObjectClassPropName.ToLower()) + "'" );
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, Int32 ObjectClassPropId)
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getByPk( ObjectClassPropId );
        }

        public Collection<Int32> getObjectClassPropIds()
        {
            return _CollImpl.getPks();
        }

        public Dictionary<Int32, string> getObjectClassPropNames()
        {
            return _CollImpl.getPkDict();
        }

        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassProps()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataObjectClassProp>();
        }

        public Collection<Int32> getObjectClassPropIdsByObjectClass( Int32 ObjectClassId )
        {
            return _CollImpl.getPks( "where objectclassid = " + ObjectClassId.ToString() );
        }
        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassPropsByObjectClass( Int32 ObjectClassId )
        {
            return _CollImpl.getWhere( "where objectclassid = " + ObjectClassId.ToString() ).Cast<CswNbtMetaDataObjectClassProp>();
        }

        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassPropsByFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CollImpl.getWhere( "where fieldtypeid in (select fieldtypeid from field_types where fieldtype = '" + FieldType.ToString() + "')" ).Cast<CswNbtMetaDataObjectClassProp>();
        }

        private string _makeModuleWhereClause()
        {
            return @" (exists (select j.jctmoduleobjectclassid
                                 from jct_modules_objectclass j
                                 join modules m on j.moduleid = m.moduleid
                                where j.objectclassid = object_class_props.objectclassid
                                  and m.enabled = '1')
                       or not exists (select j.jctmoduleobjectclassid
                                        from jct_modules_objectclass j
                                        join modules m on j.moduleid = m.moduleid
                                       where j.objectclassid = object_class_props.objectclassid))";
        }

        //public void ClearKeys()
        //{
        //    _ById.Clear();
        //    _ByObjectClass.Clear();
        //}

        //public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        //{
        //    return RegisterNew( Row, Int32.MinValue );
        //}
        //public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        //{
        //    CswNbtMetaDataObjectClassProp ObjectClassProp = null;
        //    if( PkToOverride != Int32.MinValue )
        //    {
        //        // This allows existing objects to always point to the latest version of a node type prop in the collection
        //        ObjectClassProp = getObjectClassProp( PkToOverride );
        //        Deregister( ObjectClassProp );

        //        CswNbtMetaDataObjectClassProp OldObjectClassProp = new CswNbtMetaDataObjectClassProp( _CswNbtMetaDataResources, ObjectClassProp._DataRow );
        //        _AllObjectClassProps.Add( OldObjectClassProp );

        //        ObjectClassProp.Reassign( Row );
                
        //        RegisterExisting( OldObjectClassProp );
        //        RegisterExisting( ObjectClassProp );
        //    }
        //    else
        //    {
        //        ObjectClassProp = new CswNbtMetaDataObjectClassProp( _CswNbtMetaDataResources, Row );
        //        _AllObjectClassProps.Add( ObjectClassProp );

        //        RegisterExisting( ObjectClassProp );
        //    }
        //    return ObjectClassProp;
        //}

        //public void RegisterExisting( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClassProp ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

        //    _ById.Add( ObjectClassProp.PropId, ObjectClassProp );
        //    if( !_ByObjectClass.ContainsKey( ObjectClassProp.ObjectClass.ObjectClassId ) )
        //    {
        //        _ByObjectClass.Add( ObjectClassProp.ObjectClass.ObjectClassId, new ObjectClassHashEntry() );
        //    }
        //    ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassProp.ObjectClass.ObjectClassId] as ObjectClassHashEntry;
        //    Entry.ById.Add( ObjectClassProp.PropId, ObjectClassProp );
        //    Entry.ByName.Add( ObjectClassProp.PropName.ToLower(), ObjectClassProp );
        //}

        //public void Deregister( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClassProp ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

        //    _ById.Remove( ObjectClassProp.PropId );
        //    if( _ByObjectClass.ContainsKey( ObjectClassProp.ObjectClass.ObjectClassId ) )
        //    {
        //        ObjectClassHashEntry Entry = _ByObjectClass[ObjectClassProp.ObjectClass.ObjectClassId] as ObjectClassHashEntry;
        //        Entry.ById.Remove( ObjectClassProp.PropId );
        //        Entry.ByName.Remove( ObjectClassProp.PropName.ToLower() );
        //    }
        //}

        //public void Remove( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataObjectClassProp ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionObjectClassProp.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataObjectClassProp ObjectClassProp = Object as CswNbtMetaDataObjectClassProp;

        //    _AllObjectClassProps.Remove( ObjectClassProp );
        //}

        //private class ObjectClassHashEntry
        //{
        //    public Hashtable ById = new Hashtable();
        //    public Hashtable ByName = new Hashtable();
        //}
    } // class CswNbtMetaDataCollectionObjectClassProp
} // namespace ChemSW.Nbt.MetaData