using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

        public CswNbtActSubmitRequest( CswNbtResources CswNbtResources, CswNbtActSystemViews.SystemViewName RequestViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
            if( RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory )
            {
                RequestViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _SystemViews = new CswNbtActSystemViews( _CswNbtResources, RequestViewName, null );
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            _RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );

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
                else if( CartCount == 0 )
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

        public void applyCartFilter( CswPrimaryKey NodeId )
        {
            CswNbtMetaDataObjectClassProp RequestOcp = _RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request.ToString() );
            _SystemViews.addSystemViewFilter( new CswNbtActSystemViews.SystemViewPropFilterDefinition
            {
                FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals,
                FilterValue = NodeId.PrimaryKey.ToString(),
                ObjectClassProp = RequestOcp,
                SubFieldName = CswNbtSubField.SubFieldName.NodeID
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
            return new CswNbtActSubmitRequest( _CswNbtResources, CswNbtActSystemViews.SystemViewName.CISProRequestCart, CopyToNodeId );
        }

        #endregion Public methods and props
    }


}
