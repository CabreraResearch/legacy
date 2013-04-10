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

        public Collection<Int32> getNodeTypeIds()
        {
            return _CollImpl.getPks();
        }
        public Collection<Int32> getNodeTypeIds( Int32 ObjectClassId )
        {
            return _CollImpl.getPks( "where objectclassid = " + ObjectClassId.ToString() );
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
            //            return @" ( ( exists (select j.jctmoduleobjectclassid
            //                                    from jct_modules_objectclass j
            //                                    join modules m on j.moduleid = m.moduleid
            //                                   where j.objectclassid = nodetypes.objectclassid
            //                                     and m.enabled = '1')
            //                          or not exists (select j.jctmoduleobjectclassid
            //                                           from jct_modules_objectclass j
            //                                           join modules m on j.moduleid = m.moduleid
            //                                          where j.objectclassid = nodetypes.objectclassid) )
            //                    and ( exists (select j.jctmodulenodetypeid
            //                                    from jct_modules_nodetypes j
            //                                    join modules m on j.moduleid = m.moduleid
            //                                   where j.nodetypeid = nodetypes.nodetypeid
            //                                     and m.enabled = '1')
            //                          or not exists (select j.jctmodulenodetypeid
            //                                           from jct_modules_nodetypes j
            //                                           join modules m on j.moduleid = m.moduleid
            //                                          where j.nodetypeid = nodetypes.nodetypeid) ) )";
            return " nodetypes.enabled = '1' ";
        }

        //public void ClearKeys()
        //{
        //    _ById.Clear();
        //    _LatestVersionByFirstVersion.Clear();
        //    _ByVersion.Clear();
        //    _ByObjectClass.Clear();
        //}

        //public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        //{
        //    return RegisterNew( Row, Int32.MinValue );
        //}
        //public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        //{
        //    CswNbtMetaDataNodeType NodeType = null;
        //    if( PkToOverride != Int32.MinValue )
        //    {
        //        // This allows existing objects to always point to the latest version of a field type in the collection
        //        NodeType = getNodeType( PkToOverride );
        //        Deregister( NodeType );

        //        CswNbtMetaDataNodeType OldNodeType = new CswNbtMetaDataNodeType( _CswNbtMetaDataResources, NodeType._DataRow );
        //        _AllNodeTypes.Add( OldNodeType );

        //        NodeType.Reassign( Row );

        //        RegisterExisting( OldNodeType );
        //        RegisterExisting( NodeType );
        //    }
        //    else
        //    {
        //        NodeType = new CswNbtMetaDataNodeType( _CswNbtMetaDataResources, Row );
        //        _AllNodeTypes.Add( NodeType );

        //        RegisterExisting( NodeType );
        //    }
        //    return NodeType;
        //}

        //public void RegisterExisting( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeType ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Register got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

        //    _CswNbtMetaDataResources.tryAddToMetaDataCollection( NodeType, NodeType, _ByVersion, "NodeType", NodeType.NodeTypeId, NodeType.NodeTypeName );
        //    _CswNbtMetaDataResources.tryAddToMetaDataCollection( NodeType.NodeTypeId, NodeType, _ById, "NodeType", NodeType.NodeTypeId, NodeType.NodeTypeName );

        //    // Handle index of latest version by first version
        //    if( _LatestVersionByFirstVersion.ContainsKey( NodeType.FirstVersionNodeType ) )
        //    {
        //        CswNbtMetaDataNodeType LatestVersionNodeType = (CswNbtMetaDataNodeType) _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType];
        //        if( LatestVersionNodeType.VersionNo < NodeType.VersionNo )
        //        {
        //            _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] = NodeType;
        //        }
        //    }
        //    else
        //    {
        //        _LatestVersionByFirstVersion.Add( NodeType.FirstVersionNodeType, NodeType );
        //    }

        //    if( false == _ByObjectClass.ContainsKey( NodeType.ObjectClass.ObjectClassId ) )
        //    {
        //        _ByObjectClass.Add( NodeType.ObjectClass.ObjectClassId, new ObjectClassHashEntry() );
        //    }
        //    ( (ObjectClassHashEntry) _ByObjectClass[NodeType.ObjectClass.ObjectClassId] )._ById.Add( NodeType.NodeTypeId, NodeType );

        //}

        //public void Deregister( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeType ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Deregister got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

        //    _ByVersion.Remove( NodeType );
        //    _ById.Remove( NodeType.NodeTypeId );

        //    if( ( (CswNbtMetaDataNodeType) _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] ) == NodeType )
        //    {
        //        // This is the latest version
        //        if( NodeType.PriorVersionNodeType != null )
        //            _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] = NodeType.PriorVersionNodeType;
        //        else
        //            _LatestVersionByFirstVersion.Remove( NodeType.FirstVersionNodeType );
        //    }
        //    else
        //    {
        //        throw new CswDniException( ErrorType.Warning, "This NodeType cannot be deleted", "User attempted to delete a nodetype that was not the latest version" );
        //    }

        //    if( _ByObjectClass.ContainsKey( NodeType.ObjectClass.ObjectClassId ) )
        //    {
        //        ( (ObjectClassHashEntry) _ByObjectClass[NodeType.ObjectClass.ObjectClassId] )._ById.Remove( NodeType.NodeTypeId );
        //    }
        //}

        //public void Remove( ICswNbtMetaDataObject Object )
        //{
        //    if( !( Object is CswNbtMetaDataNodeType ) )
        //        throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Deregister got an invalid Object as a parameter" );
        //    CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

        //    _AllNodeTypes.Remove( NodeType );
        //}


        //private class ObjectClassHashEntry
        //{
        //    public Hashtable _ById = new Hashtable();
        //}
    }
}
