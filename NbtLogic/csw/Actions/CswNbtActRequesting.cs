using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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
        private CswNbtMetaDataObjectClass _RequestOC = null;
        private CswNbtMetaDataObjectClass _RequestItemOC = null;
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
            _RequestOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            _RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
        }

        #endregion Constructor

        #region Public methods and props

        [DataContract]
        public class Cart
        {
            public Cart()
            {
                CopyableRequestTypes = new Collection<string>();
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
            public Collection<String> CopyableRequestTypes;

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

                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

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

                RequestView.AddViewPropertyAndFilter( RootVr, _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsRecurring ), Value: CswEnumTristate.True.ToString() );
                RequestView.AddViewPropertyAndFilter( RootVr, _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ), SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: _ThisUser.UserId.PrimaryKey.ToString() );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestView, RequireViewPermissions: false, IncludeHiddenNodes: false, IncludeSystemNodes: false );
                if( Tree.getChildNodeCount() > 0 )
                {
                    Tree.goToNthChild( 0 );
                    _RecurringRequestNode = Tree.getNodeForCurrentPosition();
                }

                if( null == _RecurringRequestNode )
                {
                    CswNbtMetaDataNodeType RequestNt = _RequestOC.getLatestVersionNodeTypes().FirstOrDefault();
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
                    } );
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
                    CswNbtMetaDataNodeType RequestNt = _RequestOC.getLatestVersionNodeTypes().FirstOrDefault();
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
                CswNbtViewRelationship RootVr = Ret.AddViewRelationship( _RequestOC, IncludeDefaultFilters: IncludeDefaultFilters );

                if( LimitToUnsubmitted )
                {
                    CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                    CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

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
            CswNbtMetaDataObjectClassProp RequestOcp = _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( RootVr,
                CswEnumNbtViewPropOwnerType.Second,
                RequestOcp, IncludeDefaultFilters: true );

            if( IncludeItemProperties )
            {
                CswNbtViewProperty ItemNumberVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ItemNumber ), 1 );
                ItemNumberVP.Width = 10;
                CswNbtViewProperty DescriptionVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description ), 2 );
                DescriptionVP.Width = 50;
                CswNbtViewProperty NeededByVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NeededBy ), 3 );
                CswNbtViewProperty LocationVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location ), 4 );
                LocationVP.Width = 40;
                CswNbtViewProperty InventoryGroupVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup ), 5 );
                InventoryGroupVP.Width = 20;
                CswNbtViewProperty RequestedForVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestedFor ), 6 );
                CswNbtViewProperty RequestTypeVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestType ), 7 );
                RequestTypeVP.Width = 20;
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
        /// Get the Add Layout Properties for the given RequestItem
        /// </summary>
        public JObject getRequestItemAddProps( CswNbtNode RequestItemNode )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            return PropsAction.getProps( RequestItemNode, "", null, CswEnumNbtLayoutType.Add );
        }

        public const string SubmittedItemsViewName = "Submitted Request Items";


        //takes the request view, adds items, and filters by submitted
        public CswNbtView getSubmittedRequestItemsView()
        {
            CswNbtView Ret = getRequestViewBase( LimitToUnsubmitted: false, AddRootRel: false, IncludeDefaultFilters: false );
            Ret.Visibility = CswEnumNbtViewVisibility.Hidden;
            Ret.ViewName = SubmittedItemsViewName;
            Ret.GridGroupByCol = CswNbtObjClassRequestItem.PropertyName.Request;
            CswNbtMetaDataObjectClassProp RequestOcp = _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( _RequestItemOC, IncludeDefaultFilters: true );

            CswNbtViewProperty NameVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Name ), 1 );
            //NameVP.ShowInGrid = true;
            CswNbtViewPropertyFilter NameVpf = Ret.AddViewPropertyFilter( NameVP, ShowAtRuntime: true );
            CswNbtViewProperty ItemNumberVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ItemNumber ), 2 );
            ItemNumberVP.Width = 10;
            CswNbtViewProperty StatusVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status ), 3 );
            CswNbtViewPropertyFilter StatusVpf = Ret.AddViewPropertyFilter( StatusVP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswNbtObjClassRequestItem.Statuses.Pending );
            StatusVpf.ShowAtRuntime = true;
            CswNbtViewProperty DescriptionVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description ), 4 );
            DescriptionVP.Width = 50;
            CswNbtViewProperty NeededByVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NeededBy ), 5 );
            CswNbtViewProperty RequestVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request ) );
            CswNbtViewProperty CommentsVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Comments ) );
            CswNbtViewProperty RequestTypeVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestType ), 7 );
            RequestTypeVP.Width = 20;

            return Ret;
        }

        //takes the default request view and filters by favorites
        public CswNbtView getFavoriteRequestNamesView()
        {
            CswNbtView Ret = getRequestViewBase( IncludeDefaultFilters: false, LimitToUnsubmitted: false );
            Ret.ViewName = "Favorite Request Names";
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite ),
                Value: CswEnumTristate.True.ToString(),
                ShowInGrid: false );
            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ),
                SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
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
                GridGroupByCol = CswNbtObjClassRequestItem.PropertyName.Name
            };
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( _RequestItemOC, false );
            CswNbtViewProperty NameVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Name ), 1 );
            NameVP.SortBy = true;
            Ret.AddViewPropertyFilter( NameVP, ShowAtRuntime: true );
            CswNbtViewProperty DescriptionVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description ), 2 );
            DescriptionVP.Width = 50;
            CswNbtViewProperty InventoryGroupVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup ), 3 );
            InventoryGroupVP.Width = 30;
            CswNbtViewProperty LocationVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location ), 4 );
            LocationVP.Width = 40;
            Ret.AddViewPropertyAndFilter( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ), Value: "me", ShowInGrid: false );
            Ret.AddViewPropertyAndFilter( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsFavorite ), Value: CswNbtNodePropLogical.toLogicalGestalt( CswEnumTristate.True ), ShowInGrid: false );
            return Ret;
        }

        public const string RecurringItemsViewName = "Recurring Request Items";

        public CswNbtView getUsersRecurringRequestItemsView()
        {
            CswNbtView Ret = getAllRecurringRequestItemsView();
            CswNbtViewRelationship RequestItemRel = Ret.Root.ChildRelationships[0];
            //We'll use the Current cart for both pending and recurring items and trust the filters to keep them separate
            Ret.AddViewPropertyAndFilter( RequestItemRel,
                _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request ),
                ShowInGrid: false,
                SubFieldName : CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                Value: getRecurringRequestNode().NodeId.PrimaryKey.ToString() );
            return Ret;
        }

        public CswNbtView getDueRecurringRequestItemsView()
        {
            CswNbtView Ret = getAllRecurringRequestItemsView( AddRunTimeFilters: false );
            CswNbtViewRelationship RequestItemRel = Ret.Root.ChildRelationships[0];
            Ret.AddViewPropertyAndFilter( RequestItemRel,
                _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NextReorderDate ),
                FilterMode: CswEnumNbtFilterMode.LessThanOrEquals,
                Value: "today" );
            return Ret;
        }

        public CswNbtView getAllRecurringRequestItemsView( bool AddRunTimeFilters = true )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = CswEnumNbtViewVisibility.Hidden,
                ViewMode = CswEnumNbtViewRenderingMode.Grid,
                ViewName = RecurringItemsViewName
            };
            //Unlike other Request Items, Recurring requests are not tied to a Request, so they don't have a Name.
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( _RequestItemOC, IncludeDefaultFilters: false );
            CswNbtViewProperty DescriptionVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description ), 1 );
            DescriptionVP.Width = 40;
            CswNbtViewProperty RecurringFrequencyVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RecurringFrequency ), 2 );
            CswNbtViewProperty NextReorderDateVP = Ret.AddViewProperty( RequestItemRel, _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NextReorderDate ), 3 );
            NextReorderDateVP.SortBy = true;
            if( AddRunTimeFilters )
            {
                Ret.AddViewPropertyFilter( NextReorderDateVP, ShowAtRuntime: true );
            }
            Ret.AddViewPropertyAndFilter( RequestItemRel,
                _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsRecurring ),
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

            CswNbtView RecurringItems = getUsersRecurringRequestItemsView();
            RecurringItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.RecurringItemsView = RecurringItems;

            CswNbtView FavoriteItems = getFavoriteRequestsItemsView();
            FavoriteItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.FavoriteItemsView = FavoriteItems;

            Cart.CopyableRequestTypes = new Collection<String>
                                            {
                                                CswNbtObjClassRequestItem.Types.EnterprisePart,
                                                CswNbtObjClassRequestItem.Types.MaterialBulk,
                                                CswNbtObjClassRequestItem.Types.MaterialSize
                                            };

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
                CswNbtViewRelationship RootVr = ReqView.AddViewRelationship( RequestOc, IncludeDefaultFilters: false );

                CswNbtMetaDataObjectClassProp RequestorOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                ReqView.AddViewPropertyAndFilter( RootVr, RequestorOcp, FilterMode: CswEnumNbtFilterMode.Equals, SubFieldName: CswEnumNbtSubFieldName.NodeID, Value: User.NodeId.PrimaryKey.ToString() );

                CswNbtMetaDataObjectClassProp RequestOcp = _RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
                CswNbtViewRelationship RequestItemRel = ReqView.AddViewRelationship( RootVr,
                    CswEnumNbtViewPropOwnerType.Second,
                    RequestOcp, IncludeDefaultFilters: false );

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
                                CswNbtObjClassRequestItem RequestItem = Tree.getNodeForCurrentPosition();
                                if( RequestItem.Status.Text == CswNbtObjClassRequestItem.Statuses.Pending )
                                {
                                    UserCache.CartCounts.PendingRequestItems += 1;
                                }
                                else//Won't this also include completed and cancelled Request Items?  Is that what we want?
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

        /// <summary>
        /// Sumbits all pending Request Items
        /// </summary>
        /// <returns>true on success</returns>
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

        /// <summary>
        /// Instance a new Container Request Item based on the selected button option - the type will either be Dispense, Move, or Dispose.
        /// </summary>
        public CswNbtObjClassRequestItem makeContainerRequestItem( CswNbtObjClassContainer Container, CswNbtObjClass.NbtButtonData ButtonData )
        {
            checkForCentralInventoryGroups( _CswNbtResources );
            CswNbtObjClassRequestItem RequestItem;
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType RequestItemNT = RequestItemOC.getNodeTypes().FirstOrDefault();
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            RequestItem = PropsAction.getAddNodeAndPostChanges( RequestItemNT, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassRequestItem RequestItemNode = NewNode;
                if( null != getCurrentRequestNodeId() && null != Container )
                {
                    RequestItemNode.Container.RelatedNodeId = Container.NodeId;
                    RequestItemNode.Material.RelatedNodeId = Container.Material.RelatedNodeId;
                    CswPrimaryKey SelectedLocationId = CswTools.IsPrimaryKey( _ThisUser.DefaultLocationId ) ? 
                        _ThisUser.DefaultLocationId : 
                        Container.Location.SelectedNodeId;
                    ButtonData.Action = CswEnumNbtButtonAction.request;
                    switch( ButtonData.SelectedText )
                    {
                        case CswEnumNbtContainerRequestMenu.Dispense:
                            RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.ContainerDispense;
                            RequestItemNode.Quantity.UnitId = Container.Quantity.UnitId;
                            RequestItemNode.Size.RelatedNodeId = Container.Size.RelatedNodeId;
                            RequestItemNode.Location.SelectedNodeId = SelectedLocationId;
                            //Scope available units of measure on Quantity based on the Container's Material
                            CswNbtNode MaterialNode = _CswNbtResources.Nodes[Container.Material.RelatedNodeId];
                            if( null != MaterialNode )
                            {
                                CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                                Vb.setQuantityUnitOfMeasureView( MaterialNode, RequestItemNode.Quantity );
                            }
                            break;
                        case CswEnumNbtContainerRequestMenu.Dispose:
                            RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.ContainerDispose;
                            RequestItemNode.Location.SelectedNodeId = Container.Location.SelectedNodeId;
                            break;
                        case CswEnumNbtContainerRequestMenu.Move:
                            RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.ContainerMove;
                            RequestItemNode.Location.SelectedNodeId = SelectedLocationId;
                            break;
                    }
                    RequestItemNode.Location.RefreshNodeName();
                }
            } );
            if( null == RequestItem )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
            }
            return RequestItem;
        }

        /// <summary>
        /// Instance a new Material Request Item based on the selected button option - the type will either be By Bulk or By Size.
        /// </summary>
        public CswNbtObjClassRequestItem makeMaterialRequestItem( CswNbtPropertySetMaterial Material, CswNbtObjClass.NbtButtonData ButtonData )
        {
            checkForCentralInventoryGroups( _CswNbtResources );
            CswNbtObjClassRequestItem RequestItem = null;
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType RequestItemNT = RequestItemOC.getNodeTypes().FirstOrDefault();
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( null != RequestItemNT )
            {
                RequestItem = PropsAction.getAddNodeAndPostChanges( RequestItemNT, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassRequestItem RequestItemNode = NewNode;
                    RequestItemNode.Material.RelatedNodeId = Material.NodeId;
                    if( null != _ThisUser.DefaultLocationId )
                    {
                        CswNbtObjClassLocation DefaultAsLocation = _CswNbtResources.Nodes.GetNode( _ThisUser.DefaultLocationId );
                        if( null != DefaultAsLocation )
                        {
                            RequestItemNode.Location.SelectedNodeId = DefaultAsLocation.NodeId;
                            RequestItemNode.Location.CachedNodeName = DefaultAsLocation.Location.CachedNodeName;
                            RequestItemNode.Location.CachedPath = DefaultAsLocation.Location.CachedPath;
                        }
                    }
                    switch( ButtonData.SelectedText )
                    {
                        case CswNbtPropertySetMaterial.CswEnumRequestOption.Size:
                            RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.MaterialSize;
                            break;
                        default: //Request by Bulk
                            RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.MaterialBulk;
                            break;
                    }
                } );
            }
            return RequestItem;
        }

        /// <summary>
        /// Instance a new EnterprisePart Request Item
        /// </summary>
        public CswNbtObjClassRequestItem makeEnterprisePartRequestItem( CswNbtObjClassEnterprisePart EnterprisePart, CswNbtObjClass.NbtButtonData ButtonData )
        {
            checkForCentralInventoryGroups( _CswNbtResources );
            CswNbtObjClassRequestItem RequestItem = null;
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType RequestItemNT = RequestItemOC.getNodeTypes().FirstOrDefault();
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            if( null != RequestItemNT )
            {
                RequestItem = PropsAction.getAddNodeAndPostChanges( RequestItemNT, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassRequestItem RequestItemNode = NewNode;
                    RequestItemNode.EnterprisePart.RelatedNodeId = EnterprisePart.NodeId;
                    RequestItemNode.Type.Value = CswNbtObjClassRequestItem.Types.EnterprisePart;
                    if( null != _ThisUser.DefaultLocationId )
                    {
                        CswNbtObjClassLocation DefaultAsLocation = _CswNbtResources.Nodes.GetNode( _ThisUser.DefaultLocationId );
                        if( null != DefaultAsLocation )
                        {
                            RequestItemNode.Location.SelectedNodeId = DefaultAsLocation.NodeId;
                            RequestItemNode.Location.CachedNodeName = DefaultAsLocation.Location.CachedNodeName;
                            RequestItemNode.Location.CachedPath = DefaultAsLocation.Location.CachedPath;
                        }
                    }
                } );
            }
            return RequestItem;
        }

        #endregion Sets

        #endregion Public methods and props

        #region Private helper functions

        public static void checkForCentralInventoryGroups( CswNbtResources _CswNbtResources )
        {
            CswNbtView InventoryGroupView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClassProp CentralOCP = InventoryGroupOC.getObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.Central );
            CswNbtViewRelationship IGVR = InventoryGroupView.AddViewRelationship( InventoryGroupOC, true );
            InventoryGroupView.AddViewPropertyAndFilter( IGVR, CentralOCP, FilterMode: CswEnumNbtFilterMode.Equals, Value: CswEnumTristate.True );
            ICswNbtTree IGTree = _CswNbtResources.Trees.getTreeFromView( InventoryGroupView, false, false, true );
            IGTree.goToRoot();
            if( IGTree.getChildNodeCount() == 0 )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Unable to make requests because there are no central Inventory Groups.", "Can't Request without at least one Central Inventory Group defined." );
            }
        }

        #endregion

    }


}
