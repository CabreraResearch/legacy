//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Exceptions;
//using ChemSW.Nbt.ObjClasses;

//namespace ChemSW.Nbt
//{
//    /// <summary>
//    /// Stores information about a node in the context of its tree
//    /// </summary>
//    public class CswNbtNodeContext : System.IEquatable<CswNbtNodeContext>
//    {
//        private CswNbtResources _CswNbtResources;
//        /// <summary>
//        /// Use this constructor to specify all the parameters of the NodeContext on creation
//        /// </summary>
//        public CswNbtNodeContext(CswNbtResources CswNbtResources, CswNbtTreeKey CswNbtTreeKey, string TreePath, Int32 NodeId, Int32 NodeTypeId, Int32 ObjectClassId, NodeSpecies NodeSpecies, CswNbtViewNode ViewNode, Int32 NodeCount)
//        {
//            _CswNbtResources = CswNbtResources;
//            _TreePath = TreePath;
//            _CswNbtTreeKey = CswNbtTreeKey;
//            _NodeId = NodeId;
//            _NodeTypeId = NodeTypeId;
//            _ObjectClassId = ObjectClassId;
//            _NodeSpecies = NodeSpecies;
//            _ViewNode = ViewNode;
//            _NodeCount = NodeCount;
//        }

//        /// <summary>
//        /// Convert to a CswNbtNodeKey
//        /// </summary>
//        public CswNbtNodeKey ToKey()
//        {
//            return new CswNbtNodeKey(_CswNbtResources, _CswNbtTreeKey, _TreePath, _NodeId, _NodeSpecies, NodeTypeId, _ObjectClassId, _ViewNode.UniqueId, NodeCount);
//        }

//        private CswNbtNodeContext _ParentNodeContext = null;
//        /// <summary>
//        /// Parent NodeContext on the Tree
//        /// </summary>
//        public CswNbtNodeContext ParentNodeContext
//        {
//            get { return (_ParentNodeContext); }
//            set { _ParentNodeContext = value; }
//        }

//        private string _TreePath = "";
//        /// <summary>
//        /// Path from root of tree to this NodeContext
//        /// </summary>
//        public string TreePath
//        {
//            get { return (_TreePath); }
//            set { _TreePath = value; }
//        }

//        private CswNbtTreeKey _CswNbtTreeKey;
//        /// <summary>
//        /// Tree identifier in which this NodeContext is valid
//        /// </summary>
//        public CswNbtTreeKey TreeKey
//        {
//            set { _CswNbtTreeKey = value; }
//            get { return _CswNbtTreeKey; }
//        }

//        private Int32 _NodeId = Int32.MinValue;
//        /// <summary>
//        /// Primary Key of node
//        /// </summary>
//        public Int32 NodeId
//        {
//            get { return (_NodeId); }
//            set { _NodeId = value; }
//        }

//        private Int32 _NodeTypeId = Int32.MinValue;
//        /// <summary>
//        /// NodeType Primary Key of Node
//        /// </summary>
//        public Int32 NodeTypeId
//        {
//            get { return (_NodeTypeId); }
//            set { _NodeTypeId = value; }
//        }

//        private Int32 _ObjectClassId = Int32.MinValue;
//        /// <summary>
//        /// ObjectClass Primary Key of Node
//        /// </summary>
//        public Int32 ObjectClassId
//        {
//            get { return (_ObjectClassId); }
//            set { _ObjectClassId = value; }
//        }//ObjectClassId

        
//        private NodeSpecies _NodeSpecies = NodeSpecies.Plain;
//        /// <summary>
//        /// <see cref="NodeSpecies"/> of Node
//        /// </summary>
//        public NodeSpecies NodeSpecies
//        {
//            get { return (_NodeSpecies); }
//            set { _NodeSpecies = value; }
//        }//NodeSpecies


//        // This is attached to the key when the node is created from a view
//        private CswNbtViewNode _ViewNode;
//        /// <summary>
//        /// ViewNode in the CswNbtView from which this Node was created
//        /// </summary>
//        public CswNbtViewNode ViewNode
//        {
//            get { return _ViewNode; }
//            set { _ViewNode = value; }
//        }

//        private Int32 _NodeCount;
//        /// <summary>
//        /// Sibling count in tree
//        /// </summary>
//        public Int32 NodeCount
//        {
//            get { return _NodeCount; }
//            set { _NodeCount = value; }
//        }


//        #region IEquatable

//        /// <summary>
//        /// IEquatable implementation: ==
//        /// </summary>
//        public static bool operator ==(CswNbtNodeContext key1, CswNbtNodeContext key2)
//        {
//            // If both are null, or both are same instance, return true.
//            if (System.Object.ReferenceEquals(key1, key2))
//            {
//                return true;
//            }

//            // If one is null, but not both, return false.
//            if (((object)key1 == null) || ((object)key2 == null))
//            {
//                return false;
//            }

//            // Now we know neither are null.  Compare values.
//            if ((key1.TreeKey == key2.TreeKey) &&
//                (key1.TreePath == key2.TreePath) &&
//                (key1.NodeId == key2.NodeId) &&
//                (key1.NodeTypeId == key2.NodeTypeId) &&
//                (key1.NodeSpecies == key2.NodeSpecies))
//                return true;
//            else
//                return false;
//        }

//        /// <summary>
//        /// IEquatable implementation: !=
//        /// </summary>
//        public static bool operator !=(CswNbtNodeContext key1, CswNbtNodeContext key2)
//        {
//            return !(key1 == key2);
//        }

//        /// <summary>
//        /// IEquatable implementation: Equals
//        /// </summary>
//        public override bool Equals(object obj)
//        {
//            if (!(obj is CswNbtNodeContext))
//                return false;
//            return this == (CswNbtNodeContext)obj;
//        }

//        /// <summary>
//        /// IEquatable implementation: Equals
//        /// </summary>
//        public bool Equals(CswNbtNodeContext obj)
//        {
//            return this == (CswNbtNodeContext)obj;
//        }

//        /// <summary>
//        /// IEquatable implementation: GetHashCode
//        /// </summary>
//        public override int GetHashCode()
//        {
//            return this.NodeId;
//        }
        
//        #endregion IEquatable

//    }//CswNbtNodeContext 

//}//namespace ChemSW.Nbt
