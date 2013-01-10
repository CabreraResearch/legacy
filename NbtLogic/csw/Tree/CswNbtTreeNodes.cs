using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    public class CswNbtTreeNodes: ICswNbtTree
    {
        #region Events

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

        #endregion Events
        
        #region Constructor

        private CswNbtColumnNames _CswNbtColumnNames = new CswNbtColumnNames();
        private CswNbtResources _CswNbtResources = null;

        private CswNbtTreeNode _CurrentNode = null;
        private CswNbtTreeNode _TreeNode = null;
        private CswNbtTreeNode _RootNode = null;

        private CswNbtNodeKey _TreeNodeKey = null;
        private CswNbtNodeKey _RootNodeKey = null;

        private CswNbtTreeKey _CswNbtTreeKey = null;
        private CswNbtNodeCollection _CswNbtNodeCollection = null;

        private Dictionary<CswNbtNodeKey, CswNbtNodeKey> NodesAndParents = null;
        private Dictionary<CswPrimaryKey, Collection<CswNbtNodeKey>> NodesById = null;
        
        /*
         * The tree is structured in such a way that the elements
         * all correspond to a NodeSpecies. All other data pertaining
         * to a node MUST be stored as attributes of said elements. 
         * Adhereing to this structure means that the navigational 
         * methods of DOM will correspond to our understanding of an 
         * NBT tree. This will result in an ugly tree because the NbtNode
         * element will have an un-Godly number of attributes, one of which
         * will be a CSV representing the allowed child node types. But at 
         * this point this approach saves us a lot of effort: otherwise, 
         * we would have to proxy the DOM navigation in such a way as to 
         * make it look like NBT-tree navigation. This way clients of this 
         * class end up proxying directly to DOM on a one-to-one basis.
         */
        
        // NbtTree element
        public sealed class Elements
        {
            public const string Tree = "NbtTree";
            public const string Prop = "NbtNodeProp";
            public const string Node = "NbtNode";
            public const string Group = "NbtNodeGroup";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="View">Tree View</param>
        /// <param name="CswNbtResources">The CswNbtResources object</param>
        /// <param name="CswNbtNodeWriter">A CswNbtNodeWriter object</param>
        /// <param name="CswNbtNodeCollection">A reference to the CswNbtNodeCollection</param>
        /// <param name="IsFullyPopulated"></param>
        public CswNbtTreeNodes( string TreeName, CswNbtView View, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection, bool IsFullyPopulated )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeCollection = CswNbtNodeCollection;
            _CswNbtTreeKey = new CswNbtTreeKey( _CswNbtResources, _View );
            _View = View;
            _IsFullyPopulated = IsFullyPopulated;


            NodesAndParents = new Dictionary<CswNbtNodeKey, CswNbtNodeKey>();
            NodesById = new Dictionary<CswPrimaryKey, Collection<CswNbtNodeKey>>();

            // Make Tree Node
            _makeNbtTreeNode( null,
                              Elements.Tree,
                              null,
                              string.Empty,
                              Int32.MinValue,
                              Int32.MinValue,
                              string.Empty,
                              false,
                              null,
                              NodeSpecies.Plain,
                              true,
                              false,
                              true,
                              out _TreeNode,
                              out _TreeNodeKey );

            _TreeNode.TreeName = TreeName;
        }//ctor
        
        #endregion Constructor

        #region Properties

        private CswNbtView _View = null;
        /// <summary>
        /// View which was used to build this tree
        /// </summary>
        public CswNbtView View
        {
            get { return _View; }
            set { _View = value; }
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

        

        #endregion Properties
        
        #region TreeNode

        private void _makeNbtTreeNode( CswNbtTreeNode ParentNode,
                                                string ElemName,
                                                CswPrimaryKey NodeId,
                                                string NodeName,
                                                Int32 NodeTypeId,
                                                Int32 ObjectClassId,
                                                string Icon,
                                                bool Selectable,
                                                CswNbtViewNode ViewNode,
                                                NodeSpecies Species,
                                                bool ShowInTree,
                                                bool Locked,
                                                bool Included,
                                                out CswNbtTreeNode NewNode,
                                                out CswNbtNodeKey NewNodeKey )
        {
            // Make the object
            NewNode = new CswNbtTreeNode( NodeId, NodeName, NodeTypeId, ObjectClassId )
                {
                    ElementName = ElemName,
                    IconFileName = Icon,
                    Selectable = Selectable,
                    ShowInTree = ShowInTree,
                    Locked = Locked,
                    Included = Included,
                    ChildNodes = new Collection<CswNbtTreeNode>(),
                    ChildProps = new Collection<CswNbtTreeNodeProp>()
                };

            CswNbtNodeKey ParentNodeKey = null;
            CswDelimitedString NodeCountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
            if( ParentNode != null )
            {
                ParentNodeKey =  _getKey( ParentNode );
                string ParentNodeCountPath = ParentNodeKey.NodeCountPath.ToString();
                NodeCountPath.FromString( ParentNodeCountPath );
                NodeCountPath.Add( ( ( ParentNode.ChildNodes.Count() ) + 1 ).ToString() );
                ParentNode.ChildNodes.Add( NewNode );
                NewNode.ParentNode = ParentNode;
            }

            // Make the key
            NewNodeKey = new CswNbtNodeKey( _CswNbtResources );
            NewNodeKey.TreeKey = _CswNbtTreeKey;
            NewNodeKey.NodeSpecies = Species;
            NewNodeKey.NodeCountPath = NodeCountPath;
            if( NewNode.ElementName == Elements.Node )
            {
                NewNodeKey.NodeId = NodeId;
                NewNodeKey.NodeTypeId = NodeTypeId;
                NewNodeKey.ObjectClassId = ObjectClassId;
                if( ViewNode != null )
                {
                    NewNodeKey.ViewNodeUniqueId = ViewNode.UniqueId;
                }
            }
            else if( NewNode.ElementName == Elements.Tree || NewNode.ElementName == Elements.Group )
            {
                // Nothing
            }
            else if( NewNode.ElementName == Elements.Prop )
            {
                throw ( new CswDniException( "_makeNbtTreeNode called on an NbtNodeProp element" ) );
            }
            else
            {
                throw ( new CswDniException( "Unknown element: " + NewNode.ElementName ) );
            }

            // Dictionaries
            if( NodeId != null )
            {
                if( false == NodesById.ContainsKey( NodeId ) )
                {
                    NodesById.Add( NodeId, new Collection<CswNbtNodeKey>() );
                }
                NodesById[NodeId].Add( NewNodeKey );
            }
            if( ParentNodeKey != null && !NodesAndParents.ContainsKey( NewNodeKey ) )
            {
                NodesAndParents.Add( NewNodeKey, ParentNodeKey );
            }

            NewNode.NodeKey = NewNodeKey;
        } // _makeNbtTreeNode()

        public void _makeTreeNodeProp( CswNbtTreeNode TreeNode,
                                      Int32 NodeTypePropId,
                                      Int32 JctNodePropId,
                                      string PropName,
                                      string Gestalt,
                                      CswNbtMetaDataFieldType.NbtFieldType FieldType,
                                      string Field1,
                                      string Field2,
                                      Int32 Field1_Fk,
                                      double Field1_Numeric,
                                      bool Hidden )
        {
            CswNbtTreeNodeProp TreeNodeProp = new CswNbtTreeNodeProp( FieldType, PropName, NodeTypePropId, JctNodePropId, TreeNode )
                {
                    ElementName = "NbtNodeProp",
                    Gestalt = Gestalt,
                    Field1 = Field1,
                    Field2 = Field2,
                    Field1_Fk = Field1_Fk,
                    Field1_Numeric = Field1_Numeric,
                    Hidden = Hidden
                };

            TreeNode.ChildProps.Add( TreeNodeProp );
        }//_makeTreeNodeProp()
        
        private Collection<CswNbtTreeNode> _getChildNodes()
        {
            return _getChildNodes( _CurrentNode );
        }

        private Collection<CswNbtTreeNode> _getChildNodes( CswNbtTreeNode TreeNode )
        {
            return TreeNode.ChildNodes;
        }

        private CswNbtTreeNode _getParentNode()
        {
            return _getParentNode( _CurrentNode );
        }

        private CswNbtTreeNode _getParentNode( CswNbtTreeNode TreeNode )
        {
            return TreeNode.ParentNode;
        }

        private Collection<CswNbtTreeNodeProp> _getChildProps()
        {
            return _getChildProps( _CurrentNode );
        }

        private Collection<CswNbtTreeNodeProp> _getChildProps( CswNbtTreeNode TreeNode )
        {
            return TreeNode.ChildProps;
        }
        
        private CswNbtNodeKey _getKey( CswNbtTreeNode TreeNode )
        {
            TreeNode.NodeKey.TreeKey = _CswNbtTreeKey;
            return TreeNode.NodeKey;
        }
        
        private CswNbtTreeNode _getMatchingGroup( CswNbtTreeNode ParentTreeNode, string ThisGroupName )
        {
            CswNbtTreeNode ret = null;
            foreach( CswNbtTreeNode PotentialGroupNode in ParentTreeNode.ChildNodes )
            {
                if( PotentialGroupNode.ElementName == Elements.Group &&
                    PotentialGroupNode.NodeName == ThisGroupName )
                {
                    ret = PotentialGroupNode;
                }
            }
            return ret;
        } // _getMatchingGroup()

        #endregion TreeNode

        #region Make Nodes


        public void makeRootNode( string IconFileName, bool Selectable )
        {
            _makeRootNode( ViewName, null, IconFileName, Selectable );
        }
        public void makeRootNode( CswNbtViewRoot ViewRoot )
        {
            _makeRootNode( ViewRoot.ViewName, ViewRoot, ViewRoot.IconFileName, ViewRoot.Selectable );
        }
        private void _makeRootNode( string ViewName, CswNbtViewRoot ViewRoot, string IconFileName, bool Selectable )
        {
            if( _RootNode == null )
            {
                _makeNbtTreeNode( _TreeNode,
                                  Elements.Node,
                                  null,
                                  ViewName,
                                  0,
                                  0,
                                  IconFileName,
                                  Selectable,
                                  ViewRoot,
                                  NodeSpecies.Root,
                                  true,
                                  false,
                                  ( ViewRoot != null ) && ViewRoot.Included,
                                  out _RootNode,
                                  out _RootNodeKey );
                _CurrentNode = _RootNode;
            }
            else
            {
                throw new CswDniException( "CswNbtTreeNodes attempted to add a second root node to the tree" );
            }
        } 
        #endregion Make Nodes

        #region Get Nodes

        //public JObject getRawJSON()
        //{
        //    return _TreeNode;
        //}

        public bool isNodeDefined( CswNbtNodeKey NodeKey )
        {
            return ( null != _getTreeNodeFromKey( NodeKey ) );
        }

        private CswNbtTreeNode _getTreeNodeFromKey( CswNbtNodeKey NodeKey )
        {
            CswNbtTreeNode ThisNode = _TreeNode;
            foreach( Int32 ThisCount in NodeKey.NodeCountPath.ToIntCollection() )
            {
                if( ThisNode != null )
                {
                    if( ThisNode.ChildNodes.Count >= ThisCount )
                    {
                        ThisNode = ThisNode.ChildNodes[ThisCount - 1];
                    }
                    else
                    {
                        ThisNode = null;
                    }
                } // if( ThisNode == null )
            }
            return ThisNode;
        } // _getTreeNodeFromKey()

        private CswNbtTreeNode _getTreeNodeFromId( CswPrimaryKey NodeId )
        {
            CswNbtTreeNode ret = null;
            if( NodesById.Keys.Contains( NodeId ) && NodesById[NodeId].Count > 0 )
            {
                CswNbtNodeKey ThisNodeKey = NodesById[NodeId].First();
                ret = _getTreeNodeFromKey( ThisNodeKey );
            }
            return ret;
        } // _getTreeNodeFromId()

        private CswNbtNode _getNbtNodeObjFromTreeNode( CswNbtTreeNode TreeNode )
        {
            if( TreeNode.ElementName != Elements.Node )
            {
                throw ( new CswDniException( "The current node is a " + TreeNode.ElementName + ", not an NbtNode" ) );
            }

            CswNbtNodeKey NodeKey = _getKey( TreeNode );
            CswNbtNode ReturnVal = _CswNbtNodeCollection[TreeNode.CswNodeId];

            if( NodeSpecies.Plain == NodeKey.NodeSpecies )
            {
                string IconName = default( string );
                string PotentialIconSuffix = TreeNode.IconFileName;
                if( false == string.IsNullOrEmpty( PotentialIconSuffix ) )
                {
                    IconName = CswNbtMetaDataObjectClass.IconPrefix16 + PotentialIconSuffix;
                }
                ReturnVal.IconFileName = IconName;
                ReturnVal.NodeName = TreeNode.NodeName;
                ReturnVal.NodeTypeId = TreeNode.NodeTypeId;
            }
            ReturnVal.Selectable = TreeNode.Selectable;
            ReturnVal.ShowInTree = TreeNode.ShowInTree;

            return ReturnVal;
        }

        public CswNbtNode getNode( CswNbtNodeKey NodeKey )
        {

            return ( _getNbtNodeObjFromTreeNode( _getTreeNodeFromKey( NodeKey ) ) );

        }//getNode()

        public CswNbtNode getParentNodeOf( CswNbtNodeKey NodeKey )
        {
            CswNbtNode ReturnVal = null;

            CswNbtTreeNode CurrentNodeSave = _CurrentNode;

            makeNodeCurrent( NodeKey );

            if( false == isCurrentPositionRoot() )
            {
                ReturnVal = _getNbtNodeObjFromTreeNode( _getTreeNodeFromKey( getNodeKeyForParentOfCurrentPosition() ) );
            }

            _CurrentNode = CurrentNodeSave;

            return ( ReturnVal );

        }//getParentNodeOf()

        /// <summary>
        /// Returns the root node of the tree
        /// </summary>
        public CswNbtNode getRootNode()
        {
            CswNbtNodeKey CurrentKey = getNodeKeyForCurrentPosition();
            goToRoot();

            CswNbtNode RootNode = _CswNbtNodeCollection.GetNode( CurrentKey.NodeId, CurrentKey.NodeTypeId, CurrentKey.NodeSpecies, DateTime.MinValue );
            makeNodeCurrent( CurrentKey );
            return RootNode;
        }//getRootNode()

        public Collection<CswNbtTreeNodeProp> getChildNodePropsOfNode()
        {
            return getChildPropNodesOfCurrentNode();
        }

        /// <summary>
        /// Returns the NodeKey for the currently indexed node
        /// </summary>
        public CswNbtNodeKey getNodeKeyForCurrentPosition()
        {
            CswNbtNodeKey ReturnVal = getKeyForCurrentNode();
            //ReturnVal.TreeKey = Key;
            return ( ReturnVal );

        }//getNodeKeyForCurrentPosition()

        /// <summary>
        /// Returns the currently indexed node
        /// </summary>
        public CswNbtNode getNodeForCurrentPosition()
        {
            return getNode( getNodeKeyForCurrentPosition() );
        }//getNodeForCurrentPosition() 
        
        #endregion Get Nodes
        
        #region Navigation and interrogation methods
        
        public void goToRoot()
        {
            _CurrentNode = _RootNode;
        }//goToRoot()

        /// <summary>
        /// Move the current position to the Nth child of the current node
        /// </summary>
        /// <param name="ChildN">0-based Index of Child</param>
        public void goToNthChild( int ChildN )
        {
            Collection<CswNbtTreeNode> CurrentChildren = _getChildNodes();
            Int32 CurrentChildCount = CurrentChildren.Count();
            if( 0 == CurrentChildCount )
            {  throw ( new CswDniException( "The current node has no children" ) );}

            if( CurrentChildCount <= ChildN )
            {    throw ( new CswDniException( "Requested child node " + ChildN + " does not exist; current node contains " + CurrentChildCount + " children" ) );}

            _CurrentNode = CurrentChildren[ChildN];

        }//goToNthChild() 


        public bool isCurrentNodeChildOfRoot()
        {
            return ( _getParentNode() == _RootNode );
        }

        public bool isCurrentPositionRoot()
        {
            return ( _CurrentNode == _RootNode );
        }

        public void goToParentNode()
        {
            if( isCurrentPositionRoot() )
            {    throw ( new CswDniException( "Already at root!" ) );}
            _CurrentNode = _getParentNode();
        }

        public void makeNodeCurrent( CswNbtNodeKey NodeKey )
        {
            if( NodeKey.TreeKey == this._CswNbtTreeKey )
            {
                _CurrentNode = _getTreeNodeFromKey( NodeKey );
            }
            else
            {
                _CurrentNode = null;
            }
        }

        public void makeNodeCurrent( CswPrimaryKey NodeId )
        {
            _CurrentNode = _getTreeNodeFromId( NodeId );
        }

        public CswNbtNode getCurrentNode()
        {
            return _getNbtNodeObjFromTreeNode( _CurrentNode );
        }

        public bool isCurrentNodeDefined()
        {
            return ( null != _CurrentNode );
        }

        public Int32 getChildNodeCount()
        {
            return _getChildNodes().Count();
        }
        
        public CswNbtNodeKey getNodeKeyForParentOfCurrentPosition()
        {
            if( isCurrentPositionRoot() )
            {    throw ( new CswDniException( "Current position is root" ) );}
            return _getKey( _getParentNode() );
        }

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
            { throw ( new CswDniException( "OnIterateNode must be set before calling iterateTree()" ) ); }

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


        #endregion Navigation And Interrogation
        
        #region Modification Methods

        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount, bool Included = true )
        {
            return _loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Relationship, Relationship.Selectable, Relationship.ShowInTree, Relationship.AddChildren, RowCount, Included );
        }

        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included = true )
        {
            return _loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, null, Selectable, ShowInTree, AddChildren, RowCount, Included );
        }

        private Collection<CswNbtNodeKey> _loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included = true )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( DataRowToAdd[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ) );
            string TableName = NodeType.TableName;
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );

            return _loadNodeAsChild( ParentNodeKey, UseGrouping, GroupName, Relationship, Selectable, ShowInTree, AddChildren, RowCount, Included,
                                     DataRowToAdd[_CswNbtColumnNames.IconFileName.ToLower()].ToString(),
                                     DataRowToAdd[_CswNbtColumnNames.NameTemplate.ToLower()].ToString(),
                                     new CswPrimaryKey( TableName, CswConvert.ToInt32( DataRowToAdd[PkColumnName] ) ),
                                     DataRowToAdd[_CswNbtColumnNames.NodeName.ToLower()].ToString(),
                                     CswConvert.ToInt32( DataRowToAdd[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ),
                                     DataRowToAdd[_CswNbtColumnNames.NodeTypeName.ToLower()].ToString(),
                                     CswConvert.ToInt32( DataRowToAdd[_CswNbtColumnNames.ObjectClassId.ToLower()].ToString() ),
                                     DataRowToAdd[_CswNbtColumnNames.ObjectClassName.ToLower()].ToString(),
                                     CswConvert.ToBoolean( DataRowToAdd[_CswNbtColumnNames.Locked.ToLower()] )
                                   );
        }

        public Collection<CswNbtNodeKey> _loadNodeAsChild( CswNbtNodeKey ParentNodeKey, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship,
                                               bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included,
                                               string IconFileName, string NameTemplate, CswPrimaryKey NodeId, string NodeName, Int32 NodeTypeId,
                                               string NodeTypeName, Int32 ObjectClassId, string ObjectClassName, bool Locked )
        {
            Collection<CswNbtNodeKey> ReturnKeyColl = new Collection<CswNbtNodeKey>();

            CswNbtTreeNode ParentNode = null;
            ParentNode = _CurrentNode ?? _TreeNode;

            Collection<CswNbtTreeNode> ParentNodes = new Collection<CswNbtTreeNode>();
            if( false == UseGrouping )
            {
                ParentNodes.Add( ParentNode );
            }
            else
            {
                // Interpret commas to denote multiple groups
                string GroupNameForLoop = GroupName;
                string ThisGroupName = GroupName;
                do
                {
                    if( GroupNameForLoop.IndexOf( ',' ) >= 0 )
                    {
                        ThisGroupName = GroupNameForLoop.Substring( 0, GroupNameForLoop.IndexOf( ',' ) ).Trim();
                        GroupNameForLoop = GroupNameForLoop.Substring( GroupNameForLoop.IndexOf( ',' ) + 1 ).Trim();
                    }
                    else
                    {
                        ThisGroupName = GroupNameForLoop.Trim();
                        GroupNameForLoop = string.Empty;
                    }

                    CswNbtTreeNode MatchingGroup = _getMatchingGroup( ParentNode, ThisGroupName );
                    if( MatchingGroup == null )
                    {
                        CswNbtNodeKey MatchingGroupKey = null;
                        _makeNbtTreeNode( ParentNode,
                                          Elements.Group,
                                          null,
                                          ThisGroupName,
                                          Int32.MinValue,
                                          Int32.MinValue,
                                          "group.gif",
                                          false,
                                          Relationship,
                                          NodeSpecies.Group,
                                          true,
                                          false,
                                          true,
                                          out MatchingGroup,
                                          out MatchingGroupKey );
                    }

                    if( MatchingGroup != null )
                    {
                        ParentNodes.Add( MatchingGroup );
                    }
                } // do
                while( GroupNameForLoop != string.Empty );
            } // if-else( !UseGrouping )


            foreach( CswNbtTreeNode ThisParentNode in ParentNodes )
            {
                CswNbtNodeKey ThisKey = null;
                CswNbtTreeNode ThisNode = null;
                _makeNbtTreeNode( ThisParentNode,
                                  Elements.Node,
                                  NodeId,
                                  NodeName,
                                  NodeTypeId,
                                  ObjectClassId,
                                  IconFileName,
                                  Selectable,
                                  Relationship,
                                  NodeSpecies.Plain,
                                  ShowInTree,
                                  Locked,
                                  Included,
                                  out ThisNode,
                                  out ThisKey );
                ReturnKeyColl.Add( ThisKey );
            }

            return ( ReturnKeyColl );

        }//loadNodeAsChild()

        private void _checkCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );
        }

        public void setCurrentNodeExpandMode( string ExpandMode )
        {
            _checkCurrentNode();
            _CurrentNode.ExpandMode = ExpandMode;
        }

        public void setCurrentNodeChildrenTruncated( bool Truncated )
        {
            _checkCurrentNode();
            _CurrentNode.Truncated = Truncated;
        }

        public bool getCurrentNodeChildrenTruncated()
        {
            _checkCurrentNode();
            return _CurrentNode.Truncated;
        }

        public CswNbtNodeKey getKeyForCurrentNode()
        {
            _checkCurrentNode();
            return _getKey( _CurrentNode );
        }//getKeyForCurrentNode()

        public CswPrimaryKey getNodeIdForCurrentPosition()
        {
            _checkCurrentNode();
            
            if( _CurrentNode.ElementName != Elements.Node )
            { throw ( new CswDniException( "The current node (" + _CurrentNode.ElementName + ") is not a CswNbtNode" ) ); }

            return _CurrentNode.CswNodeId;

        }//getNodeIdForCurrentPosition()

        public string getNodeNameForCurrentPosition()
        {
            _checkCurrentNode();
            return _CurrentNode.NodeName;
        }//getNodeNameForCurrentPosition()

        public bool getNodeLockedForCurrentPosition()
        {
            _checkCurrentNode();
            return _CurrentNode.Locked;
        }//getNodeLockedForCurrentPosition()

        public bool getNodeIncludedForCurrentPosition()
        {
            _checkCurrentNode();
            return _CurrentNode.Included;
        }//getNodeIncludedForCurrentPosition()

        public bool getNodeSelectableForCurrentPosition()
        {
            _checkCurrentNode();
            
            if( _CurrentNode.ElementName != Elements.Node )
            {    throw ( new CswDniException( "The current node (" + _CurrentNode.ElementName + ") is not a CswNbtNode" ) );}

            return _CurrentNode.Selectable;
        }//getNodeSelectableForCurrentPosition()

        public bool getNodeShowInTreeForCurrentPosition()
        {
            _checkCurrentNode();
            if( _CurrentNode.ElementName != Elements.Node )
            {    throw ( new CswDniException( "The current node (" + _CurrentNode.ElementName.ToString() + ") is not a CswNbtNode" ) );}

            return _CurrentNode.ShowInTree;
        }//getNodeShowInTreeForCurrentPosition()


        public Int32 getNodeCountForCurrentLevel()
        {
            return _getChildNodes( _getParentNode() ).Count();
        }

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
            Collection<CswNbtNodeKey> KeyList = getKeysForNodeId( NodeId );
            if( null != KeyList && KeyList.Count > 0 )
            {
                ReturnVal = KeyList[0];
            }
            return ( ReturnVal );
        }//getNodeKeyByNodeId()

        public Collection<CswNbtNodeKey> getNodeKeysByNodeIdAndViewNode( CswPrimaryKey NodeId, CswNbtViewNode ViewNode )
        {
            Collection<CswNbtNodeKey> ret = new Collection<CswNbtNodeKey>();
            if( NodesById.ContainsKey( NodeId ) )
            {
                foreach( CswNbtNodeKey NodeKey in NodesById[NodeId] )
                {
                    if( NodeKey.ViewNodeUniqueId == ViewNode.UniqueId )
                    {
                        ret.Add( NodeKey );
                    }
                }
            }
            return ret;
        } // getNodeKeysByNodeIdAndViewNode()

        public Collection<CswNbtNodeKey> getKeysForNodeId( CswPrimaryKey NodeId )
        {
            Collection<CswNbtNodeKey> Ret = new Collection<CswNbtNodeKey>();
            if( NodeId != null )
            {
                NodesById.TryGetValue( NodeId, out Ret );
            }
            return Ret;
        }

        public CswNbtNodeKey getParentKey( CswNbtNodeKey ChildKey )
        {
            return NodesAndParents[ChildKey];
        }

        public Collection<CswNbtTreeNodeProp> getChildPropNodesOfCurrentNode()
        {
            return _getChildProps();
        }

        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType.NbtFieldType FieldType, string Field1, string Field2, Int32 Field1_Fk, double Field1_Numeric, bool Hidden )
        {
            _checkCurrentNode();
            _makeTreeNodeProp( _CurrentNode, NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType, Field1, Field2, Field1_Fk, Field1_Numeric, Hidden );

        }//addProperty()


        public void removeCurrentNode()
        {
            _getChildNodes( _getParentNode() ).Remove( _CurrentNode );
            _CurrentNode = _RootNode;
        }

        #endregion Modification Methods

    }//class CswNbtTreeNodes

}//namespace ChemSW.Nbt
