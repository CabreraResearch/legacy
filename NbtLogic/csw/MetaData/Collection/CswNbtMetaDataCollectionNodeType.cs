using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeType : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private CswNbtMetaDataCollectionImpl _CollImpl;

        public CswNbtMetaDataCollectionNodeType( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            _CollImpl = new CswNbtMetaDataCollectionImpl( _CswNbtMetaDataResources,
                                                          "nodetypeid",
                                                          "nodetypename",
                                                          _CswNbtMetaDataResources.NodeTypeTableSelect,
                                                          _CswNbtMetaDataResources.NodeTypeTableUpdate,
                                                          makeNodeType,
                                                          _makeModuleWhereClause );
        }

        public void AddToCache( CswNbtMetaDataNodeType NewObj )
        {
            _CollImpl.AddToCache( NewObj );
        }

        public void clearCache()
        {
            _CollImpl.clearCache();
        }

        public CswNbtMetaDataNodeType makeNodeType( CswNbtMetaDataResources Resources, DataRow Row )
        {
            return new CswNbtMetaDataNodeType( Resources, Row );
        }

        public Dictionary<Int32,string> getNodeTypeIds()
        {
            return _CollImpl.getPkDict();
        }
        public Dictionary<Int32, string> getNodeTypeIds( Int32 ObjectClassId )
        {
            return _CollImpl.getPkDict( "where objectclassid = " + ObjectClassId.ToString() );
        }
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes()
        {
            return from NT in _CollImpl.getAll().Cast<CswNbtMetaDataNodeType>() orderby NT.NodeTypeName select NT;
        }
        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypes( Int32 ObjectClassId )
        {
            return from NT in _CollImpl.getWhere( "where objectclassid = " + ObjectClassId.ToString() ).Cast<CswNbtMetaDataNodeType>() orderby NT.NodeTypeName select NT;
        }

        private IEnumerable<CswNbtMetaDataNodeType> _getNodeTypesLatestVersion( Collection<ICswNbtMetaDataObject> NodeTypeCollection )
        {
            Dictionary<Int32, CswNbtMetaDataNodeType> Dict = new Dictionary<Int32, CswNbtMetaDataNodeType>();
            foreach( CswNbtMetaDataNodeType NT in from _NT in NodeTypeCollection.Cast<CswNbtMetaDataNodeType>() orderby _NT.NodeTypeName select _NT )
            {
                if( false == Dict.ContainsKey( NT.FirstVersionNodeTypeId ) ||
                    Dict[NT.FirstVersionNodeTypeId] == null ||
                    Dict[NT.FirstVersionNodeTypeId].NodeTypeId < NT.NodeTypeId )
                {
                    Dict[NT.FirstVersionNodeTypeId] = NT;
                }
            }
            return Dict.Values;
        }

        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypesLatestVersion( Int32 ObjectClassId )
        {
            return _getNodeTypesLatestVersion( _CollImpl.getWhere( "where objectclassid = " + ObjectClassId.ToString() ) );
        }

        public IEnumerable<CswNbtMetaDataNodeType> getNodeTypesLatestVersion()
        {
            return _getNodeTypesLatestVersion( _CollImpl.getAll() );
        }

        public CswNbtMetaDataNodeType getNodeType( Int32 NodeTypeId )
        {
            return (CswNbtMetaDataNodeType) _CollImpl.getByPk( NodeTypeId );
        }


        public CswNbtMetaDataNodeType getNodeTypeLatestVersion( Int32 NodeTypeId )
        {
            return (CswNbtMetaDataNodeType) _CollImpl.getWhereFirst( @"where nodetypeid = (select max(nodetypeid) maxntid
                                                                                             from nodetypes 
                                                                                            where firstversionid = (select firstversionid 
                                                                                                                      from nodetypes
                                                                                                                     where nodetypeid = " + NodeTypeId.ToString() + "))" );
        }

        public CswNbtMetaDataNodeType getNodeTypeLatestVersion( CswNbtMetaDataNodeType NodeType )
        {
            return (CswNbtMetaDataNodeType) _CollImpl.getWhereFirst( @"where nodetypeid = (select max(nodetypeid) maxntid
                                                                                             from nodetypes 
                                                                                            where firstversionid = " + NodeType.FirstVersionNodeTypeId.ToString() + ")" );
        }

        public CswNbtMetaDataNodeType getNodeTypeFirstVersion( Int32 NodeTypeId )
        {
            return (CswNbtMetaDataNodeType) _CollImpl.getWhereFirst( "where nodetypeid = (select firstversionid from nodetypes where nodetypeid = " + NodeTypeId.ToString() + ")" );
        }

        /// <summary>
        /// Get the first nodetype matching by name (which is guaranteed to have the same version history as any other nodetype matching by name).
        /// </summary>
        /// <param name="NodeTypeName">The name of the NodeType</param>
        /// <returns></returns>
        public CswNbtMetaDataNodeType getNodeTypeFirstVersion( string NodeTypeName )
        {
            return (CswNbtMetaDataNodeType) _CollImpl.getWhereFirst( @"where nodetypeid = (select firstversionid from nodetypes where lower(nodetypename) = '" + CswTools.SafeSqlParam( NodeTypeName.ToLower() ) + "')" );
        }

        public CswNbtMetaDataNodeType getNodeTypeLatestVersion( string NodeTypeName )
        {
            // Get any nodetype matching by name 
            // (which is guaranteed to have the same version history as any other nodetype matching by name)
            // Then fetch the latest version of that nodetype
            return (CswNbtMetaDataNodeType) _CollImpl.getWhereFirst( @"where nodetypeid = (select max(nodetypeid) maxntid
                                                                                             from nodetypes 
                                                                                            where firstversionid = (select firstversionid 
                                                                                                                      from nodetypes
                                                                                                                     where lower(nodetypename) = '" + CswTools.SafeSqlParam( NodeTypeName.ToLower() ) + "'))" );
        }

        private string _makeModuleWhereClause()
        {
            return " nodetypes.enabled = '1' ";
        }
    }
}
