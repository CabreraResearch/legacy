using System;
using System.Collections.ObjectModel;
using System.Data;
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

        public bool fetchNodeInfo( CswNbtNode CswNbtNode, DateTime Date )
        {
            bool ReturnVal = false;
            CswTableSelect NodesTableSelect = null;
            DataTable NodesTable = null;
            if( Date != DateTime.MinValue )
            {
                NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodeReaderDataNative.fetchNodeInfo_audit", "nodes_audit" );
                OrderByClause OrderBy = new OrderByClause( "recordcreated", OrderByType.Descending );
                NodesTable = NodesTableSelect.getTable( null, "nodeid", CswNbtNode.NodeId.PrimaryKey, "where recordcreated <= " + _CswNbtResources.getDbNativeDate( Date ), false, new Collection<OrderByClause>() { OrderBy } );
            }
            // there may be no audit records, so fail into finding the most current record
            if( NodesTable == null || NodesTable.Rows.Count == 0 )
            {
                NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodeReaderDataNative.fetchNodeInfo", "nodes" );
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
                ReturnVal = true;
            }
            return ( ReturnVal );
            //CswNbtNode.Modified = false; //bz # 5943
        }//fetchNodeInfo()


    }//CswNbtNodeReaderDataNative

}//namespace ChemSW.Nbt
