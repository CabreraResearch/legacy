using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;  

namespace ChemSW.Nbt
{

    public class CswNbtNodeWriterNative : ICswNbtNodeWriterImpl
    {
        private CswNbtResources _CswNbtResources = null;

        private CswTableUpdate _CswTableUpdateNodes = null;
        private CswTableUpdate CswTableUpdateNodes
        {
            get
            {
                if( _CswTableUpdateNodes == null )
                    _CswTableUpdateNodes = _CswNbtResources.makeCswTableUpdate( "CswNbtNodeWriterNative_update", "nodes" );
                return _CswTableUpdateNodes;
            }
        }

        public CswNbtNodeWriterNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        //bz # 5878
        //private bool _ManageTransaction = false;
        //public bool ManageTransaction
        //{
        //    set { _ManageTransaction = value; }
        //    get { return ( _ManageTransaction ); }
        //}

        public void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase )
        {
            DataTable NewNodeTable = CswTableUpdateNodes.getEmptyTable();
            DataRow NewNodeRow = NewNodeTable.NewRow();
            NewNodeRow[ "nodename" ] = Node.NodeName;
            NewNodeRow[ "nodetypeid" ] = Node.NodeTypeId;
            NewNodeRow[ "pendingupdate" ] = "0";
            NewNodeRow[ "issystem" ] = "0";
            NewNodeTable.Rows.Add( NewNodeRow );

            Node.NodeId = new CswPrimaryKey( "nodes", Convert.ToInt32( NewNodeTable.Rows[ 0 ][ "nodeid" ] ) );

            if( PostToDatabase )
                CswTableUpdateNodes.update( NewNodeTable );
        }


        public void write( CswNbtNode Node, bool ForceSave, bool IsCopy )
        {

            // save nodename and pendingupdate
            if ( Node.NodeId.TableName != "nodes" )
                throw new CswDniException( "Internal data error", "CswNbtNodeWriterNative attempted to write a node in table: " + Node.NodeId.TableName );

            DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", Node.NodeId.PrimaryKey );
            if ( 1 != NodesTable.Rows.Count )
                throw ( new CswDniException( "Internal data errors", "There are " + NodesTable.Rows.Count.ToString() + " node table records for node id (" + Node.NodeId.ToString() + ")" ) );
            NodesTable.Rows[ 0 ][ "nodename" ] = Node.NodeName;
            NodesTable.Rows[ 0 ][ "pendingupdate" ] = CswConvert.ToDbVal( Node.PendingUpdate );
            CswTableUpdateNodes.update( NodesTable );

        }//write()

        public void updateRelationsToThisNode( CswNbtNode Node )
        {
            if( Node.NodeId.TableName != "nodes" )
                throw new CswDniException( "Internal System Error", "CswNbtNodeWriterNative.updateRelationsToThisNode() called on a non-native node" );

            CswStaticSelect RelatedsQuerySelect = _CswNbtResources.makeCswStaticSelect( "updateRelationsToThisNode_select", "getRelationshipsToNode" );
            RelatedsQuerySelect.S4Parameters.Add( "getnodeid", Node.NodeId.PrimaryKey.ToString() );
            DataTable RelatedsTable = RelatedsQuerySelect.getTable();

            // Update the jct_nodes_props directly, to avoid having to fetch all the node info for every node with a relationship to this node
            string PkString = string.Empty;
            foreach ( DataRow RelatedsRow in RelatedsTable.Rows )
            {
                if ( PkString != string.Empty ) PkString += ",";
                PkString += RelatedsRow[ "jctnodepropid" ].ToString();
            }
            if ( PkString != string.Empty )
            {
                CswTableUpdate JctNodesPropsUpdate = _CswNbtResources.makeCswTableUpdate( "updateRelationsToThisNode_jctnodeprops_update", "jct_nodes_props" );
                DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( "where jctnodepropid in (" + PkString + ")" );
                foreach ( DataRow JctNodesPropsRow in JctNodesPropsTable.Rows )
                {
                    JctNodesPropsRow[ "pendingupdate" ] = "1";
                }
                JctNodesPropsUpdate.update( JctNodesPropsTable );
            }
        }


        public void delete( CswNbtNode CswNbtNode )
        {
            try
            {
                // Delete this node's property values

                CswTableUpdate CswTableUpdateJct = _CswNbtResources.makeCswTableUpdate( "deletenode_update","jct_nodes_props" );
                DataTable JctTable = CswTableUpdateJct.getTable( " where nodeid=" + CswNbtNode.NodeId.PrimaryKey.ToString() );
                foreach ( DataRow Row in JctTable.Rows )
                    Row.Delete();
                CswTableUpdateJct.update( JctTable );

                // Delete property values of relationships to this node
                if( CswNbtNode.NodeId.TableName != "nodes" )
                    throw new CswDniException( "Internal System Error", "CswNbtNodeWriterNative.delete() called on a non-native node" );

                CswStaticSelect RelationshipsSelect = _CswNbtResources.makeCswStaticSelect( "deletenode_select", "getRelationshipsToNode" );
                RelationshipsSelect.S4Parameters.Add( "getnodeid", CswNbtNode.NodeId.PrimaryKey.ToString() );
                DataTable RelationshipsTable = RelationshipsSelect.getTable();
                if ( RelationshipsTable.Rows.Count > 0 )
                {
                    string InClause = string.Empty;
                    foreach ( DataRow Row in RelationshipsTable.Rows )
                    {
                        if ( InClause != string.Empty ) InClause += ",";
                        InClause += Row[ "jctnodepropid" ].ToString();
                    }
                    DataTable RelatedJctTable = CswTableUpdateJct.getTable(" where jctnodepropid in (" + InClause + ")");
                    foreach ( DataRow Row in RelatedJctTable.Rows )
                        Row.Delete();
                    CswTableUpdateJct.update( RelatedJctTable );
                }

                // Delete the node

                //CswTableCaddy CswTableCaddyNodes = _CswNbtResources.makeCswTableCaddy( "nodes" );
                DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", CswNbtNode.NodeId.PrimaryKey, true );
                NodesTable.Rows[ 0 ].Delete();
                CswTableUpdateNodes.update( NodesTable );

            }//try

            catch ( System.Exception Exception )
            {
                throw ( Exception );
            }// catch

        }//delete()

    }//CswNbtNodeWriterNative

}//namespace ChemSW.Nbt
