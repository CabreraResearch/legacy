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
        private CswNbtMetaDataObjectClass _RequestItemOc = null;

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
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RequestClass );
            _RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.RequestItemClass );

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

        private CswNbtMetaDataNodeType _RequestItemNt = null;
        public CswNbtMetaDataNodeType RequestItemNt
        {
            get
            {
                if( null == _RequestItemNt )
                {
                    _RequestItemNt = _RequestItemOc.getNodeTypes().FirstOrDefault();
                }
                return _RequestItemNt;
            }
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
                CswNbtMetaDataObjectClassProp SubmittedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString() );
                CswNbtMetaDataObjectClassProp CompletedDateOcp = _RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate.ToString() );
                CswNbtViewRelationship RequestVr = RequestView.AddViewRelationship( _RequestOc, true ); //default filter says Requestor == me
                RequestView.AddViewPropertyAndFilter( RequestVr, SubmittedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null );
                RequestView.AddViewPropertyAndFilter( RequestVr, CompletedDateOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null );

                ICswNbtTree RequestTree = _CswNbtResources.Trees.getTreeFromView( RequestView, false, false );
                CartCount = RequestTree.getChildNodeCount();
                if( CartCount >= 1 )
                {
                    RequestTree.goToNthChild( 0 );
                    _CurrentRequestNode = RequestTree.getNodeForCurrentPosition();
                }
                //else if( CartCount > 1 )
                //{
                //    throw new CswDniException( ErrorType.Warning, "Only one pending request may be open at a time.", "There is more than one Pending request assigned to the current user." );
                //}
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

        public void applyCartFilter( CswPrimaryKey NodeId )
        {
            CswNbtMetaDataObjectClassProp RequestOcp = _RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request.ToString() );
            _SystemViews.addSystemViewFilter( new CswNbtActSystemViews.SystemViewPropFilterDefinition
            {
                FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals,
                FilterValue = NodeId.PrimaryKey.ToString(),
                ObjectClassProp = RequestOcp,
                SubFieldName = CswNbtSubField.SubFieldName.NodeID,
                ShowInGrid = false
            }, _RequestItemOc );
        }

        /// <summary>
        /// Fetch the current Request node for the current user and establish base counts.
        /// </summary>
        public void applyCurrentCartFilter()
        {
            CswNbtNode CartNode = CurrentRequestNode();
            if( null != CartNode )
            {
                applyCartFilter( CartNode.NodeId );
                ICswNbtTree CartTree = _CswNbtResources.Trees.getTreeFromView( _CurrentCartView, false, false );
                CartContentCount = CartTree.getChildNodeCount();
                if( null == _RequestItemNt )
                {
                    if( CartContentCount > 0 )
                    {
                        CartTree.goToNthChild( 0 );
                        _RequestItemNt = CartTree.getNodeForCurrentPosition().getNodeType();
                    }
                    else
                    {
                        _RequestItemNt = _RequestItemOc.getNodeTypes().FirstOrDefault();
                    }
                }
            }
        }

        public JObject getRequestHistory()
        {
            JObject Ret = new JObject();

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestHistoryView, true, false );
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
                    NodeAsRequest.SubmittedDate.DateTimeValue = DateTime.Now;
                    NodeAsRequest.Name.Text = NodeName;
                    NodeAsRequest.postChanges( true );
                    Ret["succeeded"] = true;
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
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CurrentCartView, false, false );
                Int32 ItemCount = Tree.getChildNodeCount();
                for( Int32 I = 0; I < ItemCount; I += 1 )
                {
                    Tree.goToNthChild( I );

                    CswNbtObjClassRequestItem CopyFromNodeAsRequestItem = Tree.getNodeForCurrentPosition();
                    if( null != CopyFromNodeAsRequestItem )
                    {
                        CswNbtObjClassRequestItem CopyToNodeAsRequestItem = CopyFromNodeAsRequestItem.copyNode();
                        if( null != CopyToNodeAsRequestItem )
                        {
                            CopyToNodeAsRequestItem.Request.RelatedNodeId = CopyToNodeId;
                            CopyToNodeAsRequestItem.postChanges( true );
                        }
                    }
                    Tree.goToParentNode();
                }
            }
            return new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true, RequestNodeId: CopyToNodeId );
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassRequestItem makeContainerRequestItem( CswNbtObjClassContainer Container, CswNbtObjClass.NbtButtonData ButtonData )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            CswNbtObjClassRequestItem RetAsRequestItem = PropsAction.getAddNode( RequestItemNt );
            if( null == RetAsRequestItem )
            {
                throw new CswDniException( ErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
            }
            if( null != CurrentRequestNodeId() && null != Container )
            {
                RetAsRequestItem.Container.RelatedNodeId = Container.NodeId;
                RetAsRequestItem.Container.setReadOnly( value: true, SaveToDb: true );

                RetAsRequestItem.Material.RelatedNodeId = Container.Material.RelatedNodeId;
                RetAsRequestItem.Material.setReadOnly( value: true, SaveToDb: ButtonData.SelectedText != CswNbtObjClassContainer.RequestMenu.Dispense );
                RetAsRequestItem.Material.setHidden( value: true, SaveToDb: ButtonData.SelectedText != CswNbtObjClassContainer.RequestMenu.Dispense );

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
                switch( ButtonData.SelectedText )
                {
                    case CswNbtObjClassContainer.RequestMenu.Dispense:
                        RetAsRequestItem.Type.Value = CswNbtObjClassRequestItem.Types.Dispense;

                        RetAsRequestItem.Quantity.UnitId = Container.Quantity.UnitId;
                        RetAsRequestItem.RequestBy.Value = CswNbtObjClassRequestItem.RequestsBy.Bulk;

                        RetAsRequestItem.Material.RelatedNodeId = Container.Material.RelatedNodeId;
                        CswNbtNode MaterialNode = _CswNbtResources.Nodes[Container.Material.RelatedNodeId];
                        Debug.Assert( null != MaterialNode, "RequestItem created without a valid Material." );
                        if( null != MaterialNode )
                        {
                            CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                            Vb.setQuantityUnitOfMeasureView( MaterialNode, RetAsRequestItem.Quantity );
                        }
                        break;
                    case CswNbtObjClassContainer.RequestMenu.Dispose:
                        RetAsRequestItem.IsTemp = false; /* This is the only condition in which we want to commit the node upfront. */
                        RetAsRequestItem.Type.Value = CswNbtObjClassRequestItem.Types.Dispose;

                        /* Kludge Alert: We don't have compound conditionals yet. Set it and hide it for now to squash the Quantity subprop. TODO: Remove this when compound conditionals arrive. */
                        RetAsRequestItem.RequestBy.Value = CswNbtObjClassRequestItem.RequestsBy.Size;
                        SelectedLocationId = Container.Location.SelectedNodeId;
                        break;
                    case CswNbtObjClassContainer.RequestMenu.Move:
                        RetAsRequestItem.Type.Value = CswNbtObjClassRequestItem.Types.Move;

                        /* Kludge Alert: We don't have compound conditionals yet. Set it and hide it for now to squash the Quantity subprop. TODO: Remove this when compound conditionals arrive. */
                        RetAsRequestItem.RequestBy.Value = CswNbtObjClassRequestItem.RequestsBy.Size;
                        break;
                    default:
                        throw new CswDniException( ErrorType.Error, "No action has been defined for this button menu.", "Menu option named " + ButtonData.SelectedText + " has not implemented a button click event." );
                }

                _setRequestItemSizesView( RetAsRequestItem.Size.View.ViewId, Container.Material.RelatedNodeId );

                RetAsRequestItem.Location.SelectedNodeId = SelectedLocationId;
                RetAsRequestItem.Location.RefreshNodeName();

                RetAsRequestItem.RequestBy.setHidden( value: ( ButtonData.SelectedText != CswNbtObjClassContainer.RequestMenu.Dispense ), SaveToDb: true );
                RetAsRequestItem.RequestBy.setReadOnly( value: ( ButtonData.SelectedText != CswNbtObjClassContainer.RequestMenu.Dispense ), SaveToDb: true );

                RetAsRequestItem.Type.setReadOnly( value: true, SaveToDb: true );

                RetAsRequestItem.postChanges( ForceUpdate: false );
            }
            return RetAsRequestItem;
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassRequestItem makeMaterialRequestItem( RequestItem Item, CswPrimaryKey NodeId, CswNbtObjClass.NbtButtonData ButtonData )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );

            CswNbtObjClassRequestItem RetAsRequestItem = PropsAction.getAddNode( RequestItemNt );
            if( null == RetAsRequestItem )
            {
                throw new CswDniException( ErrorType.Error, "Could not generate a new request item.", "Failed to create a new Request Item node." );
            }
            if( null != CurrentRequestNodeId() )
            {
                RetAsRequestItem.Request.RelatedNodeId = CurrentRequestNodeId();
                RetAsRequestItem.RequestBy.Value = ButtonData.SelectedText;
                if( null != _CswNbtResources.CurrentNbtUser.DefaultLocationId )
                {
                    CswNbtObjClassLocation DefaultAsLocation =
                        _CswNbtResources.Nodes.GetNode( _CswNbtResources.CurrentNbtUser.DefaultLocationId );
                    if( null != DefaultAsLocation )
                    {
                        RetAsRequestItem.Location.SelectedNodeId = _CswNbtResources.CurrentNbtUser.DefaultLocationId;
                        RetAsRequestItem.Location.CachedNodeName = DefaultAsLocation.Location.CachedNodeName;
                        RetAsRequestItem.Location.CachedPath = DefaultAsLocation.Location.CachedPath;
                    }
                }

                switch( Item.Value )
                {
                    case RequestItem.Material:
                        RetAsRequestItem.Material.RelatedNodeId = NodeId;
                        CswNbtNode MaterialNode = _CswNbtResources.Nodes[NodeId];
                        Debug.Assert( null != MaterialNode, "RequestItem created without a valid Material." );
                        if( null != MaterialNode )
                        {
                            CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                            Vb.setQuantityUnitOfMeasureView( MaterialNode, RetAsRequestItem.Quantity );
                        }
                        RetAsRequestItem.Container.setHidden( value: true, SaveToDb: true );
                        RetAsRequestItem.Container.setReadOnly( value: true, SaveToDb: true );
                        RetAsRequestItem.Type.Value = CswNbtObjClassRequestItem.Types.Request;
                        _setRequestItemSizesView( RetAsRequestItem.Size.View.ViewId, RetAsRequestItem.Material.RelatedNodeId );
                        break;
                }
            }
            return RetAsRequestItem;
        }

        /// <summary>
        /// Instance a new request item according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public JObject getRequestItemAddProps( CswNbtObjClassRequestItem RetAsRequestItem )
        {
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
            _CswNbtResources.EditMode = NodeEditMode.Add;

            return PropsAction.getProps( RetAsRequestItem.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
        }

        #endregion Public methods and props

        #region Private helper functions

        private void _setRequestItemSizesView( CswNbtViewId SizeViewId, CswPrimaryKey SizeMaterialId )
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            CswNbtView SizeView = _CswNbtResources.ViewSelect.restoreView( SizeViewId );
            SizeView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship SizeVr = SizeView.AddViewRelationship( SizeOc, false );
            SizeView.AddViewPropertyAndFilter( SizeVr, SizeMaterialOcp, SizeMaterialId.PrimaryKey.ToString(), SubFieldName: CswNbtSubField.SubFieldName.NodeID );
            SizeView.save();
        }

        #endregion
    }


}
