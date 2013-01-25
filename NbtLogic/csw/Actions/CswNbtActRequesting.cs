using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
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

        #endregion Private, core methods

        #region Constructor

        public CswNbtActRequesting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }

            _RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );
        }

        #endregion Constructor

        #region Public methods and props

        [DataContract]
        public class Cart
        {
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
            public CartCounts Counts;
        }

        [DataContract]
        public class CartCounts
        {
            [DataMember( IsRequired = false )]
            public Int32 PendingRequestItems = 0;
            [DataMember( IsRequired = false )]
            public Int32 SubmittedRequestItems = 0;
            [DataMember( IsRequired = false )]
            public Int32 RecurringRequestItems = 0;
            [DataMember( IsRequired = false )]
            public Int32 FavoriteRequestItems = 0;
        }

        public sealed class RequestItem
        {
            public readonly string Value;
            public RequestItem( string ItemName = Container )
            {
                switch( ItemName )
                {
                    case Material:
                        Value = Material;
                        break;
                    default:
                        Value = Container;
                        break;
                }
            }
            public const string Material = "Material";
            public const string Container = "Container";
        }

        #region Gets

        public ICswNbtTree getOpenCartTree( CswNbtView CartView )
        {
            return _CswNbtResources.Trees.getTreeFromView( CartView, false, false, false );
        }

        public int getCartContentCount()
        {
            CswNbtView CartView = getOpenCartsView();
            ICswNbtTree Tree = getOpenCartTree( CartView );
            Int32 Ret = Tree.getChildNodeCount();
            if( Ret > 0 )
            {
                Tree.goToNthChild( 0 );
                Ret = Tree.getChildNodeCount();
            }
            return Ret;
        }

        public int getCartCount()
        {
            CswNbtView CartView = getOpenCartsView();
            ICswNbtTree Tree = getOpenCartTree( CartView );
            return Tree.getChildNodeCount();
        }

        private CswNbtObjClassRequest _getFirstPendingRequest()
        {
            CswNbtNode Ret = null;
            CswNbtView CartsView = getOpenCartsView( IncludeItemProperties: false );
            CswNbtViewRelationship RootVr = CartsView.Root.ChildRelationships[0];

            CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
            CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

            CartsView.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
            CartsView.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );

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
            return Ret;
        }

        private CswNbtObjClassRequest _CurrentRequestNode;
        public CswNbtObjClassRequest getCurrentRequestNode()
        {
            if( null == _CurrentRequestNode )
            {
                _CurrentRequestNode = _getFirstPendingRequest();
                if( null == _CurrentRequestNode )
                {
                    CswNbtMetaDataNodeType RequestNt = _RequestOc.getLatestVersionNodeTypes().FirstOrDefault();
                    if( null == RequestNt )
                    {
                        throw new CswDniException( ErrorType.Warning,
                                                    "Cannot Submit Request without a valid Request object.",
                                                    "No Request NodeType could be found." );
                    }
                    _CurrentRequestNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RequestNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    _CurrentRequestNode.postChanges( true );
                }
            }
            return _CurrentRequestNode;
        }

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

        public CswNbtView getRequestViewBase( bool LimitToUnsubmitted = true, bool IncludeDefaultFilters = true )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );

            Ret.Category = "Request Configuration";
            Ret.Visibility = NbtViewVisibility.Property;
            Ret.ViewMode = NbtViewRenderingMode.Grid;

            CswNbtViewRelationship RootVr = Ret.AddViewRelationship( _RequestOc, IncludeDefaultFilters: IncludeDefaultFilters );

            if( LimitToUnsubmitted )
            {
                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

                Ret.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
                Ret.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
            }
            return Ret;
        }

        public CswNbtView getOpenCartsView( bool IncludeItemProperties = true )
        {
            CswNbtView Ret = getRequestViewBase( IncludeDefaultFilters: true, LimitToUnsubmitted: true );
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            if( null != _CurrentRequestNode )
            {
                RootVr.NodeIdsToFilterIn.Clear();
                RootVr.NodeIdsToFilterIn.Add( _CurrentRequestNode.NodeId );
            }

            foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( RootVr,
                                                                                 NbtViewPropOwnerType.Second,
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
            _CswNbtResources.EditMode = NodeEditMode.Add;

            return PropsAction.getProps( RetAsRequestItem.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
        }

        public const string SubmittedItemsViewName = "Submitted Request Items";
        public CswNbtView getSubmittedRequestItemsView()
        {
            CswNbtView Ret = getRequestViewBase( false );
            Ret.ViewName = SubmittedItemsViewName;
            Ret.GridGroupByCol = CswNbtPropertySetRequestItem.PropertyName.Name;

            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];
            CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
            CswNbtViewPropertyFilter SubmittedVpf = Ret.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode : CswNbtPropFilterSql.PropertyFilterMode.NotNull, ShowInGrid : false );
            SubmittedVpf.ShowAtRuntime = true;
            
            foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );

                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( RootVr, NbtViewPropOwnerType.Second, RequestOcp, IncludeDefaultFilters: true );

                CswNbtViewProperty NameVp = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Name ) );
                NameVp.ShowInGrid = true;
                NameVp.Order = 1;
                CswNbtViewPropertyFilter NameVpf = Ret.AddViewPropertyFilter( NameVp, ShowAtRuntime: true );

                CswNbtViewProperty Vp1 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                Vp1.Width = 7;
                Vp1.Order = 2;
                
                CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ) );
                Vp2.Order = 3;
                CswNbtViewPropertyFilter StatusVpf = Ret.AddViewPropertyFilter( Vp2, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: CswNbtPropertySetRequestItem.Statuses.Pending );
                StatusVpf.ShowAtRuntime = true;

                CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                Vp3.Width = 50;
                Vp3.Order = 4;

                CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                Vp4.Order = 5;
            }

            return Ret;
        }

        public CswNbtView getFavoriteRequestNamesView()
        {
            CswNbtView Ret = getRequestViewBase( IncludeDefaultFilters: false, LimitToUnsubmitted: false );
            Ret.ViewName = "Favorite Request Names";
            CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];

            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite ),
                Value: Tristate.True.ToString(),
                ShowInGrid: false );
            Ret.AddViewPropertyAndFilter( RootVr,
                _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ),
                SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                Value: _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString(),
                ShowInGrid: false );

            return Ret;
        }

        public const string FavoriteItemsViewName = "Favorite Request Items";
        public CswNbtView getFavoriteRequestsItemsView()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = NbtViewVisibility.Hidden,
                ViewMode = NbtViewRenderingMode.Grid,
                ViewName = FavoriteItemsViewName,
                GridGroupByCol = CswNbtPropertySetRequestItem.PropertyName.Name
            };

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );

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
            Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsFavorite ), Value: CswNbtNodePropLogical.toLogicalGestalt( Tristate.True ), ShowInGrid: false );

            return Ret;
        }

        public const string RecurringItemsViewName = "Recurring Request Items";
        public CswNbtView getRecurringRequestsItemsView()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = NbtViewVisibility.Hidden,
                ViewMode = NbtViewRenderingMode.Grid,
                ViewName = RecurringItemsViewName
            };

            //Unlike other Request Items, Recurring requests are not tied to a Request, so they don't have a Name.

            CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( MemberOc, IncludeDefaultFilters: false );

            CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
            Vp3.Width = 40;
            Vp3.Order = 1;
            CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency ) );
            Vp4.Order = 2;
            CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate ) );
            Vp5.Order = 3;
            Vp5.SortBy = true;
            Ret.AddViewPropertyFilter( Vp5, ShowAtRuntime: true );

            Ret.AddViewPropertyAndFilter( RequestItemRel,
                                          MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsRecurring ),
                                          Value: Tristate.True.ToString(),
                                          ShowInGrid: false );

            Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Requestor ), ShowInGrid: false, Value: "me" );
            return Ret;
        }

        #endregion Views

        private Int32 _getItemCount( CswNbtView View )
        {
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions: false, IncludeSystemNodes: false, IncludeHiddenNodes: false );
            if( View.Visibility == NbtViewVisibility.Property )
            {
                if( Tree.getChildNodeCount() > 0 )
                {
                Tree.goToNthChild(0);
            }
            }
            Int32 Ret = Tree.getChildNodeCount();
            return Ret;
        }

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

            CswNbtView RecurringItems = getRecurringRequestsItemsView();
            RecurringItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.RecurringItemsView = RecurringItems;

            CswNbtView FavoriteItems = getFavoriteRequestsItemsView();
            FavoriteItems.SaveToCache( IncludeInQuickLaunch: false );
            Cart.FavoriteItemsView = FavoriteItems;

            Cart.Counts = new CartCounts();
            if( CalculateCounts )
            {
                //This is expensive and we can do it in the next async request
            Cart.Counts.PendingRequestItems = getCartContentCount();
            Cart.Counts.SubmittedRequestItems = _getItemCount( SubmittedItems );
            Cart.Counts.RecurringRequestItems = _getItemCount( RecurringItems );
            Cart.Counts.FavoriteRequestItems = _getItemCount( FavoriteItems );
            }
            return Cart;
        }

        #endregion Gets

        #region Sets

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
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtPropertySetRequestItem makeContainerRequestItem( CswNbtObjClassContainer Container, CswNbtObjClass.NbtButtonData ButtonData, CswNbtMetaDataObjectClass ItemOc = null )
        {
            CswNbtPropertySetRequestItem RetAsRequestItem = null;
            if( null == ItemOc )
            {
                if( ButtonData.SelectedText == CswNbtObjClassContainer.RequestMenu.Dispense )
                {
                    ItemOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass );
                }
                else
                {
                    ItemOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass );
                }
            }

            if( null != ItemOc )
            {
                CswNbtMetaDataNodeType RequestItemNt = ItemOc.getNodeTypes().FirstOrDefault();
                CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
                RetAsRequestItem = PropsAction.getAddNode( RequestItemNt );
                if( null == RetAsRequestItem )
                {
                    throw new CswDniException( ErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
                }
                if( null != getCurrentRequestNodeId() && null != Container )
                {
                    CswPrimaryKey SelectedLocationId = new CswPrimaryKey();
                    if( CswTools.IsPrimaryKey( _CswNbtResources.CurrentNbtUser.DefaultLocationId ) )
                    {
                        SelectedLocationId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                    }
                    else
                    {
                        SelectedLocationId = Container.Location.SelectedNodeId;
                    }
                    ButtonData.Action = CswNbtObjClass.NbtButtonAction.request;
                    if( ButtonData.SelectedText == CswNbtObjClassContainer.RequestMenu.Dispense )
                    {
                        CswNbtObjClassRequestContainerDispense RetAsDispense = CswNbtObjClassRequestContainerDispense.fromPropertySet( RetAsRequestItem );

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
                            case CswNbtObjClassContainer.RequestMenu.Dispose:
                                RetAsUpdate.IsTemp = false; // This is the only condition in which we want to commit the node upfront.
                                RetAsUpdate.Type.Value = CswNbtObjClassRequestContainerUpdate.Types.Dispose;
                                RetAsUpdate.Location.SelectedNodeId = Container.Location.SelectedNodeId;
                                RetAsUpdate.Location.setReadOnly( value: true, SaveToDb: true );
                                break;
                            case CswNbtObjClassContainer.RequestMenu.Move:
                                RetAsUpdate.Location.SelectedNodeId = SelectedLocationId;
                                RetAsUpdate.Type.Value = CswNbtObjClassRequestContainerUpdate.Types.Move;
                                break;
                        }
                    }

                    RetAsRequestItem.Location.RefreshNodeName();
                    RetAsRequestItem.Type.setReadOnly( value: true, SaveToDb: true );

                    RetAsRequestItem.postChanges( ForceUpdate: false );
                }
            }
            return RetAsRequestItem;
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtPropertySetRequestItem makeMaterialRequestItem( RequestItem Item, CswPrimaryKey NodeId, CswNbtObjClass.NbtButtonData ButtonData, CswNbtMetaDataObjectClass ItemOc = null )
        {
            CswNbtPropertySetRequestItem RetAsRequestItem = null;
            if( null == ItemOc )
            {
                //TODO: This will need to be conditional when Material Create is added
                ItemOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            }

            if( null != ItemOc )
            {
                CswNbtMetaDataNodeType RequestItemNt = ItemOc.getNodeTypes().FirstOrDefault();
                CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );

                RetAsRequestItem = PropsAction.getAddNode( RequestItemNt );
                if( null == RetAsRequestItem )
                {
                    throw new CswDniException( ErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
                }
                if( null != getCurrentRequestNodeId() )
                {
                    CswNbtObjClassRequestMaterialDispense RetAsMatDisp = CswNbtObjClassRequestMaterialDispense.fromPropertySet( RetAsRequestItem );
                    RetAsMatDisp.Request.RelatedNodeId = getCurrentRequestNodeId();

                    if( null != _CswNbtResources.CurrentNbtUser.DefaultLocationId )
                    {
                        CswNbtObjClassLocation DefaultAsLocation = _CswNbtResources.Nodes.GetNode( _CswNbtResources.CurrentNbtUser.DefaultLocationId );
                        if( null != DefaultAsLocation )
                        {
                            RetAsMatDisp.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                            RetAsMatDisp.Location.CachedNodeName = DefaultAsLocation.Location.CachedNodeName;
                            RetAsMatDisp.Location.CachedPath = DefaultAsLocation.Location.CachedPath;
                        }
                    }

                    RetAsMatDisp.Material.RelatedNodeId = NodeId;
                    CswNbtNode MaterialNode = _CswNbtResources.Nodes[NodeId];
                    Debug.Assert( null != MaterialNode, "RequestItem created without a valid Material." );
                    if( null != MaterialNode )
                    {
                        CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                        Vb.setQuantityUnitOfMeasureView( MaterialNode, RetAsMatDisp.Quantity );
                    }

                    _setRequestItemSizesView( RetAsMatDisp.Size.View.ViewId, RetAsMatDisp.Material.RelatedNodeId );

                    switch( ButtonData.SelectedText )
                    {
                        case CswNbtObjClassMaterial.Requests.Bulk:
                            RetAsMatDisp.Type.Value = CswNbtObjClassRequestMaterialDispense.Types.Bulk;
                            break;

                        case CswNbtObjClassMaterial.Requests.Size:
                            RetAsMatDisp.Type.Value = CswNbtObjClassRequestMaterialDispense.Types.Size;
                            break;
                    }
                }
            }
            return RetAsRequestItem;
        }

        

        #endregion Sets



        #endregion Public methods and props

        #region Private helper functions

        private void _setRequestItemSizesView( CswNbtViewId SizeViewId, CswPrimaryKey SizeMaterialId )
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtView SizeView = _CswNbtResources.ViewSelect.restoreView( SizeViewId );
            SizeView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship SizeVr = SizeView.AddViewRelationship( SizeOc, false );
            SizeView.AddViewPropertyAndFilter( SizeVr, SizeMaterialOcp, SizeMaterialId.PrimaryKey.ToString(), SubFieldName: CswNbtSubField.SubFieldName.NodeID );
            SizeView.AddViewPropertyAndFilter( SizeVr, SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable ), "false", FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            SizeView.save();
        }

        #endregion
    }


}
