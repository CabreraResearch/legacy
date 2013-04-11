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

        public Dictionary<Int32, CswEnumNbtFieldType> getFieldTypeIds()
        {
            Dictionary<Int32, string> FTDict = _CollImpl.getPkDict();
            return FTDict.Keys
                         .Where( key => FTDict[key] != CswNbtResources.UnknownEnum )
                         .ToDictionary( key => key, key => (CswEnumNbtFieldType) FTDict[key] );
        } // getFieldTypeIds()

        public IEnumerable<CswNbtMetaDataFieldType> getFieldTypes()
        {
            return _CollImpl.getAll().Cast<CswNbtMetaDataFieldType>();
        }

        public CswNbtMetaDataFieldType getFieldType( CswEnumNbtFieldType FieldType )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getWhereFirst( "where lower(fieldtype)='" + FieldType.ToString().ToLower() + "'" );
        }

        public CswNbtMetaDataFieldType getFieldType( Int32 FieldTypeId )
        {
            return (CswNbtMetaDataFieldType) _CollImpl.getByPk( FieldTypeId );
        }

        public CswEnumNbtFieldType getFieldTypeValue( Int32 FieldTypeId )
        {
            // This fetches all of them at once.  This was done on purpose.
            // This will actually perform better in any case where you need more than one.
            return getFieldTypeIds()[FieldTypeId];
        }

        public CswEnumNbtFieldType getFieldTypeValueForNodeTypePropId( Int32 NodeTypePropId )
        {
            CswEnumNbtFieldType FieldType = CswNbtResources.UnknownEnum;
            if( NodeTypePropId != Int32.MinValue )
            {
                string FieldTypeStr = _CollImpl.getNameWhereFirst( "where fieldtypeid = (select fieldtypeid from nodetype_props where nodetypepropid = " + NodeTypePropId.ToString() + ")" );
                FieldType = FieldTypeStr;
            }
            return FieldType;
        } // getFieldTypeValueForNodeTypePropId()

        public CswEnumNbtFieldType getFieldTypeValueForObjectClassPropId( Int32 ObjectClassPropId )
        {
            CswEnumNbtFieldType FieldType = CswNbtResources.UnknownEnum;
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