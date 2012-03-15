using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
        }

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            CswPrimaryKey RetKey = null;
            CswNbtNode OriginalNode = _CswNbtResources.Nodes.GetNode( NodePk );

            if( null != OriginalNode )
            {
                CswNbtNode NewNode = null;
                CswNbtActCopyNode CswNbtActCopyNode = new CswNbtActCopyNode( _CswNbtResources );
                switch( OriginalNode.getObjectClass().ObjectClass )
                {
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass:
                        NewNode = CswNbtActCopyNode.CopyEquipmentNode( OriginalNode );
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass:
                        NewNode = CswNbtActCopyNode.CopyEquipmentAssemblyNode( OriginalNode );
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass:
                        NewNode = CswNbtActCopyNode.CopyInspectionTargetNode( OriginalNode );
                        break;
                    default:
                        NewNode = CswNbtActCopyNode.CopyNode( OriginalNode );
                        break;
                }

                if( NewNode != null )
                {
                    _CswNbtStatisticsEvents.OnCopyNode( OriginalNode, NewNode );
                    RetKey = NewNode.NodeId;
                }
            }
            return RetKey;
        }

        public bool DeleteNode( CswPrimaryKey NodePk )
        {
            bool ret = false;
            CswNbtNode NodeToDelete = _CswNbtResources.Nodes.GetNode( NodePk );
            if( null != NodeToDelete )
            {
                NodeToDelete.delete();
                ret = true;
            }
            return ret;
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId )
        {
            JObject RetObj = new JObject();
            if( null == PropId ||
                Int32.MinValue == PropId.NodeTypePropId ||
                null == PropId.NodeId ||
                Int32.MinValue == PropId.NodeId.PrimaryKey )
            {
                throw new CswDniException( ErrorType.Error, "Cannot execute a button click without valid parameters.", "Attempted to call DoObjectClassButtonClick with invalid NodeId and NodeTypePropId." );
            }

            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( PropId.NodeId );
            if( null == Node )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid node with the provided parameters.", "No node exists for NodePk " + PropId.NodeId.ToString() + "." );
            }

            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( null == NodeTypeProp )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid property with the provided parameters.", "No property exists for NodeTypePropId " + PropId.NodeTypePropId.ToString() + "." );
            }

            CswNbtObjClass NbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, Node.getObjectClassId(), Node );

            CswNbtObjClass.NbtButtonAction ButtonAction = CswNbtObjClass.NbtButtonAction.Unknown;
            string ActionData;
            string Message;
            bool Success = NbtObjClass.onButtonClick( NodeTypeProp, out ButtonAction, out ActionData, out Message );

            RetObj["action"] = ButtonAction.ToString();
            RetObj["actiondata"] = ActionData;  //e.g. popup url
            RetObj["message"] = Message;
            RetObj["success"] = Success.ToString().ToLower();

            return RetObj;
        }

        public bool deleteDemoDataNodes()
        {
            bool RetSuccess = true;
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                CswTableSelect NodesSelect = new CswTableSelect( _CswNbtResources.CswResources, "delete_demodata_nodes",
                                                                "nodes" );
                DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" },
                                                            " where to_char(isdemo)='1' " );
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                    RetSuccess = RetSuccess && DeleteNode( NodePk );
                }
            }
            else
            {
                RetSuccess = false;
            }
            return RetSuccess;
        }

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
