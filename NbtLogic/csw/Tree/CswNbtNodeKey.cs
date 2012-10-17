using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

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
        public static char delimiter = '-';
        public static char NodeCountDelimiter = '/';
        //public static char TreePathDelimiter = '/';
        private CswDelimitedString _DelimitedString = new CswDelimitedString( delimiter );

        /// <summary>
        /// Use this constructor to get a blank Key
        /// </summary>
        public CswNbtNodeKey( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Use this constructor to initialize the tree at creation
        /// </summary>
        //public CswNbtNodeKey( CswNbtResources CswNbtResources, CswNbtTreeKey inCswNbtTreeKey, string inTreePath, CswPrimaryKey inNodeId, NodeSpecies inNodeSpecies, Int32 inNodeTypeId, Int32 inObjectClassId, string inViewNodeUniqueId, string inNodeCountPath )
        public CswNbtNodeKey( CswNbtResources CswNbtResources, CswNbtTreeKey inCswNbtTreeKey, CswPrimaryKey inNodeId, NodeSpecies inNodeSpecies, Int32 inNodeTypeId, Int32 inObjectClassId, string inViewNodeUniqueId, string inNodeCountPath )
        {
            _CswNbtResources = CswNbtResources;
            //TreePath.FromString( inTreePath );
            TreeKey = inCswNbtTreeKey;
            NodeId = inNodeId;
            NodeSpecies = inNodeSpecies;
            NodeTypeId = inNodeTypeId;
            ObjectClassId = inObjectClassId;
            ViewNodeUniqueId = inViewNodeUniqueId;
            NodeCountPath.FromString( inNodeCountPath );
        }

        /// <summary>
        /// Convert the NodeKey information into a delimited string
        /// </summary>
        public override string ToString()
        {
            return _DelimitedString.ToString();
        }

        ///// <summary>
        ///// Convert the NodeKey information into a Javascript-safe delimited string
        ///// </summary>
        //public string ToJavaScriptParam()
        //{
        //    return ToString().Replace( @"\", @"\\" ).Replace( "'", @"\'" );
        //}

        ///// <summary>
        ///// Returns the TreePath to the parent node
        ///// </summary>
        //public CswDelimitedString getParentTreePath()
        //{
        //    return TreePath.SubString( 0, TreePath.Count - 2 );
        //}

        /// <summary>
        /// Use this constructor to convert the key from the string representation of a key
        /// </summary>
        public CswNbtNodeKey( CswNbtResources CswNbtResources, string StringKey )
        {
            _CswNbtResources = CswNbtResources;

            if( StringKey == string.Empty )
                throw new CswDniException( ErrorType.Error, "Misconfigured Tree", "CswNbtNodeKey.constructor(string) encountered a null StringKey" );

            _DelimitedString.FromString( StringKey );
        }//CswNbtNodeKey()

        //private CswDelimitedString _TreePath = null;
        ///// <summary>
        ///// Path from root of tree to this NodeKey
        ///// </summary>
        //public CswDelimitedString TreePath
        //{
        //    get
        //    {
        //        if( _TreePath == null )
        //        {
        //            _TreePath = new CswDelimitedString( TreePathDelimiter );
        //            _TreePath.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _TreePath_OnChange );
        //            _TreePath.FromString( _DelimitedString[0] );
        //        }
        //        return _TreePath;
        //    }
        //    set
        //    {
        //        _TreePath = value;
        //        _TreePath_OnChange();
        //    }
        //}

        //void _TreePath_OnChange()
        //{
        //    _DelimitedString[0] = _TreePath.ToString();
        //}

        ///// <summary>
        ///// Returns the species of the node at a given depth in the path of parents
        ///// </summary>
        //public NodeSpecies TreePathNodeSpecies( Int32 Depth )
        //{
        //    NodeSpecies ret = NodeSpecies.UnKnown;
        //    if( Depth >= 0 )
        //    {
        //        if( TreePath[Depth + 1].Contains( CswNbtTreeNodes._ElemName_NodeGroup ) )
        //            ret = NodeSpecies.Group;
        //        else if( TreePath[Depth + 1].Contains( CswNbtTreeNodes._ElemName_Node ) )
        //            ret = NodeSpecies.Plain;
        //    }
        //    return ret;
        //}

        ///// <summary>
        ///// Returns the primary key of the node at a given depth in the path of parents
        ///// </summary>
        //public CswPrimaryKey TreePathNodeId( Int32 Depth )
        //{
        //    CswPrimaryKey ret = null;
        //    if( Depth >= 0 )
        //    {
        //        string NodeStr = TreePath[Depth + 1];
        //        if( NodeStr.Length > ( NodeStr.IndexOf( "@tablename='" ) + "@tablename='".Length ) )
        //        {
        //            string IdStr1 = NodeStr.Substring( NodeStr.IndexOf( "@tablename='" ) + "@tablename='".Length );
        //            IdStr1 = IdStr1.Substring( 0, IdStr1.IndexOf( "'" ) );
        //            string IdStr2 = NodeStr.Substring( NodeStr.IndexOf( "@nodeid=" ) + "@nodeid=".Length );
        //            IdStr2 = IdStr2.Substring( 0, IdStr2.Length - "]".Length );
        //            if( CswTools.IsInteger( IdStr2 ) )
        //                ret = new CswPrimaryKey( IdStr1, CswConvert.ToInt32( IdStr2 ) );
        //        }
        //    }
        //    return ret;
        //}

        ///// <summary>
        ///// Returns the name of the group of the node at a given depth in the path of parents
        ///// </summary>
        //public string TreePathGroupName( Int32 Depth )
        //{
        //    string ret = string.Empty;
        //    if( Depth >= 0 )
        //    {
        //        string NameStr = TreePath[Depth + 1].Split( '=' )[1];
        //        ret = NameStr.Substring( 1, NameStr.Length - 2 );
        //    }
        //    return ret;
        //}

        /// <summary>
        /// The depth of this node on the tree
        /// </summary>
        public Int32 TreeDepth
        {
            get { return _NodeCountPath.Count - 2; }
        }

        private CswNbtTreeKey _CswNbtTreeKey = null;
        /// <summary>
        /// Identifier for Tree in which this NodeKey is valid
        /// </summary>
        public CswNbtTreeKey TreeKey
        {
            get
            {
                if( _CswNbtTreeKey == null )
                {
                    if( string.Empty != _DelimitedString[5] )
                        _CswNbtTreeKey = new CswNbtTreeKey( _CswNbtResources, _DelimitedString[5] );
                }
                return _CswNbtTreeKey;
            }
            set
            {
                _CswNbtTreeKey = value;
                if( null != _CswNbtTreeKey )
                    _DelimitedString[5] = value.ToString();
                else
                    _DelimitedString[5] = string.Empty;
            }
        }

        private CswPrimaryKey _NodeId = null;
        /// <summary>
        /// Primary Key of node
        /// </summary>
        public CswPrimaryKey NodeId
        {
            get
            {
                if( _NodeId == null )
                {
                    if( _DelimitedString[1] != string.Empty )
                    {
                        _NodeId = new CswPrimaryKey();
                        _NodeId.FromString( _DelimitedString[1] );
                    }
                }
                return ( _NodeId );
            }
            set
            {
                _NodeId = value;
                if( null != _NodeId )
                    _DelimitedString[1] = _NodeId.ToString();
                else
                    _DelimitedString[1] = String.Empty;
            }
        }

        /// <summary>
        /// NodeType Primary Key of Node
        /// </summary>
        public Int32 NodeTypeId
        {
            get
            {
                return ( CswConvert.ToInt32( _DelimitedString[3] ) );
            }
            set
            {
                _DelimitedString[3] = value.ToString();
            }
        }

        /// <summary>
        /// ObjectClass Primary Key of Node
        /// </summary>
        public Int32 ObjectClassId
        {
            get
            {
                return ( CswConvert.ToInt32( _DelimitedString[4] ) );
            }
            set
            {
                _DelimitedString[4] = value.ToString();
            }
        }

        /// <summary>
        /// <see cref="NodeSpecies"/> of Node
        /// </summary>
        public NodeSpecies NodeSpecies
        {
            get
            {
                NodeSpecies ret = _DelimitedString[2];
                if( ret == CswNbtResources.UnknownEnum )
                {
                    ret = ObjClasses.NodeSpecies.Plain;
                }
                return ret;
            }
            set
            {
                _DelimitedString[2] = value.ToString();
            }
        }

        /// <summary>
        /// UniqueId of ViewNode that created this node
        /// </summary>
        public string ViewNodeUniqueId
        {
            get { return _DelimitedString[6]; }
            set { _DelimitedString[6] = value; }
        }

        /// <summary>
        /// Sibling count of this node
        /// </summary>
        public Int32 NodeCount
        {
            get
            {
                return getNodeCountAtDepth( TreeDepth );
            }
        }

        private CswDelimitedString _NodeCountPath = null;
        /// <summary>
        /// Sibling count of this node
        /// </summary>
        public CswDelimitedString NodeCountPath
        {
            get
            {
                if( _NodeCountPath == null )
                {
                    _NodeCountPath = new CswDelimitedString( NodeCountDelimiter );
                    _NodeCountPath.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _NodeCountPath_OnChange );
                    _NodeCountPath.FromString( _DelimitedString[7] );
                }
                return _NodeCountPath;
            }
            set
            {
                _NodeCountPath = value;
                _NodeCountPath.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _NodeCountPath_OnChange );
                _NodeCountPath_OnChange();
            }
        }

        void _NodeCountPath_OnChange()
        {
            if( null != _NodeCountPath )
                _DelimitedString[7] = _NodeCountPath.ToString();
            else
                _DelimitedString[7] = String.Empty;
        }

        /// <summary>
        /// Get the sibling count at a particular depth
        /// </summary>
        /// <param name="TreeDepth">Depth for count (1 is top level)</param>
        public Int32 getNodeCountAtDepth( Int32 TreeDepth )
        {
            return CswConvert.ToInt32( NodeCountPath[TreeDepth - 1] );
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
            if( ( (object) key1 == null ) || ( (object) key2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( //( key1.TreeKey == key2.TreeKey ) &&
                //( key1.TreePath == key2.TreePath ) &&
                ( key1.NodeCountPath == key2.NodeCountPath ) &&
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
            return this == (CswNbtNodeKey) obj;
        }

        /// <summary>
        /// IEquatable implementation: Equals
        /// </summary>
        public bool Equals( CswNbtNodeKey obj )
        {
            return this == (CswNbtNodeKey) obj;
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
