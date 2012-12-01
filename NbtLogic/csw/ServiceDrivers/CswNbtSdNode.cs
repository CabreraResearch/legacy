using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        public CswNbtSdNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents = null )
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
                    case NbtObjectClass.EquipmentClass:
                        NewNode = CswNbtActCopyNode.CopyEquipmentNode( OriginalNode );
                        break;
                    case NbtObjectClass.EquipmentAssemblyClass:
                        NewNode = CswNbtActCopyNode.CopyEquipmentAssemblyNode( OriginalNode );
                        break;
                    case NbtObjectClass.GeneratorClass:
                        NewNode = CswNbtActCopyNode.CopyGeneratorNode( OriginalNode );
                        break;
                    case NbtObjectClass.InspectionTargetClass:
                        NewNode = CswNbtActCopyNode.CopyInspectionTargetNode( OriginalNode );
                        break;
                    case NbtObjectClass.InspectionDesignClass:
                        NewNode = CswNbtActCopyNode.CopyInspectionDesignNode( OriginalNode );
                        break;
                    default:
                        NewNode = CswNbtActCopyNode.CopyNode( OriginalNode );
                        break;
                }

                if( NewNode != null && null != _CswNbtStatisticsEvents )
                {
                    _CswNbtStatisticsEvents.OnCopyNode( OriginalNode, NewNode );
                    RetKey = NewNode.NodeId;
                }
            }
            return RetKey;
        }

        public bool DeleteNode( CswPrimaryKey NodePk, out string NodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _DeleteNode( NodePk, _CswNbtResources, out NodeName, DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
        }

        private bool _DeleteNode( CswPrimaryKey NodePk, CswNbtResources NbtResources, out string NodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            bool ret = false;
            NodeName = "";
            CswNbtNode NodeToDelete = NbtResources.Nodes[NodePk];
            if( null != NodeToDelete )
            {
                CswNbtMetaDataNodeType NodeType = NodeToDelete.getNodeType();
                NodeName = NodeType.NodeTypeName + ": " + NodeToDelete.NodeName;
                NodeToDelete.delete( DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes );
                ret = true;
            }
            return ret;
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText )
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

            CswNbtObjClass.NbtButtonData ButtonData = new CswNbtObjClass.NbtButtonData( NodeTypeProp ) { SelectedText = SelectedText };

            bool Success = NbtObjClass.onButtonClick( ButtonData );

            RetObj["action"] = ButtonData.Action.ToString();
            RetObj["actionData"] = ButtonData.Data;  //e.g. popup url
            RetObj["message"] = ButtonData.Message;
            RetObj["success"] = Success.ToString().ToLower();

            return RetObj;
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
            CswNbtMetaDataNodeType NodeType = MetaDataProp.getNodeType();

            if( _CswNbtResources.Permit.canNodeType( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                _CswNbtResources.Permit.canTab( Security.CswNbtPermit.NodeTypePermission.Edit, NodeType, Tab ) ||
                _CswNbtResources.Permit.isPropWritable( Security.CswNbtPermit.NodeTypePermission.Edit, MetaDataProp, Tab ) )
            {
                Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

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

            }//if user has permission to edit the property

        } // _applyPropJson

    } // class CswNbtSdNode

} // namespace ChemSW.Nbt.WebServices
