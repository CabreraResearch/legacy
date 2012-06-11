using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
                    case CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass:
                        NewNode = CswNbtActCopyNode.CopyGeneratorNode( OriginalNode );
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass:
                        NewNode = CswNbtActCopyNode.CopyInspectionTargetNode( OriginalNode );
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                        NewNode = CswNbtActCopyNode.CopyInspectionDesignNode( OriginalNode );
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

        public bool DeleteNode( CswPrimaryKey NodePk, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _DeleteNode( NodePk, _CswNbtResources );
        }

        private bool _DeleteNode( CswPrimaryKey NodePk, CswNbtResources NbtResources, bool DeleteAllRequiredRelatedNodes = false )
        {
            bool ret = false;
            CswNbtNode NodeToDelete = NbtResources.Nodes.GetNode( NodePk );
            if( null != NodeToDelete )
            {
                NodeToDelete.delete( DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
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
                        if( _DeleteNode( NodePk, NbtSystemResources, DeleteAllRequiredRelatedNodes: true ) )
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
            if( Node != null && null != PropsObj && PropsObj.HasValues )
            {
                foreach( JObject PropObj in
                    from PropJProp
                        in PropsObj.Properties()
                    where null != PropJProp.Value
                    select (JObject) PropJProp.Value
                        into PropObj
                        where PropObj.HasValues
                        select PropObj )
                {
                    addSingleNodeProp( Node, PropObj, Tab );
                }
            }
        }

        public void addSingleNodeProp( CswNbtNode Node, JObject PropObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( CswConvert.ToString( PropObj["id"] ) );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
            
            Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null, Tab );

            // Recurse on sub-props
            if( null != PropObj["subprops"] )
            {
                JObject SubPropsObj = (JObject) PropObj["subprops"];
                if( SubPropsObj.HasValues )
                {
                    foreach( JObject ChildPropObj in SubPropsObj.Properties()
                                .Where( ChildProp => null != ChildProp.Value && ChildProp.Value.HasValues )
                                .Select( ChildProp => (JObject) ChildProp.Value )
                                .Where( ChildPropObj => ChildPropObj.HasValues ) )
                    {
                        addSingleNodeProp( Node, ChildPropObj, Tab );
                    }
                }
            }

        } // _applyPropJson

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
