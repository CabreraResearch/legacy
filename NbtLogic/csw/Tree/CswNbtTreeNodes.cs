using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    public class CswNbtTreeNodes
    {
        private CswNbtColumnNames _CswNbtColumnNames = new CswNbtColumnNames();
        private CswNbtResources _CswNbtResources = null;
        private XmlDocument _XmlDoc = new XmlDocument();
        private XmlNode _CurrentNode = null;
        CswNbtTreeKey _CswNbtTreeKey = null;

        private Dictionary<CswNbtNodeKey, CswNbtNodeKey> NodesAndParents = null;

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

        //******************** NbtTree element
        public static string _ElemName_Tree = "NbtTree";
        private static string _AttrName_TreeName = "name";

        //******************** NbtNode element
        public static string _ElemName_Node = "NbtNode";
        private static string _AttrName_TableName = "tablename";
        private static string _AttrName_NodeId = "nodeid";
        private static string _AttrName_Key = "key";
        private static string _AttrName_IconFileName = "iconfilename";
        private static string _AttrName_NodeName = "nodename";
        private static string _AttrName_NodeTypeId = "nodetypeid";
        private static string _AttrName_NodeTypeName = "nodetypename";
        private static string _AttrName_ObjectClass = "objectclassname";
        private static string _AttrName_ObjectClassId = "objectclassid";
        private static string _AttrName_Selectable = "selectable";
        private static string _AttrName_ShowInTree = "showintree";
        private static string _AttrName_AddChildren = "addchildren";
        private static string _AttrName_ExpandMode = "expandmode";
        private static string _AttrName_Truncated = "truncated";
        private static string _AttrName_Locked = "locked";

        //******************** NbtNodeProp element
        public static string _ElemName_NodeProp = "NbtNodeProp";
        private static string _AttrName_NodePropId = "nodetypepropid";
        private static string _AttrName_JctNodePropId = "jctnodepropid";
        private static string _AttrName_NodePropName = "name";
        private static string _AttrName_NodePropGestalt = "gestalt";
        private static string _AttrName_NodePropFieldType = "fieldtype";

        //******************** NbtNodeGroup element
        public static string _ElemName_NodeGroup = "NbtNodeGroup";
        private static string _AttrName_GroupName = "name";
        private static string _AttrName_GroupIcon = "icon";

        private XmlNode _TreeNode = null;
        CswNbtNodeCollection _CswNbtNodeCollection = null;

        public CswNbtTreeNodes( CswNbtTreeKey CswNbtTreeKey, string XslFilePath, string TreeName, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeCollection = CswNbtNodeCollection;
            _CswNbtTreeKey = CswNbtTreeKey;

            NodesAndParents = new Dictionary<CswNbtNodeKey, CswNbtNodeKey>();

            // Make Tree Node
            _TreeNode = _XmlDoc.CreateElement( _ElemName_Tree );
            _XmlDoc.AppendChild( _TreeNode );

            _TreeNode.Attributes.Append( _XmlDoc.CreateAttribute( _AttrName_TreeName ) );
            _TreeNode.Attributes[_AttrName_TreeName].Value = TreeName;
            CswNbtNodeKey NodeKey = _makeNodeEntry( null, _TreeNode, null, new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter ), NodeSpecies.UnKnown );
            _TreeNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
        }//ctor

        private XmlNode _RootNode = null;

        public void makeMoreNodeFromRow( CswNbtNodeKey ParentNodeKey, DataRow Row, Int32 NodeCount, CswNbtViewNode ViewNode )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( Row[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ) );
            string TableName = NodeType.TableName;
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );

            XmlNode MoreNode = _XmlDoc.CreateElement( _ElemName_Node );
            _CurrentNode.AppendChild( MoreNode );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, "More..." ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_TableName, TableName ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, Row[PkColumnName].ToString() ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, Row[_CswNbtColumnNames.NodeTypeId.ToLower()].ToString() ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, Row[_CswNbtColumnNames.ObjectClassId.ToLower()].ToString() ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, "Images/icons/" + Row[_CswNbtColumnNames.IconFileName.ToLower()].ToString() ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, true.ToString().ToLower() ) );

            XmlNode ParentXmlNode = null;
            if( _CurrentNode != null )
                ParentXmlNode = _CurrentNode;
            else
                ParentXmlNode = _TreeNode;
            CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );

            CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
            if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
            CountPath.Add( NodeCount.ToString() );

            CswNbtNodeKey NodeKey = _makeNodeEntry( ParentNodeKey, MoreNode, ViewNode, CountPath, NodeSpecies.More );

            MoreNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, "None" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );
        }

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
                _RootNode = _XmlDoc.CreateElement( _ElemName_Node );
                _TreeNode.AppendChild( _RootNode );

                _RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, ViewName ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, "0" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, "0" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, "0" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, IconFileName ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, Selectable.ToString().ToLower() ) );
                CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                CountPath.Add( "1" );
                CswNbtNodeKey NodeKey = _makeNodeEntry( null, _RootNode, ViewRoot, CountPath, NodeSpecies.Root );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );
            }
            else
            {
                throw new CswDniException( "CswNbtTreeNodes attempted to add a second root node to the tree" );
            }
        }


        public XmlDocument getRawXml()
        {
            return _XmlDoc;
        }
        public void setRawXml( XmlDocument NewXmlDoc )
        {
            _XmlDoc = NewXmlDoc;
            _TreeNode = NewXmlDoc.ChildNodes[0];
            _RootNode = _TreeNode.ChildNodes[0];

            _resetNodesAndParents();

            goToRoot();
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
            return ( null != _getXmlNodeFromKey( NodeKey ) );
        }

        private XmlNode _getXmlNodeFromKey( CswNbtNodeKey NodeKey )
        {
            XmlNode CurrentNode = _XmlDoc.DocumentElement;
            foreach( Int32 ThisCount in NodeKey.NodeCountPath.ToIntCollection() )
            {
                if( CurrentNode != null )
                {
                    XmlNodeList ChildNodes = CurrentNode.SelectNodes( _ElemName_Node );
                    if( ChildNodes.Count >= ThisCount )
                    {
                        CurrentNode = ChildNodes[ThisCount - 1];
                    }
                    else
                    {
                        CurrentNode = null;
                    }

                } // if( CurrentNode == null )
            } // foreach( Int32 ThisCount in NodeKey.NodeCountPath.ToIntCollection() )
            return CurrentNode;
        } // _getXmlNodeFromKey()

        private CswNbtNodeKey _makeNodeKeyFromString( string KeyString )
        {
            CswNbtNodeKey ReturnVal = new CswNbtNodeKey( _CswNbtResources, KeyString );
            ReturnVal.TreeKey = _CswNbtTreeKey;
            return ( ReturnVal );
        }

        private CswNbtNodeKey _makeEmptyNodeKey()
        {
            CswNbtNodeKey ReturnVal = new CswNbtNodeKey( _CswNbtResources );
            ReturnVal.TreeKey = _CswNbtTreeKey;
            return ( ReturnVal );
        }

        private CswNbtNode _getNbtNodeObjFromXmlNode( XmlNode XmlNode )
        {
            if( _ElemName_Node != XmlNode.Name )
                throw ( new CswDniException( "The current node is a " + XmlNode.Name + ", not an NbtNode" ) );

            CswNbtNodeKey NodeKey = _makeNodeKeyFromString( XmlNode.Attributes[_AttrName_Key].Value );

            CswNbtNode ReturnVal = _CswNbtNodeCollection.GetNode( new CswPrimaryKey( XmlNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) ) );

            if( NodeSpecies.Plain == NodeKey.NodeSpecies )
            {
                ReturnVal.IconFileName = "Images/icons/" + XmlNode.Attributes[_AttrName_IconFileName].Value;
                ReturnVal.NodeName = XmlNode.Attributes[_AttrName_NodeName].Value;
                ReturnVal.NodeTypeId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeTypeId].Value );
            }
            if( XmlNode.Attributes[_AttrName_Selectable] != null )
                ReturnVal.Selectable = Convert.ToBoolean( XmlNode.Attributes[_AttrName_Selectable].Value.ToLower() );
            if( XmlNode.Attributes[_AttrName_ShowInTree] != null )
                ReturnVal.ShowInTree = Convert.ToBoolean( XmlNode.Attributes[_AttrName_ShowInTree].Value.ToLower() );

            return ( ReturnVal );
        }

        public CswNbtNode getNode( CswNbtNodeKey NodeKey )
        {

            return ( _getNbtNodeObjFromXmlNode( _getXmlNodeFromKey( NodeKey ) ) );

        }//getNode()

        public CswNbtNode getParentNodeOf( CswNbtNodeKey NodeKey )
        {
            CswNbtNode ReturnVal = null;

            XmlNode CurrentNodeSave = _CurrentNode;

            makeNodeCurrent( NodeKey );

            if( !isCurrentPositionRoot() )
            {
                ReturnVal = _getNbtNodeObjFromXmlNode( _getXmlNodeFromKey( getNodeKeyForParentOfCurrentPosition() ) );
            }

            _CurrentNode = CurrentNodeSave;

            return ( ReturnVal );

        }//getParentNode()


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
            Int32 CurrentChildCount = getChildNodeCount();
            if( 0 == CurrentChildCount )
                throw ( new CswDniException( "The current node has no children" ) );

            if( CurrentChildCount <= ChildN )
                throw ( new CswDniException( "Requested child node " + ChildN + " does not exist; current node contains " + CurrentChildCount + " children" ) );

            _CurrentNode = _getCurrentNodeChildren().ElementAt<XmlNode>( ChildN );

        }//goToNthChild() 


        public bool isCurrentNodeChildOfRoot()
        {

            return ( _CurrentNode.ParentNode == _RootNode );

        }//isCurrentNodeRoot()

        public bool isCurrentPositionRoot()
        {
            return ( _CurrentNode == _RootNode );

        }//isCurrentNodeRoot()

        public void goToNextSibling()
        {
            XmlNode ThisNode = _CurrentNode;
            while( ThisNode.NextSibling != null && ThisNode.NextSibling.Name == "Property" )
                ThisNode = ThisNode.NextSibling;

            if( ThisNode.NextSibling != null )
            {
                ThisNode = ThisNode.NextSibling;
                _CurrentNode = ThisNode;
            }
            else
            {
                throw ( new CswDniException( "There are no more sibling nodes" ) );
            }
        }//goToNextSibling()

        public bool nextSiblingExists()
        {
            XmlNode ThisNode = _CurrentNode;
            while( ThisNode.NextSibling != null && ThisNode.NextSibling.Name == "Property" )
                ThisNode = ThisNode.NextSibling;

            return ( ThisNode.NextSibling != null );
        }//nextSiblingExists()

        public void goToPreviousSibling()
        {
            XmlNode ThisNode = _CurrentNode;
            while( ThisNode.PreviousSibling != null && ThisNode.PreviousSibling.Name == "Property" )
                ThisNode = ThisNode.PreviousSibling;

            if( ThisNode.PreviousSibling != null )
            {
                ThisNode = ThisNode.PreviousSibling;
                _CurrentNode = ThisNode;
            }
            else
            {
                throw ( new CswDniException( "There is no previous sibling" ) );
            }

        }//goToPreviousSibling()


        public bool previousSiblingExists()
        {
            XmlNode ThisNode = _CurrentNode;
            while( ThisNode.PreviousSibling != null && ThisNode.PreviousSibling.Name == "Property" )
                ThisNode = ThisNode.PreviousSibling;

            return ( ThisNode.PreviousSibling != null );
        }//PreviousSiblingExists()


        public void goToParentNode()
        {
            if( isCurrentPositionRoot() )
                throw ( new CswDniException( "Already at root!" ) );

            _CurrentNode = _CurrentNode.ParentNode;

        }//goToParentNode()

        public void makeNodeCurrent( CswNbtNodeKey NodeKey )
        {
            _CurrentNode = _getXmlNodeFromKey( NodeKey );
        }//makeNodeCurrent() 

        public CswNbtNode getCurrentNode()
        {
            return ( _getNbtNodeObjFromXmlNode( _CurrentNode ) );
        }//

        public bool isCurrentNodeDefined()
        {
            return ( null != _CurrentNode );

        }//isCurrentNodeDefined() 


        public int getChildNodeCount()
        {
            return _getCurrentNodeChildren().Count();

        }//getChildNodeCount() 

        private IEnumerable<XmlNode> _getCurrentNodeChildren()
        {
            return from XmlNode ThisNode in _CurrentNode.ChildNodes
                   where ( ThisNode.Name == _ElemName_NodeGroup || ThisNode.Name == _ElemName_Node )
                   select ThisNode;
        }


        public CswNbtNodeKey getNodeKeyForParentOfCurrentPosition()
        {
            if( isCurrentPositionRoot() )
                throw ( new CswDniException( "Current position is root" ) );

            return ( _makeNodeKeyFromString( _CurrentNode.ParentNode.Attributes[_AttrName_Key].Value ) );

        }//getNodeKeyForParentOfCurrentPosition()


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
        #region Modification Methods


        private CswNbtNodeKey _makeNodeEntry( CswNbtNodeKey ParentNodeKey, XmlNode XmlNode, CswNbtViewNode ViewNode, CswDelimitedString NodeCountPath, NodeSpecies Species )
        {
            CswNbtNodeKey NewNodeKey = _makeEmptyNodeKey();

            NewNodeKey.NodeSpecies = Species;
            NewNodeKey.NodeCountPath = NodeCountPath;

            if( _ElemName_Node == XmlNode.Name )
            {
                if( XmlNode.Attributes[_AttrName_TableName] != null )
                    NewNodeKey.NodeId = new CswPrimaryKey( XmlNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) );
                else
                    NewNodeKey.NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) );
                NewNodeKey.NodeTypeId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeTypeId].Value );
                NewNodeKey.ObjectClassId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_ObjectClassId].Value );
                if( ViewNode != null )
                    NewNodeKey.ViewNodeUniqueId = ViewNode.UniqueId;
            }
            else if( !( _ElemName_Tree == XmlNode.Name || _ElemName_NodeGroup == XmlNode.Name ) )
            {
                throw ( new CswDniException( "Unknown element: " + XmlNode.Name ) );
            }

            if( !NodesAndParents.ContainsKey( NewNodeKey ) )
                NodesAndParents.Add( NewNodeKey, ParentNodeKey );

            return ( NewNodeKey );
        }

        private XmlAttribute _makeAttribute( string AttrName, string AttrVal )
        {
            XmlAttribute ReturnVal = _XmlDoc.CreateAttribute( AttrName );
            ReturnVal.Value = AttrVal;
            return ( ReturnVal );
        }

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

            XmlNode ParentXmlNode = null;
            if( _CurrentNode != null )
                ParentXmlNode = _CurrentNode;
            else
                ParentXmlNode = _TreeNode;

            Collection<XmlNode> ParentNodes = new Collection<XmlNode>();
            if( !UseGrouping )
            {
                ParentNodes.Add( ParentXmlNode );
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

                    string MatchingGroupPath = _ElemName_NodeGroup + "[@" + _AttrName_GroupName + "= '" + ThisGroupName + "']";
                    XmlNodeList MatchingGroupList = ParentXmlNode.SelectNodes( MatchingGroupPath );
                    if( 1 <= MatchingGroupList.Count )
                    {
                        ParentNodes.Add( MatchingGroupList[0] );
                    }
                    else if( 0 == MatchingGroupList.Count )
                    {
                        XmlNode NewGroupNode = _XmlDoc.CreateElement( _ElemName_NodeGroup );

                        XmlAttribute GroupNameAttribute = _XmlDoc.CreateAttribute( _AttrName_GroupName );
                        GroupNameAttribute.Value = ThisGroupName;
                        NewGroupNode.Attributes.Append( GroupNameAttribute );

                        XmlAttribute GroupIconAttribute = _XmlDoc.CreateAttribute( _AttrName_GroupIcon );
                        GroupIconAttribute.Value = "Images/icons/group.gif";
                        NewGroupNode.Attributes.Append( GroupIconAttribute );

                        XmlAttribute GroupSelectableAttribute = _XmlDoc.CreateAttribute( _AttrName_Selectable );
                        GroupSelectableAttribute.Value = false.ToString().ToLower();
                        NewGroupNode.Attributes.Append( GroupSelectableAttribute );

                        NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                        NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_Locked, "false" ) );

                        ParentXmlNode.AppendChild( NewGroupNode );
                        ParentNodes.Add( NewGroupNode );

                        CswDelimitedString GroupCountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                        CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );
                        if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                        {
                            GroupCountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                        }
                        XmlNodeList AllGroupList = ParentXmlNode.SelectNodes( _ElemName_NodeGroup );
                        GroupCountPath.Add( ( AllGroupList.Count + 1 ).ToString() );

                        CswNbtNodeKey GroupKey = _makeNodeEntry( ParentNodeKey, NewGroupNode, Relationship, GroupCountPath, NodeSpecies.Group );
                        NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_Key, GroupKey.ToString() ) );
                    }
                }
                while( GroupNameForLoop != string.Empty );
            }


            foreach( XmlNode ThisParentNode in ParentNodes )
            {
                XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_Node );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, "Images/icons/" + IconFileName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_TableName, NodeId.TableName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, NodeId.PrimaryKey.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, NodeName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, NodeTypeId.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeName, NodeTypeName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, ObjectClassId.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClass, ObjectClassName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, Selectable.ToString().ToLower() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, ShowInTree.ToString().ToLower() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, AddChildren.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Locked, Locked.ToString().ToLower() ) );

                ThisParentNode.AppendChild( NewXmlNode );
                CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ThisParentNode.Attributes[_AttrName_Key].Value.ToString() );

                CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                    CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                CountPath.Add( RowCount.ToString() );

                CswNbtNodeKey ThisKey = _makeNodeEntry( ThisParentKey, NewXmlNode, Relationship, CountPath, NodeSpecies.Plain );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Key, ThisKey.ToString() ) );

                ReturnKeyColl.Add( ThisKey );
            }

            return ( ReturnKeyColl );

        }//loadNodeAsChild()

        public void setCurrentNodeExpandMode( string ExpandMode )
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Attributes[_AttrName_ExpandMode] == null )
                _CurrentNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, ExpandMode ) );
            else
                _CurrentNode.Attributes[_AttrName_ExpandMode].Value = ExpandMode;
        }

        public void setCurrentNodeChildrenTruncated( bool Truncated )
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Attributes[_AttrName_Truncated] == null )
                _CurrentNode.Attributes.Append( _makeAttribute( _AttrName_Truncated, Truncated.ToString().ToLower() ) );
            else
                _CurrentNode.Attributes[_AttrName_Truncated].Value = Truncated.ToString().ToLower();
        }

        public bool getCurrentNodeChildrenTruncated()
        {
            bool ret = false;
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Attributes[_AttrName_Truncated] != null )
            {
                ret = CswConvert.ToBoolean( _CurrentNode.Attributes[_AttrName_Truncated].Value );
            }
            return ret;
        }

        public CswNbtNodeKey getKeyForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            CswNbtNodeKey Ret = new CswNbtNodeKey( _CswNbtResources, _CurrentNode.Attributes[_AttrName_Key].Value );
            return Ret;
        }//getKeyForCurrentNode()

        public CswPrimaryKey getIdForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Name != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + _CurrentNode.Name + ") is not a CswNbtNode" ) );

            return new CswPrimaryKey( _CurrentNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( _CurrentNode.Attributes[_AttrName_NodeId].Value ) );

        }//getIdForCurrentNode()

        public string getNameForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Name == _ElemName_NodeGroup )
                return _CurrentNode.Attributes[_AttrName_GroupName].Value;
            else
                return _CurrentNode.Attributes[_AttrName_NodeName].Value;
        }//getNameForCurrentNode()

        public bool getLockedForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            return CswConvert.ToBoolean( _CurrentNode.Attributes[_AttrName_Locked].Value );
        }//getLockedForCurrentNode()

        public bool getSelectableForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Name != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + _CurrentNode.Name + ") is not a CswNbtNode" ) );

            return Convert.ToBoolean( _CurrentNode.Attributes[_AttrName_Selectable].Value );
        }//getSelectableForCurrentNode()

        public bool getNodeShowInTreeForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            if( _CurrentNode.Name != _ElemName_Node )
                throw ( new CswDniException( "The current node (" + _CurrentNode.Name + ") is not a CswNbtNode" ) );

            return Convert.ToBoolean( _CurrentNode.Attributes[_AttrName_ShowInTree].Value );
        }//getNodeShowInTreeForCurrentNode()


        public int getNodeCountForCurrentLevel()
        {
            return ( _CurrentNode.ParentNode.ChildNodes.Count );
        }//

        public Collection<CswNbtNodeKey> getKeysForNodeId( CswPrimaryKey NodeId )
        {
            Collection<CswNbtNodeKey> NodeInstances = new Collection<CswNbtNodeKey>();
            foreach( CswNbtNodeKey NodeKey in NodesAndParents.Keys )
            {
                if( NodeKey.NodeId == NodeId )
                    NodeInstances.Add( NodeKey );
            }
            return ( NodeInstances );

        }//getKeysForNode()

        public Collection<CswNbtNodeKey> getKeysForLevel( Int32 Level )
        {
            Collection<CswNbtNodeKey> NodeKeys = new Collection<CswNbtNodeKey>();
            foreach( CswNbtNodeKey NodeKey in NodesAndParents.Keys )
            {
                if( NodeKey.TreeDepth == Level )
                    NodeKeys.Add( NodeKey );
            }
            return NodeKeys;
        } // getKeysForLevel()


        public CswNbtNodeKey getParentKey( CswNbtNodeKey ChildKey )
        {
            return NodesAndParents[ChildKey];
        }

        public IEnumerable<XElement> getChildPropNodesOfCurrentNode()
        {
            XElement RawXml = XElement.Parse( _CurrentNode.OuterXml );
            //Collection<XElement> Ret = new Collection<XElement>();

            if( RawXml.HasElements )
            {
                foreach( XElement NodeTypeProp in RawXml.Elements( "NbtNodeProp" ) )
                {
                    yield return NodeTypeProp;
                }

                //Grab all other NbtNodeProp nodes
                foreach( XElement NodeTypeProp in RawXml.DescendantNodesAndSelf().OfType<XElement>().Elements( "NbtNode" ).Elements( "NbtNodeProp" ) )
                {
                    yield return NodeTypeProp;
                }
            }
        }

        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType FieldType )
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

            XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_NodeProp );
            _CurrentNode.AppendChild( NewXmlNode );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropId, NodeTypePropId.ToString() ) );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_JctNodePropId, JctNodePropId.ToString() ) );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropName, Name ) );
            // BZ 7135 - write dates in XML format
            string PropValue = Gestalt;
            switch( FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    if( Gestalt != string.Empty )
                        PropValue = CswTools.ToXmlDateTimeFormat( CswConvert.ToDateTime( Gestalt ) );
                    break;
            }
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropGestalt, PropValue ) );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropFieldType, FieldType.FieldType.ToString() ) );


        }//addProperty()


        #endregion //Modification******************************

        public class CswNbtTreeNodesAttributes //: NodeAttributes
        {

            private CswNbtTreeNodes _CswNbtTreeNodes = null;
            public CswNbtTreeNodesAttributes( CswNbtTreeNodes CswNbtTreeNodes )
            {
                _CswNbtTreeNodes = CswNbtTreeNodes;
            }//ctor

            public string this[string AttributeName]
            {
                get
                {
                    return ( _CswNbtTreeNodes._CurrentNode.Attributes[AttributeName.ToLower()].Value );
                }
            }

        }//CswNbtTreeNodesAttributes


    }//class CswNbtTreeNodes

}//namespace ChemSW.Nbt
