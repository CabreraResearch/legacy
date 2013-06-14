using System;
using System.Collections.Generic;
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
                                                          makeObjectClass,
                                                          _makeModuleWhereClause );
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

        public Dictionary<Int32, CswEnumNbtObjectClass> getObjectClassIds()
        {
            Dictionary<Int32, string> OCDict = _CollImpl.getPkDict();
            return OCDict.Keys
                         .Where( key => OCDict[key] != CswNbtResources.UnknownEnum )
                         .ToDictionary( key => key, key => (CswEnumNbtObjectClass) OCDict[key] );
        }

        public Int32 getObjectClassId( CswEnumNbtObjectClass ObjectClass )
        {
            return _CollImpl.getPksFirst( "where objectclass = '" + ObjectClass.ToString() + "'" );
        }

        public IEnumerable<CswNbtMetaDataObjectClass> getObjectClasses()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataObjectClass>();
        }

        public IEnumerable<CswNbtMetaDataObjectClass> getObjectClassesByPropertySetId( Int32 PropertySetId )
        {
            return _CollImpl.getWhere( "where objectclassid in (select objectclassid from jct_propertyset_objectclass where propertysetid = " + PropertySetId.ToString() + ")" )
                            .Cast<CswNbtMetaDataObjectClass>();
        }

        public CswNbtMetaDataObjectClass getObjectClass( CswEnumNbtObjectClass ObjectClass )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getWhereFirst( "where objectclass = '" + ObjectClass.ToString() + "'" );
        }
        public CswNbtMetaDataObjectClass getObjectClass( string ObjectClass )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getWhereFirst( "where objectclass = '" + ObjectClass + "'" );
        }
        public CswNbtMetaDataObjectClass getObjectClass( Int32 ObjectClassId )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getByPk( ObjectClassId );
        }
        public CswNbtMetaDataObjectClass getObjectClassByNodeTypeId( Int32 NodeTypeId )
        {
            return (CswNbtMetaDataObjectClass) _CollImpl.getWhereFirst( "where objectclassid in (select objectclassid from nodetypes where nodetypeid = " + NodeTypeId.ToString() + ")" );
        }

        private string _makeModuleWhereClause()
        {
            return @" (exists (select j.jctmoduleobjectclassid
                                 from jct_modules_objectclass j
                                 join modules m on j.moduleid = m.moduleid
                                where j.objectclassid = object_class.objectclassid
                                  and m.enabled = '1')
                       or not exists (select j.jctmoduleobjectclassid
                                        from jct_modules_objectclass j
                                        join modules m on j.moduleid = m.moduleid
                                       where j.objectclassid = object_class.objectclassid))";
        }
    } // class CswNbtMetaDataCollectionObjectClass
} // namespace ChemSW.Nbt.MetaData
