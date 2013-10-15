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

        public CswNbtMetaDataObjectClassProp makeObjectClassProp( CswNbtMetaDataResources Resources, DataRow Row, CswDateTime Date )
        {
            return new CswNbtMetaDataObjectClassProp( Resources, Row, Date );
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId )
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getByPk( ObjectClassPropId );
        }

        public string getObjectClassPropName( Int32 ObjectClassPropId, CswDateTime Date = null )
        {
            string ret = string.Empty;
            // This fetches all of them at once.  This was done on purpose.
            // This will actually perform better in any case where you need more than one.
            Dictionary<int, string> dict = getObjectClassPropNames( Date );
            if( dict.ContainsKey( ObjectClassPropId ) )
            {
                ret = dict[ObjectClassPropId];
            }
            return ret;
        }

        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassId, string ObjectClassPropName, CswDateTime Date = null )
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getWhereFirst( "where objectclassid = " + ObjectClassId.ToString() + " and lower(propname) = '" + CswTools.SafeSqlParam( ObjectClassPropName.ToLower() ) + "'", Date );
        }
        public CswNbtMetaDataObjectClassProp getObjectClassProp( Int32 ObjectClassPropId, CswDateTime Date = null )
        {
            return (CswNbtMetaDataObjectClassProp) _CollImpl.getByPk( ObjectClassPropId, Date );
        }

        public Collection<Int32> getObjectClassPropIds()
        {
            return _CollImpl.getPks();
        }

        public Dictionary<Int32, string> getObjectClassPropNames( CswDateTime Date = null )
        {
            return _CollImpl.getPkDict( Date: Date );
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

        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassPropsByFieldType( CswEnumNbtFieldType FieldType )
        {
            return _CollImpl.getWhere( "where fieldtypeid in (select fieldtypeid from field_types where fieldtype = '" + FieldType.ToString() + "')" ).Cast<CswNbtMetaDataObjectClassProp>();
        }

        public IEnumerable<CswNbtMetaDataObjectClassProp> getObjectClassPropsByPropertySetId( Int32 PropertySetId )
        {
            return _CollImpl.getWhere( "where objectclasspropid in (select objectclasspropid from jct_propertyset_ocprop where propertysetid = " + PropertySetId + ")" ).Cast<CswNbtMetaDataObjectClassProp>();
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
    } // class CswNbtMetaDataCollectionObjectClassProp
} // namespace ChemSW.Nbt.MetaData