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

    public class CswNbtNodeReaderDataNative : ICswNbtNodeReaderData
    {

        private CswNbtResources _CswNbtResources = null;
        public CswNbtNodeReaderDataNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public bool fetchNodeInfo( CswNbtNode CswNbtNode )
        {
            bool ReturnVal = false;
            CswTableSelect NodesTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodeReaderDataNative.fetchNodeInfo", "nodes" );
            DataTable NodesTable = NodesTableSelect.getTable( "nodeid", CswNbtNode.NodeId.PrimaryKey );
            if ( NodesTable.Rows.Count > 0 )
            {
                CswNbtNode.NodeTypeId = CswConvert.ToInt32( NodesTable.Rows[ 0 ][ "nodetypeid" ].ToString() );
                CswNbtNode.NodeName = NodesTable.Rows[ 0 ][ "nodename" ].ToString();
                CswNbtNode.PendingUpdate = ( NodesTable.Rows[ 0 ][ "pendingupdate" ].ToString() == "1" );
                ReturnVal = true;
            }
            return ( ReturnVal );
            //CswNbtNode.Modified = false; //bz # 5943
        }//fetchNodeInfo()


    }//CswNbtNodeReaderDataNative

}//namespace ChemSW.Nbt
