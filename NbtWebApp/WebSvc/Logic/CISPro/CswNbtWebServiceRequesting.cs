using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.WebSvc.Logic.CISPro;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{

    public class CswNbtWebServiceRequesting
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceRequesting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
            }
        } //ctor

        public JObject getRequestViewGrid( string SessionViewId )
        {
            JObject Ret = new JObject();

            CswNbtSessionDataId SessionDataId = new CswNbtSessionDataId( SessionViewId );
            if( SessionDataId.isSet() )
            {
                CswNbtView CartView = _CswNbtResources.ViewSelect.getSessionView( SessionDataId );
                if( null != CartView )
                {
                    bool IsPropertyGrid = !( CartView.ViewName == CswNbtActRequesting.FavoriteItemsViewName ||
                                             CartView.ViewName == CswNbtActRequesting.RecurringItemsViewName ||
                                             CartView.ViewName == CswNbtActRequesting.SubmittedItemsViewName );
                    CswNbtWebServiceGrid GridWs = new CswNbtWebServiceGrid( _CswNbtResources, CartView, ForReport: false );
                    Ret = GridWs.runGrid( Title: null, IncludeInQuickLaunch: false, GetAllRowsNow: true, IsPropertyGrid: IsPropertyGrid );
                    Ret["grid"]["title"] = "";
                }
            }
            return Ret;
        }

        #region WCF

        private static CswNbtResources _validate( ICswResources CswResources )
        {
            CswNbtResources Ret = null;
            if( null != CswResources )
            {
                Ret = (CswNbtResources) CswResources;
                if( false == Ret.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
                }
            }
            return Ret;
        }

        public static void submitRequest( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, CswNbtNode.Node Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources );
            Ret.Data.Succeeded = ActRequesting.submitRequest( Request.NodeId, Request.NodeName );
        }

        /// <summary>
        /// WCF method to get the NodeTypeId of the Request Material Create 
        /// </summary>
        public static void getRequestMaterialCreate( ICswResources CswResources, CswNbtRequestDataModel.CswNbtRequestMaterialCreateReturn Ret, object Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtMetaDataObjectClass RequestMaterialCreateOc = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialCreateClass );
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
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources );
            ActRequesting.getCart( Ret.Data );
        }

        /// <summary>
        /// WCF method to get current User's tab counts
        /// </summary>
        public static void getCartCounts( ICswResources CswResources, CswNbtRequestDataModel.RequestCart Ret, string Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources );
            ActRequesting.getCart( Ret.Data, CalculateCounts: true );
        }

        /// <summary>
        /// WCF method to create a favorite
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
                CswNbtMetaDataObjectClass RequestOc = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
                CswNbtMetaDataNodeType RequestNt = RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                if( null != RequestNt )
                {
                    CswNbtObjClassRequest Favorite = NbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId, CswEnumNbtMakeNodeOperation.MakeTemp );
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

        private delegate void applyCopyLogic( CswNbtObjClassRequestMaterialDispense RequestItem );

        /// <summary>
        /// WCF method to copy a favorite to the current cart
        /// </summary>
        public static void copyFavorite( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            bool Succeeded = false;
            if( CswTools.IsPrimaryKey( Request.CswRequestId ) && Request.RequestItems.Any() )
            {
                CswNbtObjClassRequest RequestNode = NbtResources.Nodes[Request.CswRequestId];
                if( null != RequestNode )
                {
                    CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( NbtResources );
                    applyCopyLogic SetRequest = ( Item ) =>
                                                    {
                                                        Item.Request.RelatedNodeId = RequestNode.NodeId;
                                                    };
                    Succeeded = ws.copyRequestItems( Request, SetRequest );
                }
            }
            Ret.Data.Succeeded = Succeeded;
        }

        /// <summary>
        /// WCF method to copy request items to recurring
        /// </summary>
        public static void copyRecurring( ICswResources CswResources, CswNbtRequestDataModel.CswRequestReturn Ret, CswNbtRequestDataModel.CswRequestReturn.Ret Request )
        {
            CswNbtResources NbtResources = _validate( CswResources );
            bool Succeeded = false;
            if( Request.RequestItems.Any() )
            {
                CswNbtWebServiceRequesting ws = new CswNbtWebServiceRequesting( NbtResources );
                CswNbtActRequesting act = new CswNbtActRequesting( NbtResources );
                CswNbtObjClassRequest RequestNode = act.getRecurringRequestNode();
                applyCopyLogic SetRequest = ( Item ) =>
                    {
                        Item.Request.RelatedNodeId = RequestNode.NodeId;
                        Item.IsRecurring.Checked = Tristate.True;
                    };
                Succeeded = ws.copyRequestItems( Request, SetRequest );
            }
            Ret.Data.Succeeded = Succeeded;
        }

        private bool copyRequestItems( CswNbtRequestDataModel.CswRequestReturn.Ret Request, applyCopyLogic CopyLogic )
        {
            bool Succeeded = false;
            if( Request.RequestItems.Any() )
            {
                foreach( CswNbtObjClassRequestMaterialDispense NewRequestItem in
                    from Item
                        in Request.RequestItems
                    select _CswNbtResources.Nodes[Item.NodePk]
                        into PropertySetRequest
                        where null != (CswNbtPropertySetRequestItem) PropertySetRequest &&
                        ( ( (CswNbtPropertySetRequestItem) PropertySetRequest ).Type.Value == CswNbtObjClassRequestMaterialDispense.Types.Bulk ||
                        ( (CswNbtPropertySetRequestItem) PropertySetRequest ).Type.Value == CswNbtObjClassRequestMaterialDispense.Types.Size )
                        select CswNbtObjClassRequestMaterialDispense.fromPropertySet( PropertySetRequest )
                            into MaterialDispense
                            where null != MaterialDispense
                            select MaterialDispense.copyNode( ClearRequest: false )
                                into NewPropSetRequest
                                select CswNbtObjClassRequestMaterialDispense.fromPropertySet( NewPropSetRequest ) )
                {
                    NewRequestItem.Status.Value = CswNbtObjClassRequestMaterialDispense.Statuses.Pending;
                    CopyLogic( NewRequestItem );
                    //  NewRequestItem.Requestor.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                    NewRequestItem.postChanges( ForceUpdate: false );
                    Succeeded = true;
                }

            }
            return Succeeded;
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
