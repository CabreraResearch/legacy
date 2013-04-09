using System;
using System.Collections.Generic;
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
                                                          _CswNbtMetaDataResources.FieldTypeTableSelect,
                                                          _CswNbtMetaDataResources.FieldTypeTableUpdate,
                                                          makeFieldType,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataFieldType NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }



        public CswNbtMetaDataFieldType makeFieldType( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataFieldType( Resources, Row );
        }

        public Dictionary<Int32, CswNbtMetaDataFieldType.NbtFieldType> getFieldTypeIds()
        {
            Dictionary<Int32, string> FTDict = _CollImpl.getPkDict();
            return FTDict.Keys
                         .Where( key => FTDict[key] != CswNbtResources.UnknownEnum )
                         .ToDictionary( key => key, key => (CswNbtMetaDataFieldType.NbtFieldType) FTDict[key] );
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

        public CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValue( Int32 FieldTypeId )
        {
            // This fetches all of them at once.  This was done on purpose.
            // This will actually perform better in any case where you need more than one.
            return getFieldTypeIds()[FieldTypeId];
        }

        public CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValueForNodeTypePropId( Int32 NodeTypePropId )
        {
            CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtResources.UnknownEnum;
            if( NodeTypePropId != Int32.MinValue )
            {
                string FieldTypeStr = _CollImpl.getNameWhereFirst( "where fieldtypeid = (select fieldtypeid from nodetype_props where nodetypepropid = " + NodeTypePropId.ToString() + ")" );
                FieldType = FieldTypeStr;
            }
            return FieldType;
        } // getFieldTypeValueForNodeTypePropId()

        public CswNbtMetaDataFieldType.NbtFieldType getFieldTypeValueForObjectClassPropId( Int32 ObjectClassPropId )
        {
            CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtResources.UnknownEnum;
            if( ObjectClassPropId != Int32.MinValue )
            {
                string FieldTypeStr = _CollImpl.getNameWhereFirst( "where fieldtypeid = (select fieldtypeid from object_class_props where objectclasspropid = " + ObjectClassPropId.ToString() + ")" );
                FieldType = FieldTypeStr;
            }
            return FieldType;
        } // getFieldTypeValueForObjectClassPropId()


        private string _makeModuleWhereClause()
        {
            return string.Empty;
        }

    } // class CswNbtMetaDataCollectionFieldType
} // namespace ChemSW.Nbt.MetaData