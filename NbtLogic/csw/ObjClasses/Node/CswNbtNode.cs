using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNode
    {
        [DataContract]
        public class Node
        {
            public Node( CswNbtNode NbtNode )
            {
                if( null != NbtNode )
                {
                    NodeId = NbtNode.NodeId;
                    NodeName = NbtNode.NodeName;
                }
            }

            public Node( CswPrimaryKey inNodeId, string inNodeName )
            {
                NodeId = inNodeId;
                NodeName = inNodeName;
            }

            public CswPrimaryKey NodeId = null;

            [DataMember( IsRequired = true, EmitDefaultValue = true, Name = "NodeId" )]
            public string NodePk
            {
                get
                {
                    string Ret = string.Empty;
                    if( CswTools.IsPrimaryKey( NodeId ) )
                    {
                        Ret = NodeId.ToString();
                    }
                    return Ret;
                }
                set
                {
                    NodeId = CswConvert.ToPrimaryKey( value );
                }
            }

            [DataMember( IsRequired = false )]
            public string NodeName = String.Empty;

            [DataMember( IsRequired = false )]
            public string NodeLink
            {
                get { return CswNbtNode.getNodeLink( NodeId, NodeName ); }
                set { string val = value; } //this is dumb, but WCF will break without a setter
            }
        }

        public delegate void OnSetNodeIdHandler( CswNbtNode Node, CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId );

        private CswNbtNodePropColl _CswNbtNodePropColl = null;
        private CswNbtObjClass __CswNbtObjClass = null;
        private CswNbtObjClass _CswNbtObjClass
        {
            get
            {
                if( __CswNbtObjClass == null && _NodeTypeId != Int32.MinValue )
                {
                    __CswNbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, _CswNbtResources.MetaData.getObjectClassByNodeTypeId( _NodeTypeId ), this );
                }
                return __CswNbtObjClass;
            }
        }

        public CswDateTime _Date;
        private CswNbtResources _CswNbtResources;
        private CswNbtNodeWriter _CswNbtNodeWriter;
        public CswNbtNode( CswNbtResources CswNbtResources, CswNbtNodeWriter CswNbtNodeWriter, Int32 NodeTypeId, CswEnumNbtNodeSpecies NodeSpecies, CswPrimaryKey NodeId, Int32 UniqueId, CswDateTime Date, bool IsTemp = false )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeWriter = CswNbtNodeWriter;
            _UniqueId = UniqueId;
            _NodeId = NodeId;
            _NodeTypeId = NodeTypeId;
            _NodeSpecies = NodeSpecies;
            _IsTemp = IsTemp;
            _Date = Date;

            _CswNbtNodePropColl = new CswNbtNodePropColl( CswNbtResources, this );
        }//ctor()

        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();

        /// <summary>
        /// Initialize from data row
        /// </summary>
        public void read( DataRow row )
        {
            // None of these should not be considered a "modification"
            _NodeTypeId = CswConvert.ToInt32( row["nodetypeid"] );
            _NodeName = row["nodename"].ToString();
            _ReadOnly = CswConvert.ToBoolean( row["readonly"] );
            _IsDemo = CswConvert.ToBoolean( row["isdemo"] );
            _Locked = CswConvert.ToBoolean( row["locked"] );
            _IsTemp = CswConvert.ToBoolean( row["istemp"] );
            _SessionId = CswConvert.ToString( row["sessionid"] );
            _PendingUpdate = CswConvert.ToBoolean( row["pendingupdate"] );
            _Searchable = CswConvert.ToBoolean( row["searchable"] );
            if( row.Table.Columns.Contains( _CswAuditMetaData.AuditLevelColName ) )
            {
                _AuditLevel = row[_CswAuditMetaData.AuditLevelColName].ToString();
            }
            else
            {
                _AuditLevel = _CswAuditMetaData.DefaultAuditLevel;
            }
        } // read()


        #region Core Properties

        private Int32 _UniqueId = Int32.MinValue;
        public Int32 UniqueId
        {
            get
            {
                return ( _UniqueId );
            }
        }//UniqueId

        private CswEnumNbtNodeModificationState _NodeModificationState = CswEnumNbtNodeModificationState.Unknown;
        public CswEnumNbtNodeModificationState ModificationState
        {
            get
            {

                if( ( CswEnumNbtNodeModificationState.Unchanged == _NodeModificationState ||
                       CswEnumNbtNodeModificationState.Posted == _NodeModificationState ) &&
                    _CswNbtNodePropColl.Modified )
                {
                    _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
                }

                return ( _NodeModificationState );
            }//get

        }//ModificationState

        private bool _IsDemo = false;
        public bool IsDemo
        {
            get { return _IsDemo; }
            set
            {
                _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
                _IsDemo = value;
            }
        }

        private bool _IsTemp = false;
        /// <summary>
        /// If true, this is a temporary node
        /// </summary>
        public bool IsTemp
        {
            get { return _IsTemp; }
        }

        /// <summary>
        /// Converts a temp node to a real one.  
        /// Creates INSERT audit records for current values of all properties.
        /// Thus, make sure all other property modifications have been posted before calling this.
        /// </summary>
        public void PromoteTempToReal( bool OverrideUniqueValidation = false )
        {
            ICswNbtNodePersistStrategy NodePersistStrategy = new CswNbtNodePersistStrategyPromoteReal( _CswNbtResources );
            NodePersistStrategy.OverrideUniqueValidation = OverrideUniqueValidation;
            NodePersistStrategy.postChanges( this );
        }

        public void removeTemp()
        {
            _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
            SessionId = string.Empty;
            _IsTemp = false;
        }

        private bool _Hidden = false;
        public bool Hidden
        {
            get { return _Hidden; }
            set
            {
                _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
                _Hidden = value;
            }
        }

        private string _SessionId = string.Empty;
        /// <summary>
        /// If IsTemp, the SessionId associated with the Node
        /// </summary>
        public string SessionId { get { return _SessionId; } set { _SessionId = value; } }

        private bool _ReadOnly = false;
        private bool _ReadOnlyTemporary = false;
        /// <summary>
        /// Determines whether the node is readonly
        /// </summary>
        public bool ReadOnly
        {
            get { return _ReadOnly || _ReadOnlyTemporary || Locked; }
        }
        /// <summary>
        /// Should only be used by CswNbtNodeWriter
        /// </summary>
        public bool ReadOnlyPermanent
        {
            get { return _ReadOnly; }
        }
        public void setReadOnly( bool value, bool SaveToDb )
        {
            _ReadOnlyTemporary = value;
            if( SaveToDb )
            {
                _ReadOnly = value;
            }
        }

        private bool _Locked = false;
        public bool Locked
        {
            get { return _Locked; }
            set { _Locked = value; }
        }

        public bool Filled
        {
            get
            {
                return ( _CswNbtNodePropColl.Filled );
            }//get
        }//Filled

        public bool DisableSave = false;

        public bool New
        {
            get
            {
                return ( _CswNbtNodePropColl.New );
            }
        }//New

        private CswEnumNbtNodeSpecies _NodeSpecies = CswEnumNbtNodeSpecies.Plain;
        public CswEnumNbtNodeSpecies NodeSpecies { get { return ( _NodeSpecies ); } }

        private CswPrimaryKey _NodeId = null;
        public CswPrimaryKey NodeId
        {
            get { return ( _NodeId ); }
            set { _NodeId = value; }
        }

        private CswPrimaryKey _RelationalId = null;
        public CswPrimaryKey RelationalId
        {
            get { return ( _RelationalId ); }
            set
            {
                _RelationalId = value;
                //Properties.RelationalId = _RelationalId;
            }
        }//RelationalId

        private Int32 _NodeTypeId = 0;
        public Int32 NodeTypeId
        {
            get { return ( _NodeTypeId ); }
            set { _NodeTypeId = value; }
        }

        public CswNbtMetaDataNodeType getNodeType()
        {
            return _CswNbtResources.MetaData.getNodeType( NodeTypeId, _Date );
        }

        public CswNbtMetaDataNodeType getNodeTypeLatestVersion()
        {
            return _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeTypeId );
        }

        public Int32 getObjectClassId()
        {
            return getNodeType().ObjectClassId;
        }

        public CswNbtMetaDataObjectClass getObjectClass()
        {
            return _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeId );
        }

        public CswNbtObjClass ObjClass
        {
            get { return _CswNbtObjClass; }
        }

        private void _clear()
        {
            _NodeName = string.Empty;
            _PendingUpdate = false;
        }//clear()

        private string _NodeName = string.Empty;
        public string NodeName
        {
            get { return ( _NodeName ); }
            set
            {
                // case 20781 - only mark modified if we're changing the name, not assigning it from DB
                if( _NodeName != value && _NodeName != string.Empty )
                {
                    _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
                }
                _NodeName = value;
            }
        }
        private bool _PendingUpdate = false;
        public bool PendingUpdate
        {
            get { return ( _PendingUpdate ); }
            set
            {
                if( _PendingUpdate != value )
                {
                    _PendingUpdate = value;
                    _NodeModificationState = CswEnumNbtNodeModificationState.Modified;
                }
            }
        }

        public string NodeLink
        {
            get { return getNodeLink( NodeId, NodeName ); }
        }

        public static string getNodeLink( CswPrimaryKey NodeId, string NodeName )
        {
            string Id = "none";
            if( CswTools.IsPrimaryKey( NodeId ) )
            {
                Id = NodeId.ToString();
            }
            return "[[" + Id + "][" + NodeName + "]]";
        }

        public string IconFileNameOverride = "";
        public string IconFileName = "";

        private bool _Selectable = true;
        public bool Selectable { get { return ( _Selectable ); } set { _Selectable = value; } }

        private bool _ShowInTree = true;
        public bool ShowInTree { get { return ( _ShowInTree ); } set { _ShowInTree = value; } }

        public CswNbtNodePropColl Properties { get { return ( _CswNbtNodePropColl ); } }

        private string _AuditLevel = ChemSW.Audit.CswEnumAuditLevel.NoAudit;
        public string AuditLevel //27709 nodes fully support audit level now
        {
            get { return ( _AuditLevel ); }
            set { _AuditLevel = value; }
        }

        private bool _Searchable = true;
        public bool Searchable
        {
            set
            {
                _Searchable = value;
            }
            get
            {
                return ( _Searchable );
            }
        }

        public override int GetHashCode()
        {
            return this.NodeId.PrimaryKey;
        }
        #endregion

        #region Methods

        public void postChanges( bool ForceUpdate, bool IsCopy = false, bool OverrideUniqueValidation = false, bool SkipEvents = false )
        {
            ICswNbtNodePersistStrategy NodePersistStrategy = new CswNbtNodePersistStrategyUpdate( _CswNbtResources );
            NodePersistStrategy.ForceUpdate = ForceUpdate;
            NodePersistStrategy.IsCopy = IsCopy;
            NodePersistStrategy.OverrideUniqueValidation = OverrideUniqueValidation;
            NodePersistStrategy.SkipEvents = SkipEvents;
            NodePersistStrategy.postChanges( this );
        }

        public void requestWrite( bool ForceUpdate, bool IsCopy, bool OverrideUniqueValidation, bool Creating, bool AllowAuditing, bool SkipEvents )
        {
            _CswNbtNodeWriter.write( this, ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );
        }

        public void setModificationState( String ModState )
        {
            _NodeModificationState = ModState;
        }

        public void Audit()
        {
            _CswNbtNodeWriter.AuditInsert( this );
            _CswNbtNodePropColl.AuditInsert();
        }

        /// <summary>
        /// Get a tree view of this node, visible to the current user
        /// </summary>
        /// <returns></returns>
        public CswNbtView getViewOfNode( bool includeDefaultFilters = true )
        {
            CswNbtView Ret = getNodeType().CreateDefaultView( includeDefaultFilters );
            Ret.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );
            Ret.ViewMode = CswEnumNbtViewRenderingMode.Tree;
            Ret.Visibility = CswEnumNbtViewVisibility.User;
            Ret.VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId;
            return Ret;
        }

        /// <summary>
        /// Deletes the node from the database.
        /// </summary>
        /// <param name="DeleteAllRequiredRelatedNodes"></param>
        /// <param name="OverridePermissions">For internal use only. When set to true, ignores user permissions.</param>
        public void delete( bool DeleteAllRequiredRelatedNodes = false, bool OverridePermissions = false, bool ValidateRequiredRelationships = true )
        {
            CswNbtMetaDataNodeType thisNT = this.getNodeType();
            if( false == OverridePermissions && false == _CswNbtResources.Permit.canNodeType( Security.CswEnumNbtNodeTypePermission.Delete, thisNT ) )
            {
                throw ( new CswDniException( CswEnumErrorType.Warning, "You do not have permission to delete this " + thisNT.NodeTypeName, "User attempted to delete a " + thisNT.NodeTypeName + " without Delete permissions" ) );
            }

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );
            }

            _CswNbtNodeWriter.delete( this );

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.afterDeleteNode();
            }

            _NodeModificationState = CswEnumNbtNodeModificationState.Deleted;

            _CswNbtResources.Nodes.removeFromCache( this );

        }//delete()

        public void fill()
        {
            if( NodeSpecies == CswEnumNbtNodeSpecies.Plain )
            {
                //bool NodeInfoFetched = false;
                if( CswTools.IsPrimaryKey( NodeId ) && ( NodeTypeId <= 0 || NodeName == String.Empty ) )
                {
                    DataTable NodesTable = null;
                    if( CswTools.IsDate( _Date ) )
                    {
                        string NodesSql = "select * from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "nodes", _Date, NodeId.PrimaryKey );
                        CswArbitrarySelect NodesTableSelect = _CswNbtResources.makeCswArbitrarySelect( "fetchNodeInfo_Select", NodesSql );
                        NodesTable = NodesTableSelect.getTable();
                    }
                    else
                    {
                        CswTableSelect NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNode.fill_nodes", "nodes" );
                        NodesTable = NodesTableSelect.getTable( "nodeid", NodeId.PrimaryKey );
                    }
                    if( NodesTable.Rows.Count > 0 )
                    {
                        read( NodesTable.Rows[0] );
                        RelationalId = new CswPrimaryKey( NodesTable.Rows[0]["relationaltable"].ToString(), CswConvert.ToInt32( NodesTable.Rows[0]["relationalid"] ) );
                    }

                    CswTimer Timer = new CswTimer();
                    if( getNodeType() != null )
                    {
                        Properties.fill( false );
                    }
                    _CswNbtResources.logTimerResult( "Filled in node property data for node (" + NodeId.ToString() + "): " + NodeName, Timer.ElapsedDurationInSecondsAsString );

                    if( CswTools.IsDate( _Date ) )
                    {
                        setReadOnly( value: true, SaveToDb: false );
                    }
                }
            }

            _NodeModificationState = CswEnumNbtNodeModificationState.Unchanged;
        }//fill() 


        public void fillFromNodeTypeId()
        {
            Properties.fill( true );
            _NodeModificationState = CswEnumNbtNodeModificationState.Unchanged;
        }//fillFromNodeTypeId()


        /// <summary>
        /// Copies all matching properties (by name and field type) from another node. 
        /// </summary>
        /// <param name="SourceNode">Node from which to copy property values</param>
        public void copyPropertyValues( CswNbtNode SourceNode )
        {
            foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
            {
                foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
                {
                    if( ThisProp.PropName == SourceProp.PropName && ThisProp.getFieldTypeValue() == SourceProp.getFieldTypeValue() )
                    {
                        ThisProp.copy( SourceProp );
                    } // if( ThisProp.PropName == SourceProp.PropName && ThisProp.FieldType == SourceProp.FieldType )
                } // foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
            } // foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
        } // copyPropertyValues()

        /// <summary>
        /// Copies all matching properties (by name and field type) from another node. 
        /// Forces generic behavior (bypasses field-type specific copy behavior)
        /// </summary>
        /// <param name="SourceNode">Node from which to copy property values</param>
        public void copyPropertyValuesGeneric( CswNbtNode SourceNode )
        {
            foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
            {
                foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
                {
                    if( ThisProp.PropName == SourceProp.PropName && ThisProp.getFieldTypeValue() == SourceProp.getFieldTypeValue() )
                    {
                        ThisProp.copyGeneric( SourceProp );
                    } // if( ThisProp.PropName == SourceProp.PropName && ThisProp.FieldType == SourceProp.FieldType )
                } // foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
            } // foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
        } // copyPropertyValuesGeneric()

        /// <summary>
        /// Sets the values of all relationships whose target matches 
        /// the ParentNode's nodetypeid or objectclassid to the ParentNode's nodeid.
        /// </summary>
        public void RelateToNode( CswNbtNode ParentNode, CswNbtView View )
        {
            CswNbtNodePropWrapper Prop = null;
            // BZ 10372 - Iterate all relationships
            foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ) )
            {
                // BZ 8355 - Set relationships on children pointing to parents, not the other way
                if( ViewRelationship.PropOwner == CswEnumNbtViewPropOwnerType.Second )
                {
                    //if( ( ( ViewRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId && ViewRelationship.SecondId == this.NodeTypeId ) ||
                    //      ( ViewRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId && ViewRelationship.SecondId == this.getObjectClassId() ) ) &&
                    //    ( ( ViewRelationship.FirstType == NbtViewRelatedIdType.NodeTypeId && ViewRelationship.FirstId == ParentNode.NodeTypeId ) ||
                    //      ( ViewRelationship.FirstType == NbtViewRelatedIdType.ObjectClassId && ViewRelationship.FirstId == ParentNode.getObjectClassId() ) ) )
                    if( ViewRelationship.SecondMatches( this.getNodeType() ) && ViewRelationship.FirstMatches( ParentNode.getNodeType() ) )
                    {
                        if( ViewRelationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                        {
                            Prop = this.Properties[_CswNbtResources.MetaData.getNodeTypeProp( ViewRelationship.PropId )];
                        }
                        else if( ViewRelationship.PropType == CswEnumNbtViewPropIdType.ObjectClassPropId )
                        {
                            Prop = this.Properties[_CswNbtResources.MetaData.getObjectClassProp( ViewRelationship.PropId ).PropName];
                        }

                        if( Prop != null )
                        {
                            CswEnumNbtFieldType FT = Prop.getFieldTypeValue();
                            if( FT == CswEnumNbtFieldType.Relationship )
                            {
                                Prop.AsRelationship.RelatedNodeId = ParentNode.NodeId;
                                Prop.AsRelationship.RefreshNodeName();
                            }
                            if( FT == CswEnumNbtFieldType.Location )
                            {
                                Prop.AsLocation.SelectedNodeId = ParentNode.NodeId;
                                Prop.AsLocation.RefreshNodeName();
                            }
                        }
                    }
                } // if( ViewRelationship.PropOwner == PropOwnerType.Second )
            } // foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
        } // RelateToNode()

        /// <summary>
        /// Determines if the Node is marked as Favorite for the current user
        /// </summary>
        /// <returns>true if the node is marked as favorite</returns>
        public bool isFavorite()
        {
            bool isFav = false;
            if( null != _CswNbtResources.CurrentNbtUser && null != _CswNbtResources.CurrentNbtUser.UserId )
            {
                isFav = isFavorite( _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey );
            }
            return isFav;
        }

        /// <summary>
        /// Determines if the Node is marked as Favorite for the given user
        /// </summary>
        /// <returns>true if the node is marked as favorite</returns>
        public bool isFavorite( Int32 UserId )
        {
            CswTableSelect FavoriteSelect = _CswNbtResources.makeCswTableSelect( "favoriteSelect", "favorites" );
            DataTable FavoriteTable = FavoriteSelect.getTable( "where itemid = " + NodeId.PrimaryKey + " and userid = " + UserId );
            return FavoriteTable.Rows.Count > 0;
        }

        /// <summary>
        /// Gets a dictionary of PropName/Gestalt
        /// </summary>
        public Dictionary<string, string> getPropertiesAndValues()
        {
            Dictionary<string, string> PropVals = new Dictionary<string, string>();
            foreach( CswNbtNodePropWrapper Prop in this.Properties )
            {
                PropVals.Add( Prop.PropName, Prop.Gestalt );
            }
            return PropVals;
        }

        #endregion Methods

        public void updateRelationsToThisNode()
        {
            _CswNbtNodeWriter.updateRelationsToThisNode( this );
        }
        public void setSequenceValues()
        {
            _CswNbtNodeWriter.setSequenceValues( this );
        }

    }//CswNbtNode

}//namespace ChemSW.Nbt.ObjClasses
