using System;
using System.Diagnostics;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

        private bool _CreateDefaultRequestNode = true;

        public CswNbtActRequesting( CswNbtResources CswNbtResources, bool CreateDefaultRequestNode, SystemViewName RequestViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            _CreateDefaultRequestNode = CreateDefaultRequestNode;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }

            _RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );

            if( null != RequestNodeId )
            {
                CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( RequestNodeId );
                if( null != RequestNode )
                {
                    _CurrentRequestNode = RequestNode;
                }
            }
        }

        #endregion Constructor

        #region Public methods and props

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

        public CswNbtView RequestViewBase
        {
            get
            {
                CswNbtView Ret = new CswNbtView( _CswNbtResources )
                {
                    Category = "Request Configuration",
                    Visibility = NbtViewVisibility.Hidden,
                    ViewMode = NbtViewRenderingMode.Grid
                };

                CswNbtViewRelationship RootVr = Ret.AddViewRelationship( _RequestOc, IncludeDefaultFilters: true );

                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

                Ret.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
                Ret.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );

                return Ret;
            }
        }

        public CswNbtView OpenCartsView
        {
            get
            {
                CswNbtView Ret = RequestViewBase;
                CswNbtViewRelationship RootVr = Ret.Root.ChildRelationships[0];
                RootVr.ChildRelationships.Clear();

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

                    CswNbtViewProperty Vp1 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                    Vp1.Order = 1;
                    CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                    Vp2.Order = 2;
                    CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                    Vp3.Order = 3;
                    CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Location ) );
                    Vp4.Order = 4;
                    CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ) );
                    Vp5.Order = 5;
                    CswNbtViewProperty Vp6 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.RequestedFor ) );
                    Vp6.Order = 6;
                }

                Ret.SaveToCache( IncludeInQuickLaunch: false );
                return Ret;
            }
        }

        private ICswNbtTree _OpenCartTree = null;
        public ICswNbtTree OpenCartTree
        {
            get
            {
                if( null == _OpenCartTree )
                {
                    _OpenCartTree = _CswNbtResources.Trees.getTreeFromView( OpenCartsView, false, false, false );
                }
                return _OpenCartTree;
            }
        }

        public Int32 CartContentCount
        {
            get
            {
                Int32 Ret = OpenCartTree.getChildNodeCount();
                if( Ret > 0 )
                {
                    OpenCartTree.goToNthChild( 0 );
                    Ret = OpenCartTree.getChildNodeCount();
                }
                return Ret;
            }
        }

        public Int32 CartCount
        {
            get
            {
                return OpenCartTree.getChildNodeCount();
            }
        }

        private CswNbtObjClassRequest _getFirstPendingRequest()
        {
            CswNbtNode Ret = null;
            CswNbtView CartsView = RequestViewBase;
            CswNbtViewRelationship RootVr = CartsView.Root.ChildRelationships[0];
            RootVr.ChildRelationships.Clear();
            RootVr.Properties.Clear();

            CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
            CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );

            CartsView.AddViewPropertyAndFilter( RootVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
            CartsView.AddViewPropertyAndFilter( RootVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( OpenCartsView, false, false, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                Tree.goToNthChild( 0 );
                Ret = Tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private CswNbtObjClassRequest _CurrentRequestNode;
        public CswNbtObjClassRequest CurrentRequestNode()
        {
            if( null == _CurrentRequestNode )
            {
                _CurrentRequestNode = _getFirstPendingRequest();
                if( null == _CurrentRequestNode &&
                         _CreateDefaultRequestNode )
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

        public CswPrimaryKey CurrentRequestNodeId()
        {
            CswPrimaryKey Ret = null;
            if( null != CurrentRequestNode() )
            {
                Ret = CurrentRequestNode().NodeId;
            }
            return Ret;
        }

        public bool submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            bool Ret = false;
            if( null != NodeId )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( NodeId );
                if( null != NodeAsRequest )
                {
                    if( CartContentCount > 0 )
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
        /// Takes the request item nodes from one request and appends them to the current request. 
        /// Returns a new instance of CswNbtActSubmitRequest with the current request context.
        /// </summary>
        public CswNbtActRequesting copyRequest( CswPrimaryKey CopyFromNodeId, CswPrimaryKey CopyToNodeId )
        {
            if( null != CopyFromNodeId && null != CopyToNodeId )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( OpenCartsView, false, false, false );
                if( Tree.getChildNodeCount() == 1 ) //the first Item will be the Request
                {
                    Tree.goToNthChild( 0 );

                    Int32 ItemCount = Tree.getChildNodeCount();
                    for( Int32 I = 0; I < ItemCount; I += 1 )
                    {
                        Tree.goToNthChild( I );

                        CswNbtPropertySetRequestItem CopyFromNodeAsRequestItem = Tree.getNodeForCurrentPosition();
                        if( null != CopyFromNodeAsRequestItem )
                        {
                            CswNbtPropertySetRequestItem CopyToNodeAsRequestItem = CopyFromNodeAsRequestItem.copyNode();
                            if( null != CopyToNodeAsRequestItem )
                            {
                                CopyToNodeAsRequestItem.Request.RelatedNodeId = CopyToNodeId;
                                CopyToNodeAsRequestItem.postChanges( true );
                            }
                        }
                        Tree.goToParentNode();
                    }
                }
            }
            return new CswNbtActRequesting( _CswNbtResources, CreateDefaultRequestNode: true, RequestNodeId: CopyToNodeId );
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
                if( null != CurrentRequestNodeId() && null != Container )
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
                if( null != CurrentRequestNodeId() )
                {
                    CswNbtObjClassRequestMaterialDispense RetAsMatDisp = CswNbtObjClassRequestMaterialDispense.fromPropertySet( RetAsRequestItem );
                    RetAsMatDisp.Request.RelatedNodeId = CurrentRequestNodeId();

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

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public JObject getRequestItemAddProps( CswNbtPropertySetRequestItem RetAsRequestItem )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            _CswNbtResources.EditMode = NodeEditMode.Add;

            return PropsAction.getProps( RetAsRequestItem.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
        }

        public CswNbtView getSubmittedRequests()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = NbtViewVisibility.Hidden,
                ViewMode = NbtViewRenderingMode.Grid
            };

            Ret.Root.ChildRelationships.Clear();

            CswNbtMetaDataObjectClass RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtViewRelationship RootVr = Ret.AddViewRelationship( RequestOc, true );

            foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtResources.MetaData.getObjectClass( Member );
                CswNbtMetaDataObjectClassProp RequestOcp = MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request );
                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( RootVr,
                                                                                 NbtViewPropOwnerType.Second,
                                                                                 RequestOcp, false );

                CswNbtViewProperty Vp1 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ) );
                Vp1.Order = 1;
                CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                Vp2.Order = 2;
                CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                Vp3.Order = 3;
                CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ) );
                Vp4.Order = 4;
                CswNbtViewPropertyFilter StatusVpf = Ret.AddViewPropertyFilter( Vp4, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Value: CswNbtPropertySetRequestItem.Statuses.Pending );
                StatusVpf.ShowAtRuntime = true;
            }

            CswNbtViewProperty Vp5 = Ret.AddViewProperty( RootVr, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate ) );
            Vp5.Order = 5;
            CswNbtViewPropertyFilter DateVpf1 = Ret.AddViewPropertyFilter( Vp5, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals, Value: DateTime.Today.AddDays( -90 ).ToShortDateString() );
            DateVpf1.ShowAtRuntime = true;
            CswNbtViewPropertyFilter DateVpf2 = Ret.AddViewPropertyFilter( Vp5, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, Value: DateTime.Today.ToShortDateString() );
            DateVpf2.ShowAtRuntime = true;

            Ret.SaveToCache( IncludeInQuickLaunch: false );
            return Ret;
        }

        public CswNbtView getFavoriteRequests()
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources )
            {
                Category = "Request Configuration",
                Visibility = NbtViewVisibility.Hidden,
                ViewMode = NbtViewRenderingMode.Tree
            };

            Ret.Root.ChildRelationships.Clear();

            CswNbtMetaDataObjectClass RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtViewRelationship RootVr = Ret.AddViewRelationship( RequestOc, IncludeDefaultFilters: false );

            Ret.AddViewPropertyAndFilter( RootVr, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite ), Value: Tristate.True.ToString() );
            Ret.AddViewPropertyAndFilter( RootVr, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ), SubFieldName: CswNbtSubField.SubFieldName.NodeID, Value: _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString() );

            Ret.SaveToCache( IncludeInQuickLaunch: false );
            return Ret;
        }

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
