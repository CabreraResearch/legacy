using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodeReader
    {
        private CswNbtResources _CswNbtResources = null;
        
        public CswNbtNodeReader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//CswNbtNodeReader()

        //bz # 7816: Don't throw if you cannot find the node data
        public void completeNodeData( CswNbtNode CswNbtNode, CswDateTime Date )
        {
            if( CswNbtNode.NodeSpecies == CswEnumNbtNodeSpecies.Plain )
            {
                //bool NodeInfoFetched = false;
                if( CswNbtNode.NodeId != null && ( CswNbtNode.NodeTypeId <= 0 || CswNbtNode.NodeName == String.Empty ) )
                {
                    DataTable NodesTable = null;
                    if( CswTools.IsDate( Date ) )
                    {
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
                        CswNbtNode.RelationalId = new CswPrimaryKey( NodesTable.Rows[0]["relationaltable"].ToString(), CswConvert.ToInt32( NodesTable.Rows[0]["relationalid"] ) );
                    }

                    CswTimer Timer = new CswTimer();
                    _CswNbtResources.logTimerResult( "completeNodeData about to call fillFromNodeTypeId() on node (" + CswNbtNode.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );
                    fillFromNodeTypeId( CswNbtNode, CswNbtNode.NodeTypeId, Date );
                    _CswNbtResources.logTimerResult( "completeNodeData called fillFromNodeTypeId(), finished on node (" + CswNbtNode.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString );
                    if( CswNbtNode.getNodeType() != null )
                        CswNbtNode.Properties.fillFromNodePk( CswNbtNode.NodeId, CswNbtNode.NodeTypeId, Date );

                    _CswNbtResources.logTimerResult( "Filled in node property data for node (" + CswNbtNode.NodeId.ToString() + "): " + CswNbtNode.NodeName, Timer.ElapsedDurationInSecondsAsString );

                    if( CswTools.IsDate( Date ) )
                    {
                        CswNbtNode.setReadOnly( value: true, SaveToDb: false );
                    }
                }
            }

        }//completeNodeData()

        public void fillFromNodeTypeIdWithProps( CswNbtNode CswNbtNode, Int32 NodeTypeId )
        {
            CswNbtNode.Properties.fillFromNodePk( CswNbtNode.NodeId, NodeTypeId, null );
            CswNbtNode.Properties.fillFromNodeTypeId( CswNbtNode.NodeTypeId );

        }//_fillFromNodeTypeId()


        public void fillFromNodeTypeId( CswNbtNode CswNbtNode, Int32 NodeTypeId, CswDateTime Date = null )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId, Date );

            // This error causes issues for NbtSched
            //if( NodeType == null )
            //    throw ( new System.Exception( "There is no information for this NodeTypeId " + NodeTypeId.ToString() ) );

            CswNbtNode.NodeTypeId = NodeTypeId;
            if( NodeType != null )
            {
                //CswNbtNode.NameTemplate = NodeType.NameTemplateValue;
                //CswNbtNode.getObjectClassId() = NodeType.ObjectClassId;
                CswNbtNode.IconFileName = NodeType.IconFileName;
            }
        }//fillFromNodeTypeId() 


    }//CswNbtNodeReader

}//namespace ChemSW.Nbt
