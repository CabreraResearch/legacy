using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeTypeProp : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionNodeTypeProp( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "nodetypepropid",
                                                          "propname",
                                                          _CswNbtMetaDataResources.NodeTypePropTableSelect,
                                                          _CswNbtMetaDataResources.NodeTypePropTableUpdate,
                                                          makeNodeTypeProp,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataNodeTypeProp NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataNodeTypeProp makeNodeTypeProp( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataNodeTypeProp( Resources, Row );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getByPk( NodeTypePropId );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropVersion( Int32 NodeTypeId, Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and firstpropversionid = (select firstpropversionid from nodetype_props where nodetypepropid = " + NodeTypePropId.ToString() + ")" );
        }

        public Collection<Int32> getNodeTypePropIds()
        {
            return _CollImpl.getPks();
        }
        public Collection<Int32> getNodeTypePropIds( Int32 NodeTypeId )
        {
            return _CollImpl.getPks( "where nodetypeid = " + NodeTypeId.ToString() );
        }
        public Collection<Int32> getNodeTypePropIdsByTab( Int32 TabId )
        {
            return _CollImpl.getPks( "where nodetypetabsetid = " + TabId.ToString() );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByObjectClassProp( Int32 ObjectClassPropId )
        {
            return _CollImpl.getWhere( "where objectclasspropid = " + ObjectClassPropId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( Int32 NodeTypeId )
        {
            return _CollImpl.getWhere( "where nodetypeid = " + NodeTypeId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CollImpl.getWhere( "where fieldtypeid in (select fieldtypeid from field_types where fieldtype = '" + FieldType.ToString() + "')" ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps( Int32 NodeTypeId, CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            return _CollImpl.getWhere( "where nodetypeid = " + NodeTypeId.ToString() +
                                        " and fieldtypeid in (select fieldtypeid from field_types where fieldtype = '" + FieldType.ToString() + "')" ).Cast<CswNbtMetaDataNodeTypeProp>();
        }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByTab( Int32 TabId )
        {
            return _CollImpl.getWhere( "where nodetypetabsetid = " + TabId.ToString() ).Cast<CswNbtMetaDataNodeTypeProp>();
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByDisplayOrder( Int32 NodeTypeId, Int32 TabId )
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.NodeTypeLayout.getPropsInLayout( NodeTypeId, TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
        }
        
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getByPk( NodeTypePropId );
        }
        
        public CswNbtMetaDataNodeTypeProp getNodeTypeProp( Int32 NodeTypeId, string NodeTypePropName )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and lower(propname) = '" + CswTools.SafeSqlParam( NodeTypePropName.ToLower() ) + "'" );
        }
        
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassProp( Int32 NodeTypeId, Int32 ObjectClassPropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and objectclasspropid = " + ObjectClassPropId.ToString() );
        }
        
        public CswNbtMetaDataNodeTypeProp getNodeTypePropByObjectClassProp( Int32 NodeTypeId, string ObjectClassPropName )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and objectclasspropid in (select objectclasspropid from object_class_props where lower(propname) = '" + CswTools.SafeSqlParam( ObjectClassPropName.ToLower() ) + "')" );
        }
        
        public Int32 getNodeTypePropId( Int32 NodeTypeId, string PropName )
        {
            return _CollImpl.getPksFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and lower(propname) = '" + CswTools.SafeSqlParam( PropName.ToLower() ) + "'" );
        }
        
        public Int32 getNodeTypePropIdByObjectClassProp( Int32 NodeTypeId, string ObjectClassPropName )
        {
            return _CollImpl.getPksFirst( "where nodetypeid = " + NodeTypeId.ToString() + " and objectclasspropid in (select objectclasspropid from object_class_props where lower(propname) = '" + CswTools.SafeSqlParam( ObjectClassPropName.ToLower() ) + "')" );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropFirstVersion( Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( "where nodetypepropid = (select firstpropversionid from nodetype_props where nodetypepropid = " + NodeTypePropId.ToString() + ")" );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropLatestVersion( Int32 NodeTypePropId )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( @"where nodetypepropid = (select max(nodetypepropid) maxpropid
                                                                                                     from nodetype_props 
                                                                                                    where firstpropversionid = (select firstpropversionid 
                                                                                                                                  from nodetype_props 
                                                                                                                                 where nodetypepropid = " + NodeTypePropId.ToString() + "))" );
        }

        public CswNbtMetaDataNodeTypeProp getNodeTypePropLatestVersion( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return (CswNbtMetaDataNodeTypeProp) _CollImpl.getWhereFirst( @"where nodetypepropid = (select max(nodetypepropid) maxpropid
                                                                                                     from nodetype_props 
                                                                                                    where firstpropversionid = " + NodeTypeProp.FirstPropVersionId.ToString() + ")" );
        }

        public IEnumerable<CswNbtMetaDataNodeTypeProp> getLayoutProps( Int32 NodeTypeId, Int32 TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, bool PropsInLayout = true )
        {
            string NodeTypeIdStr = NodeTypeId.ToString();
            string WhereClause = "where nodetypeid = '" + NodeTypeIdStr + "' ";
            WhereClause += " and nodetypepropid ";
            if( PropsInLayout )
            {
                WhereClause += " in ";
            }
            else
            {
                WhereClause += " not in ";
            }
            WhereClause += @" (select nodetypepropid 
                                 from nodetype_layout
                                where layouttype = '" + LayoutType.ToString() + @"' 
                                  and nodetypeid = " + NodeTypeIdStr + @" ";
            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit && TabId != Int32.MinValue )
            {
                WhereClause += "and nodetypetabsetid = " + TabId.ToString();
            }
            WhereClause += ")";

            return _CollImpl.getWhere( WhereClause ).Cast<CswNbtMetaDataNodeTypeProp>();

        } // getPropsInLayout()


        private string _makeModuleWhereClause()
        {
            return " nodetype_props.nodetypeid in (select nodetypeid from nodetypes where enabled = '1') ";
        }

        
    } // class CswNbtMetaDataCollectionNodeTypeProp
} // namespace ChemSW.Nbt.MetaData