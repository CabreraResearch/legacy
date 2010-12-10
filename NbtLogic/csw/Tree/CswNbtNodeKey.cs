using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    //public enum NodeSource { UnKnown, InCurrentTree, InProspectiveTree, NonTree };

    /// <summary>
    /// Uniquely identifies a node instance on a Tree
    /// </summary>
    [Serializable()]
    public class CswNbtNodeKey : System.IEquatable<CswNbtNodeKey>
    {
        private CswNbtResources _CswNbtResources;
        /// <summary>
        /// Use this constructor to get a blank Key
        /// </summary>
        public CswNbtNodeKey( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        //private CswNbtNodeKey _ParentKey = null;
        //public CswNbtNodeKey ParentKey { get { return _ParentKey; } set { _ParentKey = value; } }
        


        //public CswNbtNodeKey( NodeSource NodeSource, CswNbtTreeKey CswNbtTreeKey, string TreePath, Int32 NodeId, Int32 NodeTypeId, NodeSpecies NodeSpecies )
        //public CswNbtNodeKey(CswNbtResources CswNbtResources, CswNbtTreeKey CswNbtTreeKey, string TreePath, Int32 NodeId, Int32 NodeTypeId, Int32 ObjectClassId, NodeSpecies NodeSpecies, CswNbtViewNode ViewNode )
        /// <summary>
        /// Use this constructor to initialize the tree at creation
        /// </summary>
        public CswNbtNodeKey( CswNbtResources CswNbtResources, CswNbtTreeKey CswNbtTreeKey, string TreePath, CswPrimaryKey NodeId, NodeSpecies NodeSpecies, Int32 NodeTypeId, Int32 ObjectClassId, string ViewNodeUniqueId, string NodeCountPath ) //, CswNbtNodeKey ParentKey )
        {
            _CswNbtResources = CswNbtResources;
            _TreePath = TreePath;
            _CswNbtTreeKey = CswNbtTreeKey;
            _NodeId = NodeId;
            _NodeSpecies = NodeSpecies;
            _NodeTypeId = NodeTypeId;
            _ObjectClassId = ObjectClassId;
            _ViewNodeUniqueId = ViewNodeUniqueId;
            _NodeCountPath = NodeCountPath;
            //_ParentKey = ParentKey;
        }

        private static char delimiter = '^';
        /// <summary>
        /// Convert the NodeKey information into a delimited string
        /// </summary>
        public override string ToString()
        {
            string ret = "";
            ret += TreePath + delimiter;
            if(NodeId != null)
                ret += NodeId.ToString() + delimiter;
            else
                ret += delimiter;
            ret += NodeSpecies.ToString() + delimiter;
            ret += NodeTypeId.ToString() + delimiter;
            ret += ObjectClassId.ToString() + delimiter;
            if (TreeKey != null)
                ret += TreeKey.ToString() + delimiter;
            else
                ret += delimiter;
            ret += ViewNodeUniqueId.ToString() + delimiter;
            ret += NodeCountPath.ToString();
            return ret;
        }

        /// <summary>
        /// Convert the NodeKey information into a Javascript-safe delimited string
        /// </summary>
        public string ToJavaScriptParam()
        {
            return this.ToString().Replace( "'", @"\'" );
        }

        /// <summary>
        /// Returns the TreePath to the parent node
        /// </summary>
        public string getParentTreePath()
        {
            return TreePath.Substring( 0, TreePath.LastIndexOf( '/' ) );
        }


        /// <summary>
        /// Use this constructor to convert the key from the string representation of a key
        /// </summary>
        public CswNbtNodeKey( CswNbtResources CswNbtResources, string StringKey )
        {
            _CswNbtResources = CswNbtResources;

            if( StringKey == string.Empty )
                throw new CswDniException( "Misconfigured Tree", "CswNbtNodeKey.constructor(string) encountered a null StringKey" );
            else
            {
                string[] Values = StringKey.Split( delimiter );

                //if( 5 > Values.Length )
                //    throw ( new CswDniException( "The specified node key does not subdivide into five segments: " + StringKey ) );


                _TreePath = Values[0].ToString();
                _NodeId = new CswPrimaryKey();
                if (Values[1] != string.Empty)
                    _NodeId.FromString( Values[1].ToString() );

                string NodeSpeciesString = Values[2].ToString();
                try
                {
                    _NodeSpecies = (NodeSpecies) Enum.Parse( typeof( NodeSpecies ), NodeSpeciesString, true );
                }
                catch( ArgumentException ex )
                {
                    throw new CswDniException( "Misconfigured Tree",
                                              "CswNbtNodeKey.constructor(string) encountered an invalid NodeSpecies: " + NodeSpeciesString,
                                              ex );
                }
                _NodeTypeId = Convert.ToInt32( Values[3] );
                _ObjectClassId = Convert.ToInt32( Values[4] );
                if( Values[5].ToString() != string.Empty )
                    _CswNbtTreeKey = new CswNbtTreeKey( _CswNbtResources, Convert.ToInt32( Values[5].ToString() ) );
                else
                    _CswNbtTreeKey = null;
                _ViewNodeUniqueId = Values[6].ToString();
                _NodeCountPath = Values[7].ToString();
            }
        }//CswNbtNodeKey()

        private string _TreePath = "";
        /// <summary>
        /// Path from root of tree to this NodeKey
        /// </summary>
        public string TreePath
        {
            get { return ( _TreePath ); }
            set
            {
                _TreePath = value;
                _TreeDepth = Int32.MinValue;
            }
        }

        /// <summary>
        /// Returns the species of the node at a given depth in the path of parents
        /// </summary>
        public NodeSpecies TreePathNodeSpecies( Int32 Depth )
        {
            NodeSpecies ret = NodeSpecies.UnKnown;
            if( Depth >= 0 )
            {
                string[] Path = TreePath.Split( '/' );
                if( Path[Depth + 2].Substring( 0, CswNbtTreeNodes._ElemName_NodeGroup.Length ) == CswNbtTreeNodes._ElemName_NodeGroup )
                    ret = NodeSpecies.Group;
                else if( Path[Depth + 2].Substring( 0, CswNbtTreeNodes._ElemName_Node.Length ) == CswNbtTreeNodes._ElemName_Node )
                    ret = NodeSpecies.Plain;
            }
            return ret;
        }

        /// <summary>
        /// Returns the primary key of the node at a given depth in the path of parents
        /// </summary>
        public CswPrimaryKey TreePathNodeId( Int32 Depth )
        {
            CswPrimaryKey ret = null;
            if( Depth >= 0 )
            {
                string[] Path = TreePath.Split( '/' );
                string NodeStr = Path[Depth + 2];
                string IdStr1 = NodeStr.Substring( NodeStr.IndexOf( "@tablename='" ) + "@tablename='".Length );
                IdStr1 = IdStr1.Substring( 0, IdStr1.IndexOf("'") );
                string IdStr2 = NodeStr.Substring( NodeStr.IndexOf( "@nodeid=" ) + "@nodeid=".Length );
                IdStr2 = IdStr2.Substring( 0, IdStr2.Length - "]".Length );
                if( CswTools.IsInteger( IdStr2 ) )
                    ret = new CswPrimaryKey( IdStr1, Convert.ToInt32( IdStr2 ) );
            }
            return ret;
        }
        
        /// <summary>
        /// Returns the name of the group of the node at a given depth in the path of parents
        /// </summary>
        public string TreePathGroupName( Int32 Depth )
        {
            string ret = string.Empty;
            if( Depth >= 0 )
            {
                string[] Path = TreePath.Split( '/' );
                string NameStr = Path[Depth + 2].Split( '=' )[1];
                ret = NameStr.Substring( 1, NameStr.Length - 2 );
            }
            return ret;
        }

        /// <summary>
        /// The depth of this node on the tree
        /// </summary>
        private Int32 _TreeDepth = Int32.MinValue;
        public Int32 TreeDepth
        {
            get
            {
                if (_TreeDepth == Int32.MinValue)
                {
                    if( TreePath != string.Empty )
                    {
                        _TreeDepth = -2;
                        Int32 pos = 0;
                        while( pos >= 0 )
                        {
                            _TreeDepth++;
                            pos = TreePath.IndexOf( "/", pos + 1 );
                            if( _TreeDepth > 50 ) break;   // runaway loop guard
                        }
                    }
                }
                return _TreeDepth;
            }
        }

        private CswNbtTreeKey _CswNbtTreeKey;
        /// <summary>
        /// Identifier for Tree in which this NodeKey is valid
        /// </summary>
        public CswNbtTreeKey TreeKey
        {
            set { _CswNbtTreeKey = value; }
            get { return _CswNbtTreeKey; }
        }

        private CswPrimaryKey _NodeId = null;
        /// <summary>
        /// Primary Key of node
        /// </summary>
        public CswPrimaryKey NodeId
        {
            get { return ( _NodeId ); }
            set { _NodeId = value; }
        }

        private Int32 _NodeTypeId = Int32.MinValue;
        /// <summary>
        /// NodeType Primary Key of Node
        /// </summary>
        public Int32 NodeTypeId
        {
            get { return ( _NodeTypeId ); }
            set { _NodeTypeId = value; }
        }

        private Int32 _ObjectClassId = Int32.MinValue;
        /// <summary>
        /// ObjectClass Primary Key of Node
        /// </summary>
        public Int32 ObjectClassId
        {
            get { return ( _ObjectClassId ); }
            set { _ObjectClassId = value; }
        }

        private NodeSpecies _NodeSpecies = NodeSpecies.Plain;
        /// <summary>
        /// <see cref="NodeSpecies"/> of Node
        /// </summary>
        public NodeSpecies NodeSpecies
        {
            get { return ( _NodeSpecies ); }
            set { _NodeSpecies = value; }
        }


        private string _ViewNodeUniqueId = string.Empty;
        /// <summary>
        /// UniqueId of ViewNode that created this node
        /// </summary>
        public string ViewNodeUniqueId
        {
            get { return _ViewNodeUniqueId; }
            set { _ViewNodeUniqueId = value; }
        }

        private Int32 _NodeCount = Int32.MinValue;
        /// <summary>
        /// Sibling count of this node
        /// </summary>
        public Int32 NodeCount
        {
            get
            {
                if(_NodeCount == Int32.MinValue)
                    _NodeCount = getNodeCountAtDepth(TreeDepth);
                return _NodeCount;
            }
        }


        private string _NodeCountPath = string.Empty;
        /// <summary>
        /// Sibling count of this node
        /// </summary>
        public string NodeCountPath
        {
            get { return _NodeCountPath; }
            set { _NodeCountPath = value; }
        }

        /// <summary>
        /// Get the sibling count at a particular depth
        /// </summary>
        /// <param name="TreeDepth">Depth for count (1 is top level)</param>
        public Int32 getNodeCountAtDepth(Int32 TreeDepth)
        {
            if (_NodeCountPath != string.Empty)
            {
                string[] SplitPath = _NodeCountPath.Split('/');
                return Convert.ToInt32(SplitPath[TreeDepth]);
            }
            else
            {
                return Int32.MinValue;
            }
        }

        #region IEquatable
        /// <summary>
        /// IEquatable implementation: ==
        /// </summary>
        public static bool operator ==( CswNbtNodeKey key1, CswNbtNodeKey key2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( key1, key2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( ( object ) key1 == null ) || ( ( object ) key2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( ( key1.TreeKey == key2.TreeKey ) &&
                ( key1.TreePath == key2.TreePath ) &&
                ( key1.NodeId == key2.NodeId ) &&
                ( key1.NodeSpecies == key2.NodeSpecies ) )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable implementation: !=
        /// </summary>
        public static bool operator !=( CswNbtNodeKey key1, CswNbtNodeKey key2 )
        {
            return !( key1 == key2 );
        }

        /// <summary>
        /// IEquatable implementation: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtNodeKey ) )
                return false;
            return this == ( CswNbtNodeKey ) obj;
        }

        /// <summary>
        /// IEquatable implementation: Equals
        /// </summary>
        public bool Equals( CswNbtNodeKey obj )
        {
            return this == ( CswNbtNodeKey ) obj;
        }

        /// <summary>
        /// IEquatable implementation: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            Int32 ret = Int32.MinValue;
            if( NodeId != null )
                ret = NodeId.PrimaryKey;
            return ret;
        }

        #endregion IEquatable

    }//CswNbtNodeKey

}//namespace ChemSW.Nbt
