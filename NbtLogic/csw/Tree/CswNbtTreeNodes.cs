using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    public class CswNbtTreeNodes
    {
        private CswNbtColumnNames _CswNbtColumnNames = new CswNbtColumnNames();
        private CswNbtResources _CswNbtResources = null;

        private JObject _CurrentNode = null;
        private JObject _TreeNode = null;
        private JObject _RootNode = null;
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

        // All
        private const string _AttrName_ElemType = "name";
        public const string _AttrName_ChildNodes = "childnodes";
        public const string _AttrName_ChildProps = "childprops";

        // NbtTree element
        public const string _ElemName_Tree = "NbtTree";
        public const string _AttrName_TreeName = "treename";

        // NbtNode element
        public const string _ElemName_Node = "NbtNode";
        public const string _AttrName_NodeId = "nodeid";
        public const string _AttrName_Key = "key";
        public const string _AttrName_IconFileName = "iconfilename";
        public const string _AttrName_NodeName = "nodename";
        public const string _AttrName_NodeTypeId = "nodetypeid";
        public const string _AttrName_ObjectClassId = "objectclassid";
        public const string _AttrName_Selectable = "selectable";
        public const string _AttrName_ShowInTree = "showintree";
        public const string _AttrName_ExpandMode = "expandmode";
        public const string _AttrName_Truncated = "truncated";
        public const string _AttrName_Locked = "locked";
        public const string _AttrName_Included = "included";

        // NbtNodeProp element
        public const string _ElemName_NodeProp = "NbtNodeProp";
        public const string _AttrName_NodePropId = "nodetypepropid";
        public const string _AttrName_JctNodePropId = "jctnodepropid";
        public const string _AttrName_NodePropName = "propname";
        public const string _AttrName_NodePropGestalt = "gestalt";
        public const string _AttrName_NodePropField1 = "field1";
        public const string _AttrName_NodePropField2 = "field2";
        public const string _AttrName_NodePropField1_Fk = "field1_fk";
        public const string _AttrName_NodePropField1_Numeric = "field1_numeric";
        public const string _AttrName_NodePropFieldType = "fieldtype";
        public const string _AttrName_NodePropHidden = "hidden";

        // NbtNodeGroup element
        public const string _ElemName_NodeGroup = "NbtNodeGroup";

        #region JSON

        private void _makeNodeJObject( JObject ParentObj,
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
                                                out JObject NewNode,
                                                out CswNbtNodeKey NewNodeKey )
        {
            // Make the object
            NewNode = new JObject();
            NewNode[_AttrName_ElemType] = ElemName.ToString();
            if( NodeId != null )
            {
                NewNode[_AttrName_NodeId] = NodeId.PrimaryKey.ToString();
            }
            else
            {
                NewNode[_AttrName_NodeId] = "0";
            }
            NewNode[_AttrName_NodeName] = NodeName;
            NewNode[_AttrName_NodeTypeId] = NodeTypeId.ToString();
            NewNode[_AttrName_ObjectClassId] = ObjectClassId.ToString();
            NewNode[_AttrName_IconFileName] = Icon;
            NewNode[_AttrName_Selectable] = Selectable.ToString().ToLower();
            NewNode[_AttrName_ShowInTree] = ShowInTree.ToString().ToLower();
            NewNode[_AttrName_Locked] = Locked.ToString().ToLower();
            NewNode[_AttrName_Included] = Included.ToString().ToLower();
            NewNode[_AttrName_ChildNodes] = new JArray();
            NewNode[_AttrName_ChildProps] = new JArray();

            CswNbtNodeKey ParentNodeKey = null;
            CswDelimitedString NodeCountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
            if( ParentObj != null )
            {
                ParentNodeKey = _getKey( ParentObj );
                NodeCountPath = ParentNodeKey.NodeCountPath;
                NodeCountPath.Add( ( ( _getChildNodes( ParentObj ).Count() ) + 1 ).ToString() );
                ( (JArray) ParentObj[_AttrName_ChildNodes] ).Add( NewNode );
            }

            // Make the key
            NewNodeKey = new CswNbtNodeKey( _CswNbtResources );
            NewNodeKey.TreeKey = _CswNbtTreeKey;
            NewNodeKey.NodeSpecies = Species;
            NewNodeKey.NodeCountPath = NodeCountPath;
            if( ElemName == _ElemName_Node )
            {
                NewNodeKey.NodeId = NodeId;
                NewNodeKey.NodeTypeId = NodeTypeId;
                NewNodeKey.ObjectClassId = ObjectClassId;
                if( ViewNode != null )
                {
                    NewNodeKey.ViewNodeUniqueId = ViewNode.UniqueId;
                }
            }
            else if( ElemName == _ElemName_Tree || ElemName == _ElemName_NodeGroup )
            {
                // Nothing
            }
            else if( ElemName == _ElemName_NodeProp )
            {
                throw ( new CswDniException( "_makeNodeJObject called on an NbtNodeProp element" ) );
            }
            else
            {
                throw ( new CswDniException( "Unknown element: " + ElemName ) );
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

            NewNode[_AttrName_Key] = NewNodeKey.ToString();
        } // _makeNodeJObject()

        public void _makePropJObject( JObject NodeObj,
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
            // BZ 7135 - write dates in XML format
            string PropValue = Gestalt;
            JObject NewProp = new JObject();

            NewProp[_AttrName_ElemType] = "NbtNodeProp";
            NewProp[_AttrName_NodePropId] = NodeTypePropId.ToString();
            NewProp[_AttrName_JctNodePropId] = JctNodePropId.ToString();
            NewProp[_AttrName_NodePropName] = PropName;
            NewProp[_AttrName_NodePropGestalt] = PropValue;
            NewProp[_AttrName_NodePropField1] = Field1;
            NewProp[_AttrName_NodePropField2] = Field2;
            NewProp[_AttrName_NodePropField1_Fk] = Field1_Fk;
            NewProp[_AttrName_NodePropField1_Numeric] = Field1_Numeric;
            NewProp[_AttrName_NodePropFieldType] = FieldType.ToString();
            NewProp[_AttrName_NodePropHidden] = Hidden;
            ( (JArray) NodeObj[_AttrName_ChildProps] ).Add( NewProp );

        }//_makePropJObject()




        private string _getElemName()
        {
            return _getElemName( _CurrentNode );
        }

        private string _getElemName( JObject NodeObj )
        {
            return NodeObj[_AttrName_ElemType].ToString();
        }

        private JArray _getChildNodes()
        {
            return _getChildNodes( _CurrentNode );
        }

        private JArray _getChildNodes( JObject NodeObj )
        {
            return (JArray) NodeObj[_AttrName_ChildNodes];
        }

        private JObject _getParentNode()
        {
            return _getParentNode( _CurrentNode );
        }

        private JObject _getParentNode( JObject NodeObj )
        {
            JObject ret = null;
            if( NodeObj.Parent != null &&               // JArray
                NodeObj.Parent.Parent != null &&        // JProperty
                NodeObj.Parent.Parent.Parent != null )  // JObject
            {
                ret = (JObject) NodeObj.Parent.Parent.Parent;
            }
            return ret;
        }

        private JArray _getChildProps()
        {
            return _getChildProps( _CurrentNode );
        }

        private JArray _getChildProps( JObject NodeObj )
        {
            return (JArray) NodeObj[_AttrName_ChildProps];
        }


        private CswPrimaryKey _getNodeId( JObject NodeObj )
        {
            return new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeObj[_AttrName_NodeId].ToString() ) );
        }

        private CswNbtNodeKey _getKey( JObject NodeObj )
        {
            return _getKey( NodeObj[_AttrName_Key].ToString() );
        }
        private CswNbtNodeKey _getKey( string KeyAsString )
        {
            CswNbtNodeKey ret = new CswNbtNodeKey( _CswNbtResources, KeyAsString );
            ret.TreeKey = _CswNbtTreeKey;
            return ret;
        }

        private string _getAttr( string AttrName )
        {
            return _getAttr( _CurrentNode, AttrName );
        }
        private string _getAttr( JObject NodeObj, string AttrName )
        {
            string ret = string.Empty;
            if( NodeObj[AttrName] != null )
            {
                ret = NodeObj[AttrName].ToString();
            }
            return ret;
        }

        private void _setAttr( string AttrName, string Value )
        {
            _setAttr( _CurrentNode, AttrName, Value );
        }
        private void _setAttr( JObject NodeObj, string AttrName, string Value )
        {
            NodeObj[AttrName] = Value;
        }

        private void _setAttr( string AttrName, bool Value )
        {
            _setAttr( _CurrentNode, AttrName, Value );
        }
        private void _setAttr( JObject NodeObj, string AttrName, bool Value )
        {
            _setAttr( NodeObj, AttrName, Value.ToString().ToLower() );
        }

        private JObject _getMatchingGroup( JObject ParentNode, string ThisGroupName )
        {
            JObject ret = null;
            JArray PotentialGroupNodes = _getChildNodes( ParentNode );
            foreach( JObject PotentialGroupNode in PotentialGroupNodes )
            {
                if( _getElemName( PotentialGroupNode ) == _ElemName_NodeGroup &&
                    _getAttr( PotentialGroupNode, _AttrName_NodeName ) == ThisGroupName )
                {
                    ret = PotentialGroupNode;
                }
            }
            return ret;
        } // _getMatchingGroup()

        #endregion JSON

        public CswNbtTreeNodes( CswNbtTreeKey CswNbtTreeKey, string XslFilePath, string TreeName, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeCollection = CswNbtNodeCollection;
            _CswNbtTreeKey = CswNbtTreeKey;

            NodesAndParents = new Dictionary<CswNbtNodeKey, CswNbtNodeKey>();
            NodesById = new Dictionary<CswPrimaryKey, Collection<CswNbtNodeKey>>();

            // Make Tree Node
            _makeNodeJObject( null,
                              _ElemName_Tree,
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

            _TreeNode[_AttrName_TreeName] = TreeName;
        }//ctor

        public void makeRootNode( string ViewName, string IconFileName, bool Selectable )
        {
            _makeRootNode( ViewName, null, IconFileName, Selectable );
        }
        public void makeRootNode( string ViewName, CswNbtViewRoot ViewRoot )
        {
            _makeRootNode( ViewName, ViewRoot, ViewRoot.IconFileName, ViewRoot.Selectable );
        }
        private void _makeRootNode( string ViewName, CswNbtViewRoot ViewRoot, string IconFileName, bool Selectable )
        {
            if( _RootNode == null )
            {
                _makeNodeJObject( _TreeNode,
                                  _ElemName_Node,
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
                                  ( ViewRoot != null ) ? ViewRoot.Included : false,
                                  out _RootNode,
                                  out _RootNodeKey );
            }
            else
            {
                throw new CswDniException( "CswNbtTreeNodes attempted to add a second root node to the tree" );
            }
        }

        public JObject getRawJSON()
        {
            return _TreeNode;
        }

        /// <summary>
        /// Repairs the NodesAndParents hashtable
        /// </summary>
        private void _resetNodesAndParents()
        {
            NodesAndParents = new Dictionary<CswNbtNodeKey, CswNbtNodeKey>();
            goToRoot();
            _resetNodesAndParentsRecursive();
        }
        private void _resetNodesAndParentsRecursive()
        {
            CswNbtNodeKey CurrentKey = getKeyForCurrentNode();
            for( Int32 c = 0; c < getChildNodeCount(); c++ )
            {
                goToNthChild( c );
                CswNbtNodeKey ChildKey = getKeyForCurrentNode();
                if( !NodesAndParents.ContainsKey( ChildKey ) )
                {
                    NodesAndParents.Add( ChildKey, CurrentKey );
                }
                _resetNodesAndParentsRecursive();
                goToParentNode();
            }
        }


        public bool isNodeDefined( CswNbtNodeKey NodeKey )
        {
            return ( null != _getJSONNodeFromKey( NodeKey ) );
        }

        private JObject _getJSONNodeFromKey( CswNbtNodeKey NodeKey )
        {
            JObject ThisNode = _TreeNode;
            foreach( Int32 ThisCount in NodeKey.NodeCountPath.ToIntCollection() )
            {
                if( ThisNode != null )
                {
                    JArray ChildNodes = _getChildNodes( ThisNode );
                    if( ChildNodes.Count >= ThisCount )
                    {
                        ThisNode = (JObject) ChildNodes[ThisCount - 1];
                    }
                    else
                    {
                        ThisNode = null;
                    }
                } // if( ThisNode == null )
            }
            return ThisNode;
        }

        private CswNbtNode _getNbtNodeObjFromJSONNode( JObject NodeObj )
        {
            string Elem = _getElemName( NodeObj );
            if( Elem != _ElemName_Node )
            {
                throw ( new CswDniException( "The current node is a " + Elem.ToString() + ", not an NbtNode" ) );
            }

            CswNbtNodeKey NodeKey = _getKey( NodeObj );
            CswNbtNode ReturnVal = _CswNbtNodeCollection.GetNode( _getNodeId( NodeObj ) );

            if( NodeSpecies.Plain == NodeKey.NodeSpecies )
            {
                string IconName = default( string );
                string PotentialIconSuffix = _getAttr( NodeObj, _AttrName_IconFileName );
                if( false == string.IsNullOrEmpty( PotentialIconSuffix ) )
                {
                    IconName = CswNbtMetaDataObjectClass.IconPrefix16 + PotentialIconSuffix;
                }
                ReturnVal.IconFileName = IconName;
                ReturnVal.NodeName = _getAttr( NodeObj, _AttrName_NodeName );
                ReturnVal.NodeTypeId = CswConvert.ToInt32( _getAttr( NodeObj, _AttrName_NodeTypeId ) );
            }
            ReturnVal.Selectable = CswConvert.ToBoolean( _getAttr( NodeObj, _AttrName_Selectable ) );
            ReturnVal.ShowInTree = CswConvert.ToBoolean( _getAttr( NodeObj, _AttrName_ShowInTree ) );

            return ReturnVal;
        }

        public CswNbtNode getNode( CswNbtNodeKey NodeKey )
        {

            return ( _getNbtNodeObjFromJSONNode( _getJSONNodeFromKey( NodeKey ) ) );

        }//getNode()

        public CswNbtNode getParentNodeOf( CswNbtNodeKey NodeKey )
        {
            CswNbtNode ReturnVal = null;

            JObject CurrentNodeSave = _CurrentNode;

            makeNodeCurrent( NodeKey );

            if( !isCurrentPositionRoot() )
            {
                ReturnVal = _getNbtNodeObjFromJSONNode( _getJSONNodeFromKey( getNodeKeyForParentOfCurrentPosition() ) );
            }

            _CurrentNode = CurrentNodeSave;

            return ( ReturnVal );

        }//getParentNodeOf()


        //Navigation and interrogation methods*****************************************
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
            JArray CurrentChildren = _getChildNodes();
            Int32 CurrentChildCount = CurrentChildren.Count();
            if( 0 == CurrentChildCount )
                throw ( new CswDniException( "The current node has no children" ) );

            if( CurrentChildCount <= ChildN )
                throw ( new CswDniException( "Requested child node " + ChildN + " does not exist; current node contains " + CurrentChildCount + " children" ) );

            _CurrentNode = (JObject) CurrentChildren[ChildN];

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
                throw ( new CswDniException( "Already at root!" ) );
            _CurrentNode = _getParentNode();
        }

        public void makeNodeCurrent( CswNbtNodeKey NodeKey )
        {
            if( NodeKey.TreeKey == this._CswNbtTreeKey )
            {
                _CurrentNode = _getJSONNodeFromKey( NodeKey );
            }
            else
            {
                _CurrentNode = null;
            }
        }

        public CswNbtNode getCurrentNode()
        {
            return _getNbtNodeObjFromJSONNode( _CurrentNode );
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
                throw ( new CswDniException( "Current position is root" ) );
            return _getKey( _getParentNode() );
        }


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
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

            JObject ParentNode = null;
            if( _CurrentNode != null )
                ParentNode = _CurrentNode;
            else
                ParentNode = _TreeNode;

            Collection<JObject> ParentNodes = new Collection<JObject>();
            if( !UseGrouping )
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

                    JObject MatchingGroup = _getMatchingGroup( ParentNode, ThisGroupName );
                    if( MatchingGroup == null )
                    {
                        CswNbtNodeKey MatchingGroupKey = null;
                        _makeNodeJObject( ParentNode,
                                          _ElemName_NodeGroup,
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


            foreach( JObject ThisParentNode in ParentNodes )
            {
                CswNbtNodeKey ThisKey = null;
                JObject ThisNode = null;
                _makeNodeJObject( ThisParentNode,
                                  _ElemName_Node,
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
            _setAttr( _CurrentNode, _AttrName_ExpandMode, ExpandMode );
        }

        public void setCurrentNodeChildrenTruncated( bool Truncated )
        {
            _checkCurrentNode();
            _setAttr( _AttrName_Truncated, Truncated );
        }

        public bool getCurrentNodeChildrenTruncated()
        {
            _checkCurrentNode();
            return CswConvert.ToBoolean( _getAttr( _AttrName_Truncated ) );
        }

        public CswNbtNodeKey getKeyForCurrentNode()
        {
            _checkCurrentNode();
            return _getKey( _CurrentNode );
        }//getKeyForCurrentNode()

        public CswPrimaryKey getIdForCurrentNode()
        {
            _checkCurrentNode();
            string Elem = _getElemName();
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _getAttr( _AttrName_NodeId ) ) );

        }//getIdForCurrentNode()

        public string getNameForCurrentNode()
        {
            _checkCurrentNode();
            return _getAttr( _AttrName_NodeName );
        }//getNameForCurrentNode()

        public bool getLockedForCurrentNode()
        {
            _checkCurrentNode();
            return CswConvert.ToBoolean( _getAttr( _AttrName_Locked ) );
        }//getLockedForCurrentNode()

        public bool getIncludedForCurrentNode()
        {
            _checkCurrentNode();
            return CswConvert.ToBoolean( _getAttr( _AttrName_Included ) );
        }//getIncludedForCurrentNode()

        public bool getSelectableForCurrentNode()
        {
            _checkCurrentNode();
            string Elem = _getElemName();
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            return CswConvert.ToBoolean( _getAttr( _AttrName_Selectable ) );
        }//getSelectableForCurrentNode()

        public bool getNodeShowInTreeForCurrentNode()
        {
            _checkCurrentNode();
            string Elem = _getElemName();
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            return CswConvert.ToBoolean( _getAttr( _AttrName_ShowInTree ) );
        }//getNodeShowInTreeForCurrentNode()


        public Int32 getNodeCountForCurrentLevel()
        {
            return _getChildNodes( _getParentNode() ).Count();
        }

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

        public JArray getChildPropNodesOfCurrentNode()
        {
            return _getChildProps();
        }

        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType.NbtFieldType FieldType, string Field1, string Field2, Int32 Field1_Fk, double Field1_Numeric, bool Hidden )
        {
            _checkCurrentNode();
            _makePropJObject( _CurrentNode, NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType, Field1, Field2, Field1_Fk, Field1_Numeric, Hidden );

        }//addProperty()


        public void removeCurrentNode()
        {
            _getChildNodes( _getParentNode() ).Remove( _CurrentNode );
            _CurrentNode = _RootNode;
        }

        #endregion //Modification******************************

    }//class CswNbtTreeNodes

}//namespace ChemSW.Nbt
