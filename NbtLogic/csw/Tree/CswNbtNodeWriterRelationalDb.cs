using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodeWriterRelationalDb : ICswNbtNodeWriterImpl
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtNodeWriterRelationalDb( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void clear()
        {
        }//clear()

        public void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase )
        {

            string TableName = Node.getNodeType().TableName;
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );
            CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtNodeWriterRelationalDb.makeNewNodeEntry_update", TableName );

            DataTable NewNodeTable = CswTableUpdate.getEmptyTable();
            DataRow NewNodeRow = NewNodeTable.NewRow();
            NewNodeRow["nodename"] = Node.NodeName;
            NewNodeRow["nodetypeid"] = Node.NodeTypeId;
            //NewNodeRow["pendingupdate"] = "0";
            //NewNodeRow["issystem"] = "0";
            NewNodeRow["hidden"] = CswConvert.ToDbVal( false );
            NewNodeTable.Rows.Add( NewNodeRow );

            Node.NodeId = new CswPrimaryKey( TableName, CswConvert.ToInt32( NewNodeTable.Rows[0][PkColumnName] ) );

            if( PostToDatabase )
                CswTableUpdate.update( NewNodeTable );
        }

        //private CswTableCaddy _CswTableCaddy;
        private void _getDataTable( CswNbtNode Node, out DataTable NodesTable, out CswTableUpdate CswTableUpdate )
        {
            string TableName = Node.NodeId.TableName;
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );

            CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtNodeWriterRelationalDb.getDataTable_update", TableName );
            NodesTable = CswTableUpdate.getTable( PkColumnName, Node.NodeId.PrimaryKey );
            if( 1 != NodesTable.Rows.Count )
                throw ( new CswDniException( ErrorType.Error, "Internal data errors", "There are " + NodesTable.Rows.Count.ToString() + " node table records for node id (" + Node.NodeId.ToString() + ")" ) );

            //return ( NodesTable );
        }//_getDataTable()

        public void write( CswNbtNode Node, bool ForceSave, bool IsCopy )
        {
            DataTable NodesTable;
            CswTableUpdate NodesUpdate;
            _getDataTable( Node, out NodesTable, out NodesUpdate );
            NodesTable.Rows[0]["nodename"] = Node.NodeName;
            NodesUpdate.update( NodesTable );

        }//write()


        public void updateRelationsToThisNode( CswNbtNode Node )
        {
            throw new CswDniException( "CswNbtNodeWriterRelationalDb.updateRelationsToThisNode() is not implemented" );

            //Steve? Heeeeeeeeeeeeeeeeeelp!!!!

            //CswQueryCaddy RelatedsQueryCaddy = _CswNbtResources.makeCswQueryCaddy( "getRelationshipsToNode" );
            //RelatedsQueryCaddy.S4Parameters.Add( "getnodeid", Node.NodeId );
            //DataTable RelatedsTable = RelatedsQueryCaddy.Table;

            //// Update the jct_nodes_props directly, to avoid having to fetch all the node info for every node with a relationship to this node
            //string PkString = string.Empty;
            //foreach ( DataRow RelatedsRow in RelatedsTable.Rows )
            //{
            //    if ( PkString != string.Empty ) PkString += ",";
            //    PkString += RelatedsRow[ "jctnodepropid" ].ToString();
            //}
            //if ( PkString != string.Empty )
            //{
            //    CswTableCaddy JctNodesPropsCaddy = _CswNbtResources.makeCswTableCaddy( "jct_nodes_props" );
            //    JctNodesPropsCaddy.WhereClause = "where jctnodepropid in (" + PkString + ")";
            //    DataTable JctNodesPropsTable = JctNodesPropsCaddy.Table;
            //    foreach ( DataRow JctNodesPropsRow in JctNodesPropsTable.Rows )
            //    {
            //        JctNodesPropsRow[ "pendingupdate" ] = "1";
            //    }
            //    JctNodesPropsCaddy.update( JctNodesPropsTable );
            //}
        }


        public void delete( CswNbtNode CswNbtNode )
        {
            try
            {

                //The native implementation deletes relationships to this node at this point. 
                DataTable NodesTable;
                CswTableUpdate NodesUpdate;
                _getDataTable( CswNbtNode, out NodesTable, out NodesUpdate );

                //DataTable NodesTable = _getDataTable( CswNbtNode );
                NodesTable.Rows[0].Delete();
                NodesUpdate.update( NodesTable );
            }//try

            catch( System.Exception Exception )
            {
                throw ( Exception );
            }// catch

        }//delete()

    }//CswNbtNodeWriterRelationalDb

}//namespace ChemSW.Nbt
