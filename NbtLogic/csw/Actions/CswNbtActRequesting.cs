using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActRequesting
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _RequestOc = null;
        private ICswNbtUser _ThisUser = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActRequesting( CswNbtResources CswNbtResources, ICswNbtUser ThisUser = null )
        {
            _CswNbtResources = CswNbtResources;
            if( null == ThisUser )
            {
                _ThisUser = _CswNbtResources.CurrentNbtUser;
            }
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
        }

        #endregion Constructor

        #region Public methods and props

        [DataContract]
        public class Cart
        {
            public Cart()
            {

            }

            [DataMember( IsRequired = false )]
            public CswNbtNode.Node CurrentRequest;
            [DataMember( IsRequired = false )]
            public CswNbtView PendingItemsView;
            [DataMember( IsRequired = false )]
            public CswNbtView FavoritesView;
            [DataMember( IsRequired = false )]
            public CswNbtView SubmittedItemsView;
            [DataMember( IsRequired = false )]
            public CswNbtView RecurringItemsView;
            [DataMember( IsRequired = false )]
            public CswNbtView FavoriteItemsView;

            [DataMember]
            public Int32 CopyableObjectClassId;

            [DataMember]
            public CswNbtObjClassUser.Cache.Cart Counts;
        }

        #region Gets

        public ICswNbtTree getOpenCartTree( CswNbtView CartView )
        {
            return _CswNbtResources.Trees.getTreeFromView( CartView, false, false, false );
        }

        public int getCartContentCount()
        {
            return CswNbtObjClassUser.getCurrentUserCache( _CswNbtResources ).CartCounts.PendingRequestItems;
        }
        //Er, not sure what the difference is between these two methods
        public int getCartCount()
        {
            CswNbtView CartView = getOpenCartsView();
            ICswNbtTree Tree = getOpenCartTree( CartView );
            return Tree.getChildNodeCount();
        }

        //I see what this is doing, but how is it possible to have more than one pending request for a given user (or is that the point)?
        private CswNbtObjClassRequest _getFirstPendingRequest()
        {
            CswNbtNode Ret = null;
            if( false == ( _ThisUser is CswNbtSystemUser ) )
            {
                CswNbtView CartsView = getOpenCartsView( IncludeItemProperties: false );
                CswNbtViewRelationship RootVr = CartsView.Root.ChildRelationships[0];

                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

                CartsView.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswEnumNbtFilterMode.Null, ShowInGrid: false );
                CartsView.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswEnumNbtFilterMode.Null, ShowInGrid: false );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CartsView, false, false, false );
                Int32 RequestCount = Tree.getChildNodeCount();
                if( RequestCount > 0 )
                {
                    Int32 ItemCount = 0;
                    Int32 MostItemsPosition = 0;
                    for( Int32 R = 0; R < RequestCount; R += 1 )
                    {
                        Tree.goToNthChild( R );
                        if( Tree.getChildNodeCount() > ItemCount )
                        {
                            ItemCount = Tree.getChildNodeCount();
                            MostItemsPosition = R;
                        }
                        Tree.goToParentNode();
                    }

                    Tree.goToNthChild( MostItemsPosition );
                    Ret = Tree.getNodeForCurrentPosition();
                }
            }
            return Ret;
        }

        private CswNbtObjClassRequest _RecurringRequestNode;

        //so this makes it sound like we get the first, and only the first recurring Request - is that right?
        public CswNbtObjClassRequest getRecurringRequestNode()
        {
            if( null == _RecurringRequestNode &&
                false == ( _ThisUser is CswNbtSystemUser ) )
            {
                CswNbtView RequestView = getRequestViewBase( LimitToUnsubmitted: false, IncludeDefaultFilters: false );
                CswNbtViewRelationship RootVr = RequestView.Root.ChildRelationships[0];

                RequestView.AddViewPropertyAndFilter( RootVr, _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsRecurring ), Value: CswEnumTristate.True.ToString() );
                RequestView.AddViewPropertyAndFilter( RootVr, _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ), SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: _ThisUser.UserId.PrimaryKey.ToString() );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestView, RequireViewPermissions: false, IncludeHiddenNodes: false, IncludeSystemNodes: false );
                if( Tree.getChildNodeCount() > 0 )
                {
                    Tree.goToNthChild( 0 );
                    _RecurringRequestNode = Tree.getNodeForCurrentPosition();
                }

                if( null == _RecurringRequestNode )
                {
                    CswNbtMetaDataNodeType RequestNt = _RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                    if( null == RequestNt )
                    {
                        //This error is misleading - it makes it sound like the user did something wrong
                        throw new CswDniException( CswEnumErrorType.Warning,
                            "Cannot make a Request without a valid Request object.",
                            "No Request NodeType could be found." );
                    }
                    _RecurringRequestNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        ( (CswNbtObjClassRequest) NewNode ).IsRecurring.Checked = CswEnumTristate.True;
                        //_RecurringRequestNode.postChanges( true );
                    } );
                }
            }
            return _RecurringRequestNode;
        }

        public CswPrimaryKey getRecurringRequestNodeId()
        {
            CswPrimaryKey Ret = null;
            CswNbtObjClassRequest RecurringRequest = getRecurringRequestNode();
            if( null != RecurringRequest )
            {
                Ret = RecurringRequest.NodeId;
            }
            return Ret;
        }

        private CswNbtObjClassRequest _CurrentRequestNode;

        //Get the first pending request, or make a new one if one doesn't exist
        public CswNbtObjClassRequest getCurrentRequestNode()
        {
            if( null == _CurrentRequestNode &&
                false == ( _ThisUser is CswNbtSystemUser ) )
            {
                _CurrentRequestNode = _getFirstPendingRequest();
                if( null == _CurrentRequestNode )
                {
                    CswNbtMetaDataNodeType RequestNt = _RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                    if( null == RequestNt )
                    {
                        //This error makes slightly more sense, but again, 
                        //it sounds like it's the user's fault that RequestNT doesn't exist
                        throw new CswDniException( CswEnumErrorType.Warning,
                            "Cannot Submit Request without a valid Request object.",
                            "No Request NodeType could be found." );
                    }
                    _CurrentRequestNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId );
                }
            }
            return _CurrentRequestNode;
        }

        //Is this really necessary?
        public CswPrimaryKey getCurrentRequestNodeId()
        {
            CswPrimaryKey Ret = null;
            CswNbtObjClassRequest RequestNode = getCurrentRequestNode();
            if( null != RequestNode )
            {
                Ret = RequestNode.NodeId;
            }
            return Ret;
        }

        #region Views

        //Okay, so here we have pending requests (not submitted and not completed)
        public CswNbtView getRequestViewBase( bool LimitToUnsubmitted = true, bool IncludeDefaultFilters = true, bool AddRootRel = true )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );

            Ret.Category = "Request Configuration";
            Ret.Visibility = CswEnumNbtViewVisibility.Property;
            Ret.ViewMode = CswEnumNbtViewRenderingMode.Grid;

            if( AddRootRel )
            {
                CswNbtViewRelationship RootVr = Ret.AddViewRelationship( _RequestOc, IncludeDefaultFilters: IncludeDefaultFilters );

                if( LimitToUnsubmitted )
                {
                    CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                    CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

                    Ret.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswEnumNbtFilterMode.Null, ShowInGrid: false );
                    Ret.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswEnumNbtFilterMode.Null, ShowInGrid: false );
                }
            }
            return Ret;
        }

        //Here we take the Request base and, for each requestItem type, add them to the Cart view
        public CswNbtView getOpenCartsView( bool IncludeItemProperties = true )
        {
            CswNbtView Ret = getRequestViewBase( IncludeDefaultFilters: true, LimitToUnsubmitted: true );
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            if( null != _CurrentRequestNode )
            {
                RootVr.NodeIdsToFilterIn.Clear();
                RootVr.NodeIdsToFilterIn.Add( _CurrentRequestNode.NodeId );
            }

            foreach( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( RootVr,
                    CswEnumNbtViewPropOwnerType.Second,
                    RequestOcp, IncludeDefaultFilters: true );

                if( IncludeItemProperties )
                {
                    CswNbtViewProperty Vp1 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                    Vp1.Width = 7;
                    Vp1.Order = 1;
                    CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                    Vp2.Width = 50;
                    Vp2.Order = 2;
                    CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                    Vp3.Order = 3;
                    CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Location ) );
                    Vp4.Width = 40;
                    Vp4.Order = 4;
                    CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ) );
                    Vp5.Width = 20;
                    Vp5.Order = 5;
                    CswNbtViewProperty Vp6 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.RequestedFor ) );
                    Vp6.Order = 6;
                }
            }

            Ret.SaveToCache( IncludeInQuickLaunch: false );
            return Ret;
        }

        public const string PendingItemsViewName = "Pending Request Items";

        //this is the open carts view, scoped to the user's current Request
        public CswNbtView getPendingItemsView()
        {
            CswNbtView Ret = getOpenCartsView();
            Ret.ViewName = PendingItemsViewName;
            CswNbtObjClassRequest Request = getCurrentRequestNode();
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            RootVr.NodeIdsToFilterIn.Clear();
            RootVr.NodeIdsToFilterIn.Add( Request.NodeId );
            return Ret;
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public JObject getRequestItemAddProps( CswNbtPropertySetRequestItem RetAsRequestItem )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            //"Note: this does not get the properties."
            //wat.
            //...oh wait, these add props are going to be different for each type - how are we going to do this?
            return PropsAction.getProps( RetAsRequestItem.Node, "", null, CswEnumNbtLayoutType.Add );
        }

        public const string SubmittedItemsViewName = "Submitted Request Items";


        //takes the request view, adds items, and filters by submitted
        public CswNbtView getSubmittedRequestItemsView()
        {
            CswNbtView Ret = getRequestViewBase( LimitToUnsubmitted: false, AddRootRel: false, IncludeDefaultFilters: false );
            Ret.Visibility = CswEnumNbtViewVisibility.Hidden;
            Ret.ViewName = SubmittedItemsViewName;
            Ret.GridGroupByCol = CswNbtPropertySetRequestItem.PropertyName.Request;

            foreach( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );

                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( MemberOc, IncludeDefaultFilters: true );

                CswNbtViewProperty NameVp = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Name ) );
                NameVp.ShowInGrid = true;
                NameVp.Order = 1;
                CswNbtViewPropertyFilter NameVpf = Ret.AddViewPropertyFilter( NameVp, ShowAtRuntime: true );

                CswNbtViewProperty Vp1 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                Vp1.Width = 7;
                Vp1.Order = 2;

                CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ) );
                Vp2.Order = 3;
                CswNbtViewPropertyFilter StatusVpf = Ret.AddViewPropertyFilter( Vp2, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswNbtPropertySetRequestItem.Statuses.Pending );
                StatusVpf.ShowAtRuntime = true;

                CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                Vp3.Width = 50;
                Vp3.Order = 4;

                CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                Vp4.Order = 5;

                CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request ) );

                CswNbtViewProperty Vp6 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Comments ) );

            }
            return Ret;
        }

        //takes the default request view and filters by favorites
        public CswNbtView getFavoriteRequestNamesView()
        {
            CswNbtView Ret = getRequestViewBase( IncludeDefaultFilters: false, LimitToUnsubmitted: false );
            Ret.ViewName = "Favorite Request Names";
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite ),
                Value: CswEnumTristate.True.ToString(),
                ShowInGrid: false );
            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ),
                SubFieldName: CswEnumNbtSubFieldName.NodeID,
                Value: _ThisUser.UserId.PrimaryKey.ToString(),
                ShowInGrid: false );

            return Ret;
        }

        public const string FavoriteItemsViewName = "Favorite Request Items";

        //takes the default request view, adds items, and filters by favorites
        public CswNbtView getFavoriteRequestsItemsView()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = CswEnumNbtViewVisibility.Hidden,
                ViewMode = CswEnumNbtViewRenderingMode.Grid,
                ViewName = FavoriteItemsViewName,
                GridGroupByCol = CswNbtPropertySetRequestItem.PropertyName.Name
            };

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );

            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( MemberOc, false );

            CswNbtViewProperty NameVp = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Name ) );
            NameVp.Order = 1;
            NameVp.SortBy = true;
            Ret.AddViewPropertyFilter( NameVp, ShowAtRuntime: true );

            CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
            Vp2.Width = 50;
            Vp2.Order = 2;
            CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ) );
            Vp3.Width = 30;
            Vp3.Order = 3;
            CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Location ) );
            Vp4.Width = 40;
            Vp4.Order = 4;

            Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Requestor ), Value: "me", ShowInGrid: false );
            Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsFavorite ), Value: CswNbtNodePropLogical.toLogicalGestalt( CswEnumTristate.True ), ShowInGrid: false );

            return Ret;
        }

        public const string RecurringItemsViewName = "Recurring Request Items";

        //we really shouldn't infer apostraphes in function names
        public CswNbtView getUsersRecurringRequestsItemsView()
        {
            CswNbtView Ret = getAllRecurringRequestsItemsView();

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            CswNbtViewRelationship RequestItemRel = Ret.Root.ChildRelationships[0];

            //We'll use the Current cart for both pending and recurring items and trust the filters to keep them separate
            Ret.AddViewPropertyAndFilter( RequestItemRel,
                MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request ),
                ShowInGrid: false,
                SubFieldName: CswEnumNbtSubFieldName.NodeID,
                Value: getRecurringRequestNode().NodeId.PrimaryKey.ToString() );
            return Ret;
        }

        public CswNbtView getDueRecurringRequestsItemsView()
        {
            CswNbtView Ret = getAllRecurringRequestsItemsView( AddRunTimeFilters: false );

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            CswNbtViewRelationship RequestItemRel = Ret.Root.ChildRelationships[0];

            Ret.AddViewPropertyAndFilter( RequestItemRel,
                MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate ),
                FilterMode: CswEnumNbtFilterMode.LessThanOrEquals,
                Value: "today" );

            return Ret;
        }

        public CswNbtView getAllRecurringRequestsItemsView( bool AddRunTimeFilters = true )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = CswEnumNbtViewVisibility.Hidden,
                ViewMode = CswEnumNbtViewRenderingMode.Grid,
                ViewName = RecurringItemsViewName
            };

            //Unlike other Request Items, Recurring requests are not tied to a Request, so they don't have a Name.

            //^hahaha, wat?

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( MemberOc, IncludeDefaultFilters: false );

            CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
            Vp3.Width = 40;
            Vp3.Order = 1;
            CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency ) );
            Vp4.Order = 2;
            CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate ) );
            Vp5.Order = 3;
            Vp5.SortBy = true;
            if( AddRunTimeFilters )
            {
                Ret.AddViewPropertyFilter( Vp5, ShowAtRuntime: true );
            }
            Ret.AddViewPropertyAndFilter( RequestItemRel,
                MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsRecurring ),
                Value: CswEnumTristate.True.ToString(),
                ShowInGrid: false );
            return Ret;
        }

        #endregion Views

        public Cart getCart( Cart Cart, bool CalculateCounts = false )
        {
            CswNbtObjClassRequest NodeAsRequest = getCurrentRequestNode();
            if( null != NodeAsRequest )
            {
                Cart.CurrentRequest = new CswNbtNode.Node( NodeAsRequest.Node );
            }

            CswNbtView PendingItemsView = getPendingItemsView();
            PendingItemsView.SaveToCache( IncludeInQuickLaunch: false );
            Cart.PendingItemsView = PendingItemsView;

            CswNbtView FavoritesView = getFavoriteRequestNamesView();
            FavoritesView.SaveToCache( IncludeInQuickLaunch: false );
            Cart.FavoritesView = FavoritesView;

            CswNbtView SubmittedItems = getSubmittedRequestItemsView();
            SubmittedItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.SubmittedItemsView = SubmittedItems;

            CswNbtView RecurringItems = getUsersRecurringRequestsItemsView();
            RecurringItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.RecurringItemsView = RecurringItems;

            CswNbtView FavoriteItems = getFavoriteRequestsItemsView();
            FavoriteItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.FavoriteItemsView = FavoriteItems;

            //We'll need to change this to use Type value instead of ObjClassId
            Cart.CopyableObjectClassId = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.RequestMaterialDispenseClass );

            Cart.Counts = CswNbtObjClassUser.getCurrentUserCache( _CswNbtResources ).CartCounts;

            //if( CalculateCounts )
            //{
            //    //This is expensive and we can do it in the next async request
            //    Cart.Counts.PendingRequestItems = getCartContentCount();
            //    Cart.Counts.SubmittedRequestItems = _getItemCount( SubmittedItems );
            //    Cart.Counts.RecurringRequestItems = _getItemCount( RecurringItems );
            //    Cart.Counts.FavoriteRequestItems = _getItemCount( FavoriteItems );
            //}
            return Cart;
        }

        #endregion Gets

        #region Sets

        //This flushes the current CartCount cache and builds it anew
        public static void resetCartCounts( ICswResources Resources )
        {
            CswNbtResources NbtResources = (CswNbtResources) Resources;
            CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources );
            CswNbtObjClassUser ThisUser = NbtResources.Nodes[NbtResources.CurrentNbtUser.UserId];
            ActRequesting.resetCartCounts( ThisUser );
        }

        public void resetCartCounts( CswNbtObjClassUser User )
        {
            if( null != User )
            {
                CswNbtObjClassUser.Cache UserCache = User.CurrentCache;
                UserCache.CartCounts = new CswNbtObjClassUser.Cache.Cart();

                CswNbtView ReqView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass RequestOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
                CswNbtViewRelationship RootVr = ReqView.AddViewRelationship( RequestOc, IncludeDefaultFilters: true );

                CswNbtMetaDataObjectClassProp RequestorOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                ReqView.AddViewPropertyAndFilter( RootVr, RequestorOcp, FilterMode: CswEnumNbtFilterMode.Equals, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: User.NodeId.PrimaryKey.ToString() );

                foreach( CswEnumNbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
                {
                    CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                    CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                    CswNbtViewRelationship RequestItemRel = ReqView.AddViewRelationship( RootVr,
                        CswEnumNbtViewPropOwnerType.Second,
                        RequestOcp, IncludeDefaultFilters: true );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( ReqView, IncludeSystemNodes: false, RequireViewPermissions: false, IncludeHiddenNodes: false );
                Int32 RequestCount = Tree.getChildNodeCount();
                if( RequestCount > 0 )
                {
                    for( Int32 R = 0; R < RequestCount; R += 1 )
                    {
                        Tree.goToNthChild( R );
                        CswNbtObjClassRequest Request = Tree.getNodeForCurrentPosition();
                        if( Request.IsFavorite.Checked == CswEnumTristate.True )
                        {
                            UserCache.CartCounts.FavoriteRequestItems += Tree.getChildNodeCount();
                        }
                        else if( Request.IsRecurring.Checked == CswEnumTristate.True )
                        {
                            UserCache.CartCounts.RecurringRequestItems += Tree.getChildNodeCount();
                        }
                        else
                        {
                            for( Int32 I = 0; I < Tree.getChildNodeCount(); I += 1 )
                            {
                                Tree.goToNthChild( I );
                                CswNbtPropertySetRequestItem RequestItem = Tree.getNodeForCurrentPosition();
                                if( RequestItem.Status.Text == CswNbtPropertySetRequestItem.Statuses.Pending )
                                {
                                    UserCache.CartCounts.PendingRequestItems += 1;
                                }
                                else
                                {
                                    UserCache.CartCounts.SubmittedRequestItems += 1;
                                }
                                Tree.goToParentNode();
                            }
                        }
                        Tree.goToParentNode();
                    }
                }
                User.CurrentCache = UserCache;
                User.postChanges( ForceUpdate: false );
            }
        }

        //Ah, so simple - this is probably the one function I won't have to change
        public bool submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            bool Ret = false;
            if( null != NodeId )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( NodeId );
                if( null != NodeAsRequest )
                {
                    if( getCartContentCount() > 0 )
                    {
                        NodeAsRequest.SubmittedDate.DateTimeValue = DateTime.Now;
                        NodeAsRequest.Name.Text = NodeName;
                        NodeAsRequest.postChanges( true );

                        Ret = true;
                    }
                }
            }

            return Ret;
        }

        //this description doesn't help - it creates dispense, move, or dispose depending on the selected button menu item
        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtPropertySetRequestItem makeContainerRequestItem( CswNbtObjClassContainer Container, CswNbtObjClass.NbtButtonData ButtonData, CswNbtMetaDataObjectClass ItemOc = null )
        {
            //Set Type
            CswNbtPropertySetRequestItem ret = null;
            if( null == ItemOc )
            {
                if( ButtonData.SelectedText == CswEnumNbtContainerRequestMenu.Dispense )
                {
                    ItemOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestContainerDispenseClass );
                }
                else
                {
                    ItemOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestContainerUpdateClass );
                }
            }

            if( null != ItemOc )
            {
                CswNbtMetaDataNodeType RequestItemNt = ItemOc.getNodeTypes().FirstOrDefault();
                CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );

                CswNbtNodeCollection.AfterMakeNode After = delegate( CswNbtNode NewNode )
                {
                    CswNbtPropertySetRequestItem RetAsRequestItem = NewNode;
                    if( null != getCurrentRequestNodeId() && null != Container )
                    {
                        //set location
                        CswPrimaryKey SelectedLocationId = new CswPrimaryKey();
                        if( CswTools.IsPrimaryKey( _ThisUser.DefaultLocationId ) )
                        {
                            SelectedLocationId = _ThisUser.DefaultLocationId;
                        }
                        else
                        {
                            SelectedLocationId = Container.Location.SelectedNodeId;
                        }
                        ButtonData.Action = CswEnumNbtButtonAction.request;
                        if( ButtonData.SelectedText == CswEnumNbtContainerRequestMenu.Dispense )
                        {
                            CswNbtObjClassRequestContainerDispense RetAsDispense = CswNbtObjClassRequestContainerDispense.fromPropertySet( RetAsRequestItem );

                            //set which properties are visible/readonly in the add layout
                            RetAsDispense.Container.RelatedNodeId = Container.NodeId;
                            RetAsDispense.Container.setReadOnly( value: true, SaveToDb: true );
                            RetAsDispense.Material.RelatedNodeId = Container.Material.RelatedNodeId;
                            RetAsDispense.Material.setReadOnly( value: true, SaveToDb: false );
                            RetAsDispense.Material.setHidden( value: true, SaveToDb: false );
                            RetAsDispense.Quantity.UnitId = Container.Quantity.UnitId;
                            RetAsDispense.Size.RelatedNodeId = Container.Size.RelatedNodeId;
                            RetAsDispense.Location.SelectedNodeId = SelectedLocationId;

                            _setRequestItemSizesView( RetAsDispense.Size.View.ViewId, Container.Material.RelatedNodeId );
                            CswNbtNode MaterialNode = _CswNbtResources.Nodes[Container.Material.RelatedNodeId];
                            Debug.Assert( null != MaterialNode, "RequestItem created without a valid Material." );
                            if( null != MaterialNode )
                            {
                                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                                Vb.setQuantityUnitOfMeasureView( MaterialNode, RetAsDispense.Quantity );
                            }
                        }
                        else
                        {
                            CswNbtObjClassRequestContainerUpdate RetAsUpdate = CswNbtObjClassRequestContainerUpdate.fromPropertySet( RetAsRequestItem );
                            RetAsUpdate.Container.RelatedNodeId = Container.NodeId;
                            RetAsUpdate.Container.setReadOnly( value: true, SaveToDb: true );

                            switch( ButtonData.SelectedText )
                            {
                                case CswEnumNbtContainerRequestMenu.Dispose:
                                    //RetAsUpdate.IsTemp = false; // This is the only condition in which we want to commit the node upfront.
                                    RetAsUpdate.Type.Value = CswNbtObjClassRequestContainerUpdate.Types.Dispose;
                                    RetAsUpdate.Location.SelectedNodeId = Container.Location.SelectedNodeId;
                                    RetAsUpdate.Location.setReadOnly( value: true, SaveToDb: true );
                                    break;
                                case CswEnumNbtContainerRequestMenu.Move:
                                    RetAsUpdate.Location.SelectedNodeId = SelectedLocationId;
                                    RetAsUpdate.Type.Value = CswNbtObjClassRequestContainerUpdate.Types.Move;
                                    break;
                            }
                        }

                        RetAsRequestItem.Location.RefreshNodeName();
                        RetAsRequestItem.Type.setReadOnly( value: true, SaveToDb: true );

                        //RetAsRequestItem.postChanges( ForceUpdate: false );
                    }
                }; // AfterMakeNode

                ret = PropsAction.getAddNodeAndPostChanges( RequestItemNt, After );//this does everything above
                if( ButtonData.SelectedText == CswEnumNbtContainerRequestMenu.Dispose )
                {
                    // This is the only condition in which we want to commit the node upfront.
                    //why?
                    ret.PromoteTempToReal();
                }

                if( null == ret )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
                }

            } // if( null != ItemOc )
            return ret;
        }

        //
        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtPropertySetRequestItem makeMaterialRequestItem( CswEnumNbtRequestItemType Item, CswPrimaryKey MaterialId, CswNbtObjClass.NbtButtonData ButtonData )
        {
            CswNbtPropertySetRequestItem Ret = null;
            CswNbtMetaDataObjectClass ItemOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            CswNbtMetaDataNodeType RequestItemNt = ItemOc.getNodeTypes().FirstOrDefault();
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( null != RequestItemNt )
            {
                //this one doesn't call postchanges, so why does the other one?
                Ret = PropsAction.getAddNode( RequestItemNt.NodeTypeId, MaterialId.ToString(), delegate( CswNbtNode NewNode )
                {
                    CswNbtPropertySetRequestItem RetAsRequestItem = NewNode;
                    if( null == RetAsRequestItem )
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
                    }
                    if( null == RetAsRequestItem.Request.RelatedNodeId )
                    {
                        RetAsRequestItem.Request.RelatedNodeId = getCurrentRequestNodeId();
                    }
                    //if( null == RetAsRequestItem.Material.RelatedNodeId )
                    //{
                    //    RetAsRequestItem.Material.RelatedNodeId = MaterialId;
                    //}
                    if( null != _ThisUser.DefaultLocationId )
                    {
                        CswNbtObjClassLocation DefaultAsLocation = _CswNbtResources.Nodes.GetNode( _ThisUser.DefaultLocationId );
                        if( null != DefaultAsLocation )
                        {
                            RetAsRequestItem.Location.SelectedNodeId = _ThisUser.DefaultLocationId;
                            RetAsRequestItem.Location.CachedNodeName = DefaultAsLocation.Location.CachedNodeName;
                            RetAsRequestItem.Location.CachedPath = DefaultAsLocation.Location.CachedPath;
                        }
                    }
                    switch( ButtonData.SelectedText )
                    {
                        case CswNbtPropertySetMaterial.CswEnumRequestOption.Size:
                            RetAsRequestItem.Type.Value = CswNbtObjClassRequestMaterialDispense.Types.Size;
                            CswNbtObjClassRequestMaterialDispense RetAsMatDisp = CswNbtObjClassRequestMaterialDispense.fromPropertySet( RetAsRequestItem );
                            _setRequestItemSizesView( RetAsMatDisp.Size.View.ViewId, MaterialId );
                            break;

                        default: //Request or Bulk
                            RetAsRequestItem.Type.Value = CswNbtObjClassRequestMaterialDispense.Types.Bulk;
                            break;
                    }
                } );
            }
            return Ret;
        }

        #endregion Sets

        #endregion Public methods and props

        #region Private helper functions

        private void _setRequestItemSizesView( CswNbtViewId SizeViewId, CswPrimaryKey SizeMaterialId )
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtView SizeView = _CswNbtResources.ViewSelect.restoreView( SizeViewId );
            SizeView.Root.ChildRelationships.Clear();

            CswNbtViewRelationship SizeVr = SizeView.AddViewRelationship( SizeOc, false );

            SizeView.AddViewPropertyAndFilter( SizeVr, SizeMaterialOcp, SizeMaterialId.PrimaryKey.ToString(), SubFieldName: CswEnumNbtSubFieldName.NodeID );
            SizeView.AddViewPropertyAndFilter( SizeVr, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ), "false", FilterMode: CswEnumNbtFilterMode.NotEquals );

            SizeView.AddViewPropertyAndFilter( SizeVr,
                MetaDataProp: SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity ),
                FilterMode: CswEnumNbtFilterMode.NotNull );
            SizeView.AddViewPropertyAndFilter( SizeVr,
                MetaDataProp: SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount ),
                FilterMode: CswEnumNbtFilterMode.NotNull );

            SizeView.save();
        }

        #endregion

    }


}
