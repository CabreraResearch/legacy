using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

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
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode this[CswPrimaryKey NodeId]
        {
            get { return GetNode( NodeId, DateTime.MinValue ); }
        }

        /// <summary>
        /// Index of nodes by NodeId.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
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
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node</param>
        public CswNbtNode GetNode( CswPrimaryKey NodeId )
        {
            return GetNode( NodeId, Int32.MinValue, NodeSpecies.Plain, DateTime.MinValue );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeTypeId is looked up and NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
        /// </summary>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, DateTime Date )
        {
            return GetNode( NodeId, Int32.MinValue, NodeSpecies.Plain, Date );
        }

        /// <summary>
        /// Fetch a node from the collection.  NodeSpecies.Plain is assumed.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeId">Primary Key of Node (if not provided, make sure NodeTypeId is)</param>
        /// <param name="NodeTypeId">Primary Key of NodeTypeId (only required if NodeId is invalid)</param>
        /// <seealso cref="this[CswPrimaryKey]"/>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId )
        {
            return GetNode( NodeId, NodeTypeId, NodeSpecies.Plain, DateTime.MinValue );
        }

        /// <summary>
        /// Index of nodes by NodeKey.  The NodeId, NodeTypeId and NodeSpecies in the Key are used.  See <see cref="GetNode(CswPrimaryKey, int, NodeSpecies, DateTime)"/>
        /// </summary>
        /// <param name="NodeKey">NodeKey for Node</param>
        public CswNbtNode this[CswNbtNodeKey NodeKey]
        {
            get
            {
                if( NodeKey == null )
                    throw new CswDniException( ErrorType.Error, "Invalid Node", "CswNbtNodeCollection received a null NodeKey" );
                if( NodeKey.NodeSpecies != NodeSpecies.Plain )
                    throw new CswDniException( ErrorType.Error, "Invalid Node", "CswNbtNodeCollection cannot fetch Node of species " + NodeKey.NodeSpecies.ToString() );
                return GetNode( NodeKey.NodeId, NodeKey.NodeTypeId, NodeKey.NodeSpecies, DateTime.MinValue );
            }
        }

        /// <summary>
        /// Fetch a node from the collection.  If the node isn't loaded from the database already, it will be.
        /// NodeTypeId is only required if you don't provide a good NodeId (so that we can still fetch Property info).
        /// </summary>
        /// <param name="NodeId">Primary Key of Node (if not provided, make sure NodeTypeId is)</param>
        /// <param name="NodeTypeId">Primary Key of NodeTypeId (only required if NodeId is invalid)</param>
        /// <param name="Species"><see cref="NodeSpecies" /></param>
        /// <param name="Date"></param>
        public CswNbtNode GetNode( CswPrimaryKey NodeId, Int32 NodeTypeId, NodeSpecies Species, DateTime Date )
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
                if( SubField.RelationalTable == string.Empty )
                {
                    SQLQuery += " (select nodeid, 'nodes' tablename ";
                    SQLQuery += "    from jct_nodes_props ";
                    SQLQuery += "   where nodetypepropid = " + MetaDataProp.PropId.ToString() + " ";
                    SQLQuery += "     and " + SubField.Column.ToString() + " = '" + PropWrapper.GetPropRowValue( SubField.Column ) + "') ";
                }
                else
                {
                    string PrimeKeyCol = _CswNbtResources.DataDictionary.getPrimeKeyColumn( SubField.RelationalTable );
                    SQLQuery += " (select " + PrimeKeyCol + " nodeid, '" + SubField.RelationalTable + "' tablename ";
                    SQLQuery += "    from " + SubField.RelationalTable + " ";
                    SQLQuery += "   where " + SubField.RelationalColumn + " = '" + PropWrapper.GetPropRowValue( SubField.Column ) + "') ";
                }
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
            public NodeSpecies Species;
            public NodeHashKey( CswPrimaryKey TheNodeId, NodeSpecies TheSpecies )
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
            CswNbtNode Node = _CswNbtNodeFactory.make( NodeSpecies.Plain, HashKey.NodeId, NodeTypeId, NodeHash.Count );

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

                Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
                Node.OnRequestDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( OnAfterDeleteNode );
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

        private void OnAfterSetNodeIdHandler( CswNbtNode Node, CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId )
        {
            NodeHash.Remove( new NodeHashKey( OldNodeId, Node.NodeSpecies ) );
            NodeHash.Add( new NodeHashKey( NewNodeId, Node.NodeSpecies ), Node );
        }

        private void OnAfterDeleteNode( CswNbtNode Node )
        {
            NodeHash.Remove( new NodeHashKey( Node.NodeId, Node.NodeSpecies ) );
        }


        /// <summary>
        /// Specifies operation to take on database when using makeNodeFromNodeTypeId
        /// </summary>
        public sealed class MakeNodeOperation : IEquatable<MakeNodeOperation>
        {

            #region Enum Member

            /// <summary>
            /// Write the new node to the database
            /// </summary>
            public const string WriteNode = "WriteNode";

            /// <summary>
            /// Just set the primary key and the default property values, but make
            /// no changes to the database
            /// </summary>
            public const string JustSetPk = "JustSetPk";

            /// <summary>
            /// Just get an empty node with meta data from the nodetype
            /// </summary>
            public const string DoNothing = "DoNothing";

            /// <summary>
            /// Write the new temporary node to the database.
            /// </summary>
            public const string MakeTemp = "MakeTemp";
            #endregion

            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { WriteNode, WriteNode },
                                                                       { JustSetPk, JustSetPk },
                                                                       { DoNothing, DoNothing },
                                                                       { MakeTemp, MakeTemp }
                                                                   };
            /// <summary>
            /// Instance value
            /// </summary>
            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            /// <summary>
            /// Constructor
            /// </summary>
            public MakeNodeOperation( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            /// <summary>
            /// Implicit enum cast
            /// </summary>
            public static implicit operator MakeNodeOperation( string Val )
            {
                return new MakeNodeOperation( Val );
            }

            /// <summary>
            /// Implicit string cast
            /// </summary>
            public static implicit operator string( MakeNodeOperation item )
            {
                return item.Value;
            }

            /// <summary>
            /// ToString
            /// </summary>
            public override string ToString()
            {
                return Value;
            }

            #region IEquatable (MakeNodeOperation)

            /// <summary>
            /// ==
            /// </summary>
            public static bool operator ==( MakeNodeOperation ft1, MakeNodeOperation ft2 )
            {
                //do a string comparison on the fieldtypes
                return ft1.ToString() == ft2.ToString();
            }

            /// <summary>
            /// !=
            /// </summary>
            public static bool operator !=( MakeNodeOperation ft1, MakeNodeOperation ft2 )
            {
                return !( ft1 == ft2 );
            }

            /// <summary>
            /// Equals
            /// </summary>
            public override bool Equals( object obj )
            {
                if( !( obj is MakeNodeOperation ) )
                    return false;
                return this == (MakeNodeOperation) obj;
            }

            /// <summary>
            /// Equals
            /// </summary>
            public bool Equals( MakeNodeOperation obj )
            {
                return this == obj;
            }

            /// <summary>
            /// Get Hash Code
            /// </summary>
            public override int GetHashCode()
            {
                int ret = 23, prime = 37;
                ret = ( ret * prime ) + Value.GetHashCode();
                ret = ( ret * prime ) + _Enums.GetHashCode();
                return ret;
            }

            #endregion IEquatable (MakeNodeOperation)

        }

        /// <summary>
        /// Create a new, fresh, empty Node from a node type.  Properties are filled in, but Property Values are not.
        /// </summary>
        /// <param name="NodeTypeId">Primary Key of Nodetype</param>
        /// <param name="Op">Specifies the action to take with regard to the database</param>
        /// <param name="OverrideUniqueValidation"></param>
        public CswNbtNode makeNodeFromNodeTypeId( Int32 NodeTypeId, MakeNodeOperation Op, bool OverrideUniqueValidation = false )
        {
            CswNbtNode Node = _CswNbtNodeFactory.make( NodeSpecies.Plain, null, NodeTypeId, NodeHash.Count );
            Node.OnAfterSetNodeId += new CswNbtNode.OnSetNodeIdHandler( OnAfterSetNodeIdHandler );
            Node.OnRequestDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( OnAfterDeleteNode );
            Node.fillFromNodeTypeId( NodeTypeId );
            Node.IsTemp = MakeNodeOperation.MakeTemp == Op;

            switch( Op )
            {
                case MakeNodeOperation.WriteNode:
                case MakeNodeOperation.MakeTemp:
                    _CswNbtNodeFactory.CswNbtNodeWriter.setDefaultPropertyValues( Node );
                    Node.postChanges( true, false, OverrideUniqueValidation );
                    break;
                case MakeNodeOperation.JustSetPk:
                    _CswNbtNodeFactory.CswNbtNodeWriter.makeNewNodeEntry( Node, false, false, OverrideUniqueValidation );
                    //_CswNbtNodeFactory.CswNbtNodeWriter.setDefaultPropertyValues( Node );    
                    break;
                case MakeNodeOperation.DoNothing:
                    //right now there are only three enum values; I'm just making this explicit
                    _CswNbtNodeFactory.CswNbtNodeWriter.setDefaultPropertyValues( Node );
                    break;
            }

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

            CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserName_ObjectClassProp = User_ObjectClass.getObjectClassProp( CswNbtObjClassUser.PropertyName.Username );

            _CswNbtResources.logTimerResult( "makeUserNodeFromUsername 1", Timer );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtNodes.makeUserNodeFromUsername(" + Username + ")";
            CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );
            CswNbtViewProperty Prop = View.AddViewProperty( UserRelationship, UserName_ObjectClassProp );
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, Username, false );

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

            CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp RoleName_ObjectClassProp = Role_ObjectClass.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtNodes.makeRoleNodeFromRoleName(" + RoleName + ")";
            CswNbtViewRelationship RoleRelationship = View.AddViewRelationship( Role_ObjectClass, false );
            CswNbtViewProperty Prop = View.AddViewProperty( RoleRelationship, RoleName_ObjectClassProp );
            CswNbtViewPropertyFilter Filter = View.AddViewPropertyFilter( Prop, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Equals, RoleName, false );

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

            _clear(); //bz # 6653

        }//finalize()

        #endregion Database

    } // CswNbtNodeCollection()
} // namespace ChemSW.Nbt


