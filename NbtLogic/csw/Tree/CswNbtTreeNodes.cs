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

        //private XmlDocument _XmlDoc = new XmlDocument();
        //private XmlNode _CurrentNode = null;
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

        //private enum _ElemName
        //{
        //    Unknown,
        //    NbtTree,
        //    NbtNode,
        //    NbtNodeGroup,
        //    NbtNodeProp
        //} // enum _ElemName

        //private enum _AttrName
        //{
        //    Unknown,

        //    // For all types
        //    elemtype,
        //    childnodes,
        //    childprops,

        //    // Tree Attributes
        //    treename,

        //    // Node Attributes
        //    nodeid,
        //    key,
        //    iconfilename,
        //    nodename,
        //    nodetypeid,
        //    nodetypename,
        //    objectclassname,
        //    objectclassid,
        //    selectable,
        //    showintree,
        //    addchildren,
        //    expandmode,
        //    truncated,
        //    locked,

        //    // Prop Attributes
        //    nodetypepropid,
        //    jctnodepropid,
        //    propname,
        //    gestalt,
        //    fieldtype,

        //} // enum _AttrName

        // All
        public static string _AttrName_ElemType = "name";
        public static string _AttrName_ChildNodes = "childnodes";
        public static string _AttrName_ChildProps = "childprops";

        // NbtTree element
        public static string _ElemName_Tree = "NbtTree";
        public static string _AttrName_TreeName = "treename";

        // NbtNode element
        public static string _ElemName_Node = "NbtNode";
        //private static string _AttrName_TableName = "tablename";
        public static string _AttrName_NodeId = "nodeid";
        public static string _AttrName_Key = "key";
        public static string _AttrName_IconFileName = "iconfilename";
        public static string _AttrName_NodeName = "nodename";
        public static string _AttrName_NodeTypeId = "nodetypeid";
        //private static string _AttrName_NodeTypeName = "nodetypename";
        //private static string _AttrName_ObjectClass = "objectclassname";
        public static string _AttrName_ObjectClassId = "objectclassid";
        public static string _AttrName_Selectable = "selectable";
        public static string _AttrName_ShowInTree = "showintree";
        //private static string _AttrName_AddChildren = "addchildren";
        public static string _AttrName_ExpandMode = "expandmode";
        public static string _AttrName_Truncated = "truncated";
        public static string _AttrName_Locked = "locked";

        // NbtNodeProp element
        public static string _ElemName_NodeProp = "NbtNodeProp";
        public static string _AttrName_NodePropId = "nodetypepropid";
        public static string _AttrName_JctNodePropId = "jctnodepropid";
        public static string _AttrName_NodePropName = "propname";
        public static string _AttrName_NodePropGestalt = "gestalt";
        public static string _AttrName_NodePropFieldType = "fieldtype";

        // NbtNodeGroup element
        public static string _ElemName_NodeGroup = "NbtNodeGroup";
        //private static string _AttrName_GroupName = "nodename";
        //private static string _AttrName_GroupIcon = "iconfilename";

        //private XmlNode _TreeNode = null;

        #region JSON

        private CswNbtNodeKey _makeNodeJObject( JObject ParentObj,
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
            //NewNode[_AttrName.nodetypename.ToString()] = NodeTypeName.ToString();
            NewNode[_AttrName_ObjectClassId] = ObjectClassId.ToString();
            //NewNode[_AttrName.objectclass.ToString()] = ObjectClass.ToString();
            NewNode[_AttrName_IconFileName] = Icon;
            NewNode[_AttrName_Selectable] = Selectable.ToString().ToLower();
            NewNode[_AttrName_ShowInTree] = ShowInTree.ToString().ToLower();
            NewNode[_AttrName_Locked] = Locked.ToString().ToLower();
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

            return NewNodeKey;
        } // _makeNodeJObject()

        public void _makePropJObject( JObject NodeObj,
                                      Int32 NodeTypePropId,
                                      Int32 JctNodePropId,
                                      string PropName,
                                      string Gestalt,
                                      CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            //XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_NodeProp );
            //_CurrentNode.AppendChild( NewXmlNode );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropId, NodeTypePropId.ToString() ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_JctNodePropId, JctNodePropId.ToString() ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropName, Name ) );

            // BZ 7135 - write dates in XML format
            string PropValue = Gestalt;
            //switch( FieldType.FieldType )
            //{
            //    case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
            //        if( Gestalt != string.Empty )
            //            PropValue = CswTools.ToXmlDateTimeFormat( CswConvert.ToDateTime( Gestalt ) );
            //        break;
            //}

            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropGestalt, PropValue ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropFieldType, FieldType.FieldType.ToString() ) );
            JObject NewProp = new JObject();
            //NewProp[_AttrName.elemtype.ToString()] = _ElemName.NbtNodeProp.ToString();
            //NewProp[_AttrName.nodetypepropid.ToString()] = NodeTypePropId.ToString();
            //NewProp[_AttrName.jctnodepropid.ToString()] = JctNodePropId.ToString();
            //NewProp[_AttrName.propname.ToString()] = PropName.ToString();
            //NewProp[_AttrName.gestalt.ToString()] = PropValue.ToString();
            //NewProp[_AttrName.fieldtype.ToString()] = FieldType.ToString();

            //( (JArray) NodeObj[_AttrName.childprops.ToString()] ).Add( NewProp );


            NewProp[_AttrName_ElemType] = "NbtNodeProp";
            NewProp[_AttrName_NodePropId] = NodeTypePropId.ToString();
            NewProp[_AttrName_JctNodePropId] = JctNodePropId.ToString();
            NewProp[_AttrName_NodePropName] = PropName.ToString();
            NewProp[_AttrName_NodePropGestalt] = PropValue.ToString();
            NewProp[_AttrName_NodePropFieldType] = FieldType.ToString();
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
            //_TreeNode = _XmlDoc.CreateElement( _ElemName_Tree );
            //_XmlDoc.AppendChild( _TreeNode );
            _makeNodeJObject( null,
                              _ElemName_Tree,
                              null,
                              string.Empty,
                              Int32.MinValue,
                              Int32.MinValue,
                              string.Empty,
                              false,
                              null,
                              NodeSpecies.UnKnown,
                              true,
                              false,
                              out _TreeNode,
                              out _TreeNodeKey );

            //CswNbtNodeKey NodeKey = _makeNodeKey( null, _TreeNode, null, new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter ), NodeSpecies.UnKnown );

            //_TreeNode.Attributes.Append( _XmlDoc.CreateAttribute( _AttrName_TreeName ) );
            //_TreeNode.Attributes[_AttrName_TreeName].Value = TreeName;
            _TreeNode[_AttrName_TreeName] = TreeName;
            //_TreeNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
            //_TreeNode[_AttrName_Key] = NodeKey.ToString();
        }//ctor

        //private XmlNode _RootNode = null;

        //public void makeMoreNodeFromRow( CswNbtNodeKey ParentNodeKey, DataRow Row, Int32 NodeCount, CswNbtViewNode ViewNode )
        //{
        //    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( Row[_CswNbtColumnNames.NodeTypeId.ToLower()] ) );
        //    string TableName = NodeType.TableName;
        //    string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );

        //    XmlNode MoreNode = _XmlDoc.CreateElement( _ElemName_Node );
        //    _CurrentNode.AppendChild( MoreNode );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, "More..." ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_TableName, TableName ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, Row[PkColumnName].ToString() ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, Row[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, Row[_CswNbtColumnNames.ObjectClassId.ToLower()].ToString() ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, "Images/icons/" + Row[_CswNbtColumnNames.IconFileName.ToLower()].ToString() ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, true.ToString().ToLower() ) );

        //    XmlNode ParentXmlNode = null;
        //    if( _CurrentNode != null )
        //        ParentXmlNode = _CurrentNode;
        //    else
        //        ParentXmlNode = _TreeNode;
        //    CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );

        //    CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
        //    if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
        //        CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
        //    CountPath.Add( NodeCount.ToString() );

        //    CswNbtNodeKey NodeKey = _makeNodeEntry( ParentNodeKey, MoreNode, ViewNode, CountPath, NodeSpecies.More );

        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, "None" ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
        //    MoreNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );
        //}

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
                //_RootNode = _XmlDoc.CreateElement( _ElemName_Node );
                //_TreeNode.AppendChild( _RootNode );

                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, ViewName ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, "0" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, "0" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, "0" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, IconFileName ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, Selectable.ToString().ToLower() ) );
                //CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                //CountPath.Add( "1" );
                //CswNbtNodeKey NodeKey = _makeNodeKey( null, _RootNode, ViewRoot, CountPath, NodeSpecies.Root );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );

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
                                  out _RootNode,
                                  out _RootNodeKey );
            }
            else
            {
                throw new CswDniException( "CswNbtTreeNodes attempted to add a second root node to the tree" );
            }
        }


        //public XmlDocument getRawXml()
        //{
        //    return _XmlDoc;
        //}
        public JObject getRawJSON()
        {
            return _TreeNode;
        }
        //public void setRawXml( XmlDocument NewXmlDoc )
        //{
        //    _XmlDoc = NewXmlDoc;
        //    _TreeNode = NewXmlDoc.ChildNodes[0];
        //    _RootNode = _TreeNode.ChildNodes[0];

        //    _resetNodesAndParents();

        //    goToRoot();
        //}

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

        //private CswNbtNodeKey _makeNodeKeyFromString( string KeyString )
        //{
        //    CswNbtNodeKey ReturnVal = new CswNbtNodeKey( _CswNbtResources, KeyString );
        //    ReturnVal.TreeKey = _CswNbtTreeKey;
        //    return ( ReturnVal );
        //}

        //private CswNbtNodeKey _makeEmptyNodeKey()
        //{
        //    CswNbtNodeKey ReturnVal = new CswNbtNodeKey( _CswNbtResources );
        //    ReturnVal.TreeKey = _CswNbtTreeKey;
        //    return ( ReturnVal );
        //}

        //private CswNbtNode _getNbtNodeObjFromXmlNode( XmlNode XmlNode )
        //{
        //    if( _ElemName_Node != XmlNode.Name )
        //        throw ( new CswDniException( "The current node is a " + XmlNode.Name + ", not an NbtNode" ) );

        //    CswNbtNodeKey NodeKey = _makeNodeKeyFromString( XmlNode.Attributes[_AttrName_Key].Value );

        //    CswNbtNode ReturnVal = _CswNbtNodeCollection.GetNode( new CswPrimaryKey( XmlNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) ) );

        //    if( NodeSpecies.Plain == NodeKey.NodeSpecies )
        //    {
        //        ReturnVal.IconFileName = "Images/icons/" + XmlNode.Attributes[_AttrName_IconFileName].Value;
        //        ReturnVal.NodeName = XmlNode.Attributes[_AttrName_NodeName].Value;
        //        ReturnVal.NodeTypeId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeTypeId].Value );
        //    }
        //    if( XmlNode.Attributes[_AttrName_Selectable] != null )
        //        ReturnVal.Selectable = Convert.ToBoolean( XmlNode.Attributes[_AttrName_Selectable].Value.ToLower() );
        //    if( XmlNode.Attributes[_AttrName_ShowInTree] != null )
        //        ReturnVal.ShowInTree = Convert.ToBoolean( XmlNode.Attributes[_AttrName_ShowInTree].Value.ToLower() );

        //    return ( ReturnVal );
        //}

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
                    IconName = "Images/icons/" + PotentialIconSuffix;
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

            //_CurrentNode = _getCurrentNodeChildren().ElementAt<XmlNode>( ChildN );
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

        //public void goToNextSibling()
        //{
        //    XmlNode ThisNode = _CurrentNode;
        //    while( ThisNode.NextSibling != null && ThisNode.NextSibling.Name == "Property" )
        //        ThisNode = ThisNode.NextSibling;

        //    if( ThisNode.NextSibling != null )
        //    {
        //        ThisNode = ThisNode.NextSibling;
        //        _CurrentNode = ThisNode;
        //    }
        //    else
        //    {
        //        throw ( new CswDniException( "There are no more sibling nodes" ) );
        //    }
        //}//goToNextSibling()

        //public bool nextSiblingExists()
        //{
        //    XmlNode ThisNode = _CurrentNode;
        //    while( ThisNode.NextSibling != null && ThisNode.NextSibling.Name == "Property" )
        //        ThisNode = ThisNode.NextSibling;

        //    return ( ThisNode.NextSibling != null );
        //}//nextSiblingExists()

        //public void goToPreviousSibling()
        //{
        //    XmlNode ThisNode = _CurrentNode;
        //    while( ThisNode.PreviousSibling != null && ThisNode.PreviousSibling.Name == "Property" )
        //        ThisNode = ThisNode.PreviousSibling;

        //    if( ThisNode.PreviousSibling != null )
        //    {
        //        ThisNode = ThisNode.PreviousSibling;
        //        _CurrentNode = ThisNode;
        //    }
        //    else
        //    {
        //        throw ( new CswDniException( "There is no previous sibling" ) );
        //    }

        //}//goToPreviousSibling()


        //public bool previousSiblingExists()
        //{
        //    XmlNode ThisNode = _CurrentNode;
        //    while( ThisNode.PreviousSibling != null && ThisNode.PreviousSibling.Name == "Property" )
        //        ThisNode = ThisNode.PreviousSibling;

        //    return ( ThisNode.PreviousSibling != null );
        //}//PreviousSiblingExists()


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

        //private IEnumerable<XmlNode>_getCurrentNodeChildren()
        //{
        //    return from XmlNode ThisNode in _CurrentNode.ChildNodes
        //           where ( ThisNode.Name == _ElemName_NodeGroup || ThisNode.Name == _ElemName_Node )
        //           select ThisNode;
        //}


        public CswNbtNodeKey getNodeKeyForParentOfCurrentPosition()
        {
            if( isCurrentPositionRoot() )
                throw ( new CswDniException( "Current position is root" ) );
            return _getKey( _getParentNode() );
        }


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
        #region Modification Methods


        //private CswNbtNodeKey _makeNodeKey( CswNbtNodeKey ParentNodeKey, string ElemName, Int32 NodeId, Int32 NodeTypeId, Int32 ObjectClassId, CswNbtViewNode ViewNode, CswDelimitedString NodeCountPath, NodeSpecies Species )
        //{
        //    CswNbtNodeKey NewNodeKey = new CswNbtNodeKey( _CswNbtResources );
        //    NewNodeKey.TreeKey = _CswNbtTreeKey;

        //    NewNodeKey.NodeSpecies = Species;
        //    NewNodeKey.NodeCountPath = NodeCountPath;

        //    if( _ElemName_Node == XmlNode.Name )
        //    {
        //        //if( XmlNode.Attributes[_AttrName_TableName] != null )
        //        //    NewNodeKey.NodeId = new CswPrimaryKey( XmlNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) );
        //        //else
        //        NewNodeKey.NodeId = new CswPrimaryKey( "nodes", NodeId );
        //        NewNodeKey.NodeTypeId = NodeTypeId;
        //        NewNodeKey.ObjectClassId = ObjectClassId;
        //        if( ViewNode != null )
        //        {
        //            NewNodeKey.ViewNodeUniqueId = ViewNode.UniqueId;
        //        }
        //    }
        //    else if( !( _ElemName_Tree == XmlNode.Name || _ElemName_NodeGroup == XmlNode.Name ) )
        //    {
        //        throw ( new CswDniException( "Unknown element: " + XmlNode.Name ) );
        //    }

        //    if( !NodesAndParents.ContainsKey( NewNodeKey ) )
        //        NodesAndParents.Add( NewNodeKey, ParentNodeKey );

        //    return ( NewNodeKey );
        //}

        //private XmlAttribute _makeAttribute( string AttrName, string AttrVal )
        //{
        //    XmlAttribute ReturnVal = _XmlDoc.CreateAttribute( AttrName );
        //    ReturnVal.Value = AttrVal;
        //    return ( ReturnVal );
        //}

        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount )
        {
            return _loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Relationship, Relationship.Selectable, Relationship.ShowInTree, Relationship.AddChildren, RowCount );
        }

        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount )
        {
            return _loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, null, Selectable, ShowInTree, AddChildren, RowCount );
        }

        private Collection<CswNbtNodeKey> _loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( DataRowToAdd[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ) );
            string TableName = NodeType.TableName;
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );

            return _loadNodeAsChild( ParentNodeKey, UseGrouping, GroupName, Relationship, Selectable, ShowInTree, AddChildren, RowCount,
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
                                               bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount,
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

                    //string MatchingGroupPath = _ElemName_NodeGroup + "[@" + _AttrName_GroupName + "= '" + ThisGroupName + "']";
                    JObject MatchingGroup = _getMatchingGroup( ParentNode, ThisGroupName );
                    if( MatchingGroup == null )
                    {
                        //XmlNode NewGroupNode = _XmlDoc.CreateElement( _ElemName_NodeGroup );

                        //XmlAttribute GroupNameAttribute = _XmlDoc.CreateAttribute( _AttrName_GroupName );
                        //GroupNameAttribute.Value = ThisGroupName;
                        //NewGroupNode.Attributes.Append( GroupNameAttribute );

                        //XmlAttribute GroupIconAttribute = _XmlDoc.CreateAttribute( _AttrName_GroupIcon );
                        //GroupIconAttribute.Value = "Images/icons/group.gif";
                        //NewGroupNode.Attributes.Append( GroupIconAttribute );

                        //XmlAttribute GroupSelectableAttribute = _XmlDoc.CreateAttribute( _AttrName_Selectable );
                        //GroupSelectableAttribute.Value = false.ToString().ToLower();
                        //NewGroupNode.Attributes.Append( GroupSelectableAttribute );

                        //NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                        //NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );

                        //ParentXmlNode.AppendChild( NewGroupNode );
                        //ParentNodes.Add( NewGroupNode );

                        //CswDelimitedString GroupCountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                        //CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );
                        //if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                        //{
                        //    GroupCountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                        //}
                        //XmlNodeList AllGroupList = ParentXmlNode.SelectNodes( _ElemName_NodeGroup );
                        //GroupCountPath.Add( ( AllGroupList.Count + 1 ).ToString() );

                        //CswNbtNodeKey GroupKey = _makeNodeKey( ParentNodeKey, NewGroupNode, Relationship, GroupCountPath, NodeSpecies.Group );
                        //NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_Key, GroupKey.ToString() ) );

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
                //XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_Node );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, "Images/icons/" + IconFileName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_TableName, NodeId.TableName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, NodeId.PrimaryKey.ToString() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, NodeName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, NodeTypeId.ToString() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeName, NodeTypeName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, ObjectClassId.ToString() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClass, ObjectClassName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, Selectable.ToString().ToLower() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, ShowInTree.ToString().ToLower() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, AddChildren.ToString() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Locked, Locked.ToString().ToLower() ) );

                //ThisParentNode.AppendChild( NewXmlNode );
                //CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ThisParentNode.Attributes[_AttrName_Key].Value.ToString() );

                //CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                //if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                //    CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                //CountPath.Add( RowCount.ToString() );

                //CswNbtNodeKey ThisKey = _makeNodeKey( ThisParentKey, NewXmlNode, Relationship, CountPath, NodeSpecies.Plain );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Key, ThisKey.ToString() ) );

                //ReturnKeyColl.Add( ThisKey );

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
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //if( _CurrentNode.Attributes[_AttrName_ExpandMode] == null )
            //    _CurrentNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, ExpandMode ) );
            //else
            //    _CurrentNode.Attributes[_AttrName_ExpandMode].Value = ExpandMode;
            _checkCurrentNode();
            _setAttr( _CurrentNode, _AttrName_ExpandMode, ExpandMode );
        }

        public void setCurrentNodeChildrenTruncated( bool Truncated )
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //if( _CurrentNode.Attributes[_AttrName_Truncated] == null )
            //    _CurrentNode.Attributes.Append( _makeAttribute( _AttrName_Truncated, Truncated.ToString().ToLower() ) );
            //else
            //    _CurrentNode.Attributes[_AttrName_Truncated].Value = Truncated.ToString().ToLower();
            _checkCurrentNode();
            _setAttr( _AttrName_Truncated, Truncated );
        }

        public bool getCurrentNodeChildrenTruncated()
        {
            //bool ret = false;
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //if( _CurrentNode.Attributes[_AttrName_Truncated] == null )
            //{
            //    ret = CswConvert.ToBoolean( _CurrentNode.Attributes[_AttrName_Truncated].Value );
            //}
            //return ret;
            _checkCurrentNode();
            return CswConvert.ToBoolean( _getAttr( _AttrName_Truncated ) );
        }

        public CswNbtNodeKey getKeyForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //CswNbtNodeKey Ret = new CswNbtNodeKey( _CswNbtResources, _CurrentNode.Attributes[_AttrName_Key].Value );
            //return Ret;
            _checkCurrentNode();
            return _getKey( _CurrentNode );
        }//getKeyForCurrentNode()

        public CswPrimaryKey getIdForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            _checkCurrentNode();
            string Elem = _getElemName();
            //if( _CurrentNode.Name != _ElemName_Node )
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            //return new CswPrimaryKey( _CurrentNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( _CurrentNode.Attributes[_AttrName_NodeId].Value ) );
            return new CswPrimaryKey( "nodes", CswConvert.ToInt32( _getAttr( _AttrName_NodeId ) ) );

        }//getIdForCurrentNode()

        public string getNameForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //if( _CurrentNode.Name == _ElemName_NodeGroup )
            //    return _CurrentNode.Attributes[_AttrName_GroupName].Value;
            //else
            //    return _CurrentNode.Attributes[_AttrName_NodeName].Value;
            _checkCurrentNode();
            return _getAttr( _AttrName_NodeName );
        }//getNameForCurrentNode()

        public bool getLockedForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //return CswConvert.ToBoolean( _CurrentNode.Attributes[_AttrName_Locked].Value );
            _checkCurrentNode();
            return CswConvert.ToBoolean( _getAttr( _AttrName_Locked ) );
        }//getLockedForCurrentNode()

        public bool getSelectableForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );
            _checkCurrentNode();
            string Elem = _getElemName();
            //if( _CurrentNode.Name != _ElemName_Node )
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            //return Convert.ToBoolean( _CurrentNode.Attributes[_AttrName_Selectable].Value );
            return CswConvert.ToBoolean( _getAttr( _AttrName_Selectable ) );
        }//getSelectableForCurrentNode()

        public bool getNodeShowInTreeForCurrentNode()
        {
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );
            _checkCurrentNode();
            string Elem = _getElemName();
            //if( _CurrentNode.Name != _ElemName_Node )
            if( Elem != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + Elem.ToString() + ") is not a CswNbtNode" ) );

            //return Convert.ToBoolean( _CurrentNode.Attributes[_AttrName_ShowInTree].Value );
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

        //public Collection<CswNbtNodeKey> getKeysForNodeId( CswPrimaryKey NodeId )
        //{
        //    Collection<CswNbtNodeKey> NodeInstances = new Collection<CswNbtNodeKey>();
        //    foreach( CswNbtNodeKey NodeKey in NodesAndParents.Keys )
        //    {
        //        if( NodeKey.NodeId == NodeId )
        //            NodeInstances.Add( NodeKey );
        //    }
        //    return ( NodeInstances );

        //}//getKeysForNode()


        //public Collection<CswNbtNodeKey> getKeysForLevel( Int32 Level )
        //{
        //    Collection<CswNbtNodeKey> NodeKeys = new Collection<CswNbtNodeKey>();
        //    foreach( CswNbtNodeKey NodeKey in NodesAndParents.Keys )
        //    {
        //        if( NodeKey.TreeDepth == Level )
        //            NodeKeys.Add( NodeKey );
        //    }
        //    return NodeKeys;
        //} // getKeysForLevel()


        public CswNbtNodeKey getParentKey( CswNbtNodeKey ChildKey )
        {
            return NodesAndParents[ChildKey];
        }

        public JArray getChildPropNodesOfCurrentNode()
        {
            //XElement RawXml = XElement.Parse( _CurrentNode.OuterXml );
            //Collection<XElement> Ret = new Collection<XElement>();

            //if( RawXml.HasElements )
            //{
            //    foreach( XElement NodeTypeProp in RawXml.Elements( "NbtNodeProp" ) )
            //    {
            //        yield return NodeTypeProp;
            //    }

            //    //Grab all other NbtNodeProp nodes
            //    foreach( XElement NodeTypeProp in RawXml.DescendantNodesAndSelf().OfType<XElement>().Elements( "NbtNode" ).Elements( "NbtNodeProp" ) )
            //    {
            //        yield return NodeTypeProp;
            //    }
            //}
            return _getChildProps();
        }

        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            _checkCurrentNode();
            //if( null == _CurrentNode )
            //    throw ( new CswDniException( "There is no current node" ) );

            //XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_NodeProp );
            //_CurrentNode.AppendChild( NewXmlNode );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropId, NodeTypePropId.ToString() ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_JctNodePropId, JctNodePropId.ToString() ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropName, Name ) );
            //// BZ 7135 - write dates in XML format
            //string PropValue = Gestalt;
            //switch( FieldType.FieldType )
            //{
            //    case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
            //        if( Gestalt != string.Empty )
            //            PropValue = CswTools.ToXmlDateTimeFormat( CswConvert.ToDateTime( Gestalt ) );
            //        break;
            //}
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropGestalt, PropValue ) );
            //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropFieldType, FieldType.FieldType.ToString() ) );

            _makePropJObject( _CurrentNode, NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType );

        }//addProperty()


        public void removeCurrentNode()
        {
            _getChildNodes( _getParentNode() ).Remove( _CurrentNode );
            _CurrentNode = _RootNode;
        }

        #endregion //Modification******************************

        //public class CswNbtTreeNodesAttributes //: NodeAttributes
        //{

        //    private CswNbtTreeNodes _CswNbtTreeNodes = null;
        //    public CswNbtTreeNodesAttributes( CswNbtTreeNodes CswNbtTreeNodes )
        //    {
        //        _CswNbtTreeNodes = CswNbtTreeNodes;
        //    }//ctor

        //    public string this[string AttributeName]
        //    {
        //        get
        //        {
        //            return ( _CswNbtTreeNodes._CurrentNode.Attributes[AttributeName.ToLower()].Value );
        //        }
        //    }

        //}//CswNbtTreeNodesAttributes


    }//class CswNbtTreeNodes

}//namespace ChemSW.Nbt
