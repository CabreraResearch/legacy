using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionPropertySet : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionPropertySet( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "propertysetid",
                                                          "name",
                                                          _CswNbtMetaDataResources.PropertySetTableSelect,
                                                          _CswNbtMetaDataResources.PropertySetTableUpdate,
                                                          makePropertySet,
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

        public CswNbtMetaDataPropertySet makePropertySet( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataPropertySet( Resources, Row );
        }

        public Dictionary<Int32, CswEnumNbtPropertySetName> getObjectClassIds()
        {
            Dictionary<Int32, string> PSDict = _CollImpl.getPkDict();
            return PSDict.Keys
                         .Where( key => PSDict[key] != CswNbtResources.UnknownEnum )
                         .ToDictionary( key => key, key => (CswEnumNbtPropertySetName) PSDict[key] );
        }

        public Int32 getPropertySetId( CswEnumNbtPropertySetName Name )
        {
            return _CollImpl.getPksFirst( "where name = '" + Name.ToString() + "'" );
        }

        public IEnumerable<CswNbtMetaDataPropertySet> getPropertySets()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataPropertySet>();
        }

        public CswNbtMetaDataPropertySet getPropertySet( CswEnumNbtPropertySetName PropertySet )
        {
            return (CswNbtMetaDataPropertySet) _CollImpl.getWhereFirst( "where name = '" + PropertySet.ToString() + "'" );
        }

        public CswNbtMetaDataPropertySet getPropertySet( Int32 PropertySetId )
        {
            return (CswNbtMetaDataPropertySet) _CollImpl.getWhereFirst( "where propertysetid = " + PropertySetId.ToString() );
        }

        public CswNbtMetaDataPropertySet getPropertySetForObjectClass( Int32 ObjectClassId )
        {
            return (CswNbtMetaDataPropertySet) _CollImpl.getWhereFirst( "where propertysetid in (select propertysetid from jct_propertyset_objectclass where objectclassid = " + ObjectClassId + ")" );
        }

        private string _makeModuleWhereClause()
        {
            return string.Empty;
        }

    } // class CswNbtMetaDataCollectionPropertySet
} // namespace ChemSW.Nbt.MetaData
