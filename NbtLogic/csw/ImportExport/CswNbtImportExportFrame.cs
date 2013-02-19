using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ImportExport
{
    /// <summary>
    /// Defines the structure of an import/export xml document
    /// </summary>
    public class CswNbtImportExportFrame
    {

        #region XML Elements and Attributes

        /// <summary>
        /// XML Element: NbtData
        /// </summary>
        public static string _Element_NbtData = "NbtData";
        /// <summary>
        /// XML Element: ViewInfo
        /// </summary>
        public static string _Element_Views = "Views";
        /// <summary>
        /// XML Element: ViewInfo
        /// </summary>
        public static string _Element_ViewInfo = "ViewInfo";
        /// <summary>
        /// XML Element: ViewName
        /// </summary>
        public static string _Element_ViewName = "ViewName";
        /// <summary>
        /// XML Element: ViewId
        /// </summary>
        public static string _Element_ViewId = "ViewId";
        /// <summary>
        /// XML Element: ViewXml
        /// </summary>
        public static string _Element_ViewXml = "ViewXml";
        /// <summary>
        /// XML Element: AccessId
        /// </summary>
        public static string _Element_AccessId = "AccessId";
        /// <summary>
        /// XML Element: UserName
        /// </summary>
        public static string _Element_UserName = "UserName";
        /// <summary>
        /// XML Element: DateTime
        /// </summary>
        public static string _Element_DateTime = "DateTime";
        /// <summary>
        /// XML Element: HandHeldData
        /// </summary>
        public static string _Element_MobileData = "HandHeldData";
        ///// <summary>
        ///// XML Element: MetaData
        ///// </summary>
        //public static string _Element_MetaData = "MetaData";
        /// <summary>
        /// XML Element: Nodes
        /// </summary>
        public static string _Element_Nodes = "Nodes";
        /// <summary>
        /// XML Element: Node
        /// </summary>
        public static string _Element_Node = "Node";
        /// <summary>
        /// XML Element: PropValue
        /// </summary>
        public static string _Element_PropValue = "PropValue";
        /// <summary>
        /// Version of Import/Export XML document
        /// </summary>
        public static string _Element_Version = "Version";

        /// <summary>
        /// XML Attribute: NodeId
        /// </summary>
        public static string _Attribute_NodeId = "nodeid";
        /// <summary>
        /// XML Attribute: NodeName
        /// </summary>
        public static string _Attribute_NodeName = "nodename";
        /// <summary>
        /// XML Attribute: NodeTypeId
        /// </summary>
        public static string _Attribute_NodeTypeId = "nodetypeid";
        /// <summary>
        /// XML Attribute: NodeTypeName
        /// </summary>
        public static string _Attribute_NodeTypeName = "nodetypename";

        #endregion XML Elements and Attributes

        // When we make changes to the structure of this document, we need to handle version issues
        private Int32 _Version = 1;


        private CswNbtResources _CswNbtResources;

        private XmlElement _RootNode;
        private XmlNode _MetaDataNode;
        private XmlNode _ViewsNode;
        private XmlNode _NodesNode;

        #region Constructors


        private XmlDocument __XmlDoc = null;
        private XmlDocument _XmlDoc
        {
            get
            {
                if( null == __XmlDoc )
                {
                    Stream FileStream = File.OpenRead( _FilePath );
                    StreamReader FileSR = new StreamReader( FileStream );
                    string FileContents = FileSR.ReadToEnd();
                    FileStream.Close();

                    __XmlDoc = new XmlDocument();
                    __XmlDoc.InnerXml = FileContents;
                }

                return ( __XmlDoc );
            }
        }



        /// <summary>
        /// Constructor for brand new empty XML document
        /// </summary>
        public CswNbtImportExportFrame( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            __XmlDoc = new XmlDocument();
            _RootNode = CswXmlDocument.SetDocumentElement( _XmlDoc, _Element_NbtData );
            CswXmlDocument.AppendXmlNode( _RootNode, _Element_AccessId, _CswNbtResources.AccessId.ToString() );
            CswXmlDocument.AppendXmlNode( _RootNode, _Element_UserName, _CswNbtResources.CurrentUser.Username );
            CswXmlDocument.AppendXmlNode( _RootNode, _Element_DateTime, DateTime.Now.ToString() );
            CswXmlDocument.AppendXmlNode( _RootNode, _Element_Version, _Version.ToString() );
        }

        /// <summary>
        /// Constructor for an existing XML document in string format
        /// </summary>
        public CswNbtImportExportFrame( CswNbtResources CswNbtResources, string FilePath )
        {

            _CswNbtResources = CswNbtResources;
            _FilePath = FilePath;

        }

        public void clear()
        {
            // For the momemnt this funciton royally breaks things if you intended to keep using the XmlDoc. 
            // I am putting this in for now in a rather raw way just to see if I can get the GC to free up 
            // the memory that is taken up by a ginormous document. Long term, we will need to refactor so that
            // we are not publically exposing the XML: once we hand out a reference, we have no way of letting the GC
            // know that he should free up the memory, because every Tom Dick and Harry caller could have is own 
            // reference to it, and the reference count will never go to zero.
            __XmlDoc = null;
        }//clear() 

        #endregion Constructors


        #region Modifying XML

        public void AddNodeType( CswNbtMetaDataNodeType NodeType )
        {
            if( _MetaDataNode == null )
                _MetaDataNode = CswXmlDocument.AppendXmlNode( _RootNode, CswNbtMetaData._Element_MetaData );
            _MetaDataNode.InnerXml += NodeType.ToXml( null, false, false ).InnerXml;
        }
        public void AddView( CswNbtView View )
        {
            if( _ViewsNode == null )
                _ViewsNode = CswXmlDocument.AppendXmlNode( _RootNode, _Element_Views );

            XmlNode ViewInfoNode = CswXmlDocument.AppendXmlNode( _ViewsNode, _Element_ViewInfo );
            CswXmlDocument.AppendXmlNode( ViewInfoNode, _Element_ViewName, View.ViewName );
            CswXmlDocument.AppendXmlNode( ViewInfoNode, _Element_ViewId, View.ViewId.ToString() );
            XmlNode ViewXmlNode = CswXmlDocument.AppendXmlNode( ViewInfoNode, _Element_ViewXml );
            CswXmlDocument.SetInnerTextAsCData( ViewXmlNode, View.ToString() );
        }

        public void AddNode( CswNbtNode Node )
        {
            if( _NodesNode == null )
                _NodesNode = CswXmlDocument.AppendXmlNode( _RootNode, _Element_Nodes );

            _makeNodeXmlRecursive( null, _NodesNode, null, Node, false );
        }

        public void AddTree( CswNbtView View, ICswNbtTree Tree, bool PropsInViewOnly )
        {
            if( _NodesNode == null )
                _NodesNode = CswXmlDocument.AppendXmlNode( _RootNode, _Element_Nodes );

            for( int i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );
                _makeNodeXmlRecursive( View, _NodesNode, Tree, Tree.getNodeForCurrentPosition(), PropsInViewOnly );
                Tree.goToParentNode();
            }
        }

        private void _makeNodeXmlRecursive( CswNbtView View, XmlNode ParentNode, ICswNbtTree CswNbtTree, CswNbtNode TreeNode, bool PropsInViewOnly )
        {
            // node xml
            XmlNode NodeNode = CswXmlDocument.AppendXmlNode( ParentNode, _Element_Node );
            CswXmlDocument.AppendXmlAttribute( NodeNode, _Attribute_NodeId, TreeNode.NodeId.PrimaryKey.ToString() );
            CswXmlDocument.AppendXmlAttribute( NodeNode, _Attribute_NodeName, TreeNode.NodeName );
            CswXmlDocument.AppendXmlAttribute( NodeNode, _Attribute_NodeTypeId, TreeNode.NodeTypeId.ToString() );
            //CswXmlDocument.AppendXmlAttribute( NodeNode, _Attribute_NodeTypeName, TreeNode.NodeTypeName );

            // prop xml
            foreach( CswNbtNodePropWrapper PropWrapper in TreeNode.Properties )
            {
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropWrapper.NodeTypePropId );
                if( !PropsInViewOnly || ( View != null && View.ContainsNodeTypeProp( Prop ) ) )
                {
                    XmlNode PropValueNode = CswXmlDocument.AppendXmlNode( NodeNode, _Element_PropValue );
                    CswXmlDocument.AppendXmlAttribute( PropValueNode, _Attribute_NodeId, TreeNode.NodeId.PrimaryKey.ToString() );
                    CswXmlDocument.AppendXmlAttribute( PropValueNode, CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropId, PropWrapper.NodeTypePropId.ToString() );
                    //CswXmlDocument.AppendXmlAttribute( PropValueNode, CswNbtMetaDataNodeTypeProp._Attribute_NodeTypePropName, PropWrapper.PropName.ToString() );
                    CswXmlDocument.AppendXmlAttribute( PropValueNode, CswNbtMetaDataNodeTypeProp._Attribute_JctNodePropId, PropWrapper.JctNodePropId.ToString() );

                    PropWrapper.ToXml( PropValueNode );
                }
            } // foreach( CswNbtNodePropWrapper PropWrapper in TreeNode.Properties )

            // Recurse
            if( CswNbtTree != null )
            {
                for( int i = 0; i < CswNbtTree.getChildNodeCount(); i++ )
                {
                    CswNbtTree.goToNthChild( i );
                    _makeNodeXmlRecursive( View, NodeNode, CswNbtTree, CswNbtTree.getNodeForCurrentPosition(), PropsInViewOnly );
                    CswNbtTree.goToParentNode();
                }
            }
        }


        public void replaceNodeIdReferenceValues( string OldValue, string NewValue )
        {
            string Xpath = "//IMCSProData/Nodes/Node/PropValue/NodeID[.='" + OldValue + "']";
            //string Xpath = "//IMCSProData/Nodes/Node/PropValue/NodeID";
            XmlNodeList TargetNodeIdElements = CswXmlDocument.getNodeListFromArbitraryXpath( _XmlDoc, Xpath );
        }
        #endregion Interacting with XML

        #region Reading XML


        public Dictionary<string, string> NodeTypes
        {
            get
            {
                Dictionary<string, string> ReturnVal = new Dictionary<string, string>();

                XmlNodeList XmlNodeList = CswXmlDocument.getNodesWithUniqueAttributeValues( _XmlDoc, "//IMCSProData/Nodes/Node", "Node", "nodetypename" );
                return ( ReturnVal );
            }//get
        }


        private XmlNodeList _Nodes = null;
        public XmlNodeList Nodes
        {
            get
            {
                if( null == _Nodes )
                {
                    _Nodes = CswXmlDocument.getNodeListFromArbitraryXpath( _XmlDoc, "//IMCSProData/Nodes/Node" );
                }

                return ( _Nodes );

            }//get

        }//Nodes

        #endregion Reading XML


        #region Output

        /// <summary>
        /// Get the XML Document
        /// </summary>
        public XmlDocument AsXmlDoc()
        {
            return _XmlDoc;
        }

        /// <summary>
        /// Get the XML Document as a string
        /// </summary>
        public string AsString()
        {
            return _XmlDoc.InnerXml;
        }

        /// <summary>
        /// Get the XML Document as a dataset
        /// </summary>
        public DataSet AsDataSet()
        {
            DataSet XmlData = new DataSet();
            XmlData.ReadXml( new System.IO.StringReader( this.AsString() ) );
            return XmlData;
        }

        private string _FilePath = string.Empty;
        public string FilePath
        {
            get { return ( _FilePath ); }
        }



        #endregion Output
    }
}
