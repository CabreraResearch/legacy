using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodeReaderDataNative : ICswNbtNodeReaderData
    {
        private ChemSW.Audit.CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        private CswNbtResources _CswNbtResources = null;
        public CswNbtNodeReaderDataNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public bool fetchNodeInfo( CswNbtNode CswNbtNode, CswDateTime Date )
        {
            bool ReturnVal = false;
            DataTable NodesTable = null;
            if( CswTools.IsDate( Date ) )
            {
                //NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodeReaderDataNative.fetchNodeInfo_audit", "nodes_audit" );
                //OrderByClause OrderBy = new OrderByClause( "recordcreated", CswEnumOrderByType.Descending );
                //NodesTable = NodesTableSelect.getTable( null, "nodeid", CswNbtNode.NodeId.PrimaryKey, "where recordcreated <= " + _CswNbtResources.getDbNativeDate( Date ), false, new Collection<OrderByClause>() { OrderBy } );
                string NodesSql = "select * from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "nodes", Date ) + " where nodeid = " + CswNbtNode.NodeId.PrimaryKey;
                CswArbitrarySelect NodesTableSelect = _CswNbtResources.makeCswArbitrarySelect( "fetchNodeInfo_Select", NodesSql );
                NodesTable = NodesTableSelect.getTable();
            }
            else
            {
                CswTableSelect NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodeReaderDataNative.fetchNodeInfo", "nodes" );
                NodesTable = NodesTableSelect.getTable( "nodeid", CswNbtNode.NodeId.PrimaryKey );
            }

            if( NodesTable.Rows.Count > 0 )
            {
                CswNbtNode.NodeTypeId = CswConvert.ToInt32( NodesTable.Rows[0]["nodetypeid"] );
                CswNbtNode.NodeName = NodesTable.Rows[0]["nodename"].ToString();
                CswNbtNode.setReadOnly( value: CswConvert.ToBoolean( NodesTable.Rows[0]["readonly"] ), SaveToDb: true );
                CswNbtNode.IsDemo = CswConvert.ToBoolean( NodesTable.Rows[0]["isdemo"] );
                CswNbtNode.Locked = CswConvert.ToBoolean( NodesTable.Rows[0]["locked"] );
                CswNbtNode.IsTemp = CswConvert.ToBoolean( NodesTable.Rows[0]["istemp"] );
                CswNbtNode.SessionId = CswConvert.ToString( NodesTable.Rows[0]["sessionid"] );
                CswNbtNode.PendingUpdate = CswConvert.ToBoolean( NodesTable.Rows[0]["pendingupdate"] );
                CswNbtNode.Searchable = CswConvert.ToBoolean( NodesTable.Rows[0]["searchable"] );

                if( NodesTable.Columns.Contains( _CswAuditMetaData.AuditLevelColName ) )
                {
                    CswNbtNode.AuditLevel = NodesTable.Rows[0][_CswAuditMetaData.AuditLevelColName].ToString();
                }
                else
                {
                    CswNbtNode.AuditLevel = _CswAuditMetaData.DefaultAuditLevel;
                }
            }

            ReturnVal = true;


            return ( ReturnVal );


            //CswNbtNode.Modified = false; //bz # 5943
        }//fetchNodeInfo()


    }//CswNbtNodeReaderDataNative

}//namespace ChemSW.Nbt
