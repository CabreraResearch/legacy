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
                string NodesSql = "select * from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "nodes", Date, CswNbtNode.NodeId.PrimaryKey );
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
                CswNbtNode.read( NodesTable.Rows[0] );
            }

            ReturnVal = true;


            return ( ReturnVal );


            //CswNbtNode.Modified = false; //bz # 5943
        }//fetchNodeInfo()


    }//CswNbtNodeReaderDataNative

}//namespace ChemSW.Nbt
