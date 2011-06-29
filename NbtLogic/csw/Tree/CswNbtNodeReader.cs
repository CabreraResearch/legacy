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

    public class CswNbtNodeReader
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtColumnNames _CswNbtColumnNames = new CswNbtColumnNames();

        public CswNbtNodeReader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeReaderDataNative = new CswNbtNodeReaderDataNative( CswNbtResources );
            _CswNbtNodeReaderDataRelationalDb = new CswNbtNodeReaderDataRelationalDb( CswNbtResources );
        }//CswNbtNodeReader()


        private CswNbtNodeReaderDataNative _CswNbtNodeReaderDataNative = null;
        private CswNbtNodeReaderDataRelationalDb _CswNbtNodeReaderDataRelationalDb = null;
        private ICswNbtNodeReaderData this[ CswPrimaryKey NodeKey ]
        {
            get
            {
                ICswNbtNodeReaderData ReturnVal = null;
                if ( "nodes" == NodeKey.TableName.ToLower() )
                {
                    ReturnVal = _CswNbtNodeReaderDataNative;
                }
                else
                {
                    ReturnVal = _CswNbtNodeReaderDataRelationalDb;
                }

                return ( ReturnVal );
            }//get
        }

        public CswNbtResources CswNbtResources
        {
            get
            {
                return ( _CswNbtResources );
            }
        }

        private Object _extractCol( DataRow DataRow, string ColName, bool ThrowOnNotExists, bool ThrowOnNull )
        {
            Object ReturnVal = null;

            if ( DataRow.Table.Columns.Contains( ColName ) )
            {
                if ( !DataRow.IsNull( ColName ) )
                {
                    ReturnVal = DataRow[ ColName ];
                }
                else
                {
                    if ( !ThrowOnNull )
                    {
                        string EmptyString = "";
                        ReturnVal = EmptyString;
                    }
                    else
                    {
                        throw ( new CswDniException( "A data error occurred", "Column value is null: " + ColName ) );
                    }

                }//if-else val is null

            }
            else
            {
                if ( !ThrowOnNotExists )
                {
                    string EmptyString = "";
                    ReturnVal = EmptyString;
                }
                else
                {
                    throw ( new CswDniException( "A data error occurred", "Column does not exist: " + ColName ) );
                }

            }//if-else table contains column

            return ( ReturnVal );

        }//_extractCol()

        //bz # 7816: Don't throw if you cannot find the node data
        public void completeNodeData( CswNbtNode CswNbtNode, DateTime Date )
        {
            if ( CswNbtNode.NodeSpecies == NodeSpecies.Plain )
            {
                //bool NodeInfoFetched = false;
                if ( CswNbtNode.NodeId != null && ( CswNbtNode.NodeTypeId <= 0 || CswNbtNode.NodeName == String.Empty ) )
                {

					if( this[CswNbtNode.NodeId].fetchNodeInfo( CswNbtNode, Date ) )
                    {
                        CswTimer Timer = new CswTimer();
                        //this[ CswNbtNode.NodeId ].fillFromNodeTypeId( CswNbtNode, CswNbtNode.NodeTypeId );
                        _CswNbtResources.logTimerResult( "completeNodeData about to call fillFromNodeTypeId() on node (" + CswNbtNode.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString ); 
                        fillFromNodeTypeId( CswNbtNode, CswNbtNode.NodeTypeId );
                        _CswNbtResources.logTimerResult( "completeNodeData called fillFromNodeTypeId(), finished on node (" + CswNbtNode.NodeId.ToString() + ")", Timer.ElapsedDurationInSecondsAsString ); 
                        if(CswNbtNode.NodeType != null)
							CswNbtNode.Properties.fillFromNodePk( CswNbtNode.NodeId, CswNbtNode.NodeTypeId, Date );
                        _CswNbtResources.logTimerResult( "Filled in node property data for node (" + CswNbtNode.NodeId.ToString() + "): " + CswNbtNode.NodeName, Timer.ElapsedDurationInSecondsAsString );
                    }
                    // BZ 8117 - This is actually possibly expected behavior now when we delete a node
                    //else
                    //{
                    //    _CswNbtResources.logError( new CswDniException( "No node information is available for node : " + CswNbtNode.NodeId.ToString() ) );
                    //}
                }
            }

        }//completeNodeData()

        public void fillFromNodeTypeIdWithProps( CswNbtNode CswNbtNode, Int32 NodeTypeId )
        {
            CswNbtNode.Properties.fillFromNodePk( CswNbtNode.NodeId, NodeTypeId, DateTime.MinValue );
			CswNbtNode.Properties.fillFromNodeTypeId( CswNbtNode.NodeTypeId );

        }//_fillFromNodeTypeId()


        public void fillFromNodeTypeId( CswNbtNode CswNbtNode, Int32 NodeTypeId )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            
            // This error causes issues for NbtSched
            //if( NodeType == null )
            //    throw ( new System.Exception( "There is no information for this NodeTypeId " + NodeTypeId.ToString() ) );

            CswNbtNode.NodeTypeId = NodeTypeId;
            if( NodeType != null )
            {
                //CswNbtNode.NameTemplate = NodeType.NameTemplateValue;
                CswNbtNode.ObjectClassId = NodeType.ObjectClass.ObjectClassId;
                CswNbtNode.IconFileName = NodeType.IconFileName;
            }
        }//fillFromNodeTypeId() 


    }//CswNbtNodeReader

}//namespace ChemSW.Nbt
