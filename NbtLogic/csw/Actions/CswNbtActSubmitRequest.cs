using System;
using System.Linq;
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

        public CswNbtActSubmitRequest( CswNbtResources CswNbtResources, CswNbtActSystemViews.SystemViewName RequestViewName = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActSubmitRequest without the required module." );
            }
            if( null == RequestViewName || ( RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && RequestViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory ) )
            {
                RequestViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _SystemViews = new CswNbtActSystemViews( _CswNbtResources, RequestViewName, null );
            _RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            _RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );

            if( RequestViewName == CswNbtActSystemViews.SystemViewName.CISProRequestCart )
            {
                _CurrentCartView = _SystemViews.SystemView;
                _CurrentCartView.SaveToCache( false );
                reInitCart();
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
                    reInitCart();
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
        public CswNbtNode CurrentRequestNode
        {
            get
            {
                if( null == _CurrentRequestNode )
                {
                    reInitCart();
                }
                return _CurrentRequestNode;
            }
        }

        /// <summary>
        /// Fetch the current Request node for the current user and establish base counts.
        /// </summary>
        public void reInitCart()
        {
            CartContentCount = 0;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( CurrentCartView, true, false );
            CartCount = Tree.getChildNodeCount();
            if( CartCount == 1 )
            {
                Tree.goToNthChild( 0 );
                _CurrentRequestNode = Tree.getNodeForCurrentPosition();
                if( CurrentRequestNode.ObjClass.ObjectClass == _RequestOc )
                {
                    CartContentCount = Tree.getChildNodeCount();
                }
            }
            else if( CartCount > 1 )
            {
                throw new CswDniException( ErrorType.Warning, "Only one pending request may be open at a time.", "There is more than one Pending request assigned to the current user." );
            }
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
                reInitCart();
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

        #endregion Public methods and props
    }


}
