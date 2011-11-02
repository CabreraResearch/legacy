using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
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
        //private CswNbtTreePermissions _CswNbtTreePermissions = null;
        //        private CswNbtNodeCatalogue _CswNbtNodeCatalogue = new CswNbtNodeCatalogue();
        //        private CswNbtNodeFactory _CswNbtNodeFactory = null;
        private CswNbtTreeNodes _CswNbtTreeNodes = null;
        //private CswNbtNodes _CswNbtNodes = null;

        private CswNbtNodeCollection _CswNbtNodeCollection = null;

        //        private CswNbtNodeKey _CurrentParentNodeKey = null;
        //        CswNbtNodeKey _CswNbtNodeKeyCurrent = null;
        private string _XslFilePath = "";
        CswNbtResources _CswNbtResources = null;

        //CswNbtTreeEventLoader _CswNbtTreeEventLoader = new CswNbtTreeEventLoader();
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

        private CswNbtNodeReader _CswNbtNodeReader = null;
        private CswNbtNodeWriter _CswNbtNodeWriter = null;

        //        private ArrayList _RetrievedNodes = new ArrayList();


        /// <summary>
        /// Name of view that created this tree.  Also name of root node of tree.
        /// </summary>
        public string ViewName
        {
            get
            {
                //return _CswNbtResources.ViewSelect.getSessionView( _Key.SessionViewId ).ViewName;
                return View.ViewName;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CswNbtTreeKey">TreeKey which indexes this tree</param>
        /// <param name="CswNbtResources">The CswNbtResources object</param>
        /// <param name="XslFilePath">File path to XSL used to translate the tree to XML</param>
        /// <param name="CswNbtNodeWriter">A CswNbtNodeWriter object</param>
        /// <param name="CswNbtNodeCollection">A reference to the CswNbtNodeCollection</param>
        public CswNbtTreeDomProxy( //CswNbtTreeKey CswNbtTreeKey, 
                                   CswNbtView View, CswNbtResources CswNbtResources, CswNbtNodeWriter CswNbtNodeWriter, CswNbtNodeCollection CswNbtNodeCollection, bool IsFullyPopulated )
        {
            _View = View;
            _Key = new CswNbtTreeKey( _CswNbtResources, _View );
            //            CswNbtResources.CswLogger.reportTraceInfo( "CswNbtTreeDomProxy", "ctor", "entered" );

            _CswNbtNodeReader = new CswNbtNodeReader( CswNbtResources );
            //            _CswNbtNodeWriter = new CswNbtNodeWriter( CswNbtResources );
            _CswNbtNodeWriter = CswNbtNodeWriter;

            _CswNbtResources = CswNbtResources;
            //_CswNbtTreePermissions = new CswNbtTreePermissions( _CswNbtResources );
            //            _CswNbtNodeFactory = CswNbtResources.makeCswNbtNodeFactory();

            _XslFilePath = CswTools.getConfigurationFilePath( CswNbtResources.SetupVbls.SetupMode );
            _CswNbtTreeNodes = new CswNbtTreeNodes( _Key, _XslFilePath, ViewName, _CswNbtResources, CswNbtNodeCollection );

            //Not required for imported rows
            //_ColsToValidateForImportRow.Add(_CswNbtColumnNames.ParentNodeId );


            //            onBeforeInsertNode += new CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleBeforeInsertNode );
            //            onAfterInsertNode += new CswNbtTreeModificationHandler( CswNbtTreeEventInsertNodeGeneric.handleAfterInsertNode );

            //_CurrentNodeAttributes = new CswNbtTreeDomProxyAttributes( this );

            //_CswNbtNodes = CswNbtNodes;
            _CswNbtNodeCollection = CswNbtNodeCollection;
            //_CswNbtNodes.PathBasedNodes[_Key] = new CswNbtPathBasedNodes(_Key, _CswNbtTreeNodes);

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
            get { return ( _View ); }
            set { _View = value; }
        }

        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="ViewRoot">The corresponding ViewRoot node in the View</param>
        public void makeRootNode( CswNbtViewRoot ViewRoot )
        {
            _CswNbtTreeNodes.makeRootNode( ViewRoot.ViewName, ViewRoot );
        }

        /// <summary>
        /// Creates a placeholder "More..." node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        public void makeMoreNodeFromRow( CswNbtNodeKey ParentNodeKey, DataRow Row, Int32 NodeCount, CswNbtViewNode ViewNode )
        {
            _CswNbtTreeNodes.makeMoreNodeFromRow( ParentNodeKey, Row, NodeCount, ViewNode );
        }

        /// <summary>
        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
        /// </summary>
        /// <param name="IconFileName">Icon filename for root node</param>
        /// <param name="Selectable">True if the root is selectable, false otherwise</param>
        /// <param name="AddChildren">True if users can add children to the root, false otherwise</param>
        public void makeRootNode( string IconFileName, bool Selectable, NbtViewAddChildrenSetting AddChildren )
        {
            _CswNbtTreeNodes.makeRootNode( ViewName, IconFileName, Selectable );//, AddChildren);
        }

        /*
        public void finalize()
        {
            
            foreach( CswNbtNode CurrentNode in _NodesByNodeId.Values )
            {
                _CswNbtNodeWriter.write( CurrentNode );
            }//iterate retrieved nodes

            _NodesByNodeId.Clear();

        }//finalize()
         */

        //private NbtAuditLevel _NbtAuditLevel = NbtAuditLevel.None;
        ///// <summary>
        ///// Audit Level for tree
        ///// </summary>
        //public NbtAuditLevel NbtAuditLevel
        //{
        //    set
        //    {
        //        _NbtAuditLevel = value;
        //    }//

        //    get
        //    {
        //        return ( _NbtAuditLevel );
        //    }//
        //}//NbtAuditLevel

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


        private XmlTreeDestinationFormat _NLevelDsXmlMode = XmlTreeDestinationFormat.TelerikRadTreeView;
        /// <summary>
        /// When converting the tree to XML, use this format
        /// </summary>
        public XmlTreeDestinationFormat XmlTreeDestinationFormat
        {
            set
            {
                _NLevelDsXmlMode = value;
                _TreeAsTransformedXml = string.Empty;   // if we change the format, clear the cached tree
            }//

            get
            {
                return ( _NLevelDsXmlMode );
            }//
        }//

        //private Stack _NodeKeyStack = new Stack();
        //public void pushCurrentKey()
        //{
        //    _NodeKeyStack.Push( getNodeKeyForCurrentPosition() );
        //}//

        //public void popKey()
        //{
        //    makeNodeCurrent( _NodeKeyStack.Pop() as CswNbtNodeKey );
        //}//

        private CswXmlTransformer _CswXmlTransformer = new CswXmlTransformer();

        private string _TreeAsTransformedXml = "";
        /// <summary>
        /// Converts the tree to an XML string
        /// </summary>

        public string getTreeAsXml()
        {
            if( string.Empty == _TreeAsTransformedXml )
            {
                if( XmlTreeDestinationFormat.None == XmlTreeDestinationFormat )
                    throw ( new System.Exception( "There is no XmlTreeDestinationFormat" ) );

                string XslFileName = XmlTreeDestinationFormat.ToString() + ".xsl";
                string XslFileNameFqn = _XslFilePath + "\\" + XslFileName;

                _CswXmlTransformer.XslDocPath = XslFileNameFqn;
                _CswXmlTransformer.SourceDoc = getRawTreeXml();
                _TreeAsTransformedXml = _CswXmlTransformer.OutputDoc;
            }

            return ( _TreeAsTransformedXml );

        }//getTreeAsXml()

        private string _TreeAsRawXml = "";
        /// <summary>
        /// Gets the Tree XML as it is stored internally
        /// </summary>
        public string getRawTreeXml()
        {
            if( string.Empty == _TreeAsRawXml )
            {
                _TreeAsRawXml = _CswNbtTreeNodes.getRawXml();
            }

            return ( _TreeAsRawXml );

        }//getRawTreeXml()

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

            ArrayList KeyList = (ArrayList) _CswNbtTreeNodes.getKeysForNodeId( NodeId );
            if( KeyList.Count > 0 )
            {
                ReturnVal = (CswNbtNodeKey) KeyList[0];
                //ReturnVal.TreeKey = Key;
            }

            return ( ReturnVal );

        }//getNodeKeyByNodeId()


        //public CswNbtNodeContext getNodeContextForNodeKey(CswNbtNodeKey NodeKey)
        //{
        //    return _CswNbtTreeNodes.getNodeContext(NodeKey);
        //}

        /*
        public CswNbtNode getNode( Int32 NodeId )
        {
            CswNbtNode ReturnVal = null;

            ArrayList NodeKeys = new ArrayList();
            _CswNbtNodeCatalogue.getKeysForNodeId( NodeId, ref NodeKeys );

            if( NodeKeys.Count > 0 )
            {
                ReturnVal = getNode( (CswNbtNodeKey) NodeKeys[0] );
            }//if we have any node keys

            return ( ReturnVal );

        }//getNode() 
         */

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
            _CswNbtTreeNodes.goToRoot();
        }//goToRoot()

        /// <summary>
        /// Set the current index to the Nth child
        /// </summary>
        /// <param name="ChildN">0-based Index of Child</param>
        public void goToNthChild( int ChildN )
        {
            _CswNbtTreeNodes.goToNthChild( ChildN );
        }//goToNthChild() 

        /// <summary>
        /// Returns true if the current node is a child of the root node
        /// </summary>
        public bool isCurrentNodeChildOfRoot()
        {
            return ( _CswNbtTreeNodes.isCurrentNodeChildOfRoot() );

        }//isCurrentNodeRoot()

        /// <summary>
        /// Returns true if the currently indexed node is the tree root
        /// </summary>
        public bool isCurrentPositionRoot()
        {
            return ( _CswNbtTreeNodes.isCurrentPositionRoot() );

        }//isCurrentNodeRoot()

        /// <summary>
        /// Sets the current index to the currently indexed node's parent's next child
        /// </summary>
        public void goToNextSibling()
        {
            _CswNbtTreeNodes.goToNextSibling();
        }//goToNextSibling()

        /// <summary>
        /// True if the currently indexed node's parent has a next child
        /// </summary>
        public bool nextSiblingExists()
        {
            return ( _CswNbtTreeNodes.nextSiblingExists() );

        }//nextSiblingExists()

        /// <summary>
        /// Sets the current index to the currently indexed node's parent's previous child
        /// </summary>
        public void goToPreviousSibling()
        {
            _CswNbtTreeNodes.goToPreviousSibling();
        }//goToPreviousSibling()

        /// <summary>
        /// True if the currently indexed node's parent has a previous child
        /// </summary>
        public bool previousSiblingExists()
        {

            return ( _CswNbtTreeNodes.previousSiblingExists() );

        }//PreviousSiblingExists()

        /// <summary>
        /// Sets the current index to the currently indexed node's parent
        /// </summary>
        public void goToParentNode()
        {
            _CswNbtTreeNodes.goToParentNode();
        }//goToParentNode()

        /// <summary>
        /// Returns the NodeKey for the currently indexed node
        /// </summary>
        public CswNbtNodeKey getNodeKeyForCurrentPosition()
        {
            CswNbtNodeKey ReturnVal = _CswNbtTreeNodes.getKeyForCurrentNode();
            //ReturnVal.TreeKey = Key;
            return ( ReturnVal );

        }//getNodeKeyForCurrentPosition()

        /// <summary>
        /// Returns the primary key of the currently indexed node
        /// </summary>
        public CswPrimaryKey getNodeIdForCurrentPosition()
        {
            return _CswNbtTreeNodes.getIdForCurrentNode();
        }

        /// <summary>
        /// Returns the Name of the currently indexed node
        /// </summary>
        public string getNodeNameForCurrentPosition()
        {
            return _CswNbtTreeNodes.getNameForCurrentNode();
        }//getNodeNameForCurrentPosition()
    
		/// <summary>
		/// Returns whether the currently indexed node is locked
		/// </summary>
		public bool getNodeLockedForCurrentPosition()
        {
			return _CswNbtTreeNodes.getLockedForCurrentNode();
        }//getNodeLockedForCurrentPosition()

		/// <summary>
        /// True if the currently indexed node is selectable, false otherwise
        /// </summary>
        public bool getNodeSelectableForCurrentPosition()
        {
            return _CswNbtTreeNodes.getSelectableForCurrentNode();
        }//getNodeSelectableForCurrentPosition()
        /// <summary>
        /// True if the currently indexed node is selectable, false otherwise
        /// </summary>
        public bool getNodeShowInTreeForCurrentPosition()
        {
            return _CswNbtTreeNodes.getNodeShowInTreeForCurrentNode();
        }//getNodeShowInTreeForCurrentPosition()

        /// <summary>
        /// Returns the NodeKey of the currently indexed node's parent
        /// </summary>
        public CswNbtNodeKey getNodeKeyForParentOfCurrentPosition()
        {
            CswNbtNodeKey ReturnVal = _CswNbtTreeNodes.getNodeKeyForParentOfCurrentPosition();
            //ReturnVal.TreeKey = Key;

            return ( ReturnVal );
        }//

        /// <summary>
        /// Sets a given node to be the currently indexed node in the tree, by node key
        /// </summary>
        /// <param name="CswNbtNodeKey">NodeKey representing the node</param>
        public void makeNodeCurrent( CswNbtNodeKey CswNbtNodeKey )
        {
            _CswNbtTreeNodes.makeNodeCurrent( CswNbtNodeKey );
        }//makeNodeCurrent() 

        /// <summary>
        /// Sets a given node to be the currently indexed node in the tree, by path
        /// </summary>
        public void makeNodeCurrent( CswDelimitedString TreePath )
        {
            _CswNbtTreeNodes.makeNodeCurrent( TreePath );
        }//makeNodeCurrent() 

        /// <summary>
        /// Returns true if there is a currently indexed node 
        /// </summary>
        public bool isCurrentNodeDefined()
        {

            return ( _CswNbtTreeNodes.isCurrentNodeDefined() );

        }//isCurrentNodeDefined() 

        /// <summary>
        /// Returns the total number of siblings of the currently indexed node
        /// </summary>
        public int getNodeCountForCurrentLevel()
        {
            return ( _CswNbtTreeNodes.getNodeCountForCurrentLevel() );
        }//getNodeCountForCurrentLevel()

        /// <summary>
        /// Returns the total number of children of the currently indexed node
        /// </summary>
        public int getChildNodeCount()
        {
            return ( _CswNbtTreeNodes.getChildNodeCount() );
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
                throw ( new CswDniException( "OnIterateNode must be set before calling iterateTree()" ) );

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
        /// <returns>Returns NodeKey index for node</returns>
        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount )
        {
            Collection<CswNbtNodeKey> ReturnVal = _CswNbtTreeNodes.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Relationship, RowCount );
            //ReturnVal.TreeKey = Key;
            _TreeAsTransformedXml = "";
            _TreeAsRawXml = "";
            return ( ReturnVal );
        }//loadNodeAsChildFromRow() 

        //public void addNodeFromKey(CswNbtView View, ref CswNbtNodeKey Key)
        //{
        //    _CswNbtTreeNodes.addNodeFromKey(View, ref Key);
        //}

        /// <summary>
        /// Adds a Child from a DataRow.  Used by TreeLoaders.
        /// </summary>
        /// <param name="ParentNodeKey">Parent Node Key (for path generation)</param>
        /// <param name="DataRowToAdd">DataRow with node information</param>
        /// <param name="UseGrouping">Whether grouping nodes</param>
        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
        /// <param name="Selectable">True if the node is selectable, false otherwise</param>
        /// <param name="AddChildren">True if the user should be allowed to add children to this node, false otherwise</param>
        /// <param name="RowCount">Row number in view results</param>
        /// <returns>Returns NodeKey index for node</returns>
        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount )
        {
            Collection<CswNbtNodeKey> ReturnVal = _CswNbtTreeNodes.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Selectable, ShowInTree, AddChildren, RowCount );
            //ReturnVal.TreeKey = Key;
            _TreeAsTransformedXml = "";
            _TreeAsRawXml = "";
            return ( ReturnVal );
        }//loadNodeAsChildFromRow() 

        /// <summary>
        /// Sets the client-side expandmode of the current node
        /// </summary>
        public void setCurrentNodeExpandMode( string ExpandMode )
        {
            _CswNbtTreeNodes.setCurrentNodeExpandMode( ExpandMode );
        }

        /// <summary>
        /// Adds a Property value to a node.  This is the uncommon way to fill property data in for nodes.
        /// </summary>
        /// <param name="NodeTypePropId">Primary key of property</param>
        /// <param name="Name">Name of Property</param>
        /// <param name="Gestalt">Text representation of the value of the property</param>
        /// <param name="FieldType">FieldType of the property</param>
        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType FieldType )
        {
            _CswNbtTreeNodes.addProperty( NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType );
        }//addProperty



        #endregion //Modification******************************


        ///// <summary>
        ///// 
        ///// </summary>
        //public class CswNbtTreeDomProxyAttributes : NodeAttributes
        //{

        //    private CswNbtTreeDomProxy _CswNbtTree = null;
        //    public CswNbtTreeDomProxyAttributes( CswNbtTreeDomProxy CswNbtTree )
        //    {
        //        _CswNbtTree = CswNbtTree;
        //    }//ctor

        //    public string this[ string AttributeName ]
        //    {
        //        get
        //        {
        //            return ( _CswNbtTree._CswNbtTreeNodes.CurrentNodeAtributes[AttributeName] );
        //        }//
        //    }

        //}//CswNbtTreeDomProxyAttributes
        //private CswNbtTreeDomProxyAttributes _CurrentNodeAttributes = null;
        //public NodeAttributes CurrentNodeAttributes 
        //{
        //    get
        //    {
        //        return ( _CurrentNodeAttributes );
        //    }
        //}



    }//class CswNbtTreeDomProxy



}//namespace ChemSW.Nbt
