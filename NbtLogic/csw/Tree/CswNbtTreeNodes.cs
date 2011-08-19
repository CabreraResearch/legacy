using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Xml;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

//using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    public class CswNbtTreeNodes
    {
        private CswNbtColumnNames _CswNbtColumnNames = new CswNbtColumnNames();
        private CswNbtResources _CswNbtResources = null;
        //private CswNbtNodeCatalogue _CswNbtNodeCatalogue = new CswNbtNodeCatalogue();
        private XmlDocument _XmlDoc = new XmlDocument();
        private XmlNode _CurrentNode = null;
        CswNbtTreeKey _CswNbtTreeKey = null;

        //private Hashtable _ViewNodeHash = null;

        //        private Hashtable NodeEntries = null;
        private Hashtable NodesAndParents = null;

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
        //private static string _AttrName_NameTemplate = "nametemplate";
        private static string _AttrName_NodeName = "nodename";
        private static string _AttrName_NodeTypeId = "nodetypeid";
        private static string _AttrName_NodeTypeName = "nodetypename";
        private static string _AttrName_ObjectClass = "objectclassname";
        private static string _AttrName_ObjectClassId = "objectclassid";
        //private static string _AttrName_ParentNodeId = "parentnodeid";
        private static string _AttrName_Selectable = "selectable";
        //private static string _AttrName_ShowInGrid = "showingrid";
        private static string _AttrName_ShowInTree = "showintree";
        private static string _AttrName_AddChildren = "addchildren";
        private static string _AttrName_ExpandMode = "expandmode";


        //******************** NbtNodeProp element
        public static string _ElemName_NodeProp = "NbtNodeProp";
        private static string _AttrName_NodePropId = "nodetypepropid";
        private static string _AttrName_NodePropName = "name";
        private static string _AttrName_NodePropGestalt = "gestalt";
        private static string _AttrName_NodePropFieldType = "fieldtype";

        //******************** NbtNodeGroup element
        public static string _ElemName_NodeGroup = "NbtNodeGroup";
        private static string _AttrName_GroupName = "name";
        private static string _AttrName_GroupIcon = "icon";

        //private Int32 _DummyNodeId = -1;

        //private static string _XPathChildren = "NbtNode";

        private XmlNode _TreeNode = null;
        //CswNbtNodes _CswNbtNodes = null;
        CswNbtNodeCollection _CswNbtNodeCollection = null;

        //private CswNbtView _CswNbtView = null;
        //public CswNbtView View 
        //{
        //    get { return _CswNbtView; }
        //    set { _CswNbtView = value; }
        //}

        public CswNbtTreeNodes( CswNbtTreeKey CswNbtTreeKey, string XslFilePath, string TreeName, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection )
        //public CswNbtTreeNodes( CswNbtTreeKey CswNbtTreeKey, string XslFilePath, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection)
        {
            //            NodeEntries = new Hashtable();
            NodesAndParents = new Hashtable();

            _CswNbtTreeKey = CswNbtTreeKey;

            //Make Tree Node*******************************************
            _TreeNode = _XmlDoc.CreateElement( _ElemName_Tree );
            _XmlDoc.AppendChild( _TreeNode );

            _TreeNode.Attributes.Append( _XmlDoc.CreateAttribute( _AttrName_TreeName ) );
            _TreeNode.Attributes[_AttrName_TreeName].Value = TreeName;
            CswDelimitedString Path = _makePathToNode( null, _TreeNode );
            CswNbtNodeKey NodeKey = _makeNodeEntry( null, _TreeNode, Path, null, new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter ), NodeSpecies.UnKnown );
            _TreeNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );

            ////Make Root Node*******************************************
            //XmlNode EmptyRootNode = _XmlDoc.CreateElement( _ElemName_Node );
            //TreeNode.AppendChild( EmptyRootNode );

            //EmptyRootNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, ViewName ) );
            //EmptyRootNode.Attributes.Append(_makeAttribute(_AttrName_NodeId, "0"));
            //EmptyRootNode.Attributes.Append(_makeAttribute(_AttrName_NodeTypeId, "0"));
            //EmptyRootNode.Attributes.Append(_makeAttribute(_AttrName_IconFileName, "view/view.gif"));
            //Path = "";
            //_makePathToNode( EmptyRootNode, ref Path );
            //CswNbtNodeKey = _makeNodeKey( EmptyRootNode, Path );
            //EmptyRootNode.Attributes.Append( _makeAttribute( _AttrName_Key, CswNbtNodeKey.ToString() ) );


            //            goToRoot();

            _CswNbtResources = CswNbtResources;

            //_CurrentNodeAtributes = new CswNbtTreeNodesAttributes( this );

            //_CswNbtNodes = CswNbtNodes;
            _CswNbtNodeCollection = CswNbtNodeCollection;

            //NodeKeyList = new ArrayList();

            //_ViewNodeHash = new Hashtable();

        }//ctor

        //private ArrayList NodeKeyList;

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
            //string Path = "";
            //_makePathToNode( ParentNodeKey != null ? ParentNodeKey.TreePath : string.Empty, MoreNode, ref Path );

            XmlNode ParentXmlNode = null;
            if( _CurrentNode != null )
                ParentXmlNode = _CurrentNode;
            else
                ParentXmlNode = _TreeNode;
            CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );
            CswDelimitedString Path = _makePathToNode( ThisParentKey.TreePath, MoreNode );

            CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
            if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
            CountPath.Add( NodeCount.ToString() );

            CswNbtNodeKey NodeKey = _makeNodeEntry( ParentNodeKey, MoreNode, Path, ViewNode, CountPath, NodeSpecies.More );

            MoreNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
            //MoreNode.Attributes.Append( _makeAttribute( _AttrName_ShowInGrid, "false" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, "None" ) );
            MoreNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );
        }

        public void makeRootNode( string ViewName, string IconFileName, bool Selectable )//, NbtViewAddChildrenSetting AddChildren)
        {
            _makeRootNode( ViewName, null, IconFileName, Selectable ); //, AddChildren);
        }
        public void makeRootNode( string ViewName, CswNbtViewRoot ViewRoot )
        {
            _makeRootNode( ViewName, ViewRoot, ViewRoot.IconFileName, ViewRoot.Selectable ); //, ViewRoot.AddChildren);        
        }
        private void _makeRootNode( string ViewName, CswNbtViewRoot ViewRoot, string IconFileName, bool Selectable ) //, NbtViewAddChildrenSetting AddChildren )
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
                CswDelimitedString Path = _makePathToNode( null, _RootNode );
                CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                CountPath.Add( "0" );
                CswNbtNodeKey NodeKey = _makeNodeEntry( null, _RootNode, Path, ViewRoot, CountPath, NodeSpecies.Root );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_Key, NodeKey.ToString() ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_ShowInGrid, "false" ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, "true" ) );
                //_RootNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, AddChildren.ToString() ) );
                _RootNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );  // false is on purpose - prevents load on demand

                //if (ViewRoot != null)
                //{
                //    _ViewNodeHash.Add(CswNbtNodeKey, ViewRoot);
                //}
            }
            else
            {
                throw new CswDniException( "CswNbtTreeNodes attempted to add a second root node to the tree" );
            }
        }


        public string getRawXml()
        {
            return ( _XmlDoc.InnerXml );
        }//


        public bool isNodeDefined( CswNbtNodeKey NodeKey )
        {
            return ( null != _XmlDoc.SelectSingleNode( NodeKey.TreePath.ToString() ) );
        }

        private XmlNode _getXmlNodeFromPath( CswDelimitedString Path )
        {
            XmlNode SelectedNode = null;

            // Don't throw -- just return null.
            //if( null == ( SelectedNode = _XmlDoc.SelectSingleNode( Path ) ) )
            //    throw ( new CswDniException( "No node matches this path: " + Path ) );
            SelectedNode = _XmlDoc.SelectSingleNode( Path.ToString(false) );

            return ( SelectedNode );

        }//_getXmlNodeFromPath()

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
        //private CswNbtNodeContext _makeEmptyNodeContext()
        //{
        //    CswNbtNodeContext ReturnVal = new CswNbtNodeContext(_CswNbtResources);
        //    ReturnVal.TreeKey = _CswNbtTreeKey;
        //    return (ReturnVal);
        //}
        //private CswNbtNodeContext _makeNodeContext(CswNbtNodeKey NodeKey, CswNbtViewNode ViewNode, Int32 NodeCount)
        //{
        //    return new CswNbtNodeContext(_CswNbtResources, NodeKey.TreeKey, NodeKey.TreePath, NodeKey.NodeId, NodeKey.NodeTypeId, NodeKey.ObjectClassId, NodeKey.NodeSpecies, ViewNode, NodeCount);
        //}


        private CswNbtNode _getNbtNodeObjFromXmlNode( XmlNode XmlNode )
        {
            if( _ElemName_Node != XmlNode.Name )
                throw ( new CswDniException( "The current node is a " + XmlNode.Name + ", not an NbtNode" ) );

            CswNbtNodeKey NodeKey = _makeNodeKeyFromString( XmlNode.Attributes[_AttrName_Key].Value );

            //CswNbtNode ReturnVal = _CswNbtNodeCollection.makeEmptyNode(CswConvert.ToInt32( XmlNode.Attributes[ _AttrName_NodeId ].Value ), 
            //                                                           CswConvert.ToInt32( XmlNode.Attributes[ _AttrName_NodeTypeId ].Value ),
            //                                                           CswNbtNodeKey.NodeSpecies);

            CswNbtNode ReturnVal = _CswNbtNodeCollection.GetNode( new CswPrimaryKey( XmlNode.Attributes[_AttrName_TableName].Value, CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeId].Value ) ) );

            //ReturnVal.NodeId = CswConvert.ToInt32( XmlNode.Attributes[ _AttrName_NodeId ].Value );
            //ReturnVal.NodeKey = CswNbtNodeKey;
            //ReturnVal.NodeKey.TreeKey = _CswNbtTreeKey;

            if( NodeSpecies.Plain == NodeKey.NodeSpecies )
            {
                ReturnVal.IconFileName = "Images/icons/" + XmlNode.Attributes[_AttrName_IconFileName].Value;
                //ReturnVal.NameTemplate = XmlNode.Attributes[ _AttrName_NameTemplate ].Value;
                ReturnVal.NodeName = XmlNode.Attributes[_AttrName_NodeName].Value;
                ReturnVal.NodeTypeId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_NodeTypeId].Value );
                //ReturnVal.NodeTypeName = XmlNode.Attributes[ _AttrName_NodeTypeName ].Value;
                //ReturnVal.ObjectClass = _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.getObjectClassFromString(XmlNode.Attributes[_AttrName_ObjectClass].Value));
                ReturnVal.ObjectClassId = CswConvert.ToInt32( XmlNode.Attributes[_AttrName_ObjectClassId].Value );
                //ReturnVal.Modified = false; //bz #5943
            }
            if( XmlNode.Attributes[_AttrName_Selectable] != null )
                ReturnVal.Selectable = Convert.ToBoolean( XmlNode.Attributes[_AttrName_Selectable].Value.ToLower() );
            //if( XmlNode.Attributes[_AttrName_ShowInGrid] != null )
            //    ReturnVal.ShowInGrid = Convert.ToBoolean( XmlNode.Attributes[_AttrName_ShowInGrid].Value.ToLower() );
            if( XmlNode.Attributes[_AttrName_ShowInTree] != null )
                ReturnVal.ShowInTree = Convert.ToBoolean( XmlNode.Attributes[_AttrName_ShowInTree].Value.ToLower() );
            //if (XmlNode.Attributes[_AttrName_AddChildren] != null)
            //    ReturnVal.AddChildren = (CswNbtView.AddChildrenSetting)Enum.Parse(typeof(CswNbtView.AddChildrenSetting), XmlNode.Attributes[_AttrName_AddChildren].Value);


            //if (_ViewNodeHash[CswNbtNodeKey] != null)
            //{
            //    ReturnVal.ViewNode = (CswNbtViewNode)_ViewNodeHash[CswNbtNodeKey];
            //}


            return ( ReturnVal );
        }//

        public CswNbtNode getNode( CswNbtNodeKey NodeKey )
        {

            return ( _getNbtNodeObjFromXmlNode( _getXmlNodeFromPath( NodeKey.TreePath ) ) );

        }//getNode()

        public CswNbtNode getParentNodeOf( CswNbtNodeKey NodeKey )
        {
            CswNbtNode ReturnVal = null;

            XmlNode CurrentNodeSave = _CurrentNode;

            makeNodeCurrent( NodeKey );

            if( !isCurrentPositionRoot() )
            {
                ReturnVal = _getNbtNodeObjFromXmlNode( _getXmlNodeFromPath( getNodeKeyForParentOfCurrentPosition().TreePath ) );
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

            //_CurrentNode = _CurrentNode.SelectNodes( _XPathChildren )[ChildN];
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
            makeNodeCurrent( NodeKey.TreePath );
        }//makeNodeCurrent() 

        public void makeNodeCurrent( CswDelimitedString TreePath )
        {
            _CurrentNode = _getXmlNodeFromPath( TreePath );
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
            // return _CurrentNode.SelectNodes( _XPathChildren ).Count;
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


        //public CswNbtNodeContext getNodeContext(CswNbtNodeKey Key)
        //{
        //    if (NodeEntries.ContainsKey(Key))
        //        return (CswNbtNodeContext)NodeEntries[Key];
        //    else
        //        throw new CswDniException("Could not find match", "CswNbtTreeNodes.getNodeContext() could not find NodeContext for Key: " + Key.ToString());
        //}


        #endregion //NavigationAndInterrogation******************************


        //Modification methods*****************************************
        #region Modification Methods



        private XmlNode _makeXmlNodeWithValue( string ElemName, string Value )
        {
            XmlNode ReturnVal = _XmlDoc.CreateElement( ElemName );
            ReturnVal.Value = Value;
            return ( ReturnVal );
        }//

        private CswNbtNodeKey _makeNodeEntry( CswNbtNodeKey ParentNodeKey, XmlNode XmlNode, CswDelimitedString Path, CswNbtViewNode ViewNode, CswDelimitedString NodeCountPath, NodeSpecies Species )
        {
            CswNbtNodeKey NewNodeKey = _makeEmptyNodeKey();

            NewNodeKey.TreePath = Path;
            NewNodeKey.NodeSpecies = Species;
            NewNodeKey.NodeCountPath = NodeCountPath;

            if( _ElemName_Node == XmlNode.Name )
            {
                //if (_RootNode != XmlNode)
                //    NewNodeKey.NodeSpecies = NodeSpecies.Plain;
                //else
                //    NewNodeKey.NodeSpecies = NodeSpecies.Placeholder;

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
                //NewNodeKey.NodeSpecies = NodeSpecies.Placeholder;
                //else
                throw ( new CswDniException( "Unknown element: " + XmlNode.Name ) );


            //CswNbtNodeContext NewNodeContext = _makeNodeContext(NewNodeKey, ViewNode, NodeCount);

            //            NodeEntries.Add(NewNodeKey, NewNodeContext);
            if( !NodesAndParents.ContainsKey( NewNodeKey ) )
                NodesAndParents.Add( NewNodeKey, ParentNodeKey );

            return ( NewNodeKey );
        }

        //        private CswNbtNodeKey _makeNodeKey( XmlNode XmlNode , string Path, CswNbtViewNode ViewNode )
        //        {

        //            CswNbtNodeKey ReturnVal = _makeEmptyNodeKey();
        //            if( _ElemName_Node == XmlNode.Name )
        //            {
        //                if( _RootNode != XmlNode )
        //                {
        //                    ReturnVal.NodeSpecies = NodeSpecies.Plain;
        //                }
        //                else
        //                {
        //                    ReturnVal.NodeSpecies = NodeSpecies.Placeholder;
        //                }

        //                ReturnVal.NodeId = CswConvert.ToInt32( XmlNode.Attributes[ _AttrName_NodeId ].Value );
        //                ReturnVal.NodeTypeId = CswConvert.ToInt32(XmlNode.Attributes[_AttrName_NodeTypeId].Value);
        //                ReturnVal.ObjectClassId = CswConvert.ToInt32( XmlNode.Attributes[ _AttrName_ObjectClassId ].Value );
        //            }
        //            else if( _ElemName_Tree == XmlNode.Name )
        //            {
        //                ReturnVal.NodeSpecies = NodeSpecies.Placeholder;
        //            }
        //            else if( _ElemName_NodeGroup == XmlNode.Name )
        //            {
        //                ReturnVal.NodeSpecies = NodeSpecies.Placeholder;
        //            }
        //            else
        //            {
        //                throw( new CswDniException( "Unknown element: " +  XmlNode.Name ) );
        //            }//
        //            ReturnVal.ViewNode = ViewNode;

        ////            ReturnVal.TreeId = _TreeNode.Name;
        //            ReturnVal.TreePath = Path;
        //            //ReturnVal.NodeSource = NodeSource.InCurrentTree;


        //            return( ReturnVal );

        //        }//_makeNodeKey()

        private XmlAttribute _makeAttribute( string AttrName, string AttrVal )
        {
            XmlAttribute ReturnVal = _XmlDoc.CreateAttribute( AttrName );
            ReturnVal.Value = AttrVal;
            return ( ReturnVal );
        }//


        //public void addNodeFromKey(CswNbtView View, ref CswNbtNodeKey Key)
        //{
        //    CswNbtViewNode ViewNode = View.FindViewNodeByUniqueId(Key.ViewNodeUniqueId);
        //    if(!(ViewNode is CswNbtViewRelationship))
        //        throw new CswDniException("addNodeFromKey expected ViewNode to be a Relationship");
        //    CswNbtViewRelationship Relationship = (CswNbtViewRelationship)ViewNode;
        //    CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType(Key.NodeTypeId);

        //    // This path is wrong for the tree being built (so MakeNodeCurrent() won't work right), 
        //    // but the path is right for the actual tree to which these nodes are being added.
        //    // This might lead to problems down the line, but for now it's the right thing.
        //    string NewPath = Key.TreePath.Substring(0, Key.TreePath.LastIndexOf('/'));
        //    Key = _loadNodeAsChild(null, NewPath, string.Empty, Relationship, Relationship.Selectable, Relationship.ShowInGrid, Relationship.AddChildren, Key.NodeCount,
        //                           Relationship.IconFileName, MetaDataNodeType.NameTemplate, Key.NodeId, string.Empty, Key.NodeTypeId, MetaDataNodeType.NodeTypeName, MetaDataNodeType.ObjectClass.ObjectClassId, MetaDataNodeType.ObjectClass.ObjectClass.ToString());
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
                                     DataRowToAdd[_CswNbtColumnNames.ObjectClassName.ToLower()].ToString()
                                   );
        }

        private Collection<CswNbtNodeKey> _loadNodeAsChild( CswNbtNodeKey ParentNodeKey, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship,
                                               bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount,
                                               string IconFileName, string NameTemplate, CswPrimaryKey NodeId, string NodeName, Int32 NodeTypeId,
                                               string NodeTypeName, Int32 ObjectClassId, string ObjectClassName )
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

                        ParentXmlNode.AppendChild( NewGroupNode );
                        ParentNodes.Add( NewGroupNode );

                        CswDelimitedString GroupCountPath = new CswDelimitedString(CswNbtNodeKey.NodeCountDelimiter);
                        CswDelimitedString GroupParentTreePath = new CswDelimitedString(CswNbtNodeKey.TreePathDelimiter);
                        CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ParentXmlNode.Attributes[_AttrName_Key].Value.ToString() );
                        if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                        {
                            GroupCountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                            GroupParentTreePath.FromDelimitedString( ThisParentKey.TreePath );
                        }
                        XmlNodeList AllGroupList = ParentXmlNode.SelectNodes( _ElemName_NodeGroup );
                        GroupCountPath.Add( ( AllGroupList.Count + 1 ).ToString() );

                        CswDelimitedString GroupTreePath = _makePathToNode( GroupParentTreePath, NewGroupNode );

                        CswNbtNodeKey GroupKey = _makeNodeEntry( ParentNodeKey, NewGroupNode, GroupTreePath, Relationship, GroupCountPath, NodeSpecies.Group );
                        NewGroupNode.Attributes.Append( _makeAttribute( _AttrName_Key, GroupKey.ToString() ) );
                    }
                }
                while( GroupNameForLoop != string.Empty );
            }


            foreach( XmlNode ThisParentNode in ParentNodes )
            {
                XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_Node );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_IconFileName, "Images/icons/" + IconFileName ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NameTemplate, NameTemplate ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_TableName, NodeId.TableName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeId, NodeId.PrimaryKey.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeName, NodeName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeId, NodeTypeId.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodeTypeName, NodeTypeName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClassId, ObjectClassId.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ObjectClass, ObjectClassName ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_Selectable, Selectable.ToString().ToLower() ) );
                //NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ShowInGrid, ShowInGrid.ToString().ToLower() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ShowInTree, ShowInTree.ToString().ToLower() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_AddChildren, AddChildren.ToString() ) );
                NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_ExpandMode, "ClientSide" ) );

                ThisParentNode.AppendChild( NewXmlNode );
                CswNbtNodeKey ThisParentKey = new CswNbtNodeKey( _CswNbtResources, ThisParentNode.Attributes[_AttrName_Key].Value.ToString() );

                CswDelimitedString Path = _makePathToNode( ThisParentKey.TreePath, NewXmlNode );

                CswDelimitedString CountPath = new CswDelimitedString( CswNbtNodeKey.NodeCountDelimiter );
                if( ThisParentKey != null && ThisParentKey.NodeCountPath.Count > 0 )
                    CountPath.FromDelimitedString( ThisParentKey.NodeCountPath );
                CountPath.Add( RowCount.ToString() );

                CswNbtNodeKey ThisKey = _makeNodeEntry( ThisParentKey, NewXmlNode, Path, Relationship, CountPath, NodeSpecies.Plain );
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


        private CswDelimitedString _makePathToNode( CswDelimitedString ParentNodeTreePath, XmlNode XmlNode )
        {
            CswDelimitedString Path = new CswDelimitedString( CswNbtNodeKey.TreePathDelimiter );

            if( XmlNode != _XmlDoc.ChildNodes[0] )
            {
                string NewSegment = XmlNode.Name;
                if( _ElemName_Node == XmlNode.Name && XmlNode != _RootNode )
                {
                    NewSegment += "[@" + _AttrName_TableName + "='" + XmlNode.Attributes[_AttrName_TableName].Value + "' and ";
                    NewSegment += " @" + _AttrName_NodeId + "=" + XmlNode.Attributes[_AttrName_NodeId].Value + "]";
                }
                if( _ElemName_NodeGroup == XmlNode.Name )
                    NewSegment += "[@" + _AttrName_GroupName + "='" + XmlNode.Attributes[_AttrName_GroupName].Value + "']";


                if( ParentNodeTreePath != null && ParentNodeTreePath.Count > 0 )
                {
                    Path.FromDelimitedString( ParentNodeTreePath );
                    Path.Add( NewSegment );
                }
                else
                {
                    Path = _makePathToNode( null, XmlNode.ParentNode );
                    Path.Add( NewSegment );
                }
            }
            else
            {
                Path.Add( "" );  // should start with a delimiter
                Path.Add( XmlNode.Name );
            }
            return Path;
        }//_makePathToNode()

        //private static string _makeNodePathSegment( string Attribute, string Value )
        //{
        //    return ( "[@" + Attribute + "=" + Value + "]" );
        //}//_makeNodePathSegment()

        public static CswDelimitedString makeProspectiveNodePath( CswNbtNodeKey ParentKey, CswNbtNode ChildNode )
        {
            CswDelimitedString Path = ParentKey.TreePath;
            if( ChildNode.NodeId != null )
            {
                if( Path.Count == 0 )
                    Path.Add( "" ); // string should start with a delimiter

                Path.Add( _ElemName_Node + "[@" + _AttrName_TableName + "='" + ChildNode.NodeId.TableName + "' and " + _AttrName_NodeId + "=" + ChildNode.NodeId.PrimaryKey.ToString() + "]" );
            }
            return Path;
        }//makeProspectiveNodePath()


        //We cache paths as we build them so that if a path is needed
        //more than once it's less expensive to get it

        /*
        private void _getPathToNode( XmlNode XmlNode, ref string Path )
        {
            XmlNode PathNode =null;
            if( null == ( PathNode = XmlNode.SelectSingleNode( _ElemName_PathToParentElement ) ) ) 
            {
                _makePathToNode( XmlNode, ref Path );
                XmlNode NewPathNode = _makeXmlNodeWithValue( _ElemName_PathToParentElement, Path );
            }
            else
            {
                Path = PathNode.Value;
            }//

        }//_getPathToNode()
         */


        private string _getValFromChild( XmlNode XmlNode, string ChildNodeName )
        {
            XmlNode ChildNode = XmlNode.SelectSingleNode( ChildNodeName );
            if( null == ChildNode )
                throw ( new CswDniException( "The " + XmlNode.Name + "does not have the requested child element: " + ChildNodeName ) );

            return ( ChildNode.InnerText );

        }//_getValFromChild()

        public CswNbtNodeKey getKeyForCurrentNode()
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );

			//if( _CurrentNode.Name != _ElemName_Node )
			//    throw ( new CswDniException( "The current node (" + _CurrentNode.Name + ") is not a CswNbtNode" ) );

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

			//if( _CurrentNode.Name != _ElemName_Node )
			//    throw ( new CswDniException( "The current node (" + _CurrentNode.Name + ") is not a CswNbtNode" ) );

			if( _CurrentNode.Name == _ElemName_NodeGroup )
				return _CurrentNode.Attributes[_AttrName_GroupName].Value;
			else
				return _CurrentNode.Attributes[_AttrName_NodeName].Value;
		}//getNameForCurrentNode()

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

        public IEnumerable getKeysForNodeId( CswPrimaryKey NodeId )
        {
            ArrayList NodeInstances = new ArrayList();
            //_CswNbtNodeCatalogue.getKeysForNodeId(NodeId, ref NodeInstances);
            foreach( CswNbtNodeKey NodeKey in NodesAndParents.Keys )
            {
                if( NodeKey.NodeId == NodeId )
                    NodeInstances.Add( NodeKey );
            }
            return ( NodeInstances );

        }//getKeysForNode()

        public CswNbtNodeKey getParentKey( CswNbtNodeKey ChildKey )
        {
            return (CswNbtNodeKey) NodesAndParents[ChildKey];
        }

        //public IEnumerable getKeysForNodeKey(CswNbtNodeKey CswNbtNodeKey)
        //{
        //    ArrayList NodeKeyInstances = new ArrayList();
        //    _CswNbtNodeCatalogue.getKeysForNodeId( CswNbtNodeKey.NodeId, ref NodeKeyInstances );
        //    return ( NodeKeyInstances );

        //}//getKeysForNode()

        //public CswNbtNodeKey loadNodeAsChildFromDb( System.Int32 NodeId, string GroupName )
        //{
        //    CswQueryCaddy CswQueryCaddy = new CswQueryCaddy( _CswNbtResources.CswDbResources );
        //    CswQueryCaddy.S4Name = "getsinglenode";
        //    CswQueryCaddy.S4Parameters.Add( "getnodeid", NodeId.ToString() );

        //    DataTable ResultTable = CswQueryCaddy.Table;

        //    if( ResultTable.Rows.Count <= 0 )
        //        throw ( new System.Exception( "There is no node for NodeId " + NodeId.ToString() ) );

        //    if( ResultTable.Rows.Count > 1 )
        //        throw ( new System.Exception( "There are more than one nodes corresponding to NodeId " + NodeId.ToString() ) );

        //    return ( loadNodeAsChildFromRow( ResultTable.Rows[ 0 ], GroupName, true, true, CswNbtView.AddChildrenSetting.None ) );

        //}//loadNodeAsChildFromDb() 


        private bool _isItemInCsvAttr( XmlAttribute XmlAttribute, string Value )
        {
            bool ReturnVal = false;
            if( XmlAttribute.Value.Length > 0 )
            {
                string[] ItemList = XmlAttribute.Value.Split( ',' );
                bool ItemNotFound = true;
                for( int idx = 0; ItemNotFound && ( idx < ItemList.Length ); idx++ )
                {
                    if( ItemList[idx] == Value )
                    {
                        ReturnVal = true;
                        ItemNotFound = false;
                    }
                }//iterate items
            }//if there's any item

            return ( ReturnVal );
        }//_isItemInCsvAttr()

        private void _addItemToCsvAttr( XmlAttribute XmlAttribute, string Value )
        {
            if( XmlAttribute.Value.Length > 0 )
            {
                XmlAttribute.Value += ",";
            }

            XmlAttribute.Value += Value;
        }//




        public void addProperty( Int32 NodeTypePropId, string Name, string Gestalt, CswNbtMetaDataFieldType FieldType )
        {
            if( null == _CurrentNode )
                throw ( new CswDniException( "There is no current node" ) );
            
            XmlNode NewXmlNode = _XmlDoc.CreateElement( _ElemName_NodeProp );
            _CurrentNode.AppendChild( NewXmlNode );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropId, NodeTypePropId.ToString() ) );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropName, Name ) );
            // BZ 7135 - write dates in XML format
            string PropValue = Gestalt;
            switch( FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    if( Gestalt != string.Empty )
                        PropValue = CswTools.ToXmlDateTimeFormat( Convert.ToDateTime( Gestalt ) );
                    break;
				//case CswNbtMetaDataFieldType.NbtFieldType.Time:
				//    if( Gestalt != string.Empty )
				//        PropValue = CswTools.ToXmlTimeFormat( Convert.ToDateTime( Gestalt ) );
				//    break;
            }
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropGestalt, PropValue ) );
            NewXmlNode.Attributes.Append( _makeAttribute( _AttrName_NodePropFieldType, FieldType.FieldType.ToString() ) );


        }//addProperty()


        #endregion //Modification******************************



        //private NodeAttributes _CurrentNodeAtributes = null;
        //public NodeAttributes CurrentNodeAtributes { get{ return(_CurrentNodeAtributes); } }
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
                }//
            }

        }//CswNbtTreeNodesAttributes


    }//class CswNbtTreeNodes

}//namespace ChemSW.Nbt
