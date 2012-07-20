using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private readonly CswNbtSdNode _NodeSd;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _NodeSd = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
        }

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            return _NodeSd.CopyNode( NodePk );
        }

        public JObject DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ret = new JObject();
            Collection<CswPrimaryKey> NodePrimaryKeys = new Collection<CswPrimaryKey>();

            if( NodeKeys.Length > 0 )
            {
                foreach( string NodeKey in NodeKeys )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
                    if( null != NbtNodeKey )
                    {
                        NodePrimaryKeys.Add( NbtNodeKey.NodeId );
                    }
                }
            }
            if( NodePks.Length > 0 )
            {
                foreach( string NodePk in NodePks )
                {
                    CswPrimaryKey PrimaryKey = new CswPrimaryKey();
                    PrimaryKey.FromString( NodePk );
                    if( null != PrimaryKey && !NodePrimaryKeys.Contains( PrimaryKey ) )
                    {
                        NodePrimaryKeys.Add( PrimaryKey );
                    }
                }
            }
            if( NodePrimaryKeys.Count > 0 )
            {
                if( NodePrimaryKeys.Count < CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                {
                    bool success = true;
                    foreach( CswPrimaryKey Npk in NodePrimaryKeys )
                    {
                        success = success && DeleteNode( Npk );
                    }
                    ret["Succeeded"] = success.ToString();
                }
                else
                {
                    CswNbtBatchOpMultiDelete op = new CswNbtBatchOpMultiDelete( _CswNbtResources );
                    CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( NodePrimaryKeys );
                    ret["batch"] = BatchNode.NodeId.ToString();
                }
            }

            return ret;
        }

        public bool DeleteNode( CswPrimaryKey NodePk, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes );
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText )
        {
            return _NodeSd.doObjectClassButtonClick( PropId, SelectedText );
        }

        public JObject deleteDemoDataNodes()
        {
            JObject Ret = new JObject();
            Int32 Succeeded = 0;
            Int32 Total = 0;
            Int32 Failed = 0;
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                /* Get a new CswNbtResources as the System User */
                CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( _CswNbtResources );
                CswNbtResources NbtSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );

                //CswTableSelect NodesSelect = new CswTableSelect( NbtSystemResources.CswResources, "delete_demodata_nodes", "nodes" );
                CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_nodes", "nodes" );

                DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" },
                                                            " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total = NodesTable.Rows.Count;
                Collection<Exception> Exceptions = new Collection<Exception>();
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    try
                    {
                        CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                        if( _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes: true ) )
                        {
                            Succeeded += 1;
                        }
                    }
                    catch( Exception Exception )
                    {
                        Failed += 1;
                        Exceptions.Add( Exception );
                    }
                }
                wsMd.finalizeOtherResources( NbtSystemResources );
                if( Exceptions.Count > 0 )
                {
                    string ExceptionText = "";
                    foreach( Exception ex in Exceptions )
                    {
                        ExceptionText += ex.Message + " " + ex.InnerException + " /n";
                    }
                    throw new CswDniException( ErrorType.Warning, "Not all demo data nodes were deleted. " + Failed + " failed out of " + Total + " total.", "The following exception(s) occurred: " + ExceptionText );
                }
            }
            Ret["succeeded"] = Succeeded;
            Ret["total"] = Total;
            Ret["failed"] = Failed;

            return Ret;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        public void addNodeProps( CswNbtNode Node, JObject PropsObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addNodeProps( Node, PropsObj, Tab );
        }

        public void addSingleNodeProp( CswNbtNode Node, JObject PropObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addSingleNodeProp( Node, PropObj, Tab );

        } // _applyPropJson

        public JObject getQuantityFromSize( CswPrimaryKey SizeId )
        {
            JObject Ret = new JObject();

            CswNbtObjClassSize Size = _CswNbtResources.Nodes.GetNode( SizeId );
            if( null != Size )
            {
                CswNbtNodePropQuantity Capacity = Size.Capacity;
                Capacity.ToJSON( Ret );
            }
            return Ret;
        }

        public JObject getSizeFromRelatedNodeId( CswPrimaryKey RelatedNodeId )
        {
            JObject Ret = new JObject();
            string SizeId = string.Empty;
            CswNbtNode RelatedNode = _CswNbtResources.Nodes.GetNode( RelatedNodeId );
            if( null != RelatedNode )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[RelatedNodeId];
                if( null != Node )
                {
                    switch( RelatedNode.ObjClass.ObjectClass.ObjectClass )
                    {
                        case CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass:
                            CswNbtObjClassContainer NodeAsContainer = Node;
                            if( null != NodeAsContainer )
                            {
                                SizeId = NodeAsContainer.Size.RelatedNodeId.ToString();
                            }
                            break;
                        case CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass:
                            CswNbtObjClassRequestItem NodeAsRequestItem = Node;
                            if( null != NodeAsRequestItem )
                            {
                                if( null != NodeAsRequestItem.Size.RelatedNodeId && Int32.MinValue != NodeAsRequestItem.Size.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsRequestItem.Size.RelatedNodeId.ToString();
                                }
                                else if( null != NodeAsRequestItem.Container.RelatedNodeId && Int32.MinValue != NodeAsRequestItem.Container.RelatedNodeId.PrimaryKey )
                                {
                                    SizeId = NodeAsRequestItem.Container.RelatedNodeId.ToString();
                                }
                            }
                            break;
                        default:
                            throw new CswDniException( ErrorType.Warning, "Cannot derive a size from an instance of this type " + RelatedNode.ObjClass.ObjectClass.ObjectClass + ".", "getSizeFromRelatedNodeId does not support this Object Class." );
                    }
                }
            }
            if( false == string.IsNullOrEmpty( SizeId ) )
            {
                Ret["sizeid"] = SizeId;
            }
            return Ret;
        }

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
