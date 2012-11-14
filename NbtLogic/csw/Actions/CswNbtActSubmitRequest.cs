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
    public class CswNbtActSubmitRequest
    {
        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _RequestOc = null;

        #endregion Private, core methods

        #region Constructor

        private CswNbtActSystemViews _SystemViews;
        private bool _CreateDefaultRequestNode = true;

        public CswNbtActSubmitRequest( CswNbtResources CswNbtResources, bool CreateDefaultRequestNode, CswNbtActSystemViews.SystemViewName RequestViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            _CreateDefaultRequestNode = CreateDefaultRequestNode;
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
            if( RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory )
            {
                RequestViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _SystemViews = new CswNbtActSystemViews( _CswNbtResources, RequestViewName, null );
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestClass );

            if( null != RequestNodeId )
            {
                CswNbtNode RequestNode = _CswNbtResources.Nodes.GetNode( RequestNodeId );
                if( null != RequestNode )
                {
                    _CurrentRequestNode = RequestNode;
                }
            }

            if( RequestViewName == CswNbtActSystemViews.SystemViewName.CISProRequestCart )
            {
                _CurrentCartView = _SystemViews.SystemView;
                _CurrentCartView.SaveToCache( false );
                applyCurrentCartFilter();
            }
            else if( RequestViewName == CswNbtActSystemViews.SystemViewName.CISProRequestHistory )
            {
                _RequestHistoryView = _SystemViews.SystemView;
                _RequestHistoryView.SaveToCache( false );
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

        private CswNbtView _CurrentCartView;
        public CswNbtView CurrentCartView
        {
            get
            {
                if( null == _CurrentCartView )
                {
                    CswNbtActSystemViews SystemViews = new CswNbtActSystemViews( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart, null );
                    _CurrentCartView = SystemViews.SystemView;
                    _CurrentCartView.SaveToCache( false );
                    applyCurrentCartFilter();
                }
                return _CurrentCartView;
            }
        }
        private CswNbtView _RequestHistoryView;
        public CswNbtView RequestHistoryView
        {
            get
            {
                if( null == _RequestHistoryView )
                {
                    CswNbtActSystemViews SystemViews = new CswNbtActSystemViews( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestHistory, null );
                    _RequestHistoryView = SystemViews.SystemView;
                    _RequestHistoryView.SaveToCache( false );
                }
                return _RequestHistoryView;
            }
        }

        public Int32 CartContentCount = 0;
        public Int32 CartCount = 0;

        private CswNbtNode _CurrentRequestNode;
        public CswNbtNode CurrentRequestNode()
        {
            if( null == _CurrentRequestNode )
            {
                CswNbtView RequestView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate );
                CswNbtViewRelationship RequestVr = RequestView.AddViewRelationship( _RequestOc, true ); //default filter says Requestor == me
                RequestView.AddViewPropertyAndFilter( RequestVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );
                RequestView.AddViewPropertyAndFilter( RequestVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null, ShowInGrid: false );

                ICswNbtTree RequestTree = _CswNbtResources.Trees.getTreeFromView( RequestView, false, false, false );
                CartCount = RequestTree.getChildNodeCount();
                if( CartCount >= 1 )
                {
                    RequestTree.goToNthChild( 0 );
                    _CurrentRequestNode = RequestTree.getNodeForCurrentPosition();
                }
                else if( CartCount == 0 &&
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

        /// <summary>
        /// Fetch the current Request node for the current user and establish base counts.
        /// </summary>
        public void applyCurrentCartFilter( CswNbtNode CartNode = null )
        {
            CartNode = CartNode ?? CurrentRequestNode();
            if( null != CartNode )
            {
                _CurrentCartView.Root.ChildRelationships[0].NodeIdsToFilterIn.Clear();
                _CurrentCartView.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( CartNode.NodeId );
                ICswNbtTree CartTree = _CswNbtResources.Trees.getTreeFromView( _CurrentCartView, false, false, false );
                CartTree.goToNthChild( 0 );
                CartContentCount = CartTree.getChildNodeCount();
            }
        }

        public JObject getRequestHistory()
        {
            JObject Ret = new JObject();

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestHistoryView, true, false, false );
            Int32 RequestCount = Tree.getChildNodeCount();
            Ret["count"] = RequestCount;
            if( RequestCount > 0 )
            {
                for( Int32 I = 0; I < RequestCount; I += 1 )
                {
                    Tree.goToNthChild( I );

                    Ret[Tree.getNodeNameForCurrentPosition()] = new JObject();
                    Ret[Tree.getNodeNameForCurrentPosition()]["requestnodeid"] = Tree.getNodeIdForCurrentPosition().ToString();
                    foreach( JObject Prop in Tree.getChildNodePropsOfNode() )
                    {
                        string PropName = Prop["propname"].ToString().ToLower();
                        Ret[Tree.getNodeNameForCurrentPosition()][PropName] = Prop["gestalt"].ToString();
                    }

                    Tree.goToParentNode();
                }
            }
            return Ret;
        }

        public JObject submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            JObject Ret = new JObject();
            if( null != NodeId )
            {
                CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( NodeId );
                if( null != NodeAsRequest )
                {
                    applyCurrentCartFilter( NodeAsRequest.Node );
                    if( CartContentCount > 0 )
                    {
                        NodeAsRequest.SubmittedDate.DateTimeValue = DateTime.Now;
                        NodeAsRequest.Name.Text = NodeName;
                        NodeAsRequest.postChanges( true );
                        Ret["succeeded"] = true;
                    }
                }
            }

            return Ret;
        }

        /// <summary>
        /// Takes the request item nodes from one request and appends them to the current request. 
        /// Returns a new instance of CswNbtActSubmitRequest with the current request context.
        /// </summary>
        public CswNbtActSubmitRequest copyRequest( CswPrimaryKey CopyFromNodeId, CswPrimaryKey CopyToNodeId )
        {
            if( null != CopyFromNodeId && null != CopyToNodeId )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CurrentCartView, false, false, false );
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
            return new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true, RequestNodeId: CopyToNodeId );
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
