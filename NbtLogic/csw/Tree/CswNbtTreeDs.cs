using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{

    using NodeColumnsDef = Dictionary<NodeColumnIds, ColumnType>;
    struct ColumnType
    {
        public ColumnType( string NameIn, Type TypeIn )
        {
            Name = NameIn;
            Type = TypeIn;
        }//ctor 
        public string Name;
        public Type Type;
    }

    enum NodeColumnIds { CswPrimeKey, NodeId, NodeName, NodeTypeName, ParentNodeId, ModifiedByMobile };
    public class CswNbtTreeDs
    {

        private NodeColumnsDef _NodeColumnDefs = new NodeColumnsDef();


        private CswNbtResources _CswNbtResources = null;

        DataSet _NLevelDs = null;
        ICswNbtTree _CswNbtTree = null;
        public CswNbtTreeDs( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            _NodeColumnDefs.Add( NodeColumnIds.CswPrimeKey, new ColumnType( NodeColumnIds.CswPrimeKey.ToString(), typeof( System.String ) ) );
            _NodeColumnDefs.Add( NodeColumnIds.NodeId, new ColumnType( NodeColumnIds.NodeId.ToString(), typeof( System.Int32 ) ) );
            _NodeColumnDefs.Add( NodeColumnIds.NodeName, new ColumnType( NodeColumnIds.NodeName.ToString(), typeof( System.String ) ) );
            _NodeColumnDefs.Add( NodeColumnIds.NodeTypeName, new ColumnType( NodeColumnIds.NodeTypeName.ToString(), typeof( System.String ) ) );
            _NodeColumnDefs.Add( NodeColumnIds.ParentNodeId, new ColumnType( NodeColumnIds.ParentNodeId.ToString(), typeof( System.Int32 ) ) );
            _NodeColumnDefs.Add( NodeColumnIds.ModifiedByMobile, new ColumnType( NodeColumnIds.ModifiedByMobile.ToString(), typeof( bool ) ) );
            //_NodeColumnDefs.Add( "NodeId", typeof( System.Int32 ) );
            //_NodeColumnDefs.Add( "NodeName", typeof( System.String ) );
            //_NodeColumnDefs.Add( "ParentNodeId", typeof( System.Int32 ) );

        }//ctor


        public DataSet TreeToDataSet( ICswNbtTree CswNbtTree )
        {
            _CswNbtTree = CswNbtTree;

            _NLevelDs = new DataSet();
            _NLevelDs.DataSetName = _CswNbtTree.SourceViewXml;


            _CswNbtTree.goToRoot();
            List<CswNbtNodeKey> RootNodeKeys = new List<CswNbtNodeKey>();
            if ( _CswNbtTree.getChildNodeCount() > 0 )
            {
                _CswNbtTree.goToNthChild( 0 );
                _readNodes( _CswNbtTree.getNodeKeyForCurrentPosition() );
                while ( _CswNbtTree.nextSiblingExists() )
                {
                    _CswNbtTree.goToNextSibling();
                    _readNodes( _CswNbtTree.getNodeKeyForCurrentPosition() );

                }
            }

            string TreeAsXml = _CswNbtTree.getTreeAsXml();

            return ( _NLevelDs );

        }//TreeToDataSet()

        private void _readNodes( CswNbtNodeKey TargetNodeKey )
        {

            _CswNbtTree.makeNodeCurrent( TargetNodeKey );
            CswNbtNode TargetNode = _CswNbtTree.getNodeForCurrentPosition();
            CswNbtNode ParentNode = null;
            if ( !_CswNbtTree.isCurrentNodeChildOfRoot() )
            {
                _CswNbtTree.goToParentNode();
                ParentNode = _CswNbtTree.getNodeForCurrentPosition();
                _CswNbtTree.makeNodeCurrent( TargetNodeKey );
            }

            _readNode( TargetNode, ParentNode );


            if ( _CswNbtTree.getChildNodeCount() > 0 )
            {
                _CswNbtTree.goToNthChild( 0 );
                _readNodes( _CswNbtTree.getNodeKeyForCurrentPosition() );
                while ( _CswNbtTree.nextSiblingExists() )
                {
                    _CswNbtTree.goToNextSibling();
                    _readNodes( _CswNbtTree.getNodeKeyForCurrentPosition() );
                }
                _CswNbtTree.goToParentNode();
            }

        }//_readNodes() 

        private void _readNode( CswNbtNode TargetNode, CswNbtNode ParentNode )
        {

            string TargetTableName = TargetNode.NodeType.NodeTypeName;

            DataTable NodeTable = null;
            if ( _NLevelDs.Tables.Contains( TargetTableName ) )
            {
                NodeTable = _NLevelDs.Tables[TargetTableName];
            }
            else
            {
                NodeTable = new DataTable( TargetTableName );
                _NLevelDs.Tables.Add( NodeTable );

                //System.Enum.GetValues
                foreach ( NodeColumnIds CurrentColumnId in System.Enum.GetValues( typeof( NodeColumnIds ) ) )
                {
                    NodeTable.Columns.Add( new DataColumn( _NodeColumnDefs[CurrentColumnId].Name, _NodeColumnDefs[CurrentColumnId].Type ) );
                }

                foreach ( CswNbtNodePropWrapper CurrentPropWrapper in TargetNode.Properties )
                {
                    if ( _isPropTypeSupported( CurrentPropWrapper ) )
                    {
                        NodeTable.Columns.Add( new DataColumn( CurrentPropWrapper.PropName, typeof( System.String ) ) );
                    }
                }//

            }//if the table does not exist yet

            DataRow CurrentRow = NodeTable.NewRow();
            NodeTable.Rows.Add( CurrentRow );
            CurrentRow[_NodeColumnDefs[NodeColumnIds.NodeId].Name] = TargetNode.NodeId.PrimaryKey;
            CurrentRow[_NodeColumnDefs[NodeColumnIds.CswPrimeKey].Name] = TargetNode.NodeId.ToString();
            CurrentRow[_NodeColumnDefs[NodeColumnIds.NodeName].Name] = TargetNode.NodeName;
            CurrentRow[_NodeColumnDefs[NodeColumnIds.NodeTypeName].Name] = TargetNode.NodeType.NodeTypeName;
            CurrentRow[_NodeColumnDefs[NodeColumnIds.ModifiedByMobile].Name] = false;
            if ( null != ParentNode )
            {
                string ParentTableName = ParentNode.NodeType.NodeTypeName;
                if ( false == _NLevelDs.Relations.Contains( _MakeTableRelationName( ParentTableName, TargetTableName ) ) )
                {
                    _NLevelDs.Relations.Add( new DataRelation( _MakeTableRelationName( ParentTableName, TargetTableName ), _NLevelDs.Tables[ParentTableName].Columns["NodeId"], NodeTable.Columns["ParentNodeId"] ) );
                }

                CurrentRow["ParentNodeId"] = ParentNode.NodeId.PrimaryKey.ToString();
            }


            //Sergei: We can actually use the multi-subfield values but they would 
            //be read-only in the table -- just write the gestalt to the read only column
            foreach ( CswNbtNodePropWrapper CurrentPropWrapper in TargetNode.Properties )
            {
                if ( _isPropTypeSupported( CurrentPropWrapper ) )
                {
                    CurrentRow[CurrentPropWrapper.PropName] = CurrentPropWrapper.Gestalt;
                }
            }

        }//_readNode()

        //see my note in WriteTreeDs() with respect to SetPropRowValue()
        private bool _isPropTypeSupported( CswNbtNodePropWrapper CswNbtNodePropWrapper )
        {
            return
                (
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.DateTime ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Memo ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Number ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Password ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Static ||
                CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Text //||
                //CswNbtNodePropWrapper.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Time
                );
        }//_isPropTypeSupported() 


        private string _MakeTableRelationName( string ParentTableName, string ChildTableName )
        {
            return ( ParentTableName + "_to_" + ChildTableName );
        }//_MakeTableRelationName()



        DataSet _DsOriginal = null;
        public void writeTreeDs( DataSet DataSet )
        {
            if ( null == _DsOriginal )
            {
                CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                CswNbtView.LoadXml( DataSet.DataSetName );//if this is really our dataset, the name is the viewxml
                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, false, true, false, true );
                _DsOriginal = TreeToDataSet( CswNbtTree ); //Dimitri: This should take a tree param rather than in the ctor

            }

            for ( int tablesidx = 0; tablesidx < DataSet.Tables.Count; tablesidx++ )
            {
                DataTable CurrentTable = DataSet.Tables[tablesidx];

                for ( int rowsidx = 0; rowsidx < CurrentTable.Rows.Count; rowsidx++ )
                {

                    DataRow CurrentRow = CurrentTable.Rows[rowsidx];

                    CswPrimaryKey CswPrimaryKey = new CswPrimaryKey();
                    CswPrimaryKey.FromString( CurrentRow[_NodeColumnDefs[NodeColumnIds.CswPrimeKey].Name].ToString() );

                    CswNbtNode CswNbtNode = _CswNbtResources.Nodes[CswPrimaryKey];

                    if ( null != CswNbtNode )
                    {
                        ArrayList NodeColumns = new ArrayList();
                        NodeColumns.AddRange( System.Enum.GetNames( typeof( NodeColumnIds ) ) );

                        if ( true == Convert.ToBoolean( CurrentRow[_NodeColumnDefs[NodeColumnIds.ModifiedByMobile].Name] ) )
                        {
                            foreach ( DataColumn CurrentColumn in CurrentTable.Columns )
                            {
                                if ( false == NodeColumns.Contains( CurrentColumn.ColumnName ) )
                                {
                                    //Sergei: the current code will not allow you to null or string.empty a column; 
                                    //I began implementing a solution where we compare the new data set values to 
                                    //the existing one, but I commented it out here because this condition is really
                                    //just comparing pointers to objects rather than actual values. Comparing the 
                                    //actual values could be a bit more complicated as you will discover as you 
                                    //begin to think this through. 
                                    //--Dimitri
//                                    if ( CurrentRow[CurrentColumn.ColumnName] != _DsOriginal.Tables[tablesidx].Rows[rowsidx][CurrentColumn.ColumnName] )
                                    if ( ( false == CurrentRow.IsNull( CurrentColumn ) ) && ( string.Empty != CurrentRow[CurrentColumn].ToString() ) )
                                    {
                                        string NodeTypeName = CurrentRow[_NodeColumnDefs[NodeColumnIds.NodeTypeName].Name].ToString();
                                        string PropName = CurrentColumn.ColumnName;
                                        CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = _CswNbtResources.MetaData.getNodeType( NodeTypeName ).getNodeTypeProp( PropName );
                                        CswNbtNodePropWrapper CswNbtNodePropWrapper = CswNbtNode.Properties[CswNbtMetaDataNodeTypeProp];


                                        //The SetPropRowValue() idiom will only work for properties that use only one column of 
                                        //jct_nodes_props. If we want the HH to deal with more complex node types, we either 
                                        //have to have a mechnaism for setting the value based on the gestalt, which Steve says is 
                                        //a nightmare, or the table we send to the hand held has to have multiple columns, which 
                                        //means that the handheld can no longer treat these tables like simple data tables, but
                                        //instead will have to have a mechanism to interpret them as properties. In other words, 
                                        //enabling the handheld to deal with anything but the simple field types will be expensive
                                        //from an engineering point of view. 
                                        CswNbtNodePropWrapper.SetPropRowValue( CswNbtMetaDataNodeTypeProp.FieldTypeRule.SubFields.Default.Column, CurrentRow[CurrentColumn] );

                                    }//if column value is not null

                                }//if we're dealing with a column that represents a property
                            }

                        }//if row was modified

                        CswNbtNode.postChanges( false );

                    }//if we have a node 

                }//iterate rows

            }//iterate tables

        }//writeTreeDs()



    }//CswNbtTreeDs

}//namespace ChemSW.Nbt
