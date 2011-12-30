using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataCollectionNodeType : ICswNbtMetaDataObjectCollection
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;

        private Collection<ICswNbtMetaDataObject> _AllNodeTypes;
        private Hashtable _ById;
        private Hashtable _ByObjectClass;
        private Hashtable _LatestVersionByFirstVersion;
        private SortedList _ByVersion;

        public CswNbtMetaDataCollectionNodeType( CswNbtMetaDataResources CswNbtMetaDataResources )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;

            _AllNodeTypes = new Collection<ICswNbtMetaDataObject>();
            _ById = new Hashtable();
            _LatestVersionByFirstVersion = new Hashtable();
            _ByVersion = new SortedList();
            _ByObjectClass = new Hashtable();
        }

        public Collection<ICswNbtMetaDataObject> All { get { return _AllNodeTypes; } }

        public ICollection getNodeTypeIds()
        {
            return _ById.Keys;
        }
        public ICollection getNodeTypes()
        {
            return _ByVersion.Values;
        }
        public ICollection getNodeTypes( Int32 ObjectClassId )
        {
            ICollection ret;
            if( _ByObjectClass.ContainsKey( ObjectClassId ) )
                ret = ( (ObjectClassHashEntry) _ByObjectClass[ObjectClassId] )._ById.Values;
            else
                ret = new ArrayList();
            return ret;
        }

        public ICollection getLatestVersionNodeTypes()
        {
            SortedList SortedLVNT = new SortedList();
            foreach( CswNbtMetaDataNodeType NodeType in _LatestVersionByFirstVersion.Values )
            {
                SortedLVNT.Add( NodeType.NodeTypeName, NodeType );
            }
            return SortedLVNT.Values;
        }

        public CswNbtMetaDataNodeType getNodeType( Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType ret = null;
            if( _ById.Contains( NodeTypeId ) )
                ret = _ById[NodeTypeId] as CswNbtMetaDataNodeType;
            return ret;
        }

        public CswNbtMetaDataNodeType getLatestVersionNodeType( CswNbtMetaDataNodeType NodeType )
        {
            return _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] as CswNbtMetaDataNodeType;
        }

        public CswNbtMetaDataNodeType getLatestVersionNodeType( string NodeTypeName )
        {
            // Get any nodetype matching by name 
            // (which is guaranteed to have the same version history as any other nodetype matching by name)
            // Then fetch the latest version of that nodetype
            CswNbtMetaDataNodeType ret = null;
            foreach( CswNbtMetaDataNodeType ThisNT in _ById.Values )
            {
                if( ThisNT.NodeTypeName.ToLower() == NodeTypeName.ToLower() )
                {
                    ret = getLatestVersionNodeType( ThisNT );
                    break;
                }
            }
            return ret;
        }

        public void ClearKeys()
        {
            _ById.Clear();
            _LatestVersionByFirstVersion.Clear();
            _ByVersion.Clear();
            _ByObjectClass.Clear();
        }

        public ICswNbtMetaDataObject RegisterNew( DataRow Row )
        {
            return RegisterNew( Row, Int32.MinValue );
        }
        public ICswNbtMetaDataObject RegisterNew( DataRow Row, Int32 PkToOverride )
        {
            CswNbtMetaDataNodeType NodeType = null;
            if( PkToOverride != Int32.MinValue )
            {
                // This allows existing objects to always point to the latest version of a field type in the collection
                NodeType = getNodeType( PkToOverride );
                Deregister( NodeType );

                CswNbtMetaDataNodeType OldNodeType = new CswNbtMetaDataNodeType( _CswNbtMetaDataResources, NodeType._DataRow );
                _AllNodeTypes.Add( OldNodeType );

                NodeType.Reassign( Row );

                RegisterExisting( OldNodeType );
                RegisterExisting( NodeType );
            }
            else
            {
                NodeType = new CswNbtMetaDataNodeType( _CswNbtMetaDataResources, Row );
                _AllNodeTypes.Add( NodeType );

                RegisterExisting( NodeType );
            }
            return NodeType;
        }

        /// <summary>
        /// Attempt to add a NodeType to the _ByVersion SortedList. Suppress and log errors.
        /// </summary>
        private void _tryAddNodeTypeByVersion( CswNbtMetaDataNodeType NodeType )
        {
            try
            {
                _ByVersion.Add( NodeType, NodeType );
            }
            catch( ArgumentNullException ArgumentNullException )
            {
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Proposed NodeType was null and cannot be added to the MetaData collection.", "", ArgumentNullException ) );
            }
            catch( ArgumentException ArgumentException )
            {
                CswNbtMetaDataNodeType ExistingNodeType = (CswNbtMetaDataNodeType) _ByVersion.GetKey( _ByVersion.IndexOfKey( NodeType ) );
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError(
                    new CswDniException(
                        ErrorType.Error,
                        "Duplicate NodeTypes exist in the database. A NodeType named: " + NodeType.NodeTypeName + " on nodetypeid " + NodeType.NodeTypeId + " has already been defined at version " + NodeType.VersionNo + ".",
                        "NodeType Name: " + ExistingNodeType.NodeTypeName + " is already defined at version " + ExistingNodeType.VersionNo + " with nodetypeid " + ExistingNodeType.NodeTypeId + ".",
                        ArgumentException )
                );
            }
            catch( InvalidOperationException InvalidOperationException )
            {
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Cannot compare the proposed NodeType: " + NodeType.NodeTypeName + " against the MetaData collection.", "", InvalidOperationException ) );
            }
            catch( NotSupportedException NotSupportedException )
            {
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Cannot add the proposed NodeType: " + NodeType.NodeTypeName + " to the MetaData collection.", "", NotSupportedException ) );
            }
        }

        /// <summary>
        /// Attempt to add a NodeType to the _ById Hashtable. Suppress and log errors.
        /// </summary>
        private void _tryAddNodeTypeById( CswNbtMetaDataNodeType NodeType )
        {
            try
            {
                _ById.Add( NodeType.NodeTypeId, NodeType );
            }
            catch( ArgumentNullException ArgumentNullException )
            {
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Proposed NodeType was null and cannot be added to the MetaData collection.", "", ArgumentNullException ) );
            }
            catch( ArgumentException ArgumentException )
            {
                CswNbtMetaDataNodeType ExistingNodeType = (CswNbtMetaDataNodeType) _ById[NodeType.NodeTypeId];
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError(
                    new CswDniException(
                        ErrorType.Error,
                        "Duplicate NodeTypes exist in the database. A NodeType named: " + NodeType.NodeTypeName + " on nodetypeid " + NodeType.NodeTypeId + " has already been defined.",
                        "NodeType Name: " + ExistingNodeType.NodeTypeName + " is already defined with nodetypeid " + ExistingNodeType.NodeTypeId + ".",
                        ArgumentException )
                );
            }
            catch( NotSupportedException NotSupportedException )
            {
                _CswNbtMetaDataResources.CswNbtResources.CswLogger.reportError( new CswDniException( ErrorType.Error, "Cannot add the proposed NodeType: " + NodeType.NodeTypeName + " to the MetaData collection.", "", NotSupportedException ) );
            }
        }

        public void RegisterExisting( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Register got an invalid Object as a parameter" );
            CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

            _tryAddNodeTypeByVersion( NodeType );
            _tryAddNodeTypeById( NodeType );

            // Handle index of latest version by first version
            if( _LatestVersionByFirstVersion.ContainsKey( NodeType.FirstVersionNodeType ) )
            {
                CswNbtMetaDataNodeType LatestVersionNodeType = (CswNbtMetaDataNodeType) _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType];
                if( LatestVersionNodeType.VersionNo < NodeType.VersionNo )
                {
                    _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] = NodeType;
                }
            }
            else
            {
                _LatestVersionByFirstVersion.Add( NodeType.FirstVersionNodeType, NodeType );
            }

            if( false == _ByObjectClass.ContainsKey( NodeType.ObjectClass.ObjectClassId ) )
            {
                _ByObjectClass.Add( NodeType.ObjectClass.ObjectClassId, new ObjectClassHashEntry() );
            }
            ( (ObjectClassHashEntry) _ByObjectClass[NodeType.ObjectClass.ObjectClassId] )._ById.Add( NodeType.NodeTypeId, NodeType );

        }

        public void Deregister( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Deregister got an invalid Object as a parameter" );
            CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

            _ByVersion.Remove( NodeType );
            _ById.Remove( NodeType.NodeTypeId );

            if( ( (CswNbtMetaDataNodeType) _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] ) == NodeType )
            {
                // This is the latest version
                if( NodeType.PriorVersionNodeType != null )
                    _LatestVersionByFirstVersion[NodeType.FirstVersionNodeType] = NodeType.PriorVersionNodeType;
                else
                    _LatestVersionByFirstVersion.Remove( NodeType.FirstVersionNodeType );
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "This NodeType cannot be deleted", "User attempted to delete a nodetype that was not the latest version" );
            }

            if( _ByObjectClass.ContainsKey( NodeType.ObjectClass.ObjectClassId ) )
            {
                ( (ObjectClassHashEntry) _ByObjectClass[NodeType.ObjectClass.ObjectClassId] )._ById.Remove( NodeType.NodeTypeId );
            }
        }

        public void Remove( ICswNbtMetaDataObject Object )
        {
            if( !( Object is CswNbtMetaDataNodeType ) )
                throw new CswDniException( "CswNbtMetaDataCollectionNodeType.Deregister got an invalid Object as a parameter" );
            CswNbtMetaDataNodeType NodeType = Object as CswNbtMetaDataNodeType;

            _AllNodeTypes.Remove( NodeType );
        }


        private class ObjectClassHashEntry
        {
            public Hashtable _ById = new Hashtable();
        }
    }
}
