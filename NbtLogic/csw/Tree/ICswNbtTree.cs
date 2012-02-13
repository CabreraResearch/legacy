using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
//using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    enum TblRtrvlType { CreateNonExistent, ComplainAboutNonExistent };

    ///// <summary>
    ///// Audit Level definition
    ///// </summary>
    //public enum NbtAuditLevel
    //{
    //    /// <summary>
    //    /// No auditing
    //    /// </summary>
    //    None, 
    //    /// <summary>
    //    /// Records changes only
    //    /// </summary>
    //    Simple, 
    //    /// <summary>
    //    /// Records changes and a one-way encrypted hash of values
    //    /// </summary>
    //    Hashed, 
    //    /// <summary>
    //    /// Requires an electronic signature
    //    /// </summary>
    //    Esig
    //}

    /// <summary>
    /// Formats for Tree XML export
    /// </summary>
    public enum XmlTreeDestinationFormat
    {
        /// <summary>
        /// No conversion (results in error if conversion is attempted)
        /// </summary>
        None,
        /// <summary>
        /// Telerik RadTreeView compatible format
        /// </summary>
        TelerikRadTreeView,
        /// <summary>
        /// Telerik RadGrid compatible format
        /// </summary>
        TelerikRadGrid,
        /// <summary>
        /// Telerik RadGrid compatible format for grid properties
        /// </summary>
        TelerikRadGridProperty,
        /// <summary>
        /// Reporting compatible format
        /// </summary>
        ReportingDataSet,
        jsTree
    }

    /// <summary>
    /// Defines event arguments for events that occur during <see cref="ICswNbtTree.iterateTree"/>
    /// </summary>
    public class NodeVisitEventArgs : EventArgs
    {
        /// <summary>
        /// Filter event to only apply to nodes of a particular nodetype
        /// </summary>
        public Int32 NodeTypeIdToFilter;
        /// <summary>
        /// Filter event to only apply to nodes of a particular objectclass
        /// </summary>
        public Int32 ObjectClassIdToFilter;
    }//NodeVisitEventArgs 

    /// <summary>
    /// Event handler for events that occur during <see cref="ICswNbtTree.iterateTree"/>
    /// </summary>
    public delegate void CswNbtNodeVisitHandler( object VisitedNodeKey, NodeVisitEventArgs NodeVisitEventArgs );

    //public interface NodeAttributes
    //{
    //    string this[ string AttributeName ] { get; }
    //}//

    /// <summary>
    /// Represents the interface to an NBT Tree
    /// </summary>
    public interface ICswNbtTree
    {


        //NodeAttributes CurrentNodeAttributes { get;}

        //        void finalize();

        ///// <summary>
        ///// TreeKey which is used to index this tree.
        ///// </summary>
        CswNbtTreeKey Key { get; }

        /// <summary>
        /// Event to call when iterating nodes.  See <see cref="iterateTree" />.
        /// </summary>
        CswNbtNodeVisitHandler OnIterateNode { get; set; }

        /// <summary>
        /// Sets the Event arguments to use when iterating nodes
        /// </summary>
        NodeVisitEventArgs NodeVisitEventArgs { get; set; }

        /// <summary>
        /// Calls the OnIterateNode event on every node in the tree
        /// </summary>
        void iterateTree();

        ///// <summary>
        ///// Audit Level for tree
        ///// </summary>
        //NbtAuditLevel NbtAuditLevel { get; set; }

        /// <summary>
        /// View XML that was used to create this tree
        /// </summary>
        string SourceViewXml { get; set; }

        ///// <summary>
        ///// When converting the tree to XML, use this format
        ///// </summary>
        //XmlTreeDestinationFormat XmlTreeDestinationFormat { get; set; }

        //void pushCurrentKey();
        //void popKey();

        ///// <summary>
        ///// Converts the tree to an XML string
        ///// </summary>
        //string getTreeAsXml();

        /// <summary>
        /// Gets the Tree XML as it is stored internally
        /// </summary>
        JObject getRawTreeJSON();
        ///// <summary>
        ///// Sets the Tree XML, for copying trees
        ///// </summary>
        //void setRawTreeXml( XmlDocument XmlDoc );

        /// <summary>
        /// Name of view that created this tree.  Also name of root node of tree.
        /// </summary>
        string ViewName { get; }

        bool IsFullyPopulated { get; }


        /// <summary>
        /// Returns a CswNbtNode indexed by a CswNbtNodeKey
        /// </summary>
        /// <param name="CswNbtNodeKey">NodeKey index</param>
        CswNbtNode getNode( CswNbtNodeKey CswNbtNodeKey );

        /// <summary>
        /// Returns the root node of the tree
        /// </summary>
        CswNbtNode getRootNode();

        /// <summary>
        /// Return a node key for the first matching node in the tree
        /// </summary>
        /// <remarks>
        /// Candidate to refactor to CswNbtNodes
        /// </remarks>
        /// <param name="NodeId">Primary key of node</param>
        CswNbtNodeKey getNodeKeyByNodeId( CswPrimaryKey NodeId );

        /// <summary>
        /// Return a node key for the first matching node in the tree which derived from the given ViewNode
        /// </summary>
        /// <param name="NodeId">Primary key of node</param>
        /// <param name="NodeId">View Node</param>
        CswNbtNodeKey getNodeKeyByNodeIdAndViewNode( CswPrimaryKey NodeId, CswNbtViewNode ViewNode );

        /// <summary>
        /// Returns the currently indexed node
        /// </summary>
        CswNbtNode getNodeForCurrentPosition();


        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="ViewRoot">The corresponding ViewRoot node in the View</param>
        void makeRootNode( CswNbtViewRoot ViewRoot );
        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="IconFileName">Icon filename for root node</param>
        /// <param name="Selectable">True if the root is selectable, false otherwise</param>
        /// <param name="AddChildren">True if users can add children to the root, false otherwise</param>
        void makeRootNode( string IconFileName, bool Selectable, NbtViewAddChildrenSetting AddChildren );

        ///// <summary>
        ///// Creates a placeholder "More..." node on the tree.  Mostly used by TreeLoaders.
        ///// </summary>
        ///// <param name="ParentNodeKey">NodeKey of Parent Node</param>
        ///// <param name="Row">DataRow of real node on which the paging stopped</param>
        ///// <param name="NodeCount">Rowcount in results for this node</param>
        ///// <param name="ViewNode">Node of View which created this placeholder node and its siblings</param>
        //void makeMoreNodeFromRow( CswNbtNodeKey ParentNodeKey, DataRow Row, Int32 NodeCount, CswNbtViewNode ViewNode );

        /// <summary>
        /// Returns all node keys of nodes of a given Object Class
        /// </summary>
        /// <param name="ObjectClassId">Primary key of Object Class</param>
        Collection<CswPrimaryKey> getNodeKeysOfClass( Int32 ObjectClassId );

        /// <summary>
        /// Returns all node keys of nodes of a given NodeType
        /// </summary>
        /// <param name="NodeTypeId">Primary key of Node Type</param>
        /// <returns>Collection of Node Keys</returns>
        Collection<CswPrimaryKey> getNodeKeysOfNodeType( Int32 NodeTypeId );

        /// <summary>
        /// Returns all node keys of nodes of a given NodeType
        /// </summary>
        /// <param name="NodeTypeId">Primary key of Node Type</param>
        /// <returns>Collection of Node Keys</returns>
        JArray getChildNodePropsOfNode();

        //Navigation and interrogation methods*****************************************
        #region Navigation and interrogation methods
        /// <summary>
        /// Set the current index to the root of the tree
        /// </summary>
        void goToRoot();


        /// <summary>
        /// Set the current index to the Nth child
        /// </summary>
        /// <param name="ChildN">0-based Index of Child</param>
        void goToNthChild( int ChildN );

        /// <summary>
        /// Returns true if the current node is a child of the root node
        /// </summary>
        bool isCurrentNodeChildOfRoot();
        /// <summary>
        /// Returns true if the currently indexed node is the tree root
        /// </summary>
        bool isCurrentPositionRoot();

        ///// <summary>
        ///// Sets the current index to the currently indexed node's parent's next child
        ///// </summary>
        //void goToNextSibling();
        ///// <summary>
        ///// True if the currently indexed node's parent has a next child
        ///// </summary>
        //bool nextSiblingExists();

        ///// <summary>
        ///// Sets the current index to the currently indexed node's parent's previous child
        ///// </summary>
        //void goToPreviousSibling();

        ///// <summary>
        ///// True if the currently indexed node's parent has a previous child
        ///// </summary>
        //bool previousSiblingExists();

        /// <summary>
        /// Sets the current index to the currently indexed node's parent
        /// </summary>
        void goToParentNode();

        //        CswNbtNodeKey getNodeKeyForNodeRow( DataRow DataRow );

        /// <summary>
        /// Returns the NodeKey for the currently indexed node
        /// </summary>
        CswNbtNodeKey getNodeKeyForCurrentPosition();
        /// <summary>
        /// Returns the primary key of the currently indexed node
        /// </summary>
        CswPrimaryKey getNodeIdForCurrentPosition();
        /// <summary>
        /// Returns the Name of the currently indexed node
        /// </summary>
        String getNodeNameForCurrentPosition();
        /// <summary>
        /// Returns whether the currently indexed node is locked
        /// </summary>
        bool getNodeLockedForCurrentPosition();
        /// <summary>
        /// True if the currently indexed node is selectable, false otherwise
        /// </summary>
        bool getNodeSelectableForCurrentPosition();
        /// <summary>
        /// True if the currently indexed node should appear in the tree, false otherwise
        /// </summary>
        bool getNodeShowInTreeForCurrentPosition();

        /// <summary>
        /// Returns the NodeKey of the currently indexed node's parent
        /// </summary>
        CswNbtNodeKey getNodeKeyForParentOfCurrentPosition();

        ///// <summary>
        ///// Returns the matching NodeContext for the given NodeKey
        ///// </summary>
        //CswNbtNodeContext getNodeContextForNodeKey(CswNbtNodeKey NodeKey);

        /// <summary>
        /// Sets a given node to be the currently indexed node in the tree, by node key
        /// </summary>
        void makeNodeCurrent( CswNbtNodeKey CswNbtNodeKey );
        ///// <summary>
        ///// Sets a given node to be the currently indexed node in the tree, by path
        ///// </summary>
        //void makeNodeCurrent( CswDelimitedString TreePath );


        /// <summary>
        /// Returns true if there is a currently indexed node 
        /// </summary>
        bool isCurrentNodeDefined();

        /// <summary>
        /// Returns the total number of siblings of the currently indexed node
        /// </summary>
        int getNodeCountForCurrentLevel();

        ///// <summary>
        ///// Returns all siblings and cousins on a tree level
        ///// </summary>
        //Collection<CswNbtNodeKey> getKeysForLevel( Int32 Level );

        /// <summary>
        /// Returns the total number of children of the currently indexed node
        /// </summary>
        int getChildNodeCount();


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
        #region Modification Methods


        /// <summary>
        /// Adds a Child from a DataRow.  Used by TreeLoaders.
        /// </summary>
        /// <param name="DataRowToAdd">DataRow with node information</param>
        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
        /// <param name="Relationship">ViewRelationship node which caused this node to be added</param>
        /// <returns>Returns NodeKey index for node</returns>
        Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount );

        ///// <summary>
        ///// Add a node from the information in its key
        ///// </summary>
        ///// <param name="View">View from which the Tree is based</param>
        ///// <param name="Key">Key of Node to Add</param>
        //void addNodeFromKey(CswNbtView View, ref CswNbtNodeKey Key);

        /// <summary>
        /// Adds a Child from a DataRow.  Used by TreeLoaders.
        /// </summary>
        /// <param name="DataRowToAdd">DataRow with node information</param>
        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
        /// <param name="Selectable">True if the node is selectable, false otherwise</param>
        /// <param name="ShowInTree">True if the node should appear in a tree, false otherwise</param>
        /// <param name="AddChildren">True if the user should be allowed to add children to this node, false otherwise</param>
        /// <returns>Returns NodeKey index for node</returns>
        Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount );

        /// <summary>
        /// Sets the client-side expandmode of the current node
        /// </summary>
        void setCurrentNodeExpandMode( string ExpandMode );


        /// <summary>
        /// Sets whether the current node's children are truncated
        /// </summary>
        void setCurrentNodeChildrenTruncated( bool Truncated );
        
        /// <summary>
        /// True if the current node's children are truncated
        /// </summary>
        /// <returns></returns>
        bool getCurrentNodeChildrenTruncated();

        /// <summary>
        /// Adds a Property value to a node.  This is the uncommon way to fill property data in for nodes.
        /// </summary>
        /// <param name="NodeTypePropId">Primary key of property</param>
        /// <param name="Name">Name of Property</param>
        /// <param name="Gestalt">Text representation of the value of the property</param>
        /// <param name="FieldType">FieldType of the property</param>
        void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType FieldType );

        //CswNbtNodeKey loadNodeAsChildFromDb( System.Int32 NodeId, string GroupName );

        void removeCurrentNode();

        #endregion //Modification******************************

    }//ICswNbtTree

}//namespace ChemSW.Nbt

