using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Logic.CISPro;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{

    public class CswNbtWebServiceRequesting
    {
        private readonly CswNbtResources _CswNbtResources;

        private CswNbtActSubmitRequest _RequestAct;

        private void _initOrderingResources( CswNbtActSystemViews.SystemViewName ViewName, CswPrimaryKey RequestNodeId = null )
        {
            _RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true, RequestViewName: ViewName, RequestNodeId: RequestNodeId );
        }

        public CswNbtWebServiceRequesting( CswNbtResources CswNbtResources, CswNbtActSystemViews.SystemViewName ViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
            }
            if( ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory )
            {
                ViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _initOrderingResources( ViewName, RequestNodeId );

        } //ctor

        public JObject getCurrentRequest( CswNbtActSubmitRequest RequestAct = null )
        {
            RequestAct = RequestAct ?? _RequestAct;
            JObject ret = new JObject();
            CswNbtWebServiceGrid GridWs = new CswNbtWebServiceGrid( _CswNbtResources, RequestAct.CurrentCartView, ForReport: false );
            ret = GridWs.runGrid( IncludeInQuickLaunch: false, GetAllRowsNow: true, IsPropertyGrid: true );
            ret["cartnodeid"] = RequestAct.CurrentRequestNode().NodeId.ToString();
            ret["cartviewid"] = RequestAct.CurrentCartView.SessionViewId.ToString();
            return ret;
        }

        public JObject getCurrentRequestId()
        {
            JObject ret = new JObject();
            CswNbtNode CurrentRequest = _RequestAct.CurrentRequestNode();
            ret["cartnodeid"] = CurrentRequest.NodeId.ToString();
            return ret;
        }

        public JObject getRequestHistory()
        {
            return _RequestAct.getRequestHistory();
        }

        public JObject submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            return _RequestAct.submitRequest( NodeId, NodeName );
        }

        public JObject copyRequest( CswPrimaryKey CopyFromNodeId, CswPrimaryKey CopyToNodeId )
        {
            /* We're need two instances of CswNbtActSubmitRequest. 
             * The current instance was loaded with CopyFromNodeId
             * For the response we need a new instance with the current RequestNodeId, CopyToNodeId */
            CswNbtActSubmitRequest CopyRequest = _RequestAct.copyRequest( CopyFromNodeId, CopyToNodeId );
            return getCurrentRequest( CopyRequest );
        }

        /// <summary>
        /// WCF method to get the NodeTypeId of the Request Material Create 
        /// </summary>
        public static void getRequestMaterialCreate( ICswResources CswResources, CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn Ret, object Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                if( false == NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                {
                    throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
                }
                CswNbtMetaDataObjectClass RequestMaterialCreateOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass );
                CswNbtMetaDataNodeType FirstNodeType = RequestMaterialCreateOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != FirstNodeType )
                {
                    Ret.Data.NodeTypeId = FirstNodeType.NodeTypeId;
                }
            }
        }

        /// <summary>
        /// WCF method to fulfill request
        /// </summary>
        public static void fulfillRequest( ICswResources CswResources, CswNbtRequestDataModel.CswNbtRequestMaterialDispenseReturn Ret, CswNbtRequestDataModel.RequestFulfill Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                if( false == NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                {
                    throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
                }
                CswNbtPropertySetRequestItem RequestAsPropSet = NbtResources.Nodes[Request.RequestItemId];
                if( null != RequestAsPropSet )
                {
                    switch( RequestAsPropSet.Type.Value )
                    {
                        case CswNbtObjClassRequestMaterialDispense.Types.Size:
                            CswNbtObjClassRequestMaterialDispense RequestNode = CswNbtObjClassRequestMaterialDispense.fromPropertySet( RequestAsPropSet );
                            Int32 ContainersMoved = moveContainers( NbtResources, RequestNode, Request );
                            Ret.Data.Succeeded = ContainersMoved > 0;
                            if( Ret.Data.Succeeded )
                            {
                                if( CswTools.IsDouble( RequestNode.TotalMoved.Value ) )
                                {
                                    RequestNode.TotalMoved.Value += ContainersMoved;
                                }
                                else
                                {
                                    RequestNode.TotalMoved.Value = ContainersMoved;
                                }
                                RequestNode.Status.Value = CswNbtObjClassRequestMaterialDispense.Statuses.Moved;
                                RequestNode.postChanges( ForceUpdate: false );
                            }
                            break;
                    }
                }
            }
        }

        private static Int32 moveContainers( CswNbtResources NbtResources, CswNbtObjClassRequestMaterialDispense RequestNode, CswNbtRequestDataModel.RequestFulfill Request )
        {
            Int32 Ret = 0;
            if( null != RequestNode )
            {
                foreach( string ContainerId in Request.ContainerIds )
                {
                    CswNbtObjClassContainer ContainerNode = NbtResources.Nodes[ContainerId];
                    if( null != ContainerNode )
                    {
                        ContainerNode.Location.SelectedNodeId = RequestNode.Location.SelectedNodeId;
                        ContainerNode.Location.RefreshNodeName();
                        ContainerNode.postChanges( ForceUpdate: false );
                        Ret += 1;
                    }
                }
            }
            return Ret;
        }

    } // class CswNbtWebServiceRequesting

} // namespace ChemSW.Nbt.WebServices
