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

        public Dictionary<Int32, CswEnumNbtPropertySet> getObjectClassIds()
        {
            Dictionary<Int32, string> PSDict = _CollImpl.getPkDict();
            return PSDict.Keys
                         .Where( key => PSDict[key] != CswNbtResources.UnknownEnum )
                         .ToDictionary( key => key, key => (CswEnumNbtPropertySet) PSDict[key] );
        }

        public Int32 getPropertySetId( CswEnumNbtPropertySet Name )
        {
            return _CollImpl.getPksFirst( "where name = '" + Name.ToString() + "'" );
        }

        public IEnumerable<CswNbtMetaDataPropertySet> getPropertySets()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataPropertySet>();
        }

        private string _makeModuleWhereClause()
        {
            return string.Empty;
        }

    } // class CswNbtMetaDataCollectionPropertySet
} // namespace ChemSW.Nbt.MetaData
