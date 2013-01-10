using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Tree
    /// </summary>
    public class CswNbtTreeDomProxy : ICswNbtTree
    {
        private CswNbtTreeNodesWcf _CswNbtTreeNodesWcf = null;

        private CswNbtNodeCollection _CswNbtNodeCollection = null;

        CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Represents a Tree Modification Event
        /// </summary>
        /// <param name="CswNbtTreeDomProxy">Tree being modified</param>
        /// <param name="CswNbtTreeModEventArgs">Modification event arguments</param>
        public delegate void CswNbtTreeModificationHandler( object CswNbtTreeDomProxy, CswNbtTreeModEventArgs CswNbtTreeModEventArgs );
        /// <summary>
        /// Event occurs before inserting a new node in the tree
        /// </summary>
        public CswNbtTreeModificationHandler onBeforeInsertNode;
        /// <summary>
        /// Event occurs after inserting a new node in the tree
        /// </summary>
        public CswNbtTreeModificationHandler onAfterInsertNode;
        /// <summary>
        /// Event occurs before changing the parent of a node in the tree
        /// </summary>
        public CswNbtTreeModificationHandler onBeforeChangeParent;
        /// <summary>
        /// Event occurs after changing the parent of a node in the tree
        /// </summary>
        public CswNbtTreeModificationHandler onAfterChangeParent;
        /// <summary>
        /// Event occurs before deleting a node from the tree
        /// </summary>
        public CswNbtTreeModificationHandler onBeforeDeleteNode;
        /// <summary>
        /// Event occurs after deleting a node from the tree
        /// </summary>
        public CswNbtTreeModificationHandler onAfterDeleteNode;

        /// <summary>
        /// Name of view that created this tree.  Also name of root node of tree.
        /// </summary>
        public string ViewName
        {
            get
            {
                //return _CswNbtResources.ViewSelect.getSessionView( _Key.SessionViewId ).ViewName;
                string ret = string.Empty;
                if( View != null )
                {
                    ret = View.ViewName;
                }
                return ret;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="View">Tree View</param>
        /// <param name="CswNbtResources">The CswNbtResources object</param>
        /// <param name="CswNbtNodeWriter">A CswNbtNodeWriter object</param>
        /// <param name="CswNbtNodeCollection">A reference to the CswNbtNodeCollection</param>
        /// <param name="IsFullyPopulated"></param>
        public CswNbtTreeDomProxy( CswNbtView View, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection, bool IsFullyPopulated )
        {
            _View = View;
            _Key = new CswNbtTreeKey( _CswNbtResources, _View );

            _CswNbtResources = CswNbtResources;

            _CswNbtTreeNodesWcf = new CswNbtTreeNodesWcf( _Key, ViewName, _CswNbtResources, CswNbtNodeCollection );

            _CswNbtNodeCollection = CswNbtNodeCollection;

            _IsFullyPopulated = IsFullyPopulated;
        }//ctor

        private bool _IsFullyPopulated = false;
        public bool IsFullyPopulated
        {
            get
            {
                return _IsFullyPopulated;
            }
        }

        private CswNbtTreeKey _Key = null;
        /// <summary>
        /// TreeKey which is used to index this tree.
        /// </summary>
        public CswNbtTreeKey Key
        {
            get { return ( _Key ); }
        }

        private CswNbtView _View = null;
        /// <summary>
        /// View which was used to build this tree
        /// </summary>
        public CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
        }

        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="ViewRoot">The corresponding ViewRoot node in the View</param>
        public void makeRootNode( CswNbtViewRoot ViewRoot )
        {
            _CswNbtTreeNodesWcf.makeRootNode( ViewRoot.ViewName, ViewRoot );
        }


        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="IconFileName">Icon filename for root node</param>
        /// <param name="Selectable">True if the root is selectable, false otherwise</param>
        /// <param name="AddChildren">True if users can add children to the root, false otherwise</param>
        public void makeRootNode( string IconFileName, bool Selectable, NbtViewAddChildrenSetting AddChildren )
        {
            _CswNbtTreeNodesWcf.makeRootNode( ViewName, IconFileName, Selectable );//, AddChildren);
        }


        private string _SourceViewXml = "";
        /// <summary>
        /// View XML that was used to create this tree
        /// </summary>
        public string SourceViewXml
        {
            set
            {
                _SourceViewXml = value;
            }
            get
            {
                return _SourceViewXml;
            }
        }
        
        /// <summary>
        /// Returns a CswNbtNode indexed by a CswNbtNodeKey
        /// </summary>
        /// <param name="CswNbtNodeKey">NodeKey index</param>
        public CswNbtNode getNode( CswNbtNodeKey CswNbtNodeKey )
        {
            return _CswNbtNodeCollection.GetNode( CswNbtNodeKey.NodeId, CswNbtNodeKey.NodeTypeId, CswNbtNodeKey.NodeSpecies, DateTime.MinValue );
        }//getNode()

        /// <summary>
        /// Returns the root node of the tree
        /// </summary>
        public CswNbtNode getRootNode()
        {
            CswNbtNodeKey CurrentKey = getNodeKeyForCurrentPosition();
            goToRoot();
            CswNbtNode RootNode = getNode( getNodeKeyForCurrentPosition() );
            makeNodeCurrent( CurrentKey );
            return RootNode;
        }//getRootNode()

        /// <summary>
        /// Return a node key for the first matching node in the tree
        /// </summary>
        /// <remarks>
        /// Candidate to refactor to CswNbtNodes
        /// </remarks>
        /// <param name="NodeId">Primary key of node</param>
        public CswNbtNodeKey getNodeKeyByNodeId( CswPrimaryKey NodeId )
        {
            CswNbtNodeKey ReturnVal = null;
            Collection<CswNbtNodeKey> KeyList = _CswNbtTreeNodesWcf.getKeysForNodeId( NodeId );
            if( null != KeyList && KeyList.Count > 0 )
            {
                ReturnVal = KeyList[0];
                //ReturnVal.TreeKey = Key;
            }
            return ( ReturnVal );
        }//getNodeKeyByNodeId()

        public Collection<CswNbtNodeKey> getNodeKeysByNodeIdAndViewNode( CswPrimaryKey NodeId, CswNbtViewNode ViewNode )
        {
            return _CswNbtTreeNodesWcf.getNodeKeysByNodeIdAndViewNode( NodeId, ViewNode );
        } // getNodeKeysByNodeIdAndViewNode()

        /// <summary>
        /// Returns the currently indexed node
        /// </summary>
        public CswNbtNode getNodeForCurrentPosition()
        {
            return getNode( getNodeKeyForCurrentPosition() );
        }//getNodeForCurrentPosition()



        //Navigation and interrogation methods*****************************************
        #region Navigation and interrogation methods

        /// <summary>
        /// Set the current index to the root of the tree
        /// </summary>
        public void goToRoot()
        {
            _CswNbtTreeNodesWcf.goToRoot();
        }//goToRoot()

        /// <summary>
        /// Set the current index to the Nth child
        /// </summary>
        /// <param name="ChildN">0-based Index of Child</param>
        public void goToNthChild( int ChildN )
        {
            _CswNbtTreeNodesWcf.goToNthChild( ChildN );
        }//goToNthChild() 

        /// <summary>
        /// Returns true if the current node is a child of the root node
        /// </summary>
        public bool isCurrentNodeChildOfRoot()
        {
            return ( _CswNbtTreeNodesWcf.isCurrentNodeChildOfRoot() );

        }//isCurrentNodeRoot()

        /// <summary>
        /// Returns true if the currently indexed node is the tree root
        /// </summary>
        public bool isCurrentPositionRoot()
        {
            return ( _CswNbtTreeNodesWcf.isCurrentPositionRoot() );

        }//isCurrentNodeRoot()

        /// <summary>
        /// Sets the current index to the currently indexed node's parent
        /// </summary>
        public void goToParentNode()
        {
            _CswNbtTreeNodesWcf.goToParentNode();
        }//goToParentNode()

        /// <summary>
        /// Returns the NodeKey for the currently indexed node
        /// </summary>
        public CswNbtNodeKey getNodeKeyForCurrentPosition()
        {
            CswNbtNodeKey ReturnVal = _CswNbtTreeNodesWcf.getKeyForCurrentNode();
            //ReturnVal.TreeKey = Key;
            return ( ReturnVal );

        }//getNodeKeyForCurrentPosition()

        /// <summary>
        /// Returns the primary key of the currently indexed node
        /// </summary>
        public CswPrimaryKey getNodeIdForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getIdForCurrentNode();
        }

        /// <summary>
        /// Returns the Name of the currently indexed node
        /// </summary>
        public string getNodeNameForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getNameForCurrentNode();
        }//getNodeNameForCurrentPosition()

        /// <summary>
        /// Returns whether the currently indexed node is locked
        /// </summary>
        public bool getNodeLockedForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getLockedForCurrentNode();
        }//getNodeLockedForCurrentPosition()

        /// <summary>
        /// Returns whether the currently indexed node is included (for filters with ResultMode.Disabled)
        /// </summary>
        public bool getNodeIncludedForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getIncludedForCurrentNode();
        }//getNodeIncludedForCurrentPosition()

        /// <summary>
        /// True if the currently indexed node is selectable, false otherwise
        /// </summary>
        public bool getNodeSelectableForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getSelectableForCurrentNode();
        }//getNodeSelectableForCurrentPosition()
        /// <summary>
        /// True if the currently indexed node is selectable, false otherwise
        /// </summary>
        public bool getNodeShowInTreeForCurrentPosition()
        {
            return _CswNbtTreeNodesWcf.getNodeShowInTreeForCurrentNode();
        }//getNodeShowInTreeForCurrentPosition()

        /// <summary>
        /// Returns the NodeKey of the currently indexed node's parent
        /// </summary>
        public CswNbtNodeKey getNodeKeyForParentOfCurrentPosition()
        {
            CswNbtNodeKey ReturnVal = _CswNbtTreeNodesWcf.getNodeKeyForParentOfCurrentPosition();
            return ( ReturnVal );
        }

        [DebuggerStepThrough]
        public Collection<CswNbtTreeNodeProp> getChildNodePropsOfNode()
        {
            return _CswNbtTreeNodesWcf.getChildPropNodesOfCurrentNode();
        }

        /// <summary>
        /// Sets a given node to be the currently indexed node in the tree, by node key
        /// </summary>
        /// <param name="CswNbtNodeKey">NodeKey representing the node</param>
        public void makeNodeCurrent( CswNbtNodeKey CswNbtNodeKey )
        {
            _CswNbtTreeNodesWcf.makeNodeCurrent( CswNbtNodeKey );
        }//makeNodeCurrent() 
        /// <summary>
        /// Sets a given node to be the currently indexed node in the tree, by node id
        /// </summary>
        /// <param name="CswPrimaryKey">NodeId representing the node</param>
        public void makeNodeCurrent( CswPrimaryKey NodeId )
        {
            _CswNbtTreeNodesWcf.makeNodeCurrent( NodeId );
        }//makeNodeCurrent() 

        /// <summary>
        /// Returns true if there is a currently indexed node 
        /// </summary>
        public bool isCurrentNodeDefined()
        {
            return ( _CswNbtTreeNodesWcf.isCurrentNodeDefined() );
        }//isCurrentNodeDefined() 

        /// <summary>
        /// Returns the total number of siblings of the currently indexed node
        /// </summary>
        public int getNodeCountForCurrentLevel()
        {
            return ( _CswNbtTreeNodesWcf.getNodeCountForCurrentLevel() );
        }//getNodeCountForCurrentLevel()

        /// <summary>
        /// Returns the total number of children of the currently indexed node
        /// </summary>
        public int getChildNodeCount()
        {
            return ( _CswNbtTreeNodesWcf.getChildNodeCount() );
        }//getChildNodeCount() 

        private void _collectNodesOfClass( object VisitedNodeKey, NodeVisitEventArgs NodeVisitEventArgs )
        {
            CswNbtNodeKey CswNbtNodeKey = (CswNbtNodeKey) VisitedNodeKey;
            if( CswNbtNodeKey.ObjectClassId == NodeVisitEventArgs.ObjectClassIdToFilter )
            {
                _NodesOfClass.Add( CswNbtNodeKey.NodeId );
            }//

        }//_collectNodesOfClass

        private Collection<CswPrimaryKey> _NodesOfClass = null;

        /// <summary>
        /// Returns all node keys of nodes of a given Object Class
        /// </summary>
        /// <param name="ObjectClassId">Primary key of Object Class</param>
        public Collection<CswPrimaryKey> getNodeKeysOfClass( Int32 ObjectClassId )
        {
            _NodesOfClass = new Collection<CswPrimaryKey>();

            OnIterateNode = new CswNbtNodeVisitHandler( _collectNodesOfClass );
            NodeVisitEventArgs = new NodeVisitEventArgs();
            NodeVisitEventArgs.ObjectClassIdToFilter = ObjectClassId;

            iterateTree();

            return ( _NodesOfClass );

        }//getNodeKeysOfClass()


        private void _collectNodesOfNodeType( object VisitedNodeKey, NodeVisitEventArgs NodeVisitEventArgs )
        {
            CswNbtNodeKey CswNbtNodeKey = (CswNbtNodeKey) VisitedNodeKey;
            if( CswNbtNodeKey.NodeTypeId == NodeVisitEventArgs.NodeTypeIdToFilter )
            {
                _NodesOfNodeType.Add( CswNbtNodeKey.NodeId );
            }//

        }//_collectNodesOfNodeType

        private Collection<CswPrimaryKey> _NodesOfNodeType = null;

        /// <summary>
        /// Returns all node keys of nodes of a given NodeType
        /// </summary>
        /// <param name="NodeTypeId">Primary key of Node Type</param>
        public Collection<CswPrimaryKey> getNodeKeysOfNodeType( Int32 NodeTypeId )
        {
            _NodesOfNodeType = new Collection<CswPrimaryKey>();

            OnIterateNode = new CswNbtNodeVisitHandler( _collectNodesOfNodeType );
            NodeVisitEventArgs = new NodeVisitEventArgs();
            NodeVisitEventArgs.NodeTypeIdToFilter = NodeTypeId;

            iterateTree();

            return ( _NodesOfNodeType );

        }//getNodeKeysOfNodeType()



        /// <summary>
        /// Calls the OnIterateNode event on every node in the tree
        /// </summary>
        public void iterateTree()
        {
            if( null == OnIterateNode )
            {    throw ( new CswDniException( "OnIterateNode must be set before calling iterateTree()" ) );}

            //cache current position
            CswNbtNodeKey CurrentPosition = getNodeKeyForCurrentPosition();

            goToRoot();
            _iterateNodes();

            //restore current position
            makeNodeCurrent( CurrentPosition );

        }//iterateTree()

        private NodeVisitEventArgs _NodeVisitEventArgs = null;

        /// <summary>
        /// Sets the Event arguments to use when iterating nodes
        /// </summary>
        public NodeVisitEventArgs NodeVisitEventArgs
        {
            set
            {
                _NodeVisitEventArgs = value;
            }

            get
            {
                return ( _NodeVisitEventArgs );
            }

        }//NodeVisitEventArgs

        private CswNbtNodeVisitHandler _OnIterateNode = null;

        /// <summary>
        /// Event to call when iterating nodes.  See <see cref="iterateTree" />.
        /// </summary>
        public CswNbtNodeVisitHandler OnIterateNode
        {
            get
            {
                return ( _OnIterateNode );
            }//
            set
            {
                _OnIterateNode = value;
            }//
        }//

        private void _iterateNodes()
        {
            OnIterateNode( getNodeKeyForCurrentPosition(), NodeVisitEventArgs );
            int TotalChilren = getChildNodeCount();
            if( TotalChilren > 0 )
            {
                for( int ChildIdx = 0; ChildIdx < TotalChilren; ChildIdx++ )
                {
                    goToNthChild( ChildIdx );
                    _iterateNodes();
                    goToParentNode();
                }//go through all children
            }

        }//_iterateChildren()


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
        #region Modification Methods

        /// <summary>
        /// Adds a Child from a DataRow.  Used by TreeLoaders.
        /// </summary>
        /// <param name="ParentNodeKey">Parent Node Key (for path generation)</param>
        /// <param name="DataRowToAdd">DataRow with node information</param>
        /// <param name="UseGrouping">Whether grouping nodes</param>
        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
        /// <param name="Relationship">ViewRelationship node which caused this node to be added</param>
        /// <param name="RowCount">Row number in view results</param>
        /// <param name="Included"></param>
        /// <returns>Returns NodeKey index for node</returns>
        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount, bool Included = true )
        {
            Collection<CswNbtNodeKey> ReturnVal = _CswNbtTreeNodesWcf.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Relationship, RowCount, Included );
            //ReturnVal.TreeKey = Key;
            //_TreeAsTransformedXml = "";
            return ( ReturnVal );
        }//loadNodeAsChildFromRow() 

        /// <summary>
        /// Adds a Child from a DataRow.  Used by TreeLoaders.
        /// </summary>
        /// <param name="ParentNodeKey">Parent Node Key (for path generation)</param>
        /// <param name="DataRowToAdd">DataRow with node information</param>
        /// <param name="UseGrouping">Whether grouping nodes</param>
        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
        /// <param name="Selectable">True if the node is selectable, false otherwise</param>
        /// <param name="ShowInTree"></param>
        /// <param name="AddChildren">True if the user should be allowed to add children to this node, false otherwise</param>
        /// <param name="RowCount">Row number in view results</param>
        /// <param name="Included"></param>
        /// <returns>Returns NodeKey index for node</returns>
        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included = true )
        {
            Collection<CswNbtNodeKey> ReturnVal = _CswNbtTreeNodesWcf.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Selectable, ShowInTree, AddChildren, RowCount, Included );
            return ( ReturnVal );
        }//loadNodeAsChildFromRow() 

        /// <summary>
        /// Sets the client-side expandmode of the current node
        /// </summary>
        public void setCurrentNodeExpandMode( string ExpandMode )
        {
            _CswNbtTreeNodesWcf.setCurrentNodeExpandMode( ExpandMode );
        }

        /// <summary>
        /// Adds a Property value to a node.  This is the uncommon way to fill property data in for nodes.
        /// </summary>
        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType.NbtFieldType FieldType, string Field1, string Field2, Int32 Field1_Fk, double Field1_Numeric, bool Hidden )
        {
            _CswNbtTreeNodesWcf.addProperty( NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType, Field1, Field2, Field1_Fk, Field1_Numeric, Hidden );
        }//addProperty


        public Collection<CswNbtNodeKey> _loadNodeAsChild( CswNbtNodeKey ParentNodeKey, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship,
                                               bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included,
                                               string IconFileName, string NameTemplate, CswPrimaryKey NodeId, string NodeName, Int32 NodeTypeId,
                                               string NodeTypeName, Int32 ObjectClassId, string ObjectClassName, bool Locked )
        {
            return _CswNbtTreeNodesWcf._loadNodeAsChild( ParentNodeKey, UseGrouping, GroupName, Relationship,
                                                       Selectable, ShowInTree, AddChildren, RowCount, Included,
                                                       IconFileName, NameTemplate, NodeId, NodeName, NodeTypeId,
                                                       NodeTypeName, ObjectClassId, ObjectClassName, Locked );
        }


        public void setCurrentNodeChildrenTruncated( bool Truncated )
        {
            _CswNbtTreeNodesWcf.setCurrentNodeChildrenTruncated( Truncated );
        }
        public bool getCurrentNodeChildrenTruncated()
        {
            return _CswNbtTreeNodesWcf.getCurrentNodeChildrenTruncated();
        }

        public void removeCurrentNode()
        {
            _CswNbtTreeNodesWcf.removeCurrentNode();
        }

        #endregion //Modification******************************


    }//class CswNbtTreeDomProxy

}//namespace ChemSW.Nbt
