using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp;
using NbtWebApp.WebSvc.Logic.CISPro;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{

    public class CswNbtWebServiceRequesting
    {
        private readonly CswNbtResources _CswNbtResources;

        private CswNbtActRequesting _RequestAct;

        private void _initOrderingResources( SystemViewName ViewName, CswPrimaryKey RequestNodeId = null )
        {
            _RequestAct = new CswNbtActRequesting( _CswNbtResources, CreateDefaultRequestNode: true, RequestViewName: ViewName, RequestNodeId: RequestNodeId );
        }

        public CswNbtWebServiceRequesting( CswNbtResources CswNbtResources, SystemViewName ViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
            }
            if( ViewName != SystemViewName.CISProRequestCart && ViewName != SystemViewName.CISProRequestHistory )
            {
                ViewName = SystemViewName.CISProRequestCart;
            }
            _initOrderingResources( ViewName, RequestNodeId );

        } //ctor

        public JObject getCurrentRequest( CswNbtActRequesting RequestAct = null )
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

        public JObject copyRequest( CswPrimaryKey CopyFromNodeId, CswPrimaryKey CopyToNodeId )
        {
            /* We're need two instances of CswNbtActSubmitRequest. 
             * The current instance was loaded with CopyFromNodeId
             * For the response we need a new instance with the current RequestNodeId, CopyToNodeId */
            CswNbtActRequesting CopyRequest = _RequestAct.copyRequest( CopyFromNodeId, CopyToNodeId );
            return getCurrentRequest( CopyRequest );
        }

        #region WCF

        private static CswNbtResources _validate( ICswResources CswResources )
        {
            CswNbtResources Ret = null;
            if( null != CswResources )
            {
                Ret = (CswNbtResources) CswResources;
                if( false == Ret.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
                {
                    throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
                }
            }
            return Ret;
        }

        public static void submitRequest( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, NodeSelect.Node Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources, false );
            Ret.Data.Succeeded = ActRequesting.submitRequest( Request.NodePk, Request.NodeName );
        }

        /// <summary>
        /// WCF method to get the NodeTypeId of the Request Material Create 
        /// </summary>
        public static void getRequestMaterialCreate( ICswResources CswResources, CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn Ret, object Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtMetaDataObjectClass RequestMaterialCreateOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass );
            CswNbtMetaDataNodeType FirstNodeType = RequestMaterialCreateOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != FirstNodeType )
            {
                Ret.Data.NodeTypeId = FirstNodeType.NodeTypeId;
            }
        }

        /// <summary>
        /// WCF method to get current User's cart data
        /// </summary>
        public static void getCart( ICswResources CswResources, CswNbtRequestDataModel.RequestCart Ret, object Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources, false );
            Ret.Data.FavoriteItemsViewId = ActRequesting.getFavoriteRequests().SessionViewId.ToString();
        }

        /// <summary>
        /// WCF method to get current User's cart data
        /// </summary>
        public static void createFavorite( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            bool Succeeded = false;
            if( null != Request && false == string.IsNullOrEmpty( Request.RequestId ) )
            {
                CswNbtObjClassRequest Favorite = NbtResources.Nodes[Request.RequestId];
                if( null != Favorite )
                {
                    Favorite.IsTemp = false;
                    Favorite.postChanges( ForceUpdate: false );
                    Succeeded = true;
                }
            }
            else
            {
                CswNbtMetaDataObjectClass RequestOc = NbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );
                CswNbtMetaDataNodeType RequestNt = RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != RequestNt )
                {
                    CswNbtObjClassRequest Favorite = NbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.MakeTemp );
                    if( null != Favorite )
                    {
                        Favorite.IsFavorite.Checked = Tristate.True;
                        Favorite.postChanges( ForceUpdate: false );
                        Succeeded = true;
                        CswPropIdAttr NameIdAttr = new CswPropIdAttr( Favorite.Node, Favorite.Name.NodeTypeProp );
                        Ret.Data.CswRequestId = Favorite.NodeId;
                        Ret.Data.CswRequestName = NameIdAttr;
                    }
                }
            }
            Ret.Data.Succeeded = Succeeded;
        }

        /// <summary>
        /// WCF method to fulfill request
        /// </summary>
        public static void fulfillRequest( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, CswNbtRequestDataModel.RequestFulfill Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
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

        #endregion WCF
    } // class CswNbtWebServiceRequesting

} // namespace ChemSW.Nbt.WebServices
