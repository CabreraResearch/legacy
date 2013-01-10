//using System;
//using System.Collections.ObjectModel;
//using System.Data;
//using System.Diagnostics;
//using ChemSW.Core;
//using ChemSW.Exceptions;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.ObjClasses;

//namespace ChemSW.Nbt
//{
//    /// <summary>
//    /// Represents an NBT Tree
//    /// </summary>
//    public class CswNbtTreeDomProxy 


//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="View">Tree View</param>
//        /// <param name="CswNbtResources">The CswNbtResources object</param>
//        /// <param name="CswNbtNodeWriter">A CswNbtNodeWriter object</param>
//        /// <param name="CswNbtNodeCollection">A reference to the CswNbtNodeCollection</param>
//        /// <param name="IsFullyPopulated"></param>
//        public CswNbtTreeDomProxy( CswNbtView View, CswNbtResources CswNbtResources, CswNbtNodeCollection CswNbtNodeCollection, bool IsFullyPopulated )
//        {
//            _View = View;
//            _Key = new CswNbtTreeKey( _CswNbtResources, _View );

//            _CswNbtResources = CswNbtResources;

//            _cswNbtTreeNodes = new CswNbtTreeNodes( _Key, ViewName, _CswNbtResources, CswNbtNodeCollection );

//            _CswNbtNodeCollection = CswNbtNodeCollection;

//            _IsFullyPopulated = IsFullyPopulated;
//        }//ctor

        

//        /// <summary>
//        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
//        /// </summary>
//        /// <param name="ViewRoot">The corresponding ViewRoot node in the View</param>
//        public void makeRootNode( CswNbtViewRoot ViewRoot )
//        {
//            _cswNbtTreeNodes.makeRootNode( ViewRoot.ViewName, ViewRoot );
//        }


//        /// <summary>
//        /// Creates a root node on the tree.  Mostly used by TreeLoaders.
//        /// </summary>
//        /// <param name="IconFileName">Icon filename for root node</param>
//        /// <param name="Selectable">True if the root is selectable, false otherwise</param>
//        /// <param name="AddChildren">True if users can add children to the root, false otherwise</param>
//        public void makeRootNode( string IconFileName, bool Selectable, NbtViewAddChildrenSetting AddChildren )
//        {
//            _cswNbtTreeNodes.makeRootNode( ViewName, IconFileName, Selectable );//, AddChildren);
//        }


        
        
        

       

//        /// <summary>
//        /// Return a node key for the first matching node in the tree
//        /// </summary>
//        /// <remarks>
//        /// Candidate to refactor to CswNbtNodes
//        /// </remarks>
//        /// <param name="NodeId">Primary key of node</param>
//        public CswNbtNodeKey getNodeKeyByNodeId( CswPrimaryKey NodeId )
//        {
//            CswNbtNodeKey ReturnVal = null;
//            Collection<CswNbtNodeKey> KeyList = getKeysForNodeId( NodeId );
//            if( null != KeyList && KeyList.Count > 0 )
//            {
//                ReturnVal = KeyList[0];
//                //ReturnVal.TreeKey = Key;
//            }
//            return ( ReturnVal );
//        }//getNodeKeyByNodeId()

       



//        //Navigation and interrogation methods*****************************************
//        #region Navigation and interrogation methods


        


//        #endregion //NavigationAndInterrogation******************************


//        //Modification methods*****************************************
//        #region Modification Methods

//        /// <summary>
//        /// Adds a Child from a DataRow.  Used by TreeLoaders.
//        /// </summary>
//        /// <param name="ParentNodeKey">Parent Node Key (for path generation)</param>
//        /// <param name="DataRowToAdd">DataRow with node information</param>
//        /// <param name="UseGrouping">Whether grouping nodes</param>
//        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
//        /// <param name="Relationship">ViewRelationship node which caused this node to be added</param>
//        /// <param name="RowCount">Row number in view results</param>
//        /// <param name="Included"></param>
//        /// <returns>Returns NodeKey index for node</returns>
//        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship, Int32 RowCount, bool Included = true )
//        {
//            Collection<CswNbtNodeKey> ReturnVal = _cswNbtTreeNodes.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Relationship, RowCount, Included );
//            //ReturnVal.TreeKey = Key;
//            //_TreeAsTransformedXml = "";
//            return ( ReturnVal );
//        }//loadNodeAsChildFromRow() 

