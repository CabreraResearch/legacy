using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    /// <summary>
    /// The Nodes Collection, to which all Trees subscribe
    /// </summary>
    public class CswNbtNodeCollection : IEnumerable
    {
        private Dictionary<NodeHashKey, CswNbtNode> _NodeHash;
        private CswNbtResources _CswNbtResources;
        private CswNbtNodeWriter _CswNbtNodeWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodeCollection( CswNbtResources CswNbtResources )
        {

            _CswNbtResources = CswNbtResources;
            _CswNbtNodeWriter = new CswNbtNodeWriter( _CswNbtResources );

            _NodeHash = new Dictionary<NodeHashKey, CswNbtNode>();
        }

        public void Clear()
        {
            _NodeHash.Clear();
            _CswNbtNodeWriter.clear();
        }

        #region Getting Nodes

        /// <summary>
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode this[CswPrimaryKey NodeId]
        {
            get { return GetNode( NodeId, null ); }
        }

        /// <summary>
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        /// <param name="NodeId">String representation of Primary Key of Node</param>
        public CswNbtNode this[string NodePk]
        {
            get
            {
                CswNbtNode Ret = null;
                CswPrimaryKey NodeId = new CswPrimaryKey();
                NodeId.FromString( NodePk );
                Debug.Assert( CswTools.IsPrimaryKey( NodeId ), "The request did not specify a valid materialid." );
                if( CswTools.IsPrimaryKey( NodeId ) )
                {
                    Ret = this[NodeId];
                }
                return Ret;
            }
        }

        public CswNbtNode GetNode( string NodeId, string NodeKey, CswDateTime Date = null )
        {
            CswNbtNode Node = null;
            if( !string.IsNullOrEmpty( NodeKey ) )
            {
                //CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( CswNbtResources, FromSafeJavaScriptParam( NodeKey ) );
                CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( NodeKey );
                Node = _CswNbtResources.getNode( RealNodeKey, Date );
            }
            else if( !string.IsNullOrEmpty( NodeId ) )
            {
                CswPrimaryKey RealNodeId = new CswPrimaryKey();
                if( CswTools.IsInteger( NodeId ) )
                {
                    RealNodeId.TableName = "nodes";
                    RealNodeId.PrimaryKey = CswConvert.ToInt32( NodeId );
                }
                else
                {
                    RealNodeId.FromString( NodeId );
                }
                Node = _CswNbtResources.getNode( RealNodeId, Date );
            }
            return Node;
        } // getNode()


        /// <summary>
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode GetNode( CswPrimaryKey NodeId )
        {
            return GetNode( NodeId, Int32.MinValue, CswEnumNbtNodeSpecies.Plain, null );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, CswDateTime Date )
        {
            return GetNode( NodeId, Int32.MinValue, CswEnumNbtNodeSpecies.Plain, Date );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node (if not provided, make sure NodeTypeId is)</param>
        /// <param name="NodeTypeId">Primary Key of NodeTypeId (only required if NodeId is invalid)</param>
        /// <seealso cref="this[CswPrimaryKey]"/>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId )
        {
            return GetNode( NodeId, NodeTypeId, CswEnumNbtNodeSpecies.Plain, null );
        }

        /// <summary>
        /// Index of nodes by NodeKey.  The NodeId, NodeTypeId and NodeSpecies in the Key are used.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, CswDateTime)"/>
        /// </summary>
        /// <param name="NodeKey">NodeKey for Node</param>
        public CswNbtNode this[CswNbtNodeKey NodeKey]
        {
            get
            {
                if( NodeKey == null )
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Node", "CswNbtNodeCollection received a null NodeKey" );
                if( NodeKey.NodeSpecies != CswEnumNbtNodeSpecies.Plain )
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid Node", "CswNbtNodeCollection cannot fetch Node of species " + NodeKey.NodeSpecies.ToString() );
                return GetNode( NodeKey.NodeId, NodeKey.NodeTypeId, NodeKey.NodeSpecies, null );
            }
        }

        /// <summary>
        /// Fetch a node from the collection.  If the node isn't loaded from the database already, it will be.
        /// NodeTypeId is only required if you don't provide a good NodeId (so that we can still fetch Property info).
        /// </summary>
        /// <param name="NodeId">Primary Key of Node (if not provided, make sure NodeTypeId is)</param>
        /// <param name="NodeTypeId">Primary Key of NodeTypeId (only required if NodeId is invalid)</param>
        /// <param name="Species"><see cref="CswEnumNbtNodeSpecies" /></param>
        /// <param name="Date"></param>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId, CswEnumNbtNodeSpecies Species, CswDateTime Date )
        {
            //bz # 7816: Return NULL rather than throwing
            CswNbtNode Node = null;
            if( NodeId != null && NodeId.PrimaryKey != Int32.MinValue )  // BZ 8753
            {
                NodeHashKey HashKey = new NodeHashKey( NodeId, Species );
                if( false == CswTools.IsDate( Date ) && _NodeHash.ContainsKey( HashKey ) )
                {
                    Node = (CswNbtNode) _NodeHash[HashKey];
                }
                else
                {
                    Node = _getExistingNode( HashKey, NodeTypeId, Date );
                    //if( false == CswTools.IsDate( Date ) && null != Node )
                    //{
                    //    Node = (CswNbtNode) _NodeHash[HashKey];
                    //}
                }
            }
            return Node;
        }//GetNode()


        public CswNbtNode getNodeByRelationalId( CswPrimaryKey RelationalId )
        {
            CswNbtNode ret = null;
            CswPrimaryKey pk = getNodeIdByRelationalId( RelationalId );
            if( null != pk )
            {
                ret = GetNode( pk );
            }
            return ret;
        } // getNodeByRelationalId()

        private Dictionary<CswPrimaryKey, CswPrimaryKey> _NodeIdByRelationalIdDict = new Dictionary<CswPrimaryKey, CswPrimaryKey>();
        public CswPrimaryKey getNodeIdByRelationalId( CswPrimaryKey RelationalId )
        {
            CswPrimaryKey ret = null;
            if( _NodeIdByRelationalIdDict.ContainsKey( RelationalId ) )
            {
                ret = _NodeIdByRelationalIdDict[RelationalId];
            }
            else
            {
                CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "getNodeByRelationalId", "nodes" );
                DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString() { "nodeid", "relationalid" }, "where relationaltable = '" + RelationalId.TableName + "'" ); //" and relationalid='" + RelationalId.PrimaryKey.ToString() + "'" );
                foreach( DataRow row in NodesTable.Rows )
                {
                    CswPrimaryKey thisRelId = new CswPrimaryKey( RelationalId.TableName, CswConvert.ToInt32( row["relationalid"] ) );
                    CswPrimaryKey thisNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( row["nodeid"] ) );
                    if( false == _NodeIdByRelationalIdDict.ContainsKey( thisRelId ) )
                    {
                        _NodeIdByRelationalIdDict.Add( thisRelId, thisNodeId );
                    }
                    if( thisRelId == RelationalId )
                    {
                        ret = thisNodeId;
                    }
                }
            }
            return ret;
        } // getNodeIdByRelationalId()


        /// <summary>
        /// Find a node by a unique property value
        /// </summary>
        /// <param name="MetaDataProp">Property to search with</param>
        /// <param name="PropWrapper">Value to find</param>
        /// <returns></returns>
        public CswNbtNode FindNodeByUniqueProperty( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtNodePropWrapper PropWrapper )
        {
            CswNbtNode ret = null;
            string SQLQuery = string.Empty;
            foreach( CswNbtSubField SubField in MetaDataProp.getFieldTypeRule().SubFields )
            {
                if( SQLQuery != string.Empty ) SQLQuery += " INTERSECT ";
                //if( SubField.RelationalTable == string.Empty )
                //{
                SQLQuery += " (select nodeid, 'nodes' tablename ";
                SQLQuery += "    from jct_nodes_props ";
                SQLQuery += "   where nodetypepropid = " + MetaDataProp.PropId.ToString() + " ";
                SQLQuery += "     and " + SubField.Column.ToString() + " = '" + PropWrapper.GetSubFieldValue( SubField ) + "') ";
                //}
                //else
                //{
                //    string PrimeKeyCol = _CswNbtResources.DataDictionary.getPrimeKeyColumn( SubField.RelationalTable );
                //    SQLQuery += " (select " + PrimeKeyCol + " nodeid, '" + SubField.RelationalTable + "' tablename ";
                //    SQLQuery += "    from " + SubField.RelationalTable + " ";
                //    SQLQuery += "   where " + SubField.RelationalColumn + " = '" + PropWrapper.GetSubFieldValue( SubField ) + "') ";
                //}
            }
            SQLQuery = "select nodeid, tablename from " + SQLQuery;

            CswArbitrarySelect UniquePropSelect = _CswNbtResources.makeCswArbitrarySelect( "FindNodeByUniqueProperty_select", SQLQuery );
            DataTable UniquePropTable = UniquePropSelect.getTable();

            if( UniquePropTable.Rows.Count > 0 )
                ret = this[new CswPrimaryKey( UniquePropTable.Rows[0]["tablename"].ToString(), CswConvert.ToInt32( UniquePropTable.Rows[0]["nodeid"] ) )];

            return ret;
        } // FindNodeByUniqueProperty()

        /// <summary>
        /// Fetches an Enumerator for the Collection
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return _NodeHash.GetEnumerator();
        }

        /// <summary>
        /// Fetches just the nodename of a node, by primary key
        /// </summary>
        public string getNodeName( CswPrimaryKey NodeId )
        {
            string ret = string.Empty;
            NodeHashKey hashKey = new NodeHashKey( NodeId, CswEnumNbtNodeSpecies.Plain );
            if( _NodeHash.ContainsKey( hashKey ) )
            {
                ret = _NodeHash[hashKey].NodeName;
            }
            else
            {
                CswTableSelect NodeSelect = _CswNbtResources.makeCswTableSelect( "getNodeName_select", "nodes" );
                DataTable NodeTable = NodeSelect.getTable( new CswCommaDelimitedString() { "nodename" }, "nodeid", NodeId.PrimaryKey, string.Empty, false );
                if( NodeTable.Rows.Count > 0 )
                {
                    ret = CswConvert.ToString( NodeTable.Rows[0]["nodename"] );
                }
            }
            return ret;
        } // getNodeName()

        #endregion Getting Nodes

        #region Hash maintenance

        /// <summary>
        /// Clears all nodes from the collection.  Does not delete nodes from the database.
        /// </summary>
        private void _clear()//bz # 6653
        {
            _NodeHash.Clear();
        }

        private class NodeHashKey : IEquatable<NodeHashKey>
        {
            public CswPrimaryKey NodeId;
            public CswEnumNbtNodeSpecies Species;
            public NodeHashKey( CswPrimaryKey TheNodeId, CswEnumNbtNodeSpecies TheSpecies )
            {
                NodeId = TheNodeId;
                Species = TheSpecies;
            }

            public override string ToString()
            {
                return Species.ToString() + ' ' + NodeId.ToString();
            }

            #region IEquatable

            public static bool operator ==( NodeHashKey key1, NodeHashKey key2 )
            {
                // If both are null, or both are same instance, return true.
                if( System.Object.ReferenceEquals( key1, key2 ) )
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if( ( (object) key1 == null ) || ( (object) key2 == null ) )
                {
                    return false;
                }

                // Now we know neither are null.  Compare values.
                if( ( key1.NodeId == key2.NodeId ) &&
                    ( key1.Species == key2.Species ) )
                    return true;
                else
                    return false;
            }

            public static bool operator !=( NodeHashKey key1, NodeHashKey key2 )
            {
                return !( key1 == key2 );
            }

            public override bool Equals( object obj )
            {
                if( !( obj is NodeHashKey ) )
                    return false;
                return this == (NodeHashKey) obj;
            }

            public bool Equals( NodeHashKey obj )
            {
                return this == (NodeHashKey) obj;
            }

            public override int GetHashCode()
            {
                if( NodeId != null )
                    return this.NodeId.PrimaryKey;
                else
                    return Int32.MinValue;
            }

            #endregion IEquatable
        }

        #endregion Hash maintenance

        #region Creating Nodes

        /// <remark>
        /// We need a NodeTypeId because the NodeId is missing from the HashKey if this is a new node we're about to add
        /// </remark>
        private CswNbtNode _getExistingNode( NodeHashKey HashKey, Int32 NodeTypeId, CswDateTime Date )
        {
            CswTimer Timer = new CswTimer();
            CswNbtNode Node = new CswNbtNode( _CswNbtResources, _CswNbtNodeWriter, NodeTypeId, CswEnumNbtNodeSpecies.Plain, HashKey.NodeId, _NodeHash.Count, Date );

            //bz # 7816 -- only add to the collection if the node got filled
            Node.fill();
            if( Node.Filled )
            {
                if( !_NodeHash.ContainsKey( HashKey ) )
                {
                    _NodeHash.Add( HashKey, Node );
                }
                _CswNbtResources.logTimerResult( "CswNbtNodeCollection.makeNode on NodeId (" + HashKey.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );
            }
            else
            {
                Node = null;
            }
            return Node;
        }

        public void removeFromCache( CswNbtNode Node )
        {
            _NodeHash.Remove( new NodeHashKey( Node.NodeId, Node.NodeSpecies ) );
        }

        public delegate void AfterMakeNode( CswNbtNode NewNode );

        // <summary>
        // 
        // </summary>
        /// <summary>
        /// Create a new, fresh, empty Node from a node type.  Properties are filled in, but Property Values are not. !!POSTS CHANGES!!
        /// </summary>
        /// <param name="NodeTypeId">Primary Key of Nodetype</param>
        /// <param name="IsTemp">If true, the node is a temp node, though still saved to the database</param>
        /// <param name="OnAfterMakeNode">Event that occurs after creating the node but before saving it for the first time</param>
        /// <param name="OverrideUniqueValidation">If true, allow this node to be created even if it violates uniqueness rules</param>
        /// <returns>The new node. !!POSTS CHANGES!!</returns>
        public CswNbtNode makeNodeFromNodeTypeId( Int32 NodeTypeId, Action<CswNbtNode> OnAfterMakeNode = null, bool IsTemp = false, bool OverrideUniqueValidation = false, bool IsCopy = false, bool OverrideMailReportEvents = false )
        {
            CswNbtNode Node = new CswNbtNode( _CswNbtResources, _CswNbtNodeWriter, NodeTypeId, CswEnumNbtNodeSpecies.Plain, null, _NodeHash.Count, null, IsTemp: true ); // temp here for auditing, but see below
            //Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
            Node.fillFromNodeTypeId();

            _CswNbtNodeWriter.makeNewNodeEntry( Node );
            _CswNbtNodeWriter.setDefaultPropertyValues( Node );

            if( null != OnAfterMakeNode )
            {
                OnAfterMakeNode( Node );
            }

            // We need to hash the Int32.MinValue keys for the Add form to work
            // But we can simply override what's in the hash if we make another new node
            NodeHashKey Hashkey = new NodeHashKey( Node.NodeId, Node.NodeSpecies );
            _NodeHash[Hashkey] = Node;

            ICswNbtNodePersistStrategy NodePersistStrategy;
            if( false == IsTemp )
            {
                NodePersistStrategy = new CswNbtNodePersistStrategyPromoteReal( _CswNbtResources );
                NodePersistStrategy.Creating = true;
            }
            else
            {
                NodePersistStrategy = new CswNbtNodePersistStrategyCreateTemp( _CswNbtResources );
            }

            // only override the defaults on these if they are true
            if( OverrideUniqueValidation )
            {
                NodePersistStrategy.OverrideUniqueValidation = true;
            }
            if( IsCopy )
            {
                NodePersistStrategy.IsCopy = true;
            }
            if( OverrideMailReportEvents )
            {
                NodePersistStrategy.OverrideMailReportEvents = true;
            }

            NodePersistStrategy.postChanges( Node );

            return ( Node );
        }//makeNodeFromNodeTypeId()


        /// <summary>
        /// Fetches a User Node from the Username, using a View and a TreeLoader.
        /// </summary>
        /// <remarks>
        /// not sure if this belongs here in CswNbtNodeCollection
        /// </remarks>
        /// <param name="Username">Username of User</param>
        /// <param name="RequireViewPermissions"></param>
        public CswNbtNode makeUserNodeFromUsername( string Username, bool RequireViewPermissions = true )
        {
            CswTimer Timer = new CswTimer();
            CswNbtNode UserNode = null;

            CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserName_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );

            _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 1", Timer );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtNodes.makeUserNodeFromUsername(" + Username + ")";
            CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );
            CswNbtViewProperty Prop = View.AddViewProperty( UserRelationship, UserName_ObjectClassProp );
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswNbtFieldTypeRuleText.SubFieldName.Text, CswEnumNbtFilterMode.Equals, Username, false );

            _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 2", Timer );

            // generate the tree
            ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions, true, IncludeHiddenNodes: true );

            _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 3", Timer );

            // get user node
            UserTree.goToRoot();
            if( UserTree.getChildNodeCount() > 0 )
            {
                UserTree.goToNthChild( 0 );
                _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 4", Timer );
                UserNode = UserTree.getNodeForCurrentPosition();
            }
            //else
            //{
            //    foreach( CswNbtMetaDataNodeType UserNodeTypes in User_ObjectClass.NodeTypes )
            //    {
            //        foreach( CswNbtNode user in UserNodeTypes.getNodes( true, false ) )
            //        {
            //            if( user.Properties[UserName_ObjectClassProp.PropName].AsText.Text == Username )
            //            {
            //                UserNode = user;
            //                break;
            //            }
            //        }
            //    }
            _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 5", Timer );
            return UserNode;
        }


        public CswNbtNode makeRoleNodeFromRoleName( string RoleName )
        {
            CswNbtNode RoleNode = null;

            CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp RoleName_ObjectClassProp = Role_ObjectClass.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtNodes.makeRoleNodeFromRoleName(" + RoleName + ")";
            CswNbtViewRelationship RoleRelationship = View.AddViewRelationship( Role_ObjectClass, false );
            CswNbtViewProperty Prop = View.AddViewProperty( RoleRelationship, RoleName_ObjectClassProp );
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswNbtFieldTypeRuleText.SubFieldName.Text, CswEnumNbtFilterMode.Equals, RoleName, false );

            // generate the tree
            ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, IncludeHiddenNodes: true );

            // get user node
            UserTree.goToRoot();
            if( UserTree.getChildNodeCount() > 0 )
            {
                UserTree.goToNthChild( 0 );
                RoleNode = UserTree.getNodeForCurrentPosition();
            }
            return RoleNode;
        }


        #endregion Creating Nodes

        #region Database

        /// <summary>
        /// Write all nodes in the collection
        /// </summary>
        /// <remarks>
        /// This might be problematic if we get into write loops
        /// </remarks>
        public void finalize()
        {
            _clear(); //bz # 6653
        }//finalize()

        #endregion Database


    } // CswNbtNodeCollection()
} // namespace ChemSW.Nbt


