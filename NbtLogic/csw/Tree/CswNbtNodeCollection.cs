using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ChemSW.Nbt
{
    /// <summary>
    /// The Nodes Collection, to which all Trees subscribe
    /// </summary>
    public class CswNbtNodeCollection : IEnumerable
    {
        private Hashtable NodeHash;
        private CswNbtResources _CswNbtResources;
        //private ICswNbtObjClassFactory _ICswNbtObjClassFactory;
        //private CswNbtNodeReader _CswNbtNodeReader;
        //private CswNbtNodeWriter _CswNbtNodeWriter;

        private Dictionary<Int32, Int32> NodeTypeCounts = new Dictionary<Int32, Int32>();
        private Dictionary<Int32, Int32> ObjClassCounts = new Dictionary<Int32, Int32>();

        public CswNbtNodeFactory CswNbtNodeFactory { get { return _CswNbtNodeFactory; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodeCollection( CswNbtResources CswNbtResources ) // , ICswNbtObjClassFactory ICswNbtObjClassFactory )
        {

            _CswNbtResources = CswNbtResources;
            //_ICswNbtObjClassFactory = ICswNbtObjClassFactory;
            //_CswNbtNodeReader = new CswNbtNodeReader( _CswNbtResources );
            //_CswNbtNodeWriter = new CswNbtNodeWriter( _CswNbtResources );

            NodeHash = new Hashtable();
        }


        //necessary to allow clearing of node factory in order to avoid memory leak
        private CswNbtNodeFactory __CswNbtNodeFactory = null;
        private CswNbtNodeFactory _CswNbtNodeFactory
        {
            get
            {
                if( null == __CswNbtNodeFactory )
                {
                    __CswNbtNodeFactory = new CswNbtNodeFactory( _CswNbtResources ); //, ICswNbtObjClassFactory );
                }

                return ( __CswNbtNodeFactory );
            }
        }//_CswNbtNodeFactory

        public void Clear()
        {
            NodeHash.Clear();
            _CswNbtNodeFactory.CswNbtNodeWriter.clear();
        }

        #region Getting Nodes

        /// <summary>
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode this[CswPrimaryKey NodeId]
        {
            get { return GetNode( NodeId, DateTime.MinValue ); }
        }

        /// <summary>
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
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

        public CswNbtNode GetNode( string NodeId, string NodeKey, CswDateTime Date )
        {
            CswNbtNode Node = null;
            Date = Date ?? new CswDateTime( _CswNbtResources.CurrentNbtUser );
            if( !string.IsNullOrEmpty( NodeKey ) )
            {
                //CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( CswNbtResources, FromSafeJavaScriptParam( NodeKey ) );
                CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( NodeKey );
                Node = _CswNbtResources.getNode( RealNodeKey, Date.ToDateTime() );
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
                Node = _CswNbtResources.getNode( RealNodeId, Date.ToDateTime() );
            }
            return Node;
        } // getNode()


        /// <summary>
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode GetNode( CswPrimaryKey NodeId )
        {
            return GetNode( NodeId, Int32.MinValue, CswEnumNbtNodeSpecies.Plain, DateTime.MinValue );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
        /// </summary>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, DateTime Date )
        {
            return GetNode( NodeId, Int32.MinValue, CswEnumNbtNodeSpecies.Plain, Date );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node (if not provided, make sure NodeTypeId is)</param>
        /// <param name="NodeTypeId">Primary Key of NodeTypeId (only required if NodeId is invalid)</param>
        /// <seealso cref="this[CswPrimaryKey]"/>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId )
        {
            return GetNode( NodeId, NodeTypeId, CswEnumNbtNodeSpecies.Plain, DateTime.MinValue );
        }

        /// <summary>
        /// Index of nodes by NodeKey.  The NodeId, NodeTypeId and NodeSpecies in the Key are used.  See <see cref="GetNode(CswPrimaryKey, int, CswEnumNbtNodeSpecies, DateTime)"/>
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
                return GetNode( NodeKey.NodeId, NodeKey.NodeTypeId, NodeKey.NodeSpecies, DateTime.MinValue );
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
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId, CswEnumNbtNodeSpecies Species, DateTime Date )
        {
            //bz # 7816: Return NULL rather than throwing
            CswNbtNode Node = null;
            if( NodeId != null && NodeId.PrimaryKey != Int32.MinValue )  // BZ 8753
            {
                NodeHashKey HashKey = new NodeHashKey( NodeId, Species );
                if( Date == DateTime.MinValue && NodeHash.ContainsKey( HashKey ) )
                {
                    Node = (CswNbtNode) NodeHash[HashKey];
                }
                else
                {
                    Node = makeNode( HashKey, NodeTypeId, Date );
                    if( Date == DateTime.MinValue && null != Node )
                    {
                        Node = (CswNbtNode) NodeHash[HashKey];
                    }
                }
                //if( !NodeHash.ContainsKey( HashKey ) )
                //    throw new CswDniException( "Invalid Node", "Failed to find node with nodeid=" + NodeId.ToString() + ", species=" + Species.ToString() );
            }
            return Node;
        }//GetNode()


        public CswNbtNode getNodeByRelationalId( CswPrimaryKey RelationalId )
        {
            CswNbtNode ret = null;
            CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "getNodeByRelationalId", "nodes" );
            DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString() {"nodeid"}, "where relationaltable = '" + RelationalId.TableName + "' and relationalid='" + RelationalId.PrimaryKey.ToString() + "'" );
            if( NodesTable.Rows.Count > 0 )
            {
                ret = GetNode( new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodesTable.Rows[0]["nodeid"] ) ) );
            }
            return ret;
        } // getNodeByRelationalId()


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
                SQLQuery += "     and " + SubField.Column.ToString() + " = '" + PropWrapper.GetPropRowValue( SubField.Column ) + "') ";
                //}
                //else
                //{
                //    string PrimeKeyCol = _CswNbtResources.DataDictionary.getPrimeKeyColumn( SubField.RelationalTable );
                //    SQLQuery += " (select " + PrimeKeyCol + " nodeid, '" + SubField.RelationalTable + "' tablename ";
                //    SQLQuery += "    from " + SubField.RelationalTable + " ";
                //    SQLQuery += "   where " + SubField.RelationalColumn + " = '" + PropWrapper.GetPropRowValue( SubField.Column ) + "') ";
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
            return NodeHash.GetEnumerator();
        }

        #endregion Getting Nodes

        #region Hash maintenance

        /// <summary>
        /// Clears all nodes from the collection.  Does not delete nodes from the database.
        /// </summary>
        private void _clear()//bz # 6653
        {
            NodeHash.Clear();
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
        private CswNbtNode makeNode( NodeHashKey HashKey, Int32 NodeTypeId, DateTime Date )
        {
            CswTimer Timer = new CswTimer();
            //CswNbtNode Node = new CswNbtNode( _CswNbtResources, NodeTypeId, HashKey.Species, NodeHash.Count, _ICswNbtObjClassFactory );
            CswNbtNode Node = _CswNbtNodeFactory.make( CswEnumNbtNodeSpecies.Plain, HashKey.NodeId, NodeTypeId, NodeHash.Count );

            //bz # 5943
            //Node.OnRequestWriteNode = new CswNbtNode.OnRequestWriteNodeHandler( _CswNbtNodeWriter.write );
            //Node.OnRequestDeleteNode = new CswNbtNode.OnRequestDeleteNodeHandler( _CswNbtNodeWriter.delete );
            //Node.OnRequestFill = new CswNbtNode.OnRequestFillHandler( _CswNbtNodeReader.completeNodeData );
            //Node.OnRequestFillFromNodeTypeId = new CswNbtNode.OnRequestFillFromNodeTypeIdHandler( _CswNbtNodeReader.fillFromNodeTypeIdWithProps );


            //Node.NodeId = HashKey.NodeId;

            //bz # 5943
            //_CswNbtNodeReader.completeNodeData( Node );
            //bz # 7816 -- only add to the collection if the node got filled
            Node.fill( Date );
            if( Node.Filled )
            {
                if( !NodeHash.ContainsKey( HashKey ) )
                {
                    NodeHash.Add( HashKey, Node );
                }
                _CswNbtResources.logTimerResult( "CswNbtNodeCollection.makeNode on NodeId (" + HashKey.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );

                //Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
                Node.OnRequestDeleteNode += OnAfterDeleteNode;
            }
            else
            {
                Node = null;
            }

            return Node;
        }
        ///// <summary>
        ///// Create a new, fresh, empty Node.
        ///// Property data is not fetched.  
        ///// The Node is only cached if the NodeId > 0.  
        ///// To get a node and its property data, use <see cref="CswNbtNodeCollection.GetNode(int, int, NodeSpecies)"/>
        ///// </summary>
        ///// <remarks>
        ///// We may not want to support this using this paradigm, but for now...
        ///// </remarks>
        ///// <param name="NodeId">Primary Key of Node</param>
        ///// <param name="NodeTypeId">Primary Key of NodeTypeId</param>
        ///// <param name="Species"><see cref="NodeSpecies"/></param>
        //public CswNbtNode makeEmptyNode( Int32 NodeId, Int32 NodeTypeId, NodeSpecies Species )
        //{
        //    //CswNbtNode Node = new CswNbtNode( _CswNbtResources, NodeTypeId, Species, NodeHash.Count, _ICswNbtObjClassFactory );
        //    CswNbtNode Node = _CswNbtNodeFactory.make( NodeSpecies.Plain, NodeTypeId, NodeHash.Count );


        //    //bz # 5943
        //    //Node.OnRequestWriteNode = new CswNbtNode.OnRequestWriteNodeHandler( _CswNbtNodeWriter.write );
        //    //Node.OnRequestDeleteNode = new CswNbtNode.OnRequestDeleteNodeHandler( _CswNbtNodeWriter.delete );
        //    //Node.OnRequestFill = new CswNbtNode.OnRequestFillHandler( _CswNbtNodeReader.completeNodeData );
        //    //Node.OnRequestFillFromNodeTypeId = new CswNbtNode.OnRequestFillFromNodeTypeIdHandler( _CswNbtNodeReader.fillFromNodeTypeIdWithProps );

        //    Node.NodeId = NodeId;


        //    //_CswNbtNodeReader.completeNodeData(Node);
        //    //if(NodeId > 0)
        //    NodeHash.Add( new NodeHashKey( NodeId, Species ), Node );

        //    Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
        //    Node.OnRequestDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( OnAfterDeleteNode );

        //    return Node;
        //}

        //private void OnAfterSetNodeIdHandler( CswNbtNode Node, CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId )
        //{
        //    NodeHash.Remove( new NodeHashKey( OldNodeId, Node.NodeSpecies ) );
        //    NodeHash.Add( new NodeHashKey( NewNodeId, Node.NodeSpecies ), Node );
        //}

        private void OnAfterDeleteNode( CswNbtNode Node )
        {
            NodeHash.Remove( new NodeHashKey( Node.NodeId, Node.NodeSpecies ) );
        }

        public delegate void AfterMakeNode( CswNbtNode NewNode );

        // <summary>
        // 
        // </summary>
        /// <summary>
        /// Create a new, fresh, empty Node from a node type.  Properties are filled in, but Property Values are not.
        /// </summary>
        /// <param name="NodeTypeId">Primary Key of Nodetype</param>
        /// <param name="IsTemp">If true, the node is a temp node, though still saved to the database</param>
        /// <param name="OnAfterMakeNode">Event that occurs after creating the node but before saving it for the first time</param>
        /// <param name="OverrideUniqueValidation">If true, allow this node to be created even if it violates uniqueness rules</param>
        /// <returns>The new node</returns>
        public CswNbtNode makeNodeFromNodeTypeId( Int32 NodeTypeId, AfterMakeNode OnAfterMakeNode = null, bool IsTemp = false, bool OverrideUniqueValidation = false )
        {
            CswNbtNode Node = _CswNbtNodeFactory.make( CswEnumNbtNodeSpecies.Plain, null, NodeTypeId, NodeHash.Count );
            //Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
            Node.OnRequestDeleteNode += OnAfterDeleteNode;
            Node.fillFromNodeTypeId( NodeTypeId );
            Node.IsTemp = IsTemp;

            _CswNbtNodeFactory.CswNbtNodeWriter.makeNewNodeEntry( Node, IsCopy: false, OverrideUniqueValidation: OverrideUniqueValidation );
            _CswNbtNodeFactory.CswNbtNodeWriter.setDefaultPropertyValues( Node );

            if( null != OnAfterMakeNode )
            {
                OnAfterMakeNode( Node );
            }

            Node.postChanges( true, false, OverrideUniqueValidation, IsCreate: ( false == IsTemp ) );

            //if( Node.NodeId != Int32.MinValue )
            //{
            //    NodeHash.Add( new NodeHashKey( Node.NodeId, Node.NodeSpecies ), Node );
            //}
            // We need to hash the Int32.MinValue keys for the Add form to work
            // But we can simply override what's in the hash if we make another new node
            NodeHashKey Hashkey = new NodeHashKey( Node.NodeId, Node.NodeSpecies );
            NodeHash[Hashkey] = Node;

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
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswEnumNbtSubFieldName.Text, CswEnumNbtFilterMode.Equals, Username, false );

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
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswEnumNbtSubFieldName.Unknown, CswEnumNbtFilterMode.Equals, RoleName, false );

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

        ///// <summary>
        ///// Save the node to the database
        ///// </summary>
        ///// <param name="Node">Node to save</param>
        //public void Write( CswNbtNode Node )
        //{
        //    //bz # 5943
        //    //_CswNbtNodeWriter.write( Node, true );
        //    Node.postChanges( true );
        //}
        ///// <summary>
        ///// Save the node to the database
        ///// </summary>
        ///// <param name="NodeId">Primary key of Node to save</param>
        //public void Write( Int32 NodeId )
        //{
        //    this.Write( GetNode( NodeId ) );
        //}
        ///// <summary>
        ///// Delete the node from the database and the Collection.  Don't try to do this while iterating.
        ///// NodeSpecies.Plain is assumed.
        ///// </summary>
        ///// <param name="NodeId">Primary Key of node to delete</param>
        //public void Delete( Int32 NodeId )
        //{
        //    Delete( NodeId, NodeSpecies.Plain );
        //}
        ///// <summary>
        ///// Delete the node from the database and the Collection.  Don't try to do this while iterating.
        ///// NodeSpecies.Plain is assumed.
        ///// </summary>
        ///// <param name="NodeId">Primary Key of node to delete</param>
        ///// <param name="Species"><see cref="NodeSpecies"/></param>
        //public void Delete( Int32 NodeId, NodeSpecies Species )
        //{
        //    CswNbtNode Node = GetNode( NodeId );

        //    //bz # 5943
        //    //_CswNbtNodeWriter.delete( Node );
        //    Node.delete();
        //    NodeHash.Remove( new NodeHashKey( NodeId, Species ) );
        //}

        /// <summary>
        /// Write all nodes in the collection
        /// </summary>
        /// <remarks>
        /// This might be problematic if we get into write loops
        /// </remarks>
        public void finalize()
        {
            //foreach( CswNbtNode Node in NodeHash.Values )
            //{

            //    //bz # 8342: we don't need to check this. 
            //    //if( NodeSpecies.Plain == Node.NodeSpecies &&
            //    //    NodeModificationState.Modified == Node.ModificationState )  //bz # 6653
            //    //{
            //    //    string ErrorStr = "The following node is in the collection but was never committed: ";
            //    //    ErrorStr += "Name=" + Node.NodeName;
            //    //    if( Node.NodeId != null )
            //    //        ErrorStr += ", Id=" + Node.NodeId.ToString();
            //    //    ErrorStr += ", UniqueID=" + Node.UniqueId;
            //    //    _CswNbtResources.CswLogger.reportError( new CswDniException( ErrorStr ) );
            //    //}
            //}

            //TODO: this is where we will execute an independent transaction to update the NodeCount column in nodetypes and object_class
            _updateNodeCounts();

            _clear(); //bz # 6653

        }//finalize()

        #endregion Database

        #region Node Counts

        public void IncrementNodeCounts( Int32 NodeTypeId )
        {
            if( NodeTypeCounts.ContainsKey( NodeTypeId ) )
            {
                NodeTypeCounts[NodeTypeId] += 1;
            }
            else
            {
                NodeTypeCounts.Add( NodeTypeId, 1 );
            }

            CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeId );
            if( null != ObjClass )
            {
                if( ObjClassCounts.ContainsKey( ObjClass.ObjectClassId ) )
                {
                    ObjClassCounts[ObjClass.ObjectClassId] += 1;
                }
                else
                {
                    ObjClassCounts.Add( ObjClass.ObjectClassId, 1 );
                }
            }
        }

        private void _updateNodeCounts()
        {
            // NOTE: This caused a table lock on newly inserted rows without the where clause.
            // Phil and Steve agree that if this function causes table locks in the future, it should
            // be removed in favor of a background task.

            string nodetypeSQL = "update nodetypes set nodecount = nodecount + case";
            bool DoUpdateNT = false;
            CswCommaDelimitedString ntInClause = new CswCommaDelimitedString();
            foreach( var Pair in NodeTypeCounts )
            {
                DoUpdateNT = true;
                nodetypeSQL += " when nodetypeid =  " + Pair.Key + " then " + Pair.Value;
                ntInClause.Add( Pair.Key.ToString() );
            }
            //nodetypeSQL += " else + 0 end ";
            nodetypeSQL += " end ";
            nodetypeSQL += " where nodetypeid in (" + ntInClause + ")";

            string objclassSQL = "update object_class set nodecount = nodecount + case";
            bool DoUpdateOC = false;
            CswCommaDelimitedString ocInClause = new CswCommaDelimitedString();
            foreach( var Pair in ObjClassCounts )
            {
                DoUpdateOC = true;
                objclassSQL += " when objectclassid =  " + Pair.Key + " then " + Pair.Value;
                ocInClause.Add( Pair.Key.ToString() );
            }
            // objclassSQL += "else + 0 end ";
            objclassSQL += " end ";
            objclassSQL += " where objectclassid in (" + ocInClause + ")";

            NodeTypeCounts.Clear();
            ObjClassCounts.Clear();

            if( DoUpdateNT )
            {
                _CswNbtResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( nodetypeSQL );
            }

            if( DoUpdateOC )
            {
                _CswNbtResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( objclassSQL );
            }

        }

        #endregion


    } // CswNbtNodeCollection()
} // namespace ChemSW.Nbt


