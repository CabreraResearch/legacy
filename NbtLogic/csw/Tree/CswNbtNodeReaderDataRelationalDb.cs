using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodeReaderDataRelationalDb : ICswNbtNodeReaderData
    {

        private CswNbtResources _CswNbtResources = null;
        public CswNbtNodeReaderDataRelationalDb( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public bool fetchNodeInfo( CswNbtNode CswNbtNode, DateTime Date )
        {
            bool ReturnVal = false;

            CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "fetchNodeInfo_select", CswNbtNode.NodeId.TableName );
            string FilterColumn = _CswNbtResources.getPrimeKeyColName( CswNbtNode.NodeId.TableName );
            DataTable DataTable = NodesSelect.getTable( FilterColumn, CswNbtNode.NodeId.PrimaryKey );
            if( DataTable.Rows.Count > 0 )
            {
                CswNbtNode.NodeTypeId = CswConvert.ToInt32( DataTable.Rows[0]["nodetypeid"].ToString() );
                CswNbtNode.NodeName = DataTable.Rows[0]["nodename"].ToString();
                //CswNbtNode.PendingUpdate = ( DataTable.Rows[ 0 ][ "pendingupdate" ].ToString() == "1" );
                ReturnVal = true;
            }

            return ( ReturnVal );

        }//fetchNodeInfo()



    }//CswNbtNodeReaderDataRelationalDb

}//namespace ChemSW.Nbt
