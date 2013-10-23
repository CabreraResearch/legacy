using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Audit;

namespace ChemSW.Nbt
{

    public class CswNbtNodeWriterNative : ICswNbtNodeWriterImpl
    {
        private CswNbtResources _CswNbtResources = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
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

        public void clear()
        {
            if( null != _CswTableUpdateNodes )
            {
                _CswTableUpdateNodes.clear();
            }
        }//clear() 

        public CswNbtNodeWriterNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase )
        {
            DataTable NewNodeTable = CswTableUpdateNodes.getEmptyTable();
            DataRow NewNodeRow = NewNodeTable.NewRow();
            NewNodeRow["nodename"] = Node.NodeName;
            NewNodeRow["nodetypeid"] = CswConvert.ToDbVal( Node.NodeTypeId );
            NewNodeRow["pendingupdate"] = CswConvert.ToDbVal( false );
            NewNodeRow["readonly"] = CswConvert.ToDbVal( false );
            NewNodeRow["isdemo"] = CswConvert.ToDbVal( false );
            NewNodeRow["issystem"] = CswConvert.ToDbVal( false );
            NewNodeRow["hidden"] = CswConvert.ToDbVal( false );
            NewNodeRow["searchable"] = CswConvert.ToDbVal( true );
            NewNodeRow["iconfilename"] = Node.IconFileNameOverride;
            NewNodeRow["created"] = DateTime.Now;

            //case 27709: nodes must have an explicit audit level
            CswNbtMetaDataNodeType CswNbtMetaDataNodeType = Node.getNodeType();
            if( null != CswNbtMetaDataNodeType )
            {
                NewNodeRow[_CswAuditMetaData.AuditLevelColName] = CswNbtMetaDataNodeType.AuditLevel;
                Node.AuditLevel = CswNbtMetaDataNodeType.AuditLevel; //Otherwise the Node's deafult NoAudit setting gets written to db; trust me on this one
            }

            NewNodeTable.Rows.Add( NewNodeRow );

            Node.NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NewNodeTable.Rows[0]["nodeid"] ) );

            if( PostToDatabase )
            {
                CswTableUpdateNodes.update( NewNodeTable, ( false == Node.IsTemp ) );
            }
        } // makeNewNodeEntry()


        public void write( CswNbtNode Node, bool ForceSave, bool IsCopy )
        {
            // save nodename and pendingupdate
            if( Node.NodeId.TableName != "nodes" )
                throw new CswDniException( CswEnumErrorType.Error, "Internal data error", "CswNbtNodeWriterNative attempted to write a node in table: " + Node.NodeId.TableName );

            DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", Node.NodeId.PrimaryKey );
            if( 1 != NodesTable.Rows.Count )
                throw ( new CswDniException( CswEnumErrorType.Error, "Internal data errors", "There are " + NodesTable.Rows.Count.ToString() + " node table records for node id (" + Node.NodeId.ToString() + ")" ) );

            NodesTable.Rows[0]["nodename"] = Node.NodeName;
            NodesTable.Rows[0]["pendingupdate"] = CswConvert.ToDbVal( Node.PendingUpdate );
            NodesTable.Rows[0]["readonly"] = CswConvert.ToDbVal( Node.ReadOnlyPermanent );
            NodesTable.Rows[0]["locked"] = CswConvert.ToDbVal( Node.Locked );
            NodesTable.Rows[0]["isdemo"] = CswConvert.ToDbVal( Node.IsDemo );
            NodesTable.Rows[0]["istemp"] = CswConvert.ToDbVal( Node.IsTemp );
            NodesTable.Rows[0]["sessionid"] = CswConvert.ToDbVal( Node.SessionId );
            NodesTable.Rows[0][_CswAuditMetaData.AuditLevelColName] = Node.AuditLevel;
            NodesTable.Rows[0]["hidden"] = CswConvert.ToDbVal( Node.Hidden );
            NodesTable.Rows[0]["iconfilename"] = Node.IconFileNameOverride;
            NodesTable.Rows[0]["searchable"] = CswConvert.ToDbVal( Node.Searchable );

            CswTableUpdateNodes.update( NodesTable, ( false == Node.IsTemp ) );

        }//write()

        public void updateRelationsToThisNode( CswNbtNode Node )
        {
            if( Node.NodeId.TableName != "nodes" )
                throw new CswDniException( CswEnumErrorType.Error, "Internal System Error", "CswNbtNodeWriterNative.updateRelationsToThisNode() called on a non-native node" );

            string SQL = @"update jct_nodes_props 
                              set pendingupdate = '" + CswConvert.ToDbVal( true ) + @"' 
                            where jctnodepropid in (select j.jctnodepropid
                                                      from jct_nodes_props j
                                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                      join field_types f on p.fieldtypeid = f.fieldtypeid
                                                     where (f.fieldtype = 'Relationship' or f.fieldtype = 'Location' or f.fieldtype = 'Quantity')
                                                       and j.field1_fk = " + Node.NodeId.PrimaryKey.ToString() + ")";

            // We're not doing this in a CswTableUpdate because it might be a large operation, 
            // and we don't care about auditing for this change.
            _CswNbtResources.execArbitraryPlatformNeutralSql( SQL );
        }


        public void delete( CswNbtNode CswNbtNode )
        {
            try
            {
                // Delete this node's property values

                CswTableUpdate CswTableUpdateJct = _CswNbtResources.makeCswTableUpdate( "deletenode_update", "jct_nodes_props" );
                DataTable JctTable = CswTableUpdateJct.getTable( " where nodeid=" + CswNbtNode.NodeId.PrimaryKey.ToString() );
                foreach( DataRow Row in JctTable.Rows )
                {
                    Row.Delete();
                }
                CswTableUpdateJct.update( JctTable );

                // Delete property values of relationships to this node
                if( CswNbtNode.NodeId.TableName != "nodes" )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Internal System Error", "CswNbtNodeWriterNative.delete() called on a non-native node" );
                }

                // From getRelationshipsToNode.  see case 27711
                string InClause = @"select j.jctnodepropid
                                      from jct_nodes_props j
                                      join nodes n on j.nodeid = n.nodeid
                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                      join field_types f on p.fieldtypeid = f.fieldtypeid
                                     where (f.fieldtype = 'Relationship' or f.fieldtype = 'Location')
                                       and j.field1_fk = " + CswNbtNode.NodeId.PrimaryKey.ToString();

                DataTable RelatedJctTable = CswTableUpdateJct.getTable( " where jctnodepropid in (" + InClause + ")" );
                foreach( DataRow Row in RelatedJctTable.Rows )
                {
                    Row.Delete();
                }
                CswTableUpdateJct.update( RelatedJctTable );

                // Delete the node

                DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", CswNbtNode.NodeId.PrimaryKey, true );
                NodesTable.Rows[0].Delete();
                CswTableUpdateNodes.update( NodesTable );

            }//try

            catch( System.Exception Exception )
            {
                throw ( Exception );
            }// catch

        }//delete()

    }//CswNbtNodeWriterNative

}//namespace ChemSW.Nbt