//        /// <summary>
//        /// Adds a Child from a DataRow.  Used by TreeLoaders.
//        /// </summary>
//        /// <param name="ParentNodeKey">Parent Node Key (for path generation)</param>
//        /// <param name="DataRowToAdd">DataRow with node information</param>
//        /// <param name="UseGrouping">Whether grouping nodes</param>
//        /// <param name="GroupName">If grouping nodes, name of Group for this node</param>
//        /// <param name="Selectable">True if the node is selectable, false otherwise</param>
//        /// <param name="ShowInTree"></param>
//        /// <param name="AddChildren">True if the user should be allowed to add children to this node, false otherwise</param>
//        /// <param name="RowCount">Row number in view results</param>
//        /// <param name="Included"></param>
//        /// <returns>Returns NodeKey index for node</returns>
//        public Collection<CswNbtNodeKey> loadNodeAsChildFromRow( CswNbtNodeKey ParentNodeKey, DataRow DataRowToAdd, bool UseGrouping, string GroupName, bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included = true )
//        {
//            Collection<CswNbtNodeKey> ReturnVal = _cswNbtTreeNodes.loadNodeAsChildFromRow( ParentNodeKey, DataRowToAdd, UseGrouping, GroupName, Selectable, ShowInTree, AddChildren, RowCount, Included );
//            return ( ReturnVal );
//        }//loadNodeAsChildFromRow() 

//        /// <summary>
//        /// Sets the client-side expandmode of the current node
//        /// </summary>
//        public void setCurrentNodeExpandMode( string ExpandMode )
//        {
//            _cswNbtTreeNodes.setCurrentNodeExpandMode( ExpandMode );
//        }

//        /// <summary>
//        /// Adds a Property value to a node.  This is the uncommon way to fill property data in for nodes.
//        /// </summary>
//        public void addProperty( Int32 NodeTypePropId, Int32 JctNodePropId, string Name, string Gestalt, CswNbtMetaDataFieldType.NbtFieldType FieldType, string Field1, string Field2, Int32 Field1_Fk, double Field1_Numeric, bool Hidden )
//        {
//            _cswNbtTreeNodes.addProperty( NodeTypePropId, JctNodePropId, Name, Gestalt, FieldType, Field1, Field2, Field1_Fk, Field1_Numeric, Hidden );
//        }//addProperty


//        public Collection<CswNbtNodeKey> _loadNodeAsChild( CswNbtNodeKey ParentNodeKey, bool UseGrouping, string GroupName, CswNbtViewRelationship Relationship,
//                                               bool Selectable, bool ShowInTree, NbtViewAddChildrenSetting AddChildren, Int32 RowCount, bool Included,
//                                               string IconFileName, string NameTemplate, CswPrimaryKey NodeId, string NodeName, Int32 NodeTypeId,
//                                               string NodeTypeName, Int32 ObjectClassId, string ObjectClassName, bool Locked )
//        {
//            return _cswNbtTreeNodes._loadNodeAsChild( ParentNodeKey, UseGrouping, GroupName, Relationship,
//                                                       Selectable, ShowInTree, AddChildren, RowCount, Included,
//                                                       IconFileName, NameTemplate, NodeId, NodeName, NodeTypeId,
//                                                       NodeTypeName, ObjectClassId, ObjectClassName, Locked );
//        }


//        public void setCurrentNodeChildrenTruncated( bool Truncated )
//        {
//            _cswNbtTreeNodes.setCurrentNodeChildrenTruncated( Truncated );
//        }
//        public bool getCurrentNodeChildrenTruncated()
//        {
//            return _cswNbtTreeNodes.getCurrentNodeChildrenTruncated();
//        }

//        public void removeCurrentNode()
//        {
//            _cswNbtTreeNodes.removeCurrentNode();
//        }

//        #endregion //Modification******************************


//    }//class CswNbtTreeDomProxy

//}//namespace ChemSW.Nbt
