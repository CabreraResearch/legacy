using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    public class CswNbtNodeReaderDataRelationalDb : ICswNbtNodeReaderData
    {

        private CswNbtResources _CswNbtResources = null;
        public CswNbtNodeReaderDataRelationalDb( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public bool fetchNodeInfo( CswNbtNode CswNbtNode )
        {
            bool ReturnVal = false;

            CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "fetchNodeInfo_select", CswNbtNode.NodeId.TableName );
            string FilterColumn = _CswNbtResources.getPrimeKeyColName( CswNbtNode.NodeId.TableName );
            DataTable DataTable = NodesSelect.getTable( FilterColumn, CswNbtNode.NodeId.PrimaryKey );
            if( DataTable.Rows.Count > 0 )
            {
                CswNbtNode.NodeTypeId = Convert.ToInt32( DataTable.Rows[0]["nodetypeid"].ToString() );
                CswNbtNode.NodeName = DataTable.Rows[0]["nodename"].ToString();
                //CswNbtNode.PendingUpdate = ( DataTable.Rows[ 0 ][ "pendingupdate" ].ToString() == "1" );
                ReturnVal = true;
            }

            return ( ReturnVal );

        }//fetchNodeInfo()



    }//CswNbtNodeReaderDataRelationalDb

}//namespace ChemSW.Nbt
