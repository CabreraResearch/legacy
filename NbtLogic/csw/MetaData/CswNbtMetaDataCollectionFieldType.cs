using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionFieldType : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionFieldType( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "fieldtypeid", 
                                                          "fieldtype",
                                                          _CswNbtMetaDataResources.FieldTypeTableUpdate,
                                                          makeFieldType );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataFieldType makeFieldType( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataFieldType( Resources, Row );
        }

        public Dictionary<CswNbtMetaDataFieldType.NbtFieldType, Int32> getFieldTypeIds()
        {
            Dictionary<CswNbtMetaDataFieldType.NbtFieldType, Int32> ret = new Dictionary<CswNbtMetaDataFieldType.NbtFieldType, Int32>();
            Dictionary<string, Int32> FTDict = _CollImpl.getPkDict();
            CswNbtMetaDataFieldType.NbtFieldType FTKey = CswNbtMetaDataFieldType.NbtFieldType.Unknown;
            foreach( string Key in FTDict.Keys )
            {
                Enum.TryParse<CswNbtMetaDataFieldType.NbtFieldType>( Key, out FTKey );
                if( false == ret.ContainsKey( FTKey ) )
                {
                    ret.Add( FTKey, FTDict[Key] );
                }
            }
            return ret;
        } // getFieldTypeIds()

        public IEnumerable<CswNbtMetaDataFieldType> getFieldTypes()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataFieldType>();
        }

        public CswNbtMetaDataFieldType getFieldType( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getWhereFirst( "where lower(fieldtype)='" + FieldType.ToString().ToLower() + "'" );
        }

        public CswNbtMetaDataFieldType getFieldType( Int32 FieldTypeId )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getByPk( FieldTypeId );
        }
    } // class CswNbtMetaDataCollectionFieldType
} // namespace ChemSW.Nbt.MetaData